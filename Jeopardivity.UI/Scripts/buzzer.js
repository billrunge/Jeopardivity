const baseUrl = "http://localhost:7071";

$(document).ready(function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + "/api")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.start().then(function () {
        console.log("connected");
    });

    let jwt;
    let gameCode;
    let user;
    let game;
    let userName;
    let currentQuestion;
    let questionCount;
    let winner;

    if (localStorage.getItem("JWT") === null) {
        $(location).attr('href', './index.html');
    } else {
        jwt = localStorage.JWT;
    }

    resetStatus(connectionOns);

    $("#Buzz").click(function (e) {
            buzz();
    });

    function buzz() {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/Buzz",
            contentType: "application/json; charset=utf-8",
            data: '{"User":"' + user + '", "Question":"' + currentQuestion + '" }',
            dataType: "json",
            success: function (msg) {

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
                return false;
            }
        });

    }

    function resetStatus(callback) {
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/GetUserStatusFromJWT",
            contentType: "application/json; charset=utf-8",
            data: '{"JWT":"' + jwt + '"}',
            dataType: "json",
            success: function (msg) {
                user = msg.User;
                gameCode = msg.GameCode;
                userName = msg.UserName;
                game = msg.Game;
                currentQuestion = msg.CurrentQuestion;
                questionCount = msg.QuestionCount;

                $("h1").html('Welcome to Jeopardivity, ' + userName + '!');
                $("h2").html('Game Code: ' + gameCode + '');
                $("h3").html('Question Count: ' + questionCount + '');

                if (currentQuestion < 1) {
                    $("#Buzz").prop("disabled", true);
                } else {
                    $("#Buzz").prop("disabled", false);
                }

                if (typeof callback === "function") {
                    // Call it, since we have confirmed it is callable
                    callback();
                }

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
            }
        });
    }

    function connectionOns() {
        connection.on("User" + user, (message) => {

            if (message == "Winner") {
                $("body").css("background-color", "red");

            }

        });

        connection.on("Game" + game, (message) => {
            if (message == "NewQuestion") {
                resetStatus();
            }

        });
    }


});