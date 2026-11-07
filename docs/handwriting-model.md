# Handwriting — Domain và database

Dữ liệu được mở rộng bằng migration `20261107120741_ExpandJapaneseLearningSeed`. Số lượng và phạm vi hiện tại được mô tả trong [Dữ liệu học tiếng Nhật](japanese-seed-content.md). Các số liệu migration gốc bên dưới ghi lại thời điểm khởi tạo module.

Phạm vi: model, invariant, EF Core mapping, migration và seed cho luyện viết không dùng AI.
Phần API đã được bổ sung riêng; xem [handwriting-api.md](handwriting-api.md) về use case, bộ chấm và tích hợp FE.
Prototype và cách chạy bảng viết vẫn ở [handwriting-lab.md](handwriting-lab.md).

## Cách chia dữ liệu

Ba bảng, tách catalog dùng chung khỏi dữ liệu riêng của learning profile:

```text
WritingSystemItem ──< HandwritingTemplate (mỗi revision là một hàng)
LearningProfile ──< HandwritingPracticeAttempt ──< HandwritingPracticeCharacter
                                                   >── HandwritingTemplate
```

Một attempt là **một lượt nộp đã chấm**, gồm 1–4 vị trí chữ cùng writing system. Ví dụ `ああ`
có hai vị trí riêng dùng cùng template. Không tạo session nháp hoặc lưu từng pointer event vào DB.
Viết lại tạo attempt mới; không ghi đè lịch sử. Chưa tự thay đổi tiến độ học, proficiency hoặc Version
của LearningProfile khi nộp bài.

Giới hạn hiện tại theo prototype đã kiểm thử: tối đa 12 nét/chữ, 512 điểm/nét, 4096 điểm/toàn lượt.
Đây là giới hạn phiên bản đầu, không phải tuyên bố hỗ trợ mọi hệ chữ.

## Domain

`src/Domain/Handwriting/` không phụ thuộc EF, JSON, HTTP hay project lab.
Entity có constructor/setter riêng, ID do Snowflake generator hiện có cấp từ bên ngoài.

### HandwritingTemplate — handwriting_templates

| Field / column | Kiểu PostgreSQL | Ý nghĩa |
| --- | --- | --- |
| Id / id | bigint PK | ID ổn định của đúng revision mẫu |
| WritingSystemItemId / writing_system_item_id | bigint FK | Mục chữ thuộc catalog hiện có |
| Revision / revision | integer | Revision nội dung, bắt đầu từ 1, unique theo mục chữ |
| Symbol / symbol | varchar(64) | Snapshot ký hiệu tại lúc tạo mẫu |
| Drawing / drawing | jsonb | Thứ tự nét và điểm chuẩn hóa |
| DrawingFormatVersion / drawing_format_version | integer | Phiên bản cấu trúc dữ liệu nét, hiện là 1 |
| SourceUrl / source_url | varchar(1024) | URL HTTPS của nguồn dữ liệu |
| SourceRevision / source_revision | varchar(128) | Commit/version nguồn, bắt buộc |
| Attribution / attribution | varchar(1000) | Ghi công nguồn, bắt buộc |
| License / license | varchar(128) | Giấy phép dữ liệu, bắt buộc |
| LicenseUrl / license_url | varchar(1024) | URL HTTPS của giấy phép |
| IsPublished / is_published | boolean | Có cho phép dùng mẫu cho lượt mới không |
| Version / version | bigint | Optimistic concurrency cho thay đổi publication |
| CreatedAt / created_at | timestamptz | Thời điểm tạo |
| UpdatedAt / updated_at | timestamptz | Thời điểm thay đổi publication gần nhất |

`Create(...)` nhận WritingSystemItem để lấy ID và snapshot Symbol. Mỗi nét tham chiếu cần
ít nhất hai điểm khác nhau. `Drawing`, `Symbol`, `Revision` và provenance không có phương thức sửa.
Sửa hình học/nguồn phải tạo hàng mới với Snowflake ID mới và revision mới.

`SetPublished(...)` kiểm tra timestamp, tăng Version bằng checked arithmetic trước khi thay đổi state;
giữ nguyên version/timestamp khi trạng thái không đổi. Unpublish không làm mất lịch sử.
Nhiều revision có thể được publish; tầng đọc sau này phải chọn revision phù hợp một cách xác định.

### HandwritingPracticeAttempt — handwriting_practice_attempts

| Field / column | Kiểu PostgreSQL | Ý nghĩa |
| --- | --- | --- |
| Id / id | bigint PK | Snowflake ID của lượt nộp |
| LearningProfileId / learning_profile_id | bigint FK | Profile sở hữu lịch sử; user được suy ra từ profile |
| WritingSystemId / writing_system_id | bigint FK | Bộ chữ dùng trong lượt |
| TargetText / target_text | varchar(256) | Ghép Symbol snapshot theo thứ tự các vị trí |
| AlgorithmVersion / algorithm_version | varchar(64) | Mã phiên bản bộ chấm và toàn bộ policy, ví dụ geometry-v1 |
| CharacterCount / character_count | integer | Số vị trí chữ, 1–4 |
| Passed / passed | boolean | Chỉ true khi tất cả vị trí đều đạt |
| MatchScore / match_score | integer | Trung bình điểm từng vị trí, 0–100 |
| Version / version | bigint | Luôn 1 vì lượt nộp không có thao tác cập nhật |
| CreatedAt / created_at | timestamptz | Thời điểm server ghi nhận kết quả |
| UpdatedAt / updated_at | timestamptz | Bằng CreatedAt với mô hình immutable hiện tại |

`Create(...)` nhận profile, language, writing system và toàn bộ answers đã chấm. Kiểm tra:

- Ngôn ngữ đang bật, writing system đã publish và thuộc đúng ngôn ngữ của profile.
- Template đã publish, trỏ đúng item, item thuộc writing system và Symbol chưa thay đổi.
- ID root/children dương và không trùng; vị trí 0-based, chữ lặp được phép.
- ExpectedStrokeCount khớp mẫu, ActualStrokeCount khớp nét nộp.
- Tổng điểm vẽ không vượt 4096; lượt nộp không sớm hơn ngày tạo mẫu.

Factory tự tính TargetText, CharacterCount, Passed và MatchScore. Không có setter công khai
hoặc phương thức ghi đè kết quả. Việc đánh giá hình học thật sự thuộc bước application/infrastructure
kế tiếp; factory không tự chạy bộ chấm. **FE không được quyền quyết định Feedback/Passed/MatchScore.**

### HandwritingPracticeCharacter — handwriting_practice_characters

| Field / column | Kiểu PostgreSQL | Ý nghĩa |
| --- | --- | --- |
| Id / id | bigint PK | Snowflake ID cho vị trí chữ |
| AttemptId / attempt_id | bigint FK | Lượt nộp chứa vị trí này |
| TemplateId / template_id | bigint FK | Tham chiếu trực tiếp đúng revision mẫu |
| Position / position | integer | Vị trí 0-based, 0–3 |
| Drawing / drawing | jsonb | Nét nộp gốc, giữ thứ tự và tọa độ trước khi bộ chấm chuẩn hóa |
| DrawingFormatVersion / drawing_format_version | integer | Schema nét, hiện là 1 |
| Feedback / feedback | jsonb | Kết quả từng nét và lý do lỗi của chữ |
| FeedbackFormatVersion / feedback_format_version | integer | Schema feedback, hiện là 1 |
| Version / version | bigint | Luôn 1 |
| CreatedAt / created_at | timestamptz | Bằng thời điểm tạo root |
| UpdatedAt / updated_at | timestamptz | Bằng CreatedAt |

Chỉ aggregate root tạo children. Geometry/provenance của template immutable nên không chép lại
toàn bộ mẫu vào từng bài nộp. FK giữ đúng template để xem lại kết quả theo đúng nguồn/revision.

## Value objects và JSONB

`InkPoint` chứa X/Y. `InkDrawing` sao chép sâu input, giới hạn ngay khi enumerate và chỉ lộ collection
read-only; `ToArray()` trả bản sao. Tọa độ double hữu hạn trong [0,1]. Không giữ pixel/canvas size,
ảnh PNG, áp lực bút, timestamp từng điểm hoặc thông tin thiết bị chưa dùng đến.

Vẽ thiếu được lưu `[]`; một nét phải có 1–512 điểm. Một lần chạm một điểm vẫn biểu diễn được để
bộ chấm trả lỗi. Mẫu tham chiếu không được rỗng hoặc có nét suy biến.

```json
[[{"x":0.1,"y":0.2},{"x":0.7,"y":0.8}]]
```

`HandwritingFeedback` immutable, tạo bằng `Rejected(...)` hoặc `Evaluated(...)`:

- Rejected: MissingDrawing, WrongStrokeCount hoặc DrawingTooSmall, kiểm tra tương thích số nét;
  điểm 0, Passed=false, chưa có chi tiết từng nét.
- Evaluated: có đúng đủ các nét với Index liên tục từ 0. Mỗi `HandwritingStrokeFeedback` có
  Index, MatchScore, Distance, LengthRatio, Issues và Passed suy ra từ Issues.
- Distance/LengthRatio hữu hạn, không âm; MatchScore nguyên trong [0,100].
- Issues không trùng, phải thuộc enum; các lỗi precheck không được đặt vào feedback từng nét.
- Điểm chữ là trung bình điểm nét; điểm lượt là trung bình điểm chữ; làm tròn `Math.Round`
  mặc định ToEven, giống lab. Đây là điểm tương đồng hình học, **không phải xác suất nhận dạng**.

Enum `HandwritingIssue`: `MissingDrawing`, `WrongStrokeCount`, `DrawingTooSmall`, `WrongStrokeOrder`,
`WrongDirection`, `StrokeTooShort`, `StrokeTooLong`, `WrongShape`. Persist nguyên tên trong JSON,
không persist số enum. Không lưu các chữ được nhận dạng bởi OCR/AI.

Codec thuộc Infrastructure/Persistence/Serialization, không thêm serialization attribute vào Domain.
Đọc JSON dựng lại value object có validation, không ngầm mặc định tọa độ bị thiếu thành 0.
Không cần snapshot mutable/ValueComparer clone vì toàn bộ value object và collection bên trong immutable.
DrawingFormatVersion/FeedbackFormatVersion là schema dữ liệu; AlgorithmVersion là logic/policy chấm,
SourceRevision là nội dung nguồn; Version là concurrency. Các giá trị này có vai trò riêng.
Thay thuật toán hoặc threshold phải cấp AlgorithmVersion mới, giữ khả năng đọc kết quả cũ.

## Ràng buộc và hiệu năng DB

- PK Snowflake, ValueGeneratedNever; Version concurrency token theo codebase.
- UNIQUE `(writing_system_item_id, revision)`; không thêm index riêng writing_system_item_id.
- UNIQUE `(attempt_id, position)`; không unique `(attempt_id, template_id)` vì từ có chữ lặp.
- Index lịch sử `(learning_profile_id, id DESC)` để đọc cursor theo profile; không index riêng profile.
- Index FK writing_system_id và template_id phục vụ kiểm tra FK/xóa, không thêm GIN JSONB khi chưa có truy vấn cần nó.
- Profile → attempts → characters: CASCADE. Item → templates, template → characters và
  writing system → attempts: RESTRICT, không xóa mất lịch sử khi dọn catalog. Unpublish thay cho xóa.
- CHECK ID dương, revision/version dương, số chữ/vị trí/điểm hợp lệ, mã algorithm, text không blank,
  URL HTTPS, timestamp hợp lệ. Attempt/character bắt buộc Version=1 và UpdatedAt=CreatedAt.
- CHECK JSONB loại dữ liệu, trường tọa độ bắt buộc, khoảng tọa độ, số nét/điểm, schema version,
  kiểu/range feedback, tên enum, actual count khớp drawing, trạng thái precheck, vị trí nét liên tục,
  trạng thái đúng/sai và tập lỗi giữa chi tiết nét và chữ.

CHECK xử lý rõ `is unknown` khi kiểm tra trường bắt buộc trong strict JSONPath: thiếu key có thể
trả unknown và bị filter bỏ qua nếu chỉ dùng NOT/exists. Xem [PostgreSQL 17 — JSONPath predicates](https://www.postgresql.org/docs/17/functions-json.html#FUNCTIONS-SQLJSON-FILTER-EX-TABLE).

CHECK không thay thế invariant xuyên nhiều hàng: đúng ngôn ngữ/ancestry, số child bằng CharacterCount,
tổng 4096 điểm cả lượt, điểm trung bình và snapshot text được bảo vệ bởi aggregate factory.
DB không dùng trigger khóa bất biến mọi cột hoặc tự chấm/tính lại điểm. Ghi SQL trực tiếp phải tuân theo
quy tắc đó; application chỉ ghi qua factory. Ownership của request phải kiểm tra bằng authenticated
user → LearningProfile ở use case, không tin một LearningProfileId do client gửi lên.

API chấm trong bộ nhớ trước; repository dùng transaction ngắn để INSERT root có điều kiện kiểm tra lại owner,
publication và template/item version, giữ FOR SHARE trên context/reference rồi batch insert children bằng một
SaveChanges. Transaction bao cả hai câu lệnh; lỗi child rollback cả root. Không mở transaction trong lúc nhận nét
hoặc chấm hình học. Lịch sử chỉ đọc cột summary ở root; chỉ detail mới cần đọc nét/feedback.

## Seed và giấy phép

Migration đầu tiên seed 4 mẫu `あ・お・ぬ・め`. Migration `20261104163042_CompleteKanaHandwritingTemplates`
bổ sung 88 mẫu để đủ **46 Hiragana + 46 Katakana** (212 nét), mỗi catalog item có đúng một template.
4 mẫu cũ giữ nguyên ID, hình học, revision, version và timestamp; không tạo lại lịch sử.
Tọa độ là đúng dataset đã dùng trong lab, nguồn KanjiVG commit
`70a0b7ae0c18ceb5cb358274b029cce0234a43bc`, giấy phép CC BY-SA 3.0, copyright Ulrich Apel.
File chuyển đổi và COPYING được giữ trong `src/Infrastructure/Persistence/Seeding/Handwriting/` và
embed trong assembly. Hướng dẫn chuyển đổi/hash SVG nguồn ở tài liệu lab.

ID cố định dùng allocation Snowflake JapaneseId hiện có, generator 1023, sequences **3000–3003** (4 mẫu cũ), **3100–3145**, **3200–3245** (bỏ qua 4 mục đã có);
không chồng foundation, pronunciation hoặc vocabulary. Ánh xạ ID→item ghi tường minh, không dựa vào
thứ tự mảng. Timestamp cố định `2026-11-04T00:00:00Z`. Không chạy HTTP, generator, clock hoặc upload
storage trong OnModelCreating/migration. Không có dependency runtime sang tools/HandwritingLab.

Khi FE được triển khai, phải hiển thị nguồn/attribution và liên kết giấy phép cho dữ liệu mẫu;
giữ thông tin giấy phép khi phân phối dataset chuyển đổi. Không seed bài nộp của user.

## Migration và kiểm thử

Migration: `20261104142057_AddHandwritingPractice`.
Up tạo 3 bảng, 5 index và 4 mẫu; Down xóa graph handwriting, giữ nguyên schema/dữ liệu các module khác.
Chưa áp vào DB của dự án. Áp migration bằng lệnh hiện có:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Console runner `tests/Handwriting.Tests`, không thêm NuGet package:

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Handwriting.Tests/bin/Debug/net10.0/Handwriting.Tests.dll
```

Mặc định chạy Domain checks. Đặt HANDWRITING_TEST_DATABASE để chạy thêm EF/PostgreSQL; chỉ chấp nhận
DB **trống, biệt lập**, tên `handwriting_tests`. Runner từ chối DB có dữ liệu, không tự truncate/drop database.
Kiểm tra geometry bounds/copies, feedback invariants, profile ancestry, publication concurrency,
JSONB round-trip, malformed JSON/direct SQL, upgrade bảo toàn dữ liệu, rollback graph khi lỗi child,
FK/unique/cascade/restrict, seed không trùng ID, down/up và model khớp snapshot.

Đã chạy thành công **178 kiểm tra** (76 Domain và 102 EF/PostgreSQL) trên PostgreSQL 17 trong
container biệt lập; bao gồm các ca thiếu key trong strict JSONPath và lỗi ghi child phải rollback root.

## Tái tạo dataset kana đầy đủ (offline)

Nguồn gốc: KanjiVG commit cố định nêu trên, CC BY-SA 3.0. Giữ SVG gốc trong
`src/Infrastructure/Persistence/Seeding/Handwriting/Sources/`, manifest `sources.json` ghi rõ
SHA-256, symbol, số nét, Snowflake sequence và item sequence cho từng mẫu. COPYING đi kèm dataset.

```bash
python3 scripts/import-handwriting-kana.py
```

Script kiểm tra hash từng SVG, viewBox, số nét, hỗ trợ đúng M/c của 92 file nguồn; lấy 32 mẫu
trên mỗi đoạn cubic Bézier và chuẩn hóa theo canvas 109×109, làm tròn 6 chữ số như 4 mẫu cũ.
Kiểm tra giới hạn điểm/nét, [0,1], coverage và so sánh chính xác 4 geometry cũ trước khi ghi JSON.
Không tải mạng lúc chạy script/migration; source được pin sẵn. Lab vẫn giữ 4 đề kiểm thử ban đầu,
API production có đầy đủ 92 mẫu.

Để có dữ liệu mới trên môi trường đang chạy, apply migration và tải lại danh sách FE:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Rollback migration coverage chỉ xóa 88 mẫu mới. FK RESTRICT sẽ chặn rollback nếu các mẫu đó đã
có bài nộp tham chiếu, bảo vệ lịch sử; không tự xóa bài của user để ép rollback.
