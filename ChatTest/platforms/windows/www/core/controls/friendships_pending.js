$("document").ready(function ()
{
    checkPendingRequests();
    var oldIDs = [];
    function checkPendingRequests()
    {
        var username = localStorage.getItem("uname");
        $.post("http://localhost/friendships/friendships_pending.php", "username=" + username, function (data, status)
        {
            if(data != "0")
            {
                $("#pendingRequestFriendship").html(data);
            }
        });

        $.post("http://localhost/friendships/friendships_who.php", "username=" + localStorage.getItem("uname"), function (data, status) {
            if (data != "") {
                var obj = JSON.parse(data);
                for (var i = 0; i < obj.length; i++) {
                    if(!inArray(obj[i].id, oldIDs))
                    {
                        $("#listPendingFriendships").append("<li class='element_no_overflow distance_bottom_xs' id='element_" + obj[i].id + "'><div class='row'><div class='col-xs-8 text-left'>" + obj[i].username + " has added you.</div><div class='col-xs-2'><button id='" + obj[i].id + "' class='btn btn-primary btn-fullwidth accept_friendship_btn text-center'><i class='glyphicon glyphicon-check accept_friendship_btn'></i></button></div><div class='col-xs-2'><button id='" + obj[i].id + "' class='btn btn-primary btn-fullwidth refuse_friendship_btn text-center'><i class='glyphicon glyphicon-remove refuse_friendship_btn'></i></button></div></div></li>");
                        oldIDs.push(obj[i].id);
                    }
                }
            }
        });
        $("#element_loading").hide();
    }

    function inArray(needle, haystack) {
        var length = haystack.length;
        for(var i = 0; i < length; i++) {
            if(haystack[i] == needle) return true;
        }
        return false;
    }

    setInterval(checkPendingRequests, 60000);

    document.addEventListener("click", function (e)
    {

        if ($(e.target).hasClass("refuse_friendship_btn"))
        {
            if ($(e.target).hasClass("glyphicon"))
                var id = $(e.target).closest("button").attr("id");
            else
                var id = e.target.id;
            var person1 = id;
            var person2 = localStorage.getItem("uname");
            $.post("http://localhost/friendships/friendships_refuse.php", "person1=" + person1 + "&person2=" + person2, function (data, status) {
                var msg = "";
                if (data != "") {
                    switch (data) {
                        case "1":
                            msg = "Accepted";
                            break;
                        case "2":
                            msg = "Error.";
                            break;
                        case "3":
                            msg = "No ME";
                            break;
                        case "4":
                            msg = "No WHO";
                            break;
                        case "5":
                            msg = "Can't refuse myself";
                            break;
                    }
                    $("#element_" + person1).hide(500);
                    for (var i = 0; i < oldIDs.length; i++) {
                        if (oldIDs[i] == person1)
                            oldIDs.splice(i, 1);
                    }
                }
            });
            checkPendingRequests();
        }

        if($(e.target).hasClass("accept_friendship_btn"))
        {
            if ($(e.target).hasClass("glyphicon"))
                var id = $(e.target).closest("button").attr("id");
            else
                var id = e.target.id;
            var person1 = id;
            var person2 = localStorage.getItem("uname");
            $.post("http://localhost/friendships/friendships_accept.php", "person1=" + person1 + "&person2=" + person2, function (data, status)
            {
                var msg = "";
                if (data != "")
                {
                    switch (data) {
                        case "1":
                            msg = "Accepted";
                            break;
                        case "2":
                            msg = "Error.";
                            break;
                        case "3":
                            msg = "No ME";
                            break;
                        case "4":
                            msg = "No WHO";
                            break;
                        case "5":
                            msg = "Can't accept myself";
                            break;
                    }
                    $("#element_" + person1).hide(500);
                    for(var i = 0; i < oldIDs.length; i++)
                    {
                        if(oldIDs[i] == person1)
                            oldIDs.splice(i, 1);
                    }
                }
            });
            checkPendingRequests();
        }
    });

});