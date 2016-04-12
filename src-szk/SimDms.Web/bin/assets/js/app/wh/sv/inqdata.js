sdms.ui({
    title: 'Inquiry Data Service',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa-refresh' },
    ],
    rows: [
        {
            name: 'pnlFilter',
            fields: [
                {
                    name: 'TableName', text: 'Table Name', type: 'select', 'class': 'span6',
                    source: {
                        data: ['SvTrnService'],
                        text: '-- SELECT TABLE --'
                    }
                },
            ]
        },
        {
            name: 'grdDataList',
            type: 'kgrid',
            data: 'wh.api/inquiry/InqDataSdms',
            filter: 'pnlFilter',
            fields: [
                { name: 'CompanyCode', text: 'CompanyCode', width: 120 },
                { name: 'CompanyName', text: 'CompanyName', width: 250 },
                { name: 'TableName', text: 'Table Name', width: 150 },
                { name: 'DelayDate', text: 'Delay Date', width: 80, type: 'number' },
            ],
            dataBound: function () {
                console.log('grid bound');
            }
        },
    ],
    onClick: function (action) {
        switch (action) {
            case 'refresh':
                sdms.refresh('grdDataList');
                break;
            default:
                break;
        }
    },
    onRendered: function () { },
    onChange: function (key, value) { sdms.refresh('grdDataList') }
});
