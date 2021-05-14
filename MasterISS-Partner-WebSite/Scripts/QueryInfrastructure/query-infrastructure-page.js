$("#apartment").change(function () {
    var bbk = $("#apartment").find(":selected").val();
    if (bbk == "") {
        $("#bbk").val("");
        $(".query-infrastructure-button").attr("disabled", true);
    }
    else {
        $("#bbk").val(bbk);
        $(".query-infrastructure-button").attr("disabled", false);
    }
});