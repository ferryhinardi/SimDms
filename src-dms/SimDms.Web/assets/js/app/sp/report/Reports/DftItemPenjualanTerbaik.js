"use strict";
function spDftItemPenjualanTerbaik($scope, $http, $injector) {

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

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });

    me.printPreview = function () {
        var tipe = "", cabang, movingCode = "", tipePart = "TIPE PART : ";

       if (me.data.TypeOfGoods == "")
        {
            tipe = "";
            tipePart += "SEMUA";
        }
        else
        {
            tipe = me.data.TypeOfGoods;
            tipePart += $("#TypeOfGoods option:selected").text();
        }

       cabang = me.data.PerCabang;

        var listMoving = ["1","2","3","4","5","0"];
        if (me.data.MovingCode0) { movingCode += "|" + listMoving[0]; }
        if (me.data.MovingCode1) { movingCode += "|" + listMoving[1]; }
        if (me.data.MovingCode2) { movingCode += "|" + listMoving[2]; }
        if (me.data.MovingCode3) { movingCode +="|" + listMoving[3]; }
        if (me.data.MovingCode4) { movingCode += "|" + listMoving[4]; }
        if (me.data.MovingCode5) { movingCode += "|" + listMoving[5]; }

        if (movingCode == "" || movingCode == null)
        { MsgBox("Harap pilih moving code yang dikehendaki!", MSG_INFO); return; }

        movingCode = movingCode.substring(1);

        var period =  me.data.Year.toString() + "-" + me.data.Month.toString() + "-" + "01";

        var prm = [
            me.data.ABCClass,
            movingCode = "movingCode|" + movingCode,
            period,
            tipe,
            cabang
        ];

        Wx.showPdfReport({
            id: "SpRpSum002",
            pparam: prm.join(","),
            rparam: tipePart,
            type: "devex"
        });
    };

    me.initialize = function () {
        me.data = {};
        me.data.PartNo = '';
        me.data.PerCabang = 0;
        me.isPrintAvailable = true;

        me.dsABCClass = [
            { "value": "A", "text": "A" }, 
            { "value": "B", "text": "B" }, 
            { "value": "C", "text": "C" }
        ];
        
        $("#ABCClass option:first").val("%");
        $("#ABCClass option:first").text("%");
        me.data.ABCClass = "%";
       
        $("[name=PerCabang]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.PerCabang = (value) ? 1 : 0;
            me.Apply();
        });

        $("[name=MovingCode0]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode0 = value;
            me.Apply();
        });

        $("[name=MovingCode1]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode1 = value;
            me.Apply();
        });

        $("[name=MovingCode2]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode2 = value;
            me.Apply();
        });

        $("[name=MovingCode3]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode3 = value;
            me.Apply();
        });
        
        $("[name=MovingCode4]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode4 = value;
            me.Apply();
        });

        $("[name=MovingCode0] ").prop('checked', 'cheked').trigger('change');
        $("[name=MovingCode1] ").prop('checked', 'cheked').trigger('change');
        $("[name=MovingCode2] ").prop('checked', 'cheked').trigger('change');
        $("[name=MovingCode3] ").prop('checked', 'cheked').trigger('change');
        $("[name=MovingCode4] ").prop('checked', 'cheked').trigger('change');
        $("[name=MovingCode5] ").prop('checked', 'cheked').trigger('change');

        $("[name=MovingCode5]").on("change", function () {
            var name = this.name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            me.data.MovingCode5 = value;
            me.Apply();
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
        title: "Dafar Item Dengan Penjualan Terbaik",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
				    { name: "ABCClass", model: "data.ABCClass", text: "ABC Class", type: "select2", cls: "span3 full", datasource: "dsABCClass", opt_text: "%"},
                    {
                        text: "Moving Code", type: "controls",
                        items: [
                            { name: "MovingCode0", model: "data.MovingCode0", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "0", cls: "span1 switchlabel" },
                            { name: "MovingCode1", model: "data.MovingCode1", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "1", cls: "span1 switchlabel" },
                        ]
                    },
                    {
                        text: "", type: "controls",
                        items: [
                            { name: "MovingCode2", model: "data.MovingCode2", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "2", cls: "span1 switchlabel" },
                            { name: "MovingCode3", model: "data.MovingCode3", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "3", cls: "span1 switchlabel" }
                        ]
                    },
                     {
                         text: "", type: "controls",
                         items: [
                            { name: "MovingCode4", model: "data.MovingCode4", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "4", cls: "span1 switchlabel" },
                            { name: "MovingCode5", model: "data.MovingCode5", type: "switch", cls: "span1", float: "left" },
                            { type: "label", text: "5", cls: "span1 switchlabel" }
                         ]
                     },
                    { name: "Month", model: "data.Month", text: "Bulan", type: "select2", cls: "span3 full", datasource: "dsMonth" },
				    { name: "Year", model: "data.Year", text: "Tahun", type: "select2", cls: "span3 full", datasource: "dsYear" },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO" },
                    {
                        text: "", type: "controls",
                        items: [
                            { name: "PerCabang", model: "data.PerCabang", type: "switch", cls: "span1 full", float: "left" },
                            { type: "label", text: "Per-Cabang", cls: "span1 switchlabel" }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 10px 0px 5px")
        //$(".eraser").attr("style", "height:32px;width:26px; margin-left:-7px;padding: 2px 13px 0 13px")
        SimDms.Angular("spDftItemPenjualanTerbaik");
    }
});