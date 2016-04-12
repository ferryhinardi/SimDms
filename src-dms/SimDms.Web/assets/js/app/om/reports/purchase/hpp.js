"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '-1', "text": 'ALL' },
        { "value": '0', "text": 'OPEN' },
        { "value": '1', "text": 'PRINTED' },
        { "value": '2', "text": 'APPROVED' },
        { "value": '3', "text": 'CANCELED' },
        { "value": '9', "text": 'FINISHED' },
    ];

    me.HPPNoFrom = function () {
        var lookup = Wx.blookup({
            name: "HPPNoLookup",
            title: "No. HPP",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("HPP4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "HPPNo asc",
            columns: [
                { field: "HPPNo", title: "No. HPP" },
                { field: "HPPDate", title: "Tgl. HPP", template: "#= (HPPDate == undefined) ? '' : moment(HPPDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.HPPNoFrom = data.HPPNo;
                me.data.HPPDateFrom = data.HPPDate;
                me.Apply();
            }
        });

    }

    me.HPPNoTo = function () {
        var lookup = Wx.blookup({
            name: "NoHPPLookup",
            title: "No. HPP",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("HPP4Lookup").withParameters({ Status: me.data.Status }),
            defaultSort: "HPPNo asc",
            columns: [
                 { field: "HPPNo", title: "No. HPP" },
                { field: "HPPDate", title: "Tgl. HPP", template: "#= (HPPDate == undefined) ? '' : moment(HPPDate).format('DD MMM YYYY') #" },
                 { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.HPPNoTo = data.HPPNo;
                me.data.HPPDateTo = data.HPPDate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.HPPNoFrom || me.data.HPPNoTo) {
            BootstrapDialog.show({
                message: $(
                    '<div class="container">' +
                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Print Satu Halaman</div>' +

                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Print Setengah Halaman</div>'),
                closable: false,
                draggable: true,
                type: BootstrapDialog.TYPE_INFO,
                title: 'Print',
                buttons: [{
                    label: ' Print',
                    cssClass: 'btn-primary icon-print',
                    action: function (dialogRef) {
                        me.Print();
                        dialogRef.close();
                    }
                }, {
                    label: ' Cancel',
                    cssClass: 'btn-warning icon-remove',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }]
            });
        } else {
            MsgBox("No. HPP Awal/ Akhir Harus diisi", MSG_ERROR);
        }
    }

    me.Print = function () {
        //alert(me.data.ProfitCenterCode);
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurTRN003' : 'OmRpPurTRN003A';
        var prm = [
                   // me.data.CompanyCode,
                    me.data.HPPNoFrom,
                    me.data.HPPNoTo,
                    me.data.ProfitCenterCode,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: ReportId,//"OmRpPurTRN001",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Status = '-1';
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              //me.data.ProfitCenterCode = dl.ProfitCenter

          });

        $http.get('breeze/sales/ProfitCenter').
      success(function (dl, status, headers, config) {
          me.data.ProfitCenterCode = dl.ProfitCenter;
      });
        me.data.HPPDateFrom = me.now();
        me.data.HPPDateTo = me.now();
        $('#HPPDateFrom').attr('disabled', true);
        $('#HPPDateTo').attr('disabled', true);
        me.isPrintAvailable = true;
    }
    
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report HPP",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "No.HPP",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "HPPNoFrom", cls: "span3", type: "popup", btnName: "btnHPPNoFrom", click: "HPPNoFrom()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "HPPDateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "HPPNoTo", cls: "span3", type: "popup", btnName: "btnHPPNoTo", click: "HPPNoTo()", disable: "data.isActive == false", required: true, validasi: "required" },
                                { name: "HPPDateTo", text: "", cls: "span3", type: "ng-datepicker" },

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
        SimDms.Angular("spRptMstSales");

    }
});