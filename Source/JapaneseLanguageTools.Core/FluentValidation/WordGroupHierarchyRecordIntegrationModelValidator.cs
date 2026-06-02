using FluentValidation;

using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class WordGroupHierarchyRecordIntegrationModelValidator : AbstractValidator<WordGroupHierarchyRecordIntegrationModel>
{
    public WordGroupHierarchyRecordIntegrationModelValidator()
    {
        When(wordGroupHierarchyRecordIntegrationModel => wordGroupHierarchyRecordIntegrationModel.NestedWordGroupId <= 0, () =>
        {
            RuleFor(wordGroupHierarchyRecordIntegrationModel => wordGroupHierarchyRecordIntegrationModel.NestedWordGroupCaption)
                .NotEmpty();

            ;
        });
    }
}
