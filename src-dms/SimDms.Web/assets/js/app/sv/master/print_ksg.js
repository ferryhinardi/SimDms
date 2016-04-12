function KSGController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/BasicModel').
    success(function (data, status, headers, config) {
        me.BasicModelList = data;
    });

    me.TransmissionType = [
       { "value": 'AT', "text": 'Automatic Transmission' },
       { "value": 'MT', "text": 'Manual Transmission' },
    ];

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = "1";
        $('#IsCampaign').prop('checked', false);
        me.data.IsCampaign = false;

    }

    $("[name = 'IsCampaign']").on('change', function () {
        me.data.IsCampaign = $('#IsCampaign').prop('checked');
        me.Apply();
    });

    me.Print = function () {
        var BasicModel = me.data.BasicModel == undefined ? "%" : me.data.BasicModel;
        var IsCampaign = me.data.IsCampaign == true ? "1" : "0";
        var TransmissionType = me.data.TransmissionType == undefined ? "%" : me.data.TransmissionType;
        var Description = me.data.Description == undefined ? "%" : me.data.Description;
        var IsActive = me.data.IsActive;

        var ReportId = 'SvRpMst008';
        var par = [
           'companycode', IsCampaign
           , BasicModel, TransmissionType
           , Description, IsActive
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
        title: "Pdi Fsc Rate",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "ProductType", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    { name: "BasicModel", cls: "span4 full", type: "select2", text: "Basic Model", datasource: "BasicModelList", opt_text: "-- SELECT ALL --" },
                    { name: "IsCampaign", text: "Tipe PDI/FSC", cls: "span5", type: "check", float: "left", model: "data.IsCampaign" },
                    { name: "TransmissionType", text: "Transmission Type", type: "select2", cls: "span4 full", opt_text: "-- SELECT ALL --", datasource: "TransmissionType", },
                    {
                        name: "Description", text: "Description", placeholder: "Keterangan", cls: "span4"
                    },
                    {
                        type: "optionbuttons", name: "IsActive", model: "data.IsActive", text: "Is Active",
                        items: [
                            { name: "1", text: "Aktif" },
                            { name: "0", text: "Tidak Aktif" },
                        ]
                    },
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