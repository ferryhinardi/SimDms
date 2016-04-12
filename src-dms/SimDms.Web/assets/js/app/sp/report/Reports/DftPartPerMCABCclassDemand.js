"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });

   $http.post('sp.api/Combo/MovingCode?').
   success(function (data, status, headers, config) {
       me.MovingCode = data;
   });

   me.AbcClass = [
         { "value": '0', "text": 'A' },
         { "value": '1', "text": 'B' },
         { "value": '2', "text": 'C' }
   ];

    $http.post('sp.api/Combo/Months?').
    success(function (data, status, headers, config) {
        me.Months = data;
    });

    $http.post('sp.api/Combo/Years?').
    success(function (data, status, headers, config) {
        me.Years = data;
    });
    
    me.printPreview = function () {
        var prm = [
                   me.data.Years,
                   me.data.Months,
                   me.data.Years1,
                   me.data.Months1,
                   me.data.TypeOfGoods,
                   me.data.MovingCode,
                   me.data.AbcClass,
                   me.data.Nilai,
                   me.data.History
        ];
        Wx.showPdfReport({
            id: "SpRpTrn040",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Years = new Date().getFullYear();
        me.data.Months = (new Date().getMonth() + 1);
        me.data.Years1 = new Date().getFullYear();
        me.data.Months1 = (new Date().getMonth() + 1);
        //me.data.Period1 = '1-' + me.now("MMM-YYYY");
        //me.data.Period2 = me.now("DD-MMM-YYYY");

        me.isPrintAvailable = true;
        
        $(".switch:last").on("click", function () {
            var name = 'TypePart';
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            if (value)
                me.data.TypePart = false
            else
                me.data.TypePart = true;

            me.Apply();
        });

        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

    }
    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Part Per MC, ABC classD dan Demand",
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
                        {
                            text: "Start Date", type: "controls", cls: "span6 full",
                            items: [
                                     { name: 'Month', model: "data.Months", text: "Month", type: "select2", cls: "span3 full", optionalText: "-- SELECT MONTH --", datasource: "Months" },
                                     { name: 'Years', model: "data.Years", text: "Year", type: "select2", cls: "span2", optionalText: "-- SELECT YEAR --", datasource: "Years" },
                            ]
                        },
                         {
                             text: "End Date", type: "controls", cls: "span6 full",
                             items: [
                                      { name: 'Month1', model: "data.Months1", text: "Month", type: "select2", cls: "span3 full", optionalText: "-- SELECT MONTH --", datasource: "Months" },
                                      { name: 'Years1', model: "data.Years1", text: "Year", type: "select2", cls: "span2", optionalText: "-- SELECT YEAR --", datasource: "Years" },
                             ]
                         },
                        { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", readonly: true, type: "select2", text: "Type Part", datasource: "comboTPGO", disable: "data.TypeOfGoods" },
                        { name: "MovingCode", model: "data.MovingCode", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "data.MovingCode", type: "select2", text: "Moving Code", datasource: "MovingCode" },
                        { name: "AbcClass", model: "data.AbcClass", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "data.AbcClass", type: "select2", text: "ABC Class", datasource: "AbcClass" },
                        //{ text: "Type Part",type: "controls",             
                        //    items: [
                        //            { name: "TypePart", text: "Type Part", cls: "span1", type: "switch", float: "left" },
                        //            { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span1 full", readonly: true, type: "select2", text: "Type Part", datasource: "comboTPGO", disable: "isPrintAvailable" },
                        //    ]
                        //},
                        //{
                        //    text: "Moving Code", type: "controls",
                        //    items: [
                        //            { name: "MvngCode", text: "No. Part", cls: "span1", type: "switch", float: "left" },
                        //            { name: "MovingCode", model: "data.MovingCode", opt_text: "[SELECT ALL]", cls: "span1 full", disable: "data.MvngCode", type: "select2", text: "Type Part", datasource: "MovingCode"},
                        //    ]
                        //},
                        //{
                        //    text: "ABC Class", type: "controls",
                        //    items: [
                        //            { name: "ABCcode", text: "ABC Class", cls: "span1", type: "switch", float: "left" },
                        //            { name: "AbcClass", model: "data.AbcClass", opt_text: "[SELECT ALL]", cls: "span1 full", disable: "data.ABCcode", type: "select2", text: "Type Part", datasource: "AbcClass"},
                        //    ]
                        //},
                        {
                            type: "optionbuttons",
                            name: "History",
                            model: "isHistory",
                            text: "History",
                            items: [
                                        { name: "0", text: "Demand" },
                                        { name: "1", text: "Sales" }          
                            ]
                        },
                        {
                            type: "optionbuttons",
                            name: "Nilai",
                            model: "isNilai",
                            text: "Nilai",
                            items: [
                                        { name: "0", text: "Flag" },
                                        { name: "1", text: "QTY" }   
                            ]
                        },
                         {
                             type: "optionbuttons",
                             name: "Report",
                             model: "isReport",
                             text: "Report",
                             items: [
                                         { name: "0", text: "Excel" },
                                         { name: "1", text: "TXT" }
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
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptLstMstSparePart");
    }
});