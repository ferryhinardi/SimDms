Ext.define('DMS.store.Onlines', {
    extend: 'Ext.data.Store',
    model: 'DMS.model.Online',
    autoLoad: true,
    pageSize: 1000
});