"use strict"; //Reportid OmRpSalesTrn002
function RptSalesDeliveryOrder($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '-1', "text": 'All' },
        { "value": '0', "text": 'Open' },
        { "value": '1', "text": 'Printed' },
        { "value": '2', "text": 'Approved' },
        { "value": '3', "text": 'Cenceled' },
        { "value": '9', "text": 'Finished' }
    ];

    me.DONo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "DOLookup4Report",
            title: "DO",
            manager: spSalesManager,
            query: "DOLookup4Report?Status=" + me.data.Status,
            defaultSort: "DONo asc",
            columns: [
                { field: "DONo", title: "No. DO" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "DODate", title: "Tgl. DO",
                    template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DONo = data.DONo;
                me.data.DODate = data.DODate;
                me.Apply();
            }
        });

    }

    me.DONoTo = function () {
        if (me.data.Status == '') {
            me.data.Status = '-1';
        }
        var lookup = Wx.blookup({
            name: "DOLookup4Report",
            title: "DO",
            manager: spSalesManager,
            query: "DOLookup4Report?Status=" + me.data.Status,
            defaultSort: "DONo asc",
            columns: [
                { field: "DONo", title: "No. DO" },
                { field: "GroupPriceCode", title: "Tipe" },
                {
                    field: "DODate", title: "Tgl. DO",
                    template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                },
                { field: "Status", title: "Status" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DONoTo = data.DONo;
                me.data.DODateTo = data.DODate;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        if (me.data.DONo == undefined || me.data.DONoTo == undefined) {
            MsgBox('Ada Informasi Yang Belum Lengkap', MSG_ERROR);
        }
        else {
            $http.post('om.api/ReportSales/ValidatePrintDO', me.data)
           .success(function (e) {
               if (e.success) {
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
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    }

    me.Print = function () {
        if (me.data.Status == '-1') {
            me.data.Status = '';
        }
            var sizeType = $('input[name=sizeType]:checked').val() === 'full';
            var ReportId = sizeType ? 'OmRpSalesTrn002' : 'OmRpSalesTrn002A';
            var par = [
                me.data.DONo,
                me.data.DONoTo,
                me.data.ProfitCenterCode,
                me.data.Status
            ]
            var rparam = 'Print Delivery Order'

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                textprint: true,
                rparam: rparam,
                type: "devex"
            });
      
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.DODate = me.now();
        me.data.DODateTo = me.now();

        me.data.Status = '-1';
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });

        me.isPrintAvailable = true;
    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Delivery Order",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "Status", datasource: "Status" },
                        {
                            text: "DO",
                            type: "controls",
                            items: [
                                { name: "DONo", cls: "span2", type: "popup", click: "DONo()" },
                                { name: "DODate", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
                            ]
                        },
                        {
                            text: "s/d",
                            type: "controls",
                            items: [
                                { name: "DONoTo", cls: "span2", type: "popup", click: "DONoTo()" },
                                { name: "DODateTo", cls: "span3", type: "ng-datepicker", disable: "true", readonly: true },
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
        SimDms.Angular("RptSalesDeliveryOrder");

    }
});