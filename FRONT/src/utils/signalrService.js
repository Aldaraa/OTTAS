// //signalrService.js

// import * as signalR from "@microsoft/signalr";

// const broadcastChannel = new BroadcastChannel("signalr_channel");

// let connection = null;
// let subscribedGroups = new Set(); // Keep track of subscribed groups
// let messageListeners = []; // Callbacks for message handling

// const createConnection = async () => {
//   if (!connection) {
//     connection = new signalR.HubConnectionBuilder()
//       .withUrl(`${process.env.REACT_APP_CDN_URL}/screenhub`, {
//         skipNegotiation: false,
//       })
//       .withAutomaticReconnect()
//       .build();

//     try {
//       await connection.start();
//       console.log("SignalR connection established.");
//       broadcastChannel.postMessage({ type: "CONNECTED" });
//     } catch (error) {
//       console.error("Error establishing SignalR connection:", error);
//     }

//     // Listen for messages from the server
//     connection.on("UsersInScreen", (screenName, message) => {
//       console.log(`Message received from room ${screenName}:`, message, broadcastChannel);
//       // Broadcast the message to all tabs/components
//       broadcastChannel.postMessage({
//         type: "MESSAGE_RECEIVED",
//         payload: { screenName, message },
//       });
//     });
//   }
// };

// const waitForConnection = (connection) => {
//   return new Promise((resolve, reject) => {
//     const checkInterval = 100; // Interval to check if the connection is ready
//     const maxWaitTime = 5000; // Maximum time to wait (5 seconds)
//     let waitedTime = 0;

//     const checkConnection = () => {
//       if (connection.state === signalR.HubConnectionState.Connected) {
//         resolve();
//       } else if (waitedTime < maxWaitTime) {
//         waitedTime += checkInterval;
//         setTimeout(checkConnection, checkInterval);
//       } else {
//         reject(new Error("SignalR connection failed to connect within the timeout period."));
//       }
//     };

//     checkConnection();
//   });
// };

// // Subscribe to a SignalR group (room)
// const joinGroup = async (groupName, screenName) => {
//   if (!subscribedGroups.has(screenName)) {
//     console.log('joining', groupName, screenName, subscribedGroups, connection);
//     await waitForConnection(connection)
//     connection.invoke("JoinScreen", groupName, screenName).then(() => {
//       subscribedGroups.add(screenName);
//       console.log(`Joined group: ${screenName}`);
  
//       // Notify other tabs about the group subscription
//       broadcastChannel.postMessage({
//         type: "GROUP_SUBSCRIBED",
//         payload: screenName,
//       });
//     });
//   }
// };

// // Unsubscribe from a SignalR group (room)
// const leaveGroup = async (groupName, screenName) => {
//   if (subscribedGroups.has(screenName)) {
//     await waitForConnection(connection)
//     await connection.invoke("LeaveScreen",groupName , screenName);
//     subscribedGroups.delete(screenName);
//     console.log(`Left group: ${screenName}`);

//     // Notify other tabs about the group unsubscription
//     broadcastChannel.postMessage({
//       type: "GROUP_UNSUBSCRIBED",
//       payload: screenName,
//     });
//   }
// };

// export const startSignalRConnection = () => {
//   let connectionChecked = false;

//   broadcastChannel.onmessage = async (event) => {
//     const { type, payload } = event.data;

//     switch (type) {
//       case "CONNECTED":
//         console.log("Another tab has an active SignalR connection.");
//         connectionChecked = true;
//         break;

//       case "CHECK_CONNECTION":
//         broadcastChannel.postMessage({
//           type: connection ? "CONNECTED" : "DISCONNECTED",
//         });
//         break;

//       case "GROUP_SUBSCRIBED":
//         // Keep track of groups subscribed by other tabs
//         subscribedGroups.add(payload);
//         break;

//       case "GROUP_UNSUBSCRIBED":
//         subscribedGroups.delete(payload);
//         break;

//       case "MESSAGE_RECEIVED":
//         // Notify all registered listeners
//         console.log('message listener', messageListeners);
//         messageListeners.forEach((listener) => {
//           if (listener.screenName === payload.screenName) {
//             listener.callback(payload.message);
//           }
//         });
//         break;

//       default:
//         break;
//     }
//   };

//   // Check if a SignalR connection exists in another tab
//   const checkConnection = () => {
//     if (!connectionChecked) {
//       broadcastChannel.postMessage({ type: "CHECK_CONNECTION" });
//       setTimeout(() => {
//         if (!connectionChecked) {
//           createConnection(); // No connection exists, create one
//         }
//       }, 1000);
//     }
//   };

//   checkConnection();
// };

// export const stopSignalRConnection = () => {
//   if (connection) {
//     connection.stop();
//     broadcastChannel.postMessage({ type: "DISCONNECTED" });
//     console.log("SignalR connection stopped.");
//   }
// };

// export const subscribeToGroup = async (groupName, screenName, callback) => {
//   console.log('subscribeToGroup', groupName, screenName, callback);
//   // Register the listener
//   messageListeners.push({ screenName, callback });
  
//   if (connection) {
//     await joinGroup(groupName, screenName);
//   } else {
//     // If the connection isn't ready yet, wait until it is
//     const interval = setInterval(() => {
//       if (connection) {
//         joinGroup(groupName, screenName);
//         clearInterval(interval);
//       }
//     }, 100);
//   }
// };

// export const unsubscribeFromGroup = async (groupName, screenName, callback) => {
//   // Remove the listener
//   messageListeners = messageListeners.filter(
//     (listener) =>
//       listener.screenName !== screenName || listener.callback !== callback
//   );

//   // Check if any listeners are still subscribed to this group
//   const isGroupStillSubscribed = messageListeners.some(
//     (listener) => listener.screenName === screenName
//   );

//   if (!isGroupStillSubscribed && connection) {
//     await leaveGroup(groupName, screenName);
//   }
// };

// export const sendMessageToGroup = async (groupName, screenName) => {
//   if (connection) {
//     await connection.invoke("JoinScreen", groupName, screenName);
//   }
// };

// ------------- PROFILE ------------
//signalrService.js

import * as signalR from "@microsoft/signalr";

const broadcastChannel = new BroadcastChannel("signalr_channel");

let connection = null;
let subscribedGroups = new Set(); // Keep track of subscribed groups
let messageListeners = []; // Callbacks for message handling

const createConnection = async (callback) => {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_CDN_URL}/screenhub`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    try {
      await connection.start();
      console.log("SignalR connection established.");
      broadcastChannel.postMessage({ type: "CONNECTED" });
    } catch (error) {
      console.error("Error establishing SignalR connection:", error);
    }

    // Listen for messages from the server
    connection.on("UsersInScreen", (screenName, message) => {
      // Broadcast the message to all tabs/components
      if(callback){
        callback(message)
      }
      // broadcastChannel.postMessage({
      //   type: "MESSAGE_RECEIVED",
      //   payload: { screenName, message },
      // });
    });
  }
};

const waitForConnection = (connection) => {
  return new Promise((resolve, reject) => {
    const checkInterval = 100; // Interval to check if the connection is ready
    const maxWaitTime = 5000; // Maximum time to wait (5 seconds)
    let waitedTime = 0;

    const checkConnection = () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        resolve();
      } else if (waitedTime < maxWaitTime) {
        waitedTime += checkInterval;
        setTimeout(checkConnection, checkInterval);
      } else {
        reject(new Error("SignalR connection failed to connect within the timeout period."));
      }
    };

    checkConnection();
  });
};

// Subscribe to a SignalR group (room)
const joinGroup = async (groupName, screenName) => {
  if (!subscribedGroups.has(screenName)) {
    console.log('joining', groupName, screenName, subscribedGroups, connection);
    await waitForConnection(connection)
    connection.invoke("JoinScreen", groupName, screenName).then(() => {
      subscribedGroups.add(screenName);
      console.log(`Joined group: ${screenName}`);
  
      // Notify other tabs about the group subscription
      broadcastChannel.postMessage({
        type: "GROUP_SUBSCRIBED",
        payload: screenName,
      });
    });
  }
};

// Unsubscribe from a SignalR group (room)
const leaveGroup = async (groupName, screenName) => {
  if (subscribedGroups.has(screenName)) {
    await waitForConnection(connection)
    await connection.invoke("LeaveScreen",groupName , screenName);
    subscribedGroups.delete(screenName);
    console.log(`Left group: ${screenName}`);

    // Notify other tabs about the group unsubscription
    broadcastChannel.postMessage({
      type: "GROUP_UNSUBSCRIBED",
      payload: screenName,
    });
  }
};

export const startSignalRConnection = (callback) => {
  let connectionChecked = false;

  broadcastChannel.onmessage = async (event) => {
    console.log('channel message', event);
    const { type, payload } = event.data;

    switch (type) {
      case "CONNECTED":
        console.log("Another tab has an active SignalR connection.");
        connectionChecked = true;
        break;

      case "CHECK_CONNECTION":
        broadcastChannel.postMessage({
          type: connection ? "CONNECTED" : "DISCONNECTED",
        });
        break;

      case "GROUP_SUBSCRIBED":
        // Keep track of groups subscribed by other tabs
        subscribedGroups.add(payload);
        break;

      case "GROUP_UNSUBSCRIBED":
        subscribedGroups.delete(payload);
        break;

      case "MESSAGE_RECEIVED":
        // Notify all registered listeners
        console.log('message listener', messageListeners);
        messageListeners.forEach((listener) => {
          if (listener.screenName === payload.screenName) {
            listener.callback(payload.message);
          }
        });
        break;

      default:
        break;
    }
  };

  // Check if a SignalR connection exists in another tab
  const checkConnection = (callback) => {
    if (!connectionChecked) {
      broadcastChannel.postMessage({ type: "CHECK_CONNECTION" });
      setTimeout(() => {
        if (!connectionChecked) {
          createConnection(callback); // No connection exists, create one
        }
      }, 1000);
    }
  };

  checkConnection(callback);
};

export const stopSignalRConnection = () => {
  if (connection) {
    connection.stop();
    broadcastChannel.postMessage({ type: "DISCONNECTED" });
    console.log("SignalR connection stopped.");
  }
};

export const subscribeToGroup = async (groupName, screenName, callback) => {
  console.log('subscribeToGroup', groupName, screenName, callback);
  messageListeners.push({ screenName, callback });
  // Register the listener
  
  try {
    if(!connection){
      await createConnection(callback)
    }
    await waitForConnection(connection);
    await joinGroup(groupName, screenName);
  } catch (err){
    // If the connection isn't ready yet, wait until it is
    console.log('error joing', err);
  }
};

export const unsubscribeFromGroup = async (groupName, screenName, callback) => {
  // Remove the listener
  messageListeners = messageListeners.filter(
    (listener) =>
      listener.screenName !== screenName || listener.callback !== callback
  );

  // Check if any listeners are still subscribed to this group
  const isGroupStillSubscribed = messageListeners.some(
    (listener) => listener.screenName === screenName
  );

  if (!isGroupStillSubscribed && connection) {
    try {
      if(!connection){
        await createConnection(callback)
      }
      await waitForConnection(connection)
      await leaveGroup(groupName, screenName);
    } catch (err) {
      console.log('error leaving', err);
    }
  }
};

export const sendMessageToGroup = async (groupName, screenName) => {
  if (connection) {
    await connection.invoke("JoinScreen", groupName, screenName);
  }
};
