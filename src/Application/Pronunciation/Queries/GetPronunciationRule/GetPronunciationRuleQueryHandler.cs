using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Pronunciation.Models;

namespace Application.Pronunciation.Queries.GetPronunciationRule;

public sealed class GetPronunciationRuleQueryHandler(IPronunciationReadRepository repository)
{
    public async Task<PronunciationRuleDto> Handle(GetPronunciationRuleQuery query, CancellationToken ct)
    {
        var language = CatalogQueryValidation.Language(query.LanguageCode);
        var code = CatalogQueryValidation.Code(query.RuleCode);
        var locale = CatalogQueryValidation.Locale(query.Locale);
        return await repository.GetByCodeAsync(language, code, locale, ct)
            ?? throw new NotFoundException("Pronunciation rule was not found.");
    }
}
