$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: 'Generate Sales Person Contribution',
        xtype: 'panels',
        toolbars: [
            { name: "btnProcess", text: "Generate Excel", icon: "fa fa-file-excel-o" }
        ],
        panels: [
            {
                name: "pnlFilter",
                                items: [
                                    { name: "DateFrom", text: "Period From", type: "datepicker", cls: "span3" },
                                    { name: 'Area', text: 'Area', type: 'select', cls: 'span3', opt_text: '-- ALL AREA --' },
                                    { name: "DateTo", text: "Period To", type: "datepicker", cls: "span3" },
                                    { name: "Dealer", text: "Dealer", type: "select", cls: "span4" },
                                    {
                                        name: "Filter", text: "Filter", type: "select", cls: "span3", items: [
                                            { value: "INQ", text: "Inquiry Date" },
                                            { value: "SPK", text: "SPK Date" },
                                            { value: "DO", text: "Do Date" },
                                            { value: "DELIVERY", text: "Delivery Date" }
                                        ],
                                        fullItem: true
                                    },
                                { name: "Outlet", text: "Outlet", type: "select", cls: "span5" },
                               ],
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            }
        ],

    });

    widget.render(function () {
        widget.post("wh.api/inquiryprod/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);
                //widget.select({ selector: "select[name=Filter]", url: "wh.api/inquiryprod/FilterGenerateITS", selected: "0" });
                //widget.select({ selector: "select[name=Area]", url: "wh.api/inquiryprod/dealermappingareas", optionalText: "--SELECT ALL--" });
            }
        });
        //var date = new Date();
        //date = date.getFullYear();
        //var initial = {
        //    //new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
        //    DateFrom: new Date(Date.Year, Date.Month,1),
        //    DateTo: new Date(),
        //};
        //setTimeout(function () { widget.populate(initial) }, 0);
        renderCallback();
    });

    function renderCallback() {
        initElementState();
        initElementEvent();
    }

    function initElementState() {
        var setSelectOptions = [

            {
                name: "Area",
                url: "wh.api/Combo/GroupAreas",
                optionalText: "-- ALL AREA --"
            },
            {
                name: "Dealer",
                url: "wh.api/Combo/ComboDealerList",
                optionalText: "--ALL DEALER --",
                cascade: {
                    name: "Area",
                    additionalParams: [
                        { name: "groupArea", source: "Area", type: "select" }
                    ]
                }
            },
            {
                name: "Outlet",
                url: "wh.api/Combo/ComboOutletList",
                optionalText: "-- ALL OUTLET --",
                cascade: {
                    name: "Dealer",
                    additionalParams: [
                        { name: "companyCode", source: "Dealer", type: "select" },
                    ]
                }
            }
        ];
        widget.setSelect(setSelectOptions);
    }

    function initElementEvent() {
        $('[name="Area"]').on('change', function () {
            $('[name="Outlet"]').val('');
        });
    }

    $('#btnProcess').on('click', function (e) {
        var url = "wh.api/inquiryprod/GenerateSalesPersonContribution?";
        var params = "&DateFrom=" + $('[name="DateFrom"]').val();
        params += "&DateTo=" + $('[name="DateTo"]').val();
        params += "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="Dealer"]').val();
        params += "&Outlet=" + $('[name="Outlet"]').val();
        params += "&Filter=" + $('[name="Filter"]').val();
       
        url = url + params;
        window.location = url;
    });
});


