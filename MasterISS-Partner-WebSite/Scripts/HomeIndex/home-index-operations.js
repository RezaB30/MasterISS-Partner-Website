$("#subsNo").mask("99999999999");

function ValidBillsBySubscriberNo(validationUrl){
    var lastChangedSubscriberNo = "";
    var currentCheckedCheckboxDataValue = [];

    $('input[type=checkbox]').click(function () {
        var checkedCheckBoxDatavalueNo = $(this).attr("dataValue");
        if (this.checked) {

            var thisCheckbox = $(this);

            var checkedCheckBoxSubscriptionNo = $(this).attr("subsNo");

            if (lastChangedSubscriberNo == "") {
                lastChangedSubscriberNo = checkedCheckBoxSubscriptionNo;
            }

            if (checkedCheckBoxSubscriptionNo != lastChangedSubscriberNo) {
                thisCheckbox.prop("checked", false);
            }
            else {
                $("#subscriberNo").val(checkedCheckBoxSubscriptionNo);
                var subscriberNo = $("#subscriberNo").val();
                var customerCode = $("#customerCode").val();

                currentCheckedCheckboxDataValue = [];
                $('input[type=checkbox]:checked').each(function (index, element) {
                    var checkedCheckboxBillId = parseFloat($(this).val());
                    currentCheckedCheckboxDataValue.push(checkedCheckboxBillId);
                });

                var postData = { selectedBills: currentCheckedCheckboxDataValue, subscriberNo: subscriberNo, customerCode: customerCode };

                $.ajax({
                    url: validationUrl,
                    type: "POST",
                    dataType: "json",
                    data: postData,
                    complete: function (data, status) {
                        if (status == "success") {
                            var status = data.responseJSON.status;
                            if (status == "Failed") {
                                thisCheckbox.prop("checked", false);
                                lastChangedSubscriberNo = "";
                            }
                        }
                        else {
                            console.log("Failed Get Staff Work Days");
                        }
                    }
                });
            }
        }
        else {

            $('input[type=checkbox]:checked').each(function (index, element) {
                var dataValue = $(this).attr("dataValue");
                if (dataValue > checkedCheckBoxDatavalueNo) {
                    $(this).prop("checked", false);
                }
            });
            lastChangedSubscriberNo = "";
        }
    });

}