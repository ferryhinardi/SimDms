$(document).ready(function () {
    var options = {
        title: "Generate ITS",
        xtype: "panels",
        toolbars: [
            { name: "btnGenerate", text: "Generate Excel", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                     { name: "DateFrom", text: "Periode From", cls: "span4", type: "datepicker" },
                     { name: "DateTo", text: "to", cls: "span4", type: "datepicker" },
                     { name: "Area", text: "Area", cls: "span4", type: "select" },
                     { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
                     { name: "Dealer", text: "Dealer", cls: "span4", type: "select" },
                     { name: "Filter", text: "Filter", cls: "span4", type: "select" },
                ]
            },
        ]
    }

    var widget = new SimDms.Widget(options);

    var paramsSelect = [
         {
             //name: "Outlet", url: "wh.api/inquiryprod/outlets",
             //name: "Outlet", url: "wh.api/combo/ListBranchesNew",
             name: "Outlet", url: "wh.api/combo/ComboOutletList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Dealer",
                 additionalParams: [
                     //{ name: "Area", source: "Area", type: "select" },
                     //{ name: "Dealer", source: "Dealer", type: "select" }
                      { name: "companyCode", source: "Dealer", type: "select" }
                 ]
             }
         },
         {
             //name: "Dealer", url: "wh.api/inquiryprod/DealerMappingDealers",
             //name: "Dealer", url: "wh.api/combo/ListDealersNew",
             name: "Dealer", url: "wh.api/combo/ComboDealerList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Area",
                 additionalParams: [
                     { name: "GroupArea", source: "Area", type: "select" }
                 ]
             }
         },
    ];

    widget.setSelect(paramsSelect);
    var nationalSLS = '';

    widget.render(function () {
        $('#Dealer, #Outlet').attr('disabled', 'disabled');
        $(".frame").css({ top: 240 });
        widget.post("wh.api/inquiryprod/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                widget.select({ selector: "select[name=Filter]", url: "wh.api/inquiryprod/FilterGenerateITS", selected: "0" });
                //widget.select({ selector: "select[name=Area]", url: "wh.api/inquiryprod/dealermappingareas", optionalText: "--SELECT ALL--" });
                //widget.select({ selector: "select[name=Area]", url: "wh.api/combo/Areas", optionalText: "-- SELECT ALL --" });
                widget.select({ selector: "select[name=Area]", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" });
            }
        });

        $("#btnRefreshGrid").on("click", function () {
            refreshGrid();
        });
    });

    function refreshGrid() {
        var params = {
            Area: $("[name=Area]").val(),
            DealerCode: $("[name=Dealer]").val(),
            OutletCode: $("[name=Outlet]").val(),
            //StartDate: getSqlDate($("[name=DateFrom]").val()),
            StartDate: $("[name=DateFrom]").val(),
            //EndDate: getSqlDate($("[name=DateTo]").val()),
            EndDate: $("[name=DateTo]").val(),
            ReportType: $("[name=Filter]").val(),
        }
        
        widget.kgrid({
            url: "wh.api/InquiryProd/ExportGenerateITSs",
            name: "GenerateITS",
            params: params,
            //serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "InquiryNumber", text: "Inquiry Number", width: 120 },
                { field: "InquiryDate", text: "Inquiry Date", width: 280, type: "date" },
                { field: "Area", text: "Area", width: 120 },
                { field: "CompanyCode", text: "Company Code", width: 180 },
            ],
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }

    $('#Area').on('change', function (e) {
        if ($('#Area').val() == "") {
            $('#Dealer, #Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Dealer').removeAttr('disabled', 'disabled');
        }
    });

    $('#Dealer').on('change', function (e) {
        if ($('#Dealer').val() == "") {
            $('#Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Outlet').removeAttr('disabled', 'disabled');
        }
    });
   
    $("#btnGenerate").on('click', function (e) {
        //widget.exportXls({
        //    source: "wh.api/inquiryprod/ExportGenerateITSs?area=" + $("[name=Area]").val() + "&dealerCode=" + $("[name=Dealer]").val() + "&outletCode=" + $("[name=Outlet]").val() + "&startDate=" + $("[name=DateFrom]").val() + "&endDate=" + $("[name=DateTo]").val() + "&reportType=" + $('[name="Filter"]').val(),
        //    fileName: "personal_list",
        //    items: [
        //        { name: "InquiryNumber", text: "Inquiry Number", width: 120 },
        //        { name: "InquiryDate", text: "Inquiry Date", width: 280, type: "date" },
        //        { name: "Area", text: "Area", width: 120 },
        //        { name: "CompanyCode", text: "Company Code", width: 180 },
        //        //{ name: "HPNo", text: "HPNo", width: 180 },
        //        //{ name: "CarType", text: "Car TYpe", width: 180 },
        //        //{ name: "Color", text: "Color", width: 80 },
        //        //{ name: "PoliceRegNo", text: "Police No", width: 100 },
        //        //{ name: "Engine", text: "Engine", width: 140 },
        //        //{ name: "Chassis", text: "Chassis", width: 180 },
        //        //{ name: "Salesman", text: "Salesman", width: 280 },
        //        //{ name: "IsDeliveredA", text: "Penjelasan Isi Buku Manual", width: 250 },
        //        //{ name: "IsDeliveredB", text: "Penjelasan Fitur keamanan", width: 250 },
        //        //{ name: "IsDeliveredC", text: "Penjelasan Jadwal servis berkala", width: 280 },
        //        //{ name: "IsDeliveredD", text: "Penjelasan Garansi", width: 200 },
        //        //{ name: "IsDeliveredE", text: "Kartu nama PIC Servis / bengkel", width: 280 },
        //        //{ name: "IsDeliveredF", text: "Customer Feedback Card", width: 240 },
        //        //{ name: "Comment", text: "Customer comments", width: 240 },
        //        //{ name: "Additional", text: "Additional inquiries", width: 240 },
        //    ]
        //});

        var area = $("[name='Area']").val();
                  
        if (widget.isNullOrEmpty(area)) {
            widget.showNotification("Area harus diisi.");
        }
        else {          
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();
            var hh = today.getHours();
            var mn = today.getMinutes();
            var ss = today.getSeconds();

            var filter = '';
            if ($('[name="Filter"]').val() == "0")
                filter = "InquiryDt";
            else
                filter = "NextFollowUpDt";

            window.location.href = "wh.api/inquiryprod/CreateExcelExportGenerateITS?area=" + $('[name="Area"]').val() + '&dealerCode=' + $('[name="Dealer"]').val()
            + '&outletCode=' + $('[name="Outlet"]').val() + '&startDate=' + $('[name="DateFrom"]').val() + '&endDate=' + $('[name="DateTo"]').val()
            + '&reportType=' + $('[name="Filter"]').val();

            //widget.showReport({
            //    id: "ItsGenerate",
            //    par: [$('[name="DateFrom"]').val(), $('[name="DateTo"]').val(), $('[name="Area"]').val(), $('[name="Dealer"]').val(), $('[name="Outlet"]').val(), $('[name="Filter"]').val()],
            //    type: "export",
            //    filename: "ItsGenerate_" + filter + "_" + yyyy + dd + mm + "_" + hh + mn + ss,
            //});
        }      
    });
});