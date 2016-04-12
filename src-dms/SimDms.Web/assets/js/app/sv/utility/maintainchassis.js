function maintainChassis($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.initialize = function () {
        me.clearTable(me.grid1);
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ChassisLookup",
            title: "Chassis Lookup",
            manager: SvUtilityManager,
            query: "MaintainChassisLookup",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "ServiceBookNo", title: "No Buku Service" },
                { field: "PoliceRegNo", title: "No Polisi" }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.Apply();

                var src = "sv.api/maintainchassis/GetMaintainChassisLinkData";
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        chassisCode: me.data.ChassisCode,
                        chassisNo: me.data.ChassisNo
                    },
                    url: src,
                    success: function (data) {
                        me.loadTableData(me.grid1, data);
                    }
                });
                $('#wxlinkdatastatus').show();
            }
        });
    }

    me.cancelOrClose = function () {
        me.init();
        $('#wxlinkdatastatus').hide();
    }

    me.grid1 = new webix.ui({
        container: "wxlinkdatastatus",
        view: "wxtable", css:"alternating",
        autoWidth: false,
        width: 400,
        columns: [
            { id: "SeqNo", header: "No", width: 40 },
            { id: "LinkData", header: "Link Data", width: 180 },
            { id: "StatusDesc", header: "Status", width: 180 },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.saveData = function (e, param) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                chassisCode: me.data.ChassisCode,
                chassisNo: me.data.ChassisNo,
                chassisCodeNew: me.data.ChassisCodeNew,
                chassisNoNew: me.data.ChassisNoNew
            },
            url: "sv.api/maintainchassis/MaintainChassisSave",
            success: function (data) {
                if (data.message == "") {
                    $('#ChassisCode').val(me.data.ChassisCodeNew);
                    $('#ChassisCodeNew').val("");
                    $('#ChassisNo').val(me.data.ChassisNoNew);
                    $('#ChassisNoNew').val("");
                }
            }
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Maintain Chassis",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                title: 'Details',
                items: [
                    {
                        text: "Kode & No Chassis",
                        type: "controls",
                        items: [
                            { name: "ChassisCode", placeHolder: "Chassis Code", cls: "span4", readonly: true },
                            { name: "ChassisNo", placeHolder: "Chassis No", cls: "span4", readonly: true }
                        ]
                    },
                    {
                        text: "→",
                        type: "controls",
                        items: [
                            { name: "ChassisCodeNew", placeHolder: "New Chassis Code", cls: "span4", validasi: "required" },
                            { name: "ChassisNoNew", placeHolder: "New Chassis No", cls: "span4", validasi: "required" }
                        ]
                    },
                    {
                        text: "Kode & No Mesin",
                        type: "controls",
                        items: [
                            { name: "EngineCode", placeHolder: "Engine Code", cls: "span4", readonly: true },
                            { name: "EngineNo", placeHolder: "Engine No", cls: "span4", readonly: true }
                        ]
                    },
                    { name: "ServiceBookNo", text: "No Buku Service", placeHolder: "Service Book No", cls: "span4", readonly: true },
                    { name: "PoliceRegNo", text: "No Polisi", placeHolder: "Police Reg No", cls: "span4", readonly: true },
                    {
                        name: "wxlinkdatastatus",
                        title: "Link Data Status",
                        type: "wxdiv",
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);
    $('#wxlinkdatastatus').hide();

    function init(s) {
        SimDms.Angular("maintainChassis");
    }
});
