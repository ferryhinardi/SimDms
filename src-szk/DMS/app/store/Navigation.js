Ext.define('DMS.store.Navigation', {
    extend: 'Ext.data.TreeStore',
    alias: 'store.navigation',

    constructor: function(config) {
        var me = this,
            queryParams = Ext.Object.fromQueryString(location.search);

        me.callParent([Ext.apply({
            root: {
                text: 'All',
                id: 'all',
                expanded: true,
                children: me.getNavItems()
            }
        }, config)]);
    },
  
    getNavItems: function() {
        return [       
            {
                text: 'Dealer Management Studio',
                id: 'msg-core',
                iconCls : 'icon-company',
                expanded: true,
                children: [
                    { id: 'view-dealer-online',    text: 'Status Dealer', iconCls : 'icon-sch-sorting',      leaf: true },
                    { id: 'view-dealer-management',    text: 'Data Management', iconCls : 'icon-material',      leaf: true }
                ]
            }                     
        ];
    }
});
