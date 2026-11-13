# Dữ liệu học tiếng Đức

Migration `20261107132245_AddGermanLearningContent` bổ sung catalog `de` vào các module học hiện có.
Dữ liệu học được tách theo LanguageId và LearningProfileId. Mã ngôn ngữ chuẩn dùng trong bộ này là `de`.
Mã có vùng như `de-de` vẫn là profile tag riêng theo quy tắc hiện tại, chưa tự ánh xạ về `de`.

| Nội dung | Tiếng Đức |
| --- | --- |
| Ngôn ngữ | Deutsch, có tên và mô tả Việt/Anh |
| Hệ chữ | `latin` |
| Mục chữ | 60, gồm 26 chữ A–Z và Ä/Ö/Ü/ẞ ở cả hai dạng hoa/thường |
| Tên chữ | 60 cách đọc IPA |
| Mẫu luyện viết | 60, tổng 136 nét |
| Quy tắc phát âm | 12, có 24 bản dịch và 60 ví dụ IPA |
| Từ vựng | 200 từ, 200 nghĩa, 400 bản dịch |
| Ngữ pháp | 50 điểm, 100 bản dịch |
| Audio | 30 bản ghi tên chữ, gắn rõ tới 60 reading |

## Phạm vi học

Bảng chữ phân biệt đầy đủ dạng hoa/thường, gồm ß và ẞ. IPA trong reading của bảng chữ thể hiện
**tên chữ cái**, như tên V là /faʊ/. Cách đọc V trong Vater hoặc Vase được giải thích bằng quy tắc phát âm.

Phát âm có độ dài nguyên âm, umlaut, ei/ie, au/eu/äu, ch, sch/sp/st, z/w/v, s/ss/ß,
vô thanh hóa phụ âm cuối, biến thể r, h chỉ độ dài nguyên âm và trọng âm. Mỗi quy tắc có năm ví dụ.
IPA là phiên âm rộng mang tính hướng dẫn, có các biến thể chuẩn vùng miền phù hợp.

Từ vựng gồm chào hỏi, đại từ, số, thời gian, gia đình, nơi ở, đi lại, đồ ăn, đồ dùng, động từ,
tính từ và các từ nối thường dùng. Danh từ có ghi chú mạo từ và số nhiều theo nghĩa được giới thiệu.
Từ không đếm được ghi rõ cách dùng cơ bản. Các trường hợp sie/Sie, wissen, helfen và mögen có lưu ý.
Giống và số nhiều được trình bày bằng UsageNote theo model hiện tại. Bộ này chưa cung cấp các trường
morphology riêng để lọc hoặc sinh bài luyện chia giống/số nhiều tự động.

Term giữ đúng chữ hoa/thường tiếng Đức. Reading của từ vựng được để null khi chưa biên tập phiên âm
riêng. Tên chữ cái không được dùng để giả lập cách đọc của cả từ. Audio từ/câu và ví dụ phát âm từ/câu
còn để null khi chưa có bản ghi đúng nội dung.

Ngữ pháp có vị trí động từ, câu hỏi, đại từ, hiện tại, giống/mạo từ/số nhiều, phủ định, sở hữu,
Nominativ/Akkusativ/Dativ, giới từ, giờ giấc, động từ tình thái, động từ tách, yêu cầu, liên từ,
Perfekt, so sánh và tính từ trước danh từ. Giải thích có ví dụ Đức cùng nghĩa Việt/Anh và lưu ý.
Nội dung được biên soạn cho bộ nhập môn, chưa được giáo viên biên tập toàn bộ hoặc chứng nhận bao phủ
một kỳ thi. Nguồn tham khảo về chủ đề gồm Goethe-Institut, IDS và các quy tắc chính tả của Duden.
[Goethe — danh sách từ tham khảo](https://www.goethe.de/pro/relaunch/prf/de/A1_SD1_Wortliste_02.pdf),
[IDS grammis](https://grammis.ids-mannheim.de/),
[Duden — quy tắc s/ss/ß](https://www.duden.de/sprachwissen/rechtschreibregeln/doppel-s-und-scharfes-s).

Mẫu viết là hướng dẫn nét chữ in monoline. Mẫu được lấy từ Hershey Sans 1-stroke ở revision
`262f4782cd412ee539eb43d4d9d9b92562d7590b`. Mẫu ẞ được RealityLoop điều chỉnh từ nét eszett để bổ sung
ký tự còn thiếu trong nguồn. Path nguồn, checksum, mô tả sửa đổi và toàn văn giấy phép nằm cùng bundle.
Phép chuẩn hóa giữ tỉ lệ, thứ tự nét và đặt chữ vào ô vuông. Importer tái tạo được dữ liệu offline.
Cách chấm geometry-v1 hiện tại so với mẫu tham chiếu. Các test khớp mẫu chưa xác nhận độ chính xác
với chữ viết tay người thật, chữ nối hoặc mọi biến thể chữ in.
[Hershey Sans 1-stroke, nguồn và điều kiện tái sử dụng](https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg).

## Áp dụng

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api --no-launch-profile -- --environment Development --initialize-foundation-audio
```

Lệnh thứ hai dùng cấu hình Storage:R2 và FoundationAudioSeed:PublicBaseUrl hiện có. Lệnh xử lý tất cả
manifest được đóng gói, gồm Nhật và Đức. Tổng hiện tại là 101 bản ghi, phủ 202 reading và năm ví dụ
kana đơn. URLs chỉ được cập nhật sau khi mọi upload/xác minh object thành công, trong một transaction.
Chạy lại bảo toàn URL tùy chỉnh và không tăng Version khi URL đã đúng. Khởi tạo không phụ thuộc
Wikimedia, Python hoặc font ngoài môi trường runtime.

30 MP3 Đức dùng nguồn Wikimedia Commons với Public domain, CC BY-SA 3.0 hoặc CC BY-SA 4.0 theo từng file.
Attribution, URL nguồn, phiên bản giấy phép và checksum lưu trong manifest. MP3 Đức được bổ sung ID3
TIT2/TPE1/TCOP ghi credit và license, giữ các frame audio. Đóng gói ứng dụng cần giữ manifest/license;
giao diện phân phối audio cần hiển thị credit phù hợp và liên kết nguồn theo giấy phép từng bản ghi.
Thông tin gốc từng file nằm trong `Seeding/German/Audio/manifest.json`, không suy ra giấy phép từ Commons nói chung.
[Commons — ví dụ bản ghi A](https://commons.wikimedia.org/wiki/File:De-A.ogg),
[Commons — ví dụ bản ghi Eszett](https://commons.wikimedia.org/wiki/File:De-Eszett.ogg).

## API và FE

FE lấy danh sách ngôn ngữ từ GET `/api/v1/languages?locale=vi`, sau đó tạo profile với TargetLanguage `de`.
Chọn profile qua endpoint current hiện tại. Với bảng chữ Đức dùng SystemCode do API trả về là `latin`.

| API | Nội dung |
| --- | --- |
| GET `/api/v1/languages/de/writing-systems` | Hệ chữ Latin |
| GET `/api/v1/languages/de/writing-systems/latin/items` | Chữ hoa/thường và tên chữ IPA |
| GET `/api/v1/languages/de/pronunciation-rules` | Quy tắc phát âm |
| GET `/api/v1/languages/de/vocabulary` | Từ vựng và nghĩa theo locale |
| GET `/api/v1/languages/de/grammar-points` | Danh mục ngữ pháp |
| Các route `/api/v1/learning-profiles/{id}/...` | Tiến độ, trắc nghiệm, luyện viết và ôn từ của profile tương ứng |

Các contract hiện tại được dùng cho cả hai ngôn ngữ. FE phải theo nextCursor để tải đủ danh sách và
hiển thị nút nghe khi AudioUrl có giá trị. Tag lang cần lấy từ languageCode/profile, tránh gán mặc định ja.
Một user có thể giữ profile Nhật và Đức đồng thời; đổi selection không tăng version của profile hoặc
chuyển dữ liệu học giữa hai profile.

## Cấu trúc và thêm ngôn ngữ tiếp theo

```text
Infrastructure/Persistence/Seeding/
  CatalogSeedData.cs
  Japanese/
    FoundationSeedData.cs
    PronunciationSeedData.cs
    VocabularySeedData.cs
    GrammarSeedData.cs
    HandwritingSeedData.cs
    Handwriting/
    Audio/
  German/
    FoundationSeedData.cs
    PronunciationSeedData.cs
    VocabularySeedData.cs
    GrammarSeedData.cs
    HandwritingSeedData.cs
    Handwriting/
    Audio/
  Audio/
    FoundationAudioCatalog.cs
    FoundationAudioInitializer.cs
```

CatalogSeedData ghép các bộ dữ liệu theo loại entity để EF configuration dùng chung. Seed Nhật giữ
nguyên mọi ID, timestamp/version và nội dung đã phát hành. Loader audio phát hiện manifest qua embedded
resource `FoundationAudio.{languageCode}.manifest.json`, kiểm tra file/checksum, target tồn tại đúng ngôn ngữ,
ReadingId/ExampleId duy nhất và ví dụ khớp nội dung reading. Mapping bằng ID tường minh thay cho suy luận
mỗi bản ghi phải có một cặp Hiragana/Katakana hoặc một số lượng file cố định cho toàn hệ thống.

Ngôn ngữ mới cần bundle nội dung, allocation ID riêng, ghép vào CatalogSeedData, manifest/media tương ứng
và migration mới. Manifest có ExpectedRecordings cho chính bundle đó. File .mp3 có logical resource name
`FoundationAudio.{languageCode}.{fileName}`. Object key dùng `foundation/{languageCode}/...` cùng hash nội dung.

GermanId `1563703640076578816` dùng generator Snowflake 1023 và millisecond liền sau hai allocation Nhật.
Sequence 0–21 dành cho ngôn ngữ/hệ chữ/bản dịch, 100–159 cho mục chữ, 200–259 cho reading,
300–359 cho mẫu viết, 500–659 cho phát âm, 1000–1999 cho từ vựng và 2200–2399 cho ngữ pháp.
Các key được ghi tường minh, độc lập vị trí trong mảng. Bổ sung/sửa nội dung cần giữ key cũ và tạo migration.

Migration chỉ INSERT 1232 dòng catalog Đức. Down khóa ghi bảng profile rồi chặn rollback nếu còn profile `de`, bảo vệ lịch sử khỏi cascade delete.
Khi không còn profile Đức, Down xóa đúng các dòng của release theo thứ tự FK. Dữ liệu profile cần được
bảo toàn hoặc di chuyển một cách tường minh trước khi rollback.

## Kiểm chứng

GermanChecks kiểm tra dữ liệu Nhật giữ nguyên sau khi tách seed và migration Đức chỉ thêm dữ liệu.
Các kiểm thử HTTP đi qua API bình thường với PostgreSQL biệt lập và authentication giả lập, gồm tạo/chuyển
profile, discovery catalog, phân trang 200 từ, ghi chú danh từ, 50 điểm ngữ pháp, 12 quy tắc phát âm,
đếm tiến độ, ôn từ, hoàn thành trắc nghiệm IPA và nộp đủ 60 mẫu viết. Các request dùng ID sai profile/ngôn ngữ
được kiểm tra. Audio dùng file thật với S3 giả lập để kiểm tra mapping hai ngôn ngữ, metadata và idempotency.
Bucket R2 thực tế vẫn cần được xác minh khi chạy khởi tạo trong môi trường triển khai.

Đã kiểm chứng ngày 07/11/2026. Build thành công; sáu suite Grammar (gồm GermanChecks và audio),
Vocabulary, Handwriting, VocabularyLearning, WritingSystemPractice và LearningProgress đều pass trên
PostgreSQL/HTTP biệt lập. Rollback bị chặn khi còn profile Đức và giữ nguyên lượt viết/lịch sử ôn từ.
Hai importer mẫu viết tái tạo đúng bytes đóng gói khi chạy offline.
