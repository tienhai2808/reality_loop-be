# Tiến độ học bảng chữ: Domain và DB

## Phạm vi

Theo dõi người học đã bắt đầu hoặc đánh dấu học qua một mục chữ trong một LearningProfile.
Giữ nguyên bốn layer; phần này chỉ thêm entity, enum, EF mapping, DbSet và migration.
API/handler/repository đã được triển khai ở bước tiếp theo, xem [tài liệu API cho FE](learning-progress-api.md).
Luyện tập và chấm câu trả lời là module riêng, xem [Domain/DB luyện bảng chữ](writing-system-practice-model.md).
Chưa có lịch ôn tập hoặc cơ chế suy ra mastery.

Mỗi profile có tiến độ riêng. Đổi profile hiện tại hoặc sửa preferences không sửa tiến độ,
và thay đổi tiến độ không tăng `LearningProfile.Version`.

## Entity và trạng thái

`Domain/LearningProgress/WritingSystemItemProgress.cs` kế thừa `Entity`, constructor private,
không có setter công khai. `Id` do caller cấp từ Snowflake hiện có; Domain không tự sinh ID,
đọc clock, truy cập DB hoặc gọi dịch vụ ngoài.

`WritingSystemItemProgressStatus` gồm:

| Trạng thái | Ý nghĩa |
| --- | --- |
| `Learning` | Đã bắt đầu học hoặc chủ động đánh dấu học lại |
| `Completed` | Người học đánh dấu đã học qua; không khẳng định đã thuộc chữ |

Không có bản ghi nghĩa là chưa bắt đầu. Không tạo sẵn 92 dòng tiến độ cho mỗi profile,
không lưu `NotStarted` vào DB và không tạo bản ghi chỉ vì FE tải danh sách chữ.
Xem/nghe audio không tự chuyển sang `Completed`.

## Các field

| Field | DB | Vai trò |
| --- | --- | --- |
| `Id` | `bigint`, PK | Snowflake ID, dương, không do DB sinh |
| `LearningProfileId` | `bigint`, NOT NULL, FK | Profile sở hữu tiến độ |
| `WritingSystemItemId` | `bigint`, NOT NULL, FK | Mục chữ đang học |
| `Status` | `varchar(32)`, NOT NULL | Tên enum, mặc định `Learning` |
| `LastStudiedAt` | `timestamptz`, NOT NULL | Thời điểm tương tác học gần nhất được ghi nhận |
| `CompletedAt` | `timestamptz`, NULL | Thời điểm đánh dấu hoàn thành trong chu kỳ hiện tại |
| `Version` | `bigint`, NOT NULL | Bắt đầu 1; optimistic concurrency token |
| `CreatedAt` | `timestamptz`, NOT NULL | Lần bắt đầu đầu tiên của cặp profile–chữ |
| `UpdatedAt` | `timestamptz`, NOT NULL | Lần thay đổi bản ghi gần nhất |

Không lặp `UserId`, language code, tên chữ, phiên âm hoặc AudioUrl trong progress.
Chủ sở hữu được xác định qua `LearningProfile.UserId`; nội dung lấy từ catalog.
Các khóa liên kết của progress không thể đổi sau khi tạo.

`CreatedAt` đã thể hiện lần bắt đầu nên không thêm `StartedAt` trùng nghĩa.
Mọi timestamp từ Domain được chuẩn hóa UTC; EF dùng timestamp with time zone theo convention dự án.

## Hành vi Domain

```csharp
WritingSystemItemProgress.Create(id, learningProfile, writingSystemItem, writingSystem, language, now);
progress.RecordStudy(now);
progress.MarkCompleted(now);
progress.RestartLearning(now);
```

| Hành vi | Kết quả |
| --- | --- |
| `Create` | Kiểm tra ID và đúng chuỗi item → writing system → language → target language của profile; tạo `Learning`, Version 1, CompletedAt null, ba timestamp còn lại cùng thời điểm |
| `RecordStudy` | Cập nhật LastStudiedAt, UpdatedAt và Version; giữ nguyên Status/CompletedAt |
| `MarkCompleted` | Từ Learning sang Completed, đặt CompletedAt/LastStudiedAt/UpdatedAt, tăng Version |
| `RestartLearning` | Từ Completed sang Learning, xóa CompletedAt, cập nhật LastStudiedAt/UpdatedAt và tăng Version; giữ CreatedAt |

- MarkCompleted khi đã Completed hoặc RestartLearning khi đã Learning là no-op.
- RecordStudy với cùng LastStudiedAt là no-op.
- No-op không tăng Version, không đổi timestamp; timestamp trước UpdatedAt vẫn bị từ chối.
- Khi học thêm một chữ đã Completed, dùng RecordStudy: mốc CompletedAt cũ được giữ nguyên.
- Sau RestartLearning, lần MarkCompleted tiếp theo tạo mốc CompletedAt của chu kỳ mới.
- Tính version tiếp theo bằng `checked` trước khi đổi state; timestamp sai hoặc overflow không để lại state cập nhật dở.
- Domain không suy ra mức độ ghi nhớ, không tính điểm, streak hay lịch ôn tập.

Language tags được đối chiếu chính xác sau chuẩn hóa của các entity hiện có;
`ja` và `ja-jp` không tự coi là cùng một target language.

## Database

Bảng mới: `writing_system_item_progress`.

Khóa và delete behavior:

- PK `(id)`.
- FK `learning_profile_id → learning_profiles(id)`, ON DELETE CASCADE.
- FK `writing_system_item_id → writing_system_items(id)`, ON DELETE CASCADE.
- UNIQUE `(learning_profile_id, writing_system_item_id)`.

Xóa profile sẽ xóa tiến độ của profile. Xóa mục chữ sẽ xóa tiến độ phụ thuộc mục chữ đó.
Đây là trạng thái hiện tại, không phải lịch sử học tập bất biến. Ẩn nội dung bằng IsPublished
không xóa item và không làm mất progress.

CHECK constraints:

1. Id, LearningProfileId, WritingSystemItemId đều dương.
2. Version > 0.
3. Status chỉ thuộc `Learning`, `Completed`.
4. UpdatedAt >= CreatedAt.
5. CreatedAt <= LastStudiedAt <= UpdatedAt.
6. Learning bắt buộc CompletedAt null; Completed bắt buộc CompletedAt khác null.
7. Nếu có CompletedAt: CreatedAt <= CompletedAt <= LastStudiedAt.

CHECK dùng `IS NULL`/`IS NOT NULL` rõ ràng để PostgreSQL không bỏ lọt trạng thái sai
do biểu thức so sánh với NULL trả về UNKNOWN.

Index:

- `ux_writing_system_item_progress_profile_id_item_id`: unique theo profile–chữ;
  đồng thời phục vụ tìm tiến độ của profile và FK cascade theo profile.
- `ix_writing_system_item_progress_writing_system_item_id`: phục vụ FK/cascade từ item.

Không thêm index LearningProfileId đơn lẻ vì unique index đã có prefix đó.
Chưa có query workload nên không tạo thêm index status/timestamp hoặc bảng tổng hợp tiến độ.
Phần trăm học có thể tính từ catalog và các dòng progress hiện có; chữ chưa học được xác định
bằng phép đối chiếu với catalog, không chỉ lọc riêng bảng progress.

## Ranh giới validation và phần API sau này

DB bảo vệ tham chiếu tồn tại, unique, trạng thái và timestamp. Quy tắc cùng target language
được kiểm tra trong factory Domain bằng cả chuỗi catalog; FK độc lập không tự so sánh
language code qua nhiều bảng. Không dùng CHECK chứa truy vấn sang bảng khác.

Application/API áp dụng các quy tắc sau:

- Lấy profile với điều kiện `UserId` của caller; không tin ProfileId do FE gửi là đã có quyền.
- Lấy đúng item và ancestry từ DB, không nhận language/ancestor do FE tự khai để gọi factory.
- Có thể lấy context qua một query join; chữ ký factory không yêu cầu bốn lượt query riêng.
- Dùng thời gian server từ TimeProvider; không nhận mốc study/completion tùy ý từ FE.
- Áp dụng expected Version khi cập nhật. EF đã cấu hình Version làm concurrency token,
  nhưng caller vẫn phải xử lý xung đột; raw SQL phải có điều kiện version tương đương.
- Khi hai request tạo cùng progress, xử lý unique conflict; kiểm tra tồn tại trước insert
  không thay thế được unique constraint.
- GET catalog không tự ghi tiến độ. Việc ghi nhận học/đánh dấu hoàn thành là thao tác riêng.
- Những thay đổi business state đi qua method Domain; tác vụ import/raw SQL phải tuân thủ
  cùng quy tắc ngôn ngữ và version.

Các flow đọc/ghi tiến độ đã triển khai theo [tài liệu API](learning-progress-api.md); phần model/DB vẫn giữ nguyên invariant và schema.

## Migration và triển khai

Migration: `20261027114751_AddWritingSystemItemProgress`.

Migration chỉ tạo bảng, FK/CHECK và hai index mới; không sửa bảng hiện có,
không cập nhật seed catalog, không tạo progress cá nhân và không tác động storage/audio.
Down chỉ xóa bảng progress cùng constraint/index của nó.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Chạy với connection string của đúng môi trường đích. Bước triển khai này không tự áp migration
vào DB dự án hoặc production. Các kiểm tra PostgreSQL được thực hiện trên DB/container tạm riêng.

## Xác minh

Đã chạy 63 kiểm tra Domain, EF metadata và PostgreSQL 17 trên môi trường tạm:

- Tạo, học tiếp, hoàn thành, học lại, no-op và chuẩn hóa UTC.
- Từ chối ghép sai ngôn ngữ/ancestry, timestamp đi lùi, ID không dương và version overflow;
  thao tác bị từ chối không làm thay đổi một phần state.
- Enum string, Snowflake ID không tự sinh ở DB, concurrency token, đúng hai index và không có shadow column.
- Các CHECK/FK/NOT NULL/unique từ chối dữ liệu sai khi ghi SQL trực tiếp.
- Hai EF context cùng đọc một version: cập nhật sau bằng version cũ bị DbUpdateConcurrencyException,
  không ghi đè cập nhật đã commit.
- Cùng item có thể có tiến độ độc lập ở hai profile; cascade profile, user và item đúng phạm vi.
- Áp toàn bộ migration trên DB trống, rollback migration mới và áp lại thành công;
  không tự tạo progress cá nhân, runtime model khớp migration snapshot.

63 kiểm tra ở bước model dùng chương trình xác minh tạm. Bước API bổ sung console runner
`tests/LearningProgress.Tests`, xem [cách chạy và kết quả kiểm tra API](learning-progress-api.md#9-kiểm-tra-tự-động).
Không thêm package, không thay đổi console runner AI và không dùng DB dự án cho các kiểm tra.
