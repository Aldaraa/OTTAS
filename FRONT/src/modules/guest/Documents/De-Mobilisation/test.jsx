import React, { useState } from 'react';
import * as signalR from '@microsoft/signalr';

const ChatComponent = () => {
    const [hubConnection, setHubConnection] = useState(null);
    const [screenName, setScreenName] = useState('');
    const [userName, setUserName] = useState('JohnDoe');

    // Initialize SignalR connection
    const initSignalR = async () => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/signalrHub')
            .build();

        connection.on('ReceiveMessage', (message) => {

        });

        connection.on('UsersInScreen', (screenName, users) => {
            // console.log(Users in ${screenName}:, users);
        });

        await connection.start();
        setHubConnection(connection);
    };

    const changeSubscription = async (newScreenName) => {
        if (hubConnection && screenName !== newScreenName) {
            await hubConnection.invoke('ChangeSubscription', screenName, newScreenName, userName);
            setScreenName(newScreenName);
        }
    };

    const subscribeToScreen = async () => {
        if (hubConnection && screenName) {
            await hubConnection.invoke('SubscribeToScreen', screenName, userName);
        }
    };

    const unsubscribeFromScreen = async () => {
        if (hubConnection && screenName) {
            await hubConnection.invoke('UnsubscribeFromScreen', screenName, userName);
        }
    };

    return (
        <div>
            <button onClick={initSignalR}>Connect</button>
            <input 
                type="text" 
                placeholder="Screen Name" 
                value={screenName} 
                onChange={(e) => setScreenName(e.target.value)} 
            />
            <button onClick={() => changeSubscription('NewScreen')}>Change Subscription</button>
            <button onClick={subscribeToScreen}>Subscribe to Screen</button>
            <button onClick={unsubscribeFromScreen}>Unsubscribe from Screen</button>
        </div>
    );
};

export default ChatComponent;