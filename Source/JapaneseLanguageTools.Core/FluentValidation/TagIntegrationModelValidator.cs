using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.FluentValidation;

public class TagIntegrationModelValidator : AbstractValidator<TagIntegrationModel>
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
    public TagIntegrationModelValidator()
    {
        RuleFor(tagIntegrationModel => tagIntegrationModel.Action)
            .Must(action => s_snapshotObjectActionsAvailable.Contains(action))
            .IsInEnum();

        When(tagIntegrationModel => s_snapshotObjectActionsIdRequired.Contains(tagIntegrationModel.Action), () =>
        {
            RuleFor(tagIntegrationModel => tagIntegrationModel.Id)
                .GreaterThan(0);

            ;
        });

        RuleFor(tagIntegrationModel => tagIntegrationModel.Caption)
            .Must(caption => caption.Length == caption.Trim().Length)
            .NotEmpty();

        static bool HasNoTrailingWhitespaces(string? value) => value?.Length == value?.Trim().Length;

        When(tagIntegrationModel => tagIntegrationModel.PlaceholderMarker is not null, () =>
        {
            RuleFor(tagIntegrationModel => tagIntegrationModel.PlaceholderMarker)
                .Must(placeholderMarker => HasNoTrailingWhitespaces(placeholderMarker))
                .NotEmpty();

            ;
        });
    }
}
