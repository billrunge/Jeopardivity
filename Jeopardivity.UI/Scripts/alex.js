let jwt;
let user;
let gameCode;
let game;
let userName;
let currentQuestion;
let questionCount;
let isAlex;
const baseUrl = "http://localhost:7071";


$(document).ready(function () {

    if (localStorage.getItem("JWT") === null) {
        $(location).attr('href', './index.html');
    } else {
        jwt = localStorage.JWT;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + "/api")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.start();

    resetStatus(connectionOns);


    $("#EndQuestion").click(function () {
        endQuestion();       
    });


    $("#NextQuestion").click(function (e) {
        resetStatus();
    });


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
                game = msg.Game;
                userName = msg.UserName;
                currentQuestion = msg.CurrentQuestion;
                questionCount = msg.QuestionCount;
                isAlex = msg.IsAlex;

                if (!isAlex) {
                    $(location).attr('href', './buzzer.html');
                }

                $("h1").html('Welcome to Jeopardivity, ' + userName + '!');
                $("h2").html('Game Code: ' + gameCode + '');
                $("h3").html('Question Count: ' + questionCount + '');


                if (typeof callback === "function") {
                    // Call it, since we have confirmed it is callable
                    callback();
                }

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
                return false;
            }
        });
    }


    function connectionOns() {
        connection.on("User" + user, (message) => {





            $("p").html("<div>" + message + "</div>");
        });

        connection.on("Game" + game, (message) => {


        });
    }




    function endQuestion() {
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateQuestion",
            contentType: "application/json; charset=utf-8",
            data: '{"Game":"' + game + '"}',
            dataType: "json",
            success: function (msg) {
                currentQuestion = msg.Question;
                $.ajax({
                    type: "POST",
                    url: baseUrl + "/api/EndQuestion",
                    contentType: "application/json; charset=utf-8",
                    data: '{"Question":"' + (currentQuestion) + '"}',
                    dataType: "json",
                    success: function (msg) {
                        $(this).prop("disabled", true);

                    },
                    error: function (req, status, error) {
                        $("h1").html('<error-text>Unable to begin next question</error-text>');
                        return false;
                    }
                });
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to begin next question</error-text>');
                return false;
            }
        });
    }
});