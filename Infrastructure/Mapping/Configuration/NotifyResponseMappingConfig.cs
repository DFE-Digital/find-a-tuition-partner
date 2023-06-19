using Application.Common.DTO;
using Domain;
using Mapster;

namespace Infrastructure.Mapping.Configuration
{
    public static class NotifyResponseMappingConfig
    {
        public static void Configure()
        {
            ConfigureNotifyResponse();
        }

        private static void ConfigureNotifyResponse()
        {
            TypeAdapterConfig<NotifyResponseDto, EmailNotifyResponseLog>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.EmailLogId)
                .Ignore(dest => dest.EmailLog);
        }
    }
}
