function PrintClubController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = "1";
        me.data.ChassisNoFrom = 0;
        me.data.ChassisNoTo = 999999999;
    }

    me.ClubCode = function () {
        var lookup = Wx.blookup({
            name: "ClubOpen",
            title: "Master Klub Browse",
            manager: MasterService,
            query: "ClubOpen",
            defaultSort: "ClubCodeStr asc",
            columns: [
                { field: "ClubCodeStr", Title: "Club Code", Width: "110px" },
                { field: "Description", Title: "Description", Width: "80px" },
                { field: "IsActiveStr", Title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.ClubCode = result.ClubCodeStr;
            }
        });

    }

    me.Print = function () {
        var clubCode = me.data.ClubCode == undefined ? "%" : me.data.ClubCode;
        var desc = me.data.Description == undefined ? "%" : me.data.Description;
        var IsActive = me.data.IsActive;
        var policeNo = me.data.PoliceRegNo == undefined ? "%" : me.data.PoliceRegNo;
        var clubNo = me.data.ClubNo == undefined ? "%" : me.data.ClubNo;
        var custName = me.data.CustomerName == undefined ? "%" : me.data.CustomerName;
        var chassisCode = me.data.ChassisCode == undefined ? "%" : me.data.ChassisCode;
        var ChassisNoFrom = me.data.ChassisNoFrom;
        var ChassisNoTo = me.data.ChassisNoTo;
        var servBookNo = me.data.ServiceBookNo == undefined ? "%" : me.data.ServiceBookNo;

        var ReportId = 'SvRpMst011';
        var par = [
           'companycode', clubCode,
            desc, IsActive, policeNo,
            clubNo, custName, chassisCode,
            ChassisNoFrom, ChassisNoTo, servBookNo,
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
        title: "Master Club",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "ProductType", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    { name: "ClubCode", cls: "span4 full", text: "Kode Klub", type: "popup", click: "ClubCode()" },
                    { name: "Description", text: "Description", placeholder: "Keterangan", cls: "span8", type: "textarea" },
                    {
                        type: "optionbuttons", name: "IsActive", model: "data.IsActive", text: "Status",
                        items: [
                            { name: "0", text: "Tidak Aktif" },
                            { name: "1", text: "Aktif" },
                        ]
                    },
                    { name: "PoliceRegNo", text: "No. Polisi", cls: "span8" },
                    { name: "ClubNo", text: "No. Klub", cls: "span8" },
                    { name: "CustomerName", text: "Nama Pelanggan", cls: "span8" },
                    { name: "ChassisCode", text: "Kode Rangka", cls: "span8" },
                    { name: "ChassisNoFrom", text: "No Rangka", cls: "span4 number" },
                    { name: "ChassisNoTo", text: "S/D", cls: "span4 number" },
                    { name: "ServiceBookNo", text: "No Buku Service", cls: "span8" },
                    { type: "hr" },
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
        SimDms.Angular("PrintClubController");
    }

});