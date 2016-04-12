var type = "";
var flatfile="";

function svGenKSGSPKController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post('sv.api/genksgspk/default')
       .success(function (e) {
           me.data = e;
           me.data.ReceiptDate = me.now();
           me.fullName = e.FullName;
       });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "GenKSGSPKLookup",
            title: "Pencarian No. Batch KSG",
            manager: svServiceManager,
            query: "GenKSGSPKLookup",
            defaultSort: "BatchNo desc",
            columns: [
               { field: 'BatchNo', title: 'No. Batch' },
               {
                   field: "BatchDate", title: "Tgl. Batch", sWidth: "130px",
                   template: "#= (BatchDate == undefined) ? '' : moment(BatchDate).format('DD MMM YYYY') #"
               },
               { field: 'GenerateNoStart', title: 'GenerateNo Start' },
               { field: 'GenerateNoEnd', title: 'GenerateNo End' },
               { field: 'TotalNoOfItem', title: 'Jml. Item' },
               { field: 'TotalAmt', title: 'Tot. Nilai', format: '{0:n2}' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BatchNo = data.BatchNo;
            me.loadDetail();
            $('#btnSave').hide();
        });
    }

    me.loadDetail = function () {
        $http.post('sv.api/genksgspk/GetPdiFscBatchFromSPK', me.data)
       .success(function (e) {
           me.loadTableData(me.grid1, e);
           me.hasChanged = false;
           $('#btnQuery').prop('disabled', true);
           $('#btnGenerate').prop('disabled', false);
           me.isPrintAvailable = true;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.query = function () {
        $http.post('sv.api/genksgspk/SelectPdiFscFromSPK')
       .success(function (e) {
           me.loadTableData(me.grid1, e);
           me.hasChanged = true;
           me.detail = e;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.save = function () {
        var datDetail = [];
        $.each(me.detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                datDetail.push({ BranchCode: val["BranchCode"], GenerateNo: val["GenerateNo"] });
            }
        })

        $http.post('sv.api/genksgspk/save', { model: datDetail, ReceiptNo: me.data.ReceiptNo, ReceiptDate: me.data.ReceiptDate }).
            success(function (e) {
                if (e.success) {
                    me.data.BatchNo = e.data.BatchNo;
                    me.loadDetail();
                    Wx.Success(e.message);
                }
                else {
                    MsgBox(e.message);
                }
            }).error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.print = function () {
        var param = [
            'producttype',
            me.data.BatchNo
        ];
        Wx.showPdfReport({
            id: "SvRpTrn011",
            pparam: param,
            rparam: me.fullName,
            type: "devex"
        });
    }

    $('#BatchNo').on('blur', function (e) {
        $http.post('sv.api/genksgspk/checkbatchno', me.data)
       .success(function (e) {
           if (e) {
               me.loadDetail();
           }
       })
       .error(function (e) {
           me.browse();
       });
    });

    me.generate = function () {
        $http.post('sv.api/genksgspk/createwfres', me.data)
               .success(function (e) {
                   if (e.success) {
                       flatfile = e.contents;
                       Wx.showFlatFile({ data: e.contents });
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
    }

    SavePopup = function () {
        window.location = 'sv.api/genksgspk/GetWFRES';
    }

    SendPopup = function () {
        $http.post('sv.api/genksgspk/ValidateHeaderFile', { contents: $('#pnlViewData').val() })        
        .success(function (e) {
            //console.log(flatfile);
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                //$http.post('sv.api/genksgspk/SendFile', { contents: $('#pnlViewData').val() })
                                $http.post('sv.api/genksgspk/SendFile', { contents: flatfile })
                                .success(function (data, status, headers, config) {
                                    if (data.success) {
                                        Wx.Success(data.message);
                                        me.HideForm();
                                    }
                                    else {
                                        MsgBox(data.message, MSG_ERROR);
                                    }
                                }).error(function (e, status, headers, config) {
                                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                });
                            }
                        });
                    }
                });
            }
            else {
                MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                    if (result) {
                        //$http.post('sv.api/genksgspk/SendFile', { contents: $('#pnlViewData').val() })
                        $http.post('sv.api/genksgspk/SendFile', { contents: flatfile })
                        .success(function (data, status, headers, config) {
                            if (data.success) {
                                Wx.Success(data.message);
                                me.HideForm();
                            }
                            else {
                                MsgBox(data.message, MSG_ERROR);
                            }
                        }).error(function (e, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        });
                    }
                });
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.initialize = function () {
        me.detail = {};
        me.default();
        me.clearTable(me.grid1);
        me.isPrintAvailable = false;
        $('#btnQuery').prop('disabled', false);
        $('#btnGenerate').prop('disabled', true);
        me.grid1.hideColumn('BranchCode');
    }

    me.grid1 = new webix.ui({
        container: "wxgenksgspk",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 330,
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "BranchData", header: "Kode Cabang", width: 150 },
            { id: "BranchCode", header: "Kode Cabang", width: 150 },
            { id: "GenerateNo", header: "No. PDI-FSC", width: 150 },
            { id: "GenerateDate", header: "Tgl. PDI-FSC", width: 130, format: me.dateFormat },
            { id: "SenderDealerCode", header: "Kode Dealer", width: 130 },
            { id: "SenderDealerName", header: "Nama Dealer", width: 200 },
            { id: "TotalNoOfItem", header: "Jml. Kupon", format: me.intFormat, width: 110 },
            { id: "TotalLaborAmt", header: "Nilai Jasa", format: me.intFormat, width: 120 },
            { id: "TotalMaterialAmt", header: "Nilai Material", format: me.intFormat, width: 120 },
            { id: "TotalAmt", header: "Total", format: me.intFormat, width: 120 },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }

}


$(document).ready(function () {
    var options = {
        title: "Generate Data KSG dari SPK",
        xtype: "panels",
        toolbars: [
                    { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "cancelOrClose()" },
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()" },
                    { name: "btnPrint", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable", click: "print()" },
        ],
        panels: [
             {
                 name: "pnlbutton",
                 items: [{
                     type: "buttons",
                     items: [
                         { name: "btnQuery", text: "Query", cls: "btn btn-info", icon: "icon-search", click: "query()" },
                         { name: "btnGenerate", text: "Generate File (WFRES)", cls: "btn btn-info", icon: "icon-bolt", click: "generate()", disable: true }
                     ]
                 },
                 ]
             },
            {
                name: "pnlgenksgspk",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", placeHolder: "Company Code", cls: "span2", readonly: true },
                            { name: "CompanyName", placeHolder: "Name", cls: "span6", readonly: true }
                        ]
                    },
                    { name: "BatchNo", text: "No. Batch", cls: " full", placeHolder: "BAT/XX/YYYYYY", required: true },
                    { name: "ReceiptNo", text: "No. Kwitansi", cls: "span4" },
                    { name: "ReceiptDate", text: "Tanggal", cls: "span4", type: "ng-datepicker" },
                    {
                        name: "wxgenksgspk",
                        title: "Informasi Data PDI dan FSC",
                        type: "wxdiv"
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svGenKSGSPKController");
    }
});