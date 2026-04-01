using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class CharacterGroupIntegrationModelValidator : AbstractValidator<CharacterGroupIntegrationModel>
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
    public CharacterGroupIntegrationModelValidator()
    {
        RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Action)
            .Must(action => s_snapshotObjectActionsAvailable.Contains(action))
            .IsInEnum();

        When(characterGroupIntegrationModel => s_snapshotObjectActionsIdRequired.Contains(characterGroupIntegrationModel.Action), () =>
        {
            RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Id)
                .GreaterThan(0);

            ;
        });

        RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Caption)
            .NotEmpty();

        static bool HasNoTrailingWhitespaces(string? value) => value?.Length == value?.Trim().Length;

        When(characterGroupIntegrationModel => characterGroupIntegrationModel.Comment is not null, () =>
        {
            RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Comment)
                .Must(comment => HasNoTrailingWhitespaces(comment))
                .NotEmpty();

            ;
        });

        RuleFor(characterGroupIntegrationModel => characterGroupIntegrationModel.Characters)
            .Must((characterGroupIntegrationModel, characters) =>
            {
                bool characterActionSelected = characters
                    .Where(characterIntegrationModel => characterIntegrationModel.Action != SnapshotObjectAction.None)
                    .Any();
                bool characterGroupActionSelected = characterGroupIntegrationModel.Action != SnapshotObjectAction.None;

                return !(characterActionSelected && characterGroupActionSelected);
            })
            .WithMessage("Unable to perform an action on a child entity when there is an action selected for the container entity.");

        RuleForEach(characterGroupIntegrationModel => characterGroupIntegrationModel.Characters)
            .SetValidator(new CharacterIntegrationModelValidator());

        ;
    }
}
