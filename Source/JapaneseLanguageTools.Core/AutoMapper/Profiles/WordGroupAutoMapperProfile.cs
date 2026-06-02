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

        CreateMap<WordGroupHierarchyRecord, WordGroupHierarchyRecordModel>();

        CreateMap<WordGroupModel, WordGroup>()
            .AfterMap(ModelToEntity_SetWordGroupHierarchyRecordNavigationProperties)
            .AfterMap(ModelToEntity_SetWordNavigationProperties);

        CreateMap<WordGroupHierarchyRecordModel, WordGroupHierarchyRecord>();

        static void ModelToEntity_SetWordGroupHierarchyRecordNavigationProperties(WordGroupModel wordGroupModel, WordGroup wordGroup)
        {
            foreach (WordGroupHierarchyRecord wordGroupHierarchyRecord in wordGroup.WordGroupHierarchyRecords)
            {
                wordGroupHierarchyRecord.WordGroupId = wordGroup.Id;
                wordGroupHierarchyRecord.WordGroup = wordGroup;
            }
        }

        static void ModelToEntity_SetWordNavigationProperties(WordGroupModel wordGroupModel, WordGroup wordGroup)
        {
            foreach (Word word in wordGroup.Words)
            {
                word.WordGroupId = wordGroup.Id;
                word.WordGroup = wordGroup;
            }
        }

        CreateMap<WordGroupModel, WordGroupIntegrationModel>().ReverseMap();

        CreateMap<WordGroupHierarchyRecordModel, WordGroupHierarchyRecordIntegrationModel>()
            .ForMember(wordGroupHierarchyRecordIntegrationModel => wordGroupHierarchyRecordIntegrationModel.NestedWordGroupCaption, options => options.MapFrom(wordGroupHierarchyRecordModel => wordGroupHierarchyRecordModel.NestedWordGroup!.Caption));

        CreateMap<WordGroupIntegrationModel, WordGroupJsonModel>();

        CreateMap<WordGroupHierarchyRecordIntegrationModel, WordGroupHierarchyRecordJsonModel>();

        CreateMap<WordGroupIntegrationModel, WordGroupXmlModel>();

        CreateMap<WordGroupHierarchyRecordIntegrationModel, WordGroupHierarchyRecordXmlModel>();

        CreateMap<WordGroupJsonModel, WordGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        CreateMap<WordGroupHierarchyRecordJsonModel, WordGroupHierarchyRecordIntegrationModel>();

        CreateMap<WordGroupXmlModel, WordGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        CreateMap<WordGroupHierarchyRecordXmlModel, WordGroupHierarchyRecordIntegrationModel>();
    }

    private static void ReplaceEmptyCommentWithNull(object wordGroupSerializedModel, WordGroupIntegrationModel wordGroupIntegrationModel)
    {
        wordGroupIntegrationModel.Comment = !string.IsNullOrWhiteSpace(wordGroupIntegrationModel.Comment)
            ? wordGroupIntegrationModel.Comment
            : null;

        ;
    }
}
