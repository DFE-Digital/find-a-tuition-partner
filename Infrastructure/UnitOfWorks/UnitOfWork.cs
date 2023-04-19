using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly NtpDbContext _context;
    public UnitOfWork(NtpDbContext context)
    {
        _context = context;
        EmailLogRepository = new EmailLogRepository(_context);
        EnquiryRepository = new EnquiryRepository(_context);
        EnquiryResponseRepository = new EnquiryResponseRepository(_context);
        LocalAuthorityDistrictRepository = new LocalAuthorityDistrictRepository(_context);
        SubjectRepository = new SubjectRepository(_context);
        TuitionPartnerRepository = new TuitionPartnerRepository(_context);
        TuitionTypeRepository = new TuitionTypeRepository(_context);
        SchoolRepository = new SchoolRepository(_context);
        MagicLinkRepository = new MagicLinkRepository(_context);
        TuitionPartnerEnquiryRepository = new TuitionPartnerEnquiryRepository(_context);
        KeyStageSubjectEnquiryRepository = new KeyStageSubjectEnquiryRepository(_context);
    }
    public IEmailLogRepository EmailLogRepository { get; private set; }
    public IEnquiryRepository EnquiryRepository { get; private set; }
    public IEnquiryResponseRepository EnquiryResponseRepository { get; private set; }
    public ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; private set; }
    public ISubjectRepository SubjectRepository { get; private set; }
    public ITuitionPartnerRepository TuitionPartnerRepository { get; private set; }
    public ITuitionTypeRepository TuitionTypeRepository { get; private set; }
    public ISchoolRepository SchoolRepository { get; private set; }
    public IMagicLinkRepository MagicLinkRepository { get; private set; }

    public ITuitionPartnerEnquiryRepository TuitionPartnerEnquiryRepository { get; private set; }

    public IKeyStageSubjectEnquiryRepository KeyStageSubjectEnquiryRepository { get; private set; }

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _context.Dispose();
    }
}