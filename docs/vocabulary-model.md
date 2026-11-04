# Từ vựng cơ bản: Domain, DB và seed

## Phạm vi và cấu trúc

Danh mục từ vựng dùng chung theo ngôn ngữ, không thuộc một user hoặc LearningProfile.
Tài liệu này mô tả Domain, enum, EF mappings, DbSet, migration và seed.
API đọc danh mục, query handlers và repository đã được bổ sung;
xem [API từ vựng và tích hợp FE](vocabulary-api.md).
Chưa triển khai API ghi catalog, frontend, tiến độ từ vựng hoặc tích hợp AI.

Giữ bốn layer và convention hiện có:

```text
Domain/Vocabulary/
  VocabularyEntry.cs
  VocabularySense.cs
  VocabularySenseTranslation.cs
  PartOfSpeech.cs
Infrastructure/Persistence/
  Configurations/VocabularyEntryConfiguration.cs
  Configurations/VocabularySenseConfiguration.cs
  Configurations/VocabularySenseTranslationConfiguration.cs
  Seeding/VocabularySeedData.cs
  Migrations/20261104064528_AddVocabularyCatalog.cs
tests/Vocabulary.Tests/
```

Quan hệ: `Language → VocabularyEntry → VocabularySense → VocabularySenseTranslation`.
Mỗi cạnh là một-nhiều, FK bắt buộc, ON DELETE CASCADE theo convention catalog hiện tại.
Không yêu cầu các ký tự của Term tồn tại trong bảng chữ đã seed.

Một từ có nhiều nghĩa, mỗi nghĩa có từ loại riêng và các bản dịch tương ứng.
Ví dụ `ご飯` có nghĩa cơm và bữa ăn; `聞く` có nghĩa nghe và hỏi. Dịch nghĩa theo Locale
giữ cùng một SenseId để bản Việt/Anh không bị coi thành hai nghĩa không liên quan.

## Field chung và hành vi

Ba entity kế thừa `Domain.Common.Entity`, constructor private, factory Create và method Update;
không có public setters. Các field chung:

| Field | DB | Ý nghĩa |
| --- | --- | --- |
| Id | bigint, PK | Snowflake ID dương từ caller/IIdGenerator, không identity/DB generation |
| Version | bigint, NOT NULL, default 1 | Optimistic concurrency token, luôn dương |
| CreatedAt | timestamptz, NOT NULL, default now() | Thời điểm tạo; Domain gán UTC và giữ nguyên khi update |
| UpdatedAt | timestamptz, NOT NULL | Lần sửa gần nhất; không trước CreatedAt/UpdatedAt trước đó |

Domain sử dụng lại `FoundationContentValidation` và `LanguageTag`:

- Code trim/lowercase, tối đa 64 ký tự, bắt đầu chữ ASCII, chỉ chữ/số/hyphen.
- Nội dung trim, chuẩn hóa Unicode NFC, giữ nguyên hoa/thường, từ chối NUL và quá dài.
- Optional text blank → null. Required text blank bị từ chối.
- AudioUrl nullable, tối đa 1024; nếu có phải là HTTPS tuyệt đối, không credentials,
  fragment, whitespace, control hoặc backslash theo validator URL hiện có.
- Locale là language tag string chuẩn hóa lowercase, tối đa 35, chẳng hạn `vi`, `en-us`, `zh-hant`.
- Update tính checked Version + 1 và kiểm tra toàn bộ input trước khi thay state;
  lỗi validation, thời gian đi lùi hoặc overflow không tạo mutation dở.
- Update hợp lệ tăng Version ngay cả khi nội dung hoặc timestamp giống trước, theo convention catalog.
- Version thuộc từng entity. Sửa bản dịch không tự tăng Version của Sense hoặc Entry.

Domain nhận clock/ID qua tham số; không truy cập DB, storage, HTTP hoặc dependency framework.

## vocabulary_entries / VocabularyEntry

| Field / column | DB | Ý nghĩa |
| --- | --- | --- |
| LanguageId / language_id | bigint, NOT NULL, FK languages(id) | Ngôn ngữ của từ, không đổi sau khi tạo |
| Code / code | varchar(64), NOT NULL | Code ổn định trong ngôn ngữ, không đổi sau khi tạo |
| Term / term | varchar(200), NOT NULL | Mặt chữ chuẩn để hiển thị, ví dụ `学校` |
| Reading / reading | varchar(200), NULL | Cách đọc mặc định dạng text, ví dụ `がっこう` |
| AudioUrl / audio_url | varchar(1024), NULL | URL HTTPS audio cho cách đọc mặc định, dùng được URL public R2 |
| IsPublished / is_published | boolean, NOT NULL, default false | Trạng thái công khai của entry |
| SortOrder / sort_order | integer, NOT NULL, default 0 | Thứ tự không âm trong catalog ngôn ngữ |

Reading không phải code hoặc danh sách; không chuyển lowercase và không tự sinh romaji.
Mỗi entry có một cách đọc/audio mặc định trong phạm vi hiện tại. Cùng mặt chữ nhưng khác
cách đọc có thể là entry khác với Code riêng. **Không UNIQUE theo Term hoặc Reading**.
Không dùng code seed như `gakkou` để suy ra cách đọc cho UI; Reading là dữ liệu riêng.

Indexes:

- UNIQUE `(language_id, code)`.
- `(language_id, sort_order, id)` phục vụ list/cursor sau này.
- Không thêm language_id index đơn lẻ vì các index trên đã phủ prefix FK đó.

CHECK: ID/LanguageId dương, Version dương, timestamp hợp lệ, code hợp lệ, Term có nội dung,
Reading null hoặc có nội dung, SortOrder không âm, AudioUrl null hoặc có dạng HTTPS hợp lệ cơ bản.
Validator URI ở Domain chặt hơn regex DB; không coi regex là đầy đủ mọi quy tắc URI.

## vocabulary_senses / VocabularySense

| Field / column | DB | Ý nghĩa |
| --- | --- | --- |
| VocabularyEntryId / vocabulary_entry_id | bigint, NOT NULL, FK vocabulary_entries(id) | Từ sở hữu nghĩa, không đổi sau khi tạo |
| Code / code | varchar(64), NOT NULL | Code nghĩa ổn định trong entry; không đổi khi sửa bản dịch |
| PartOfSpeech / part_of_speech | varchar(32), NOT NULL | Từ loại của chính nghĩa này; enum name, không persist số |
| SortOrder / sort_order | integer, NOT NULL, default 0 | Thứ tự hiển thị nghĩa, không âm |

Enum `PartOfSpeech`:

`Noun`, `Verb`, `Adjective`, `Adverb`, `Pronoun`, `Determiner`, `Adposition`,
`Conjunction`, `Particle`, `Interjection`, `Numeral`, `Auxiliary`, `Other`.

Đây là phân loại từ loại tổng quát dùng chung, không mô hình hóa biến tố hoặc toàn bộ ngữ pháp
của từng ngôn ngữ. Domain kiểm tra Enum.IsDefined; DB có CHECK danh sách tên hợp lệ.
Không đặt từ loại ở Entry vì một mặt từ có thể có các nghĩa khác từ loại.

Indexes:

- UNIQUE `(vocabulary_entry_id, code)`; cùng từ loại có thể xuất hiện ở nhiều nghĩa.
- `(vocabulary_entry_id, sort_order, id)`; không thêm index entry_id đơn lẻ.
- SortOrder được phép trùng theo convention catalog; các query hiện tại tie-break bằng Id.

CHECK: ID/EntryId/Version dương, timestamp/code/enum hợp lệ, SortOrder không âm.

## vocabulary_sense_translations / VocabularySenseTranslation

| Field / column | DB | Ý nghĩa |
| --- | --- | --- |
| VocabularySenseId / vocabulary_sense_id | bigint, NOT NULL, FK vocabulary_senses(id) | Nghĩa được diễn giải; không đổi sau khi tạo |
| Locale / locale | varchar(35), NOT NULL | Ngôn ngữ giải thích, không đổi sau khi tạo |
| Meaning / meaning | varchar(500), NOT NULL | Diễn giải nghĩa trong Locale này |
| UsageNote / usage_note | varchar(2000), NULL | Ghi chú ngắn phân biệt nghĩa/cách dùng bằng cùng Locale |

UNIQUE `(vocabulary_sense_id, locale)`; không thêm index sense_id đơn lẻ.
Locale không có FK tới `languages`: danh mục ngôn ngữ đích hiện chỉ có `ja`, nhưng vẫn phải
giải thích nghĩa bằng `vi`/`en`. Cách tổ chức này giống các translation table hiện có.
Meaning không rỗng; UsageNote null hoặc có nội dung. CHECK bảo vệ locale/ID/version/timestamp.
Các CHECK text mới từ chối cả chuỗi toàn tab/whitespace khi ghi SQL trực tiếp.

## Publication, transaction và ranh giới DB

Entry quản lý publication chung cho các nghĩa/bản dịch, không thêm cờ Enabled/IsPublished vào
từng bảng con. Model cho phép soạn dần entry trước khi có đủ nghĩa/bản dịch;
không có CHECK xuyên bảng để bắt entry đã publish phải có ít nhất một nghĩa hoặc bản dịch.

FK/unique/NOT NULL/varchar/CHECK bảo vệ toàn vẹn dữ liệu từng hàng và tham chiếu.
Chuẩn hóa NFC, trim, URI đầy đủ và tăng version khi sửa là trách nhiệm Domain/caller;
không dùng trigger âm thầm cập nhật thời gian/version. Raw SQL/ExecuteUpdate phải tự áp dụng
điều kiện version và quy tắc tương đương; EF concurrency token không tự bảo vệ các cách ghi đó.

API hiện tại chỉ đọc nên không mở transaction ghi. Khi tạo/sửa cả một cụm entry/sense/
translations trong use case sau, lưu cùng SaveChanges/transaction để không commit dở dữ liệu.
Các UPDATE đơn lẻ vẫn cần expected Version và xử lý xung đột khi bổ sung API ghi sau này.
Translation fallback được triển khai trong query, theo exact locale → en → null.
FE truyền TargetLanguage/ExplanationLanguage từ profile đang dùng; catalog không đọc lại
LearningProfile hoặc đặt hành vi API vào Domain. Chi tiết trong [tài liệu API](vocabulary-api.md).

Không có UserId, LearningProfileId, tiến độ, điểm, CEFR/JLPT, topic, ví dụ câu, flashcard hoặc AI metadata
trong ba bảng này. Catalog được chia sẻ, dữ liệu học cá nhân sẽ là feature riêng.

## Seed và nguồn đối chiếu

`VocabularySeedData` seed **40 entry tiếng Nhật, 42 sense, 84 translation** (`vi`/`en`).
Gồm từ về đồ ăn/đồ uống, người, nơi chốn, thời gian, động vật, động từ và tính từ nhập môn.
Mọi entry có Reading, IsPublished=true, Version 1, AudioUrl=null. Hai entry có nhiều nghĩa:

- `gohan`: `cooked-rice`, `meal`.
- `kiku`: `listen`, `ask`.

Diễn giải Việt/Anh và UsageNote được biên soạn ngắn cho ứng dụng. Nguồn đối chiếu cách đọc/nghĩa:

- [Irodori Starter — word list, The Japan Foundation](https://www.irodori.jpf.go.jp/assets/data/wordlist_X.pdf).
- [Digital Daijisen — 聞く, Shogakukan qua Kotobank](https://kotobank.jp/word/%E8%81%9E%E3%81%8F-472904): phân biệt nghĩa nghe và hỏi.
- [Cambridge Japanese–English — 飯](https://dictionary.cambridge.org/ja/dictionary/japanese-english/%E9%A3%AF): cơm và bữa ăn.

Không import file từ điển, bài giảng hay audio từ các nguồn này vào dự án. Audio sẽ bổ sung qua
storage R2 hiện có, không tải file mạng/upload storage khi chạy migration.

IDs được cấp trước trong allocation Snowflake của JapaneseId hiện có, generator 1023:
offsets **2000..2283**, không chồng foundation 0..445 hoặc pronunciation 1000..1114.
Entries dùng key 0..39, senses dùng key 100..141, translations 200..283 tính từ SeedBase;
key ghi tường minh, không lấy vị trí mảng làm ID. Allocation ID cũ không bắt buộc timestamp
CreatedAt phải trùng millisecond trong ID. Seed có timestamp cố định `2026-11-04T00:00:00Z`.

HasData khai báo dữ liệu trong model; migration sinh INSERT seed một lần. Startup không tự reseed.
Thêm/sắp lại dữ liệu phải giữ ID/code đã phát hành. Sửa seed cũ cần tăng Version/UpdatedAt
của hàng bị sửa và tạo migration mới; không gọi clock/generator runtime trong OnModelCreating.

## Migration và xác minh

Migration: **`20261104064528_AddVocabularyCatalog`**.
Up tạo ba bảng, 20 CHECK, ba FK CASCADE, năm index và seed. Không sửa bảng/seed cũ.
Down xóa translations → senses → entries; không xóa user/profile/progress hoặc catalog khác.
API không tự apply migration khi startup. Áp vào môi trường của bạn bằng lệnh hiện có:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

`tests/Vocabulary.Tests` là console runner theo convention dự án; không thêm NuGet package.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Vocabulary.Tests/bin/Debug/net10.0/Vocabulary.Tests.dll
```

Mặc định chạy Domain checks. Đặt `VOCABULARY_TEST_DATABASE` thành connection string của DB
**trống, biệt lập**, tên bắt buộc `vocabulary_tests`, để chạy thêm EF/PostgreSQL checks.
Runner từ chối DB sai tên hoặc đã có bảng, không truncate/xóa dữ liệu sẵn có. Dữ liệu kiểm thử
được giữ trong DB đó sau khi chạy; không dùng DB dự án/production làm DB test.

Kiểm tra gồm validation/NFC/UTC, mutation nguyên vẹn khi lỗi/overflow, enum string, Snowflake,
seed không trùng ID/toàn vẹn bản dịch, upgrade từ schema cũ, down/up và migrate lặp lại,
DB constraints/unique/cascade, homograph, locale vùng, optimistic concurrency cho cả ba entity,
và transaction rollback cả graph khi insert bản dịch trùng.

Bước model chạy thành công **199 kiểm tra**, gồm 97 Domain checks và 102 EF/PostgreSQL checks,
trên PostgreSQL 17 trong container biệt lập. Build toàn solution thành công, 0 warning/0 error.
Model khớp snapshot; migration upgrade/down/up,
seed lặp không trùng, concurrency và rollback đều qua. Container kiểm thử đã được dọn;
chưa apply migration hoặc ghi dữ liệu vào DB dự án/production.

Bước API mở rộng runner thành **382 kiểm tra** Domain/EF/PostgreSQL/HTTP,
bao gồm publication, locale fallback, phân trang, query budget và rate limit;
xem [cách tích hợp API và xác minh](vocabulary-api.md).
