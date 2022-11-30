using Application;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Search;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Extensions;
using UI.Models;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace UI.Pages;
public class ShortlistModel : PageModel
{
    private readonly IMediator _mediator;

    public ShortlistModel(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();

    public async Task OnGet(Query data)
    {
        data.From = Enums.ReferrerList.Shortlist;
        Data = await _mediator.Send(data);

        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    public record Query : SearchModel, IRequest<ResultsModel>;

    public record ResultsModel : SearchModel
    {
        public ResultsModel() { }
        public ResultsModel(SearchModel query) : base(query) { }

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a valid postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));
        }
    }

    public class Handler : IRequestHandler<Query, ResultsModel>
    {
        private readonly ILocationFilterService _locationService;
        private readonly ITuitionPartnerService _tuitionPartnerService;
        private readonly INtpDbContext _db;
        private readonly ILogger<TuitionPartner> _logger;

        public Handler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService, INtpDbContext db, ILogger<TuitionPartner> logger)
        {
            _locationService = locationService;
            _tuitionPartnerService = tuitionPartnerService;
            _db = db;
            _logger = logger;
        }

        public async Task<ResultsModel> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryResponse = new ResultsModel(request);

            var searchResults = await GetSearchResults(GetShortlistSeoUrls(), request, cancellationToken);

            return searchResults switch
            {
                SuccessResult => queryResponse with
                {
                    Results = searchResults.Data,
                },
                Domain.ValidationResult error => queryResponse with
                {
                    Validation = new FluentValidationResult(error.Failures)
                },
                ErrorResult error => queryResponse with
                {
                    Validation = new FluentValidationResult(new[]
                    {
                        new ValidationFailure("Postcode", error.ToString()),
                    }),
                },
                _ => queryResponse with { Validation = UnknownError() },
            };

            static FluentValidationResult UnknownError() =>
                new(new[] { new ValidationFailure("", "An unknown problem occurred") });
        }

        private string[] GetShortlistSeoUrls()
        {
            //TODO - integrate with shortlist selection
            var tuitionPartnersIds = new string[] { "em-tuition", "learning-hive", "equal-education", "m-2-r-education", "tutors-green" };

            //TODO - Validation that the postcode passed in via query and the cookie saved shortlist TPs are from same LAD
            //  - can also check that a count of the distinct TP Ids with Any TuitionTypes (from GetTuitionPartnersAsync - before filter) matches the number of TP listed in cookie

            return tuitionPartnersIds;
        }

        private async Task<IResult<TuitionPartnersResult>> GetSearchResults(string[] tuitionPartnerSeoUrls, Query request, CancellationToken cancellationToken)
        {
            var location = await GetSearchLocation(request, cancellationToken);

            if (location is IErrorResult error)
            {
                //Shouldn't be invalid, unless query string edited - since postcode on this page comes from previous page with validation
                _logger.LogWarning("Invalid postcode '{Postcode}' provided on shortlist page", request.Postcode);
                return error.Cast<TuitionPartnersResult>();
            }
            else if (location.Data.LocalAuthorityDistrictId is null)
            {
                _logger.LogError("Unable to get LocalAuthorityDistrictId for supplied postcode '{Postcode}' on TP shortlist page", request.Postcode);
                return Result.Error<TuitionPartnersResult>("Unable to get LocalAuthorityDistrictId for supplied postcode");
            }

            var results = await FindTuitionPartners(
                        tuitionPartnerSeoUrls,
                        location.Data,
                        cancellationToken);

            var result = new TuitionPartnersResult(results, location.Data.LocalAuthority);

            return Result.Success(result);
        }

        private async Task<IResult<LocationFilterParameters>> GetSearchLocation(Query request, CancellationToken cancellationToken)
        {
            var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Postcode))
                return Result.Success(new LocationFilterParameters { });

            if (!validationResults.IsValid)
            {
                return Result.Invalid<LocationFilterParameters>(validationResults.Errors);
            }
            else
            {
                return (await _locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
            }
        }

        private async Task<IEnumerable<TuitionPartnerResult>> FindTuitionPartners(
            string[] tuitionPartnerSeoUrls,
            LocationFilterParameters parameters,
            CancellationToken cancellationToken)
        {

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                SeoUrls = tuitionPartnerSeoUrls
            }, cancellationToken);

            var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
            {
                TuitionPartnerIds = tuitionPartnersIds,
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                Urn = parameters?.Urn
            }, cancellationToken);

            tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering());

            return tuitionPartners;
        }
    }
}