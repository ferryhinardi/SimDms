$(document).ready(function () {
    var options = {
        title: "Maintain Invoice Tax",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" }
        ], 
        panels: [
            {
                title: "Invoice Information",
                items: [
                        {
                            text: "Branch",
                            type: "controls",
                            items: [
                                { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                                { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                            ]
                        },

                        { name: "FPJNo", text: "Invoice Tax No", cls: "span4", type: "popup", btnName: "btnFPJNo" },
                        { name: "FPJDate", text: "Invoice Tax Date", cls: "span4", readonly: true, type: "date" },
                        { name: "FPJGovNo", text: "Invoice Tax Gov. No", readonly: true },
                        {
                            text: "Customer Bill",
                            type: "controls",
                            items: [
                                { name: "CustomerCode", text: "Customer Bill", cls: "span2", type: "popup", btnName: "btnCustCodeBill" },
                                { name: "CustomerName", cls: "span6", readonly: true },
                            ]
                        },

                        { name: "Address1", text: "Address", readonly: true },
                        { name: "Address2", readonly: true },
                        { name: "Address3", readonly: true },
                        { name: "Address4", readonly: true },
                        { name: "PhoneNo", text: "Phone No", cls: "span4", readonly: true },
                        { name: "HPNo", text: "Mobile No", cls: "span4", readonly: true },

                        { name: "NPWPNo", text: "NPWP No", cls: "span4", readonly: true },
                        { name: "NPWPDate", text: "NPWP Date", cls: "span4", readonly: true, type: "date" },

                        { name: "SKPNo", text: "NPPKP No", cls: "span4", readonly: true },
                        { name: "SKPDate", text: "NPPKP Date", cls: "span4", readonly: true, type: "date" },
                    ]
                }
            ]
    }
   
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post("sv.api/maintainfpj/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate($.extend({}, widget.default, data));
        console.log(data);
        $("#btnSave").show();
        widget.lookup.hide();
    });

    $("#btnNew").on("click", function () {
        $("#btnSave").hide();
        widget.clearForm();
        widget.populate(widget.default);
    });

    $("#btnFPJNo, #btnBrowse").on("click", function () {
        widget.lookup.init({
            name: "InvoiceList",
            title: "Invoice List",
            source: "sv.api/grid/invoicesmaintainfpj",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "InvoiceNo", sTitle: "Invoice No", sWidth: "110px" },
                {
                    mData: "InvoiceDate", sTitle: "Invoice Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { mData: "FPJNo", sTitle: "Invoice Tax No" },
                 {
                     mData: "FPJDate", sTitle: "Invoice Tax Date", sWidth: "130px",
                     mRender: function (data, type, full) {
                         return moment(data).format('DD MMM YYYY');
                     }
                 },
                { mData: "JobOrderNo", sTitle: "Job Order No" },
                
                {
                    mData: "JobOrderDate", sTitle: "Job Order Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
            ]
        });
        widget.lookup.show();
    });

    $("#btnCustCodeBill").on("click", function () {
        if ($('#FPJNo').val() == "") {
            
            return;
        }

        widget.lookup.init({
            name: "CustTaxInvoice",
            title: "Cust Tax Invoice",
            source: "sv.api/grid/customers",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "CustomerCode", sTitle: "Customer Code", sWidth: "110px" },
                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "130px" }
                ]
        });
        widget.lookup.show();
    });

    $("#btnSave").on("click", function () {
        if (confirm("Apakah anda yakin ?")) {
            $.post('sv.api/maintainfpj/save/?fpjno=' + $('#FPJNo').val() + '&customerCodeBill=' + $('#CustomerCode').val(), function (result) {
                if (result.success) {
                    widget.populate(result);
                    alert(result.message);
                }
                else {
                    alert(result.message);
                }
            });
        };
    });
});