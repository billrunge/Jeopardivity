let game;
let userName;
let gameCode;
let baseUrl = "http://localhost:7071";

$(document).ready(function () {


    $("#SubmitButton").click(function (e) {

        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateGame",
            contentType: "application/json; charset=utf-8",
            data: '{ "UserName":"' + $("#UserName").val() + '"}',
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './alex.html');
            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Unabled to generate JWT</error-text>');

            }
        });

        return false;

    });

});