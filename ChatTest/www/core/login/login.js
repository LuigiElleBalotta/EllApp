$("#avviso").hide();
$("document").ready(function () {

    var isConnected = false;
    var conn = null;

    if (localStorage.getItem("uname") != "" && localStorage.getItem("psw") != "")
    {
        $("#user_login").val(localStorage.getItem("uname"));
        $("#user_pass").val(localStorage.getItem("psw"));
        executeLogin();
    }
    $("#loginBTN").click(function (e) {
        executeLogin(e);
    });

    function resetAvviso() {
        $("#avviso").hide(300);
        $("#risultato").html("");
    }

    function executeLogin(e)
    {
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
        else
        {
			conn = new WebSocket('ws://192.168.0.113:8080');
			conn.onmessage = function (e) {
				try {
					//Risposta del login da parte del server
					dataReceived = e.data;
					if (dataReceived != "") {
						var obj = JSON.parse(dataReceived);
						switch (parseInt(obj.MessageType)) {
							case 1:
								var msg = "";
								var canGoOn = false;
								switch (obj.data) {
								case 1:
									msg = "Accesso effettuato! Attendi..";
									canGoOn = true;
									localStorage.setItem("uname", username);
									localStorage.setItem("psw", password);
									break;
								case 2:
									msg = "Credenziali errate";
									break;
								case 3:
									msg = "Nessuna password";
									break;
								case 4:
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
							break;
						default: //Throw away everything else
							break;
						}
					}
				}
				catch(e) {
					alert(e);
					$("#loginBTN").button("reset");
				}
			};

            conn.onopen = function (e) {
                var loginObj = new Object();
                loginObj.Type = 0;
                loginObj.Username = username;
                loginObj.Psw = password;
                loginObj.WantWelcomeMessage = 0;
                loginObj = JSON.stringify(loginObj);
                conn.send(loginObj);

                isConnected = true;
            };

            conn.onclose = function (e) { /*TODO: to handle*/ };

        

        //e.preventDefault();
        
        
        /*else {
            $.post("http://localhost/login/elaboraLogin.php", "username=" + username + "&password=" + password, function (data, status) {
                
            });*/
        }
    }
});