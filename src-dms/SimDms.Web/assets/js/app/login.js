$(document).ready(function () {
    $("#btnLogIn").on("click", function () {
        $("div.login form").submit();
    });
});

function runScript(e) {
    if (e.keyCode == 13) {
        $("div.login form").submit();
    }
}