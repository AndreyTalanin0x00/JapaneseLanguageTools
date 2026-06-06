using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.AutoMapper.ValueResolvers;

public class CharacterTagCaptionStringValueResolver : IValueResolver<CharacterModel, CharacterIntegrationModel, string?>
{
    private static readonly string s_tagSeparator = $"{CharacterIntegrationModel.TagSeparator} ";

    public static readonly CharacterTagCaptionStringValueResolver Instance = new();

    private CharacterTagCaptionStringValueResolver()
    {
    }

    /// <inheritdoc />
    public string Resolve(CharacterModel characterModel, CharacterIntegrationModel characterIntegrationModel, string? tagCaptionString, ResolutionContext context)
    {
        IList<TagModel> tagModels = characterModel.CharacterTags;
        IEnumerable<string> tagCaptions = tagModels.Select(tagModel => tagModel.Caption);

        tagCaptionString = string.Join(s_tagSeparator, tagCaptions);

        return tagCaptionString;
    }
}
