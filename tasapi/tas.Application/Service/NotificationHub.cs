using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut;
using tas.Application.Features.NotificationFeature.NotificationGetDetail;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;

namespace tas.Application.Service
{
  [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IMediator _mediator;

                private readonly static ConnectionMapping<string> _connections = 
            new ConnectionMapping<string>();
        public NotificationHub(HTTPUserRepository hTTPUserRepository, IMediator mediator, IAuthenticationRepository authenticationRepository, IHubContext<NotificationHub> hubContext)
        {
            _hTTPUserRepository = hTTPUserRepository;
            _mediator = mediator;   
            _authenticationRepository = authenticationRepository;
            _hubContext = hubContext;
        }

        public void JoinGroup(string? group)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, group);
        }


        public override  Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            string name = Context.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnectedAsync(ex);
        }

        private async Task<EmployeeNoticationShortcutResponse> GetUserNotif(int empId)
        {
            var aa = _connections.Count;
            var response =await _mediator.Send(new EmployeeNoticationShortcutRequest(empId));
            return response;
        }



        public async Task newMessage(int userId)
        {
            string name = Context.User.Identity.Name;
            var data = _authenticationRepository.LoginUserMiddleware(new LoginUserRequest(Context.User.Identity.Name)).Result;
            if (data != null)
            {
                var returnData = await GetUserNotif(data.Id);
                JoinGroup(data.Role);
                await Clients.Clients(Context.ConnectionId).SendAsync("SendMessage", returnData);
            }
        }

        public async Task UserNotification(string name)
        {
            var data = _authenticationRepository.LoginUserMiddleware(new LoginUserRequest(name)).Result;
            var returnData = await GetUserNotif(data.Id);

            var currentConnection   =  _connections.GetConnections(name);
            if (currentConnection != null)
            {
                foreach (var item in currentConnection)
                {
                    await _hubContext.Clients.Clients(item).SendAsync("SendMessage", returnData);
                }

            }
        }
            



        public Task BroadCastAllUsersUpdateNotification()
        {
            return _hubContext.Clients.All.SendAsync("socketUpdate", "true");
        }

        public async Task SendNotificationAccommodtionTeam()
        { 

        }
    }
}
