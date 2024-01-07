$(document).ready(function () {
    // Get the current path
    var currentPath = window.location.pathname;

    // Find all <a> elements inside the portal-menu
    var menuItems = $('.portal-menu a');

    // Loop through the menu items
    menuItems.each(function () {
        // Check if the href of the current menu item matches the current path
        if (currentPath.includes($(this).attr('href'))) {
            // Add the 'active' class to the matching menu item
            $(this).addClass('active');
        }
    });
});