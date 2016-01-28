$("document").ready(function () 
{
    $.post("http://localhost/friendships/friendships_get_list.php", "username=" + localStorage.getItem("uname"), function (data, status)
    {
        var obj = JSON.parse(data);
        for (var i = 0; i < obj.length; i++)
        {
            $("#content_under_navbar").append("<div class='row row_background'><div class='col-xs-8'>" + obj[i].username + "</div><div class='col-xs-4'><button id='" + obj[i].id + "' class='btn btn-primary btn-fullwidth'>CHAT</button></div></div>");
        }
    });
});