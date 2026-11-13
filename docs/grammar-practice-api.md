# API luyện ngữ pháp và tích hợp FE

## Mục đích và cấu trúc

API hỗ trợ học xong một điểm ngữ pháp rồi luyện điền chỗ trống, nhận đúng/sai và giải thích,
tiếp tục lượt đang làm và xem lịch sử. Người học có thể chọn luyện tổng hợp hoặc một điểm cụ thể.
Câu hỏi được lấy từ catalog đã xuất bản, đáp án được chấm tại server bằng dữ liệu đã lưu.

Feature được chia theo cấu trúc hiện có của dự án.

- `Domain/GrammarPractice` giữ aggregate, invariant và bản chụp câu hỏi.
- `Application/GrammarPractice` chứa Commands/Queries, handler, Models, Services và Validation.
- `Application/Common/Interfaces/IGrammarPracticeRepository` định nghĩa contract persistence.
- `Infrastructure/Persistence/Repositories/GrammarPracticeRepository` thực hiện đọc/chọn câu và ghi có transaction/CAS.
- `Api/Contracts/GrammarPractice` kiểm tra payload, controller gọi handler qua DI.

## Quy ước HTTP

Prefix chung của sáu endpoint là `/api/v1/learning-profiles/{profileId}/grammar-practice`.
Các đường dẫn trong bảng được nối tiếp prefix này.

| Method | Đường dẫn | Phục vụ màn hình / thao tác |
| --- | --- | --- |
| GET | `/options` | Nội dung có thể luyện, số câu khả dụng, giới hạn số câu |
| POST | `/sessions` | Tạo lượt luyện tổng hợp hoặc theo điểm ngữ pháp |
| GET | `/sessions` | Lịch sử, tìm lượt đang làm, phân trang |
| GET | `/sessions/{sessionId}` | Tiếp tục lượt hoặc xem kết quả |
| POST | `/sessions/{sessionId}/items/{sessionItemId}/answer` | Nộp một lựa chọn và lấy feedback |
| POST | `/sessions/{sessionId}/abandon` | Kết thúc sớm và giữ các đáp án đã ghi |

Tất cả endpoint yêu cầu `Authorization: Bearer <accessToken>`. User được lấy từ JWT.
Profile, session và item đều được kiểm tra trong phạm vi chủ sở hữu.
Các phản hồi dùng camelCase, enum dùng tên chuỗi, Snowflake ID dùng chuỗi thập phân.
Version dùng JSON number theo convention hiện có. Response cá nhân có `Cache-Control: no-store`.
Controller dùng policy rate limit `learning-profiles` hiện có và giới hạn body 4 KiB.

## Nội dung có thể luyện

`GET /options` trả 200 cùng các field sau.

| Field | Ý nghĩa |
| --- | --- |
| `learningProfileId`, `languageId`, `languageCode` | Profile và target language đã xác thực |
| `explanationLanguage` | Ngôn ngữ giải thích hiện tại của profile |
| `minQuestionCount`, `maxQuestionCount`, `defaultQuestionCount` | Lần lượt 5, 10, 10 |
| `availableQuestionCount` | Tổng số câu đủ điều kiện trong ngôn ngữ |
| `grammarPoints[]` | Những điểm đã xuất bản đang có câu đủ điều kiện |

Mỗi point có `id`, `code`, `nativeName`, `name`, `translationLocale`,
`isTranslationFallback`, `availableQuestionCount`, `sortOrder`.
Tên ưu tiên locale của profile rồi `en`, sau cùng dùng `nativeName`.
Số câu chỉ đếm exercise đã xuất bản thuộc point đã xuất bản và có bản dịch đúng locale hoặc `en`.
Một câu có cả hai bản dịch vẫn được đếm một lần.

FE dùng kết quả để hiển thị danh sách và đặt giới hạn chọn số câu.
Điểm có ít hơn 5 câu có thể hiển thị kèm số lượng và trạng thái chưa đủ câu để luyện riêng.
Ngôn ngữ đang bật và chưa có câu trả `grammarPoints: []`, `availableQuestionCount: 0`.
Ngôn ngữ bị ẩn, profile thiếu hoặc thuộc user khác trả 404.

Seed hiện có 100 câu Nhật và 100 câu Đức, phủ 10 điểm đầu mỗi ngôn ngữ.
Options phản ánh dữ liệu thực tế sau các lần cập nhật nội dung.

## Tạo lượt

Luyện một điểm bằng `POST /sessions`.

```json
{
  "grammarPointCode": "noun-copula",
  "questionCount": 5
}
```

Luyện tổng hợp bằng body `{}` hoặc `{"questionCount":10}`.
`grammarPointCode` null hoặc bị bỏ qua chọn phạm vi toàn ngôn ngữ của profile.
Code được trim/lowercase và kiểm tra định dạng dùng chung. `questionCount` nhận mọi số nguyên 5–10,
mặc định 10. FE có thể trình bày hai nút 5 câu và 10 câu.

Server đọc context profile/ngôn ngữ/phạm vi, rồi PostgreSQL lấy ngẫu nhiên tối đa số câu yêu cầu.
Mỗi exercise chọn đúng một translation, ưu tiên locale yêu cầu rồi `en`.
Các exercise trong lượt là duy nhất. Lựa chọn được xáo tại Application, Domain ánh xạ lại đáp án đúng.
Nếu số câu đủ điều kiện thấp hơn yêu cầu, trả 400 để FE giảm số câu hoặc đổi phạm vi.
Điểm thiếu, sai target language hoặc đã ẩn trả 404.

Server tạo bản chụp prompt, instruction, options theo thứ tự đã phát, đáp án, explanation,
locale thực tế và revision nguồn. Response 201 có `Location` trỏ tới GET chi tiết.
Tạo lượt mới là một thao tác mới, FE chỉ gửi một lần khi người dùng bấm bắt đầu.
Nếu mất kết nối trước khi nhận kết quả tạo, FE kiểm tra lịch sử lượt đang làm trước khi gửi lại.

## DTO chi tiết

Cả tạo lượt, GET chi tiết, trả lời và bỏ lượt đều trả cùng cấu trúc `{session, items}`.

```ts
type PracticeStatus = "InProgress" | "Completed" | "Abandoned";

interface GrammarPracticeSessionSummary {
  id: string;
  learningProfileId: string;
  languageId: string;
  grammarPointId: string | null;
  explanationLanguage: string;
  questionCount: number;
  status: PracticeStatus;
  answeredCount: number;
  correctCount: number;
  incorrectCount: number;
  unansweredCount: number;
  accuracyPercentage: number | null;
  version: number;
  createdAt: string;
  updatedAt: string;
  endedAt: string | null;
}

interface GrammarPracticeQuestion {
  id: string;
  grammarExerciseId: string;
  grammarPointId: string;
  position: number;
  prompt: string;
  instruction: string;
  translationLocale: string;
  isTranslationFallback: boolean;
  options: string[];
  selectedOptionIndex: number | null;
  correctOptionIndex: number | null;
  isCorrect: boolean | null;
  explanation: string | null;
  answeredAt: string | null;
}

interface GrammarPracticeSession {
  session: GrammarPracticeSessionSummary;
  items: GrammarPracticeQuestion[];
}
```

`items` được sắp theo `position`, bắt đầu 0. Prompt chứa đúng một token `{blank}`.
FE dùng token này để dựng chỗ trống và render text của các lựa chọn theo thứ tự server trả.
`selectedOptionIndex` là vị trí zero-based trong chính mảng đó.
ID dùng nguyên chuỗi khi lưu state và ghép URL.

Trước khi chấm, `correctOptionIndex`, `isCorrect`, `explanation`, `answeredAt` đều null.
Sau khi chấm một câu, các field này được điền cho câu đó. Câu còn chờ giữ trạng thái ẩn,
kể cả khi phiên đã bị bỏ. `instruction` luôn hiển thị vì diễn đạt yêu cầu làm bài.

`explanationLanguage` là locale yêu cầu được chụp từ profile lúc tạo lượt.
`translationLocale` là locale thực tế của từng câu. FE có thể dùng `isTranslationFallback`
để hiển thị ngôn ngữ đang được dùng. Thay preferences hoặc sửa/ẩn catalog sau đó vẫn giữ
nguyên đề và feedback của lượt đã tạo.

## Trả lời và bỏ lượt

Nộp câu bằng `POST /sessions/{sessionId}/items/{sessionItemId}/answer`.

```json
{
  "selectedOptionIndex": 0,
  "version": 1
}
```

`version` là `session.version` mới nhất FE đã nhận.
Server tải aggregate tối đa 10 câu, kiểm tra version, item và lựa chọn, rồi chấm bằng bản chụp.
Lựa chọn hợp lệ nằm trong khoảng 0 đến `options.length - 1` của câu đó.
Mỗi câu nhận một câu trả lời. Server cập nhật root và item trong cùng transaction.
Khi câu cuối được trả lời, phiên chuyển `Completed` và có `endedAt` trong cùng lần ghi.

Nộp lại cùng lựa chọn với version hiện tại là no-op. Nộp lựa chọn khác cho câu đã trả lời trả 409.
Version cũ trả 409, kể cả cùng lựa chọn. Quy tắc này khớp luồng luyện bảng chữ hiện có.
FE cập nhật toàn bộ DTO từ response sau mỗi thao tác thành công.

`POST /sessions/{sessionId}/abandon` nhận `{"version":2}`.
Server giữ các câu đã trả lời, chuyển `Abandoned` và tăng version. Gửi lại cùng trạng thái
với version hiện tại là no-op. Lượt đã `Completed` trả 409 khi bị yêu cầu bỏ.
Câu chưa làm vẫn được tính trong `unansweredCount`.

`accuracyPercentage` tính trên số câu đã trả lời, làm tròn hai chữ số thập phân,
null khi `answeredCount` bằng 0. Khi hoàn thành toàn bộ, đây cũng là tỷ lệ đúng của cả lượt.

## Lịch sử và tiếp tục

`GET /sessions?pageSize=20&status=InProgress&grammarPointCode=noun-copula&before=...`.

- `pageSize` mặc định 20, giới hạn 1–100.
- `status` nhận `InProgress`, `Completed`, `Abandoned`.
- `grammarPointCode` lọc phạm vi point đã chọn khi tạo lượt. Một lượt tổng hợp có `grammarPointId` null.
- `before` là `nextCursor` nguyên chuỗi từ trang trước, biểu diễn session ID dương.

Response có `items` là các summary DTO và `nextCursor` nullable, thứ tự ID giảm dần.
Mỗi trang dùng `pageSize + 1` để xác định trang tiếp theo.
GET chi tiết theo session ID cung cấp DTO đầy đủ để resume hoặc xem feedback.
Ẩn point/language vẫn cho phép chủ sở hữu đọc và hoàn tất các lượt đã tạo.

## Ghép luồng FE

1. Lấy profile hiện tại qua API LearningProfile, giữ `profile.id` dạng string.
2. Gọi options để dựng danh sách điểm và số câu. Khi vào từ trang chi tiết điểm ngữ pháp,
   dùng `code` của điểm đó đối chiếu với options.
3. Gửi POST tạo lượt theo lựa chọn, lưu `session.id` để resume và render `items`.
4. Chọn một đáp án rồi POST answer với `item.id`, `selectedOptionIndex` và version hiện tại.
   Khóa thao tác trong khi request đang chạy, cập nhật DTO và render feedback server trả.
5. Khi refresh trang, GET detail theo session ID. Có thể tìm lượt đang làm bằng history với `status=InProgress`.
6. Khi bỏ lượt, POST abandon và cập nhật lại detail/history. Khi đổi profile, tải dữ liệu trong phạm vi profile mới.

Ví dụ TypeScript sử dụng HTTP client có sẵn của FE, đã gắn access token và xử lý refresh token.
Trong ví dụ, `api.get/post<T>` trả response body kiểu T và nhận URL tính từ API origin.

```ts
const path = (profileId: string) =>
  `/api/v1/learning-profiles/${profileId}/grammar-practice`;

const start = (profileId: string, grammarPointCode?: string) =>
  api.post<GrammarPracticeSession>(`${path(profileId)}/sessions`, {
    grammarPointCode,
    questionCount: 5,
  });

const resume = (profileId: string, sessionId: string) =>
  api.get<GrammarPracticeSession>(`${path(profileId)}/sessions/${sessionId}`);

const answer = (detail: GrammarPracticeSession, itemId: string, selectedOptionIndex: number) =>
  api.post<GrammarPracticeSession>(
    `${path(detail.session.learningProfileId)}/sessions/${detail.session.id}/items/${itemId}/answer`,
    { selectedOptionIndex, version: detail.session.version },
  );

const abandon = (detail: GrammarPracticeSession) =>
  api.post<GrammarPracticeSession>(
    `${path(detail.session.learningProfileId)}/sessions/${detail.session.id}/abandon`,
    { version: detail.session.version },
  );
```

Nếu answer timeout hoặc nhận 409, GET detail để xác định kết quả đã được ghi.
Nếu câu vẫn chưa trả lời và lượt còn đang làm, FE có thể gửi lại lựa chọn đã chọn với version mới.
Nếu câu đã trả lời, render kết quả được server ghi nhận. Với lỗi tạo lượt 409, tải lại options.

## Lỗi HTTP

| Status | Ý nghĩa / cách xử lý |
| --- | --- |
| 400 | Payload/query sai hoặc chưa đủ câu, sửa input/số câu/phạm vi |
| 401 | Thực hiện luồng refresh/login hiện có của FE |
| 404 | Resource thiếu, ẩn hoặc ngoài phạm vi sở hữu, tải lại danh sách tương ứng |
| 409 | Version/trạng thái hoặc nội dung nguồn thay đổi, tải lại detail/options |
| 413 | Body vượt giới hạn 4 KiB |
| 429 | Chờ theo `Retry-After` |

Lỗi dùng ProblemDetails và `traceId` theo pipeline chung, lỗi validation payload có thể kèm `errors`.
POST body dùng strict unmapped-member validation để kiểm soát field được gửi.

## DB, transaction và query budget

| Luồng thành công | Số lệnh dữ liệu của module |
| --- | --- |
| Options | 1 |
| Tạo lượt | 4 |
| Chi tiết | 1 |
| Lịch sử có kết quả | 1 |
| Lịch sử trống | 2, thêm kiểm tra profile tồn tại/thuộc owner |
| Trả lời mới | 3 |
| Trả lời trùng với version hiện tại | 2 |
| Bỏ lượt / bỏ lại với version hiện tại | 2 |

Query budget đo bằng interceptor trong kiểm thử HTTP. BEGIN/COMMIT thuộc điều khiển transaction,
xác thực và kiểm tra phiên dùng pipeline chung của hệ thống.

Tạo lượt đọc context một lần và lấy mẫu một lần, giới hạn kết quả ở DB.
`ORDER BY random()` chọn đều trong tập exercise hợp lệ, PostgreSQL duyệt các ứng viên để lấy mẫu.
Query này phù hợp catalog hiện có. Thời gian lấy mẫu tăng theo số exercise đủ điều kiện.

Transaction ghi ngắn dùng READ COMMITTED. Một conditional INSERT với CTE kiểm tra lại user/profile,
target language, locale profile, publication, exercise revision và translation revision của toàn bộ câu.
Các dòng profile/language/point/exercise/translation liên quan được khóa SHARE tới khi batch item insert hoàn tất.
Xung đột hoặc lỗi ở bất kỳ bước ghi nào rollback root và các item cùng nhau.
Lựa chọn câu và xáo thứ tự diễn ra trước transaction ghi.

Trả lời dùng một lần đọc aggregate, conditional UPDATE root theo owner/version/state và conditional
UPDATE item theo session/version/chưa trả lời. Hai UPDATE cùng transaction, root là điểm cạnh tranh giữa
hai request trả lời hai câu khác nhau. Bỏ lượt cập nhật một root bằng một conditional UPDATE.
No-op kiểm tra lại version/trạng thái hiện tại bằng EXISTS để phát hiện thay đổi sau lần đọc.

Lịch sử project summary trong DB, đếm child qua index `session_id`, mỗi session có tối đa 10 câu.
Read dùng `AsNoTracking`. Index phục vụ ownership/history/cursor đã có trong migration model.
Schema của API dùng migration `20261111042903_AddGrammarPractice` đã tạo ở bước model.

## Chạy và kiểm thử

Áp migration bằng cấu hình DB môi trường rồi chạy API như các module hiện có.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
```

Console runner `tests/GrammarPractice.Tests` kiểm tra Domain, EF, PostgreSQL và HTTP thật qua Kestrel.
Đặt `GRAMMAR_PRACTICE_TEST_DATABASE` tới DB trống biệt lập tên `grammar_practice_tests` để chạy toàn bộ.
Runner xác nhận tên/trạng thái DB trước khi áp migrations.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/GrammarPractice.Tests/bin/Debug/net10.0/GrammarPractice.Tests.dll
```

Các kiểm tra bao gồm owner isolation, JSON validation, che đáp án/giải thích, tạo lượt đúng ngôn ngữ,
case tiếng Đức, fallback, scope, phân trang, resume, completion, abandon, retry/version, rate limit,
query budget, source revision/publication race và rollback root khi child CAS/insert thất bại.

Lần xác minh đạt 313 kiểm tra Domain/EF/PostgreSQL/HTTP của module và 8.313 kiểm tra hồi quy
`Grammar.Tests`. Build toàn solution thành công. Môi trường có warning NU1900 khi gọi NuGet audit.
Lần biên dịch Application cũng báo CS8604 có sẵn tại `OAuthLoginService` với email nullable.
Các kiểm thử DB chạy trên PostgreSQL 17 trong container biệt lập.
