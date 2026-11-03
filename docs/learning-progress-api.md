# API tiến độ học bảng chữ cho FE

## Phạm vi và mục đích

Có 4 API để hiển thị tiến độ của từng LearningProfile và ghi nhận học từng chữ.
Dùng chung JWT ứng dụng, prefix `/api/v1`, ProblemDetails, CORS và rate limiter hiện có.
Không có AI, chấm điểm trí nhớ, lịch ôn tập hay thao tác upload audio trong module này.

`Completed` nghĩa là người học đánh dấu đã học qua. Xem bảng chữ hoặc phát audio không tự
đánh dấu hoàn thành. Không có row progress nghĩa là `NotStarted`; GET không tạo dữ liệu.

## Endpoint

| Method | URL sau `/api/v1` | Dùng cho |
| --- | --- | --- |
| GET | `/learning-profiles/{profileId}/writing-system-progress` | Tổng quan tiến độ các bộ chữ của profile |
| GET | `/learning-profiles/{profileId}/writing-systems/{systemCode}/progress` | Trạng thái từng chữ, có filter/cursor |
| GET | `/learning-profiles/{profileId}/writing-systems/{systemCode}/items/{itemId}/progress` | Đọc lại một chữ, nhất là sau conflict |
| POST | `/learning-profiles/{profileId}/writing-systems/{systemCode}/items/{itemId}/progress` | Ghi thao tác Study, Complete hoặc Restart |

Mọi API cần:

```http
Authorization: Bearer <ACCESS_TOKEN_CUA_UNG_DUNG>
```

`profileId`, `itemId` là ID Snowflake dương. FE giữ ID dạng string, không ép sang JavaScript Number.
`itemId` lấy từ `items[].id` của API bảng chữ; không dùng item code hoặc reading ID thay item ID.
`systemCode` lấy từ catalog, ví dụ `hiragana`, `katakana`; handler chuẩn hóa code như các API nội dung hiện tại.
Không tự suy ra profile đang active ở server: URI chỉ rõ profile để thao tác không đổi đích khi người dùng đổi selection.

## 1. Tổng quan

```http
GET /api/v1/learning-profiles/1001/writing-system-progress
```

Ví dụ response 200, sau khi đã học một chữ và hoàn thành một chữ Hiragana:

```json
{
  "learningProfileId": "1001",
  "targetLanguage": "ja",
  "writingSystems": [
    {
      "writingSystemId": "1563703640068190218",
      "systemCode": "hiragana",
      "totalItems": 46,
      "notStartedItems": 44,
      "learningItems": 1,
      "completedItems": 1,
      "lastStudiedAt": "2026-10-27T00:03:00+00:00",
      "completionPercentage": 2.17
    },
    {
      "writingSystemId": "1563703640068190219",
      "systemCode": "katakana",
      "totalItems": 46,
      "notStartedItems": 46,
      "learningItems": 0,
      "completedItems": 0,
      "lastStudiedAt": null,
      "completionPercentage": 0
    }
  ]
}
```

ID trong ví dụ chỉ minh họa; dùng giá trị từ response thực tế.

- Chỉ tính nội dung của target language đang enabled và writing system đang published.
- `totalItems` lấy từ catalog; không lấy số row progress làm tổng số chữ.
- `notStartedItems = totalItems - learningItems - completedItems`.
- `completionPercentage = completedItems / totalItems * 100`, làm tròn 2 chữ số thập phân;
  bộ chữ rỗng trả 0.
- Bộ chữ chưa có progress vẫn xuất hiện với count đúng và lastStudiedAt null.
- Profile thuộc caller nhưng chưa có catalog enabled/published: trả 200 với writingSystems rỗng.
- Profile không tồn tại hoặc của người khác: trả cùng một 404.
- Tên/giải thích bản địa hóa tiếp tục lấy từ API catalog, ghép bằng writingSystemId/systemCode.

## 2. Danh sách trạng thái từng chữ

```http
GET /api/v1/learning-profiles/1001/writing-systems/hiragana/progress?pageSize=100
GET /api/v1/learning-profiles/1001/writing-systems/hiragana/progress?status=NotStarted&pageSize=20
```

Query:

| Field | Quy tắc |
| --- | --- |
| `status` | Tùy chọn: NotStarted, Learning, Completed; bỏ qua để lấy tất cả |
| `pageSize` | 1–100, mặc định 100 |
| `after` | Cursor do server trả về, tối đa 30 ký tự |

Ví dụ một chữ chưa học:

```json
{
  "items": [
    {
      "learningProfileId": "1001",
      "writingSystemItemId": "1563703640068190308",
      "itemCode": "kana-r0-c0",
      "sortOrder": 0,
      "status": "NotStarted",
      "version": 0,
      "lastStudiedAt": null,
      "completedAt": null,
      "createdAt": null,
      "updatedAt": null
    }
  ],
  "nextCursor": null
}
```

`NotStarted`/version 0 là giá trị của read model, không phải state/row được lưu trong DB.
Bản ghi đã tồn tại có version dương. Mỗi item hiện tại trả một tiến độ; audio và nội dung chữ
không bị lặp lại trong response này.

Phân trang theo `(sortOrder, itemId)`, không OFFSET và không COUNT riêng cho từng trang.
Khi nextCursor khác null, gửi nguyên cursor qua `encodeURIComponent`. Khi hết trang trả
items rỗng/nextCursor null. Filter không có kết quả vẫn trả 200 nếu profile và bộ chữ hợp lệ.
Khi đổi profile, systemCode hoặc status, xóa cursor cũ và tải trang đầu.

Phân trang là dữ liệu đang thay đổi, không phải snapshot cố định: thao tác học có thể làm chữ
rời khỏi filter NotStarted/Learning. FE nên tải lại danh sách filter sau mutation thay vì tiếp tục
coi cursor cũ là một snapshot bất biến.

## 3. Đọc lại một chữ

```http
GET /api/v1/learning-profiles/1001/writing-systems/hiragana/items/1563703640068190308/progress
```

Trả trực tiếp object có cấu trúc giống một phần tử `items` ở trên.
Chữ hợp lệ nhưng chưa có progress trả 200, NotStarted và version 0.
Profile/item/bộ chữ sai phạm vi, nội dung bị ẩn hoặc không thuộc caller trả 404.

## 4. Ghi thao tác học

```http
POST /api/v1/learning-profiles/1001/writing-systems/hiragana/items/1563703640068190308/progress
Content-Type: application/json
Authorization: Bearer <ACCESS_TOKEN_CUA_UNG_DUNG>

{
  "action": "Study",
  "version": 0
}
```

Hai field đều bắt buộc. Version là **version tiến độ của chữ**, không phải LearningProfile.Version.
Không gửi UserId, ProfileId trong body, timestamps, progress ID hoặc Status do FE tự gán;
field không khai báo trong contract bị từ chối.

| Action | Chưa có row, version 0 | Đã có row, version hiện tại |
| --- | --- | --- |
| `Study` | Tạo Learning, version 1 | Ghi LastStudiedAt; giữ nguyên Status/CompletedAt |
| `Complete` | Tạo rồi hoàn thành trong một request; version 2 | Chuyển sang Completed, đặt CompletedAt |
| `Restart` | 400: chưa có tiến độ để học lại | Chuyển Completed về Learning, xóa CompletedAt |

POST thành công trả **200** và DTO tiến độ mới. Timestamps do server cấp, chuẩn hóa UTC.
Ví dụ sau Study đầu tiên:

```json
{
  "learningProfileId": "1001",
  "writingSystemItemId": "1563703640068190308",
  "itemCode": "kana-r0-c0",
  "sortOrder": 0,
  "status": "Learning",
  "version": 1,
  "lastStudiedAt": "2026-10-27T00:00:00+00:00",
  "completedAt": null,
  "createdAt": "2026-10-27T00:00:00+00:00",
  "updatedAt": "2026-10-27T00:00:00+00:00"
}
```

Quy tắc:

- Version 0 chỉ hợp lệ khi chưa có row. Nếu thiết bị khác đã tạo thì trả 409.
- Version dương phải khớp row hiện tại; row không còn hoặc version cũ trả 409.
- Complete khi đã Completed là no-op, không đổi version/timestamp.
- Restart khi đã Learning là no-op.
- Study cùng server timestamp với LastStudiedAt là no-op; bình thường study mới tăng version.
- Study một chữ đã Completed vẫn giữ Completed và CompletedAt cũ.
- Complete sau Restart ghi mốc CompletedAt mới; CreatedAt ban đầu được giữ.
- No-op vẫn kiểm tra lại ownership/publication/version hiện tại trước khi trả thành công.
- Timestamps không đi lùi khi clock giữa các instance lệch nhẹ; handler dùng tối thiểu UpdatedAt hiện có.
- Version overflow trả 409 thay vì lưu một trạng thái dở.
- Progress không đổi version/preferences/current selection của LearningProfile.

Không gọi Study ở mỗi frame audio/timeupdate hoặc mọi lần component render.
Gọi khi người học chủ động bắt đầu/học tiếp chữ; gộp sự kiện trùng và tuần tự hóa mutation của cùng item.
Nút “Đã học” gọi Complete; nút “Học lại” gọi Restart. Không tự suy luận Completed từ lượt nghe audio.

## 5. FE ghép vào màn hình bảng chữ

1. Lấy profileId và targetLanguage từ module LearningProfile hiện có.
2. Load metadata bộ chữ theo locale qua catalog; load summary khi hiển thị màn hình chọn bộ chữ.
3. Khi mở chart, tải catalog items và progress page song song.
4. Ghép theo string ID; catalog cung cấp symbol/grid/primaryReading.audioUrl, progress cung cấp status/version.
5. Sau mutation, thay DTO progress của item bằng response rồi refresh summary. Nếu đang lọc status,
   tải lại trang đầu hoặc loại item đã rời filter khỏi UI.

Ví dụ TypeScript (dùng hàm authenticated request của FE):

```typescript
type ProgressStatus = "NotStarted" | "Learning" | "Completed";
type ProgressAction = "Study" | "Complete" | "Restart";
type ItemProgress = {
  learningProfileId: string;
  writingSystemItemId: string;
  itemCode: string;
  sortOrder: number;
  status: ProgressStatus;
  version: number;
  lastStudiedAt: string | null;
  completedAt: string | null;
  createdAt: string | null;
  updatedAt: string | null;
};

const progressRoot = `/api/v1/learning-profiles/${profileId}/writing-systems/${systemCode}`;
const [catalog, progressPage] = await Promise.all([
  apiGet(`/api/v1/languages/${targetLanguage}/writing-systems/${systemCode}/items?pageSize=100`),
  apiGet(`${progressRoot}/progress?pageSize=100`),
]);
const progressById = new Map<string, ItemProgress>(
  progressPage.items.map((item: ItemProgress) => [item.writingSystemItemId, item]),
);
// Ghép item.id với progressById.get(item.id); giữ toàn bộ ID dạng string.
```

Nếu bộ chữ vượt một trang, tải độc lập nextCursor của từng API. Không coi một progress item
không xuất hiện trong trang filter/page hiện tại là NotStarted; lấy trạng thái từ trang đầy đủ
hoặc GET riêng item khi cần. Bộ kana seed hiện tại mỗi chart có 46 chữ nên pageSize 100 lấy đủ.

Ghi thao tác, không tự cộng version tại client:

```typescript
async function recordProgress(itemId: string, action: ProgressAction, current: ItemProgress) {
  return apiPost(`${progressRoot}/items/${itemId}/progress`, {
    action,
    version: current.version,
  }); // lưu DTO và version từ response
}
```

Hàm apiGet/apiPost ở ví dụ là wrapper FE đang dùng, cần gắn Bearer JWT và Content-Type JSON
cho POST, xử lý ProblemDetails và auth refresh theo module auth hiện tại.

## 6. Xử lý đồng thời và lỗi

Khi nhận 409:

1. GET lại progress của đúng profile/system/item.
2. Cập nhật UI bằng state/version server.
3. Chỉ gửi thao tác mới nếu người dùng vẫn muốn thực hiện; không tự ghi đè bằng một version đoán được.

Hai request cùng version có thể có một request thành công, request còn lại 409. Hai lần tạo
cùng profile–item cũng vậy. Không có hai row trùng nhờ unique constraint và ON CONFLICT.

Nếu POST mất response do mạng, GET lại trạng thái trước khi quyết định gửi tiếp.
API không có receipt/idempotency key cho từng lượt Study; retry mù với version 0/cũ sẽ bị 409,
không tự tạo thêm dòng. Complete/Restart có tính no-op với version hiện tại nếu state đã đúng.

| HTTP | FE xử lý |
| --- | --- |
| 200 | Dùng response làm nguồn state/version mới |
| 400 | Sửa action/version/filter/cursor; Restart chưa bắt đầu cũng là 400 |
| 401 | Đi qua refresh/login hiện có |
| 404 | Profile không thuộc caller, tài nguyên không tồn tại, sai target language/system hoặc catalog đã ẩn |
| 409 | Reload item; phiên bản/policy thay đổi trong lúc thao tác |
| 413 | Body vượt giới hạn 4096 bytes |
| 429 | Chờ Retry-After; không retry liên tục |
| 500 | Thông báo chung, giữ traceId để hỗ trợ |

Business errors dùng ProblemDetails hiện có; lỗi model binding có thể có thêm `errors`.
Không phụ thuộc vào title tiếng Anh để phân nhánh nghiệp vụ.
Tất cả response tiến độ được đặt `Cache-Control: no-store` và không cache qua proxy.

FE cache key phải bao gồm user/profile/system/filter. Xóa state khi logout, đổi profile;
dùng AbortController hoặc request generation để response cũ không ghi đè màn hình profile mới.
Không dùng LearningProfile.Version hoặc version của catalog để đại diện cho progress.

## 7. Logic DB và hiệu năng

Read:

- Summary: một query aggregate SQL, LEFT JOIN catalog/progress để đếm cả chữ chưa có row.
- Item detail: một query projection, trả NotStarted khi thiếu row.
- Page thường: một query keyset, lấy pageSize+1, không COUNT/OFFSET/N+1.
- Page rỗng: thêm một EXISTS để phân biệt filter hết kết quả với parent không tồn tại/không thuộc caller.

Write:

1. Một query lấy profile thuộc caller, language enabled, system published, item đúng system
   và progress hiện tại; không tải từng ancestor bằng các request DB riêng.
2. Áp dụng method Domain với thời gian server.
3. Một câu ghi atomic: INSERT...SELECT...ON CONFLICT DO NOTHING khi version 0,
   hoặc UPDATE có điều kiện ID/profile/item/version và EXISTS kiểm tra lại ownership/catalog.
4. No-op dùng một EXISTS kiểm tra lại version/policy, không UPDATE vô nghĩa lên row.

Flow bình thường có hai lượt truy cập DB ở module tiến độ. Số này không tính SQL của
JWT/session validation chung. Query budget được đo bằng interceptor trong test runner.

Không mở transaction đọc–ghi riêng vì chỉ có một row thay đổi và câu ghi đã atomic,
kiểm tra lại policy/version tại DB. FK/unique bảo vệ integrity và race với xóa parent;
không có counter/table tổng hợp thứ hai phải commit cùng lúc. Thêm BEGIN/COMMIT ở đây
sẽ tăng roundtrip mà không thay thế được điều kiện version/unique.

Nếu policy/version thay đổi giữa context read và write, affected rows bằng 0 → 409.
Nếu parent bị xóa gây FK violation → 404. Những lỗi race này không được trả thành 500.
Không giữ khóa DB trong thời gian người dùng đọc/nghe audio.

Index dùng từ [model/DB](learning-progress-model.md): unique profile–item, index item,
PK/index catalog hiện có. Không có migration, cache Redis, index hoặc package mới cho API.

## 8. Bố cục code và vận hành

- `Api/Controllers/LearningProgressController`: controller mỏng, 4 endpoint.
- `Api/Contracts/LearningProgress`: request HTTP/validation, body từ chối field ngoài contract.
- `Application/LearningProgress`: DTO, query/command và handler riêng từng file.
- `Application/Common/Interfaces/IWritingSystemItemProgressRepository`: contract, không có EF/HTTP.
- `Infrastructure/Persistence/Repositories/WritingSystemItemProgressRepository`: projection/aggregate/CAS và xử lý lỗi DB.
- `Domain/LearningProgress`: state transition/invariant từ bước model, không đổi để phục vụ HTTP.

JWT và session validation dùng cơ chế hiện tại. Endpoint không nhận user ID từ FE,
không cho phép đọc/ghi progress profile của tài khoản khác. Rate limit dùng chung policy
`learning-profiles`, mặc định 120 request/IP/60 giây theo appsettings; trả Retry-After khi vượt.
Giới hạn body POST 4096 bytes. CORS và header no-store/referrer dùng cấu hình hiện có.

Nếu migration model chưa áp vào môi trường đích:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

API không tự migrate khi startup. Cấu hình auth/DB/R2/AI vẫn theo setup hệ thống hiện tại;
module tiến độ không thêm biến cấu hình bắt buộc mới.

## 9. Kiểm tra tự động

Console runner theo pattern test hiện có, không thêm NuGet package:

```bash
# Dùng một PostgreSQL trống, chuyên cho test, tên DB bắt buộc learning_progress_tests.
LEARNING_PROGRESS_TEST_DATABASE='Host=localhost;Port=5432;Database=learning_progress_tests;Username=postgres;Password=YOUR_TEST_PASSWORD' \
  dotnet run --project tests/LearningProgress.Tests
```

Runner migrate DB riêng, tạo fixture và HTTP server loopback port ngẫu nhiên. Từ chối DB khác tên
hoặc đã chứa user/progress, không truncate DB. Không đặt biến này trỏ vào DB ứng dụng.
Auth giả chỉ đăng ký trong project test; API production vẫn xác thực JWT/session bình thường.
Runner không gọi OAuth, AI hay R2 thật. Khi không có biến môi trường, in thông báo skip.

Kiểm tra gồm HTTP/JSON validation, authorization/isolation, virtual NotStarted không ghi row,
aggregate/filter/cursor, state transitions, no-op, query budgets, create/update races,
conditional update/no-op bằng snapshot cũ và thay đổi policy giữa read/write.

Kết quả xác minh khi triển khai: **227 kiểm tra đã qua** trên PostgreSQL 17 tạm,
bao gồm payload quá lớn trả 413, profile bị xóa/catalog bị ẩn giữa read/write và
câu ghi vẫn từ chối bằng điều kiện DB. Build solution thành công. Chưa áp migration
hoặc chạy thao tác tiến độ trên DB dự án trong phiên triển khai này.
