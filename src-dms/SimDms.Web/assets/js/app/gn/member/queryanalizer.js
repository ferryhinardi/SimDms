var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
"use strict";



function gnQueryAnalizer($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.execute = function () {
        $http.post('gn.api/Query/Analizer', {ssql:me.data.ssql}).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   

                   var hdr = [];
                   if (!data.success)
                       $("p[data-name='tabMessage']").click();

                   else {
                       $("p[data-name='tabResult']").click();
                       $.each(data.hdr, function (k, v) {
                           hdr.push({ field: v, title: v, width: 150 });
                       });
                   }                   
                   me.reloadgrid(data.data, hdr);
                   me.data.sqlmsg = data.msg;
                  
               }).
               error(function (e, status, headers, config) {
                   MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);                   
               });
    }
    
    me.export = function () {
        window.open('gn.api/Query/ExportXLS?ssql=' + me.data.ssql)       
    }

    me.reloadgrid = function (datas, columns) {
        Wx.kgrid({
            name: "gridUser",
            data: datas,
            columns: columns
        });
    }

    me.initialize = function () {
        me.reloadgrid([], []);

        $("div[data-id='tabQuery']").on('click', function () {
            if ($("p[data-name='tabResult']").hasClass('active'))
                $(".panel.kgrid").show();
            else
                $(".panel.kgrid").hide();
        });

        $("p[data-name='tabResult']").click();
    };

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Query Analizer",
        xtype: "panels",
        toolbars:  [ 
                    { name: "btnExecute", text: "Execute", cls: "btn btn-info", icon: "icon-bolt",  click: "execute()" },
                    { name: "btnExport", text: "Export", cls: "btn btn-info", icon: "icon-file", click: "export()" },
                    ],
        panels: [
            {
                name: "Queryanalizer",
                title: "Query",
                items: [
                    { name: "Query", type: "textarea", model:"data.ssql", text: "", cls: "span8" },
                ]
            },            
            {
                xtype: "tabs",
                name: "tabQuery",
                items: [
                    { name: "tabResult", text: "Result", cls: "active" },
                    { name: "tabMessage", text: "Message" },
                ],

            },
//tab 1
            {
                name: "tabResult",
                cls: "tabQuery tabResult"                
            },
//tab 2
            {
                name: "tabMessage",
                cls: "tabQuery tabMessage",
                items: [
                      { name: "Message",model:"data.sqlmsg", type: "textarea", text: "", cls: "span8" }
                ]
            },
            {
                name: "gridUser",
                xtype: "k-grid"
            }
            
        ]
    };



     Wx = new SimDms.Widget(options);
    

    Wx.default = {};
    Wx.render(init);

    //widget.render(init)

    function init(s) {
            SimDms.Angular("gnQueryAnalizer");
        //reloadGrid();
    }

   




});