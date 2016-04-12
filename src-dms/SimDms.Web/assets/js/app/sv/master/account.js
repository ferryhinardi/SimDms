var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function KSGController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $http.post('sv.api/svaccount/default', me.data).
               success(function (data, status, headers, config) {
                   me.data = data;
                   if (data.DescriptionSales == "") { me.btnSalesAccNo(); }
                   else if (data.DescriptionDiscount == "") { me.btnDiscAccNo(); }
                   else if (data.DescriptionReturn == "") { me.btnReturnAccNo(); }
                   else if (data.DescriptionCOGS == "") { me.btnCOGSAccNo(); }
                   else if (data.DescriptionPyb == "") { me.btnReturnPybAccNo(); }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
    }

    me.btnSalesAccNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.SalesAccNo = result.AccountNo;
                me.data.DescriptionSales = result.AccDescription;
                me.Apply();
            }
        });
    }

    $("[name='SalesAccNo']").on('blur', function () {
        if (me.data.SalesAccNo != null) {
            $http.post('sv.api/svaccount/SalesAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesAccNo = data[0].AccountNo;
                       me.data.DescriptionSales = data[0].AccDescription;
                   }
                   else {
                       me.data.SalesAccNo = "";
                       me.data.DescriptionSales = "";
                       me.btnSalesAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.btnDiscAccNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.DiscAccNo = result.AccountNo;
                me.data.DescriptionDiscount = result.AccDescription;
                me.Apply();
            }
        });
    }

    $("[name='DiscAccNo']").on('blur', function () {
        if (me.data.DiscAccNo != null) {
            $http.post('sv.api/svaccount/DiscAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.DiscAccNo = data[0].AccountNo;
                       me.data.DescriptionDiscount = data[0].AccDescription;
                   }
                   else {
                       me.data.DiscAccNo = "";
                       me.data.DescriptionDiscount = "";
                       me.btnDiscAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.btnReturnAccNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.ReturnAccNo = result.AccountNo;
                me.data.DescriptionReturn = result.AccDescription;
                me.Apply();
            }
        });
    
    }

    $("[name='ReturnAccNo']").on('blur', function () {
        if (me.data.ReturnAccNo != null) {
            $http.post('sv.api/svaccount/ReturnAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.ReturnAccNo = data[0].AccountNo;
                       me.data.DescriptionReturn = data[0].AccDescription;
                   }
                   else {
                       me.data.ReturnAccNo = "";
                       me.data.DescriptionReturn = "";
                       me.btnReturnAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.btnCOGSAccNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.COGSAccNo = result.AccountNo;
                me.data.DescriptionCOGS = result.AccDescription;
                me.Apply();
            }
        });
    }

    $("[name='COGSAccNo']").on('blur', function () {
        if (me.data.COGSAccNo != null) {
            $http.post('sv.api/svaccount/COGSAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.COGSAccNo = data[0].AccountNo;
                       me.data.DescriptionCOGS = data[0].AccDescription;
                   }
                   else {
                       me.data.COGSAccNo = "";
                       me.data.DescriptionCOGS = "";
                       me.btnReturnAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.btnReturnPybAccNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.ReturnPybAccNo = result.AccountNo;
                me.data.DescriptionPyb = result.AccDescription;
                me.Apply();
            }
        });
    }

    $("[name='ReturnPybAccNo']").on('blur', function () {
        if (me.data.ReturnPybAccNo != null) {
            $http.post('sv.api/svaccount/ReturnPybAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.ReturnPybAccNo = data[0].AccountNo;
                       me.data.DescriptionPyb = data[0].AccDescription;
                   }
                   else {
                       me.data.ReturnPybAccNo = "";
                       me.data.DescriptionPyb = "";
                       me.btnReturnAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.saveData = function (e, param) {
        $http.post('sv.api/svaccount/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                console.log(e);
            });
    }

    me.printPreview = function () {

        var ReportId = 'SvRpMst017';
        var par = [];
        //alert(par);
        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            type: "devex"
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Account",
        xtype: "panels",
        toolbars: [
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        text: "Acc No. Sales",
                        type: "controls",
                        items: [
                            {
                                name: "SalesAccNo", cls: "span3", text: "Sales Acc No.", maxlength: "100", type: "popup", btnName: "btnSalesAccNo", click: "btnSalesAccNo()"
                            },
                            {
                                name: "DescriptionSales", cls: "span5", text: "Description Sales", maxlength: "100", readonly: true
                            },

                        ]
                    },
                    {
                        text: "Acc No. Discount",
                        type: "controls",
                        items: [
                            {
                                name: "DiscAccNo", cls: "span3", text: "Disc Acc No", maxlength: "100", type: "popup", btnName: "btnDiscAccNo", click: "btnDiscAccNo()"
                            },
                            {
                                name: "DescriptionDiscount", cls: "span5", text: "Description Discount", maxlength: "100", readonly: true
                            },

                        ]
                    },
                    {
                        text: "Acc No. Return",
                        type: "controls",
                        items: [
                            {
                                name: "ReturnAccNo", cls: "span3", text: "Return Acc No", maxlength: "100", type: "popup", btnName: "btnReturnAccNo", click: "btnReturnAccNo()"
                            },
                            {
                                name: "DescriptionReturn", cls: "span5", text: "Description Return", maxlength: "100", readonly: true
                            },

                        ]
                    },
                    {
                        text: "Acc No. HPP",
                        type: "controls",
                        items: [
                            {
                                name: "COGSAccNo", cls: "span3", text: "COGS Acc No", maxlength: "100", type: "popup", btnName: "btnCOGSAccNo", click: "btnCOGSAccNo()"
                            },
                            {
                                name: "DescriptionCOGS", cls: "span5", text: "Description COGS", maxlength: "100", readonly: true
                            },

                        ]
                    },
                    {
                        text: "Acc No. Payback",
                        type: "controls",
                        items: [
                            {
                                name: "ReturnPybAccNo", cls: "span3", text: "Return Pyb Acc No", maxlength: "100", type: "popup", btnName: "btnReturnPybAccNo", click: "btnReturnPybAccNo()"
                            },
                            {
                                name: "DescriptionPyb", cls: "span5", text: "Description Pyb", maxlength: "100", readonly: true
                            },

                        ]
                    },
                    {
                        name: "TypeOfGoods",
                        cls:"hide"
                    }  
                ]
            },
            
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("KSGController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Account",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnCreate", text: "New", icon: "icon-file" },
//            //{ name: "btnBrowse", text: "Browse", icon: "icon-search" },
//            { name: "btnSave", text: "Save", icon: "icon-save" },
//            //{ name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
//            //{ name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
//        ],
//        panels: [
//            {
//                name: "pnlRefService",
//                //title: "Service Information",
//                items: [
//                    {
//                        text: "Acc No. Sales",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "SalesAccNo",
//                                cls: "span3",
//                                text: "Sales Acc No.",
//                                maxlength: "100",
//                                type: "popup",
//                                btnName: "btnSalesAccNo",
//                                readonly: true
//                            },
//                            {

//                                name: "DescriptionSales",
//                                cls: "span5",
//                                text: "Description Sales",
//                                maxlength: "100",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        text: "Acc No. Discount",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "DiscAccNo",
//                                cls: "span3",
//                                text: "Disc Acc No",
//                                maxlength: "100",
//                                type: "popup",
//                                btnName: "btnDiscAccNo",
//                                readonly: true
//                            },
//                            {

//                                name: "DescriptionDiscount",
//                                cls: "span5",
//                                text: "Description Discount",
//                                maxlength: "100",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        text: "Acc No. Return",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "ReturnAccNo",
//                                cls: "span3",
//                                text: "Return Acc No",
//                                maxlength: "100",
//                                type: "popup",
//                                btnName: "btnReturnAccNo",
//                                readonly: true
//                            },
//                            {

//                                name: "DescriptionReturn",
//                                cls: "span5",
//                                text: "Description Return",
//                                maxlength: "100",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        text: "Acc No. HPP",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "COGSAccNo",
//                                cls: "span3",
//                                text: "COGS Acc No",
//                                maxlength: "100",
//                                type: "popup",
//                                btnName: "btnCOGSAccNo",
//                                readonly: true
//                            },
//                            {

//                                name: "DescriptionCOGS",
//                                cls: "span5",
//                                text: "Description COGS",
//                                maxlength: "100",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        text: "Acc No. Payback",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "ReturnPybAccNo",
//                                cls: "span3",
//                                text: "Return Pyb Acc No",
//                                maxlength: "100",
//                                type: "popup",
//                                btnName: "btnReturnPybAccNo",
//                                readonly: true
//                            },
//                            {

//                                name: "DescriptionPyb",
//                                cls: "span5",
//                                text: "Description Pyb",
//                                maxlength: "100",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        name: "TypeOfGoods",
//                        cls:"hide"
//                    }
                    

                   
//                ]
//            },
            
//        ],
//    }

//    var widget = new SimDms.Widget(options);
//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/svaccount/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });

    
//    $("#btnSalesAccNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "sales",
//            title: "Account No Sales",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnDiscAccNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "discount",
//            title: "Account No Sales",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnReturnAccNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "return",
//            title: "Account No Sales",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnCOGSAccNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "hpp",
//            title: "Account No Sales",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnReturnPybAccNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "pyb",
//            title: "Account No Sales",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "sales":
//                $("#DescriptionSales").val(data["AccDescription"]);
//                $("#SalesAccNo").val(data["AccountNo"]);
//                //widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                $("#btnSave").removeClass("hide");
//                break;
//            case "discount":
//                $("#DescriptionDiscount").val(data["AccDescription"]);
//                $("#DiscAccNo").val(data["AccountNo"]);
//                //widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                $("#btnSave").removeClass("hide");
//                break;
//            case "return":
//                $("#DescriptionReturn").val(data["AccDescription"]);
//                $("#ReturnAccNo").val(data["AccountNo"]);
//                //widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                $("#btnSave").removeClass("hide");
//                break;
//            case "hpp":
//                $("#DescriptionCOGS").val(data["AccDescription"]);
//                $("#COGSAccNo").val(data["AccountNo"]);
//                //widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                $("#btnSave").removeClass("hide");
//                break;
//            case "pyb":
//                $("#DescriptionPyb").val(data["AccDescription"]); 
//                $("#ReturnPybAccNo").val(data["AccountNo"]);
//                //widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                $("#btnSave").removeClass("hide");
//                break;
//            default:
//                break;
//        }
//    });
 
      
//    $("#btnSave").on("click", saveData);
//    function saveData(p) {
        
//        var param = $(".main .gl-widget").serializeObject();
        
//            widget.post("sv.api/svaccount/save", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data updated...");
//                    //clear("new");
//                }
//            });
        
//    }

//    $('#btnCreate').on('click', function (e) {
//        clear("new");
//    });

//    $('#btnEdit').on('click', function (e) {
//        clear("btnEdit");

//    });


//    function clear(p) {
//        if (p == "clear") {
//            $("#btnSave").addClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//        } else if (p == "dbclick") {
//            $("#StallCode").attr("readonly", "readonly");
//            $("#Description").attr("readonly", "readonly");
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');
//            $("#btnSave").addClass("hide");
//        } else if (p == "new") {
//            clearData();
//            $("#StallCode").removeAttr('readonly');
//            $("#btnSave").removeClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            $("#Description").removeAttr('readonly');
//        } else if (p == "btnEdit") {
//            $("#StallCode").removeAttr('readonly');
//            $("#Description").removeAttr('readonly');
//            $("#btnSave").removeClass('hide');
//        } 
//    }
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/svaccount/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }

//});