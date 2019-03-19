$(document).ready(function () {
    let jwt;

    $("#SubmitButton").click(function (e) {
        $.ajax({
            type: "POST",
            url: "http://localhost:7071/api/CreateUser",
            contentType: "application/json; charset=utf-8",
            data: '{"GameCode":"' + $("#GameCode").val() + '", "Name":"' + $("#UserName").val() + '"}',
            dataType: "json",
            success: function (msg) {
                jwt = msg.JWT;
                localStorage.JWT = jwt;
                $(location).attr('href', './buzzer.html');

            },
            error: function (req, status, error) {
                $("h1").html('<error-text>Something bad has happened.</error-text>');
                return false;
            }
        });
        return false;
    });

});

