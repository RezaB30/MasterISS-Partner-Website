$(document).ready(function () {
    $(function () {
        $("#ExtraInfo_XDSLNo").mask("9999999999");
        $("#ExtraInfo_PSTN").mask("9999999999");
        $("#IDCard_TCKNo").mask("99999999999");
        $("#CorporateInfo_CentralSystemNo").mask("9999999999999999");
        $("#CorporateInfo_TaxNo").mask("9999999999");
        $("#CorporateInfo_TradeRegistrationNo").mask("999999");
        $("#GeneralInfo_ContactPhoneNo").mask("9999999999");
        $("#IDCard_BirthDate").mask("99.99.9999");
        $("#IDCard_TCIDCardWithChip_ExpiryDate").mask("99.99.9999");
        $("#IDCard_TCBirthCertificate_DateOfIssue").mask("99.99.9999");
    });


    $("#IDCard_CardTypeId").change(function () {
        var tckType = $(this).find(":selected").val();
        if (tckType == "1") {//This Is TCIDCardWithChip
            $("#TCBirthCertificateContainer").hide();
            $("#TCIDCardWithChipContainer").show();
        }
        else {
            $("#TCBirthCertificateContainer").show();
            $("#TCIDCardWithChipContainer").hide();
        }

        if ($("#ExtraInfo_SubscriptionRegistrationTypeId").find(":selected").val() == 2) {
            $("#xdslNoContainer").show();
        }
        else {
            $("#xdslNoContainer").hide();
        }

    });



    $("#ExtraInfo_SubscriptionRegistrationTypeId").change(function () {
        var registrationType = $(this).find(":selected").val();
        if (registrationType == "2") {//This Is Transiction
            $("#xdslNoContainer").show();
        }
        else {
            $("#xdslNoContainer").hide();
        }
    });

    $("#pstnSelect").change(function () {
        var value = $("#pstnSelect option:selected").val();
        if (value == "0") {
            $("#pstnContainer").hide();
        }

        else {
            $("#pstnContainer").show();
        }
    });


    CheckValidAndHideContainer("#GeneralInfo_SameSetupAddressByBilling", "#BillingAddressContainer")
    $("#GeneralInfo_SameSetupAddressByBilling").change(function () {
        CheckValidAndHideContainer(this, "#BillingAddressContainer")
    });

    CheckValidAndHideContainer("#Individual_SameSetupAddressByIndividual", "#IndividualResidencyContainer")
    $("#Individual_SameSetupAddressByIndividual").change(function () {
        CheckValidAndHideContainer(this, "#IndividualResidencyContainer")
    });

    CheckValidAndHideContainer("#CorporateInfo_SameSetupAddressByCorporativeResidencyAddress", "#ExecutiveResidencyContainer")
    $("#CorporateInfo_SameSetupAddressByCorporativeResidencyAddress").change(function () {
        CheckValidAndHideContainer(this, "#ExecutiveResidencyContainer")
    });

    CheckValidAndHideContainer("#CorporateInfo_SameSetupAddressByCorporativeCompanyAddress", "#CompanyContainer")
    $("#CorporateInfo_SameSetupAddressByCorporativeCompanyAddress").change(function () {
        CheckValidAndHideContainer(this, "#CompanyContainer")
    });


    $("#GeneralInfo_CustomerTypeId").change(function () {
        var value = $("#GeneralInfo_CustomerTypeId option:selected").val();
        if (value == "1") {
            $("#corporateCustomerInfoContainer").hide();
            $("#individualCustomerInfoContainer").show();
        }
        else if (value == "") {
            $("#corporateCustomerInfoContainer").hide();
            $("#individualCustomerInfoContainer").hide();
        }
        else {
            $("#corporateCustomerInfoContainer").show();
            $("#individualCustomerInfoContainer").hide();
        }
    });


    $("#telephoneNumberInputContainer").on('click', '.remove-button', function () {
        $(this).parent().remove();
    });

    var billingPeriodSelect = $("#billingPeriodId").find(":selected").val();
    if (billingPeriodSelect != "") {
        $("#billingPeriodId").attr("disabled", false);
    }

    var cardTypeSelect = $("#IDCard_CardTypeId").find(":selected").val();
    if (cardTypeSelect != "") {
        $("#IDCard_CardTypeId").trigger('change');
    }

    var customerTypeSelect = $("#GeneralInfo_CustomerTypeId").find(":selected").val();
    if (customerTypeSelect != "") {
        $("#GeneralInfo_CustomerTypeId").trigger('change');
    }

    var pstnSelectvalue = $("#pstnSelect").find(":selected").val()
    if (pstnSelectvalue != "") {
        $("#pstnSelect").trigger('change');
    }

});



function GetTariffPaymentDayList(url) {

    $("#partnerTariffId").change(function () {

        $("#billingPeriodId").find("option").not(":first").remove();

        $("#billingPeriodId").attr("disabled", "disabled")


        var id = $("#partnerTariffId").find(":selected").val();

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
                            $("#billingPeriodId").append($('<option>', {
                                value: result[i].Value,
                                text: result[i].Name,
                            }));
                        }
                        $("#billingPeriodId").attr("disabled", false)
                    }
                    else {
                        console.log(data.responseJSON.errorMessage);
                    }
                }
            });
        }
    });
}

function CheckValidAndHideContainer(checkId, containerId) {
    if ($(checkId).is(':checked')) {
        $(containerId).hide();
    }
    else {
        $(containerId).show();
    }
}

function SendRequestValidDate(year, month, day, type, url) {

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'JSON',
        contentType: 'application/json; charset=utf-8',
        data: {
            year: year,
            month: month,
            day, day
        },
        complete: function (data, status) {
            if (status == "success") {
                var statusData = data.responseJSON.status;
                if (statusData == "Failed") {
                    var errorMessage = data.responseJSON.ErrorMessage;
                    ChangeSelectSelectedValue(type);
                    GetAlert(errorMessage, "false", null)
                }
            }
            else {
                console.log("An error occurred");
            }
        }
    });
}

