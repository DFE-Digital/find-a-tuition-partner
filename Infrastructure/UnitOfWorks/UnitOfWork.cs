using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly NtpDbContext _context;
    public UnitOfWork(NtpDbContext context)
    {
        _context = context;
        EmailLogRepository = new EmailLogRepository(_context);
        EmailStatusRepository = new EmailStatusRepository(_context);
        EnquirerNotInterestedReasonRepository = new EnquirerNotInterestedReasonRepository(_context);
        EnquiryRepository = new EnquiryRepository(_context);
        EnquiryResponseRepository = new EnquiryResponseRepository(_context);
        EnquiryResponseStatusRepository = new EnquiryResponseStatusRepository(_context);
        LocalAuthorityDistrictRepository = new LocalAuthorityDistrictRepository(_context);
        SubjectRepository = new SubjectRepository(_context);
        TuitionPartnerRepository = new TuitionPartnerRepository(_context);
        TuitionSettingRepository = new TuitionSettingRepository(_context);
        ScheduledProcessingInfoRepository = new ScheduledProcessingInfoRepository(_context);
        SchoolRepository = new SchoolRepository(_context);
        MagicLinkRepository = new MagicLinkRepository(_context);
        TuitionPartnerEnquiryRepository = new TuitionPartnerEnquiryRepository(_context);
        KeyStageSubjectEnquiryRepository = new KeyStageSubjectEnquiryRepository(_context);
    }
    public IEmailLogRepository EmailLogRepository { get; private set; }
    public IEmailStatusRepository EmailStatusRepository { get; private set; }
    public IEnquirerNotInterestedReasonRepository EnquirerNotInterestedReasonRepository { get; private set; }
    public IEnquiryRepository EnquiryRepository { get; private set; }
    public IEnquiryResponseRepository EnquiryResponseRepository { get; private set; }
    public IEnquiryResponseStatusRepository EnquiryResponseStatusRepository { get; private set; }
    public ILocalAuthorityDistrictRepository LocalAuthorityDistrictRepository { get; private set; }
    public ISubjectRepository SubjectRepository { get; private set; }
    public ITuitionPartnerRepository TuitionPartnerRepository { get; private set; }
    public ITuitionSettingRepository TuitionSettingRepository { get; private set; }
    public IScheduledProcessingInfoRepository ScheduledProcessingInfoRepository { get; private set; }
    public ISchoolRepository SchoolRepository { get; private set; }
    public IMagicLinkRepository MagicLinkRepository { get; private set; }

    public ITuitionPartnerEnquiryRepository TuitionPartnerEnquiryRepository { get; private set; }

    public IKeyStageSubjectEnquiryRepository KeyStageSubjectEnquiryRepository { get; private set; }

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void RollbackChanges()
    {
        var changedEntries = _context.ChangeTracker.Entries()
            .Where(x => x.State != EntityState.Unchanged).ToList();

        foreach (var entry in changedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    entry.Reload();
                    break;
                default: break;
            }
        }
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