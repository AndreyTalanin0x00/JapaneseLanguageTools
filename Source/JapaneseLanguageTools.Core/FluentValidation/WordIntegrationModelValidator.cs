using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class WordIntegrationModelValidator : AbstractValidator<WordIntegrationModel>
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
    public WordIntegrationModelValidator()
    {
        RuleFor(wordIntegrationModel => wordIntegrationModel.Action)
            .Must(action => s_snapshotObjectActionsAvailable.Contains(action))
            .IsInEnum();

        When(wordIntegrationModel => s_snapshotObjectActionsIdRequired.Contains(wordIntegrationModel.Action), () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Id)
                .GreaterThan(0);

            ;
        });

        When(wordIntegrationModel => wordIntegrationModel.WordGroupId is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.WordGroupId)
                .GreaterThan(0);

            ;
        });

        RuleFor(wordIntegrationModel => wordIntegrationModel.Characters)
            .NotEmpty();

        static bool IsInCharacterTypesEnum(CharacterTypes value)
        {
            if (value == CharacterTypes.None)
                return true;

            const int bitCount = sizeof(CharacterTypes) * 8;

            for (int flag = 1, bitIndex = 0; bitIndex < bitCount; flag *= 2, bitIndex++)
            {
                if ((value & (CharacterTypes)flag) > 0 && !Enum.IsDefined((CharacterTypes)flag))
                    return false;
            }

            return true;
        }

        RuleFor(wordIntegrationModel => wordIntegrationModel.CharacterTypes)
            .Must(characterTypes => IsInCharacterTypesEnum(characterTypes));

        static bool HasNoTrailingWhitespaces(string? value) => value?.Length == value?.Trim().Length;

        When(wordIntegrationModel => wordIntegrationModel.Pronunciation is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Pronunciation)
                .Must(pronunciation => HasNoTrailingWhitespaces(pronunciation))
                .NotEmpty();

            ;
        });

        When(wordIntegrationModel => wordIntegrationModel.Furigana is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Furigana)
                .Must(furigana => HasNoTrailingWhitespaces(furigana))
                .NotEmpty();

            ;
        });

        When(wordIntegrationModel => wordIntegrationModel.Okurigana is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Okurigana)
                .Must(okurigana => HasNoTrailingWhitespaces(okurigana))
                .NotEmpty();

            ;
        });

        When(wordIntegrationModel => wordIntegrationModel.Meaning is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Meaning)
                .Must(meaning => HasNoTrailingWhitespaces(meaning))
                .NotEmpty();

            ;
        });

        When(wordIntegrationModel => wordIntegrationModel.Tags is not null, () =>
        {
            RuleFor(wordIntegrationModel => wordIntegrationModel.Tags)
                .Must(tags => HasNoTrailingWhitespaces(tags))
                .NotEmpty();

            ;
        });
    }
}
