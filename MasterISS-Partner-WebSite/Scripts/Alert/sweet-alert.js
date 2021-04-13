function GetAlert(message, Issuccess) {
    if (Issuccess == "true") {
        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: message,
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Hata Oluştu',
            text: message,
        })
    }

}