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
                        text: "Dealer",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: "CompanyCode", type: "text", cls: "hide" },
                            { name: 'Dealer', text: 'Dealer', type: 'text', cls: 'span5', opt_text: '-- ALL DEALER --', readonly:true }
                        ]
                    },
                    {
                        text: "Showroom Name",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: "BranchCode", type: "text", cls: "hide" },
                            { name: 'Outlet', text: 'outlet', type: 'text', cls: 'span7', opt_text: '-- ALL OUTLET --', readonly: true }
                        ]
                    },
                    {
                        text: "Series / Model Type",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Series', text: 'Series', type: 'select', cls: 'span2', opt_text: '-- ALL Series --' },
                            { name: "ModelType", text: "ModelType", cls: "span4", type: "select", opt_text: '-- ALL Model --' }
                        ]
                    },
                     {
                         text: 'Year',
                         type: 'controls',
                         items: [
                          //{ name: 'Year', text: 2014, type: 'int', cls: 'span2' }
                         { name: 'Year', text: 2014, type: "select", cls: "span2", optionalText: "-- SELECT YEAR --" }
                         ]
                     }
                ]
            }
        ],

    });

    widget.render(function () {
        renderCallback();
        var date = new Date();
        date = date.getFullYear();
        // alert(date);
        var initial = {
            Year: date
        };
        setTimeout(function () { widget.populate(initial) }, 0);
    });

    function renderCallback() {
        reloadData();
        initElementState();
        //initElementEvent();
        
    }

    function reloadData() {
        widget.post("om.api/Report/default", function (result) {
            widget.default = result;
            widget.populate(widget.default);
        });
    }
    
    function initElementState() {
        var setSelectOptions = [
            //{
            //    name: "Dealer",
            //    url: "its.api/inquiry/dealermappingdealers",
            //    optionalText: "--SELECT ALL--",
            //    cascade: {
            //        name: "Area",
            //        additionalParams: [
            //            { name: "Area", source: "Area", type: "select" }
            //        ]
            //    }
            //},
            //{
            //    name: "Outlet", url: "its.api/inquiry/outlets",
            //    optionalText: "--SELECT ALL--",
            //    cascade: {
            //        name: "Dealer",
            //        additionalParams: [
            //            { name: "Area", source: "Area", type: "select" },
            //            { name: "Dealer", source: "Dealer", type: "select" }
            //        ]
            //    }
            //},
             {
                 name: "Series",
                 url: "om.api/Report/Series"
             },
               {
                   name: "ModelType",
                   url: "om.api/Report/ModelTypes",
                   cascade: {
                       name: "Series",
                       additionalParams: [
                           { name: "Series", source: "Series", type: "select" }
                       ]
                   }
               },
             {
                 name: "Year",
                 url: "om.api/Report/years"
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
        var url = "om.api/Report/GenerateOmSalesNStock?";
        var params = "&CompanyCode=" + $('[name="CompanyCode"]').val();
        params += "&BranchCode=" + $('[name="BranchCode"]').val();
        params += "&Series=" + $('[name="Series"]').val();
        params += "&ModelType=" + $('[name="ModelType"]').val();
        params += "&Year=" + $('[name="Year"]').val();
        url = url + params;
        window.location = url;
    });
});