using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class ApplicationDictionaryObjectPackageIntegrationModelValidator : AbstractValidator<ApplicationDictionaryObjectPackageIntegrationModel>
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

    public ApplicationDictionaryObjectPackageIntegrationModelValidator()
    {
        RuleFor(applicationDictionaryObjectPackageIntegrationModel => applicationDictionaryObjectPackageIntegrationModel.SnapshotType)
            .NotEqual(SnapshotType.Unknown)
            .IsInEnum();

        When(applicationDictionaryObjectPackageIntegrationModel => s_snapshotTypesAllSnapshotObjectActions.Contains(applicationDictionaryObjectPackageIntegrationModel.SnapshotType), () =>
        {
            RuleFor(applicationDictionaryObjectPackageIntegrationModel => applicationDictionaryObjectPackageIntegrationModel.ApplicationDictionary)
                .ChildRules((applicationDictionaryIntegrationModelValidator) =>
                {
                    applicationDictionaryIntegrationModelValidator.RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Characters)
                        .ChildRules((characterIntegrationModelValidator) =>
                        {
                            characterIntegrationModelValidator
                                .RuleFor(characterIntegrationModel => characterIntegrationModel.Action)
                                .IsInEnum();

                            ;
                        });
                    applicationDictionaryIntegrationModelValidator.RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.CharacterGroups)
                        .ChildRules((characterGroupIntegrationModelValidator) =>
                        {
                            characterGroupIntegrationModelValidator
                                .RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Action)
                                .IsInEnum();

                            ;
                        });

                    applicationDictionaryIntegrationModelValidator.RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Words)
                        .ChildRules((wordIntegrationModelValidator) =>
                        {
                            wordIntegrationModelValidator
                                .RuleFor(wordIntegrationModel => wordIntegrationModel.Action)
                                .IsInEnum();

                            ;
                        });
                    applicationDictionaryIntegrationModelValidator.RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.WordGroups)
                        .ChildRules((wordGroupIntegrationModelValidator) =>
                        {
                            wordGroupIntegrationModelValidator
                                .RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Action)
                                .IsInEnum();

                            ;
                        });

                    applicationDictionaryIntegrationModelValidator.RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Tags)
                        .ChildRules((tagIntegrationModelValidator) =>
                        {
                            tagIntegrationModelValidator
                                .RuleFor(tagIntegrationModel => tagIntegrationModel.Action)
                                .IsInEnum();

                            ;
                        });

                    ;
                });

            ;
        });

        When(applicationDictionaryObjectPackageIntegrationModel => s_snapshotTypesChangeStateSnapshotObjectActions.Contains(applicationDictionaryObjectPackageIntegrationModel.SnapshotType), () =>
        {
            RuleFor(applicationDictionaryObjectPackageIntegrationModel => applicationDictionaryObjectPackageIntegrationModel.ApplicationDictionary)
                .ChildRules((applicationDictionaryIntegrationModelValidator) =>
                {
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Characters)
                        .ChildRules((characterIntegrationModelValidator) =>
                        {
                            characterIntegrationModelValidator
                                .RuleFor(characterIntegrationModel => characterIntegrationModel.Action)
                                .Must(action => action is SnapshotObjectAction.None or SnapshotObjectAction.ChangeState)
                                .IsInEnum();

                            ;
                        });
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.CharacterGroups)
                        .ChildRules((characterGroupIntegrationModelValidator) =>
                        {
                            characterGroupIntegrationModelValidator
                                .RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Action)
                                .Must(action => action is SnapshotObjectAction.None or SnapshotObjectAction.ChangeState)
                                .IsInEnum();

                            ;
                        });

                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Words)
                        .ChildRules((wordIntegrationModelValidator) =>
                        {
                            wordIntegrationModelValidator
                                .RuleFor(wordIntegrationModel => wordIntegrationModel.Action)
                                .Must(action => action is SnapshotObjectAction.None or SnapshotObjectAction.ChangeState)
                                .IsInEnum();

                            ;
                        });
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.WordGroups)
                        .ChildRules((wordGroupIntegrationModelValidator) =>
                        {
                            wordGroupIntegrationModelValidator
                                .RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Action)
                                .Must(action => action is SnapshotObjectAction.None or SnapshotObjectAction.ChangeState)
                                .IsInEnum();

                            ;
                        });

                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Tags)
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

            ;
        });

        When(applicationDictionaryObjectPackageIntegrationModel => s_snapshotTypesChangeStateSnapshotObjectActions.Contains(applicationDictionaryObjectPackageIntegrationModel.SnapshotType), () =>
        {
            RuleFor(applicationDictionaryObjectPackageIntegrationModel => applicationDictionaryObjectPackageIntegrationModel.ApplicationDictionary)
                .ChildRules((applicationDictionaryIntegrationModelValidator) =>
                {
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Characters)
                        .SetValidator(new CharacterIntegrationModelValidator());
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.CharacterGroups)
                        .SetValidator(new CharacterGroupIntegrationModelValidator());

                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Words)
                        .SetValidator(new WordIntegrationModelValidator());
                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.WordGroups)
                        .SetValidator(new WordGroupIntegrationModelValidator());

                    applicationDictionaryIntegrationModelValidator
                        .RuleForEach(applicationDictionaryIntegrationModel => applicationDictionaryIntegrationModel.Tags)
                        .SetValidator(new TagIntegrationModelValidator());

                    ;
                });

            ;
        });
    }
}
