using Bmg.API.Converters.Clients;
using Bmg.Application.Clients.UseCases.Delete;
using Bmg.Application.Clients.UseCases.Upsert;
using Bmg.BuildingBlocks.Web.API.Controllers;
using Bmg.BuildingBlocks.Web.API.Validators;
using Bmg.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bmg.API.Controllers
{
    [ApiController]
    [Route("clients")]
    public class ClientController : ApiBaseController
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IMediator mediator, RequestStateValidator requestStateValidator, IClientRepository clientRepository) : base(mediator, requestStateValidator)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GetAll()
        {
            var clients = await _clientRepository.GetAllAsync();

            if (clients == null || !clients.Any())
            {
                return NoContent();
            }

            var clientDto = ClientConverter.Converter(clients);

            return Ok(clientDto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Create([FromBody] UpsertClientRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Update([FromBody] UpsertClientRequest request, [FromRoute] int id)
        {
            request.Id = id;

            return await Dispatch(request);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteClientRequest
            {
                Id = id
            };

            return await Dispatch(request);
        }
    }
}
