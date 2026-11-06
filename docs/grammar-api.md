# Grammar API và tích hợp FE

Module cung cấp danh mục điểm ngữ pháp và trang giải thích theo ngôn ngữ. Nội dung hiện có gồm
14 điểm nhập môn tiếng Nhật, mỗi điểm có giải thích Việt và Anh, theo seed của model.
Chi tiết entity/constraint nằm trong tài liệu model. [Grammar model](grammar-model.md)

## Endpoint và mục đích

| Method | Route đầy đủ | Mục đích |
| --- | --- | --- |
| GET | `/api/v1/languages/{languageCode}/grammar-points` | Danh sách cho màn ngữ pháp, có phân trang và bản dịch tiêu đề |
| GET | `/api/v1/languages/{languageCode}/grammar-points/{pointCode}` | Trang nội dung của một điểm ngữ pháp, gồm giải thích và lưu ý |

Cả hai endpoint yêu cầu `Authorization: Bearer <accessToken>` và dùng policy `learning-content` hiện có.
Catalog dùng chung cho các user đã đăng nhập. `languageCode` chọn ngôn ngữ đang học, `locale` chọn ngôn ngữ diễn giải.
FE có thể lấy hai giá trị này từ `LearningProfile.targetLanguage` và `LearningProfile.explanationLanguage`.

## Danh sách điểm ngữ pháp

```http
GET /api/v1/languages/ja/grammar-points?locale=vi&pageSize=1
Authorization: Bearer ACCESS_TOKEN
```

| Query | Mặc định | Quy tắc |
| --- | --- | --- |
| locale | en | Language tag normalize trim/lowercase, tối đa 35 ký tự |
| pageSize | 20 | Từ 1 đến 100 |
| after | null | Gửi lại nextCursor của trang trước |

Response ví dụ cho trang đầu của seed, pageSize=1.

```json
{
  "items": [
    {
      "id": "1563703640068193708",
      "languageCode": "ja",
      "code": "noun-copula",
      "nativeName": "名詞文の肯定",
      "pattern": "N1 は N2 です",
      "name": "Câu khẳng định với danh từ",
      "translationLocale": "vi",
      "isTranslationFallback": false,
      "sortOrder": 0,
      "version": 1,
      "updatedAt": "2026-11-05T00:00:00+00:00"
    }
  ],
  "nextCursor": "0:1563703640068193708"
}
```

`name` là tiêu đề phù hợp để hiển thị. `nativeName` là tên gốc, `pattern` là cấu trúc mẫu để trình bày
trên thẻ. Danh sách chỉ chọn các field summary để giữ response gọn.

Thứ tự `(sort_order ASC, id ASC)`, cursor dạng `sortOrder:id`. Giữ cursor là opaque string và gửi nguyên
giá trị bằng URLSearchParams. ID là string để bảo toàn Snowflake trong JavaScript.
Tải tiếp khi nextCursor có giá trị và dừng khi null. Giữ language/locale/pageSize nhất quán giữa các trang.
Khi đổi ngôn ngữ hoặc locale, lấy lại trang đầu và thay query key tương ứng.

Cursor hoạt động theo catalog hiện tại. Nếu nội dung hoặc thứ tự được biên tập giữa các request,
FE có thể refresh trang đầu để cập nhật danh sách.

## Chi tiết điểm ngữ pháp

```http
GET /api/v1/languages/ja/grammar-points/noun-copula?locale=vi
Authorization: Bearer ACCESS_TOKEN
```

Detail chứa mọi field của item danh sách và thêm hai field sau.

```json
{
  "explanation": "Dùng は để nêu chủ đề và kết thúc bằng danh từ kèm です khi giới thiệu tên, nghề nghiệp hoặc phân loại. N1 và N2 là các danh từ.",
  "usageNote": "Trợ từ は được đọc là wa. Có thể lược chủ đề đã rõ trong hội thoại."
}
```

`pointCode` là code ổn định trong ngôn ngữ, ví dụ noun-copula, question-ka hoặc polite-verbs.
Route language và code đều normalize theo convention catalog. Code tối đa 64 ký tự, bắt đầu bằng
chữ ASCII, gồm chữ, số và dấu gạch ngang. Language tag dùng chính xác code trong bảng languages.

`explanation` có thể null khi thiếu bản dịch được chọn. `usageNote` có thể null cả khi bản dịch đã có.
FE render nội dung dạng text, giữ line break và chỉ hiện khối lưu ý khi có dữ liệu.

## Chọn bản dịch và thời điểm cập nhật

Backend chọn bản dịch theo thứ tự sau, trong cùng truy vấn SQL.

1. Locale yêu cầu, ví dụ vi hoặc en-us.
2. Tiếng Anh en.
3. Tên gốc NativeName, cùng explanation/usageNote/translationLocale bằng null.

`isTranslationFallback` true khi kết quả dùng locale khác yêu cầu hoặc dùng tên gốc.
TranslationLocale phản ánh bản dịch thực sự được chọn. Locale mặc định en bám các API catalog hiện có,
vì vậy FE cần gửi ExplanationLanguage của profile khi muốn giải thích theo lựa chọn người dùng.

Tên, giải thích và lưu ý luôn lấy từ cùng một bản dịch. Locale vùng dùng exact match rồi đến en.
Ví dụ yêu cầu fr-ca sẽ chọn en nếu chỉ có fr và en. Khi thiếu exact locale và en, các field nội dung
nullable cho FE biểu diễn trạng thái thiếu bản dịch.

`version` là Version của GrammarPoint. `updatedAt` là giá trị lớn hơn giữa UpdatedAt của point và bản dịch
được chọn. Khi sửa bản dịch Việt, response locale=vi nhận timestamp mới cùng nội dung mới. Response
locale=en tiếp tục phản ánh timestamp của point và bản dịch Anh tương ứng.

## Publication và lỗi

Backend chỉ trả điểm có IsPublished=true và language có IsEnabled=true.
Language hợp lệ đang bật, chưa có điểm hoặc cursor đã hết dữ liệu trả 200 với items rỗng và nextCursor=null.

| HTTP | Trường hợp | FE xử lý |
| --- | --- | --- |
| 400 | Language/locale/code/cursor sai định dạng hoặc pageSize ngoài giới hạn | Kiểm tra tham số và lấy lại trang đầu khi cursor lỗi |
| 401 | Thiếu/hết hạn token | Dùng auth refresh hiện có rồi retry GET |
| 404 | Language thiếu/bị tắt, point thiếu hoặc đã unpublish | Hiện trạng thái nội dung không khả dụng |
| 429 | Vượt policy learning-content | Tôn trọng Retry-After |

Lỗi nghiệp vụ dùng ProblemDetails với traceId. Lỗi model binding có thể dùng ValidationProblemDetails.
Cache-Control no-store và Pragma no-cache được áp theo middleware catalog, gồm các response lỗi.
Rate limit chia sẻ với các route learning-content khác, dùng cấu hình hệ thống hiện tại.

## Luồng FE

1. Lấy profile đang chọn. Gọi list với targetLanguage và locale=explanationLanguage.
2. Render `name`, `pattern`, có thể kèm nativeName và nhãn translationLocale khi fallback.
3. Nút tải thêm dùng nextCursor. Với seed 14 mục, pageSize mặc định 20 trả toàn bộ nội dung hiện có.
4. Khi chọn một điểm, mở route FE theo languageCode + pointCode và GET detail bằng đúng locale.
5. Render explanation, usageNote và trạng thái thiếu bản dịch theo các field nullable.

Ví dụ wrapper GET, apiBase đã gồm `/api/v1`. Dùng auth wrapper hiện có của FE để cấp và refresh accessToken.

```ts
type GrammarPointSummary = {
  id: string;
  languageCode: string;
  code: string;
  nativeName: string;
  pattern: string;
  name: string;
  translationLocale: string | null;
  isTranslationFallback: boolean;
  sortOrder: number;
  version: number;
  updatedAt: string;
};
type GrammarPointDetail = GrammarPointSummary & {
  explanation: string | null;
  usageNote: string | null;
};
type GrammarPointPage = { items: GrammarPointSummary[]; nextCursor: string | null };

async function grammarGet<T>(url: string, accessToken: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(url, {
    headers: { Authorization: `Bearer ${accessToken}` }, signal,
  });
  if (!response.ok) throw Object.assign(new Error("Không tải được nội dung ngữ pháp"), {
    status: response.status,
    problem: await response.json(),
    retryAfter: response.headers.get("Retry-After"),
  });
  return response.json();
}

function grammarListUrl(apiBase: string, languageCode: string, locale: string, after?: string) {
  const query = new URLSearchParams({ locale, pageSize: "20" });
  if (after) query.set("after", after);
  return `${apiBase}/languages/${encodeURIComponent(languageCode)}/grammar-points?${query}`;
}

function grammarDetailUrl(apiBase: string, languageCode: string, pointCode: string, locale: string) {
  const query = new URLSearchParams({ locale });
  return `${apiBase}/languages/${encodeURIComponent(languageCode)}` +
    `/grammar-points/${encodeURIComponent(pointCode)}?${query}`;
}
```

Query key FE nên chứa languageCode và locale, thêm pointCode cho detail. Abort hoặc bỏ qua response cũ
khi người dùng đổi profile/locale để dữ liệu cũ không ghi đè màn mới. Bearer token, CORS và
Retry-After tiếp tục đi qua cấu hình API hiện có.

## Kiến trúc và query budget

- Api/Contracts/Grammar kiểm tra query parameters, GrammarController gọi handler trực tiếp.
- Application/Grammar gồm Queries và Models, dùng CatalogQueryValidation chung.
- IGrammarReadRepository nằm trong Application/Common/Interfaces.
- Infrastructure/Persistence/Repositories/GrammarReadRepository thực hiện projection, publication và locale lookup.
- Domain GrammarPoint/Translation và migration model cung cấp dữ liệu catalog.

List giới hạn pageSize+1 trước khi chọn bản dịch. Translation lookup dùng exact/en, Take(1) và left join
trong SQL, giữ point khi thiếu bản dịch. Tất cả truy vấn AsNoTracking.

| Flow | SQL command cho module |
| --- | --- |
| List có dữ liệu | 1 |
| List rỗng hoặc language thiếu/bị tắt | 2 để phân biệt empty với 404 |
| Detail thành công hoặc 404 | 1 |
| Input không hợp lệ | 0 |

Budget trên tính riêng feature và tách khỏi kiểm tra session JWT của hệ thống.
Các GET chạy bằng SELECT, phù hợp transaction đọc đơn lệnh của PostgreSQL.

## Chạy và kiểm thử

Áp migration model hiện có rồi chạy API theo quy trình dự án.

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
```

Request mẫu có trong `src/Api/Api.http`. Bộ regression chạy qua entry point HTTP thực tế.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Grammar.Tests/bin/Debug/net10.0/Grammar.Tests.dll
```

Đặt GRAMMAR_TEST_DATABASE thành connection string của DB trống, biệt lập, tên grammar_tests để chạy
EF/PostgreSQL/HTTP. Runner kiểm tra tên DB và schema public trống trước khi tạo dữ liệu kiểm thử.
Fake authentication và SQL counter thuộc test host, production dùng JWT/session hiện có.

Đã đạt **290 kiểm tra** gồm 53 Domain, 60 EF/PostgreSQL và 177 HTTP/application. Các ca bao gồm toàn bộ
14 seed point, locale exact/English/native fallback, missing translation, stable cursor, publication,
timestamp bản dịch, authenticated access, 8 GET song song, query projection và rate limit.
