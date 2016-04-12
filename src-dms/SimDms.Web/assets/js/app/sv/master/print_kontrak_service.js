function KSGController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.hasChanged = false;
        me.data.Status = "1";

    }

    me.ContractNo = function () {
        var lookup = Wx.blookup({
            name: "KontrakServiceOpen",
            title: "Master Kontrak Browse",
            manager: MasterService,
            query: "KontrakServiceOpen",
            defaultSort: "ContractNo asc",
            columns: [
                { field: "ContractNo", Title: "Contract No", Width: "110px" },
                { field: "ContractDate", Title: "Contract Date", Width: "130px" },
                { field: "CustomerCode", Title: "Customer Code", Width: "110px" },
                { field: "CustomerName", Title: "Customer Name", Width: "110px" }
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.ContractNo = result.ContractNo;
                me.Apply();
            }
        });
    }

    me.Print = function () {
        var contractNo = me.data.ContractNo == undefined ? "%" : me.data.ContractNo;
        var reffNo = me.data.RefferenceNo == undefined ? "%" : me.data.RefferenceNo;
        var desc = me.data.Description == undefined ? "%" : me.data.Description;
        var customerName = me.data.CustomerName == undefined ? "%" : me.data.CustomerName;
        var Status = me.data.Status;
        var policeNo = me.data.PoliceNo == undefined ? "%" : me.data.PoliceNo;
        var chassisCode = me.data.ChassisCode == undefined ? "%" : me.data.ChassisCode;
        var chassisNo = me.data.ChassisNo == undefined ? "%" : me.data.ChassisNo;
        var serviceBookNo = me.data.ServiceBookNo == undefined ? "%" : me.data.ServiceBookNo;

        var ReportId = 'SvRpMst010';
        var par = [
         'companycode',
         contractNo,
         reffNo,
         desc,
         customerName,
         Status,
         policeNo,
         chassisCode,
         chassisNo,
         serviceBookNo
        ]
        var rparam = '';

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
        title: "Master Kontrak",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "ProductType", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    { name: "ContractNo", cls: "span4 full", text: "No. Kontrak", type: "popup", readonly: true, btnName: "btnContractNo",click: "ContractNo()" },
                    { name: "RefferenceNo", text: "No. Refferensi", cls: "span4 full" },
                    { name: "Description", text: "Keterangan", cls: "span4 full" },
                    { name: "CustomerName", text: "Nama Pelanggan", cls: "span4 full" },
                    {
                        type: "optionbuttons", name: "Status", model: "data.Status", text: "Status",
                        items: [
                            { name: "1", text: "Aktif" },
                            { name: "0", text: "Tidak Aktif" },
                        ]
                    },
                    { name: "PoliceRegNo", text: "No. Polisi", cls: "span4 full" },
                    { name: "ChassisCode", text: "Kode Rangka", cls: "span4 full" },
                    { name: "ChassisNo", text: "No Rangka", cls: "span4 full" },
                    { name: "ServiceBookNo", text: "No Buku Service", cls: "span4 full" },
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