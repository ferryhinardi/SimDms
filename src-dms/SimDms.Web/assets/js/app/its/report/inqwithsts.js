$(document).ready(function () {
    var options = {
        title: "Inquiry with Status",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Generate Excel", icon: "icon-bolt" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "PositionID", type: "text", cls: "hide" },
                    { name: "EmployeeID", type: "text", cls: "hide" },

                    { name: "DateFrom", text: "Periode From", cls: "span4", type: "kdatepicker" },
                    { name: "DateTo", text: "to", cls: "span4", type: "kdatepicker" },

                    { name: "Area", text: "Area", cls: "span4", type: "select" },
                    { name: "GroupModel", text: "Group Model", cls: "span4", type: "select" },

                    { name: "Dealer", text: "Dealer", cls: "span4", type: "select" },
                    { name: "ModelType", text: "Tipe Kendaraan", cls: "span4", type: "select" },

                    { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
                    { name: "Variant", text: "Variant", cls: "span4", type: "select" },

                    { name: "CompanyCode", cls: "hide", type: "text" },
                    { name: "BranchCode", cls: "hide", type: "text" },
                ]
            },
            {
                name: "pnlKGrid",
                xtype: "k-grid",
            },
        ]
    }

    var widget = new SimDms.Widget(options);

    var paramsSelect = [
        {
            name: "Area", url: "its.api/inquiry/dealermappingareas",
            optionalText: "--SELECT ALL--"
        },
         {
             name: "Dealer", url: "its.api/inquiry/dealermappingdealers",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Area",
                 additionalParams: [
                     { name: "Area", source: "Area", type: "select" }
                 ]
             }
         },
         {
             name: "Outlet", url: "its.api/inquiry/outlets",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Dealer",
                 additionalParams: [
                     { name: "Area", source: "Area", type: "select" },
                     { name: "Dealer", source: "Dealer", type: "select" }
                 ]
             }
         },
    ];

    widget.setSelect(paramsSelect);
    var nationalSLS = '';

    widget.render(function () {
        //$('#Area, #Dealer, #Outlet').attr('disabled', 'disabled');
        $(".frame").css({ top: 240 });
        widget.post("its.api/inquiry/default", function (result) {
            widget.default = result.data;
            widget.populate(widget.default);

            //loadData(result.PositionID, result.EmployeeID, result.NationalSLS);
            nationalSLS = result.NationalSLS;

            widget.select({ name: "Dealer", url: "its.api/inquiry/dealermappingdealers", optionalText: "--SELECT ALL--" });
            widget.select({ name: "Outlet", url: "its.api/inquiry/outlets", optionalText: "--SELECT ALL--" });
            widget.select({ name: "GroupModel", url: "its.api/combo/groupmodels", optionalText: "--SELECT ALL--" });
            widget.selectparam({ name: "ModelType", url: "its.api/combo/modeltypes", param: "GroupModel", optionalText: "--SELECT ALL--" });
            widget.selectparam({ name: "Variant", url: "its.api/combo/carvariants", param: "ModelType", optionalText: "--SELECT ALL--" });

            if (nationalSLS == "1") {
                $('#Dealer, #Outlet').attr('disabled', 'disabled');
            }
            else {
                widget.enable({ value: false, items: ["Area", "Dealer"] });
                widget.select({
                    name: "Area",
                    url: "its.api/inquiry/dealermappingareas",
                    selected: result.data.Area
                });

                widget.select({
                    name: "Dealer",
                    url: "its.api/inquiry/dealermappingdealers",
                    selected: result.data.Dealer
                });

            }

            if (result.data.IsBranch) {
                widget.enable({ value: false, items: ["Outlet"] });
                widget.select({
                    name: "Outlet",
                    url: "its.api/inquiry/outlets",
                    selected: result.data.Outlet
                });
            }            
        });
    });

    $('#Area').on('change', function (e) {
        if (nationalSLS == "1") {
            if ($('#Area').val() == "") {
                $('#Dealer, #Outlet').attr('disabled', 'disabled');
            }
            else {
                $('#Dealer').removeAttr('disabled', 'disabled');
            }
        }
    });

    $('#Dealer').on('change', function (e) {
        if (nationalSLS == "1") {
            if ($('#Dealer').val() == "") {
                $('#Outlet').attr('disabled', 'disabled');
            }
            else {
                $('#Outlet').removeAttr('disabled', 'disabled');
            }
        }
    });

    $('#btnProcess').on('click', function (e) {
        if (moment($("[name=DateFrom]").val(), "DD-MMM-YYYY").format("YYYYMMDD") > moment($("[name=DateTo]").val(), "DD-MMM-YYYY").format("YYYYMMDD")) {
            alert("Tanggal akhir harus lebih besar dari tanggal awal periode");
            return;
        }

        var dtStart = moment($("[name=DateFrom]").val(), "DD-MMM-YYYY").format("YYYYMMDD");
        var yearStart = dtStart.toString().substring(0, 4);
        var monthStart = dtStart.toString().substring(6, 4);
        var dtStart = moment($("[name=DateTo]").val(), "DD-MMM-YYYY").format("YYYYMMDD");
        var yearEnd = dtStart.toString().substring(0, 4);
        var monthEnd = dtStart.toString().substring(6, 4);       
        if (yearStart != yearEnd || monthStart != monthEnd) {
            alert("Bulan Periode harus sama");
        }

        window.location.href = "its.api/inquiry/CreateExcelInqWithSts?DateFrom=" + $("[name=DateFrom]").val() + '&DateTo=' + $("[name=DateTo]").val() + '&Area=' + $("[name=Area]").val()
            + '&Dealer=' + $("[name=Dealer]").val() + '&Outlet=' + $("[name=Outlet]").val() + '&GroupModel=' + $("[name=GroupModel]").val() + '&ModelType=' + $("[name=ModelType]").val() + '&Variant=' + $("[name=Variant]").val();

        //widget.showReport({
        //    id: "InqWithSts",
        //    par: [$("[name=CompanyCode]").val(), $("[name=BranchCode]").val(), $("[name=DateFrom]").val(), $("[name=DateTo]").val(), $("[name=DateFrom]").val(), $("[name=DateTo]").val(), $("[name=Area]").val(), $("[name=GroupModel]").val(), $("[name=ModelType]").val(), $("[name=Variant]").val(), 0 ],
        //    panel: "pnlKGrid",
        //    type: "export",
        //    filename: "InqWithStatus",
        //});

        //$("[name=DateFrom]").val() + '&DateTo=' + $("[name=DateTo]").val() + '&Area=' + $("[name=Area]").val()+ '&GroupModel=' + + '&ModelType=' + $("[name=ModelType]").val() + '&Variant=' + $("[name=Variant]").val()

        //widget.export({
        //    url: "its.api/inquiry/exportinqwithsts?DateFrom=" + $("[name=DateFrom]").val() + '&DateTo=' + $("[name=DateTo]").val() + '&Area=' + $("[name=Area]").val()
        //    + '&GroupModel=' + $("[name=GroupModel]").val() + '&ModelType=' + $("[name=ModelType]").val() + '&Variant=' + $("[name=Variant]").val()
        //});
    });
});