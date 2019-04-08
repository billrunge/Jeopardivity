const baseUrl = "https://jeopardivity.azurewebsites.net";
//let baseUrl = "http://localhost:7071";
let currentButtonMode;
let jwt;

const buttonMode = {
    CREATE_GAME: 'CREATE GAME',
    JOIN_GAME: 'JOIN GAME'
}


$(document).ready(function () {

    currentButtonMode = buttonMode.CREATE_GAME;

    $("#CreateButton").html(currentButtonMode);


    $("#CreateButton").click(function (e) {
        $("#ErrorArea").html("");
        e.preventDefault();

        if (currentButtonMode == buttonMode.CREATE_GAME) {
            createGame();
        } else {
            joinGame();
        }

    });

    $("#GameCode").bind('input propertychange', function () {
        let fieldStatus = $("#GameCode").val();

        if (fieldStatus == null || fieldStatus.trim() === '') {
            currentButtonMode = buttonMode.CREATE_GAME;
            $("#CreateButton").html(currentButtonMode);
        } else {
            currentButtonMode = buttonMode.JOIN_GAME;
            $("#CreateButton").html(currentButtonMode);
        }
    });

    function joinGame() {
        let gameCode = $("#GameCode").val().trim();
        let userName = encodeURIComponent($("#UserName").val());


        if (gameCode.length != 5) {
            $("#ErrorArea").html(`INVALID GAME CODE`);

        } else if (userName.length < 1) {
            $("#ErrorArea").html(`PLEASE ENTER NAME`);
        } else {
            $.ajax({
                type: "POST",
                url: baseUrl + "/api/CreateUser",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ GameCode: gameCode, Name: userName}),
                dataType: "json",
                success: function (msg) {
                    jwt = msg.JWT;
                    localStorage.JWT = jwt;
                    $(location).attr('href', './buzzer.html');

                },
                error: function (req, status, error) {
                    $("#ErrorArea").html(`SOMETHING BAD HAPPENED`);
                    console.log(error);
                }
            });
        }
    }

    function createGame() {
        let userName = encodeURIComponent($("#UserName").val());

        if (userName.length < 1) {
            $("#ErrorArea").html(`PLEASE ENTER NAME`);
        } else {
            $.ajax({
                type: "POST",
                url: `${baseUrl}/api/CreateGame`,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ Name: encodeURIComponent($("#UserName").val())}),
                dataType: "json",
                success: function (msg) {
                    jwt = msg.JWT;
                    localStorage.JWT = jwt;
                    $(location).attr('href', './buzzer.html');
                },
                error: function (req, status, error) {
                    $("#ErrorArea").html(`SOMETHING BAD HAPPENED`);
                    console.log(error);
                }
            });
        }
    }
});