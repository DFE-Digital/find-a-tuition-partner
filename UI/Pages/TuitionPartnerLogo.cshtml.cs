using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages;

[ResponseCache(Duration = 60 * 60 * 24 * 7, Location = ResponseCacheLocation.Any)]
public class TuitionPartnerLogo : PageModel
{
    private readonly INtpDbContext ntpDb;

    public TuitionPartnerLogo(INtpDbContext ntpDb) => this.ntpDb = ntpDb;

    public async Task<IActionResult> OnGet(string id)
    {
        var logo = await ntpDb.TuitionPartners
            .Include(x => x.Logo)
            .Where(x => x.SeoUrl == id)
            .Select(x => x.Logo)
            .FirstOrDefaultAsync();

        return logo?.Logo is null
            ? NotFound()
            : FileContent(logo);
    }

    private static IActionResult FileContent(Domain.TuitionPartnerLogo logo)
        => new FileContentResult(
            Convert.FromBase64String(logo.Logo),
            MimeTypeFor.Extension(logo.FileExtension));
}