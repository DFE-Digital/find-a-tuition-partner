using Application.Common.Interfaces.Repositories;

namespace Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmailLogRepository EmailLogRepository { get; }
    IEmailStatusRepository EmailStatusRepository { get; }
    IEnquiryRepository EnquiryRepository { get; }
    IEnquiryResponseRepository EnquiryResponseRepository { get; }
    IEnquiryResponseStatusRepository EnquiryResponseStatusRepository { get; }
    ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ITuitionPartnerRepository TuitionPartnerRepository { get; }
    ITuitionSettingRepository TuitionSettingRepository { get; }
    IScheduledProcessingInfoRepository ScheduledProcessingInfoRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    IMagicLinkRepository MagicLinkRepository { get; }
    ITuitionPartnerEnquiryRepository TuitionPartnerEnquiryRepository { get; }
    IKeyStageSubjectEnquiryRepository KeyStageSubjectEnquiryRepository { get; }
    Task<bool> Complete();
    void RollbackChanges();
}