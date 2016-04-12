var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var isDetail = false;

"use strict";

function ownershivehicleController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.temp = [];

    me.loadDetail = function () {
        var sono = localStorage.getItem('SONo');
        
        $http.post('om.api/SalesOrder/GridOwnerShip?SONo=' + me.data.SONo).
           success(function (data, status, headers, config) {
               me.grid.detail = data.data;
               me.temp = data.data;
               me.loadTableData(me.grid1, me.grid.detail);
                   if (me.grid.detail != "") {
                       me.isDetail = true;
                       me.isLoadData = true;
                       me.data.SONo = localStorage.getItem('SONo');
                       //localStorage.removeItem('SONo');
                   }
                   else {
                       me.isDetail = false;
                       me.isLoadData = false;
                   }
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxcolorcode",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "SalesModelCode", header: "Sales Model Code", width: 200 },
                { id: "SalesModelYear", header: "Sales Model Year", width: 200 },
                { id: "StatusVehicle", header: "Status Vehicle", width: 200 },
                { id: "BrandCode", header: "Brand Code", width: 250 },
                { id: "ModelName", header: "Model Name", width: 250 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined)
                    {
                        me.data = this.getItem(me.grid1.getSelectedId().id);
                        me.data.oid = me.grid1.getSelectedId();
                        if (this.getItem(me.grid1.getSelectedId().id).StatusVehicle == "A") me.switchSO = "0";
                        if (this.getItem(me.grid1.getSelectedId().id).StatusVehicle == "B") me.switchSO = "1";
                        if (this.getItem(me.grid1.getSelectedId().id).StatusVehicle == "C") me.switchSO = "2";
                        if (this.getItem(me.grid1.getSelectedId().id).StatusVehicle == "D") me.switchSO = "3";
                        if (this.getItem(me.grid1.getSelectedId().id).StatusVehicle == "E") me.switchSO = "4";
                        me.isNyala = false; // nyalain option button
                        me.data.SONo = localStorage.getItem('SONo');
                        me.Apply();
                    }
                }
            }
        });
    }

    me.Merk = function () {
        if (!me.data.SalesModelCode) {
            MsgBox("Pilih terlebih dahulu sales model code di tabel!", MSG_INFO);
        } else {
        if (me.switchSO == "1" || me.switchSO == "3") {
            me.isSuzuki = true;
        } else {
            me.isSuzuki = false;
        }
        var lookup = Wx.blookup({
            name: "MerkLookup",
            title: "Merk",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("Merk").withParameters({ SalesModelCode: me.data.SalesModelCode, isSuzuki: me.isSuzuki }),
            defaultSort: "BrandCode asc",
            columns: [
                { field: "BrandCode", title: "Brand" },
                { field: "ModelName", title: "Description" },
                { field: "ModelType", title: "Type" },
                { field: "Variant", title: "Var" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BrandCode = data.BrandCode;
                me.data.ModelName = data.ModelName;
                me.Apply();
            }
        });
        }
    };

    me.add = function () {
        if (me.switchSO != 0 &&  $('#BrandCode').val() == "") { 
            me.Merk();
        }
        else {
            var mes = me.temp;
        var long = mes.length;
        for (i = 0; i < long; i++) {
            if (mes[i].SalesModelCode == $('#SalesModelCode').val()) {
                if (me.switchSO == 0) {
                    mes[i].StatusVehicle = "A";
                } else if (me.switchSO == 1) {
                    mes[i].StatusVehicle = "B";
                } else if (me.switchSO == 2) {
                    mes[i].StatusVehicle = "C";
                } else if (me.switchSO == 3) {
                    mes[i].StatusVehicle = "D";
                } else if (me.switchSO == 4) {
                    mes[i].StatusVehicle = "E";
                }

                if (me.switchSO == 0) {
                    mes[i].BrandCode = "";
                    mes[i].ModelName = "";
                } else {
                    mes[i].BrandCode = $('#BrandCode').val();
                    mes[i].ModelName = $('#ModelName').val();
                }
            }
        }
        me.temp = mes;
        me.grid.detail = me.temp;
        me.loadTableData(me.grid1, me.grid.detail);
        me.data = {};
        me.data.SONo = me.data.SONo = localStorage.getItem('SONo');
        }
        
    }
    
    me.approve = function (e, param) {
            var mes = me.temp;
            var long = mes.length;
            for (i = 0; i < long; i++) {
                if (me.switchSO == 0) {
                    mes[i].StatusVehicle = "A";
                } else if (me.switchSO == 1) {
                    mes[i].StatusVehicle = "B";
                } else if (me.switchSO == 2) {
                    mes[i].StatusVehicle = "C";
                } else if (me.switchSO == 3) {
                    mes[i].StatusVehicle = "D";
                } else if (me.switchSO == 4) {
                    mes[i].StatusVehicle = "E";
                }
            }
            me.temp = mes;
            me.grid.detail = me.temp;
            me.loadTableData(me.grid1, me.grid.detail);

            var params = {
                SONo: me.data.SONo,
                islinkITS: true,
                additionalOwnership: me.temp
            };
            $http.post('om.api/SalesOrder/approveSO', params)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    //me.startEditing();
                    //MsgBox(data.message, MSG_INFO);
                    localStorage.setItem("CloseInterval", true);
                    localStorage.setItem("RefreshGrid", true);
                } else {
                    MsgBox(data.message, MSG_INFO);
                    //location.reload();
                }
            }).
                error(function (data, status, headers, config) {
                    MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
            });
        //}
    };

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
        me.detail.Status = true;
        $('#ColourCode').removeAttr('disabled');
        $('#btnColourCode').removeAttr('disabled');
        $('#btnAddColour').attr('disabled', 'disabled');
        $('#btnCancelModel').attr('disabled', 'disabled');
        
    }

    me.initialize = function () {
        //alert(localStorage.getItem('SONo'));
        me.data.SONo = localStorage.getItem('SONo');
        me.loadDetail();
        me.clearTable(me.grid1);
        me.grid1.clearSelection();
        $('#btnBrandCode').attr('disabled', 'disabled');
        me.isDisableHdr = false;
        $("#label[ng-model='switchSO']").attr('disabled', true);
        me.switchSO = "0";
        me.isNyala = true; // matiin option button
    }

    me.initGrid();
    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.$watch('switchSO', function (a, b) {
        if (a == 0) {
            $('#btnBrandCode').attr('disabled', 'disabled');
        } else{
            $('#btnBrandCode').removeAttr('disabled');
        }
    }, true);

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Ownership Vehicle",
        xtype: "panels",
        toolbars: [
            { name: "btnSave", text: "Approve", cls: "btn btn-success", icon: "icon-save",  click: "approve()" },
            ],
        panels: [
            {
                name: "wxcolorcode",
                xtype: "wxtable",
            },
            {
                name: "pnlModelColour",
                title: "Sales Type",
                items: [
                         {
                            type: "optionbuttons",
                            name: "switchSO",
                            //text: "Kategori",
                            model: "switchSO",
                            cls: "span7",
                            items: [
                                { name: "0", text: "Kendaraan Baru", disable : "isNyala" },
                                { name: "1", text: "Ganti Kendaraan Dari Suzuki", disable: "isNyala" },
                                { name: "2", text: "Ganti Kendaraan Dari Merk Lain", disable: "isNyala" },
                                { name: "3", text: "Tambah Kendaraan Sebelumnya menggunakan Suzuki", disable: "isNyala" },
                                { name: "4", text: "Tambah Kendaraan Sebelumnya menggunakan merk lain", disable: "isNyala" },
                            ]
                         },
                         { name: "lbl3", type: "label", text: "Kendaraan yang di inginkan", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                         { name: "lbl4", type: "label", text: "Kendaraan yang dimiliki", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                         { type: "div", cls: "divider span3" },
                         { type: "separator", cls: "span1" },
                         { type: "div", cls: "divider span4" },
                            {
                                type: "controls",
                                text: "SalesModelCode/ year",
                                cls: "span4",
                                items: [
                                    { name: "SalesModelCode", model: "data.SalesModelCode", text: "", cls: "span5", readonly: true },
                                    { name: "SalesModelYear", model: "data.SalesModelYear", text: "", cls: "span3", readonly: true },
                                ]
                            },
                            {
                                type: "controls",
                                text: "Merk/ Type",
                                cls: "span4",
                                items: [
                                    { name: "BrandCode", model: "data.BrandCode", text: "", cls: "span4", readonly: true, type: "popup", click: "Merk()" },
                                    { name: "ModelName", model: "data.ModelName", text: "", cls: "span4", readonly: true },
                                ]
                            },
                          {
                              type: "buttons",
                              items: [
                                      //{ name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddColour()", show: "data.oid === undefined", disable: "isDisableHdr" },
                                      { name: "btnUpdateColour", text: "Add", icon: "icon-save", cls: "btn btn-success", click: "add()", show: "data.oid !== undefined" },
                                      //{ name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "data.oid !== undefined" },
                                      //{ name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "data.oid !== undefined || data.oid == undefined", disable: true }
                              ]
                          },

                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ownershivehicleController");
    }
});