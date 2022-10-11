namespace Tests.Utilities;

public class CleanSliceFixture : IAsyncLifetime
{
    public CleanSliceFixture(SliceFixture fixture) => Fixture = fixture;

    public SliceFixture Fixture { get; }

    public virtual Task InitializeAsync() => ResetDatabase();

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public Task ResetDatabase() => Fixture.ExecuteDbContextAsync(async db =>
    {
        db.Prices.RemoveRange(db.Prices);
        db.SubjectCoverage.RemoveRange(db.SubjectCoverage);
        db.TuitionPartners.RemoveRange(db.TuitionPartners);
        await db.SaveChangesAsync();
    });
}
