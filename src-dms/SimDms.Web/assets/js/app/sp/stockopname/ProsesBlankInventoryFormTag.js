$(document).ready(function () {

    var options = {
        title: "Blank Inventory Form/Tag",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        items: [
            { name: "Qty", text: "Jumlah Form / Tag", cls: "span3 full", validasi: "required" },
            { name: "frmtag", text: " Form / Tag", cls: "span3 full", disable: "!isInitialize" },
            { name: "lblInfo", text: "", type: "label" }
        ]
    }

    var widget = new SimDms.Widget(options);

    
    widget.render(function () {
        init();
    });

    function init() {

        widget.post('sp.api/stockopname/SparepartValidation?menuType=ST', function (dl) {
            if (dl.success) {
                $("#frmtag").val(dl.formtag);
                $("#lblInfo").html(dl.message);
                //$(".panel.frame").hide()
                $(".panel iframe").removeAttr("src").hide();

                MsgBox(dl.message);
            } else {
                MsgBox(dl.message, MSG_ERROR);                
                $(".panel").hide();        
            }
        });        
        $(".frame").css({ top: 134 });
        initElementEvent();
    }

    function initElementEvent() {
        $("#btnProcess").on('click', function () {
            widget.post('sp.api/StockOpname/ProsesInvBlankTag?sqty=' + $("#Qty").val(), function (dl) {
                if (dl.success) {
                    $("#lblInfo").html(dl.data.LabelInfo);
                    MsgBox(dl.message);
                    exportXls(dl.data);
                } else {
                    MsgBox(dl.message, MSG_ERROR);
                }
            });
        });
    }


    function exportXls(data) {
        //var inqType = $('#InqType').val();
        var rptid = (data.TypeCode == "1" ? "SpRpTrn020" : "SpRpTrn021");
        //widget.showReport({
        //    id: rptid,
        //    par: [
        //       data.CompanyCode,
        //       data.BranchCode,
        //       data.SThdrNo,
        //       data.FirstSTKNO,
        //       data.LastSTKNO
        //    ],
        //    //subrptindx: "1",
        //    //panel: "panelreport",
        //    type: "devex"
        //});
    }

});