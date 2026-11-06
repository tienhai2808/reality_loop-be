# Vocabulary Learning API — flashcard, hàng đợi ôn và tích hợp FE

Backend theo dõi từng **nghĩa của từ**, cung cấp mặt thẻ/đáp án và tự tính lịch sau mỗi lần tự đánh giá.
Không gọi AI, không tự đánh dấu đã nhớ mọi nghĩa của một từ và không gộp với tiến độ luyện viết.
Model/constraint: [vocabulary-learning-model.md](vocabulary-learning-model.md).

## Endpoint

Prefix chung: **`/api/v1/learning-profiles/{profileId}`**. Tất cả yêu cầu Bearer accessToken.
Profile phải thuộc user đăng nhập. Scope của người khác luôn trả 404, không cho probe receipt/history.

| Method | Route sau prefix | Phục vụ |
| --- | --- | --- |
| PUT | `/vocabulary-senses/{senseId}/learning-progress` | Thêm đúng một nghĩa vào danh sách học, không reset nếu đã có |
| GET | `/vocabulary-learning` | Hàng đợi đến hạn hoặc danh sách tất cả thẻ đã thêm |
| GET | `/vocabulary-learning/summary` | Tổng số thẻ, trạng thái, số đến hạn và lịch tương lai gần nhất |
| GET | `/vocabulary-learning/{progressId}` | Chi tiết thẻ và đáp án theo locale |
| POST | `/vocabulary-learning/{progressId}/reviews` | Ghi tự đánh giá với request ID và expected version |
| GET | `/vocabulary-learning/{progressId}/reviews` | Lịch sử ôn, có phân trang |

Lấy **senseId** từ API vocabulary senses hiện có, không dùng entryId thay vào. Snowflake ID trong JSON
luôn là **string**: giữ nguyên, không parseInt/Number vì vượt độ chính xác JavaScript.
Version/counter/interval là số; cursor là opaque string, gửi lại nguyên vẹn.
Route dùng profile rõ ràng; không tự đổi sang profile hiện tại nếu user đang thao tác ở một tab cũ.

## 1. Thêm nghĩa vào danh sách học

```http
PUT /api/v1/learning-profiles/PROFILE_ID/vocabulary-senses/SENSE_ID/learning-progress
Authorization: Bearer ACCESS_TOKEN
```

Không cần body. Entry phải publish, ngôn ngữ phải bật và khớp TargetLanguage của profile.
Lần đầu trả **201 Created** và Location trỏ đến thẻ; đã có trả **200 OK**, không đổi lịch/counter/version.
Hai request thêm đồng thời chỉ tạo một row. Không seed sẵn tiến độ cho mọi từ trong catalog.

Response là Progress, dùng chung ở card/queue:

```ts
type Progress = {
  id: string;
  learningProfileId: string;
  vocabularySenseId: string;
  status: "New" | "Learning" | "Reviewing";
  nextReviewAt: string;
  lastReviewedAt: string | null;
  intervalMinutes: number;
  reviewCount: number;
  againCount: number;
  consecutiveRecallCount: number;
  schedulerVersion: string | null;
  version: number;
  createdAt: string;
  updatedAt: string;
};
```

New: version=1, reviewCount/againCount/streak/interval=0, lastReviewedAt/schedulerVersion=null,
nextReviewAt=createdAt. Thẻ mới đến hạn ngay; thêm thẻ không được tính là đã học hay đã ôn.

## 2. Hàng đợi và thẻ

```http
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning?pageSize=20
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning?dueOnly=false&status=Reviewing&pageSize=20
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning/PROGRESS_ID?locale=vi
```

List query:

- dueOnly mặc định true: NextReviewAt <= AsOf theo clock server, không theo đồng hồ FE.
- dueOnly=false: mọi thẻ đã thêm còn nội dung khả dụng, kể cả lịch tương lai.
- status tùy chọn New/Learning/Reviewing; không có trạng thái giả Completed/Mastered.
- pageSize 1–100, mặc định 20; after là NextCursor của trang trước.
- locale tùy chọn: normalize lowercase language tag. Không truyền thì dùng **ExplanationLanguage của profile**.

List trả `{ items: Flashcard[], nextCursor: string | null, asOf: string }`. Flashcard:

```ts
type Flashcard = {
  progress: Progress;
  vocabularyEntryId: string;
  entryCode: string;
  term: string;
  reading: string | null;
  audioUrl: string | null;
  senseCode: string;
  partOfSpeech: string; // enum tên từ catalog hiện có
  meaning: string | null;
  usageNote: string | null;
  requestedLocale: string;
  translationLocale: string | null;
  isTranslationFallback: boolean;
};
```

Detail trả trực tiếp một Flashcard. Dữ liệu hiện tại có cả mặt trước lẫn đáp án để FE lật thẻ;
đây là tự đánh giá, không phải kỳ thi có đáp án bí mật. FE tự quản lý trạng thái front/back.

Bản dịch: exact requestedLocale -> en -> null. Không tự fallback locale cha hoặc chọn ngẫu nhiên ngôn ngữ khác.
Nếu meaning=null, FE hiển thị thiếu bản dịch, không chế đáp án. AudioUrl có thể null và chỉ phát khi có URL;
module này không upload audio hoặc ký URL mới. Sense và meaning/POS luôn đi cùng nhau.

Thứ tự `(next_review_at ASC, id ASC)`. Cursor chứa AsOf + due timestamp + ID, không OFFSET/totalCount.
AsOf giữ nguyên khi chuyển trang để thẻ vừa đến hạn sau cutoff không chen vào trang sau.
Đây **không phải snapshot DB cố định**: thêm thẻ, review hoặc thay catalog có thể thay đổi tập kết quả.
Sau một review thành công, invalidate queue/summary và lấy trang đầu mới. Không dùng cursor cũ như một session khóa thẻ.
Không có claim/reservation: hai tab có thể thấy cùng thẻ, expected version sẽ bảo vệ lúc ghi.

Thẻ bị ẩn khi entry unpublish, language tắt hoặc ancestry không còn khớp. Detail cũng trả 404.
Không xóa tiến độ/historical reviews; khi nội dung khả dụng lại, lịch trước đó vẫn giữ nguyên.
Profile hợp lệ nhưng không có thẻ/nội dung khả dụng trả 200 danh sách rỗng.

## 3. Tổng quan

```http
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning/summary
```

```ts
type Summary = {
  learningProfileId: string;
  total: number;
  new: number;
  learning: number;
  reviewing: number;
  due: number;
  nextFutureReviewAt: string | null;
  asOf: string;
};
```

Chỉ đếm thẻ đã thêm và đang khả dụng, không đếm toàn bộ catalog. Due gồm các thẻ New đến hạn ngay.
NextFutureReviewAt là min hạn **lớn hơn AsOf**, null nếu không có lịch tương lai. Không trả hạn quá khứ
dưới nhãn lịch tương lai. Profile chưa thêm thẻ trả đủ các count=0, không phải 404.
Backend aggregate bằng SQL, không tải toàn bộ progress để đếm trong RAM.

## 4. Ghi review và thuật toán simple-v1

Sau khi người dùng lật đáp án, gửi một trong Again (Quên), Hard (Khó), Good (Nhớ), Easy (Dễ):

```http
POST /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning/PROGRESS_ID/reviews
Authorization: Bearer ACCESS_TOKEN
Content-Type: application/json

{
  "requestId": "44c73d27-5395-4dfe-bc7c-4c167e9a0d45",
  "version": 1,
  "rating": "Good"
}
```

RequestId là UUID khác rỗng, do FE tạo **một lần cho một lần tự đánh giá**. Version lấy từ Progress mới nhất.
Rating phải là chuỗi enum, không nhận số enum. Thiếu trường, UUID sai, quoted version, trường lạ như intervalMinutes,
nextReviewAt/schedulerVersion/userId đều bị từ chối. JSON body tối đa 4 KiB.
Clock server quyết định ReviewedAt; không nhận thời gian hoặc lịch do client gửi.

Policy `simple-v1`:

| Rating | Lần đầu | Các lần tiếp theo |
| --- | --- | --- |
| Again | 1 phút | Luôn 1 phút, chuyển Learning, tăng AgainCount và xóa streak |
| Hard | 10 phút | max(10, ceil(interval cũ × 1.2)) phút |
| Good | 1 ngày | max(1 ngày, interval cũ × 2) |
| Easy | 3 ngày | max(3 ngày, interval cũ × 3) |

Mọi interval tối đa **525600 phút (365 ngày)**. Dưới 1 ngày là Learning; từ 1 ngày là Reviewing.
Hard/Good/Easy tăng chuỗi nhớ liên tiếp. AgainCount tính mọi lần chọn Again, không suy diễn mastery.
NextReviewAt = ReviewedAt + interval mới, kể cả ôn sớm hoặc ôn muộn.
Scheduler thuần, không HTTP/EF/AI, không cộng interval vào hạn cũ, không gọi random/clock bên trong.
Đây là policy heuristic ban đầu, **không gọi là SM-2/FSRS hoặc xác suất nhớ đã được hiệu chỉnh**.
Đổi threshold/công thức phải cấp scheduler version mới; review đã ghi giữ nguyên lịch/version cũ.

Thành công và replay đều trả **200 OK**, một immutable Review receipt:

```ts
type Review = {
  id: string;
  progressId: string;
  requestId: string;
  reviewNumber: number;
  rating: "Again" | "Hard" | "Good" | "Easy";
  previousStatus: "New" | "Learning" | "Reviewing";
  status: "Learning" | "Reviewing";
  previousDueAt: string;
  nextReviewAt: string;
  previousIntervalMinutes: number;
  intervalMinutes: number;
  againCount: number;
  consecutiveRecallCount: number;
  progressVersionBefore: number;
  progressVersionAfter: number;
  schedulerVersion: string;
  reviewedAt: string;
};
```

Không GET lại trong POST để dựng response. ReviewNumber là tổng số lần ôn sau thao tác, còn
ProgressVersionBefore/After bảo vệ mutation root. Không sửa Version của profile hay module khác.

### Quy tắc retry

- Cùng progress + RequestId + rating + version: trả đúng review cũ, không tăng counter hay dời lịch lần nữa.
- Cùng key nhưng khác rating hoặc version: 409, kể cả khi thay đổi đó do người dùng đổi ý.
- Key mới, version cũ: 409. Không tự lấy version mới rồi âm thầm ghi lại rating cũ.
- Nhiều request cùng key chạy đồng thời: chỉ một lần ôn được ghi, các caller hợp lệ nhận cùng receipt.
- Hai key khác nhau trên cùng version: chỉ một bên thành công; bên còn lại 409.
- Receipt đã lưu vẫn replay được sau khi content unpublish; thao tác ôn mới lúc content ẩn trả 404.
- Ownership luôn kiểm tra trước, kể cả replay. Xóa profile cascade cả receipt, không thể replay receipt đã xóa.

RequestId nằm trong **body**, không dùng header Idempotency-Key của module khác.
Receipt là lịch sử bất biến: replay một review cũ sau khi đã có review mới vẫn trả event cũ,
không phải state hiện tại. FE không dùng một receipt cũ để ghi đè version mới hơn đang có trong cache.

## 5. Lịch sử

```http
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning/PROGRESS_ID/reviews?pageSize=20
GET /api/v1/learning-profiles/PROFILE_ID/vocabulary-learning/PROGRESS_ID/reviews?before=NEXT_CURSOR&pageSize=20
```

Trả `{ items: Review[], nextCursor: string | null }`. PageSize 1–100, before là reviewNumber dạng string.
Thứ tự reviewNumber DESC, không OFFSET. Thẻ chưa có review trả trang rỗng; thẻ không thuộc user trả 404.
History vẫn đọc được khi catalog unpublish/language tắt. Không tải toàn bộ history để lập lịch lần mới.
Các review ghi quyết định lịch/tự đánh giá, chưa snapshot nguyên văn flashcard/bản dịch cũ.

## Luồng FE đề xuất

1. Màn catalog gọi API vocabulary senses sẵn có. Nút “Thêm vào học” PUT theo senseId.
2. Trang học GET summary và GET queue song song. Nếu queue rỗng, hiển thị NextFutureReviewAt hoặc hướng dẫn thêm từ.
3. Mặt trước: term, tùy chọn reading/audio. Bấm lật mới hiện meaning, usageNote và POS của đúng sense.
4. Chọn rating: tạo pending submission chứa progressId, version, rating và crypto.randomUUID(). Khóa cả 4 nút khi đang gửi.
5. Sau 200, lưu receipt để phản hồi người dùng rồi invalidate queue/summary/card. Tải lại thẻ nếu cần state mới nhất.
6. Khi mất mạng/timeout, retry **chính pending submission đó**. Sau refresh accessToken, dùng lại cùng body.
7. Khi 409, giữ lại lựa chọn để giải thích nhưng tải card mới và yêu cầu người dùng xác nhận lại, không tự submit với version mới.

Ví dụ wrapper gửi review, apiBase đã bao gồm `/api/v1`:

```ts
type PendingReview = {
  requestId: string;
  version: number;
  rating: "Again" | "Hard" | "Good" | "Easy";
};

async function sendReview(
  apiBase: string, accessToken: string, profileId: string, progressId: string,
  pending: PendingReview, signal?: AbortSignal,
) {
  const response = await fetch(
    `${apiBase}/learning-profiles/${encodeURIComponent(profileId)}` +
    `/vocabulary-learning/${encodeURIComponent(progressId)}/reviews`,
    {
      method: "POST", signal,
      headers: { Authorization: `Bearer ${accessToken}`, "Content-Type": "application/json" },
      body: JSON.stringify(pending),
    },
  );
  if (!response.ok) throw Object.assign(new Error("Không ghi nhận được lần ôn"), {
    status: response.status, problem: await response.json(),
    retryAfter: response.headers.get("Retry-After"),
  });
  return response.json() as Promise<Review>;
}

// Tạo ngoài hàm retry; không tạo UUID mới bên trong sendReview.
const pending: PendingReview = {
  requestId: crypto.randomUUID(), version: card.progress.version, rating: "Good",
};
```

Giữ pending khi retry; đổi progress/profile phải tách state theo thẻ. Abort request không bảo đảm server
chưa commit, nên nếu gửi lại vẫn dùng key cũ. Không cho sửa rating của một pending chưa rõ kết quả rồi dùng lại key.
CORS, auth refresh, Location/Retry-After exposure dùng cấu hình hệ thống hiện có, không thêm domain/cookie riêng.

## Lỗi và giới hạn

| HTTP | Nghĩa |
| --- | --- |
| 400 | JSON/enum/version/UUID/cursor/locale sai hoặc tham số ngoài giới hạn |
| 401 | Thiếu/hết hạn accessToken; dùng auth flow hiện có |
| 404 | Không sở hữu profile/thẻ hoặc content mới không khả dụng |
| 409 | Version cũ, key đổi payload hoặc owner/catalog thay đổi trong lúc ghi |
| 413 / 415 | Body quá 4 KiB / sai Content-Type |
| 429 | Policy learning-profiles hiện có, tuân thủ Retry-After |

Cache-Control no-store/Pragma no-cache trên route này. Business errors là ProblemDetails + traceId;
JSON/model binding có thể là ValidationProblemDetails. Rate limiter hiện có theo IP/process,
không phải quota phân tán giữa replica. Không thêm Redis cache hoặc background job để tạo queue;
queue suy ra từ NextReviewAt và clock server ngay khi đọc.

## Kiến trúc và DB

- Api: controller mỏng, request strict; prefix/Auth/CORS/limiter sẵn có.
- Application/VocabularyLearning: commands/queries/models/validation và scheduler thuần; port tại Common/Interfaces.
- Domain: factory/RecordReview/invariant hiện có, không phụ thuộc framework.
- Infrastructure: repository ownership/projection/keyset và atomic write.
- Không dùng MediatR/dispatcher hoặc thêm NuGet package. Không thêm migration cho bước API này.

PUT dùng một INSERT SELECT kiểm tra owner/catalog và FOR SHARE source, ON CONFLICT không ghi đè progress.
POST đọc context + receipt trong một SELECT, tính lịch ngoài DB rồi dùng **một SQL statement**:
CTE giữ SHARE locks trên profile/sense/entry/language, UPDATE root với expected version, INSERT log từ
RETURNING của UPDATE. Mọi phần cùng một statement transaction: lỗi INSERT rollback cả UPDATE.
Không cần thêm BEGIN/COMMIT cho một statement nguyên tử; thêm thao tác DB độc lập sau này phải đánh giá lại boundary.
Receipt lookup sau write conflict chạy sau rollback statement, để phân biệt retry hợp lệ với stale rating.

Budget command của module, không tính kiểm tra session JWT/các module khác:

| Flow | SQL command |
| --- | --- |
| Input sai | 0 |
| PUT mới / đã có | 2 / 1 |
| PUT đụng concurrent add | Tối đa 3 |
| Queue hoặc history có kết quả | 1 |
| Queue/history rỗng cần xác minh scope | 2 |
| Card / summary | 1 |
| Review mới | 2 |
| Replay đã tồn tại | 1 |
| Write race cần đọc receipt | 3 |

Translation exact/en được chọn trong SQL, không gọi từng sense riêng. Queue lấy LIMIT trước khi dựng card;
summary aggregate trong DB, history dùng unique index progress/reviewNumber, không truy vấn cả log để chấm lịch.

## Chạy và kiểm thử

Áp migration model đã có trước khi chạy API:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
```

Các request copy được ở `src/Api/Api.http`. Test runner:

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/VocabularyLearning.Tests/bin/Debug/net10.0/VocabularyLearning.Tests.dll
```

Mặc định chạy Domain/scheduler. Đặt VOCABULARY_LEARNING_TEST_DATABASE để chạy PostgreSQL/HTTP;
chỉ chấp nhận DB **trống, biệt lập**, tên vocabulary_learning_tests. Không dùng DB dự án/production.
Fake authentication và SQL counter chỉ thuộc test host; production vẫn dùng JWT/session thật.

Đã chạy thành công **298 kiểm tra** (53 Domain, 10 scheduler, 67 EF/PostgreSQL, 168 HTTP/repository)
trên PostgreSQL 17 biệt lập. Bao gồm 8 concurrent PUT, 8 concurrent retry cùng key, hai rating khác key
tranh cùng version, exact receipt replay, rollback root khi log insert lỗi, owner/publication race và query budget.
Đây là kiểm tra tính đúng đắn, không phải chứng nhận throughput hoặc hiệu quả ghi nhớ của policy.
