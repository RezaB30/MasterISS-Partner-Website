function GetAlert(message, Issuccess, Url) {
    if (Issuccess == "true") {
        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: message,
        }).then(function () {
            window.location.href = Url;
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Hata Oluştu',
            text: message,
        }).then(function () {
            window.location.href = Url;
        })
    }

}