namespace Application.Handwriting.Queries.GetHandwritingTemplates;

public sealed record GetHandwritingTemplatesQuery(long ProfileId, string SystemCode, int PageSize = 20, string? After = null);
