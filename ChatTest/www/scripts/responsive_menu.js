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
});