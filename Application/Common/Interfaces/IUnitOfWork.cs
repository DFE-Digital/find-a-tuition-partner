using Application.Common.Interfaces.Repositories;

namespace Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEnquiryRepository EnquiryRepository { get; }
    IEnquiryResponseRepository EnquiryResponseRepository { get; }
    ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ITuitionPartnerRepository TuitionPartnerRepository { get; }
    ITuitionSettingRepository TuitionSettingRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    IMagicLinkRepository MagicLinkRepository { get; }
    ITuitionPartnerEnquiryRepository TuitionPartnerEnquiryRepository { get; }
    IKeyStageSubjectEnquiryRepository KeyStageSubjectEnquiryRepository { get; }
    Task<bool> Complete();
}