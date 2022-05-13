using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TuitionPartners",
                columns: new[] { "Id", "Name", "Website" },
                values: new object[,]
                {
                    { 1, "Action Tutoring", "https://actiontutoring.org.uk/" },
                    { 2, "Affinity Workforce Solutions", "https://tutoring.affinityworkforce.com/" },
                    { 3, "Appla Tuition", "https://www.appla.co.uk/home/" },
                    { 4, "BYT Centres", "https://brightyoungthings.co.uk/" },
                    { 5, "Cambridge Tuition Limited", "https://www.tutordoctor.co.uk/cambridge/" },
                    { 6, "Career Group Courses", "https://www.careertree.com/" },
                    { 7, "Coachbright", "https://www.coachbright.org/nationaltutoringprogramme" },
                    { 8, "Conexus Tuition", "https://conexustuition.co.uk/" },
                    { 9, "Connex Education Partnership Limited", "https://connex-education.com/" },
                    { 10, "EM Skills Enterprise CIC (EM Tuition)", "https://emtuition.org.uk/" },
                    { 11, "Engage Partners", "https://engage-education.com/national-tutoring-programme/" },
                    { 12, "Equal Education", "https://www.equal.education/national-tutoring-programme" },
                    { 13, "Equal Education Partners (E-Qual)", "https://equaleducationpartners.com/tutoring/#:~:text=The%20NTP%20aims%20to%20help,we%20can%20support%20your%20school." },
                    { 14, "FFT", "https://fft.org.uk/tutoring/" },
                    { 15, "Fledge Tuition Ltd", "https://www.fledgetuition.com/#/" },
                    { 16, "Fleet Education Services Limited", "https://www.fleet-tutors.co.uk/" },
                    { 17, "Lancashire County Council", "https://www.lancsngfl.ac.uk/projects/ema/index.php?category_id=289&s=!B121cf29d70ec8a3d54a33343010cc2" },
                    { 18, "Learning Academies", "https://learningacademies.co.uk/national-tuition-partners/" },
                    { 19, "Mannings Tutors", "https://manningstutors.co.uk/ntp/#:~:text=We%20are%20very%20excited%20to,doing%20during%20these%20challenging%20times." },
                    { 20, "My Tutor", "https://www.mytutor.co.uk/schools/national-tutoring-programme/" },
                    { 21, "Pearson Education Ltd", "https://www.pearson.com/uk/educators/schools/pearson-tutoring-programme.html" },
                    { 22, "Pet-Xi Training", "https://www.pet-xi.co.uk/services/school-programmes/national-tuition-partners-programme/" },
                    { 23, "Protocol Education Ltd", "https://www.protocol-education.com/ntp?source=google.co.uk" },
                    { 24, "Quest for Learning", "https://questforlearning.org.uk/ntp-tutoring/" },
                    { 25, "Randstad HR Solutions", "https://www.randstad.co.uk/job-seeker/areas-of-expertise/education/ntp-tutor-training/" },
                    { 26, "Reed Tutors", "https://offers.reed.com/national-tutoring-programme" },
                    { 27, "Schools Partnership Tutors", "https://www.sptutors.co.uk/sp-tutors-national-tutoring-programme/" },
                    { 28, "Step Teachers Limited", "https://www.stepteachers.co.uk/job/ntp-tutor" },
                    { 29, "Talent-Ed Education", "https://www.talent-ed.uk/" },
                    { 30, "Targeted provision Limited", "https://targetedprovision.com/national-tutoring-programme#:~:text=What%20modes%20of%20tutoring%20do,available%20to%20continue%20throughout%20holidays." },
                    { 31, "Teaching Personnel", "https://www.teachingpersonnel.com/ntp" },
                    { 32, "Tempest Resourcing", "https://tempestresourcing.co.uk/the-national-tutoring-programme/" },
                    { 33, "The Brilliant Club", "https://thebrilliantclub.org/news/national-tutoring-programme/" },
                    { 34, "The Tutor Trust", "https://www.thetutortrust.org/national-tutoring-programme" },
                    { 35, "TLC Live", "https://www.tlclive.com/approved-ntp-partner" },
                    { 36, "Tute Education", "https://www.tute.com/national-tutoring-programme/" },
                    { 37, "Tutor Doctor Beeston Park", "https://www.tutordoctor.co.uk/beeston-park/national-tutoring-programme/" },
                    { 38, "Tutors Green", "https://tutorsgreen.com/" },
                    { 39, "White Rose Maths", "https://whiterosemaths.com/" },
                    { 40, "Zen Educate", "https://www.zeneducate.com/national-tutoring-programme" },
                    { 41, "Nebula Education Ltd (Trading as Seven Springs Education)", "https://seven-springs.co.uk/national-tutoring-programme.php" },
                    { 42, "The St Albans Tuition Centre", "https://stalbanstuitioncentre.co.uk/new-page-3" },
                    { 43, "Nudge Education Limited", "https://nudgeeducation.co.uk/" },
                    { 44, "Learning Hive", "https://www.learninghive.co.uk/blog/learning-hive-approved-tuition-provider-national-tutoring-programme" },
                    { 45, "Bright Heart Education Ltd", "https://www.brightheart.co.uk/ntp/" },
                    { 46, "Avon Education Services Ltd (Tutor Doctor Bristol)", "https://www.tutordoctor.co.uk/bristol/" },
                    { 47, "K&G Recruitment Consultancy Ltd t/a REESON Education", "https://www.reesoneducation.com/national-tutoring-programme" },
                    { 48, "3D Recruit Ltd", "https://www.3drecruiteducation.com/#!amy-gudgeon/zoom/exss3/dataItem-io5s4mw6" },
                    { 49, "Toranj Tuition", "https://toranjtuition.org/2022/04/07/national-tutoring-partnership/#:~:text=TORANJ%20TUITION%2C%20a%20community%20and,of%20learning%20during%20the%20pandemic." },
                    { 50, "ADM Computer Services Ltd (The Online Teacher)", "" },
                    { 51, "Guardian Selection Ltd", "" },
                    { 52, "Simply Learning Tuition Agency Ltd", "https://www.simplylearningtuition.co.uk/" },
                    { 53, "Prospero Group Ltd", "https://prosperoteaching.com/prospero-tutoring-for-tutors/#:~:text=Prospero%20are%20an%20approved%20tuition,those%20receiving%20Pupil%20Premium%20funding." },
                    { 54, "Third Space Learning/Virtual Class Ltd", "https://thirdspacelearning.com/national-tutoring-programme/" },
                    { 55, "Tutor In A Box", "https://www.tutorinabox.co.uk/ntp/" },
                    { 56, "Assess Education Ltd", "https://assesseducation.co.uk/national-tutoring-programme/" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 56);
        }
    }
}
