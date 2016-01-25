$("#avviso").hide();
$("document").ready(function () {
    $("#loginBTN").click(function (e) {
        e.preventDefault();
        $("#loginBTN").button("loading");
        var username = $("#user_login").val().trim();
        var password = $("#user_pass").val().trim();
        if (username == "" || password == "") {
            $("#loginBTN").button("reset");
            $("#risultato").html("Non hai compilato tutti i campi!");
            $("#avviso").show(300);
            setTimeout(resetAvviso, 3000);
            return false;
        }
        else {
            $.post("http://localhost/login/elaboraLogin.php", "username=" + username + "&password=" + password, function (data, status) {
                var msg = "";
                var canGoOn = false;
                switch(data)
                {
                    case "1":
                        msg = "Accesso effettuato! Attendi..";
                        canGoOn = true;
                        break;
                    case "2":
                        msg = "Credenziali errate";
                        break;
                    case "3":
                        msg = "Nessuna password";
                        break;
                    case "4":
                        msg = "Nessun username";
                        break;
                }
                $("#risultato").html(msg);
                $("#avviso").show(300);
                $("#loginBTN").button("reset");
                if (!canGoOn)
                    setTimeout(resetAvviso, 3000);
                else
                    setTimeout(function () { window.location.href = "../../index.html"; }, 100);
            });
        }
    });

    function resetAvviso() {
        $("#avviso").hide(300);
        $("#risultato").html("");
    }
});