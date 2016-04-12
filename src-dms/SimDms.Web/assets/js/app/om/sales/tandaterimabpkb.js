"use strict"
var status = 0;

function omTandaTerimaBPKBController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BPKBOutType = [
        { "value": '0', "text": 'Leasing' },
        { "value": '1', "text": 'Cabang' },
        { "value": '2', "text": 'Pelanggan' }
    ];

    me.BPKBOutBy = function () {
        var lookup = Wx.blookup({
            name: "BPKBOutLookup",
            title: "Serah",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('BPKBOutLookup').withParameters({ Type: me.data.BPKBOutType }),
            defaultSort: "BPKBOutBy asc",
            columns: [
                { field: "BPKBOutBy", title: "Kode" },
                { field: "BPKBOutByName", title: "Nama" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BPKBOutBy = data.BPKBOutBy;
            me.data.BPKBOutByName = data.BPKBOutByName;
            me.Apply();
        });
    }

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "ChassisCode4BPKB",
            title: "Kode Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ChassisCode4BPKB').withParameters({ BPKBOutType: me.data.BPKBOutType, BPKBOutBy: me.data.BPKBOutBy }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            //me.detail.ChassisCode = data.ChassisCode;
            me.detail = data;
            me.Apply();
        });
    }

    me.ChassisNo = function () {
        if (me.detail.ChassisCode != undefined) {
            var lookup = Wx.blookup({
                name: "ChassisNo4BPKB",
                title: "No Rangka",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from('ChassisNo4BPKB').withParameters({ ChassisCode: me.detail.ChassisCode, BPKBOutType: me.data.BPKBOutType, BPKBOutBy: me.data.BPKBOutBy }),
                defaultSort: "ChassisNo asc",
                columns: [
                    { field: "ChassisNo", title: "No Rangka" },
                    { field: "ChassisCode", title: "Kode Rangka" },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail = data;
                me.Apply();
            });
        } else {
            MsgBox('Silahkan pilih dahulu Kode Rangka!', MSG_INFO);
        }
    }

    me.approve = function (e, param) {
        $http.post('om.api/TandaTerimaBPKB/Approve', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#Status').html(data.status);
                    $('#btnApprove').attr('disabled', 'disabled');
                    status = data.Result;
                    me.isStatus = status == 2;
                    Wx.Success("Data approved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.printPreview = function () {
        $http.post('om.api/TandaTerimaBPKB/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#Status').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }
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
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.Print = function () {
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';

        var ReportId = sizeType ? 'OmRpSalRgs028' : 'OmRpSalRgs028';
        var par = [
            me.data.DocNo,
            me.data.DocNo,
            me.data.BPKBOutBy
        ]
        var rparam = 'Print Tanda Terima BPKB'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            textprint: true,
            rparam: rparam,
            type: "devex"
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/TandaTerimaBPKB/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    if (me.data.DocNo == null) {
                        me.data.DocNo = data.data.DocNo;
                        me.saveData();
                    }
                    $('#Status').html(data.status);
                    $('#pnlDetail').show();
                    $('#wxBPKBDetail').show();
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.griddetail.adjust();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddDetail = function (e, param) {
        $http.post('om.api/TandaTerimaBPKB/SaveDetail', { model: me.data, detailModel: me.detail }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.clearTable(me.griddetail);
                    me.loadTableData(me.griddetail, data.grid);
                    me.detail = {};
                    $('#btnAddDetail').show();
                    $('#btnUpdateDetail').hide();
                    $('#btnDeleteDetail').hide();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/TandaTerimaBPKB/Delete', { model: me.data }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        $('#Status').html(data.Status);
                        $('#pnlDetail').hide();
                        $('#wxBPKBDetail').hide();
                        $('#BPKBOutType').attr('disabled', true);
                        $('#BPKBOutBy').attr('disabled', true);
                        $('#btnBPKBOutBy').attr('disabled', true);
                        $('#Remark').attr('disabled', true);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.DeleteDetail = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/TandaTerimaBPKB/DeleteDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        me.clearTable(me.griddetail);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.grid;
                        me.loadTableData(me.griddetail, me.grid.detail);
                        me.detail = {};
                        $('#btnAddDetail').show();
                        $('#btnUpdateDetail').hide();
                        $('#btnDeleteDetail').hide();
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });

    }

    me.browse = function () {
        me.cancelOrClose();
        var lookup = Wx.blookup({
            name: "BPKBBrowse",
            title: "BPKB",
            manager: spSalesManager,
            query: "BPKBBrowse",
            defaultSort: "DocNo desc",
            columns: [
                { field: "DocNo", title: "No. Doc" },
                {
                    field: "DocDate", title: "Tgl",
                    template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #"
                },
                { field: "BPKBOutTypeDes", title: "Tipe Serah" },
                { field: "BPKBOutBy", title: "Kod.Serah" },
                { field: "BPKBOutByName", title: "Nama.Serah" },
                { field: "Stat", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#Status').html(data.Stat);
                me.lookupAfterSelect(data);
                me.loadDetail(data);
                me.status = data.Stat;
                status = data.Status;
                //me.isStatus = status == 2;
                switch (data.Status) {
                    case "1":
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetail').show();
                        $('#wxBPKBDetail').show();
                        $('#btnAddDetail').show();
                        break;
                    case "2":
                        $('#btnAddDetail').show();
                        $('#btnAddDetail').attr('disabled', true);
                        $('#btnCancelDetail').attr('disabled', true);
                        $('#pnlDetail').show();
                        $('#wxBPKBDetail').show();
                        break;
                    case "3":
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        //$('#btnApprove').removeAttr('disabled');
                        $('#btnApprove').attr('disabled', true)
                        $('#pnlDetail').show();
                        $('#wxBPKBDetail').show();
                        $('#btnAddDetail').show();
                }
                me.Apply();
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/TandaTerimaBPKB/DetailSalesBPKB', data)
               .success(function (e) {
                   if (e.success) {
                       me.loadTableData(me.griddetail, e.grid);
                       me.griddetail.adjust();
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    }

    me.griddetail = new webix.ui({
        container: "wxBPKBDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "ChassisCode", header: "Kode Rangka", width: 200 },
            { id: "ChassisNo", header: "No Rangka", width: 200 },
            { id: "CustomerCode", header: "Kode Pelanggan ", width: 200 },
            { id: "CustomerName", header: "Nama Pelanggan", width: 200 },
            { id: "EngineCode", header: "Kode Mesin", width: 200 },
            { id: "EngineNo", header: "No Mesin", width: 200 },
            { id: "SalesModelCode", header: "Sales Model", width: 200 },
            { id: "ColourCode", header: "Warna", width: 200 },
            { id: "BPKBNo", header: "No BPKB", width: 200 },
            { id: "PoliceRegistrationNo", header: "No Pol", width: 200 },
            { id: "PoliceRegistrationDate", header: "Tgl No Pol", width: 200 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetail.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.griddetail.getSelectedId().id);
                    switch (status) {
                        case "1":
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                            $('#Remark1').attr('disabled', true);
                            $('#btnChassisCode').attr('disabled', true);
                            $('#btnChassisNo').attr('disabled', true);
                            $('#ChassisCode').attr('disabled', true);
                            $('#ChassisNo').attr('disabled', true);
                            break;
                        case "2":
                            $('#btnAddDetail').attr('disabled', true);
                            $('#btnCancelDetail').attr('disabled', true);
                            $('#btnChassisCode').attr('disabled', true);
                            $('#btnChassisNo').attr('disabled', true);
                            break;
                        case "3":
                            break;
                        default:
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnChassisCode').removeAttr('disabled');
                            $('#btnChassisNo').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                    }
                    me.detail.oid = me.griddetail.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.CancelDetail = function () {
        me.detail = {};
        me.detail.PoliceRegistrationDate = me.now();
        me.griddetail.clearSelection();
    }

    me.initialize = function () {
        me.isStatus = false;
        me.detail = {};
        me.data.StatusFaktur = true;
        me.data.BPKBOutType = "0";


        me.status = "NEW";
        $('#Status').html(me.status);
        $('#Status').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('#pnlDetail').hide();
        $('#wxBPKBDetail').hide();
        $('#btnApprove').attr('disabled', true)

        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();

        me.data.DocDate = me.now();
        me.isPrintAvailable = true;
        //me.isApprove = true;
        //$('#btnApprove').attr('disabled', true);
        me.griddetail.adjust();
    }

    me.$watch('isLoadData', function (newData, oldData) {
        console.log(me.status);
        console.log(me.isLoadData);

        if (me.status != "NEW") {
            if (!me.isLoadData) {
                me.isLoadData = true;
            }
            else {
                me.isLoadData = true;
            }
        }
        else {
            me.isLoadData = false;
        }
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Tanda Terima BPKB",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "DocNo", model: "data.DocNo", text: "No.BPKB Terima", cls: "span3", placeHolder: "TTB/YY/XXXXXX", disable: true },
                    {
                        type: "controls",
                        text: "Tgl. BPKB Terima",
                        cls: "span5",
                        items: [
                            { name: "DocDate", model: "data.DocDate", type: "ng-datepicker", cls: "span3", disable: "isStatus" },
                            { name: "Status", text: "", cls: "span3 right", readonly: true, type: "label" },
                            {
                                type: "buttons", cls: "span2 right", items: [
                                    {
                                        name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()",
                                        disable: "data.Stat == 0 || data.Stat == 2 || data.Stat == 3"
                                    }
                                ]
                            },
                        ]
                    },
                    { name: "BPKBOutType", model: "data.BPKBOutType", cls: "span3 full", type: "select2", datasource: "BPKBOutType", text: "Tipe Penerima", disable: "isStatus", required: true, validasi: "required" },
                    {
                        text: "Diserahkan ke-",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "BPKBOutBy", model: "data.BPKBOutBy", cls: "span2", disable: "isStatus", validasi: "required", type: "popup", click: "BPKBOutBy()" },
                            { name: "BPKBOutByName", model: "data.BPKBOutByName", cls: "span6", readonly: true, disable: "isStatus" },
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                ]
            },
            {
                name: "pnlDetail",
                title: "Detail BPKB",
                items: [
                    { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisCode()" },
                    { name: "ChassisNo", model: "detail.ChassisNo", text: "No Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisNo()" },
                    { name: "EngineCode", model: "detail.EngineCode", text: "Kode Mesin", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "EngineNo", model: "detail.EngineNo", text: "No Mesin", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "SalesModelCode", model: "detail.SalesModelCode", text: "Sales Model", readonly: true, cls: "span3", disable: "isStatus" },
                    { name: "ColourCode", model: "detail.ColourCode", text: "Kode Warna", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "PoliceRegistrationNo", model: "detail.PoliceRegistrationNo", text: "No Polisi", readonly: true, cls: "span3", disable: "isStatus" },
                    { name: "PoliceRegistrationDate", model: "detail.PoliceRegistrationDate", text: "Tgl No Polisi", type: "ng-datepicker", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "FakturPolisiNo", model: "detail.FakturPolisiNo", text: "No Faktur Polis", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "BPKBNo", model: "detail.BPKBNo", text: "No BPKB", cls: "span3", readonly: true, disable: "isStatus" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "CustomerCode", model: "detail.CustomerCode", cls: "span3", readonly: true, disable: "isStatus" },
                            { name: "CustomerDesc", model: "detail.CustomerName", cls: "span5", readonly: true, disable: "isStatus" },
                        ]
                    },
                    { name: "Remark1", model: "detail.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnUpdateDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", disable: "isStatus" },
                                { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelDetail()", disable: "isStatus" }
                        ]
                    },
                ]
            },
            {
                name: "wxBPKBDetail",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omTandaTerimaBPKBController");
    }



});