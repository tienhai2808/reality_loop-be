using System.Globalization;

namespace Application.Common.Models;

public sealed record CatalogCursor(int SortOrder, long Id)
{
    public static string Encode(int sortOrder, string id) =>
        sortOrder.ToString(CultureInfo.InvariantCulture) + ":" + id;
}
