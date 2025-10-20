using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using System;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Features.BedFeature.DeleteBed;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
//using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.UpdateBed;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class BedController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BedController> _logger;
        private readonly HTTPUserRepository _userRepository;

        public BedController(IMediator mediator, ILogger<BedController> logger, HTTPUserRepository userRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _userRepository = userRepository;   
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetBedResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetBedRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllBedResponse>>> GetAll(int? active, int roomId, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllBedRequest(null, roomId), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllBedRequest(active, roomId), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateBedRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _userRepository.ClearAllMasterCache<Room>();


            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateBedRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteBedRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _userRepository.ClearAllMasterCache<Room>();
            return Ok(response);
        }

    }
}
