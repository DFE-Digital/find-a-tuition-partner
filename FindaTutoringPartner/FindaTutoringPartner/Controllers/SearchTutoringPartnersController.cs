using MediatR;
using UI.Handlers.SearchTutoringPartners;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class SearchTutoringPartnersController : Controller
{
    private readonly ISender _sender;

    public SearchTutoringPartnersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        return RedirectToAction("School");
    }

    [HttpGet]
    public async Task<IActionResult> School(School.Query query)
    {
        var command = await _sender.Send(query);

        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> School(School.Command command)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("Subjects");
    }

    [HttpGet]
    public async Task<IActionResult> Subjects()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subjects(Subjects.Command command)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("TutorTypes");
    }

    [HttpGet]
    public async Task<IActionResult> TutorTypes()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TutorTypes(TutorTypes.Command command)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("Sessions");
    }

    [HttpGet]
    public async Task<IActionResult> Sessions()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sessions(Sessions.Command command)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("Results");
    }

    [HttpGet]
    public async Task<IActionResult> Results()
    {
        return View();
    }
}