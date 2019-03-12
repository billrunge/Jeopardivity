$(document).ready(function () {

    let jwt;
    let gameCode;
    let user;
    let userName;
    let currentQuestion;
    let questionCount;
    let winner;

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
            userName = msg.UserName;
            currentQuestion = msg.CurrentQuestion;
            questionCount = msg.QuestionCount;

            $("h1").html('Welcome to Jeopardivity, ' + userName + '!');
            $("h2").html('Game Code: ' + gameCode + '');
            $("h3").html('Question Count: ' + questionCount + '');
        },
        error: function (req, status, error) {
            $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
            return false;
        }
    });

    $("#Buzz").click(function (e) {

        if (currentQuestion < 1) {
            $("h1").html('<error-text>This game has not begun. Please be patient.</error-text>');

        } else {

            $.ajax({
                type: "POST",
                url: "http://localhost:7071/api/Buzz",
                contentType: "application/json; charset=utf-8",
                data: '{"User":"' + user + '", "Question":"' + currentQuestion + '" }',
                dataType: "json",
                success: function (msg) {
                    winner = msg.Winner;
                    if (winner) {

                        $("h3").html("<error-text>It's your guess.</error-text>");

                    } else {
                        $("#Buzz").prop("disabled", true);
                    }
                },
                error: function (req, status, error) {
                    $("h1").html('<error-text>Unable to get user information from JWT</error-text>');
                    return false;
                }
            });
        }
    });


});