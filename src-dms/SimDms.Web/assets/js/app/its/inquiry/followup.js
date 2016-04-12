var isCEO = false;
var empID = "";
var pType = "";
var Position = "";
var outletID = "";
var OutletCode = "";
var isUserLink = false;
var branch = "";
var pembantu = 1;
"use strict"

function followup($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                if (dt.success) {
                    isCEO = dt.data.isCOO;
                    empID = dt.data.EmployeeID;
                    pType = dt.data.ProductType;
                    Position = dt.data.Position;
                    outletID = dt.data.outletID;
                    OutletCode = dt.data.OutletCode;
                    isUserLink = true;
                    branch = dt.data.Branch;
                } else {
                    MsgBox(dt.message, MSG_ERROR);
                    Wx.enable({ value: false, items: ["cmbBranch", "cmbSH", "cmbSC", "cmbSM"] });
                    $('#btnSearch').attr('disabled', 'disabled');
                    $('#btnGenerate').attr('disabled', 'disabled');
                    isUserLink = false;
                }
            }
        });
    }

    me.initialize = function () {
        me.data = {};
        me.comboBranch = {};
        me.comboSH = {};
        me.comboSC = {};
        me.comboSM = {};
        me.clearTable(me.grid1);
        me.data.dtpTo = new Date();
        me.data.dtpFrom = new Date().getMonth() + 1 + '/' + 01 + '/' + new Date().getFullYear();
        me.data.ITSSIS = false;
        me.isComboShow = false;
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        me.data.Detail = false;
        me.isPrintAvailable = true;
        $("[name='btnEC']").attr('disabled', 'disabled');
        if (branch == null && isCEO == false) {
            MsgBox("Cabang tempat employee bekerja belum di set (Manpower Management di tab Mutation)!", MSG_INFO);
            $("#btnSearch, #btnGenerate, #dtpFrom, #dtpTo, #cmbBranch, #cmbSH, #cmbSC, #cmbSM, #Detail, #ITSSIS").attr("disabled", "disabled");
            //$("[name='pnlFilter]").attr("disabled", "disabled");
        } else {
            if (isUserLink) {
                $http.post('its.api/InquiryIts/itsFollowUpGetBranch').
                    success(function (dt, status, headers, config) {
                        if (dt.success == true) {
                            if (!isCEO) {
                                $http.post('its.api/Combo/getBranchList?id=' + dt.data.BranchCode).
                                    success(function (dt, status, headers, config) {
                                        if (dt.success == true) {
                                            me.comboBranch = dt.data;
                                            me.data.cmbBranch = OutletCode;
                                        } else { }
                                    });
                            } else {
                                $http.post('its.api/Combo/Branch').
                                   success(function (dt, status, headers, config) {
                                       me.comboBranch = dt;
                                       Wx.enable({ value: false, items: ["cmbSH", "cmbSM"] })
                                   });
                            }
                        }
                    });

                if (!isCEO) {
                    setTimeout(function () {
                        $('[name=cmbBranch]').select2().select2('val', $('[name=cmbBranch] option:eq(1)').val());
                    }, 2000)

                    $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                        success(function (dt, status, headers, config) {
                            if (Position == 'BM') {
                                me.comboSH = dt.list;
                                me.comboSM = dt.listSM;
                            } else if (Position == 'BMSH') {
                                me.comboSH = dt.list;
                                me.comboSM = dt.listSM;
                            } else if (Position == 'SH' || Position == 'SHTS' || Position == 'SHSTD') {
                                me.data.cmbSH = empID;
                                me.comboSH = dt.list;
                                $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.cmbSH + '&pos=S').
                                success(function (data, status, headers, config) {
                                    me.comboSM = data;
                                });
                                Wx.enable({ value: false, items: ["cmbSH"] })
                            } else if (Position == 'SC') {
                                me.data.cmbSC = empID;
                                me.comboSC = dt.list;
                                $http.post('its.api/Combo/getSalesmanByPos?emp=' + me.data.cmbSH + '&pos=S').
                                success(function (data, status, headers, config) {
                                    me.comboSM = data;
                                });
                                Wx.enable({ value: false, items: ["cmbSH"] })
                            } else {
                                me.comboSH = dt.ListSH;
                                me.data.cmbSH = dt.emplSH;
                                me.comboSM = dt.ListSM;
                                me.data.cmbSM = dt.emplSM;
                                Wx.enable({ value: false, items: ["cmbSH"] })
                                Wx.enable({ value: false, items: ["cmbSM"] })
                            }
                        });
                    Wx.enable({ value: false, items: ["cmbBranch"] })
                } else {
                    empID = "";
                    $http.post('its.api/Combo/SalesHead?EmployeeID=').
                       success(function (dt, status, headers, config) {
                           me.comboSH = dt.list;
                       });
                }
            }

        }
        
    }

    //me.btnSearchClick = function () {
    //    var data = {
    //        DateFrom: me.data.dtpFrom,
    //        DateTo: me.data.dtpTo,
    //        Branch: $("[name=cmbBranch]").val(),
    //        Emp1: empID,
    //        Emp2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
    //        Emp3: $("[name=cmbSM]").val()
    //    }
    //    console.log(data);
    //    $('.page > .ajax-loader').show();
    //    $http.post('its.api/InquiryIts/itsLoadFollowUp', data)
    //        .success(function (dt, status, headers, config) {
    //            $('.page > .ajax-loader').hide();
    //            me.loadTableData(me.grid1, dt)
    //        })
    //        .error(function (e, status, header, config) {
    //            $('.page > .ajax-loader').hide();
    //            MsgBox(e, MSG_ERROR)
    //        });

    //    //if (me.data.ITSSIS == true) {
    //    //    $('.page > .ajax-loader').show();
    //    //    $http.post('its.api/InquiryIts/itsLoadFollowUp', data)
    //    //        .success(function (dt, status, headers, config) {
    //    //            $('.page > .ajax-loader').hide();
    //    //            me.isPrintAvailable = true;
    //    //            me.loadTableData(me.grid1, dt)
    //    //        })
    //    //        .error(function (e, status, header, config) {
    //    //            $('.page > .ajax-loader').hide();
    //    //            MsgBox(e, MSG_ERROR)
    //    //        });
    //    //} else {
    //    //    $('.page > .ajax-loader').show();
    //    //    $http.post('its.api/InquiryIts/itsLoadFollowUp2', data)
    //    //        .success(function (dt, status, headers, config) {
    //    //            $('.page > .ajax-loader').hide();
    //    //            me.isPrintAvailable = false;
    //    //            me.loadTableData(me.grid1, dt)
    //    //        })
    //    //        .error(function (e, status, header, config) {
    //    //            MsgBox(e, MSG_ERROR)
    //    //            $('.page > .ajax-loader').hide();
    //    //        });
    //    //}
    //}

    me.btnGenerateClick = function () {
        var Branch = $("[name=cmbBranch]").val() == undefined ? "" : $("[name=cmbBranch]").val();
        var outlet = isCEO == true ? "" : outletID;
        console.log(outletID);
        console.log(isCEO);
        var data = {
            Branch: Branch,
            DateFrom: moment(me.data.dtpFrom).format('YYYYMMDD'),
            DateTo: moment(me.data.dtpFrom).format('YYYYMMDD'),
            outletID: outlet,
            Emp: $("[name=cmbSM]").val(),
            Param: me.data.ITSSIS == true ? '1' : '0',
            Head: $("[name=cmbSH]").val()
        }
        $('.page > .ajax-loader').show();
        $http.post('its.api/InquiryIts/GetGenerateFollowUp', data)
               .success(function (e) {
                   $('.page > .ajax-loader').hide();
                   if (e.success) {
                       Wx.showFlatFile({ data: e.contents });
                   }
               })
               .error(function (e) {
                   $('.page > .ajax-loader').hide();
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
    }

    SavePopup = function () {
        window.location = 'its.api/InquiryIts/SaveGenerateFollowUp';
    }

    SendPopup = function () {
        $http.post('its.api/InquiryIts/ValidateHeaderFile', { contents: $('#pnlViewData').val() })
        .success(function (e) {
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('its.api/InquiryIts/SendFile', { contents: $('#pnlViewData').val() })
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
                        $http.post('its.api/InquiryIts/SendFile', { contents: $('#pnlViewData').val() })
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

    $("[name=cmbBranch]").on('click', function () {
        if (me.data.cmbBranch != "") {
            $.ajax({
                async: true,
                type: "POST",
                data: { branch: $("[name='cmbBranch']").val() },
                url: 'its.api/Combo/getEmployeeSH',
                success: function (data) {
                    if (data.BM != "") {
                        me.comboSH = data.listSH;
                        me.comboSM = data.listSM;
                        empID = data.BM;
                        console.log(data.BM);
                        console.log(empID);
                        me.Apply();
                    } 
                }
            });
            Wx.enable({ value: true, items: ["cmbSH"] })
            Wx.enable({ value: true, items: ["cmbSC"] })
            Wx.enable({ value: true, items: ["cmbSM"] })
        } else {
            Wx.enable({ value: false, items: ["cmbSH"] })
            Wx.enable({ value: false, items: ["cmbSC"] })
            Wx.enable({ value: false, items: ["cmbSM"] })
            me.data.cmbSH = "";
            me.data.cmbSM = "";
            empID = "";
            console.log(empID);
            me.Apply();
        }
    });

    $("[name=cmbSH]").on('click', function () {
        if (me.data.cmbSH != "") {
            $.ajax({
                async: true,
                type: "POST",
                //data: { emp: $("[name='cmbSH']").val(), pos: 'SH' },
                //url: 'its.api/Combo/getSalesmanByPos',
                data: { emp: $("[name='cmbSH']").val() },
                url: 'its.api/Combo/getEmployee',
                success: function (data) {
                    Wx.enable({ value: true, items: ["cmbSM"] })
                    me.comboSM = data;
                    me.Apply();
                }
            });
        } else {
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        me.comboSH = dt.list;
                        me.comboSM = dt.listSM;
                    });
            me.data.cmbSM = "";
        }      
    });
    
    $("[name=cmbSC]").on('click', function () {
        if (me.data.cmbSC != "") {
            $.ajax({
                async: true,
                type: "POST",
                //data: { emp: $("[name='cmbSH']").val(), pos: 'SH' },
                //url: 'its.api/Combo/getSalesmanByPos',
                data: { emp: $("[name='cmbSC']").val() },
                url: 'its.api/Combo/getEmployee',
                success: function (data) {
                    Wx.enable({ value: true, items: ["cmbSM"] })
                    me.comboSM = data;
                    me.Apply();
                }
            });
        } else {
            $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                   success(function (dt, status, headers, config) {
                       me.comboSH = dt.list;
                       me.comboSM = dt.listSM;
                   });
            me.data.cmbSM = "";
        }
    });

    $("[name=cmbSM]").on('click', function () {
        if (me.data.cmbSH == "" || me.data.cmbSC == "") {
            if (me.data.cmbSM != "") {
                $.ajax({
                    async: true,
                    type: "POST",
                    data: { emp: $("[name='cmbSM']").val() },
                    url: 'its.api/Combo/getTeamLeader',
                    success: function (data) {
                        if (pType == '4W') {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.comboSH = data.listSH;
                            me.data.cmbSH = data.empSH;
                            me.Apply();
                        } else {
                            Wx.enable({ value: false, items: ["cmbSH"] })
                            me.comboSC = data.listSH;
                            me.data.cmbSC = data.empSH;
                            me.Apply();
                        }
                    }
                });
            } else {
                $http.post('its.api/Combo/SalesHead?EmployeeID=' + empID).
                    success(function (dt, status, headers, config) {
                        if (Position == 'BM') {
                            me.comboSH = dt.list;
                            me.comboSM = dt.listSM;
                        } else if (Position == 'BMSH') {
                            me.comboSH = dt.list;
                            me.comboSM = dt.listSM;
                        } 
                    });
                Wx.enable({ value: false, items: ["cmbSH"] })
                //me.data.cmbSH = "";
                //me.data.cmbSC = "";
                me.Apply();
            }
        }
    });

    $("[name=ITSSIS]").on('change', function () {
        me.data.ITSSIS = $('#ITSSIS').prop('checked');
        me.Apply();
    });


    me.FollowUpPivote = function () {
        refreshGrid();
        //if (me.data.ITSSIS == true) {
        //    refreshGrid();
        //    me.isPrintAvailable = true;
        //} else {
        //    refreshGrid();
        //    me.isPrintAvailable = false;
        //}
    }

    function refreshGrid() {
        var params = {
                    DateFrom: moment(me.data.dtpFrom).format("YYYYMMDD"),
                    DateTo: moment(me.data.dtpTo).format("YYYYMMDD"),
                    Branch: $("[name=cmbBranch]").val(),
                    Emp1: empID,
                    Emp2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
                    Emp3: $("[name=cmbSM]").val(),
                    outletID : isCEO == true ? "" : outletID,
                    Param : me.data.ITSSIS == true ? '1' : '0'
        }

        console.log(params);
        Wx.kgrid({
            url: "its.api/InquiryIts/itsLoadFollowUp",
            name: "gridfollowup",
            params: params,
            pageSize: 500,
            height: 350,
            aggregate: [ 
                    { field: "InquiryNumber", aggregate: "count"},
                    { field: "Pelanggan", aggregate: "count" },
                    { field: "AlamatProspek", aggregate: "count" },
                    { field: "TelpRumah", aggregate: "count" },
                    { field: "NamaPerusahaan", aggregate: "count" },
                    { field: "InquiryDate", aggregate: "count" },
                    { field: "TipeKendaraan", aggregate: "count" },
                    { field: "Variant", aggregate: "count" },
                    { field: "Transmisi", aggregate: "count" },
                    { field: "Warna", aggregate: "count" },
                    { field: "PerolehanData", aggregate: "count" },
                    { field: "Employee", aggregate: "count" },
                    { field: "NextFollowUpDate", aggregate: "count" },
                    { field: "LastProgress", aggregate: "count" },
                    { field: "ActivityDetail", aggregate: "count" }
            ],
            groupable: true,
            scrollable: true,
            filterable: true,
            resizable: false,
            columnMenu: true,
            dataBound: function () {
                $("[name='btnEC']").attr('disabled', 'disabled');
                var grid = $('#gridfollowup').data("kendoGrid");
                console.log(grid.tbody.find(">tr.k-grouping-row")[0]);
                if (grid.tbody.find(">tr.k-grouping-row")[0]) {
                    grid.collapseGroup(grid.tbody.find(">tr.k-grouping-row"));
                    //$('tr[role*="row"]').hide();
                    $('tr[role*="row"]').slice(1).hide();
                    $("[name='btnEC']").text('Expand data');
                    $("[name='btnEC']").removeAttr('disabled');
                }
            },
            columns: [
            { field: "InquiryNumber", title: "No", width: 70, aggregates: "count", groupHeaderTemplate: "No: #= value # (#= count#)" },
            { field: "Pelanggan", title: "Pelanggan", width: 150, aggregates: "count", groupHeaderTemplate: "Pelanggan: #= value # (#= count#)" },
            { field: "AlamatProspek", title: "Alamat", width: 250, aggregates: "count", groupHeaderTemplate: "Alamat: #= value # (#= count#)" },
            { field: "TelpRumah", title: "No Telp", width: 150, aggregates: "count", groupHeaderTemplate: "No Telp: #= value # (#= count#)" },
            { field: "NamaPerusahaan", title: "Nama Perusahaan", width: 200, aggregates: "count", groupHeaderTemplate: "Nama Perusahaan: #= value # (#= count#)" },
            { field: "InquiryDate", title: "Tgl KDP", width: 150, aggregates: "count", groupHeaderTemplate: "Tgl KDP: #= value # (#= count#)" },
            { field: "TipeKendaraan", title: "Tipe", width: 120, aggregates: "count", groupHeaderTemplate: "Tipe: #= value # (#= count#)" },
            { field: "Variant", title: "Varian", width: 120, aggregates: "count", groupHeaderTemplate: "Varian: #= value # (#= count#)" },
            { field: "Transmisi", title: "AT/MT", width: 60, aggregates: "count", groupHeaderTemplate: "AT/MT: #= value # (#= count#)" },
            { field: "Warna", title: "Warna", width: 150, aggregates: "count", groupHeaderTemplate: "Warna: #= value # (#= count#)" },
            { field: "PerolehanData", title: "Perolehan Data", width: 90, aggregates: "count", groupHeaderTemplate: "Perolehan Data: #= value # (#= count#)" },
            { field: "Employee", title: "Wiraniaga", width: 150, aggregates: "count", groupHeaderTemplate: "Wiraniaga: #= value # (#= count#)" },
            { field: "NextFollowUpDate", title: "NextFollowUp", width: 150, aggregates: "count", groupHeaderTemplate: "NextFollowUp: #= value # (#= count#)" },
            { field: "LastProgress", title: "Last Progress", width: 90, aggregates: "count", groupHeaderTemplate: "Last Progress: #= value # (#= count#)" },
            { field: "ActivityDetail", title: "Follow Up Detail", width: 280, aggregates: "count", groupHeaderTemplate: "Follow Up Detail: #= value # (#= count#)" }
            ],
            change: function (arg) {
                var i = 2;
                var no = i.toString();
                var selected = $.map(this.select(), function (item) {
                    //return $(item).find('td').first().text();
                    return $(item).find('td:first-child').text();
                });
                //if (selected[0].length == 1) {
                //    //if (selected[0] != ' ' && selected[0] != '') {
                //    selected = $.map(this.select(), function (item) {
                //        return $(item).find('td:nth-child('+no+')').text();
                //        });
                //    //}
                //}
                while (selected[0].replace(" ", "0")==0 ) {
                    selected = $.map(this.select(), function (item) {
                        return $(item).find('td:nth-child(' + no + ')').text();
                    });
                    i = i + 1;
                    no = i.toString();
                }
                var data = {
                    InquiryNumber: selected[0]
                }
                console.log(selected[0].replace(" ", "0"));
                console.log(data);
                Wx.loadForm();
                Wx.showForm({ url: "its/trans/inputkdp", params: JSON.stringify(data) });
            },
        });
    }

    me.ExpandorCollapse = function () {
        console.log($("[name='btnEC']").text());
        if ($("[name='btnEC']").text() == 'Expand data') {
            $('tr[role*="row"]').show();
            $("[name='btnEC']").text('Collapse data');
        } else {
            $("[name='btnEC']").text('Expand data');
            var grid = $('#gridfollowup').data("kendoGrid");
            grid.collapseGroup(grid.tbody.find(">tr.k-grouping-row"));
            $('tr[role*="row"]').slice(1).hide();
        }
    }

    me.printPreview = function () {
        var tanggal = moment(me.data.dtpFrom).format("DD-MM-YYYY") + " S/D " + moment(me.data.dtpTo).format("DD-MM-YYYY");
        var param1 = pType == '4W' ? me.data.cmbSH == "" ? "SEMUA" : $("[name=cmbSH]").select2("data").text : me.data.cmbSC == "" ? "SEMUA" : $("[name=cmbSC]").select2("data").text
        var param2 = "-";
        var param3 = me.data.cmbSM == "" ? "SEMUA" : $("[name=cmbSM]").select2("data").text;
        var param4 = pType == '4W' ? "Nama Sales Head" : "Nama Sales Kordinator";
        param1 = param1.replace(',', '.');
        param3 = param3.replace(',', '.');
        param4 = param4.replace(',', '.');
        var rparam = tanggal + "," + param1 + "," + param2 + "," + param3 + "," + param4;
        var Branch = $("[name=cmbBranch]").val() == undefined ? "" : $("[name=cmbBranch]").val();
        var ReportId = me.data.Detail == false ? 'PmRpInqFollowUpDet2015' : 'PmRpInqFollowUpDet2015B';
        console.log(ReportId, Branch);
        var outletID = isCEO == true ? "" : outletID;
        var prm = [
            'companycode',
            Branch,
            moment(me.data.dtpFrom).format('YYYYMMDD'),
            moment(me.data.dtpTo).format('YYYYMMDD'),
            outletID,
            $("[name=cmbSM]").val(),
            me.data.ITSSIS == true ? '1' : '0',
            $("[name=cmbSH]").val()
        ];
        console.log(prm, ReportId);
        Wx.showPdfReport({
            id: ReportId,
            pparam: prm.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry - Follow Up",
        xtype: "panels",
        toolbars: [
                    { name: "btnPrint", cls: "btn btn-primary", text: "Print", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        panels: [
            {
                items: [                        
                    {
                        text: "Date (From - To)",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "dtpFrom", text: "Date From", cls: "span4", type: "ng-datepicker" },
                            { name: "dtpTo", text: "Date To", cls: "span4", type: "ng-datepicker" },
                        ]
                    },
                    {
                        text: "Detail Data",
                        cls: "span3",
                        type: "controls", items: [
                            { name: "Detail", model: "data.Detail", type: "ng-check" },
                        ]
                    },
                    {
                        text: "Branch/Outlet",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "cmbBranch", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboBranch" }
                        ]
                    },
                    {
                        text: "ITS ke SIS",
                        cls: "span3",
                        type: "controls", items: [
                            { name: "ITSSIS", type: "ng-check" },
                        ]
                    },
                    {
                        name: "divSalesHead",
                        text: "Sales Head",
                        cls: "span5",
                        show: "isComboShow",
                        type: "controls", items: [
                            { name: "cmbSH", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSH", show: "isComboShow" },
                        ]
                    },
                    {
                        name: "divSalesCo",
                        text: "Sales Coordinator",
                        cls: "span5",
                        show: "!isComboShow",
                        type: "controls", items: [
                            { name: "cmbSC", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSC", show: "!isComboShow" }
                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span3",
                        items: [
                            { name: "btnSearch", cls: "btn btn-info", icon: "icon-search", text: "Search", click: "FollowUpPivote()", style: "width:120px;" },
                        ]
                    },
                    //{
                    //    type: "buttons",
                    //    cls: "span3",
                    //    items: [
                    //        { name: "btnGenerate", cls: "btn btn-info", icon: "icon-gear", text: "Generate File", click: "btnGenerateClick()", style: "width:120px;" }
                    //    ]
                    //},
                    {
                        text: "Salesman",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "cmbSM", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSM" }
                        ]
                    },
                    
                    {
                        cls: "span3",
                        //text: "Klik untuk",
                        type: "controls", items: [
                            {
                                type: "buttons",
                                cls: "span1",
                                text: "Klik untuk",
                                items: [
                                    { name: "btnEC", cls: "btn", text: "Expand data", click: "ExpandorCollapse()", style: "width:120px;" }
                                ]
                            },
                            //{ type: "label", text: "data" },
                        ]
                    },
                    

                ]
            },
            
            //{
            //    items: [
            //        {
            //            type: "buttons",
            //            cls: "span3",
            //            items: [
            //                { name: "btnEC", cls: "btn btn-info", text: "Expand", click: "Expand()", style: "width:120px;" }
            //                //{ name: "btnSearch", cls: "btn btn-info", icon: "icon-search", text: "Search", click: "btnSearchClick()", style: "width:120px;" }  
            //            ]
            //        },
            //    ]
            //},
            //{
            //    name: 'wxpivotgrid',
            //    xtype: 'wxtable',
            //    style: 'margin-top: 35px;'
            //},
            {
                name: "gridfollowup",
                xtype: "k-grid",
                cls: "expand"
            },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("followup");
    }
});