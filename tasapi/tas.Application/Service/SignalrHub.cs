using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace tas.Application.Service
{

    public class SignalrHub : Hub
    {
        // private static readonly ConcurrentDictionary<string, MultiViewersUser> connectedUsersScreenRoom = new ConcurrentDictionary<string, MultiViewersUser>();

        private static readonly List<MultiViewersUser> connectedUsersScreenRoom = new List<MultiViewersUser>();


        private static readonly List<MultiViewersUser> connectedUsersRoleChangeRoom = new List<MultiViewersUser>();

        private static readonly ConcurrentDictionary<string, int> userConnections = new ConcurrentDictionary<string, int>();

        private readonly IHubContext<SignalrHub> _hubContext;

        private readonly ILogger<SignalrHub> _logger;
        private const int MaxConnectionsPerUser = 1;
        public SignalrHub(IHubContext<SignalrHub> hubContext, ILogger<SignalrHub> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }


        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    connectedUsersRoleChangeRoom.TryRemove(Context.ConnectionId, out var _);
        //    connectedUsersScreenRoom.TryRemove(Context.ConnectionId, out var _);



        //    var userScreens = connectedUsersScreenRoom.Values
        //                                    .Where(u => u.ConnectionId == Context.ConnectionId)
        //                                    .Select(u => u.ScreenName)
        //                                    .Distinct();

        //    foreach (var screenName in userScreens)
        //    {
        //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, screenName);

        //        //  await Clients.Group(screenName).SendAsync("ReceiveMessage", $"{connectedUsersScreenRoom[Context.ConnectionId].UserName} has left the screen {screenName}");
        //        var usersInScreen = connectedUsersScreenRoom.Values.Where(u => u.ScreenName == screenName).ToList();
        //        await _hubContext.Clients.Group(screenName).SendAsync("UsersInScreen", screenName, usersInScreen);
        //    }


        //    await base.OnDisconnectedAsync(exception);
        //}

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var aa = 0;
            //// Check if the user is in a room and remove them if so
            ///

            var disconnectedConnection= connectedUsersScreenRoom.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();

            if (disconnectedConnection != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, disconnectedConnection.ScreenName);

                var usersInScreen = connectedUsersScreenRoom.Where(x => x.ScreenName == disconnectedConnection.ScreenName).ToList();
                await _hubContext.Clients.Group(disconnectedConnection.ScreenName).SendAsync("UsersInScreen", disconnectedConnection.ScreenName, usersInScreen);
            }


            await base.OnDisconnectedAsync(exception);
        }


        //public override async Task OnConnectedAsync()
        //{
        //    string connectionId = Context.ConnectionId;

        //    // Increment the connection count for the connectionId or initialize it if not present
        //    userConnections.AddOrUpdate(connectionId, 1, (key, oldValue) => oldValue + 1);

        //    // Check if the user exceeds the maximum allowed connections
        //    if (userConnections[connectionId] > MaxConnectionsPerUser)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "Too many open tabs. Please close some.");
        //        await DisconnectUserAsync();
        //        return;
        //    }

        //    await base.OnConnectedAsync();
        //}



        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            // Increment the connection count or initialize it if not present
            int connectionCount = userConnections.AddOrUpdate(connectionId, 1, (key, oldValue) => oldValue + 1);

            // Check if the user exceeds the maximum allowed connections
            if (connectionCount > MaxConnectionsPerUser)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Too many open tabs. Please close some.");
                Context.Abort();
                return;
            }

            await base.OnConnectedAsync();
        }
     
        private async Task DisconnectUserAsync()
        {
            Context.Abort();
            await Task.CompletedTask;
        }




        public async Task JoinScreen(string userName, string screenName)
        {

            try
            {
                var chatUser = new MultiViewersUser
                {
                    ConnectionId = Context.ConnectionId,
                    UserName = userName,
                    ScreenName = screenName
                };


                 var oldata =   connectedUsersScreenRoom.Where(x => x.UserName == userName && x.ScreenName == screenName).FirstOrDefault();
                var usersInRoom = connectedUsersScreenRoom.Where(x => x.ScreenName == screenName).ToList();

                if (oldata == null)
                {
                    connectedUsersScreenRoom.Add(chatUser);
                    await Groups.AddToGroupAsync(Context.ConnectionId, screenName);
                    await _hubContext.Clients.Group(screenName).SendAsync("UsersInScreen", screenName, usersInRoom);

                    var mydata = usersInRoom.Where(x => x.UserName != userName).ToList();
                    if (mydata.Count > 0)
                    {
                        // await _hubContext.Clients.Client.SendAsync("UsersInScreen", mydata);
                        await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("UsersInScreen", screenName, mydata);
                    }


                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, screenName);
                    var mydata = usersInRoom.Where(x => x.UserName != userName).ToList();
                    if (mydata.Count > 0)
                    {
                       // await _hubContext.Clients.Client.SendAsync("UsersInScreen", mydata);
                       await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("UsersInScreen", screenName, mydata);
                    }
                    
                }
                
            

            }
            catch (Exception)
            {

            }


        }


        public async Task JoinSystem(string userName)
        {

            try
            {
                var chatUser = new MultiViewersUser
                {
                    ConnectionId = Context.ConnectionId,
                    UserName = userName,
                    ScreenName = "SystemRole"
                };


                connectedUsersRoleChangeRoom.Add(chatUser);

                await _hubContext.Groups.AddToGroupAsync(Context.ConnectionId, "SystemRole");
            }
            catch (Exception)
            {

            }


        }

        public async Task LeaveScreen(string userName, string screenName)
        {
            try
            {

              var removedata =  connectedUsersScreenRoom.Where(x => x.UserName == userName && x.ScreenName == screenName).FirstOrDefault();
                  connectedUsersScreenRoom.Remove(removedata);

                // Update the list of users in the room after the user has left
                var usersInRoom = connectedUsersScreenRoom
                                   .Where(u => u.ScreenName == screenName)
                                   .ToList();

                // Send the updated list or empty array if no users are left
                await _hubContext.Clients.Group(screenName).SendAsync("UsersInScreen", screenName, usersInRoom.Count > 0 ? usersInRoom : Array.Empty<int>());
                await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("UsersInScreen", screenName, null);

            }
            catch (Exception ex)
            {
               // _logger.LogError("Error in LeaveScreen method: {Message}", ex.Message);
            }
        }




        public async Task RoleChange(string? userName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    var usersInRoom = connectedUsersRoleChangeRoom.Where(u => u.ScreenName == "SystemRole" && u.UserName == userName).ToList();
                    foreach (var user in usersInRoom)
                    {

                        await _hubContext.Clients.Client(user.ConnectionId).SendAsync("Rolechanged", "Your account permissions have been updated.");

                        //  await _hubContext.Clients.Group("SystemRole").SendAsync("Rolechanged", "Rolechanged", "test");
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }



        //public async Task RequestRoomNotification(string notificationIndex, string? fullname, string? description, int notificationCount, string? requester, string? currentAction, string? docnumber)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"{fullname} - SENDING user notification.");

        //        if (!string.IsNullOrWhiteSpace(fullname))
        //        {
        //            // Fetch the users in the room only once to reduce dictionary access
        //            var usersInRoom = connectedUsersRequestRoom.Values
        //                                .Where(u => u.ScreenName == "RequestRoom" && u.UserName.StartsWith(fullname))
        //                                .ToList();

        //            // Log the number of users matching the fullname condition
        //            _logger.LogInformation($"{usersInRoom.Count} users matched for notification.");

        //            // Send notifications in parallel
        //            var sendTasks = usersInRoom.Select(user => _hubContext.Clients.Client(user.ConnectionId).SendAsync("RequestAssigned", new
        //            {
        //                notificationIndex,
        //                description,
        //                notifcount = notificationCount,
        //                requester,
        //                currentAction,
        //                docnumber
        //            })).ToList();

        //            await Task.WhenAll(sendTasks);  // Await all tasks at once for parallel execution
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception to capture any errors
        //        _logger.LogError(ex, "An error occurred while sending room notifications.");
        //    //    throw;  // Rethrow the exception to ensure it's not silently swallowed
        //    }
        //}


        //public async Task RequestRoomNotification(string notificationIndex,  string? fullname, string? description, int notificationCount, string? requester, string? currentAction, string? docnumber)
        //{
        //    try
        //    {

        //        _logger.LogInformation(fullname + "----------------------------SENDING user--------------------------------------");
        //        if (!string.IsNullOrWhiteSpace(fullname))
        //        {
        //            var usersInRoom = connectedUsersRequestRoom.Values.Where(u => u.ScreenName == "RequestRoom").ToList();
        //            foreach (var user in usersInRoom)
        //            {
        //              //  _logger.LogInformation(user.UserName + "----------------------------++++++++++++ user--------------------------------------");

        //                if (user.UserName.StartsWith(fullname))
        //                {
        //                 //   _logger.LogInformation(user.UserName + "----------------------------has user--------------------------------------");

        //                    await _hubContext.Clients.Client(user.ConnectionId).SendAsync("RequestAssigned",
        //                        new
        //                        {
        //                            notificationIndex = notificationIndex,
        //                            description = description,
        //                            notifcount = notificationCount,
        //                            requester = requester,
        //                            currentAction = currentAction,
        //                            docnumber = docnumber
        //                        });
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}




    }

    public class MultiViewersUser
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string ScreenName { get; set; }
    }
}
