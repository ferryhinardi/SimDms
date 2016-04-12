Ext.define('DMS.controller.Root', {
    extend: 'Ext.app.Controller',
    
    requires: [
        'DMS.view.login.Login',
        'DMS.view.main.Main',
        'Ext.window.*',
        'DMS.LoginManager'
    ],

    config: {
        control: {
            'navigation-tree': {
                selectionchange: 'onTreeNavSelectionChange',
                itemclick: 'onTreeNavItemClick'
            }
        },
        refs: {
            viewport: 'viewport',
            navigationTree: 'navigation-tree'
        },
        routes  : {
            ':id': {
                action: 'handleRoute',
                before: 'beforeHandleRoute'
            }
        }
    },

    beforeHandleRoute: function(id, action) {
        var me = this,
            node = Ext.StoreMgr.get('navigation').getNodeById(id);

        if (node) {
            //resume action
            action.resume();
        } else {
            Ext.Msg.alert(
                'Route Failure',
                'The view for ' + id + ' could not be found. You will be taken to the application\'s start',
                function() {
                    me.redirectTo(me.getApplication().getDefaultToken());
                }
            );
            //stop action
            action.stop();
        }
    },

    onTreeNavItemClick: function ( sender, record, item, index, e, eOpts )
    {
        if (record !== null && record !== undefined)
        {
            this.handleRoute(record.data.id);
        }
    },

    handleRoute: function(id) {
        var me = this,
            navigationTree = me.getNavigationTree(),
            store = Ext.StoreMgr.get('navigation'),
            node = store.getNodeById(id),
            text = node.get('text'), className = undefined;

            var view = me.getViewport();

        if (node.isLeaf() && view !== undefined)
        {
            className = Ext.ClassManager.getNameByAlias('widget.' + id);    
            if (className !== undefined)
            {
                
                var tabs = view.lookupReference('contentPanel');
                var tab = tabs.items.getByKey(id);

                if (!tab) {
                    var cfg = {
                        xtype: id, itemId : id, closable : true, text: text, iconCls: node.get('iconCls')
                    };
                    tab = tabs.add(cfg);
                }

                tabs.setActiveTab(tab);
                document.title = document.title.split(' - ')[0] + ' - ' + text;                
            }
        }

    },
    
    exampleRe: /^\s*\/\/\s*(\<\/?example\>)\s*$/,
    themeInfoRe: /this\.themeInfo\.(\w+)/g,

    onSetRegion: function (tool) {
        var panel = tool.toolOwner;

        var regionMenu = panel.regionMenu || (panel.regionMenu =
            Ext.widget({
                xtype: 'menu',
                items: [{
                    text: 'North',
                    checked: panel.region === 'north',
                    group: 'mainregion',
                    handler: function () {
                        panel.setBorderRegion('north');
                    }
                },{
                    text: 'South',
                    checked: panel.region === 'south',
                    group: 'mainregion',
                    handler: function () {
                        panel.setBorderRegion('south');
                    }
                },{
                    text: 'East',
                    checked: panel.region === 'east',
                    group: 'mainregion',
                    handler: function () {
                        panel.setBorderRegion('east');
                    }
                },{
                    text: 'West',
                    checked: panel.region === 'west',
                    group: 'mainregion',
                    handler: function () {
                        panel.setBorderRegion('west');
                    }
                }]
            }));

        regionMenu.showBy(tool.el);
    },

    onTreeNavSelectionChange: function(selModel, records) {
        var record = records[0];
        if (record) {
            this.redirectTo(record.getId());
        }
    },

    onBreadcrumbNavSelectionChange: function(breadcrumb, node) {
        if (node) {
            this.redirectTo(node.getId());
        }
    },

    onThumbnailClick: function(view, node) {
        this.redirectTo(node.getId());
    },

    loadingText: 'Loading...',

       splashscreen: {},

    //autoCreateViewport: true,

    init: function() {

        // Start the mask on the body and get a reference to the mask
         splashscreen = Ext.getBody().mask('Loading application', 'splashscreen');

        // Add a new class to this mask as we want it to look different from the default.
         splashscreen.addCls('splashscreen');

        // Insert a new div before the loading icon where we can place our logo.
        Ext.DomHelper.insertFirst(Ext.query('.x-mask-msg')[0], {
            cls: 'x-splash-icon'
        });
        
        var socketClient = io('http://tbsdmsap01:9091');
        
        socketClient.on('connect', function(){	
	
		socketClient.emit('add user','MGMTSTD', 'MGMTSTD', 'MGMTSTD', true);	
		
		socketClient.on('login', function(info){
			console.log('login', info);
		});		
		
		socketClient.on('user joined', function(info){
			console.log('user joined', info);
		});	
		
		socketClient.on('user left', function(info){
			console.log('user left', info);
		});	
		
		socketClient.on('pm', function(from, command){
			console.log('pm: ', from,' > ', command);			
			socketClient.emit('reply',from,command + ' result');
		});	

		socketClient.on('ping', function(info){
			console.log('ping: ', info);
		});	

		socketClient.on('log', function(info){
			console.log('log: ', info);
		});
		
		socketClient.on('command', function(from, command){

		});	
		
		
	});

    },
   
    onLaunch: function () {

        if (Ext.isIE8) {
            Ext.Msg.alert('Not Supported', 'This example is not supported on Internet Explorer 8. Please use a different browser.');
            return;
        }

        Ext.tip.QuickTipManager.init();

        var me = this;

        var task = new Ext.util.DelayedTask(function() {

            //Fade out the body mask
            splashscreen.fadeOut({
                //remove:true,
                duration: 500
            });

            //Fade out the icon and message
            splashscreen.next().fadeOut({
                duration: 1000,
                //remove:true,
                listeners: {
                    afteranimate: function(el, startTime, eOpts ){

                        var session = me.session = new Ext.data.Session();
                        
                        if (GlobalShared.autoLogin)
                        {
                            Packt.util.SessionMonitor.start();
                            me.viewport = new DMS.view.main.Main({
                                session: me.session,
                                viewModel: {
                                    data: {                    
                                        currentUser: 'Admin'
                                    }
                                }
                            });            
                        } else {
                            me.login = new DMS.view.login.Login({
                                session: session,
                                autoShow: true,
                                listeners: {
                                    scope: me,
                                    login: 'onLogin'
                                }
                            });            
                        }
                        
                    }
                }
            });

           // Ext.widget('mainviewport');
           //Ext.widget('login');

            //console.log('launch');
       });

       task.delay(1500);


    },

    onLogin: function (loginController, user, loginManager) {
        
        var form   = this.login.down('form'),
            param = form.getValues();        

        this.loginManager = loginManager;
        this.user = user;     
        
        //Start the server and send username to server.
        $.connection.hub.start().done(function() {
            chat.server.userSignin(param['userName'], chat.connection.id);
        });
        
        this.login.destroy();       
        this.showUI();
    },
    
    onLogingOut : function () {
        this.viewport.destroy();
        var session = this.session = new Ext.data.Session();        
        this.login = new DMS.view.login.Login({
            session: session,
            autoShow: true,
            listeners: {
                scope: this,
                login: 'onLogin'
            }
        });
    },

    showUI: function() {
        
        this.viewport = new DMS.view.main.Main({
            session: this.session,
            viewModel: {
                data: {                    
                    currentUser: GlobalShared.userLogin
                }
            }
        });
        
    },
    
    getSession: function() {
        return this.session;
    }
});
