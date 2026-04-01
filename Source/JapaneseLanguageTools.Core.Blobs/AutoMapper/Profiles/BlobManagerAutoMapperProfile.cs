using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;

using AutoMapper;

using JapaneseLanguageTools.Contracts.Models.Blobs.Requests;
using JapaneseLanguageTools.Contracts.Models.Blobs.Responses;

namespace JapaneseLanguageTools.Core.Blobs.AutoMapper.Profiles;

public class BlobManagerAutoMapperProfile : Profile
{
    public BlobManagerAutoMapperProfile()
    {
        SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;

        CreateMap<UploadBlobRequest, UploadBlobRequestModel>().ReverseMap();
        CreateMap<UploadBlobResponse, UploadBlobResponseModel>().ReverseMap();

        CreateMap<DownloadBlobRequest, DownloadBlobRequestModel>().ReverseMap();
        CreateMap<DownloadBlobResponse, DownloadBlobResponseModel>().ReverseMap();

        CreateMap<GetBlobMetadataRequest, GetBlobMetadataRequestModel>().ReverseMap();
        CreateMap<GetBlobMetadataResponse, GetBlobMetadataResponseModel>().ReverseMap();

        CreateMap<GetBlobExpirationTimeRequest, GetBlobExpirationTimeRequestModel>().ReverseMap();
        CreateMap<GetBlobExpirationTimeResponse, GetBlobExpirationTimeResponseModel>().ReverseMap();
    }
}
