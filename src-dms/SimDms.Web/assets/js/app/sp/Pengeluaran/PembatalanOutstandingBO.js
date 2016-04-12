"use strict";

function spPembatalanOutstandingBOController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/TransactionType?CodeId=SLTP').
    success(function (data, status, headers, config) {
        me.cmbSales = data;
        me.init();
    });

    $('#SalesType').on('change', function (e) {
        var index = $('#SalesType').select2("val");
        switch (index) {
            case "0":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTPJ').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "1":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTNP').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "2":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTSR').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "3":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTSL').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
        }

        //me.init();
    });


    me.rawQuery = function () {
        var bc = me.data.BranchCode;
        var st = $('#SalesType').select2("val");
        var tt = $('#TransType').select2("val");
        if ($('#CustomerCode').val() == null || $('#CustomerCode').val() == '') {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        var url = "sp.api/pengeluaran/GetDataOutstandingBO?CompanyCode=" + me.data.CompanyCode
                    + '&BranchCode=' + bc + '&CustomerCode=' + me.data.CustomerCode
                    + '&TrsType=' + tt + '&TypeOfGoods=' + me.data.TypeOfGoods
                    + '&SlsType=' + st + '&ProductType=' + me.data.ProductType;
        layout.loadAjaxLoader();
        $http.post(url)
            .success(function (data, status, headers, config) {
                if (data.success == true) {
                    me.loadTableData(me.gridOutsBO, data);
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
                console.log(data.data);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });

        console.log(me.data.CompanyCode, bc, me.data.CustomerCode, tt, me.data.TypeOfGoods, st, me.data.ProductType);
    }

    me.Update = function () {
        var SONo = $('#SONo').val();
        var PartNo = $('#PartNo').val();
        var PartNoOriginal = $('#PartNoOriginal').val();
        var QtyBOOts = $('#QtyBOOts').val();
        var QtyBOCancel = $('#QtyBOCancel').val();
        var Note = $('#Note').val();

        var st = $('#SalesType').select2("val");
        var tt = $('#TransType').select2("val");
        var wh ="00";

        var tax = '10';

        MsgConfirm("Apakah anda yakin melakukan pembatalan Outstanding BO ?", function (result) {
            if (result) {
                $http.post("sp.api/CancelBoOutstanding/ProsesBatalBO", { CompanyCode: me.data.CompanyCode, BranchCode: me.data.BranchCode, PartNo: PartNoOriginal, SalesType: st, QtyBo: QtyBOCancel, warehouseCode: wh, NoSO: SONo, User: me.data.User, PartNoOri: PartNoOriginal, Tax: tax, QtyOut: QtyBOOts, CustomerCode: me.data.CustomerCode, TransType: tt, Note: Note })
               .success(function (e) {
                   if (e.success) {
                       Wx.Success(e.message);
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
              .error(function (e) {
                  MsgBox(e, MSG_ERROR);
              });
            }

            console.log(QtyBOCancel);
        });
    }

    me.Cancel = function () {
        $('#SONo').val('');
        $('#SODate').val('');
        $('#PartNoOriginal').val('');
        $('#QtyBOOts').val('');
        $('#QtyBOCancel').val('');
        $('#Note').val('');
        me.clearTable(me.gridOutsBO);
        me.rawQuery();
        me.button();
        
        console.log(me.data.User);
    }

    me.button = function () {
        if ($('#SONo').val() == '') {
            $('#btnUpdate').attr('disabled', true);
            $('#btnCancel').attr('disabled', true);
        }
        else {
            $('#btnUpdate').removeAttr('disabled');
            $('#btnCancel').removeAttr('disabled');
        }
    }

    me.CustomerCode = function () {
        var lookupstring = "CustomerCodeLookup";
        var lookup = Wx.blookup({
            name: "CustomerLookup",
            title: "Master Customer Lookup",
            manager: pengeluaranManager,
            query: new breeze.EntityQuery.from(lookupstring).withParameters({ status: '1' }),
            defaultSort: "CustomerCode asc",

            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
                { field: 'Address', title: 'Address' },
                { field: 'ProfitCenter', title: 'Profit Center' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
            }
            me.Apply();
        });
    };

    me.gridOutsBO = new webix.ui({
        container: "OutsBO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "DocNo", header: "No. SO", width: 200 },
            { id: "DocDate", header: "Tgl. SO", width: 200, format: me.dateFormat },
            { id: "PartNo", header: "No. Part", width: 200 },
            { id: "PartNoOriginal", header: "No. Part Original", width: 250 },
            { id: "QtyBOOts", header: "BO Outstanding", width: 100 },
            { id: "QtyBOCancel", header: "BO Cancel", width: 100 },
        ],

        on: {
            onSelectChange: function () {
                if (me.gridOutsBO.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridOutsBO.getSelectedId().id);

                    $('#SONo').val(data.DocNo);
                    $('#SODate').val(moment(data.DocDate).format('DD-MMM-YYYY'));
                    $('#PartNo').val(data.PartNo);
                    $('#PartNoOriginal').val(data.PartNoOriginal);
                    $('#QtyBOOts').val(data.QtyBOOts);
                    $('#QtyBOCancel').val(data.QtyBOCancel);

                    me.button();
                }
            }
        }
    });
    
    me.initialize = function () {
        me.data = {};
        $http.get('breeze/SparePart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              me.data.TypeOfGoods = dl.TypeOfGoods;
              me.data.ProductType = dl.ProductType;
              me.data.User = dl.UserId;
          });
        $('#btnUpdate').attr('disabled', true);
        $('#btnCancel').attr('disabled', true);
        me.gridOutsBO.adjust();
    }    

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Cancel BO Outstanding",
        xtype: "panels",
        toolbars: [
            { name: "btnQuery", text: "Query", icon: "icon-search", click: "rawQuery()" },
            //{ name: "btnPrint", text: "Print", icon: "icon-print" }
        ],
        panels: [
            {
                name: "pnlMain",
                title: "",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Company", cls: "span4 full", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Branch", cls: "span4 full", show: false },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", text: "Tipe Barang", cls: "span4 full", show: false },
                    { name: "ProductType", model: "data.ProductType", text: "Tipe Produk", cls: "span4 full", show: false },
                    { name: "Ppn", model: "data.Ppn", text: "PPn", cls: "span4 full", show: false },
                    { name: "User", model: "data.User", text: "PPn", cls: "span4 full", show: false },

                    { name: "SalesType", text: "Sales Type", type: "select2", cls: "span4", datasource: 'cmbSales' },
                    { name: "TransType", text: "Transaction Type", type: "select2", cls: "span4", datasource: 'cmbTrans' },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", type: "popup", readonly: false, click: "CustomerCode()", required: true, validasi: "required" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    }
                ]
            },
            {
                name: "pnlDetail",
                title: "",
                items: [
                        { name: "SONo", text: "SO No.", cls: "span4", readonly: true },
                        { name: "SODate", text: "SO Date", cls: "span4", readonly: true },
                        { name: "PartNoOriginal", text: "Part No. Original", cls: "span4", readonly: true },
                        { name: "QtyBOOts", text: "BO Out.", cls: "span4", readonly: true },
                        { name: "QtyBOCancel", text: "BO Cancel", cls: "span4 number-int" },
                        { name: "Note", text: "Note", cls: "span4" },
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnUpdate", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "Update()" },
                                     { name: "btnCancel", text: "Cancel", icon: "icon-cancel", cls: "btn btn-success", click: "Cancel()" },
                             ]
                         },
                        {
                            name: "gridSODetail",
                            type: "wxdiv",
                        },
                ]
            },
             {
                 xtype: "tabs",
                 name: "tabpage1",
                 items: [
                     { name: "tA", text: "Outstanding BO", cls: "active" },
                 ]
             },
                    {
                        name: "OutsBO",
                        title: "OutsBO",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spPembatalanOutstandingBOController");
    }

});