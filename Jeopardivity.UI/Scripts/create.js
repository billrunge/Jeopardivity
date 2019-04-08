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
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateUser",
            contentType: "application/json; charset=utf-8",
            data: "{ " + `"GameCode":"${$("#GameCode").val().trim()}", "Name":"${encodeURIComponent($("#UserName").val())}"` + " }",
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './buzzer.html');

            },
            error: function (req, status, error) {
                $("#ErrorArea").html(`<error-text>SOMETHING BAD HAPPENED</error-text>`);
                console.log(error);
            }
        });
    }

    function createGame() {
        $.ajax({
            type: "POST",
            url: `${baseUrl}/api/CreateGame`,
            contentType: "application/json; charset=utf-8",
            data: "{ " + `"Name":"${encodeURIComponent($("#UserName").val())}"` + " }",
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './buzzer.html');
            },
            error: function (req, status, error) {
                $("#ErrorArea").html(`<error-text>SOMETHING BAD HAPPENED</error-text>`);
                console.log(error);
            }
        });
    }
});