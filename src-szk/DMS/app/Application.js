Ext.Loader.setConfig({ 
    enabled: true
});

Ext.define('GlobalShared', {
    singleton: true,
    url: "http://tbsdmsap01:9091/",
    login: 0,
    token: {},
    autoLogin: true,
    userLogin: ''
});  

         
Ext.define('DMS.Application', {
    extend: 'Ext.app.Application',
    namespace: 'DMS',

    requires: [
                'Ext.app.*',
                'Packt.util.*',
                'Ext.ux.form.MultiSelect',
                'Ext.state.CookieProvider',
                'Ext.window.MessageBox',
                'Ext.tip.QuickTipManager',
                'Ext.menu.*',
                'Ext.form.Panel',
                'Ext.layout.container.Accordion',
                'Ext.form.Label',
                'Ext.data.proxy.Ajax',
                'Ext.form.FieldSet',
                'Ext.form.field.*',
                'Ext.grid.plugin.*',
                'Ext.grid.filters.Filters',
                'Ext.ux.LiveSearchGridPanel',
                'Ext.grid.feature.Grouping',
                'Ext.grid.column.*',
                'Ext.chart.axis.*',
                'Ext.ux.ProgressBarPager',
                'Ext.ux.TabCloseMenu',
                'DMS.*',
                'Ext.picker.*',                
                'DMS.util.ux.*',
                'Ext.Ajax',
                'GlobalShared'
            ],

    stores: [],

    controllers: [         
       'Root'
    ],
 
    onBeforeLaunch: function () {
        
        if ('nocss3' in Ext.Object.fromQueryString(location.search)) {
            Ext.supports.CSS3BorderRadius = false;
            Ext.getBody().addCls('x-nbr x-nlg');
        }

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
                    }else {
                        var x = Ext.util.JSON.decode(errorMsg);
                        if (x !== undefined && x.message !== undefined)
                        {
                            errorMsg = x.message;
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
                                enableSourceEdit : false,
                                onRender: function() {
                                    Ext.form.HtmlEditor.prototype.onRender.apply(this, arguments);
                                    this.height = win.height;
                                }
                            }
                        ]
                    }]
                });

               
                win.show();
            };

        Ext.Ajax.useDefaultXhrHeader = false;

        Ext.Ajax.on('beforerequest',function(conn,o,result){
            
            o.url = GlobalShared.url + o.url;      
            o.useDefaultXhrHeader = false;
            
           if ( GlobalShared.login == 1)
           {
                o.withCredentials= true;
                o.headers = { 
                    'Authorization' : GlobalShared.token.token_type + ' ' + GlobalShared.token.access_token
                }                
           }
        });

        Ext.Ajax.on('requestcomplete',function(conn,o,result){
            
        });

        Ext.Ajax.on('requestexception',function(conn,o,result){
            showFailure (o);
        });

        Ext.setGlyphFontFamily('FontAwesome');
        Ext.tip.QuickTipManager.init();
        Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));
        
        Ext.create('DMS.store.Navigation', {
            storeId: 'navigation'
        });

        //Set the default route to start the application.
        //this.setDefaultToken('basic-panels');

        this.callParent();
    }

});
