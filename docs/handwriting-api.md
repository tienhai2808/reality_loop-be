# Handwriting API — tích hợp frontend

Module luyện viết một đề đã biết, không gọi AI/OCR và không cần upload ảnh lên storage.
Backend nhận chuỗi nét chuẩn hóa, chấm hình dạng/thứ tự/hướng nét, lưu một lượt nộp hoàn chỉnh và trả feedback.
Model/schema: [handwriting-model.md](handwriting-model.md). Bảng viết thử: [handwriting-lab.md](handwriting-lab.md).

## Endpoint và mục đích

Tất cả endpoint có prefix **`/api/v1`**, yêu cầu `Authorization: Bearer <accessToken>`.
`profileId` phải thuộc user đang đăng nhập; profile của người khác được xử lý như không tồn tại (404).
Không nhận UserId hoặc kết quả chấm từ client.

| Method | Route sau /api/v1 | Phục vụ FE |
| --- | --- | --- |
| GET | `/learning-profiles/{profileId}/writing-systems/{systemCode}/handwriting-templates` | Danh sách chữ đã có mẫu nét, mỗi chữ một revision đã publish mới nhất |
| GET | `/learning-profiles/{profileId}/writing-systems/{systemCode}/handwriting-templates/{templateId}` | Lấy nét mẫu cho màn hướng dẫn và giới hạn input |
| POST | `/learning-profiles/{profileId}/writing-systems/{systemCode}/handwriting-attempts` | Nộp 1–4 chữ, backend chấm và lưu cả lượt |
| GET | `/learning-profiles/{profileId}/handwriting-attempts` | Lịch sử, phân trang và lọc kết quả |
| GET | `/learning-profiles/{profileId}/handwriting-attempts/{attemptId}` | Xem lại nét đã nộp và feedback từng chữ/từng nét |

`systemCode` dùng code catalog hiện có, ví dụ `hiragana`. Backend chuẩn hóa trim/lowercase theo convention.
Ngôn ngữ suy ra từ profile.TargetLanguage, không có language parameter để client chọn lệch profile.
Mỗi chữ trong lượt phải thuộc cùng writing system. Chữ lặp trong từ hợp lệ, ví dụ `ああ`.

Hiện có **104 mục Hiragana + 104 mục Katakana** (208 mẫu, 765 nét), gồm chữ cơ bản, âm đục/bán đục và âm ghép thông dụng. Mỗi bảng cần nhiều trang khi tải đủ mẫu. [Phạm vi seed](japanese-seed-content.md)
Danh sách mặc định 20 mục/trang: dùng nextCursor để tải tiếp, hoặc pageSize=100 để lấy cả bảng.
FE hiện có nút More characters để tải tiếp. Giới hạn 4 vị trí trong một lượt nộp không giới hạn số mẫu trong danh mục.
Writing system đã publish nhưng chưa có mẫu trả 200 với items rỗng; FE hiển thị trạng thái chưa có nội dung.
Không tự ghép chữ chưa có mẫu hoặc đổi sang AI khi thiếu mẫu.

## 1. Danh sách mẫu

```http
GET /api/v1/learning-profiles/PROFILE_ID/writing-systems/hiragana/handwriting-templates?pageSize=20
Authorization: Bearer ACCESS_TOKEN
```

Query: `pageSize` mặc định 20, từ 1–100; `after` là nextCursor của trang trước, gửi nguyên giá trị đó.
Thứ tự `(item.SortOrder ASC, item.Id ASC)`, cursor dạng `sortOrder:writingSystemItemId`.
Cursor dùng item ID ổn định, không dùng template ID có thể thay đổi khi thêm revision.

Một item trả về có cấu trúc:

```json
{
  "reference": {
    "id": "1563703640068193208",
    "writingSystemItemId": "1563703640068190308",
    "symbol": "あ",
    "revision": 1,
    "source": {
      "url": "https://github.com/KanjiVG/kanjivg/tree/70a0b7ae0c18ceb5cb358274b029cce0234a43bc",
      "revision": "70a0b7ae0c18ceb5cb358274b029cce0234a43bc",
      "attribution": "KanjiVG, copyright Ulrich Apel. Paths sampled into normalized point sequences by RealityLoop.",
      "license": "CC BY-SA 3.0",
      "licenseUrl": "https://creativecommons.org/licenses/by-sa/3.0/"
    }
  },
  "itemCode": "kana-r0-c0",
  "sortOrder": 0,
  "version": 1,
  "updatedAt": "2026-11-04T00:00:00+00:00"
}
```

Response ngoài cùng: `{ "items": [...], "nextCursor": null, "policy": {...} }`.
Danh sách không chứa geometry. `reference.id` là templateId để gọi detail/nộp bài.
Tất cả Snowflake ID trong JSON là **string**; không đưa qua Number/parseInt vì JavaScript mất chính xác.
Revision là phiên bản nội dung mẫu; Version là concurrency của publication, không phải điểm/kết quả.

Mỗi item chỉ xuất hiện một lần, chọn Revision lớn nhất trong các mẫu đang publish và khớp Symbol hiện tại.
Detail/nộp bài vẫn cho phép revision cũ **còn publish**, để màn đang mở không tự chuyển sang mẫu khác.
Muốn ngừng cho nộp revision cũ thì unpublish nó. Mẫu của item đã đổi Symbol không dùng cho lượt mới.

## 2. Chi tiết mẫu

```http
GET /api/v1/learning-profiles/PROFILE_ID/writing-systems/hiragana/handwriting-templates/TEMPLATE_ID
Authorization: Bearer ACCESS_TOKEN
```

Response:

```ts
type Point = { x: number; y: number };
type TemplateDetail = {
  template: TemplateSummary; // item của danh sách trên
  drawingFormatVersion: 1;
  strokes: Point[][];        // nét theo thứ tự; điểm trong nét theo hướng viết
  strokeCount: number;
  policy: {
    version: "geometry-v1";
    samplesPerStroke: 48;
    shapeTolerance: 0.10;
    simplificationTolerance: 0.01;
    minimumDrawingExtent: 0.18;
    minimumRawStrokeLength: 0.008;
    minimumLengthRatio: 0.65;
    maximumLengthRatio: 1.50;
    maximumCharacters: 4;
    maximumStrokesPerCharacter: 12;
    maximumPointsPerStroke: 512;
    maximumTotalPoints: 4096;
  };
};
```

Mẫu bị unpublish, sai profile/ngôn ngữ/writing system hoặc không tồn tại trả 404.
FE dùng source.attribution và liên kết source.licenseUrl khi trình bày dữ liệu mẫu; giữ giấy phép nếu phân phối dataset.
Policy công khai giúp FE giới hạn input và giải thích kết quả; server luôn dùng policy của chính nó.

## 3. Nộp bài

```http
POST /api/v1/learning-profiles/PROFILE_ID/writing-systems/hiragana/handwriting-attempts
Authorization: Bearer ACCESS_TOKEN
Content-Type: application/json

{
  "characters": [
    { "templateId": "1563703640068193208", "strokes": [] }
  ]
}
```

Ví dụ này cố ý nộp ô trống: request hợp lệ, lưu lượt không đạt với lỗi MissingDrawing.
Khi nộp thật, thay strokes bằng `Point[][]` thu từ người dùng. Backend ghép TargetText từ template,
không nhận targetText, score, passed, feedback, algorithmVersion, userId hoặc version từ client.
Trường JSON không khai báo bị từ chối, kể cả trong từng chữ/từng điểm.

Giới hạn:

- 1–4 phần tử characters, giữ đúng thứ tự từ trái sang phải của đề. Chữ lặp dùng cùng templateId ở hai vị trí.
- 0–12 nét/chữ; `[]` là chưa vẽ. Nét đã tồn tại phải có 1–512 điểm, không được `[]` hoặc null.
- Tổng không quá 4096 điểm cho **toàn bộ lượt**, không phải mỗi chữ.
- X/Y bắt buộc, là số JSON hữu hạn trong [0,1]. Không nhận chuỗi số, NaN/Infinity, null hoặc thiếu tọa độ.
- Body tối đa **256 KiB**. Body lớn trả 413; content type sai trả 415.
- Nét sai hình/hướng/thứ tự, quá ngắn/dài hoặc thiếu nét là **kết quả học tập**, không phải lỗi HTTP.

Thành công trả **201 Created**, `Location` trỏ đến API chi tiết lượt nộp, và body:

```json
{
  "attempt": {
    "id": "ATTEMPT_SNOWFLAKE",
    "learningProfileId": "PROFILE_SNOWFLAKE",
    "writingSystemId": "WRITING_SYSTEM_SNOWFLAKE",
    "targetText": "あ",
    "algorithmVersion": "geometry-v1",
    "characterCount": 1,
    "passed": false,
    "matchScore": 0,
    "version": 1,
    "createdAt": "2026-11-04T12:00:00+00:00",
    "updatedAt": "2026-11-04T12:00:00+00:00"
  },
  "characters": [{
    "id": "CHARACTER_SNOWFLAKE",
    "position": 0,
    "template": {
      "id": "1563703640068193208",
      "writingSystemItemId": "1563703640068190308",
      "symbol": "あ",
      "revision": 1,
      "source": { "url": "...", "revision": "...", "attribution": "...", "license": "CC BY-SA 3.0", "licenseUrl": "..." }
    },
    "drawingFormatVersion": 1,
    "strokes": [],
    "feedbackFormatVersion": 1,
    "feedback": {
      "expectedStrokeCount": 3,
      "actualStrokeCount": 0,
      "issues": ["MissingDrawing"],
      "strokes": [],
      "passed": false,
      "matchScore": 0
    }
  }]
}
```

Khi qua precheck, `feedback.strokes` có đúng số nét chuẩn, theo Index từ 0:

```ts
type StrokeFeedback = {
  index: number;
  passed: boolean;
  matchScore: number; // 0..100
  distance: number;
  lengthRatio: number;
  issues: HandwritingIssue[];
};
type HandwritingIssue =
  | "MissingDrawing" | "WrongStrokeCount" | "DrawingTooSmall"
  | "WrongStrokeOrder" | "WrongDirection" | "StrokeTooShort" | "StrokeTooLong" | "WrongShape";
```

FE có thể dịch các mã thành: chưa viết, sai số nét, chữ quá nhỏ, sai thứ tự, ngược hướng,
nét quá ngắn, nét quá dài, hình nét chưa khớp. Chỉ hiển thị lỗi từ response; không tự sửa kết quả.
Passed chỉ true khi mọi nét/mọi chữ đạt. MatchScore là điểm tương đồng hình học, không phải xác suất OCR
hoặc chứng nhận trình độ. Điểm cao vẫn có thể trượt do sai hướng/thứ tự; không dùng ngưỡng điểm riêng ở FE.

POST tạo lượt mới mỗi lần; **chưa có idempotency key cho endpoint này**. FE khóa nút khi đang gửi,
không tự retry POST sau timeout/refresh token. Nếu không rõ request đã lưu chưa, tải lịch sử để đối chiếu;
không chỉ nhìn TargetText vì người dùng có thể cố ý luyện lại cùng từ. Viết lại tạo lượt mới, không PATCH/ghi đè bài cũ.
Abort request không bảo đảm DB chưa commit nếu client đã mất kết nối sau lúc server ghi xong.

## 4. Lịch sử và chi tiết

```http
GET /api/v1/learning-profiles/PROFILE_ID/handwriting-attempts?systemCode=hiragana&passed=false&pageSize=20
GET /api/v1/learning-profiles/PROFILE_ID/handwriting-attempts?before=NEXT_CURSOR&pageSize=20
GET /api/v1/learning-profiles/PROFILE_ID/handwriting-attempts/ATTEMPT_ID
```

List trả `{ "items": AttemptSummary[], "nextCursor": string | null }`, summary chính là object attempt ở trên.
Filter systemCode/passed đều tùy chọn; passed nhận true/false. PageSize 1–100, mặc định 20.
Thứ tự `id DESC`, before là Snowflake ID cuối trang trước, giữ dạng string. Giữ nguyên filter khi chuyển trang.
Không totalCount/OFFSET hoặc trả toàn bộ nét cho màn danh sách.

Detail có cùng shape với POST response và giữ template revision, geometry đã nộp, feedback, AlgorithmVersion.
History vẫn xem được sau khi unpublish mẫu/writing system hoặc tắt ngôn ngữ. Ownership luôn được kiểm tra;
xóa LearningProfile sẽ cascade lịch sử riêng của profile đó. Nộp bài không tự tăng tiến độ tự học hoặc Version profile.

## 5. Ghép canvas và API ở FE

1. Lấy profile hiện tại qua API LearningProfile sẵn có, rồi chọn writing system từ catalog.
2. Lấy danh sách mẫu. Chỉ cho tạo đề từ các item có trong danh sách, tối đa 4 vị trí.
3. GET detail từng template đã chọn, có thể song song. Nếu ID trùng trong đề, chỉ tải một lần.
4. Mỗi vị trí dùng ô vuông độc lập và lưu `Point[][]` trong state. Pointerdown bắt đầu nét,
   pointermove thêm điểm, pointerup kết thúc nét. Pointercancel hủy nét dở dang. Undo xóa nét cuối.
5. Nộp một request chứa mọi vị trí theo thứ tự. Dùng body response cập nhật kết quả ngay, không cần GET lại.
6. Khi xem lịch sử, GET detail và replay chính strokes đã nộp; feedback.strokes[index] tương ứng nét cùng index.

Chuẩn hóa từ tọa độ CSS của canvas, không dùng backing-store width/height đã nhân DPR:

```ts
function pointFromPointer(event: PointerEvent, canvas: HTMLCanvasElement) {
  const r = canvas.getBoundingClientRect();
  return {
    x: Math.max(0, Math.min(1, (event.clientX - r.left) / r.width)),
    y: Math.max(0, Math.min(1, (event.clientY - r.top) / r.height)),
  };
}
```

Giữ canvas CSS hình vuông; tránh border/padding trong vùng tính tọa độ. Set `touch-action: none`,
setPointerCapture khi viết. Resize/DPR chỉ thay cách render; không đổi tọa độ logic đang lưu.
Không tự xoay, lật, căn từng nét hoặc gộp nhiều nét trước khi gửi vì sẽ làm mất thông tin cần chấm.
Thu điểm có giới hạn, giữ điểm đầu/cuối; nếu cần giảm điểm dùng phương pháp bảo toàn hình học và kiểm thử.
Không lặng lẽ cắt bỏ nửa cuối nét khi vượt 512 điểm. Hiển thị số nét và giới hạn trước khi nộp.

Ví dụ hàm nộp (apiBase đã gồm `/api/v1`):

```ts
type CharacterInput = { templateId: string; strokes: Point[][] };

async function submitHandwriting(
  apiBase: string, accessToken: string, profileId: string, systemCode: string,
  characters: CharacterInput[], signal?: AbortSignal,
) {
  const response = await fetch(
    `${apiBase}/learning-profiles/${encodeURIComponent(profileId)}` +
    `/writing-systems/${encodeURIComponent(systemCode)}/handwriting-attempts`,
    {
      method: "POST",
      headers: { Authorization: `Bearer ${accessToken}`, "Content-Type": "application/json" },
      body: JSON.stringify({ characters }), signal,
    },
  );
  if (!response.ok) throw Object.assign(new Error("Không nộp được bài viết"), {
    status: response.status, problem: await response.json(),
    retryAfter: response.headers.get("Retry-After"),
  });
  return response.json();
}
```

Chặn double-click bằng state inFlight, giữ strokes khi lỗi, chỉ thay feedback nếu response vẫn thuộc đề/input
đang hiển thị. Dùng request ID cục bộ hoặc AbortController để response cũ không ghi đè bài đã sửa.
Token refresh tiếp tục dùng cơ chế auth hiện có; riêng handwriting không có cookie/token riêng.
CORS dùng cấu hình Frontend hiện có và expose Location/Retry-After; không cần domain media/R2.

## Lỗi và giới hạn vận hành

| HTTP | Trường hợp | FE xử lý |
| --- | --- | --- |
| 400 | Input/cursor sai, vượt điểm/nét/chữ, thiếu tọa độ, trường lạ | Giữ nét, sửa input; không retry nguyên request |
| 401 | Thiếu/hết hạn token | Auth flow hiện có; không tự gửi lặp POST khi kết quả lần đầu chưa rõ |
| 404 | Không sở hữu profile, sai system, template thiếu/unpublished hoặc attempt không tồn tại | Tải lại nội dung/kiểm tra profile |
| 409 | Context/reference thay đổi giữa lúc đọc và ghi, hoặc xung đột ID | Giữ nét, reload mẫu, xác nhận lại đề trước khi nộp |
| 413 | Body >256 KiB | Giảm dữ liệu capture trước khi nộp |
| 415 | Content-Type không phải JSON | Gửi application/json |
| 429 | Hết rate limit | Tôn trọng Retry-After, không vòng lặp retry |

Lỗi nghiệp vụ dùng ProblemDetails + traceId; lỗi JSON/model binding dùng ValidationProblemDetails.
Cache-Control no-store/Pragma no-cache áp cho các route này, kể cả lỗi và 429.

POST dùng limiter IP riêng `RateLimiting:Handwriting` (mặc định 30 request/60 giây, không queue).
GET dùng policy learning-profiles hiện có. Reverse proxy phải cấu hình KnownProxies đúng như hướng dẫn auth,
để client IP/rate limit được xác định đúng. Limiter hiện tại theo process, không phải quota phân tán giữa replica.
Không thêm Redis cache cho geometry ở bước này; xử lý thuần theo request, chưa có cache invalidation để làm sai publication.

## Kiến trúc, query budget và transaction

- Api: request/JSON validation, controller mỏng gọi handler trực tiếp, JWT/routing/CORS/rate limit hiện có.
- Application/Handwriting: commands/queries/models/validation và bộ chấm thuần .NET, không EF/HTTP/storage;
  port IHandwritingRepository đặt tại Application/Common/Interfaces theo codebase.
- Domain: aggregate/value object/invariant của model hiện có.
- Infrastructure: implementation HandwritingRepository, SQL/EF projection, ownership và ghi có transaction.
- Lab tham chiếu Application và adapter để dùng **cùng một bộ chấm**, không duy trì hai thuật toán khác nhau.

Các bước POST:

1. Validate toàn bộ input/giới hạn trước DB.
2. Một SELECT lấy owned profile + language/system + tối đa 4 mẫu được yêu cầu; không tải cả catalog.
3. Chấm trong bộ nhớ, tạo aggregate và Snowflake IDs. Có cancellation và subset DP tối đa 2^12 trạng thái
   cho phép kiểm tra đảo thứ tự, không dùng tìm hoán vị factorial trên input mơ hồ.
4. Mở transaction ngắn; một INSERT root có điều kiện, kiểm tra lại user/profile, language/system publication,
   đúng item, Symbol và version của template/item, giữ FOR SHARE trên các hàng context/reference đến commit.
5. Một EF SaveChanges batch insert các children; commit. Lỗi child rollback cả root.
6. Trả DTO từ aggregate đã có, không đọc lại DB.

Budget SQL command cho riêng module, không tính BEGIN/COMMIT hoặc kiểm tra session JWT hiện có:

| Flow | SQL commands |
| --- | --- |
| Input sai | 0 |
| List template/history có dữ liệu | 1 |
| List rỗng cần phân biệt scope tồn tại với 404 | 2 |
| Detail template/attempt | 1 |
| POST thành công (1–4 chữ) | 3 |
| POST scope/template không hợp lệ lúc đọc | 1 |

List chọn/projection/phân trang ở DB, không tải JSONB nét/feedback và không N+1.
History detail chỉ lấy geometry đã nộp, không đọc geometry mẫu lần nữa.
FOR SHARE cho phép nhiều lượt nộp hợp lệ cùng đọc context; không giữ transaction trong lúc chấm.
Coverage được bổ sung bởi migration `20261104163042_CompleteKanaHandwritingTemplates`: chỉ INSERT 88 mẫu, giữ nguyên 4 mẫu cũ; không đổi schema/package/API contract.

## Chạy và kiểm thử

Áp migration theo cách triển khai hiện có:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
```

Ví dụ request nằm trong `src/Api/Api.http`. Thay accessToken, learningProfileId và template/attempt ID từ response.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Handwriting.Tests/bin/Debug/net10.0/Handwriting.Tests.dll
dotnet tests/HandwritingLab.Tests/bin/Debug/net10.0/HandwritingLab.Tests.dll
```

Handwriting.Tests mặc định chạy Domain/scoring. Để chạy EF/PostgreSQL/HTTP, đặt HANDWRITING_TEST_DATABASE
thành connection string của DB **trống, biệt lập**, tên bắt buộc handwriting_tests. Không dùng DB dự án/production.
Runner từ chối DB có bảng; test fake authentication chỉ thuộc test host, production vẫn dùng JWT/session thật.
Kiểm tra gồm API/JSON contract, ownership, publication, cursor, grading, query budget, race tại write,
rollback khi child conflict, concurrent submissions và rate limit/Retry-After.

Đã chạy thành công **962 kiểm tra**: 76 Domain, 5 scoring, 200 EF/PostgreSQL và 681 HTTP/repository integration,
trên PostgreSQL 17 biệt lập; đồng thời **162 kiểm tra hồi quy lab** vẫn đạt với bộ chấm dùng chung.
Các kiểm tra đồng thời là kiểm tra tính đúng đắn, không phải chứng nhận throughput/độ chính xác trên người học.

Coverage checks xác minh đủ 92 mẫu tự chấm đạt 100, từng mẫu đều nộp/lưu/đọc được qua API,
phân trang 20 mục tải đủ 46 Hiragana không trùng/thiếu, và down/up migration chỉ tác động 88 mẫu bổ sung.
