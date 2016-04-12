var mobile = new Sdms.Mobile({
    brand: 'Suzuki Indomobile Motor',
    pages: [
        { name: 'login', type: 'login' },
        {
            name: 'register', type: 'form', title: 'Register User', toolbars: ['back'], back: 'login',
            footer: {
                toolbars: [
                    { action: 'save', text: 'Save', icon: ' fa-save' },
                    { action: 'cancel', text: 'Cancel', icon: ' fa-undo' }
                ]
            },
        },
        {
            name: 'branch', type: 'form', title: 'Set Branch',
            footer: {
                toolbars: [
                    { action: 'save', text: 'Save', icon: ' fa-save' },
                ]
            },
        },
        { name: 'mnlist', type: 'form', title: 'Menu List' },
        {
            name: 'sprinput', type: 'form', title: 'Input Part', toolbars: ['back'], back: 'mnlist',
            footer: {
                toolbars: [
                    { action: 'scan', text: 'Scan', icon: ' fa-barcode' },
                    { action: 'save', text: 'Save', icon: ' fa-save' },
                    { action: 'cancel', text: 'Cancel', icon: ' fa-undo' }
                ]
            },
        },
        {
            name: 'sprlist', type: 'form', title: 'List of Part', toolbars: ['back'], back: 'mnlist',
            footer: {
                toolbars: [
                    { action: 'add', text: 'Add', icon: ' fa-plus' },
                    { action: 'edit', text: 'Edit', icon: ' fa-edit' },
                    { action: 'remove', text: 'Remove', icon: ' fa-trash-o' },
                    { action: 'process', text: 'Process', icon: ' fa-bolt' },
                    { action: 'clear', text: 'Clear', icon: ' fa-refresh' },
                    { action: 'checkall', text: 'Select All', icon: ' fa-check' },
                ]
            },
        },
    ],
    onRendered: function () {
        // for simulate only
        //mobile.storage.remove('sdms.user');

        var storage = mobile.storage;
        var user = storage.get('sdms.user');
        if (user) {
            mobile.navigate('mnlist');
        }
        else {
            mobile.navigate('login');
        }
    },
    onPageChanged: pageChanged,
});

function exit() {
    if (mobile.cordova) {
        if (navigator.app) {
            navigator.app.exitApp();
        }
        else if (navigator.device) {
            navigator.device.exitApp();
        }
    }
}

function pageChanged(page, options) {
    var back = options && options.back;
    if (!back) {
        if (page == 'login') loginView();
        if (page == 'register') registerView();
        if (page == 'branch') branchView();
        if (page == 'mnlist') mnlistView();
        if (page == 'sprinput') sprinputView();
        if (page == 'sprlist') sprlistView();
    }

    // set to current page
    window.localStorage.setItem('sdms.page', page);
    window.location.hash = page;
}

function mnlistView() {
    var data = [
        { name: 'sprinput', text: 'Input Part', subtext: 'Input part using barcode' },
        { name: 'sprlist', text: 'List of Part', subtext: 'List of Inputed parts' },
        { name: 'sprexit', text: 'Exit', subtext: 'Exit Application' }
    ];
    mobile.list({ selector: '[data-name=mnlist] .nav .list', data: data })

    mobile.onclick('mnlist', 'sprinput', function () {
        mobile.navigate('sprinput');
    });

    mobile.onclick('mnlist', 'sprlist', function () {
        mobile.navigate('sprlist');
    });

    mobile.onclick('mnlist', 'sprexit', function () {
        exit();
    });
}
