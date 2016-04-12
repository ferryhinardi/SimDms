$(document).ready(function () {
    var options = {
        title: "Template",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details - Vehicle",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2 ticket", placeHolder: "Company Code", readonly: true },
                            { name: "CustomerCode", cls: "span6 ticket", placeHolder: "Dealer Name", readonly: false }
                        ]
                    },
                    { name: "Chassis", text: "Chassis" },
                    { text: "Date1", cls: "span4", type: "datepicker" },
                    { text: "Date2", cls: "span4", type: "datepicker" },
                ]

            },
            {
                items: [
                    { text: "Company Code" },
                    { text: "Customer" },
                    { text: "Date1", cls: "span4", type: "datepicker" },
                    { text: "Date2", cls: "span4", type: "datepicker" },
                ]

            },
        ],
        toolbars: [
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnClear", text: "Clear", icon: "icon-undo", cls: "hide" },
        ],
    };

    var widget = new SimDms.Widget(options);
    widget.render();

    widget.lookup.onDblClick(function (e, data, name) {
        console.log(data);
        widget.lookup.hide();
        widget.populate(data);

        //widget.post("cs.api/customer/GetTDayCall", data, function (result) {
        //    if (result.success) {
        //        widget.lookup.hide();
        //        widget.populate(data);
        //        $(".toolbar > button").hide();
        //        $("#btnBrowse,#btnSave,#btnCancel").show();
        //    }
        //});
    });

    $("#btnBrowse").on("click", function () {
        widget.lookup.init({
            name: "CustList",
            title: "Customer Buy List",
            source: "cs.api/grid/customers",
            columns: [
                { mData: "CustomerCode", sTitle: "Cust Code", sWidth: "110px" },
                { mData: "CustomerName", sTitle: "Cust Name" },
                { mData: "PoliceRegNo", sTitle: "No Polisi" },
                { mData: "Chassis", sTitle: "Chassis" },
            ]
        });
        widget.lookup.show();
    });
});