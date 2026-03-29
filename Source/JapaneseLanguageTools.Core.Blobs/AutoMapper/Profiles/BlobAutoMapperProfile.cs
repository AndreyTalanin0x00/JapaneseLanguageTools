using AndreyTalanin0x00.Integrations.Blobs;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Blobs;

namespace JapaneseLanguageTools.Core.Blobs.AutoMapper.Profiles;

public class BlobAutoMapperProfile : Profile
{
    public BlobAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<BlobReference, BlobReferenceModel>().ReverseMap();
        CreateMap<BlobMetadata, BlobMetadataModel>().ReverseMap();
    }
}
