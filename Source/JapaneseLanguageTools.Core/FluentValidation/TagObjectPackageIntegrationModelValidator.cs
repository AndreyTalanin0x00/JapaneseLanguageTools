using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class TagObjectPackageIntegrationModelValidator : AbstractValidator<TagObjectPackageIntegrationModel>
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotType> s_snapshotTypesAllSnapshotObjectActions = new()
    {
        SnapshotType.General,
        SnapshotType.GeneralNoAction,
        SnapshotType.Patch,
    };

    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotType> s_snapshotTypesChangeStateSnapshotObjectActions = new()
    {
        SnapshotType.ChangeState,
    };

    public TagObjectPackageIntegrationModelValidator()
    {
        RuleFor(tagObjectPackageIntegrationModel => tagObjectPackageIntegrationModel.SnapshotType)
            .NotEqual(SnapshotType.Unknown)
            .IsInEnum();

        When(tagObjectPackageIntegrationModel => s_snapshotTypesAllSnapshotObjectActions.Contains(tagObjectPackageIntegrationModel.SnapshotType), () =>
        {
            RuleForEach(tagObjectPackageIntegrationModel => tagObjectPackageIntegrationModel.Tags)
                .ChildRules((tagIntegrationModelValidator) =>
                {
                    tagIntegrationModelValidator
                        .RuleFor(tagIntegrationModel => tagIntegrationModel.Action)
                        .IsInEnum();

                    ;
                });

            ;
        });

        When(tagObjectPackageIntegrationModel => s_snapshotTypesChangeStateSnapshotObjectActions.Contains(tagObjectPackageIntegrationModel.SnapshotType), () =>
        {
            RuleForEach(tagObjectPackageIntegrationModel => tagObjectPackageIntegrationModel.Tags)
                .ChildRules((tagIntegrationModelValidator) =>
                {
                    tagIntegrationModelValidator
                        .RuleFor(tagIntegrationModel => tagIntegrationModel.Action)
                        .Must(action => action is SnapshotObjectAction.None or SnapshotObjectAction.ChangeState)
                        .IsInEnum();

                    ;
                });

            ;
        });

        RuleForEach(tagObjectPackageIntegrationModel => tagObjectPackageIntegrationModel.Tags)
            .SetValidator(new TagIntegrationModelValidator());

        ;
    }
}
