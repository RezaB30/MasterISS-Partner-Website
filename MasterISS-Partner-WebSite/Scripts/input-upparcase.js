$("input[type=text], textarea").on("input", function () {
    var inputId = $(this).attr("id");
    if (inputId != "Username" && inputId != "PartnerCode" && inputId != "Password" && inputId != "UserEmail") {
        $(this).val($(this).val().toLocaleUpperCase('tr'));
    }
});
