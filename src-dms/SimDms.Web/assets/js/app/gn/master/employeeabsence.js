"use strict";

function gnMstEmpAbsController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYY-MM-DD");
    }
    
    me.initialize = function () {
        var today = new Date();
        //me.data.BeginDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        me.data.BeginDate = me.now();
        me.Apply();
        me.loadGrid();
        me.HideForm();
    }

    $("[name=BeginDate]").on("change", function () {
        me.data.BeginDate = getSqlDate($("[name=BeginDate]").val());
        $("a.k-icon.k-i-collapse").removeClass("collapse");
        $("a.k-icon.k-i-").addClass("expand");
    });

    me.loadGrid = function () {
        if (me.data.BeginDate == null) {
            var today = new Date();
            me.data.BeginDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        }
        me.data.BeginDate= getSqlDate($("[name=BeginDate]").val());
        var date = new Date(me.data.BeginDate.toString().replace(/-/g, "/"));
        var date1 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 1);
        var date2 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 2);
        var date3 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 3);
        var date4 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 4);
        var date5 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 5);
        var date6 = new Date(date.getFullYear(), date.getMonth(), date.getDate() - 6);
        //alert(me.data.BeginDate.toString().replace(/-/g, "/"));
        Wx.kgrid({
            url: "gn.api/EmployeeAbsence/EmployeeAbsenceList",
            name: "KGridMSIInfo",
            params: me.data,
            scrollable: true,
            pageSize: 100,
            serverBinding: true,
            dataBound: function () {
                var grid = $('#KGridMSIInfo').data('kendoGrid');
                grid.collapseGroup(grid.tbody.find(">tr.k-grouping-row"));
                $('tr[role*="row"]').hide();
            },

            group: [
                   { field: "TitleDesc" },
            ],
            columns: [
                { field: "EmployeeID", title: "NIP", width: 100 },
                {
                    field: "TitleDesc", title: "Bagian", fillspace: true
                    //,
                    //groupHeaderTemplate: "Group : #= TitleDesc #",
                },
                { field: "EmployeeName", title: 'Nama', width: 180 },
                { field: "D01", title: moment(date6).format('DD-MMM-YYYY'), width: 120 },
                { field: "D02", title: moment(date5).format('DD-MMM-YYYY'), width: 120 },
                { field: "D03", title: moment(date4).format('DD-MMM-YYYY'), width: 120 },
                { field: "D04", title: moment(date3).format('DD-MMM-YYYY'), width: 120 },
                { field: "D05", title: moment(date2).format('DD-MMM-YYYY'), width: 120 },
                { field: "D06", title: moment(date1).format('DD-MMM-YYYY'), width: 120 },
                { field: "D07", title: moment(date).format('DD-MMM-YYYY'), width: 120 },
            ],
        });

        //$("[name=BeginDate]").val(me.data.BeginDate.toString("DD-MMM-YYYY"));
        
    }

    me.HideForm = function () {
        $(".tbody > .panel").fadeOut();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Employee Absence",
        xtype: "panels",
        toolbars: [{ name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" }],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                          { name: "BeginDate", type: "ng-datepicker", text: "Select Date", cls: "span4" },
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnLoad", text: "Load Data", icon: "icon-search", cls: "btn btn-info", click: "loadGrid()" },
                              ]
                          },
                ]
            },
            {
                name: "KGridMSIInfo",
                title: "Divisi",
                xtype: "k-grid",
                cls : "expand"
            },
        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("gnMstEmpAbsController");
    }
});