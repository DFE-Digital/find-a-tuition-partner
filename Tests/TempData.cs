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
        data.Get<string>("Bananas").Should().Be("this is a string");
    }

    [Fact]
    public void No_exception_when_setting_null_temp_data()
    {
        ((ITempDataDictionary?)null).Set("Bananas", "this is a string");
    }

    [Fact]
    public void No_exception_when_getting_null_temp_data()
    {
        ((ITempDataDictionary?)null).Get<string>("Bananas").Should().BeNull();
    }

    [Fact]
    public void No_exception_when_getting_incorrect_type()
    {
        var data = new TestableTempDataDictionary();
        data.Set("Bananas", "this is a string");
        data.Get<SomeClass>("Bananas").Should().BeNull();
    }


    internal class SomeClass { }

    internal class TestableTempDataDictionary : Dictionary<string, object?>, ITempDataDictionary
    {
        public void Keep() => throw new NotImplementedException();
        public void Keep(string key) => throw new NotImplementedException();
        public void Load() => throw new NotImplementedException();
        public object? Peek(string key) => throw new NotImplementedException();
        public void Save() => throw new NotImplementedException();
    }
}