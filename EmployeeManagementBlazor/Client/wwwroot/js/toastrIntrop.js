console.log('toastrIntrop.js loaded');
    window.toastrFunctions = {
        success: function (message, title) {
            if (typeof toastr !== 'undefined') {
                toastr.success(message, title);
            } else {
                console.error('Toastr is not defined');
            }
        },
        error: function (message, title) {
            if (typeof toastr !== 'undefined') {
                toastr.error(message, title);
            } else {
                console.error('Toastr is not defined');
            }
        }
    };

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

