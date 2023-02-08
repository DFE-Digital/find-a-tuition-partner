using Application.Common.Interfaces.Repositories;

namespace Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEnquiryRepository EnquiryRepository { get; }

    ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ITuitionPartnerRepository TuitionPartnerRepository { get; }
    ITuitionTypeRepository TuitionTypeRepository { get; }

    ISchoolRepository SchoolRepository { get; }
    Task<bool> Complete();
}