# API ngôn ngữ và bảng chữ cho FE

[README dự án](../README.md) · [Model, DB và seed](language-foundations-model.md)

## Phạm vi và chức năng

Module cung cấp **6 API chỉ đọc** để user chọn ngôn ngữ, mở bộ chữ, xem ký tự và cách đọc.
Nội dung dùng chung, không gọi AI và không cần API key của người học. API không ghi tiến độ,
không chấm bài và không sửa profile/selection. Chưa có API quản trị nội dung.

Base URL: `<backend-origin>/api/v1`. Mọi endpoint yêu cầu JWT ứng dụng:
`Authorization: Bearer <accessToken>`. Không gửi token Google/Microsoft/Facebook hoặc key AI.
Response camelCase, ID Snowflake là string, không có wrapper `data`.

| Method | Path | Phục vụ FE | Logic chính |
| --- | --- | --- | --- |
| GET | `/languages` | Danh sách ngôn ngữ được hỗ trợ | Chỉ IsEnabled=true, dịch tên/giới thiệu, phân trang |
| GET | `/languages/{languageCode}` | Tiêu đề/giới thiệu ngôn ngữ hoặc deep link | Ngôn ngữ không tồn tại/bị tắt đều 404 |
| GET | `/languages/{languageCode}/writing-systems` | Danh sách/tab bộ chữ | Ngôn ngữ bật và bộ chữ IsPublished=true, dịch tên, phân trang |
| GET | `/languages/{languageCode}/writing-systems/{systemCode}` | Giới thiệu/hướng dẫn của bộ chữ | Kiểm tra cả ngôn ngữ và bộ chữ; không trả nội dung bị ẩn |
| GET | `/languages/{languageCode}/writing-systems/{systemCode}/items` | Dựng bảng chữ | Ký tự, tọa độ tùy chọn, cách đọc đầu tiên; phân trang |
| GET | `/languages/{languageCode}/writing-systems/{systemCode}/items/{itemCode}/readings` | Các cách đọc trong panel chi tiết một ký tự | Chỉ item thuộc đúng bộ chữ/ngôn ngữ đang mở; phân trang |

Tất cả thành công trả 200 JSON, kể cả danh sách rỗng. Không có 204 ở 6 endpoint này.

## Query parameters

| Parameter | Áp dụng | Mặc định / quy tắc |
| --- | --- | --- |
| `locale` | List/detail ngôn ngữ và bộ chữ | en; tag tối đa 35 ký tự, được trim/lowercase |
| `pageSize` | Mọi list | 20; riêng list items mặc định 100; cho phép 1–100 |
| `after` | Mọi list | Bỏ qua ở trang đầu; gửi nguyên nextCursor trang trước |

`languageCode` dùng language tag như ja, en-us, zh-hant; không dùng ID cho route này.
`systemCode` và `itemCode` lấy từ response, không tự đoán bằng symbol hoặc phiên âm.
Content code tối đa 64 ký tự, ASCII chữ/số/hyphen, bắt đầu bằng chữ; được trim/lowercase.
Dùng encodeURIComponent khi đưa từng code vào path.

Items/readings không có nội dung được dịch nên không cần locale. PrimaryReading là dữ liệu
phát âm của ngôn ngữ đích, không phải bản dịch theo locale.

## Fallback bản dịch

Backend chọn **một bản dịch cho mỗi ngôn ngữ/bộ chữ**, theo thứ tự:

1. Locale chính xác sau normalize.
2. en nếu không có locale yêu cầu.
3. NativeName nếu không có cả hai; Description=null.

Các field Name và Description thuộc cùng bản dịch; không trộn tên tiếng Việt với mô tả tiếng Anh.
Nếu bản dịch được chọn có Description=null thì giữ null, không tìm mô tả từ locale khác.

- `translationLocale`: locale thật sự được chọn; null khi dùng NativeName.
- `isTranslationFallback`: true khi không có bản dịch đúng locale yêu cầu.
- `nativeName`: tên gốc luôn có, độc lập với tên đã dịch.
- `updatedAt` ở language/system là mốc lớn hơn giữa entity và bản dịch đang được dùng.
- `version` là version entity, không phải version gộp của mọi bản dịch/con. Không dùng riêng
  version để kết luận cả cây nội dung không đổi; API chưa có ETag/conditional GET.

Không tự đổi `vi-vn` thành `vi`, hoặc bỏ script/region. Ví dụ seed có vi nhưng request vi-vn
sẽ dùng en. FE nên truyền đúng ExplanationLanguage mà user đã chọn; nếu cần bản dịch vi thì
dùng locale=vi rõ ràng. Locale hợp lệ nhưng chưa có bản dịch vẫn trả 200 qua fallback.

## Kiểu dữ liệu TypeScript

```ts
export interface CatalogPage<T> {
  items: T[];
  nextCursor: string | null;
}

export interface LanguageDto {
  id: string;
  code: string;
  nativeName: string;
  name: string;
  description: string | null;
  translationLocale: string | null;
  isTranslationFallback: boolean;
  sortOrder: number;
  version: number;
  updatedAt: string; // ISO 8601 UTC
}

export interface WritingSystemDto {
  id: string;
  languageCode: string;
  code: string;
  nativeName: string;
  name: string;
  description: string | null;
  translationLocale: string | null;
  isTranslationFallback: boolean;
  sortOrder: number;
  version: number;
  updatedAt: string;
}

export interface WritingSystemItemReadingDto {
  id: string;
  code: string;
  transcription: string;
  transcriptionSystem: string;
  audioUrl: string | null;
  sortOrder: number;
  version: number;
  updatedAt: string;
}

export interface WritingSystemItemDto {
  id: string;
  code: string;
  symbol: string;
  sortOrder: number;
  rowIndex: number | null;
  columnIndex: number | null;
  version: number;
  updatedAt: string;
  primaryReading: WritingSystemItemReadingDto | null;
}
```

Không parseInt/Number các ID. Dùng code để gọi endpoint và ID string để làm key ổn định cho UI.

## Ví dụ response

### 1. Danh sách ngôn ngữ

```http
GET /api/v1/languages?locale=vi&pageSize=20
Authorization: Bearer <accessToken>
```

```json
{
  "items": [{
    "id": "1563703640068190208",
    "code": "ja",
    "nativeName": "日本語",
    "name": "Tiếng Nhật",
    "description": "Bắt đầu với bảng Hiragana và Katakana cơ bản.",
    "translationLocale": "vi",
    "isTranslationFallback": false,
    "sortOrder": 0,
    "version": 1,
    "updatedAt": "2026-10-25T00:00:00+00:00"
  }],
  "nextCursor": null
}
```

GET `/languages/ja?locale=vi` trả trực tiếp một LanguageDto như phần tử trên.

### 2. Bộ chữ

```http
GET /api/v1/languages/ja/writing-systems?locale=vi
Authorization: Bearer <accessToken>
```

Trả `CatalogPage<WritingSystemDto>`. Seed có hiragana và katakana.
GET `/languages/ja/writing-systems/hiragana?locale=vi` trả trực tiếp:

```json
{
  "id": "1563703640068190218",
  "languageCode": "ja",
  "code": "hiragana",
  "nativeName": "ひらがな",
  "name": "Hiragana",
  "description": "Bảng 46 chữ Hiragana cơ bản. Phiên âm chỉ hỗ trợ ghi nhớ; chưa bao gồm âm đục, âm ghép và quy tắc đọc trong câu.",
  "translationLocale": "vi",
  "isTranslationFallback": false,
  "sortOrder": 0,
  "version": 1,
  "updatedAt": "2026-10-25T00:00:00+00:00"
}
```

List DTO đã đủ dựng tab và giới thiệu. Khi đi từ list, không bắt buộc gọi detail lại;
detail phục vụ deep link/reload riêng.

### 3. Bảng ký tự

```http
GET /api/v1/languages/ja/writing-systems/hiragana/items?pageSize=1
Authorization: Bearer <accessToken>
```

```json
{
  "items": [{
    "id": "1563703640068190308",
    "code": "kana-r0-c0",
    "symbol": "あ",
    "sortOrder": 0,
    "rowIndex": 0,
    "columnIndex": 0,
    "version": 1,
    "updatedAt": "2026-10-25T00:00:00+00:00",
    "primaryReading": {
      "id": "1563703640068190508",
      "code": "hepburn",
      "transcription": "a",
      "transcriptionSystem": "hepburn",
      "audioUrl": null,
      "sortOrder": 0,
      "version": 1,
      "updatedAt": "2026-10-25T00:00:00+00:00"
    }
  }],
  "nextCursor": "0:1563703640068190308"
}
```

Để lấy cả bảng seed trong một request, bỏ pageSize hoặc đặt 100. Mỗi bộ seed có 46 mục;
không hardcode rằng mọi bộ chữ tương lai đều có 46 mục hoặc vừa một trang.

- Dựng grid bằng RowIndex/ColumnIndex khi có; giữ các ô trống, không tự dồn ô.
- Khi tọa độ null thì hiển thị theo thứ tự list. Model cũng cho phép nhiều mục cùng tọa độ;
  render nhiều mục trong ô đó thay vì làm mất dữ liệu.
- PrimaryReading là reading đầu theo SortOrder rồi Id. Đây là cách đọc hiển thị mặc định,
  không khẳng định item chỉ có một cách đọc. Không gọi readings cho từng ô khi dựng bảng.
- Không có reading thì PrimaryReading=null; vẫn hiển thị Symbol.
- Nếu AudioUrl=null thì ẩn/disable nút nghe; không dựng URL giả hoặc gọi AI tự tạo audio.

### 4. Các cách đọc của một ký tự

```http
GET /api/v1/languages/ja/writing-systems/hiragana/items/kana-r0-c0/readings
Authorization: Bearer <accessToken>
```

```json
{
  "items": [{
    "id": "1563703640068190508",
    "code": "hepburn",
    "transcription": "a",
    "transcriptionSystem": "hepburn",
    "audioUrl": null,
    "sortOrder": 0,
    "version": 1,
    "updatedAt": "2026-10-25T00:00:00+00:00"
  }],
  "nextCursor": null
}
```

Chỉ gọi khi user mở chi tiết để xem đủ readings. Dùng itemCode từ row đang chọn;
không truyền item ID hoặc symbol vào vị trí itemCode.

## Ghép với LearningProfile và luồng FE

### Màn hình chọn ngôn ngữ

1. Sau đăng nhập, gọi GET /languages với locale của UI/ngôn ngữ giải thích.
2. Hiển thị Name/NativeName; load trang tiếp theo khi NextCursor khác null.
3. User chọn một language.code rồi tạo LearningProfile qua API profile hiện có.
   API catalog không tự tạo profile hoặc chuyển current selection.

### Màn hình học bảng chữ

1. Đọc GET /learning-profiles/current. Nếu 204 thì hướng user sang chọn/tạo profile.
2. Lấy `profile.targetLanguage` làm languageCode và `profile.explanationLanguage` làm locale.
3. GET danh sách writing-systems. Chọn một tab từ system.code trả về.
4. GET items của tab đó; dùng PrimaryReading để hiển thị phiên âm ngay.
5. Chạm item thì mở panel, GET readings khi cần xem đầy đủ.
6. Khi đổi profile/ngôn ngữ, hủy request cũ, bỏ tab/cursor/items cũ và tải lại.
   Khi chỉ đổi ExplanationLanguage, tải lại phần language/system có bản dịch;
   không cần tải lại items/readings nếu ngôn ngữ/bộ chữ vẫn như cũ.

Một profile cũ có thể trỏ tới tag chưa có catalog hoặc chưa được bật. Khi nhận 404, hiển thị
“Chưa có nội dung cho ngôn ngữ này”; không tự sửa targetLanguage, chọn ngôn ngữ khác hoặc xóa profile.

### Phân trang và trạng thái rỗng

Mọi list sắp SortOrder tăng rồi Id tăng, lấy pageSize+1 để biết còn trang; không totalCount.
Coi NextCursor là opaque; dùng URLSearchParams để encode, không tự phân tích/tạo cursor.
Cursor chỉ dùng lại trong cùng endpoint/phạm vi đang duyệt. Đổi language/system/item thì bỏ cursor.

- Languages rỗng: chưa có ngôn ngữ được bật.
- Writing-systems rỗng: ngôn ngữ có tồn tại/được bật nhưng chưa có bộ chữ xuất bản.
- Items/readings rỗng: parent hợp lệ nhưng chưa có nội dung, hoặc đã qua trang cuối.
- 404: parent thiếu/bị ẩn hoặc code không thuộc đúng cây đang yêu cầu.

Cursor là phân trang dữ liệu đang thay đổi, không phải snapshot cố định. Nếu nội dung được
sắp xếp lại trong lúc tải nhiều trang, refresh từ đầu và dedupe bằng ID khi cần.

## Helper TypeScript minh họa

Dùng auth store/cơ chế refresh 401 hiện có của FE. Helper không log JWT và không tự retry.

```ts
interface ApiProblem {
  title?: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

class ContentApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: ApiProblem,
    public readonly retryAfter: string | null,
  ) { super(problem.detail ?? problem.title ?? `HTTP ${status}`); }
}

async function getContent<T>(
  apiBase: string, // https://backend.example.com/api/v1, không có / cuối
  accessToken: string,
  path: string,
  query: Record<string, string> = {},
  signal?: AbortSignal,
): Promise<T> {
  const params = new URLSearchParams(query);
  const url = `${apiBase}${path}${params.size ? `?${params}` : ''}`;
  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${accessToken}` },
    signal,
  });
  if (!response.ok) {
    const problem: ApiProblem = await response.json().catch(() => ({}));
    throw new ContentApiError(response.status, problem, response.headers.get('Retry-After'));
  }
  return await response.json() as T;
}

// Lấy từ profile hiện tại; giữ nguyên script/region trong tag.
const language = encodeURIComponent(profile.targetLanguage);
const locale = profile.explanationLanguage;
const systemsPath = `/languages/${language}/writing-systems`;
const systems = await getContent<CatalogPage<WritingSystemDto>>(
  apiBase, accessToken, systemsPath, { locale }, controller.signal,
);

// Chỉ thực hiện sau khi user chọn một phần tử trong systems.items.
const systemCode = encodeURIComponent(selectedSystem.code);
const itemsPath = `${systemsPath}/${systemCode}/items`;
const firstPage = await getContent<CatalogPage<WritingSystemItemDto>>(
  apiBase, accessToken, itemsPath, { pageSize: '100' }, controller.signal,
);
if (firstPage.nextCursor !== null) {
  const nextPage = await getContent<CatalogPage<WritingSystemItemDto>>(
    apiBase, accessToken, itemsPath,
    { pageSize: '100', after: firstPage.nextCursor }, controller.signal,
  );
  // Ghép nextPage.items vào state, dedupe bằng item.id.
}
```

`profile`, `selectedSystem`, `apiBase`, `accessToken` lấy từ state/config/auth hiện có.
`controller` là AbortController của lần tải. Đổi profile/tab thì abort và tạo controller mới;
đồng thời đối chiếu khóa request hiện tại trước khi setState để response cũ không ghi đè tab mới.
Ví dụ trên lấy tối đa hai trang minh họa; UI tiếp tục dùng NextCursor cho những trang còn lại.
Không render Description bằng raw HTML; dùng text hoặc renderer Markdown đã kiểm soát.

## Lỗi và triển khai

| HTTP | Xử lý FE |
| --- | --- |
| 400 | Tag/code/pageSize/cursor không hợp lệ; sửa request |
| 401 | JWT ứng dụng hết hạn/không hợp lệ; đi qua refresh auth hiện có |
| 403 | Kiểm tra quyền hoặc origin nếu detail báo origin bị chặn |
| 404 | Không có nội dung được công bố ở đúng phạm vi này; xử lý trạng thái chưa hỗ trợ |
| 429 | Chờ Retry-After, không tự lặp request liên tục |
| 500 | Thông báo chung và giữ traceId để báo lỗi |

Tất cả response đặt no-store. Không cache kết quả theo targetLanguage đơn thuần: nếu FE có
cache state thì language/system phải phân biệt locale. Xóa state phù hợp khi logout/đổi profile;
không cho rằng Version entity bao quát thay đổi của mọi bản dịch/readings.

CORS dùng allowlist hiện có, cho Authorization và expose Retry-After. Audio là tài nguyên
riêng: host audio phải cho phép nhúng/phát theo cách FE sử dụng; API không proxy/tải file.
Seed trong migration khởi tạo AudioUrl=null. Sau khi chạy [lệnh khởi tạo audio kana trên R2](foundation-audio-seeding.md), 92 reading Hiragana/Katakana có URL audio thật. FE tiếp tục dùng `primaryReading.audioUrl` hoặc `audioUrl` từ danh sách readings; không đổi contract API.

RateLimiting:LearningContent mặc định 240 request/IP/60 giây, dùng chung quota cho hai controller.
Limiter chạy trước JWT/DB, trong từng instance. Đa instance cần thêm limiter tổng ở gateway.

## Bố cục code và query

- Api/Controllers: controller mỏng, route tương đối dùng prefix /api/v1, JWT kế thừa ApiControllerBase.
- Api/Contracts/Languages và WritingSystems: request/validation HTTP, mỗi type một file.
- Application/Languages và WritingSystems: DTO, query, handler riêng từng file; không phụ thuộc EF/HTTP.
- Application/Common/Interfaces: hai read repository contract; validation/cursor dùng chung cho catalog.
- Infrastructure/Persistence/Repositories: AsNoTracking, projection DTO và bộ lọc visibility.
- DI đăng ký tại DependencyInjection.cs của mỗi project, cùng quy ước Authentication/LearningProfiles.

Không tải toàn bộ bảng dịch/readings vào bộ nhớ. Translation chọn một dòng trong SQL; item lấy
một PrimaryReading bằng subquery có thứ tự. Không chạy một request/query riêng cho từng ký tự.
Danh sách giới hạn pageSize+1, không COUNT/OFFSET, không include cây dữ liệu không giới hạn.

Theo code: list language và các detail dùng 1 SQL; list system/items/readings có dữ liệu dùng
1 SQL; trang rỗng thêm tối đa 1 query kiểm tra parent để phân biệt 200 rỗng và 404.
Các con số không gồm xác thực JWT và chưa đo bằng PostgreSQL thật.
Các query con luôn gắn với ngôn ngữ bật/bộ chữ xuất bản ngay trong SQL, không chỉ kiểm tra trước rồi bỏ lọc.
Đọc không tạo transaction/row lock; dữ liệu có thể đổi sau khi request đọc xong.

Không cần migration mới cho API. Áp dụng migration model trước khi sử dụng:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Migration cần có: `20261025123943_AddLanguagesAndWritingSystems`.
Build được kiểm tra riêng; chưa chạy integration test HTTP/PostgreSQL cho các API này.
Request mẫu có trong src/Api/Api.http.
