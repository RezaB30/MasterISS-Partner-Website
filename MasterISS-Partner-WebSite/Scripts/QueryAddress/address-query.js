function QueryAddress(triggerSelectId, triggeredSelectId, url) {

    $(document).ready(function () {
        $(triggerSelectId).change(function () {

            $("#service-avaibility-response").empty();

            $(triggeredSelectId).find("option").not(":first").remove();
            $(triggeredSelectId).attr("disabled", "disabled");

            if (triggeredSelectId != "#apartment") {
                $(triggeredSelectId).trigger('change');
            }

            var id = $(this).find(":selected").val();

            if (id != "") {
                var selectId = { "id": id }
                $.ajax({
                    url: url,
                    type: 'POST',
                    dataType: 'JSON',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(selectId),
                    complete: function (data, status) {
                        if (status == "success") {
                            var result = data.responseJSON.list;
                            for (var i = 0; i < result.length; i++) {
                                $(triggeredSelectId).append($('<option>', {
                                    value: result[i].Value,
                                    text: result[i].Name,
                                }));
                            }
                            $(triggeredSelectId).attr("disabled", false)
                        }
                        else {
                            console.log(data.responseJSON.errorMessage);
                        }
                    }
                });
            }
        });
    });
}
