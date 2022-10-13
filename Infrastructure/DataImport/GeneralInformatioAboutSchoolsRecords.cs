using System.Globalization;
using Application.DataImport;
using Application.Mapping;
using CsvHelper;
using CsvHelper.Configuration;

namespace Infrastructure.DataImport
{
    internal class GeneralInformatioAboutSchoolsRecords : IGeneralInformationAboutSchoolsRecords
    {
        public async Task<IReadOnlyCollection<SchoolDatum>> GetSchoolDataAsync(CancellationToken cancellationToken)
        {
            var dateFilename = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            HttpClient _httpClient = new HttpClient();
            await using var response = await _httpClient.GetStreamAsync($"https://ea-edubase-api-prod.azurewebsites.net/edubase/downloads/public/edubasealldata{dateFilename}.csv", cancellationToken);
            using var reader = new StreamReader(response);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<SchoolDatumMap>();
            var records = csv.GetRecords<SchoolDatum>().ToArray();

            return records;
        }

        private class SchoolDatumMap : ClassMap<SchoolDatum>
        {
            public SchoolDatumMap()
            {
                Map(m => m.Urn).Name("URN");
                Map(m => m.Name).Name("EstablishmentName");
                Map(m => m.street).Name("Street");
                Map(m => m.Locality).Name("Locality");
                Map(m => m.Address3).Name("Address3");
                Map(m => m.Town).Name("Town");
                Map(m => m.County).Name("County (name)");
                Map(m => m.Postcode).Name("Postcode");
                Map(m => m.EstablishmentType).Name("TypeOfEstablishment (code)");
                Map(m => m.EstablishmentTypeGroup).Name("EstablishmentTypeGroup (code)");
                Map(m => m.EstablishmentStatus).Name("EstablishmentStatus (code)");
                Map(m => m.PhaseOfEducation).Name("PhaseOfEducation (code)");
                Map(m => m.LocalAuthorityCode).Name("LA (code)");
                Map(m => m.LocalAuthorityDistrictCode).Name("DistrictAdministrative (code)");
            }
        }
    }
}