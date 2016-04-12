"use strict"

function svCopyJobs($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.dataSource = function () {
        var lookup = Wx.blookup({
            name: "BasicModelBrowse",
            title: "Sumber BasicModel Lookup",
            manager: MasterService,
            query: "BasmodPekerjaan",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model", fillspace: true },
                { field: "ModelDescription", title: "Keterangan ", fillspace: true },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.dataSource = data.BasicModel;
            me.Apply();
        });
    }

    me.targetSource = function () {
        var lookup = Wx.blookup({
            name: "BasicModelBrowse",
            title: "Target BasicModel Lookup",
            manager: MasterService,
            query: "BasmodPekerjaan",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model", fillspace: true },
                { field: "ModelDescription", title: "Keterangan ", fillspace: true },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.dataTarget = data.BasicModel;
            me.Apply();
        });
    }

    me.process = function (e) {
        var data = $(".main .gl-widget").serializeObject();
        if (data.dataSource == undefined || data.dataSource == '') {
            MsgBox("Sumber Data Belum dipilih", MSG_INFO);
            return;
        }

        if (data.dataTarget == undefined || data.dataTarget == '') {
            MsgBox("Target Data Belum dipilih", MSG_INFO);
            return;
        }

        if (data.dataSource == data.dataTarget) {
            MsgBox("Sumber data dan Target data tidak boleh sama ", MSG_INFO);
            return;
        }

        if (confirm("Apakah anda yakin???")) {
            
            var param = {
                BasicModelSumber: data.dataSource,
                BasicModelTarget: data.dataTarget
            }
            $http.post("sv.api/pekerjaan/prosesbasicmodel", param, function (result) {
                if (result.success) {
                    SimDms.Success("data copied...");
                }
            });
            //console.log(data,param);
        }
    }

    me.initialize = function () {
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Copy Pekerjaan Untuk Basic Model baru",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "dataSource", cls: "span4 full", text: "Sumber Data", type: "popup", click: "dataSource()"
                    },
                    {
                        name: "dataTarget", cls: "span4 full", text: "Target Data", type: "popup", click: "targetSource()"
                    },
                    {
                        type: "buttons",
                        items: [
                                    { name: "btnProcess", text: "Proses", icon: "icon-bolt", cls: "btn btn-info", click: "process()" },
                        ]
                    },
                ]
            },
        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svCopyJobs");
    }
});