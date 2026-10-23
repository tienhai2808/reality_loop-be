# Learning Profile API cho FE

[README dự án](../README.md)

Base URL: `<backend-origin>/api/v1/learning-profiles`.
Tất cả endpoint yêu cầu `Authorization: Bearer <accessToken>` của ứng dụng.
Response dùng camelCase, enum là tên chuỗi, Snowflake ID là **string**.
Không gửi UserId: backend lấy owner từ JWT. Response không có wrapper `data`.

## Endpoint

| Method | Path tương đối | Request | Thành công |
| --- | --- | --- | --- |
| GET | `/options` | Không body | 200, `LearningProfileOptions` |
| GET | `?pageSize=20&after=...` | Không body | 200, `{ items, nextCursor }` |
| GET | `/{id}` | Không body | 200, `LearningProfile` |
| POST | `/` | Thông tin profile | 201, `LearningProfile`, có Location |
| PUT | `/{id}/preferences` | Toàn bộ preferences và version | 200, `LearningProfile` |
| DELETE | `/{id}?version=...` | Không body | 204, body rỗng |
| GET | `/current` | Không body | 200, `CurrentLearningProfile`; 204 nếu chưa chọn |
| PUT | `/current` | `{ "profileId": "..." }` | 200, `CurrentLearningProfile` |

Đường dẫn tạo/list thực tế là `/api/v1/learning-profiles`, không cần `/` ở cuối.
`options` cung cấp enum để FE không hardcode danh sách; ngôn ngữ là tag dạng string,
không có language enum hoặc API catalog ngôn ngữ ở module này.

## Luồng FE

1. Khi mở màn hình, tải options, trang đầu danh sách và current. Có thể gọi song song.
2. Tạo profile theo target language. Thành công thêm DTO trả về vào danh sách.
3. Nếu muốn chuyển sang profile vừa tạo, gọi riêng `PUT /current` với ID của profile.
   Tạo profile **không tự thay lựa chọn hiện tại**, kể cả profile đầu tiên.
4. Sửa preferences bằng version mới nhất; thay row/state bằng DTO trả về.
5. Đổi ngôn ngữ đang học bằng `PUT /current`; không sửa target language trên profile cũ.
6. Xóa profile đang chọn thì selection bị xóa theo FK cascade. Không tự chọn profile khác;
   tải lại current và hiển thị bước chọn/tạo profile khi nhận 204.

## Kiểu dữ liệu

```ts
export type ProficiencyLevel =
  | 'Beginner' | 'Elementary' | 'Intermediate' | 'UpperIntermediate' | 'Advanced';

export type LearningGoal =
  | 'Travel' | 'Work' | 'Study' | 'DailyLife' | 'Social' | 'Media' | 'Relocation' | 'Other';

export interface LearningProfile {
  id: string; // giữ nguyên string; không dùng Number()/parseInt()
  targetLanguage: string;
  nativeLanguage: string;
  explanationLanguage: string;
  proficiencyLevel: ProficiencyLevel; // tự đánh giá, không phải thang chứng chỉ
  primaryGoal: LearningGoal;
  goalNote: string | null;
  version: number;
  createdAt: string; // ISO 8601 UTC
  updatedAt: string; // ISO 8601 UTC
}

export interface LearningProfileOptions {
  proficiencyLevels: ProficiencyLevel[];
  learningGoals: LearningGoal[];
}

export interface LearningProfilePage {
  items: LearningProfile[];
  nextCursor: string | null;
}

export interface CurrentLearningProfile {
  profile: LearningProfile;
  selectionUpdatedAt: string; // ISO 8601 UTC, độc lập với profile.updatedAt
}
```

`GET /options` trả:

```json
{
  "proficiencyLevels": ["Beginner", "Elementary", "Intermediate", "UpperIntermediate", "Advanced"],
  "learningGoals": ["Travel", "Work", "Study", "DailyLife", "Social", "Media", "Relocation", "Other"]
}
```

## Tạo profile

```http
POST /api/v1/learning-profiles
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "targetLanguage": "JA",
  "nativeLanguage": "vi",
  "explanationLanguage": "vi",
  "proficiencyLevel": "Beginner",
  "primaryGoal": "Travel",
  "goalNote": "Muốn giao tiếp khi đi du lịch Nhật Bản"
}
```

201 trả DTO:

```json
{
  "id": "123456789012345678",
  "targetLanguage": "ja",
  "nativeLanguage": "vi",
  "explanationLanguage": "vi",
  "proficiencyLevel": "Beginner",
  "primaryGoal": "Travel",
  "goalNote": "Muốn giao tiếp khi đi du lịch Nhật Bản",
  "version": 1,
  "createdAt": "2026-10-20T11:00:00+00:00",
  "updatedAt": "2026-10-20T11:00:00+00:00"
}
```

- Ba language fields bắt buộc, tối đa 35 ký tự, trim/lowercase nhất quán.
- Dùng tag như `ja`, `vi`, `en`, `en-us`, `zh-hant`. Subtag phân cách bằng `-`, không dùng `_`.
  Backend kiểm tra cú pháp ASCII; không đối chiếu với catalog culture trên hệ điều hành.
- GoalNote tùy chọn/null, tối đa 500 ký tự; note được trim, trắng thành null.
- ProficiencyLevel và PrimaryGoal bắt buộc là giá trị enum hợp lệ.
- Mỗi user chỉ có một profile cho cùng target language đã chuẩn hóa.
- Tạo đồng thời cùng target language: unique constraint quyết định, một request thắng, request còn lại 409.
- API không dùng Idempotency-Key. Nếu mất response tạo, tải lại danh sách để kiểm tra profile
  đã tồn tại trước khi tạo lại. POST lặp lại cho cùng ngôn ngữ trả 409, không tạo bản sao.

## Cập nhật preferences

Đây là **PUT thay toàn bộ preferences**, không phải PATCH. Gửi đủ nativeLanguage,
explanationLanguage, proficiencyLevel, primaryGoal và version. GoalNote bỏ qua/null/trắng sẽ xóa note.

```http
PUT /api/v1/learning-profiles/123456789012345678/preferences
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "version": 1,
  "nativeLanguage": "vi",
  "explanationLanguage": "en-US",
  "proficiencyLevel": "Elementary",
  "primaryGoal": "DailyLife",
  "goalNote": null
}
```

200 trả profile với `explanationLanguage="en-us"`, version tăng và updatedAt mới.
Ngay cả gửi lại cùng preferences, version vẫn tăng theo hợp đồng UpdatePreferences.
Không tự cộng version ở FE; dùng nguyên version mới server trả về.

Không được sửa `targetLanguage`, `userId`, `id`, timestamp hoặc version tùy ý.
Các JSON body tạo/sửa/chọn từ chối field không thuộc contract bằng 400; targetLanguage
trong body cập nhật bị từ chối. Version chỉ là điều kiện cập nhật, không phải version cần ghi.

409 do version cũ: tải lại profile, cho user xem dữ liệu mới rồi xác nhận sửa lại.
Không tự lấy version mới để ghi đè bằng dữ liệu cũ. Nếu không còn tồn tại, dọn row/state tương ứng.

## Profile hiện tại

```http
PUT /api/v1/learning-profiles/current
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "profileId": "123456789012345678" }
```

PUT và GET `/current` trả cùng dạng:

```json
{
  "profile": {
    "id": "123456789012345678",
    "targetLanguage": "ja",
    "nativeLanguage": "vi",
    "explanationLanguage": "en-us",
    "proficiencyLevel": "Elementary",
    "primaryGoal": "DailyLife",
    "goalNote": null,
    "version": 2,
    "createdAt": "2026-10-20T11:00:00+00:00",
    "updatedAt": "2026-10-20T11:01:00+00:00"
  },
  "selectionUpdatedAt": "2026-10-20T11:02:00+00:00"
}
```

GET chưa có selection trả **204**, không có JSON body. Không gọi `response.json()` với 204.
Một user chỉ có tối đa một selection; không nhận UserId hoặc target language ở endpoint chọn.
Profile không thuộc user và profile không tồn tại đều trả 404.

Đổi selection không cập nhật Version/UpdatedAt của LearningProfile; selection có timestamp riêng.
Selection không có version: các lần đổi đồng thời được serialize tại DB, lần ghi sau cùng thắng.
Response PUT là snapshot đọc trước commit trong cùng transaction; request khác vẫn có thể đổi
selection sau khi transaction hoàn tất. FE nên disable nút khi đang đổi và reload current khi cần đồng bộ tab/thiết bị.

## Danh sách và xóa

`GET /api/v1/learning-profiles?pageSize=20` trả `{ items, nextCursor }`.
Sắp theo ID tăng dần, pageSize mặc định 20, cho phép 1–100. Trang tiếp theo dùng
`URLSearchParams({ pageSize: '20', after: nextCursor })`. Giữ nguyên cursor server trả về;
`nextCursor=null` là hết trang. Không có totalCount, COUNT hoặc OFFSET.

```http
DELETE /api/v1/learning-profiles/123456789012345678?version=2
Authorization: Bearer <accessToken>
```

Xóa thành công trả 204. Version bắt buộc; version cũ trả 409, không xóa dữ liệu mới.
Xóa lại profile đã mất trả 404. Xóa profile hiện tại đồng thời xóa selection bằng DB cascade;
việc này là một thao tác nguyên tử, không có khoảng selection trỏ tới profile không tồn tại.

## Lỗi

Lỗi dùng ProblemDetails: `title`, `status`, `detail`, `traceId`; validation có thể thêm
`errors: { field: string[] }`. Không phụ thuộc vào một chuỗi detail cố định để điều khiển UI.

| HTTP | Ý nghĩa và xử lý |
| --- | --- |
| 400 | Body/ID/cursor/pageSize/version/tag/enum không hợp lệ hoặc field ngoài contract |
| 401 | JWT ứng dụng không hợp lệ; dùng luồng refresh auth hiện có |
| 404 | Profile không tồn tại hoặc không thuộc user |
| 409 | Trùng target language hoặc version cũ; tải lại dữ liệu trước thao tác tiếp |
| 413 | Body vượt giới hạn 8 KiB |
| 429 | Rate limit; chờ Retry-After, không spam request |
| 500 | Lỗi chung, giữ traceId để báo lỗi |

## DB, concurrency và số query

- Application có command/query và handler riêng từng file; controller chỉ ánh xạ HTTP.
- Domain giữ validation, immutable target language và tăng version. Infrastructure triển khai
  repository, EF projections, conditional writes và transaction; API không xử lý trực tiếp lỗi EF/Npgsql.
- Unique `(user_id, target_language)` bắt duplicate ngay cả khi request đồng thời; không query kiểm tra trùng trước insert.
- Đọc danh sách/chi tiết/current dùng AsNoTracking và projection. Update WHERE có owner, ID và version;
  không có read-modify-write bỏ qua concurrency. DELETE có owner/ID/version.
- Chọn current dùng INSERT SELECT từ profile thuộc owner, FOR KEY SHARE trước khi ghi selection,
  rồi ON CONFLICT(user_id) DO UPDATE. Composite FK tiếp tục bảo vệ ownership ở DB.
- Upsert + đọc current nằm trong transaction READ COMMITTED ngắn; giữ row lock đến commit để response
  khớp selection vừa ghi. Timestamp selection dùng GREATEST để không lùi khi clock giữa các instance lệch.
- Tạo/cập nhật/xóa là một SQL write nguyên tử; không thêm transaction bao quanh bước đọc không cần thiết.

Số lệnh thành công **theo code**, chưa đo bằng DB thật; không gồm xác thực JWT và BEGIN/COMMIT:

| Thao tác | Lệnh DB |
| --- | ---: |
| Options | 0 |
| List/detail/current | 1 |
| Create | 1 INSERT |
| Update preferences | 2: đọc snapshot + conditional UPDATE |
| Delete | 1 DELETE; thêm 1 query nếu cần phân biệt 404/409 khi không xóa được |
| Set current | 2: upsert + đọc response trong transaction |

RateLimiting:LearningProfiles mặc định 120 request/IP/60 giây, chạy trước JWT/DB.
Đây là limiter trong process; nhiều instance cần giới hạn tổng tại gateway. CORS hiện có cho phép
GET/POST/PUT/DELETE, Authorization/Content-Type và expose Retry-After/Location. Responses đặt no-store.
Không log body preferences/GoalNote vào telemetry khi tích hợp FE/APM.

## Migration và trạng thái xác minh

Áp dụng migration model trước khi gọi API:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Migration: `20261020113217_AddLearningProfiles`. API không tự chạy migration lúc startup.
Không cần migration bổ sung cho phần API này. Build đã thành công; chưa chạy integration test
PostgreSQL hoặc HTTP end-to-end cho LearningProfile trong môi trường hiện tại.
