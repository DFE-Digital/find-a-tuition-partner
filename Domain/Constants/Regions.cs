namespace Domain.Constants;

public class Regions
{
    public class Id
    {
        public const int NorthEast = 1;
        public const int NorthWest = 2;
        public const int YorkshireandTheHumber = 3;
        public const int EastMidlands = 4;
        public const int WestMidlands = 5;
        public const int EastofEngland = 6;
        public const int London = 7;
        public const int SouthEast = 8;
        public const int SouthWest = 9;
    }

    private class Initials
    {
        public const string NorthEast = "NE";
        public const string NorthWest = "NW";
        public const string YorkshireandTheHumber = "YH";
        public const string EastMidlands = "EM";
        public const string WestMidlands = "WM";
        public const string EastofEngland = "E";
        public const string London = "L";
        public const string SouthEast = "SE";
        public const string SouthWest = "SW";
    }

    public static readonly IDictionary<string, int> InitialsToId = new Dictionary<string, int>
    {
        { Initials.NorthEast, Id.NorthEast },
        { Initials.NorthWest, Id.NorthWest },
        { Initials.YorkshireandTheHumber, Id.YorkshireandTheHumber },
        { Initials.EastMidlands, Id.EastMidlands },
        { Initials.WestMidlands, Id.WestMidlands },
        { Initials.EastofEngland, Id.EastofEngland },
        { Initials.London, Id.London },
        { Initials.SouthEast, Id.SouthEast },
        { Initials.SouthWest, Id.SouthWest }
    };
}