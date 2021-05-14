function AjaxGetRequestForRevenues(clickedButton, url) {
    $(clickedButton).click(function () {
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#generic-Container").html(data);
            },
        });
    });
}