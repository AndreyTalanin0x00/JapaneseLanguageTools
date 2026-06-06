using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;

namespace JapaneseLanguageTools.Core.AutoMapper.ValueResolvers;

public class WordTagCaptionStringValueResolver : IValueResolver<WordModel, WordIntegrationModel, string?>
{
    private static readonly string s_tagSeparator = $"{WordIntegrationModel.TagSeparator} ";

    public static readonly WordTagCaptionStringValueResolver Instance = new();

    private WordTagCaptionStringValueResolver()
    {
    }

    /// <inheritdoc />
    public string Resolve(WordModel wordModel, WordIntegrationModel wordIntegrationModel, string? tagCaptionString, ResolutionContext context)
    {
        IList<TagModel> tagModels = wordModel.WordTags;
        IEnumerable<string> tagCaptions = tagModels.Select(tagModel => tagModel.Caption);

        tagCaptionString = string.Join(s_tagSeparator, tagCaptions);

        return tagCaptionString;
    }
}
