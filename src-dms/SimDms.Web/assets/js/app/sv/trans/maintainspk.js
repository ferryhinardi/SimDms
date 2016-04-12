$(document).ready(function () {
    var options = {
        title: "Maintenance SPK",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },

        ],
        panels: [
           {
               name: "pnlMaintenanceSPK",
               title: "Maintenance SPK",
               items: [
                   { name: "ServiceNo", cls: "hide", readonly: true },
                   { name: "JobOrderNo", text: "SPK No", placeHolder: "XXX/YY/99999", cls: "span4", readonly: true },
                   { name: "JobOrderDate", text: "SPK Date", cls: "span4", type: "date", readonly: true },
                   { name: "PoliceRegNo", text: "Police Reg No", cls: "span4", readonly: true },
                   { name: "ServiceBookNo", text: "Service Book No", cls: "span4", readonly: true },
                   { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                   { name: "TransmissionType", text: "Trans Type", cls: "span4", readonly: true },
                   { name: "ChassisCode", text: "Chassis Code", cls: "span4", readonly: true },
                   { name: "ChassisNo", text: "Chassis No", cls: "span4", readonly: true },
                   { name: "EngineCode", text: "Engine Code", cls: "span4", readonly: true },
                   { name: "EngineNo", text: "Engine No", cls: "span4", readonly: true },
                   { name: "ColorCode", text: "Color", cls: "span4", readonly: true },
                   { name: "Odometer", text: "Odometer", cls: "span4 number", readonly: true },
                   {
                       text: "Customer",
                       type: "controls",
                       items: [
                           { name: "CustomerCode", cls: "span2", placeHolder: "Cust Code", readonly: true },
                           { name: "CustomerName", cls: "span6", placeHolder: "Cust Name", readonly: true },
                       ]
                   },
                   {
                       text: "Customer Bill",
                       type: "controls",
                       items: [
                           { name: "CustomerCodeBill", cls: "span2", placeHolder: "Cust Code Bill", readonly: true },
                           { name: "CustomerNameBill", cls: "span6", placeHolder: "Cust Name Bill", readonly: true },
                       ]
                   },
                   {
                       text: "Front / Fore Man",
                       type: "controls",
                       items: [
                           { name: "ForemanID", cls: "span2", placeHolder: "Foreman ID", readonly: true },
                           { name: "ForemanName", cls: "span6", placeHolder: "Foreman Name", readonly: true },
                       ]
                   },
                   { name: "JobType", text:"Job Type", cls: "span8", readonly: true },
                   {
                       name: "ServiceStatus", text: "Service Status", type: "select", cls: "span4",
                       items: [
                              { value: '0', text: 'BUKA SPK' },
                              { value: '1', text: 'ALOKASI PEKERJAAN' },
                              { value: '2', text: 'PROSES PENGERJAAN' },
                              { value: '3', text: 'PEMERIKSAAN AKHIR' },
                              { value: '4', text: 'PENCUCIAN KENDARAAN' },
                              { value: '5', text: 'PROSES ADMINISTRASI' }
                       ]
                   },
               ]
           },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("sv.api/maintainspk/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $('#btnBrowse').on('click', function () {
        widget.lookup.init({
            name: 'Maintainance SPK',
            source: 'sv.api/grid/MaintainSPK',
            sortings: [[0, "desc"]],
            columns: [
                { mData: "JobOrderNo", sTitle: "JobOrder No", sWidth: "110px" },
                {
                    mData: "JobOrderDate", sTitle: "JobOrder Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY - HH:mm');
                    }
                },
                { mData: "PoliceRegNo", sTitle: "Police Reg" },
                { mData: "BasicModel", sTitle: "Model" },
                { mData: "JobType", sTitle: "Job Type"},
                { mData: "ForemanName", sTitle: "Foreman" },
                { mData: "ServiceName", sTitle: "Status", sWidth: "140px" },
            ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate($.extend({}, widget.default, data));
        if (data.ServiceStatus == '5') {
            $('#ServiceStatus').attr('disabled', 'disabled');
            $('#btnSave').attr('disabled', 'disabled');
            $("#ServiceStatus option[value='5']").show();
        }
        else {
            $('#ServiceStatus').removeAttr('disabled');
            $('#btnSave').removeAttr('disabled');
            $("#ServiceStatus option[value='']").hide();
            $("#ServiceStatus option[value='5']").hide();
        }
        widget.lookup.hide();
    });

    $('#btnSave').on('click', function (e) {
        var dataMain = $(".main form").serializeObject();

        var data = {
            ServiceNo: dataMain.ServiceNo,
            ServiceStatus: dataMain.ServiceStatus
        };

        MsgConfirm("Anda yakin akan mengubah data ini?", function (result) {
            if (result) {
                widget.post("sv.api/MaintainSPK/Save", data, function (result) {
                    if (result.success) {
                        widget.showNotification(result.message);
                    }
                    else {
                        widget.showNotification(result.message);
                    }
                });
            }
        });
    });
    $('#btnSave').attr('disabled', 'disabled');
});