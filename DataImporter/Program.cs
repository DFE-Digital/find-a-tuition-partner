using Infrastructure.Importers;

if (Directory.Exists(@"C:\Farsight"))
{
    // Get only xlsx files from directory.
    string[] dirs = Directory.GetFiles(@"C:\Farsight", "*.xlsx");

    foreach (string fileName in dirs)
    {
        NtpTutionPartnerExcelImporter.Import(fileName);
    }
}
