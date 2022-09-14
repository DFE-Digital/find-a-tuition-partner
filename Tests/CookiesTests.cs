using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Cookies = UI.Pages.Cookies;

namespace Tests;
public class CookiesTests
{

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
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };
        Cookies cookiePage = new()
        {
            Consent = true,
            PageContext = pageContext,
            Url = new Microsoft.AspNetCore.Mvc.Routing.UrlHelper(actionContext)
        };


        var resultPrivacy = cookiePage.OnGet(consent, preferencesSet);
        resultPrivacy.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Cookies_OnPost_Test()
    {
        Cookies cookiePage = new();
        var resultPrivacy = cookiePage.OnPost();
        resultPrivacy.Should().BeOfType<RedirectToPageResult>();
    }
}
