using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UI.Extensions;

namespace Tests;

public class TempData
{
    [Fact]
    public void Can_set_and_get_temp_data()
    {
        var data = new TestableTempDataDictionary();
        data.Set("Bananas", "this is a string");
        data.Peek<string>("Bananas").Should().Be("this is a string");
    }

    [Fact]
    public void No_exception_when_setting_null_temp_data()
    {
        ((ITempDataDictionary?)null).Set("Bananas", "this is a string");
    }

    [Fact]
    public void No_exception_when_getting_null_temp_data()
    {
        ((ITempDataDictionary?)null).Peek<string>("Bananas").Should().BeNull();
    }

    [Fact]
    public void No_exception_when_getting_incorrect_type()
    {
        var data = new TestableTempDataDictionary();
        data.Set("Bananas", "this is a string");
        data.Peek<SomeClass>("Bananas").Should().BeNull();
    }

    internal class SomeClass
    { }

    internal class TestableTempDataDictionary : TempDataDictionary
    {
        public TestableTempDataDictionary()
            : base(new DefaultHttpContext(), new TestableTempDataProvider())
        {
        }

        private class TestableTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context)
                => new Dictionary<string, object>();

            public void SaveTempData(HttpContext context, IDictionary<string, object> values)
            {
            }
        }
    }
}