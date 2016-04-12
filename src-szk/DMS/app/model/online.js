Ext.define('DMS.model.Online', {
     extend: 'DMS.model.Base',
        idProperty:'DealerCode', fields: [      
            { name: 'DealerCode', type: 'string' }, 
            { name: 'CompanyCode', type: 'string' }, 
            { name: 'CompanyName', type: 'string' }, 
            { name: 'ProductType', type: 'string' }, 
            { name: 'Status', type: 'boolean' }, 
            { name: 'SessionId', type: 'string' }, 
            { name: 'Location', type: 'string' }, 
            { name: 'LastProccess', type: 'string' }, 
            { name: 'LastUpdate', type: 'date', dateFormat: 'c' }
        ] ,
        proxy: {
            type: 'ajax',
            api: {
                read: 'api/dealeronline'
            },
            reader: {
                type: 'json',
                rootProperty: 'Result'
            },
            actionMethods: {
              read: 'POST'
            }
        }
});