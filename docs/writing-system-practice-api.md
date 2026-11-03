# API luyện tập bảng chữ và tích hợp frontend

## 1. Phạm vi và kiến trúc

Luồng **nhìn chữ → chọn cách đọc** sử dụng catalog bảng chữ/cách đọc hiện có, không gọi AI.
Backend tạo và lưu đề, chấm từng câu, tự kết thúc khi trả lời đủ và cung cấp lịch sử để tiếp tục
hoặc xem kết quả. Không thay đổi learning progress hoặc `LearningProfile.Version`.

Giữ bốn layer theo convention dự án:

- `Domain/WritingSystemPractice`: invariant, snapshot đề, chấm một lần và chuyển trạng thái.
- `Application/WritingSystemPractice`: command/query và handler inject trực tiếp, DTO, validation,
  chọn câu/đáp án nhiễu. Không dùng MediatR hoặc truy cập EF từ Application.
- `Application/Common/Interfaces/IWritingSystemPracticeRepository`: port truy cập dữ liệu.
- `Infrastructure/Persistence/Repositories/WritingSystemPracticeRepository`: PostgreSQL/EF, ownership,
  transaction và conditional updates.
- `Api/Contracts/WritingSystemPractice`, `Api/Controllers/WritingSystemPracticeController`: HTTP contracts.

Không thêm package hoặc migration cho bước API. Cần áp migration model
`20261103121729_AddWritingSystemPracticeSessions` nếu môi trường chưa có hai bảng tương ứng.
Xem [model và ranh giới constraint](writing-system-practice-model.md).

## 2. Các API phục vụ việc gì?

Prefix chung: `/api/v1/learning-profiles/{profileId}`.
Mọi endpoint yêu cầu `Authorization: Bearer <accessToken>`, trả JSON camelCase,
enum theo tên và dữ liệu cá nhân có `Cache-Control: no-store`.

| Method | Đường dẫn sau prefix | Dùng cho màn hình/thao tác |
| --- | --- | --- |
| POST | `/writing-systems/{systemCode}/practice-sessions` | Bắt đầu lượt; nhận luôn đề để hiển thị |
| GET | `/practice-sessions` | Lịch sử, lượt đang luyện để tiếp tục, lọc theo bộ chữ/trạng thái |
| GET | `/practice-sessions/{sessionId}` | Reload, tiếp tục lượt, xem kết quả, đối chiếu sau conflict/mất response |
| POST | `/practice-sessions/{sessionId}/items/{sessionItemId}/answer` | Nộp một lựa chọn, nhận feedback và snapshot mới của lượt |
| POST | `/practice-sessions/{sessionId}/abandon` | Chủ động kết thúc lượt đang luyện khi chưa trả lời đủ |

`profileId`, `sessionId`, `sessionItemId` là số nguyên 64-bit dương trên route.
**Mọi ID trong JSON là string** để tránh mất chính xác Snowflake trong JavaScript.
Giữ ID ở dạng string từ đầu đến cuối, không dùng `Number(id)`/`parseInt(id)`.
`version`, vị trí và chỉ số lựa chọn là số JSON.

## 3. Tạo lượt luyện

```http
POST /api/v1/learning-profiles/{profileId}/writing-systems/hiragana/practice-sessions
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "questionCount": 10 }
```

- `questionCount` tùy chọn; `{}` dùng mặc định 10; giới hạn 1–20.
- Body là JSON object bắt buộc; không gửi `UserId`, đề, đáp án hoặc thời gian từ FE.
- Backend xác nhận profile thuộc caller, đúng target language, language enabled và system published.
- Đọc toàn bộ item/readings của bộ chữ qua một query join; không lấy từng chữ/cách đọc bằng N+1 queries.
- Chọn hệ phiên âm có nhiều chữ đủ điều kiện nhất; nếu hòa, chọn theo code ordinal.
  Một lượt chỉ dùng một hệ phiên âm, FE không cần chọn hệ này.
- Một chữ đủ điều kiện khi có cách đọc trong hệ đó và có ít nhất một đáp án nhiễu không phải
  bất kỳ cách đọc hợp lệ nào của chính chữ đó trong cùng hệ.
- Chọn ngẫu nhiên các chữ không lặp. Với mỗi chữ, dùng cách đọc có SortOrder rồi ID nhỏ nhất;
  lấy tối đa ba cách đọc khác từ cùng bộ chữ/hệ phiên âm làm nhiễu và xáo trộn lựa chọn.
- Mỗi câu có 2–4 lựa chọn tùy dữ liệu. Domain trim/NFC, kiểm tra distinct không phân biệt hoa thường
  và bảo đảm chính xác một lựa chọn là cách đọc hợp lệ.
- Không đủ câu: **400**, detail cho biết số câu tối đa đủ điều kiện; không tạo lượt thiếu câu
  hoặc âm thầm giảm số câu. Bộ chữ hợp lệ nhưng chưa có dữ liệu luyện cũng trả 400.

Thành công: **201 Created**, `Location` trỏ tới API GET chi tiết có đủ `/api/v1`.
Response cùng định dạng GET chi tiết ở mục 4; Version ban đầu bằng 1.

Việc tạo không dùng Idempotency-Key và mỗi POST thành công tạo một lượt mới.
FE khóa nút bắt đầu trong lúc gửi, không tự retry POST khi timeout/mất response.
Lúc đó tải lịch sử để người dùng chọn tiếp tục lượt đã được lưu hoặc bắt đầu lượt mới.
Cho phép nhiều lượt InProgress của cùng profile; không có luật tự bỏ các lượt khác.

## 4. Định dạng chi tiết và tải lại lượt

```http
GET /api/v1/learning-profiles/{profileId}/practice-sessions/{sessionId}
Authorization: Bearer <accessToken>
```

Ví dụ lượt vừa tạo với một câu:

```json
{
  "session": {
    "id": "SESSION_ID",
    "learningProfileId": "PROFILE_ID",
    "writingSystemId": "WRITING_SYSTEM_ID",
    "questionCount": 1,
    "status": "InProgress",
    "answeredCount": 0,
    "correctCount": 0,
    "version": 1,
    "createdAt": "2026-11-03T12:00:00+00:00",
    "updatedAt": "2026-11-03T12:00:00+00:00",
    "endedAt": null,
    "incorrectCount": 0,
    "unansweredCount": 1,
    "accuracyPercentage": null
  },
  "items": [
    {
      "id": "SESSION_ITEM_ID",
      "writingSystemItemId": "CATALOG_ITEM_ID",
      "position": 0,
      "symbol": "あ",
      "transcriptionSystem": "hepburn",
      "options": ["i", "a", "u", "e"],
      "selectedOptionIndex": null,
      "correctOptionIndex": null,
      "isCorrect": null,
      "answeredAt": null
    }
  ]
}
```

- `items` sắp theo Position tăng dần, bắt đầu 0; thứ tự lựa chọn là bản chụp đã lưu.
- `items[].id` là ID **câu trong lượt**, khác với `writingSystemItemId` của chữ nguồn.
- Câu chưa nộp luôn có CorrectOptionIndex/IsCorrect/SelectedOptionIndex/AnsweredAt null.
  Không mở đáp án của câu chưa nộp chỉ vì lượt đã Abandoned.
- Câu đã nộp trả lựa chọn, đáp án chuẩn, đúng/sai và thời điểm ghi nhận.
- `answeredCount` là số câu đã nộp; `incorrectCount = answeredCount - correctCount`;
  `unansweredCount = questionCount - answeredCount`.
- `accuracyPercentage` là phần trăm đúng **trên số câu đã nộp**, làm tròn hai chữ số;
  null khi chưa nộp câu nào. Câu bỏ qua không bị tính thành câu sai.
- Không dùng DTO/entity Domain trực tiếp làm response: mapper che đáp án từng câu chưa nộp.

GET không sinh lại đề, không tự tạo dữ liệu và không thay Version.
Nguồn catalog đổi symbol/reading không đổi đề cũ. Catalog bị unpublish hoặc language disabled
chặn tạo lượt mới, nhưng **lượt đã lưu vẫn xem, trả lời và bỏ lượt được** theo snapshot.
Mọi thao tác vẫn yêu cầu profile thuộc caller; sai owner/profile/session trả cùng 404.

## 5. Nộp đáp án

```http
POST /api/v1/learning-profiles/{profileId}/practice-sessions/{sessionId}/items/{sessionItemId}/answer
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "selectedOptionIndex": 1, "version": 1 }
```

Cả hai field bắt buộc; field ngoài contract bị từ chối 400.

- `selectedOptionIndex`: chỉ số **zero-based** trong `options` đã nhận; phải nằm trong số lựa chọn thực tế của câu.
  Không gửi text đáp án, IsCorrect, CorrectOptionIndex hoặc timestamp.
- `version`: `session.version` mới nhất, **không phải** LearningProfile.Version hoặc version của item.
- Backend tải aggregate có đủ câu (tối đa 20) cùng điều kiện ownership trong một query.
- Chấm bằng đề snapshot. Ghi câu trả lời và cập nhật Version/UpdatedAt của lượt atomically.
- Nộp câu cuối tự chuyển Status Completed, đặt EndedAt; không cần API finish riêng.
- Thành công **200**, trả toàn bộ detail mới. FE thay state bằng response và dùng version mới cho thao tác sau.

Ví dụ câu trên đã trả lời đúng:

```json
{
  "id": "SESSION_ITEM_ID",
  "writingSystemItemId": "CATALOG_ITEM_ID",
  "position": 0,
  "symbol": "あ",
  "transcriptionSystem": "hepburn",
  "options": ["i", "a", "u", "e"],
  "selectedOptionIndex": 1,
  "correctOptionIndex": 1,
  "isCorrect": true,
  "answeredAt": "2026-11-03T12:00:01+00:00"
}
```

Đây là ví dụ phần tử `items`; response thực tế vẫn là `{ session, items }`.

Retry cùng câu/cùng lựa chọn với **version hiện tại** là 200 no-op, không đổi AnsweredAt/Version.
Cùng câu/khác lựa chọn trả 409. Version cũ luôn 409, kể cả retry cùng đáp án đã lưu.
Hai request cùng version chỉ có một request thay state thành công; request còn lại phải tải lại lượt.

Khi timeout/mất response: GET lại detail trước. Nếu câu đã có lựa chọn, dùng kết quả server;
nếu còn chưa nộp, cho phép gửi lại bằng version vừa tải. Không tự ghi đè hoặc tăng version ở FE.

## 6. Bỏ lượt

```http
POST /api/v1/learning-profiles/{profileId}/practice-sessions/{sessionId}/abandon
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "version": 3 }
```

Version bắt buộc, dương. Thành công 200 trả detail:

- InProgress → Abandoned; tăng version một lần, đặt EndedAt/UpdatedAt, giữ câu trả lời đã có.
- Đã Abandoned + đúng version hiện tại → 200 no-op.
- Đã Completed hoặc version cũ → 409.
- Sau Abandoned không nhận câu trả lời mới. Retry câu đã nộp cùng lựa chọn/đúng version vẫn là no-op.

Đóng tab/chuyển màn hình không bắt buộc gọi abandon. Lượt InProgress lưu lại để tiếp tục.
Không có API xóa câu, sửa đáp án hoặc reopen lượt đã kết thúc; luyện lại tạo lượt mới.

## 7. Lịch sử và tiếp tục lượt

```http
GET /api/v1/learning-profiles/{profileId}/practice-sessions?status=InProgress&systemCode=hiragana&pageSize=20
```

Query tùy chọn:

| Field | Quy tắc |
| --- | --- |
| `status` | `InProgress`, `Completed`, `Abandoned` |
| `systemCode` | Chuẩn hóa trim/lowercase theo code catalog, tối đa 64 ký tự; lọc cả catalog đã ẩn |
| `pageSize` | 1–100, mặc định 20 |
| `before` | Dùng nguyên `nextCursor` từ response trước, không tự tính |

```json
{ "items": [], "nextCursor": null }
```

Mỗi phần tử items có cấu trúc `session` ở mục 4, không kèm đề/đáp án.
Sắp ID giảm dần, lấy pageSize+1 để xác định nextCursor, không OFFSET hoặc COUNT tổng lịch sử.
Trang tiếp theo gửi `before=nextCursor`, giữ nguyên filter. Trang/filter rỗng của profile hợp lệ
trả 200; profile không tồn tại/không thuộc caller trả 404.
Status có thể đổi trong lúc phân trang; refresh từ trang đầu khi cần danh sách trạng thái mới nhất.

FE hiển thị các lượt InProgress để người dùng chọn tiếp tục, sau đó GET detail tương ứng.
Không mặc định profile chỉ có một lượt InProgress.

## 8. Cách ghép FE

1. Từ LearningProfile hiện tại và catalog, người dùng mở một bộ chữ rồi bấm luyện.
2. POST tạo lượt; lưu `session.id` vào URL màn hình luyện. Dùng đề trả về ngay, không cần
   gọi thêm catalog, reading hoặc từng API question để render lượt này.
3. Hiển thị chữ, hệ phiên âm và options theo đúng thứ tự backend; không shuffle ở FE.
4. Chọn đáp án → POST answer với `items[].id` và `session.version`. Chỉ hiển thị feedback sau response.
5. Thay toàn bộ state bằng detail trả về. Backend hoàn thành lượt thì chuyển sang màn kết quả.
6. Reload/mở lại URL → GET detail, tìm câu có `selectedOptionIndex === null` để tiếp tục nếu InProgress.
7. Sau trả lời/bỏ lượt, invalidate lịch sử cùng profile; không tự sửa tiến độ học bảng chữ.

Kiểu dữ liệu TypeScript tối thiểu:

```ts
type PracticeStatus = 'InProgress' | 'Completed' | 'Abandoned';
type PracticeSummary = {
  id: string; learningProfileId: string; writingSystemId: string;
  questionCount: number; status: PracticeStatus;
  answeredCount: number; correctCount: number; incorrectCount: number;
  unansweredCount: number; accuracyPercentage: number | null;
  version: number; createdAt: string; updatedAt: string; endedAt: string | null;
};
type PracticeQuestion = {
  id: string; writingSystemItemId: string; position: number;
  symbol: string; transcriptionSystem: string; options: string[];
  selectedOptionIndex: number | null; correctOptionIndex: number | null;
  isCorrect: boolean | null; answeredAt: string | null;
};
type PracticeDetail = { session: PracticeSummary; items: PracticeQuestion[] };
type PracticePage = { items: PracticeSummary[]; nextCursor: string | null };

const profileBase = (profileId: string) => `/api/v1/learning-profiles/${profileId}`;

async function submitAnswer(
  detail: PracticeDetail, questionId: string, index: number,
  accessToken: string, signal?: AbortSignal
): Promise<PracticeDetail> {
  const base = `${profileBase(detail.session.learningProfileId)}/practice-sessions/${detail.session.id}`;
  const response = await fetch(`${base}/items/${questionId}/answer`, {
    method: 'POST', signal,
    headers: { Authorization: `Bearer ${accessToken}`, 'Content-Type': 'application/json' },
    body: JSON.stringify({ selectedOptionIndex: index, version: detail.session.version })
  });
  if (!response.ok) throw await response.json(); // App phân loại ProblemDetails theo status.
  return response.json();
}
```

Ví dụ dùng URL tương đối khi FE có reverse proxy `/api/v1`; nếu API khác origin thì ghép API base URL
của môi trường và cấu hình CORS allowlist hiện có. JWT dùng Authorization header như các module khác;
refresh/logout tiếp tục theo [tài liệu xác thực ở README](../README.md#endpoint).

**Khóa/serialize thao tác ghi theo toàn lượt**, không chỉ theo từng câu: các câu chung session.version.
Không cho answer và abandon gửi đồng thời từ một màn hình. Không optimistic-update điểm hoặc tự mở đáp án.
Gặp 409 thì GET detail, thay state và hiển thị kết quả đã được server ghi nhận.
Gặp 429 thì đọc Retry-After; không tự retry mutation với version cũ.

Cache key cần chứa user, profile và session; history còn chứa filter/cursor.
Khi đổi profile/logout, hủy request cũ và không đưa response profile cũ vào màn hình mới.
Audio không phải dữ liệu của bài nhận diện này; đề không có audioUrl gợi đáp án.
Muốn mở nội dung chữ sau khi luyện thì dùng writingSystemItemId để đối chiếu catalog hiện có.

## 9. Lỗi, giới hạn và tương tác DB

| HTTP | Ý nghĩa |
| --- | --- |
| 400 | Body/query sai, thiếu field bắt buộc, lựa chọn ngoài phạm vi, catalog thiếu câu đủ điều kiện |
| 401 | Thiếu/sai/hết hạn access token hoặc session xác thực đã bị revoke |
| 404 | Không thuộc owner, sai profile/session/question, tạo mới từ catalog không công khai |
| 409 | Version cũ, đổi đáp án đã nộp, thao tác không hợp trạng thái hoặc write race |
| 413 | Body lớn hơn 4096 bytes |
| 429 | Rate limiter learning-profiles hiện có; Retry-After tính bằng giây |
| 500 | Lỗi ngoài dự kiến; ProblemDetails không lộ dữ liệu nội bộ |

Lỗi theo ProblemDetails hiện có của dự án, kèm traceId khi middleware cung cấp.
API dùng policy `learning-profiles`, mặc định **120 request/IP/60 giây**, chia sẻ ngân sách
với profile/progress. Không tạo policy, Redis hoặc cấu hình Enabled riêng.
Giới hạn chạy theo instance; CORS, JWT validation và proxy settings dùng chung codebase.

Số SQL command của feature trên đường thành công (không gồm shared JWT/session validation,
BEGIN/COMMIT/ROLLBACK):

| Flow | SQL commands | Giải thích |
| --- | ---: | --- |
| Tạo 1–20 câu | 3 | Catalog join + conditional insert session + batch insert questions |
| GET detail | 1 | Ownership và toàn bộ câu trong cùng query, tối đa 20 câu |
| History có kết quả | 1 | Projection trang, tính count đúng/đã trả lời tại DB |
| History rỗng | 2 | Thêm EXISTS để phân biệt profile không hợp lệ với filter/trang rỗng |
| Answer mới | 3 | Tải aggregate + conditional UPDATE session + conditional UPDATE item |
| Retry answer no-op | 2 | Tải aggregate + xác nhận owner/version/state hiện tại, không UPDATE |
| Abandon hoặc retry abandon | 2 | Tải aggregate + một UPDATE hoặc EXISTS |

Catalog join tải đủ các cách đọc để validation không bỏ sót đáp án thay thế. Chi phí tạo đề tăng theo
số item/readings của bộ chữ; các request answer/detail không tải lại catalog.
History chỉ đọc trang cần trả, không tải toàn bộ câu/lịch sử về Application để đếm.

Tạo lượt dùng transaction READ COMMITTED ngắn: INSERT...SELECT kiểm tra lại owner, target language,
enabled/publication ngay lúc ghi và giữ FOR SHARE trên profile/language/system đến khi chèn đủ câu.
Thiếu FK hoặc lỗi bất kỳ câu nào rollback toàn bộ lượt. Snapshot nội dung được chụp khi đọc catalog;
sửa nội dung catalog sau mốc đó không làm thay đổi đề.

Answer cập nhật root trước, item sau trong transaction READ COMMITTED. Root UPDATE có điều kiện
owner/profile/id/expectedVersion/InProgress; item UPDATE có điều kiện session/id/Version 1/chưa nộp.
Nếu bất kỳ conditional update không ảnh hưởng đúng một dòng thì trả 409 và rollback cả hai.
No-op vẫn kiểm tra owner/version/state ở DB để từ chối snapshot cũ. Abandon chỉ đổi root nên một
UPDATE có điều kiện là đủ atomic, không mở transaction riêng.

## 10. Kiểm thử và triển khai

Request mẫu trong [`src/Api/Api.http`](../src/Api/Api.http). Không có migration mới của bước API;
áp migration model bằng lệnh hiện có nếu cần:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Runner `tests/WritingSystemPractice.Tests` chạy Domain checks trước; khi đặt
`WRITING_SYSTEM_PRACTICE_TEST_DATABASE`, chạy thêm EF/PostgreSQL và HTTP thật qua loopback Kestrel.
DB bắt buộc **trống, biệt lập**, tên `writing_system_practice_tests`. Runner từ chối DB sai tên
hoặc có bảng, không truncate và không dùng DB dự án.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/WritingSystemPractice.Tests/bin/Debug/net10.0/WritingSystemPractice.Tests.dll
```

Auth scheme giả và credentials test chỉ nằm trong test project; không đổi JWT production,
không gọi OAuth, AI hoặc R2 thật. Kiểm thử bao gồm auth/ownership, không lộ đáp án,
đề bền qua reload, filter/cursor, query budget, validation, lifecycle/no-op, ghi đồng thời,
rollback khi child CAS/insert lỗi và recheck quyền/publication tại lúc ghi.

Đã chạy thành công **329 kiểm tra** trên PostgreSQL 17 biệt lập: 56 Domain, 79 EF/PostgreSQL
và 194 kiểm tra HTTP/repository bổ sung. Migration model khớp runtime model, down/up thành công.
Build solution thành công; kiểm tra không áp migration hay ghi dữ liệu vào DB dự án/production.
