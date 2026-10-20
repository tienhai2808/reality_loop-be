# Ghép API setup AI cho FE

[README dự án](../README.md) · [Model, DB và vận hành](ai-setup-architecture.md)

**Base URL:** `<backend-origin>/api/v1`. Tất cả endpoint bên dưới yêu cầu
`Authorization: Bearer <accessToken>` của ứng dụng. Không dùng Google/Microsoft/Facebook token
hoặc API key AI làm Bearer token. Response dùng camelCase, enum dùng chuỗi.

## Luồng màn hình kết nối AI

1. Mở màn hình: gọi song song `GET /ai/providers` và `GET /ai/connections`.
2. User chọn provider, nhập API key → `POST /ai/providers/{provider}/models` để lấy model.
   Đổi provider/key thì bỏ danh sách model và lựa chọn model cũ. Bước này chưa lưu connection.
3. User chọn **một model**, nhập tên tùy chọn → `POST /ai/connections` với UUID mới trong
   `Idempotency-Key`. Thành công thêm connection trả về vào UI, xóa API key khỏi form.
4. Mỗi connection có đổi tên, bật/tắt, đổi model/key, kiểm tra lại và xóa. Dùng `version`
   mới nhất từ server khi sửa/verify/xóa. Thay row bằng DTO trả về sau PATCH/verify.
5. Muốn thêm AI khác thì lặp lại bước 2–3, kể cả cùng provider. Không có lựa chọn AI theo
   tác vụ hoặc nhiều model trên một connection.

Key chỉ giữ trong bộ nhớ tạm của form; không đưa vào URL, localStorage, sessionStorage,
analytics hoặc log lỗi/request. Tránh gửi lại khi component render hoặc tự retry probe.
Kiểm tra model lúc thêm/đổi key/model/verify có gọi inference với prompt cố định, output nhỏ,
có thể tiêu tốn token/phí của key người dùng.

## Danh sách endpoint

Các path trong bảng đã gồm `/ai`; ghép với base URL `/api/v1`.

| Method | Path | Request | Thành công |
| --- | --- | --- | --- |
| GET | `/ai/providers` | Không body | 200, `AiProviderDto[]` |
| POST | `/ai/providers/{provider}/models` | `{ apiKey }` | 200, `AiModelPage` |
| GET | `/ai/connections?pageSize=20&after=...` | Không body | 200, `AiConnectionPage` |
| POST | `/ai/connections` | `{ provider, apiKey, modelId, displayName? }` + `Idempotency-Key` | 201, `AiConnectionDto` |
| PATCH | `/ai/connections/{id}` | `{ version, displayName?, modelId?, apiKey?, isEnabled? }` | 200, `AiConnectionDto` |
| POST | `/ai/connections/{id}/verify` | `{ version }` | 200, `AiConnectionDto` |
| DELETE | `/ai/connections/{id}?version=...` | Không body | 204, body rỗng |

Không có wrapper `data` quanh response. API không nhận `userId`, `sortOrder`, health status,
ciphertext hoặc base URL provider từ FE. Provider của connection không thể đổi; tạo connection mới
nếu cần provider khác. Tất cả response setup đặt `Cache-Control: no-store`.

## Kiểu dữ liệu response

```ts
export type AiProvider = 'OpenAI' | 'Anthropic' | 'Google';
export type AiConnectionStatus = 'Ready' | 'CoolingDown' | 'NeedsAttention';
export type AiFailureCode =
  | 'InvalidApiKey' | 'QuotaExceeded' | 'RateLimited'
  | 'ProviderUnavailable' | 'ModelUnavailable';

export interface AiProviderDto { id: AiProvider; name: string }
export interface AiModelDto { id: string; name: string }
export interface AiModelPage { items: AiModelDto[]; isFallback: boolean }
export interface AiConnectionPage {
  items: AiConnectionDto[];
  nextCursor: string | null;
}
export interface AiConnectionDto {
  id: string;                  // Snowflake bigint: giữ string, không parseInt/Number
  provider: AiProvider;
  displayName: string;
  apiKeyLast4: string;          // chỉ hint, không thể dùng làm API key
  modelId: string;
  sortOrder: number;           // server quản lý
  isEnabled: boolean;          // lựa chọn bật/tắt của user
  status: AiConnectionStatus;  // tình trạng hoạt động do server ghi nhận
  retryAfterAt: string | null; // ISO 8601 UTC
  lastFailureCode: AiFailureCode | null;
  version: number;             // gửi lại nguyên giá trị, không tự cộng
  createdAt: string;           // ISO 8601 UTC
  updatedAt: string;           // ISO 8601 UTC
}
```

`GET /ai/providers` trả trực tiếp:

```json
[
  { "id": "OpenAI", "name": "OpenAI" },
  { "id": "Anthropic", "name": "Anthropic" },
  { "id": "Google", "name": "Google" }
]
```

Lấy model gửi key trong JSON body:

```http
POST /api/v1/ai/providers/OpenAI/models
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "apiKey": "<key người dùng vừa nhập>" }
```

Response có dạng dưới đây; ID chỉ minh họa, FE dùng nguyên `id` thực tế server trả về:

```json
{ "items": [{ "id": "model-id-from-provider", "name": "Model name" }], "isFallback": false }
```

`isFallback=true` nghĩa là danh sách model do operator cấu hình khi catalog bị từ chối quyền;
không chứng minh key gọi được model. POST tạo vẫn kiểm tra key/model trước khi lưu.
Danh sách có thể rỗng; lúc đó không cho submit kết nối và hướng dẫn kiểm tra quyền/key.

## Tạo connection và retry an toàn

```http
POST /api/v1/ai/connections
Authorization: Bearer <accessToken>
Content-Type: application/json
Idempotency-Key: 47ef1522-59e1-4e31-8883-0d875e218bb7

{
  "provider": "OpenAI",
  "apiKey": "<key người dùng vừa nhập>",
  "modelId": "<id model đã chọn>",
  "displayName": "AI cá nhân"
}
```

201 trả một `AiConnectionDto`, ví dụ:

```json
{
  "id": "123456789012345678",
  "provider": "OpenAI",
  "displayName": "AI cá nhân",
  "apiKeyLast4": "abcd",
  "modelId": "model-id-from-provider",
  "sortOrder": 0,
  "isEnabled": true,
  "status": "Ready",
  "retryAfterAt": null,
  "lastFailureCode": null,
  "version": 1,
  "createdAt": "2026-10-18T08:00:00+00:00",
  "updatedAt": "2026-10-18T08:00:00+00:00"
}
```

- Tạo UUID bằng `crypto.randomUUID()` một lần cho một lần submit. UUID không được rỗng.
- Mất mạng/chưa biết kết quả: retry **cùng UUID và cùng payload**. Giữ payload trong memory
  trong lúc thao tác còn pending. Không sinh UUID mới cho mỗi lần retry.
- Trong 24 giờ, retry cùng UUID và cùng nội dung đã chuẩn hóa trả lại DTO tạo ban đầu,
  không thêm bản ghi/probe lần nữa. DTO replay có thể cũ nếu connection đã sửa/xóa;
  reload danh sách sau khi phục hồi một thao tác cũ.
- UUID cũ với nội dung khác → 409. Khi user đổi form để thực hiện thao tác mới, sinh UUID mới.
- 409 có `code="CreationInProgress"`: chờ theo `Retry-After` rồi retry cùng UUID/payload.
  Disable nút submit khi đang chờ; không tạo vòng lặp retry không giới hạn.
- Sau 24 giờ hết bảo đảm replay: tải danh sách và để user xác nhận thao tác mới trước khi tạo lại.

## Sửa, verify, xóa và phân trang

Đổi tên/tắt sử dụng không gọi provider:

```http
PATCH /api/v1/ai/connections/123456789012345678
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "version": 1, "displayName": "AI dự phòng", "isEnabled": false }
```

Đổi model/key gửi cùng endpoint với `version` mới nhất và field cần đổi. Field bỏ qua hoặc
`null` giữ nguyên; chuỗi rỗng không phải cách xóa key/model. `displayName` trắng đưa tên về
mặc định provider. Key/model mới phải qua probe; lỗi thì cấu hình cũ được giữ nguyên.
Không điền `apiKeyLast4` vào ô key hoặc gửi lại nó khi chỉnh sửa.

Verify: `POST /ai/connections/{id}/verify` với `{ "version": <version hiện tại> }`.
**HTTP 200 của verify nghĩa là đã ghi nhận kết quả; FE phải đọc `status` để hiển thị:**

| Trạng thái | FE hiển thị/xử lý |
| --- | --- |
| `Ready` | Key/model đã qua kiểm tra; vẫn có thể đang bị user tắt qua `isEnabled=false` |
| `CoolingDown` | Hiển thị lỗi tạm thời và `retryAfterAt`; cho user kiểm tra lại |
| `NeedsAttention` | Hiển thị `lastFailureCode`, yêu cầu sửa key/model/quota rồi kiểm tra lại |

Verify luôn ghi version mới, kể cả vẫn Ready; không tự bật connection đang bị user tắt.
Cooldown hết hạn có thể được backend chọn lại mà không cần ghi DB để đổi trạng thái ngay.

Xóa: `DELETE /ai/connections/{id}?version=<version hiện tại>`. Thành công 204, không gọi
`response.json()`. Xóa khỏi ứng dụng không revoke API key ở provider; request provider đã
bắt đầu trước khi tắt/xóa vẫn có thể hoàn tất.

Danh sách: `GET /ai/connections?pageSize=20` trả `{ items, nextCursor }`.
Trang sau dùng `URLSearchParams({ pageSize: '20', after: nextCursor })`; coi cursor là chuỗi
opaque, không tự tạo/tách nó. `nextCursor=null` là hết trang. Page size 1–100, mặc định 20;
sắp theo `sortOrder` rồi ID tăng dần, không có `totalCount`. Khi tạo/xóa/sửa, có thể tải lại
trang đầu để đồng bộ. Không cần UI chọn thứ tự/fallback ở phần setup hiện tại.

## Xử lý lỗi phía FE

Lỗi dùng ProblemDetails JSON. Ví dụ lỗi key trong bước tạo/list model/sửa key:

```json
{
  "title": "AI connection cannot be used",
  "status": 422,
  "detail": "The AI API key is invalid, expired, or revoked.",
  "instance": "/api/v1/ai/connections",
  "traceId": "...",
  "code": "InvalidApiKey",
  "retryAfterAt": null
}
```

`code`/`retryAfterAt` chỉ có ở những lỗi tương ứng; không giả định mọi lỗi đều có chúng.
Validation HTTP có thể có thêm `errors: { field: string[] }`. Dùng `traceId` khi báo lỗi;
không gửi key hoặc toàn bộ request body vào hệ thống báo lỗi.

| HTTP | Cách xử lý |
| --- | --- |
| 400 | Sửa input theo validation; không tự retry |
| 401 | JWT ứng dụng không hợp lệ/hết hạn: đi qua cơ chế refresh đăng nhập hiện có của FE |
| 403 | Không có quyền ứng dụng; kiểm tra cấu hình origin nếu detail báo origin bị chặn |
| 404 | Connection không tồn tại/không thuộc user: tải lại danh sách |
| 409 | Version cũ: tải lại dữ liệu, cho user xem thay đổi trước khi gửi lại; không tự ghi đè |
| 409 + `CreationInProgress` | Chờ `Retry-After`, retry tạo với UUID/payload cũ |
| 422 | Key/model/quota AI có vấn đề: hướng dẫn sửa theo `code`, không logout tài khoản app |
| 429 | App giới hạn request: chờ `Retry-After`, tránh spam submit |
| 502 | Provider tạm lỗi/rate limit; hiển thị thông báo, tôn trọng `retryAfterAt`/`Retry-After` nếu có |
| 504 | Provider timeout; cho retry có kiểm soát; tạo connection vẫn dùng UUID cũ khi retry |
| 500 | Thông báo lỗi chung và traceId |

Nếu 409 khi tạo không có `CreationInProgress`, kiểm tra UUID/payload bị dùng lại khác nội dung.
Server không tự retry inference. FE cũng không tự retry vô hạn các bước có probe.
CORS đã cho phép `Idempotency-Key` và expose `Retry-After` để JavaScript đọc được header này.

## Ví dụ gọi API bằng TypeScript

Helper nhận access token từ auth store hiện có. Cơ chế refresh JWT tập trung của FE nên xử lý
401; helper dưới đây không tự retry. Không log `init.body` vì có thể chứa API key.

```ts
interface AiProblem {
  title?: string;
  status?: number;
  detail?: string;
  traceId?: string;
  code?: AiFailureCode | 'CreationInProgress';
  retryAfterAt?: string | null;
  errors?: Record<string, string[]>;
}

class AiApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: AiProblem,
    public readonly retryAfter: string | null,
  ) { super(problem.detail ?? problem.title ?? `HTTP ${status}`); }
}

async function callAi<T>(
  apiBase: string, // ví dụ https://backend.example.com/api/v1, không có / cuối
  accessToken: string,
  path: string,   // ví dụ /connections
  init: RequestInit = {},
): Promise<T> {
  const headers = new Headers(init.headers);
  headers.set('Authorization', `Bearer ${accessToken}`);
  if (init.body != null) headers.set('Content-Type', 'application/json');
  const response = await fetch(`${apiBase}/ai${path}`, { ...init, headers });
  if (!response.ok) {
    const problem: AiProblem = await response.json().catch(() => ({}));
    throw new AiApiError(response.status, problem, response.headers.get('Retry-After'));
  }
  if (response.status === 204) return undefined as T;
  return await response.json() as T;
}

// Tạo các giá trị này một lần khi user submit; giữ nguyên khi retry mất mạng.
const requestId = crypto.randomUUID();
const createBody = JSON.stringify({ provider, apiKey, modelId, displayName });
const connection = await callAi<AiConnectionDto>(apiBase, accessToken, '/connections', {
  method: 'POST',
  headers: { 'Idempotency-Key': requestId },
  body: createBody,
});
// Sau thành công: cập nhật UI bằng connection và xóa key/body khỏi state form.

const updated = await callAi<AiConnectionDto>(
  apiBase, accessToken, `/connections/${connection.id}`, {
    method: 'PATCH', body: JSON.stringify({ version: connection.version, isEnabled: false }),
  },
);
await callAi<void>(
  apiBase, accessToken, `/connections/${updated.id}?version=${updated.version}`,
  { method: 'DELETE' },
);
```

`provider`, `apiKey`, `modelId`, `displayName`, `apiBase`, `accessToken` trong ví dụ lấy từ
form/config/auth store. Không hardcode key thật vào source FE. Cấu hình origin frontend trong
`Cors:AllowedOrigins`; URL tunnel backend và origin frontend có thể khác nhau.

