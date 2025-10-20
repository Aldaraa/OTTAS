using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysResponseTimeFeature.CreateSysResponseTime;
using tas.Application.Features.SysResponseTimeFeature.DeleteSysResponseTime;
using tas.Domain.Entities;

namespace tas.Application.Service
{
    public class  RequestResponseTimeMiddleware  : IMiddleware
    {
       // private readonly RequestDelegate _next;
        private readonly IMediator _mediator;
        public RequestResponseTimeMiddleware(IMediator mediator)
        {
            _mediator= mediator;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        {
            try
            {
                if (context.Request.Path.Value.StartsWith("/api"))
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await _next(context);

                    stopwatch.Stop();

                    var responseTime = stopwatch.ElapsedMilliseconds;
                    await _mediator.Send(new CreateSysResponseTimeRequest(Convert.ToInt32(responseTime), context.Request.Path));
                    await _mediator.Send(new DeleteSysResponseTimeRequest());
                }
                else
                {
                    await _next(context);
                    return;
                }
            }
            catch (Exception)
            {
               await _next(context);
                return;
            }
            


        }
    }
}
