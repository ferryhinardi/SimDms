var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

$(document).ready(function () {
    var options = {
        title: "Maintenance Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        name: "BatchNo",
                        cls: "span4",
                        text: "Batch No",
                        readonly: true,
                    },
                    {
                        name: "BatchDate",
                        text: "Batch Date",
                        cls: "span4",
                        readonly: true
                    },
                    {
                        name: "ReceiptNo",
                        text: "ReceiptNo",
                        cls: "span4",
                        readonly:true
                    },
                    {
                        name: "ReceiptDate",
                        text: "ReceiptDate",
                        cls: "span4",
                        readonly: true
                    },
                    {
                        name: "FPJNo",
                        text: "FPJ No",
                        cls: "span4",
                        readonly: true
                    },
                    {
                        name: "FPJDate",
                        text: "FPJ Date",
                        cls: "span4",
                        readonly: true
                    },
                    {
                        name: "FPJGovNo",
                        text: "FPJ Gov No",
                        readonly: true
                    },
                    
                ]
            },
            {
                name: "PnlTabel",
                cls: "hide",
                xtype: "table",
                tblname: "tblPart",
                columns: [
                    { name: "BranchCode", text: "BranchCode", width: 80 },
                    { name: "GenerateNo", text: "GenerateNo", width: 100 },
                    { name: "DocumentNo", text: "DocumentNo", width: 110 },
                    { name: "ServiceBookNo", text: "ServiceBookNo", width: 110 },
                    { name: "BasicModel", text: "BasicModel", width: 80 },
                    { name: "ChassisCode", text: "ChassisCode", width: 80 },
                    { name: "ChassisNo", text: "ChassisNo", width: 80 },
                    { name: "Odometer", text: "Odometer", width: 80, type: 'price' },
                    { name: "TransAmt", text: "TransAmt", width: 80, type: 'price' }
                ]
            }
            

        ],
    }

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/MaintainClaim/default', function (result) {
            widget.default = result;
            widget.populate(result);

        });
    });
    
    $("#btnBrowse").on("click", function () {
        isBrowse = true;
        var lookup = widget.klookup({
            name: "BrowseCLM",
            title: "Inquiry data CLM",
            url: "sv.api/grid/MaintainClaim?KsgClaim=CLM",
            serverBinding: true,
            pageSize: 12,
            columns: [
                { field: "BranchCode", title: "Branch Code", width: 130 },
                { field: "BatchNo", title: "Batch No.", width: 200 },
                {
                    field: "BatchDate", title: "Batch Date", width: 110,
                    template: "#= (BatchDate == undefined) ? '' : moment(BatchDate).format('DD MMM YYYY') #"
                },
                { field: 'ReceiptNo', title: 'Receipt No.', width: 250 },
                {
                    field: "ReceiptDate", title: "Receipt Date", width:110,
                    template: "#= (ReceiptDate == undefined) ? '' : moment(ReceiptDate).format('DD MMM YYYY') #"
                },
                { field: 'FPJNo', title: 'FPJ. No.', width: 160 },
                {
                    field: "FPJDate", title: "FPJ. Date", width: 110,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: 'FPJGovNo', title: 'FPJGov. No.', width: 160 }
            ],
        });

        lookup.dblClick(function (data) {
            console.log(data);
            widget.populate($.extend({}, widget.default, data));
            var date = new Date(parseInt(data["BatchDate"].substring(6, 20)));
            var date1 = new Date(parseInt(data["ReceiptDate"].substring(6, 20)));
            var date2 = new Date(parseInt(data["FPJDate"].substring(6, 20)));
            console.log(date);
            //var timeFormat = date.toLocaleDateString("dd-mm-yyyy") + "    " + date.toLocaleTimeString("HH:mm:ss");
            //var timeFormat1 = date1.toLocaleDateString("dd-mm-yyyy") + "    " + date1.toLocaleTimeString("HH:mm:ss");
            //var timeFormat2 = date2.toLocaleDateString("dd-mm-yyyy") + "    " + date2.toLocaleTimeString("HH:mm:ss");

            
            $("#BatchDate").val(moment(data.BatchDate).format('DD-MM-YYYY') + "    " + moment(data.BatchDate).format('HH:mm:ss'));
            $("#ReceiptDate").val(moment(data.ReceiptDate).format('DD-MM-YYYY') + "    " + moment(data.ReceiptDate).format('HH:mm:ss'));
            $("#FPJDate").val(moment(data.FPJDate).format('DD-MM-YYYY') + "    " + moment(data.FPJDate).format('HH:mm:ss'));
            clear("dbclick");
            widget.lookup.hide();
            getTable();
            $("#PnlTabel").removeClass('hide');
        });
    });

   
    $("#btnDelete").on("click", deleteData);

    $('#btnCreate').on('click', function (e) {
        clear("new");
    });

    $('#btnEdit').on('click', function (e) {
        clear("btnEdit");
    });
 
    function getTable() {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/MaintainClaim/getdatatable", param, function (result) {
            widget.populateTable({ selector: "#tblPart", data: result });

        });
    }

    function deleteData() {
        if (confirm("Apakah anda yakin???")) {
            var param = $(".main .gl-widget").serializeObject();
            widget.post("sv.api/MaintainClaim/deletedata", param, function (result) {
                if (result.success) {
                    SimDms.Success("data deleted...");
                    clear("new");
                } else {
                    SimDms.Error("fail deleted...");
                }
            });
        }
    }

    function clear(p) {
        if (p == "clear") {
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        } else if (p == "dbclick") {
            $("#btnEdit").removeClass('hide');
            $("#btnDelete").removeClass('hide');
        }  else if (p == "new") {
            clearData(); 
            $("#PnlTabel").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        } else if (p == "btnEdit") {
            $("#btnSave").removeClass('hide');
        }
    }
  
    function clearData() {
        widget.clearForm();
        widget.post("sv.api/MaintainClaim/default", function (result) {
            widget.default = $.extend({}, result);
            widget.populate(widget.default);
           
        });
    }


});