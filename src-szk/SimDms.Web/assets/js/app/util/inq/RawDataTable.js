$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Raw Data Table",
        xtype: "panels",
        toolbars: [
            //{ action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            //{ action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { name: "btnProcess", text: "Generate Excel", icon: "fa fa-file-excel-o" }
            //{ action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: 'TableName', text: 'Table Name', type: 'select', cls: 'span5' }
                    //{
                    //    text: "Periode",
                    //    type: "controls", items: [
                    //        { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                    //        { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                    //    ]
                    //}
                ]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            },
        ],
        
    });

    widget.render(function () {
        var filter = {
            DateFrom: new Date(),
            DateTo: new Date()
        };
        widget.populate(filter);

        widget.post("wh.api/combo/GnCollDataLogFilter", {}, function (result) {
            widget.bind({
                name: 'TableName',
                text: '-- SELECT ALL --',
                data: result[2]
                //onChange: refresh
            });
        });

        //setTimeout(refresh, 1000);
    });

    $('#btnProcess').on('click', function (e) {
        var url = "wh.api/inquiryprod/RawData?";
        var params = "&TableName=" + $('[name="TableName"]').val();
        url = url + params;
        window.location = url;
    });
    
    
});


