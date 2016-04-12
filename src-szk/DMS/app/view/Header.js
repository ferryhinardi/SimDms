Ext.define('DMS.view.Header', {
    extend: 'Ext.Container',
    xtype: 'appHeader',
    id: 'app-header',
    title: 'DEALER MANAGEMENT STUDIO',
    height: 54,
    layout: {
        type: 'hbox',
        align: 'middle'
    },

    initComponent: function () {
        document.title = this.title;

        this.items = [
                {
                    xtype: 'component',
                    id: 'app-header-logo',
                    glyph: 'xf0e0'
                },
                {
                    xtype: 'component',
                    id: 'app-header-title',
                    html: this.title,
                    flex: 1
                }
//              ,{
//                    xtype: 'component',
//                    html: this.title                
//               }
            ];

        this.callParent();
    }
});
