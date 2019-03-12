let game;
let userName;

$(document).ready(function () {


    $("#SubmitButton").click(function (e) {

        userName = $("#UserName").val();

        $.get("http://localhost:7071/api/CreateGame", function (data, status) {

            var obj = jQuery.parseJSON(data);
            game = obj.Game;

            $.ajax({
                type: "POST",
                url: "http://localhost:7071/api/CreateUser",
                contentType: "application/json; charset=utf-8",
                data: '{"Game":"' + game + '", "Name":"' + userName + '", "IsAlex":"' + true + '" }',
                dataType: "json",
                success: function (msg) {
                    user = msg.User;

                    $.ajax({
                        type: "POST",
                        url: "http://localhost:7071/api/CreateJWT",
                        contentType: "application/json; charset=utf-8",
                        data: '{"User":"' + user + '", "Game":"' + game + '", "UserName":"' + userName + '", "IsAlex":"' + true +'" }',
                        dataType: "json",
                        success: function (msg) {
                            jwt = msg.JWT;
                            localStorage.JWT = jwt;
                            $(location).attr('href', './alex.html');
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
        });

        return false;

    });

});