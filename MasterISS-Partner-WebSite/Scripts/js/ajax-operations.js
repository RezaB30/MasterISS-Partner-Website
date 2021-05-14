function AjaxRequestTypeGetAndReturnHTML(AjaxDataValue, GetUrl, ContainerIdForReturnHTMLData) {
    $.ajax({
        url: GetUrl,
        type: 'GET',
        data: AjaxDataValue,
        dataType: 'html',
        success: function (data) {
            $(ContainerIdForReturnHTMLData).html(data);
        },
    });
}

function AjaxRequestTypePostAndReturnHTML(AjaxDataValue, PostUrl, ContainerIdForReturnHTMLData) {
    $.ajax({
        url: PostUrl,
        type: 'POST',
        data: AjaxDataValue,
        dataType: 'html',
        success: function (data) {
            $(ContainerIdForReturnHTMLData).html(data);
        },
    });
}