namespace Tests.Utilities;

public class CleanSliceFixture : IAsyncLifetime
{
    public CleanSliceFixture(SliceFixture fixture) => Fixture = fixture;

    public SliceFixture Fixture { get; }

    public Task InitializeAsync() => ResetDatabase();

    public Task DisposeAsync() => Task.CompletedTask;

    public Task ResetDatabase() => Fixture.ExecuteDbContextAsync(async db =>
    {
        //await db.Database.EnsureDeletedAsync();
        //await db.Database.EnsureCreatedAsync();
        //db.Subjects.RemoveRange(db.Subjects);
        db.TuitionPartnerCoverage.RemoveRange(db.TuitionPartnerCoverage);
        //db.LocalAuthorityDistricts.RemoveRange(db.LocalAuthorityDistricts);
        //db.TuitionTypes.RemoveRange(db.TuitionTypes);
        db.TuitionPartners.RemoveRange(db.TuitionPartners);
        //db.Regions.RemoveRange(db.Regions);
        //db.TutorTypes.RemoveRange(db.TutorTypes);
        //return db.SaveChangesAsync();
    });
}
