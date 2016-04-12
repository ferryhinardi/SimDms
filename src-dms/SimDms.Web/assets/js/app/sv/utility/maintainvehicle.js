function maintainvehicle($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.comboVehicleType = [
        { value: 0, text: "Passenger Car" },
        { value: 1, text: "Commercial Vehicle" }
    ]
    
    me.cmbChangeVehicleType = function () {
        var vehicleType = $('#PartCategory').select2("val");
        if (vehicleType == "") return;
        $http.post('sv.api/maintainvehicle/GetVehicleTypeData', { vehicleType: vehicleType }).
        success(function (data, status, headers, config) {
            me.loadTableData(me.grid1, data);
            me.detail = data;
        });
    }

    me.grid1 = new webix.ui({
        container: "wxvehicledata",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 330,
        width: 400,
        autowidth: false,
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "BasicModel", header: "Basic Model", width: 180 },
            { id: "Description", header: "Keterangan", width: 180 }
        ]
    });

    me.btnChangeVehicleType = function () {
        me.isProcessing = true;
        var datDetail = [];
        $.each(me.detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                datDetail.push({ BasicModel: val["BasicModel"]});
            }
        })
        var dat = {};
        dat["model"] = datDetail;
        $http.post('sv.api/maintainvehicle/ChangeVehicleType', JSON.stringify(dat)).
            success(function (data, status, headers, config) {
                if (data.message == "") {
                    MsgBox("Convert Completed");
                    me.cmbChangeVehicleType();
                }
                else {
                    MsgBox(data.message);
                }
                me.isProcessing = false;
            }).error(function (data, status, headers, config) {
                MsgBox("Error");
                me.isProcessing = false;
            });
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.initialize = function () {
        me.detail = {};
        me.clearTable(me.grid1);
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Maintain Vehicle",
        xtype: "panels",
        panels: [
            {
                title: 'Details',
                items: [
                    { name: "PartCategory", text: "Part Category", cls: "span4", validasi: "required", type: "select2", datasource: "comboVehicleType", change: "cmbChangeVehicleType()" },
                    {
                        name: "wxvehicledata",
                        title: "Data",
                        type: "wxdiv"
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnChange", text: "Change Vehicle Type", cls: "btn btn-info", click: "btnChangeVehicleType()" }
                        ]                        
                    }                    
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("maintainvehicle");
    }
});