"use strict";

function SalesTransDO($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.uistate = function () {
        switch (me.data.Status) {
            case "": //new
                $("input,button")
                    .prop('disabled', false);

                $("#pnlHeader input ,#pnlKet input,#pnlKet button,#btnApprove")
                    .prop('disabled', true);
                $("#Remark,#DODate").prop('disabled', false);

                $("#Status label").html(me.data.StatusDsc);
                $("#btnDelete,#btnPrintPreview").hide();

                break;
            case "0": //open
                $("#Status label").html(me.data.StatusDsc);
                //$("#Remark").prop('disabled', true);
                //$("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#detRemark").prop('disabled', false)
                $("#btnDelete,#btnPrintPreview").show();
                $("#btnAddDetail,#btnRemDetail,#btnNewDetail").show();
                break;
            case "1": //printed
                $("#Status label").html(me.data.StatusDsc);
                $("#Remark").prop('disabled', true);
                $("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#detRemark,#btnApprove").prop('disabled', false)
                $("#btnDelete,#btnPrintPreview").show();
                break;
            case "2": //approved
                $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);
                $("#btnSave,#btnAddDetail,#btnRemDetail,#btnNewDetail,#btnDelete").hide();

                break;
            case "3": //canceled
                $("#Status label").html(me.data.StatusDsc);
                $("form input,form button").prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").hide();
                $("#btnSave,#btnAddDetail,#btnRemDetail,#btnNewDetail").hide();
                break;
            case "9":
                break;
            default: break;
        }
    }

    me.save = function () {
        $http.post("om.api/SalesDo/Save", me.data)
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.DONo = rslt.DONo;
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    me.Apply();
                    MsgBox("Data Saved");
                }
                else {
                    if (rslt.browse != undefined) {
                        me.Apply();
                        if (rslt.browse == "so") {
                            $("#btnSONo").click();
                        }
                        else {
                            $("#btnWareHouseCode").click();
                        }
                    }
                    else {
                        MsgBox(rslt.message, MSG_ERROR);
                    }
                }
            });
    }

    me.delete = function () {
        $http.post("om.api/SalesDo/Delete", { DONo: me.data.DONo })
        .success(function (rslt) {
            if (rslt.success == true) {
                me.data.Status = rslt.Status;
                me.data.StatusDsc = rslt.StatusDsc;
                me.uistate();
            }
            else {
                MsgBox(rslt.message, MSG_ERROR);
            }
        });
    }

    me.approve = function () {
        console.log(me.data.DONo);
        //$http.post("om.api/SalesDo/Approve", me.data)
        $http.post("om.api/SalesDo/Approve", { doNo: me.data.DONo })
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    Wx.Success(rslt.message);
                }
                else {
                    MsgBox(rslt.message, MSG_ERROR);
                }
            });
    }

    me.newdetail = function () {
        me.data.detail.DOSeq = "";
        me.data.detail.SalesModelCode = "";
        me.data.detail.SalesModelYear = "";
        me.data.detail.SalesModelDesc = "";
        me.data.detail.ChassisCode = "";
        me.data.detail.ChassisNo = "";
        me.data.detail.EngineCode = "";
        me.data.detail.EngineNo = "";
        me.data.detail.ColourCode = "";
        me.data.detail.RefferenceDesc1 = "";
        me.data.detail.Remark = "";
        me.griddetildo.clearSelection();
        me.Apply();

    }

    me.adddetail = function () {
        me.Apply();
        if (me.data.detail.SalesModelCode == "") {
            // me.Apply();
            $("#btnSalesModelCode").click();
            return;
        }

        if (me.data.detail.SalesModelYear == "") {
            //me.Apply();
            $("#btnSalesModelYear").click();
            return;
        }

        if (me.data.detail.ChassisNo == "") {

            //$("#btnChassisNo").click();
            //return;
        }



        var data = me.data.detail;
        data.DONo = me.data.DONo;
        console.log(me.data.detail);
        $http.post("om.api/SalesDo/SaveDetail", data)
          .success(function (rslt) {
              if (rslt.success == true) {

                  me.data.detail.DOSeq = rslt.DOSeq;
                  me.uistate();
                  me.Apply();
                  MsgBox("Data Saved");
                  $http.post("om.api/Grid/SlsDoDtl", { DONo: me.data.DONo })
                    .success(function (rslt) {
                        me.data.grid = rslt;
                        me.loadTableData(me.griddetildo, me.data.grid);
                        me.griddetildo.adjust();
                    });
                  $("#btnNewDetail").click();
              }
              else {
                  if (rslt.browse != undefined) {
                      me.Apply();
                      if (rslt.browse == "so") {
                          $("#btnSONo").click();
                      }
                      else {
                          $("#btnWareHouseCode").click();
                      }
                  }
                  else {
                      MsgBox(rslt.message, MSG_ERROR);
                  }
              }
          });
        console.log(me.data.detail);
    }

    me.deletedetail = function () {
        if (me.data.detail.DOSeq != "") {
            $http.post("om.api/SalesDo/DeleteDetail", { DONo: me.data.DONo, DOSeq: me.data.detail.DOSeq })
          .success(function (rslt) {
              if (rslt.success == true) {
                  $("#btnNewDetail").click();
                  me.uistate();
                  me.Apply();
                  $http.post("om.api/Grid/SlsDoDtl", { DONo: me.data.DONo })
                    .success(function (rslt) {
                        me.data.grid = rslt;
                        me.loadTableData(me.griddetildo, me.data.grid);
                        me.griddetildo.adjust();
                    });
                  $("#btnNewDetail").click();
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
          });
        }
    }

    me.printPreview = function () {
        $http.post("om.api/SalesDo/PrintDO", { DONo: me.data.DONo })
        .success(function (rslt) {
            if (rslt.success == true) {
                me.data.Status = rslt.Status;
                me.data.StatusDsc = rslt.StatusDsc;
                me.uistate();
                me.printDialog();
            }
            else {
                MsgBox(rslt.message, MSG_ERROR);
            }
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
            ReportId = 'OmRpSalesTrn002';
        }
        else {
            ReportId = 'OmRpSalesTrn002A';
        }

        console.log(ReportId);

        var par = me.data.DONo + ',' + me.data.DONo + ',profitcenter,';


        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            textprint: true,
            //rparam: rparam,
            type: "devex"
        });
    }

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "BrowseDOLKp",
            title: "DO Lookup",
            url: "om.api/grid/SlsDoBrowse",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "DONo", title: "No DO", width: 110 },
                { field: "TypeSales", title: "Tipe", width: 130 },
                {
                    field: "DODate", title: "Tgl DO", width: 110,
                    template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                },
                { field: "SONo", title: "No SO", width: 130 },
                { field: "SKPKNo", title: "No. SKPK", width: 150 },
                { field: "RefferenceNo", title: "No. Reff", width: 130 },
                { field: "CustomerName", title: "Pelanggan", width: 200 },
                { field: "Address", title: "Alamat", width: 400 },
                { field: "ShipToDsc", title: "Kirim Ke", width: 300 },
                { field: "WareHouseCode", title: "Gudang", width: 100 },
                { field: "Expedition", title: "Ekspedisi", width: 100 },
                { field: "StatusDsc", title: "Status", width: 110 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.DONo = data.DONo;
            me.data.DODate = data.DODate;
            me.data.SONo = data.SONo;
            me.data.CustomerCode = data.Customer;
            me.data.CustomerName = data.CustomerName;
            me.data.Address = data.Address;
            me.data.ShipTo = data.ShipTo;
            me.data.ShipToDsc = data.ShipToDsc;
            me.data.WareHouseCode = data.WareHouseCode;
            me.data.WrhDsc = data.WrhDsc;
            me.data.Expedition = data.Expedition;
            me.data.ExpeditionDsc = data.Expedition;
            me.data.Status = data.Status;
            me.data.StatusDsc = data.StatusDsc;
            me.data.Remark = data.Remark;
            $http.post("om.api/grid/SlsDoDtl", { DONo: me.data.DONo })
            .success(function (rslt, status, headers, config) {
                me.data.grid = rslt;
                me.loadTableData(me.griddetildo, me.data.grid);
                me.griddetildo.adjust();
            });

            me.uistate();

        });
    };

    me.solkp = function () {
        var lookup = Wx.klookup({
            name: "lookupSO",
            title: "SO",
            url: "om.api/Grid/slsdolkpso",
            serverBinding: true,
            pageSize: 10,
            columns: [
                 { field: "SONo", title: "No.SO", width: 150 },
                 { field: "TypeSales", title: "Tipe", width: 150 },
                 { field: "SODate", title: "Tgl. SO", width: 150, template: "#= SODate==null ? '' : moment(SODate).format('DD MMM YYYY') #" },
                 { field: "SKPKNo", title: "No.SKPK", width: 150 },
                 { field: "RefferenceNo", title: "No.Reff", width: 150 },
                 { field: "CustomerCode", title: "Kode", width: 150 },
                 { field: "CustomerName", title: "Pelanggan", width: 250 },
                 { field: "Address", title: "Alamat", width: 500 }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.SONo = data.SONo;
            me.data.CustomerName = data.CustomerName;
            me.data.CustomerCode = data.CustomerCode;
            if (data.RefferenceNo.substring(0, 2) == 'PO') {
                $http.post('om.api/SalesOrder/getRemarkPO', { PONo: data.RefferenceNo, NOPlg: data.CustomerCode }).
                    success(function (data, status, headers, config) {
                        me.data.Remark = data.data;
                    }).
                    error(function (e, status, headers, config) {
                        console.log(e);
                    });
            }
            me.isSave = true;
            me.Apply();
        });
    }

    //me.Shiptolkp = function () {
    //    var lookup = Wx.klookup({
    //        name: "lookupShipto",
    //        title: "Shipto",
    //        url: "om.api/Grid/slsdolkpshipto",
    //        serverBinding: true,
    //        pageSize: 10,
    //        columns: [
    //             { field: "CustomerCode", title: "Customer Code", width: 150 },
    //             { field: "CustomerName", title: "Customer Name" }]
    //    });

    //    lookup.dblClick(function (data) {
    //        me.data.ShipTo = data.CustomerCode;
    //        me.data.ShipToDsc = data.CustomerName;
    //        me.Apply();
    //    });
    //}

    me.Shiptolkp = function () {
        var lookup = Wx.klookup({
            name: "lookupShipto",
            title: "Shipto",
            url: "om.api/Grid/SlsDoLkpShiptoV2?cols=" + 5,
            serverBinding: true,
            pageSize: 10,
            columns: [
                 { field: "CustomerCode", title: "Customer Code", width: 150 },
                 { field: "CustomerName", title: "Customer Name" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ShipTo = data.CustomerCode;
            me.data.ShipToDsc = data.CustomerName;
            me.Apply();
        });
    }

    me.WrhLkp = function () {
        var lookup = Wx.klookup({
            name: "btnWareHouseCode",
            title: "Warehouse",
            url: "om.api/grid/WarehouseList",
            serverBinding: true,
            columns: [
                { field: "LookUpValue", title: "Kode Gudang", width: 100 },
                { field: "LookUpValueName", title: "Nama Gudang", width: 200 },
            ]
        });

        lookup.dblClick(function (data) {
            me.data.WareHouseCode = data.LookUpValue;
            me.data.WrhDsc = data.LookUpValueName;
            me.Apply();
        });


    }

    me.expdtionlkp = function () {
        var lookup = Wx.klookup({
            name: "ExpLkp",
            title: "Warehouse",
            url: "om.api/grid/slsdolkpexpedition",
            serverBinding: true,
            columns: [
                { field: "SupplierCode", title: "Kode", width: 150 },
                { field: "SupplierName", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.Expedition = data.SupplierCode;
            me.data.ExpeditionDsc = data.SupplierName;
            me.Apply();
        });
    }

    me.slsmdlcode = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model Code",
            url: "om.api/grid/SlsDoLkpSlsMdlCd",
            params: { SONo: me.data.SONo },
            serverBinding: true,
            columns: [
                { field: "SalesModelCode", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.detail.SalesModelCode = data.SalesModelCode;
            me.Apply();
        });
    }

    me.slsmdlyear = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model Year",
            params: { SONo: me.data.SONo, SalesModelCode: me.data.detail.SalesModelCode },
            url: "om.api/grid/SlsDoLkpSlsMdlYear",
            serverBinding: true,
            columns: [
                { field: "SalesModelYear", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.detail.SalesModelYear = data.SalesModelYear;
            me.data.detail.SalesModelDesc = data.SalesModelDesc;
            me.data.detail.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.chassisnolkp = function () {
        if (me.data.detail.ChassisCode == "")
            return;

        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Chassis No",
            params: {
                SONo: me.data.SONo,
                SalesModelCode: me.data.detail.SalesModelCode,
                SalesModelYear: me.data.detail.SalesModelYear,
                ChassisCode: me.data.detail.ChassisCode,
                WareHouseCode: me.data.WareHouseCode
            },
            url: "om.api/grid/SlsDoLkpChasisNo",
            serverBinding: true,
            columns: [
                { field: "ChassisNo", title: "Chassis No", width: 150 },
                { field: "EngineNo", title: "Engine No" }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.detail.ChassisNo = data.ChassisNo;
            me.data.detail.EngineCode = data.EngineCode;
            me.data.detail.EngineNo = data.EngineNo;
            me.data.detail.ColourCode = data.ColourCode;
            me.data.detail.RefferenceDesc1 = data.ColourName;
            me.Apply();
            //console.log(data);
        });
    }

    me.griddetildo = new webix.ui({
        container: "wxdetildo",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year" },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "NO. Rangka", template: function (obj) { return (obj.ChassisNo == null ? "" : obj.ChassisNo) } },
            { id: "EngineCode", header: "Kode Mesin", template: function (obj) { return (obj.EngineCode == null ? "" : obj.EngineCode) } },
            { id: "EngineNo", header: "No Mesin", template: function (obj) { return (obj.EngineNo == null ? "" : obj.EngineNo) } },
            { id: "ColourCode", header: "Kode Warna", template: function (obj) { return (obj.ColourCode == null ? "" : obj.ColourCode) } },
            { id: "RefferenceDesc1", header: "Nama Warna", width: 200, template: function (obj) { return (obj.RefferenceDesc1 == null ? "" : obj.RefferenceDesc1) } },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetildo.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.griddetildo.getSelectedId());
                    console.log(griddata);
                    me.data.detail.SalesModelCode = griddata.SalesModelCode;
                    me.data.detail.SalesModelYear = griddata.SalesModelYear;
                    me.data.detail.SalesModelDesc = griddata.SalesModelDesc;
                    me.data.detail.ChassisCode = griddata.ChassisCode;
                    me.data.detail.ChassisNo = griddata.ChassisNo;
                    me.data.detail.EngineCode = griddata.EngineCode;
                    me.data.detail.EngineNo = griddata.EngineNo;
                    me.data.detail.ColourCode = griddata.ColourCode;
                    me.data.detail.RefferenceDesc1 = griddata.RefferenceDesc1;
                    me.data.detail.Remark = griddata.Remark;
                    me.data.detail.DOSeq = griddata.DOSeq;
                    me.Apply();
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.griddetildo.adjust();
    })

    me.initialize = function () {

        $('#DODate').attr('onkeydown', 'return false');

        $('#Status label').css(
         {
             "font-size": "32px",
             "color": "red",
             "font-weight": "bold",
             "text-align": "center"
         });
        me.data = {};
        me.data.StatusDsc = "New";
        me.data.Status = "";
        me.data.detail = {};
        me.data.detail.SalesModelCode = "";
        me.data.detail.SalesModelYear = "";
        me.data.detail.ChassisNo = "";
        me.data.grid = [];
        me.griddetildo.adjust();
        me.data.DODate = moment().format();
        $http.post("om.api/grid/WarehouseList")
        .success(function (data) {
            if (data.data.length > 0) {
                me.data.WareHouseCode = data.data[0].LookUpValue;
                me.data.WrhDsc = data.data[0].LookUpValueName;
            }
            me.isLoadData = false;
        });

        me.griddetildo.clearAll();
        me.uistate();
        $("#btnDelete,#btnPrintPreview").removeClass('ng-hide');

        me.stdChangedMonitoring = function (n, o) {
            if (!me.isInProcess) {

                var eq = (n == o);

                // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
                if (!(_.isEmpty(n)) && !eq) {
                    if (!me.hasChanged && !me.isLoadData) {
                        me.hasChanged = true;
                        me.isLoadData = false;
                        console.log('1');
                    }
                    if (!me.isSave) {
                        console.log('2');
                        me.isSave = true;
                        me.hasChanged = true;
                        me.isLoadData = false;
                    }
                } else {
                    console.log('3');
                    me.hasChanged = false;
                    me.isSave = false;
                }
            }
        }
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Delivery Order",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlHeader",
                items: [
                    { name: "DONo", text: "No.DO", model: "data.DONo", cls: "span3", placeHolder: "DOS/YY/XXXXXX", disabled: true },
                    {
                        name: "DODateCtrl", type: "controls", text: "Tgl. DO", cls: "span5",
                        items: [
                            { name: "DODate", type: "ng-datepicker", model: "data.DODate", cls: "span3" },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                   { name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()", disable: "me.statusBPU === NEW || me.statusBPU === APPROVED" }
                                ]
                            },
                            { name: "Status", mdl: "data.StatusDsc", text: "New", cls: "span2", readonly: true, type: "label" },
                        ]
                    },
                    { name: "SONo", text: "SO.No", model: "data.SONo", validasi: "required", type: "popup", cls: "span3", click: "solkp()" },
                    { name: "CustomerName", text: "Pelanggan", model: "data.CustomerName", cls: "span5" },
                    {
                        name: "kirimke",
                        type: "controls",
                        text: "Kirim Ke",
                        items: [
                            { name: "ShipTo", type: "popup", model: "data.ShipTo", cls: "span3", click: "Shiptolkp()" },
                            { name: "ShipToDsc", model: "data.ShipToDsc", cls: "span5" },
                        ]
                    },
                    {
                        name: "gudng",
                        type: "controls",
                        text: "Gudang",
                        items: [
                            { name: "WareHouseCode", validasi: "required", model: "data.WareHouseCode", type: "popup", cls: "span3", click: "WrhLkp() " },
                            { name: "WrhDsc", model: "data.WrhDsc", cls: "span5" }
                        ]
                    },
                    {
                        name: "expedisi",
                        type: "controls",
                        text: "Expedisi",
                        items: [
                            { name: "Expedition", model: "data.Expedition", type: "popup", cls: "span3", click: "expdtionlkp()" },
                            { name: "ExpeditionDsc", model: "data.ExpeditionDsc", cls: "span5" }
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8" }
                ]
            },
            {
                name: "pnlKet",
                title: "Detil Do",
                items: [
                     { name: "SalesModelCode", model: "data.detail.SalesModelCode", text: "Sales Model Code", type: "popup", cls: "span3", click: "slsmdlcode()" },
                     {
                         name: "ctrlSlsModelYear",
                         type: "controls",
                         text: "Sales Model Year",
                         items: [
                             { name: "SalesModelYear", model: "data.detail.SalesModelYear", type: "popup", cls: "span2", click: "slsmdlyear()" },
                             { name: "SalesModelDesc", model: "data.detail.SalesModelDesc", cls: "span3" }
                         ]
                     },
                     {
                         name: "ctrlChassisno",
                         type: "controls",
                         text: "Kode/No Rangka",
                         items: [
                             { name: "ChassisCode", model: "data.detail.ChassisCode", cls: "span3" },
                             { name: "ChassisNo", model: "data.detail.ChassisNo", type: "popup", cls: "span2", click: "chassisnolkp()" }
                         ]
                     },
                      {
                          name: "ctrlEngingNo",
                          type: "controls",
                          text: "Kode/No Mesin",
                          items: [
                              { name: "EngineCode", model: "data.detail.EngineCode", cls: "span3" },
                              { name: "EngineNo", model: "data.detail.EngineNo", cls: "span2" }
                          ]
                      },
                      //{ name: "RefferenceDesc1", model: "data.detail.RefferenceDesc1", text: "Warna", cls: "span8" },
                      {
                          name: "ctrlColour",
                          type: "controls",
                          text: "Warna",
                          items: [
                                { name: "ColourCode", model: "data.detail.ColourCode", cls: "span3" },
                                { name: "RefferenceDesc1", model: "data.detail.RefferenceDesc1", cls: "span5" },
                          ]
                      },
                      { name: "detRemark", model: "data.detail.Remark", text: "Keterangan", cls: "span8" },
                      {
                          type: "buttons", cls: "span4 left", items: [
                              { name: "btnAddDetail", text: "Save", cls: "btn-small btn-info", icon: "icon-ok", click: "adddetail()", disable: 'me.data.Status ===""' },
                              { name: "btnRemDetail", text: "Delete", cls: "btn-small btn-info", icon: "icon-ok", click: "deletedetail()", disable: 'me.data.Status === ""' },
                              { name: "btnNewDetail", text: "New", cls: "btn-small btn-info", icon: "icon-ok", click: "newdetail()", disable: 'me.data.DOSeq ===""' }
                          ]
                      },
                      {
                          name: "wxdetildo",
                          type: "wxdiv"
                      }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("SalesTransDO");
    }
});