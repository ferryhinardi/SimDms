Ext.require([
    'Ext.grid.*',
    'Ext.data.*',
    'Ext.form.field.Number',
    'Ext.form.field.Date',
    'Ext.tip.QuickTipManager',
    'Ext.layout.container.Fit']);

Ext.define('SDMS.store.TPGO', {
    extend: 'Ext.data.Store', model: 'SDMS.model.Combo',
    alias:  'store.tpgos', 
    proxy: {
        type:   'ajax',
        reader: 'json',
        url:    'sp.api/combo/loadlookup?CodeId=TPGO'
    },  autoLoad: true
});

Ext.define('SDMS.store.PrintType', {
    extend: 'Ext.data.Store', model: 'SDMS.model.Combo',
    alias:  'store.printtypes', 
    proxy: {
        type:   'ajax',
        reader: 'json',
        url:    'sp.api/combo/loadlookup?CodeId=INQSPR'
    },  autoLoad: true
});

Ext.define('SDMS.store.Area', {
    extend: 'Ext.data.Store', model: 'SDMS.model.Combo',
    alias:  'store.areas', 
    proxy: {
        type:   'ajax',
        reader: 'json',
        url:    'sp.api/combo/comboado'
    },  autoLoad: true
});

Ext.define('SDMS.store.Dealer', {
    extend: 'Ext.data.Store', model: 'SDMS.model.Combo',
    alias:  'store.dealers', 
    proxy: {
        type:   'ajax',
        reader: 'json',
        url:    'sp.api/combo/comboado?TypeID=2'
    }//,  autoLoad: true
});

Ext.define('SDMS.store.Outlet', {
    extend: 'Ext.data.Store', model: 'SDMS.model.Combo',
    alias:  'store.outlets', 
    proxy: {
        type:   'ajax',
        reader: 'json',
        url:    'sp.api/combo/comboado?TypeID=3'
    }//,  autoLoad: true
}); 

Ext.define('SDMS.view.ContentPanel', {
    extend: 'Ext.tab.Panel',
    requires: [
        'Ext.layout.container.Fit'
    ],  
    xtype: 'contentPanel',
    id: 'content-panel',
    layout: 'fit',    
    autoScroll: false
});


Ext.define('Task', {
    extend: 'Ext.data.Model',
    idProperty: 'taskId',
    fields: [{
        name: 'projectId',
        type: 'int'
    }, {
        name: 'project',
        type: 'string'
    }, {
        name: 'taskId',
        type: 'int'
    }, {
        name: 'description',
        type: 'string'
    }, {
        name: 'estimate',
        type: 'float'
    }, {
        name: 'rate',
        type: 'float'
    }, {
        name: 'due',
        type: 'date',
        dateFormat: 'm/d/Y'
    }]
});

var data = [{
    projectId: 100,
    project: 'Ext Forms: Field Anchoring',
    taskId: 112,
    description: 'Integrate 2.0 Forms with 2.0 Layouts',
    estimate: 6,
    rate: 150,
    due: '06/24/2007'
}, {
    projectId: 100,
    project: 'Ext Forms: Field Anchoring',
    taskId: 113,
    description: 'Implement AnchorLayout',
    estimate: 4,
    rate: 150,
    due: '06/25/2007'
}, {
    projectId: 100,
    project: 'Ext Forms: Field Anchoring',
    taskId: 114,
    description: 'Add support for multiple types of anchors',
    estimate: 4,
    rate: 150,
    due: '06/27/2007'
}, {
    projectId: 100,
    project: 'Ext Forms: Field Anchoring',
    taskId: 115,
    description: 'Testing and debugging',
    estimate: 8,
    rate: 0,
    due: '06/29/2007'
}, {
    projectId: 101,
    project: 'Ext Grid: Single-level Grouping',
    taskId: 101,
    description: 'Add required rendering "hooks" to GridView',
    estimate: 6,
    rate: 100,
    due: '07/01/2007'
}, {
    projectId: 101,
    project: 'Ext Grid: Single-level Grouping',
    taskId: 102,
    description: 'Extend GridView and override rendering functions',
    estimate: 6,
    rate: 100,
    due: '07/03/2007'
}, {
    projectId: 101,
    project: 'Ext Grid: Single-level Grouping',
    taskId: 103,
    description: 'Extend Store with grouping functionality',
    estimate: 4,
    rate: 100,
    due: '07/04/2007'
}, {
    projectId: 101,
    project: 'Ext Grid: Single-level Grouping',
    taskId: 121,
    description: 'Default CSS Styling',
    estimate: 2,
    rate: 100,
    due: '07/05/2007'
}, {
    projectId: 101,
    project: 'Ext Grid: Single-level Grouping',
    taskId: 104,
    description: 'Testing and debugging',
    estimate: 6,
    rate: 100,
    due: '07/06/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 105,
    description: 'Ext Grid plugin integration',
    estimate: 4,
    rate: 125,
    due: '07/01/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 106,
    description: 'Summary creation during rendering phase',
    estimate: 4,
    rate: 125,
    due: '07/02/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 107,
    description: 'Dynamic summary updates in editor grids',
    estimate: 6,
    rate: 125,
    due: '07/05/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 108,
    description: 'Remote summary integration',
    estimate: 4,
    rate: 125,
    due: '07/05/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 109,
    description: 'Summary renderers and calculators',
    estimate: 4,
    rate: 125,
    due: '07/06/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 110,
    description: 'Integrate summaries with GroupingView',
    estimate: 10,
    rate: 125,
    due: '07/11/2007'
}, {
    projectId: 102,
    project: 'Ext Grid: Summary Rows',
    taskId: 111,
    description: 'Testing and debugging',
    estimate: 8,
    rate: 125,
    due: '07/15/2007'
}];


    var store = Ext.create('Ext.data.Store', {
        model: 'Task',
        data: data,
        sorters: {
            property: 'due',
            direction: 'ASC'
        },
        groupField: 'project'
    });

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });
    var showSummary = true;

    var grid = Ext.create('Ext.grid.Panel', {
        frame: true,
        // title: 'Sponsored Projects',
        margin: '-40 0 2 0',
        iconCls: 'icon-grid',
        store: store,
        plugins: [cellEditing],
        selModel: {
            selType: 'cellmodel'
        },
        dockedItems: [{
            dock: 'top',
            xtype: 'toolbar',
            items: [{
                tooltip: 'Toggle the visibility of the summary row',
                text: 'Toggle Summary',
                enableToggle: true,
                pressed: true,
                handler: function () {
                    showSummary = !showSummary;
                    var view = grid.lockedGrid.getView();
                    view.getFeature('group').toggleSummaryRow(showSummary);
                    view.refresh();
                    view = grid.normalGrid.getView();
                    view.getFeature('group').toggleSummaryRow(showSummary);
                    view.refresh();
                }
            }]
        }],
        features: [{
            id: 'group',
            ftype: 'groupingsummary',
            groupHeaderTpl: '{name}',
            hideGroupedHeader: true,
            enableGroupingMenu: false
        }],
        columns: [{
            text: 'Task',
            width: 300,
            locked: true,
            tdCls: 'task',
            sortable: true,
            dataIndex: 'description',
            hideable: false,
            summaryType: 'count',
            summaryRenderer: function (value, summaryData, dataIndex) {
                return ((value === 0 || value > 1) ? '(' + value + ' Tasks)' : '(1 Task)');
            }
        }, {
            header: 'Project',
            width: 180,
            sortable: true,
            dataIndex: 'project'
        }, {
            header: 'Due Date',
            width: 130,
            sortable: true,
            dataIndex: 'due',
            summaryType: 'max',
            renderer: Ext.util.Format.dateRenderer('m/d/Y'),
            summaryRenderer: Ext.util.Format.dateRenderer('m/d/Y'),
            field: {
                xtype: 'datefield'
            }
        }, {
            header: 'Estimate',
            width: 130,
            sortable: true,
            dataIndex: 'estimate',
            summaryType: 'sum',
            renderer: function (value, metaData, record, rowIdx, colIdx, store, view) {
                return value + ' hours';
            },
            summaryRenderer: function (value, summaryData, dataIndex) {
                return value + ' hours';
            },
            field: {
                xtype: 'numberfield'
            }
        }, {
            header: 'Rate',
            width: 130,
            sortable: true,
            renderer: Ext.util.Format.usMoney,
            summaryRenderer: Ext.util.Format.usMoney,
            dataIndex: 'rate',
            summaryType: 'average',
            field: {
                xtype: 'numberfield'
            }
        }, {
            header: 'Cost',
            width: 130,
            sortable: false,
            groupable: false,
            renderer: function (value, metaData, record, rowIdx, colIdx, store, view) {
                return Ext.util.Format.usMoney(record.get('estimate') * record.get('rate'));
            },
            summaryType: function (records, values) {
                var i = 0,
                    length = records.length,
                    total = 0,
                    record;

                for (; i < length; ++i) {
                    record = records[i];
                    total += record.get('estimate') * record.get('rate');
                }
                return total;
            },
            summaryRenderer: Ext.util.Format.usMoney
        }]
    });

Ext.define('SDMS.view.SparePartInqueryController', 
{
    extend: 'Ext.app.ViewController',
    alias: 'controller.sparepartInquiry',

    HowToInsertContent: function()
    {
        var me = this, contentPanel = me.getContentPanel();

        Ext.suspendLayouts();

        contentPanel.removeAll(true);

        //cmp = new ViewClass();

        //contentPanel.add(cmp);

        Ext.resumeLayouts(true);

        // if (cmp.floating) {
        //     cmp.show();
        // }

    },

    onRawDataClicked: function()
    {
        var me = this, view = me.getView();

        var param = view.down('form').getForm().getValues();
        var tabs = view.down('tabpanel');

        var tabId = "tab-rawdata";
        var tab = tabs.getComponent(tabId);
 
        if (!tab) {
            tab = tabs.add({
                id: tabId,
                title: 'Tab ' + (tabs.items.length + 1),
                layout: 'fit',    
                autoScroll: true,
                items: [grid]
            });
        }
 
        tabs.setActiveTab(tab);

        tab.tab.hide();

    },

    onPreviewClicked: function()
    {
        var me = this, view = me.getView();

        var param = view.down('form').getForm().getValues();
        var tabs = view.down('tabpanel');

        var tabId = "tab-preview";
        var tab = tabs.getComponent(tabId);
        var url = "http://localhost/dmsreport/viewer.aspx?id=SpRpTrn011Short&pparam='6006406','6006401','FPJ/11/000025','FPJ/11/000025','300', '10','0'&rparam=test";

        if (!tab) {
            tab = tabs.add({
                id: tabId,
                title: 'Tab ' + (tabs.items.length + 1),
                layout: 'fit',    
                items: [{
                    id: 'e-pdfreport2014',
                    xtype: 'component',
                    autoEl: {
                        tag: 'iframe',
                        style: 'height: 100%; width: 100%; border: none',
                        src: url
                    }
                }]        
            });
        }
 
        tabs.setActiveTab(tab);
        tab.tab.hide();

    },

    onExportToExcelClicked: function()
    {
        alert("Export To Excel CLicked");
    }

});


Ext.create('Ext.container.Viewport', {
    layout: 'fit',
    controller: 'sparepartInquiry',

    items: [
        {
            layout: 'border',
            bodyBorder: false,
            xtype: 'panel',
            margin: '0 5 10 5',

            defaults: {
                collapsible: true,
                split: { height: 1, collapsible: false },
                bodyPadding: '0 5 5 5'
            },          

             items: [
                {
                    title: { text:'Filter', height:10 },
                    region: 'north',
                    height: 158,
                    maxHeight: 300,
                    //titleCollapse: true,                 
                    bodyPadding: '5 5 10',   
                    xtype: 'form',
                    id: 'formFilter',
					viewModel: true,
					referenceHolder: true,
                    
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
                                    plugins: ['clearbutton'],
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
                                    displayField: 'text',
									valueField: 'value',
                                    store: {
                                        type: 'printtypes'
                                    },                            
                                    queryMode: 'remote',
                                    editable: true,
                                    triggerAction: 'all',
                                    value: '0'
                                }, {
                                    id: 'cboItemType',
                                    name: 'ItemType',
                                    fieldLabel: 'Item Type',
                                    anchor: '100%',
                                    xtype: 'tagfield',                                    
                                    store: {
                                        type: 'tpgos'
                                    },
                                    displayField: 'text',
									valueField: 'value',
                                    //filterPickList: true,
                                    editable: true,
                                    queryMode: 'remote',
                                    publishes: 'value',
                                    margin: '0 0 10',
                                    value: ['0','1','2','3','5'],
                                    listeners: { 
                                        autoSize: function(cb, height, eOpts )
                                        {
                                           console.log(height);  
                                        }
                                    }
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
									reference: 'area',
                                    xtype:'combobox',
                                    fieldLabel: 'Area',
                                    plugins: ['clearbutton'],
                                    //anchor: '100%',
                                    name: 'Area',
                                    displayField: 'text',
									valueField: 'value',									
									publishes: 'value',
                                    store: {
                                        type: 'areas'
                                    },                            
                                    queryMode: 'remote',
                                    editable: true,
                                    triggerAction: 'all',
                                    listeners: { 
                                        change: function (cb, newValue, oldValue, eOpts )
                                        {
											var cb = Ext.getCmp('cboDealer');
											cb.clearValue();
											cb.setDisabled(false);
											var cb2 = Ext.getCmp('cboOutlet');
											cb2.clearValue();
											//alert(combo.getValue());											
										}
									}
                                }, {
                                    id: 'cboDealer',
									reference: 'dealer',
                                    plugins: ['clearbutton'],
									//disabled: true,
                                    xtype:'combobox',
                                    fieldLabel: 'Dealer',
                                    anchor: '100%',
                                    name: 'Dealer',
									displayField: 'text',
									valueField: 'value',
									publishes: 'value',
                                    store: {
                                        type: 'dealers'
                                    },                            
                                    queryMode: 'remote',
                                    editable: true,
                                    triggerAction: 'all',
									bind: {
										//visible: '{area.value}',
										filters: {
											property: 'parent',
											value: '{area.value}'
										}
									},
                                    listeners: { 
										change: function (cbo, newValue, oldValue, eOpts )
                                        {
											var cb = Ext.getCmp('cboOutlet');
											cb.setDisabled(false);
											cb.clearValue();
											cb.getStore().load({
												params:{  
													pCode: cbo.getValue(),
													TypeID:3
												}  
											}); 
										}
									}									
                                },{
                                    id: 'cboOutlet',
									reference: 'outlet',
                                    plugins: ['clearbutton'],
									//disabled: true,
                                    xtype:'combobox',
                                    fieldLabel: 'Outlet',
                                    anchor: '100%',
                                    name: 'Outlet',
                                    displayField: 'text',
                                    valueField: 'value',
                                    store: {
                                        type: 'outlets'
                                    },                            
                                    queryMode: 'local',
                                    editable: true,
                                    triggerAction: 'all'
                                }, {

                                    layout: 'hbox',

                                    margin: '5 5 5 85',

                                    defaults: {
                                        bodyPadding: 2,
                                        margin: '0 2 5 2'
                                    },

                                    items:[
                                    {
                                       xtype: 'button',
                                       text: 'Raw Data', handler: 'onRawDataClicked' 
                                    },
                                    {
                                       xtype: 'button',
                                       text: 'Preview', handler: 'onPreviewClicked' 
                                    },
                                     {
                                       xtype: 'button',
                                       text: 'Export To Excel', handler: 'onExportToExcelClicked' 
                                    }                                   
                                    ]

                                }
                                ]
                        }]
                }, 
                {
                    region: 'center',
                    xtype: 'contentPanel',
                    reference: 'contentPanel',
                    collapsible: false,
                    margin: '0 0 0 0',
                    header: {
                        hidden: true
                    }
                } 
             ]
        },
    ]
});