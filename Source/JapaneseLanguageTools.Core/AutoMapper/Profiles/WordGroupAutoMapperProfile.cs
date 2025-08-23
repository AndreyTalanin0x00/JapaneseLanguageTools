using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class WordGroupAutoMapperProfile : Profile
{
    public WordGroupAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<WordGroup, WordGroupModel>();

        CreateMap<WordGroupModel, WordGroup>()
            .AfterMap(ModelToEntity_SetWordNavigationProperties);

        static void ModelToEntity_SetWordNavigationProperties(WordGroupModel wordGroupModel, WordGroup wordGroup)
        {
            foreach (Word word in wordGroup.Words)
            {
                word.WordGroupId = wordGroup.Id;
                word.WordGroup = wordGroup;
            }
        }

        CreateMap<WordGroupModel, WordGroupIntegrationModel>().ReverseMap();

        CreateMap<WordGroupIntegrationModel, WordGroupJsonModel>();
        CreateMap<WordGroupIntegrationModel, WordGroupXmlModel>();

        CreateMap<WordGroupJsonModel, WordGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);
        CreateMap<WordGroupXmlModel, WordGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        ;
    }

    private static void ReplaceEmptyCommentWithNull(object wordGroupSerializedModel, WordGroupIntegrationModel wordGroupIntegrationModel)
    {
        wordGroupIntegrationModel.Comment = !string.IsNullOrWhiteSpace(wordGroupIntegrationModel.Comment)
            ? wordGroupIntegrationModel.Comment
            : null;

        ;
    }
}
