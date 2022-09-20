using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using Cookies = UI.Pages.Cookies;



namespace Tests;

[Collection(nameof(SliceFixture))]
public class CookiesTests : CleanSliceFixture
{
    public CookiesTests(SliceFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(null, true)]
    [InlineData(true, null)]
    [InlineData(null, null)]
    public void Cookies_OnGet_Test(bool? consent, bool? preferencesSet)
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var pageContext = new PageContext(actionContext);
        Cookies cookiePage = new()
        {
            PageContext = pageContext
        };
        var result = cookiePage.OnGet(consent, preferencesSet);
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async void Cookies_For_FindATuitionPartner_Created_OnGet()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
              .Execute(page =>
              {
                  page.PageContext = GetPageContext();
                  page.OnGet(true, true);
                  return Task.FromResult(page);
              });
        cookiePage.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
        cookieVariables.ToString().Should().Contain(".FindATuitionPartner.Consent=True");
    }

    [Fact]
    public async void Cookies_For_FindATuitionPartner_Retrieved_OnGet()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
              .Execute(page =>
              {
                  page.PageContext = GetPageContext();
                  page.Request.Cookies = MockRequestCookieCollection(".FindATuitionPartner", "true");
                  page.OnGet(true, true);
                  return Task.FromResult(page);
              });
        cookiePage.Request.Cookies.ContainsKey(".FindATuitionPartner").Should().BeTrue();
        cookiePage.Request.Cookies[".FindATuitionPartner"]?.ToString().Should().Be("true");
        cookiePage.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
        cookieVariables.ToString().Should().Contain(".FindATuitionPartner.Consent=True");
    }

    [Fact]
    public async void Cookies_For_FindATuitionPartner_Retrieved_And_Set_To_True_OnGet()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
              .Execute(page =>
              {
                  page.PageContext = GetPageContext();
                  page.Request.Cookies = MockRequestCookieCollection(".FindATuitionPartner", "false");
                  page.OnGet(true, true);
                  return Task.FromResult(page);
              });
        cookiePage.Request.Cookies.ContainsKey(".FindATuitionPartner").Should().BeTrue();
        cookiePage.Request.Cookies[".FindATuitionPartner"]?.ToString().Should().Be("false");

        cookiePage.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
        cookieVariables.ToString().Should().Contain(".FindATuitionPartner.Consent=True");
    }

    [Fact]
    public async void Cookies_For_FindATuitionPartner_Retrieved_And_Set_To_False_OnGet()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
              .Execute(page =>
              {
                  page.PageContext = GetPageContext();
                  page.Request.Cookies = MockRequestCookieCollection(".FindATuitionPartner", "true");
                  page.OnGet(false, true);
                  return Task.FromResult(page);
              });
        cookiePage.Request.Cookies.ContainsKey(".FindATuitionPartner").Should().BeTrue();
        cookiePage.Request.Cookies[".FindATuitionPartner"]?.ToString().Should().Be("true");

        cookiePage.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
        cookieVariables.ToString().Should().Contain(".FindATuitionPartner.Consent=False");
    }

    [Fact]
    public async void Cookies_Associated_With_FindATuitionPartner_Deleted_When_Set_To_False()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
              .Execute(page =>
              {
                  page.PageContext = GetPageContext();
                  page.Request.Cookies = MockRequestCookieCollection(".FindATuitionPartner", "true");
                  page.Request.Cookies.ContainsKey("_ga").Should().BeTrue();
                  page.Request.Cookies.ContainsKey("_gid").Should().BeTrue();
                  var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(12), HttpOnly = true, Secure = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict };
                  page.Response.Cookies.Append("_ga", "hasValueToDelete", cookieOptions);
                  page.Response.Cookies.Append("_gid", "hasValueToDelete", cookieOptions);
                  page.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
                  cookieVariables.ToString().Should().Contain("_ga=hasValueToDelete;");
                  cookieVariables.ToString().Should().Contain("_gid=hasValueToDelete;");
                  page.OnGet(false, true);
                  return Task.FromResult(page);
              });
        cookiePage.Response.Headers.TryGetValue("Set-Cookie", out var cookieVariables);
        cookieVariables.ToString().Should().Contain("_ga=;");
        cookieVariables.ToString().Should().Contain("_gid=;");
    }

    [Fact]
    public async void Cookies_OnPost_Model_Valid()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
            .Execute(page =>
            {
                page.PageContext = GetPageContext();
                page.OnPost();
                return Task.FromResult(page);
            });
        cookiePage.Should().BeOfType<Cookies>();
        cookiePage.ModelState.IsValid.Should().BeTrue();

        var result = cookiePage.OnPost();
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async void Cookies_OnPost_Model_Invalid()
    {
        var cookiePage = await Fixture.GetPage<Cookies>()
           .Execute(page =>
           {
               page.PageContext = GetPageContext();
               page.ModelState.AddModelError("cookies", "You must select an option");
               page.OnPost();
               return Task.FromResult(page);
           });
        cookiePage.Should().BeOfType<Cookies>();
        cookiePage.ModelState.IsValid.Should().BeFalse();

        cookiePage.ModelState.AddModelError("cookies", "You must select an option");
        var result = cookiePage.OnPost();
        result.Should().BeOfType<PageResult>();
    }

    private static IRequestCookieCollection MockRequestCookieCollection(string key, string value)
    {
        var requestFeature = new HttpRequestFeature();
        var featureCollection = new FeatureCollection();
        requestFeature.Headers = new HeaderDictionary();
        requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));
        requestFeature.Headers.Append(HeaderNames.Cookie, new StringValues("_ga" + "=" + "true"));
        requestFeature.Headers.Append(HeaderNames.Cookie, new StringValues("_gid" + "=" + "true"));
        featureCollection.Set<IHttpRequestFeature>(requestFeature);
        var cookiesFeature = new RequestCookiesFeature(featureCollection);
        return cookiesFeature.Cookies;
    }

    private static PageContext GetPageContext()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        return new PageContext(actionContext);
    }
}
