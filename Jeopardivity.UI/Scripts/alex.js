let jwt;
let user;
let gameCode;
let game;
let userName;
let currentQuestion;
let questionCount;
let isAlex;
const baseUrl = "https://jeopardivity.azurewebsites.net";


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
        .configureLogging(signalR.LogLevel.Error)
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
            url: baseUrl + "/api/GetQuestionStatus",
            contentType: "application/json; charset=utf-8",
            data: '{"JWT":"' + jwt + '"}',
            dataType: "json",
            success: function (msg) {

                question = msg.Question;
                answerable = msg.Answerable;
                userBuzzed = msg.UserBuzzed;

                if (question < 1) {
                    createQuestion();
                }

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
                console.log(error);
            }
        });
    }


    function connectionOns() {
        connection.on("User" + user, (message) => {
            $("#buzzes").append($("<li>").text(message));
        });

    }


    function parseJwt(token) {


        let json = jwt_decode(token);

        user = json.User;
        gameCode = json.GameCode;
        userName = json.UserName;
        game = json.Game;
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
                console.log(error);
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
                console.log(error);
            }
        });

    }

    return false;
});