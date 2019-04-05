let game;
let userName;
let gameCode;
//let baseUrl = "https://jeopardivity.azurewebsites.net";
//let baseUrl = "http://localhost:7071";

$(document).ready(function () {


    $("#CreateGame").click(function (e) {
        e.preventDefault();

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateGame",
            contentType: "application/json; charset=utf-8",
            data: '{ "UserName":"' + encodeURIComponent($("#UserName").val()) + '"}',
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './buzzer.html');
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Error when creating user</error-text>');
                console.log(error);
            }
        });

        return false;

    });

});