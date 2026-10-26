# Danh mục ngôn ngữ và bảng chữ: model, DB, seed

[README dự án](../README.md)

Phạm vi hiện tại gồm hai phần: `Domain/Languages` và `Domain/WritingSystems`.
Tài liệu này mô tả domain entity, validation, EF Core mapping và migration/seed.
Nội dung dùng chung, không chứa UserId/LearningProfileId. API đọc được mô tả riêng tại
[API ngôn ngữ và bảng chữ cho FE](language-foundations-api.md); chưa có bài luyện, tiến độ hoặc AI cho hai phần này.

## Quy ước chung

- Cả 6 entity kế thừa Entity, có Id bigint/Snowflake, private constructor, factory Create và Update.
- ID do caller cung cấp từ generator hiện có, mapping ValueGeneratedNever; DB không sinh identity.
- Có Version bigint bắt đầu 1, concurrency token; Update tăng version, kể cả gửi lại cùng dữ liệu.
- CreatedAt/UpdatedAt là DateTimeOffset UTC/timestamptz; UpdatedAt không lùi so với lần cập nhật trước.
- Update kiểm tra toàn bộ input và version overflow trước khi thay đổi entity.
- Parent ID, code và locale không đổi sau khi tạo. Muốn chuyển phạm vi phải tạo bản ghi khác.
- Required text trim, Unicode NFC, không rỗng; optional text trắng thành null.
- Language code/locale trim/lowercase, tối đa 35 ký tự; cùng LanguageTag.Normalize với LearningProfile.
  Không đối chiếu culture catalog của hệ điều hành và không tự bỏ region/script trong tag.
- Các mã nội dung là ASCII lowercase, bắt đầu bằng chữ, dùng chữ/số/hyphen, tối đa 64 ký tự.
- FK cascade delete; xóa cha xóa nội dung con. IsEnabled/IsPublished chỉ là cờ, không cascade trạng thái.
  Các use case đọc kiểm tra cả ngôn ngữ và bộ chữ đang được bật/xuất bản.

## 1. Languages

### languages / Language

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| Code | varchar(35), unique | Tag ngôn ngữ ổn định, ví dụ ja hoặc zh-hant |
| NativeName | varchar(150) | Tên bằng ngôn ngữ gốc |
| IsEnabled | boolean, default false | Cho phép sử dụng mục ngôn ngữ này |
| SortOrder | integer, default 0 | Thứ tự không âm |
| Version | bigint, default 1 | Concurrency token |
| CreatedAt, UpdatedAt | timestamptz | Thời gian UTC |

Index unique(code) tra cứu tag; index(is_enabled, sort_order, id) phục vụ danh mục được bật.
Có mặt trong catalog không có nghĩa đã có đủ giáo trình hoặc audio cho ngôn ngữ đó.

### language_translations / LanguageTranslation

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| LanguageId | bigint FK languages | Ngôn ngữ cần dịch tên/giới thiệu |
| Locale | varchar(35) | Ngôn ngữ bản dịch, ví dụ vi hoặc en |
| Name | varchar(150) | Tên hiển thị được dịch |
| Description | varchar(2000), nullable | Giới thiệu ngắn |
| Version, CreatedAt, UpdatedAt | bigint/timestamptz | Quy ước chung |

Unique(language_id, locale). Locale không có FK tới languages: ngôn ngữ giải thích có thể
được hỗ trợ ngay cả khi chưa có nội dung để học chính ngôn ngữ đó.

## 2. WritingSystems

### writing_systems / WritingSystem

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| LanguageId | bigint FK languages | Ngôn ngữ sở hữu bộ chữ |
| Code | varchar(64) | Mã ổn định trong ngôn ngữ: hiragana, katakana… |
| NativeName | varchar(150) | Tên gốc |
| IsPublished | boolean, default false | Có thể cung cấp bộ chữ cho người học |
| SortOrder | integer, default 0 | Thứ tự không âm |
| Version, CreatedAt, UpdatedAt | bigint/timestamptz | Quy ước chung |

Unique(language_id, code); index(language_id, sort_order, id) để đọc theo thứ tự.
Một ngôn ngữ có thể có nhiều bộ chữ. Chưa tách loại bộ chữ thành enum.

### writing_system_translations / WritingSystemTranslation

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| WritingSystemId | bigint FK writing_systems | Bộ chữ |
| Locale | varchar(35) | Ngôn ngữ giải thích |
| Name | varchar(150) | Tên hiển thị |
| Description | varchar(4000), nullable | Giới thiệu và hướng dẫn ngắn |
| Version, CreatedAt, UpdatedAt | bigint/timestamptz | Quy ước chung |

Unique(writing_system_id, locale). Nội dung giải thích thuộc bản dịch, không hardcode một
ngôn ngữ trong item hoặc reading. Fallback API: locale chính xác → en → tên gốc; chi tiết trong tài liệu API.

### writing_system_items / WritingSystemItem

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| WritingSystemId | bigint FK writing_systems | Bộ chữ chứa mục này |
| Code | varchar(64) | Mã ổn định trong bộ chữ |
| Symbol | varchar(64) | Chuỗi ký tự/tổ hợp chữ, không giới hạn một char |
| SortOrder | integer, default 0 | Thứ tự tuyến tính không âm |
| RowIndex, ColumnIndex | integer, nullable | Vị trí grid tính từ 0; cả hai cùng null hoặc cùng không âm |
| Version, CreatedAt, UpdatedAt | bigint/timestamptz | Quy ước chung |

Unique(writing_system_id, code); index(writing_system_id, sort_order, id).
Không đặt unique cho Symbol hoặc tọa độ: code là identity nội dung; không ép mọi bộ chữ
vào một layout duy nhất. SortOrder/Id vẫn cho thứ tự ổn định khi không dùng grid.

### writing_system_item_readings / WritingSystemItemReading

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint PK | Snowflake ID |
| WritingSystemItemId | bigint FK writing_system_items | Mục có cách đọc này |
| Code | varchar(64) | Mã reading ổn định trong item |
| Transcription | varchar(200) | Cách đọc/phiên âm, ví dụ shi |
| TranscriptionSystem | varchar(64) | Hệ phiên âm dạng code, ví dụ hepburn hoặc ipa |
| AudioUrl | varchar(1024), nullable | URL HTTPS mẫu có sẵn; không lưu binary trong DB |
| SortOrder | integer, default 0 | Thứ tự không âm |
| Version, CreatedAt, UpdatedAt | bigint/timestamptz | Quy ước chung |

Unique(writing_system_item_id, code); index(writing_system_item_id, sort_order, id).
Cho phép nhiều cách đọc/mẫu audio cho cùng item. TranscriptionSystem dùng string vì mỗi
ngôn ngữ có thể bổ sung hệ phiên âm riêng. URL không chứa credentials/fragment và không
có whitespace; validation không gọi mạng hoặc kiểm tra file có tồn tại.

## Constraints

DB có CHECK cho ID/FK dương, version dương, updated_at >= created_at, thứ tự không âm,
code/locale đã chuẩn hóa, required text không rỗng, tọa độ grid hợp lệ và định dạng audio URL.
Độ dài dùng varchar(n), required columns NOT NULL. Unique index bảo vệ mã/trùng bản dịch.
Không thêm index FK một cột khi index unique hoặc index thứ tự đã có cùng prefix.

Các bản ghi LearningProfile hiện có tiếp tục lưu tag; chưa thêm FK từ profile sang catalog.
Migration không xóa hoặc đổi ngôn ngữ của profile cũ, cũng không copy nội dung cho từng user.

## Seed ban đầu

`Infrastructure/Persistence/Seeding/FoundationSeedData.cs` cung cấp HasData:

| Bảng | Số bản ghi |
| --- | ---: |
| languages | 1: ja / 日本語 |
| language_translations | 2: vi và en |
| writing_systems | 2: hiragana, katakana |
| writing_system_translations | 4: vi/en cho mỗi bộ |
| writing_system_items | 92: 46 Hiragana + 46 Katakana |
| writing_system_item_readings | 92: một reading Hepburn/mục |

Có khoảng trống đúng vị trí các hàng ya/yu/yo và wa/wo; n nằm hàng riêng.
Phiên âm của を/ヲ trong bảng là wo; cách phát âm theo ngữ cảnh sẽ thuộc nội dung quy tắc đọc.
Chưa seed dakuten/handakuten, yoon, chữ nhỏ, Kanji hoặc nét viết.
Migration nền tảng để AudioUrl=null. Bộ [audio kana và lệnh khởi tạo R2](foundation-audio-seeding.md) bổ sung 46 bản MP3 public domain cho 92 reading hiện có, upload trước rồi cập nhật URL trong transaction riêng; không tải audio trong migration hoặc sinh file bằng AI.

Seed có timestamp và Snowflake ID được cấp trước cố định theo IdGen default epoch/bit layout;
không đọc clock, không chạy generator ngẫu nhiên trong OnModelCreating. Các mã seed và ID
không được đổi khi chỉ sửa nội dung. Khi đổi seed, tăng version/cập nhật UpdatedAt của hàng
liên quan và tạo migration; không tự ghi đè lại tất cả seed mỗi lần app khởi động.
HasData chỉ phù hợp bộ nội dung nhỏ, cố định này; import lớn sau này cần workflow riêng.

## Migration

Migration: `20261025123943_AddLanguagesAndWritingSystems`.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

API không tự áp dụng migration khi startup. Chưa áp dụng migration vào DB thật trong lần
triển khai này. Migration chỉ tạo 6 bảng mới, FK/index/check và seed; không đổi schema profile/auth/AI.
