$(document).ready(function () {
    var isBranch;
    var outTotal;
    var outJobInfo;
    var isUserHolding = "";

    var options = {
        title: 'Get KSG From SPK',
        xtype: 'panels',
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnSave', text: 'Save', icon: 'icon-save', cls: 'hide' },
            { name: 'btnDelete', text: 'Delete', cls: 'hide'},
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnQuery', text: 'Query', icon: 'icon-search' },
            { name: 'btnPrint', text: 'Print', icon: 'icon-print' }
        ],
        panels: [
              {
                  name: "pnlMaintenanceSPK",
                  title: "Maintenance SPK",
                  items: [
                      { name: "GenerateNo", text: "PDI / FSC No.", placeHolder: "FSC/XX/YYYYYY", type: "popup", cls: "span4", readonly: true },
                      { name: "GenerateDate", text: "PDI / FSC Date", cls: "span4", type: "datepicker" },
                      { name: "BranchFrom", text: "Branch", cls: "span4", type: "popup", readonly: true, required: true },
                      { name: "BranchTo", text: "s/d", cls: "span4", type: "popup", readonly: true, required: true },
                      { name: "PeriodeDateFrom", text: "Periode", type: "datepicker", cls: "span4" },
                      { name: "PeriodeDateTo", text: "s/d", type: "datepicker", cls: "span4" },
                      { name: "isPDI", text: "PDI / PDC", type: "switch", cls: "span4", readonly: true },
                      { name: "isFSC", text: "FSC / KSG", type: "switch", cls: "span4", readonly: true },
                  ]
              },
               {
                   title: "Total Amount Info",
                   xtype: "table",
                   tblname: "tblTotal",
                   columns: [
                       { name: "RecNo", text: "No", width: 20 },
                       { name: "BasicModel", text: "Basic Model", width: 50 },
                       { name: "PdiFscSeq", text: "FS#", width: 20 },
                       { name: "RecCount", text: "Amount", width: 20, type: "numeric", cls: "text-right" },
                       { name: "PdiFscAmount", text: "Total", width: 50, type: "numeric", cls: "text-right" }
                   ]
               },
               {
                   title: "Job Order Info",
                   xtype: "table",
                   tblname: "tblInfo",
                   showcheckbox: true,
                   columns: [
                       { name: "No", text: "No" },
                       { name: "BranchCode", text: "Branch" },
                       { name: "JobOrderNo", text: "Job Order No." },
                       { name: "JobOrderDate", text: "Job Order Date", type: "date" },
                       { name: "BasicModel", text: "Basic Model" },
                       { name: "ServiceBookNo", text: "Service Book No" },
                       { name: "PdiFscSeq", text: "FS#" },
                       { name: "Odometer", text: "Odometer", type: "numeric", cls: "text-right" },
                       { name: "LaborGrossAmt", text: "Labor", type: "numeric", cls: "text-right" },
                       { name: "MaterialGrossAmt", text: "Material", type: "numeric", cls: "text-right" },
                       { name: "PdiFscAmount", cls: "hide" },
                       { name: "FakturPolisiDate", text: "Polisi Date", type: "date" },
                       { name: "BPKDate", text: "Delivery Date", type: "date" },
                       { name: "ChassisCode", text: "Chassis Code" },
                       { name: "ChassisNo", text: "Chassis No" },
                       { name: "EngineCode", text: "Engine Code" },
                       { name: "EngineNo", text: "Engine No" },
                       { name: "InvoiceNo", cls: "hide" },
                       { name: "FPJNo", cls: "hide" },
                       { name: "FPJDate", type: "date", cls: "hide" },
                       { name: "FPJGovNo", cls: "hide" },
                       { name: "TransmissionType", cls: "hide" },
                       { name: "ServiceStatus", cls: "hide" },
                       { name: "CompanyCode", cls: "hide" },
                       { name: "ProductType", cls: "hide" },
                   ]
               }
        ]
    }

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/KSGSPK/default', function (result) {
            isUserHolding = result.IsUserHolding;
            $('#isFSC').val(true);
            if (!result.isEnable) {
                widget.alert(result.message);
                $('#pnlMaintenanceSPK button, #pnlMaintenanceSPK input, .toolbar button').attr('disabled', 'disabled');
            }
            else {
                widget.default = result;
                widget.populate(result);
                $('#BranchFrom, #BranchTo').val('');
                $('#btnQuery').prop('disabled', false);
            }

            if (!result.IsUserHolding) {
                $('#btnBranchFrom, #btnBranchTo').prop('disabled', true);
                $('#BranchFrom').val(result.BranchFrom)
                $('#BranchTo').val(result.BranchTo);
            }
            else {
                $('#btnQuery, #btnBranchFrom, #btnBranchTo').prop('disabled', false);
            }
        });
    });

    $("#isPDIN").on('change', function (e) {
        var value = true;
        $("#isFSCY").prop('checked', value).val(value);
        $("#isFSCN").prop('checked', !value).val(value);
    });
    $("#isPDIY").on('change', function (e) {
        var value = false;
        $("#isFSCY").prop('checked', value).val(value);
        $("#isFSCN").prop('checked', !value).val(value);
    });

    $("#isFSCN").on('change', function (e) {
        var value = true;
        $("#isPDIY").prop('checked', value).val(value);
        $("#isPDIN").prop('checked', !value).val(value);
    });
    $("#isFSCY").on('change', function (e) {
        var value = false;
        $("#isPDIY").prop('checked', value).val(value);
        $("#isPDIN").prop('checked', !value).val(value);
    });

    $('#btnBranchFrom, #btnBranchTo').on('click', function (e) {
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.lookup.init({
            name: this.id == "btnBranchFrom" ? "0" : "1",
            title: "Select Branch",
            source: 'sv.api/grid/branch',
            columns: [
                { mData: 'BranchCode', sTitle: 'Branch Code', sWidth: '110px' },
                { mData: 'CompanyName', sTitle: 'Company Name' }
            ]
        });
        widget.lookup.show();
    });

    $("#btnBrowse, #btnGenerateNo").on("click", function () {
        widget.lookup.init({
            name: "TransPDIFSC",
            title: "Transaction - PDI FSC Lookup",
            source: "sv.api/grid/AllBranchFromSPKNew",
            sortings: [[1, "desc"]],         
            columns: [
                { mData: "BranchCode", sTitle: "Branch Code", sWidth: "110px" },
                { mData: "GenerateNo", sTitle: "PDI FSC No.", sWidth: "110px" },
                {
                    mData: "GenerateDate", sTitle: "PDI FSC Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return data == null ? "-" : moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "Invoice", sTitle: "No Invoice" },
                { mData: "FPJNo", sTitle: "Tax Invoice No.", sWidth: "300px" },
                {
                    mData: "FPJDate", sTitle: "Tax Invoice Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return data == null ? "-" : moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "FPJGovNo", sTitle: "Tax Invoice GOV No." },
                { mData: "TotalNoOfItem", sTitle: "Total Record" },
                { mData: "TotalAmt", sTitle: "Total PDI FSC" },
                { mData: "SenderDealerName", sTitle: "Sender", sWidth: "300px" },
                { mData: "RefferenceNo", sTitle: "Refference No" },
                {
                    mData: "RefferenceDate", sTitle: "Refference Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return data == null ? "-" : moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "PostingFlagDesc", sTitle: "Status", sWidth: "200px" }
            ],
        });
        widget.lookup.show();
        widget.showToolbars(["btnNew", "btnBrowse", "btnDelete", "btnPrint"]);
    });


    $("#btnPrint").on('click', function () {
        
        widget.loadForm();
        widget.showForm({ url: "sv/trans/ksgspkprint" });
        
    })
    
    widget.lookup.onDblClick(function (e, data, name) {
        switch (name) {
            case "TransPDIFSC":
                $('#GenerateNo').val(data.GenerateNo);
                data.IsUserHolding = isUserHolding;
                $.post('sv.api/ksgspk/getspk', data, function (result) {
                    var total = result.total.concat(result.sumtotal);

                    widget.populateTable({ selector: "#tblTotal", data: total });
                    widget.populateTable({ selector: "#tblInfo", data: result.jobinfo, selectable: false, multiselect: false });

                    widget.populate(result.forms)
                    $('#btnBranchFrom, #btnBranchTo, #btnGenerateNo').prop('disabled', true);
                });
                break;
            case "TblOutstanding":
                widget.populateTable({ selector: "#tblTotal", data: outTotal });
                widget.populateTable({ selector: "#tblInfo", data: outJobInfo,  selectable: true, multiselect: true});
                widget.showToolbars(["btnNew", "btnSave"]);
                break;
            default:
                if (name == "0") {
                    $('#BranchFrom').val(data.BranchCode);
                    if ($('#BranchFrom').val() == "") {
                        $('#BranchTo').val("");
                    }
                    else {
                        if ($('#BranchTo').val() == "" || parseFloat($('#BranchFrom').val()) > parseFloat($('#BranchTo').val())) {
                            $('#BranchTo').val($('#BranchFrom').val());
                        }
                    }
                }
                else {
                    $('#BranchTo').val(data.BranchCode);
                    if ($('#BranchTo').val() == "") {
                        $('#BranchFrom').val("");
                    }
                    else {
                        if ($('#BranchFrom').val() == "" || parseFloat($('#BranchFrom').val()) > parseFloat($('#BranchTo').val())) {
                            $('#BranchFrom').val($('#BranchTo').val());
                        }
                    }
                }
                break;
        }
        widget.lookup.hide();
    });

    $('input[name="PeriodeDateFrom"]').on('change', function (e) {

        //console.log($('input[name="PeriodeDateFrom"]').val());
        var from = Date.parse($('input[name="PeriodeDateFrom"]').val().substr(6,13));
        var to = Date.parse($('input[name="PeriodeDateTo"]').val().substr(6,13));
        if ($("PeriodeDateTo").val() === "" || from > to) {
            $('#PeriodeDateTo').val($('input[name="PeriodeDateFrom"]').val());
        }
    });

    $('input[name="PeriodeDateTo"]').on('change', function (e) {

        //console.log($('input[name="PeriodeDateTo"]').val());
        var from = Date.parse($('input[name="PeriodeDateFrom"]').val().substr(6, 13));
        var to = Date.parse($('input[name="PeriodeDateTo"]').val().substr(6, 13));
        if (from > to) {
            $('#PeriodeDateFrom').val($('input[name="PeriodeDateTo"]').val());
        }
    });

    $('#btnQuery').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main .gl-widget").serializeObject();
            $.post('sv.api/ksgspk/cekspk', data, function (result) {
                if (result.success) {
                    outJobInfo = result.jobinfo;
                    outTotal = result.total.concat(result.sumtotal);
                    widget.lookup.init({
                        name: "TblOutstanding",
                        title: "Terdapat SPK yang statusnya belum Tutup SPK, apakah proses akan dilanjutkan ? ",
                        additionalParams: [
                            { name: "BranchFrom", value: data.BranchFrom },
                            { name: "BranchTo", value: data.BranchTo },
                            { name: "PeriodeDateFrom", value: data.PeriodeDateFrom },
                            { name: "PeriodeDateTo", value: data.PeriodeDateTo },
                            { name: "isPDI", value: data.isPDI },
                            { name: "isFSC", value: data.isFSC },
                        ],
                        source: 'sv.api/ksgspk/spkoutstanding',
                        sortings: [[0, "asc"]],
                        columns: [
                            { mData: "BranchCode", sTitle: "Branch Code", sWidth: "110px" },
                            { mData: "JobOrderNo", sTitle: "Job Order No.", sWidth: "110px" },
                            {
                                mData: "JobOrderDate", sTitle: "Job Order Date", sWidth: "130px",
                                mRender: function (data, type, full) {
                                    return moment(data).format('DD-MMM-YYYY');
                                }
                            },
                            { mData: "PoliceRegNo", sTitle: "Police No.", sWidth: "110px" },
                            { mData: "BasicModel", sTitle: "Basic Model", sWidth: "110px" },
                            { mData: "JobType", sTitle: "Job Type.", sWidth: "110px" },
                            { mData: "EmployeeName", sTitle: "SA", sWidth: "110px" },
                            { mData: "Status", sTitle: "Status", sWidth: "110px" },
                        ]
                    });
                    widget.lookup.show();

                    widget.showNotification(result.message);
                    outJobInfo = result.jobinfo;
                    outTotal = result.total.concat(result.sumtotal);

                    //var total = result.total.concat(result.sumtotal);
                    widget.populateTable({ selector: "#tblTotal", data: outTotal });
                    widget.populateTable({ selector: "#tblInfo", data: outJobInfo, selectable: true, multiselect: true });

                    widget.showToolbars(["btnNew", "btnSave", "btnBrowse", "btnQuery"]);
                }
                else {
                    widget.showNotification(result.message);
                    outJobInfo = result.jobinfo;
                    outTotal = result.total.concat(result.sumtotal);

                    //var total = result.total.concat(result.sumtotal);

                    widget.populateTable({ selector: "#tblTotal", data: outTotal });
                    widget.populateTable({ selector: "#tblInfo", data: outJobInfo, selectable: true, multiselect: true });

                    widget.showToolbars(["btnNew", "btnSave", "btnBrowse", "btnQuery"]);
                }
            });
        }
        else {
            MsgBox('Ada informasi yang belum lengkap');
            return;
        }
    });

    $("#btnNew").on("click", function () {
        clear();
    });

    $('#btnSave').on('click', function (e) {
        var data = [];
        var nModel = [];
        data[0] = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            return td.eq(2).text();
        }).get();
        data[1] = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            return td.eq(3).text();
        }).get();
        data[2] = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            return td.eq(24).text();
        }).get();
        data[3] = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            return td.eq(25).text();
        }).get();


        //Old Coding

        //for (var i = 0; i <= 26 ; i++) {
        //xz    data[i] = $('.row_selected').map(function (idx, el) {
        //        var td = $(el).find('td');

        //        return td.eq(i).text();

        //    }).get();
        //}

        //End Old coding

       
        var sampleLen = data[0].length;
        for (var i = 0; i < sampleLen; i++) {
            var myModel = new Model2(data, i);
            nModel.push((myModel));
        }

        
        console.log(data)

        if (nModel.length == 0) { alert("Pilih Data yang mau disave terlebih dahulu.") }
        else {
            var dataModel = [];
            dataModel["modelFSC"] = $(".main .gl-widget").serializeObject();
            var params = {};
            var params = {
                data: JSON.stringify(nModel),
            };
            params["modelFSC"] = $(".main .gl-widget").serializeObject();
            
            var dat = JSON.stringify(params);

             $.ajax({
                 type: 'POST',
                 url: 'sv.api/ksgspk/Save',
                 data: dat,//JSON.stringify({ data: JSON.stringify(nModel) }),
                 contentType: 'application/json; charset=utf8',
                 dataType: 'json',
                 success: function (result) {
                     if (result.success) {
                         $('#GenerateNo').val(result.genNo);
                         var data = { "generateNo": result.genNo };
                         $.post('sv.api/ksgspk/getspk', data, function (result) {
                             var total = result.total.concat(result.sumtotal);

                             widget.populateTable({ selector: "#tblTotal", data: total });
                             widget.populateTable({ selector: "#tblInfo", data: result.jobinfo, selectable: false, multiselect: false });

                             widget.populate(result.forms)
                             widget.showNotification(result.message);
                         });
                     }
                     else {
                         widget.alert(result.message);
                     }
                 }
             });
         }
    });

    $('#btnDelete').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();
        widget.confirm("Do you want to delete this data?", function (result) {
            if (result == "Yes") {
                $.post('sv.api/ksgspk/delete', data, function (result) {
                    if (result.success != "")
                    {
                        widget.Info(result.message);
                        clear();
                    }
                });
            }
        });
    });

    function clear() {
        widget.clearForm();
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.populateTable({ selector: "#tblTotal", data: {} });
        widget.populateTable({ selector: "#tblInfo", data: {} });
        $('#btnBranchFrom, #btnBranchTo, #btnGenerateNo').prop('disabled', false);
        widget.populate(widget.default);
    }
});

function Model(data, row) {
    this.BranchCode = data[2][row];
    this.JobOrderNo = data[3][row];
    this.JobOrderDate = data[4][row];
    this.BasicModel = data[5][row];
    this.ServiceBookNo = data[6][row];
    this.PdiFscSeq = data[7][row];
    this.Odometer = data[8][row];
    this.LaborGrossAmt = data[9][row];
    this.MaterialGrossAmt = parseInt(data[10][row]);
    this.PdiFscAmount = data[11][row];
    this.FakturPolisiDate = data[12][row];
    this.BPKDate = data[13][row];
    this.ChassisCode = data[14][row];
    this.ChassisNo = data[15][row];
    this.EngineCode = data[16][row];
    this.EngineNo = data[17][row];
    this.InvoiceNo = data[18][row];
    this.FPJNo = data[19][row];
    this.FPJDate = data[20][row];
    this.FPJGovNo = data[21][row];
    this.TransmissionType = data[22][row];
    this.ServiceStatus = data[23][row];
    this.CompanyCode = data[24][row];
    this.ProductType = data[25][row];
}

function Model2(data, row) {
    this.BranchCode = data[0][row];
    this.JobOrderNo = data[1][row];
    this.CompanyCode = data[2][row];
    this.ProductType = data[3][row];
}
