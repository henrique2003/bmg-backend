using Bmg.API.Dtos;
using Bmg.BuildingBlocks.Domain.Models;
using Bmg.BuildingBlocks.Web.API.Patterns.Envelop;
using Bmg.BuildingBlocks.Web.API.Validators;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bmg.BuildingBlocks.Web.API.Controllers
{
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RequestStateValidator _requestStateValidator;

        protected ApiBaseController(IMediator mediator, RequestStateValidator requestStateValidator)
        {
            _mediator = mediator;
            _requestStateValidator = requestStateValidator;
        }

        protected static IActionResult OkFromEnvolpe(object? result)
        {
            return new EnvelopeResult(Envelope.Ok(result), result is null ? HttpStatusCode.NoContent : HttpStatusCode.OK);
        }

        protected static IActionResult NotFound(Error error)
        {
            return new EnvelopeResult(Envelope.Error(error), HttpStatusCode.NotFound);
        }

        protected static IActionResult ServerError(Error error)
        {
            return new EnvelopeResult(Envelope.Error(error), HttpStatusCode.InternalServerError);
        }

        protected static IActionResult Error(Error error)
        {
            return new EnvelopeResult(Envelope.Error(error), HttpStatusCode.BadRequest);
        }

        protected IActionResult InvalidRequest(string message)
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { message }
            });
        }

        protected static IActionResult FromResult<T>(IResult<T, Error> result)
        {
            if (result.IsSuccess)
            {
                return OkFromEnvolpe(result.Value);
            }

            return result.Error == Errors.Http.NotFound() ? NotFound(result.Error) : Error(result.Error);
        }

        protected IActionResult FromResult(Result result)
        {
            return result.IsFailure ? Error(Errors.General.Business(result.Error)) : NoContent();
        }

        protected static IActionResult FromResult<T>(CSharpFunctionalExtensions.Result<T> result)
        {
            return result.IsFailure ? Error(Errors.General.Business(result.Error)) : OkFromEnvolpe(result.Value);
        }

        protected async ValueTask<IActionResult> Dispatch(IRequest<Result> request)
        {
            var result = await _mediator.Send(request);

            return _requestStateValidator.IsValid()
                ? FromResult(result)
                : new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
        }

        protected async ValueTask<IActionResult> Dispatch<T>(IRequest<CSharpFunctionalExtensions.Result<T>> request)
        {
            var result = await _mediator.Send(request);

            return _requestStateValidator.IsValid()
                ? FromResult(result)
                : (IActionResult) new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
        }

        protected async ValueTask<IActionResult> Dispatch(IRequest<IResult<Unit, Error>> request)
        {
            var result = await _mediator.Send(request);

            if (_requestStateValidator.IsValid() && result.IsSuccess)
            {
                return FromResult(result);
            }

            if (!_requestStateValidator.IsValid())
            {
                return  new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
            }

            if (result.Error.Code == Errors.Http.Duplicated().Code)
            {
                return  new EnvelopeResult(Envelope.Error(result.Error), HttpStatusCode.Conflict);
            }

            return  new EnvelopeResult(Envelope.Error(result.Error), HttpStatusCode.BadRequest);
        }

        protected static IActionResult NotAcceptable()
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.NotAcceptable()), HttpStatusCode.NotAcceptable);
        }

        protected static IActionResult EmptyRequestBody()
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.EmptyRequestBody()), HttpStatusCode.BadRequest);
        }

        protected static IActionResult CreatedResult<TId>(TId id)
        {
            return new EnvelopeResult(Envelope.Ok(id), HttpStatusCode.Created);
        }

        protected static IActionResult BadRequestFromErrorMessage(string errorMessage)
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.BadRequest(errorMessage)), HttpStatusCode.BadRequest);
        }

        protected async ValueTask<IActionResult> DispatchAsync<TResponse>(IRequest<TResponse> request)
        {
            if (request == null)
            {
                return EmptyRequest();
            }

            return Result(await _mediator.Send(request));
        }

        protected IActionResult EmptyRequest()
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { "Informe os dados da requisição" }
            });
        }

        protected IActionResult Result<TResponse>(TResponse result, int statusCodeOnSuccess = StatusCodes.Status200OK)
        {
            if (statusCodeOnSuccess == StatusCodes.Status201Created)
            {
                return Created(string.Empty, result);
            }

            return Ok(result);
        }
    }
}
