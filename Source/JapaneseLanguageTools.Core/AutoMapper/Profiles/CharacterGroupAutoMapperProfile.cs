using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class CharacterGroupAutoMapperProfile : Profile
{
    public CharacterGroupAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<CharacterGroup, CharacterGroupModel>();

        CreateMap<CharacterGroupHierarchyRecord, CharacterGroupHierarchyRecordModel>();

        CreateMap<CharacterGroupModel, CharacterGroup>()
            .AfterMap(ModelToEntity_SetCharacterGroupHierarchyRecordNavigationProperties)
            .AfterMap(ModelToEntity_SetCharacterNavigationProperties);

        CreateMap<CharacterGroupHierarchyRecordModel, CharacterGroupHierarchyRecord>();

        static void ModelToEntity_SetCharacterGroupHierarchyRecordNavigationProperties(CharacterGroupModel characterGroupModel, CharacterGroup characterGroup)
        {
            foreach (CharacterGroupHierarchyRecord characterGroupHierarchyRecord in characterGroup.CharacterGroupHierarchyRecords)
            {
                characterGroupHierarchyRecord.CharacterGroupId = characterGroup.Id;
                characterGroupHierarchyRecord.CharacterGroup = characterGroup;
            }
        }

        static void ModelToEntity_SetCharacterNavigationProperties(CharacterGroupModel characterGroupModel, CharacterGroup characterGroup)
        {
            foreach (Character character in characterGroup.Characters)
            {
                character.CharacterGroupId = characterGroup.Id;
                character.CharacterGroup = characterGroup;
            }
        }

        CreateMap<CharacterGroupModel, CharacterGroupIntegrationModel>().ReverseMap();

        CreateMap<CharacterGroupHierarchyRecordModel, CharacterGroupHierarchyRecordIntegrationModel>()
            .ForMember(characterGroupHierarchyRecordIntegrationModel => characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupCaption, options => options.MapFrom(characterGroupHierarchyRecordModel => characterGroupHierarchyRecordModel.NestedCharacterGroup!.Caption));

        CreateMap<CharacterGroupIntegrationModel, CharacterGroupJsonModel>();

        CreateMap<CharacterGroupHierarchyRecordIntegrationModel, CharacterGroupHierarchyRecordJsonModel>();

        CreateMap<CharacterGroupIntegrationModel, CharacterGroupXmlModel>();

        CreateMap<CharacterGroupHierarchyRecordIntegrationModel, CharacterGroupHierarchyRecordXmlModel>();

        CreateMap<CharacterGroupJsonModel, CharacterGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        CreateMap<CharacterGroupHierarchyRecordJsonModel, CharacterGroupHierarchyRecordIntegrationModel>();

        CreateMap<CharacterGroupXmlModel, CharacterGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        CreateMap<CharacterGroupHierarchyRecordXmlModel, CharacterGroupHierarchyRecordIntegrationModel>();
    }

    private static void ReplaceEmptyCommentWithNull(object characterGroupSerializedModel, CharacterGroupIntegrationModel characterGroupIntegrationModel)
    {
        characterGroupIntegrationModel.Comment = !string.IsNullOrWhiteSpace(characterGroupIntegrationModel.Comment)
            ? characterGroupIntegrationModel.Comment
            : null;

        ;
    }
}
