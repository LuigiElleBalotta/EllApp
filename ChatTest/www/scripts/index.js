// Per un'introduzione al modello vuoto, vedere la seguente documentazione:
// http://go.microsoft.com/fwlink/?LinkID=397704
// Per eseguire il debug del codice al caricamento della pagina in Ripple o in dispositivi/emulatori Android: avviare l'app, impostare i punti di interruzione, 
// quindi eseguire "window.location.reload()" nella console JavaScript.
(function () {
    "use strict";

    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    /*******
    * ENUMS
    ********/
    var MessageType = {
        "MSG_TYPE_LOGIN_INFO": 1,
        "MSG_TYPE_GLOBAL_MESSAGE": 2,
        "MSG_TYPE_CHAT_WITH_USER": 3,
        "MSG_TYPE_CHAT_WITH_GROUP":4
    }

    /*******
    * VARS
    ********/
    var lastMessage = "";
    var dataReceived = "";
    var conn = null;
    var isConnected = false;
    var accID = 0;
    function onDeviceReady() {
        // Gestire gli eventi di sospensione e ripresa di Cordova
        document.addEventListener( 'pause', onPause.bind( this ), false );
        document.addEventListener('resume', onResume.bind(this), false);
        setInterval(connectToServer, 1000);
        document.getElementById("message").addEventListener('keydown', sendMessageOnEnterKey.bind(this), false);
        document.getElementById("sendMessageBTN").addEventListener('click', sendMessage.bind(this), false);
        document.getElementById("backBTN").addEventListener('click', backBtnClick.bind(this), false);
        var chats = document.getElementsByClassName('user_box_chat_link');
        for (var i = 0; i < chats.length; i++)
        {
            chats[i].addEventListener('click', showChat.bind(this), false);
        }
        
        // TODO: Cordova è stato caricato. Eseguire qui eventuali operazioni di inizializzazione richieste da Cordova.
        $("#message").focus();
        setInterval(updateScroll, 100);
        $("#container_chat").hide();
        $("#container_text").hide();
        $("#backBTN").hide();
        $("#container_box_chat").hide();
        $("#messaggio").html("Connessione");
    };

    function connectToServer()
    {
        if (!isConnected)
        {
            conn = new WebSocket('ws://127.0.0.1:8080');
            conn.onmessage = function (e) {
                dataReceived = e.data;
                if (dataReceived != "") {
                    var obj = JSON.parse(dataReceived);
                    switch(obj.MessageType)
                    {
                        case MessageType.MSG_TYPE_LOGIN_INFO:
                            accID = obj.message;
                            break;
                        case MessageType.MSG_TYPE_GLOBAL_MESSAGE:
                                //var arr = lastMessage.split("|");
                                var utente = obj.from;
                                var messaggio = obj.message;
                                //lastMessage = arr[1];
                                $("#container_chat").append('<div class="row"><div class="col-xs-12"><div class="bubble"><div class="text-left"><h6><small><b>' + utente + '</b></small></h6></div>' + messaggio + '</div></div></div>');
                        
                            break;
                    }
                }
            };
            conn.onopen = function (e) {
                $("#messages").hide();
                $("#messaggio").html("");
                $("#container_box_chat").show();
                $("#container_chat").hide();
                $("#container_text").hide();
                var loginObj = new Object();
                loginObj.Type = 0;
                loginObj.Username = localStorage.getItem("uname");
                loginObj.Psw = localStorage.getItem("psw");
                loginObj = JSON.stringify(loginObj);
                conn.send(loginObj);
                isConnected = true;
            };

            conn.onclose = function (e) {
                $("#messaggio").html("Tentativo di riconnessione..");
                $("#messages").show();
                $("#container_box_chat").hide();
                $("#container_chat").hide();
                $("#container_text").hide();
                isConnected = false;
            };
        }
    }

    function onPause() {
        // TODO: questa applicazione è stata sospesa. Salvarne lo stato qui.
    };

    function onResume() {
        // TODO: questa applicazione è stata riattivata. Ripristinarne lo stato qui.
    };

    function sendMessageOnEnterKey(e)
    {
        if(e.keyCode == 13)
        {
            sendMessage();
        }
    }

    function sendMessage()
    {
        var messaggio = $("#message").val();
        if (messaggio != "") {
            if (isSpecialCommand(messaggio))
                executeCommand(messaggio);
            else {
                $("#container_chat").append('<div class="row"><div class="col-xs-12 text-right"><div class="bubble bubble--alt">' + messaggio + '</div></div></div>');
                var messageObj = new Object();
                messageObj.Type = 1;
                messageObj.Message = messaggio;
                messageObj.ToType = MessageType.MSG_TYPE_GLOBAL_MESSAGE;
                messageObj.From = accID;
                messageObj.To = 0;
                messageObj = JSON.stringify(messageObj);
                conn.send(messageObj);
                $("#message").val("");
            }
        }
    }

    function updateScroll() {
        $("#container_chat").scrollTop($("#container_chat")[0].scrollHeight);
    }

    function isSpecialCommand(cmd)
    {
        var bool = false;
        switch(cmd)
        {
            case "/clearchat":
                bool = true;
                break;
        }
        return bool;
    }

    function executeCommand(cmd)
    {
        switch(cmd)
        {
            case "/clearchat":
                $("#container_chat").html("");
                $("#message").val("");
                break;
        }
    }

    function backBtnClick()
    {
        $("#container_chat").hide();
        $("#container_text").hide();
        $("#backBTN").hide();
        $("#container_box_chat").show();
    }

    function showChat()
    {
        $("#container_chat").show();
        $("#container_text").show();
        $("#backBTN").show();
        $("#container_box_chat").hide();
    }

} )();