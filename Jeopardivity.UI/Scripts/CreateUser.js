$(document).ready(function () {
    let gameCode;
    let userName;
    let game = 0;
    let user = 0;
    let isAlex;
    let jwt;

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
                        data: '{"Game":"' + game + '", "Name":"' + userName + '","IsAlex":"' + false +'" }',
                        dataType: "json",
                        success: function (msg) {
                            user = msg.User;

                            $.ajax({
                                type: "POST",
                                url: "http://localhost:7071/api/CreateJWT",
                                contentType: "application/json; charset=utf-8",
                                data: '{"User":"' + user + '", "Game":"' + game + '", "UserName":"' + userName + '", "GameCode":"' + gameCode + '",  "IsAlex":"' + false +'" }',
                                dataType: "json",
                                success: function (msg) {
                                    jwt = msg.JWT;
                                    localStorage.JWT = jwt;
                                    $(location).attr('href', './buzzer.html');
                                },
                                error: function (req, status, error) {
                                    $("h1").html('<error-text>Unabled to generate JWT</error-text>');
                                    return false;
                                }
                            });
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

