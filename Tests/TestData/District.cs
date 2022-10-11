namespace Tests.TestData;

public record District(int Id, string Code, string Name, string SamplePostcode)
{
    public static District Dacorum { get; } = new(254, "E07000096", "Dacorum", "AA00AA");
    public static District EastRidingOfYorkshire { get; } = new(1, "E06000011", "East Riding of Yorkshire", "");
    public static District NorthEastLincolnshire { get; } = new(2, "E06000012", "North East Lincolnshire", "");
    public static District Ryedale { get; } = new(9, "E07000167", "Ryedale", "");
}
