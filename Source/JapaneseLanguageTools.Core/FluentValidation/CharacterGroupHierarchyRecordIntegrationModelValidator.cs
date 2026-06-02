using FluentValidation;

using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class CharacterGroupHierarchyRecordIntegrationModelValidator : AbstractValidator<CharacterGroupHierarchyRecordIntegrationModel>
{
    public CharacterGroupHierarchyRecordIntegrationModelValidator()
    {
        When(characterGroupHierarchyRecordIntegrationModel => characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupId <= 0, () =>
        {
            RuleFor(characterGroupHierarchyRecordIntegrationModel => characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupCaption)
                .NotEmpty();

            ;
        });
    }
}
