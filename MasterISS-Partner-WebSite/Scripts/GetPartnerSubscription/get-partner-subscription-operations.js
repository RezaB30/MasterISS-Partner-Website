function GetDropzone(deleteButtonName, successResponseUrl) {

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
                    GetAlert(message, "true", successResponseUrl);
                }
                else {
                    $(".error-codes-container").show();
                    $(".error-codes").html(response.ErrorMessage);
                }

            });
        }
    });
}
