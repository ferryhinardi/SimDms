"use strict"

function svTransInputVORController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('options', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "VORBrowse",
            title: "VOR",
            manager: svServiceManager,
            query: "VORBrowse",
            defaultSort: "JobOrderNo asc",
            columns: [
                { field: "JobOrderNo", title: "No SPK", width: 120 },
                {
                    field: "CreatedDate", title: "Tgl. Delay", width: 120,
                    template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('DD MMM YYYY') #"
                },
                { field: "Customer", title: "Pelanggan" , width : 250},
                { field: "SA", title: "SA", width: 180 },
                { field: "FM", title: "Foreman", width: 180 },
                { field: "JobTypes", title: "Tipe Pekerjaan", width: 180 },
                { field: "JobReasonDesc", title: "Alasan Tunda" },
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#btnJobOrderNo').prop('disabled', true);
            
            me.loadDetail(data);
        });
    }

    me.browseJobOrder = function () {
        var lookup = Wx.blookup({
            name: "JobOrderBrowse",
            title: "Input Perawatan Kendaraan (SPK)",
            manager: svServiceManager,
            query: "JobOrderBrowse",
            defaultSort: "JobOrderNo desc",
            columns: [
                { field: "JobOrderNo", title: "No SPK", width: 120 },
                {
                    field: "JobOrderDate", title: "Tgl. SPK", width: 120,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "No. Polisi", width: 80 },
                { field: "ServiceBookNo", title: "No. Buku Service", width: 120 },
                { field: "BasicModel", title: "Model", width: 80 },
                { field: "TransmissionType", title: "Tipe Transmisi", width: 100 },
                { field: "ChassisCode + ChassisNo", title: "Kode Rangka", width: 100 },
                { field: "EngineCode + EngineNo", title: "Kode Mesin", width: 100 },
                { field: "ColorCode", title: "Warna", width: 80 },
                { field: "Pelanggan", title: "Pelanggan", width: 200 },
                { field: "Pembayar", title: "Pembayar", width: 200 },
                { field: "Odometer", title: "Odometer (KM)", width: 105 },
                { field: "JobType", title: "Jenis Pekerjaan", width: 110 },
                { field: "ForemanID", title: "Foreman", width: 85 },
                { field: "MechanicID", title: "Mekanik", width: 85 },
                { field: "ServiceStatus", title: "Status", width: 100 },
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            me.data.IsActive = true;
        });
    }

    me.browseJobDelay = function () {
        if (me.data.JobOrderNo == null) {
            MsgBox("Silahkan masukkan No SPK terlebih dahulu", MSG_ERROR);
        }
        else {
            var lookup = Wx.blookup({
                name: "JobDelayBrowse",
                title: "Job Delay",
                manager: svServiceManager,
                query: new breeze.EntityQuery.from("JobDelayBrowse").withParameters({JobOrderNo : me.data.JobOrderNo}),
                defaultSort: "JobDelayCode asc",
                columns: [
                    { field: "JobDelayCode", title: "Kode Job" },
                    { field: "JobDelayDesc", title: "Deskripsi" },
                ]
            });
            lookup.dblClick(function (data) {
                me.data.JobDelayCode = data.JobDelayCode;
                me.data.JobDelayDesc = data.JobDelayDesc;
                me.data.IsSparepart = data.JobDelayCode == "DELAY8";
                me.Apply();
            });
        }
    }

    me.POSNo = function () {
        var lookup = Wx.blookup({
            name: "POSNoBrowse",
            title: "PO",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from('POSNoBrowse').withParameters({ServiceNo : me.data.ServiceNo}),
            defaultSort: "POSNo desc",
            columns: [
                   { field: 'POSNo', title: 'No. POS' ,width:100},
            ]
        });
        lookup.dblClick(function (data) {
            me.dtlPart.POSNo = data.POSNo;
            me.Apply();
        });
    }

    me.PartNo = function () {
        if (me.dtlPart.POSNo == null || me.dtlPart.POSNo == undefined) {
            MsgBox("Silahkan isi No PO terlebih dahulu", MSG_ERROR);
        }
        else {
            var lookup = Wx.blookup({
                name: "PartNoBrowse",
                title: "PO",
                manager: svServiceManager,
                query: new breeze.EntityQuery.from('PartNoBrowse').withParameters({ POSNo: me.dtlPart.POSNo }),
                defaultSort: "PartNo asc",
                columns: [
                       { field: 'PartNo', title: 'No. Part' },
                       { field: 'PartName', title: 'Nama Part' },
                       { field: 'PartQty', title: 'Qty' },
                ]
            });
            lookup.dblClick(function (data) {
                me.dtlPart.PartNo = data.PartNo;
                me.dtlPart.PartName = data.PartName;
                me.dtlPart.PartQty = data.PartQty;
                me.Apply();
            });
        }
    }

    me.loadDetail = function (data) {
        $http.post('sv.api/inputvor/getgridvor', data)
        .success(function (e) {
            if (e.success) {
                if (data.IsSparepart && e.gridPart.length === 0)
                {
                    MsgBox('Informasi Part wajib diisi!!!', MSG_ERROR);
                }
                me.loadTableData(me.gridMechanic, e.gridMechanic);
                me.loadTableData(me.gridPart, e.gridPart);
                $('#btnPOSNo').prop('disabled', false);
                $('#btnPartNo').prop('disabled', false);
                me.dtlPart = {};
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.save = function () {
        $http.post('sv.api/inputvor/save', me.data)
       .success(function(e){
           if (e.success) {
               Wx.Success(e.message);
               me.loadDetail(e.record);
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function(e){ 
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.delete = function () {
        Wx.confirm("Apakah Anda Yakin???", function (e) {
            if (e === 'Yes') {
                $http.post('sv.api/inputvor/delete', me.data)
                    .success(function (e) {
                        if (e.success) {
                            Wx.Success(e.message);
                            me.init();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    }

    me.savePart = function () {
        if (me.dtlPart.POSNo == undefined || me.dtlPart.PartNo == undefined) {
            MsgBox('Ada Informasi yang belum lengkap', MSG_ERROR);
        }
        else if (me.dtlPart.PartQty <= 0 || me.dtlPart.PartQty == undefined) {
            MsgBox('Qty Part tidak boleh lebih kecil atau sama dengan nol', MSG_ERROR);
        }
        else {
            $http.post('sv.api/inputvor/savepart', { model: me.data, partModel: me.dtlPart })
                .success(function (e) {
                    if (e.success) {
                        Wx.Success(e.message);
                        me.loadDetail(me.data);
                    } else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    }

    me.deletePart = function () {
        Wx.confirm("Apakah Anda Yakin???", function (e) {
            if (e === 'Yes') {
                $http.post('sv.api/inputvor/deletepart', { model: me.data, partModel: me.dtlPart })
                    .success(function (e) {
                        if (e.success) {
                            Wx.Success(e.message);
                            me.loadDetail(me.data);
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    }

    me.printModel = function () {
         BootstrapDialog.show({
             message: $(
                 '<div class="container">' +
                 '<div class="row">' +

                 '<input type="checkbox" id="chkDate" style="margin-left: 15px" onclick=\'handleClick(this)\'> Tgl. Delay &nbsp&nbsp' +
                
                 '<input id="StartDate" type=\"text\" data-type=\"kdatepicker\" placeholder=\"dd-MMM-yyyy\" disabled=true>&nbsp s/d &nbsp' +
                 '<input id="EndDate" type=\"text\" data-type=\"kdatepicker\" placeholder=\"dd-MMM-yyyy\" disabled=true>' +

                 '</div>' +
                 '<div class="row" id="lblWIPCount" style="margin-bottom: -29px;margin-top: 15px;display:none">' +
                 '<label style="color:red">Terdapat WIP lebih dari 3 hari !!</label>'+
                 '</div>' +
                 '</div>' +

                 '<script type=\"text/javascript\">'+
                  'function handleClick(cb) {'+
                      'if(cb.checked) {$("[data-type=kdatepicker]").prop("disabled", false)}'+
                       'else { $("[data-type=kdatepicker]").prop("disabled", true)} }' +
                        '</script>'
                      ),
                   closable: false,
                   draggable: true,
                   type: BootstrapDialog.TYPE_INFO,
                   title: 'Print',
                   buttons: [{
                       label: ' Print',
                       cssClass: 'btn-primary icon-print',
                       action: function (dialogRef) {
                           //me.Print();
                           me.exportToXLS();
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
         setTimeout(function () {
             $("[data-type=kdatepicker]").kendoDatePicker({ format: "dd-MMM-yyyy" });
             $("#StartDate").data("kendoDatePicker").value(new Date());
             $("#EndDate").data("kendoDatePicker").value(new Date());

             var par = {
                 startDate: $("#StartDate").val(),
                 endDate: $("#EndDate").val(),
             };

             $.post('sv.api/inputvor/countwip', par)
                 .success(function (e) {
                     if (!e.success) {
                         $('#lblWIPCount').hide();
                     } else {
                         $('#lblWIPCount').show();
                     }
                 })
                 .error(function (e) {
                     MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                 });

             $("#StartDate, #EndDate").on('change', function () {

                 var par = {
                     startDate: $("#StartDate").val(),
                     endDate: $("#EndDate").val(),
                 };

                 $.post('sv.api/inputvor/countwip', par)
                     .success(function (e) {
                         if (!e.success) {
                             $('#lblWIPCount').hide();
                         } else {
                             $('#lblWIPCount').show();
                         }
                     })
                     .error(function (e) {
                         MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                     });

             });

         }, 200)
    }

    me.NoVOR = function () {
        if ($('#btnNoVOR').text() == 'Tidak ada VOR') {
            $http.post('sv.api/inputvor/novor', { novor: 1 })
            .success(function (e) {
                $('div.gl-widget').css('display', 'none');
                $('.main.animated').append('<div id="divNoVOR" class="gl-widget" disabled="disabled" "=""><label style="font-size: xx-large;' +
                  '">TIDAK ADA VOR HARI INI</label>' +
                  '</div>');

                $('#btnNoVOR').text('Ada VOR');
                $('#btnNoVOR').prepend('<i class="icon icon-check"></i>')
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });

        } else {
            $http.post('sv.api/inputvor/novor', { novor: 0 })
            .success(function (e) {
                $('#divNoVOR').remove();
                $('div.gl-widget').css('display', 'block');
                $('#btnNoVOR').text('Tidak ada VOR');
                $('#btnNoVOR').prepend('<i class="icon icon-remove"></i>');
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    }

    me.Print = function () {
        var par = {
            startDate: $("#StartDate").val(),
            endDate: $("#EndDate").val(),
        };

        Wx.XlsxReport({
            url: 'sv.api/inputvor/print',
            type: 'xlsx',
            params: par
        });
    }

    me.exportToXLS = function () {
        var par = {
            startDate: $("#StartDate").val(),
            endDate: $("#EndDate").val(),
        };

        $('.page > .ajax-loader').show();

        $.fileDownload('sv.api/inputvor/print1', {
            httpMethod: "POST",
            //preparingMessageHtml: "We are preparing your report, please wait...",
            //failMessageHtml: "There was a problem generating your report, please try again.",
            data: par
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });
    }

    me.gridMechanic = new webix.ui({
        container: "wxMechanic",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "MechanicID", header: "NIK", fillspace: true },
            { id: "MechanicName", header: "Mekanik", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                //if (me.gridMechanic.getSelectedId() !== undefined) {
                //    me.dtlPart = this.getItem(me.gridMechanic.getSelectedId().id);
                //    me.Apply();
                //}
            }
        }
    });

    me.gridPart = new webix.ui({
        container: "wxPart",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SeqNo", header: "No", width: 50 },
            { id: "POSNo", header: "No. POS", fillspace: true },
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartName", header: "Deskripsi", fillspace: true },
            { id: "PartQty", header: "Qty", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPart.getSelectedId() !== undefined) {
                    me.dtlPart = this.getItem(me.gridPart.getSelectedId().id);
                    me.dtlPart.old = me.dtlPart.PartNo;
                    me.Apply();
                }
            }
        }
    });

    me.initialize = function () {
        me.dtlPart = {};
        me.data.IsActive = true;
        me.clearTable(me.gridMechanic);
        me.clearTable(me.gridPart);

        $('#btnJobOrderNo').prop('disabled', false);
        $('#btnPOSNo').prop('disabled', true);
        $('#btnPartNo').prop('disabled', true);
        
    }

    me.OnTabChange = function (e, id) {
        me.gridMechanic.adjust();
        me.gridPart.adjust();
    };

    me.options = "Mechanic";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Input VOR",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnNoVOR", text: "Tidak ada VOR", cls: "btn btn-success", icon: "icon-remove", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "NoVOR()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    //{ name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printModel()" },
                    { name: "btnExportXLS", text: "Print", cls: "btn btn-success", icon: "icon-print", click: "printModel()" },
        ],
        panels: [
            {
                name: "pnlInputVOR",
                items: [
                    { name: "JobOrderNo", required: true, text: "No. SPK", cls: "span4", type: "popup", readonly: true, click: "browseJobOrder()" },
                    { name: "JobOrderDate", text: "Tgl. SPK", cls: "span4", readonly: true, type: "ng-datetimepicker" },
                    {
                        text: "Kode Delay Job",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "JobDelayCode", cls: "span2", placeHolder: "Kode Delay", readonly: true, type: "popup", click: "browseJobDelay()" },
                            { name: "JobDelayDesc", cls: "span6", placeHolder: "Deskripsi Delay", readonly: true }
                        ]
                    },
                    { name: "JobReasonDesc", text: "Alasan Delay", type: "textarea", required: true },
                    { name: "ClosedDate", text: "Tgl. Selesai", cls: "span4", readonly: true, type: "ng-datepicker" },
                    { name: "IsSparepart", text: "IsSparepart", cls: "span2", type: "x-switch", style: "margin-bottom:15px" },
                    { name: "IsActive", text: "IsActive", cls: "span2", type: "x-switch", style: "margin-bottom:15px" },
                    { name: "PoliceRegNo", text: "No. Polisi", cls: "span3", readonly: true },
                    {
                        text: "Basic Model / Trans ",
                        type: "controls",
                        cls: "span3",
                        items: [
                            { name: "BasicModel", cls: "span6", placeHolder: "Basic Model", readonly: true },
                            { name: "TransmissionType", cls: "span2", placeHolder: "Trans", readonly: true }
                        ]
                    },
                    { name: "Odometer", text: "KM (Odometer)", cls: "span2", readonly: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", readonly: true },
                            { name: "CustomerName", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Jenis Pekerjaan",
                        type: "controls",
                        items: [
                            { name: "JobType", cls: "span2", readonly: true },
                            { name: "JobTypeDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "SA",
                        type: "controls",
                        items: [
                            { name: "MechanicID", cls: "span2", readonly: true },
                            { name: "MechanicName", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "FM",
                        type: "controls",
                        items: [
                            { name: "ForemanID", cls: "span2", readonly: true },
                            { name: "ForemanName", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                ]
            },
            {
                name: "tabpageVOR",
                xtype: "tabs",
                model: "options",
                items: [
                    { name: "Mechanic", text: "Informasi Mekanik" },
                    { name: "Part", text: "Informasi Part" }
                ]
            },
             {
                 name: "Mechanic",
                 //title: "Informasi Mekanik",
                 cls: "tabpageVOR Mechanic active",
                 items: [
                      {
                          name: "wxMechanic",
                          type: "wxdiv"
                      }
                 ]
             },
             {
                 name: "Part",
                 //title: "Informasi Part",
                 cls: "tabpageVOR Part",
                 items: [
                     { name: "POSNo", model: "dtlPart.POSNo", text: "No. POS", cls: "span4", placeHolder: "No. POS", type: "popup", readonly: true, click: "POSNo()" },
                     {
                         text: "No. Part",
                         type: "controls",
                         cls:'span6',
                         items: [
                                 { name: "PartNo", model: "dtlPart.PartNo", cls: "span2", placeHolder: "No. Part", type: "popup", readonly: true, click: "PartNo()" },
                                 { name: "PartName",model:"dtlPart.PartName", cls: "span6", placeHolder: "Deskripsi", readonly: true },
                         ]
                     },
                     { name: "PartQty",model:"dtlPart.PartQty", text: "Qty", value: "0.00", cls: "span2 number" },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAddPart", text: "Add Part", icon: "icon-plus", cls: "btn btn-info", click: "savePart()", disable: "data.JobOrderNo === undefined" },
                                 { name: "btnDeletePart", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deletePart()", disable: "dtlPart.old === undefined" },
                         ]
                     },
                     {
                         name: "wxPart",
                         type: "wxdiv"
                     }
                 ]
             },
        ]
    };
javascript:void(0)
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svTransInputVORController");
    }
});