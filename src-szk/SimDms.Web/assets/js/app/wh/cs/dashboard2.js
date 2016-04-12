sdms.ui({
    title: 'Customer Satisfaction - Today\'s Reminder 2',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa-refresh' },
        { text: 'Back', action: 'back', icon: 'fa-hand-o-left', hide: true },
        { text: 'Export (xls)', action: 'export', icon: 'fa-file-excel-o', hide: true },
    ],
    rows: [
        {
            fields: [
                {
                    name: 'CompanyCode', text: 'Dealer Name', type: 'select', 'class': 'span4',
                    source: {
                        url: 'wh.api/Combo/DealerList',
                        params: { LinkedModule: 'CS' },
                        text: '-- SELECT DEALER --'
                    }
                },
                {
                    name: 'BranchCode', text: 'Outlet Name', type: 'select', 'class': 'span4',
                    source: {
                        url: 'wh.api/Combo/Branchs',
                        cascade: { source: 'CompanyCode', name: 'comp' },
                        text: '-- SELECT OUTLET --'
                    }
                },
                { name: 'ReminderDate', text: 'Date of Reminder', type: 'popup', 'class': 'span3 full', placeholder: 'DD-MMM-YYYY', readonly: true },
            ]
        },
        {
            name: 'pnlCro',
            text: 'C. R. O.',
            fields: [
                { name: '3DaysCall', text: '3 Days Call', type: 'popup', icon: 'fa-hand-o-right', 'class': 'span6', readonly: true },
                { name: 'BDayCall', text: 'Birthday Call', type: 'popup', icon: 'fa-hand-o-right', 'class': 'span6', readonly: true },
                { name: 'BpkbRemind', text: 'BPKB Reminder', type: 'popup', icon: 'fa-hand-o-right', 'class': 'span6', readonly: true },
                { name: 'StnkExt', text: 'STNK Extension', type: 'popup', icon: 'fa-hand-o-right', 'class': 'span6', readonly: true },
            ]
        },
        {
            name: "pnl3DayCall",
            title: "3 Days Call List",
            xtype: "k-grid",
        },
        {
            name: "pnlBirthDayCall",
            title: "BirthDay Call List",
            xtype: "k-grid",
        },
        {
            name: "pnlStnkExt",
            title: "STNK Extension List",
            xtype: "k-grid",
        },
        {
            name: "pnlBpkb",
            title: "BPKB Reminder List",
            xtype: "k-grid",
        },
    ],
    onClick: function (action) {
        console.log(action);
    },
    onRendered: function () {
        var init = { ReminderDate: moment().format('DD-MMM-YYYY') };
        sdms.populate(init);
    }
});

