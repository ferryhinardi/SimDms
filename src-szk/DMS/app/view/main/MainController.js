Ext.define('DMS.view.main.MainController', {
    extend: 'Ext.app.ViewController',
    alias: 'controller.main',
    
    
    appLogingOut: function(button, e, options) 
    {
        button.up('mainviewport').destroy();
        window.location.reload();
    }, 

    createTab: function(tabId)
    {
    	alert(tabId);
    }

});
