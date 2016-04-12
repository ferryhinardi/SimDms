"use strict";
function spRptKartuStock($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.dsMonth = data;
    });

    $http.post('sp.api/Combo/YearsOld').
    success(function (data, status, headers, config) {
        me.dsYear = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=WRCD').
    success(function (data, status, headers, config) {
        data = $(data).filter(function () {
            return (this.value <= '99');
        });

        me.comboWRCD = data;
    });

    me.PartLkp = function () {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Item Lookup",
            manager: spManager,
            query: new breeze.EntityQuery().from("SparePartLookup").withParameters({ partNo: me.data.PartNo }),
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "No Part" },
                { field: "PartName", title: "Nama Part" },
                { field: "SupplierCode", title: "Kode Suplier" },
                { field: "IsGenuinePart", title: "Prd Suzuki" },
                { field: "ProductType", title: "Tipe Produk" },
                { field: "PartCategory", title: "Kategori" },
                { field: "CategoryName", title: "Nama Kategori" },
                { field: "TypeOfGoods", title: "Tipe Part" }
            ]
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.data.PartName = data.PartName;
                me.Apply();
            }
        });
    };

    me.DelPart = function(){
        me.data.PartNo = "";
        me.data.PartName = "";
    }

    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
        { MsgBox("Kode Gudang tidak boleh kosong!", MSG_INFO); return; }
        
        var prm = [
            me.data.Month,
            me.data.Year,
            me.data.WarehouseCode,
            me.data.PartNo
        ];

        Wx.showPdfReport({
            id: me.data.IncludePrice ? "SpRpSum001" : "SpRpSum016",
            pparam: prm.join(','),
            rparam: ($("#Month").select2('data').text + ',' + $("#Year").select2('data').text),
            type: "devex"
        });
    };

    me.initialize = function () {
        me.data = {};
        me.data.WarehouseCode = '';
        me.data.PartNo = '';
        me.data.IncludePrice = false;
        me.isPrintAvailable = true;

        $("[name=IncludePrice]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.IncludePrice = value
            me.Apply();
        });

        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        $http.post('sp.api/SpInquiry/DefaultReportKartuStock').
        success(function (data, status, headers, config) {
            me.data.Month = data.Month;
            me.data.Year = data.Year;
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Kartu Stock",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span3 full", disable: "isPrintAvailable" },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable" },                       
				    { name: "Month", model: "data.Month", text: "Bulan", type: "select2", cls: "span3 full", datasource: "dsMonth" },
				    { name: "Year", model: "data.Year", text: "Tahun", type: "select2", cls: "span3 full", datasource: "dsYear" },
                    { name: "WarehouseCode", model: "data.WarehouseCode", text: "Gudang", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", datasource: "comboWRCD", validasi: "required", opt_text: "-- Pilih Gudang --" },
                    {
                        text: "No. Part", type: "controls",
                        items: [
                            { name: "PartNo", model: "data.PartNo", cls: "span3", placeHolder: "-- SEMUA PART --", readonly: true, type: "popup", click: "PartLkp()" },
                            {
                            type: "buttons", items: [
                                    { name: "btnDelPart", text: "", icon: "icon-eraser", cls: "span1 eraser", click: "DelPart()" },
                                ]
                            }
                        ]
                    },
                    { name: "PartName", model: "data.PartName", text: "Nama Part", cls: "span4 full", placeHolder: "Nama Part", readonly: true },
                    { name: "IncludePrice", model: "data.IncludePrice", text: "Tampilkan Harga", type: "switch", cls: "span3 full", float: "left" }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".eraser").attr("style", "height:32px;width:26px; margin-left:-7px;padding: 2px 13px 0 13px")
        SimDms.Angular("spRptKartuStock");
    }
});