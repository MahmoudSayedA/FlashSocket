"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable send button until connection is established
$("#btnSend").prop('disabled', false);

connection.start().then(function () {
    $("#btnSend").prop('disabled', false);
    alert('Connected to chat');

}).catch(function (err) {
    return console.error(err.toString());
});

$("#btnSend").on('click', function (event) {                       
    var user = $("#user").val();
    var message = $("#txtMessage").val();
    var userId = $("#userId").val();

    if (userId === 'group') {
        connection.invoke("SendToMyGroup", user, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else if (userId !== '') {
        connection.invoke("SendPrivateMessage", userId, user, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
    }

    // Clear message input box
    $("#txtMessage").val('');

    // focus on message input box
    $("#txtMessage").prop('focus');

    event.preventDefault();
});

connection.on("ReceiveMessage", function (user, message) {
    var content = `<b>${user}</b>: ${message}`;
    $('#messageList').append('<li>' + content + '</li>');
});
