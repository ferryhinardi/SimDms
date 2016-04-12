var Wx;

"use strict";

function AOSAlert($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    
    webix.event(window, "resize", function () {
        me.aosgrid.adjust();
    })

    me.aosgrid = new webix.ui({
        container: "wxgrid",
        view: "wxtable", css: "alternating",
        //scrollX: true,
        columns: [
            { id: "Branch", header: "Branch", width: 100 },
            { id: "BranchName", header: "Branch Name", width: 200 },
            { id: "POSNo", header: "POS No", width: 200 },
            { id: "POSDate", header: "POS Date", width: 200, format:webix.i18n.parseFormat = "%m.%d.%Y" },
        ],
        on: {
            onSelectChange: function () {
                if (me.aosgrid.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.aosgrid.getSelectedId());
                    me.data.detail.POSNo = griddata.POSNo;
                    me.data.detail.Branch = griddata.Branch;
                    me.data.detail.POSDate = griddata.POSDate;
                    me.data.detail.PORDS = griddata.PORDS;
                    if (me.data.detail.PORDS == false) {
                        $('#btnCreatePORDS').show();
                    } else {
                        $('#btnCreatePORDS').hide();
                    }
                    me.Apply();
                }
            }
        }
    });


    me.doView = function () {
        localStorage.setItem('POSNo', me.data.detail.POSNo);
        localStorage.setItem('Branch', me.data.detail.Branch);
        Wx.loadForm();
        Wx.showForm({ url: "sp/pembelian/entryorderaos" });
        //Wx.alert("Customer Code tidak boleh kosong");




        //MsgBox(me.data.detail.POSNo, MSG_INFO);
    };

    me.createPRODS = function () {
        $http.post("sp.api/PembelianEntryOrderSparepart/GetSODetailAOS", { POSNo: me.data.detail.POSNo, branch: me.data.detail.Branch }).
            success(function (data, status, headers, config) {
                me.data2 = data.dataHeader;
                $http.post('sp.api/PembelianEntryOrderSparepart/CreatePORDSAOS', { model: data.dataHeader, Branch: me.data.detail.Branch })
                .success(function (v, status, headers, config) {
                    if (v.success) {
                        Wx.showFlatFile({ data: v.data });
                    } else {
                        // show an error message
                        MsgBox(v.message, MSG_ERROR);
                        console.log(v.err);
                    }
                    me.startEditing();
                }).error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    SavePopup = function () {
        window.location = "sp.api/PembelianEntryOrderSparepart/DownloadFileAOS?SupplierCode=" + me.data2.SupplierCode + "&POSNo=" + me.data.detail.POSNo + "&Branch=" + me.data.detail.Branch;
    }

    SendPopup = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/ValidateHeaderFile', { model: me.data2, Branch: me.data.detail.Branch })
        .success(function (e) {
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('sp.api/PembelianEntryOrderSparepart/SendFileAOS', { model: me.data2, Branch: me.data.detail.Branch })
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
                        $http.post('sp.api/PembelianEntryOrderSparepart/SendFileAOS', { model: me.data2, Branch: me.data.detail.Branch })
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

    me.AOSLog = function () {
        $('#pnlA').hide();
        $('#btnAOSLog').attr('disabled', 'disabled');
        $('#btnCreatePORDS').attr('disabled', 'disabled');
        $('#btnView').attr('disabled', 'disabled');
        $('#pnlB').show();
        $('[name=Tahun]').val(new Date().getFullYear())
        $('[name=Bulan]').select2('val', new Date().getMonth()+1);

    }

    me.doExport = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/doExport', { Branch: me.data.detail.Branch, PMonth: me.data.Bulan , PYear: me.data.Tahun })
            .success(function (data, status, headers, config) {
            if (data.success) {
                location.href = 'sp.api/PembelianEntryOrderSparepart/DownloadExcelFile?key=' + data.value + '&fileName=AOSLog' + me.data.Bulan + me.data.Tahun;
            }
            else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.doUndo = function () {
        $('#pnlA').show();
        $('#btnAOSLog').removeAttr('disabled');
        $('#btnCreatePORDS').removeAttr('disabled');
        $('#btnView').removeAttr('disabled');
        $('#pnlB').hide();
    }

    me.PMonth = [
        { "value": '1', "text": 'JANUARI' },
        { "value": '2', "text": 'FEBRUARI' },
        { "value": '3', "text": 'MARET' },
        { "value": '4', "text": 'APRIL' },
        { "value": '5', "text": 'MEI' },
        { "value": '6', "text": 'JUNI' },
        { "value": '7', "text": 'JULI' },
        { "value": '8', "text": 'AGUSTUS' },
        { "value": '9', "text": 'SEPTEMBER' },
        { "value": '10', "text": 'OKTOBER' },
        { "value": '11', "text": 'NOVEMBER' },
        { "value": '12', "text": 'DESEMBER' }
    ];

    me.initialize = function () {
        //$('[name=Tahun]').val(new Date().getFullYear())
        me.data2 = {};
        me.data.grid = [];
        me.data.detail = {};
        me.aosgrid.adjust();
        me.aosgrid.clearAll();
        $('#btnCreatePORDS').hide();
        $('#pnlB').hide();

        $http.post('sp.api/SpInquiry/checkAOS')
            .success(function (data, status, headers, config) {
                if (data.success) {
                    //MsgConfirm("Terdapat data AOS apakah ingin lakukan proses?", function (result) {
                    //if (result) {
                    //$('#btnAOSLog').show();
                    //$('#btnCreatePORDS').show();
                    //$('#btnView').show();
                    //$('#pnlA').show();
                    //$('.title h3').text('AOS Alert');
                    me.data.grid = data.data;
                    me.loadTableData(me.aosgrid, me.data.grid);
                    me.aosgrid.adjust();
                    //       }
                    //    });
                    //} else {
                    //    $('#btnAOSLog').hide();
                    //    $('#btnCreatePORDS').hide();
                    //    $('#btnView').hide();
                    //    $('#pnlA').hide();
                    //    $('.title h3').text('');
                    //}
                    //}), 300
                }   
         });       
    };

    me.start();
}


$(document).ready(function () {
    

    var options = {
        title: "AOS Alert",
        xtype: "panels",
        toolbars: [
                    { name: "btnAOSLog", text: "AOS Log", icon: "icon-book", cls: "btn", click: "AOSLog()" },
                    { name: "btnCreatePORDS", text: "Create PORDS", icon: "icon-file", cls: "btn", click: "createPRODS()" },
                    { name: "btnView", text: "View", icon: "icon-print", cls: "btn", click: "doView()" }
        ],
        panels: [
                {
                    name: "pnlA",
                    title: "Data AOS",
                    items:[
                        {
                            name: "wxgrid",
                            type: "wxdiv"
                        }
                    ]
                },
                {
                    name: "pnlB",
                    title: "AOS LOG",
                    items: [
                            { name: "Bulan", text: "Bulan", cls: "span3  ", type: "select2", datasource: "PMonth" },
                            { name: "Tahun", text: "Tahun", cls: "span3" },
                            {
                                type: "buttons",
                                items: [
                                          { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-info", click: "doExport()" },
                                          { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "btn btn-info", click: "doUndo()"  },
                                       ]
                                },

                    ]
                }
        ],
    }

    function init(s) {
        SimDms.Angular("AOSAlert");
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    //function init(s) {
    //    SimDms.Angular("AOSAlert");
    //}

});