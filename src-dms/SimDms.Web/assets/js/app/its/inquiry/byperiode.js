var report = "PmRpInqPeriodeWeb";
var isCEO = false;
var empID = "";
var empName = "";
var pType = "";
var Position = "";
var OutletCode = "";
var OutletName = "";
var bm = "";
var branch = "";


"use strict"

function byperiode($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data = {};
        me.isPrintAvailable = false;
        me.comboBranch = {};
        me.comboSH = {};
        me.comboSC = {};
        me.comboSM = {};
        me.clearTable(me.grid1);
        me.data.dtpTo = new Date();
        me.data.dtpFrom = new Date().getMonth() + 1 + '/' + 01 + '/' + new Date().getFullYear();
        me.isComboShow = false;
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        $("[name='btnEC']").attr('disabled', 'disabled');
        if (branch == null && isCEO == false) {
            MsgBox("Cabang tempat employee bekerja belum di set (Manpower Management di tab Mutation)!", MSG_INFO);
            $("#btnSearch, #dtpFrom, #dtpTo, #cmbBranch, #cmbSH, #cmbSC, #cmbSM, #Detail").attr("disabled", "disabled");
            //$("[name='pnlFilter]").attr("disabled", "disabled");
        } else {
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
                        $http.post('its.api/Combo/getBranchList?id=').
                            success(function (dt, status, headers, config) {
                                if (dt.success == true) {
                                    me.comboBranch = dt.data;
                                    //me.data.cmbBranch = OutletCode;
                                    Wx.enable({ value: false, items: ["cmbSH", "cmbSM"] })
                                } else { }
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
                            } else if (Position == 'SH' || Position == 'SHTS') {
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
                console.log(Position);
                Wx.enable({ value: false, items: ["cmbBranch"] })

            } else {
                $http.post('its.api/Combo/SalesHead?EmployeeID=').
                   success(function (dt, status, headers, config) {
                       me.comboSH = dt;
                   });
            }
              
        }
        
       

    }

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                isCEO = dt.data.isCOO;
                empID = dt.data.EmployeeID;
                pType = dt.data.ProductType;
                empName = dt.data.EmployeeName;
                Position = dt.data.Position;
                OutletCode = dt.data.OutletCode;
                OutletName = dt.data.outletName;
                branch = dt.data.Branch;
                if (Position == 'SH') {
                    bm = dt.data.UpperLvl != null ? bm = dt.data.UpperLvl : '';
                }
                if (Position == 'BM') {
                    bm = dt.data.EmployeeID;
                }
            }
        });
    }

    me.btnSearchClick = function () {
        var data = {
            DateFrom: me.data.dtpFrom,
            DateTo: me.data.dtpTo,
            Branch: $("[name=cmbBranch]").val(),
            Emp1: empID,
            Emp2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
            Emp3: $("[name=cmbSM]").val()
        }
            $('.page > .ajax-loader').show();
            $http.post('its.api/InquiryIts/itsLoadPeriode', data)
                .success(function (dt, status, headers, config) {
                    $('.page > .ajax-loader').hide();
                    me.isPrintAvailable = true;
                    me.loadTableData(me.grid1, dt);
                })
                .error(function (e, status, header, config) {
                    $('.page > .ajax-loader').hide();
                    MsgBox(e, MSG_ERROR)
                });
    }

    me.Salesman = function () {
                var lookup = Wx.klookup({
                    name: "btnBrowse",
                    title: "Salesman",
                    //url: "its.api/grid/SMlist?NikSH=" + me.data.NikSH,
                    url: "its.api/grid/SMlist1",
                    params: { name: "controls", items: [{ name: "cmbSH", param: "cmbSH" }] },
                    pageSize: 10,
                    serverBinding: true,
                    columns: [
                        { field: "EmployeeID", title: "ID" },
                        { field: "EmployeeName", title: "Salesman" },
                        { field: "Position", title: "Jabatan" },
                    ]
                });
                lookup.dblClick(function (data) {
                    if (data != null) {
                        me.data.cmbSM = data.EmployeeID;
                        me.data.cmbSMName = data.EmployeeName;
                        me.Apply();
                    }
                });
    }

    $("[name=cmbBranch]").on('click', function () {
        $.ajax({
            async: true,
            type: "POST",
            data: { branch: $("[name='cmbBranch']").val() },
            url: 'its.api/Combo/getEmployeeSH',
            success: function (data) {
                if (data.BM != "") {
                    Wx.enable({ value: true, items: ["cmbSH"] })
                    me.comboSH = data.listSH;
                    empID = data.BM;
                    bm = data.BM;
                    console.log(empID);
                    me.Apply();
                } else {
                    Wx.enable({ value: false, items: ["cmbSH"] })
                    Wx.enable({ value: false, items: ["cmbSM"] })
                    me.data.cmbSH = "";
                    me.data.cmbSM = "";
                    me.comboSM = data.listSH;
                    empID = data.BM;
                    console.log(empID);
                    me.Apply();
                }
            }
        });
    });

    $("[name=cmbSH]").on('click', function () {
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
    });

    $("[name=cmbSC]").on('click', function () {
        $.ajax({
            async: false,
            type: "POST",
            data: { emp: $("[name='cmbSC']").val(), pos: 'SC' },
            url: 'its.api/Combo/getSalesmanByPos',
            success: function (data) {
                Wx.enable({ value: true, items: ["cmbSM"] })
                me.comboSM = data;
                me.Apply();
            }
        });
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

    me.ByPeriodePivote = function () {
        refreshGrid();
        me.isPrintAvailable = true;
    }

    function refreshGrid() {
        var params = {
            DateFrom: moment(me.data.dtpFrom).format("YYYYMMDD"),
            DateTo: moment(me.data.dtpTo).format("YYYYMMDD"),
            Branch: $("[name=cmbBranch]").val(),
            Emp1: bm,
            Emp2: pType === '4W' ? $("[name=cmbSH]").val() : $("[name=cmbSC]").val(),
            Emp3: $("[name=cmbSM]").val()
        }

        console.log(params);
        Wx.kgrid({
            url: "its.api/InquiryIts/itsLoadPeriode",
            name: "gridbyperiode",
            params: params,
            pageSize: 500,
            height: 350,
            aggregate: [
                    { field: "InquiryNumber", aggregate: "count" },
                    { field: "InquiryNumber", aggregate: "count" },
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
                var grid = $('#gridbyperiode').data("kendoGrid");
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
                while (selected[0].length == 1) {
                    selected = $.map(this.select(), function (item) {
                        return $(item).find('td:nth-child(' + no + ')').text();
                    });
                    i = i + 1;
                    no = i.toString();
                }
                var data = {
                    InquiryNumber: selected[0]
                }
                console.log(selected);
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
            var grid = $('#gridbyperiode').data("kendoGrid");
            grid.collapseGroup(grid.tbody.find(">tr.k-grouping-row"));
            $('tr[role*="row"]').slice(1).hide();
        }
    }
    //me.grid1 = webix.ui({
    //    container: "wxbyperiode",
    //    view: "wxtable", css:"alternating",
    //    scrollX: true,
    //    scrollY: true,
    //    autoHeight: false,
    //    height: 450,
    //    columns: [
    //        { id: "InquiryNumber", header: "No", width: 70 },
    //        { id: "Pelanggan", header: "Pelanggan", width: 150 },
    //        { id: "InquiryDate", header: "Tgl KDP", width: 90, format: me.dateFormat },
    //        { id: "TipeKendaraan", header: "Tipe", width: 120 },
    //        { id: "Variant", header: "Varian", width: 120 },
    //        { id: "Transmisi", header: "AT/MT", width: 60 },
    //        { id: "Warna", header: "Warna", width: 150 },
    //        { id: "PerolehanData", header: "Perolehan Data", width: 90 },
    //        { id: "Employee", header: "Wiraniaga", width: 150 },
    //        { id: "NextFollowUpDate", header: "NextFollowUp", width: 150, format: me.replaceNull, format: me.dateFormat},
    //        { id: "LastProgress", header: "Last Progress", width: 90 },
    //        { id: "ActivityDetail", header: "Follow Up Detail", width: 280, format: me.replaceNull, }
    //    ]
    //});

    //webix.event(window, "resize", function () {
    //    me.grid1.adjust();
    //});

    me.printPreview = function () {
        //MsgBox("Fungsi print belum jalan ya!", MSG_INFO)
        var firstPeriod = moment(me.data.dtpFrom).format("YYYYMMDD");
        var endPeriod = moment(me.data.dtpTo).format("YYYYMMDD");
        var period = moment(me.data.dtpFrom).format("DD-MM-YYYY") + " S/D " + moment(me.data.dtpFrom).format("DD-MM-YYYY");
        //var bm = $("[name=cmbBM]").val();
        var sh = $("[name=cmbSH]").val();
        var sm = $("[name=cmbSM]").val();
        var par = [];
        var rparam = ""
        var Branch, outlet, spv, emp;
        outlet = ""
        if (isCEO && $("[name=cmbBranch]").val() == "")
            Branch = "";
        else 
            Branch = $("[name=cmbBranch]").val();  
           

        if (pType === '4W') {
            if ($("[name=cmbSH]").val() == "" || $("[name=cmbSH]").val() == "undefined")
                spv = "";
            else
                spv = $("[name=cmbSH]").val();
        }else {
            if ($("[name=cmbSC]").val() == "" || $("[name=cmbSC]").val() == "undefined")
                spv = "";
            else
                spv = $("[name=cmbSC]").val();
        }
        //if ($("[name=cmbSC]").val() == "" || $("[name=cmbSC]").val() == "undefined")
        //    spv = "";
        //else
        //    spv = $("[name=cmbSC]").val();

        if ($("[name=cmbSM]").val() == "" || $("[name=cmbSM]").val() == "undefined")
            emp = "";
        else
            emp = $("[name=cmbSM]").val();

        console.log(Branch, outlet, bm, spv);

        if ($("#Detail").prop('checked') == true)
        {
            if (!isCEO)
            {
                report = "PmRpInqByPeriodDet"
                par = [
                    'companycode',
                    Branch,
                    firstPeriod,
                    endPeriod,
                    outlet, spv, emp
                ];
                //viewer = new XReportViewer("PmRpInqByPeriodDet", user.CompanyCode, user.BranchCode,
                //                            dtpAwal.Value.ToString("yyyyMMdd"), dtpAkhir.Value.ToString("yyyyMMdd"),
                //                            outlet, spv, emp);
            }
            else
            {
                report = "PmRpInqByPeriodDet"
                par = [
                     'companycode',
                    Branch,
                    firstPeriod,
                    endPeriod,
                    bm, spv, emp
                ];
                //viewer = new XReportViewer("PmRpInqByPeriodDet", user.CompanyCode, "",
                //            dtpAwal.Value.ToString("yyyyMMdd"), dtpAkhir.Value.ToString("yyyyMMdd"),
                //            outlet, spv, emp);
            }
        }
        else
        {
            if (!isCEO)
            {
                report = "PmRpInqPeriodeWeb"
                par = [
                    'companycode',
                    Branch,
                    firstPeriod,
                    endPeriod,
                    bm, spv, emp
                ];
                //viewer = new XReportViewer("PmRpInqByPeriod", user.CompanyCode, user.BranchCode,
                //                            dtpAwal.Value.ToString("yyyyMMdd"), dtpAkhir.Value.ToString("yyyyMMdd"),
                //                            outlet, spv, emp);                    
            }
            else
            {
                report = "PmRpInqPeriodeWeb"
                par = [
                    'companycode',
                    Branch,
                    firstPeriod,
                    endPeriod,
                    bm, spv, emp
                ];
                //viewer = new XReportViewer("PmRpInqByPeriod", user.CompanyCode, "",
                //            dtpAwal.Value.ToString("yyyyMMdd"), dtpAkhir.Value.ToString("yyyyMMdd"),
                //            outlet, spv, emp);
            }
        }
        var tanggal = moment(me.data.dtpFrom).format("DD-MMM-YYYY") + " S/D " + moment(me.data.dtpTo).format("DD-MMM-YYYY");
        var param1, param2, param3;

        if ($("[name=cmbBranch]").val() == "" || $("[name=cmbBranch]").val() == "undefined")
            OutletName = "SEMUA";
        else
            OutletName = $("[name=cmbBranch]").select2("data").text;

        console.log(OutletName);
        if ($("[name=cmbSH]").val() == "" || $("[name=cmbSM]").val() == "undefined")
            param1 = "SEMUA";
        else
            param1 = $("[name=cmbSH]").select2("data").text;//$("[name=cmbSH]").val();

        if ($("[name=cmbSC]").val() == "")
            param2 = "SEMUA" ;
        else
            param2 = "SEMUA";
            //param2 = $("[name=cmbSC]").val()
        if ($("[name=cmbSM]").val() == "" || $("[name=cmbSM]").val() == "undefined")
            param3 = "SEMUA";
        else
            param3 = $("[name=cmbSM]").select2("data").text;

        param1 = param1.replace(',', '.');
        param2 = param2.replace(',', '.');
        param3 = param3.replace(',', '.');
        var param5 = pType == '4W' ? "Nama Sales Head" : "Nama Sales Kordinator";
        rparam = [tanggal, param1, param2, param3, param5];
        if ($("#Detail").prop('checked') == true) {
            var url = "its.api/Inquiry/IndByPeriode?";
            var params = "&firstPeriod=" + firstPeriod;
            params += "&endPeriod=" + endPeriod;
            params += "&outlet=" + Branch;
            params += "&bm=" + bm;
            params += "&spv=" + spv;
            params += "&emp=" + emp;
            params += "&ReportId=" + report;
            params += "&tanggal=" + tanggal;
            params += "&param1=" + param1;
            params += "&param2=" + param2;
            params += "&param3=" + param3;
            params += "&outletname=" + OutletName;
            url = url + params;
            window.location = url;
        } else {
            Wx.showPdfReport({
                id: report,
                pparam: par,
                rparam: rparam,
                type: "devex"
            });
        }

        
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry - By Periode",
        xtype: "panels",
        toolbars: [
                    { name: "btnPrint", cls: "btn btn-primary", text: "Print", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        name:'pnlFilter',
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
                            { name: "Detail", type: "ng-check" },
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
                        name: "divSalesHead",
                        text: "Sales Head",
                        cls: "span5",
                        show: "isComboShow",
                        type: "controls", items: [
                            { name: "cmbSH", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSH", show: "isComboShow"},
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
                        cls: "span2",
                        items: [
                            { name: "btnSearch", cls: "btn btn-info", icon: "icon-search", text: "Search", click: "ByPeriodePivote()", style: "width:120px;" }
                            //{ name: "btnSearch", cls: "btn btn-info", icon: "icon-search", text: "Search", click: "btnSearchClick()", style: "width:120px;" }
                        ]
                    },
                    {
                        text: "Salesman",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "cmbSM", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSM" }
                        ]
                    },
                    //{
                    //    text: "Salesman",
                    //    cls: "span5",
                    //    type: "controls", items: [
                    //        //{ name: "cmbSM", type: "select2", opt_text: "[SELECT ALL]", datasource: "comboSM" }
                    //        { name: "cmbSM", type: "popup", click: "Salesman()", cls: "span2" },
                    //        { name: "cmbSMName", type: "text", cls: "span6", readonly: true }
                    //    ]
                    //},
                    
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
            {
                name: "gridbyperiode",
                xtype: "k-grid",
                cls: "expand"
            },
            //{
            //    title: "List By Periode",
            //    items: [
            //        //{
            //        //    name: "wxbyperiode",
            //        //    title: "List By Periode",
            //        //    type: "wxdiv"
            //        //},
                    
            //    ]
            //}
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("byperiode");
    }
});