const baseUrl = "https://jeopardivity.azurewebsites.net";
//const baseUrl = "http://localhost:7071";

let jwt;
let gameCode;
let user;
let game;
let userName;
let question;
let answerable;
let userBuzzed;
let buzzCount;
let currentButtonMode;
let isAlex;

const buttonMode = {
    ALLOW_BUZZES: 'ALLOW BUZZES',
    NEXT_QUESTION: 'NEXT QUESTION',
    BUZZ: 'BUZZ'

}



$(document).ready(function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + "/api")
        .configureLogging(signalR.LogLevel.Error)
        .build();

    connection.start();


    if (localStorage.getItem("JWT") === null) {
        $(location).attr('href', './index.html');
    } else {
        jwt = localStorage.JWT;
    }

    parseJwt(jwt);

    $("#TitleHeader").html('Jeopardivity');
    $("#GameInfo").html(gameCode);

    resetStatus(connectionOns);

    $("#DynamicButton").click(function (e) {

        if (isAlex) {
            $("#Buzzes").empty();
            if (currentButtonMode === buttonMode.ALLOW_BUZZES) {
                makeQuestionAnswerable();
            } else
                if (currentButtonMode === buttonMode.NEXT_QUESTION) {
                    createQuestion();
                }

        } else {
            currentButtonMode = buttonMode.BUZZ;
            $("#DynamicButton").html(currentButtonMode);
            $("#DynamicButton").prop("disabled", true);
            buzz();
        }

    });

    function buzz() {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/Buzz",
            contentType: "application/json; charset=utf-8",
            data: '{ "JWT":"' + jwt + '" }',
            dataType: "json",
            success: function (msg) {

                resetStatus();

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to buzz</error-text>');
                console.log(error);
            }
        });

    }

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

                buzzCount = 0;

                $("#UserInfo").html(userName);


                if (isAlex) {
                    if (question < 1) {
                        createQuestion();
                    }
                    $("#DynamicButton").prop("disabled", false);
                    if (answerable === true) {
                        currentButtonMode = buttonMode.NEXT_QUESTION;
                        $("#DynamicButton").html(currentButtonMode);
                    }
                    else {
                        currentButtonMode = buttonMode.ALLOW_BUZZES;
                        $("#DynamicButton").html(currentButtonMode);
                    }


                } else {
                    currentButtonMode = buttonMode.BUZZ;
                    $("#DynamicButton").html(currentButtonMode);
                    if (question < 1) {
                        $("#DynamicButton").prop("disabled", true);
                    }

                    if (answerable === true) {
                        $("#DynamicButton").prop("disabled", false);
                    } else {
                        $("#DynamicButton").prop("disabled", true);
                    }

                    if (userBuzzed === true) {
                        $("#DynamicButton").prop("disabled", true);
                    }

                }

                if (typeof callback === "function") {
                    callback();
                }

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to get question status</error-text>');
                console.log(error);
            }
        });
    }

    function connectionOns() {
        if (isAlex) {
            connection.on("User" + user, (message) => {
                if (buzzCount == 0) {
                    $("#Buzzes").append($('<li class="winning-buzz">').text(decodeURIComponent(message.toUpperCase())));
                } else {
                    $("#Buzzes").append($('<li class="losing-buzz">').text(decodeURIComponent(message.toUpperCase())));
                }
                buzzCount++;
            });
        } else {
            connection.on("Game" + game, (message) => {

                if (message == "NewQuestion") {
                    resetStatus();
                }

                if (message == "Answerable") {

                    resetStatus();
                }

            });
        }
    }

    function makeQuestionAnswerable(callback) {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/MakeQuestionAnswerable",
            contentType: "application/json; charset=utf-8",
            data: '{"JWT":"' + jwt + '"}',
            dataType: "json",
            success: function (msg) {
                if (typeof callback === "function") {
                    // Call it, since we have confirmed it is callable
                    callback();
                }

                resetStatus();
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unable to begin next question</error-text>');
                console.log(error);
            }
        });

    }

    function createQuestion(callback) {
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateQuestion",
            contentType: "application/json; charset=utf-8",
            data: '{"JWT":"' + jwt + '"}',
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

    function parseJwt(token) {


        let json = jwt_decode(token);

        user = json.User;
        gameCode = json.GameCode;
        userName = decodeURIComponent(json.UserName);
        game = json.Game;
        isAlex = json.IsAlex;
    }

});