# Pronunciation: model, DB và seed

[README dự án](../README.md) · [Ngôn ngữ và bảng chữ](language-foundations-model.md)

Module `Domain/Pronunciation` lưu quy tắc đọc/phát âm và ví dụ dùng chung theo ngôn ngữ.
Tài liệu này mô tả 3 entity, 3 mapping EF Core, migration và seed.
API đọc được mô tả tại [Pronunciation API cho FE](pronunciation-api.md);
luyện tập/chấm phát âm và tiến độ được triển khai ở các bước riêng.

## Quan hệ và bố cục

```text
Language
  └─ PronunciationRule
       ├─ PronunciationRuleTranslation
       └─ PronunciationExample
```

- Domain/Pronunciation: mỗi entity một file, factory Create và Update kiểm soát invariant.
- Domain/Common: tái sử dụng LanguageTag và FoundationContentValidation hiện có.
- Infrastructure/Persistence/Configurations: mỗi mapping một file, tự nhận qua ApplyConfigurationsFromAssembly.
- Infrastructure/Persistence/Seeding/PronunciationSeedData.cs: dữ liệu tĩnh được đưa vào HasData.
- ApplicationDbContext có DbSet cho cả 3 entity; không cần đăng ký DI mới.

Rule thuộc trực tiếp Language để cùng quy tắc có thể áp dụng cho nhiều bộ chữ. Ví dụ giữ
nguyên văn chữ/từ/câu; không buộc từng ký tự phải tồn tại trong bảng chữ đã seed.
Không có UserId/LearningProfileId: nội dung được dùng chung, không copy theo user.

## Quy ước chung

Cả 3 entity kế thừa Entity và có các field:

| Field | DB | Quy tắc |
| --- | --- | --- |
| Id | bigint PK | Snowflake do caller cung cấp; ValueGeneratedNever, không identity |
| Version | bigint NOT NULL, default 1 | Bắt đầu 1, dương, EF concurrency token |
| CreatedAt | timestamptz NOT NULL, default now() | Factory truyền thời gian chuẩn hóa UTC |
| UpdatedAt | timestamptz NOT NULL | Không trước CreatedAt và không lùi so với lần update trước |

Private constructor/setters; parent ID, code và locale dùng private init, không đổi sau tạo.
Update kiểm tra toàn bộ input và overflow version trước khi thay đổi entity, luôn tăng Version,
kể cả input giống dữ liệu cũ. Domain không tự gọi clock/generator hoặc truy cập DB.
Update bản dịch/ví dụ chỉ thay Version của chính bản ghi đó, không tự tăng Version của rule.

Text được trim và chuẩn hóa Unicode NFC; required text không rỗng, không chứa null character.
Optional text trắng thành null. Code lowercase ASCII bắt đầu bằng chữ, chỉ có chữ/số/hyphen.
Các enum cho loại quy tắc hoặc hệ phiên âm chưa cần thiết: dùng code string để thêm nội dung
cho ngôn ngữ khác mà không phải thêm enum/migration schema.

## pronunciation_rules / PronunciationRule

| Field riêng | DB | Ý nghĩa |
| --- | --- | --- |
| LanguageId | bigint NOT NULL FK languages(id) | Ngôn ngữ của quy tắc |
| Code | varchar(64) NOT NULL | Mã nội dung ổn định trong ngôn ngữ |
| NativeName | varchar(150) NOT NULL | Tên gốc, phục vụ hiển thị khi chưa có bản dịch |
| IsPublished | boolean NOT NULL, default false | Trạng thái xuất bản |
| SortOrder | integer NOT NULL, default 0 | Thứ tự không âm |

Unique(language_id, code) bảo vệ mã, đồng thời hỗ trợ lọc theo parent.
Index(language_id, sort_order, id) phục vụ danh sách/cursor theo thứ tự.
Không thêm index language_id một cột trùng prefix.

Rule có thể tồn tại ở dạng chưa xuất bản để bổ sung nội dung. API đọc lọc
cả Language.IsEnabled và Rule.IsPublished; hai cờ này không tự cascade trạng thái.

## pronunciation_rule_translations / PronunciationRuleTranslation

| Field riêng | DB | Ý nghĩa |
| --- | --- | --- |
| PronunciationRuleId | bigint NOT NULL FK pronunciation_rules(id) | Rule được giải thích |
| Locale | varchar(35) NOT NULL | Ngôn ngữ giải thích, normalize giống LanguageTag |
| Name | varchar(150) NOT NULL | Tên đã dịch |
| Explanation | varchar(8000) NOT NULL | Giải thích quy tắc, có thể nhiều đoạn |
| UsageNote | varchar(2000) NULL | Lưu ý, ngoại lệ hoặc lỗi dễ nhầm |

Unique(pronunciation_rule_id, locale). Locale không có FK tới languages: ngôn ngữ giải thích
không bắt buộc đã có catalog học tương ứng. Không xóa region/script khi normalize locale.
Name/Explanation là required; UsageNote null/trắng được giữ thành null.
Các giới hạn độ dài nằm ở public constants của entity để contract API sau này tái sử dụng.

## pronunciation_examples / PronunciationExample

| Field riêng | DB | Ý nghĩa |
| --- | --- | --- |
| PronunciationRuleId | bigint NOT NULL FK pronunciation_rules(id) | Rule được minh họa |
| Code | varchar(64) NOT NULL | Mã ví dụ ổn định trong rule |
| Text | varchar(500) NOT NULL | Chữ, từ hoặc câu nguyên bản |
| Transcription | varchar(500) NOT NULL | Cách đọc tương ứng |
| TranscriptionSystem | varchar(64) NOT NULL | Hệ phiên âm dạng code, ví dụ hepburn/ipa |
| AudioUrl | varchar(1024) NULL | Audio mẫu có sẵn; không lưu binary |
| SortOrder | integer NOT NULL, default 0 | Thứ tự không âm |

Unique(pronunciation_rule_id, code), index(pronunciation_rule_id, sort_order, id).
Một example chứa một bản phiên âm; có thể thêm example code khác để minh họa cách đọc khác.
Text và Transcription là Unicode string, không giả định một char hoặc chỉ dùng ASCII.
AudioUrl nullable, dùng cùng validation HTTPS hiện có của WritingSystemItemReading;
không gọi mạng, không sinh audio hoặc kiểm tra tài nguyên tồn tại khi tạo entity.
Ví dụ không chứa field nghĩa mặc định bằng một locale cố định; phần diễn giải nằm trong bản dịch rule.

## Constraints và xóa dữ liệu

- CHECK ID/FK dương, version > 0, updated_at >= created_at, thứ tự không âm.
- CHECK code ASCII lowercase, locale chuẩn hóa; tên/nội dung/phiên âm required không trắng.
- UsageNote chỉ null hoặc có nội dung; độ dài dùng varchar(n), required fields NOT NULL.
- CHECK định dạng HTTPS cho AudioUrl khi khác null, cùng quy ước mapping phần bảng chữ.
- Xóa Language cascade tới rules; xóa rule cascade tới translations/examples.
- Mỗi FK đã có index composite bắt đầu bằng parent ID; không có index FK một cột dư thừa.
- Concurrency dùng Version của từng entity; persistence raw SQL sau này cần kiểm tra version
  và tăng version cùng câu UPDATE. Mapping không tự tăng version khi ghi SQL ngoài Domain.

## Seed tiếng Nhật

| Nội dung | Số bản ghi |
| --- | ---: |
| pronunciation_rules | 5, IsPublished=true |
| pronunciation_rule_translations | 10: vi/en cho từng rule |
| pronunciation_examples | 15: 3 ví dụ mỗi rule |

Các rule theo thứ tự:

1. dakuten-handakuten — dấu âm đục/âm bán đục.
2. contracted-sounds — âm ghép với kana nhỏ.
3. geminate-consonants — っ/ッ nhỏ.
4. long-vowels — nguyên âm dài.
5. particle-readings — cách đọc は/へ/を ở ngữ cảnh trợ từ.

Ví dụ gồm kana đơn, từ và câu ngắn. Phiên âm dùng hepburn; nguyên âm dài trong ví dụ từ/câu
có macron như gakkō, obāsan, kōhī. Các code như gakkou là mã ổn định, không phải phiên âm
cần hiển thị cho người học. Không dùng code làm nguồn suy ra cách đọc.

AudioUrl của cả 15 ví dụ đều null. Seed chưa mở rộng bảng Hiragana/Katakana và không đổi
nội dung seed cũ; ví dụ có thể chứa kana mở rộng dù bảng chữ cơ bản chưa có những mục đó.

ID/timestamp cố định, không chạy generator/clock khi build EF model. Seed mới dùng offset
1000..1114 của cùng allocation Snowflake với JapaneseId hiện có; seed nền tảng dùng 0..445.
Mỗi hàng có ID/code riêng được cố định tường minh, không phụ thuộc vị trí trong mảng.
Thêm/sắp lại seed không được đổi ID/code cũ. Sửa nội dung cần tăng version/UpdatedAt của
hàng liên quan rồi tạo migration. HasData được áp dụng qua migration, không ghi đè khi app khởi động.

### Nguồn đối chiếu nội dung

Giải thích Việt/Anh được biên soạn ngắn cho ứng dụng; không nhập nguyên giáo trình, hình hay audio.
Các nhóm kiến thức và ví dụ kana được đối chiếu với:

- [Japan Foundation — Irodori, bài 2 và bảng kana](https://nd.jpf.go.jp/wp-content/uploads/2024/09/X_L02.pdf).
- [Agency for Cultural Affairs — quy tắc kana cơ bản](https://www.bunka.go.jp/kokugo_nihongo/sisaku/joho/joho/kijun/naikaku/gendaikana/honbun_dai1.html).
- [Agency for Cultural Affairs — các trường hợp viết trợ từ](https://www.bunka.go.jp/kokugo_nihongo/sisaku/joho/joho/kijun/naikaku/gendaikana/honbun_dai2.html).

## Migration và xác minh

Migration: `20261025162039_AddPronunciationRules`.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Migration tạo đúng 3 bảng mới cùng FK/index/check và seed; không chỉnh schema hoặc bản ghi
của module profile/auth/AI/bảng chữ. Chưa áp dụng migration vào DB thật trong lần triển khai này.
Không thêm hoặc chạy test suite ở bước model/DB này; build và EF model/migration được kiểm tra riêng.
