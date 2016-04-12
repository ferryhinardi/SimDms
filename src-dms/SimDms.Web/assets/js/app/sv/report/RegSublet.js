

"use strict"

function svRegSubletController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    //me.init();




    me.initialize = function () {        
        $http.post('sv.api/Sublet/Default').
        success(function (data, status, headers, config) {
            me.data.DateFrom = data.Year + "/" + data.Month + "/1";
            me.data.DateTo = moment(data.Now);
        });
    }


    me.exportXls = function () {
       
        var url = "sv.api/Sublet/GenerateSubletXls?";


        var params = "&StartDate=" +moment(me.data.DateFrom).format("YYYY-MM-DD");
        params += "&EndDate=" + moment(me.data.DateTo).format("YYYY-MM-DD");
        

        url = url + params;
        window.open ( url, '_blank');

        //console.log(url);
    }


    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Sublet",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Generate", cls: "btn btn-primary", icon: "fa fa-file-excel-o", click: "exportXls()" },
        ],
        panels: [
            {
                title: "",
                name: "pnlSublet",
                items: [
                    { name: "DateFrom", text: "Periode", cls: "span4", type: "ng-datepicker", required: true, model: "data.DateFrom" },
                    { name: "DateTo", text: "s/d", cls: "span4", type: "ng-datepicker", required: true, model: "data.DateTo" },
                    //{ text: "", name: "SupplierName", model: "data.SupplierName", display: "SupplierName", cls: "span4" },
                    //{ name: "Status", model: "data.Status", required: true, cls: "span4 full", text: "Status", type: "select2", datasource: 'dsReffType' },
                ],
            },
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svRegSubletController");
    }
});
