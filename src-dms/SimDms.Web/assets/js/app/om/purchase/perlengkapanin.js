"use strict"

var batchNo = '';

function omPerlengkapanInController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    var equipmentType = [
        { text: 'BPU', value: "1" },
        { text: 'TRANSFER', value: "2"},
        { text: 'RETURN', value: "3"}
    ]
    me.comboEquipmentType = equipmentType;

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('0', function () {
        $('#btnRefferenceNo').attr('disabled', true);
        $('#btnSourceDoc').removeAttr('disabled');
    });

    $scope.$on('1', function () {
        $('#btnRefferenceNo').removeAttr('disabled');
        $('#btnSourceDoc').attr('disabled', true);
    });

    $('#RefferenceNo').on('change', function (e) {
        var refNo = me.data.RefferenceNo;
        if (refNo !== '') {
            $('#RefferenceDate').removeAttr('disabled');
        }
        else {
            $('#RefferenceDate').attr('disabled', true);
        }
    })

    me.browse = function () {
        me.options = '0';
        me.init();
        var lookup = Wx.blookup({
            name: "EquipmentIn",
            title: "Perlengkapan",
            manager: spSalesManager,
            query: "EquipmentInBrowse",
            defaultSort: "PerlengkapanNo desc",
            columns: [
               { field: 'PerlengkapanNo', title: 'No. Perlengkapan' },
               { field: 'PerlengkapanDate', title: 'Tgl. Perlengkapan' },
               { field: 'PerlengkapanTypeName', title: 'Type' },
               { field: 'RefferenceNo', title: 'Referensi' },
               { field: 'SourceDoc', title: 'Source' },
               { field: 'Remark', title: 'Keterangan' },
               { field: 'Status', title: 'Status' }
            ]
        });
        lookup.dblClick(function (data) {
            $('#btnSourceDoc').attr('disabled', 'disabled');
            $('.isbrowse').attr('disabled', 'disabled');
            $('#RefferenceNo').attr('disabled', 'disabled');
            $('#RefferenceDate').attr('disabled', 'disabled');
            $('#PerlengkapanDate').attr('disabled', 'disabled');
            $('#PerlengkapanType').attr('disabled', 'disabled');
            
            $('#lblStatus').html(data.Status);
            me.lookupAfterSelect(data);
            me.isApprove = data.Stat == "2";
            if (data.Stat == "1") {
                $('#btnApprove').removeAttr('disabled');
            }
            else {
                $('#btnApprove').attr('disabled', 'disabled');
            }

            if (data.Stat == "3") {
                $('#btnPerlengkapanNo').attr('disabled', 'disabled');
            }
            else {
                $('#btnPerlengkapanNo').removeAttr('disabled');
            }

            me.loadDetail(data.PerlengkapanNo);
        });
    }

    me.EquipmentBrowse = function () {
        me.options = '0';
        var lookup = Wx.blookup({
            name: "Equipment",
            title: "Perlengkapan",
            manager: spSalesManager,
            query: "EquipmentBrowse",
            defaultSort: "PerlengkapanCode asc",
            columns: [
               { field: 'PerlengkapanCode', title: 'No. Perlengkapan' },
               { field: 'PerlengkapanName', title: 'Tgl. Perlengkapan' },
               { field: 'Remark', title: 'Keterangan' },
            ]
        });
        lookup.dblClick(function (data) {
            //me.lookupAfterSelect(data);
            me.detail.PerlengkapanCode = data.PerlengkapanCode;
            me.detail.PerlengkapanName = data.PerlengkapanName;
            me.Apply();
        });
    }

    me.sourceDocBrowse = function () {
        var columns = {};

        switch (me.data.PerlengkapanType) {
            case "1":
                columns = [
                     { field: 'BPUNo', title: 'No. BPU' },
                     { field: 'BPUDate', title: 'Tgl. BPU' },
                     { field: 'PONo', title: 'No. PO' },
                     { field: 'ReffereneSJNo', title: 'No. Ref. SJ' },
                ]
                break;
            case "2":
                columns = [
                     { field: 'TransferInNo', title: 'No. Transfer' },
                     { field: 'TransferInDate', title: 'Tgl. Transfer' },
                ]
                break;
            case "3":
                columns = [
                     { field: 'ReturnNo', title: 'No. Return' },
                     { field: 'ReturnDate', title: 'Tgl. Return' },
                     { field: 'RefferenceNo', title: 'No. Reff' }
                ]
                break;
        }
        var lookup = Wx.blookup({
            name: "SourceDocBrowse",
            title: $("#PerlengkapanType").select2('data').text,
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SourceDocBrowse").withParameters({ type: me.data.PerlengkapanType }),
            columns: columns
        });
        lookup.dblClick(function (data) {
            switch (me.data.PerlengkapanType) {
                case "1":
                    me.data.SourceDoc = data.BPUNo;
                    break;
                case "2":
                    me.data.SourceDoc = data.TransferInNo;
                    break;
                case "3":
                    me.data.SourceDoc = data.ReturnNo;
                    break;
            }
            me.Apply();
        });
    }

    me.refferenceNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "RefferenceNoBrowse",
            title: "Refference",
            manager: spSalesManager,
            query: "ReffEqInNoBrowse",
            defaultSort: "BPPNo asc",
            columns: [
                 { field: 'BPPNo', title: 'No. BPPNo' },
                 { field: 'BatchNo', title: 'No. Batch' },
                 { field: 'RefferenceSJNo', title: 'No. SJ' },
                 { field: 'BPPDate', title: 'Tgl. BPP' }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.RefferenceNo = data.BPPNo;
            me.Apply();
        });
    }

    me.gridDetailPerlengkapanIn = new webix.ui({
        container: "wxDetailPerlengkapanIn",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PerlengkapanCode", header: "Kode", fillspace: true },
            { id: "PerlengkapanName", header: "Perlengkapan", fillspace: true },
            { id: "Quantity", header: "Jumlah PO", width: 100, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetailPerlengkapanIn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridDetailPerlengkapanIn.getSelectedId());
                    me.detail = data;
                    me.Apply();
                }
            }
        }
    });

    me.save = function () {
        if (me.data.SourceDoc == undefined || me.data.SourceDoc == null) {
            MsgBox("Sumber Data Harus Diisi terlebih dahulu");
        }
        else {
            $http.post('om.api/perlengkapanin/validateSave', { model: me.data, options: me.options, batchNo: batchNo })
            .success(function (e) {
                if (e.success) {
                    Wx.Success(e.message);
                    me.loadDetail(e.perlengkapanNo);
                    $("#pnlDetailPerlengkapan").show();
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            })
        }
    }

    me.delete = function () {
        if (confirm("Apakah Anda Yakin ???", "Posting Data")) {
            $http.post('om.api/perlengkapanin/delete', me.data)
            .success(function (e) {
                if (e.success) {
                    me.resetStat(e.status);
                    me.isApprove = true;
                    me.isCancel = true;
                }
                Wx.Success(e.message);
            })
            .error(function (e) {

            });
        }
    }

    me.SaveDetail = function () {
        $http.post('om.api/perlengkapanin/SaveDetail', { model: me.detail, perlengkapanNo: me.data.PerlengkapanNo })
        .success(function (e) {
            if (e.success) {
                Wx.Success(e.message);
                me.loadDetail(me.data.PerlengkapanNo);
                me.resetStat("OPEN");
                me.detail = {};
                me.isPrintAvailable = true;
            } else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.DeleteDetail = function () {
        $http.post('om.api/perlengkapanin/deletedetail', { model: me.detail, perlengkapanNo: me.data.PerlengkapanNo })
       .success(function (e) {
           if (e.success) {
               Wx.Success(e.message);
               me.loadDetail(me.data.PerlengkapanNo);
               me.resetStat("OPEN");
               me.detail = {};
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.approve = function () {
        if (confirm("Apakah Anda Yakin ???", "Posting Data")) {
            $http.post('om.api/perlengkapanin/approve', me.data)
            .success(function (e) {
                if (e.success) {
                    me.resetStat(e.status);
                    me.isApprove = true;
                    me.isCancel = true;
                    me.hasChanged = false;
                }
                Wx.Success(e.message);
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    }

    me.printPreview = function () {
        $http.post('om.api/perlengkapanin/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               
               if (e.stat == "1") {
                   var ReportId = 'OmRpPurTrn005';

                   var par = [
                       me.data.PerlengkapanNo,
                       me.data.PerlengkapanNo,
                       ''
                   ]
                   var rparam = 'Print Perlengkapan In'

                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: par.join(','),
                       rparam: rparam,
                       type: "devex"
                   });

                   $('#btnApprove').removeAttr('disabled');
                   $('#lblStatus').html(e.Status);
               }


           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.resetStat = function (stat) {
        $('#lblStatus').html(stat);
        $('#btnApprove').attr('disabled', 'disabled');
    }

    me.loadDetail = function (data) {
        $("#pnlDetailPerlengkapan").show();

        $http.post('om.api/perlengkapanin/getdetailgrid', {perlengkapanNo : data})
       .success(function (e) {
           me.data = e.data
           me.loadTableData(me.gridDetailPerlengkapanIn, e.grid);
           if (e.grid.length > 0) me.isPrintAvailable = true;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    webix.event(window, "resize", function () {
        me.gridDetailPerlengkapanIn.adjust();
    });

    me.initialize = function () {
        me.detail = {};
        me.data.PerlengkapanType = "1";
        me.data.RefferenceDate = me.now();
        me.data.PerlengkapanDate = me.now();
        me.isPrintAvailable = false;
        me.isApprove = true;
        me.isCancel = false;
        $('#lblStatus').html("NEW");
        $('#lblStatus').css(
         {
             "font-size": "32px",
             "color": "red",
             "font-weight": "bold",
             "text-align": "center"
         });

        $('.isbrowse').removeAttr('disabled');
        $("#pnlDetailPerlengkapan").hide();
        $('#btnSourceDoc').removeAttr('disabled');
        $('#RefferenceNo').removeAttr('disabled');
        $('#RefferenceDate').removeAttr('disabled');
        $('#PerlengkapanDate').removeAttr('disabled');
        $('#PerlengkapanType').removeAttr('disabled');
    }

    me.start();
    me.isApprove = true;
    me.isCancel = false;
    me.options = '0';
    $('#btnRefferenceNo').attr('disabled', true);
}

$(document).ready(function () {
    var options = {
        title: "Perlengkapan In",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnloptions",
                items: [
                      {
                          type: "optionbuttons",
                          name: "tabpageoptions",
                          model: "options",
                          items: [
                              { name: "0", text: "Manual" },
                              { name: "1", text: "Upload" },
                          ]
                      },
                ]
            },
            {
                name: "pnlStatus",
                items: [
                    { name: "lblStatus", text: "", cls: "span4", readonly: true, type: "label" },
                    {
                        type: "buttons", cls: "span4", items: [
                               { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                        ]
                    }
                ]
            },
            {
                name: "pnlPerlengkapanIn",
                items: [
                    { name: "PerlengkapanNo", text: "No. Perlengkapan", cls: "span4", readonly: true, placeHolder: 'PIN/XX/YYYYYY' },
                    { name: "PerlengkapanDate", text: "Tgl. Perlengkapan", cls: "span4 isbrowse", type: "ng-datepicker" },
                    { name: "RefferenceNo", text: "No. Reff", cls: "span4 isbrowse", type: "popup", click: "refferenceNoBrowse()" },
                    { name: "RefferenceDate", text: "Tgl. Reff", cls: "span4 isbrowse", type: 'ng-datepicker', disabled: true },
                    { name: "PerlengkapanType", text: "Tipe Perlengkapan", cls: "span4 isbrowse", type: 'select2', datasource: 'comboEquipmentType' },
                    { name: "SourceDoc", text: "No. Sumber Dok.", cls: "span4 isbrowse", type: "popup", required: true, readonly:true, click: "sourceDocBrowse()" },
                    { name: "Remark", text: "Keterangan", cls: "span12 isbrowse", },
                ]
            },
            {
                name: "pnlDetailPerlengkapan",
                title: "Detail Perlengkapan",
                items: [
                {
                    text: "Perlengkapan",
                    type: "controls",
                    required : true,
                    items: [
                        { name: "PerlengkapanNo", model:"detail.PerlengkapanCode", cls: "span2", placeHolder: "Kode Perlengkapan", readonly: true, type: "popup", click:"EquipmentBrowse()" },
                        { name: "PerlengkapanName", model: "detail.PerlengkapanName", cls: "span6", placeHolder: "Nama Perlengkapan", readonly: true }
                    ]
                },
                { name: "Quantity", text: "Jumlah", model: "detail.Quantity", cls: "span4 number-int", readonly: false, required: true },
                { name: "Remark", text: "Keterangan", model: "detail.Remark", cls: "span12", readonly: false },
                {
                     type: "buttons",
                     items: [
                             { name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "SaveDetail()", disable: "detail.PerlengkapanCode === undefinedisCancel", show: "!isCancel" },
                             { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", disable: "detail.PerlengkapanCode === undefined", show: "!isCancel" },
                     ]
                 },
                         {
                             name: "wxDetailPerlengkapanIn",
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
        SimDms.Angular("omPerlengkapanInController");
    }
});