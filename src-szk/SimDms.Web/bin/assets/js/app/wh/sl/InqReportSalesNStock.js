$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: 'Generate Sales & Stock',
        xtype: 'panels',
        toolbars: [
            { name: "btnProcess", text: "Generate Excel", icon: "fa fa-file-excel-o" }
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                  
                    {
                        text: "Area / Dealer",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Area', text: 'Area', type: 'select', cls: 'span2'},
                            { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5'}
                        ]
                    },
                    {
                        text: "Showroom",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span7'}
                        ]
                    },
                    {
                        text: "Series / Model",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'GroupModel', text: 'GroupModel', type: 'select', cls: 'span2'},
                            { name: "ModelType", text: "ModelType", cls: "span4", type: "select"}
                        ]
                    },
                     {
                         text: 'Year',
                         type: 'controls',
                         items: [
                             //{ name: 'Year', text: 2014, type: 'intt', cls: 'span2' }
                               { name: "Year", text: "Year", type: "select", cls: "span2" }
                         ]
                     }
                ]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            }
        ],
       
    });

    widget.render(function () {
        renderCallback();
        var date = new Date();
        date = date.getFullYear();
       // alert(date);
        var initial = {
            Year: date,
            Area :100 
        };
        setTimeout(function () { widget.populate(initial) }, 0);
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
                url: "wh.api/Combo/CompaniesR2",
                optionalText: "--ALL DEALER --",
                cascade: {
                    name: "Area"
                }
            },
            {
                name: "Outlet",
                url: "wh.api/Combo/BranchesR2",
                optionalText: "-- ALL OUTLET --",
                cascade: {
                    name: "Dealer",
                    additionalParams: [
                        { name: "Area", source: "Area", type: "select" },
                        { name: "Dealer", source: "Dealer", type: "select" }
                    ]
                }
            },
             {
                 name: "GroupModel",
                 url: "wh.api/inquiryprod/groupmodelsr2",
                 optionalText: "--ALL SERIES--"
             },
               {
                   name: "ModelType",
                   url: "wh.api/inquiryprod/modeltypesr2",
                   optionalText: "--ALL MODEL--",
                   cascade: {
                       name: "GroupModel",
                       additionalParams: [
                           { name: "GroupModel", source: "GroupModel", type: "select" }
                       ]
                   }
               },
            {
                name: "Year",
                url: "wh.api/combo/years",
               optionalText: "-- SELECT YEAR --"
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
       var url = "wh.api/inquiryprod/GenerateSLSalesNStock?";
        var params = "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="Dealer"]').val();
        params += "&Outlet=" + $('[name="Outlet"]').val();
        params += "&GroupModel=" + $('[name="GroupModel"]').val();
        params += "&ModelType=" + $('[name="ModelType"]').val();
        params += "&Year=" + $('[name="Year"]').val();
        url = url + params;
        window.location = url;
        
        //window.location.href = "wh.api/inquiryprod/GenerateSLSalesNStock?DateFrom=" + $("[name=DateFrom]").val() + '&DateTo=' + $("[name=DateTo]").val() + '&Area=' + $("[name=Area]").val()
        //    + '&Dealer=' + $("[name=Dealer]").val() + '&Outlet=' + $("[name=Outlet]").val() + '&GroupModel=' + $("[name=GroupModel]").val()
        //    + '&ModelType=' + $("[name=ModelType]").val() + '&Variant=' + $("[name=Variant]").val() + '&Report=' + $("[name=Report]").val()
        //    + '&ReportType=' + $("[name=ReportType]").val();
    });

    //function exportToExcel() {
    //    var url = "wh.api/inquiryprod/GenerateSLSalesNStock?";
    //    var params = "&Area=" + $('[name="Area"]').val();
    //    params += "&Dealer=" + $('[name="Dealer"]').val();
    //    params += "&Outlet=" + $('[name="Outlet"]').val();
    //    params += "&Groupmodel=" + $('[name="Groupmodel"]').val();
    //    params += "&ModelType=" + $('[name="ModelType"]').val();
    //    params += "&Year=" + $('[name="Year"]').val();
    //    url = url + params;
    //    window.location = url;
    //}
});
