namespace Tests.TestData;

public record District(int Id, string Code, string Name, string SamplePostcode, string LocalAuthorityName, int LocalAuthorityId)
{
    public static District Dacorum { get; } = new(254, "E07000096", "Dacorum", "AA00AA", "Hertfordshire", 919);
    public static District EastRidingOfYorkshire { get; } = new(1, "E06000011", "East Riding of Yorkshire", "DN14 0AA", "East Riding of Yorkshire", 811);
    public static District NorthEastLincolnshire { get; } = new(2, "E06000012", "North East Lincolnshire", "DN31 1AB", "North East Lincolnshire", 812);
    public static District Ryedale { get; } = new(9, "E07000167", "Ryedale", "DL6 3QE", "North Yorkshire", 815);
}
