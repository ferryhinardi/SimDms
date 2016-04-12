var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spCompanyAccController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/combo/LoadLookup?CodeId=PFCN').
           success(function (data, status, headers, config) {
               me.comboPFCN = data;
           });
    

    me.Holiday = function () {
        localStorage.setItem('ProfitCenter', $('#ProfitCenterCode').select2("data").text);
        localStorage.setItem('ProfitCenterCode', me.data.ProfitCenterCode);
        ////localStorage.setItem('EmployeeName', me.data.EmployeeName);
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/addholiday" });
        //Wx.showFormsave({ url: "gn/master/addholiday" });
        var stop = setInterval(function () {
            var b = localStorage.getItem("RefreshGrid");
            console.log(b);
            if (b == "true") {
                me.loadDetail();
                clearInterval(stop);
            }
        }, 3000);
    }

    me.Holidays = function () {
        localStorage.setItem('ProfitCode', $('#ProfitCenterCode').select2("data").text);
        localStorage.setItem('ProfitCenterCode', me.data.ProfitCenterCode);
        //localStorage.setItem('EmployeeName', me.data.EmployeeName);
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/addholidays" });
        var stop = setInterval(function () {
            var b = localStorage.getItem("RefreshGrid");
            console.log(b);
            if (b == "true") {
                me.loadDetail();
                clearInterval(stop);
            }
        }, 3000);
    }

    
    me.loadDetail = function (data) {
        $http.post('gn.api/Calender/CalenderLoad?ProfitCenterCode=' + me.data.ProfitCenterCode).
           success(function (data, status, headers, config) {
               me.grid.data = data;
               me.loadTableData(me.grid1, me.grid.data);
               
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }


    
    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "CalendarDate", header: "Date", width: 300 },
                { id: "CalendarDescription", header: "Notes", width: 300 },
            ],

            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.data = this.getItem(me.grid1.getSelectedId().id);
                        me.data.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }


    me.LinkDetail = function () {
    }

    $("[name = 'ProfitCenterCode']").on('change', function () {
        me.data.ProfitCenterCode = $("[name='ProfitCenterCode']").val();
        me.loadDetail();
    });

    
    me.initialize = function () {
        me.clearTable(me.grid1);
        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
        me.data.ProfitCenterCode = "000";
        me.Apply();
        //alert($('#ProfitCenterCode').select2("data").text);
        //$('#CompanyCode').css("text-align", "right");
        $('#btnAddHolidays').prop("disabled", true); // Read me : because save to ... ? view calender not yet. (Pending)
        me.loadDetail();
        me.hasChanged = false;
        me.isSave = false;
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}


$(document).ready(function () {
    window.UserInfo = {};
    var options = {
        title: "Master Calender",
        xtype: "panels",
        toolbars: "",
        panels: [
            {
                name: "DataEmployeeClass",
                title: "",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", type: "text", text: "Company Code", cls: "span8", readonly : true },
                        { name: "BranchCode", model: "data.BranchCode", type: "text", text: "Branch Code", cls: "span8", readonly: true },
                        { name: "ProfitCenterCode", model: "data.ProfitCenterCode", type: "select2", text: "Profit Center", cls: "span4 full", datasource: "comboPFCN" },
                        //{ name: "Email", text: "Email", cls: "span4 ignore-uppercase", maxlength: 10 },
                        //{ name: "ResignDate", type: "ng-datepicker", text: "Resign Date", cls: "span4" }
                        {
                            type: "buttons",
                            cls: "span4",
                            items: [
                                    { name: "btnAddHoliday", text: "Add Holiday", icon: "icon-plus", cls: "btn btn-info", click: "Holiday()" },
                                    { name: "btnAddHolidays", text: "Add Holidays", icon: "icon-plus", cls: "btn btn-info", click: "Holidays()" }
                            ]
                        },              
                ]
            },
           
             {
                 name: "wxsalestarget",
                 xtype: "wxtable",
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spCompanyAccController");
    }

});
