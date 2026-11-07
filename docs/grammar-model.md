# Grammar catalog — model và database

Dữ liệu được mở rộng bằng migration `20261107120741_ExpandJapaneseLearningSeed`. Số lượng và phạm vi hiện tại được mô tả trong [Dữ liệu học tiếng Nhật](japanese-seed-content.md). Các số liệu migration gốc bên dưới ghi lại thời điểm khởi tạo module.

Phần triển khai gồm Domain entity, validation, EF Core mapping, migration và seed cho hai bảng
grammar_points và grammar_point_translations. Cấu trúc bám catalog Pronunciation/Vocabulary hiện có.
API đọc, payload và luồng tích hợp FE được mô tả trong tài liệu API. [Grammar API](grammar-api.md)

## Quan hệ

```text
Language -> GrammarPoint -> GrammarPointTranslation
```

Language xác định ngôn ngữ đang học. Locale của translation xác định ngôn ngữ dùng để giải thích.
Một điểm ngữ pháp có thể có nhiều bản dịch, mỗi locale chỉ có một bản dịch.

## GrammarPoint

| Field | Cột | Kiểu PostgreSQL | Vai trò |
| --- | --- | --- | --- |
| Id | id | bigint PK | Snowflake được caller cấp |
| LanguageId | language_id | bigint NOT NULL FK | Ngôn ngữ của điểm ngữ pháp, cố định sau tạo |
| Code | code | varchar(64) NOT NULL | Code ổn định, unique theo ngôn ngữ, cố định sau tạo |
| NativeName | native_name | varchar(150) NOT NULL | Tên trong ngôn ngữ đang học |
| Pattern | pattern | varchar(500) NOT NULL | Cấu trúc mẫu dạng văn bản |
| IsPublished | is_published | boolean NOT NULL | Trạng thái xuất bản |
| SortOrder | sort_order | integer NOT NULL | Thứ tự hiển thị, cho phép bằng nhau |
| Version | version | bigint NOT NULL | Optimistic concurrency, bắt đầu 1 |
| CreatedAt | created_at | timestamptz NOT NULL | Thời điểm tạo, cố định |
| UpdatedAt | updated_at | timestamptz NOT NULL | Thời điểm cập nhật gần nhất |

Pattern giữ nguyên ký hiệu phù hợp ngôn ngữ. Seed tiếng Nhật dùng N cho danh từ, N1/N2 để phân biệt
các vị trí danh từ và V cho phần động từ đã chia phù hợp với đuôi trong cấu trúc. Nội dung bản dịch
giải thích cách sử dụng. Các ký hiệu phục vụ trình bày cho người học.

`GrammarPoint.Create(...)` kiểm tra ID dương, code hợp lệ, tên/cấu trúc có nội dung, giới hạn độ dài
và SortOrder không âm. Code trim/lowercase, bắt đầu bằng chữ ASCII và gồm chữ, số, dấu gạch ngang.
Tên/cấu trúc trim và normalize NFC bằng FoundationContentValidation dùng chung.

`Update(nativeName, pattern, isPublished, sortOrder, now)` kiểm tra toàn bộ dữ liệu trước khi sửa state,
tăng Version bằng checked arithmetic và cập nhật UpdatedAt. Ngôn ngữ/code/CreatedAt được giữ bằng private init.

## GrammarPointTranslation

| Field | Cột | Kiểu PostgreSQL | Vai trò |
| --- | --- | --- | --- |
| Id | id | bigint PK | Snowflake được caller cấp |
| GrammarPointId | grammar_point_id | bigint NOT NULL FK | Điểm ngữ pháp sở hữu bản dịch, cố định |
| Locale | locale | varchar(35) NOT NULL | Language tag của nội dung diễn giải, cố định |
| Name | name | varchar(150) NOT NULL | Tiêu đề bản địa hóa |
| Explanation | explanation | varchar(8000) NOT NULL | Nội dung giải thích |
| UsageNote | usage_note | varchar(2000) NULL | Lưu ý khi sử dụng |
| Version | version | bigint NOT NULL | Optimistic concurrency của riêng bản dịch |
| CreatedAt | created_at | timestamptz NOT NULL | Thời điểm tạo |
| UpdatedAt | updated_at | timestamptz NOT NULL | Thời điểm cập nhật |

`Create(...)` normalize locale qua LanguageTag, ví dụ EN-US thành en-us. Name/Explanation bắt buộc,
UsageNote chỉ có khoảng trắng được chuyển thành null. Các field văn bản trim/NFC và kiểm tra NUL.
`Update(name, explanation, usageNote, now)` tăng Version độc lập với parent, giữ locale và parent ID.
Hai entity dùng constructor/setter riêng và factory để kiểm soát invariant.

DateTimeOffset được chuẩn hóa UTC. UpdatedAt phải >= CreatedAt và mọi lần update phải >= UpdatedAt cũ.
Lỗi validation hoặc overflow xảy ra trước khi mutation, bảo đảm object còn nguyên khi thao tác thất bại.

## EF và ràng buộc DB

- Id bigint, ValueGeneratedNever theo hệ thống Snowflake hiện có.
- Version default 1, ValueGeneratedNever và IsConcurrencyToken trên cả hai entity.
- CreatedAt có default SQL now() theo convention catalog.
- FK language_id -> languages(id), ON DELETE CASCADE.
- FK grammar_point_id -> grammar_points(id), ON DELETE CASCADE.
- UNIQUE (language_id, code) cho code ổn định theo ngôn ngữ.
- Index (language_id, sort_order, id) phục vụ danh sách có thứ tự/keyset.
- UNIQUE (grammar_point_id, locale) bảo vệ mỗi bản dịch theo locale.
- Các composite index phủ prefix FK tương ứng. Tổng cộng 3 index mới.
- 14 CHECK kiểm tra ID/version dương, SortOrder không âm, code/locale đúng định dạng,
  required text có ký tự ngoài khoảng trắng, optional note có nội dung nếu khác null và timestamp hợp lệ.

CHECK dùng biểu thức POSIX whitespace tương tự Vocabulary. varchar giới hạn độ dài các cột ở DB.
Domain áp dụng chuẩn hóa văn bản trước khi ghi. Metadata publication và nội dung được EF theo dõi bằng Version.

Khi ghi một điểm cùng các bản dịch, Add các entity rồi một SaveChanges để EF commit graph trong transaction.
Nếu thao tác được chia thành nhiều câu lệnh ghi riêng, ứng dụng cần transaction chung cho toàn thao tác.
Mỗi bản dịch có concurrency token riêng để tránh ghi đè nội dung người khác vừa cập nhật.

## Seed nhập môn tiếng Nhật

Mình chọn **14 điểm** cho bộ nội dung khởi đầu, mỗi điểm có một bản dịch vi và một bản dịch en.
Tổng 14 GrammarPoint + 28 GrammarPointTranslation. Tất cả bắt đầu Version 1, đã publish.

| Thứ tự | Code | Cấu trúc |
| --- | --- | --- |
| 0 | noun-copula | N1 は N2 です |
| 1 | noun-negation | N1 は N2 ではありません |
| 2 | question-ka | N は N ですか／Vますか |
| 3 | noun-modifier-no | N1 の N2 |
| 4 | additive-mo | N も … |
| 5 | demonstratives | これ・それ・あれ／この・その・あの + N |
| 6 | polite-verbs | Vます |
| 7 | polite-verb-negation | Vません |
| 8 | polite-verb-past | Vました／Vませんでした |
| 9 | object-o | N を Vます |
| 10 | time-ni | 時刻 に Vます |
| 11 | action-location-de | 場所 で Vます |
| 12 | existence-arimasu | 場所 に N が あります |
| 13 | existence-imasu | 場所 に N が います |

Các giải thích Việt/Anh được biên soạn ngắn cho ứng dụng. Nguồn đối chiếu khái niệm và nội dung nhập môn
là tài liệu của Japan Foundation. Các liên kết tham khảo nằm ở cuối đoạn này.
[Marugoto Plus — Grammar Practice](https://a1.marugotoweb.jp/ja/grammar_topic5_9.php),
[Irodori — Other Resources](https://www.irodori.jpf.go.jp/en/resources.html),
[Akiko — Grammar Notes, Japan Foundation Bangkok](https://ba.jpf.go.jp/wp-content/uploads/2022/03/Akiko1_bunpoo382002.pdf).

Seed ID dùng allocation Snowflake JapaneseId hiện có, millisecond cố định 2026-10-25T00Z, generator 1023.
Offsets 3500–3513 dành cho điểm ngữ pháp, 3550–3577 cho bản dịch, nằm trong sequence 12 bit 0–4095.
Các key trong GrammarSeedData ghi tường minh, độc lập với vị trí mảng/thứ tự trình bày.
Kiểm thử kiểm tra toàn bộ seed của mọi module để phát hiện ID trùng nhau.
Timestamp seed cố định 2026-11-05T00Z. HasData sinh INSERT qua migration, dùng dữ liệu đã đóng gói trong assembly.
Sửa nội dung đã phát hành cần giữ ID/code, tăng Version/UpdatedAt và tạo migration cho thay đổi đó.

## Migration và kiểm chứng

Migration `20261105055444_AddGrammarCatalog` tạo hai bảng, hai FK CASCADE, 14 CHECK, ba index và 42 hàng seed.
Down xóa translations rồi points. Kiểm thử chạy trên PostgreSQL biệt lập.

Áp vào DB đang chạy bằng lệnh theo quy trình dự án.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Console runner cho regression của hai entity và persistence.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Grammar.Tests/bin/Debug/net10.0/Grammar.Tests.dll
```

Mặc định chạy Domain checks. Để chạy EF/PostgreSQL, đặt GRAMMAR_TEST_DATABASE thành connection string
của DB **trống, biệt lập**, tên grammar_tests. Runner kiểm tra tên DB và yêu cầu schema public trống.

Đã qua **113 kiểm tra** gồm 53 Domain và 60 EF/PostgreSQL. Các ca kiểm tra bao gồm bounds/NFC/locale,
mutation atomic/overflow, seed coverage và ID, schema/constraint/unique, stale updates của cả hai entity,
rollback graph khi trùng locale, cascade, upgrade bảo toàn user/catalog, down/up và model khớp snapshot.
