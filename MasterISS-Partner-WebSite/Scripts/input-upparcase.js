$("input[type=text], textarea").on("input", function () {
    var inputId = $(this).attr("id");
    if (inputId != "Username" && inputId != "PartnerCode" && inputId != "Password" && inputId != "UserEmail" && inputId != "GeneralInfo_Email") {
        $(this).val($(this).val().toLocaleUpperCase('tr'));
    }
});
$(".pagination-container").children().children().children().addClass("btn btn-icon btn-sm border-0 btn-hover-primary mr-2 my-1");
$("li.active").children().addClass("active");
