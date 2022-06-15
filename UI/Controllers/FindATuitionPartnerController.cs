using Application.Exceptions;
using Application.Handlers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using Application.Repositories;

namespace UI.Controllers;

[Route("[controller]/[action]")]
public class FindATuitionPartnerController : Controller
{
    private readonly ISender _sender;
    private readonly ISearchRequestBuilderRepository _searchRequestBuilderRepository;
    private readonly ILookupDataRepository _lookupDataRepository;

    public FindATuitionPartnerController(ISender sender, ISearchRequestBuilderRepository searchRequestBuilderRepository, ILookupDataRepository lookupDataRepository)
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
        catch (LocationNotAvailableException)
        {
            ModelState.AddModelError("Postcode", "This service covers England only");
            builder.Adapt(viewModel);
            return View(viewModel);
        }
        catch (LocationNotMappedException)
        {
            ModelState.AddModelError("Postcode", "Could not identify Local Authority for the supplies postcode");
            builder.Adapt(viewModel);
            return View(viewModel);
        }

        return RedirectToAction("Subjects", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> Subjects(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<SubjectsSearchViewModel>();
        viewModel.SubjectIds = viewModel.SearchState?.Subjects?.Keys;
        viewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subjects(SubjectsSearchViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        if (!ModelState.IsValid)
        {
            builder.Adapt(viewModel);
            viewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();
            return View(viewModel);
        }

        await builder.WithSubjectIds(viewModel.SubjectIds!);

        return RedirectToAction("Results", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> TutorTypes(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<TutorTypesSearchViewModel>();
        viewModel.TutorTypeIds = viewModel.SearchState?.TutorTypes?.Keys;
        viewModel.TutorTypes = await _lookupDataRepository.GetTutorTypesAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TutorTypes(TutorTypesSearchViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        if (!ModelState.IsValid)
        {
            builder.Adapt(viewModel);
            viewModel.TutorTypes = await _lookupDataRepository.GetTutorTypesAsync();
            return View(viewModel);
        }

        await builder.WithTutorTypeIds(viewModel.TutorTypeIds!);

        return RedirectToAction("TuitionTypes", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> TuitionTypes(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<TuitionTypeSearchViewModel>();
        viewModel.TuitionTypeId = viewModel.SearchState?.TuitionType;
        viewModel.TuitionTypes = await _lookupDataRepository.GetTuitionTypesAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuitionTypes(TuitionTypeSearchViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        if (!ModelState.IsValid)
        {
            builder.Adapt(viewModel);
            viewModel.TuitionTypes = await _lookupDataRepository.GetTuitionTypesAsync();
            return View(viewModel);
        }

        await builder.WithTuitionTypeId(viewModel.TuitionTypeId!);

        return RedirectToAction("Results", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> Results(Guid searchId)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var viewModel = builder.Adapt<TuitionPartnerSearchResultsViewModel>();
        viewModel.LocationSearchViewModel = builder.Adapt<LocationSearchViewModel>();
        viewModel.LocationSearchViewModel.Postcode = viewModel.SearchState?.LocationFilterParameters?.Postcode;
        viewModel.SubjectsSearchViewModel = builder.Adapt<SubjectsSearchViewModel>();
        viewModel.SubjectsSearchViewModel.SubjectIds = viewModel.SearchState?.Subjects?.Keys;
        viewModel.SubjectsSearchViewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();
        viewModel.TuitionTypeSearchViewModel = builder.Adapt<TuitionTypeSearchViewModel>();
        viewModel.TuitionTypeSearchViewModel.TuitionTypeId = viewModel.SearchState?.TuitionType;
        viewModel.TuitionTypeSearchViewModel.TuitionTypes = await _lookupDataRepository.GetTuitionTypesAsync();
        viewModel.SearchResultsPage = await _sender.Send(builder.Build().Adapt<SearchTuitionPartnerHandler.Command>());

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Results(TuitionPartnerSearchResultsViewModel viewModel)
    {
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(viewModel.SearchId);

        try
        {
            if (!string.IsNullOrWhiteSpace(viewModel.LocationSearchViewModel.Postcode))
            {
                await builder.WithPostcode(viewModel.LocationSearchViewModel.Postcode);
            }
        }
        catch (LocationNotFoundException)
        {
            ModelState.AddModelError("LocationSearchViewModel.Postcode", "Enter a valid postcode");
        }
        catch (LocationNotAvailableException)
        {
            ModelState.AddModelError("LocationSearchViewModel.Postcode", "This service covers England only");
        }
        catch (LocationNotMappedException)
        {
            ModelState.AddModelError("LocationSearchViewModel.Postcode", "Could not identify Local Authority for the supplies postcode");
        }

        if (!ModelState.IsValid)
        {
            viewModel.SubjectsSearchViewModel.Subjects = await _lookupDataRepository.GetSubjectsAsync();
            viewModel.TuitionTypeSearchViewModel.TuitionTypes = await _lookupDataRepository.GetTuitionTypesAsync();
            viewModel.SearchResultsPage = await _sender.Send(builder.Build().Adapt<SearchTuitionPartnerHandler.Command>());
            return View(viewModel);
        }

        await builder.WithSubjectIds(viewModel.SubjectsSearchViewModel.SubjectIds!);
        await builder.WithTuitionTypeId(viewModel.TuitionTypeSearchViewModel.TuitionTypeId!);

        return RedirectToAction("Results", new { builder.SearchState.SearchId });
    }

    [HttpGet]
    public async Task<IActionResult> TuitionPartner(int id, Guid searchId)
    {
        //TODO: This all needs replacing with a proper view model based on the populated TuitionPartner class and pulled from a repository or similar
        var builder = await _searchRequestBuilderRepository.RetrieveAsync(searchId);

        var searchResultsPage = await _sender.Send(builder.Build().Adapt<SearchTuitionPartnerHandler.Command>());

        var searchResult = searchResultsPage.Results.Single(e => e.Id == id);

        var viewModel = searchResult.Adapt<TuitionPartnerViewModel>();
        viewModel.SearchId = searchId;

        return View(viewModel);
    }
}