# RealityLoop Backend

.NET 10, PostgreSQL và bốn layer `Domain`, `Application`, `Infrastructure`, `Api`.
Cách chia `Common`, feature `Commands`/`Queries`, dependency injection và persistence tham khảo
[Jason Taylor CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture).

## Mục lục

- [Cấu trúc và quy tắc mở rộng](#cấu-trúc-và-quy-tắc-mở-rộng)
- [Cấu hình và chạy](#cấu-hình-và-chạy)
- [Endpoint xác thực](#endpoint)
- [Tài liệu chi tiết](#tài-liệu-chi-tiết)

## Cấu trúc và quy tắc mở rộng

```text
src/
  Domain/
    Common/                         Entity nền tảng
    Users/                          User, OAuthAccount, Session, RefreshToken và invariant
    AI/                             AiConnection, provider/status/failure enum và invariant
    LearningProfiles/               Profile theo ngôn ngữ và lựa chọn profile hiện tại
    Languages/                      Danh mục ngôn ngữ và bản dịch
    WritingSystems/                 Bộ chữ, mục chữ và cách đọc
    Pronunciation/                  Quy tắc phát âm, bản dịch và ví dụ
  Application/
    Common/
      Interfaces/                   Contract repository, transaction, OAuth, token, current user
      Exceptions/                   Lỗi use case, không phụ thuộc HTTP
    Authentication/
      Commands/
        GoogleLogin/                Đổi code, tìm/tạo user, mở session, phát token
        MicrosoftLogin/             Microsoft code → verified identity → session/token dùng chung
        FacebookLogin/              Facebook code → token inspection/profile → session/token dùng chung
        RefreshToken/               Rotate, phát hiện reuse và revoke
        RevokeRefreshToken/         Revoke bằng refresh token
        Logout/                     Logout session hiện tại
        LogoutSessions/             Logout một hoặc nhiều session được chọn
        LogoutOtherSessions/        Logout mọi session khác
        LogoutAllSessions/          Logout mọi session, kể cả caller
      Queries/
        GetGoogleAuthorizationUrl/
        GetMicrosoftAuthorizationUrl/
        GetFacebookAuthorizationUrl/
        GetCurrentUser/
        GetSessions/                Danh sách session với cursor pagination
      Models/                       Kết quả use case và DTO
      Services/                     OAuthLoginService và SessionLogoutService
    AI/
      Commands/
        CreateAiConnection/         Command và handler riêng từng file
        UpdateAiConnection/
        VerifyAiConnection/
        DeleteAiConnection/
      Queries/
        GetAiProviders/
        GetAiModels/
        GetAiConnections/
      Models/                       DTO/result, mỗi type một file
      Services/                     Provider registry và validation use case
    LearningProfiles/
      Commands/                     Tạo, sửa preferences, xóa, chọn current
      Queries/                      Options, list, detail và current
      Models/                       DTO/result
      Validation/                   Kiểm tra ID/version/cursor
    Languages/
      Queries/                      Danh mục và chi tiết ngôn ngữ
      Models/                       DTO/result
    WritingSystems/
      Queries/                      Bộ chữ, ký tự và cách đọc
      Models/                       DTO/result
    Pronunciation/
      Queries/                      Danh sách, chi tiết quy tắc và ví dụ
      Models/                       Summary/detail/page DTO
    DependencyInjection.cs
  Infrastructure/
    Authentication/
      Google/                       HTTP exchange, PKCE, state, nonce, JWKS/ID token
      Microsoft/                    HTTP exchange, PKCE, state/nonce, tenant/issuer, metadata/JWKS
      Facebook/                     HTTP exchange, state, token inspection, appsecret_proof/profile
      Tokens/                       JWT, random refresh token, SHA-256, lifetime options
    AI/
      Providers/                    HttpClient adapter OpenAI, Anthropic, Google
      Security/                     AES-GCM, key ring và fingerprint
    Storage/
      R2/                           Cloudflare R2 options, validation và S3 SDK client factory
    IdGeneration/
    Persistence/
      Entities/                     Receipt idempotency kỹ thuật
      BackgroundServices/           Dọn receipt hết hạn
      Configurations/
      Migrations/
      Repositories/
      Seeding/                      Dữ liệu nền tảng theo migration và bộ audio kana/init job
      ApplicationDbContext.cs
      UnitOfWork.cs
    DependencyInjection.cs
  Api/
    Common/
      Routing/                      Prefix controller /api/v1
      Security/                     JWT validation events, kiểm tra session
      RateLimiting/                 Options/limiter/middleware của AI setup
    Contracts/
      Authentication/               Request HTTP và validation
      AI/                           Request setup AI, mỗi type một file
      LearningProfiles/             Request profile/preferences/selection
      Languages/                    Locale/cursor cho catalog ngôn ngữ
      WritingSystems/               Locale/cursor cho bộ chữ và cách đọc
      Pronunciation/                Locale/cursor cho quy tắc và ví dụ
    Filters/                        Audit filter cho AI setup
    Controllers/                    Controller gọi handler
    Infrastructure/                 ProblemDetails/exception handler
    Services/                       CurrentUser từ HttpContext
    DependencyInjection.cs
    WebApplicationExtensions.cs
    Program.cs
    appsettings.json
docs/                               Tài liệu tích hợp và vận hành
tests/
  AiSetup.Tests/                    Console runner AI; PostgreSQL checks bật qua biến môi trường
```

Dependency: `Application → Domain`, `Infrastructure → Application/Domain`,
`Api → Application/Infrastructure`. `Domain` không phụ thuộc framework.
`Application` không biết EF Core, HTTP, Google SDK hoặc cách ký JWT.

Đặt feature mới trong `Application/<Feature>/Commands/<UseCase>` hoặc
`Queries/<UseCase>`, với command/query và handler riêng. Interface của dịch vụ ngoài đặt
ở `Common/Interfaces`, implementation đặt trong `Infrastructure`, contract HTTP đặt trong
`Api/Contracts`. Controller dùng route tương đối, ví dụ `[Route("projects")]`, tự thành
`/api/v1/projects`. Route tuyệt đối bị chặn để tránh bỏ qua prefix.

Handler được inject trực tiếp. Đây là cách tổ chức CQRS không dùng MediatR;
không có dispatcher/pipeline tự viết. Google/Microsoft/Facebook OAuth sử dụng `HttpClient`, không cài
OAuth SDK hoặc middleware OAuth. Hai package xác thực thêm vào là JWT Bearer và IdentityModel JWT.

## Cấu hình và chạy

Hướng dẫn ghép API AI cho FE: [docs/ai-setup-api.md](docs/ai-setup-api.md).
Khóa mã hóa AI bắt buộc cấu hình theo [cấu hình và vận hành](docs/ai-setup-architecture.md#ai-cấu-hình-và-vận-hành).

Các khóa nằm trong `src/Api/appsettings.json`. Khóa thật dùng user-secrets ở development
hoặc environment variables/secret store khi deploy. Các placeholder Google, Microsoft, Facebook, JWT và khóa mã hóa AI phải
được thay; API kiểm tra cấu hình khi khởi động.

```bash
dotnet user-secrets set 'ConnectionStrings:Database' 'Host=localhost;Port=5432;Database=reality_loop;Username=admin;Password=YOUR_PASSWORD' --project src/Api
dotnet user-secrets set 'Authentication:Google:ClientId' 'YOUR_CLIENT_ID.apps.googleusercontent.com' --project src/Api
dotnet user-secrets set 'Authentication:Google:ClientSecret' 'YOUR_CLIENT_SECRET' --project src/Api
dotnet user-secrets set 'Authentication:Microsoft:ClientId' 'YOUR_APPLICATION_CLIENT_ID_GUID' --project src/Api
dotnet user-secrets set 'Authentication:Microsoft:ClientSecret' 'YOUR_MICROSOFT_CLIENT_SECRET_VALUE' --project src/Api
dotnet user-secrets set 'Authentication:Facebook:AppId' 'YOUR_FACEBOOK_APP_ID' --project src/Api
dotnet user-secrets set 'Authentication:Facebook:AppSecret' 'YOUR_FACEBOOK_APP_SECRET' --project src/Api
dotnet user-secrets set 'Authentication:Jwt:SigningKey' 'YOUR_RANDOM_KEY_AT_LEAST_32_BYTES' --project src/Api
```

Tạo khóa ngẫu nhiên, ví dụ `openssl rand -base64 48`, rồi dùng kết quả cho SigningKey.
JWT của ứng dụng dùng HS256 với issuer/audience riêng; Google ID token dùng RS256 và được
kiểm tra bằng public keys của Google. Các token có mục đích và khóa ký riêng.

Trong Google Cloud Console, tạo OAuth client loại **Web application**, cấu hình consent screen/test users
và đăng ký Authorized redirect URI chính xác:

```text
http://localhost:5285/api/v1/auth/google/callback
```

Giá trị phải giống `Authentication:Google:RedirectUri`, kể cả scheme/port/path.
Flow này chỉ xin `openid email profile`, không cần quyền Google API/offline access.

`Authentication:Google:RedirectUri` đang cấu hình callback frontend. Để dùng callback
backend ở ví dụ trên, override giá trị này cho khớp URI đã đăng ký. Khi dùng tunnel,
đăng ký chính xác URL HTTPS của môi trường; origin FE cũng phải có trong CORS allowlist.

## Luồng Microsoft login

Đã có đủ redirect trực tiếp, lấy URL cho SPA và GET/POST callback. Dùng chung service
login/session/token với Google, cùng refresh/get-me/logout. Cấu hình mặc định dùng
`TenantId=common`, callback frontend `http://localhost:3000/auth/microsoft/callback`.
Đăng ký redirect ở platform **Web**, vì backend giữ secret và đổi code.

Xem [hướng dẫn Microsoft OAuth](docs/microsoft-oauth.md) để đăng ký ứng dụng, cấu hình
user-secrets, chọn tenant/account types, tích hợp SPA hoặc trả JSON trực tiếp qua browser.
Microsoft định danh bằng `tid:oid`; email là profile tùy chọn, không tự link tài khoản.
Không cần migration hoặc package mới cho flow này.

```bash
dotnet restore RealityLoop.slnx
dotnet build RealityLoop.slnx
# Nếu chưa có dotnet-ef: dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api --launch-profile http
```

API HTTP ở `http://localhost:5285`. Development cho phép HTTP loopback; môi trường triển khai
phải dùng HTTPS. Profile `https` dùng `https://localhost:7186`; khi đổi profile hãy cập nhật
RedirectUri và Google Console cho khớp.

API không tự chạy migration khi startup. `InitialCreate` tạo schema auth và các index/constraint
session/refresh token. `AddAiConnections` tạo bảng kết nối AI; `AddAiConnectionCreationReceipts`
tạo bảng receipt idempotency. Chạy `database update` vào đúng DB đích khi triển khai.

## Luồng Facebook login

Đã có redirect trực tiếp, lấy URL cho SPA và GET/POST callback. Backend đổi code,
inspection token đúng app/user/expiry, lấy profile qua `/me` với `appsecret_proof`, rồi
gọi `OAuthLoginService` để phát token ứng dụng. Refresh/get-me/logout dùng chung với
Google/Microsoft. Cấu hình ở `Authentication:Facebook`, mặc định callback frontend
`http://localhost:3000/auth/facebook/callback`, Graph API cố định `v26.0`.

Xem [hướng dẫn Facebook OAuth](docs/facebook-oauth.md) để đăng ký app, cấu hình redirect,
user-secrets và tích hợp SPA/browser. Email có thể thiếu; không tự nối tài khoản theo email.
Không cần thêm migration hoặc package.

## Luồng Google login

1. Mở `http://localhost:5285/api/v1/auth/google` trong browser. Backend tạo `state`,
   `nonce`, PKCE verifier/challenge và cookie correlation HttpOnly, rồi redirect Google.
2. Google redirect về callback với `code` và `state`. Backend đối chiếu cookie đã bảo vệ
   bằng Data Protection, kiểm tra thời hạn 10 phút, dùng client secret + PKCE verifier đổi code qua HTTP.
3. Backend kiểm tra chữ ký Google ID token, issuer, audience, expiration, nonce,
   authorized party và email verified. Danh tính tài khoản dùng `sub` của Google.
4. Tìm/tạo user, tạo session, lưu **hash** refresh token và trả JSON dưới đây.

Callback backend mặc định trả JSON trực tiếp để thử toàn bộ flow bằng browser.
Để SPA nhận JSON và điều hướng UI, dùng frontend callback như phần tiếp theo.

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "random-base64url-token",
  "tokenType": "Bearer",
  "accessTokenExpiresAt": "2026-10-11T00:15:00+00:00",
  "refreshTokenExpiresAt": "2026-10-18T00:00:00+00:00",
  "sessionId": "1234567890123456789",
  "user": {
    "id": "1234567890123456788",
    "email": "user@example.com",
    "firstName": "Example",
    "lastName": "User",
    "avatarUrl": "https://...",
    "status": "Active",
    "createdAt": "2026-10-11T00:00:00+00:00"
  }
}
```

ID được trả dưới dạng string để JavaScript không mất độ chính xác với Snowflake.
Google access/refresh token không được gửi cho client hoặc lưu vào DB.

### Tích hợp SPA

Đổi `Authentication:Google:RedirectUri` thành trang callback của frontend,
ví dụ `http://localhost:5173/auth/google/callback`, và đăng ký đúng URL này ở Google Console.
Trang này nhận code/state rồi gọi API; không gọi thẳng endpoint callback với code tự tạo.

Bắt đầu login:

```javascript
const api = 'http://localhost:5285/api/v1';
const response = await fetch(`${api}/auth/google/authorize`, { credentials: 'include' });
if (!response.ok) throw new Error('Cannot start Google login');
const { authorizationUrl } = await response.json();
window.location.assign(authorizationUrl);
```

Trang frontend callback:

```javascript
const params = new URLSearchParams(window.location.search);
const code = params.get('code');
const state = params.get('state');
// Xóa query ngay để code không xuất hiện trong navigation/referrer về sau.
window.history.replaceState({}, '', window.location.pathname);
if (params.has('error') || !code || !state) throw new Error('Google login was cancelled');
const response = await fetch(`${api}/auth/google/callback`, {
  method: 'POST',
  credentials: 'include',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ code, state, device: { deviceType: 'Desktop', deviceName: 'Web' } })
});
if (!response.ok) throw new Error('Google login failed');
const authentication = await response.json();
// Lưu token bằng cơ chế của client, sau đó điều hướng tới ứng dụng.
```

`code` phải được đổi đúng một lần, kể cả khi framework frontend chạy effect hai lần.
Callback page không tải các script/tracker không cần thiết và nên đặt Referrer-Policy: no-referrer.

Cookie correlation dùng HttpOnly, sống 10 phút, giới hạn path Google auth và được xóa ở callback.
HTTPS dùng Secure + SameSite=None cho frontend/API khác site; HTTP development dùng SameSite=Lax,
phù hợp frontend và backend cùng host `localhost`. Với domain khác site cần HTTPS;
chính sách chặn third-party cookie của browser có thể yêu cầu triển khai frontend/API cùng site.

API trả token qua JSON theo yêu cầu; giữ access token trong memory và dùng storage an toàn phù hợp
với loại client. Nếu sau này chuyển refresh token sang HttpOnly cookie, cần thiết kế thêm CSRF protection
cho refresh/revoke. Phiên bản hiện tại nhận refresh token trong JSON và bảo vệ endpoint riêng bằng token đó.

## Endpoint

Tất cả endpoint dưới `/api/v1`:

| Method | Path | Quyền | Kết quả |
| --- | --- | --- | --- |
| GET | `/auth/google` | Public | Cookie + redirect Google |
| GET | `/auth/google/authorize` | Public | Cookie + `{ authorizationUrl }` |
| GET | `/auth/google/callback` | Cookie correlation | Đổi code, trả token + user |
| POST | `/auth/google/callback` | Cookie correlation | SPA gửi `{ code, state, device? }` |
| GET | `/auth/microsoft` | Public | Cookie + redirect Microsoft |
| GET | `/auth/microsoft/authorize` | Public | Cookie + `{ authorizationUrl }` |
| GET | `/auth/microsoft/callback` | Cookie correlation | Đổi code, trả token + user |
| POST | `/auth/microsoft/callback` | Cookie correlation | SPA gửi `{ code, state, device? }` |
| GET | `/auth/facebook` | Public | Cookie + redirect Facebook |
| GET | `/auth/facebook/authorize` | Public | Cookie + `{ authorizationUrl }` |
| GET | `/auth/facebook/callback` | Cookie correlation | Đổi code, trả token + user |
| POST | `/auth/facebook/callback` | Cookie correlation | SPA gửi `{ code, state, device? }` |
| POST | `/auth/refresh` | Refresh token | `{ refreshToken }` → cặp token mới + user |
| POST | `/auth/revoke` | Refresh token | Revoke toàn bộ session, 204; idempotent |
| POST | `/auth/logout` | Bearer JWT | Revoke session hiện tại, 204 |
| GET | `/auth/me` | Bearer JWT | Thông tin user |
| GET | `/auth/sessions` | Bearer JWT | Session còn hoạt động, cursor pagination |
| POST | `/auth/logout/sessions` | Bearer JWT | `{ sessionIds: ["..."] }`, revoke 1–100 session được chọn |
| POST | `/auth/logout/others` | Bearer JWT | Revoke mọi session khác, giữ caller |
| POST | `/auth/logout/all` | Bearer JWT | Revoke tất cả session, gồm caller |

Gửi access token bằng `Authorization: Bearer <accessToken>`.
Request mẫu nằm trong `src/Api/Api.http`. Development OpenAPI:
`/api/v1/openapi/v1.json`.

Refresh token có 32 byte ngẫu nhiên từ CSPRNG; DB chỉ lưu SHA-256 hex (64 ký tự).
Mỗi lần refresh dùng token mới, token trước có `ConsumedAt`, token mới có `ParentTokenId`.
Không xóa token đã dùng trong thời gian session còn tồn tại, vì cần lịch sử để phát hiện reuse.
Client phải serialize các lần refresh và thay cả hai token sau khi refresh thành công.
Dùng lại token đã consumed sẽ revoke toàn bộ session, kể cả token mới nhất.

Login, refresh và logout đều có transaction READ COMMITTED. Gọi Google xong mới bắt đầu
transaction login; Microsoft/Facebook cũng dùng service login và transaction đó. Quy tắc khóa là user trước, session sau:

- Login/refresh/logout một session: `FOR SHARE` ở user, `FOR UPDATE` ở session khi cần.
  Các thiết bị khác nhau của cùng user có thể xử lý song song.
- Logout batch/all/others: `FOR UPDATE` ở user, sau đó đọc lại caller/ownership trước khi cập nhật.
  Khóa độc quyền phối hợp với mọi writer để không bỏ sót session do login đồng thời.
- Revoke session và toàn bộ token bằng một SQL CTE; không tải lịch sử token hoặc update từng token.
- Refresh chỉ tải token được gửi; consume → insert token thay thế → cập nhật LastSeenAt
  bằng một SQL CTE có dependency RETURNING để giữ đúng thứ tự và unique constraint.
- Mọi lỗi phải rollback toàn bộ transaction. Reuse/disabled-user persist revoke và commit rồi mới trả lỗi.

Login tạo session sau thời điểm logout-all hoàn tất là một lần đăng nhập mới hợp lệ.
JWT kiểm tra signature/issuer/audience/lifetime và session/user trong DB trên mỗi request.
Do đó logout/revoke/reuse chặn cả refresh token và JWT của session ở request tiếp theo,
dù JWT chưa hết hạn. Request đã qua bước xác thực trước khi revoke vẫn có thể hoàn tất.

Default: access 15 phút, refresh 7 ngày, session tối đa 30 ngày.
Refresh được gia hạn sau mỗi lần dùng nhưng không vượt tuổi thọ tuyệt đối của session.
User suspended/disabled bị từ chối login, JWT và refresh. Account không được tự động link
chỉ vì trùng email; email đã thuộc tài khoản khác trả 409 và cần flow link riêng có xác thực.

Lỗi dùng ProblemDetails với traceId: validation 400, authentication 401, forbidden 403,
session không tồn tại/không thuộc user 404, conflict 409, rate limit 429, OAuth provider unavailable 502, lỗi server 500 không lộ chi tiết nội bộ.
Response auth đặt no-store và no-referrer.

### Quản lý session và số lệnh SQL

`GET /auth/sessions?pageSize=20&before=<sessionId>` trả `{ items, nextCursor }`.
`before` là cursor, không phải page number. Session sắp theo ID giảm dần, lọc revoked/expired;
chỉ lấy pageSize+1 dòng, không COUNT và không OFFSET. Giới hạn pageSize 1–100.
DTO có metadata thiết bị, `isCurrent`; không có token/hash. `LastSeenAt` hiện phản ánh login/refresh.
Mỗi lần login là một session; IP/device name không phải định danh thiết bị tin cậy.

Batch logout chuẩn hóa ID trùng. Có ID không tồn tại hoặc thuộc user khác: trả cùng lỗi 404
và không thu hồi một phần. Thành công trả 204 sau commit. Session đã revoked vẫn có thể
được chọn và trả 204 nếu caller còn hoạt động. Caller tự logout rồi gọi lại bằng JWT cũ sẽ nhận 401.
`/auth/revoke` vẫn hỗ trợ logout bằng refresh token khi JWT hết hạn, chỉ tác động session sở hữu token.

Số lệnh SQL trên đường thành công, được interceptor kiểm tra:

| Request | SQL command | Ghi chú |
| --- | ---: | --- |
| Get-me | 1 | JOIN session/user đồng thời xác thực và lấy profile; DTO chỉ dùng lại trong request |
| Session list | 2 | Xác thực + một projection phân trang |
| Refresh | 4 | Khóa user + khóa session + đọc một token sau khóa + CTE rotate |
| Logout current/batch/all/others | 4 | Xác thực + khóa user + kiểm tra session/ownership + CTE revoke |
| Revoke bằng refresh token | 3 | Khóa user theo token + khóa session + CTE revoke |

Số trên gồm truy vấn xác thực khi endpoint yêu cầu JWT; không gồm lệnh điều khiển transaction
BEGIN/COMMIT/ROLLBACK. Refresh/revoke không cần gửi thêm JWT. Số lệnh không tăng theo
số token trong lịch sử hoặc số session trong batch; số dòng DB cần cập nhật vẫn tăng theo dữ liệu.
Không có Redis/cache trạng thái session. Snapshot user chỉ tồn tại trong một request đã xác thực.

## CORS và triển khai

`Cors:AllowedOrigins` là allowlist origin đầy đủ, không dùng wildcard/trailing slash.
Credentials được cho phép để gửi cookie correlation; request headers cho phép `Authorization`,
`Content-Type`, `Idempotency-Key`; response expose `Retry-After` và `Location` cho JavaScript phía FE.
Đổi allowlist sang origin frontend thật khi deploy. CORS không thay thế xác thực.

`RateLimiting:Authentication` mặc định 30 request/IP/60 giây cho controller auth, có Retry-After.
Limiter theo IP chạy trước JWT/DB validation để request đã bị giới hạn không tạo query.
Đây là limiter trong từng instance. Khi chạy nhiều instance, áp dụng thêm limiter ở gateway.

`ReverseProxy:KnownProxies` chỉ chứa IP proxy thực sự được tin cậy. Chỉ khi cấu hình danh sách
này API mới dùng X-Forwarded-For/Proto. Không tin proxy/header do client tùy ý gửi.

Persist và chia sẻ Data Protection key ring giữa các instance khi deploy để cookie Google/Microsoft/Facebook
correlation vẫn đọc được sau restart hoặc khi callback vào instance khác; dùng cùng ApplicationName.
Dùng HTTPS, signing key mạnh và cùng key trên các instance. Mỗi instance cần Snowflake GeneratorId
riêng (0–1023). User-secrets chỉ là nguồn cấu hình cho development.

## Tài liệu chi tiết

- [Khởi tạo audio kana trên R2](docs/foundation-audio-seeding.md): nguồn public domain, 46 file cho 92 reading, lệnh init migration/upload/seed và nối FE.
- [Cloudflare R2 storage setup](docs/r2-storage-setup.md): tạo bucket, credentials, cấu hình bắt buộc, domain audio, CORS và triển khai.
- [Pronunciation API cho FE](docs/pronunciation-api.md): 3 API đọc, fallback bản dịch, pagination và luồng nối LearningProfile.
- [Pronunciation: model, DB và seed](docs/pronunciation-model.md): 3 bảng quy tắc phát âm và seed nhập môn tiếng Nhật.
- [API ngôn ngữ và bảng chữ cho FE](docs/language-foundations-api.md): 6 API đọc, fallback bản dịch, pagination và luồng màn hình.
- [Ngôn ngữ và bảng chữ: model, DB, seed](docs/language-foundations-model.md): 6 bảng dùng chung, constraints và seed kana cơ bản.
- [Learning Profile API cho FE](docs/learning-profiles-api.md): 8 endpoint, payload/response, current selection, version và transaction.
- [Ghép API setup AI cho FE](docs/ai-setup-api.md): luồng màn hình, 7 endpoint, payload/response, TypeScript, xử lý lỗi và retry.
- [AI setup: model, DB và vận hành](docs/ai-setup-architecture.md): schema, migration, mã hóa, transaction, concurrency, rate limit và kiểm tra.
- [Microsoft OAuth](docs/microsoft-oauth.md).
- [Facebook OAuth](docs/facebook-oauth.md).

## Tài liệu tham khảo

- [Google OAuth cho web server](https://developers.google.com/identity/protocols/oauth2/web-server)
- [Google OpenID Connect và kiểm tra ID token](https://developers.google.com/identity/openid-connect/openid-connect)
- [Microsoft authorization code flow](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow)
- [Microsoft ID token claims](https://learn.microsoft.com/en-us/entra/identity-platform/id-token-claims-reference)
- [Facebook manual login flow](https://developers.facebook.com/docs/facebook-login/guides/advanced/manual-flow/)
- [ASP.NET Core JWT Bearer](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0)
- [ASP.NET Core CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0)
