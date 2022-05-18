using Application;
using Application.Exceptions;
using Application.Handlers;
using Domain.Search;
using Mapster;
using MediatR;
using UI.Handlers.SearchTutoringPartners;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using Application.Repositories;

namespace UI.Controllers;

public class SearchTutoringPartnersController : Controller
{
    private readonly ISender _sender;
    private readonly ISearchRequestBuilderRepository _searchRequestBuilderRepository;
    private readonly ILookupDataRepository _lookupDataRepository;

    public SearchTutoringPartnersController(ISender sender, ISearchRequestBuilderRepository searchRequestBuilderRepository, ILookupDataRepository lookupDataRepository)
    {
        _sender = sender;
        _searchRequestBuilderRepository = searchRequestBuilderRepository;
        _lookupDataRepository = lookupDataRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        var builder = await _searchRequestBuilderRepository.CreateAsync();

        return RedirectToAction("Location", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> Location(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<LocationSearchViewModel>();
        viewModel.Postcode = viewModel.SearchState?.LocationFilterParameters?.Postcode;

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Location(LocationSearchViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        if (!ModelState.IsValid)
        {
            builder.Adapt(viewModel);
            return View(viewModel);
        }

        try
        {
            await builder.WithPostcode(viewModel.Postcode);
        }
        catch (LocationNotFoundException)
        {
            ModelState.AddModelError("Postcode", "Enter a valid postcode");
            builder.Adapt(viewModel);
            return View(viewModel);
        }

        return RedirectToAction("Subjects", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> School(School.Query query)
    {
        var command = await _sender.Send(query);

        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> School([FromBody] School.Command command)
    {
        if (!ModelState.IsValid)
        {
            command = await _sender.Send(new School.HydrateCommand(command));
            return View(command);
        }

        command = await _sender.Send(command);
        if (command.IsComplete)
        {
            return RedirectToAction("Subjects");
        }

        // Post Redirect Get
        return RedirectToAction("School", command.Adapt<School.Query>());
    }

    [HttpGet]
    public async Task<IActionResult> Subjects(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<SubjectSearchViewModel>();
        viewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subjects(SubjectSearchViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        if (!ModelState.IsValid)
        {
            builder.Adapt(viewModel);
            viewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();
            return View(viewModel);
        }

        //await builder.WithSubjectIds(viewModel.SubjectIds);

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
        var result = await _sender.Send(new SearchTuitionPartnerHandler.Command{PageSize = SearchRequestBase.MaxPageSize});
        var viewModel = new TuitionPartnerSearchResultsViewModel
        {
            SearchResultsPage = result,
            Subjects = await _lookupDataRepository.GetSubjectsAsync()
        };

        return View(viewModel);
    }
}