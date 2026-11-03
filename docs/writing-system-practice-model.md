# Luyện tập bảng chữ: Domain và DB

## Phạm vi

Module `WritingSystemPractice` hỗ trợ lượt luyện **nhìn chữ → chọn cách đọc** bằng
catalog hiện có. Tài liệu này mô tả Domain, EF mappings, DbSet, migration và invariant.
API, handler, repository và bộ chọn đề đã được bổ sung ở bước sau;
xem [API và luồng tích hợp FE](writing-system-practice-api.md). Chưa triển khai frontend hoặc AI.

Giữ nguyên bốn layer và cách chia feature. Không thêm NuGet package.
Kết quả luyện lưu riêng với [tiến độ học bảng chữ](learning-progress-model.md):
trả lời đúng/sai không tự tạo progress, không chuyển progress sang `Completed`,
không sửa `LearningProfile.Version`.

## Các type

- `WritingSystemPracticeSession`: aggregate root, đại diện một lượt luyện của một profile.
- `WritingSystemPracticeSessionItem`: câu hỏi cùng câu trả lời duy nhất trong lượt đó.
- `WritingSystemPracticeSessionStatus`: `InProgress`, `Completed`, `Abandoned`;
  persist tên enum thành `varchar(32)` và có CHECK danh sách giá trị.
- `WritingSystemPracticeQuestion`: input bất biến để tạo aggregate; **không phải bảng thứ ba**.
  Factory nhận item, cách đọc đúng, toàn bộ cách đọc của item và các lựa chọn đã sắp thứ tự.
  Nó chụp nội dung và kiểm tra đề trước khi tạo session item.

Entity kế thừa `Domain.Common.Entity`, constructor private, không có setter công khai.
Caller cung cấp Snowflake ID thông qua `IIdGenerator` hiện có cho lượt và từng câu;
Domain không tự đọc clock, sinh ID, random, truy cập DB hay gọi dịch vụ ngoài.

## `writing_system_practice_sessions`

| Field / column | Kiểu DB | Mục đích |
| --- | --- | --- |
| `Id` / `id` | bigint, PK | Snowflake ID, dương, `ValueGeneratedNever` |
| `LearningProfileId` / `learning_profile_id` | bigint, NOT NULL | Profile sở hữu lượt; user được xác định qua profile |
| `WritingSystemId` / `writing_system_id` | bigint, NOT NULL | Bộ chữ được luyện trong lượt |
| `QuestionCount` / `question_count` | integer, NOT NULL | Số câu cố định, 1–20; cũng phát hiện navigation chưa được tải đủ khi chấm |
| `Status` / `status` | varchar(32), NOT NULL | Mặc định `InProgress` |
| `EndedAt` / `ended_at` | timestamptz, NULL | Thời điểm hoàn thành hoặc bỏ lượt; null khi đang luyện |
| `Version` / `version` | bigint, NOT NULL | Bắt đầu 1, concurrency token, tăng khi có câu trả lời mới hoặc bỏ lượt |
| `CreatedAt` / `created_at` | timestamptz, NOT NULL | Bắt đầu lượt; không thêm StartedAt trùng nghĩa |
| `UpdatedAt` / `updated_at` | timestamptz, NOT NULL | Lần chuyển state gần nhất |

FK và index:

- `learning_profile_id → learning_profiles(id) ON DELETE CASCADE`.
- `writing_system_id → writing_systems(id) ON DELETE RESTRICT`.
- `ix_ws_practice_sessions_profile_id_id (learning_profile_id ASC, id DESC)`:
  phục vụ lịch sử theo profile, cursor ID và cascade. Không thêm index profile đơn lẻ.
- `ix_ws_practice_sessions_writing_system_id`: hỗ trợ FK khi xóa catalog.

CHECK bảo vệ ID dương, 1–20 câu, version dương, enum hợp lệ, `UpdatedAt >= CreatedAt`.
`InProgress` bắt buộc `EndedAt IS NULL`; `Completed`/`Abandoned` bắt buộc có EndedAt,
`CreatedAt <= EndedAt = UpdatedAt`.

Không giới hạn một lượt đang luyện trên mỗi profile: nhiều thiết bị có thể mở lượt riêng.
Chưa có yêu cầu resume một lượt duy nhất, vì vậy không đặt partial unique index tùy tiện.

## `writing_system_practice_session_items`

| Field / column | Kiểu DB | Mục đích |
| --- | --- | --- |
| `Id` / `id` | bigint, PK | Snowflake ID của câu trong lượt |
| `SessionId` / `session_id` | bigint, NOT NULL | Lượt sở hữu câu |
| `WritingSystemItemId` / `writing_system_item_id` | bigint, NOT NULL | Tham chiếu chữ nguồn để xem lại, thống kê sau này |
| `Position` / `position` | integer, NOT NULL | Thứ tự cố định, bắt đầu 0, nhỏ hơn 20 |
| `Symbol` / `symbol` | varchar(64), NOT NULL | Bản chụp chữ được hỏi |
| `TranscriptionSystem` / `transcription_system` | varchar(64), NOT NULL | Bản chụp hệ phiên âm; phân biệt các cách biểu diễn khác hệ |
| `Options` / `options` | varchar(200)[], NOT NULL | 2–4 lựa chọn theo đúng thứ tự đã phát |
| `CorrectOptionIndex` / `correct_option_index` | integer, NOT NULL | Chỉ số đáp án chuẩn, bắt đầu 0 |
| `SelectedOptionIndex` / `selected_option_index` | integer, NULL | Lựa chọn đã nộp; null nghĩa là chưa trả lời |
| `AnsweredAt` / `answered_at` | timestamptz, NULL | Thời điểm server ghi nhận câu trả lời đầu tiên |
| `Version` / `version` | bigint, NOT NULL | 1 khi chưa trả lời, 2 khi đã trả lời; concurrency token |
| `CreatedAt` / `created_at` | timestamptz, NOT NULL | Cùng lúc tạo lượt |
| `UpdatedAt` / `updated_at` | timestamptz, NOT NULL | Ban đầu bằng CreatedAt, sau đó bằng AnsweredAt |

`IsCorrect` là property tính toán nullable, không có cột DB:
null nếu chưa trả lời, ngược lại so sánh SelectedOptionIndex với CorrectOptionIndex.
Số đúng/sai và điểm được suy ra từ câu trả lời; không lưu thêm counter để tránh lệch dữ liệu.

Options dùng mảng PostgreSQL thay vì thêm bảng option hoặc serialize JSON tùy ý.
EF truy cập backing field `_options`; getter trả bản sao để caller không đổi được đề.
Danh sách Items dùng backing field `_items`, bọc read-only theo convention navigation hiện có.

FK và unique:

- `session_id → writing_system_practice_sessions(id) ON DELETE CASCADE`.
- `writing_system_item_id → writing_system_items(id) ON DELETE RESTRICT`.
- UNIQUE `(session_id, position)`: mỗi vị trí có đúng tối đa một câu.
- UNIQUE `(session_id, writing_system_item_id)`: không lặp cùng chữ trong một lượt.
- Index `writing_system_item_id` cho FK; không thêm index session đơn lẻ vì unique index đã phủ prefix.

CHECK bảo vệ:

- ID dương, vị trí 0–19, version dương, symbol không rỗng, transcription system đúng định dạng code.
- Options là mảng một chiều, lower bound 1 theo PostgreSQL, có 2–4 phần tử không null;
  mỗi phần tử có tối đa 200 ký tự, không toàn whitespace, các phần tử không trùng chính xác.
- CorrectOptionIndex trong khoảng `[0, số lựa chọn - 1]`.
- Chưa trả lời: cả SelectedOptionIndex/AnsweredAt đều null, Version 1 và UpdatedAt = CreatedAt.
- Đã trả lời: cả hai đều có giá trị, chỉ số hợp lệ, Version 2,
  `CreatedAt <= AnsweredAt = UpdatedAt`.

Chỉ số lựa chọn trong Domain là **zero-based**; chỉ số trong biểu thức SQL kiểm tra mảng là
**one-based** theo PostgreSQL. Client tương lai gửi chỉ số zero-based.

## Tạo đề và hành vi Domain

```csharp
var question = WritingSystemPracticeQuestion.Create(
    questionId, item, correctReading, allReadingsForItem, orderedOptions);

var session = WritingSystemPracticeSession.Create(
    sessionId, profile, writingSystem, language, questions, now);

session.Answer(sessionItemId, selectedOptionIndex, now);
session.Abandon(now);
```

`Question.Create` trim và chuẩn hóa NFC từng lựa chọn, từ chối blank/NUL/quá dài,
trùng sau chuẩn hóa với so sánh OrdinalIgnoreCase. Đáp án đúng phải thuộc item và xuất hiện
trong tập cách đọc được cung cấp. Factory tự xác định CorrectOptionIndex, không tin chỉ số
đúng do caller tự khai. Trong cùng hệ phiên âm chỉ được có một lựa chọn là cách đọc hợp lệ
của chữ đó; một cách đọc thay thế hợp lệ không được dùng làm đáp án nhiễu.

Caller phải tải **đầy đủ** cách đọc của item từ DB. Domain không thể phát hiện
những cách đọc bị cố ý bỏ khỏi input. Cũng không tự xác nhận distractor thuộc catalog;
use case sinh đề hiện tại chọn distractor từ các chữ khác trong cùng bộ chữ/hệ phiên âm.

`Session.Create` xác nhận system → language → target language của profile, language enabled,
system published, 1–20 câu, ID câu không trùng, các chữ không trùng và cùng bộ chữ.
Thứ tự câu được gán theo input. Mọi nội dung câu được chụp ngay khi tạo.
Thay symbol/reading trong catalog sau đó không sửa đề hoặc kết quả cũ.

`Answer`:

1. Kiểm tra thời gian không đi lùi, aggregate đã tải đủ câu, câu thuộc lượt và lựa chọn hợp lệ.
2. Cùng câu/cùng lựa chọn đã ghi nhận: no-op, giữ Version và AnsweredAt cũ,
   kể cả khi lượt đã kết thúc. Cùng câu/khác lựa chọn: từ chối.
3. Chỉ lượt InProgress nhận câu trả lời mới. Câu tăng Version 1 → 2;
   lượt tăng Version một lần và cập nhật UpdatedAt.
4. Khi tất cả câu đã trả lời, tự chuyển Completed và đặt EndedAt trong cùng thay đổi.

Không bắt buộc trả lời theo thứ tự; Position thể hiện thứ tự hiển thị.
Không có retry ghi đè đáp án, xóa riêng câu trả lời hoặc chuyển lượt đã kết thúc về InProgress.
Muốn luyện lại thì tạo lượt mới.

`Abandon` giữ kết quả đã trả lời, đặt EndedAt/UpdatedAt và tăng Version.
Bỏ lại lượt đã Abandoned là no-op; không được bỏ lượt đã Completed.
Câu chưa trả lời của lượt bỏ dở vẫn chưa trả lời, không tự tính thành câu sai.

Timestamps chuẩn hóa UTC. Mọi version increment dùng `checked` trước khi thay state;
timestamp/option sai hoặc overflow không để lại mutation dở.

## Ranh giới toàn vẹn và tích hợp Application/API

DB bảo vệ FK tồn tại, unique và invariant từng dòng. Các quy tắc qua nhiều dòng như
QuestionCount khớp số câu thực tế, position liên tục, cùng bộ chữ/target language,
Completed chỉ khi trả lời đủ được aggregate kiểm tra. Không dùng CHECK truy vấn bảng khác,
trigger hoặc sửa thêm catalog để giả định đã bảo vệ những quy tắc này ở DB.

FK không thay thế authorization. API lọc theo `LearningProfile.UserId`
của caller, không nhận UserId từ FE và không tin profile thuộc người gọi chỉ vì FK hợp lệ.
Đáp án chuẩn thuộc dữ liệu nội bộ: **không serialize trực tiếp entity/question ra FE**;
DTO chỉ công bố CorrectOptionIndex/kết quả sau khi đã chấm câu tương ứng.

Concurrency tokens đã được cấu hình. API so expected Version của FE trước khi gọi Domain,
repository kiểm tra lại bằng conditional updates có owner/id/version/state, trả 409 nếu xung đột.
Mỗi câu trả lời mới cập nhật **cả session và item** trong cùng transaction READ COMMITTED;
lỗi ở conditional update bất kỳ rollback toàn bộ thay đổi. Aggregate được tải đầy đủ, tối đa 20 câu.
Tạo lượt dùng transaction cho conditional insert session và batch insert câu hỏi;
owner/target language/publication được kiểm tra lại ngay lúc insert.
Abandon chỉ cập nhật một dòng bằng conditional UPDATE nên không mở transaction riêng.

Hai thiết bị trả lời hai câu khác nhau của cùng lượt vẫn cạnh tranh Version của session;
writer thua phải tải lại aggregate trước khi xử lý tiếp. No-op không phát lệnh update;
repository vẫn kiểm tra owner/version/state hiện tại bằng EXISTS để từ chối snapshot cũ.
Raw SQL/ExecuteUpdate không tự áp dụng concurrency token hoặc invariant Domain;
writer tương lai phải tự có điều kiện owner/version/state và transaction tương đương.
CHECK không cấm được mọi chuyển trạng thái sai hay ghi đè lịch sử bằng SQL tùy ý.

Xóa profile/user xóa lịch sử cá nhân tương ứng. Xóa session xóa câu của session.
Xóa cứng catalog đang có lịch sử bị RESTRICT; dùng IsPublished/IsEnabled để ẩn nội dung.
Chưa thêm expiry, active-session singleton, lịch ôn tập, mastery, streak hoặc AI.

## Migration và kiểm thử

Migration: `20261103121729_AddWritingSystemPracticeSessions`.
Chỉ tạo hai bảng, FK/CHECK và năm index; không sửa seed hay bảng hiện có.
Down xóa items trước, sessions sau. Không tự migrate khi API startup.

Áp migration vào môi trường của bạn bằng cấu hình connection string hiện có:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Console runner `tests/WritingSystemPractice.Tests` theo cách tổ chức test hiện có,
không thêm test package. Build và chạy Domain checks:

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/WritingSystemPractice.Tests/bin/Debug/net10.0/WritingSystemPractice.Tests.dll
```

Để chạy thêm PostgreSQL checks, đặt `WRITING_SYSTEM_PRACTICE_TEST_DATABASE` thành connection
string của DB **trống, biệt lập**, tên bắt buộc `writing_system_practice_tests`, rồi chạy lại
DLL trên. Runner từ chối DB sai tên/đã có bảng; không truncate hoặc xóa dữ liệu sẵn có.
Runner áp migrations, kiểm tra down/up, chạy thêm HTTP thật qua Kestrel, ghi dữ liệu thử
và giữ lại DB để kiểm tra sau chạy.
Không dùng connection string DB dự án/production cho runner.

Kiểm thử bao gồm state transitions, snapshot/array bất biến, chuẩn hóa/đề nhập nhằng,
retry, timestamp/version overflow, metadata EF, native array round-trip, CHECK/FK/unique,
partial navigation, cascade/restrict và hai EF contexts cạnh tranh Version với rollback
toàn bộ thay đổi của writer thua.

Bước model chạy thành công **135 kiểm tra**, gồm 56 Domain checks và 79 EF/PostgreSQL checks,
trên PostgreSQL 17 trong container tạm. Migration áp trên DB trống, rollback rồi áp lại
thành công; runtime model khớp snapshot. Build toàn solution thành công, 0 warning/0 error
ở lần xác minh cuối. Container kiểm thử được dọn sau khi chạy; chưa áp migration vào DB dự án.

Bước API mở rộng runner thành **329 kiểm tra** Domain/EF/PostgreSQL/HTTP;
xem [luồng API, query budget và kiểm thử](writing-system-practice-api.md).
