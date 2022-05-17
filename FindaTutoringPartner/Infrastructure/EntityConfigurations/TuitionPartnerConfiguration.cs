using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerConfiguration : IEntityTypeConfiguration<TuitionPartner>
{
    public void Configure(EntityTypeBuilder<TuitionPartner> builder)
    {
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new TuitionPartner {Id = 1, Name = "Action Tutoring", Website = "https://actiontutoring.org.uk/"},
            new TuitionPartner {Id = 2, Name = "Affinity Workforce Solutions", Website = "https://tutoring.affinityworkforce.com/"},
            new TuitionPartner {Id = 3, Name = "Appla Tuition", Website = "https://www.appla.co.uk/home/"},
            new TuitionPartner {Id = 4, Name = "BYT Centres", Website = "https://brightyoungthings.co.uk/"},
            new TuitionPartner {Id = 5, Name = "Cambridge Tuition Limited", Website = "https://www.tutordoctor.co.uk/cambridge/"},
            new TuitionPartner {Id = 6, Name = "Career Group Courses", Website = "https://www.careertree.com/"},
            new TuitionPartner {Id = 7, Name = "Coachbright", Website = "https://www.coachbright.org/nationaltutoringprogramme"},
            new TuitionPartner {Id = 8, Name = "Conexus Tuition", Website = "https://conexustuition.co.uk/"},
            new TuitionPartner {Id = 9, Name = "Connex Education Partnership Limited", Website = "https://connex-education.com/"},
            new TuitionPartner {Id = 10, Name = "EM Skills Enterprise CIC (EM Tuition)", Website = "https://emtuition.org.uk/"},
            new TuitionPartner {Id = 11, Name = "Engage Partners", Website = "https://engage-education.com/national-tutoring-programme/"},
            new TuitionPartner {Id = 12, Name = "Equal Education", Website = "https://www.equal.education/national-tutoring-programme"},
            new TuitionPartner {Id = 13, Name = "Equal Education Partners (E-Qual)", Website = "https://equaleducationpartners.com/tutoring/#:~:text=The%20NTP%20aims%20to%20help,we%20can%20support%20your%20school."},
            new TuitionPartner {Id = 14, Name = "FFT", Website = "https://fft.org.uk/tutoring/"},
            new TuitionPartner {Id = 15, Name = "Fledge Tuition Ltd", Website = "https://www.fledgetuition.com/#/"},
            new TuitionPartner {Id = 16, Name = "Fleet Education Services Limited", Website = "https://www.fleet-tutors.co.uk/"},
            new TuitionPartner {Id = 17, Name = "Lancashire County Council", Website = "https://www.lancsngfl.ac.uk/projects/ema/index.php?category_id=289&s=!B121cf29d70ec8a3d54a33343010cc2"},
            new TuitionPartner {Id = 18, Name = "Learning Academies", Website = "https://learningacademies.co.uk/national-tuition-partners/"},
            new TuitionPartner {Id = 19, Name = "Mannings Tutors", Website = "https://manningstutors.co.uk/ntp/#:~:text=We%20are%20very%20excited%20to,doing%20during%20these%20challenging%20times."},
            new TuitionPartner {Id = 20, Name = "My Tutor", Website = "https://www.mytutor.co.uk/schools/national-tutoring-programme/"},
            new TuitionPartner {Id = 21, Name = "Pearson Education Ltd", Website = "https://www.pearson.com/uk/educators/schools/pearson-tutoring-programme.html"},
            new TuitionPartner {Id = 22, Name = "Pet-Xi Training", Website = "https://www.pet-xi.co.uk/services/school-programmes/national-tuition-partners-programme/"},
            new TuitionPartner {Id = 23, Name = "Protocol Education Ltd", Website = "https://www.protocol-education.com/ntp?source=google.co.uk"},
            new TuitionPartner {Id = 24, Name = "Quest for Learning", Website = "https://questforlearning.org.uk/ntp-tutoring/"},
            new TuitionPartner {Id = 25, Name = "Randstad HR Solutions", Website = "https://www.randstad.co.uk/job-seeker/areas-of-expertise/education/ntp-tutor-training/"},
            new TuitionPartner {Id = 26, Name = "Reed Tutors", Website = "https://offers.reed.com/national-tutoring-programme"},
            new TuitionPartner {Id = 27, Name = "Schools Partnership Tutors", Website = "https://www.sptutors.co.uk/sp-tutors-national-tutoring-programme/"},
            new TuitionPartner {Id = 28, Name = "Step Teachers Limited", Website = "https://www.stepteachers.co.uk/job/ntp-tutor"},
            new TuitionPartner {Id = 29, Name = "Talent-Ed Education", Website = "https://www.talent-ed.uk/"},
            new TuitionPartner {Id = 30, Name = "Targeted provision Limited", Website = "https://targetedprovision.com/national-tutoring-programme#:~:text=What%20modes%20of%20tutoring%20do,available%20to%20continue%20throughout%20holidays."},
            new TuitionPartner {Id = 31, Name = "Teaching Personnel", Website = "https://www.teachingpersonnel.com/ntp"},
            new TuitionPartner {Id = 32, Name = "Tempest Resourcing", Website = "https://tempestresourcing.co.uk/the-national-tutoring-programme/"},
            new TuitionPartner {Id = 33, Name = "The Brilliant Club", Website = "https://thebrilliantclub.org/news/national-tutoring-programme/"},
            new TuitionPartner {Id = 34, Name = "The Tutor Trust", Website = "https://www.thetutortrust.org/national-tutoring-programme"},
            new TuitionPartner {Id = 35, Name = "TLC Live", Website = "https://www.tlclive.com/approved-ntp-partner"},
            new TuitionPartner {Id = 36, Name = "Tute Education", Website = "https://www.tute.com/national-tutoring-programme/"},
            new TuitionPartner {Id = 37, Name = "Tutor Doctor Beeston Park", Website = "https://www.tutordoctor.co.uk/beeston-park/national-tutoring-programme/"},
            new TuitionPartner {Id = 38, Name = "Tutors Green", Website = "https://tutorsgreen.com/"},
            new TuitionPartner {Id = 39, Name = "White Rose Maths", Website = "https://whiterosemaths.com/"},
            new TuitionPartner {Id = 40, Name = "Zen Educate", Website = "https://www.zeneducate.com/national-tutoring-programme"},
            new TuitionPartner {Id = 41, Name = "Nebula Education Ltd (Trading as Seven Springs Education)", Website = "https://seven-springs.co.uk/national-tutoring-programme.php"},
            new TuitionPartner {Id = 42, Name = "The St Albans Tuition Centre", Website = "https://stalbanstuitioncentre.co.uk/new-page-3"},
            new TuitionPartner {Id = 43, Name = "Nudge Education Limited", Website = "https://nudgeeducation.co.uk/"},
            new TuitionPartner {Id = 44, Name = "Learning Hive", Website = "https://www.learninghive.co.uk/blog/learning-hive-approved-tuition-provider-national-tutoring-programme"},
            new TuitionPartner {Id = 45, Name = "Bright Heart Education Ltd", Website = "https://www.brightheart.co.uk/ntp/"},
            new TuitionPartner {Id = 46, Name = "Avon Education Services Ltd (Tutor Doctor Bristol)", Website = "https://www.tutordoctor.co.uk/bristol/"},
            new TuitionPartner {Id = 47, Name = "K&G Recruitment Consultancy Ltd t/a REESON Education", Website = "https://www.reesoneducation.com/national-tutoring-programme"},
            new TuitionPartner {Id = 48, Name = "3D Recruit Ltd", Website = "https://www.3drecruiteducation.com/#!amy-gudgeon/zoom/exss3/dataItem-io5s4mw6"},
            new TuitionPartner {Id = 49, Name = "Toranj Tuition", Website = "https://toranjtuition.org/2022/04/07/national-tutoring-partnership/#:~:text=TORANJ%20TUITION%2C%20a%20community%20and,of%20learning%20during%20the%20pandemic."},
            new TuitionPartner {Id = 50, Name = "ADM Computer Services Ltd (The Online Teacher)", Website = ""},
            new TuitionPartner {Id = 51, Name = "Guardian Selection Ltd", Website = ""},
            new TuitionPartner {Id = 52, Name = "Simply Learning Tuition Agency Ltd", Website = "https://www.simplylearningtuition.co.uk/"},
            new TuitionPartner {Id = 53, Name = "Prospero Group Ltd", Website = "https://prosperoteaching.com/prospero-tutoring-for-tutors/#:~:text=Prospero%20are%20an%20approved%20tuition,those%20receiving%20Pupil%20Premium%20funding."},
            new TuitionPartner {Id = 54, Name = "Third Space Learning/Virtual Class Ltd", Website = "https://thirdspacelearning.com/national-tutoring-programme/"},
            new TuitionPartner {Id = 55, Name = "Tutor In A Box", Website = "https://www.tutorinabox.co.uk/ntp/"},
            new TuitionPartner {Id = 56, Name = "Assess Education Ltd", Website = "https://assesseducation.co.uk/national-tutoring-programme/"}
        );
    }
}