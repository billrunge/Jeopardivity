let game;
let userName;
let gameCode;
let baseUrl = "http://localhost:7071";

$(document).ready(function () {

    
    $("#SubmitButton").click(function (e) {

        userName = $("#UserName").val();

        $.get(baseUrl + "/api/CreateGame", function (data, status) {

            var obj = jQuery.parseJSON(data);
            game = obj.Game;
            gameCode = obj.GameCode;
            console.log(gameCode);

            $.ajax({
                type: "POST",
                url: baseUrl + "/api/CreateUser",
                contentType: "application/json; charset=utf-8",
                data: '{"Game":"' + game + '", "Name":"' + userName + '","IsAlex":"' + true + '" }',
                dataType: "json",
                success: function (msg) {
                    user = msg.User;

                    $.ajax({
                        type: "POST",
                        url: baseUrl + "/api/CreateJWT",
                        contentType: "application/json; charset=utf-8",
                        data: '{"User":"' + user + '", "Game":"' + game + '", "UserName":"' + userName + '", "GameCode":"' + gameCode + '",  "IsAlex":"' + true + '" }',
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
                },
                error: function (req, status, error) {
                    $("h1").html('<error-text>Something bad has happened.</error-text>');

                }
            });           
        });
        return false;

    });

});