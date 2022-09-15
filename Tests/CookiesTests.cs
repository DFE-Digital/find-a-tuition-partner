using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
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
        var pageContext = new PageContext(actionContext);
       
        Cookies cookiePage = new()
        {
            PageContext = pageContext
        };
        var result = cookiePage.OnGet(consent, preferencesSet);
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Cookies_OnPost_Model_Invalid_Test()
    {
        Cookies cookiePage = new();
        cookiePage.PageContext.ModelState.AddModelError("Message.Text", "test error");
        var result = cookiePage.OnPost();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Cookies_OnPost_Model_Valid_Test()
    {
        Cookies cookiePage = new();
        var result = cookiePage.OnPost();
        result.Should().BeOfType<RedirectToPageResult>();
    }
}
