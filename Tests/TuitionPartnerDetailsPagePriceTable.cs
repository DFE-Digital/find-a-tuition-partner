using Domain.Constants;
using UI.Extensions;
using GroupPrice = UI.Pages.TuitionPartner.GroupPrice;

namespace Tests;

public class TuitionPartnerDetailsPagePriceTable
{
    [Theory]
    [MemberData(nameof(PriceData))]
    public void Prices_are_hidden_when_there_are_none_for_a_tuition_type(
        bool showInSchool, bool showOnline, Dictionary<int, GroupPrice> price)
    {
        price.ContainsInSchoolPrice().Should().Be(showInSchool);
        price.ContainsOnlinePrice().Should().Be(showOnline);
    }

    public static IEnumerable<object[]> PriceData()
    {
        yield return new object[]
        {
            false, false, new Dictionary<int, GroupPrice> { }
        };

        yield return new object[]
        {
            false, false, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(null, null, null, null) },
            },
        };

        yield return new object[]
        {
            true, false, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: 1, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            true, false, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: null, SchoolMax: 1, OnlineMin: null, OnlineMax: null) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            false, true, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: 1, OnlineMax: null) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            false, true, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: 1) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            false, true, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: 1, OnlineMax: 1) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            true, false, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: 1, SchoolMax: 1, OnlineMin: null, OnlineMax: null) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };

        yield return new object[]
        {
            true, true, new Dictionary<int, GroupPrice>
            {
                { 1, new GroupPrice(SchoolMin: 1, SchoolMax: null, OnlineMin: 1, OnlineMax: null) },
                { 2, new GroupPrice(SchoolMin: null, SchoolMax: null, OnlineMin: null, OnlineMax: null) },
            },
        };
    }

    [Theory]
    [MemberData(nameof(GroupPriceFormat))]
    public void Formats_group_price(TuitionTypes tuitionType, GroupPrice price, string expected)
    {
        price.FormatFor(tuitionType).Should().Be(expected);
    }

    public static IEnumerable<object[]> GroupPriceFormat()
    {
        yield return new object[]
        {
            TuitionTypes.InSchool, new GroupPrice(SchoolMin: null, null, OnlineMin: null, null), ""
        };
        yield return new object[]
        {
            TuitionTypes.InSchool, new GroupPrice(SchoolMin: 1, SchoolMax: 2, OnlineMin: 3, OnlineMax: 4), "£1 to £2"
        };
        yield return new object[]
        {
            TuitionTypes.InSchool, new GroupPrice(SchoolMin: 1.12m, SchoolMax: 2.99m, OnlineMin: 3.33m, OnlineMax: 4.99m), "£1.12 to £2.99"
        };
        yield return new object[]
        {
            TuitionTypes.InSchool, new GroupPrice(SchoolMin: 1.1m, SchoolMax: 2.8m, OnlineMin: 3.33m, OnlineMax: 4.99m), "£1.10 to £2.80"
        };
        yield return new object[]
        {
            TuitionTypes.InSchool, new GroupPrice(SchoolMin: 8, SchoolMax: 8, OnlineMin: 12, OnlineMax: 12), "£8"
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: null, null, OnlineMin: null, null), ""
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: 1, SchoolMax: 2, OnlineMin: 3, OnlineMax: 4), "£3 to £4"
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: 1.12m, SchoolMax: 2.99m, OnlineMin: 3.33m, OnlineMax: 4.99m), "£3.33 to £4.99"
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: 8, SchoolMax: 8, OnlineMin: 12, OnlineMax: 12), "£12"
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: 8, SchoolMax: 8, OnlineMin: 12.22m, OnlineMax: 12.22m), "£12.22"
        };
        yield return new object[]
        {
            TuitionTypes.Online, new GroupPrice(SchoolMin: 8, SchoolMax: 8, OnlineMin: 12.20m, OnlineMax: 12.20m), "£12.20"
        };
    }
}