"use strict";
function RptDaftarBpkbPerlokasi($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    
    me.BranchCd = function () {
        var lookup = Wx.blookup({
            name: "DaftarBpkbPerlokasiBrnc4Report",
            title: "Daftar Cabang",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('DaftarBpkbPerlokasiBrnc4Report'),
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "BranchName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCd = data.BranchCode;
                me.data.BranchNm = data.BranchName;
                me.Apply();
            }
        });
        console.log(data);
    }

    me.printPreview = function () {
        if (me.data.BranchCd == null) {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }

        if ($('#chkBranch').prop('checked') == true) {
            var param = [
                    'companycode',
                    me.data.BranchCd

            ];
            var reportId = "OmRpSalRgs030B";
        }
        else {
            var param = [                
                   'companycode'
            ];
            var reportId = "OmRpSalRgs030A";
        }

        Wx.showPdfReport({
            id: reportId,
            pparam: param.join(','),
            textprint: true,
            rparam: "Print Daftar BPKB Perlokasi",
            type: "devex"
        });

    }
    
    $("[name = 'chkBranch']").on('change', function () {
        if ($('#chkBranch').prop('checked')==true) {
            me.data.BranchCd = "";
            me.data.BranchNm = "";
        }
        else {
            me.data.BranchCd = me.data.BranchCode;
            me.data.BranchNm = me.data.BranchName;
        }
        
        me.Apply();
    });

    

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              me.data.BranchName = dl.CompanyName;

              me.data.BranchCd = dl.BranchCode;
              me.data.BranchNm = dl.CompanyName;
          });

        me.isPrintAvailable = true;

    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Daftar BPKB Perlokasi",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span2 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchName", model: "data.BranchName", text: "Nama Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },
                        {
                            text: "Daftar BPKB di ",
                            type: "controls",
                            items: [

                                { name: "chkBranch", model: "data.chkBranch", text: "Branch", cls: "span1", type: "ng-check" },
                                { name: "BranchCd", model: "data.BranchCd", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "BranchCd()", disable: "!data.chkBranch" },
                                { name: "BranchNm", model: "data.BranchNm", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkBranch" }
                            ]
                        },
                        
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("RptDaftarBpkbPerlokasi");

    }
});