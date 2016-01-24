// Per un'introduzione al modello vuoto, vedere la seguente documentazione:
// http://go.microsoft.com/fwlink/?LinkID=397704
// Per eseguire il debug del codice al caricamento della pagina in Ripple o in dispositivi/emulatori Android: avviare l'app, impostare i punti di interruzione, 
// quindi eseguire "window.location.reload()" nella console JavaScript.
(function () {
    "use strict";

    document.addEventListener( 'deviceready', onDeviceReady.bind( this ), false );
    var lastMessage = "";
    var conn = new WebSocket('ws://localhost:8080/echo');
    conn.onmessage = function (e) {
        lastMessage = e.data;
        if (lastMessage != "")
            $("#container_chat").append('<div class="row"><div class="col-xs-12"><div class="bubble">' + lastMessage + '</div></div></div>');
    };
    conn.onopen = function (e) {
        //document.getElementById("conn").innerText = "Connection established!";
        //conn.send('Hello Me!');
    };
    function onDeviceReady() {
        // Gestire gli eventi di sospensione e ripresa di Cordova
        document.addEventListener( 'pause', onPause.bind( this ), false );
        document.addEventListener('resume', onResume.bind(this), false);
        document.getElementById("message").addEventListener('keydown', sendMessageOnEnterKey.bind(this), false);
        document.getElementById("sendMessageBTN").addEventListener('click', sendMessage.bind(this), false);
        
        // TODO: Cordova è stato caricato. Eseguire qui eventuali operazioni di inizializzazione richieste da Cordova.
        $("#message").focus();
        setInterval(updateScroll, 100);
        

    };

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
                $("#container_chat").append('<div class="row"><div class="col-xs-12 text-right"><div class="bubble bubble--alt"><div class="text-left"><h6><small><b>nomeutente</b></small></h6>' + messaggio + '</div></div></div>');
                conn.send(messaggio);
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

} )();