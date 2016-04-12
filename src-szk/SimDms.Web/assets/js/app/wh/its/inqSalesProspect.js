$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: 'Generate ITS',
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
                            { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --'},
                            { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- ALL DEALER --' }
                        ]
                    },
                    {
                        text: "Showroom",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span7', opt_text: '-- ALL OUTLET --' }
                        ]
                    },
                    {
                        text: "Series / Model",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'GroupModel', text: 'GroupModel', type: 'select', cls: 'span2', opt_text: '-- ALL SERIES --' },
                            { name: "ModelType", text: "ModelType", cls: "span4", type: "select", opt_text: '-- ALL MODEL --' }
                        ]
                    },
                     {
                         text: 'Year',
                         type: 'controls',
                         items: [
                             //{ name: 'Year', text: 2014, type: 'int', cls: 'span2' }
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
        
        var date = new Date();
        date = date.getFullYear();
        // alert(date);
        var initial = {
            Year: date,
            Area:100

        };
        setTimeout(function () { widget.populate(initial) }, 0);
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
                   optionalText: "--SELECT ONE--",
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

    //widget.post("wh.api/Combo/BranchesR2/?id=" + Area + "&dealer=" + id, function (Area) {
    //    $("[name=BranchCode]").val(Area);
    //});
    function initElementEvent() {
        $('[name="Area"]').on('change', function () {
            $('[name="Outlet"]').val('');
        });
    }

    $('#btnProcess').on('click', function (e) {
       var url = "wh.api/inquiryprod/GenerateItsSalesProspect?";
       var params = "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="Dealer"]').val();
        params += "&Outlet=" + $('[name="Outlet"]').val();
        params += "&GroupModel=" + $('[name="GroupModel"]').val();
        params += "&ModelType=" + $('[name="ModelType"]').val();
        params += "&Year=" + $('[name="Year"]').val();
        url = url + params;
        window.location = url;   
    });
});
