var today = new Date();
var y = today.getFullYear();
var isDeleted = false;
var isMlt = false;
var widget = new SimDms.Widget({
    title: "Calender",
    xtype: "panels",
    toolbars: [
        { action: "add", text: "Sabtu/ Minggu", icon: "fa fa-save", name: "add" },
        { action: "save", text: "add Holiday", icon: "fa fa-save", name: "save" },
        //{ action: "delete", text: "Delete", name: "delete" },
        { action: "cancel", text: "Cancel", name: "cancel" },
        
    ],
    panels: [
           {
                name: "pnlStatus",
                items: [
                    { name: "Holiday", cls: "span8", type: "multidates" },
                    { name: "Calender", cls: "span8", type: "multidates" },
                ]
            }
    ],

    onInit: function (wgt) {
        wgt.call('initial', wgt);
    },

    initial: function (wgt) {
        widget = widget || wgt;
        var params = { year: y }
        var dates = [];
        $('#add').show();
        $('#save').show();
        $('#cancel').hide();
        $('#save').text("add Holiday");
        isDeleted = false;
        if (isMlt) {
            $('#Holiday').multiDatesPicker('destroy');
            $('#Calender').multiDatesPicker('destroy');
        }
        $.ajax({
            type: "POST",
            url: 'wh.api/calender/Default',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.length !== 0) {
                    dates = response;
                    $('#Calender').multiDatesPicker({
                        dateFormat: "yymmdd",
                        addDates: dates,
                        numberOfMonths: [4, 3],
                        defaultDate: y + '0101',
                        changeYear: true,
                        //disabled: true,
                    }).show();
                }
                else {
                    $('#Calender').multiDatesPicker({
                        dateFormat: "yymmdd",
                        numberOfMonths: [4, 3],
                        defaultDate: y + '0101',
                        changeYear: true,
                        //disabled: true,
                    }).show();
                    sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                }
            }
        });
    },
    
    add: function () {
        //var date = $('#Calender').date('getDate')
        //y = date.getFullYear();
        var years = $("[class=ui-datepicker-year]").children("option").filter(":selected").text();
        console.log(years);
        if (years.length > 4) years = years.substr(4, 7);
        console.log(years);    
        widget.post("wh.api/Calender/AddSabtuMinggu?Year="+ years, function (result) {
            if (result) {
                widget.call('initial');
            }
        });
    },

    save: function () {
        if ($('#save').text() == "add Holiday") {
            var years = $("[class=ui-datepicker-year]").children("option").filter(":selected").text();
            y = years;
            $('#Calender').multiDatesPicker().hide();
            //$('#Holiday').multiDatesPicker({
            //    dateFormat: "yymmdd",
            //    numberOfMonths: [4, 3],
            //    defaultDate: y + '0101',
            //    changeYear: true,
            //}).show();
            isMlt = true;
            console.log(years);
            var dates = [];
            var params = { year: y, type: 'holiday' }
            $.ajax({
                type: "POST",
                url: 'wh.api/calender/Default',
                dataType: 'json',
                data: params,
                success: function (response) {
                    if (response.length !== 0) {
                        dates = response;
                        $('#Holiday').multiDatesPicker({
                            dateFormat: "yymmdd",
                            addDates: dates,
                            numberOfMonths: [4, 3],
                            defaultDate: y + '0101',
                            //changeYear: true,
                        }).show();
                    }
                    else {
                        $('#Holiday').multiDatesPicker({
                            dateFormat: "yymmdd",
                            numberOfMonths: [4, 3],
                            defaultDate: y + '0101',
                            //changeYear: true
                        }).show();
                        sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                        $('.page > .ajax-loader').hide();
                    }
                }
            });
            $('#save').text("save Holiday");
            $('#add').hide();
            $('#cancel').show();
        } else {
            var data = $('#Holiday').multiDatesPicker('value');
            data = data == "" ? y + '0101,' : data;
                widget.post("wh.api/calender/save?MultiDates=" + data, function (result) {
                    if (result.success) {
                        sdms.info({ type: "success", text: result.message });
                        $('#save').text("add Holiday");
                        $('#Holiday').multiDatesPicker().hide();
                        widget.call('initial');
                    }
                    else {
                        sdms.info({ type: "error", text: "Tanggal merah gagal disimpan ke database. " + result.message });
                    }
                });
        }
    },

    cancel: function () {
        $('#Holiday').multiDatesPicker().hide();
        widget.call('initial');
    },

    delete: function () {
        if (isDeleted) {
            var data = $(".main form").serializeObject();
            console.log(data);
            widget.post("wh.api/calender/save?", data, function (result) {
                if (result.success) {
                    sdms.info({ type: "success", text: result.message });
                    widget.call('initial');
                }
                else {
                    sdms.info({ type: "error", text: "Tanggal merah gagal disimpan ke database. " + result.message });
                    //alert("Data SPK gagal disimpan ke database. " + result.message);
                }
            });
        } else {
            $('#cancel').show();
            $('#delete').show();
            $('#add').hide();
            $('#save').hide();
            $('#Calender').multiDatesPicker().hide();
            console.log(isDeleted);
            isDeleted = true;
            console.log(isDeleted);
            var dates = [];
            var params = { year: y, type : 'holiday' }
            $.ajax({
                type: "POST",
                url: 'wh.api/calender/Default',
                dataType: 'json',
                data: params,
                success: function (response) {
                    if (response.length !== 0) {
                        //console.log(response);
                        dates = response;

                        $('#Holiday').multiDatesPicker({
                            dateFormat: "yymmdd",
                            addDates: dates,
                            numberOfMonths: [4, 3],
                            defaultDate: y + '0101',
                            //changeYear: true,
                        }).show();

                        $('.page > .ajax-loader').hide();
                    }
                    else {
                        $('#Holiday').multiDatesPicker({
                            dateFormat: "yymmdd",
                            numberOfMonths: [4, 3],
                            defaultDate: y + '0101',
                            //changeYear: true
                        }).show();
                        sdms.info({ type: "error", text: "Tidak ada data yang ditampilkan" });
                        $('.page > .ajax-loader').hide();
                    }
                    //console.log(response)
                }
            });
        }
    },
    //excel: function () {
    //    var url = SimDms.baseUrl + 'wh.api/InquiryProd/Generatechart';
    //    window.open(url, '_blank')
    //},
    
    populate: function (result) {
        if (result.success) {
            widget.populate(result.data);
        }
    }
});