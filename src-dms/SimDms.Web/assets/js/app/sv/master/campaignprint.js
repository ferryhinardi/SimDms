"use strict"

function svMstCampaignPrintController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $('#ComplainCode').on('blur', function () {
        if (me.data.ComplainCode == "" || me.data.ComplainCode == null) return;
        $http.post('sv.api/campaign/GetComplCode', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.ComplainCode = data.data.ComplainCode;
            } else {
                me.data.ComplainCode = "";
                me.ComplainCode();
            }
        });
    });

    me.ComplainCode = function () {
        var lookup = Wx.klookup({
            name: "ComplainCode",
            title: "Kode Complain",
            url: "sv.api/campaign/ComplainCode",
            serverBinding: true,
            pageSize: 10,
            columns:
                [{ field: "ComplainCode", title: "Kode", width: 110 },
                { field: "DescriptionComplain", title: "Keterangan", width: 110 },
                { field: "DescriptionEngComplain", title: "Keterangan (Eng.)", width: 80 }]
        });
        lookup.dblClick(function (data) {
            me.data.ComplainCode = data.ComplainCode;
            me.Apply();
        });
    }

    $('#DefectCode').on('blur', function () {
        if (me.data.DefectCode == "" || me.data.DefectCode == null) return;
        $http.post('sv.api/campaign/GetDefectCode', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.DefectCode = data.data.DefectCode;
            } else {
                me.data.DefectCode = "";
                me.DefectCode();
            }
        });
    });

    me.DefectCode = function () {
        var lookup = Wx.klookup({
            name: "DefectCode",
            title: "Kode Defect",
            url: "sv.api/campaign/DefectCode",
            serverBinding: true,
            pageSize: 10,
            columns:
                [{ field: "DefectCode", title: "Kode", width: 110 },
                { field: "DescriptionComplain", title: "Keterangan", width: 110 },
                { field: "DescriptionEngComplain", title: "Keterangan (Eng.)", width: 80 }]
        });
        lookup.dblClick(function (data) {
            me.data.DefectCode = data.DefectCode;
            me.Apply();
        });
    }

    $('#ChassisCode').on('blur', function () {
        if (me.data.ChassisCode == "" || me.data.ChassisCode == null) return;
        $http.post('sv.api/campaign/GetChassisCode', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.ChassisCode = data.data.ChassisCode;
            } else {
                me.data.ChassisCode = "";
                me.ChassisCode();
            }
        });
    });

    me.ChassisCode = function () {
        var lookup = Wx.klookup({
            name: "ChassisCode",
            title: "Kode Rangka",
            url: "sv.api/campaign/ChassisCode",
            serverBinding: true,
            pageSize: 10,
            columns:
                [{ field: "ChassisCode", title: "Kode Rangka", width: 110 },
                { field: "ChassisStartNo", title: "No. Rangka Awal", width: 110 },
                { field: "ChassisEndNo", title: "No. Rangka Akhir", width: 80 }]
        });
        lookup.dblClick(function (data) {
            me.data.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.OperationNo = function () {
        var lookup = Wx.klookup({
            name: "OperationNo",
            title: "Jenis Pekerjaan",
            url: "sv.api/campaign/OperationNo",
            serverBinding: true,
            pageSize: 10,
            columns:
                [{ field: "OperationNo", title: "Jenis Pekerjaan", width: 100 },
                { field: "Description", title: "Keterangan" },
                { field: "IsActive", title: "Status", width: 100 }]
        });
        lookup.dblClick(function (data) {
            me.data.OperationNo = data.OperationNo;
            me.Apply();
        });
    }

    $('#OperationNo').on('blur', function () {
        if (me.data.OperationNo == "" || me.data.OperationNo == null) return;
        $http.post('sv.api/campaign/get', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.OperationNo = data.data.OperationNo;
            } else {
                me.data.ChassisCode = "";
                me.ChassisCode();
            }
        });
    });

    me.print = function () {
        var complainCode = "%", defectCode = "%", chassisCode = "%", operationNo = "%", desc = "%";

        if (me.data.ComplainCode == null || me.data.ComplainCode == '') complainCode = '%';
        else complainCode = me.data.ComplainCode;
        if (me.data.DefectCode == null || me.data.DefectCode == '') defectCode = '%';
        else defectCode = me.data.DefectCode;
        if (me.data.ChassisCode == null || me.data.ChassisCode == '') chassisCode = "%"
        else chassisCode = me.data.ChassisCode;
        if (me.data.OperationNo == null || me.data.OperationNo == '') operationNo = "%"
        else operationNo = me.data.OperationNo;
        if (me.data.Description == null || me.data.Description == '') desc = "%"
        else desc = me.data.Description;

        var prm = [
          'companycode',
           complainCode,
           defectCode,
           chassisCode,
           operationNo,
           desc,
           me.data.IsActive
        ];

        Wx.showPdfReport({
            id: 'SvRpMst007',
            pparam: prm.join(','),
            rparam: 'Master Campaign',
            type: 'devex'
        });
    }

    me.initialize = function () {
        $('#IsActive').attr('checked', true);
        me.data.IsActive = true;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Master Campaign",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "ComplainCode", text: "Kode Complain", type: "popup", cls: "span4", click: "ComplainCode()" },
                    { name: "DefectCode", text: "Kode Defect", type: "popup", cls: "span4", click: "DefectCode()" },
                    { name: "ChassisCode", text: "Kode Rangka", type: "popup", cls: "span4", click: "ChassisCode()" },
                    { name: "OperationNo", text: "Jenis Pekerjaan", type: "popup", cls: "span4", click: "OperationNo()" },
                    { name: "Description", text: "Keterangan", cls: "span4" },
                    { name: "IsActive", text: "Status", cls: "span4", type: "x-switch", float: "left" },
                    { type: "buttons", items: [{ name: "btnPrint", text: "Print", icon: "icon-print", cls: "span4", click: 'print()' }] }
                ]
            },

        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstCampaignPrintController");
    }

});