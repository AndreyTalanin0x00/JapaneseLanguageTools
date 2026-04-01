using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class CharacterIntegrationModelValidator : AbstractValidator<CharacterIntegrationModel>
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotObjectAction> s_snapshotObjectActionsAvailable = new()
    {
        SnapshotObjectAction.Add,
        SnapshotObjectAction.Update,
        SnapshotObjectAction.AddOrUpdate,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotObjectAction> s_snapshotObjectActionsIdRequired = new()
    {
        SnapshotObjectAction.Update,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0200:Remove unnecessary lambda expression", Justification = "Personal preference.")]
    public CharacterIntegrationModelValidator()
    {
        RuleFor(characterIntegrationModel => characterIntegrationModel.Action)
            .Must(action => s_snapshotObjectActionsAvailable.Contains(action))
            .IsInEnum();

        When(characterIntegrationModel => s_snapshotObjectActionsIdRequired.Contains(characterIntegrationModel.Action), () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Id)
                .GreaterThan(0);

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.CharacterGroupId is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.CharacterGroupId)
                .GreaterThan(0);

            ;
        });

        RuleFor(characterIntegrationModel => characterIntegrationModel.Symbol)
            .NotEmpty();

        RuleFor(characterIntegrationModel => characterIntegrationModel.Type)
            .IsInEnum();

        static bool HasNoTrailingWhitespaces(string? value) => value?.Length == value?.Trim().Length;

        When(characterIntegrationModel => characterIntegrationModel.Pronunciation is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Pronunciation)
                .Must(pronunciation => HasNoTrailingWhitespaces(pronunciation))
                .NotEmpty();

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.Syllable is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Syllable)
                .Must(syllable => HasNoTrailingWhitespaces(syllable))
                .NotEmpty();

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.Onyomi is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Onyomi)
                .Must(onyomi => HasNoTrailingWhitespaces(onyomi))
                .NotEmpty();

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.Kunyomi is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Kunyomi)
                .Must(kunyomi => HasNoTrailingWhitespaces(kunyomi))
                .NotEmpty();

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.Meaning is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Meaning)
                .Must(meaning => HasNoTrailingWhitespaces(meaning))
                .NotEmpty();

            ;
        });

        When(characterIntegrationModel => characterIntegrationModel.Tags is not null, () =>
        {
            RuleFor(characterIntegrationModel => characterIntegrationModel.Tags)
                .Must(tags => HasNoTrailingWhitespaces(tags))
                .NotEmpty();

            ;
        });
    }
}
