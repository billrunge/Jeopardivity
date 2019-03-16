const baseUrl = "http://localhost:7071";
let jwt;
let gameCode;
let user;
let game;
let userName;
let question;
let answerable;
let userBuzzed;

$(document).ready(function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + "/api")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.start().then(function () {
        console.log("connected");
    });


    if (localStorage.getItem("JWT") === null) {
        $(location).attr('href', './index.html');
    } else {
        jwt = localStorage.JWT;
    }

    parseJwt(jwt);

    $("h1").html('Welcome to Jeopardivity, ' + userName + '!');
    $("h2").html('Game Code: ' + gameCode + '');

    resetStatus(connectionOns);

    $("#Buzz").click(function (e) {
        buzz();
    });

    function buzz() {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/Buzz",
            contentType: "application/json; charset=utf-8",
            data: '{"User":"' + user + '", "Question":"' + question + '" }',
            dataType: "json",
            success: function (msg) {

                resetStatus();

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
            url: baseUrl + "/api/GetQuestionStatusFromUser",
            contentType: "application/json; charset=utf-8",
            data: '{"User":"' + user + '"}',
            dataType: "json",
            success: function (msg) {

                question = msg.Question;
                answerable = msg.Answerable;
                userBuzzed = msg.UserBuzzed;

                if (question < 1) {
                    $("#Buzz").prop("disabled", true);
 
                }

                if (answerable === true) {
                    $("#Buzz").prop("disabled", false);
                }

                if (userBuzzed === true) {
                    $("#Buzz").prop("disabled", true);
                }

                if (typeof callback === "function") {
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

    function parseJwt(token) {


        let json = jwt_decode(token);

        user = json.User;
        gameCode = json.GameCode;
        userName = json.UserName;
        game = json.Game;
        console.log(json.User);
    }

});