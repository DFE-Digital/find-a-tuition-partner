using Application.Extraction;
using Domain;

namespace Application.Factories;

public interface IQualityAssuredSpreadsheetTuitionPartnerFactory
{
    Task<TuitionPartner> GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, CancellationToken cancellationToken);
}