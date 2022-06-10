namespace Infrastructure.Entities;

public class TuitionPartnerDataImportHistory
{
    public int Id { get; set; }
    public string Importer { get; set; } = null!;
    public string Md5Checksum { get; set; } = null!;
    public DateTime ImportDateTime { get; set; }
}