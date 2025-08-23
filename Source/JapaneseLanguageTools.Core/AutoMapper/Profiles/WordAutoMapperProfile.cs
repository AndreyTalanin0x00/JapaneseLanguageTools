using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class WordAutoMapperProfile : Profile
{
    public WordAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Word, WordModel>();

        CreateMap<WordModel, Word>()
            .AfterMap(ModelToEntity_SetWordTagNavigationProperties);

        CreateMap<IEnumerable<WordTag>, IEnumerable<TagModel>>().ConstructUsing((wordTags, context) =>
        {
            return wordTags
                .Select(wordTag => wordTag.Tag)
                .Select(tag => context.Mapper.Map<TagModel>(tag))
                .ToList();
        });

        CreateMap<IEnumerable<TagModel>, IEnumerable<WordTag>>().ConstructUsing((tagModels, context) =>
        {
            return tagModels
                .Select(tagModel => context.Mapper.Map<Tag>(tagModel))
                .Select(tag => new WordTag() { TagId = tag.Id, Tag = tag })
                .ToList();
        });

        CreateMap<WordModel, WordIntegrationModel>().ReverseMap();

        CreateMap<WordIntegrationModel, WordJsonModel>().ReverseMap();
        CreateMap<WordIntegrationModel, WordXmlModel>().ReverseMap();

        static void ModelToEntity_SetWordTagNavigationProperties(WordModel wordModel, Word word)
        {
            foreach (WordTag wordTag in word.WordTags)
            {
                wordTag.WordId = word.Id;
                wordTag.Word = word;
            }
        }
    }
}
