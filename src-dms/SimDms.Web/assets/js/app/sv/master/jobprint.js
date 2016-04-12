"use strict"

function svMstJobController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BasicModel = function () {
        var lookup = Wx.klookup({
            name: "BasicModel",
            title: "Basic Model",
            url: "sv.api/pekerjaan/LookupBasicModelPrint",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "BasicModel", title: "Basic Model", width: 100 },
                { field: "TechnicalModelCode", title: "Technical Model" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BasicModel = data.BasicModel;
            me.Apply();
        });
    }

    me.JobType = function () {
        var lookup = Wx.klookup({
            name: "JobType",    
            title: "Job Type",
            url: "sv.api/pekerjaan/LookupReffSrvPrint?reffType=JOBSTYPE",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "RefferenceCode", title: "Pekerjaan", width: 100 },
                { field: "Description", title: "Keterangan" },
                { field: "IsActive", title: "Status", width: 100 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.JobType = data.RefferenceCode;
            me.Apply();
        });
    }

    me.GroupJobType = function () {
        var lookup = Wx.klookup({
            name: "GroupJobType",
            title: "Group Job Type",
            url: "sv.api/pekerjaan/LookupReffSrvPrint?reffType=GRPJOBTY",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "RefferenceCode", title: "Pekerjaan", width: 100 },
                { field: "Description", title: "Keterangan" },
                { field: "IsActive", title: "Status", width: 100 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.GroupJobType = data.RefferenceCode;
            me.Apply();
        });
    }

    me.OperationNo = function () {
        var lookup = Wx.klookup({
            name: "OperationNo",
            title: "Jenis Pekerjaan",
            url: "sv.api/pekerjaan/LookupOperationNoPrint",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "OperationNo", title: "Jenis Pekerjaan", width:100 },
                { field: "Description", title: "Keterangan", width:200 },
                { field: "TechnicalModelCode", title: "Technical Model", width:100 },
                { field: "IsSubCon", title: "Sub-Con?", width:100 },
                { field: "IsCampaign", title: "Campaign?", width:100 },
                { field: "IsActive", title: "Status", width:100 },
                { field: "OperationHour", title: "Lama Pengerjaan - Pelanggan", width: 200, template: '<div style="text-align:right;">#= kendo.toString(OperationHour, "n2") #</div>' },
                { field: "ClaimHour", title: "Lama Pengerjaan - Claim", width: 200, template: '<div style="text-align:right;">#= kendo.toString(ClaimHour, "n2") #</div>' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.OperationNo = data.OperationNo;
            me.Apply();
        });
    }

    $('#GroupJobType').on('blur', function (e) {
        if (me.data.GroupJobType == null || me.data.GroupJobType == '') return;
        $http.post('sv.api/pekerjaan/GroupJobType?GroupJobType=' + me.data.GroupJobType).
        success(function (data) {
            if (data.success) {
                me.data.GroupJobType = data.data.GroupJobType;
                me.data.GroupJobDescription = data.data.GroupJobDescription;
            }
            else {
                me.data.GroupJobType = me.data.GroupJobDescription = '';
                me.GroupJobType();
            }
        });
    });

    $('#BasicModel').on('blur', function () {
        if (me.data.BasicModel == "" || me.data.BasicModel == null) return;
        $http.post('sv.api/pekerjaan/GetBasicModel', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.BasicModel = data.data.RefferenceCode;
            } else {
                me.data.BasicModel = "";
                me.BasicModel();
            }
        });
    });

    $('#JobType').on('blur', function (e) {
        if (me.data.JobType == null || me.data.JobType == '') return;
        $http.post('sv.api/pekerjaan/JobType?JobType=' + me.data.JobType).
        success(function (data) {
            console.log(data.success);
            if (data.success) {
                me.data.JobType = data.data.RefferenceCode;
                me.data.JobDescription = data.data.Description;
            }
            else {
                me.data.JobType = me.data.JobDescription = '';
                me.JobType();
            }
        });
    });

    $('#GroupJobType').on('blur', function (e) {
        if (me.data.GroupJobType == null || me.data.GroupJobType == '') return;
        $http.post('sv.api/pekerjaan/GroupJobType?GroupJobType=' + me.data.GroupJobType).
        success(function (data) {
            if (data.success) {
                me.data.GroupJobType = data.data.GroupJobType;
                me.data.GroupJobDescription = data.data.GroupJobDescription;
            }
            else {
                me.data.GroupJobType = me.data.GroupJobDescription = '';
                me.GroupJobType();
            }
        });
    });

    $('#OperationNo').on('blur', function (e) {
        if (me.data.OperationNo == null || me.data.OperationNo == '') return;
        $http.post('sv.api/pekerjaan/GetOperationNo?operationNo=' + me.data.OperationNo).
        success(function (data) {
            if (data.success) {
                me.data.OperationNo = data.data.OperationNo;
            }
            else {
                me.data.OperationNo = me.data.OperationNo = '';
                me.OperationNo();
            }
        });
    });

    me.PartNo = function () {
        var lookup = Wx.klookup({
            name: "PartNo",
            title: "Jenis Item",
            url: "sv.api/pekerjaan/LookupPartNoPrint",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field:"PartNo",  title:"No. Part", width:100 },
                { field:"PartName",  title:"Nama Part", width:200 },
                { field: "RetailPriceInclTax", title: "Harga Jual + Tax", width: 150 },
                { field:"Status",  title:"Status", width:100 }
               ]
        });
        lookup.dblClick(function (data) {
            me.data.PartNo = data.PartNo;
            me.Apply();
        });
    }

    me.print = function () {
        var isActive = "", subCon = "", campaign = "", basicModel = "%", jobType = "%", groupJobType = "%", operationNo = "%", partNo = "%";
        
        if (me.data.BasicModel == null || me.data.BasicModel == '') basicModel = '%';
        else basicModel = me.data.BasicModel;
        if (me.data.JobType == null || me.data.JobType == '') jobType = '%';
        else jobType = me.data.JobType;
        if (me.data.GroupJobType == null || me.data.GroupJobType == '') groupJobType = '%';
        else groupJobType = me.data.GroupJobType;
        if (me.data.OperationNo == null || me.data.OperationNo == '') operationNo = '%';
        else operationNo = me.data.OperationNo;
        if (me.data.PartNo == null || me.data.PartNo == '') partNo = "%"
        else partNo = me.data.PartNo;
        if ($('#IsActive').is(':checked')) status = '1';
        else  status = "0";
        if ($('#SubCon').is(':checked'))  subCon = '1';
        else  subCon = "0";
        if ($('#Campaign').is(':checked'))  campaign = '1';
        else  campaign = "0";

        var prm = [basicModel, jobType, groupJobType, status, operationNo, subCon, campaign, partNo];

        Wx.showPdfReport({
            id: 'SvRpMst014',
            pparam: prm.join(','),
            rparam: 'Master Pekerjaan',
            type: 'devex'
        });
    }

    me.initialize = function () {
        me.data = {};
        $('#IsActive').attr('checked', true);
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Master Pekerjaan",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "BasicModel", text: "Basic Model", model: "data.BasicModel", type: "popup", btnName: "btnBasicModel", click: "BasicModel()", cls: "span4" },
                    { name: "JobType", text: "Pekerjaan", model: "data.JobType", type: "popup", btnName: "btnJobType", click: "JobType()", cls: "span4" },
                    { name: "GroupJobType", text: "Group Pekerjaan", model: "data.GroupJobType", type: "popup", btnName: "btnGroupJobType", click: "GroupJobType()", cls: "span4" },
                    { name: "OperationNo", text: "Jenis Pekerjaan", model: "data.OperationNo", type: "popup", btnName: "btnOperationNo", click: "OperationNo()", cls: "span4" },
                    { name: "PartNo", text: "No Part", model: "data.PartNo", type: "popup", btnName: "btnPartNo", click: "PartNo()", cls: "span4" },
                    { name: "IsActive", text: "Status", type: "check", cls: "span1", model: "data.IsActive" },
                    { name: "SubCon", text: "Sub Con ?", type: "check", cls: "span1", model: "data.SubCon" },
                    { name: "Campaign", text: "Campaign ?", type: "check", cls: "span1", model: "data.Campaign" },
                    { type: "buttons", items: [{ name: "btnPrint", text: "Print", icon: "icon-print", cls: "span4", click: 'print()' }] }
                ]
            },
        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstJobController");
    }
});