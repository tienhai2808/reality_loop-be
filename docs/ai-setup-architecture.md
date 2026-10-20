# AI setup: model, DB và vận hành

[README dự án](../README.md) · [Hướng dẫn ghép API cho FE](ai-setup-api.md)

## AI: model và DB

Một user sở hữu nhiều `AiConnection`; mỗi connection có một provider, một key mã hóa và
đúng một model. Có thể thêm nhiều connection cùng provider. Connection là aggregate riêng,
liên kết bằng UserId; không tải cả collection khi đọc User. FK tới users cascade delete.
Index `(user_id, sort_order, id)` phục vụ ownership/list/order, không thêm index user_id trùng.

### Các cột

| Cột | Kiểu PostgreSQL | Quy tắc |
| --- | --- | --- |
| id | bigint | PK, Snowflake từ application, dương |
| user_id | bigint | FK, dương, bắt buộc |
| provider | varchar(32) | Tên enum hợp lệ, bắt buộc |
| display_name | varchar(150) | Không rỗng; factory tự dùng tên provider nếu bỏ trống |
| encrypted_api_key | text | Ciphertext bắt buộc |
| api_key_last4 | varchar(4) | Chính xác 4 ký tự ASCII có thể hiển thị, không chứa whitespace |
| model_id | varchar(200) | Một ID model, không rỗng; giữ nguyên chữ hoa/thường |
| sort_order | integer | Không âm, mặc định DB 0; application gán thứ tự khi thêm |
| is_enabled | boolean | Mặc định true |
| status | varchar(32) | Enum trạng thái; mặc định Ready |
| retry_after_at | timestamptz | Nullable, chỉ có khi CoolingDown |
| last_failure_code | varchar(32) | Nullable, chỉ có khi CoolingDown/NeedsAttention |
| version | bigint | Concurrency token, khởi tạo 1 và phải dương |
| created_at | timestamptz | Bắt buộc, DB có default now() |
| updated_at | timestamptz | Bắt buộc, không trước created_at |

Độ dài và invariant được kiểm tra ở Domain và ràng buộc ở DB. Enums lưu tên chuỗi theo
quy ước hiện tại của project. Khi thêm giá trị enum cần tạo migration cập nhật CHECK constraint.

### Quy tắc entity và persistence

Domain kiểm tra invariant; DB có CHECK constraint bảo vệ ID/version dương, timestamp, enum,
key hint và tổ hợp status/failure/retry time. Enum lưu tên chuỗi; thêm enum cần migration
cập nhật constraint. `Version` là concurrency token; repository bảo vệ ghi/xóa bằng version.

`Create` chỉ nhận ciphertext sau probe. `UpdateSettings` không tự xác nhận health. `ReplaceApiKey`
chỉ nhận ciphertext/hint mới. `MarkReady` xóa failure/retry; `MarkCoolingDown` yêu cầu lỗi tạm thời
và thời điểm retry tương lai; `MarkNeedsAttention` không có retry time. `RecordVerification` buộc
version tăng kể cả health không đổi. Một request có thể tăng version nhiều lần; FE dùng nguyên
version DTO trả về. `CanUse(now)` yêu cầu đang bật và Ready hoặc cooldown đã hết.

Receipt idempotency không thuộc Domain: entity/mapping/cleanup ở Infrastructure/Persistence;
Application chỉ biết reservation/result qua interface. Không trả entity/ciphertext ra API.

## AI: cấu hình và vận hành

Phần hiện tại cung cấp setup AI. Luồng thực thi tác vụ và tự chuyển sang connection kế tiếp
khi quota/lỗi sẽ sử dụng các connection này; chưa có API thực thi/fallback tác vụ trong phạm vi này.

### Khóa mã hóa và cấu hình provider

Sinh **hai khóa khác nhau**, mỗi khóa 32 byte ngẫu nhiên: chạy `openssl rand -base64 32`
hai lần, dùng hai kết quả riêng cho encryption và fingerprint. Cấu hình trước khi khởi động:

```bash
dotnet user-secrets set 'AI:KeyProtection:ActiveKeyId' 'primary' --project src/Api
dotnet user-secrets set 'AI:KeyProtection:Keys:primary' 'BASE64_ENCRYPTION_KEY' --project src/Api
dotnet user-secrets set 'AI:KeyProtection:FingerprintKey' 'DIFFERENT_BASE64_FINGERPRINT_KEY' --project src/Api
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Placeholder trong `appsettings.json` phải được thay; options được validate lúc startup.
Giữ key ring/fingerprint giống nhau trên mọi instance và tồn tại qua restart.
User-secrets dùng development; production dùng secret store/environment overrides,
ví dụ `AI__KeyProtection__Keys__primary`. Không dùng API key AI của user làm khóa mã hóa server.

AES-256-GCM dùng nonce ngẫu nhiên, tag 16 byte và associated data gắn user/connection.
Ciphertext envelope gồm version/key ID. Rotate bằng cách triển khai key ring có cả khóa cũ/mới
trên mọi instance, rồi đổi ActiveKeyId. Giữ khóa cũ đến khi dữ liệu cũ đã được mã hóa lại.
Thêm/thay API key sẽ dùng active key mới; chưa có job mã hóa lại toàn bộ dữ liệu.
Giữ fingerprint key ổn định trong thời hạn idempotency để retry vẫn khớp receipt.

Provider adapter dùng `HttpClient`, fixed host, không nhận URL từ client, không theo redirect,
không dùng cookie, không tự retry inference. OpenAI kiểm tra qua Responses API, Anthropic qua
Messages, Google Gemini qua generateContent. Catalog có deadline tổng 45 giây, tối đa 10 trang;
HTTP timeout 20 giây/request, connect timeout 5 giây, response buffer tối đa 1 MiB.

Khi catalog provider trả 403, có thể dùng model ID do operator xác nhận và cấu hình tại:

```text
AI:Providers:OpenAiFallbackModels
AI:Providers:AnthropicFallbackModels
AI:Providers:GoogleFallbackModels
```

Mặc định các danh sách rỗng. Fallback catalog trả `isFallback=true`; tạo/sửa connection vẫn
phải probe. Không cache model catalog chung giữa key/user. OpenAI chỉ liệt kê các họ text model
adapter hỗ trợ; Google chỉ liệt kê model có generateContent.

### Transaction, concurrency và số truy vấn

- Ownership được lấy từ JWT, query lọc `UserId` và connection ID. ID của user khác trả cùng 404.
- Tạo dùng upsert reserve receipt; HTTP probe/mã hóa ở ngoài transaction. Insert connection và
  complete receipt nằm trong một transaction ngắn, thất bại rollback cả hai.
- Receipt kỹ thuật `ai_connection_creations` thuộc Infrastructure/Persistence, PK `(user_id, request_id)`,
  HMAC fingerprint, lease token 2 phút và DTO response 24 giờ. Receipt vẫn replay khi connection đã xóa.
  Completion phải khớp lease token và lease còn hạn; worker cũ không ghi đè lease mới.
- Process crash có thể khiến probe phải gọi lại sau lease expiry; không cam kết provider chỉ tính phí một lần.
  Lỗi tạo giải phóng lease với timeout riêng; cleanup thất bại thì lease tự hết hạn.
- Background service dọn tối đa 1000 receipt hết hạn mỗi 5 phút bằng `SKIP LOCKED`.
- PATCH/verify đọc snapshot trước HTTP rồi ghi có điều kiện version. Repository chuyển EF concurrency
  exception thành `ConflictException` của Application. API không xử lý trực tiếp EF exception.
- PATCH không đổi dữ liệu vẫn kiểm tra version bằng conditional write. Verify luôn tăng version.
  Key/model mới probe thất bại không ghi cấu hình mới; verify thất bại ghi health state của cấu hình đã kiểm tra.
- DELETE thành công dùng một câu lệnh có owner/version; chỉ query thêm khi cần phân biệt 404 và 409.
- Connection mặc định SortOrder=0; Snowflake ID phân định thứ tự, không query MAX/COUNT để cấp thứ tự.
  List dùng projection không lấy ciphertext, cursor pagination không COUNT/OFFSET.

Số lệnh trên đường thành công **theo code**, chưa đo lại bằng PostgreSQL thật cho phần AI.
Không gồm query xác thực JWT, transaction BEGIN/COMMIT và HTTP provider:

| Thao tác | Lệnh DB |
| --- | ---: |
| Provider catalog/models | 0 |
| Connection list | 1 |
| Tạo mới | 3: reserve, insert, complete receipt |
| Replay tạo đã hoàn tất | 2: reserve attempt, đọc receipt |
| PATCH/verify | 2: đọc snapshot, conditional update |
| DELETE | 1 |

### Rate limit và log

`RateLimiting:AiSetup` mặc định IP=120/user=60 request mỗi 60 giây, tối đa 2 POST/PATCH
đồng thời cho mỗi user. IP limiter chạy trước JWT/DB; user limiter chạy sau xác thực/authorization.
Limiter trong process chỉ giới hạn từng instance; nhiều instance cần thêm limiter tổng ở gateway
hoặc cơ chế dùng chung. Ownership/version/idempotency vẫn được bảo vệ bởi DB giữa các instance.

Audit chỉ ghi operation/user/connection/outcome/duration/traceId. Named HttpClient tắt logger.
Không log body, key, ciphertext, credential headers hoặc raw provider response. Nếu bổ sung APM/body
logging, phải tiếp tục loại các trường credential/body setup khỏi telemetry.

## Kiểm tra phần AI

Runner hiện có: `tests/AiSetup.Tests`, không phải HTTP end-to-end suite.

```bash
dotnet build RealityLoop.slnx
dotnet run --project tests/AiSetup.Tests
```

Mặc định runner kiểm tra AES/AAD, HTTP adapter giả lập, EF model metadata, handler,
ownership/idempotency/stale verification qua fake repository. Lần chạy trước khi chỉnh bố cục:
**50 kiểm tra passed; PostgreSQL checks skipped**. Không suy ra đã kiểm tra query/transaction
trên DB thật hoặc credential provider thật từ kết quả mặc định này.

Để bật kiểm tra PostgreSQL, chuẩn bị DB riêng có tên chứa `ai_tests`:

```bash
AI_TEST_DATABASE='Host=127.0.0.1;Port=55439;Database=reality_loop_ai_tests;Username=ai_test;Password=integration-only' \
  dotnet run --project tests/AiSetup.Tests
```

Runner áp dụng migrations và tạo/xóa user riêng của lần chạy; không dùng DB ứng dụng.
PostgreSQL checks chỉ chạy khi đặt biến trên. Chưa chạy nhóm này trong môi trường hiện tại.
Cần kiểm tra thêm provider thật với model/key của môi trường trước khi xác nhận vận hành production.

