var mobile = mobile || {};

function loginView() {
    mobile.onclick('login', 'register', function () {
        mobile.showProcess();
        mobile.post({
            url: 'DealerList',
            key: 'sdms.dealers',
            success: function (result) {
                mobile.hideProcess();
                mobile.navigate('register');
            },
            error: function () {
                mobile.hideProcess();
            }
        })
    });

    mobile.onclick('login', 'exit', function () {
        exit();
    });
}
