"use strict"

function svMstWarrantyPrintController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/BasicModelWarranty').
     success(function (data, status, headers, config) {
         me.BasicModel = data;
     });

    $http.post('sv.api/Combo/OperationNoWarranty').
     success(function (data, status, headers, config) {
         me.OperationNo = data;
     });

    $('#AllStatus').on('change', function () {
        if ($('#AllStatus').is(':checked')) {
            $('#IsActive').attr("disabled", true);
        }
        else {
            $('#IsActive').removeAttr("disabled");
        }
        //me.Apply();
    });

    me.print = function () {
        var status = "", basicModel = "%", operationNo = "%", description = "%";

        if ($('#AllStatus').is(':checked')){
            status = '2';
        }
        else{
            if ($('#IsActive').is(':checked')){ 
                status = "1"; 
            }
            else status = "0"; 
        }
        if (me.data.BasicModel == null || me.data.BasicModel == '') basicModel = '%';
        else basicModel = me.data.BasicModel;
        if (me.data.OperationNo == null || me.data.OperationNo == '') operationNo = '%';
        else operationNo = me.data.OperationNo;
        if (me.data.Description == null || me.data.Description == '') description = "%"
        else description = me.data.Description;

        var prm = [
          'companycode',
           basicModel,
           operationNo,
           description,
           status
        ];

        Wx.showPdfReport({
            id: 'SvRpMst006',
            pparam: prm.join(','),
            rparam: 'Master Garansi',
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
        title: "Master Garansi",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "BasicModel", text: "Basic Model", type: "select2", cls: "span4", datasource: "BasicModel", opt_text: "-- SELECT ALL --", model: "data.BasicModel" },
                    { name: "OperationNo", text: "Jenis Pekerjaan", type: "select2", cls: "span4", datasource: "OperationNo", opt_text: "-- SELECT ALL --", model: "data.OperationNo" },
                    { name: "Description", text: "Keterangan", placeholder: "Keterangan", maxlength: 100, model: "data.Description" },
                    { name: "AllStatus", text: "All Status", cls: "span4", type: "check" },
                    { name: "IsActive", text: "Aktif", cls: "span4", type: "check", model: "data.IsActive" },
                    { type: "buttons", items: [{ name: "btnPrint", text: "Print", icon: "icon-print", cls: "span4", click: 'print()' }] }
                ]
            },

        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstWarrantyPrintController");
    }

});