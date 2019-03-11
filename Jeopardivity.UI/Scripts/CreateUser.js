$("#SubmitButton").click(function () {
    let userName = $("#UserName").val();
    let gameCode = $("#GameCode").val();

    $.post("http://localhost:7071/api/CreateUser",
        {
            name: userName,
            game: gameCode
        },
        function (data, status) {
            alert("Here we are!");

            //alert("Data: " + data + "\nStatus: " + status);
        });
});