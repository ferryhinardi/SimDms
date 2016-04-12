"use strict"

function followup($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    
    me.SALES = 10;
    me.SALES_CO = 20;
    me.SALES_HEAD = 30;
    me.BRANCH_MANAGER = 40;
    me.COO = 50;
    me.SALES_ADMIN = 60;

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.grid1 = webix.ui({
        container: "wxfollowup",
        view: "wxtable", css:"alternating",
        leftSplit: 3,
        columns: [
            { id: "InquiryNumber", header: "No", width: 50 },
            { id: "Pelanggan", header: "Pelanggan", width: 100 },
            { id: "InquiryDate", header: "Tgl KDP", width: 70 },
            { id: "TipeKendaraan", heade: "Tipe", width: 120 },
            { id: "Variant", header: "Varian", width: 120 },
            { id: "Transmisi", header: "AT/MT", width: 60 },
            { id: "Warna", header: "Warna", width: 100 },
            { id: "PerolehanData", header: "Perolehan Data", width: 80 },
            { id: "Employee", header: "Wiraniaga", width: 110 },
            { id: "Supervisor", header: "Koordinator", width: 110 },
            { id: "NextFollowUpDate", header: "NextFollowUp", width: 70 },
            { id: "LastProgress", header: "Last Progress", width: 70 },
            { id: "ActivityDetail", header: "Follow Up Detail", width: 200 }
        ]
    });

    me.getPositionId = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: "its.api/InquiryFollowUp/GetPositionId",
            success: function (data) {
                me.positionId = data;
            }
        });
    }

    me.getOutletItems = function (useBranch) {
        $.ajax({
            async: false,
            type: "POST",
            data: { useBranch: useBranch },
            url: 'its.api/InquiryFollowUp/ComboSourceBranchOutlets',
            success: function (data) {
                me.dsOutlet = data.list;
            }
        });
    }

    me.setDataSourceEachComboBox = function (useBranch) {
        me.bindCombo(cmbSalesHead, me.SALES_HEAD, useBranch);
        //me.bindCombo(cmbSalesCo, me.SALES_CO, useBranch);
        me.bindCombo(cmbSalesman, me.SALES, useBranch);
    }

    me.bindCombo = function (sender, positionID, useBranch) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                positionID: positionID,
                useBranch: useBranch
            },
            url: 'its.api/InquiryFollowUp/ComboSourceEmployeeByPosition',
            success: function (data) {
                switch (sender) {
                    case cmbSalesHead:
                        me.dsSalesHead = data.list;
                        break;
                    //case cmbSalesCo:
                    //    me.dsSalesCo = data.list;
                    //    break;
                    case cmbSalesman:
                        me.dsSalesman = data.list;
                        break;
                    default: break;
                }
            }
        });
    }

    me.getUserEmployeeName = function (positionID) {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryFollowUp/GetUserEmployee',
            success: function (data) {
                var control = positionID ==
                    me.SALES_HEAD ? "#cmbSalesHead" :
                    //me.SALES_CO ? "#cmbSalesCo" :
                    me.SALES ? "#cmbSalesman" : "";
                if (control != "") $(control).select2('val', data.EmployeeID);
            }
        });
    }

    me.getChildPosition = function (positionID) {
        $.ajax({
            async: false,
            type: "POST",
            data: { positionID: positionID },
            url: 'its.api/InquiryFollowUp/ComboSourceChildPosition',
            success: function (data) {
                if (data.message == "") {
                    switch (positionID) {
                        case me.SALES_HEAD:
                            me.dsSalesCo = data.list;
                            me.bindCombo(cmbSalesman, me.SALES, true);
                            break;
                        //case me.SALES_CO:
                        //    me.dsSalesman = data.list;
                        //    break;
                        default: break;
                    }
                } else {
                    MsgBox(data.message, MSG_ERROR);
                    $('#cmbOutlet').attr('disabled', 'disabled');
                    $('#cmbSalesHead').attr('disabled', 'disabled');
                    $('#cmbSalesCo').attr('disabled', 'disabled');
                    $('#cmbSalesman').attr('disabled', 'disabled');
                    $('#btnSearch').attr('disabled', 'disabled');
                }
            }
        });
    }

    me.getParentPosition = function (positionID, userID) {
        $.ajax({
            async: false,
            type: "POST",
            data: { positionID: positionID, userID: userID },
            url: 'its.api/InquiryFollowUp/ComboSourceParentPosition',
            success: function (data) {
                if (data.message == "") {
                    switch (positionID) {
                        case me.SALES_CO:
                            $('#cmbSalesHead').select2('val', data.result.EmployeeName);
                            break;
                        case me.SALES:
                            $('#cmbSalesCo').select2('val', data.result.EmployeeName);
                            me.getParentPosition(me.SALES_CO, data.result.UserID);
                            break;
                        default: break;
                    }
                    $('#cmbOutlet').select2('val', data.result.OutletName);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }
        });
    }

    me.comboOnChange = function (sender) {
        if (me.isInitial) return;
        switch (sender) {
            case 'cmbSalesHead':
                var id = $('#cmbSalesHead').select2('val');
                if (id == "") return; break;
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        positionID: me.SALES_HEAD,
                        employeeID: id,
                        isCOO: me.isCOO
                    },
                    url: 'its.api/InquiryFollowUp/ComboOnChange',
                    success: function (data) {
                        me.dsSalesCo = data.list;
                    }
                });
                break;
            case 'cmbSalesCo':
                var id = $('#cmbSalesCo').select2('val');
                if (id == "") return; break;
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        positionID: me.SALES_CO,
                        employeeID: id,
                        isCOO: me.isCOO
                    },
                    url: 'its.api/InquiryFollowUp/ComboOnChange',
                    success: function (data) {
                        me.dsSalesman = data.list;
                        $('#cmbOutlet').select2('val', data.outlet != null ? data.outlet.OutletID : "");
                    }
                });
                break;
            default: break;
        }
    }

    me.btnGenerateClick = function () {
        var outlet = $('#cmbOutlet').select2('val');
        var head = $('#cmbSalesHead').select2('val');
        //var spv = $('#cmbSalesCo').select2('val');
        var emp = $('#cmbSalesman').select2('val');
        var param = "0";
    }

    me.btnSearchClick = function () {

    }

    me.initialize = function () {
        me.positionId = 0;
        me.isInitial = true;
        me.isCOO = false;
        me.data.DateFrom = me.data.DateTo = new Date();
        me.data.ITSSIS = true;
        me.getPositionId();
        $('#cmbOutlet').removeAttr('disabled');
        $('#cmbSalesHead').removeAttr('disabled');
        //$('#cmbSalesCo').removeAttr('disabled');
        $('#cmbSalesman').removeAttr('disabled');

        if (!me.isCOO) {
            me.getOutletItems(true);
            if (me.positionId != 0) {
                me.setDataSourceEachComboBox(true);
            }
        }
        
        switch (me.positionId) {
            case me.COO:
                me.isInitial = false;
                me.isCOO = true;
                me.getOutletItems(false);
                me.setDataSourceEachComboBox(false);
                $('#btnSearch').removeAttr("disabled");
                break;
            case me.SALES_ADMIN:
                $('#btnSearch').removeAttr("disabled");
                break;
            case me.BRANCH_MANAGER:
                me.isInitial = false;
                $('#btnSearch').removeAttr("disabled");
                break;
            case me.SALES_HEAD:
                me.isInitial = false;
                me.bindCombo(cmbSalesHead, me.SALES_HEAD, true);
                me.getUserEmployeeName(me.SALES_HEAD);
                me.getChildPosition(me.SALES_HEAD);
                $('#btnSearch').attr("disabled", "disabled");
                break;
            //case me.SALES_CO:
            //    me.isInitial = true;
            //    me.bindCombo(cmbSalesCo, me.SALES_CO, true);
            //    me.getUserEmployeeName(me.SALES_CO);
            //    me.getParentPosition(me.SALES_CO, "");
            //    me.getChildPosition(me.SALES_CO);
            //    $('#btnSearch').attr("disabled", "disabled");
            //    break;
            case me.SALES:
                me.isInitial = true;
                me.bindCombo(cmbSalesman, me.SALES, true);
                me.getParentPosition(me.SALES, "");
                $('#btnSearch').attr("disabled", "disabled");
                break;
            default:
                MsgBox("User belum di-setting di Master Position !", MSG_ERROR);
                me.dsOutlet = {};
                break;
        }
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry - Follow Up",
        xtype: "panels",
        toolbars: [
        ],
        panels: [
             {
                 items: [
                     {
                         text: "Date (From - To)",
                         cls: "span5",
                         type: "controls", items: [
                             { name: "DateFrom", text: "Date From", cls: "span4", type: "ng-datepicker" },
                             { name: "DateTo", text: "Date To", cls: "span4", type: "ng-datepicker" },
                         ]
                     },
                     {
                         text: "Detail Data",
                         cls: "span3",
                         type: "controls", items: [
                             { name: "Detail", type: "ng-check" },
                         ]
                     },
                     {
                         text: "Outlet",
                         cls: "span5",
                         type: "controls", items: [
                             {
                                 name: "cmbOutlet", type: "select2", opt_text: "[SELECT ALL]", datasource: "dsOutlet", change: "comboOnChange('cmbOutlet')"
                             },
                         ]
                     },
                     {
                         text: "ITS ke SIS",
                         cls: "span3",
                         type: "controls", items: [
                             { name: "ITSSIS", type: "ng-check" },

                         ]
                     },
                     {
                         text: "Sales Head (SH)",
                         cls: "span5",
                         type: "controls", items: [
                             { name: "cmbSalesHead", type: "select2", opt_text: "[SELECT ALL]", datasource: "dsSalesHead", change: "comboOnChange('cmbSalesHead')" },
                         ]
                     },
                     {
                         type: "buttons",
                         cls: "span3",
                         items: [
                             { name: "btnGenerate", cls: "btn btn-info", icon: "icon-gear", text: "Generate File", click: "btnGenerateClick()", style: "width:120px;margin:-100px" }
                         ]
                     },
                     //{
                     //    text: "Sales Coordinator (SC)",
                     //    cls: "span5",
                     //    type: "controls", items: [
                     //        { name: "cmbSalesCo", type: "select2", opt_text: "[SELECT ALL]", datasource: "dsSalesCo", change: "comboOnChange('cmbSalesCo')" },
                     //    ]
                     //},
                     {
                         type: "buttons",
                         cls: "span3",
                         items: [
                             { name: "btnSearch", cls: "btn btn-info", icon: "icon-search", text: "Search", click: "btnSearchClick()", style: "width:120px;margin:-100px" }
                         ]
                     },
                     {
                         text: "Salesman (S)",
                         cls: "span5",
                         type: "controls", items: [
                             { name: "cmbSalesman", type: "select2", opt_text: "[SELECT ALL]", datasource: "dsSalesman", change: "comboOnChange('cmbSalesman')" },
                         ]
                     },
                 ]
             },
             {
                 title: "List of Follow Up",
                 items: [
                     {
                         name: "wxfollowup",
                         title: "List of Follow Up",
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
        SimDms.Angular("followup");
    }
});

// bawah -> old
//$(document).ready(function () {
//    var widget = new SimDms.Widget({
//        title: "Inquiry - Follow up",
//        xtype: "panels",
//        panels: [
//            {
//                name: "pnlFilter",
//                items: [
//                    {
//                        text: "Date (From - To)",
//                        type: "controls", items: [
//                            { name: "DateFrom", text: "Date From", cls: "span2", type: "kdatepicker" },
//                            { name: "DateTo", text: "Date To", cls: "span2", type: "kdatepicker" },
//                        ]
//                    },
//                    {
//                        text: "Outlet",
//                        type: "controls", items: [
//                            { name: "OutletCode", text: "NIK", cls: "span4", readonly: true, type:"hidden" },
//                            { name: "OutletName", text: "Name", cls: "span4", readonly: true },
//                            { name: "CompanyCode", text: "Name", cls: "span2", readonly: true, type: "hidden" },
//                            { name: "BranchCode", text: "Name", cls: "span2", readonly: true, type:"hidden" },
//                        ]
//                    },
//                    {
//                        text: "Sales Head (SH)",
//                        type: "controls", items: [
//                            { name: "NikSH", text: "NIK", cls: "span4", readonly: true, type: "select" },
//                        ]
//                    },
//                    {
//                        text: "Sales Coordinator (SC)",
//                        type: "controls", items: [
//                            { name: "NikSC", text: "NIK", cls: "span4", readonly: true, type: "select" },
//                        ]
//                    },
//                    {
//                        text: "Salesman (S)",
//                        type: "controls", items: [
//                            { name: "Nik", text: "NIK", cls: "span4", readonly: true, type: "select" },
//                        ]
//                    },
//                    {
//                        text: "Detail Data",
//                        type: "controls", items: [
//                            { name: "Detail", cls: "span2", type: "switch" },
//                        ]
//                    },
//                    {
//                        text: "ITS ke SIS",
//                        type: "controls", items: [
//                            { name: "ITSSIS", cls: "span2", type: "switch" },
//                        ]
//                    },
//                ],
//            },
//            {
//                name: "pnlList",
//                title: "Followup Inquiry",
//                xtype: "kgrid",
//            },
//        ],
//        toolbars: [
//            { name: "btnRefresh", text: "Query", icon: "icon-refresh" },
//        ]
//    });

//    widget.render(init);
//    function init() {
//        $("#pnlList").css({width: 1400, height:400 });
//        widget.post("its.api/inquiryits/default", function (result) {
//            if (result.success) {
//                widget.default = result.data;
//                widget.select({ name: "Nik", data: result.data.EmpSLList });
//                widget.select({ name: "NikSC", data: result.data.EmpSCList, optionText: "-- ALL SC --", optionValue: "--" });
//                widget.select({ name: "NikSH", data: result.data.EmpSHList });
//                if (result.data.Position == "S") {
//                    widget.enable({ value: false, items: ["Nik", "NikSC", "NikSH"] })
//                    $('#Nik').val(result.data.EmpSLList[0].value);
//                };
//                if (result.data.Position == "SC") widget.enable({ value: false, items: ["NikSC", "NikSH"] });
//                if (result.data.Position == "SH") {
//                    widget.enable({ value: false, items: ["NikSH"] });
//                    widget.selectparam({ name: "Nik", url: "its.api/combo/employee", param: "NikSC", optionText: "-- ALL SALESMAN --" });
//                }
//                widget.populate(widget.default);
//            }
//            else {
//                widget.alert(result.message || "User belum terdaftar di Master Position !");
//                widget.showToolbars([]);
//            }
//        });
//    }

//    $('#btnRefresh').click(function () {
//        var params = {
//            Outlet:   $("[name=OutletCode]").val(),
//            DateFrom: getSqlDate($("[name=DateFrom]").val()),
//            DateTo: getSqlDate($("[name=DateTo]").val()),
//            SH: $("#NikSH").val(),
//            SC: $('#NikSC').val(),
//            SALES: $('#Nik').val(),
//            DetData: $("[name=Detail]").val(),
//            ItsSis: $("[name=ITSSIS]").val()
//        }

//        //var params = { DateFrom: "20100101", DateTo: "20140117", Outlet: "0601", SC: "50438", SALES: "52153", SH: "", ItsSis:"true" };

//        widget.kgrid({
//            url: "its.api/inquiryits/FollowUpInqury",
//            name: "pnlList",
//            params: params,
//            columns: [
//                { field: "InquiryNumber", title: "No. Inquiry", width: 100 },
//                { field: "Pelanggan", title: "Pelanggan", width: 200 },
//                { field: "InquiryDate", title: "Tgl Inquiry", width: 120, template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
//                { field: "TipeKendaraan", title: "Tipe", width: 120},
//                { field: "Variant", title: "Varian", width: 120 },
//                { field: "Transmisi", title: "AT/MT", width: 120 },
//                { field: "Warna", title: "Warna", width: 200 },
//                { field: "PerolehanData", title: "Perolehan Data", width: 120},
//                { field: "Employee", title: "Wiraniaga", width: 200 },
//                { field: "Supervisor", title: "Koordinator", width: 200 },
//                { field: "NextFollowUpDate", title: "Next Followup", width: 120, template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
//                { field: "LastProgress", title: "Last Progress", width: 150 },
//                { field: "ActivityDetail", title: "Follow Up Detail", width: 300 },
//            ],
//        });
//        console.log(params);
//    });

//    $('#btnExportXls').click(function () {

//        widget.showReport({
//            id: "PmRpInqFollowUpDet",
//            par: [$('#CompanyCode').val(), $('#BranchCode').val(), getSqlDate($("input[name='DateFrom']").val()), getSqlDate($("input[name='DateTo']").val()), $('#OutletCode').val(), $('#NikSC').val(),
//                    $('#Nik').val(), "", $('#NikSH').val()],
//            panel: "pnlList",
//            type: "rdlc"
//        });

//        widget.exportXls({
//            name: "pnlList",
//            type: "kgrid",
//            items: [
//                { field: "InquiryNumber", title: "No. Inquiry", width: 100 },
//                { field: "Pelanggan", title: "Pelanggan", width: 280 },
//                { field: "InquiryDate", title: "Tgl KDP", width: 120, type: "date" },
//                { field: "TipeKendaraan", title: "Tipe", width: 120},
//                { field: "Variant", title: "Varian", width: 200 },
//                { field: "Transmisi", title: "AT/MT", width: 180 },
//                { field: "Warna", title: "Warna", width: 200 },
//                { field: "PerolehanData", title: "Perolehan Data", width: 120},
//                { field: "Employee", title: "Wiraniaga", width: 300 },
//                { field: "Supervisor", title: "Koordinator", width: 300 },
//                { field: "NextFollowUpDate", title: "Next Follow Up", width: 300 },
//                { field: "LastProgress", title: "Last Progress", width: 300 },
//                { field: "LastCaseCategory", title: "Follow Up Detail", width: 300 },
//            ]
//        });
//    });

//    function getSqlDate(value) {
//        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
//    }
//});