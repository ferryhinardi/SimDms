var mobile = mobile || {};

function extractedView() {
    var user = mobile.storage.get();

    mobile.form({
        body: {
            panels: [
                {
                    name: 'panel1',
                    text: 'List of Dealers',
                    fields: [{ name: 'list1', type: 'list' }]
                }
            ],
        },
        footer: {
            bars: [
                { name: 'refresh', text: 'Refresh', icon: 'fa-refresh' },
            ]
        },
        onRendered: refreshList,
        onClick: function (action, text) {
            if (action == 'refresh') refreshList();
        }
    });

    function refreshList() {
        mobile.bind({
            url: 'DealerExtracted',
            type: 'list',
            selector: '.page.active #list1',
            showProcess: true,
            onClick: function (action, text) {
                user.dealer = { name: action, text: text };
                mobile.navigate('extracteddealer', user);
            },
            onComplete: function () {
                d3.select('.page.active .panel .title')
                    .html('List of Dealers <br/><span class="small">Last sync: '
                        + moment().format('DD-MMM-YYYY  HH:mm:ss')
                        + '</span>');
            }
        })
    }
}

function extracteddetailView() {
    mobile.form({
        body: {
            panels: [
                {
                    name: 'panel1',
                    text: 'List of Dealers',
                    fields: [
                        { name: 'TableName', text: 'Table', type: 'select' },
                        { name: 'list1', type: 'list' }
                    ]
                }
            ],
        },
        footer: {
            bars: [
                { name: 'refresh', text: 'Refresh', icon: 'fa-refresh' },
            ]
        },
        onRendered: function () {
            mobile.bind({
                name: 'TableName',
                url: 'DealerTables',
                onChange: function (value) {
                    refreshList();
                }
            });
        },
        onClick: function (action) {
            if (action == 'refresh') refreshList();
        }
    });

    function refreshList() {
        mobile.bind({
            url: 'DealerExtracted',
            type: 'list',
            selector: '.page.active #list1',
            params: mobile.serializeData(),
            showProcess: true,
            onComplete: function () {
                d3.select('.page.active .panel .title')
                    .html('List of Dealers <br/><span class="small">Last sync: '
                        + moment().format('DD-MMM-YYYY  HH:mm:ss')
                        + '</span>');
            }
        })
    }
}

function extracteddealerView() {
    mobile.form({
        body: {
            panels: [
                {
                    name: 'panel1',
                    text: 'Detail Data',
                    fields: [
                        { name: 'list1', type: 'list' }
                    ]
                }
            ],
        },
        footer: {
            bars: [
                { name: 'refresh', text: 'Refresh', icon: 'fa-refresh' },
            ]
        },
        onReady: refreshList,
        onRendered: function () {
            console.log('rendered');
        },
        onClick: function (action) {
            if (action == 'refresh') refreshList();
        }
    });

    function refreshList() {
        var user = mobile.storage.get();
        var params = { DealerCode: user.dealer.name }

        mobile.bind({
            url: 'DealerExtracted',
            type: 'list',
            selector: '.page.active #list1',
            params: params,
            showProcess: true,
            onComplete: function () {
                d3.select('.page.active .panel .title')
                    .html('Detail Data ' + user.dealer.text + '<br/><span class="small">Last sync: '
                        + moment().format('DD-MMM-YYYY  HH:mm:ss')
                        + '</span>');
            }
        })
    }
}
