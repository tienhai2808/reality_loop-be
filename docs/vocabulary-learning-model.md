# Tiến độ từ vựng và lịch sử ôn — model/DB

Phạm vi bước này: Domain entities, enum/value validation, EF mapping, constraint, index, migration và kiểm thử.
API/handler/repository và scheduler simple-v1 đã được triển khai ở bước tiếp theo; xem [vocabulary-learning-api.md](vocabulary-learning-api.md).
Tài liệu này mô tả Domain/DB; chưa có AI hoặc frontend production trong backend.
Không đổi tên bốn layer và không thay đổi module tiến độ bảng chữ/luyện viết.

## Quan hệ và hành vi

```text
User -> LearningProfile -> VocabularyLearningProgress -> VocabularyReview
VocabularyEntry -> VocabularySense --^ (RESTRICT)
```

Tiến độ theo **từng nghĩa** (VocabularySense), không gộp tất cả nghĩa của một từ.
Không có progress row nghĩa là user chưa thêm nghĩa đó vào danh sách học. Thêm vào tạo thẻ New,
đến hạn ngay, không đánh dấu là đã học/đã nhớ. Không seed tiến độ hoặc review của user.

Các trạng thái New/Learning/Reviewing chỉ mô tả vòng ôn, không phải trình độ, mastery hay kết quả kiểm tra.
Rating là mức **tự đánh giá sau khi lật thẻ**: Again (Quên), Hard (Khó), Good (Nhớ), Easy (Dễ).
Không dùng kết quả viết chữ để tự tăng tiến độ này hoặc sửa Version của LearningProfile.

## VocabularyLearningProgress — vocabulary_learning_progress

| Field | Kiểu DB | Vai trò |
| --- | --- | --- |
| Id | bigint PK | Snowflake do caller cấp |
| LearningProfileId | bigint FK | Chủ sở hữu suy ra qua profile.UserId, không lặp UserId |
| VocabularySenseId | bigint FK | Đúng nghĩa đang học; immutable |
| Status | varchar(32) | New, Learning hoặc Reviewing |
| NextReviewAt | timestamptz | Hạn ôn; New khởi tạo bằng CreatedAt |
| LastReviewedAt | timestamptz NULL | Null trước lần ôn đầu tiên |
| IntervalMinutes | integer | Khoảng cách từ lần ôn gần nhất đến NextReviewAt; New = 0 |
| ReviewCount | bigint | Số lần tự đánh giá đã ghi nhận |
| AgainCount | bigint | Tổng số lần chọn Again, bao gồm lần đầu; không gọi đây là số lần mất mastery |
| ConsecutiveRecallCount | bigint | Chuỗi Hard/Good/Easy liên tiếp; Again đặt về 0 |
| SchedulerVersion | varchar(64) NULL | Phiên bản thuật toán/policy tạo lịch gần nhất; New = null |
| Version | bigint | Optimistic concurrency, bắt đầu 1 |
| CreatedAt | timestamptz | Thời điểm thêm vào danh sách học, không thay đổi |
| UpdatedAt | timestamptz | Thời điểm cập nhật gần nhất |

Các cột dùng snake_case theo tên field, ví dụ learning_profile_id, next_review_at.

`Create(id, profile, sense, entry, language, now)` kiểm tra dương/null và ancestry
sense -> entry -> language == profile.TargetLanguage. Entry phải publish, language phải bật.
Target sense/profile không thể đổi sau tạo. Timestamp chuẩn hóa UTC.

`RecordReview(reviewId, requestId, rating, schedule, now)`:

1. Kiểm tra ID, UUID khác rỗng, enum và schedule. Thời gian không được lùi trước UpdatedAt.
2. Tính toàn bộ lịch/counter/version bằng checked arithmetic trước khi sửa state.
3. Tạo review immutable chụp state trước/sau, tăng ReviewCount/Version một lần.
4. Again tăng AgainCount, xóa streak và yêu cầu trạng thái Learning. Hard/Good/Easy tăng streak.
5. LastReviewedAt = UpdatedAt = thời điểm ôn; NextReviewAt = thời điểm ôn + IntervalMinutes.
6. Trả review để caller ghi cùng root trong một transaction.

Có thể ôn sớm: now không bắt buộc >= hạn cũ. Lịch mới tính từ thời điểm ôn thật, không từ hạn cũ.
Không có Reset/Suspend/Delete hay setter công khai ở bước này. Không tải toàn bộ lịch sử vào aggregate
để ghi một lần ôn; root không có navigation collection tăng vô hạn.

## VocabularyReview — vocabulary_reviews

| Field | Kiểu DB | Vai trò |
| --- | --- | --- |
| Id | bigint PK | Snowflake của lần ôn |
| ProgressId | bigint FK | Progress sở hữu review; profile/user/sense suy ra qua progress |
| RequestId | uuid | Khóa chống ghi lại cùng request, scope theo progress |
| ReviewNumber | bigint | Thứ tự lần ôn, bắt đầu 1 |
| Rating | varchar(32) | Again, Hard, Good, Easy |
| PreviousStatus / Status | varchar(32) | Trạng thái trước/sau lần ôn |
| PreviousDueAt / NextReviewAt | timestamptz | Hạn ôn trước/sau |
| PreviousIntervalMinutes / IntervalMinutes | integer | Khoảng cách ôn trước/sau |
| AgainCount / ConsecutiveRecallCount | bigint | Counter sau lần ôn, để kiểm tra lịch sử |
| ProgressVersionBefore / ProgressVersionAfter | bigint | Version root trước/sau, phải tăng đúng 1 |
| SchedulerVersion | varchar(64) | Policy tạo lịch của chính lần ôn đó |
| ReviewedAt | timestamptz | Thời điểm user tự đánh giá, do server cung cấp |
| Version | bigint | Luôn 1 vì review immutable |
| CreatedAt / UpdatedAt | timestamptz | Đều bằng ReviewedAt |

Chỉ root gọi factory internal của review. Rating không sửa sau khi ghi; thao tác ôn tiếp là record mới.
ReviewNumber và ProgressVersion có vai trò riêng: version bảo vệ mutation root, number sắp thứ tự ôn.
History lưu quyết định/lịch, không snapshot nội dung câu hỏi hoặc bản dịch; chưa tuyên bố replay được
nguyên văn flashcard cũ nếu catalog được chỉnh sửa.

## VocabularyReviewSchedule

Value object immutable, chưa persist thành bảng: Status (Learning/Reviewing), IntervalMinutes (>0),
SchedulerVersion (code lowercase ASCII, tối đa 64 ký tự, ví dụ simple-v1).
Status không được New sau review; Again chỉ chấp nhận schedule Learning.

Value object này kiểm tra **quyết định lịch**, không tự tính lịch. Application hiện có policy simple-v1
được mô tả ở tài liệu API; không gọi đó là SM-2/FSRS và không cài package scheduler. Chưa có ease factor/stability/difficulty cho thuật toán cụ thể.
Khi triển khai scheduler tiếp theo, dùng interval/counters này cho policy đã chọn, hoặc thêm state cần thiết
bằng migration nếu chọn thuật toán cần thêm dữ liệu. Không dựng lại toàn bộ history trên mỗi request.
Chỉ backend scheduler được tạo schedule; FE sau này chỉ gửi rating và request/version.

## Mapping, constraints và index

- bigint IDs ValueGeneratedNever; Version concurrency token, default 1, ValueGeneratedNever.
- Enum lưu **nguyên tên** dạng varchar(32) + CHECK allowed values. Không lưu số enum.
- DateTimeOffset lưu timestamptz, required theo bảng trên; CreatedAt có SQL default now() như codebase.
- UNIQUE `(learning_profile_id, vocabulary_sense_id)`: một progress/nghĩa/profile.
- Index `(learning_profile_id, next_review_at, id)`: hàng đợi đến hạn, thứ tự ổn định và cursor tương lai.
- Index sense ID phục vụ FK; không thêm standalone profile index vì composite đã phủ prefix.
- UNIQUE `(progress_id, request_id)`: chặn replay cùng request trên cùng thẻ.
- UNIQUE `(progress_id, review_number DESC)`: trình tự/lịch sử, không trùng số lần ôn.
- UNIQUE `(progress_id, progress_version_before)`: không ghi hai kết quả cho cùng version root.
- Không thêm index riêng progress_id vì các unique index đã phủ FK prefix. Tổng 6 index mới.
- Profile -> progress -> reviews: CASCADE khi user xóa profile.
- Sense -> progress: RESTRICT để xóa catalog không làm mất lịch sử. Unpublish entry thay vì xóa sense đang học.

CHECK bảo vệ ID/version/counter dương hoặc không âm, AgainCount + streak <= ReviewCount;
version > ReviewCount. Trạng thái New bắt buộc toàn bộ counters/interval ban đầu và các null đúng.
Thẻ đã review bắt buộc LastReviewedAt = UpdatedAt >= CreatedAt, lịch khớp đúng interval dương,
SchedulerVersion hợp lệ và không null. Review kiểm tra rating/status, số thứ tự, state trước lần đầu,
Again -> Learning/streak 0, rating còn lại -> streak dương, lịch và hai version tiến đúng 1.

CHECK không truy vấn xuyên nhiều hàng: ancestry/ngôn ngữ, số reviews bằng ReviewCount, chuỗi snapshot
và kết quả của thuật toán được bảo vệ bằng domain + protocol ghi của application. Không thêm trigger
tự tính lịch, tự append lịch sử hoặc cấm mọi SQL UPDATE. Ghi SQL trực tiếp phải giữ invariant này.

## Quy tắc tích hợp application/API

Use case hiện tại thực hiện các quy tắc sau:

1. Lọc progress qua authenticated user -> profile, không tin UserId/ProfileId từ body.
2. Lookup RequestId trước xử lý: replay cùng input trả kết quả cũ; cùng key khác input trả conflict.
3. Kiểm tra expected Version của root, lấy scheduler decision, gọi RecordReview một lần.
4. Update root bằng CAS version và INSERT review trong cùng SQL statement transaction. Repository dùng
   CTE + UPDATE RETURNING + INSERT; lỗi phần log rollback cả phần root, không mở transaction lúc user xem thẻ.
5. Unique/CAS conflict phải rollback cả hai phần. Nếu request trùng/race, reload receipt/current root trước
   khi quyết định response, không tăng counter thêm và không retry mutation trên state tracker đã lỗi.

Unique request key là hàng rào DB; API phối hợp receipt lookup và CAS để thực hiện idempotency. Domain không giữ mọi request
cũ trong RAM; application truy vấn bằng index để xử lý replay. Đổi RequestId tạo lần ôn mới hợp lệ.
Không mở transaction trong lúc FE đang xem mặt trước/sau của thẻ. Queue phân trang root theo profile + due
rồi ghép metadata/bản dịch để dựng thẻ; chỉ đọc review history khi cần lịch sử/audit.

## Migration và kiểm chứng

Migration: **20261105023639_AddVocabularyLearningProgress**. Up thêm hai bảng, 16 CHECK, 3 FK và 6 index;
không thay dữ liệu catalog, auth, learning profile hoặc handwriting. Không seed dữ liệu cá nhân.
Down xóa reviews rồi progress. Chưa áp vào DB chính của dự án.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/VocabularyLearning.Tests/bin/Debug/net10.0/VocabularyLearning.Tests.dll
```

Console runner không thêm package test. Mặc định chạy Domain checks; muốn chạy EF/PostgreSQL đặt
VOCABULARY_LEARNING_TEST_DATABASE là connection string của DB **trống, biệt lập**, bắt buộc tên
vocabulary_learning_tests. Runner từ chối DB có bảng/sai tên, không tự truncate hay xóa dữ liệu sẵn có.

Đã qua **120 kiểm tra** (53 Domain + 67 EF/PostgreSQL) trên PostgreSQL 17: enum/UTC/ancestry,
counter/overflow/mutation atomic, direct SQL constraints, unique, replay rollback, stale concurrent review,
cascade/restrict, upgrade bảo toàn user/catalog, down/up và model khớp snapshot.
