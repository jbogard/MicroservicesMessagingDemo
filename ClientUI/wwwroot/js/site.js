// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/submissionHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("receiveMessage", message => {
    toastr.info(message);
});

connection.start().catch(err => console.error(err.toString()));


const startSignalRConnection = connection => connection.start()
    .then(() => console.info('Websocket Connection Established'))
    .catch(err => console.error('SignalR Connection Error: ', err));

connection.onclose(() => setTimeout(startSignalRConnection(connection), 5000));
