Ext.define('DMS.view.login.Login', {
    extend: 'Ext.window.Window',
    id: 'login',
    alias: 'widget.login',
    
    requires: [
        'DMS.view.login.LoginController',
        'DMS.view.login.LoginModel',
        'Ext.form.Panel',
        'Ext.button.Button',
        'Ext.form.field.Text',
        'Ext.form.field.ComboBox'
    ],
    
    viewModel: 'login',    
    controller: 'login',
    bodyPadding: 10,
    title: 'Login',
    closable: false,
    frame: true,
    cls: 'login',
    height:175,
    defaultFocus: 'username',
    items: {
        xtype: 'form',
        reference: 'form',
        bodyStyle: 'background:transparent;',
        layout: 'form',
        plain: true,
        border: false,
        width:350,
        draggable:false,
        resizable:false,
        items:[
            {id:"Container1",
            margin:"0 0 35 0",
            xtype:"container",
            items:[
                {
                    xtype:'box',
                    id:'user_img',
                    width:80,
                    autoEl:{
                        style:'position:absolute;top:15px;left:15px;',
                        tag:'img',
                        src:'resources/images/lock-key.png'
                    }
                },
                {
                    border:false,
                    height:150,
                    margin:"0 0 0 0",
                    xtype:"fieldset",
                    defaults:{"width":234,"labelWidth":60},
                    items:[
                        {
                            id:'userName', 
                            name: 'userName', 
                            xtype:"textfield",
                            fieldLabel:"<b>Username</b>",
                            allowBlank:false,
                            blankText:"Your username is required.",
                            enableKeyEvents:true,
                            listeners: {
                                specialKey: 'onSpecialKey'
                            }
                        },
                        {
                            id:"password", 
                            name: 'password', 
                            margin:"5 0 0 0",
                            xtype:"textfield",
                            fieldLabel:"<b>Password</b>",
                            inputType:"password",
                            allowBlank:false,
                            blankText:"Your password is required.",
                            enableKeyEvents:true,
                            listeners: {
                                specialKey: 'onSpecialKey'
                            }
                        },
                        {
                            id:"remember",
                            name: "remember",
                            margin:"5 0 0 65",
                            xtype:"checkboxfield",
                            boxLabel:"Remember me in next time."
                        },
                        {
                            xtype : 'hidden',
                            name: 'grant_type',
                            value: 'password'
                        }
                    ]
            }],
            layout:"hbox"}
         ]
    },

    buttons: [{
        text: 'Login',
        listeners: {
            click: 'onLoginClick'
        }
    }],
    listeners: {

        afterlayout: function(sender,e,o)
        {
             
        },

        show: function(win) {
            if (Ext.supports.LocalStorage)
            {
                var data = localStorage["JSMS_SAVE"];
                if ( (data === "" || data === undefined)) { console.log(data);}
                else
                {
                    var auth = Ext.util.JSON.decode(data);
                    if (auth !== undefined && auth !== {})
                    {
                        if(auth.remember=="on")
                        {
                            Ext.getCmp('userName').setValue(auth.userName);
                            Ext.getCmp('password').setValue(auth.password);
                            Ext.getCmp('remember').setValue(auth.remember);                            
                        }                    
                    }                    
                }

            }
            

        }
    }

});
