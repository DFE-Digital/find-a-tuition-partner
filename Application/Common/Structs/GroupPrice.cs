namespace Application.Common.Structs;

public readonly record struct GroupPrice(decimal? SchoolMin, decimal? SchoolMax, decimal? OnlineMin, decimal? OnlineMax)
{
    public bool HasAtLeastOnePrice =>
        OnlineMin != null || OnlineMax != null || SchoolMin != null || SchoolMax != null;

    public bool HasVariation => OnlineMin != OnlineMax || (SchoolMin != SchoolMax);
}