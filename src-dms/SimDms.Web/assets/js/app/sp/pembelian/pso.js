var status = 'N';

"use strict";

function spPSOController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/ComboOrderType').
    success(function (data, status, headers, config) {
        me.comboOrderType = data;
    });

    me.partType = function () {
        $http.post('sp.api/pso/index')
            .success(function (e) {
                me.data.PartType = e.PartType;
                //setTimeout(function () {
                //    me.isInProcess = false;
                //    me.isInitialize = false;
                //    me.hasChanged = false;
                //}, 1000);
            });
    }

    me.ProcessSuggor = function () {
        if (me.data.MovingCode === undefined || me.data.MovingCode == null) {
            MsgBox("Moving Code tidak boleh kosong!!!", MSG_ERROR);
            return;
        }
        else if (me.data.SupplierCode === undefined || me.data.SupplierCode == null) {
            MsgBox("Kode Supplier tidak boleh kosong!!!", MSG_ERROR);
            return;
        }
        else if (me.data.OrderType === undefined || me.data.OrderType == null) {
            MsgBox("Tipe Order harus pilih salah satu", MSG_ERROR);
            return;
        }
        else {
            me.isInProcess = true;

            $(".page .ajax-loader").fadeIn();

            $http.post('sp.api/pso/DoWork', me.data)
             .success(function (e) {
                 me.kgrid.detail = e;
                 if (e.success == false) {
                     MsgBox(e.message, MSG_INFO);
                     $(".page .ajax-loader").fadeOut();
                 }
                 else {

                     me.kgrid("DoWork");
                     setTimeout(function () {
                         $(".page .ajax-loader").fadeOut();
                     }, 25000);                    
                 }
                 
             });
        }
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/pso/processsuggorsave', { header: me.data, dtDetail: me.kgrid.detail }).
            success(function (e) {
                if (e.success) {
                    Wx.Success("berhasil save");
                    $('#SuggorStatus').html(e.lblstatus);
                    me.display.SuggorNo = e.display.SuggorNo;
                    me.display.SuggorDate = e.display.SuggorDate;
                    me.display.MovingCode = e.display.MovingCode;
                    me.display.OrderType = e.display.OrderType;
                    me.display.SupplierCode = e.display.SupplierCode;
                    me.display.SupplierName = e.display.SupplierName;

                    if (e.lblstatus == 'Open') {
                        me.readyToModified();
                        me.isPrintAvailable = true;
                        $('#btnProcess').attr('disabled', 'disabled');
                        $('#btnCreatePO').attr('disabled', 'disabled');
                    }
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.printPreview = function () {
        $http.post('sp.api/pso/reorderprint', { suggorNo: me.display.SuggorNo })
       .success(function (e) {
           if (e.success) {

               var par = me.display.SuggorNo + "," + me.display.SuggorNo + "," + "typeofgoods";
               var rparam = "Print SpRpTrn001";
               Wx.showPdfReport({
                   id: "SpRpTrn001",
                   pparam: par,
                   rparam: rparam,
                   textprint:true,
                   type: "devex"
               });

               me.isPrintAvailable = false;
               $('#SuggorStatus').html(e.lblstatus);
               $('#btnProcess').attr('disabled', 'disabled');
               $('#btnCreatePO').removeAttr('disabled');
           }
           else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) { });
    }

    me.CreatePO = function (e, param) {
        $http.post('sp.api/pso/processtopo', { suggorNo: me.display.SuggorNo, dtSuggor: me.kgrid.detail }).
            success(function (e) {
                if (e.success) {
                    Wx.Success("proses PO berhasil");
                    me.isPrintAvailable = false;
                    $('#btnProcess').attr('disabled', 'disabled');
                    $('#btnCreatePO').attr('disabled', 'disabled');
                    $('#btnDelete').hide();
                    $('#SuggorStatus').html(e.lblstatus);
                    me.display.POSNo = e.display.POSNo;
                    me.display.POSDate = e.display.POSDate;
                    me.Apply();
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/pso/delete', { suggorNo: me.display.SuggorNo }).
                    success(function (e) {
                        if (e.success) {
                            Wx.Info("Record has been deleted...");
                            $('#SuggorStatus').html(e.lblstatus);
                            $('#btnDelete').hide();
                            me.Apply();
                            //me.init();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    }).
                    error(function (e, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    };

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "btnSuggorView",
            title: "Suggor Code Lookup",
            url: 'sp.api/pso/PembelianBrowse',
            serverBinding: true,
            pageSize: 10,
            sort: [
		        { 'field': 'SuggorNo', 'dir': 'asc' },
            ],
            columns: [
            { field: "SuggorNo", title: "SuggorNo" },
            { field: "SuggorDate", title: "SuggorDate", type: "date", format: "{0:dd-MMM-yyyy}" },
            { field: "SupplierCode", title: "SupplierCode" },
            { field: "SupplierName", title: "SupplierName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.display.SuggorNo = data.SuggorNo;
                me.display.SuggorDate = data.SuggorDate;
                me.display.MovingCode = data.MovingCode;
                me.display.OrderType = data.OrderType;
                me.display.SupplierCode = data.SupplierCode;
                me.display.SupplierName = data.SupplierName;
                //me.partType();
                me.isSave = false;
                me.Apply();
                me.loadDetail(data.SuggorNo);
            }
        });
    }

    me.loadDetail = function (suggorNo) {
        layout.loadAjaxLoader();
        $http.post('sp.api/pso/getstatus', { suggorNo: suggorNo }).
            success(function (e) {
                me.kgrid("getTablePSO");
                //me.grid.detail = e.table;
                //me.loadTableData(me.grid1, me.grid.detail);

                $('#SuggorStatus').html(e.lblstatus);

                if (e.lblstatus == 'Printed') {
                    //me.readyToModified();
                    me.isPrintAvailable = false;
                    $('#btnProcess').attr('disabled', 'disabled');
                    $('#btnCreatePO').removeAttr('disabled');
                }

                if (e.lblstatus == 'Open') {
                    //me.readyToModified();
                    me.isPrintAvailable = true;
                    $('#btnProcess').attr('disabled', 'disabled');
                    $('#btnCreatePO').removeAttr('disabled');
                }
            }).
            error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.MovingCodeBrowse = function () {
        var lookup = Wx.klookup({
            name: "btnMovingCodeView",
            title: "Moving Code Lookup",
            url: "sp.api/grid/MovingCodeLookup",
            serverBinding: true,
            pageSize: 10,
            sort: [
		        { 'field': 'MovingCode', 'dir': 'asc' },
            ],
            filters: [
                { name: "MovingCode", text: "Moving Code", cls: "span4" },
                { name: "MovingCodeName", text: "Moving Name", cls: "span4" }
            ],
            columns: [
                { field: "MovingCode", title: "Moving Code" },
                { field: "MovingCodeName", title: "Moving Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.MovingCode = data.MovingCode;
                me.data.MovingCodeName = data.MovingCodeName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.SupplierCodeBrowse = function () {
        var lookup = Wx.klookup({
            name: "btnSupplierCodeView",
            title: "Supplier Lookup",
            url: "sp.api/grid/SupplierLookup",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'SupplierCode', dir: 'asc' }
            ],
            columns: [
                { field: "SupplierCode", title: "Kode Supplier", width: 120 },
                { field: "SupplierName", title: "Nama Supplier", width: 200 },
                { field: "Alamat", title: "Alamat", width: 500 },
                { field: "Diskon", title: "Diskon", width: 75 },
                { field: "Profit", title: "Profit Center", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.saveDetail = function (e) {
        $http.post('sp.api/pso/saveDetail', me.detail).
            success(function (e) {
                if (e.success) {
                    Wx.Success("Data saved...");

                    me.detail = {};
                    me.kgrid("getTablePSO");
                    $('#SuggorStatus').html(e.lblstatus);
                    me.isPrintAvailable = true;
                    $('#btnProcess').attr('disabled', 'disabled');
                    $('#btnCreatePO').attr('disabled', 'disabled');
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.cancelDetail = function () {
        me.detail = {};
    }

    me.cancelOrClose = function () {
        me.isPrintAvailable = false;
        me.init();
    }

    me.UpdateGridDetail = function (data) {
        me.grid.detail = data;
        me.loadTableData(me.grid1, me.grid.detail);
    };

    me.kgrid = function (controller) {
        
        var URL = "sp.api/pso/" + controller
        var data = controller == "DoWork" ? me.data : me.display;
        
        var lookup = Wx.kgrid({
            url: URL,
            name: "KGridSalesTarget",
            params: data,
            columns: [
            { field: "SeqNo", title: "No.", swidth: 5 },
            { field: "PartNo", title: "No. Part", swidth: 20 },
            { field: "AvailableQty", title: "Available", swidth: 15 },
            { field: "SuggorQty", title: "Qty. Suggor", swidth: 15 },
            { field: "SuggorCorrecQty", title: "Qty. Koreksi", swidth: 15 },
            ],
        });

        if (controller == "DoWork") {
            me.isLoadData = true;
            me.isSave = true;
            $('#btnProcess').attr('disabled', 'disabled');
        }
    }

    $("#KGridSalesTarget").on("click", "table", function (e) {
        Wx.selectedRow("KGridSalesTarget", function (e) {
            me.detail = e;
            me.Apply();
        });
    });

    me.initialize = function () {
        //me.clearTable(me.grid1);
        me.detail = {};
        me.display = {};

        me.partType();

        $('#SuggorStatus').html("New Suggor");
        $('#SuggorStatus').css(
            {
                "font-size": "28px",
                "color": "red",
                "font-weight": "bold",
                "text-align": "right"
            });

        //me.isPrintAvailable = true;
        $('#btnCreatePO').attr('disabled', 'disabled');
        $('#btnProcess').removeAttr('disabled');
        me.kgrid("getTablePSO");
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Proses Suggestion Order",
        xtype: "panels",
        toolbars: [ 
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!isInProcess", click: "browse()" },
                    //{ name: "btnEdit",   text: "Edit",   cls:"btn btn-primary",    icon: "icon-edit",   show: "isLoadData && !isSave", click: "allowEdit()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && !isSave || isPrintAvailable", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "isLoadData && isSave", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "isLoadData && isSave || isPrintAvailable", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData && !isSave", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
              {
                  name: "pnlStatus",
                  items: [
                      { name: "SuggorStatus", text: "", cls: "span4", readonly: true, type: "label" },
                      {
                          type: "buttons", cls: "span4", items: [
                              { name: "btnProcess", text: "Process Suggor", click: "ProcessSuggor()", cls: "btn", disable: false },
                              { name: "btnCreatePO", text: "Create PO", click: "CreatePO()", cls: "btn", disable: true },
                          ]
                      }
                  ]
              },
              {
                  name: "pnlA",
                  title: "Parameter Suggor",
                  items: [
                       {
                           text: "Moving Code",
                           type: "controls",
                           required: true,
                           items: [
                               { name: "MovingCode", cls: "span2", placeHolder: "Moving Code", readonly: true, type: "popup", click: "MovingCodeBrowse()", validasi: "required" },
                               { name: "MovingCodeName", cls: "span6", placeHolder: "Moving Name", readonly: true }
                           ]
                       },
                       {
                           text: "Pemasok",
                           type: "controls",
                           required: true,
                           items: [
                               { name: "SupplierCode", cls: "span2", placeHolder: "Kode Supplier", readonly: true, type: "popup", click: "SupplierCodeBrowse()", validasi: "required" },
                               { name: "SupplierName", cls: "span6  ", placeHolder: "Nama Supplier", readonly: true },
                           ]
                       },
                       { name: "OrderType", text: "Tipe Order", cls: "span3", type: "select2", datasource: "comboOrderType" },
                       { name: "PartType", text: "Tipe Part", cls: "span5", readonly: true },
                  ]
              },
              {
                  name: "pnlPOS",
                  title: "",
                  items: [
                      { name: "POSNo", text: "No. POS", model: "display.POSNo", cls: "span3", readonly: true },
                      { name: "POSDate", text: "Tgl. POS", model: "display.POSDate", type: "ng-datepicker", cls: "span3", readonly: true },
                  ]
              },
              {
                  name: "pnlSuggor",
                  title: "",
                  items: [
                          { name: "SuggorNo", text: "No. Suggor", model: "display.SuggorNo", cls: "span3", readonly: true },
                          { name: "SuggorDate", text: "Tgl. Suggor", model: "display.SuggorDate", type: "ng-datepicker", cls: "span3", readonly: true},
                          { name: "MovingCode", text: "Moving Code", model: "display.MovingCode", cls: "span3", readonly: true },
                          { name: "OrderType", text: "Tipe Order", model: "display.OrderType", cls: "span3", readonly: true },
                          { 
                              text: "Pemasok",
                              type: "controls",
                              items: [
                                  { name: "SupplierCode", model: "display.SupplierCode", cls: "span2", placeHolder: "Kode Supplier", readonly: true },
                                  { name: "SupplierName", model: "display.SupplierName",cls: "span6", placeHolder: "Nama Supplier", readonly: true },
                              ]
                          },
                  ]
              },
              {
                  name: "pnlC",
                  title: "",
                  items: [
                          { name: "SeqNo", model: "detail.SeqNo", text: "No", cls: "span2", readonly: true },
                          { name: "PartNo", model: "detail.PartNo", text: "No. Part", cls: "span4", readonly: true },
                          { name: "AvailableQty", model: "detail.AvailableQty", text: "Available Qty", cls: "span2", readonly: true },
                          { name: "SuggorQty", model: "detail.SuggorQty", text: "Qty. Suggor", cls: "span2", readonly: true },
                          { name: "SuggorCorrecQty", model: "detail.SuggorCorrecQty", text: "Qty. Koreksi", cls: "span2", disable: "display.SuggorNo === undefined" },
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnSaveDtl", text: "Save", icon: "icon-save", cls: "btn btn-info", click: "saveDetail()", show: true, disable: "detail.PartNo === undefined" },
                                      { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "cancelDetail()", show: true, disable: "detail.PartNo === undefined" }
                              ]
                          },
                  ]
              },
              {},
              {
                  name: "KGridSalesTarget",
                  xtype: "k-grid",
              },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spPSOController");
    }

});