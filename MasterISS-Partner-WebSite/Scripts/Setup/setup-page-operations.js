function GetDropzoneSetup(deleteButtonName, url) {

    Dropzone.autoDiscover = false;
    $('#myDropzone').dropzone({
        //parameter name value
        paramName: "files",
        //clickable div id
        clickable: '#previews',
        //preview files container Id
        previewsContainer: "#previews",
        autoProcessQueue: false,
        uploadMultiple: true,
        parallelUploads: 100,
        maxFiles: 100,
        //  url:"/", // url here to save file
        maxFilesize: 100,//max file size in MB,
        addRemoveLinks: true,
        dictResponseError: 'Server not Configured',
        acceptedFiles: ".png,.jpg,.jpeg,.pdf",// use this to restrict file type
        init: function () {
            var self = this;
            // config
            self.options.addRemoveLinks = true;
            self.options.dictRemoveFile = deleteButtonName;
            //New file added
            self.on("addedfile", function (file) {
                console.log('new file added ', file);
                $('.dz-success-mark').hide();
                $('.dz-error-mark').hide();
            });
            // Send file starts
            self.on("sending", function (file) {
                console.log('upload started', file);
                $('.meter').show();
            });

            //File upload Progress
            self.on("totaluploadprogress", function (progress) {
                console.log("progress ", progress);
                $('.roller').width(progress + '%');
            });

            self.on("queuecomplete", function (progress) {
                $('.meter').delay(999).slideUp(999);
            });

            // On removing file
            self.on("removedfile", function (file) {
                console.log(file);
            });

            $('#Submit').on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                // Validate form here if needed

                if (self.getQueuedFiles().length > 0) {
                    self.processQueue();


                } else {
                    self.uploadFiles([]);
                    $('#myDropzone').submit();
                }
            });

            self.on("successmultiple", function (files, response) {
                if (response.status == "Success") {
                    var message = response.message;
                    GetAlert(message, "true", url);
                }
                else {
                    $(".error-codes-container").show();
                    $(".error-codes").html(response.ErrorMessage);
                }

            });
        }
    });
}

$(function () {
    $("#search_TaskListStartDate").mask("99.99.9999 99:99");
    $("#search_TaskListEndDate").mask("99.99.9999 99:99");
    $("#search_SearchedTaskNo").mask("99999");
});

$("#TaskUpdateContainer").on('change', '.FaultCodesDrowndownByUpdateTaskStatus', function () {
    var value = $(this).val();
    if (value == '9') {//Rendezvous Made
        $('.selectedSetupTeamContainer').show(100);
        $('#selectedSetupTeamUserInfo').show(100);
        $('.assignTaskButton').hide(100);
    } else {
        $('.selectedSetupTeamContainer').hide(100);
        $('#selectedSetupTeamUserInfo').hide(100);
        $('.assignTaskButton').show(100);
    }
});

$(".customer-localition-info-button").click(function () {
    var dataValue = $(this).attr("dataValue");
    $(".taskNoToUpdateClientLocation").val(dataValue);
});

function GetSwalFireForConfirmation(dataValue, ajaxUrl, title, titleYes, titleCancel, returnUrl) {
    Swal.fire({
        title: title,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: titleYes,
        cancelButtonText: titleCancel,
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: ajaxUrl,
                type: 'GET',
                data: {
                    taskNo: dataValue,
                },
                dataType: 'JSON',
                success: function (data) {
                    var result = data.status;
                    if (result == "Success") {
                        var message = data.message;
                        GetAlert(message, "true", returnUrl);
                    }
                    else {
                        var errorMessage = data.ErrorMessage;
                        GetAlert(errorMessage, "false", returnUrl);
                    }
                },
            });
        }
    })
}



function TaskUpdateWithRendezvousPost(postUrl, returnSuccessUrl, returnFailedUrl) {

    $("#TaskUpdateContainer").on('click', '.assignTaskSetupTeamButton', function () {
        var dataValue = $(this).attr("dataValue");
        var selectedDate = null;
        var selectedTime = null;

        var taskNo = $(".genericContainer").children().children().children("#TaskNo").val();
        var description = $('.genericContainer').children().children().children().children('textarea#description').val();
        var faultCode = $(".genericContainer").children().children().children("#FaultCodes").find(":selected").val();
        $(".assignTaskButtonContainer").each(function () {
            var date = $(this).find("#item_SelectedDate");
            var time = $(this).find("#item_SelectedTime");
            if (date.attr("dataValue") == dataValue.toString()) {
                selectedDate = date.find(":selected").val()
                selectedTime = time.find(":selected").val()
            };
        });

        var postData = { PostTimeValue: selectedTime, PostDateValue: selectedDate, TaskNo: taskNo, Description: description, staffId: dataValue, FaultCodes: faultCode, };
        $.ajax({
            url: postUrl,
            type: "POST",
            dataType: "json",
            data: postData,
            complete: function (data, status) {
                if (status = "success") {
                    var responseStatus = data.responseJSON.status;
                    if (responseStatus == "Success") {
                        var message = data.responseJSON.message;
                        var url = returnSuccessUrl;
                        GetAlert(message, "true", url);
                    }
                    else if (responseStatus == "FailedAndRedirect") {
                        var redirectMessage = data.responseJSON.ErrorMessage;
                        var urlHome = returnFailedUrl;
                        GetAlert(redirectMessage, "false", urlHome);
                    }
                    else {
                        $(".error-codes-container").show();
                        $(".error-codes").html(data.responseJSON.ErrorMessage);
                    }
                }
                else {
                    console.log("Error");
                }
            }
        });

    });
}


function GetStaffAvailableHours(postUrl) {
    $("#TaskUpdateContainer").on('change', '.setupTeamWorkDays', function () {
        var hoursSelect = $(this).parent().children('.setupTeamWorkHours');

        var selectedSelectDateValue = $(this).find(":selected").val();
        var selectDataValue = $(this).attr("dataValue");
        var postData = { staffId: selectDataValue, date: selectedSelectDateValue };

        $.ajax({
            url: postUrl,
            type: "POST",
            dataType: "json",
            data: postData,
            complete: function (data, status) {
                if (status == "success") {
                    var result = data.responseJSON.list;
                    $(hoursSelect).find("option").not(":first").remove();
                    for (var i = 0; i < result.length; i++) {
                        hoursSelect.append($('<option>', {
                            value: result[i].Value,
                            text: result[i].Name,
                        }));
                    }
                }
                else {
                    console.log("Failed Get Staff Work Days");
                }
            }
        });
    });
}
function GetStaffAvailableDays(postUrl) {
    $("#TaskUpdateContainer").on('click', '.setupTeamWorkDays', function () {
        var selectedSelect = $(this);

        if ($(this).children().length <= 1) {
            var dataValue = selectedSelect.attr("dataValue");
            var postData = { staffId: dataValue };
            $.ajax({
                url: postUrl,
                type: "POST",
                dataType: "json",
                data: postData,
                complete: function (data, status) {
                    if (status == "success") {
                        var result = data.responseJSON.list;
                        for (var i = 0; i < result.length; i++) {
                            selectedSelect.append($('<option>', {
                                value: result[i].Value,
                                text: result[i].Name,
                            }));
                        }
                    }
                    else {
                        console.log("Failed Get Staff Work Days");
                    }
                }
            });
        }
    });
}
