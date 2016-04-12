$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "WRF Report",
        xtype: "panels",
        toolbars: [            
            { action: 'exportexcel', text: 'Generate Report', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel' }
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                     { name: "DateFrom", cls: "span3", type: "datepicker" ,text:"Periode"}          
                ]
            }           
        ],       
    });

    widget.render(function () {
        widget.post('wh.api/osticket/default', function (r) {            
            $("input[name='DateFrom']").val(r.curdate);
            $("input[name='DateFrom']").prop('disabled',true);
        });
        

        
        $("#exportToExcel").on("click", function (e) {
            var url = "wh.api/Osticket/WRFReport";
            window.location = url;
        });

    });
});
