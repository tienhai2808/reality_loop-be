# Cloudflare R2 qua AWS S3 SDK

## 1. Phạm vi và kiến trúc

Storage là thành phần bắt buộc của dự án, không có cờ `Enabled`. `Storage:R2` được bind và validate khi ứng dụng khởi động; thiếu cấu hình hoặc còn placeholder thì startup thất bại, báo tên field nhưng không in secret.

- `Infrastructure/Storage/R2/R2Options.cs`: cấu hình.
- `R2OptionsValidator.cs`: kiểm tra endpoint, bucket, credentials, timeout và retry.
- `R2ClientFactory.cs`: tạo `IAmazonS3` singleton; DI quản lý vòng đời/dispose.
- `Infrastructure/DependencyInjection.cs`: đăng ký options và client.
- `Api/appsettings.json`: cấu hình mẫu; credentials thật đặt ở user-secrets hoặc secret store của môi trường chạy.

Giữ nguyên bốn layer. `AWSSDK.S3` chỉ nằm ở Infrastructure. Đã có chế độ khởi tạo bộ audio kana qua lệnh riêng, xem [hướng dẫn seed audio](foundation-audio-seeding.md). Chưa có API upload hoặc port Application cho nghiệp vụ upload người dùng.

Client dùng endpoint R2, ký request với region `auto`, path-style, HTTPS, retry Standard và checksum `WHEN_REQUIRED` của AWS SDK v4. Không dùng AWS IAM/default credential chain vì đích thực tế là R2. Client được dùng lại giữa các request, không tạo lại cho mỗi HTTP request. Options validation chỉ kiểm tra cấu hình cục bộ; không xác minh mạng, token còn hiệu lực hay bucket tồn tại. API startup không phát sinh request tới R2; lệnh khởi tạo audio mới upload/xác minh file.

## 2. Tạo bucket

1. Đăng nhập Cloudflare Dashboard và chọn đúng **account** sở hữu dự án.
2. Mở **R2 Object Storage**. Hoàn tất kích hoạt R2/billing nếu dashboard yêu cầu; xem giá hiện hành trên Cloudflare trước khi xác nhận.
3. Chọn **Create bucket**.
4. Ví dụ đặt tên `realityloop-media-dev`; production dùng bucket khác như `realityloop-media-prod` để tách dữ liệu và credentials.
5. Tên bucket dài 3–63 ký tự, chỉ chữ thường, số và dấu `-`; không bắt đầu/kết thúc bằng `-`.
6. Với audio thường xuyên đọc, bắt đầu với storage class **Standard**. Chọn location/jurisdiction theo nhu cầu thực tế; location không phải AWS signing region.
7. Tạo bucket, mở trang bucket để kiểm tra tên và settings.

Bucket ban đầu private. Chưa cần mở public để backend truy cập qua SDK.

## 3. Tạo credentials cho backend

1. Từ trang R2, mở **Manage R2 API Tokens**.
2. Tạo token cho ứng dụng. Nếu account hỗ trợ và bạn có quyền, ưu tiên **Account API Token** cho deployment để không phụ thuộc vòng đời tài khoản cá nhân.
3. Đặt tên rõ môi trường, ví dụ `realityloop-backend-dev`.
4. Chọn quyền **Object Read & Write** và giới hạn vào đúng bucket vừa tạo. Backend không cần quyền quản trị toàn bộ account/bucket.
5. Chọn thời hạn/IP restriction phù hợp với hạ tầng chạy. IP restriction phải bao gồm IP egress thực tế của backend.
6. Tạo token, lưu **Access Key ID** và **Secret Access Key** vào password manager/secret store ngay. Secret chỉ hiện lúc tạo; mất thì tạo credentials mới.
7. Copy **S3 API endpoint** từ dashboard. Bucket có jurisdiction có thể có endpoint khác; dùng đúng endpoint dashboard cung cấp.

Không nhầm Cloudflare API bearer token với cặp credentials S3. Không dùng AWS access key hay Zone ID. Nếu cần rotate: tạo token mới cùng scope, cập nhật secret deployment, restart/redeploy toàn bộ instance, xác minh rồi revoke token cũ.

## 4. Phân biệt các giá trị

| Giá trị | Ví dụ | Dùng làm gì |
| --- | --- | --- |
| ServiceUrl | `https://<ACCOUNT_ID>.r2.cloudflarestorage.com` | Backend gọi S3 API, copy nguyên endpoint từ dashboard |
| BucketName | `realityloop-media-dev` | Bucket truyền trong request SDK |
| AccessKeyId / SecretAccessKey | Credentials S3 của R2 | Chỉ lưu phía backend |
| Signing region | `auto` | Đã cố định trong client; không phải `ap-southeast-1` |
| URL public | `https://pub-YOUR_BUCKET_ID.r2.dev` | Public Bucket URL để FE đọc audio, bật trong bucket Settings |
| Object key | `audio/ja/hiragana/a-v1.mp3` | Đường dẫn object bên trong bucket, phân biệt hoa/thường |

`ServiceUrl` không chứa bucket/path/query và không phải domain public hay URL `r2.dev`. Public URL ở trên là placeholder; phải copy URL thật từ dashboard. Public URL tương ứng là `https://pub-YOUR_BUCKET_ID.r2.dev/audio/ja/hiragana/a-v1.mp3`, không chèn thêm tên bucket vào path.

## 5. Cấu hình local bằng user-secrets

Chạy tại root backend. API đã có `UserSecretsId`, không cần init lại. Thay toàn bộ giá trị mẫu trước khi chạy:

```bash
dotnet user-secrets set 'Storage:R2:ServiceUrl' 'https://YOUR_ACCOUNT_ID.r2.cloudflarestorage.com' --project src/Api
dotnet user-secrets set 'Storage:R2:BucketName' 'realityloop-media-dev' --project src/Api
dotnet user-secrets set 'Storage:R2:AccessKeyId' 'YOUR_R2_ACCESS_KEY_ID' --project src/Api
dotnet user-secrets set 'Storage:R2:SecretAccessKey' 'YOUR_R2_SECRET_ACCESS_KEY' --project src/Api
```

Dùng endpoint thật đã copy, không giữ chữ `YOUR_ACCOUNT_ID`. Lệnh chứa secret có thể lưu trong shell history; nhập trên máy tin cậy và quản lý history phù hợp. User-secrets dành cho development, không phải kho secret được mã hóa cho production. Không commit credentials vào appsettings, `.env`, README hay Docker image. Không đưa key vào FE.

Các giá trị mặc định còn lại trong appsettings:

```json
{
  "Storage": {
    "R2": {
      "ServiceUrl": "__SET_R2_S3_API_ENDPOINT__",
      "BucketName": "__SET_R2_BUCKET_NAME__",
      "AccessKeyId": "__SET_R2_ACCESS_KEY_ID__",
      "SecretAccessKey": "__SET_R2_SECRET_ACCESS_KEY__",
      "TimeoutSeconds": 60,
      "MaxErrorRetry": 2
    }
  }
}
```

Timeout hợp lệ 1–300 giây; retry 0–5. Đây là cấu hình SDK, không thay thế CancellationToken/deadline của nghiệp vụ tương lai. Bắt đầu bằng defaults, chỉ điều chỉnh sau khi đo request thực tế.

Chạy API với môi trường Development để host load user-secrets:

```bash
dotnet run --project src/Api --environment Development
```

Các cấu hình bắt buộc khác của dự án (DB, Redis, OAuth, JWT...) vẫn cần đầy đủ. R2 config đúng không tự cấu hình các thành phần đó. Không xóa cấu hình local đang dùng.

## 6. Deploy production

Trong secret/configuration settings của hosting, container orchestrator hoặc service manager, cung cấp:

```text
ASPNETCORE_ENVIRONMENT=Production
Storage__R2__ServiceUrl=https://YOUR_ACCOUNT_ID.r2.cloudflarestorage.com
Storage__R2__BucketName=realityloop-media-prod
Storage__R2__AccessKeyId=<R2_ACCESS_KEY_ID>
Storage__R2__SecretAccessKey=<R2_SECRET_ACCESS_KEY>
Storage__R2__TimeoutSeconds=60
Storage__R2__MaxErrorRetry=2
```

Dấu `__` ánh xạ sang `:` trong .NET. Dùng secret riêng cho production và bucket production. Không trông chờ user-secrets trên server Production. Inject secret tại runtime qua nền tảng hosting; không bake vào image hoặc truyền trong Dockerfile build args.

Sau khi cập nhật cấu hình, restart/redeploy các instance để client singleton nhận credentials mới. Cho phép backend outbound HTTPS tới endpoint R2. Kiểm tra startup log không còn OptionsValidationException; không log option object chứa key. Thành công ở bước startup chưa chứng minh token truy cập được bucket vì setup không gọi R2 khi khởi động.

## 7. Dùng public URL r2.dev để phát audio

Không cần custom domain. Mở bucket → **Settings** → **Public Development URL** → **Enable**, xác nhận theo dashboard rồi copy Public Bucket URL dạng `https://pub-<ID>.r2.dev`. Kiểm tra Public URL Access là **Allowed**.

Theo lựa chọn hiện tại, dùng URL này cho audio. Cloudflare xác định r2.dev là endpoint development có rate limit, không hỗ trợ các tính năng Cloudflare Cache/WAF như custom domain. Không cần thiết lập DNS/media subdomain cho luồng hiện tại.

Public URL cho phép đọc object khi biết key. CORS không bảo vệ file riêng tư. Dùng bucket riêng chỉ chứa tài nguyên được phép public; không bỏ dữ liệu cá nhân vào đây.

Endpoint SDK vẫn là ServiceUrl R2. Với lệnh seed audio, đặt `FoundationAudioSeed:PublicBaseUrl` bằng URL r2.dev vừa copy, xem [các lệnh cấu hình và khởi tạo](foundation-audio-seeding.md).

## 8. CORS cho FE đọc audio

Backend gọi R2 bằng SDK không chịu browser CORS. Nhưng FE fetch/Web Audio đọc từ URL r2.dev cần policy của **bucket R2**, độc lập với CORS của API .NET.

Mở bucket → **Settings** → **CORS Policy** → **Add/Edit** → **JSON**. Dán policy dạng dashboard/S3 dưới đây; giữ đúng origin FE thực tế, bỏ localhost ở production nếu không cần:

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

Origin là địa chỉ trang FE: scheme + host + port, không có path hay dấu `/` cuối. Nếu FE chạy origin khác, thay bằng origin trong DevTools. Không thêm PUT/DELETE khi FE chỉ đọc. Browser tự xử lý preflight OPTIONS, không thêm OPTIONS vào AllowedMethods. Không gửi Authorization của API app sang URL media public.

Wrangler và Cloudflare REST API có schema CORS khác; không copy nguyên JSON này sang chúng. Sau khi đổi policy, chờ propagation; nếu custom domain đã cache object trước đó, purge cache tương ứng để nhận CORS header mới.

## 9. Kiểm tra thủ công sau khi cấu hình Cloudflare

### File public và CORS

Có thể upload một MP3 được phép sử dụng bằng dashboard, key ví dụ `audio/ja/hiragana/a-v1.mp3`. Đây là cách kiểm tra thủ công; bộ kana có lệnh upload/seed riêng trong tài liệu seed audio. Đặt Content-Type đúng định dạng (`audio/mpeg` cho MP3, `audio/ogg` cho Ogg, `audio/wav` cho WAV).

```bash
curl -I 'https://pub-YOUR_BUCKET_ID.r2.dev/audio/ja/hiragana/a-v1.mp3'
curl -I -H 'Origin: https://my-client.tienhai.click' 'https://pub-YOUR_BUCKET_ID.r2.dev/audio/ja/hiragana/a-v1.mp3'
curl -D - -o /dev/null -H 'Range: bytes=0-1023' 'https://pub-YOUR_BUCKET_ID.r2.dev/audio/ja/hiragana/a-v1.mp3'
```

Kỳ vọng file tồn tại trả 200; request có Origin hợp lệ trả Access-Control-Allow-Origin phù hợp; request Range hợp lệ trả 206/Content-Range nếu object và đường truyền hỗ trợ. URL ở ví dụ chỉ dùng được sau khi bật Public Development URL và upload object. curl không có Origin sẽ không chứng minh CORS hoạt động.

FE có thể dùng `<audio controls src="https://pub-YOUR_BUCKET_ID.r2.dev/audio/ja/hiragana/a-v1.mp3"></audio>`. Với fetch/Web Audio/crossOrigin, kiểm tra thêm DevTools Network. Không gọi endpoint S3 ký request trực tiếp từ FE bằng secret.

### Credentials S3 (tùy chọn, cần AWS CLI đã cài)

```bash
aws configure --profile realityloop-r2
```

Nhập R2 Access Key ID/Secret Access Key, region `auto`, output `json`. Đây là profile của CLI, backend vẫn dùng Storage:R2 trong config, không đọc profile này.

```bash
aws s3api list-objects-v2 --bucket realityloop-media-dev --max-keys 1 --region auto --endpoint-url https://YOUR_ACCOUNT_ID.r2.cloudflarestorage.com --profile realityloop-r2
```

Lệnh chỉ đọc và kiểm tra quyền với đúng bucket; không cần quyền liệt kê mọi bucket. Không dán output chứa thông tin nhạy cảm vào ticket. Public URL hoạt động không chứng minh S3 credentials đúng, và ngược lại.

## 10. Cache và URL audio

Với r2.dev, không dựa vào Cloudflare edge-cache features của custom domain. Seed đặt Cache-Control dài cho browser vì object key chứa hash và không overwrite. Khi file thay đổi phải dùng key mới. Không giả định Cache-Control đồng nghĩa r2.dev đã có CDN cache tùy chỉnh.

Lệnh khởi tạo audio ghép PublicBaseUrl và object key, ghi vào AudioUrl hiện có. Xem [hướng dẫn seed](foundation-audio-seeding.md) để chạy migration/upload/ghi DB và kiểm tra FE.

## 11. Upload qua SDK

Uploader của bộ seed đặt UseChunkEncoding=false, DisablePayloadSigning=true và DisableDefaultChecksumValidation=true trên PutObjectRequest theo tương thích R2, kèm HTTPS và SHA-256 kiểm tra bộ file trước upload. Client SDK v4 đặt checksum WHEN_REQUIRED.

Uploader này phục vụ bộ dữ liệu bootstrap đã review, không phải API upload file người dùng. Các use case upload khác cần validation, giới hạn dung lượng và phân quyền phù hợp khi được triển khai.

## 12. Chẩn đoán

| Hiện tượng | Kiểm tra |
| --- | --- |
| Startup báo Storage:R2 | Key thiếu/placeholder, endpoint không hợp lệ, bucket sai định dạng; user-secrets chỉ load trong Development |
| SignatureDoesNotMatch | Cặp R2 credentials, endpoint đúng jurisdiction, clock server, ký region auto; proxy không sửa host/path |
| AccessDenied | Scope token đúng bucket, chưa hết hạn/revoke, IP egress đúng restriction |
| NoSuchBucket | Tên bucket, đúng account/endpoint và môi trường |
| NotImplemented/checksum/streaming lúc upload | Cấu hình request upload theo tài liệu R2; xem uploader và hướng dẫn seed audio |
| Public URL chưa vào được | Public Development URL đang Enabled/Allowed trên đúng bucket |
| URL 404 | Object key phân biệt hoa/thường, file đã upload, không thêm tên bucket vào public path |
| Browser CORS | Origin FE thực tế, policy R2, headers/method, purge cache sau đổi policy |
| Backend chạy nhưng chưa biết kết nối được chưa | Startup chỉ validate cấu hình; dùng kiểm tra S3 CLI riêng ở trên |

## Nguồn chính thức

- [Tạo bucket và quy tắc tên](https://developers.cloudflare.com/r2/buckets/create-buckets/)
- [R2 S3 API setup](https://developers.cloudflare.com/r2/get-started/s3/)
- [Authentication và R2 credentials](https://developers.cloudflare.com/r2/api/tokens/)
- [Public buckets, custom domain, r2.dev](https://developers.cloudflare.com/r2/buckets/public-buckets/)
- [CORS dashboard và xử lý cache](https://developers.cloudflare.com/r2/buckets/cors/)
- [AWS SDK .NET với R2](https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/)
- [AWS CLI với R2](https://developers.cloudflare.com/r2/examples/aws/aws-cli/)
