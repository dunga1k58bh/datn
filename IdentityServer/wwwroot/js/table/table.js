$(document).ready(function () {

    //Table selected
    $('input[type="checkbox"]').change(function () {
        var isChecked = $(this).prop('checked');
        
        if (isChecked) {
            $(this).closest('.js-item').addClass('item-selected');
        } else {
            $(this).closest('.js-item').removeClass('item-selected');
        }
    });
});