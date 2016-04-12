"use strict"

function svMstReffSrvPrintController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/ListLookupDtlByCodeID?CodeID=RFTP').
      success(function (data, status, headers, config) {
          me.ReffSrv = data;
      });

    $("#IsActive").on('change', function () {
        if ($('#IsActive').is(':checked')) {
            me.data.IsActive = true;
        }
        else {
            me.data.IsActive = false;
        }
        me.Apply();
    });

    me.print = function () {
        if (me.data.RefferenceType == "") me.data.RefferenceType = "%";
        if (me.data.RefferenceCode == "") me.data.RefferenceCode = "%";
        if (me.data.Description == "") me.data.Description = "%";
        if (me.data.DescriptionEng == "") me.data.DescriptionEng = "%";
        var prm = [
          'companycode',
           me.UserInfo.ProductType,
           me.data.RefferenceType,
           me.data.RefferenceCode,
           me.data.Description,
           me.data.DescriptionEng,
           me.data.IsActive
        ];
        
        Wx.showPdfReport({
            id: 'SvRpMst001',
            pparam: prm.join(','),
            rparam: 'Master Referensi Service',
            type: 'devex'
        });
    }

    me.initialize = function () {
        $('#IsActive').attr('checked', true);
        me.data = {};
        me.data.RefferenceType = "";
        me.data.RefferenceCode = "";
        me.data.Description = "";
        me.data.DescriptionEng = "";
        me.data.IsActive = true;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Master Refference",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "RefferenceType", text: "Tipe Refference", type: "select2", cls: "span4", datasource: "ReffSrv", opt_text: "-- SELECT ALL --", model: "data.RefferenceType" },
                    { name: "RefferenceCode", text: "Kode Refference", cls: "span4", placeholder: "Kode Referensi", maxlength: 15, model: "data.RefferenceCode" },
                    { name: "Description", text: "Keterangan", placeholder: "Keterangan", maxlength: 100, model: "data.Description" },
                    { name: "DescriptionEng", text: "Keterangan (Eng.)", placeholder: "Keterangan", maxlength: 100, model: "data.DescriptionEng" },
                    { name: "IsActive", text: "Status", cls: "span4", type: "check", model: "data.IsActive" },
                    { type: "buttons", items: [  { name: "btnPrint", text: "Print", icon: "icon-print", cls: "span4", click: 'print()' } ] }
                ]
            },
            
        ],
    }
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstReffSrvPrintController");
    }
    
});