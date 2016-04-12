var menu_grid = new Ext.menu.Menu({
    items:
    [
        { text: 'Add', handler: function () { console.log("Add"); } },
        { text: 'Delete', handler: function () { console.log("Delete"); } }
    ]
});

Ext.define("DMS.view.dealer.Onlines", {
    extend: "Ext.grid.Panel",

    requires: [
        'Ext.grid.filters.Filters',
        'Ext.grid.feature.Grouping'
    ],

    xtype: 'view-dealer-online',
    header: false,
    title: 'Dealer Status',

    loadMask: true,
    autoheight: true,
    itemId: 'grid-dealer-online',
    plugins: 'gridfilters',

    initComponent: function() {

        this.store = new DMS.store.Onlines();

        this.columns = [
            { header: '#', xtype: 'rownumberer', width: 33},
            { header: 'Dealer Code', dataIndex: 'DealerCode', width: 100, filter: { type: 'string' } },
            //{ header: 'Company Code', dataIndex: 'CompanyCode', width: 100, filter: { type: 'string' } },
            { header: 'Company Name', dataIndex: 'CompanyName', width: 150,  flex: 1,filter: { type: 'string' } },
            { header: 'Type', dataIndex: 'ProductType', width: 45, filter: { type: 'string' } },
            //{ header: 'Session', dataIndex: 'SessionId', width: 50, filter: { type: 'string' } },

            { header: 'Location', dataIndex: 'Location', width: 110, filter: { type: 'string' } },
            { header: 'Status', dataIndex: 'Status', width: 50,
                filter: { type: 'boolean', defaultValue: null, yesText: 'Finish', noText: 'In Progress' },
                    renderer: function(value, metaData, record, rowIndex, colIndex, store) {
                        if (record.data.Status)
                        {
                            return '<div align="center" valign="middle"><img src="resources/icons/check1.png" /></div>';
                        } else {
                            return '<div align="center" valign="middle"><img src="resources/icons/stop-red-icon.png" /></div>';
                        }
                    }
            },
            { header: 'Info', dataIndex: 'LastProccess', width: 200, filter: { type: 'string' } },
            { header: 'Last Update', dataIndex: 'LastUpdate', width: 125, renderer: displayDate }
        ];

         // paging bar on the bottom
        this.bbar = Ext.create('Ext.PagingToolbar', {
            store: this.store,
            displayInfo: true,
            displayMsg: 'Displaying Dealer(s) {0} - {1} of {2}',
            emptyMsg: "No Dealer to display",
            plugins: Ext.create('Ext.ux.ProgressBarPager', {})
        });


        this.callParent(arguments);
    }


    , listeners : {
        itemdblclick: function(dv, record, item, index, e) {
          var link = "mstsc.exe /v:" + record.data.Location;
          console.log(link);
            if(record.data.Location != ""){
              var WshShell = new ActiveXObject("WScript.Shell");
                if (WshShell !== null && WshShell !== undefined)
                {                  
                  WshShell.Exec(link );
                }
            }

        },
        containercontextmenu: function (grid, e) {
            var position = e.getXY();
            e.stopEvent();
            //menu_grid.showAt(position);

                console.log("containercontextmenu");
            
        }
    },

});
