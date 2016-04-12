/// <reference path="../../../vendors/jquery-2.0.3.js" />
$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Clean Up ITS",
        xtype: "panels",
        toolbars: [
            { name: "btnQuery", text: "QUERY" },
            { name: "btnExcel", text: "EXCEL" },
            { name: "btnProcess", text: "PROCESS" },
            //{ name: "btnSelectAll", text: "SELECT ALL" }
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                { name: "CompanyCode", type: "text", cls: "hide" },
                { name: "BranchCode", type: "text", cls: "hide" },

                { name: "PerDate", text: "Per", cls: "span4", type: "kdatepicker" },
                { name: "SalesCoord", text: "Sales Koordinator", cls: "span4", type: "select" },

                { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
                { name: "Salesman", text: "Wiraniaga", cls: "span4", type: "select" },

                { name: "SalesHead", text: "Sales Head", cls: "span4", type: "select" },
                ]
            },
            {
            name: "InqClnUpKDP",
            xtype: "k-grid",
            },
             {
                 name: "pnlFollowUpDtl",
                 showDivider: false,
                 cls: "hide",
                 items: [
                     { name: "ActivityDate", text: "Tgl Follow Up", cls: "span4", type: "kdatepicker", required: true },
                     { name: "ActivityType", text: "Jenis Pertemuan", cls: "span4", type: "select" },
                     { name: "ActivityDetail", text: "Keterangan", type: "textarea" },
                     { name: "NextFollowUpDate", text: "Next Follow Up", cls: "span4", type: "kdatepicker", required: true },
                     { name: "LastProgress", text: "Last Progress", cls: "span4", type: "select" },
                     { type: "buttons", items: [{ name: "btnAddDtl", text: "Add" }, { name: "btnCancelDtl", text: "Cancel" }] },
                     { name: "InquiryNumber", type: "hidden" },
                 ],               
             },             
        ]
    });
    widget.default = {};
    
    var paramsSelect = [
       {
           name: "Salesman", url: "its.api/clnupkdp/combosalesman?lookup=10",
           optionalText: "--SELECT ALL--",
           cascade:
               {
                   name: "SalesCoord",
                   additionalParams: [
                       { name: "EmployeeID", source: "SalesCoord", type: "select" },
                       { name: "Outlet", source: "Outlet", type: "select" }
                   ]
               }
       },
        {
            name: "SalesCoord", url: "its.api/clnupkdp/combosalesman?lookup=20",
            optionalText: "--SELECT ALL--",
            cascade: {
                name: "SalesHead",
                additionalParams: [
                    { name: "EmployeeID", source: "SalesHead", type: "select" },
                    { name: "Outlet", source: "Outlet", type: "select" }
                ]
            }
        },
        {
            name: "SalesHead", url: "its.api/clnupkdp/combosalesman?lookup=30",
            optionalText: "--SELECT ALL--",
            //cascade: {
            //    name: "BranchManager",
            //    additionalParams: [
            //        { name: "Outlet", source: "Outlet", type: "select" }
            //    ]
            //}
        }
    ];
    widget.setSelect(paramsSelect);
    widget.render(init);

    function detailInit(e) {
        widget.post("its.api/clnupkdp/inqclnupkdpdtl", { BranchCode: e.data.BranchCode, InquiryNumber: e.data.InquiryNumber }, function (result) {
            if (result.total > 0) {               
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: result.data, pageSize: 10 },
                    pageable: true,
                    columns: [
                        { field: "ActivityDate", width: 120, title: "Tgl Activiy", template: "#= ((ActivityDate === undefined) ? \"\" : moment(ActivityDate).format('DD MMM YYYY')) #" },
                        { field: "ActivityDetail", title: "Detail Activity" },
                        { field: "ActivityType", title: "Tipe Activity", width: 150 },
                        { field: "NextFollowupDate", title: "Tgl Next Follow Up", template: "#= ((NextFollowUpDate === undefined) ? \"\" : moment(NextFollowUpDate).format('DD MMM YYYY')) #" },
                    ]
                });
            }   
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Inquiry ini tidak memiliki Follow Up Detail" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function init() {
        $("#Outlet,#SalesHead,#SalesCoord,#Salesman,#PerDate").attr('disabled', 'disabled');
        

        widget.post("its.api/clnupkdp/default", function (result) {
            if (result.success) {
				
				console.log(result.data);
				
                widget.select({ name: "SalesHead", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=30" });
                widget.select({ name: "SalesCoord", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=20" });
                widget.select({ name: "Salesman", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=10" });
				
                widget.default = result.data;		
				
                if (result.data.DisabledPerDate) {
                    $("[name=PerDate]").removeAttr('disabled');
                }

                if (result.data.PositionID == "40") {
                    $("#SalesHead").removeAttr('disabled');
                }

                if (result.data.PositionID == "30") {                    
                    widget.select({ name: "SalesHead", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=30", selected: result.data.EmployeeID });
                    widget.select({ name: "SalesCoord", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=20" });
                    $("#SalesCoord").removeAttr('disabled');
                }
               
                if (result.data.PositionID == "20") {
                    widget.select({ name: "SalesHead", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=30", selected: result.data.SalesHead });
                    widget.select({ name: "SalesCoord", url: "its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=20", selected: result.data.EmployeeID });
                    $("#Salesman").removeAttr('disabled');
                }

                widget.select({ name: "Outlet", url: "its.api/clnupkdp/outlets", selected: result.data.Outlet });

                //if (result.data.NationalSLS == "1") {
                //    $("#Outlet,#SalesHead,#SalesCoord,#Salesman").removeAttr('disabled');
                //}
                //else {
                //    widget.post("its.api/clnupkdp/outlets", function (result) {
                //        $("[name=Outlet]").val(result[0].value);
                //    });
                //    widget.post("its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=30", function (result1) {
                //        if (result1.message != null) {
                //            widget.alert(result1.message);
                //            return;
                //        }

                //        if (result.data.PositionID == "10" || result.data.PositionID == "20" || result.data.PositionID == "30") {
                //            $("[name=SalesHead]").val(result1[0].value);
                //        }
                //        else {
                //            $("#SalesHead").removeAttr('disabled');
                //        }
                //    });
                //    widget.post("its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=20", function (result1) {
                //        if (result1.message != null) {
                //            widget.alert(result1.message);
                //            return;
                //        }

                //        if (result.data.PositionID == "10" || result.data.PositionID == "20") $("[name=SalesCoord]").val(result1[0].value);
                //        else {
                //            if (result.data.PositionID == "30" || result.data.PositionID == "40")
                //                $("[name=SalesCoord]").val(result1[0].value);
                //            else
                //                $("#SalesCoord").removeAttr('disabled');
                //        }
                //    });
                //    widget.post("its.api/clnupkdp/combosalesman?employeeID=" + result.data.EmployeeID + "&lookup=10", function (result1) {
                //        if (result1.message != null) {
                //            widget.alert(result1.message);
                //            return;
                //        }
                //        if (result.data.PositionID == "10") $("[name=Salesman]").val(result1[0].value);
                //        else {
                //            if (result.data.PositionID == "30" || result.data.PositionID == "40")
                //                $("[name=Salesman]").val(result1[0].value);
                //            else
                //                $("#Salesman").removeAttr('disabled');
                //        }
                //    });
                //}
				setTimeout(function(){
					widget.populate(widget.default);  				

                if (result.data.PositionID == "30") {                    
                    $("#SalesHead").val(widget.default.EmployeeID);
                }
               
                if (result.data.PositionID == "20") {
					$("#SalesHead").val(widget.default.SalesHead);
					$("#SalesCoord").val(widget.default.EmployeeID);
                }

				$("#Outlet").val(widget.default.Outlet);				
				
					
					
				}, 2000);				
            }
            else {
                if (result.message != "") {
                    widget.alert(result.message);
                }
                else {
                    widget.alert("User belum terdaftar di Master Position !");
                }
                
                widget.showToolbars([]);
            }
        });

        $("#btnProcess").on("click", process);
        $("#btnQuery").on("click", refreshGrid);
        $("#btnExcel").on("click", exportXls);
        //$("#btnSelectAll").on("click", selectAll);                

        $("#pnlFilter [name=SalesHead],#pnlFilter [name=SalesCoord],#pnlFilter [name=Salesman]").on("change", refreshGrid);
    }
    
    function chkOrUnchk() {
        $('tr.k-state-selected', '#refegrid').removeClass('k-state-selected');
    }

    
    function refreshGrid() {
        //$(".ajax-loader").hide();
       widget.kgrid({
            url: "its.api/clnupkdp/inqclnupkdp",
            name: "InqClnUpKDP",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,          
            columns: [
                { field: "check_item", title: "<input type='checkbox' id='chkSelectAll'/>", template: "<input class='check_item' type='checkbox' id='inpchk''/>", width: '40px', sortable: false, filterable: false },
                { field: "InquiryNumberStr", width: 75, title: "NO KDP" },
                { field: "InquiryDate", width: 120, title: "Tgl KDP", template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #" },
                { field: "NamaProspek", width: 100, title: "Pelanggan" },
                { field: "TipeKendaraan", width: 100, title: "Tipe" },
                { field: "Variant", width: 80, title: "Varian" },
                { field: "Transmisi", width: 70, title: "AT/MT" },
                { field: "ColourCode", width: 70, title: "Warna" },
                { field: "PerolehanData", width: 120, title: "Perolehan Data" },
                { field: "Wiraniaga", width: 150, title: "Wiraniaga" },
                { field: "Coordinator", width: 150, title: "Koordinator" },
                { field: "LastProgress", width: 100, title: "Last Progress" },
                { field: "NextFollowUpDate", width: 100, title: "Next Follow Up Date", template: "#= (NextFollowUpDate == undefined) ? '' : moment(NextFollowUpDate).format('DD MMM YYYY') #" },
            ],            
            detailInit: detailInit,
            onDblClick: addFollowUp
       });

       $("#chkSelectAll").on("change", selectDeselectAll);
       $("#inpchk").on("change", selectDeselectAll);
       //$("#btnAddDtl").on("click", saveFollowUp);
       //$('#btnAddDtl').unbind('click', saveFollowUp);
       $("#btnCancelDtl").on("click", cancelFollowUp);
    }      
    
    $("#InqClnUpKDP").on("change", ".check_item", function (e) {
        var row = $(e.target).closest("tr");
        var checkbox = $(this);

        if (checkbox.is(':checked')) {
            row.addClass("k-state-selected");
            row.attr("aria-selected", true);
        }
        else {
            row.removeClass("k-state-selected");
        }
    });

    $("#InqClnUpKDP").on("click", "tr", function (e) {

        //var row = $(e.target).closest("tr");
        //var checkbox = $(this);

        //if (checkbox.is(':checked')) {
        //    row.addClass("k-state-selected");
        //    row.attr("aria-selected", true);
        //}
        //else {
        //    row.removeClass("k-state-selected");
        //}

        //var grid = $("#InqClnUpKDP").data("kendoGrid");
        //var allData = grid.dataSource.data();
        //var row = grid.select();
        //var ckbox, state;

        //$.each(grid, function (index, content) {
        //    if (grid.content.find("tr").hasClass("k-state-selected")) {
        //        ckbox = row.find(".check_item");
        //        state = ckbox.prop("checked");
        //        ckbox.prop("checked", true);
        //    }
        //    else {
        //        ckbox = row.find(".check_item");
        //        state = ckbox.prop("checked");
        //        ckbox.prop("checked", false);
        //    }
        //});
    });
       
    function selectDeselectAll() {
        var grid = $("#InqClnUpKDP").data("kendoGrid");
        var allData = grid.dataSource.data();
       
        if ($('#chkSelectAll').is(':checked')) {
            $('.check_item').prop('checked', true);
            $("#InqClnUpKDP [role=row]").addClass("k-state-selected");
            $("#InqClnUpKDP [role=row]").attr("aria-selected", true);
        }
        else {
            $('.check_item').prop('checked', false);
            $("#InqClnUpKDP [role=row]").removeClass("k-state-selected");
        }       
    }

    function cancelFollowUp() {
        widget.hidePanel("pnlFollowUpDtl");
        widget.showPanel("InqClnUpKDP");
    }

    function addFollowUp() {
        var grid = $("#InqClnUpKDP").data("kendoGrid");
        var inquiryNumber = grid.dataItem(grid.select()).InquiryNumber;
        var lastProg = grid.dataItem(grid.select()).LastProgress;

        widget.clear("pnlFollowUpDtl");
        widget.hidePanel("InqClnUpKDP");
        widget.showPanel("pnlFollowUpDtl");
        
        widget.post("its.api/clnupkdp/DetailFollowUp?inquiryNumber=" + inquiryNumber + '&lastProgress=' + lastProg, function (result) {
            if (result.success == true) {
                widget.select({ name: "ActivityType", url: "its.api/combo/lookups/pact" });
                widget.select({
                    name: "LastProgress",
                    url: "its.api/combo/itsstatuswithoutdodelivery/?last=" + result.data.LastProgress,
                    selected: result.data.LastProgress });
                widget.populate(result.data);
            }                           
        });
    }

    function exportXls() {       
        var model = [$("[name=Outlet]").val(), $("[name=SalesHead]").val(), $("[name=SalesCoord]").val(), $("[name=Salesman]").val()];

        widget.export({
            url: "its.api/clnupkdp/exportclnupkdp?outlet="+ $("[name=Outlet]").val() + "&perdate="+ $("[name=PerDate]").val() + "&saleshead=" + $("[name=SalesHead]").val() 
                + "&salescoord=" + $("[name=SalesCoord]").val() + "&salesman=" + $("[name=Salesman]").val(),
            params: model           
        });        
    }

    function process() {
        var grid = $("#InqClnUpKDP").data("kendoGrid");
        var row = grid.select();

        if (row.length == 0) {
            alert("Tidak ada data yang dipilih");
            return;
        }
        else {
            //var ckbox, index=0;
            //$.each(row, function (index, content) {
            //    index = 0;
            //    ckbox = row.find(".check_item");
               
            //    if (ckbox.is(':checked')) {
            //        index = index + 1;
            //    }
            //    console.log(index);
            //    if (index == 0) {
            //        alert("Tidak ada data yang dipilih");                    
            //        return;
            //    }
            //    else {
                    if (confirm("Apakah anda yakin data ITS/KDP akan dibuatkan menjadi LOST dan sudah di Generate ke EXCEL ?")) {
                        var inquiry = new Array();

                        $.each(row, function (index, content) {
                            var dataRow = grid.dataItem(content);
                            console.log(dataRow.InquiryNumber);
                            inquiry.push(dataRow.InquiryNumber);
                        });

                        widget.post("its.api/clnupkdp/ClnUpKDP?inquirynumber=" + inquiry + "&perDate=" + $("[name=PerDate]").val(), function (result) {
                            refreshGrid();
                        });
                    }
                    else {
                        return;
                    }
            //    }
            //});            
        }        
    }

    function saveFollowUp() {
        var valid = widget.validate();
        console.log("asdasd");
        if (valid) {
            var data = widget.serializeObject("pnlFollowUpDtl");

            widget.post("its.api/clnupkdp/saveact", data, function (result) {
                if (result.success) {
                    widget.hidePanel("pnlFollowUpDtl");
                    widget.showPanel("InqClnUpKDP");
                    widget.showNotification("Follow Up Saved...!");                    
                }
                else {
                    widget.hidePanel("pnlFollowUpDtl");
                    widget.showPanel("InqClnUpKDP");
                    widget.alert(result.message);

                    //refreshGrid();
                }

                refreshGrid();
                //$('#btnAddDtl').unbind('click', saveFollowUp);

            });
        }          
    }

    $('#SalesCoord').on('change', function (e) {
        if ($('#SalesCoord').val() == "") {
            $('#Salesman').attr('disabled', 'disabled');
            //widget.post("its.api/clnupkdp/combosalesman?employeeid=" + $('#SalesCoord').val(), function (result) {
            //        $('#Salesman').val(result[0].value);
            //    });
        }
        else {
            $('#Salesman').removeAttr('disabled', 'disabled');
        }
    });

    $('#SalesHead').on('change', function (e) {        
        if ($('#SalesHead').val() == "") {
            $('#SalesCoord, #Salesman').attr('disabled', 'disabled');
            //widget.select({ selector: "select[name=Salesman]", url: "its.api/clnupkdp/combosalesman?employeeid=" + $('#SalesHead').val() });
            //widget.post("its.api/clnupkdp/combosalesman?employeeid=" + $('#SalesHead').val(), function (result) {
            //    $('#SalesCoord, #Salesman').val(result[0].value);
            //});
        }
        else {
            $('#SalesCoord').removeAttr('disabled', 'disabled');
            $('#Salesman').attr('disabled', 'disabled');
        }

    });

    $("#btnAddDtl").on("click", saveFollowUp);
});