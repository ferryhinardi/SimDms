function KSGController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/event/BasicModel').
    success(function (data, status, headers, config) {
        me.BasicModelList = data;
    });

    $http.post('sv.api/event/JobType').
    success(function (data, status, headers, config) {
        me.JobTypeList = data;
    });

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = "1";
    }

    me.Print = function () {
        var EventNo = me.data.EventNo == undefined ? "%" : me.data.EventNo;
        var Description = me.data.Description == undefined ? "%" : me.data.Description;
        var BasicModel = me.data.BasicModel == undefined ? "%" : me.data.BasicModel;
        var JobType = me.data.JobType == undefined ? "%" : me.data.JobType;
        var IsActive = me.data.IsActive;

        var ReportId = 'SvRpMst012';
        var par = [
           'companycode', EventNo, Description
           , BasicModel, JobType
           , IsActive
        ]
        var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Master Event",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "ProductType", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    { name: "EventNo", text: "EventNo", placeholder: "EventNo", cls: "span4 full" },
                    { name: "Description", text: "Description", placeholder: "Keterangan", cls: "span4 full" },
                    { name: "BasicModel", cls: "span4 full", type: "select2", text: "Basic Model", datasource: "BasicModelList", opt_text: "-- SELECT ALL --" },
                    { name: "JobType", text: "JobT ype", type: "select2", cls: "span4 full", opt_text: "-- SELECT ALL --", datasource: "JobTypeList", },
                    {
                        type: "optionbuttons", name: "IsActive", model: "data.IsActive", text: "Is Active",
                        items: [
                            { name: "0", text: "Tidak Aktif" },
                            { name: "1", text: "Aktif" },
                            { name: "2", text: "Semua" },
                        ]
                    },
                    {type: "hr"},
                    {
                        type: "buttons",
                        items: [
                                { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-primary", click: "Print()" },
                        ]
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("KSGController");
    }

});