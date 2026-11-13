# Luyện ngữ pháp

## Phạm vi triển khai

Module `GrammarPractice` bổ sung Domain, EF Core mapping, migration và dữ liệu câu hỏi
cho bài luyện điền một chỗ trống bằng lựa chọn. Nội dung gắn với `GrammarPoint` hiện có,
phiên luyện thuộc `LearningProfile`. Cấu trúc giữ bốn layer của dự án.

Application, repository và API đã được triển khai ở bước tiếp theo.
Luồng endpoint và ghép FE được mô tả trong [tài liệu API luyện ngữ pháp](grammar-practice-api.md).

## Domain và quy ước dữ liệu

- `GrammarExercise` quản lý câu hỏi nguồn bằng `Create` và `Update`.
- `GrammarExerciseTranslation` quản lý hướng dẫn và giải thích theo locale bằng `Create` và `Update`.
- `GrammarPracticeSession` là aggregate root, tạo lượt bằng `Create`, ghi đáp án bằng `Answer`, kết thúc sớm bằng `Abandon`.
- `GrammarPracticeSessionItem` lưu bản chụp câu hỏi và câu trả lời, được thay đổi thông qua aggregate root.
- `GrammarPracticeQuestion` là input bất biến đã xác thực để tạo lượt. Factory nhận exercise,
  translation, grammar point, language và hoán vị lựa chọn rồi xác định lại chỉ số đáp án đúng.
- `GrammarPracticeSessionStatus` gồm `InProgress`, `Completed`, `Abandoned`.
  EF persist tên enum thành `varchar(32)` và CHECK các giá trị hợp lệ.

Các entity kế thừa `Entity`, có constructor private và setter được kiểm soát.
Caller cấp Snowflake ID qua hệ thống sinh ID hiện có. EF dùng `ValueGeneratedNever`.
`Version` bắt đầu 1, là concurrency token và tăng bằng phép tính có kiểm tra overflow.
Timestamp dùng UTC `DateTimeOffset`, persist `timestamptz`, có CHECK `updated_at >= created_at`.
Domain nhận thời gian từ caller và từ chối cập nhật có thời gian đi lùi.

Text được trim, chuẩn hóa NFC và kiểm tra giới hạn bằng validation dùng chung.
Locale dùng language tag chuẩn hóa lowercase, dài tối đa 35.

## Bảng `grammar_exercises`

| Field | Kiểu DB | Ý nghĩa |
| --- | --- | --- |
| `Id` | bigint PK | ID câu hỏi nguồn |
| `GrammarPointId` | bigint | Điểm ngữ pháp của câu hỏi, cố định sau tạo |
| `Code` | varchar(64) | Mã ổn định trong một điểm ngữ pháp, cố định sau tạo |
| `Prompt` | varchar(1000) | Câu có đúng một token `{blank}` |
| `Options` | varchar(250)[] | 2–4 lựa chọn theo thứ tự của câu hỏi nguồn |
| `CorrectOptionIndex` | integer | Chỉ số đáp án đúng, bắt đầu 0 |
| `IsPublished` | boolean | Cho phép dùng câu hỏi trong lượt luyện mới |
| `SortOrder` | integer | Thứ tự nội dung, không âm |
| `Version` | bigint | Revision câu hỏi và concurrency token |
| `CreatedAt`, `UpdatedAt` | timestamptz | Thời điểm tạo và cập nhật |

FK tới `grammar_points` dùng CASCADE. UNIQUE `(grammar_point_id, code)` bảo vệ mã.
Index `(grammar_point_id, sort_order, id)` hỗ trợ đọc câu hỏi theo điểm ngữ pháp.
Alternate key `(id, grammar_point_id)` phục vụ composite FK từ câu hỏi trong lịch sử.

CHECK bảo vệ ID/version dương, code hợp lệ, thứ tự, timestamp, token chỗ trống,
chỉ số đáp án và mảng lựa chọn. Mảng phải một chiều, lower bound 1, có 2–4 phần tử
không null, không blank và khác nhau. Domain kiểm tra trùng sau chuẩn hóa NFC/trim.
So sánh lựa chọn phân biệt hoa thường để luyện được `Sie`/`sie` và viết hoa danh từ tiếng Đức.
Getter `Options` trả bản sao, EF đọc backing field để caller không sửa nội dung qua tham chiếu mảng.

## Bảng `grammar_exercise_translations`

| Field | Kiểu DB | Ý nghĩa |
| --- | --- | --- |
| `Id` | bigint PK | ID bản dịch |
| `GrammarExerciseId` | bigint | Câu hỏi được dịch, cố định sau tạo |
| `Locale` | varchar(35) | Ngôn ngữ hướng dẫn và giải thích, cố định sau tạo |
| `Instruction` | varchar(500) | Yêu cầu làm bài, bắt buộc có nội dung |
| `Explanation` | varchar(4000) | Giải thích đáp án, bắt buộc có nội dung |
| `Version` | bigint | Revision bản dịch và concurrency token |
| `CreatedAt`, `UpdatedAt` | timestamptz | Thời điểm tạo và cập nhật |

FK tới exercise dùng CASCADE. UNIQUE `(grammar_exercise_id, locale)` đồng thời phủ
prefix cho việc tìm bản dịch. CHECK bảo vệ ID/version, locale, text và timestamp.

## Bảng `grammar_practice_sessions`

| Field | Kiểu DB | Ý nghĩa |
| --- | --- | --- |
| `Id` | bigint PK | ID lượt luyện |
| `LearningProfileId` | bigint | Profile sở hữu lượt |
| `LanguageId` | bigint | Ngôn ngữ đang luyện |
| `GrammarPointId` | bigint nullable | Phạm vi một điểm cụ thể, null khi luyện tổng hợp |
| `ExplanationLanguage` | varchar(35) | Ngôn ngữ giải thích được yêu cầu từ profile lúc tạo |
| `QuestionCount` | integer | Số câu cố định trong khoảng 5–10 |
| `Status` | varchar(32) | Trạng thái lượt luyện |
| `EndedAt` | timestamptz nullable | Thời điểm hoàn thành hoặc bỏ lượt |
| `Version` | bigint | Concurrency token của aggregate |
| `CreatedAt`, `UpdatedAt` | timestamptz | Thời điểm tạo và chuyển trạng thái gần nhất |

FK profile dùng CASCADE. FK language và điểm ngữ pháp tùy chọn dùng RESTRICT.
Index `(learning_profile_id ASC, id DESC)` phục vụ lịch sử/cursor và FK profile.
Các FK catalog có index tương ứng.

CHECK bảo vệ ID/version, số câu, locale, enum và timestamp. Lượt `InProgress` có
`EndedAt` null. Lượt `Completed` hoặc `Abandoned` phải có `CreatedAt <= EndedAt = UpdatedAt`.

Factory xác nhận ngôn ngữ đang bật và khớp target language của profile, điểm ngữ pháp
được xuất bản và cùng ngôn ngữ, các câu thuộc đúng phạm vi, ID không trùng và exercise
không lặp. Thời điểm tạo lượt phải từ lần cập nhật nguồn gần nhất trở đi.

## Bảng `grammar_practice_session_items`

| Field | Kiểu DB | Ý nghĩa |
| --- | --- | --- |
| `Id` | bigint PK | ID câu trong lượt |
| `SessionId` | bigint | Lượt sở hữu câu |
| `GrammarExerciseId`, `GrammarPointId` | bigint | Nguồn câu hỏi và điểm ngữ pháp tương ứng |
| `ExerciseVersion`, `TranslationVersion` | bigint | Revision nguồn khi tạo lượt |
| `TranslationLocale` | varchar(35) | Locale thực tế được dùng cho câu hỏi |
| `Position` | integer | Vị trí cố định, bắt đầu 0 |
| `Prompt` | varchar(1000) | Bản chụp câu điền chỗ trống |
| `Instruction` | varchar(500) | Bản chụp yêu cầu làm bài |
| `Explanation` | varchar(4000) | Bản chụp giải thích đáp án |
| `Options` | varchar(250)[] | Bản chụp lựa chọn sau hoán vị |
| `CorrectOptionIndex` | integer | Đáp án đúng theo thứ tự đã hoán vị |
| `SelectedOptionIndex` | integer nullable | Lựa chọn đã nộp, bắt đầu 0 |
| `AnsweredAt` | timestamptz nullable | Thời điểm ghi đáp án đầu tiên |
| `Version` | bigint | 1 khi chưa trả lời, 2 khi đã trả lời |
| `CreatedAt`, `UpdatedAt` | timestamptz | Thời điểm tạo câu và ghi đáp án |

FK session dùng CASCADE. Composite FK `(grammar_exercise_id, grammar_point_id)` tới
`grammar_exercises(id, grammar_point_id)` dùng RESTRICT và bảo vệ đúng cặp nguồn.
UNIQUE `(session_id, position)` và `(session_id, grammar_exercise_id)` bảo vệ thứ tự và
chống lặp câu. Index composite nguồn hỗ trợ FK.

CHECK bảo vệ các revision dương, vị trí 0–9, mảng/text/token tương tự exercise, locale
và chỉ số đáp án. Khi chưa trả lời, lựa chọn/thời điểm trả lời đều null, Version 1,
UpdatedAt = CreatedAt. Khi đã trả lời, cả hai có giá trị, chỉ số hợp lệ, Version 2 và
CreatedAt <= AnsweredAt = UpdatedAt.

`IsCorrect` được suy ra từ hai chỉ số và trả null khi chưa trả lời. Điểm tổng hợp có thể
tính từ các câu đã trả lời. Các bản chụp giữ nguyên khi nguồn được chỉnh sửa hoặc ẩn.
Locale thực tế từng câu hỗ trợ trường hợp dùng bản dịch fallback khác locale yêu cầu của lượt.

## Trạng thái, concurrency và tích hợp bước tiếp theo

`Answer` yêu cầu tải đủ navigation `Items`, kiểm tra câu thuộc lượt, lựa chọn hợp lệ
và trạng thái `InProgress`. Câu trả lời mới tăng version cả item và session. Câu cuối
tự chuyển session sang `Completed`. Nộp lại cùng lựa chọn là no-op, lựa chọn khác bị từ chối.
`Abandon` giữ đáp án đã ghi, kết thúc lượt và tăng version. Bỏ lại lượt đã bỏ là no-op.
Mọi validation và kiểm tra overflow được thực hiện trước khi thay đổi trạng thái liên quan.

Application cần ghi root và item trong cùng `SaveChanges`, dùng transaction tự động của EF.
Nếu dùng nhiều lệnh SQL hoặc `ExecuteUpdate`, cần transaction bao toàn bộ và điều kiện
owner/version/state cho từng cập nhật. Concurrency token root giúp hai request trả lời
hai câu khác nhau cạnh tranh cùng version, lỗi của writer thua phải rollback cả item.

Các invariant qua nhiều dòng như target language khớp profile, số câu thực tế khớp
QuestionCount và hoàn thành khi trả lời đủ được aggregate bảo vệ. SQL ghi trực tiếp cần
tuân thủ cùng quy tắc. Application/API cần kiểm tra profile thuộc user hiện tại và ánh xạ
DTO để chỉ công bố đáp án/giải thích khi câu tương ứng đã được trả lời.

Xóa profile/user cascade lịch sử cá nhân. Nguồn đã có lịch sử được FK RESTRICT bảo vệ.
Có thể dùng `IsPublished` để ngừng phát câu mới.

## Dữ liệu ban đầu

Seed chứa 200 câu, mỗi câu có hướng dẫn/giải thích Việt và Anh, tổng cộng 400 bản dịch.
Phạm vi ban đầu gồm 10 điểm tiếng Nhật và 10 điểm tiếng Đức, mỗi điểm 10 câu.
Danh mục hiện có 100 điểm ngữ pháp, bộ câu hỏi bước này phủ 20 điểm nhập môn.

| Ngôn ngữ | Mã điểm có câu hỏi |
| --- | --- |
| `ja` | `noun-copula`, `noun-negation`, `question-ka`, `noun-modifier-no`, `additive-mo`, `demonstratives`, `polite-verbs`, `polite-verb-negation`, `polite-verb-past`, `object-o` |
| `de` | `statement-v2`, `yes-no-questions`, `w-questions`, `personal-pronouns`, `sein-present`, `haben-present`, `regular-present`, `verb-stem-changes`, `formal-address`, `noun-capitals` |

Dữ liệu tách theo ngôn ngữ trong `Seeding/Japanese` và `Seeding/German`, được nối qua
`Seeding/GrammarPractice/GrammarPracticeSeedData`. ID cố định dùng generator 1023 đã dành
cho seed, hai millisecond allocation +3/+4 sau allocation Nhật gốc, sequence 0–99 cho
exercise và 200–399 cho translation. Code `fill-{key}` dùng khóa tường minh trong seed.
Mỗi giải thích có câu hoàn chỉnh theo đáp án đã chọn.

Nội dung được biên soạn cho module này. Kiểm thử xác nhận cấu trúc, số lượng, liên kết và
khả năng khởi tạo Domain. Chưa có vòng thẩm định nội dung bởi giáo viên bản ngữ.

## Migration và xác minh

Migration `20261111042903_AddGrammarPractice` tạo bốn bảng, constraints, index và seed
600 dòng. `Down` xóa bảng câu trong lượt trước khi xóa session và nguồn câu hỏi.
Rollback module sẽ xóa cả lịch sử luyện ngữ pháp nằm trong bốn bảng này.

Áp migration bằng cấu hình connection string của môi trường triển khai.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Console runner `tests/GrammarPractice.Tests` kiểm tra Domain và PostgreSQL thật.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/GrammarPractice.Tests/bin/Debug/net10.0/GrammarPractice.Tests.dll
```

Để chạy kiểm thử DB, đặt `GRAMMAR_PRACTICE_TEST_DATABASE` thành connection string tới DB
trống, biệt lập, tên `grammar_practice_tests`. Runner kiểm tra tên và từ chối DB đã có bảng.
Nó áp migration trước module, tạo user/profile, nâng cấp, kiểm tra constraint và concurrency,
xóa profile thử, rollback module rồi áp lại để xác nhận tính lặp của seed.

Lần xác minh module đạt 85 kiểm tra, gồm 48 Domain và 37 EF/PostgreSQL trên PostgreSQL 17.
Đã kiểm tra bản chụp bất biến, đáp án sau hoán vị, idempotency, overflow, partial navigation,
CHECK/FK, cascade/restrict, nâng cấp với dữ liệu có sẵn, Down/Up và rollback khi tranh chấp version.

Suite hồi quy `Grammar.Tests` đạt thêm 8.313 kiểm tra Domain/EF/PostgreSQL/HTTP.
Build toàn solution thành công, 0 error, còn warning NU1900 vì môi trường không truy cập
được dịch vụ NuGet vulnerability audit. Migration được xác minh trên DB kiểm thử biệt lập.

Bước API mở rộng runner lên 313 kiểm tra Domain/EF/PostgreSQL/HTTP.
Chi tiết endpoint và transaction/query budget nằm trong [tài liệu tích hợp FE](grammar-practice-api.md).
