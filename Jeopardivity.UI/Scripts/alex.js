let jwt;
let user;
let gameCode;
let game;
let userName;
let currentQuestion;
let questionCount;
let isAlex;


$(document).ready(function () {

    if (localStorage.getItem("JWT") === null) {
        $(location).attr('href', './index.html'); 
    } else {
        jwt = localStorage.JWT;
    }

    $.ajax({
        type: "POST",
        url: "http://localhost:7071/api/GetUserStatusFromJWT",
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
        },
        error: function (req, status, error) {
            $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
            return false;
        }
    });

    $("#Question").mousedown(function () {
        $.ajax({
            type: "POST",
            url: "http://localhost:7071/api/CreateQuestion",
            contentType: "application/json; charset=utf-8",
            data: '{"Game":"' + game + '"}',
            dataType: "json",
            success: function (msg) {
                currentQuestion = msg.Question;
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to begin next question</error-text>');
                return false;
            }
        });
    });

    $("#Question").mouseup(function () {
        $(this).prop("disabled", true);
        $.ajax({
            type: "POST",
            url: "http://localhost:7071/api/EndQuestion",
            contentType: "application/json; charset=utf-8",
            data: '{"Question":"' + (currentQuestion) + '"}',
            dataType: "json",
            success: function (msg) {
                let winner = 0;

                setInterval(function () {
                    $.ajax({
                        type: "POST",
                        url: "http://localhost:7071/api/GetWinner",
                        contentType: "application/json; charset=utf-8",
                        data: '{"Question":"' + (currentQuestion) + '"}',
                        dataType: "json",
                        success: function (msg) {
                            winner = msg.User;
                            console.log(winner);
                            if (winner > 0) {
                                $("h2").html('<error-text>User: ' + winner + ' is winner!</error-text>');
                                clearInterval();
                            }

                        },
                        error: function (req, status, error) {
                            $("h1").html('<error-text>Unable to begin next question</error-text>');
                            return false;
                        }
                    });

                }, 3000);
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to begin next question</error-text>');
                return false;
            }
        });

    });

    $("#NextQuestion").click(function (e) {
        $(location).attr('href', './alex.html');
    });


});