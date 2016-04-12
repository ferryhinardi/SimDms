Ext.define('DMS.view.main.Main', {
    extend: 'Ext.container.Viewport',
    alias: 'widget.mainviewport',

    requires:[
        'Ext.tab.Panel',
        'Ext.layout.container.*',
        'Ext.toolbar.Breadcrumb',
        'Ext.button.*',
        'Ext.form.*'
    ],

    controller: 'main',
    viewModel: 'main',

    layout: 'border',
    stateful: true,
    stateId: 'DMS-viewport',

    items: [{
            region: 'north',
            xtype: 'appHeader'
        }, 
        {
            xtype: 'navigation-tree',
            width: 200,
            collapsible: true,
            region: 'west'
        },
        {
            id: 'contentPanel',
            region: 'center',
            xtype: 'tabpanel',             
            itemId: 'contentPanel',    
            reference: 'contentPanel',   
            plugins: ['tabclosemenu'],
            listeners: {
                tabchange: function(panel, newTab, oldTab) {
                    document.title = document.title.split(' - ')[0] + ' - ' + newTab.title;
                }
            },
            items:[{
                title: 'Dashboard',
                iconCls: 'icon-rpt-operation',
                xtype: 'view-dealer-online'
            }]
        },        
        {
            xtype: 'container',
            region: 'south',
            height: 20,
            style: 'border-top: 1px solid #4c72a4;',
            html: ''
        }
    ],

    destroy : function(e, opt)
    {
        // console.log('viewport destroy');
    }
});
