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

    parseJwt(jwt);

    $("h1").html('Welcome to Jeopardivity, ' + userName + '!');
    $("h2").html('Game Code: ' + gameCode + '');

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + "/api")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.start();

    resetStatus(connectionOns);


    $("#AllowBuzzes").click(function () {
        makeQuestionAnswerable();
    });


    $("#NextQuestion").click(function (e) {
        createQuestion();
        resetStatus();
    });


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
                    createQuestion();
                }

                console.log(msg);

                if (answerable === true) {
                    $("#AllowBuzzes").prop("disabled", true);
                    $("#NextQuestion").prop("disabled", false);
                }
                else {
                    $("#AllowBuzzes").prop("disabled", false);
                    $("#NextQuestion").prop("disabled", true);

                }

                if (typeof callback === "function") {
                    // Call it, since we have confirmed it is callable
                    callback();
                }

                $("#buzzes").empty();

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to get question status</error-text>');
            }
        });
    }


    function connectionOns() {
        connection.on("User" + user, (message) => {

            console.log(message);

            $("#buzzes").append($("<li>").text(message));

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



    function createQuestion(callback) {
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateQuestion",
            contentType: "application/json; charset=utf-8",
            data: '{"Game":"' + game + '"}',
            dataType: "json",
            success: function (msg) {
                if (typeof callback === "function") {
                    // Call it, since we have confirmed it is callable
                    callback();
                }
                resetStatus();
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to create question</error-text>');
                return false;
            }
        });
    }

    function makeQuestionAnswerable() {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/MakeQuestionAnswerable",
            contentType: "application/json; charset=utf-8",
            data: '{"Question":"' + question + '", "Game":"' + game +'"}',
            dataType: "json",
            success: function (msg) {
                resetStatus();
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to begin next question</error-text>');
                return false;
            }
        });

    }

    return false;
});