using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class WordGroupIntegrationModelValidator : AbstractValidator<WordGroupIntegrationModel>
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotObjectAction> s_snapshotObjectActionsAvailable = new()
    {
        SnapshotObjectAction.Add,
        SnapshotObjectAction.Update,
        SnapshotObjectAction.AddOrUpdate,
        SnapshotObjectAction.ChangeState,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly HashSet<SnapshotObjectAction> s_snapshotObjectActionsIdRequired = new()
    {
        SnapshotObjectAction.Update,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0200:Remove unnecessary lambda expression", Justification = "Personal preference.")]
    public WordGroupIntegrationModelValidator()
    {
        RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Action)
            .Must(action => s_snapshotObjectActionsAvailable.Contains(action))
            .IsInEnum();

        When(wordGroupIntegrationModel => s_snapshotObjectActionsIdRequired.Contains(wordGroupIntegrationModel.Action), () =>
        {
            RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Id)
                .GreaterThan(0);

            ;
        });

        RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Caption)
            .NotEmpty();

        static bool HasNoTrailingWhitespaces(string? value) => value?.Length == value?.Trim().Length;

        When(wordGroupIntegrationModel => wordGroupIntegrationModel.Comment is not null, () =>
        {
            RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Comment)
                .Must(comment => HasNoTrailingWhitespaces(comment))
                .NotEmpty();

            ;
        });

        RuleFor(wordGroupIntegrationModel => wordGroupIntegrationModel.Words)
            .Must((wordGroupIntegrationModel, words) =>
            {
                bool wordActionSelected = words
                    .Where(wordIntegrationModel => wordIntegrationModel.Action != SnapshotObjectAction.None)
                    .Any();
                bool wordGroupActionSelected = wordGroupIntegrationModel.Action != SnapshotObjectAction.None;

                return !(wordActionSelected && wordGroupActionSelected);
            })
            .WithMessage("Unable to perform an action on a child entity when there is an action selected for the container entity.");

        RuleForEach(wordGroupIntegrationModel => wordGroupIntegrationModel.Words)
            .SetValidator(new WordIntegrationModelValidator());

        ;
    }
}
