# Dữ liệu học tiếng Nhật

Bản mở rộng `20261107120741_ExpandJapaneseLearningSeed` bổ sung nội dung cho các module hiện có.
Các catalog dùng chung cho người học có TargetLanguage là `ja`. Giải thích và nghĩa có đủ hai locale `vi` và `en`.

| Nội dung | Tổng sau migration |
| --- | --- |
| Ngôn ngữ học | 1, tiếng Nhật |
| Bảng chữ | 2, Hiragana và Katakana |
| Mục bảng chữ | 208, mỗi bảng 104 mục |
| Cách đọc | 208, theo Hepburn |
| Mẫu luyện viết | 208, mỗi mục một mẫu revision 1 |
| Quy tắc phát âm | 12 với 24 bản dịch |
| Ví dụ phát âm | 60 |
| Từ vựng | 200 từ, 202 nghĩa, 404 bản dịch |
| Ngữ pháp | 50 điểm, 100 bản dịch |
| Audio đóng gói | 71 MP3, 2.13 MB, phủ 142 mục kana đơn |

## Phạm vi nội dung

Mỗi bảng có 46 chữ cơ bản, 25 âm đục/bán đục và 33 âm ghép với ya/yu/yo thông dụng.
Code `kana-r*` cũ giữ nguyên. Code mới dùng `kana-voiced-*` và `kana-contracted-*`.
Thứ tự giữ 46 chữ cơ bản ở đầu, tiếp theo là các nhóm mới. RowIndex/ColumnIndex phục vụ bố trí bảng.
じ/ぢ cùng phiên âm ji và ず/づ cùng phiên âm zu theo convention Hepburn hiện tại.
Ứng dụng có thể có nhiều đáp án chữ đúng cho cùng cách đọc trong bài trắc nghiệm.

208 mẫu nét phủ toàn bộ 208 mục. Mẫu kana đơn lấy từ KanjiVG ở revision đã ghim.
Mẫu âm ghép ghép hai glyph cùng nguồn trong một ô, viết glyph đầu trước rồi glyph nhỏ sau.
Phép biến đổi tọa độ được ghi rõ trong importer, nguồn SVG và SHA-256 được giữ trong sources.json.
Bốn mẫu đầu và toàn bộ 92 mẫu đã phát hành giữ nguyên dữ liệu điểm, ID và revision.
Mỗi ô có thể biểu diễn một mục âm ghép gồm hai ký tự Unicode. Giới hạn bốn vị trí luyện viết của API vẫn áp dụng.

Phát âm bổ sung nhịp mora, ん, xu hướng vô thanh hóa nguyên âm, cách viết âm dài, tổ hợp Katakana
trong từ mượn, じ/ぢ/ず/づ, cao độ và ngữ điệu. Mỗi quy tắc có năm ví dụ.
Ví dụ phát âm cũng có こんにちは và こんばんは để chỉ rõ は được đọc wa trong hai lời chào này.
Các ví dụ hướng dẫn đếm nhịp dùng TranscriptionSystem `mora-guide`; ví dụ khác dùng `hepburn`.
Ví dụ cao độ chỉ có cách đọc chữ, chưa có ký hiệu pitch accent.

Từ vựng gồm chào hỏi, cảm ơn, xin lỗi, đáp lời, đồ ăn, gia đình, đại từ, số, thời gian, địa điểm, nghề nghiệp, phương tiện,
phương hướng, đồ dùng, thời tiết, động từ, tính từ và trạng từ thường gặp.
Những trường hợp dễ nhầm có UsageNote, gồm cách gọi thành viên gia đình, đại từ あなた,
những cách đọc số khác nhau, tính từ な, 知る/知っています và các từ đồng âm khác cách viết.

Ngữ pháp bổ sung tính từ, mong muốn, đích đến/phương tiện, tần suất, lời mời, thể て/ない/た,
yêu cầu, hành động đang diễn ra, xin phép, điều cấm, nghĩa vụ, khả năng, trình tự thời gian,
trải nghiệm, liệt kê, nguyên nhân, tương phản, so sánh, lượng từ, mục đích và dạng ngắn/lịch sự.
Các điểm mới có cấu trúc, giải thích, ví dụ được dịch và lưu ý Việt/Anh.
Bộ này là nội dung nhập môn được chọn cho ứng dụng, chưa gắn nhãn chứng nhận hoặc danh sách JLPT chính thức.

## Áp dụng dữ liệu

Cập nhật DB theo quy trình triển khai hiện tại.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Migration thêm 116 mục/cách đọc/mẫu viết, 160 từ/nghĩa và 320 bản dịch nghĩa,
36 điểm ngữ pháp và 72 bản dịch, 7 quy tắc và 14 bản dịch cùng 45 ví dụ phát âm.
Sáu mô tả ngôn ngữ/bảng chữ được cập nhật Version 2 và UpdatedAt cố định.
Migration chạy trong transaction theo cơ chế hiện tại của EF Core/PostgreSQL.
IDs/code của dữ liệu cũ và dữ liệu tiến độ/lịch sử được giữ nguyên khi nâng cấp.
Các mục mới được tính là chưa học cho đến khi người dùng thực hiện tác vụ.

Audio được upload và gắn URL bằng lệnh khởi tạo đã có, sau khi cấu hình R2/PublicBaseUrl.

```bash
dotnet run --project src/Api --no-launch-profile -- --environment Development --initialize-foundation-audio
```

Lệnh kiểm tra 71 file embedded cùng metadata/checksum, áp migration, upload/xác minh object rồi cập nhật
142 reading trong transaction. Những ví dụ phát âm có nội dung đúng một kana đã ghi âm cũng được gắn
bản ghi tương ứng. URL do người quản trị tự gắn được giữ lại. Chạy lại lệnh không tạo bản ghi catalog trùng
hoặc tăng Version của các URL đã đúng. Mapping chỉ xét những kana có bản ghi trong manifest.

66 mục âm ghép, ví dụ từ/câu và 200 từ vựng chưa có bộ audio tương ứng đầy đủ.
AudioUrl của những mục chưa có bản ghi được để null. Bộ âm đơn không được ghép lại thành audio từ/câu.

Hướng dẫn cấu hình bucket và public URL nằm trong tài liệu audio hiện có. [Khởi tạo audio R2](foundation-audio-seeding.md)

## Tích hợp FE

Các endpoint hiện có đọc dữ liệu mở rộng qua cùng contract và cursor hiện tại.
Mỗi bảng có 104 mục nên FE phải tải các trang đến khi NextCursor là null, kể cả khi PageSize là 100.
Áp dụng cho danh sách bảng chữ, mẫu viết và tiến độ. Danh sách từ vựng có 200 mục và cũng cần phân trang.
Hiển thị nút nghe khi AudioUrl có giá trị. Với `mora-guide`, hiển thị dấu nối như hướng dẫn nhịp.

## ID, nguồn và bảo trì

Dữ liệu cũ giữ allocation Snowflake đã phát hành. Dữ liệu mới dùng cùng generator 1023 ở millisecond
liền sau allocation cũ, base ID `1563703640072384512`, chia các sequence theo module.

| Nhóm | Sequence |
| --- | --- |
| Mục/cách đọc Hiragana và Katakana | 0–357 |
| Từ, nghĩa và bản dịch mới | 1000–1719 |
| Ngữ pháp mới | 2000–2171 |
| Phát âm mới | 2400–2544 |
| Mẫu viết mới | 3000–3157 |

Key được ghi tường minh trong các mảng seed. Thêm dữ liệu cần cấp key mới trong vùng còn trống hoặc
allocation mới, giữ các key cũ và tạo migration tiếp theo. Nội dung đã phát hành cần tăng Version/UpdatedAt
khi sửa. Nội dung giải thích và bản dịch được biên soạn cho ứng dụng. Các nguồn đối chiếu học thuật gồm
bảng kana và phần ghi chú ngữ pháp của Japan Foundation. Việc đối chiếu nguồn chưa thay thế một vòng
biên tập toàn bộ nội dung bởi giáo viên tiếng Nhật.
[Irodori — tài liệu bổ trợ](https://www.irodori.jpf.go.jp/en/resources.html),
[Irodori — bài 2, bảng Katakana](https://nd.jpf.go.jp/wp-content/uploads/2024/09/X_L02.pdf).

SVG từ KanjiVG theo CC BY-SA 3.0. Audio từ Wikimedia Commons thuộc public domain/PD-self;
manifest ghi nguồn, tác giả, URL gốc và checksum cho từng file. Importer có thể tái tạo mẫu viết offline.
[KanjiVG revision](https://github.com/KanjiVG/kanjivg/tree/70a0b7ae0c18ceb5cb358274b029cce0234a43bc),
[Wikimedia — bộ audio kana](https://commons.wikimedia.org/wiki/Category:Audio_files_of_hiragana_(set_by_Hakatanoshio117117)).

## Kiểm chứng

SeedChecks trong Grammar.Tests kiểm tra coverage, cặp bản dịch, Snowflake không trùng giữa module,
field của dữ liệu cũ, version của mô tả đã sửa, loại operation của migration và toàn bộ bundle audio.
Các suite PostgreSQL/HTTP hiện có kiểm tra migration lên/xuống, giữ tiến độ và URL tùy chỉnh,
API phân trang, chấm mẫu viết và các luồng học với catalog mới.

Lần kiểm chứng ngày 07/11/2026 đã build thành công và chạy qua sáu suite PostgreSQL/HTTP gồm
Grammar (kèm seed và audio), Vocabulary, VocabularyLearning, Handwriting, WritingSystemPractice và
LearningProgress. PostgreSQL chạy trong container test biệt lập. Audio kiểm tra bằng toàn bộ file thật
với S3 giả lập và DB thật, chưa kiểm chứng bucket R2 thực tế của môi trường triển khai.
