$(document).ready(function () {
    var options = {
        title: "Generate ITS - Report Fresh Inquiry Harian Dealer",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "StartDate", type: "datepicker", cls: "span4 full", text: "Periode" },
                    { name: "Model", type: "select", cls: "span4 full ", text: "Group Model", opt_text: '' },
                    { name: "Variant", type: "select", cls: "span4 full", text: "Model", opt_text: '' },
                    { name: "MVariant", type: "select", cls: "span4 ", text: "Variant", opt_text: '' }                
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    var dtvariant = [];

    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();        
    }

    function initElementEvents() {
        var btnExportExcel = $('#btnExportExcel');

        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            window.location.href = 'wh.api/InquiryProd/GenerateITSReportFreshInqHarianDealer?StartDate=' + '01-' + $('[name=StartDate]').val() + '&model=' + $("#Variant").select2('val') + '&groupmodel=' + $("#Model").select2('val') + '&variant=' + $("#MVariant").select2('val');
        });


        $("input[name='StartDate']")
        .on('change', function () {
            $(this).val($(this).val().substr(3))            
        });

        widget.post('wh.api/Combo/MasterModelVariant', function (result) {
            dtvariant = result;
            $("#Variant").select2('destroy')
            $("#Variant option:first").remove();
            $("#Variant").attr('multiple', 'multiple');
            $("#Variant").select2()

            $("#MVariant").select2('destroy')
            $("#MVariant option:first").remove();
            $("#MVariant").attr('multiple', 'multiple');
            $("#MVariant").select2()


        });



        widget.post('wh.api/Combo/GroupModel', function (result) {
            $("#Model").select2('destroy')
            $("#Model option:first").remove();
            $("#Model").attr('multiple', 'multiple');

            $.each(result, function (k, v) {
                $('#Model')
                    .append($("<option></option>")
                    .attr("value", v.value)
                    .text(v.text));
            });

            $("#Model").select2()
        });

     

        $("#Model").on('change', function (x) {
            var mdl = $('#Model').val();
            $('#Variant option').remove();
            $('#MVariant option').remove();
            $("#Variant").trigger("change");
            if (mdl != null) {
                var arr = jQuery.grep(dtvariant, function (a) {
                    return mdl.indexOf(a.GroupModel) != -1;
                });


                $.each(arr, function (k, v) {
                    $('#Variant')
                        .append($("<option></option>")
                        .attr("value", v.value)                                  
                        .text(v.text));
                });

                $('#Variant option').prop('selected', 'selected');
                $("#Variant").trigger("change");
            }                
        });
   
      
        $("#Variant").on('change', function (x) {
            $('#MVariant option').remove();
            $("#MVariant").trigger("change");
            var prm = $("#Variant").select2('val')
            if (prm == [])
                return;

            $.ajax({
                type: "POST",
                url: 'wh.api/Combo/GetVariantFromModel',
                data: { lstModel: prm },
                dataType: 'json',
                traditional: true,
                success: function (rslt) {
                    $('#MVariant option').remove();
                    $("#MVariant").trigger("change");
                    $.each(rslt.variant, function (k, v) {
                        $('#MVariant')
                            .append($("<option></option>")
                            .attr("value", v)
                            .text(v));
                    });
                }
            });       
        });
        
        
        //setTimeout($('#Variant').select2('val', '')
        
        

    }
});