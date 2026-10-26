# Khởi tạo audio kana từ Wikimedia Commons lên Cloudflare R2

## Kết quả

Bộ `ja-kana-v1` cung cấp 46 bản ghi âm tiếng Nhật cho 92 cách đọc đang seed: 46 Hiragana và 46 Katakana. Hai bảng chữ dùng chung âm tương ứng nên không upload hai lần. Ví dụ あ và ア cùng dùng audio `a`.

Nguồn: bộ của **Hakatanoshio117117** trên [Wikimedia Commons](https://commons.wikimedia.org/wiki/Category:Audio_files_of_hiragana_(set_by_Hakatanoshio117117)), cùng các file [Ja-Ka.oga](https://commons.wikimedia.org/wiki/File:Ja-Ka.oga), [Ja-So.oga](https://commons.wikimedia.org/wiki/File:Ja-So.oga) của cùng tác giả. Metadata từng file công bố **public domain / PD-self**; không suy diễn giấy phép từ giấy phép chung của website. Manifest lưu tác giả, trang nguồn, URL gốc, timestamp, SHA-1 của bản nguồn gốc, SHA-256 của file và giấy phép cho từng bản ghi. Đây là bản ghi người thật, không dùng AI/TTS.

Bản MP3 transcode do Wikimedia cung cấp được giữ nguyên bytes và đóng gói làm embedded resource của Infrastructure. App publish có sẵn cả manifest lẫn audio; lần khởi tạo trên server không cần Python, Wikimedia, ffmpeg hay thư mục audio bên ngoài. Đây là bộ dữ liệu bootstrap nhỏ có nguồn cố định; media người dùng và các bộ lớn tiếp theo vẫn lưu ở R2, không nhét vào Git.

Phạm vi chỉ là cách đọc kana cơ bản hiện có. Không gắn âm đơn vào ví dụ từ/câu trong `pronunciation_examples`; các ví dụ đó cần bản ghi đúng toàn bộ từ/câu. Bộ hiện tại không được quảng cáo là đủ mọi phát âm tiếng Nhật hoặc mọi ngôn ngữ. Kana を vẫn giữ phiên âm chart `wo` của codebase; nghe âm theo bản ghi, không dùng tên file nguồn để suy ra mọi cách đọc trong câu.

## 1. Chuẩn bị bucket và credentials

Nếu đã làm [R2 storage setup](r2-storage-setup.md), giữ cấu hình đó. Chỉ cần bổ sung public URL và bật truy cập public.

1. Cloudflare Dashboard → **R2 Object Storage** → tạo/chọn bucket audio.
2. R2 → **Manage R2 API Tokens** → tạo credentials có **Object Read & Write**, giới hạn đúng bucket.
3. Lưu Access Key ID, Secret Access Key và copy **S3 API endpoint**. Không dùng Cloudflare bearer token làm secret S3.
4. Mở bucket → **Settings** → **Public Development URL** → **Enable**.
5. Nhập xác nhận theo dashboard rồi kiểm tra Public URL Access là **Allowed**.
6. Copy Public Bucket URL, ví dụ `https://pub-<ID_THAT>.r2.dev`.

Theo lựa chọn hiện tại, dùng luôn URL `r2.dev`; không cần custom domain/DNS. Cloudflare quy định r2.dev là development URL có rate limit và không có các tính năng cache/WAF của custom domain. Đây là giới hạn của endpoint, không phải hạn chế trong code; hướng dẫn này vẫn dùng r2.dev như bạn yêu cầu.

Bucket bật public chỉ nên chứa media được phép công khai. Không dùng CORS để bảo vệ file cá nhân. Credentials luôn ở backend.

Phân biệt:

| Config | Giá trị |
| --- | --- |
| `Storage:R2:ServiceUrl` | Endpoint S3 API `https://<ACCOUNT_ID>.r2.cloudflarestorage.com` hoặc endpoint jurisdiction do dashboard cung cấp |
| `Storage:R2:BucketName` | Tên bucket |
| `Storage:R2:AccessKeyId` | Access Key ID của credentials R2 |
| `Storage:R2:SecretAccessKey` | Secret Access Key của credentials R2 |
| `FoundationAudioSeed:PublicBaseUrl` | Public Bucket URL `https://pub-<ID_THAT>.r2.dev` |

Hai URL khác nhau. Không lấy public URL điền vào ServiceUrl. PublicBaseUrl chỉ là root, không có bucket, thư mục, query hay object key. Initializer ghép object key từ manifest.

## 2. Cấu hình development

Từ root backend, thay các giá trị mẫu bằng dữ liệu thật:

```bash
dotnet user-secrets set 'Storage:R2:ServiceUrl' 'https://YOUR_ACCOUNT_ID.r2.cloudflarestorage.com' --project src/Api
dotnet user-secrets set 'Storage:R2:BucketName' 'realityloop-media-dev' --project src/Api
dotnet user-secrets set 'Storage:R2:AccessKeyId' 'YOUR_R2_ACCESS_KEY_ID' --project src/Api
dotnet user-secrets set 'Storage:R2:SecretAccessKey' 'YOUR_R2_SECRET_ACCESS_KEY' --project src/Api
dotnet user-secrets set 'FoundationAudioSeed:PublicBaseUrl' 'https://pub-YOUR_BUCKET_ID.r2.dev' --project src/Api
```

Connection string `ConnectionStrings:Database` phải trỏ tới PostgreSQL cần khởi tạo. Nếu chưa có, cấu hình qua user-secrets theo môi trường thực tế:

```bash
dotnet user-secrets set 'ConnectionStrings:Database' 'Host=localhost;Port=5432;Database=realityloop;Username=YOUR_USER;Password=YOUR_PASSWORD' --project src/Api
```

Không ghi đè connection string đang dùng nếu đã đúng. Người dùng DB chạy khởi tạo phải có quyền apply migration; production có thể dùng tài khoản deployment riêng với tài khoản runtime.

Cấu hình trong shell có thể lưu vào history; không chia sẻ secret/history. Không commit secret vào repository. User-secrets chỉ dành cho Development.

## 3. Chạy một lệnh khởi tạo

```bash
dotnet run --project src/Api --no-launch-profile -- --environment Development --initialize-foundation-audio
```

Lệnh chạy ở chế độ initialization, hoàn tất rồi thoát; không mở HTTP server. Chỉ cần DB, R2 và PublicBaseUrl; không cần OAuth/AI/JWT/Redis để chạy chế độ này.

Thứ tự thực hiện:

1. Resolve/validate cấu hình R2 và public URL.
2. Kiểm tra manifest, mapping với bộ kana hiện tại, đủ 46 file, kích thước, MP3 header và SHA-256.
3. `MigrateAsync` áp dụng các migration chưa chạy, bao gồm seed ngôn ngữ/bảng chữ hiện có. Migration dùng cơ chế khóa của EF Core/PostgreSQL.
4. Upload hoặc xác minh 46 object, tối đa 4 tác vụ đồng thời; object key chứa hash nội dung.
5. Chỉ sau khi tất cả upload thành công, mở transaction PostgreSQL, lấy advisory lock để các lượt init không ghi đè nhau, khóa 92 reading theo thứ tự ID.
6. Kiểm tra mapping ID/item/cách đọc vẫn đúng seed. Dùng method update của Domain để validate URL và tăng Version/UpdatedAt.
7. Ghi các dòng thay đổi bằng **một câu UPDATE theo tập dữ liệu JSON**, có điều kiện version; commit toàn bộ.
8. Log số file, số reading đã cập nhật, số URL tự nhập được giữ nguyên; thoát với mã 0 nếu thành công.

Không cần `dotnet ef database update` riêng trước lệnh này. Nếu đã chạy update DB thì initializer chỉ tiếp tục từ trạng thái hiện tại. **Chạy riêng `dotnet ef database update` sẽ không upload hay gắn URL audio**: không nhét thao tác HTTP/R2 vào migration. Không có migration/schema mới cho bước này vì `audio_url`, Version và timestamp đã có sẵn.

Nếu muốn deployment “init xong là có audio”, chạy lệnh init này làm bước release/job trước khi start các API instance. API startup bình thường không tải file, upload hay tự chạy migration.

## 4. Tính an toàn khi chạy lại

- Object key bất biến theo nội dung, dạng `foundation/ja/kana/v1/a-<SHA256_PREFIX>.mp3`.
- PUT có `If-None-Match: *`; không overwrite object đã có.
- Nếu object đã tồn tại, đọc metadata kiểm tra kích thước và SHA-256; sai thì dừng để bạn kiểm tra, không ghi đè tùy tiện.
- SDK dùng timeout/retry đã cấu hình; lỗi không được bỏ qua. Nếu bị 409 khi hai lượt upload tranh nhau, chạy lại sau khi lượt kia xong.
- Không tạo thêm reading; sử dụng đúng 92 seed ID đã có.
- URL bằng giá trị mong muốn: không UPDATE, không tăng version.
- URL đang null: gắn URL mới.
- URL có cùng object key của bundle nhưng khác public domain: cho phép chuyển sang PublicBaseUrl mới.
- URL khác do bạn đã tự nhập: giữ nguyên và log số lượng.
- Nếu bản ghi đổi transcription/mapping so với bộ seed: dừng toàn bộ phần cập nhật DB để tránh gắn nhầm âm.
- Upload lỗi: không ghi URL mới vào DB. Các migration đã thành công vẫn giữ nguyên.
- DB lỗi sau upload: rollback transaction DB; object đã upload vẫn còn trên R2 và dùng lại khi chạy tiếp.
- Không có distributed transaction giữa R2 và PostgreSQL; không xóa các object thành công để “rollback giả”. Object key ổn định giúp retry an toàn.
- Không giữ transaction/row lock trong thời gian upload. Trong transaction chỉ có advisory lock, một lượt đọc/khóa tập reading và một bulk UPDATE nếu có thay đổi.

Lưu ý: bước kiểm tra cấu hình không thể xác minh PublicBaseUrl thuộc đúng bucket bạn đã chọn. Copy URL từ **chính bucket đó** và kiểm tra phát audio sau init. Metadata remote dùng để nhận diện bundle đã upload; không tải lại toàn bộ file R2 trên mỗi lượt init.

## 5. CORS trên R2

Bucket → Settings → CORS Policy → JSON, dùng policy cho frontend đang có:

```json
[
  {
    "AllowedOrigins": [
      "https://my-client.tienhai.click",
      "http://localhost:5173",
      "http://localhost:3000"
    ],
    "AllowedMethods": ["GET", "HEAD"],
    "AllowedHeaders": ["Range"],
    "ExposeHeaders": ["ETag", "Content-Length", "Content-Range", "Accept-Ranges"],
    "MaxAgeSeconds": 3600
  }
]
```

Đây là JSON dashboard/S3, không phải schema Wrangler/Cloudflare REST API. Origin phải khớp nơi FE thật sự chạy, không có path hoặc `/` cuối. CORS R2 độc lập CORS API .NET. FE không cần Authorization header khi đọc audio public. Không cấp PUT/DELETE cho FE chỉ để phát audio.

## 6. Kiểm tra DB và nối FE

SQL kiểm tra coverage:

```sql
SELECT s.code AS writing_system,
       count(*) AS readings,
       count(r.audio_url) AS readings_with_audio
FROM writing_system_item_readings r
JOIN writing_system_items i ON i.id = r.writing_system_item_id
JOIN writing_systems s ON s.id = i.writing_system_id
JOIN languages l ON l.id = s.language_id
WHERE l.code = 'ja' AND s.code IN ('hiragana', 'katakana')
GROUP BY s.code
ORDER BY s.code;
```

Với DB seed nguyên bản, kỳ vọng mỗi bảng có 46 reading và 46 URL audio. Nếu đã chỉnh sửa nội dung/bổ sung reading, đánh giá kết quả theo dữ liệu thực tế; initializer chỉ quản lý bộ ID cố định.

API đọc nội dung vẫn dùng JWT ứng dụng theo module hiện có. API hiện tại đã trả `audioUrl` trong `primaryReading` khi lấy items và trong danh sách readings. Không thêm endpoint mới:

```text
GET /api/v1/languages/ja/writing-systems/hiragana/items?pageSize=100
GET /api/v1/languages/ja/writing-systems/katakana/items?pageSize=100
GET /api/v1/languages/ja/writing-systems/hiragana/items/kana-r0-c0/readings
```

FE lấy URL trả về, ví dụ:

```javascript
const url = item.primaryReading?.audioUrl;
if (url) {
  const audio = new Audio(url);
  await audio.play(); // gọi từ thao tác bấm của người dùng để tránh autoplay policy
}
```

Có thể dùng `<audio controls src="URL_TU_API"></audio>`. Với fetch/Web Audio cần CORS đúng. Không tự ghép ID/file name phía FE và không gửi secret R2 xuống browser.

Copy một URL **thật từ API/DB** để kiểm tra:

```bash
curl -I 'URL_AUDIO_THAT'
curl -I -H 'Origin: https://my-client.tienhai.click' 'URL_AUDIO_THAT'
curl -D - -o /dev/null -H 'Range: bytes=0-1023' 'URL_AUDIO_THAT'
```

Kỳ vọng Content-Type `audio/mpeg`, file tồn tại trả 200, Origin hợp lệ nhận CORS header tương ứng, Range hợp lệ nhận 206/Content-Range. Bộ seed dùng MP3 playback derivative của Wikimedia, không transcode hay cắt ghép lại trên server. Kiểm tra phát trên thiết bị/browser mục tiêu trước khi phát hành.

## 7. Deploy server/container

Inject các giá trị sau tại runtime bằng secret/config store:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Database=<POSTGRES_CONNECTION_STRING>
Storage__R2__ServiceUrl=<R2_S3_API_ENDPOINT>
Storage__R2__BucketName=<BUCKET_NAME>
Storage__R2__AccessKeyId=<R2_ACCESS_KEY_ID>
Storage__R2__SecretAccessKey=<R2_SECRET_ACCESS_KEY>
FoundationAudioSeed__PublicBaseUrl=https://pub-YOUR_BUCKET_ID.r2.dev
```

Publish như bình thường:

```bash
dotnet publish src/Api -c Release -o ./artifacts/api
```

Trong thư mục publish hoặc image/container đã được cấp env:

```bash
dotnet Api.dll --initialize-foundation-audio
```

Chỉ sau khi init thành công, chạy API bình thường bằng `dotnet Api.dll`. API runtime vẫn cần các cấu hình OAuth/JWT/AI/Redis khác của hệ thống. Job init không cần các thành phần đó. Không để mọi replica tự chạy job migration/upload cùng lúc; chạy một release job, dù phần ghi DB vẫn có khóa bảo vệ khi vô tình chạy trùng.

## 8. File triển khai và nguồn dữ liệu

```text
scripts/
  fetch-foundation-audio.py              công cụ maintainer lấy/kiểm tra lại nguồn
src/Infrastructure/
  Persistence/Seeding/Audio/
    Assets/*.mp3                        MP3 playback derivative
    manifest.json                       mapping, nguồn, tác giả, giấy phép, checksum
    FoundationAudioEntry.cs
    FoundationAudioManifest.cs
    FoundationAudioCatalog.cs           kiểm tra bundle offline
    FoundationAudioSeedOptions.cs
    FoundationAudioInitializer.cs       migration + orchestration + transaction/bulk update
  Storage/R2/
    R2FoundationAudioUploader.cs         thao tác S3 tương thích R2
src/Api/
  Program.cs                            chọn chế độ initialization
```

Giữ nguyên bốn layer; không thêm project hoặc package. Domain/Application không phụ thuộc SDK S3. Không có controller hay API seed public.

Để maintainer làm mới bundle từ Wikimedia:

```bash
python3 scripts/fetch-foundation-audio.py
```

Script cần mạng; kiểm tra giấy phép public domain, tác giả và mô tả kana của từng file từ nguồn, MP3 header, lưu SHA-256 cùng SHA-1 của bản Ogg gốc; tái dùng download cũ hợp lệ theo manifest/checkpoint. Checkpoint `.json.tmp` không được commit hoặc đóng gói; chỉ thay manifest chính khi đủ 46 file. Gặp 429 phải đợi Retry-After, không spam hoặc lách rate limit. Nếu license/author thay đổi thì dừng để review. Review manifest và mapping, nghe kiểm tra trước khi phát hành. Khi chủ động thay nội dung bundle, dùng version mới và quyết định migration/chuyển URL; script không tự sửa âm đã được review trong DB.

Không chạy script tải nguồn trong migration hoặc mỗi lần API startup. Không xóa metadata/license khi copy bộ dữ liệu sang dự án khác. Public domain không bắt buộc attribution nhưng dự án giữ credit và provenance cho từng file để có thể kiểm tra nguồn.

## 9. Xử lý lỗi

| Lỗi | Hướng xử lý |
| --- | --- |
| OptionsValidationException | Điền R2 credentials/endpoint và PublicBaseUrl đúng; Development mới load user-secrets |
| PostgreSQL không kết nối/apply migration được | Kiểm tra connection string, mạng và quyền schema/migration |
| Missing manifest/resource/checksum | Rebuild từ checkout có đủ bundle; không sửa file nhị phân bằng tay |
| Audio mapping differs | DB đã sửa reading nền tảng; review nội dung, không force overwrite |
| R2 403 | Token đúng account/bucket/quyền Object Read & Write, thời hạn và egress restriction |
| Existing object differs | Kiểm tra object key và metadata bị thay; không tự xóa/ghi đè file khác |
| R2 409 | Có lượt upload đồng thời; đợi rồi chạy init lại |
| R2.dev 404 | Public URL Enabled/Allowed, đúng bucket, đúng object key và đã upload thành công |
| R2.dev 429 | Rate limit của development endpoint; giảm tần suất/thử lại theo response |
| DB có URL nhưng FE lỗi CORS | Sửa policy bucket theo Origin thật của FE; không sửa endpoint SDK |
| DB có URL nhưng audio không phát | Mở URL kiểm tra dữ liệu thật, định dạng MP3, browser autoplay policy và response status |

## Nguồn chính thức

- [Wikimedia Commons: bộ kana](https://commons.wikimedia.org/wiki/Category:Audio_files_of_hiragana_(set_by_Hakatanoshio117117))
- [File nguồn ka và giấy phép](https://commons.wikimedia.org/wiki/File:Ja-Ka.oga)
- [Mẫu metadata/giấy phép file i](https://commons.wikimedia.org/wiki/File:Japanese_I.mp3)
- [MediaWiki videoinfo/derivatives API](https://www.mediawiki.org/wiki/Extension:TimedMediaHandler/API)
- [PD-self](https://commons.wikimedia.org/wiki/Template:PD-self)
- [R2 public development URL](https://developers.cloudflare.com/r2/buckets/public-buckets/#public-development-url)
- [R2 CORS](https://developers.cloudflare.com/r2/buckets/cors/)
- [R2 conditional writes](https://developers.cloudflare.com/r2/api/s3/extensions/#conditional-operations-in-putobject)
- [R2 AWS SDK .NET](https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/)
