//const baseUrl = "https://jeopardivity.azurewebsites.net";
let baseUrl = "http://localhost:7071";

$(document).ready(function () {
    let jwt;

    $("#JoinGame").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: baseUrl + "/api/CreateUser",
            contentType: "application/json; charset=utf-8",
            data: '{"GameCode":"' + $("#GameCode").val().trim() + '", "Name":"' + encodeURIComponent($("#UserName").val()) + '"}',
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './buzzer.html');

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Something bad has happened.</error-text>');
                console.log(error);
            }
        });
        return false;
    });

});

