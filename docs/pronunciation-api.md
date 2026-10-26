# Pronunciation API cho FE

[README dự án](../README.md) · [Model, DB và seed](pronunciation-model.md) · [API bảng chữ](language-foundations-api.md)

## API phục vụ việc gì?

Module cung cấp nội dung giải thích quy tắc đọc và ví dụ đã biên soạn theo ngôn ngữ.
FE có thể thêm mục **Quy tắc đọc** bên cạnh **Bảng chữ**: user chọn quy tắc, đọc giải thích,
xem ví dụ và nghe khi có audio. Các API này không ghi LearningProfile/selection/tiến độ,
không chấm phát âm, không cần API key AI và không có thao tác quản trị nội dung.

Base URL: `<backend-origin>/api/v1/languages/{languageCode}/pronunciation-rules`.
JWT ứng dụng bắt buộc: `Authorization: Bearer <accessToken>`. Dùng accessToken từ auth flow
hiện có, không dùng token OAuth provider hoặc API key AI.

| Method | Path tương đối | Phục vụ FE | Response |
| --- | --- | --- | --- |
| GET | `/` | Menu/danh sách quy tắc theo thứ tự | 200, PronunciationRulePage |
| GET | `/{ruleCode}` | Tiêu đề, giải thích đầy đủ và lưu ý | 200, PronunciationRuleDto |
| GET | `/{ruleCode}/examples` | Danh sách ví dụ của quy tắc đang mở | 200, PronunciationExamplePage |

List thực tế là `/api/v1/languages/ja/pronunciation-rules`, không cần dấu `/` cuối.
Response camelCase, ID Snowflake là **string**, không wrapper `data`.
Danh sách rỗng vẫn trả 200 `{ items: [], nextCursor: null }`; không có 204 ở ba API này.

## Input và chuẩn hóa

| Input | Áp dụng | Quy tắc |
| --- | --- | --- |
| languageCode | Path của cả ba API | Tag tối đa 35 ký tự, trim/lowercase; ví dụ ja, en-us, zh-hant |
| ruleCode | Detail/examples | Lấy từ response list; tối đa 64 ký tự ASCII, bắt đầu bằng chữ, dùng chữ/số/hyphen, normalize lowercase |
| locale | List/detail | Ngôn ngữ giải thích; mặc định en; tối đa 35 ký tự |
| pageSize | List/examples | Mặc định 20; cho phép 1–100 |
| after | List/examples | Gửi nguyên NextCursor của trang trước; bỏ qua ở trang đầu |

Ví dụ không có nội dung dịch nên endpoint examples không cần locale. Text/Transcription
thuộc ngôn ngữ đích; thay locale không dịch hoặc thay đổi phiên âm.
Không nhận userId/profileId: dữ liệu dùng chung, còn quyền truy cập ứng dụng dựa trên JWT.

## Logic nội dung và bản dịch

Mọi query rule/example đều gắn với Language.IsEnabled=true và Rule.IsPublished=true.
Không trả nội dung bị ẩn dù client biết ID/code. Rule chỉ hợp lệ trong đúng languageCode
được yêu cầu; ruleCode không được dùng để tìm kiếm xuyên ngôn ngữ.

Bản dịch được chọn theo cùng quy ước API bảng chữ:

1. Locale chính xác sau normalize.
2. en nếu không có locale yêu cầu.
3. NativeName nếu không có cả hai.

Không tự rút gọn region/script: vi-vn không tự thành vi, zh-hant không tự thành zh.
Nếu cần nội dung tiếng Việt trong seed hiện có, truyền locale=vi. Locale hợp lệ nhưng chưa
có bản dịch không gây 404; server dùng fallback.

- TranslationLocale là locale thật được dùng, hoặc null nếu chỉ có tên gốc.
- IsTranslationFallback=true khi không có bản dịch đúng locale yêu cầu.
- Detail dùng cùng một row dịch cho Name/Explanation/UsageNote; không trộn nhiều ngôn ngữ.
- Không có bản dịch nào: Name=NativeName, Explanation=null, UsageNote=null.
- UsageNote có thể null ngay cả khi bản dịch tồn tại.
- List chỉ trả tên/metadata để dựng menu, không lấy Explanation, UsageNote hoặc cây examples.
- Version là version của rule; UpdatedAt trong list/detail là mốc lớn hơn giữa rule và bản dịch được chọn.
  Version không đại diện thay đổi của cả cây bản dịch/ví dụ; API chưa dùng ETag/conditional GET.

## Kiểu dữ liệu TypeScript

```ts
export interface CatalogPage<T> {
  items: T[];
  nextCursor: string | null;
}

export interface PronunciationRuleSummaryDto {
  id: string;
  languageCode: string;
  code: string;
  nativeName: string;
  name: string;
  translationLocale: string | null;
  isTranslationFallback: boolean;
  sortOrder: number;
  version: number;
  updatedAt: string; // ISO 8601 UTC
}

export interface PronunciationRuleDto extends PronunciationRuleSummaryDto {
  explanation: string | null;
  usageNote: string | null;
}

export interface PronunciationExampleDto {
  id: string;
  code: string;
  text: string;
  transcription: string;
  transcriptionSystem: string;
  audioUrl: string | null;
  sortOrder: number;
  version: number;
  updatedAt: string;
}

export type PronunciationRulePage = CatalogPage<PronunciationRuleSummaryDto>;
export type PronunciationExamplePage = CatalogPage<PronunciationExampleDto>;
```

Giữ ID dạng string, không dùng Number()/parseInt(). Dùng Code để gọi route, ID để làm key UI.
Không suy ra phiên âm từ Code; ví dụ code gakkou có transcription gakkō.

## 1. Lấy danh sách quy tắc

```http
GET /api/v1/languages/ja/pronunciation-rules?locale=vi&pageSize=1
Authorization: Bearer <accessToken>
```

```json
{
  "items": [{
    "id": "1563703640068191208",
    "languageCode": "ja",
    "code": "dakuten-handakuten",
    "nativeName": "濁音・半濁音",
    "name": "Âm đục và âm bán đục",
    "translationLocale": "vi",
    "isTranslationFallback": false,
    "sortOrder": 0,
    "version": 1,
    "updatedAt": "2026-10-25T00:00:00+00:00"
  }],
  "nextCursor": "0:1563703640068191208"
}
```

Seed có 5 rule: dakuten-handakuten, contracted-sounds, geminate-consonants, long-vowels,
particle-readings. Mặc định pageSize=20 lấy đủ seed hiện tại trong một request; vẫn xử lý
NextCursor vì nội dung sau này có thể nhiều hơn. Không hardcode danh sách seed ở FE.

## 2. Lấy chi tiết quy tắc

```http
GET /api/v1/languages/ja/pronunciation-rules/dakuten-handakuten?locale=vi
Authorization: Bearer <accessToken>
```

```json
{
  "id": "1563703640068191208",
  "languageCode": "ja",
  "code": "dakuten-handakuten",
  "nativeName": "濁音・半濁音",
  "name": "Âm đục và âm bán đục",
  "explanation": "Dấu dakuten ゛ làm đổi cách đọc của một số kana, như か thành が và は thành ば. Dấu handakuten ゜ dùng với hàng は để tạo hàng âm p, như は thành ぱ. Các dấu này cũng xuất hiện trong Katakana.",
  "usageNote": "Quan sát cả chữ và dấu đi kèm; không đọc chữ đã thêm dấu như chữ gốc.",
  "translationLocale": "vi",
  "isTranslationFallback": false,
  "sortOrder": 0,
  "version": 1,
  "updatedAt": "2026-10-25T00:00:00+00:00"
}
```

FE hiển thị Name, Explanation và UsageNote khi có. Nếu Explanation=null thì hiện trạng thái
chưa có giải thích phù hợp; vẫn có thể hiển thị các ví dụ. Không render text thành raw HTML.
Nếu dùng renderer Markdown, kiểm soát/sanitize nội dung theo renderer của ứng dụng.

## 3. Lấy ví dụ

```http
GET /api/v1/languages/ja/pronunciation-rules/dakuten-handakuten/examples?pageSize=1
Authorization: Bearer <accessToken>
```

```json
{
  "items": [{
    "id": "1563703640068191308",
    "code": "ga",
    "text": "が",
    "transcription": "ga",
    "transcriptionSystem": "hepburn",
    "audioUrl": null,
    "sortOrder": 0,
    "version": 1,
    "updatedAt": "2026-10-25T00:00:00+00:00"
  }],
  "nextCursor": "0:1563703640068191308"
}
```

Text có thể là một chữ, từ hoặc cả câu. Giữ nguyên Unicode/dấu dài trong Transcription.
AudioUrl=null thì ẩn/disable nút nghe. Không tạo URL từ Code, không gọi AI sinh audio.
Khi có URL, FE phát audio trực tiếp từ host tài nguyên; API không proxy/download audio.
Seed hiện tại có 3 ví dụ mỗi rule và chưa có audio.

## Luồng nối FE với LearningProfile

1. Đọc GET /api/v1/learning-profiles/current qua auth flow hiện có.
   Nếu 204 thì hướng user sang chọn/tạo profile.
2. Lấy profile.targetLanguage làm languageCode, profile.explanationLanguage làm locale.
3. Mở tab Quy tắc đọc: gọi list, render menu/card bằng Name và NativeName.
4. Khi user chọn rule, gọi detail và examples song song. Không tải detail/examples cho
   từng rule ngay khi render menu.
5. Hiển thị nội dung giải thích và các ví dụ; load thêm ví dụ bằng NextCursor khi cần.
6. Đổi rule/profile thì hủy request cũ, bỏ cursor cũ, đối chiếu khóa request trước khi setState.
   Đổi riêng ExplanationLanguage thì tải lại list/detail theo locale; examples không cần dịch lại.

Nội dung quy tắc thuộc ngôn ngữ, không thuộc bộ chữ. FE không cần systemCode khi gọi
Pronunciation. Không tự gắn một ký tự với rule bằng cách tìm chuỗi trong tên/code; hiện chưa
có relation item-to-rule. Có thể đặt liên kết sang tab Quy tắc đọc của ngôn ngữ hiện tại.

Profile cũ có thể dùng tag chưa được catalog hỗ trợ hoặc đã bị tắt. 404 không phải lỗi đăng nhập;
hiển thị trạng thái chưa có nội dung và giữ nguyên profile. API không tự sửa targetLanguage,
đổi current profile hoặc ghi tiến độ khi user đọc một rule.

### Ví dụ TypeScript

Helper dưới đây nhận accessToken từ auth store. Refresh JWT khi 401 nên đi qua cơ chế
chung của FE; không log token, không tự retry vô hạn.

```ts
interface ApiProblem {
  title?: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

class PronunciationApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: ApiProblem,
    public readonly retryAfter: string | null,
  ) { super(problem.detail ?? problem.title ?? `HTTP ${status}`); }
}

async function getPronunciation<T>(
  apiBase: string, // https://backend.example.com/api/v1, không có / cuối
  accessToken: string,
  path: string,
  query: Record<string, string>,
  signal: AbortSignal,
): Promise<T> {
  const params = new URLSearchParams(query);
  const response = await fetch(`${apiBase}${path}${params.size ? `?${params}` : ''}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
    signal,
  });
  if (!response.ok) {
    const problem: ApiProblem = await response.json().catch(() => ({}));
    throw new PronunciationApiError(response.status, problem, response.headers.get('Retry-After'));
  }
  return await response.json() as T;
}

// Tạo AbortController mới cho mỗi lần tải; hủy controller cũ khi đổi scope/unmount.
const controller = new AbortController();
const locale = profile.explanationLanguage;
const language = encodeURIComponent(profile.targetLanguage);
const basePath = `/languages/${language}/pronunciation-rules`;
const rules = await getPronunciation<PronunciationRulePage>(
  apiBase, accessToken, basePath, { locale, pageSize: '20' }, controller.signal,
);

// Chỉ chạy khi user chọn một rule; selectedRule lấy từ response list hoặc route deep link.
const ruleCode = encodeURIComponent(selectedRule.code);
const rulePath = `${basePath}/${ruleCode}`;
const [detail, examples] = await Promise.all([
  getPronunciation<PronunciationRuleDto>(
    apiBase, accessToken, rulePath, { locale }, controller.signal,
  ),
  getPronunciation<PronunciationExamplePage>(
    apiBase, accessToken, `${rulePath}/examples`, { pageSize: '20' }, controller.signal,
  ),
]);

// Khi user yêu cầu trang ví dụ tiếp theo, giữ nguyên language/rule và bỏ qua nếu đã hết trang.
if (examples.nextCursor !== null) {
  const next = await getPronunciation<PronunciationExamplePage>(
    apiBase, accessToken, `${rulePath}/examples`,
    { pageSize: '20', after: examples.nextCursor }, controller.signal,
  );
  // Ghép next.items vào state, dedupe bằng id, giữ next.nextCursor cho lần tải tiếp.
}
```

profile, selectedRule, apiBase, accessToken lấy từ state/config/auth hiện có của FE.
Đoạn gọi list và đoạn mở chi tiết là hai hành động UI riêng; không tự mở mọi rule sau list.
Dùng khóa request/state theo (languageCode, ruleCode, locale); khi response về phải còn khớp
khóa hiện tại mới cập nhật UI. AbortError do đổi màn hình được bỏ qua trong luồng báo lỗi.

## Pagination, rỗng và lỗi

List/examples sắp SortOrder tăng rồi Id tăng, lấy pageSize+1; không totalCount/COUNT/OFFSET.
Cursor là opaque: gửi nguyên qua URLSearchParams, không tự tính. Reset cursor khi đổi endpoint,
language hoặc rule. Cursor không khóa snapshot; nếu nội dung được sắp lại trong lúc đọc nhiều
trang thì tải từ đầu/dedupe bằng ID khi cần.

- List 200 rỗng: ngôn ngữ hợp lệ/được bật nhưng chưa có rule xuất bản hoặc đã hết trang.
- Examples 200 rỗng: rule hợp lệ/đã xuất bản nhưng chưa có ví dụ hoặc đã hết trang.
- Ngôn ngữ thiếu/bị tắt, rule thiếu/chưa xuất bản/sai ngôn ngữ: 404.

Lỗi dùng ProblemDetails với title/status/detail/traceId; validation có thể thêm errors theo field.
Không điều khiển UI bằng cách so khớp nguyên văn detail.

| HTTP | FE xử lý |
| --- | --- |
| 400 | Tag/code/pageSize/cursor sai; sửa request |
| 401 | JWT app hết hạn/không hợp lệ; dùng luồng refresh auth hiện có |
| 403 | Kiểm tra quyền hoặc origin nếu detail báo origin bị chặn |
| 404 | Không có nội dung xuất bản ở scope đang xem; không logout hoặc xóa profile |
| 429 | Chờ Retry-After, tránh gửi lại liên tục |
| 500 | Hiện lỗi chung, giữ traceId để báo lỗi |

Rate limit dùng chung policy learning-content với Languages/WritingSystems:
RateLimiting:LearningContent mặc định 240 request/IP/60 giây, áp dụng trong từng instance
trước JWT/DB validation. Khi chạy đa instance cần limiter tổng tại gateway.
CORS giữ allowlist hiện có, cho Authorization và expose Retry-After. Response đặt no-store.
Không cache nội dung dịch chỉ bằng rule ID: locale là một phần của khóa state/cache FE.
Audio là tài nguyên riêng, host audio cần hỗ trợ cách nhúng/phát của FE.

## Code và DB

- Api/Controllers/PronunciationController.cs: route tương đối, JWT kế thừa ApiControllerBase,
  ánh xạ request sang query handler.
- Api/Contracts/Pronunciation: request/HTTP validation, mỗi type một file.
- Application/Pronunciation: query/handler/DTO riêng file, dùng CatalogQueryValidation và CatalogCursor hiện có.
- Application/Common/Interfaces/IPronunciationReadRepository.cs: contract đọc, không expose EF/IQueryable.
- Infrastructure/Persistence/Repositories/PronunciationReadRepository.cs: lọc publication, SQL projection và pagination.
- DI nằm tại DependencyInjection.cs của từng project; dùng lại rate limit/CORS/ProblemDetails hiện có.

Theo code, list có dữ liệu và detail dùng 1 SQL; examples có dữ liệu dùng 1 SQL. Trang rỗng
thêm tối đa 1 query kiểm tra parent để phân biệt rỗng hợp lệ với 404. Các con số chưa đo
trên PostgreSQL thật và không gồm query xác thực JWT.

Chọn một bản dịch trong SQL; list không lấy Explanation/UsageNote/examples. Examples join
trực tiếp với rule đã xuất bản và ngôn ngữ đang bật, không chỉ kiểm tra trước rồi bỏ scope.
Không query theo từng rule/example, không tải toàn bộ navigation, không dùng transaction/row lock
cho request đọc. Mỗi query đọc trạng thái tại thời điểm thực thi; không cam kết snapshot toàn bộ
nội dung qua nhiều request/trang.

## Migration và trạng thái kiểm tra

API không cần migration bổ sung. Cần áp dụng model/seed trước khi sử dụng:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Migration: `20261025162039_AddPronunciationRules`, kế tiếp migration catalog/bảng chữ.
API không tự chạy migration startup. Build đã thành công; còn warning nullability có sẵn
ở OAuthLoginService. Chưa chạy integration HTTP/PostgreSQL cho API Pronunciation.
Request mẫu nằm trong src/Api/Api.http.
