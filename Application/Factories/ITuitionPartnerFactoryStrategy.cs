using Application.Extraction;
using Domain;

namespace Application.Factories;

public interface ITuitionPartnerFactoryStrategy
{
    Task<TuitionPartner> GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, CancellationToken cancellationToken);
}