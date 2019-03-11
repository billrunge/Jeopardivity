$(document).ready(function () {
    let gameCode;
    let userName;
    let game = 0;
    let user = 0;

    $("#SubmitButton").click(function (e) {

        gameCode = $("#GameCode").val();
        userName = $("#UserName").val();

        $.ajax({
            type: "POST",
            url: "http://localhost:7071/api/GetGameFromCode",
            contentType: "application/json; charset=utf-8",
            data: '{"GameCode":"' + gameCode + '"}',
            dataType: "json",
            success: function (msg) {
                game = msg.game;

                if (userName.trim().length < 1) {
                    $("#UserNameHelp").html('<error-text>Please enter a username</error-text>');
                    return false;
                }
                else {

                    $.ajax({
                        type: "POST",
                        url: "http://localhost:7071/api/CreateUser",
                        contentType: "application/json; charset=utf-8",
                        data: '{"Game":"' + game + '", "Name":"' + userName + '"}',
                        dataType: "json",
                        success: function (msg) {
                            console.log(msg);
                            user = msg.User;
                            $(location).attr('href', './buzzer.html?User=' + user)
                        },
                        error: function (req, status, error) {
                            $("h1").html('<error-text>Something bad has happened.</error-text>');
                            return false;
                        }
                    });

                }
            },
            error: function (req, status, error) {
                $("#GameCodeHelp").html('<error-text>Unable to validate Game Code</error-text>');
                return false;
            }
        });
        return false

    });

});

