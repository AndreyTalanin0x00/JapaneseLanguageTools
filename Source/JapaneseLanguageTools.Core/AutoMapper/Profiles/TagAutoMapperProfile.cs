using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Data.Entities;

namespace JapaneseLanguageTools.Core.AutoMapper.Profiles;

public class TagAutoMapperProfile : Profile
{
    public TagAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<Tag, TagModel>().ReverseMap();

        CreateMap<IEnumerable<TagModel>, string>().ConstructUsing((tags, context) =>
        {
            return string.Join("; ", tags.Select(tag => tag.Caption));
        });

        CreateMap<TagModel, TagIntegrationModel>().ReverseMap();

        CreateMap<TagIntegrationModel, TagJsonModel>();
        CreateMap<TagIntegrationModel, TagXmlModel>();

        CreateMap<TagJsonModel, TagIntegrationModel>()
            .AfterMap(ReplaceEmptyPlaceholderMarkerWithNull);
        CreateMap<TagXmlModel, TagIntegrationModel>()
            .AfterMap(ReplaceEmptyPlaceholderMarkerWithNull);

        CreateMap<TagObjectPackageIntegrationModel, TagObjectPackageJsonModel>().ReverseMap();
        CreateMap<TagObjectPackageIntegrationModel, TagObjectPackageXmlModel>().ReverseMap();
    }

    private static void ReplaceEmptyPlaceholderMarkerWithNull(object tagSerializedModel, TagIntegrationModel tagIntegrationModel)
    {
        tagIntegrationModel.PlaceholderMarker = !string.IsNullOrWhiteSpace(tagIntegrationModel.PlaceholderMarker)
            ? tagIntegrationModel.PlaceholderMarker
            : null;

        ;
    }
}
