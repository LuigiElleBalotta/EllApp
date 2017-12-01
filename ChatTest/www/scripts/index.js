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
        "MSG_TYPE_CHAT_REQUEST_RESPONSE": 2,
        "MSG_TYPE_CHAT": 3,
		"MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE": 4,
        "MSG_TYPE_REGISTRATION_RESPONSE": 5
    }

    var CommandType = {
        "login": 0,
        "message": 1,
        "chatsrequest": 2,
		"chatlistrequest": 3,
		"registration": 4
    }

    var ChatType = {
        "CHAT_TYPE_NULL": 0,
        "CHAT_TYPE_GLOBAL_CHAT": 1,
        "CHAT_TYPE_USER_TO_USER": 2,
        "CHAT_TYPE_GROUP_CHAT": 3,
    }

    /*******
    * VARS
    ********/
    var lastMessage = "";
    var dataReceived = "";
    var CurrentChatRoomID = "";
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
        checkForChatToBind();
        
        // TODO: Cordova è stato caricato. Eseguire qui eventuali operazioni di inizializzazione richieste da Cordova.
        $("#message").focus();
        setInterval(updateScroll, 100);
        $("#container_chat").hide();
        $("#container_text").hide();
        $("#backBTN").hide();
        $("#container_box_chat").hide();
        $("#container_box_chat_with_user").hide();
        $("#messaggio").html("Connessione");
    };

    function connectToServer()
    {
        if (!isConnected)
        {
			conn = new WebSocket('ws://192.168.0.113:10726');
            conn.onmessage = function (e) {
                dataReceived = e.data;
                if (dataReceived != "") {
                    var obj = JSON.parse(dataReceived);
                    switch(obj.MessageType)
                    {
                        case MessageType.MSG_TYPE_LOGIN_INFO:
                            accID = obj.data.AccountID;

                            //Request existing chat:
                            var chatreqObj = new Object();
                            chatreqObj.Type = CommandType.chatlistrequest;
                            chatreqObj.accid = accID;
                            chatreqObj = JSON.stringify(chatreqObj);
                            conn.send(chatreqObj);
                            break;
                        case MessageType.MSG_TYPE_CHAT:
                            alert("MSG_TYPE_CHAT received");
                            var chatobject = JSON.parse(obj.data);
                                switch(chatobject.chattype)
                                {
                                    case ChatType.CHAT_TYPE_GLOBAL_CHAT:
                                        var utente = obj.from;
                                        var messaggio = chatobject.text;
                                        $("#container_chat").append('<div class="row"><div class="col-xs-12"><div class="bubble"><div class="text-left"><h6><small><b>' + utente + '</b></small></h6></div>' + messaggio + '</div></div></div>');
                                        break;
                                    case ChatType.CHAT_TYPE_USER_TO_USER:
                                        alert("Received user to user chat");
                                        var room = chatobject.ChatRoom;
                                        var text = chatobject.text;
                                        var from = chatobject.ChatFrom;
                                        var to = chatobject.ChatTo;
                                        var timestamp = chatobject.timestamp;
                                        if ($('.item[data-chatroomid="' + room + '" ]').is(":visible")) //we do this only if this is visible, since the chat container is written every time we open a chat
                                        {
                                            alert("visible");
                                        }
                                        else
                                            alert("NOT visible");
                                        break;
                                    case ChatType.CHAT_TYPE_GROUP_CHAT:
                                        alert("not yet implemented.");
                                        break;
                                    case ChatType.CHAT_TYPE_NULL:
                                        alert("CHAT_TYPE_NULL");
                                        break;
                                }
                                break;
                        case MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE:
							var ChatList = JSON.parse(obj.data.ChatList);
							for (var i = 0; i < ChatList.length; i++)
                            {
								var chat = ChatList[i];
                                switch (chat.chattype)
                                {
                                    case ChatType.CHAT_TYPE_USER_TO_USER:
                                        if ($('.item[data-chatroomid="' + chat.ChatRoom + '" ]').length == 0)
                                        {
                                            var contact = "";
                                            if (chat.ChatFrom == localStorage.getItem("uname").toString().toUpperCase())
                                                contact = chat.ChatTo;
                                            else
                                                contact = chat.ChatFrom;
                                            $("#container_box_chat_with_user").append('<div class="user_box_chat" data-chatroomid="' + chat.ChatRoom + '"><a id="' + chat.ChatRoom + '" href="#" class="user_box_chat_link" data-chatroomid="' + chat.ChatRoom + '"><div class="row"><div class="col-xs-8"><h5 class="text_distance_from_left"><b>' + contact + '</b></h5></div><div class="col-xs-4"><h6><small>' + UnixToTime(chat.timestamp) + '</small></h6></div></div><div class="row"><div class="col-xs-12 text_distance_from_left">' + chat.text + '</div></div></a></div>');
                                        }
                                        break;
                                    default:
                                        alert("ERR_NO_CHAT_TYPE");
                                        break;
                                }
                            }
                            checkForChatToBind();
                            break;
                        case MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE:
                            var RequestedChat = JSON.parse(obj.data);
                            for (var i = 0; i < RequestedChat.length; i++)
                            {
                                var chat = RequestedChat[i];
                                switch(chat.chattype)
                                {
                                    case ChatType.CHAT_TYPE_USER_TO_USER:
                                        var contact = "";
                                        var myusername = localStorage.getItem("uname");
                                        if (chat.ChatFrom == myusername.toString().toUpperCase())
                                        {
                                            $("#container_chat").append('<div class="row"><div class="col-xs-12 text-right"><div class="bubble bubble--alt">' + chat.text + '</div></div></div>');
                                        }
                                        else
                                        {
                                            $("#container_chat").append('<div class="row"><div class="col-xs-12"><div class="bubble"><div class="text-left"><h6><small><b>' + chat.ChatFrom + '</b></small></h6></div>' + chat.text + '</div></div></div>');
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
            };
            conn.onopen = function (e) {
                $("#messages").hide();
                $("#messaggio").html("");
                $("#container_box_chat").show();
                $("#container_box_chat_with_user").show();
                $("#container_chat").hide();
                $("#container_text").hide();
                var loginObj = new Object();
                loginObj.Type = CommandType.login;
                loginObj.LoginPacket = {
                    Username: localStorage.getItem("uname"),
                    Psw: localStorage.getItem("psw"),
                    WantWelcomeMessage: 1
                };
                loginObj = JSON.stringify(loginObj);
                conn.send(loginObj);

                isConnected = true;
            };

            conn.onclose = function (e) {
                $("#messaggio").html("Tentativo di riconnessione..");
                $("#messages").show();
                $("#container_box_chat").hide();
                $("#container_box_chat_with_user").hide();
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
            sendMessage($("#container_chat").data("roomtype"));
        }
    }

    function sendMessage(_ChatType)
    {
        var messaggio = $("#message").val();
        if (messaggio != "") {
            if (isSpecialCommand(messaggio))
                executeCommand(messaggio);
            else {
                $("#container_chat").append('<div class="row"><div class="col-xs-12 text-right"><div class="bubble bubble--alt">' + messaggio + '</div></div></div>');
                var to = 0;
                if(CurrentChatRoomID == "globalchat")
                    to = 0;
                else
                {
                    var res = CurrentChatRoomID.split("-");
                    if (res[0] != accID)
                        to = res[0];
                    else
                        to = res[1];
                }

                var messageObj = new Object();
                messageObj.Type = CommandType.message;
                messageObj.MessagePacket = {
                    Message: messaggio,
                    ToType: _ChatType,
                    From: accID,
                    To: to
                };
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
        $("#container_box_chat_with_user").show();
    }

    function checkForChatToBind()
    {
        var chats = document.getElementsByClassName('user_box_chat_link');
        for (var i = 0; i < chats.length; i++) {
            chats[i].addEventListener('click', showChat.bind(this, chats[i]), false);
        }
    }

    function showChat(ChatBox)
    {
        $("#container_chat").html(""); //Clean chat template
        $("#container_chat").show();
        $("#container_text").show();
        $("#backBTN").show();
        //hide global chat
        $("#container_box_chat").hide();
        //hide user_to_user chat
        $("#container_box_chat_with_user").hide();
        CurrentChatRoomID = ChatBox.dataset.chatroomid;
        if (CurrentChatRoomID != "globalchat") {
            $("#container_chat").data("roomtype", ChatType.CHAT_TYPE_USER_TO_USER);
            //I Have to ask to the server for the chat that i selected
            var ReqObj = new Object();
            ReqObj.Type = CommandType.chatsrequest;
            ReqObj.ChatsRequestPacket = {
                AccID: accID,
                ChatRequestID: CurrentChatRoomID
            };
            ReqObj = JSON.stringify(ReqObj);
            conn.send(ReqObj);
        }
        else
        {
            $("#container_chat").data("roomtype", ChatType.CHAT_TYPE_GLOBAL_CHAT);
        }
    }

    function UnixToTime(unix_timestamp)
    {
        // Create a new JavaScript Date object based on the timestamp
        // multiplied by 1000 so that the argument is in milliseconds, not seconds.
        var date = new Date(unix_timestamp * 1000);
        // Hours part from the timestamp
        var day = date.getDay();
        var month = date.getMonth();
        var year = date.getFullYear();
        var hours = date.getHours();
        // Minutes part from the timestamp
        var minutes = "0" + date.getMinutes();
        // Seconds part from the timestamp
        var seconds = "0" + date.getSeconds();
        var formattedTime = "";
        return formattedTime = day + "/" + month + "/" + year + "<br/>" + "(" + hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2) + ")";
    }

} )();