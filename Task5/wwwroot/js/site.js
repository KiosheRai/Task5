
function check(field, flag) {
    if (flag == 1) { for (i = 0; i < field.length; i++) field[i].checked = true; }
    else { for (i = 0; i < field.length; i++) field[i].checked = false; }
}

function SelectedCheckbox() {
    var selectedItems = new Array();
    $("input[id='check']:checked").each(function () { selectedItems.push($(this).val()); });

    return (selectedItems.toString());
}