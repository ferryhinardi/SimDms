"use strict"

function omBPUController($scope, $http, $injector) {
    var me = $scope;
    var currentDate = moment().format();

    $injector.invoke(BaseController, this, { $scope: me });

    me.PoClick = function () {
        me.data.ReffNo = "";

        $("#btnNo b:first").attr("class", "fa fa-dot-circle-o");
        $("#btnReff b:first").attr("class", "fa fa-circle-o");
        $('#btnPONo').removeAttr('disabled');
        $('#btnReffNo').attr('disabled', 'disabled');
    };

    me.ReffClick = function () {
        $("#btnNo b:first").attr("class", "fa fa-circle-o");
        if (me.optionsPO_Reff == "PO_Reff1") {
            me.data.PONo = "";
        }

        $("#btnReff b:first").attr("class", "fa fa-dot-circle-o");
        $('#btnPONo').attr('disabled', 'disabled');
        $('#btnReffNo').removeAttr('disabled');
    };

    me.chkReffDODate_change = function () {
        (me.data.chkReffDODate) ? $("#RefferenceDODate").removeAttr("disabled") :
            $("#RefferenceDODate").attr("disabled", "disabled");
    }

    me.chkReffSJDate_change = function () {
        (me.data.chkReffSJDate) ? $("#RefferenceSJDate").removeAttr("disabled") :
            $("#RefferenceSJDate").attr("disabled", "disabled");
    }

    //LookUp
    me.lookupReffDO = function () {
        var onCase = 0;
        var url = "om.api/bpu/LookUpDO";
        var params = {};
        var colums;
        var sortColums;
        if (me.data.Tipe == "DO_SJ" || me.data.TipeSJ_BOOKING == "SJ_BOOKING") {
            onCase = 1;
            url = "om.api/bpu/LookUpDO_DOSJorSJBOOKING";
            params = {
                tipe: me.data.Tipe
            };

            sortColums = [
                { 'field': 'BatchNo', 'dir': 'asc' },
                { 'field': 'DONo', 'dir': 'asc' },
                { 'field': 'SKPNo', 'dir': 'asc' }
            ];

            columns = [
                { field: "BatchNo", title: "No Batch", width: 150 },
                { field: "DONo", title: "No. DO", width: 150 },
                {
                    field: "DODate", title: "Tgl. Reff DO", width: 130,
                    template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                },
                { field: "SJNo", title: "No. Reff SJ", width: 150 },
                {
                    field: "SJDate", title: "Tgl. Reff SJ", width: 130,
                    template: "#= (SJDate == undefined) ? '' : moment(SJDate).format('DD MMM YYYY') #"
                },
                { field: "SKPNo", title: "No. SKP", width: 150 },
                { field: "PONo", title: "No. PO", width: 150 },
                { field: "SupplierCode", title: "Pemasok", width: 100 },
                { field: "DealerName", title: "Nama Pemasok", width: 200 },
                { field: "ShipTo", title: "Kirim ke", width: 200 }
            ];
        }
        else if (me.data.Tipe == "SJ" && me.optionsTrans == "Upload") {
            onCase = 2;
            url = "om.api/bpu/LookupDO_SJandUpload";

            sortColums = [
                { 'field': 'BatchNo', 'dir': 'asc' },
                { 'field': 'DONo', 'dir': 'asc' },
                { 'field': 'SKPNo', 'dir': 'asc' }
            ];

            columns = [
                { field: "BatchNo", title: "No Batch", width: 150 },
                { field: "SJNo", title: "No. Reff SJ", width: 150 },
                {
                    field: "SJDate", title: "Tgl. Reff SJ", width: 130,
                    template: "#= (SJDate == undefined) ? '' : moment(SJDate).format('DD MMM YYYY') #"
                },
                { field: "SKPNo", title: "No. SKP", width: 150 },
                { field: "DONo", title: "No. DO", width: 150 },
                {
                    field: "DODate", title: "Tgl. Reff DO", width: 130,
                    template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Pemasok", width: 100 },
                { field: "DealerName", title: "Nama Pemasok", width: 200 },
                { field: "ShipTo", title: "Kirim ke", width: 200 }
            ];
        }
        else {
            sortColums = [
                { 'field': 'BatchNo', 'dir': 'asc' },
                { 'field': 'DONo', 'dir': 'asc' },
                { 'field': 'SKPNo', 'dir': 'asc' }
            ];

            var columns = [
            { field: "BatchNo", title: "No Batch", width: 150 },
            { field: "DONo", title: "No. DO", width: 150 },
            {
                field: "DODate", title: "Tgl. Reff DO", width: 130,
                template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
            },
            { field: "PONo", title: "No. PO", width: 150 },
            { field: "SupplierCode", title: "Pemasok", width: 100 },
            { field: "DealerName", title: "Nama Pemasok", width: 200 },
            { field: "ShipTo", title: "Kirim ke", width: 200 }
            ];
        }
        var lookup = Wx.klookup({
            name: "lookupDO",
            title: "Reff. DO",
            url: url,
            params: params,
            serverBinding: true,
            pageSize: 10,
            sort: sortColums,
            columns: columns
        });
        lookup.dblClick(function (data) {
            data.RefferenceDONo = data.DONo;
            data.RefferenceDODate = data.DODate;
            data.DealerCode = data.SupplierCode;
            me.populateData(data);

            $http.post("om.api/bpu/SupplierName", { PONo: data.PONo })
            .success(function (result) {
                if (result.success) {
                    me.populateData(result);
                }
                else {
                    MsgBox(result.message, MSG_WARNING);
                }
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });


            if (onCase == 0) {
                me.data.chkReffDODate = true;
            }

            if (onCase == 1) {
                me.data.chkReffDODate = true;
                me.data.chkReffSJDate = true;
            }
            else if (onCase == 2) {
                if (me.data.BPUDate != null) {
                    params = {
                        PONo: data.PONo,
                        DONo: data.DONo
                    }
                    $http.post("om.api/bpu/GetRecord", params)
                    .success(function (result) {
                        if (result.success) {
                            me.populateRecord(result, data);
                            var tipe = {
                                Tipe: "SJ"
                            }
                            me.populateData(tipe);
                            me.optionsTrans = "Upload";
                            me.isTrans1 = false;
                            me.isTrans2 = false;
                            me.chkReffDODate_change();

                            //me.loadTableData(me.gridDetailBPU, result.dataDtl);
                        }
                        else {
                            MsgBox(result.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
            else {
                me.data.chkReffDODate = true;
                me.chkReffDODate_change();
            }
        });
    }

    me.lookupReffSJ = function () {
        var isSJ = me.isSJ;
        var bpuSJDate = {
           BPUSJDate: me.data.BPUSJDate
        };

        var dtpBPUSJDisabled = true;
        if (isSJ) {
            if (!$("#BPUSJDate").hasClass("ng-hide k-input")) {
                dtpBPUSJDisabled = ($("#BPUSJDate").attr("disabled") ? true : false);
                //$("#BPUDate").attr("disabled", "disabled");
            }
        }

        var onCase = onCase;
        var url = "om.api/bpu/LookUpSJ_True";
        var params = {};
        var columns = [
            { field: "BatchNo", title: "No Batch", width: 150 },
            { field: "DONo", title: "No. DO", width: 150 },
            {
                field: "DODate", title: "Tgl. Reff DO", width: 130,
                template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
            },
            { field: "SJNo", title: "No. Reff SJ", width: 150 },
            {
                field: "SJDate", title: "Tgl. Reff SJ", width: 130,
                template: "#= (SJDate == undefined) ? '' : moment(SJDate).format('DD MMM YYYY') #"
            },
            { field: "SKPNo", title: "No. SKP", width: 150 },
            { field: "PONo", title: "No. PO", width: 150 },
            { field: "SupplierCode", title: "Pemasok", width: 100 },
            { field: "DealerName", title: "Nama Pemasok", width: 200 },
            { field: "ShipTo", title: "Kirim ke", width: 200 }
        ];

        var sortColums = [
                { 'field': 'FlagRevisi', 'dir': 'desc' },
                { 'field': 'BatchNo', 'dir': 'asc' },
                { 'field': 'SJNo', 'dir': 'asc' },
                { 'field': 'SKPNo', 'dir': 'asc' }
        ];

        if (me.data.Tipe != "DO") {
            if (me.data.Tipe == "SJ") {
                onCase = 1;
                sortColums = [
                    { 'field': 'FlagRevisi', 'dir': 'desc' },
                    { 'field': 'BatchNo', 'dir': 'asc' },
                    { 'field': 'SJNo', 'dir': 'asc' },
                    { 'field': 'SKPNo', 'dir': 'asc' }
                ];

                columns = [
                    { field: "FlagRevisi", title: "Revisi", width: 150 },
                    { field: "BatchNo", title: "No Batch", width: 150 },
                    { field: "SJNo", title: "No. Reff SJ", width: 150 },
                    {
                        field: "SJDate", title: "Tgl. Reff SJ", width: 130,
                        template: "#= (SJDate == undefined) ? '' : moment(SJDate).format('DD MMM YYYY') #"
                    },
                    { field: "SKPNo", title: "No. SKP", width: 150 },
                    { field: "DONo", title: "No. DO", width: 150 },
                    {
                        field: "DODate", title: "Tgl. Reff DO", width: 130,
                        template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #"
                    },
                    { field: "SupplierCode", title: "Pemasok", width: 100 },
                    { field: "DealerName", title: "Nama Pemasok", width: 200 },
                    { field: "ShipTo", title: "Kirim ke", width: 200 }
                ];
            }
            else {
                if (me.data.Tipe == "DO_SJ") {
                    url = "om.api/bpu/LookUpSJ_False";
                    sortColums = [
                        { 'field': 'FlagRevisi', 'dir': 'desc' },
                        { 'field': 'BatchNo', 'dir': 'asc' },
                        { 'field': 'SJNo', 'dir': 'asc' },
                        { 'field': 'SKPNo', 'dir': 'asc' }
                    ];
                }
                else if (me.data.Tipe == "SJ_BOOKING") {
                    url = "om.api/bpu/LookUpSJ_BOOKING";

                    sortColums = [
                        { 'field': 'BatchNo', 'dir': 'asc' },
                        { 'field': 'SJNo', 'dir': 'asc' },
                        { 'field': 'SKPNo', 'dir': 'asc' }
                    ];
                }
            }
        }

        var lookup = Wx.klookup({
            name: "lookupSJ",
            title: "Reff. SJ",
            url: url,
            params: params,
            serverBinding: true,
            pageSize: 10,
            sort: sortColums,
            columns: columns
        });
        lookup.dblClick(function (data) {
                data.RefferenceSJNo = data.SJNo;
                data.RefferenceSJDate = data.SJDate;
                data.DealerCode = data.SupplierCode;
                me.populateData(data);
                if (onCase == 1 && me.optionsTrans == "Upload") {
                    if (me.data.BPUDate != null) {
                        params = {
                            PONo: data.PONo,
                            DONo: data.DONo
                        }
                        $http.post("om.api/bpu/GetRecord", params)
                        .success(function (result) {
                            if (result.success) {
                                me.populateRecord(result, data);
                                me.isSJ = isSJ;
                                $("#BPUSJDate").prop("disabled", dtpBPUSJDisabled);
                                $("#BPUDate").prop("disabled", !dtpBPUSJDisabled);
                                me.populateData(bpuSJDate);

                                me.isTrans1 = false;
                                me.isTrans2 = false;
                                if (result.data.Status == 2) {
                                    $("#btnRefferenceDONo, #RefferenceSJNo, #btnRefferenceSJNo, #chkReffSJDate").removeAttr("disabled");
                                    me.data.chkReffSJDate = true;
                                }
                                var tipe = {
                                    Tipe: "SJ"
                                }
                                me.populateData(tipe);
                                me.optionsTrans = "Upload";
                                me.chkReffSJDate_change();
                            }
                            else {
                                MsgBox(result.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
                    }
                }
                else {
                    me.data.chkReffDODate = true;
                    me.data.chkReffSJDate = true;
                    me.chkReffSJDate_change();
                }
        });
    }

    me.lookupPONo = function () {
        var lookup = Wx.klookup({
            name: "lookupPO",
            title: "Purchase Order",
            url: "om.api/bpu/LookupPONo",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PONo", title: "No. PO", width: 150 },
                { field: "SupplierName", title: "Pemasok", width: 200 },
                { field: "ShipTo", title: "Kirim Ke", width: 250 },
                { field: "SupplierCode", title: "Kode Pemasok.", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateData(data);
        });
    }

    me.lookupReffNo = function () {
        var lookup = Wx.klookup({
            name: "lookupPO",
            title: "Purchase Order",
            url: "om.api/bpu/LookupReffNo",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "ReffNo", title: "No. Reff", width: 150 },
                { field: "PONo", title: "No. PO", width: 150 },
                { field: "SupplierName", title: "Pemasok", width: 200 },
                { field: "ShipTo", title: "Kirim Ke", width: 250 },
                { field: "SupplierCode", title: "Kode Pemasok.", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateData(data);
        });
    }

    //me.lookupReffNo = function () {
    //    var lookup = Wx.klookup({
    //        name: "lookupReffNo",
    //        title: "Refference No",
    //        url: "om.api/bpu/LookupReffNo",
    //        serverBinding: true,
    //        pageSize: 10,
    //        columns: [
    //            { field: "ReffNo", title: "No. Reff", width: 150 },
    //            { field: "PONo", title: "No. PO", width: 150 },
    //            { field: "SupplierName", title: "Pemasok", width: 200 },
    //            { field: "ShipTo", title: "Kirim Ke", width: 250 },
    //            { field: "SupplierCode", title: "Kode Pemasok.", width: 150 }
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        me.populateData(data);
    //    });
    //}

    me.lookupExpedition = function () {
        var lookup = Wx.klookup({
            name: "lookupExped",
            title: "Ekspedisi",
            url: "om.api/bpu/LookupExpedition",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "Expedition", title: "Kode", width: 150 },
                { field: "ExpeditionName", title: "Ekspedisi", width: 200 },
            ]
        });
        lookup.dblClick(function (data) {
            me.populateData(data);
        });
    }

    me.lookupWarehouse = function () {
        var params = {
            Tipe: me.data.Tipe
        };

        var lookup = Wx.klookup({
            name: "lookupWH",
            title: "Gudang",
            url: "om.api/bpu/LookupWarehouse",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "WarehouseCode", title: "Kode", width: 150 },
                { field: "WarehouseName", title: "Nama", width: 200 },
                { field: "RefferenceDesc2", title: "Keterangan", width: 250 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateData(data);
        });
    }

    me.lookupModel = function () {
        var params = {
            PONo: me.data.PONo
        };

        var lookup = Wx.klookup({
            name: "lookupModel",
            title: "Model",
            url: "om.api/bpu/LookupModel",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code", width: 150 },
                { field: "EngineCode", title: "Kode Mesin", width: 200 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateDetailLookup(data);
        });
    }

    me.lookupModelYear = function () {
        var params = {
            PONo: me.data.PONo,
            SalesModelCode: me.detail.SalesModelCode
        };

        var lookup = Wx.klookup({
            name: "lookupModelYear",
            title: "Model Year",
            url: "om.api/bpu/LookupModelYear",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SalesModelYear", title: "Sales Model Year", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi", width: 200 },
                { field: "ChassisCode", title: "Kode Rangka", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateDetailLookup(data);
        });
    }

    me.lookupColour = function () {
        var params = {
            SalesModelCode: me.detail.SalesModelCode
        };

        var lookup = Wx.klookup({
            name: "lookupColour",
            title: "Warna",
            url: "om.api/bpu/LookupColour",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "ColourCode", title: "Kode Warna", width: 150 },
                { field: "ColourName", title: "Nama Warna", width: 200 }
            ]
        });
        lookup.dblClick(function (data) {
            me.populateDetailLookup(data);
        });
    };
    //

    // LOGIC
    me.saveData = function () {
        me.data.BPUType = me.data.Tipe;
        var dtpBPUSJEnable = false;
        if(!$("#BPUSJDate").hasClass("ng-hide k-input")){
            dtpBPUSJEnable = ($("#BPUSJDate").attr("disabled") ? false : true);
        }

        var params = {
            model: me.data,
            optionsTrans: me.optionsTrans,
            dtpBPUEnable: ($("#BPUDate").attr('disabled') == undefined) ? true : false,
            dtpBPUSJEnable: dtpBPUSJEnable,
            chkReffDODate: me.data.chkReffDODate,
            chkReffSJDate: me.data.chkReffSJDate
        };

        $http.post("om.api/bpu/Save", params)
        .success(function (result) {
            if (result.success) {
                me.data.BPUNo = result.BPUNo;
                me.getBPU(me.data);
                me.setViewByStatus(result.BPUStatus);
                Wx.Success(result.message);
            }
            else {
                MsgBox(result.message, MSG_WARNING);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.saveDetail = function (params, data) {
        var bpuTipe = me.data.Tipe;
        var rbSJ = (bpuTipe == "SJ") ? true : false;
        var rbDO = (bpuTipe == "DO") ? true : false;
        var rbDOSJ = (bpuTipe == "DO_SJ") ? true : false;
        var rbSJBooking = (bpuTipe == "SJ_BOOKING") ? true : false;
        if (rbDO) {
            var standardCode = "";
            if (data.StandardCode != null) {
                standardCode = data.StandardCode;
            }
            if (standardCode == "2000000" && data.DOCount > 0) {
                MsgBox("Untuk Pemasok [" + me.data.SupplierName + "] hanya bisa 1 DO 1 Unit", MSG_WARNING);
                return;
            }
        }

        if (rbDO == false) {
            if (me.detail.ChassisNo == "" | me.detail.ChassisNo == "0") {
                MsgBox("No.Rangka harus diisi!", MSG_WARNING);
                $("#ChassisNo").focus();
                return;
            }

            if (me.detail.EngineNo == "" | me.detail.EngineNo == "0") {
                MsgBox("No.Mesin harus diisi!", MSG_WARNING);
                $("#EngineNo").focus();
                return;
            }

            if (me.detail.ServiceBookNo == "") {
                MsgBox("No.Buku Service harus diisi!", MSG_WARNING);
                $("#ServiceBookNo").focus();
                return;
            }

            if (data.VehicleStatus == "6") {
                Wx.confirm("Unit ini pernah dibeli dan dijual, apakah ingin dibeli kembali?", function (e) {
                    if (e == "YES")
                        me.data.isBuyBack = true;
                    else me.data.isBuyBack = false;
                });
            }

            if (!me.data.isBuyBack) {
                if (me.detail.ChassisNo != me.xChassisNo) {
                    if (data.isExistsChassisNo) {
                        MsgBox("No.Rangka sudah ada", MSG_WARNING);
                        $("#ChassisNo").focus();
                        return;
                    }
                }

                if (me.detail.EngineNo != me.xEngineNo) {
                    if (data.isExistsEngineNo) {
                        XMessageBox.ShowWarning("No.Mesin sudah ada");
                        $("#EngineNo").focus();
                        return;
                    }
                }
            }

            if (me.IsCheckChassis) {
                if (data.StandardCode == "2000000") {
                    if (data.EngineNo != "") {
                        if (me.detail.EngineNo != data.EngineNo) {
                            MsgBox("No. Mesin yang diinput tidak sesuai dengan data dari PT. SIS", MSG_WARNING);
                            return;
                        }
                    }
                    else {
                        MsgBox("Data PT. SIS tidak ada silahkan upload dahulu data dari DCS.", MSG_WARNING);
                        return;
                    }
                }
            }
        }
        if (rbSJ) {
            if (me.data.RefferenceSJNo == "") {
                MsgBox("No.Reff.SJ harus diisi", MSG_WARNING);
                $("#RefferenceSJNo").focus();
                return;
            }
            if (me.data.chkReffSJDate == false) {
                XMessageBox.ShowWarning("Tanggal Reff.SJ harus diisi");
                return;
            }
        }

        $http.post("om.api/bpu/savedetail", params)
        .success(function (result) {
            if (result.success) {
                me.resetDetail();
                me.loadTableData(me.gridDetailBPU, result.dataDtl);
                Wx.Success(result.message);
            }
            else {
                MsgBox(result.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    };

    me.saveDetailValidate = function () {
        if ($(".main form").valid()) {
            // cek colour from PO
            var bpuTipe = me.data.Tipe;
            var rbDOSJ = (bpuTipe == "DO_SJ") ? true : false;
            var rbDO = (bpuTipe == "DO") ? true : false;

            me.detail.BPUNo = me.data.BPUNo;
            var params = {
                model: me.data,
                modelDtl: me.detail
            };
            $http.post("om.api/bpu/SaveDetailValidate", params)
            .success(function (result) {
                if (result.success) {
                    var data = result.data;
                    if (data.POModelColour == null || data.POModelColour == undefined) {
                        var msgConfir = "";
                        if (rbDOSJ && data.ProductType == "2W")
                            msgConfir = "Tahun atau warna tidak ada di PO, Lanjut atau tidak?";
                        else
                            msgConfir = "Warna tidak ada di PO, Lanjut atau tidak?";
                        Wx.confirm(msgConfir, function (e) {
                            if (e === "Yes") {
                                if (rbDO) { $("#RemarkDetail").focus(); }
                                else { $("#ChassisNo").focus(); }
                                me.saveDetail(params, data);
                            }
                            else {
                                if (!rbDOSJ && data.ProductType != "2W")
                                    me.lookupColour();
                                return;
                            }
                        });
                    }
                    else{
                        me.saveDetail(params, data);
                    }
                }
                else {
                    MsgBox(result.message, MSG_ERROR);
                }
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    };

    me.delete = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e == true) {
                var params = {
                    PONo: me.data.PONo,
                    BPUNo: me.data.BPUNo
                };
                $http.post("om.api/bpu/delete", params)
                .success(function (result) {
                    if (result.success) {
                        $http.post('om.api/bpu/GetBPU', params)
                        .success(function (result) {
                            if (result.success) {
                                me.populateRecord(result, me.data);
                            }
                            else {
                                MsgBox(result.message, MSG_ERROR);
                            }
                        })
                        .error(function (result) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
                        me.ModelYearOld = "";
                        Wx.Success(result.message);
                    }
                    else {
                        MsgBox(result.message, MSG_ERROR)
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {

            }
        })
    };

    me.deleteDetail = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e == true) {
                var bpuTipe = me.data.Tipe;
                var params = {
                    modelDtl: me.detail,
                    rbSJ: (bpuTipe == "SJ") ? true : false
                }
                $http.post("om.api/bpu/deletedetail", params)
                .success(function (result) {
                    if (result.success) {
                        var params = {
                            pONo: me.data.PONo,
                            bPUNo: me.data.BPUNo
                        }
                        $http.post('om.api/bpu/GetBPU', params)
                        .success(function (result) {
                            if (result.success) {
                                me.populateRecord(result, me.data);
                            }
                            else {
                                MsgBox(result.message, MSG_ERROR);
                            }
                        })
                        .error(function (result) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
                        me.ModelYearOld = "";
                        Wx.Success(result.message);
                    }
                    else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {
                me.resetDetail();
                me.Apply();
                me.gridDetailBPU.clearSelection();
            }
        });
    }

    me.printPreview = function () {
        var params = {
            PONo: me.data.PONo,
            BPUNo: me.data.BPUNo
        };
        $http.post("om.api/bpu/print", params)
        .success(function (result) {
            if (result.success) {
                me.printPreviewShow();
                $http.post('om.api/bpu/GetBPU', params)
                .success(function (result) {
                    if (result.success) {
                        me.populateRecord(result, me.data);
                    }
                    else {
                        MsgBox(result.message, MSG_INFO);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
                Wx.Success(result.message);
            }
            else {
                MsgBox(result.message, MSG_ERROR)
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.approveRecord = function () {
        MsgConfirm("Apakah anda yakin???", function (e) {
            if (e == true) {
                var bpuTipe = me.data.Tipe;
                var params = {
                    PONo: me.data.PONo,
                    BPUNo: me.data.BPUNo,
                    rbDO: (bpuTipe === "DO") ? true : false, 
                    rbSJ: (bpuTipe === "SJ") ? true : false, 
                    isBuyBack: me.data.isBuyBack ? true : false, 
                    isConfirmBuyBack: false
                };
                $http.post("om.api/bpu/ApproveRecord", params)
                .success(function (result) {
                    if (result.success == undefined) {
                        MsgConfirm(result.message, function (e) {
                            if (e == true) {
                                me.data.isBuyBack = true;
                                var params = {
                                    PONo: me.data.PONo,
                                    BPUNo: me.data.BPUNo,
                                    rbDO: (bpuTipe === "DO") ? true : false, 
                                    rbSJ: (bpuTipe === "SJ") ? true : false, 
                                    isBuyBack: me.data.isBuyBack,
                                    isConfirmBuyBack: true
                                };
                                $http.post("om.api/bpu/ApproveRecord", params)
                                .success(function (result) {
                                    if (result.success) {
                                        me.getBPU(me.data);
                                        me.setViewByStatus(result.BPUStatus);
                                        if (me.status == "APPROVED") {
                                            if (me.isSJ) {
                                                me.allowEdit = false;
                                            }
                                        }
                                        MsgBox(result.message, MSG_SUCCESS);
                                    }
                                    else {
                                        MsgBox(result.message, MSG_WARNING);
                                    }
                                })
                                .error(function (e) {
                                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                });
                            }
                            else {
                                return;
                            }
                        })
                    }
                    else {
                        if (result.success) {
                            me.getBPU(me.data);
                            me.setViewByStatus(result.BPUStatus);
                            MsgBox(result.message, MSG_SUCCESS);
                        }
                        else {
                            MsgBox(result.message, MSG_WARNING);
                        }
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {
                me.resetDetail();
            }
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "BPULookup",
            title: "BPU",
            manager: spSalesManager,
            query: "BPUBrowse",
            defaultSort: "BPUNo desc",
            columns: [
                { field: "BPUNo", title: "No BPU", width: 130 },
                {
                    field: "BPUDate", title: "Tgl BPU", width: 130,
                    template: "#= (BPUDate == undefined) ? '' : moment(BPUDate).format('DD MMM YYYY') #"
                },

                { field: "RefferenceDONo", title: "No.Reff.DO", width: 130 },
                { field: "RefferenceSJNo", title: "No.Reff.SJ", width: 150 },
                { field: "PONo", title: "No.PO", width: 130 },
                { field: "SupplierName", title: "Pemasok", width: 300 },
                { field: "Tipe", title: "Tipe", width: 80 },
                { field: "StatusBPU", title: "Status", width: 130 }
            ]
        });
        lookup.dblClick(function (data) {
            me.getBPU(data);
        });
    }
    // EOF LOGIC

    me.printPreviewShow = function () {
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
            title: 'Print BPU',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    var sizeType = $('input[name=sizeType]:checked').val() === 'full';
                    var ReportId = (sizeType) ? 'OmRpPurTrn002' : 'OmRpPurTrn002A';

                    var par = [
                        me.data.BPUNo,
                        me.data.BPUNo,
                        '100',
                        ''
                    ]
                    var rparam = 'Print BPU'

                    Wx.showPdfReport({
                        id: ReportId,
                        pparam: par.join(','),
                        rparam: rparam,
                        type: "devex"
                    });

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
    }

    me.getBPU = function (data) {
        if (data != null) {
            var params = {
                pONo: data.PONo,
                bPUNo: data.BPUNo
            }
            $http.post('om.api/bpu/GetBPU', params)
           .success(function (result) {
               if (result.success) {
                   me.populateRecord(result, data);
                   $("#BPUDate").attr("disabled", "disabled");
               }
               else {
                   MsgBox(result.message, MSG_ERROR);
               }
           })
           .error(function (result) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    };

    me.loadDetail = function (data) {
        $http.post('om.api/po/getpo', data)
        .success(function (e) {
            if (e.success) {
                me.loadTableData(me.gridDetailBPU, e.grid);
            }
            else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.SetManual = function () {
        me.isManual = true;
        switch (me.status) {
            case "NEW":
                $("label[btn-radio=\"'SJ'\"]").attr("disabled", "disabled");
                if (me.isDO) {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo, #chkReffSJDate").attr("disabled", "disabled");
                }
                else {
                    if (me.isSJ) {
                        $("#btnRefferenceDONo, #btnRefferenceSJNo").attr("disabled", "disabled");
                    }
                    else {
                        $("#btnRefferenceDONo, #btnRefferenceSJNo").attr("disabled", "disabled");
                    }
                }
                $("label[for='divTglBPU']").html("Tgl. BPU");

                break;
            case "OPEN":
                if (me.isDO) {
                    $("#btnRefferenceDONo").attr("disabled", "disabled");
                }
                if (me.isSJ) {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo").attr("disabled", "disabled");
                }
                break;
            case "PRINTED":
                if (me.isDO) {
                    $("#btnRefferenceDONo").attr("disabled", "disabled");
                }
                if (me.isSJ) {
                    $("#btnRefferenceDONoXXX, #btnRefferenceSJNo").attr("disabled", "disabled");
                }
                break;
            case "APPROVED":
                $("#btnRefferenceSJNo").attr("disabled", "disabled");
                break;
        }
    };

    me.SetUpload = function () {
        me.isManual = false;
        switch (me.status) {
            case "NEW":
                $("label[btn-radio=\"'SJ'\"]").removeAttr("disabled");
                if (me.isDO) {
                    $("#btnRefferenceSJNo, #chkReffSJDate").attr("disabled", "disabled");
                    $("#btnRefferenceDONo").removeAttr("disabled");
                }
                else {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo, #chkReffSJDate").removeAttr("disabled");
                }

                if (me.isSJ) {
                    $("label[for='divTglBPU']").html("Tgl. BPU / Tgl. BPU SJ");
                }
                break;
            case "OPEN":
                if (me.isDO) {
                    $("#btnRefferenceDONo").removeAttr("disabled");
                    me.data.BPUSJDate = currentDate;
                    $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'DO_SJ'\"]").attr("disabled", "disabled");
                    $("label[btn-radio=\"'SJ'\"]").removeAttr("disabled");
                }
                if (me.isSJ) {
                    $("#btnRefferenceSJNo").removeAttr("disabled");
                }
                break;
            case "PRINTED":
                if (me.isDO) {
                    $("label[btn-radio=\"'SJ'\"], #btnRefferenceDONo").removeAttr("disabled");
                }
                if (me.isSJ) {
                    $("#btnRefferenceSJNo").removeAttr("disabled");
                    me.data.Remark = me.data.Remark + " ";
                }
                break;
            case "APPROVED":
                $("#btnRefferenceSJNo, #RefferenceSJNo, #chkReffSJDate, #BPUSJDate").removeAttr("disabled");
                break;
        }
    };

    me.SetDO = function () {
        me.isDO = true;
        me.isSJ = false;
        $("#BPUSJDate").attr("disabled", "disabled");
        $("#BPUDate").removeAttr("disabled");
        switch (me.status) {
            case "NEW":
                if (me.isManual) {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo, #RefferenceSJNo, #chkReffSJDate").attr("disabled", "disabled");
                }
                else {
                    $("#btnRefferenceDONo").removeAttr("disabled");
                    $("#btnRefferenceSJNo, #RefferenceSJNo, #chkReffSJDate").attr("disabled", "disabled");
                }
                $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'DO_SJ'\"], #btnNo label:first, #btnReff label:first").removeAttr("disabled");

                break;
            case "OPEN":
                break;
        }

        $("label[for='divTglBPU']").html("Tgl. BPU");
    };

    me.SetSJ = function () {
        me.isDO = false;
        me.isSJ = true;
        $("#BPUDate").attr("disabled", "disabled");
        $("#BPUSJDate").removeAttr("disabled");
        switch (me.status) {
            case "NEW":
                $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'DO_SJ'\"], #chkReffSJDate").removeAttr("disabled");
                $("#RefferenceSJNo").removeAttr("disabled");
                if (me.isManual) {
                    $("#btnRefferenceSJNo").attr("disabled", "disabled");
                }
                else {
                    $("#btnRefferenceSJNo").removeAttr("disabled");
                    //$("label[for='divTglBPU']").html("Tgl. BPU / Tgl. BPU SJ");
                }
                break;
            case "OPEN":
                //if (!me.data.chkReffSJDate) {
                $("#RefferenceSJDate, #chkReffDODate, #chkReffSJDate").attr("disabled", "disabled");
                if (me.isManual == false    ) {
                    $("#RefferenceSJNo, #btnRefferenceSJNo, #chkReffSJDate").removeAttr("disabled");
                }
                //}
                break;
            case "PRINTED":
                if (me.isManual == false) {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo, #RefferenceSJNo" +
                    ", #BPUSJDate, #chkReffSJDate").removeAttr("disabled");
                }
                else {
                    $("#btnRefferenceDONo, #btnRefferenceSJNo").attr("disabled", "disabled");
                    $("#RefferenceSJNo, #BPUSJDate, #chkReffSJDate").removeAttr("disabled");
                }
                me.data.BPUSJDate = currentDate;
                break;
            case "APPROVED":
                if (me.isManual) {
                    $("#btnRefferenceSJNo").attr("disabled");
                    $("#RefferenceSJNo, #chkReffSJDate, #BPUSJDate").removeAttr("disabled");
                    me.data.WarehouseCode = "";
                    me.data.WarehouseName = "";
                }
                else {
                    $("#btnRefferenceSJNo, #RefferenceSJNo, #chkReffSJDate, #BPUSJDate").removeAttr("disabled");
                }
                me.data.BPUSJDate = currentDate;
                break;
        }
    };

    me.SetDOSJ = function () {
        me.isDO = false;
        me.isSJ = false;
        $("#BPUSJDate").attr("disabled", "disabled");
        $("#BPUDate").removeAttr("disabled");
        switch (me.status) {
            case "NEW":
                if (me.isManual) {
                    $("#btnRefferenceDONo, #RefferenceSJNo").attr("disabled", "disabled");
                    $("#btnRefferenceSJNo, #chkReffSJDate, #RefferenceSJNo").removeAttr("disabled");
                }
                else {
                    $("#chkReffSJDate").attr("disabled", "disabled");
                    $("#btnRefferenceDONo, #btnRefferenceSJNo, #chkReffSJDate, #RefferenceSJNo").removeAttr("disabled");
                }
                $("label[for='divTglBPU']").html("Tgl. BPU");
                break;
            case "OPEN":
                break;
            case "PRINTED":
                break;
        }
    };

    me.RetrieveData = function (value) {
        if (value == null || value == undefined) return;

        me.isLoadData = true;
        setTimeout(function () {
            me.hasChanged = false;
            me.startEditing();
            me.isSave = true;

            me.ReformatNumber();
            var selectorContainer = "";
            $.each(value, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;

                ctrl.removeClass("error");
            });

            $scope.$apply();
            me.isSave = false;
        }, 200);
    };

    me.setViewByStatus = function (status, tipe) {
        var datTipe = {
            Tipe: (tipe === "DO & SJ") ? "DO_SJ" : tipe
        }

        me.RetrieveData(datTipe);
        switch (parseInt(status)) {
            case 0:
                me.status = "OPEN";
                me.allowApproval = false;
                me.allowEdit = true;
                me.allowInputDetail = true;
                me.allowDeleteDetail = true;

                if (me.data.Tipe === "SJ") {
                    me.data.Tipe = "DO";
                    me.data.WarehouseCode = "";
                    me.data.WarehouseName = "";
                }

                //if (me.isDO) {
                    $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'SJ'\"], label[btn-radio=\"'DO_SJ'\"]").attr("disabled", "disabled");
                    $("#RefferenceDONo, #RefferenceDODate, #chkReffDODate, #chkReffSJDate, #RefferenceSJNo, " +
                        "#PO_Reff1, #PO_Reff2, #btnNo label:first, #btnReff label:first, #btnPONo").attr("disabled", "disabled");
                //}

                break;
            case 1:

                me.status = "PRINTED";
                me.allowApproval = true;
                me.allowEdit = false;
                me.allowInputDetail = true;
                me.allowDeleteDetail = true;

                $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'SJ'\"], label[btn-radio=\"'DO_SJ'\"]").attr("disabled", "disabled");

                $("#RefferenceDONo, #RefferenceDODate, #BPUSJDate, #RefferenceSJNo, #RefferenceDODate, #RefferenceSJDate, #chkReffDODate, \
                    #chkReffSJDate, #PO_Reff1, #PO_Reff2 #btnNo label:first, #btnReff label:first, #btnPONo, #BPUSJDate").attr("disabled", "disabled");

                break;
            case 2:
                me.status = "APPROVED";
                me.allowApproval = false;
                me.allowEdit = false;
                me.allowInputDetail = false;
                me.allowDeleteDetail = false;

                $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'DO_SJ'\"]").attr("disabled", "disabled");
                $("label[btn-radio=\"'Manual'\"], label[btn-radio=\"'Upload'\"], label[btn-radio=\"'SJ'\"]").removeAttr("disabled");

                $("#RefferenceDONo, #RefferenceDODate, #BPUSJDate, #RefferenceSJNo, #RefferenceSJDate, #chkReffDODate \
                    , #chkReffSJDate, #PO_Reff1, #PO_Reff2, #btnNo label:first, #btnReff label:first, #btnPONo").attr("disabled", "disabled");


                break;
            case 3:
                me.status = "CANCELED";
                me.disableBPUType = true;
                me.allowApproval = false;
                me.allowEdit = false;
                me.allowInputDetail = false;
                me.allowDeleteDetail = false;

                $("label[btn-radio=\"'DO'\"], label[btn-radio=\"'SJ'\"], label[btn-radio=\"'DO_SJ'\"]" +
                    ", label[btn-radio='\'Manual\''], label[btn-radio='\'Upload\'']").attr("disabled", "disabled");

                break;
            case 9:
                me.status = "FINISHED";
                me.allowApproval = false;
                me.allowEdit = false;
                me.allowInputDetail = false;
                me.allowDeleteDetail = false;
                break;
        }

        $('#POStatus').html(me.status);
    }

    me.populateRecord = function (result, data) {
        me.isSJ = (data.Tipe === "SJ") ? true : false;
        me.lookupAfterSelect(result.data);
        me.data.DealerCode = data.SupplierCode;
        //me.data.SupplierCode = data.SupplierCode;
        me.data.SupplierName = data.SupplierName;

        if (me.dateFormat(result.data.RefferenceDODate) == "01 Jan 1900") {
            me.data.chkReffDODate = false;
            me.data.RefferenceDODate = me.now();
        }
        else {
            me.data.chkReffDODate = true;
            me.data.RefferenceDODate = result.data.RefferenceDODate;
        }

        if (me.dateFormat(result.data.RefferenceSJDate) == "01 Jan 1900") {
            me.data.chkReffSJDate = false;
            me.data.RefferenceSJDate = me.now();
        }
        else {
            me.data.chkReffSJDate = true;
            me.data.RefferenceSJDate = result.data.RefferenceSJDate;
            // me.data.BPUSJDate = val.BPUSJDate;
        }

        me.isManual = true;
        me.optionsTrans = "Manual";
        me.SetManual();

        if (result.data.Tipe === "DO") {
            me.isInProcess = false;
            me.SetDO();
        }
        else if (result.data.Tipe === "SJ") {
            if (result.data.Status == 2) {
                me.isInProcess = true;
            }
            else {
                me.isInProcess = false;
            }
            me.SetSJ();
        }
        else if (result.data.Tipe === "DO & SJ" || result.data.Tipe === "DO_SJ") {
            if (result.data.Status == 2) {
                me.isInProcess = true;
            }
            else {
                me.isInProcess = false;
            }
            me.SetDOSJ();
        }

        me.optionsPO_Reff = (result.data.PONo != "") ? "PO_Reff1" : "PO_Reff2";
        me.resetDetail();
        me.loadTableData(me.gridDetailBPU, result.dataDtl);

        me.setViewByStatus(result.data.Status, result.data.Tipe);
        if (result.dataDtl != null) {
            if (result.dataDtl.length < 1) {
                $("#RefferenceDONo, #RefferenceSJNo, #chkReffDODate, #chkReffSJDate, #btnNo label:first, #btnReff label:first").removeAttr("disabled");
                me.chkReffDODate_change();
            }
        }
    };

    me.resetDetail = function () {
        me.detail = {};
        me.dtlModelOld = undefined;
    };

    me.populateDetail = function (data) {
        me.detail = data;
        me.detail.BPUNo = data.BPUNo;
        me.detail.BPUSeq = data.BPUSeq;
        me.dtlModelOld = data;

        me.ModelYearOld = data.SalesModelYear;
        if (me.data.Tipe != "DO") {
            me.xChassisNo = data.ChassisNo;
            me.xEngineNo = data.EngineNo
            //txtBookSVNo.Text = recordDetail.ServiceBookNo.ToString();
            //txtKeyNo.Text = recordDetail.KeyNo.ToString();
        }
    };

    me.populateDetailLookup = function (data) {
        if (me.detail == undefined || me.detail == "") {
            me.detail = {};
        }
        setTimeout(function () {
            me.ReformatNumber();
            var selectorContainer = "";
            $.each(data, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.detail[key] = val;
                ctrl.removeClass("error");
            });
            me.Apply();
        }, 200);
    };

    me.populateData = function (data) {
        if (me.data == undefined || me.data == "") {
            me.data = {};
        }
        setTimeout(function () {
            me.ReformatNumber();
            var selectorContainer = "";
            $.each(data, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;
                ctrl.removeClass("error");
            });
            me.Apply();
        }, 200);
    };

    me.getWarehouse = function (BPUType) {
        $http.post("om.api/bpu/GetWarehouse", { BPUType: BPUType })
        .success(function (result) {
            if (result != undefined) {
                me.populateData(result);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
        
    }

    me.initialize = function () {
        me.detail = {};
        me.dtlModelOld = undefined;

        me.allowApproval = false;
        me.allowEdit = true;
        me.allowInputDetail = false;
        me.allowDeleteDetail = false;
        me.isSJBooking = false;
        me.isTrans1 = true;
        me.isTrans2 = true;

        me.status = "NEW";
        me.isManual = true;
        me.isDO = true;
        me.isSJ = false;

        me.xChassisNo = "";
        me.xEngineNo = "";
        me.data.BatchNo = "";
        me.data.DealerCode = "";
        me.data.SupplierCode = "";
        me.ModelYearOld = "";
        me.data.bpuCode = "";
        me.data.isBPUWH = false;
        me.data.isBuyBack = false;
        me.data.isCheckChassis = false;
        me.data.chkReffDODate = false;
        me.data.chkReffSJDate = false;

        me.data.BPUDate = currentDate;
        me.data.BPUSJDate = currentDate;
        me.data.RefferenceDODate = currentDate;
        me.data.RefferenceSJDate = currentDate;

        me.optionsTrans = "Manual";
        me.data.Tipe = "DO";
        me.optionsPO_Reff = "PO_Reff1";

        me.SetDO();
        me.SetManual();
        me.PoClick();
        me.chkReffDODate_change();
        me.chkReffSJDate_change()
        me.clearTable(me.gridDetailBPU);

        $http.post("om.api/bpu/default")
        .success(function (result) {
            if (result != undefined) {
                me.isCheckChassis = result.isCheckChassis;
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        $("label[btn-radio=\"'SJ'\"], #RefferenceSJNo").attr("disabled", "disabled");
        $("#chkReffDODate").removeAttr("disabled");

        $("#BPUDate").removeAttr("disabled");

        $('#POStatus').html(me.status);
        $('#POStatus').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        $("#btnNo b:first, #btnReff b:first").css(
        {
            "font-size": "16px",
            "padding-top": "3px"
        });

        $(".page .main form").submit();
    }

    me.stdChangedMonitoring = function (n, o) {
        if (me.status == "NEW") {
            if (me.isManual) {
                if (me.isDO) {
                    if (me.allowEdit) {
                        $("#RefferenceDONo").removeAttr("disabled");
                    }
                }
            }
        }

        if (me.status == "CANCELED" || me.status == "FINISHED") {
            me.hasChanged = false;
            me.isSave = true;
            $("#btnDelete").hide();
            me.isPrintAvailable = false;
        }
        else {
            if (me.status === "APPROVED") {
                $("#btnDelete").hide();
            }
            else {
                $("#btnDelete").show();
            }

            if (me.isLoadData) {
                me.isPrintAvailable = true;
            }

            if (!me.isInProcess) {
                var eq = (n == o);

                // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
                if (!(_.isEmpty(n)) && !eq) {
                    if (!me.hasChanged && !me.isLoadData) {
                        me.hasChanged = true;
                        me.isLoadData = false;
                    }
                    if (!me.isSave) {
                        //me.isLoadData = false;
                        me.hasChanged = true;
                        me.isSave = true;
                    }
                } else {
                    me.hasChanged = false;
                    me.isSave = false;
                }
            }
        }
    }

    me.$watch('data.Tipe', function (n, o) {
        if (n == undefined || n == "")
            n = "DO";

        me.getWarehouse(n);
    });

    me.$watch('data.Status', function (n, o) {
        console.log('data.Status : ' + n);

        //if (n == 3) {
        //    console.log('a');
        //    me.hasChanged = true;
        //    me.isSave = false;
        //    me.isLoadData = false;
        //    me.isPrintAvailable = false;
        //    me.isInitialize = false;
        //}
        //else {
        //    console.log('b');
        //        me.isLoadData = false;
        //        me.hasChanged = true;
        //        me.isSave = false;
        //        me.isInitialize = false;
        //        me.isPrintAvailable = true;
        //}
    })

    webix.event(window, "resize", function () {
        me.gridDetailBPU.adjust();
    });

    me.start();

    me.gridDetailBPU = new webix.ui({
        container: "wxDetailPO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 140 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 140 },
            { id: "SalesModelDesc", header: "Sales Model Desc.", width: 200 },
            { id: "ColourCode", header: "Kode Warna", width: 140 },
            { id: "ColourName", header: "Nama Warna", width: 200 },
            { id: "ChassisCode", header: "Kode Rangka", width: 140, format: me.replaceNull },
            { id: "ChassisNo", header: "No.Rangka", width: 100, format: me.replaceNull },
            { id: "EngineCode", header: "Kode Mesin", width: 140, format: me.replaceNull },
            { id: "EngineNo", header: "No.Mesin", width: 100, format: me.replaceNull },
            { id: "ServiceBookNo", header: "No.Buku Servis", width: 140, format: me.replaceNull },
            { id: "KeyNo", header: "No.Kunci", width: 100, format: me.replaceNull },
            { id: "Remark", header: "Keterangan", width: 140, format: me.replaceNull }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetailBPU.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridDetailBPU.getSelectedId().id);
                    if (rec.isReturn == true) {
                        MsgBox("Sudah di retur", MSG_WARNING);
                        return;
                    }
                    var params = {
                        model: rec,
                        PONo: me.data.PONo
                    };
                    $http.post('om.api/bpu/getbpudetail', params)
                    .success(function (result) {
                        if (result.success) {
                            me.populateDetail(result.data);
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });
}

$(document).ready(function () {
    var options = {
        title: "Bukti Penerimaan Unit",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnloptions",
                items: [
                    {
                        type: "optionbuttons",
                        name: "optionsTrans",
                        model: "optionsTrans",
                        items: [
                            { name: "Manual", model: "data.Manual", text: "Manual", click: "SetManual()" },
                            { name: "Upload", model: "data.Upload", text: "Upload", click: "SetUpload()" },
                        ]
                    }
                ]
            },
            {
                name: "pnlStatus",
                items: [
                    {
                        text: "Tipe BPU",
                        type: "optionbuttons",
                        name: "optionsBPU",
                        model: "data.Tipe",
                        cls: 'span3',
                        items: [
                            { name: "DO", model: "data.DO", text: "DO", click: "SetDO()" },
                            { name: "SJ", model: "data.SJ", text: "SJ", click: "SetSJ()" },
                            { name: "DO_SJ", model: "data.DO_SJ", text: "DO & SJ", click: "SetDOSJ()" }
                        ]
                    },
                    {
                        text: "",
                        type: "optionbuttons",
                        name: "optionsBPUSJ_BOOKING",
                        model: "data.TipeSJ_BOOKING",
                        cls: 'span1',
                        items: [
                            { name: "SJ_BOOKING", model: "data.SJ_BOOKING", text: "SJ Booking", show: "isSJBooking", disable: "!isSJBooking || !allowEdit" }
                        ]
                    },
                    { name: "POStatus", model: "data.POStatus", text: "", cls: "span2", readonly: true, type: "label" },
                    {
                        type: "buttons", cls: "span1", items: [
                            {
                                name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approveRecord()",
                                disable: "!allowApproval"
                            }
                        ]
                    }
                ]
            },
            {
                name: "pnlBPU",
                items: [
                    { name: "BPUNo", model: "data.BPUNo", text: "No. BPU", cls: "span4", readonly: true, placeHolder: 'PBU/XX/YYYYYY' },
                    {
                        name: "divTglBPU",
                        type: "controls",
                        text: "Tgl. BPU",
                        cls: "span4",
                        items: [
                            { name: "BPUDate", model: "data.BPUDate", cls: "span4", type: "ng-datepicker" },
                            { name: "BPUSJDate", model: "data.BPUSJDate", cls: "span4", type: "ng-datepicker", show: "isSJ" }
                        ]
                    },
                    { name: "RefferenceDONo", model: "data.RefferenceDONo", text: "No. Reff. DO", cls: "span4", type: 'popup', click: "lookupReffDO()", required: true, validasi: "required", disable: "!allowEditX", maxlength: 15 },
                    {
                        text: "Tgl. Reff. DO",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkReffDODate", model: "data.chkReffDODate", cls: "span1", type: "ng-check", disable: "me.status === 'APPROVED'", change: "chkReffDODate_change()" },
                            { name: "RefferenceDODate", model: "data.RefferenceDODate", placeHolder: "Tgl. Reff DO", cls: "span7", type: 'ng-datepicker' },
                        ]
                    },
                    { name: "RefferenceSJNo", model: "data.RefferenceSJNo", text: "No. Reff. SJ", cls: "span4", type: 'popup', click: "lookupReffSJ()", maxlength: 15 },
                    {
                        text: "Tgl. Reff. SJ",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkReffSJDate", model: "data.chkReffSJDate", cls: "span1", type: "ng-check", change: "chkReffSJDate_change()" },
                            { name: "RefferenceSJDate", model: "data.RefferenceSJDate", placeHolder: "Tgl. Reff SJ", cls: "span7", type: 'ng-datepicker' },
                        ]
                    },
                    {
                        name: "btnNo",
                        text: "No. PO",
                        type: "controls",
                        required: true,
                        cls: "span4",
                        items: [
                            {
                                type: "optionbuttons",
                                name: "optionsPO_Reff",
                                model: "optionsPO_Reff",
                                cls: "span1",
                                items: [
                                    { name: "PO_Reff1", model: "PO_Reff1", text: "<b></b>", click: "PoClick()", cls: "" }
                                ]
                            },
                            { name: "PONo", model: "data.PONo", cls: "span7", placeHolder: "No. PO", readonly: true, type: "popup", click: "lookupPONo()", validasi: "required", maxlength: 15 }
                        ]
                    },
                    {
                        text: "Pemasok",
                        type: "controls",
                        cls: "span4",
                        items: [
                        { name: "SupplierName", model: "data.SupplierName", text: "Pemasok", cls: "span8", placeHolder: "Nama Pemasok", readonly: true },
                        ]
                    },
                    { type: "span" },
                    {
                        name: "btnReff",
                        text: "No. Reff",
                        type: "controls",
                        cls: "span4 left",
                        items: [
                            {
                                type: "optionbuttons",
                                name: "optionsPO_Reff",
                                model: "optionsPO_Reff",
                                cls: "span1",
                                items: [
                                    { name: "PO_Reff2", model: "PO_Reff2", text: "<b></b>", click: "ReffClick()", cls: "" }
                                ]
                            },
                            { name: "ReffNo", model: "data.ReffNo", cls: "span7", placeHolder: "No. Reff", readonly: true, type: "popup", click: "lookupReffNo()" },
                        ]
                    },
                    {
                        text: "Kirim Ke",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ShipTo", model: "data.ShipTo", text: "Kirim Ke", cls: "span8", placeHolder: "Kirim Ke", maxlength: 100 }
                        ]
                    },
                    {
                        text: "Ekspedisi",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "Expedition", model: "data.Expedition", cls: "span2", placeHolder: "Ekspedisi", readonly: true, type: "popup", click: "lookupExpedition()", validasi: "required" },
                            { name: "ExpeditionName", model: "data.ExpeditionName", cls: "span6", placeHolder: "Nama Ekspedisi", readonly: true }
                        ]
                    },
                    {
                        text: "Gudang",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "WarehouseCode", model: "data.WarehouseCode", cls: "span2", placeHolder: "Gudang", readonly: true, type: "popup", click: "lookupWarehouse()", validasi: "required" },
                            { name: "WarehouseName", model: "data.WarehouseName", cls: "span6", placeHolder: "Nama Gudang", readonly: true }
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span12", maxlength: 100 }
                ]
            },
            {
                name: "pnlDetailBPU",
                title: "Detil BPU",
                items: [
                { name: "SalesModelCode", model: "detail.SalesModelCode", text: "Sales Model Code", cls: "span3", placeHolder: "Sales Model Code", readonly: true, type: "popup", click: "lookupModel()", required: true },
                {
                    type: "controls",
                    text: "Sales Model Year",
                    required: true,
                    cls: "span5",
                    items: [
                        { name: "SalesModelYear", model: "detail.SalesModelYear", placeHolder: "Sales Model Year", cls: "span1", type: "popup", required: true, readonly: true, click: "lookupModelYear()" },
                        { name: "SalesModelDesc", model: "detail.SalesModelDesc", placeHolder: "Sales Model Desc", cls: "span4", readonly: true },
                    ]
                },
                {
                    text: "Warna",
                    type: "controls",
                    required: true,
                    items: [
                        { name: "ColourCode", model: "detail.ColourCode", cls: "span2", placeHolder: "Warna", readonly: true, type: "popup", click: "lookupColour()", required: true },
                        { name: "ColourName", model: "detail.ColourName", cls: "span6", placeHolder: "Deskripsi Warna", readonly: true }
                    ]
                },
                { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span4", show: "isDO", readonly: true },
                { name: "EngineCode", model: "detail.EngineCode", text: "Kode Mesin", cls: "span4", show: "isDO", readonly: true },
                {
                    type: "controls",
                    text: "Kode/No. Rangka",
                    cls: "span4",
                    required: true,
                    show: "!isDO",
                    items: [
                        { name: "ChassisCode", model: "detail.ChassisCode", cls: "span4", readonly: true },
                        { name: "ChassisNo", model: "detail.ChassisNo", cls: "span4", required: true, type: "numeric", maxlength: 10 }
                    ]
                },
                {
                    type: "controls",
                    text: "Kode/No. Mesin",
                    cls: "span4",
                    required: true,
                    show: "!isDO",
                    items: [
                        { name: "EngineCode", model: "detail.EngineCode", cls: "span4", readonly: true },
                        { name: "EngineNo", model: "detail.EngineNo", cls: "span4", required: true, type: "numeric", maxlength: 10 }
                    ]
                },
                { name: "ServiceBookNo", model: "detail.ServiceBookNo", text: "No. Buku Servis", cls: "span4", required: true, show: "!isDO", maxlength: 15 },
                { name: "KeyNo", model: "detail.KeyNo", text: "No. Kunci", cls: "span4", disable: "isSJzzz", show: "!isDO", maxlength: 15 },
                { name: "RemarkDetail", model: "detail.Remark", text: "Keterangan", cls: "span12", disable: "isSJzzz", maxlength: 100 },
                {
                    type: "buttons",
                    items: [
                        { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveDetailValidate()", disable: "!allowInputDetail" },
                        { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail()", disable: "(!allowDeleteDetail) || dtlModelOld === undefined" },
                    ]
                },
                {
                    name: "wxDetailPO",
                    type: "wxdiv",
                }
                ]
            }
        ]

    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omBPUController");
    }
});