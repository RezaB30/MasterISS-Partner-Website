function GetAlert(message, Issuccess, Url) {
    if (Issuccess == "true") {
        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            confirmButtonText: `Tamam`,
            text: message,
        }).then(function () {
            window.location.href = Url;
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Hata Oluştu',
            cancelButtonText: 'Tamam',
            text: message,
        }).then(function () {
            if (Url != null) {
                window.location.href = Url;
            }
        })
    }

}