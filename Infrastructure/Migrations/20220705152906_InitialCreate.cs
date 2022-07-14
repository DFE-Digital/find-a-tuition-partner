using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyStage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyStage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Experience = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    LegalStatus = table.Column<string>(type: "text", nullable: false),
                    HasSenProvision = table.Column<bool>(type: "boolean", nullable: false),
                    AdditionalServiceOfferings = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuitionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeoUrl = table.Column<string>(type: "text", nullable: false),
                    KeyStageId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_KeyStage_KeyStageId",
                        column: x => x.KeyStageId,
                        principalTable: "KeyStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    GroupSize = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalAuthorityDistricts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalAuthorityDistricts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistricts_LocalAuthority_LocalAuthorityId",
                        column: x => x.LocalAuthorityId,
                        principalTable: "LocalAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistricts_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalAuthorityDistrictCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalAuthorityDistrictCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_LocalAuthorityDistricts_Loca~",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_TuitionPartners_TuitionPartn~",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KeyStage",
                columns: new[] { "Id", "Name", "SeoUrl" },
                values: new object[,]
                {
                    { 1, "Key stage 1", "key-stage-1" },
                    { 2, "Key stage 2", "key-stage-2" },
                    { 3, "Key stage 3", "key-stage-3" },
                    { 4, "Key stage 4", "key-stage-4" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "E12000001", "North East" },
                    { 2, "E12000002", "North West" },
                    { 3, "E12000003", "Yorkshire and The Humber" },
                    { 4, "E12000004", "East Midlands" },
                    { 5, "E12000005", "West Midlands" },
                    { 6, "E12000006", "East of England" },
                    { 7, "E12000007", "London" },
                    { 8, "E12000008", "South East" },
                    { 9, "E12000009", "South West" }
                });

            migrationBuilder.InsertData(
                table: "TuitionTypes",
                columns: new[] { "Id", "Name", "SeoUrl" },
                values: new object[,]
                {
                    { 1, "Online", "online" },
                    { 2, "In School", "in-school" }
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

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "KeyStageId", "Name", "SeoUrl" },
                values: new object[,]
                {
                    { 1, 1, "Literacy", "key-stage-1-literacy" },
                    { 2, 1, "Numeracy", "key-stage-1-numeracy" },
                    { 3, 1, "Science", "key-stage-1-science" },
                    { 4, 2, "Literacy", "key-stage-2-literacy" },
                    { 5, 2, "Numeracy", "key-stage-2-numeracy" },
                    { 6, 2, "Literacy", "key-stage-2-science" },
                    { 7, 3, "English", "key-stage-3-english" },
                    { 8, 3, "Humanities", "key-stage-3-humanities" },
                    { 9, 3, "Maths", "key-stage-3-maths" },
                    { 10, 3, "Modern Foreign Languages", "key-stage-3-modern-foreign-languages" },
                    { 11, 3, "Science", "key-stage-3-science" },
                    { 12, 4, "English", "key-stage-4-english" },
                    { 13, 4, "Humanities", "key-stage-4-humanities" },
                    { 14, 4, "Maths", "key-stage-4-maths" },
                    { 15, 4, "Modern Foreign Languages", "key-stage-4-modern-foreign-languages" },
                    { 16, 4, "Science", "key-stage-4-science" }
                });

            migrationBuilder.InsertData(
                table: "LocalAuthorityDistricts",
                columns: new[] { "Id", "Code", "LocalAuthorityId", "Name", "RegionId" },
                values: new object[,]
                {
                    { 1, "E06000011", 811, "East Riding of Yorkshire", 3 },
                    { 2, "E06000012", 812, "North East Lincolnshire", 3 },
                    { 3, "E06000013", 813, "North Lincolnshire", 3 },
                    { 4, "E06000014", 816, "York", 3 },
                    { 5, "E07000163", 815, "Craven", 3 },
                    { 6, "E07000164", 815, "Hambleton", 3 },
                    { 7, "E07000165", 815, "Harrogate", 3 },
                    { 8, "E07000166", 815, "Richmondshire", 3 },
                    { 9, "E07000167", 815, "Ryedale", 3 },
                    { 10, "E07000168", 815, "Scarborough", 3 },
                    { 11, "E07000169", 815, "Selby", 3 },
                    { 12, "E08000016", 370, "Barnsley", 3 },
                    { 13, "E08000017", 371, "Doncaster", 3 },
                    { 14, "E08000018", 372, "Rotherham", 3 },
                    { 15, "E08000019", 373, "Sheffield", 3 },
                    { 16, "E08000032", 380, "Bradford", 3 },
                    { 17, "E08000033", 381, "Calderdale", 3 },
                    { 18, "E08000034", 382, "Kirklees", 3 },
                    { 19, "E08000035", 383, "Leeds", 3 },
                    { 20, "E08000036", 384, "Wakefield", 3 },
                    { 21, "E06000010", 810, "Kingston upon Hull, City of", 3 },
                    { 22, "E06000019", 884, "Herefordshire, County of", 5 },
                    { 23, "E06000020", 894, "Telford and Wrekin", 5 },
                    { 24, "E06000021", 861, "Stoke-on-Trent", 5 },
                    { 25, "E06000051", 893, "Shropshire", 5 },
                    { 26, "E07000192", 860, "Cannock Chase", 5 },
                    { 27, "E07000193", 860, "East Staffordshire", 5 },
                    { 28, "E07000194", 860, "Lichfield", 5 },
                    { 29, "E07000195", 860, "Newcastle-under-Lyme", 5 },
                    { 30, "E07000196", 860, "South Staffordshire", 5 },
                    { 31, "E07000197", 860, "Stafford", 5 },
                    { 32, "E07000198", 860, "Staffordshire Moorlands", 5 },
                    { 33, "E07000199", 860, "Tamworth", 5 },
                    { 34, "E07000218", 937, "North Warwickshire", 5 },
                    { 35, "E07000219", 937, "Nuneaton and Bedworth", 5 },
                    { 36, "E07000220", 937, "Rugby", 5 },
                    { 37, "E07000221", 937, "Stratford-on-Avon", 5 },
                    { 38, "E07000222", 937, "Warwick", 5 },
                    { 39, "E07000234", 885, "Bromsgrove", 5 },
                    { 40, "E07000235", 885, "Malvern Hills", 5 },
                    { 41, "E07000236", 885, "Redditch", 5 },
                    { 42, "E07000237", 885, "Worcester", 5 },
                    { 43, "E07000238", 885, "Wychavon", 5 },
                    { 44, "E07000239", 885, "Wyre Forest", 5 },
                    { 45, "E08000025", 330, "Birmingham", 5 },
                    { 46, "E08000026", 331, "Coventry", 5 },
                    { 47, "E08000027", 332, "Dudley", 5 },
                    { 48, "E08000028", 333, "Sandwell", 5 },
                    { 49, "E08000029", 334, "Solihull", 5 },
                    { 50, "E08000030", 335, "Walsall", 5 },
                    { 51, "E08000031", 336, "Wolverhampton", 5 },
                    { 52, "E06000022", 800, "Bath and North East Somerset", 9 },
                    { 53, "E06000023", 801, "Bristol, City of", 9 },
                    { 54, "E06000024", 802, "North Somerset", 9 },
                    { 55, "E06000025", 803, "South Gloucestershire", 9 },
                    { 56, "E06000026", 879, "Plymouth", 9 },
                    { 57, "E06000027", 880, "Torbay", 9 },
                    { 58, "E06000030", 866, "Swindon", 9 },
                    { 59, "E06000052", 908, "Cornwall", 9 },
                    { 60, "E06000053", 420, "Isles of Scilly", 9 },
                    { 61, "E06000054", 865, "Wiltshire", 9 },
                    { 62, "E06000058", 839, "Bournemouth, Christchurch and Poole", 9 },
                    { 63, "E06000059", 835, "Dorset", 9 },
                    { 64, "E07000040", 878, "East Devon", 9 },
                    { 65, "E07000041", 878, "Exeter", 9 },
                    { 66, "E07000042", 878, "Mid Devon", 9 },
                    { 67, "E07000043", 878, "North Devon", 9 },
                    { 68, "E07000044", 878, "South Hams", 9 },
                    { 69, "E07000045", 878, "Teignbridge", 9 },
                    { 70, "E07000046", 878, "Torridge", 9 },
                    { 71, "E07000047", 878, "West Devon", 9 },
                    { 72, "E07000078", 916, "Cheltenham", 9 },
                    { 73, "E07000079", 916, "Cotswold", 9 },
                    { 74, "E07000080", 916, "Forest of Dean", 9 },
                    { 75, "E07000081", 916, "Gloucester", 9 },
                    { 76, "E07000082", 916, "Stroud", 9 },
                    { 77, "E07000083", 916, "Tewkesbury", 9 },
                    { 78, "E07000187", 933, "Mendip", 9 },
                    { 79, "E07000188", 933, "Sedgemoor", 9 },
                    { 80, "E07000189", 933, "South Somerset", 9 },
                    { 81, "E07000246", 933, "Somerset West and Taunton", 9 },
                    { 82, "E07000181", 931, "West Oxfordshire", 8 },
                    { 83, "E07000207", 936, "Elmbridge", 8 },
                    { 84, "E07000208", 936, "Epsom and Ewell", 8 },
                    { 85, "E07000209", 936, "Guildford", 8 },
                    { 86, "E07000210", 936, "Mole Valley", 8 },
                    { 87, "E07000211", 936, "Reigate and Banstead", 8 },
                    { 88, "E07000212", 936, "Runnymede", 8 },
                    { 89, "E06000035", 887, "Medway", 8 },
                    { 90, "E07000213", 936, "Spelthorne", 8 },
                    { 91, "E06000036", 867, "Bracknell Forest", 8 },
                    { 92, "E07000214", 936, "Surrey Heath", 8 },
                    { 93, "E06000037", 869, "West Berkshire", 8 },
                    { 94, "E07000215", 936, "Tandridge", 8 },
                    { 95, "E06000038", 870, "Reading", 8 },
                    { 96, "E07000216", 936, "Waverley", 8 },
                    { 97, "E06000039", 871, "Slough", 8 },
                    { 98, "E07000217", 936, "Woking", 8 },
                    { 99, "E06000040", 868, "Windsor and Maidenhead", 8 },
                    { 100, "E07000223", 938, "Adur", 8 },
                    { 101, "E06000041", 872, "Wokingham", 8 },
                    { 102, "E07000224", 938, "Arun", 8 },
                    { 103, "E06000042", 826, "Milton Keynes", 8 },
                    { 104, "E07000225", 938, "Chichester", 8 },
                    { 105, "E06000043", 846, "Brighton and Hove", 8 },
                    { 106, "E07000226", 938, "Crawley", 8 },
                    { 107, "E06000044", 851, "Portsmouth", 8 },
                    { 108, "E07000227", 938, "Horsham", 8 },
                    { 109, "E06000045", 852, "Southampton", 8 },
                    { 110, "E07000228", 938, "Mid Sussex", 8 },
                    { 111, "E06000046", 921, "Isle of Wight", 8 },
                    { 112, "E07000229", 938, "Worthing", 8 },
                    { 113, "E06000060", 825, "Buckinghamshire", 8 },
                    { 114, "E07000061", 845, "Eastbourne", 8 },
                    { 115, "E07000062", 845, "Hastings", 8 },
                    { 116, "E07000063", 845, "Lewes", 8 },
                    { 117, "E07000064", 845, "Rother", 8 },
                    { 118, "E07000065", 845, "Wealden", 8 },
                    { 119, "E07000084", 850, "Basingstoke and Deane", 8 },
                    { 120, "E07000085", 850, "East Hampshire", 8 },
                    { 121, "E07000086", 850, "Eastleigh", 8 },
                    { 122, "E07000087", 850, "Fareham", 8 },
                    { 123, "E07000088", 850, "Gosport", 8 },
                    { 124, "E07000089", 850, "Hart", 8 },
                    { 125, "E07000090", 850, "Havant", 8 },
                    { 126, "E07000091", 850, "New Forest", 8 },
                    { 127, "E07000092", 850, "Rushmoor", 8 },
                    { 128, "E07000093", 850, "Test Valley", 8 },
                    { 129, "E07000094", 850, "Winchester", 8 },
                    { 130, "E07000105", 886, "Ashford", 8 },
                    { 131, "E07000106", 886, "Canterbury", 8 },
                    { 132, "E07000107", 886, "Dartford", 8 },
                    { 133, "E07000108", 886, "Dover", 8 },
                    { 134, "E07000109", 886, "Gravesham", 8 },
                    { 135, "E07000110", 886, "Maidstone", 8 },
                    { 136, "E07000111", 886, "Sevenoaks", 8 },
                    { 137, "E07000112", 886, "Folkestone and Hythe", 8 },
                    { 138, "E07000113", 886, "Swale", 8 },
                    { 139, "E07000114", 886, "Thanet", 8 },
                    { 140, "E07000115", 886, "Tonbridge and Malling", 8 },
                    { 141, "E07000116", 886, "Tunbridge Wells", 8 },
                    { 142, "E07000177", 931, "Cherwell", 8 },
                    { 143, "E07000178", 931, "Oxford", 8 },
                    { 144, "E07000179", 931, "South Oxfordshire", 8 },
                    { 145, "E07000180", 931, "Vale of White Horse", 8 },
                    { 146, "E06000006", 876, "Halton", 2 },
                    { 147, "E06000007", 877, "Warrington", 2 },
                    { 148, "E06000008", 889, "Blackburn with Darwen", 2 },
                    { 149, "E06000009", 890, "Blackpool", 2 },
                    { 150, "E06000049", 895, "Cheshire East", 2 },
                    { 151, "E06000050", 896, "Cheshire West and Chester", 2 },
                    { 152, "E07000026", 909, "Allerdale", 2 },
                    { 153, "E07000027", 909, "Barrow-in-Furness", 2 },
                    { 154, "E07000028", 909, "Carlisle", 2 },
                    { 155, "E07000029", 909, "Copeland", 2 },
                    { 156, "E07000030", 909, "Eden", 2 },
                    { 157, "E07000031", 909, "South Lakeland", 2 },
                    { 158, "E07000117", 888, "Burnley", 2 },
                    { 159, "E07000118", 888, "Chorley", 2 },
                    { 160, "E07000119", 888, "Fylde", 2 },
                    { 161, "E07000120", 888, "Hyndburn", 2 },
                    { 162, "E07000121", 888, "Lancaster", 2 },
                    { 163, "E07000122", 888, "Pendle", 2 },
                    { 164, "E07000123", 888, "Preston", 2 },
                    { 165, "E07000124", 888, "Ribble Valley", 2 },
                    { 166, "E07000125", 888, "Rossendale", 2 },
                    { 167, "E07000126", 888, "South Ribble", 2 },
                    { 168, "E07000127", 888, "West Lancashire", 2 },
                    { 169, "E07000128", 888, "Wyre", 2 },
                    { 170, "E08000001", 350, "Bolton", 2 },
                    { 171, "E08000002", 351, "Bury", 2 },
                    { 172, "E08000003", 352, "Manchester", 2 },
                    { 173, "E08000004", 353, "Oldham", 2 },
                    { 174, "E08000005", 354, "Rochdale", 2 },
                    { 175, "E08000006", 355, "Salford", 2 },
                    { 176, "E08000007", 356, "Stockport", 2 },
                    { 177, "E08000008", 357, "Tameside", 2 },
                    { 178, "E08000009", 358, "Trafford", 2 },
                    { 179, "E08000010", 359, "Wigan", 2 },
                    { 180, "E08000011", 340, "Knowsley", 2 },
                    { 181, "E08000012", 341, "Liverpool", 2 },
                    { 182, "E08000013", 342, "St. Helens", 2 },
                    { 183, "E08000014", 343, "Sefton", 2 },
                    { 184, "E08000015", 344, "Wirral", 2 },
                    { 185, "E06000001", 805, "Hartlepool", 1 },
                    { 186, "E06000002", 806, "Middlesbrough", 1 },
                    { 187, "E06000003", 807, "Redcar and Cleveland", 1 },
                    { 188, "E06000004", 808, "Stockton-on-Tees", 1 },
                    { 189, "E06000005", 841, "Darlington", 1 },
                    { 190, "E06000047", 840, "County Durham", 1 },
                    { 191, "E06000057", 929, "Northumberland", 1 },
                    { 192, "E08000021", 391, "Newcastle upon Tyne", 1 },
                    { 193, "E08000022", 392, "North Tyneside", 1 },
                    { 194, "E08000023", 393, "South Tyneside", 1 },
                    { 195, "E08000024", 394, "Sunderland", 1 },
                    { 196, "E08000037", 390, "Gateshead", 1 },
                    { 197, "E09000027", 318, "Richmond upon Thames", 7 },
                    { 198, "E09000028", 210, "Southwark", 7 },
                    { 199, "E09000029", 319, "Sutton", 7 },
                    { 200, "E09000030", 211, "Tower Hamlets", 7 },
                    { 201, "E09000031", 320, "Waltham Forest", 7 },
                    { 202, "E09000032", 212, "Wandsworth", 7 },
                    { 203, "E09000033", 213, "Westminster", 7 },
                    { 204, "E09000001", 201, "City of London", 7 },
                    { 205, "E09000002", 301, "Barking and Dagenham", 7 },
                    { 206, "E09000003", 302, "Barnet", 7 },
                    { 207, "E09000004", 303, "Bexley", 7 },
                    { 208, "E09000005", 304, "Brent", 7 },
                    { 209, "E09000006", 305, "Bromley", 7 },
                    { 210, "E09000007", 202, "Camden", 7 },
                    { 211, "E09000008", 306, "Croydon", 7 },
                    { 212, "E09000009", 307, "Ealing", 7 },
                    { 213, "E09000010", 308, "Enfield", 7 },
                    { 214, "E09000011", 203, "Greenwich", 7 },
                    { 215, "E09000012", 204, "Hackney", 7 },
                    { 216, "E09000013", 205, "Hammersmith and Fulham", 7 },
                    { 217, "E09000014", 309, "Haringey", 7 },
                    { 218, "E09000015", 310, "Harrow", 7 },
                    { 219, "E09000016", 311, "Havering", 7 },
                    { 220, "E09000017", 312, "Hillingdon", 7 },
                    { 221, "E09000018", 313, "Hounslow", 7 },
                    { 222, "E09000019", 206, "Islington", 7 },
                    { 223, "E09000020", 207, "Kensington and Chelsea", 7 },
                    { 224, "E09000021", 314, "Kingston upon Thames", 7 },
                    { 225, "E09000022", 208, "Lambeth", 7 },
                    { 226, "E09000023", 209, "Lewisham", 7 },
                    { 227, "E09000024", 315, "Merton", 7 },
                    { 228, "E09000025", 316, "Newham", 7 },
                    { 229, "E09000026", 317, "Redbridge", 7 },
                    { 230, "E06000031", 874, "Peterborough", 6 },
                    { 231, "E06000032", 821, "Luton", 6 },
                    { 232, "E06000033", 882, "Southend-on-Sea", 6 },
                    { 233, "E06000034", 883, "Thurrock", 6 },
                    { 234, "E06000055", 822, "Bedford", 6 },
                    { 235, "E06000056", 823, "Central Bedfordshire", 6 },
                    { 236, "E07000008", 873, "Cambridge", 6 },
                    { 237, "E07000009", 873, "East Cambridgeshire", 6 },
                    { 238, "E07000010", 873, "Fenland", 6 },
                    { 239, "E07000011", 873, "Huntingdonshire", 6 },
                    { 240, "E07000012", 873, "South Cambridgeshire", 6 },
                    { 241, "E07000066", 881, "Basildon", 6 },
                    { 242, "E07000067", 881, "Braintree", 6 },
                    { 243, "E07000068", 881, "Brentwood", 6 },
                    { 244, "E07000069", 881, "Castle Point", 6 },
                    { 245, "E07000070", 881, "Chelmsford", 6 },
                    { 246, "E07000071", 881, "Colchester", 6 },
                    { 247, "E07000072", 881, "Epping Forest", 6 },
                    { 248, "E07000073", 881, "Harlow", 6 },
                    { 249, "E07000074", 881, "Maldon", 6 },
                    { 250, "E07000075", 881, "Rochford", 6 },
                    { 251, "E07000076", 881, "Tendring", 6 },
                    { 252, "E07000077", 881, "Uttlesford", 6 },
                    { 253, "E07000095", 919, "Broxbourne", 6 },
                    { 254, "E07000096", 919, "Dacorum", 6 },
                    { 255, "E07000098", 919, "Hertsmere", 6 },
                    { 256, "E07000099", 919, "North Hertfordshire", 6 },
                    { 257, "E07000102", 919, "Three Rivers", 6 },
                    { 258, "E07000103", 919, "Watford", 6 },
                    { 259, "E07000143", 926, "Breckland", 6 },
                    { 260, "E07000144", 926, "Broadland", 6 },
                    { 261, "E07000145", 926, "Great Yarmouth", 6 },
                    { 262, "E07000146", 926, "King's Lynn and West Norfolk", 6 },
                    { 263, "E07000147", 926, "North Norfolk", 6 },
                    { 264, "E07000148", 926, "Norwich", 6 },
                    { 265, "E07000149", 926, "South Norfolk", 6 },
                    { 266, "E07000200", 935, "Babergh", 6 },
                    { 267, "E07000202", 935, "Ipswich", 6 },
                    { 268, "E07000203", 935, "Mid Suffolk", 6 },
                    { 269, "E07000240", 919, "St Albans", 6 },
                    { 270, "E07000241", 919, "Welwyn Hatfield", 6 },
                    { 271, "E07000242", 919, "East Hertfordshire", 6 },
                    { 272, "E07000243", 919, "Stevenage", 6 },
                    { 273, "E07000244", 935, "East Suffolk", 6 },
                    { 274, "E07000245", 935, "West Suffolk", 6 },
                    { 275, "E06000015", 831, "Derby", 4 },
                    { 276, "E06000016", 856, "Leicester", 4 },
                    { 277, "E06000017", 857, "Rutland", 4 },
                    { 278, "E06000018", 892, "Nottingham", 4 },
                    { 279, "E06000061", 940, "North Northamptonshire", 4 },
                    { 280, "E06000062", 941, "West Northamptonshire", 4 },
                    { 281, "E07000032", 830, "Amber Valley", 4 },
                    { 282, "E07000033", 830, "Bolsover", 4 },
                    { 283, "E07000034", 830, "Chesterfield", 4 },
                    { 284, "E07000035", 830, "Derbyshire Dales", 4 },
                    { 285, "E07000036", 830, "Erewash", 4 },
                    { 286, "E07000037", 830, "High Peak", 4 },
                    { 287, "E07000038", 830, "North East Derbyshire", 4 },
                    { 288, "E07000039", 830, "South Derbyshire", 4 },
                    { 289, "E07000129", 855, "Blaby", 4 },
                    { 290, "E07000130", 855, "Charnwood", 4 },
                    { 291, "E07000131", 855, "Harborough", 4 },
                    { 292, "E07000132", 855, "Hinckley and Bosworth", 4 },
                    { 293, "E07000133", 855, "Melton", 4 },
                    { 294, "E07000134", 855, "North West Leicestershire", 4 },
                    { 295, "E07000135", 855, "Oadby and Wigston", 4 },
                    { 296, "E07000136", 925, "Boston", 4 },
                    { 297, "E07000137", 925, "East Lindsey", 4 },
                    { 298, "E07000138", 925, "Lincoln", 4 },
                    { 299, "E07000139", 925, "North Kesteven", 4 },
                    { 300, "E07000140", 925, "South Holland", 4 },
                    { 301, "E07000141", 925, "South Kesteven", 4 },
                    { 302, "E07000142", 925, "West Lindsey", 4 },
                    { 303, "E07000170", 891, "Ashfield", 4 },
                    { 304, "E07000171", 891, "Bassetlaw", 4 },
                    { 305, "E07000172", 891, "Broxtowe", 4 },
                    { 306, "E07000173", 891, "Gedling", 4 },
                    { 307, "E07000174", 891, "Mansfield", 4 },
                    { 308, "E07000175", 891, "Newark and Sherwood", 4 },
                    { 309, "E07000176", 891, "Rushcliffe", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyStage_Name",
                table: "KeyStage",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_KeyStage_SeoUrl",
                table: "KeyStage",
                column: "SeoUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_Code",
                table: "LocalAuthority",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_Name",
                table: "LocalAuthority",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthority_RegionId",
                table: "LocalAuthority",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_LocalAuthorityDistrictId",
                table: "LocalAuthorityDistrictCoverage",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_Code",
                table: "LocalAuthorityDistricts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_LocalAuthorityId",
                table: "LocalAuthorityDistricts",
                column: "LocalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_Name",
                table: "LocalAuthorityDistricts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistricts_RegionId",
                table: "LocalAuthorityDistricts",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_SubjectId",
                table: "Prices",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_TuitionPartnerId",
                table: "Prices",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_TuitionTypeId",
                table: "Prices",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_SubjectId",
                table: "SubjectCoverage",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId",
                table: "SubjectCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionTypeId",
                table: "SubjectCoverage",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_KeyStageId",
                table: "Subjects",
                column: "KeyStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SeoUrl",
                table: "Subjects",
                column: "SeoUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_Name",
                table: "TuitionPartners",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_SeoUrl",
                table: "TuitionPartners",
                column: "SeoUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionTypes_Name",
                table: "TuitionTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionTypes_SeoUrl",
                table: "TuitionTypes",
                column: "SeoUrl",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "SubjectCoverage");

            migrationBuilder.DropTable(
                name: "LocalAuthorityDistricts");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "TuitionPartners");

            migrationBuilder.DropTable(
                name: "TuitionTypes");

            migrationBuilder.DropTable(
                name: "LocalAuthority");

            migrationBuilder.DropTable(
                name: "KeyStage");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
