$("document").ready(function ()
{
    function searchUser()
    {
        var username = $("#searchUserTXT").val();
        if(username.length >= 3)//at least 3 chars
        {
            $.post("http://localhost/searchuser/elaboraRicerca.php", "username=" + username, function (data, status)
            {
                $("#result").html("");
                var obj = JSON.parse(data);
                for(var i = 0; i < obj.length; i++)
                {
                    $("#result").append("<div class='row row_background'><div class='col-xs-8 text-left'>" + obj[i].name + "</div><div class='col-xs-4'><button id='" + obj[i].id + "' class='btn btn-primary btnricerca' data-loading-text='<i class=\"spinner-loading\"></i>' >Add</button></div></div>");
                }
            });
        }
        else
        {
            alert("More characters please");
        }
    }

    $("#searchUserBTN").click(function () { searchUser(); });

    $("#searchUserTXT").keydown(function (e) { if (e.keyCode == 13) searchUser(); });

    document.addEventListener("click", function (e)
    {
        if($(e.target).hasClass("btnricerca"))
        {
            var id = e.target.id;
            $(e.target).button("loading");
            $.post("http://localhost/searchuser/addfriend.php", "from="+localStorage.getItem("uname")+"&idTo="+id, function (data, status)
            {
                var msg = "";
                switch(data)
                {
                    case "1":
                        msg = "Added!";
                        break;
                    case "2":
                        msg = "Error: no 'idTo' parameter.";
                        break;
                    case "3":
                        msg = "Error: no 'from' parameter.";
                        break;
                    case "4":
                        msg = "Friendship already pending.";
                        break;
                    case "5":
                        msg = "You can't add yourself.";
                        break;
                }
                alert(msg);
            });
            $(e.target).button("reset");
        }
    });

});