namespace Tests.Utilities;

public class CleanSliceFixture : IAsyncLifetime
{
    public CleanSliceFixture(SliceFixture fixture) => Fixture = fixture;

    public SliceFixture Fixture { get; }

    public virtual Task InitializeAsync() => ResetDatabase();

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public Task ResetDatabase() => Fixture.ExecuteDbContextAsync(async db =>
    {
        //await db.Database.EnsureDeletedAsync();
        //await db.Database.EnsureCreatedAsync();
        //db.Subjects.RemoveRange(db.Subjects);
        //db.LocalAuthorityDistricts.RemoveRange(db.LocalAuthorityDistricts);
        //db.TuitionTypes.RemoveRange(db.TuitionTypes);
        db.Prices.RemoveRange(db.Prices);
        db.SubjectCoverage.RemoveRange(db.SubjectCoverage);
        db.TuitionPartners.RemoveRange(db.TuitionPartners);
        //db.Regions.RemoveRange(db.Regions);
        //db.TutorTypes.RemoveRange(db.TutorTypes);
        await db.SaveChangesAsync();
    });
}
