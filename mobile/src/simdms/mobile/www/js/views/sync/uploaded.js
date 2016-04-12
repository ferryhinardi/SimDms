var mobile = mobile || {};

function uploadedView() {
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
        onClick: function (action) {
            if (action == 'refresh') refreshList();
        }
    });

    function refreshList() {
        mobile.showProcess();
        mobile.post({
            url: 'DealerOutstanding',
            key: 'list',
            success: function (data) {
                mobile.list({
                    data: data,
                    selector: '.page.active #list1',
                });
                //$('.page.active .panel .title').text('123');
                d3.select('.page.active .panel .title').html('List of Dealers <br/><span class="small">Last sync: ' + moment().format('DD-MMM-YYYY  HH:mm:ss') + '</span>');
            },
            complete: function () {
                mobile.hideProcess()
            }
        });
    }
}
