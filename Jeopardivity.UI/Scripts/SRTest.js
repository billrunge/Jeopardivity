$(document).ready(function () {


    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:7071/api")
        .configureLogging(signalR.LogLevel.Information)
        .build();



    connection.start().then(function () {
        console.log("connected");
    });


    connection.on("1", (message) => {

        console.log(message);
        //const encodedMsg = user + " says " + message;
        //const li = document.createElement("li");
        //li.textContent = encodedMsg;
        //document.getElementById("messagesList").appendChild(li);

        //$("h1").html("<div>" + user + " says " + message + "</div>");


    });

    



});

