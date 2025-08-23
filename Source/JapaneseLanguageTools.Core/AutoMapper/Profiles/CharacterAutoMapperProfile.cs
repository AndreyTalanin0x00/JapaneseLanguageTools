using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class CharacterAutoMapperProfile : Profile
{
    public CharacterAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Character, CharacterModel>();

        CreateMap<CharacterModel, Character>()
            .AfterMap(ModelToEntity_SetCharacterTagNavigationProperties);

        CreateMap<IEnumerable<CharacterTag>, IEnumerable<TagModel>>().ConstructUsing((characterTags, context) =>
        {
            return characterTags
                .Select(characterTag => characterTag.Tag)
                .Select(tag => context.Mapper.Map<TagModel>(tag))
                .ToList();
        });

        CreateMap<IEnumerable<TagModel>, IEnumerable<CharacterTag>>().ConstructUsing((tagModels, context) =>
        {
            return tagModels
                .Select(tagModel => context.Mapper.Map<Tag>(tagModel))
                .Select(tag => new CharacterTag() { TagId = tag.Id, Tag = tag })
                .ToList();
        });

        CreateMap<CharacterModel, CharacterIntegrationModel>().ReverseMap();

        CreateMap<CharacterIntegrationModel, CharacterJsonModel>().ReverseMap();
        CreateMap<CharacterIntegrationModel, CharacterXmlModel>().ReverseMap();

        static void ModelToEntity_SetCharacterTagNavigationProperties(CharacterModel characterModel, Character character)
        {
            foreach (CharacterTag characterTag in character.CharacterTags)
            {
                characterTag.CharacterId = character.Id;
                characterTag.Character = character;
            }
        }
    }
}
