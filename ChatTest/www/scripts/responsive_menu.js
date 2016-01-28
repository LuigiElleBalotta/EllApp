$("document").ready(function ()
{
    collapseNavbar();
    // jQuery to collapse the navbar on scroll
    function collapseNavbar() {
        if ($(".navbar").offset().top > 50) {
            $(".navbar-fixed-top").addClass("top-nav-collapse");
        } else {
            $(".navbar-fixed-top").removeClass("top-nav-collapse");
        }
    }

    $(window).scroll(collapseNavbar);
    // Closes the Responsive Menu on Menu Item Click
    $('.navbar-collapse ul li a').click(function (e) {
        if (hasClass(e.target, "nav-page")) {
            $('.navbar-toggle:visible').click();
        }
    });
    function hasClass(elem, className) {
        return elem.className.split(' ').indexOf(className) > -1;
    }

    document.addEventListener("click", function (e)
    {
        if(hasClass(e.target, "navbar-toggle") || hasClass(e.target, "headerBTN"))
        {
            if (hasClass(e.target, "headerBTN"))
                var id = $(e.target).closest(".navbar-toggle").attr("id");
            else
                var id = e.target.id;
            var menuitems = document.getElementsByClassName("navbar-toggle");
            for(var i = 0; i < menuitems.length; i++)
            {
                if ($(menuitems[i]).attr("id") != id)
                {
                    var target = $(menuitems[i]).attr("data-target");
                    if ($(target).is(":visible"))
                        $(menuitems[i]).click();
                }
            }
            $("#"+id).click();
        }
    });
});