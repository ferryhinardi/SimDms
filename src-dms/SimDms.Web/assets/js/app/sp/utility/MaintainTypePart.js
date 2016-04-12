var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spMaintainTypePartController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.PartNo = function () {

        var lookup = Wx.blookup({
            name: "btnPartNo",
            title: "Part No Lookup",
            manager: SpUtilityManager,
            query: "PartNoMntHargaPokok",
            defaultSort: "PartNo ASC",
            columns: [
            { field: "PartNo", title: "No. Part" },
            { field: "PartName", title: "Nama Part" },
            { field: "TipePart", title: "Kode Kategori" },
            { field: "TypeOfGoods", title: "Nama Kategori" },

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                //me.lookupAfterSelect(data);
                //me.data.PartNo = data.PartNo;
               // me.PopulateData();
                me.Apply();

            }
        });
    }
 
    me.TypePart = function () {

        var lookup = Wx.blookup({
            name: "btnLookupValue",
            title: "Tipe Part Lookup",
            manager: SpUtilityManager,
            query: "TypePartLookUp",
            defaultSort: "LookUpValue ASC",
            columns: [
            { field: "LookUpValue", title: "Kode Tipe Part" },
            { field: "LookUpValueName", title: "Deskripsi" },

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.LookupValue = data.LookUpValue;
                me.detail.LookupValueName = data.LookUpValueName;
                me.Apply();

            }
        });
    }

    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Apakah anda yakin melakukan maintain tipe part semua cabang untuk part " + me.data.PartNo + "?", function (result) {
            if (result) {
                $http.post('sp.api/MaintainHargaPokok/SaveMaintainTypeOfGoods', me.savemodel).
                success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Maintain tipe part berhasil untuk semua cabang !");
                    me.init();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
                }
        });
        
    }


    me.FisrtLoad = function () {
        $http.post('sp.api/MaintainHargaPokok/FisrtLoad', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.data.opt = "1";
                    } else {
                        me.data.opt = "0";
                    }
                }).
                error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }


  

    me.initialize = function()
    {
        me.data = {};
        me.detail = {};
        me.FisrtLoad();
    }


  
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Maintenance Type Part",
        xtype: "panels",
        toolbars: [
            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
     
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        {name:"opt", show:"data.feryyp != undefined"},
                        {
                            text: "Part No",
                            type: "controls",
                            items: [
                                {
                                    name: "PartNo",
                                    cls: "span3",
                                    placeHolder: "Part Code",
                                    type: "popup",
                                    btnName: "btnPartNo",
                                    click: "PartNo()",
                                    validasi: "required",
                                    readonly: true,
                                    disable :"data.opt == 0"
                                },
                                {
                                    name: "PartName",
                                    cls: "span5",
                                    placeHolder: "Part Name",
                                    readonly: true,
                                    disable: "data.opt == 0"
                                }
                            ]
                        },
                         {
                             text: "Current Tipe Part",
                             type: "controls",
                             items: [
                                 {
                                     name: "TipePart",
                                     cls: "span3",
                                     placeHolder: "Part Code",
                                     readonly: true,
                                     disable: "data.opt == 0"
                                 },
                                 {
                                     name: "TypeOfGoods",
                                     cls: "span5",
                                     placeHolder: "Part Name",
                                     readonly: true,
                                     disable: "data.opt == 0"
                                 }
                             ]
                         },
                        {
                            text: "New Tipe Part",
                            type: "controls",
                            items: [
                                {
                                    name: "LookupValue",
                                    cls: "span3",
                                    placeHolder: "Tipe Part Code",
                                    type: "popup",
                                    model: "detail.LookupValue",
                                    btnName: "btnLookupValue",
                                    click: "TypePart()",
                                    validasi: "required",
                                    readonly: true,
                                    disable: "data.opt == 0"
                                },
                                {
                                    name: "LookupValueName",
                                    cls: "span5",
                                    model: "detail.LookupValueName",
                                    placeHolder: "Tipe Part Name",
                                    readonly: true,
                                    disable: "data.opt == 0"
                                }
                            ]
                        },
                       
                        
                     ]
          
            }

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spMaintainTypePartController");
    }

});