/**/

'use strict;';

function showFailure(response, errorMsg) {
        var bodySize = Ext.getBody().getViewSize(),
            width = (bodySize.width < 432) ? bodySize.width - 32 : 432,
            height = (bodySize.height < 234) ? bodySize.height - 34 : 234,
            win;

        if (Ext.isEmpty(errorMsg)) {
            errorMsg = response.responseText;
            if (errorMsg == "")
            {
                errorMsg = "Service Not Found.";
            } else {
                var x = Ext.util.JSON.decode(errorMsg);
                if (x.messageDetail !== undefined)
                {
                    errorMsg = x.messageDetail;
                }
            }
        }

        win = new Ext.window.Window({ 
            modal: true, 
            width: width, 
            height: height, 
            title: "Request Failure", 
            layout: "fit", 
            maximizable: true,            
            items : [{
                xtype:"container",
                padding: '0 0 0 0',                        
                layout  : {
                    type: "vbox",
                    align: "stretch"
                },                
                items : [
                    {
                        xtype: "container",
                        height: 42,
                        layout: "absolute",
                        defaultType: "label",
                        items: [
                            {
                                xtype : "component",
                                x    : 5,
                                y    : 5,
                                html : '<div class="x-message-box-error" style="width:32px;height:32px"></div>'
                            },
                            {
                                x    : 42,
                                y    : 6,
                                html : "<b>Status Code: </b>"
                            },
                            {
                                x    : 125,
                                y    : 6,
                                text : response.status
                            },
                            {
                                x    : 42,
                                y    : 25,
                                html : "<b>Status Text: </b>"
                            },
                            {
                                x    : 125,
                                y    : 25,
                                text : response.statusText
                            }  
                        ]
                    },                    
                    {
                        flex: 1,
                        layout: 'fit',
                        itemId : "__ErrorMessageEditor",
                        xtype    : "htmleditor",
                        value    : errorMsg,
                        readOnly : true,
                        enableAlignments : false,
                        enableColors     : false,
                        enableFont       : false,
                        enableFontSize   : false,
                        enableFormat     : false,
                        enableLinks      : false,
                        enableLists      : false,
                        enableSourceEdit : false
                    }
                ]
            }]
        });

       
        win.show();
};

console.log("test");

function GetData(p, onAjaxReturn ){

    var dataPanel = Ext.getCmp('gridView-profitLoss');
    Ext.Ajax.on('beforerequest',function(conn,o,result){
        dataPanel.getEl().mask('Loading ...','x-mask-loading');
    });

    Ext.Ajax.on('requestcomplete',function(conn,o,result){
        dataPanel.getEl().unmask(true);
    });

    Ext.Ajax.on('requestexception',function(conn,o,result){
        dataPanel.getEl().unmask(true);
        showFailure(o);
    });

    Ext.Ajax.request({
        url : "sp.api/inquiry/InquirySparepartAnalisys",
        method : "POST",
        params : p,
        callback: onAjaxReturn
    });
};

console.log("test 1");

//*/

Ext.define('SDMS.model.Base', {
    extend: 'Ext.data.Model',
    schema: {
        namespace: 'SDMS.model'
    }
});

Ext.define('SDMS.model.Combo', {
    extend: 'SDMS.model.Base',
    fields: [
        'code',
        'description'
    ]
});

Ext.define('SDMS.store.PrintType', 
{
    extend: 'Ext.data.ArrayStore',
    alias: 'store.printtype',
    model: 'SDMS.model.Combo',    
    storeId: 'printtype', 
    data: [
            [0, '01', 'Sparepart Analysis Report']
          ]
});

Ext.define('SDMS.model.MetaProfitloss', {
    extend: 'SDMS.model.Base',
    fields: [
        'display',
        'quarter',
        'region'
    ]
});


Ext.define('SDMS.store.ItemType', {
    extend: 'Ext.data.ArrayStore',
    alias: 'store.itemtype',
    model: 'SDMS.model.Combo',    
    storeId: 'itemtype',    
    data: [
        [0, '0', 'NON SGA'],
        [1, '1', 'SGP'],
        [2, '2', 'SGO'],
        [3, '3', 'SGA'],
        [4, '4', 'NON SGP']
    ]
});

Ext.define('SDMS.store.Area', 
{
    extend: 'Ext.data.ArrayStore',
    alias: 'store.areas',
    model: 'SDMS.model.Combo',    
    storeId: 'areas', 
    data: [
            [0, '01', 'CABANG']
          ]
});

Ext.define('SDMS.store.Dealer', 
{
    extend: 'Ext.data.ArrayStore',
    alias: 'store.dealers',
    model: 'SDMS.model.Combo',    
    storeId: 'dealers', 
    data: [
            [0, '6006401', 'PT. BUANA INDOMOBIL TRADA'],
            [1, '5005501', 'PT. SISTEM BUANA TRADA']
          ]
});

Ext.define('SDMS.store.Outlet', 
{
    extend: 'Ext.data.ArrayStore',
    alias: 'store.outlets',
    model: 'SDMS.model.Combo',    
    storeId: 'outlets', 
    data: [
            [0, '6006401', 'PT. BUANA INDOMOBIL TRADA - PULO GADUNG' ],
            [1, '6006402', 'PT. BUANA INDOMOBIL TRADA - PONDOK INDAH']
          ]
});

Ext.define('SDMS.model.FullProfitloss', {
    extend: 'SDMS.model.Base',

    fields: [
        'Account',
        { name: 'JAN', type: 'int' },
        { name: 'FEB', type: 'int' },
        { name: 'MAR', type: 'int' },
        { name: 'APR', type: 'int' },
        { name: 'MAY', type: 'int' },
        { name: 'JUN', type: 'int' },
        { name: 'JUL', type: 'int' },
        { name: 'AUG', type: 'int' },
        { name: 'SEP', type: 'int' },
        { name: 'OCT', type: 'int' },
        { name: 'NOV', type: 'int' },
        { name: 'DEC', type: 'int' },
        'BranchName'
    ]
});

Ext.define('SDMS.store.ProfitLoss', {
    extend: 'Ext.data.Store',
    alias: 'store.profitloss',
    model: 'SDMS.model.FullProfitloss',
    proxy: {
        type: 'ajax',
        url: '/data/full_data.json',
        reader: 'json'
    }
});


Ext.define('SDMS.view.profitloss.ProfitLoss', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.profitloss',
    requires: [
        'Ext.grid.feature.Grouping',
        'SDMS.store.ProfitLoss'
    ],

    itemId: 'profitloss',
    cls: 'dynamic-pl-grid',

    controller: 'profitloss',

    viewModel: {
        type: 'profitloss'
    },

    columns: [],
    enableLocking: true,

    store: {
        type: 'profitloss',
        sorters: 'id',
        groupField: 'Account'
    },

    features: [{
        ftype: 'grouping',
        id: 'profitLossGrouper',
        groupHeaderTpl: '<b>{name}</b>',
        startCollapsed: false
    }],

    tbar: [{
        text: 'Periode',
        width: 150,
        textAlign: 'left',
        reference: 'quartersButton',
        menu: {
            id: 'period-menu',
            cls: 'pl-option-menu',
            items: []
        }
    },{
        text: 'Outlet',
        width: 150,
        textAlign: 'left',
        reference: 'regionsButton',
        menu: {
            id: 'outlet-menu',
            cls: 'pl-option-menu',
            items: []
        }
    },
    { xtype: 'tbfill' },
    { xtype: 'button', text: 'Run Query' , handler:'onRunQuery'},
    { xtype: 'button', text: 'Export To Excel', handler:'onExportExcel' }

    ],

    // These properties are aspects of the view that get used to create dynamic grid
    // columns and menu items.

    regionColumn: {
        text:'Outlet',
        dataIndex:'BranchName',
        menuDisabled: true,
        sortable: false,
        resizable: false,
        hideable: false,
        groupable: false,
        locked: true,

        plugins: 'responsive',
        responsiveConfig: {
            'width < 600': {
                width: 150
            },
            'width >= 600': {
                width: 250
            }
        }
    },

    menuItemDefaults: {
        checked: true,
        hideOnClick: false
    },

    quarterColumnDefaults: {
        formatter: 'currency',
        flex: 1,
        minWidth: 130,
        align: 'right',
        groupable: false,
        menuDisabled: true,
        resizable: false,
        sortable: false,
        summaryType: 'sum'
    },

    viewConfig: {
        listeners: {
            refresh: 'onViewRefresh'
        }
    }
});

Ext.define('SDMS.view.profitloss.ProfitLossController', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.profitloss',

    fnCallback : function(options, success, result) {

        var response = Ext.util.JSON.decode(result.responseText);
        console.log(response);
        if (!response.success )
        {
            Ext.MessageBox.show({
                title   : 'Error',
                msg     : '<div style="margin-top:5px;">' + response.message + '</div>',
                width   : 300,
                buttons : Ext.MessageBox.OK,
                animEl  : 'login',
                icon    : Ext.MessageBox.ERROR
            }); 
        } else {

        var me = this, view = me.getView();

            view.setColumns(columns);

            //view.store.load(); // displays loadMask so include in layout batch

            view.getStore().add(response.data);
            view.doLayout();
            //view.getViewModel().getStore().loadRecords(response.metadata);
        }

    },    

    onRunQuery : function()
    {

        var form = Ext.getCmp('formFilter').getForm();

        var data = form.getValues();

        $.extend(data, {'ItemTypeS': JSON.stringify(data.ItemType)} );

       // GetData(form.getValues(),fnCallback);
        Ext.Ajax.request({
            url : "sp.api/inquiry/InquirySparepartAnalisys",
            method : "POST",
            scope: this,
            params : data,
            callback: this.fnCallback
        });        
        
    },

    onExportExcel: function()
    {
        Ext.MessageBox.show({
            title: 'Address',
            msg: 'Please enter your address:',
            width:300,
            buttons: Ext.MessageBox.OKCANCEL,
            multiline: true,
            scope: this
        });
    },

    onMetaDataLoad: function (metaProfitLoss) {
        console.log(metaProfitLoss);
        var me = this,
            references = me.getReferences(),
            view = me.getView(),
            menus = {
                period: {
                    items: [],
                    listeners: {
                        click: me.onQuarterItemClick,
                        scope: me
                    }
                },
                outlet: {
                    items: [],
                    listeners: {
                        click: me.onRegionItemClick,
                        scope: me
                    }
                }
            },
            columns = [ view.regionColumn ];
console.log("preparing");

        metaProfitLoss.each(function (metaRecord) {
            var type = metaRecord.data.type,
                value = metaRecord.data.value;

            console.log(metaRecord);

            menus[type].items.push(Ext.apply({
                text: metaRecord.data.display,
                value: value,
                type: type,
                listeners: menus[type].listeners
            }, view.menuItemDefaults));

        });

        menus.region.items.sort(function (lhs, rhs) {
            return (lhs.text < rhs.text) ? -1 : ((rhs.text < lhs.text) ? 1 : 0);
        });

        // We want to tinker with the UI in batch so we don't trigger multiple layouts
        Ext.batchLayouts(function () {
            references.quartersButton.menu.add(menus.period.items);
            references.regionsButton.menu.add(menus.outlet.items);

            view.setColumns(columns);

            view.store.load(); // displays loadMask so include in layout batch
        });
    },

    onQuarterItemClick: function (menuItem) {
        var column = this.getView().getColumnManager().getHeaderByDataIndex(menuItem.value);
        column.setVisible(menuItem.checked);
    },

    onRegionItemClick: function () {
        var view = this.getView(),
            filter = {
                // The id ensures that this filter will be replaced by subsequent calls
                // to this method (while leaving others in place).
                id: 'regionFilter',
                property: 'BranchCode',
                operator: 'in',
                value: []
            },
            regionMenu = this.lookupReference('regionsButton').menu;

        regionMenu.items.each(function (item) {
            if (item.checked) {
                filter.value.push(item.value);
            }
        });

        if (filter.value.length === regionMenu.items.length) {
            // No need for a filter that includes everything, so remove it (in case it
            // was there - harmless if it wasn't)
            view.store.getFilters().removeByKey(filter.id);
        } else {
            view.store.getFilters().add(filter);
        }
    },

    // Fix an issue when using touch scrolling and hiding columns, occasionally
    // there is an issue wher the total scroll size is not updated.
    onViewRefresh: function(view) {
        if (view.ownerGrid.normalGrid === view.ownerCt) {
            var scrollManager = view.scrollManager,
                scroller;

            if (scrollManager) {
                scroller = scrollManager.scroller;
                scroller.setSize('auto');
                scroller.refresh();
            }
        }
    }
});

Ext.define('SDMS.view.profitloss.ProfitLossModel', {
    extend: 'Ext.app.ViewModel',
    alias: 'viewmodel.profitloss',

    requires: [
        'SDMS.model.MetaProfitloss',
        'SDMS.model.FullProfitloss'
    ],

    stores: {
        metaProfitLoss: {
            model: 'SDMS.model.MetaProfitloss',
            autoLoad: false,
            listeners: {
                load: 'onMetaDataLoad'
            }
        }
    }
});


Ext.create('Ext.container.Viewport', {
    layout: 'fit',
    items: [
        {
            layout: 'border',
            bodyBorder: false,
            xtype: 'panel',
            margin: '0 5 0 5',

            defaults: {
                collapsible: true,
                split: { height: 1, collapsible: false },
                bodyPadding: 1
            },          

             items: [
                {
                    title: { text:'Filter', height:10},
                    region: 'north',
                    height: 132,
                    maxHeight: 132,
                    //titleCollapse: true,                 
                    bodyPadding: '5 5 0',   
                    xtype: 'form',
                    id: 'formFilter',
                    
                    defaults: {
                        border: false,
                        xtype: 'panel',
                        flex: 1,
                        layout: 'anchor'
                    },

                    layout: 'hbox',
                    
                    items: [{
                        defaults: {
                            labelAlign: 'right',
                            labelWidth: 80
                        },                        
                        margin: '0 6 0 6',
                        items: [{
                                    xtype: 'textfield',
                                    fieldLabel: 'Periode',
                                    //anchor: '100%',
                                    width: 140,
                                    name: 'Periode',
                                    value: '2014'
                                }, {
                                    id: 'cboPrintType',
                                    xtype:'combobox',
                                    fieldLabel: 'Print Type',
                                    anchor: '100%',
                                    name: 'PrintType',
                                    displayField: 'description',
                                    valueField: 'code',
                                    store: {
                                        type: 'printtype'
                                    },                            
                                    queryMode: 'local',
                                    editable: false,
                                    triggerAction: 'all',
                                    value: '01'
                                }, {
                                    id: 'cboItemType',
                                    name: 'ItemType',
                                    fieldLabel: 'Item Type',
                                    anchor: '100%',
                                    xtype: 'tagfield',
                                    store: {
                                        type: 'itemtype'
                                    },
                                    displayField: 'description',
                                    valueField: 'code',
                                    //filterPickList: true,
                                    editable: false,
                                    queryMode: 'local',
                                    publishes: 'value',
                                    value: ['0','1','2','3','4']
                                }
                            ]
                    }, {
                        margin: '0 6 0 6',
                        defaults: {
                            labelAlign: 'right',
                            labelWidth: 80
                        },                       
                        items: [{
                                    id: 'cboArea',
                                    xtype:'combobox',
                                    fieldLabel: 'Area',
                                    //anchor: '100%',
                                    name: 'Area',
                                    displayField: 'description',
                                    valueField: 'code',
                                    store: {
                                        type: 'areas'
                                    },                            
                                    queryMode: 'local',
                                    editable: false,
                                    triggerAction: 'all',
                                    listeners: { 'expand': function(self) { self.clearValue(); } }
                                }, {
                                    id: 'cboDealer',
                                    xtype:'combobox',
                                    fieldLabel: 'Dealer',
                                    anchor: '100%',
                                    name: 'Dealer',
                                    displayField: 'description',
                                    valueField: 'code',
                                    store: {
                                        type: 'dealers'
                                    },                            
                                    queryMode: 'local',
                                    editable: false,
                                    triggerAction: 'all'
                                    
                                },{
                                    id: 'cboOutlet',
                                    xtype:'combobox',
                                    fieldLabel: 'Outlet',
                                    anchor: '100%',
                                    name: 'Outlet',
                                    displayField: 'description',
                                    valueField: 'code',
                                    store: {
                                        type: 'outlets'
                                    },                            
                                    queryMode: 'local',
                                    editable: false,
                                    triggerAction: 'all'
                                    
                            }]
                        }]
                },
                {
                    id: 'gridView-profitLoss',
                    xtype: 'profitloss',
                    collapsible: false,
                    region: 'center',
                    autoheight: true
                } 
             ]
        },
    ]
});

