"use strict";

function SalesRetur($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    //$http.post("om.api/grid/WarehouseList")
    //.success(function (data) {

    //    //var wrdata = $.grep(data.data, function (element, index) {
    //    //    return element.LookUpValue == '99';
    //    //});


    //    ////console.log(wrdata[0].LookUpValue);
    //    //me.data.WareHouseCode = wrdata[0].LookUpValue;
    //    //me.data.WarehouseName = wrdata[0].LookUpValueName;
    //    //me.Apply();
    //    //$(data).greo
    //    //if (data.data.length > 0) {
    //    //    me.data.WareHouseCode = data.data[0].LookUpValue;
    //    //    me.data.WarehouseName = data.data[0].LookUpValueName;
    //    //}
    //});

    me.uistate = function () {
        switch (me.data.Status) {
            case "": //new
                $('#pnlDetail').hide();
                me.data.StatusDsc = "New";
                $("#Status label").html(me.data.StatusDsc);
                $("#btnDelete,#btnPrintPreview").hide();                
                break;
            case "0": //open
                $("#Status label").html(me.data.StatusDsc);
                $('#pnlDetail').show();
                $('#btnNewItem, #btnAddItem').show
                $('#btnDeleteItem').hide();
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isEQAvailable = false;
                me.isInitialize = false;
                me.Apply()
                $('#btnDelete, #btnPrintPreview').show();
                break;
            case "1": //printed
                $("#Status label").html(me.data.StatusDsc);
                $('#pnlDetail').show();
                $('#btnNewItem, #btnAddItem').show
                $('#btnDeleteItem').hide();
                $('#btnApprove').removeAttr('disabled');
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isEQAvailable = false;
                me.isInitialize = false;
                me.Apply()
                $('#btnDelete, #btnPrintPreview').show();
                break;
            case "2": //approved
                $("#Status label").html(me.data.StatusDsc);
                $('#pnlDetail').show();
                $('#btnApprove').prop('disabled', true);
                $("#btnDelete,#btnSave").hide();
                $("form button, form input").prop('disabled', true);
                me.isLoadData = true;
                me.isPrintAvailable = true;
                $('#btnPrintPreview').show();
                me.Apply()
                break;
            case "3": //canceled
                $("#Status label").html(me.data.StatusDsc);
                $("form input,form button").prop('disabled', true);
                //$("#btnDelete,#btnPrintPreview").hide();
                break;
            case "5":
                $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);
                break;
            case "9":
                $("#Status label").html(me.data.StatusDsc); $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                //$("#btnDelete,#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);
                break;
            default: break;
        }
    }

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "BPULookup",
            title: "Return",
            url: "om.api/grid/SlsRtrnLkpBrowse",
            serverBinding: true,
            pageSize: 10,          
            columns: [
                 { field: "ReturnNo", title: "No. Return" ,width:150},
                { field: "SalesTypeDsc", title: "Tipe", width: 150 },
                {
                    field: "ReturnDate", title: "Tanggal Return", width: 150,
                    template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #"
                },
                { field: "InvoiceNo", title: "No. Invoice", width: 150 },
                { field: "CustomerCode", title: "Kode Pelanggan", width: 150 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 250 },
                { field: "Address", title: "Alamat", width: 350 },
                { field: "WareHouseCode", title: "Kode Gudang", width: 150 },
                { field: "WarehouseName", title: "Nama Gudang", width: 250 },
                { field: "StatusDsc", title: "Status", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {            
            me.data = data;
            me.Apply();
            me.uistate();
            $http.post("om.api/Grid/SlsReturGridModel", { ReturnNo: me.data.ReturnNo })
                .success(function (rslt) {
                     me.detil.grid = rslt;
                     me.loadTableData(me.griddetil, me.detil.grid);
            });
            me.griddetil.adjust();
        });
    }

    me.invlkp = function () {
        var lookup = Wx.klookup({
            name: "lookupSO",
            title: "Return",
            url: "om.api/Grid/SlsRtrnLkpInv",
            serverBinding: true,
            pageSize: 10,
            columns: [
                 { field: "InvoiceNo", title: "No.Invoice", width: 150 },
                 { field: "SalesTypeDsc", title: "Tipe", width: 150 },
                 { field: "TypeSales", title: "Kode", width: 150 },
                 { field: "CustomerCode", title: "Nama Pelanggan", width: 250 },
                 { field: "Address", title: "Alamat", width: 350 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.data.FakturPajakDate = data.FakturPajakDate;
            me.data.FakturPajakNo = data.FakturPajakNo;
            me.data.InvoiceDate = data.InvoiceDate;
            me.data.InvoiceNo = data.InvoiceNo;
            me.Apply();
            me.uistate();            
        });
    }

    me.WrhLkp = function () {
        var lookup = Wx.klookup({
            name: "btnWarehouseCode",
            title: "Warehouse",
            url: "om.api/grid/WarehouseListRetur",
            serverBinding: true,
            columns: [
                { field: "LookUpValue", title: "Kode Gudang", width: 100 },
                { field: "LookUpValueName", title: "Nama Gudang", width: 200 },
            ]
        });

        lookup.dblClick(function (data) {
            me.data.WareHouseCode = data.LookUpValue;
            me.data.WarehouseName = data.LookUpValueName;
            me.Apply();
        });


    }

    me.slsmdlcode = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model",
            url: "om.api/grid/SlsRtrnLkpSlsMdlCd",
            serverBinding: true,
            params:{InvoiceNo:me.data.InvoiceNo},
            columns: [
                { field: "SalesModelCode", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.detil.SalesModelCode = data.SalesModelCode;
            me.detil.SalesModelDesc = data.SalesModelDesc;
            me.Apply();
        });
    }

    me.slsmdlyear = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model",
            url: "om.api/grid/SlsRtrnLkpSlsMdlYear",
            params:{SalesModelCode:me.detil.SalesModelCode,InvoiceNo:me.data.InvoiceNo},
            serverBinding: true,
            columns: [
                { field: "SalesModelYear", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.detil = data;
            me.Apply();
        });
    }

    me.chassisnolkp = function () {
        var lookup = Wx.klookup({
            name: "chassisnolkp",
            title: "Chassis No",
            url: "om.api/grid/SlsRtrnLkpChassisNo",
            params:{InvoiceNo:me.data.InvoiceNo,ChassisCode:me.detil.ChassisCode, SalesModelCode: me.detil.SalesModelCode, SalesModelYear:me.detil.SalesModelYear},
            serverBinding: true,
            columns: [
                { field: "ChassisCode", title: "Kode Rangka", width: 150 },
                { field: "ChassisNo", title: "No Rangka" },
                { field: "BPKNo", title: "No BPk" }
            ]
        });

        lookup.dblClick(function (data) {
            me.detil.BPKNo = data.BPKNo;
            me.detil.ChassisNo = data.ChassisNo;
            me.Apply();
        });
    }

    me.griddetil = new webix.ui({
        container: "wxdetilslsmdl",
        scrollX: true,
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", format: webix.i18n.intFormat, width: 150 },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", format: webix.i18n.intFormat, width: 150 },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", format: webix.i18n.intFormat, width: 150 },
            { id: "OthersDPP", header: "DPP Lain-lain", format: webix.i18n.intFormat, width: 130 },
            { id: "OthersPPn", header: "PPN Lain-lain", format: webix.i18n.intFormat, width: 130 },
            { id: "ChassisCode", header: "Kode Rangka", width: 130 },
            { id: "ChassisNo", header: "No. Rangka", width: 130 },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetil.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.griddetil.getSelectedId());
                    me.detil = griddata;
                    $('#btnDeleteItem').show();
                    me.Apply();
                }
            }
        }
    });

    //webix.event(window, "resize", function () {
    //    me.griddetil.adjust();
    //})

    me.save = function () {
        $http.post('om.api/Return/save', me.data)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    if (me.data.ReturnNo == null || me.data.ReturnNo == undefined) {
                        me.data.ReturnNo = data.ReturnNo;
                    }
                    me.data.Status = data.Status;
                    me.data.StatusDsc = data.StatusDsc;
                    Wx.Success("Data Sudah Disimpan!");
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            })
            .error(function (e) {
                MsgBox('Proses simpan return gagal!', MSG_ERROR)
            });
    };

    me.addItem = function () {
        if (me.detil.SalesModelYear == "" || me.detil.SalesModelYear == undefined || me.detil.SalesModelCode == "" || me.detil.SalesModelCode == undefined || me.detil.ChassisNo == "" || me.detil.ChassisNo == undefined) {
            return MsgBox("Silahkan lengkapi data detail lebih dahulu!", MSG_INFO);
        }
        $http.post('om.api/Return/addDetail', { mdlHdr: me.data, mdlDet: me.detil })
           .success(function (data, status, headers, config) {
               if (data.success) {
                   Wx.Success(data.message);
                   me.detil = {};
                   $http.post("om.api/Grid/SlsReturGridModel", { ReturnNo: me.data.ReturnNo })
                    .success(function (rslt) {
                        me.detil.grid = rslt;
                        me.loadTableData(me.griddetil, me.detil.grid);
                        me.griddetil.adjust();
                    });
               } else {
                   MsgBox(data.message, MSG_INFO);
               }
           })
           .error(function (e) {
               MsgBox('Proses add item return gagal!', MSG_ERROR)
           });
    }

    me.newItem = function () {
        me.detil = {};
        $('#btnDeleteItem').hide();
    }

    me.deleteItem = function () {
        if (confirm("Anda yakin ingin hapus detail ini?"))
        {
            $http.post('om.api/Return/deleteDetail', { ReturnNo: me.data.ReturnNo, InvoiceNo:me.data.InvoiceNo, mdlDet: me.detil })
            .success(function (data, status, headers, config) {
              if (data.success) {
                  Wx.Success(data.message);
                  me.detil = {};
                  $http.post("om.api/Grid/SlsReturGridModel", { ReturnNo: me.data.ReturnNo })
                    .success(function (rslt) {
                        me.detil.grid = rslt;
                        me.loadTableData(me.griddetil, me.detil.grid);
                        me.griddetil.adjust();
                  });
                  $('#btnDeleteItem').hide();
              } else {
                  MsgBox(data.message, MSG_INFO);
              }
          })
          .error(function (e) {
              MsgBox('Proses hapus detail return gagal!', MSG_ERROR)
          });
        }
    }

    me.printPreview = function () {
        $http.post('om.api/Return/printPreview', { ReturnNo: me.data.ReturnNo })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    me.infoBranch = data.infoBranch;
                    me.printDialog();
                    me.uistate();
                    $http.post("om.api/Grid/SlsReturGridModel", { ReturnNo: me.data.ReturnNo })
                        .success(function (rslt) {
                            me.detil.grid = rslt;
                            me.loadTableData(me.griddetil, me.detil.grid);
                        });
                    me.griddetil.adjust();
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            })
          .error(function (e) {
              MsgBox('Proses print return gagal!', MSG_ERROR)
          });
    }

    me.printDialog = function () {
        BootstrapDialog.show({
            message: $(
                '<div class="container">' +
                '<div class="row">' +
                '<input type="radio" name="PrintType" id="PrintType1" value="0" checked>&nbsp Print Satu Halaman</div>' +
                '<div class="row">' +
                '<input type="radio" name="PrintType" id="PrintType2" value="1">&nbsp Print Setengah Halaman</div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.Print();
                    dialogRef.close();
                }
            }, {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    }

    me.Print = function () {
        var ReportId = "";
        if ($('input[name=PrintType]:checked').val() === '0') {
            ReportId = 'OmRpSalesTrn005';
        }
        else {
            ReportId = 'OmRpSalesTrn005A';
        }

        var par = me.data.ReturnNo + ',' + me.data.ReturnNo;
        var infoBranch = "";

        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: infoBranch,
            type: "devex"
        });
    }

    me.approve = function () {
        $http.post('om.api/Return/approve', { ReturnNo: me.data.ReturnNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.data.Status = data.Status;
                    me.uistate();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.initialize = function () {
        $('#ReturnDate').attr('onkeydown', 'return false');
        $('#Status label').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold"
        });
        
        me.data = {};
        me.detil = {};
        me.data.Status = "";
        me.data.StatusDsc = "";

        me.infoBranch = "";

        me.data.WareHouseCode = "";
        me.data.WarehouseName = "";

        me.data.ReturnDate = me.now();
        me.griddetil.adjust();
        me.uistate();
    };


    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Retur",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlB",
                items: [
                    { name: "ReturnNo", model:"data.ReturnNo",text: "No.Return", cls: "span3", placeHolder: "RTS/YY/XXXXXX", disabled: true },
                    {
                        name: "ReturnDateCtrl", type: "controls", text: "Tgl. Retur", cls: "span5",
                        items: [
                            { name: "ReturnDate", model: "data.ReturnDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                   { name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()", disable: "true" }
                                ]
                            },
                         { name: "Status", model: "data.StatusDsc", text: "Open", cls: "span2", readonly: true, type: "label" }
                        ]
                    },
                    { name: "InvoiceNo", model:"data.InvoiceNo",text: "No Invoice", type: "popup", cls: "span3", click: "invlkp()" },
                    { name: "CustomerCodeDsc", model:"data.CustomerName", text: "Pelanggan", cls: "span5" },
                    {
                        name: "gudng",
                        type: "controls",
                        text: "Gudang",
                        items: [
                            { name: "WareHouseCode",model:"data.WareHouseCode", type: "popup", cls: "span3", click: "WrhLkp() " },
                            { name: "WareHouseCodeDsc", model: "data.WarehouseName", cls: "span5", readonly:true }
                        ]
                    },
                    { name: "Remark", model:"data.Remark",text: "Keterangan", cls: "span8" }
                ]
            },
            {
                name: "pnlDetail",
                title: "Detail Sales Model",
                items: [
                     { name: "SalesModelCode", model:"detil.SalesModelCode", text: "Sales Model Code", type: "popup", cls: "span3", click: "slsmdlcode()" },
                     {
                         name: "expedisi",
                         type: "controls",
                         text: "Sales Model Year",
                         cls: "span5",
                         items: [
                             { name: "SalesModelYear", model: "detil.SalesModelYear", type: "popup", cls: "span3", click: "slsmdlyear()" },
                             { name: "SalesModelDesc", model: "detil.SalesModelDesc", cls: "span5", readonly: true }
                         ]
                     },
                     { name: "BeforeDiscDPP", model: "detil.BeforeDiscDPP", text: "DPP Belum Diskon", cls: "span3 number-int", readonly: true },
                     { name: "AfterDiscDPP", model: "detil.AfterDiscDPP", text: "DPP Setelah Diskon", cls: "span3 number-int", readonly: true },
                     { name: "DiscExcludePPn", model: "detil.DiscExcludePPn", text: "Diskon", cls: "span3 number-int", readonly: true },
                     { name: "AfterDiscPPn", model: "detil.AfterDiscPPn", text: "PPn Setelah Diskon", cls: "span3 number-int", readonly: true },
                     { name: "hidden1", text: "", type: "hidden", cls: "span3" },
                     { name: "AfterDiscPPnBM", model: "detil.AfterDiscPPnBM", text: "PPnBM Setelah Diskon", cls: "span number-int3", readonly: true },
                     { name: "OthersDPP", model: "detil.OthersDPP", text: "DPP Lain-lain", cls: "span3 number-int", readonly: true },
                     { name: "OthersPPn", model: "detil.OthersPPn", text: "PPn Lain-lain", cls: "span3 number-int", readonly: true },
                     {
                         name: "expedisi",
                         type: "controls",
                         text: "Kode/No Rangka",
                         cls: "span4",
                         items: [
                             { name: "ChassisCode", model: "detil.ChassisCode", cls: "span5", readonly: true },
                             { name: "ChassisNo", model: "detil.ChassisNo", type: "popup", cls: "span3", click: "chassisnolkp()" }
                         ]
                     },

                      { name: "BPKNo", model: "detil.BPKNo", text: "No.BPK", cls: "span4", readonly: true },
                      { name: "RemarkDtl", model: "detil.Remark", text: "Keterangan", cls: "span8" },
                      {
                          type: "buttons", cls: "span4 left", items: [
                                { name: "btnNewItem", text: "New", cls: "btn-small btn-info", icon: "icon-new", click: "newItem()" },
                                { name: "btnAddItem", text: "Add", cls: "btn-small btn-success", icon: "icon-save", click: "addItem()" },
                                { name: "btnDeleteItem", text: "Delete", cls: "btn-small btn-danger", icon: "icon-ok", click: "deleteItem()" }
                          ]
                      },
                    {
                        name: "wxdetilslsmdl",
                        type: "wxdiv"
                    },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("SalesRetur");
    }

});



