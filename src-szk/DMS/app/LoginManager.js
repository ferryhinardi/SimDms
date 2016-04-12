/**
 * This class manages the login process.
 */
Ext.define('DMS.LoginManager', {
    config: {
        model: null,
        session: null
    },

    constructor: function (config) {
        this.initConfig(config);
    },

    applyModel: function(model) {
        return model;
    },

    login: function(options) 
    {
        if ( options.data.remember )
        {
            localStorage["JSMS_SAVE"] = JSON.stringify(options.data);            
        } else {
            localStorage["JSMS_SAVE"] = "";
        }

        Ext.Ajax.request({
            url: 'token',
            method: 'POST',
            params: options.data,
            scope: this,
            callback: this.onLoginReturn,
            original: options
        });
    },
    
    onLoginReturn: function(options, success, result) {
        options = options.original;
        var session = this.getSession(),
            resultSet;

        var response = Ext.util.JSON.decode(result.responseText);
        
        if (response.access_token !== undefined )
        {
            GlobalShared.token = response;
            GlobalShared.login = 1;
            GlobalShared.userLogin = options.data.userName
            localStorage["JSMS_AUTH"] = response;
            Ext.callback(options.success, options.scope, '');
            Packt.util.SessionMonitor.start();
            return;
        } 
        else 
        {
            Packt.util.Util.showErrorMsg(result.responseText); 
        }

        Ext.callback(options.failure, options.scope, [result, response]);
    }
});
