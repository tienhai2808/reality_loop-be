namespace Application.Handwriting.Queries.GetHandwritingTemplate;

public sealed record GetHandwritingTemplateQuery(long ProfileId, string SystemCode, long TemplateId);
