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

        CreateMap<CharacterGroupModel, CharacterGroup>()
            .AfterMap(ModelToEntity_SetCharacterNavigationProperties);

        static void ModelToEntity_SetCharacterNavigationProperties(CharacterGroupModel characterGroupModel, CharacterGroup characterGroup)
        {
            foreach (Character character in characterGroup.Characters)
            {
                character.CharacterGroupId = characterGroup.Id;
                character.CharacterGroup = characterGroup;
            }
        }

        CreateMap<CharacterGroupModel, CharacterGroupIntegrationModel>().ReverseMap();

        CreateMap<CharacterGroupIntegrationModel, CharacterGroupJsonModel>();
        CreateMap<CharacterGroupIntegrationModel, CharacterGroupXmlModel>();

        CreateMap<CharacterGroupJsonModel, CharacterGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);
        CreateMap<CharacterGroupXmlModel, CharacterGroupIntegrationModel>()
            .AfterMap(ReplaceEmptyCommentWithNull);

        ;
    }

    private static void ReplaceEmptyCommentWithNull(object characterGroupSerializedModel, CharacterGroupIntegrationModel characterGroupIntegrationModel)
    {
        characterGroupIntegrationModel.Comment = !string.IsNullOrWhiteSpace(characterGroupIntegrationModel.Comment)
            ? characterGroupIntegrationModel.Comment
            : null;

        ;
    }
}
