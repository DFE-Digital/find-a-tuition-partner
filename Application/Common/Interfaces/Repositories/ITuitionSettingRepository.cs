using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionSettingRepository : IGenericRepository<TuitionSetting>
{
    Task<IEnumerable<TuitionSetting>> GetTuitionSettingsAsync(CancellationToken cancellationToken = default);
}