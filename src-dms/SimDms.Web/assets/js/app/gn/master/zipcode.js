var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    //me.browse = function () {
    //    var lookup = Wx.blookup({
    //        name: "MasterZipCode",
    //        title: "Zip Code Browse",
    //        manager: gnManager,
    //        query: "ZipCodes",
    //        defaultSort: "ZipCode asc",
    //        columns: [
    //            { field: "ZipCode", title: "ZIp Code" },
    //            { field: "KelurahanDesa", title: "Kelurahan Desa" },
    //            { field: "KecamatanDistrik", title: "Kecamatan/Distrik" },
    //            { field: "KotaKabupaten", title: "Kota/Kabupaten" },
    //            { field: "IbuKota", title: "Ibu Kota" },
    //            { field: "Notes", title: "Notes" },
    //            { field: "isCity", title: "Is City" }
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.isSave = false;
    //            me.data.ZipCode = data.ZipCode;
    //            me.data.KelurahanDesa = data.KelurahanDesa;
    //            me.data.IbuKota = data.IbuKota;
    //            me.data.KotaKabupaten = data.KotaKabupaten;
    //            me.data.KecamatanDistrik = data.KecamatanDistrik;
    //            me.data.Notes = data.Notes;
    //            me.lookupAfterSelect(data);
    //            me.Apply();
    //            city(data.isCity);
    //        }
    //    });

    //    function city(x) {
    //        if (x == true) { me.data.isCity = "1"; } else { me.data.isCity = "0"; }
    //    }
    //}

    //me.initGrid = function () {
    //    me.grid1 = new webix.ui({
    //        container: "wxbpu",
    //        view: "wxtable", css:"alternating", scrollX: true,
    //        columns: [
    //            { id: "ZipCode", header: "Kode Pos", width: 200 },
    //            { id: "KelurahanDesa", header: "Kelurahan/ Desa", width: 200 },
    //            { id: "KecamatanDistrik", header: "Kecamatan/ Distrik", width: 200 },
    //            { id: "KotaKabupaten", header: "Kota/ Kabupaten", width: 250 },
    //            { id: "IbuKota", header: "Ibu Kota", width: 250 },
    //            { id: "isCity", header: "is City", width: 250 },
    //        ],
    //        on: {
    //            onSelectChange: function () {
    //                if (me.grid1.getSelectedId() !== undefined) {
    //                    //alert(me.grid1.getSelectedId().id.columns.ColourCode);
    //                    me.data = this.getItem(me.grid1.getSelectedId().id);
    //                    me.data.oid = me.grid1.getSelectedId();
    //                    //console.log(this.getItem(me.grid1.getSelectedId().id))
    //                    //if (me.data.Status == 'PRINT' || me.data.Status == 'CLOSE') {
    //                    //    //alert(me.data.Status);
    //                    //    $('#btnUpdateDetail').attr('disabled', true);
    //                    //    $('#btnDeleteDetail').attr('disabled', true);
    //                    //}
    //                    me.Apply();
    //                }
    //            }
    //        }
    //    });
    //}
    //me.loadDetail = function () {
    //    //alert('sdfghj');
    //    $http.post('gn.api/ZipCode/ZipCodeLoad?').
    //           success(function (data, status, headers, config) {
    //               //alert('asd');
    //               me.grid.detail = data;
    //               me.loadTableData(me.grid1, me.grid.detail);
    //           }).
    //           error(function (e, status, headers, config) {
    //               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    //               console.log(e);
    //           });
    //}

    me.initialize = function () {
        //me.clearTable(me.grid1);
        //me.loadDetail();
        reloadGrid();
        me.data.isCity = "1";
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/ZipCode/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data zip code deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.saveData = function (e, param) {
        if (me.data.isCity == 1) { me.data.isCity = true; } else { me.data.isCity = false; }
        $http.post('gn.api/ZipCode/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    reloadGrid();
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    function reloadGrid() {
        //var data = $("#panelFilter").serializeObject();
        Wx.kgrid({
            //url: "gn.api/Grid/Users",
            url: "gn.api/ZipCode/ZipCodeLoad?cols=" + 6,
            name: "gridUser",
            //params: data,
            serverBinding: true,
            //pageSize: 10,
            columns: [
                { field: "ZipCode", title: "Kode Pos", width: 100 },
                { field: "KelurahanDesa", title: "Kelurahan/ Desa", width: 200 },
                { field: "KecamatanDistrik", title: "Kecamatan/ Distrik", width: 150 },
                { field: "KotaKabupaten", title: "Kota/ Kabupaten", width: 200 },
                { field: "IbuKota", title: "Ibu Kota", width: 100 },
                { field: "isCity", title: "Is City", width: 100, template: "#= (isCity)==true ? 'Yes' : 'No' #" },
            ],
            onDblClick: function (a, b, c) {
                Wx.selectedRow("gridUser", function (data) {
                    me.lookupAfterSelect(data);
                    //me.data = data;
                    Wx.populate($.extend(data));
                    //alert(data.isCity);
                    if (data.isCity == true) {
                        me.data.isCity = "1"
                    } else {
                        me.data.isCity = "0"
                    }

                    me.Apply();
                    //$("[name='UserId']").attr("disabled", true);
                    //$('#btnReset').show();
                    //widget.showToolbars(["btnSave", "btnCancel"]);
                });
            }
        });
    }


    //me.initGrid();

    //webix.event(window, "resize", function () {
    //    me.grid1.adjust();
    //})

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Master Zip Code",
        xtype: "panels",
        toolbars: [
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "Zip Code Setting",
                items: [
					{ name: "ZipCode", model: "data.ZipCode", text: "Zip Code", cls: "span4", required: true, validasi: "required" },
                    { name: "IbuKota", model: "data.IbuKota", text: "Ibu Kota", cls: "span6", required: true, validasi: "required" },
                     {
                         type: "controls", text: "Kota/Kabupaten", cls: "span6", required: true, items: [
                             { name: "KotaKabupaten", model: "data.KotaKabupaten", text: "Kota/Kabupaten", cls: "span6", required: true, validasi: "required" },
                             {
                                 type: "optionbuttons",
                                 name: "isCity",
                                 model: "data.isCity",
                                 text: "",
                                 items: [
                                     { name: "1", text: "   Kota   " },
                                     { name: "0", text: "Kabupaten" },
                                 ]
                             },
                         ]
                     },

                    { name: "KecamatanDistrik", model: "data.KecamatanDistrik", text: "Kecamatan/Distrik", cls: "span6", required: true, validasi: "required" },
                    { name: "KelurahanDesa", model: "data.KelurahanDesa", text: "Kelurahan/Desa", cls: "span6", required: true, validasi: "required" },
					{ name: "Notes", model: "data.Notes", text: "Notes", cls: "span6" },
                ]
            },
            //{
            //    name: "wxbpu",
            //    xtype: "wxtable",
            //},
             {
                 name: "gridUser",
                 xtype: "k-grid"
             },
        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});