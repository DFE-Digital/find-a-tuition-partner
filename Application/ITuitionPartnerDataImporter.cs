namespace Application;

public interface ITuitionPartnerDataImporter
{
    void Import();
    Task ImportAsync();
}