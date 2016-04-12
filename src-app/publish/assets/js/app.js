var Sdms = Sdms || {};
var layout = { state: {} };
var cacheInterval = 3600;

var util = new Sdms.Util();
var widget = new Sdms.Mobile({
    pages: [
        { name: 'login', type: 'login' },
        { name: 'module', type: 'page', title: 'Module', icon: 'fa-gears', listTitle: 'Select Module' },
        { name: 'mdlcs', type: 'page', title: 'Module CS', icon: 'fa-users', listTitle: 'Customer Satisfaction', toolbars: ['back'] },
        { name: 'csdsh', type: 'page', title: 'CS Dashboard', listTitle: 'Dasboards', icon: 'glyphicon-dashboard', toolbars: ['back'] },
        { name: 'csinq', type: 'page', title: 'CS Inquiry', listTitle: 'Inquiries', icon: 'fa-tasks', toolbars: ['back'] },
        { name: 'csdshouts', type: 'page', title: 'Outstanding Data', toolbars: ['back'] },
        { name: 'csdshproc', type: 'page', title: 'Processed Data', toolbars: ['back'] },
    ],
    onInitialized: function (e) {
        e.initBehaviour();

        layout = new Sdms.Layout({
            name: 'Sdms.Mobile',
            title: 'Sdms Mobile',
            onStateChanged: function (e, state) {
                switch (state) {
                    case 'login':
                        loginView();
                        break;
                    case 'module':
                        moduleView();
                        break;
                    case 'mdlcs':
                        mdlcsView();
                        break;
                    case 'csdsh':
                        csdshView();
                        break;
                    case 'csinq':
                        csinqView();
                        break;
                    case 'csdshouts':
                        csdshOutsView();
                        break;
                    case 'csdshproc':
                        csdshProcView();
                        break;
                    default:
                        break;
                }

                changeStateBehaviour();
            },
        });
    }
});

function changeStateBehaviour() {
    $('#shortcut').slideUp('fast');
    $('#shortcut').removeClass('shown');
}

function logOut() {
    $('#shortcut').slideUp('fast');
    $('#shortcut').removeClass('shown');
    window.location.hash = '';
    window.location.reload();
}

function loginView() {
    if (layout.state['login'] == undefined) {
        layout.state['login'] = 'created';
        $('#login #btnLogin').off();
        $('#login #btnLogin').on('click', function (e) {
            e.preventDefault();

            window.location.hash = 'module';
        });
    }

    $('#main .page').addClass('hide');
    $('#main .page#login').removeClass('hide');
}

function moduleView() {
    util.reload({
        name: 'module',
        data: [
            { name: 'mdlcs', text: 'Customer Staticfaction', icon: 'fa-smile-o' },
        ],
        onRowClick: function (name) {
            window.location.hash = name;
        },
    });
}

function mdlcsView() {
    util.reload({
        name: 'mdlcs',
        data: [
            { name: 'csdsh', text: 'Dashboard', icon: 'glyphicon-dashboard' },
            { name: 'csinq', text: 'Inquiry', icon: 'fa-table' },
        ],
        back: 'module',
        onRowClick: function (name) {
            window.location.hash = name;
        },
    });
}

function csdshView() {
    util.reload({
        name: 'csdsh',
        data: [
            { name: 'csdshouts', text: 'Outstanding Data', icon: 'fa-tags' },
            { name: 'csdshproc', text: 'Processed Data', icon: 'fa-tags' },
        ],
        back: 'mdlcs',
        onRowClick: function (name) {
            window.location.hash = name;
        },
    });
}

function csinqView() {
    util.reload({
        name: 'csinq',
        data: [
            { name: 'csinq-tdcall', text: '3 Days Call', icon: 'fa-phone' },
            { name: 'csinq-birthd', text: 'Cust Birthday', icon: 'fa-phone' },
        ],
        back: 'mdlcs',
    });
}

function csdshOutsView() {
    util.reload({
        name: 'csdshouts',
        type: 'form',
        form: {
            items: [
                {
                    name: 'DealerCode', text: 'Dealer Code', type: 'select',
                    data: [
                        { name: '6006406', text: 'BUANA INDOMOBIL TRADA' }
                    ]
                },
                {
                    name: 'BranchCode', text: 'Branch Code', type: 'select', info:'SELECT ALL',
                    data: [
                        { name: '6006404', text: 'BIT - BUMI SERPONG DAMAI' },
                        { name: '6006406', text: 'BIT - DEWI SARTIKA' }
                    ]
                },
                { name: 'ParDate', text: 'Reminder Date', type: 'date' },
                {
                    name: 'tblDash', text: 'C.R.O. Outstanding', type: 'table', url: 'wh.api/mobile/JsonpCsDashOut',
                    columns: [
                        { name: 'name', text: 'C.R.O.' },
                        { name: 'value', text: 'Value', style: 'width:60px', dclass: 'number' },
                    ]
                }
            ]
        },
        back: 'csdsh',
    });
}

function csdshProcView() {
    util.reload({
        name: 'csdshproc',
        type: 'form',
        form: {
            items: [
                {
                    name: 'DealerCode', text: 'Dealer Code', type: 'select',
                    data: [
                        { name: '6006406', text: 'BUANA INDOMOBIL TRADA' }
                    ]
                },
                {
                    name: 'BranchCode', text: 'Branch Code', type: 'select', info: 'SELECT ALL',
                    data: [
                        { name: '6006404', text: 'BIT - BUMI SERPONG DAMAI' },
                        { name: '6006406', text: 'BIT - DEWI SARTIKA' }
                    ]
                },
                { name: 'ParDate', text: 'Reminder Date', type: 'date' },
                {
                    name: 'tblDash', text: 'C.R.O. Processed', type: 'table', url: 'wh.api/mobile/JsonpCsDashOut',
                    columns: [
                        { name: 'name', text: 'C.R.O.' },
                        { name: 'value', text: 'Value', style: 'width:60px', dclass: 'number' },
                    ]
                }
            ]
        },
        back: 'csdsh',
    });
}
