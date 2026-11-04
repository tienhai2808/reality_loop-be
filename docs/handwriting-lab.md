# Thử nghiệm chấm nét viết không dùng AI

## Phạm vi

`tools/HandwritingLab` là công cụ phát triển độc lập để kiểm chứng thuật toán và thu mẫu hiệu chỉnh.
Lab có bảng viết trên trình duyệt, bộ chấm .NET và dữ liệu nét chuẩn đóng gói offline.
Các production project `Domain`, `Application`, `Infrastructure`, `Api` giữ nguyên kiến trúc.
Lab hiện tham chiếu bộ chấm thuần .NET trong `Application/Handwriting/Scoring` qua adapter, cùng implementation
với API production; host lab vẫn độc lập DB/auth và không ghi kết quả vào progress hoặc practice session.
Xem [model/DB](handwriting-model.md) và [API production](handwriting-api.md) cho phần đã triển khai tiếp theo.

Coverage ban đầu: **あ / お**, **ぬ / め**, và bài hai chữ **あお**.
Mỗi chữ một ô logic riêng; UI chuyển ô qua tab, giữ nét của các ô trước.
Đây là chấm một đề đã biết, chưa nhận dạng từ bất kỳ từ ảnh/bảng viết tự do.

## Chạy bản thử

Không cần database, Redis, R2, user-secrets, AI key hoặc cấu hình của API chính.
Không thêm NuGet/npm package. Từ root backend:

```bash
dotnet build tools/HandwritingLab/HandwritingLab.csproj -m:1 -p:UseSharedCompilation=false
dotnet tools/HandwritingLab/bin/Debug/net10.0/HandwritingLab.dll
```

Mở **http://127.0.0.1:5387**. Đổi port bằng `--port 5388` sau đường dẫn DLL.
Host luôn bind loopback; cấu hình URL của API chính không đổi địa chỉ này.
App chỉ nhận Host loopback và request cùng origin khi browser gửi Origin.
Không deploy host lab thành API công khai; API production đi qua JWT/rate limiting/ownership hiện có.

Chọn chữ → viết bằng chuột/ngón tay/bút → nhấc tay giữa các nét → bấm **Chấm bài**.
Có hoàn tác nét, xóa ô, bật/tắt mẫu và xem phản hồi từng chữ/từng nét.
Chấm sau khi nộp bài; chưa có tự chấm từng nét khi chữ mới viết được một phần.
Request bị hủy hoặc nội dung đã sửa sẽ không dùng response cũ để ghi đè feedback mới.

## Cách chấm

FE gửi chuỗi tọa độ, không gửi ảnh hoặc kết quả tự khai `isCorrect`.
Backend chọn template từ TargetId đã biết và tính lại kết quả hoàn toàn bằng toán học:

1. Kiểm tra số chữ, số nét, số điểm, tọa độ finite và giới hạn payload.
2. Thiếu/thừa nét hoặc không có bài viết: trả feedback tương ứng.
3. Kiểm tra kích thước tối thiểu của hình trước khi normalize để dấu chấm nhỏ không được phóng thành chữ.
4. Căn giữa và co giãn **đồng đều toàn bộ chữ**. Giữ tỉ lệ ngang/dọc, vị trí tương đối các nét;
   không tự xoay, lật hoặc căn chỉnh riêng từng nét để ép khớp.
5. Ramer–Douglas–Peucker loại nhiễu nhỏ trước khi đo độ dài nét. Đây là bước sửa lỗi được phát hiện
   bởi ca rung nhẹ: nhiều điểm jitter có thể làm raw path dài bất thường dù hình đúng.
6. Resample theo độ dài đường đi thành 48 điểm/nét để giảm ảnh hưởng tốc độ và mật độ pointer event.
7. So sánh bằng discrete Fréchet distance, chiều đi, tỉ lệ chiều dài và thứ tự nét.
   So ngược chiều/permutation chỉ dùng để phân loại lỗi; không tự sửa rồi coi là viết đúng.
8. Chữ chỉ đạt nếu mọi nét đạt; từ chỉ đạt nếu mọi chữ đạt.

`ScoringPolicy` giữ các ngưỡng cố định trên server, version **geometry-v1**:

| Tham số | Giá trị |
| --- | ---: |
| Resample | 48 điểm/nét |
| Simplification tolerance | 0.01 sau normalize |
| Shape tolerance | 0.10 sau normalize |
| Kích thước tối thiểu | 0.18 trong hệ tọa độ bảng viết 0..1 |
| Độ dài nét tối thiểu trước normalize | 0.008 |
| Tỉ lệ chiều dài nét | 0.65–1.50 so với template |

MatchScore là chỉ số khớp hình học để giải thích feedback, **không phải xác suất viết đúng**.
Pass/fail không lấy trung bình điểm rồi bỏ qua một nét sai. Tolerance hiện tại mới được kiểm tra
trên các ca tổng hợp; chưa đo false accept/false reject trên bộ chữ viết tay được người có chuyên môn gắn nhãn.
Mọi thay đổi thuật toán/ngưỡng sau khi phát hành dữ liệu hiệu chỉnh cần tăng AlgorithmVersion.

## API cục bộ của lab

Các endpoint này thuộc host công cụ, không phải controller production:

| Method | Route | Mục đích |
| --- | --- | --- |
| GET | `/api/v1/handwriting-lab/templates` | Policy, nguồn/license, 4 template và 5 target |
| POST | `/api/v1/handwriting-lab/grade` | Chấm toàn bộ các ô chữ từ tọa độ |

Body:

```json
{
  "targetId": "a",
  "characters": [
    [
      [{ "x": 0.28, "y": 0.30 }, { "x": 0.40, "y": 0.31 }]
    ]
  ]
}
```

Thứ tự lồng: **characters → strokes → points**. Ví dụ trên chỉ có một nét nên được chấm thiếu nét
cho あ. TargetId nhận `a`, `o`, `nu`, `me`, `ao`. Backend biết các chữ trong từng target;
không nhận SVG/template/threshold/score từ client.

Response gồm TargetId, AlgorithmVersion, TemplateRevision, Passed, MatchScore và kết quả từng chữ:
Symbol, ExpectedStrokeCount, ActualStrokeCount, Issues, danh sách StrokeGrade.
StrokeGrade.Index bắt đầu 0; UI hiển thị index+1, kèm distance, length ratio và lỗi.

Issue codes: `MissingDrawing`, `WrongStrokeCount`, `DrawingTooSmall`, `WrongStrokeOrder`,
`WrongDirection`, `StrokeTooShort`, `StrokeTooLong`, `WrongShape`.

Giới hạn: 256 KiB body; tối đa 4 ô theo policy (target hiện có chỉ cần 1 hoặc 2), 12 nét/ô,
512 điểm/nét, 4096 điểm/toàn bài. Tọa độ x/y phải là số JSON finite trong [0,1], có đủ hai field.
Malformed JSON, field ngoài contract, null array hoặc vượt giới hạn cấu trúc → 400;
payload quá lớn → 413; sai Content-Type → 415; Host/Origin không phù hợp → 403.
Các lỗi HTTP có ProblemDetails JSON. Bài viết hợp lệ về cấu trúc nhưng sai chữ/số nét vẫn trả 200 với Passed=false.

Không có HTTP request theo từng pixel/pointer event. FE giữ nét trong bộ nhớ, gửi một bài khi người dùng nộp.
Pointer capture, single active pointer, pointercancel/lostpointercapture và devicePixelRatio được xử lý ở bảng viết.
Không lưu bài viết vào DB/localStorage hoặc tự gửi lên nguồn dữ liệu bên ngoài.

## Nguồn mẫu và khả năng tái tạo

Nguồn: [KanjiVG](https://github.com/KanjiVG/kanjivg), copyright Ulrich Apel, **CC BY-SA 3.0**.
Commit ghim: `70a0b7ae0c18ceb5cb358274b029cce0234a43bc`.
Raw SVG: 03042 (あ), 0304a (お), 0306c (ぬ), 03081 (め), giữ nguyên trong `Data/KanjiVG/` cùng COPYING.
JSON chuyển đổi có attribution và license; UI cũng hiện attribution và link license.

`import_templates.py` kiểm tra SHA-256 từng nguồn, viewBox, số nét và bounds;
lấy mẫu 32 bước trên mỗi cubic Bézier, chia tọa độ cho 109 và làm tròn 6 chữ số.
Importer chỉ hỗ trợ các lệnh M/L/C cần cho bốn file này và fail nếu gặp lệnh khác;
không phải parser SVG tổng quát. Rebuild hoàn toàn offline:

```bash
python3 tools/HandwritingLab/import_templates.py
```

App nạp template thành các collection read-only và không sửa dữ liệu khi chấm.
Không tải KanjiVG/CDN lúc người dùng viết. Số nét theo mẫu ghim: あ=3, お=3, ぬ=2, め=2.
Các biến thể chữ viết tay hoặc cách chia nét khác chưa được bảo đảm chấp nhận.

## Thu dữ liệu hiệu chỉnh

Mở **Kiểm tra bộ chấm & lưu mẫu viết**:

- Mẫu chuẩn/đảo chiều/đảo thứ tự/chữ dễ nhầm là dữ liệu sinh sẵn để debug, có nguồn `generated`.
- Viết thật, chọn nhãn “Tôi viết đúng” hoặc “Tôi cố tình viết sai”, rồi **Xuất bài viết JSON**.
- JSON giữ formatVersion, target, tọa độ, nhãn, nguồn nét, pointer types, AlgorithmVersion,
  TemplateRevision và thời điểm xuất. Tải về máy theo thao tác của người dùng.
- Nét sinh sẵn pha thêm nét mới được đánh dấu `mixed`; không tự coi cả bài là mẫu người viết thật.
- **Mở mẫu JSON** khôi phục tọa độ/nhãn để xem và chấm lại; kiểm tra kích thước file, version template và giới hạn điểm.

Nhãn là thông tin do người thử khai, không phải ground truth được tự xác minh.
Các capture/screenshot trong `artifacts/handwriting-lab` được browser smoke test tạo bằng sự kiện
tổng hợp, **không phải corpus chữ viết tay của người thật**, kể cả pointer type là mouse/touch.

Trước khi tích hợp học thật, cần thu mẫu từ nhiều người và nhiều thiết bị, có người biết chữ Nhật
đối chiếu nhãn/biến thể, rồi tách tập chỉnh ngưỡng và tập kiểm chứng độc lập.
Theo dõi riêng chữ đúng bị từ chối và chữ sai được chấp nhận cho từng cặp, từng thiết bị.
Sau khi đạt yêu cầu sản phẩm mới chốt dữ liệu template/variant, model/DB lượt viết và API lưu kết quả.

## Kiểm thử

```bash
dotnet build RealityLoop.slnx -m:1 -p:UseSharedCompilation=false
dotnet tests/HandwritingLab.Tests/bin/Debug/net10.0/HandwritingLab.Tests.dll
```

Console runner không cần DB hoặc package test mới. **162 kiểm tra** gồm:

- Template chuẩn, sai chữ, rung nhẹ tổng hợp, co giãn/dịch toàn chữ, lật/xoay.
- Đảo chiều từng nét, đổi thứ tự, thiếu/thừa nét, tap, chữ quá nhỏ, nét lệch và vòng lặp thừa.
- Bài hai chữ, input không hữu hạn, payload/điểm/nét quá giới hạn, không sửa template.
- 100 lượt chấm đồng thời cho tính nhất quán; số đo máy local không phải load test production.
- HTTP thật: asset/template, request chuẩn, validation, 400/403/413/415, same-origin và dữ liệu nguồn/license.

Kiểm tra trình duyệt bằng Chrome cài sẵn và Node 24, không dùng npm package; chạy lab trước:

```bash
node tools/HandwritingLab/check-browser.mjs http://127.0.0.1:5387
```

Script tạo profile Chrome tạm và đóng sau chạy, không dùng profile trình duyệt cá nhân.
Đã qua **20 browser checks** cho chuột/cảm ứng, ô chữ, feedback, undo/clear, export/import và layout 390px;
chụp desktop/mobile vào `artifacts/handwriting-lab/` (gitignored).
Những kiểm tra này chứng minh hoạt động phần mềm và ca hình học tổng hợp;
không được diễn giải số test pass thành tỷ lệ chấm đúng chữ viết tay ngoài thực tế.
