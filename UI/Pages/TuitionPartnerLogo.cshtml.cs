using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages;

[ResponseCache(Duration = 60*60*24*7, Location = ResponseCacheLocation.Any)]
public class TuitionPartnerLogo : PageModel
{
    private readonly INtpDbContext ntpDb;

    public TuitionPartnerLogo(INtpDbContext ntpDb) => this.ntpDb = ntpDb;

    public async Task<FileContentResult> OnGet(string id)
    {
        var logo = await ntpDb.TuitionPartners
            .Where(x => x.SeoUrl == id)
            .Select(x => x.Logo)
            .FirstOrDefaultAsync()
            ?? throw new ArgumentException("No logo found", nameof(id));

        var bytes = Convert.FromBase64String(logo);
        return new FileContentResult(bytes, "image/svg+xml");
    }
}