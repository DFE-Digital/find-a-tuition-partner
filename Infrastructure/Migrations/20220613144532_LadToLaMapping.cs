using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class LadToLaMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalAuthorityId",
                table: "LocalAuthorityDistricts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LocalAuthority",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalAuthority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalAuthority_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LocalAuthority",
                columns: new[] { "Id", "Code", "Name", "RegionId" },
                values: new object[,]
                {
                    { 201, "E09000001", "City of London", 7 },
                    { 202, "E09000007", "Camden", 7 },
                    { 203, "E09000011", "Greenwich", 7 },
                    { 204, "E09000012", "Hackney", 7 },
                    { 205, "E09000013", "Hammersmith and Fulham", 7 },
                    { 206, "E09000019", "Islington", 7 },
                    { 207, "E09000020", "Kensington and Chelsea", 7 },
                    { 208, "E09000022", "Lambeth", 7 },
                    { 209, "E09000023", "Lewisham", 7 },
                    { 210, "E09000028", "Southwark", 7 },
                    { 211, "E09000030", "Tower Hamlets", 7 },
                    { 212, "E09000032", "Wandsworth", 7 },
                    { 213, "E09000033", "Westminster", 7 },
                    { 301, "E09000002", "Barking and Dagenham", 7 },
                    { 302, "E09000003", "Barnet", 7 },
                    { 303, "E09000004", "Bexley", 7 },
                    { 304, "E09000005", "Brent", 7 },
                    { 305, "E09000006", "Bromley", 7 },
                    { 306, "E09000008", "Croydon", 7 },
                    { 307, "E09000009", "Ealing", 7 },
                    { 308, "E09000010", "Enfield", 7 },
                    { 309, "E09000014", "Haringey", 7 },
                    { 310, "E09000015", "Harrow", 7 },
                    { 311, "E09000016", "Havering", 7 },
                    { 312, "E09000017", "Hillingdon", 7 },
                    { 313, "E09000018", "Hounslow", 7 },
                    { 314, "E09000021", "Kingston upon Thames", 7 },
                    { 315, "E09000024", "Merton", 7 },
                    { 316, "E09000025", "Newham", 7 },
                    { 317, "E09000026", "Redbridge", 7 },
                    { 318, "E09000027", "Richmond upon Thames", 7 },
                    { 319, "E09000029", "Sutton", 7 },
                    { 320, "E09000031", "Waltham Forest", 7 },
                    { 330, "E08000025", "Birmingham", 5 },
                    { 331, "E08000026", "Coventry", 5 },
                    { 332, "E08000027", "Dudley", 5 },
                    { 333, "E08000028", "Sandwell", 5 },
                    { 334, "E08000029", "Solihull", 5 },
                    { 335, "E08000030", "Walsall", 5 },
                    { 336, "E08000031", "Wolverhampton", 5 },
                    { 340, "E08000011", "Knowsley", 2 },
                    { 341, "E08000012", "Liverpool", 2 },
                    { 342, "E08000013", "St. Helens", 2 },
                    { 343, "E08000014", "Sefton", 2 },
                    { 344, "E08000015", "Wirral", 2 },
                    { 350, "E08000001", "Bolton", 2 },
                    { 351, "E08000002", "Bury", 2 },
                    { 352, "E08000003", "Manchester", 2 },
                    { 353, "E08000004", "Oldham", 2 },
                    { 354, "E08000005", "Rochdale", 2 },
                    { 355, "E08000006", "Salford", 2 },
                    { 356, "E08000007", "Stockport", 2 },
                    { 357, "E08000008", "Tameside", 2 },
                    { 358, "E08000009", "Trafford", 2 },
                    { 359, "E08000010", "Wigan", 2 },
                    { 370, "E08000016", "Barnsley", 3 },
                    { 371, "E08000017", "Doncaster", 3 },
                    { 372, "E08000018", "Rotherham", 3 },
                    { 373, "E08000019", "Sheffield", 3 },
                    { 380, "E08000032", "Bradford", 3 },
                    { 381, "E08000033", "Calderdale", 3 },
                    { 382, "E08000034", "Kirklees", 3 },
                    { 383, "E08000035", "Leeds", 3 },
                    { 384, "E08000036", "Wakefield", 3 },
                    { 390, "E08000020", "Gateshead", 1 },
                    { 391, "E08000021", "Newcastle Upon Tyne", 1 },
                    { 392, "E08000022", "North Tyneside", 1 },
                    { 393, "E08000023", "South Tyneside", 1 },
                    { 394, "E08000024", "Sunderland", 1 },
                    { 420, "E06000053", "Isles of Scilly", 9 },
                    { 800, "E06000022", "Bath and North East Somerset", 9 },
                    { 801, "E06000023", "Bristol, City of", 9 },
                    { 802, "E06000024", "North Somerset", 9 },
                    { 803, "E06000025", "South Gloucestershire", 9 },
                    { 805, "E06000001", "Hartlepool", 1 },
                    { 806, "E06000002", "Middlesbrough", 1 },
                    { 807, "E06000003", "Redcar and Cleveland", 1 },
                    { 808, "E06000004", "Stockton-on-Tees", 2 },
                    { 810, "E06000010", "Kingston upon Hull, City of", 3 },
                    { 811, "E06000011", "East Riding of Yorkshire", 3 },
                    { 812, "E06000012", "North East Lincolnshire", 3 },
                    { 813, "E06000013", "North Lincolnshire", 3 },
                    { 815, "E10000023", "North Yorkshire", 3 },
                    { 816, "E06000014", "York", 3 },
                    { 821, "E06000032", "Luton", 6 },
                    { 822, "E06000055", "Bedford", 6 },
                    { 823, "E06000056", "Central Bedfordshire", 6 },
                    { 825, "E10000002", "Buckinghamshire", 8 },
                    { 826, "E06000042", "Milton Keynes", 8 },
                    { 830, "E10000007", "Derbyshire", 4 },
                    { 831, "E06000015", "Derby", 4 },
                    { 835, "E10000009", "Dorset", 9 },
                    { 839, "E06000058", "Bournemouth, Christchurch & Poole", 9 },
                    { 840, "E06000047", "Durham", 1 },
                    { 841, "E06000005", "Darlington", 1 },
                    { 845, "E10000011", "East Sussex", 8 },
                    { 846, "E06000043", "Brighton and Hove", 8 },
                    { 850, "E10000014", "Hampshire", 8 },
                    { 851, "E06000044", "Portsmouth", 8 },
                    { 852, "E06000045", "Southampton", 8 },
                    { 855, "E10000018", "Leicestershire", 4 },
                    { 856, "E06000016", "Leicester", 4 },
                    { 857, "E06000017", "Rutland", 4 },
                    { 860, "E10000028", "Staffordshire", 5 },
                    { 861, "E06000021", "Stoke-on-Trent", 5 },
                    { 865, "E06000054", "Wiltshire", 9 },
                    { 866, "E06000030", "Swindon", 9 },
                    { 867, "E06000036", "Bracknell Forest", 8 },
                    { 868, "E06000040", "Windsor and Maidenhead", 8 },
                    { 869, "E06000037", "West Berkshire", 8 },
                    { 870, "E06000038", "Reading", 8 },
                    { 871, "E06000039", "Slough", 8 },
                    { 872, "E06000041", "Wokingham", 8 },
                    { 873, "E10000003", "Cambridgeshire", 6 },
                    { 874, "E06000031", "Peterborough", 6 },
                    { 876, "E06000006", "Halton", 2 },
                    { 877, "E06000007", "Warrington", 2 },
                    { 878, "E10000008", "Devon", 9 },
                    { 879, "E06000026", "Plymouth", 9 },
                    { 880, "E06000027", "Torbay", 9 },
                    { 881, "E10000012", "Essex", 6 },
                    { 882, "E06000033", "Southend-on-Sea", 6 },
                    { 883, "E06000034", "Thurrock", 6 },
                    { 884, "E06000019", "Herefordshire", 5 },
                    { 885, "E10000034", "Worcestershire", 5 },
                    { 886, "E10000016", "Kent", 8 },
                    { 887, "E06000035", "Medway", 8 },
                    { 888, "E10000017", "Lancashire", 2 },
                    { 889, "E06000008", "Blackburn with Darwen", 2 },
                    { 890, "E06000009", "Blackpool", 2 },
                    { 891, "E10000024", "Nottinghamshire", 4 },
                    { 892, "E06000018", "Nottingham", 4 },
                    { 893, "E06000051", "Shropshire", 5 },
                    { 894, "E06000020", "Telford and Wrekin", 5 },
                    { 895, "E06000049", "Cheshire East", 2 },
                    { 896, "E06000050", "Cheshire West and Chester", 2 },
                    { 908, "E06000052", "Cornwall", 9 },
                    { 909, "E10000006", "Cumbria", 2 },
                    { 916, "E10000013", "Gloucestershire", 9 },
                    { 919, "E10000015", "Hertfordshire", 6 },
                    { 921, "E06000046", "Isle of Wight", 8 },
                    { 925, "E10000019", "Lincolnshire", 4 },
                    { 926, "E10000020", "Norfolk", 6 },
                    { 929, "E06000048", "Northumberland", 1 },
                    { 931, "E10000025", "Oxfordshire", 8 },
                    { 933, "E10000027", "Somerset", 9 },
                    { 935, "E10000029", "Suffolk", 6 },
                    { 936, "E10000030", "Surrey", 8 },
                    { 937, "E10000031", "Warwickshire", 5 },
                    { 938, "E10000032", "West Sussex", 8 },
                    { 940, "E06000061", "North Northamptonshire", 4 },
                    { 941, "E06000062", "West Northamptonshire", 4 }
                });

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 1,
                column: "LocalAuthorityId",
                value: 811);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 2,
                column: "LocalAuthorityId",
                value: 812);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 3,
                column: "LocalAuthorityId",
                value: 813);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 4,
                column: "LocalAuthorityId",
                value: 816);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 5,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 6,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 7,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 8,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 9,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 10,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 11,
                column: "LocalAuthorityId",
                value: 815);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 12,
                column: "LocalAuthorityId",
                value: 370);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 13,
                column: "LocalAuthorityId",
                value: 371);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 14,
                column: "LocalAuthorityId",
                value: 372);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 15,
                column: "LocalAuthorityId",
                value: 373);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 16,
                column: "LocalAuthorityId",
                value: 380);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 17,
                column: "LocalAuthorityId",
                value: 381);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 18,
                column: "LocalAuthorityId",
                value: 382);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 19,
                column: "LocalAuthorityId",
                value: 383);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 20,
                column: "LocalAuthorityId",
                value: 384);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 21,
                column: "LocalAuthorityId",
                value: 810);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 22,
                column: "LocalAuthorityId",
                value: 884);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 23,
                column: "LocalAuthorityId",
                value: 894);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 24,
                column: "LocalAuthorityId",
                value: 861);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 25,
                column: "LocalAuthorityId",
                value: 893);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 26,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 27,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 28,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 29,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 30,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 31,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 32,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 33,
                column: "LocalAuthorityId",
                value: 860);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 34,
                column: "LocalAuthorityId",
                value: 937);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 35,
                column: "LocalAuthorityId",
                value: 937);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 36,
                column: "LocalAuthorityId",
                value: 937);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 37,
                column: "LocalAuthorityId",
                value: 937);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 38,
                column: "LocalAuthorityId",
                value: 937);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 39,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 40,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 41,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 42,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 43,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 44,
                column: "LocalAuthorityId",
                value: 885);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 45,
                column: "LocalAuthorityId",
                value: 330);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 46,
                column: "LocalAuthorityId",
                value: 331);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 47,
                column: "LocalAuthorityId",
                value: 332);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 48,
                column: "LocalAuthorityId",
                value: 333);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 49,
                column: "LocalAuthorityId",
                value: 334);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 50,
                column: "LocalAuthorityId",
                value: 335);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 51,
                column: "LocalAuthorityId",
                value: 336);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 52,
                column: "LocalAuthorityId",
                value: 800);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 53,
                column: "LocalAuthorityId",
                value: 801);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 54,
                column: "LocalAuthorityId",
                value: 802);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 55,
                column: "LocalAuthorityId",
                value: 803);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 56,
                column: "LocalAuthorityId",
                value: 879);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 57,
                column: "LocalAuthorityId",
                value: 880);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 58,
                column: "LocalAuthorityId",
                value: 866);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 59,
                column: "LocalAuthorityId",
                value: 908);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 60,
                column: "LocalAuthorityId",
                value: 420);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 61,
                column: "LocalAuthorityId",
                value: 865);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 62,
                column: "LocalAuthorityId",
                value: 839);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 63,
                column: "LocalAuthorityId",
                value: 835);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 64,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 65,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 66,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 67,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 68,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 69,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 70,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 71,
                column: "LocalAuthorityId",
                value: 878);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 72,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 73,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 74,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 75,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 76,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 77,
                column: "LocalAuthorityId",
                value: 916);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 78,
                column: "LocalAuthorityId",
                value: 933);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 79,
                column: "LocalAuthorityId",
                value: 933);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 80,
                column: "LocalAuthorityId",
                value: 933);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 81,
                column: "LocalAuthorityId",
                value: 933);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 82,
                column: "LocalAuthorityId",
                value: 931);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 83,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 84,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 85,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 86,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 87,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 88,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 89,
                column: "LocalAuthorityId",
                value: 887);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 90,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 91,
                column: "LocalAuthorityId",
                value: 867);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 92,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 93,
                column: "LocalAuthorityId",
                value: 869);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 94,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 95,
                column: "LocalAuthorityId",
                value: 870);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 96,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 97,
                column: "LocalAuthorityId",
                value: 871);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 98,
                column: "LocalAuthorityId",
                value: 936);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 99,
                column: "LocalAuthorityId",
                value: 868);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 100,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 101,
                column: "LocalAuthorityId",
                value: 872);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 102,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 103,
                column: "LocalAuthorityId",
                value: 826);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 104,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 105,
                column: "LocalAuthorityId",
                value: 846);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 106,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 107,
                column: "LocalAuthorityId",
                value: 851);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 108,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 109,
                column: "LocalAuthorityId",
                value: 852);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 110,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 111,
                column: "LocalAuthorityId",
                value: 921);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 112,
                column: "LocalAuthorityId",
                value: 938);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 113,
                column: "LocalAuthorityId",
                value: 825);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 114,
                column: "LocalAuthorityId",
                value: 845);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 115,
                column: "LocalAuthorityId",
                value: 845);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 116,
                column: "LocalAuthorityId",
                value: 845);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 117,
                column: "LocalAuthorityId",
                value: 845);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 118,
                column: "LocalAuthorityId",
                value: 845);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 119,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 120,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 121,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 122,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 123,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 124,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 125,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 126,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 127,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 128,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 129,
                column: "LocalAuthorityId",
                value: 850);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 130,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 131,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 132,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 133,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 134,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 135,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 136,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 137,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 138,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 139,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 140,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 141,
                column: "LocalAuthorityId",
                value: 886);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 142,
                column: "LocalAuthorityId",
                value: 931);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 143,
                column: "LocalAuthorityId",
                value: 931);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 144,
                column: "LocalAuthorityId",
                value: 931);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 145,
                column: "LocalAuthorityId",
                value: 931);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 146,
                column: "LocalAuthorityId",
                value: 876);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 147,
                column: "LocalAuthorityId",
                value: 877);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 148,
                column: "LocalAuthorityId",
                value: 889);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 149,
                column: "LocalAuthorityId",
                value: 890);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 150,
                column: "LocalAuthorityId",
                value: 895);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 151,
                column: "LocalAuthorityId",
                value: 896);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 152,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 153,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 154,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 155,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 156,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 157,
                column: "LocalAuthorityId",
                value: 909);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 158,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 159,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 160,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 161,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 162,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 163,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 164,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 165,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 166,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 167,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 168,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 169,
                column: "LocalAuthorityId",
                value: 888);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 170,
                column: "LocalAuthorityId",
                value: 350);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 171,
                column: "LocalAuthorityId",
                value: 351);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 172,
                column: "LocalAuthorityId",
                value: 352);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 173,
                column: "LocalAuthorityId",
                value: 353);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 174,
                column: "LocalAuthorityId",
                value: 354);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 175,
                column: "LocalAuthorityId",
                value: 355);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 176,
                column: "LocalAuthorityId",
                value: 356);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 177,
                column: "LocalAuthorityId",
                value: 357);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 178,
                column: "LocalAuthorityId",
                value: 358);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 179,
                column: "LocalAuthorityId",
                value: 359);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 180,
                column: "LocalAuthorityId",
                value: 340);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 181,
                column: "LocalAuthorityId",
                value: 341);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 182,
                column: "LocalAuthorityId",
                value: 342);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 183,
                column: "LocalAuthorityId",
                value: 343);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 184,
                column: "LocalAuthorityId",
                value: 344);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 185,
                column: "LocalAuthorityId",
                value: 805);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 186,
                column: "LocalAuthorityId",
                value: 806);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 187,
                column: "LocalAuthorityId",
                value: 807);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 188,
                column: "LocalAuthorityId",
                value: 808);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 189,
                column: "LocalAuthorityId",
                value: 841);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 190,
                column: "LocalAuthorityId",
                value: 840);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 191,
                column: "LocalAuthorityId",
                value: 929);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 192,
                column: "LocalAuthorityId",
                value: 391);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 193,
                column: "LocalAuthorityId",
                value: 392);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 194,
                column: "LocalAuthorityId",
                value: 393);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 195,
                column: "LocalAuthorityId",
                value: 394);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 196,
                column: "LocalAuthorityId",
                value: 390);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 197,
                column: "LocalAuthorityId",
                value: 318);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 198,
                column: "LocalAuthorityId",
                value: 210);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 199,
                column: "LocalAuthorityId",
                value: 319);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 200,
                column: "LocalAuthorityId",
                value: 211);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 201,
                column: "LocalAuthorityId",
                value: 320);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 202,
                column: "LocalAuthorityId",
                value: 212);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 203,
                column: "LocalAuthorityId",
                value: 213);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 204,
                column: "LocalAuthorityId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 205,
                column: "LocalAuthorityId",
                value: 301);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 206,
                column: "LocalAuthorityId",
                value: 302);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 207,
                column: "LocalAuthorityId",
                value: 303);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 208,
                column: "LocalAuthorityId",
                value: 304);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 209,
                column: "LocalAuthorityId",
                value: 305);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 210,
                column: "LocalAuthorityId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 211,
                column: "LocalAuthorityId",
                value: 306);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 212,
                column: "LocalAuthorityId",
                value: 307);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 213,
                column: "LocalAuthorityId",
                value: 308);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 214,
                column: "LocalAuthorityId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 215,
                column: "LocalAuthorityId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 216,
                column: "LocalAuthorityId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 217,
                column: "LocalAuthorityId",
                value: 309);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 218,
                column: "LocalAuthorityId",
                value: 310);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 219,
                column: "LocalAuthorityId",
                value: 311);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 220,
                column: "LocalAuthorityId",
                value: 312);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 221,
                column: "LocalAuthorityId",
                value: 313);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 222,
                column: "LocalAuthorityId",
                value: 206);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 223,
                column: "LocalAuthorityId",
                value: 207);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 224,
                column: "LocalAuthorityId",
                value: 314);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 225,
                column: "LocalAuthorityId",
                value: 208);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 226,
                column: "LocalAuthorityId",
                value: 209);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 227,
                column: "LocalAuthorityId",
                value: 315);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 228,
                column: "LocalAuthorityId",
                value: 316);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 229,
                column: "LocalAuthorityId",
                value: 317);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 230,
                column: "LocalAuthorityId",
                value: 874);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 231,
                column: "LocalAuthorityId",
                value: 821);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 232,
                column: "LocalAuthorityId",
                value: 882);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 233,
                column: "LocalAuthorityId",
                value: 883);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 234,
                column: "LocalAuthorityId",
                value: 822);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 235,
                column: "LocalAuthorityId",
                value: 823);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 236,
                column: "LocalAuthorityId",
                value: 873);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 237,
                column: "LocalAuthorityId",
                value: 873);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 238,
                column: "LocalAuthorityId",
                value: 873);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 239,
                column: "LocalAuthorityId",
                value: 873);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 240,
                column: "LocalAuthorityId",
                value: 873);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 241,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 242,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 243,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 244,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 245,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 246,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 247,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 248,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 249,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 250,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 251,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 252,
                column: "LocalAuthorityId",
                value: 881);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 253,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 254,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 255,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 256,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 257,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 258,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 259,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 260,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 261,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 262,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 263,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 264,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 265,
                column: "LocalAuthorityId",
                value: 926);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 266,
                column: "LocalAuthorityId",
                value: 935);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 267,
                column: "LocalAuthorityId",
                value: 935);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 268,
                column: "LocalAuthorityId",
                value: 935);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 269,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 270,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 271,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 272,
                column: "LocalAuthorityId",
                value: 919);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 273,
                column: "LocalAuthorityId",
                value: 935);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 274,
                column: "LocalAuthorityId",
                value: 935);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 275,
                column: "LocalAuthorityId",
                value: 831);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 276,
                column: "LocalAuthorityId",
                value: 856);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 277,
                column: "LocalAuthorityId",
                value: 857);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 278,
                column: "LocalAuthorityId",
                value: 892);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 279,
                column: "LocalAuthorityId",
                value: 940);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 280,
                column: "LocalAuthorityId",
                value: 941);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 281,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 282,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 283,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 284,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 285,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 286,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 287,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 288,
                column: "LocalAuthorityId",
                value: 830);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 289,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 290,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 291,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 292,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 293,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 294,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 295,
                column: "LocalAuthorityId",
                value: 855);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 296,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 297,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 298,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 299,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 300,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 301,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 302,
                column: "LocalAuthorityId",
                value: 925);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 303,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 304,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 305,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 306,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 307,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 308,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.UpdateData(
                table: "LocalAuthorityDistricts",
                keyColumn: "Id",
                keyValue: 309,
                column: "LocalAuthorityId",
                value: 891);

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_LocalAuthorityId",
                table: "LocalAuthorityDistricts",
                column: "LocalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_Code",
                table: "LocalAuthority",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_RegionId",
                table: "LocalAuthority",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAuthorityDistricts_LocalAuthority_LocalAuthorityId",
                table: "LocalAuthorityDistricts",
                column: "LocalAuthorityId",
                principalTable: "LocalAuthority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAuthorityDistricts_LocalAuthority_LocalAuthorityId",
                table: "LocalAuthorityDistricts");

            migrationBuilder.DropTable(
                name: "LocalAuthority");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistricts_LocalAuthorityId",
                table: "LocalAuthorityDistricts");

            migrationBuilder.DropColumn(
                name: "LocalAuthorityId",
                table: "LocalAuthorityDistricts");
        }
    }
}
