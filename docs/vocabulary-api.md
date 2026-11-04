# API từ vựng và tích hợp frontend

## 1. Phạm vi và kiến trúc

Ba API đọc catalog từ vựng, dùng chung cho các user đã đăng nhập. Backend phục vụ danh sách
từ với nghĩa xem nhanh, metadata của từng từ và danh sách nghĩa/bản dịch có phân trang.
Không gọi AI hoặc ghi tiến độ khi tải nội dung.

Theo cấu trúc dự án:

- `Application/Vocabulary/Queries/<UseCase>`: query và handler riêng, inject trực tiếp.
- `Application/Vocabulary/Models`: DTO/page; không trả entity Domain ra HTTP.
- `Application/Common/Interfaces/IVocabularyReadRepository`: port đọc dữ liệu.
- `Infrastructure/Persistence/Repositories/VocabularyReadRepository`: EF projection, filter/publication và cursor.
- `Api/Contracts/Vocabulary`, `Api/Controllers/VocabularyController`: query HTTP và validation.

Giữ bốn layer, dùng các helper `CatalogQueryValidation`/`CatalogCursor` hiện có,
không thêm MediatR, package hoặc migration cho bước API.
Các bảng/seed cần migration `20261104064528_AddVocabularyCatalog`, xem [model và DB](vocabulary-model.md).

## 2. Endpoint phục vụ việc gì?

Base URL: `/api/v1/languages/{languageCode}/vocabulary`.
Mọi request dùng `Authorization: Bearer <accessToken>` theo cơ chế JWT/session hiện có.
Response JSON camelCase, enum theo tên; `Cache-Control: no-store`.

| Method | URL sau base | Chức năng FE |
| --- | --- | --- |
| GET | rỗng | Danh sách/card từ, một nghĩa xem nhanh, lọc từ loại và tải tiếp bằng cursor |
| GET | `/{entryCode}` | Mặt từ, cách đọc và audio của từ đang mở |
| GET | `/{entryCode}/senses` | Các nghĩa, từ loại, bản dịch và ghi chú; tải thêm theo trang |

Metadata và nghĩa là hai resource có giới hạn rõ: khi mở chi tiết FE gọi hai GET sau song song.
API metadata không nhét toàn bộ nghĩa vào response, và không nhận locale vì resource đó không chứa bản dịch.
Không có request riêng cho từng nghĩa hoặc N+1 request để render một trang danh sách.

`languageCode` là target language, chẳng hạn `ja`; `entryCode` là code ổn định từ response list,
chẳng hạn `mizu`, không dùng mặt chữ hoặc Snowflake ID thay cho code trên route.
Language tag/code được trim/lowercase bằng helper chung.
`ja` và `ja-jp` là hai code khác nhau; không tự ánh xạ vùng ngôn ngữ sang catalog khác.

**ID trong JSON luôn là string**; giữ nguyên trong JavaScript, không dùng `Number(id)`/`parseInt(id)`.
SortOrder/Version là số JSON, UpdatedAt là ISO 8601 UTC.

## 3. Danh sách từ

```http
GET /api/v1/languages/ja/vocabulary?locale=vi&pageSize=20
Authorization: Bearer <accessToken>
```

| Query | Quy tắc |
| --- | --- |
| `locale` | Tùy chọn, mặc định `en`; language tag lowercase sau normalize, tối đa 35 ký tự |
| `partOfSpeech` | Tùy chọn, enum name, ví dụ `Noun`, `Verb`, `Adjective` |
| `pageSize` | 1–100, mặc định 20 |
| `after` | Cursor `nextCursor` từ trang trước; gửi nguyên giá trị, không tự tạo |

Các tên PartOfSpeech hợp lệ:

`Noun`, `Verb`, `Adjective`, `Adverb`, `Pronoun`, `Determiner`, `Adposition`,
`Conjunction`, `Particle`, `Interjection`, `Numeral`, `Auxiliary`, `Other`.

Ví dụ response của trang đầu với `pageSize=1` trên seed ban đầu:

```json
{
  "items": [
    {
      "id": "1563703640068192208",
      "languageCode": "ja",
      "code": "mizu",
      "term": "水",
      "reading": "みず",
      "audioUrl": null,
      "primarySense": {
        "id": "1563703640068192308",
        "code": "water",
        "partOfSpeech": "Noun",
        "meaning": "Nước.",
        "translationLocale": "vi",
        "isTranslationFallback": false,
        "sortOrder": 0,
        "version": 1,
        "updatedAt": "2026-11-04T00:00:00+00:00"
      },
      "sortOrder": 0,
      "version": 1,
      "updatedAt": "2026-11-04T00:00:00+00:00"
    }
  ],
  "nextCursor": "0:1563703640068192208"
}
```

Danh sách sắp `(SortOrder ASC, Id ASC)`. Query lấy pageSize+1 từ để xác định trang sau,
không OFFSET hoặc COUNT tổng danh mục. Giữ nguyên language/locale/filter/pageSize khi gửi after.
Filter/locale thay đổi thì xóa cursor và tải lại trang đầu. SortOrder có thể được sửa trong catalog;
khi nội dung được biên tập giữa các lần tải, refresh trang đầu để có thứ tự mới nhất.

`primarySense` chỉ là **một nghĩa xem nhanh**:

- Không filter: nghĩa đầu theo `(sense.SortOrder, sense.Id)`.
- Có filter: entry chỉ xuất hiện nếu có ít nhất một nghĩa đúng từ loại; preview là nghĩa đầu
  trong các nghĩa khớp filter. Vì vậy card trong bộ lọc Verb không hiện preview Noun.
- Entry chưa có nghĩa: PrimarySense null. Không dùng Term làm một nghĩa giả.
- Nghĩa không có bản dịch phù hợp: PrimarySense vẫn có ID/code/từ loại nhưng Meaning/TranslationLocale null.
- List không tải UsageNote dài; mở senses endpoint để xem đầy đủ.

Language enabled nhưng danh mục hoặc filter không có kết quả: 200 `{ items: [], nextCursor: null }`.
Language không tồn tại hoặc disabled: 404. Entry unpublished không xuất hiện.

## 4. Metadata của một từ

```http
GET /api/v1/languages/ja/vocabulary/mizu
Authorization: Bearer <accessToken>
```

```json
{
  "id": "1563703640068192208",
  "languageCode": "ja",
  "code": "mizu",
  "term": "水",
  "reading": "みず",
  "audioUrl": null,
  "sortOrder": 0,
  "version": 1,
  "updatedAt": "2026-11-04T00:00:00+00:00"
}
```

Reading và AudioUrl có thể null. Không suy romaji từ Code; Reading là dữ liệu riêng.
Nếu AudioUrl null, FE ẩn hoặc disable nút nghe. Nếu có URL HTTPS public R2 thì dùng trực tiếp
cho audio player; không ghép ServiceUrl S3 hoặc cần gọi backend để tải từng file.
Seed từ vựng hiện chưa có audio, đúng phạm vi bước model.

Entry không tồn tại, unpublished hoặc thuộc language disabled đều trả 404.
Entry published nhưng chưa có nghĩa vẫn có metadata 200.

## 5. Các nghĩa và ghi chú

```http
GET /api/v1/languages/ja/vocabulary/kiku/senses?locale=vi&pageSize=20
Authorization: Bearer <accessToken>
```

Query gồm `locale`, `pageSize`, `after`, cùng validation/default như list.
Response page `{ items, nextCursor }`. Mỗi item:

```json
{
  "id": "1563703640068192339",
  "code": "listen",
  "partOfSpeech": "Verb",
  "meaning": "Nghe; lắng nghe.",
  "usageNote": "Cùng cách viết và cách đọc còn có nghĩa hỏi; chọn nghĩa theo ngữ cảnh.",
  "translationLocale": "vi",
  "isTranslationFallback": false,
  "sortOrder": 0,
  "version": 1,
  "updatedAt": "2026-11-04T00:00:00+00:00"
}
```

Seed `kiku` có cả nghĩa `listen` và `ask`, mỗi nghĩa có ID/locale riêng nhưng cùng EntryId.
Sắp `(sense.SortOrder, sense.Id)`; khóa ID tránh trùng nghĩa khi hai dòng cùng SortOrder.
Endpoint này trả mọi nghĩa, kể cả khi trước đó người dùng mở từ từ một list đang lọc từ loại.

Entry published không có nghĩa hoặc cursor đã hết: 200 page rỗng.
Entry không tồn tại/unpublished hoặc language disabled: 404.

## 6. Locale fallback và version

Với từng nghĩa, backend chọn **đúng locale yêu cầu → en → không có bản dịch**,
giống convention các catalog hiện có. Đây là lookup độc lập cho từng SenseId.

- Có đúng locale: TranslationLocale là locale đó, IsTranslationFallback=false.
- Không có đúng locale nhưng có en: trả Meaning/UsageNote tiếng Anh, IsTranslationFallback=true.
- Không có cả hai: Meaning/UsageNote/TranslationLocale đều null, IsTranslationFallback=true.
- Không chọn ngẫu nhiên một locale khác và không trộn Meaning của một bản dịch với UsageNote bản dịch khác.
- Không tự fallback parent tag: yêu cầu `vi-vn` không tự lấy `vi`. Nếu muốn bản Việt chung,
  FE gửi `vi` theo ExplanationLanguage đã cấu hình.
- Có exact `en-us` thì được ưu tiên khi yêu cầu `en-us`; thiếu exact thì mới lấy `en`.

FE có thể hiện nhãn ngôn ngữ thực tế từ TranslationLocale và thông báo chưa có bản dịch khi Meaning null.
Không gộp các nghĩa chỉ vì text/PartOfSpeech giống nhau; dùng SenseId làm key.

Version vẫn thuộc entity nguồn, không phải version tổng hợp:

- Entry metadata Version/UpdatedAt thuộc VocabularyEntry.
- Sense Version thuộc VocabularySense; UpdatedAt trong sense DTO là max của sense và bản dịch được chọn.
- List Version thuộc entry; UpdatedAt trong summary là max của entry, preview sense và bản dịch được chọn.
- Sửa bản dịch có thể đổi Meaning/UpdatedAt mà không tăng Entry.Version/Sense.Version.

Các endpoint hiện read-only, không dùng Version làm ETag hoặc điều kiện cho API sửa catalog.

## 7. Ghép với LearningProfile và FE

1. Dùng [LearningProfile API](learning-profiles-api.md) lấy profile hiện tại.
2. Gọi list với `languageCode = profile.targetLanguage`, `locale = profile.explanationLanguage`.
   Backend không phải query lại profile cho mỗi request catalog.
3. Render Term, Reading, preview Meaning/PartOfSpeech và AudioUrl nếu có. Một response list đủ cho cả trang card.
4. Khi mở một từ, dùng entry.Code để gọi metadata và senses **song song**.
5. Append các trang nghĩa theo NextCursor khi cần; không tải mọi nghĩa của mọi từ từ màn danh sách.
6. Đổi profile/locale/filter thì hủy request cũ, xóa cursor và tải lại dữ liệu tương ứng.

Ví dụ TypeScript sử dụng base API đã cấu hình của FE:

```ts
type PartOfSpeech =
  | 'Noun' | 'Verb' | 'Adjective' | 'Adverb' | 'Pronoun' | 'Determiner'
  | 'Adposition' | 'Conjunction' | 'Particle' | 'Interjection'
  | 'Numeral' | 'Auxiliary' | 'Other';
type SenseSummary = {
  id: string; code: string; partOfSpeech: PartOfSpeech; meaning: string | null;
  translationLocale: string | null; isTranslationFallback: boolean;
  sortOrder: number; version: number; updatedAt: string;
};
type Sense = SenseSummary & { usageNote: string | null };
type Entry = {
  id: string; languageCode: string; code: string; term: string;
  reading: string | null; audioUrl: string | null;
  sortOrder: number; version: number; updatedAt: string;
};
type EntrySummary = Entry & { primarySense: SenseSummary | null };
type Page<T> = { items: T[]; nextCursor: string | null };

async function getJson<T>(path: string, accessToken: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(path, {
    headers: { Authorization: `Bearer ${accessToken}` }, signal
  });
  if (!response.ok) throw await response.json(); // App xử lý ProblemDetails/401/429 dùng cơ chế chung.
  return response.json();
}

async function loadEntry(
  language: string, code: string, locale: string, accessToken: string, signal?: AbortSignal
) {
  const base = `/api/v1/languages/${encodeURIComponent(language)}/vocabulary/${encodeURIComponent(code)}`;
  const [entry, senses] = await Promise.all([
    getJson<Entry>(base, accessToken, signal),
    getJson<Page<Sense>>(`${base}/senses?${new URLSearchParams({ locale })}`, accessToken, signal)
  ]);
  return { entry, senses };
}
```

Ví dụ dùng URL tương đối khi FE có reverse proxy `/api/v1`. Nếu API khác origin thì ghép base URL
môi trường; CORS allowlist và JWT/refresh dùng cấu hình chung của dự án.

Cache key list gồm language, locale, PartOfSpeech, pageSize và cursor;
cache key senses gồm language, entry code, locale, pageSize và cursor.
Không dùng mỗi Entry.Version để quyết định nghĩa/bản dịch có đổi hay chưa.
Response cũ không được ghi vào màn hình của profile/locale mới sau khi người dùng chuyển lựa chọn.
Đăng xuất thì xóa cache cá nhân theo cơ chế FE hiện có.

Catalog được chia sẻ giữa các user đã xác thực; không nhận UserId/ProfileId trên API này.
GET không tạo progress, không tăng version, không đánh dấu người dùng đã học hoặc đã nhớ từ.

## 8. Lỗi, query budget và giới hạn

| HTTP | Ý nghĩa |
| --- | --- |
| 400 | Language tag/code, locale, enum, pageSize hoặc cursor không hợp lệ |
| 401 | Thiếu/sai/hết hạn JWT hoặc session xác thực đã bị revoke |
| 404 | Language không tồn tại/disabled; entry không tồn tại hoặc chưa công khai |
| 429 | Hết ngân sách rate limit; đọc header Retry-After |
| 500 | Lỗi ngoài dự kiến; ProblemDetails không trả chi tiết nội bộ |

Lỗi theo ProblemDetails hiện có. Các route được prefix qua convention `/api/v1`, controller kế thừa
ApiControllerBase/Authorize và dùng policy `learning-content` mặc định **240 request/IP/60 giây**,
chia sẻ với các catalog ngôn ngữ/bảng chữ/phát âm. Limiter chạy trước JWT validation và DB.
Không thêm policy, cache Redis, package hay cấu hình Enabled riêng.

Số SQL command của feature, không gồm shared JWT/session validation:

| Flow | SQL commands |
| --- | ---: |
| List có dữ liệu, kể cả lọc từ loại/locale | 1 |
| Metadata có/không có entry | 1 |
| Senses có dữ liệu | 1 |
| List rỗng | 2: thêm EXISTS phân biệt language không hợp lệ với trang/filter rỗng |
| Senses rỗng | 2: thêm EXISTS phân biệt entry ẩn/không tồn tại với trang rỗng |
| Input bị từ chối trước query | 0 |

Projection SQL giới hạn pageSize+1 trước khi lookup preview/bản dịch, dùng index parent+sort+id
và unique parent+locale hiện có. Không materialize entity graph, không OFFSET/COUNT tổng catalog,
không thêm index khi chưa có workload mới cần nó. Filter PartOfSpeech dùng EXISTS theo các nghĩa
của entry, không nhân đôi entry nếu nhiều nghĩa khớp.
Mỗi truy vấn đều kiểm tra language enabled và entry published trước khi trả nội dung con.
Chỉ đọc dữ liệu nên không mở transaction/khóa ghi cho các GET.

## 9. Kiểm thử và triển khai

Request mẫu trong [`src/Api/Api.http`](../src/Api/Api.http). Không có migration mới ở bước API.
Nếu môi trường chưa có catalog, áp migration model bằng lệnh hiện có:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Console runner `tests/Vocabulary.Tests` chạy Domain checks mặc định. Đặt `VOCABULARY_TEST_DATABASE`
thành connection string DB **trống, biệt lập**, tên bắt buộc `vocabulary_tests`, để chạy thêm
EF/PostgreSQL và HTTP thật qua Kestrel. Runner từ chối DB sai tên/đã có bảng, không truncate DB sẵn có.

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/Vocabulary.Tests/bin/Debug/net10.0/Vocabulary.Tests.dll
```

Auth scheme giả và credentials kiểm thử chỉ nằm trong test project; không thay JWT production,
không gọi OAuth, AI hoặc R2 thật. HTTP checks kiểm tra fallback từng nghĩa, missing translation/sense,
lọc từ loại, cursor với SortOrder trùng, publication/lang visibility, null audio/reading, chỉ đọc dữ liệu,
timestamp theo bản dịch, validation ở cả HTTP/Application và số SQL command thực tế.

Đã chạy thành công **382 kiểm tra** trên PostgreSQL 17 biệt lập: 97 Domain, 102 EF/PostgreSQL
và 183 HTTP/Application checks bổ sung. Query budget đo bằng DbCommandInterceptor; kiểm tra SQL
xác nhận list không tải UsageNote và không dùng OFFSET. Model vẫn khớp migration snapshot.
Container kiểm thử được dọn sau chạy; không apply migration hay ghi dữ liệu vào DB dự án/production.
