"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterHarianPenerbitan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.loadsigname = function (x) {

        $http.post('tax.api/Combo/SignNameSeq', { ProfitCenter: (x != undefined ? x : $("#ProfitCenter").val()) }).
        success(function (data, status, headers, config) {
            me.SignName = data;

            $http.get('breeze/tax/SignName?ProfitCentre=').
        success(function (dl, status, headers, config) {
            setTimeout(function () {
                $("#SignName").select2("val", dl.SeqNo);
            }, 1000);
            me.data.SignName = dl.SignName;
            me.data.JobTitle = dl.TitleSign;
        });

        });
    }
    me.UnitUsaha = [
        { "value": '000', "text": 'GENERAL' },
        { "value": '100', "text": 'UNIT' },
        { "value": '200', "text": 'SERVICE' },
        { "value": '300', "text": 'SPAREPART' }
    ];

    me.FakturNo = function (x, sql, col) {
        var sDate = moment(me.data.DateFrom).format('YYYYMMDD');
        var eDate = moment(me.data.DateTo).format('YYYYMMDD');
        if (sql == "FakturNo") {
            var lookup = Wx.blookup({
                name: "FakturNumber",
                title: "No.Faktur",
                manager: TaxManager,
                query: new breeze.EntityQuery.from("FakturNo").withParameters({ profitCenterCode: me.data.ProfitCenter, stardate: sDate, enddate: eDate }),
                defaultSort: "FPJGovDate asc",
                columns: col,
            });
        } else {
            var lookup = Wx.blookup({
                name: "FakturNumber",
                title: "No.Faktur",
                manager: TaxManager,
                query: new breeze.EntityQuery.from("FakturNoGab").withParameters({ profitCenterCode: me.data.ProfitCenter, stardate: sDate, enddate: eDate }),
                defaultSort: "FPJGovDate asc",
                columns: col,
            });
        }
        lookup.dblClick(function (data) {
            if (x) {
                me.data.FakturNoFrom = data.FPJGovNo;
                console.log(me.data.FakturNoTo);
                if (me.data.FakturNoTo == '' || me.data.FakturNoTo == undefined) me.data.FakturNoTo = data.FPJGovNo;
                console.log(me.data.FakturNoFrom);
            } else {
                me.data.FakturNoTo = data.FPJGovNo;
                if (me.data.FakturNoFrom == '' || me.data.FakturNoFrom == undefined) me.data.FakturNoFrom = data.FPJGovNo;
            }
            me.Apply();
        });
    }

    me.printPreview = function () {
        console.log('ini profit center ' + me.data.ProfitCenter);
        console.log('ini optionPPFF ' + me.data.optionPPFF);
        if (me.data.ProfitCenter == 300 && (me.data.optionPPFF == 0 || me.data.optionPPFF == 2)) {
            BootstrapDialog.show({
                message: $(
                    '<div class="container">' +
                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Faktur Pajak Pre-Printed</div>' +

                    '<div class="row">' +

                    '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Lampiran Faktur Pajak</div>'),
                closable: false,
                draggable: true,
                type: BootstrapDialog.TYPE_INFO,
                title: 'Jenis Faktur Pajak',
                buttons: [{
                    label: ' Print',
                    cssClass: 'btn-primary icon-print',
                    action: function (dialogRef) {
                        me.Print();
                        dialogRef.close();
                    }
                }, {
                    label: ' Cancel',
                    cssClass: 'btn-warning icon-remove',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }]
            });
        } else {
            if (me.data.ProfitCenter == "000") {
                var statusCoret = 0;
                if ($('#isHargaJual').prop('checked') == true)
                    statusCoret++;
                if ($('#isPenggantian').prop('checked') == true)
                    statusCoret++;
                if ($('#isTermijn').prop('checked') == true)
                    statusCoret++;
                if ($('#isDP').prop('checked') == true)
                    statusCoret++;

                me.data.StatusCoret = statusCoret;
                console.log(statusCoret, $('#isHargaJual').prop('checked'));
                //MsgConfirm("Are you sure to p current record?", function (result) {
                //    if (result == true) {
                me.Print();
                //}
                //else return;
                //});
            } else me.Print();
        }
    }

    me.Print = function () {
        //console.log(me.data.SignName);  
        if ($("#SignName").select2('data').text != null)
            me.data.SignName = $("#SignName").select2('data').text
        else
            return;

        var ReportId = "";
        var prm = [];
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        me.data.rbPrePrinted = sizeType;
        $http.post('tax.api/FakturPajak/ValidatePrint', me.data).
              success(function (data, status, headers, config) {
                  if (data.success) {
                      //console.log(data.data.ReportId);
                      ReportId = data.data.ReportId;
                      $.each(data.data, function (key, val) {
                          if (key != "ReportId") prm.push(data.data[key]);
                      });
                      var rprm = me.data.SignName;

                      if ($('#ProfitCenter').val() == '100') {
                          rprm += ',,,' + $('#isHargaJual').prop('checked') + ',' + $('#isPenggantian').prop('checked') + ',' + $('#isDP').prop('checked') + ',' + $('#isTermijn').prop('checked');
                      }
                      else if ($('#ProfitCenter').val() == '300') {
                          if (data.data.ReportId == "SpRpTrn038A" || data.data.ReportId == "SpRpTrn038V2") {
                              prm[4] = '300';
                              prm[5] = $("#SignName").val();
                          }
                          else if (data.data.ReportId == "SpRpTrn011A") {
                              rprm = ',,' + me.data.SignName + ',' + me.data.JobTitle;
                          }                          
                      }
                      else {
                          if (data.data.ReportId == "ArRpTrn017") {
                              rprm = me.data.SignName + ',' + $('#isHargaJual').prop('checked') + ',' + $('#isPenggantian').prop('checked') + ',' + $('#isDP').prop('checked') + ',' + $('#isTermijn').prop('checked');
                          }
                      }


                      console.log(rprm);

                      Wx.showPdfReport({
                          id: ReportId,
                          pparam: prm.join(','),
                          rparam: rprm,
                          type: "devex"
                      });
                  }
                  else {
                      MsgBox(data.message, MSG_INFO);
                  }
              }).
              error(function (e, status, headers, config) {
                  MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                  //console.log(e);
              });

    }

    $("[name = 'isALL']").on('change', function () {
        if ($('#isALL').prop('checked') == true) {
            $('#FakturNoFrom').removeAttr('disabled');
            $('#FakturNoTo').removeAttr('disabled');
            $('#btnFakturNoFrom').removeAttr('disabled');
            $('#btnFakturNoTo').removeAttr('disabled');
        } else {
            $('#FakturNoFrom').attr('disabled', true);
            $('#FakturNoTo').attr('disabled', true);
            $('#btnFakturNoFrom').attr('disabled', true);
            $('#btnFakturNoFrom, #btnFakturNoTo').val('');
            $('#FakturNoFrom, #FakturNoTo').val('');
            me.data.FakturNoFrom = me.data.FakturNoTo = '';
        }
    });

    $("[name = 'ProfitCenter']").on('change', function () {
        $('#FakturNoFrom').attr('disabled', true);
        $('#FakturNoTo').attr('disabled', true);
        $('#btnFakturNoFrom').attr('disabled', true);
        $('#btnFakturNoFrom').val('');
        $('#FakturNoFrom').val('');
        $('#FakturNoTo').val('');
        $('#isC3').prop('checked', false);

        me.loadsigname();
        //$http.get('breeze/tax/SignName?ProfitCentre=' + $('#ProfitCenter').val()).
        //    success(function (dl, status, headers, config) {

        //        me.data.SignName = dl.SignName;
        //        me.data.JobTitle = dl.TitleSign;

        //        //$("#SignName").select2("val", dl.);
        //    });
    });

    me.field = function (x) {
        var query = "FakturNo";
        var col = "";

        if (me.data.ProfitCenter != '200') {
            if (me.data.ProfitCenter == '300') {
                col = [
                    { field: 'FPJGovNo', title: 'No.Faktur Pajak' },
                    { field: 'FPJGovDate', title: 'Tgl.Faktur Pajak', template: "#= (FPJGovDate == undefined) ? '' : moment(FPJGovDate).format('DD MMM YYYY') #" },
                    { field: 'FpjNo', title: 'No.Faktur' },
                    { field: 'FPJDate', title: 'Tgl.Faktur', template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
                    { field: 'DocNo', title: 'No.Dokumen' },
                    { field: 'DocDate', title: 'Tgl.Dokumen', template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                    { field: 'DueDate', title: 'Jatuh Tempo', template: "#= (DueDate == undefined) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                    { field: 'CustomerName', title: 'Nama Pelanggan' },
                    { field: 'addrees', title: 'Alamat' },
                    { field: 'DPPAmt', title: 'DPP' },
                    { field: 'PPNAmt', title: 'PPN' },
                    { field: 'Total', title: 'Total' }
                ];
            } else {
                col = [
                    { field: 'FPJGovNo', title: 'No.Faktur Pajak' },
                    { field: 'FPJGovDate', title: 'Tgl.Faktur Pajak', template: "#= (FPJGovDate == undefined) ? '' : moment(FPJGovDate).format('DD MMM YYYY') #" },
                    { field: 'DocNo', title: 'No.Dokumen' },
                    { field: 'DocDate', title: 'Tgl.Dokumen', template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                    { field: 'DueDate', title: 'Jatuh Tempo', template: "#= (DueDate == undefined) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                    { field: 'CustomerName', title: 'Nama Pelanggan' },
                    { field: 'addrees', title: 'Alamat' },
                    { field: 'DPPAmt', title: 'DPP' },
                    { field: 'PPNAmt', title: 'PPN' },
                    { field: 'Total', title: 'Total' }
                ];
            }

            query = "FakturNo";
        } else {
            col = [
                { field: 'FPJGovNo', title: 'No.Faktur Pajak' },
                { field: 'FPJGovDate', title: 'Tgl.Faktur Pajak', template: "#= (FPJGovDate == undefined) ? '' : moment(FPJGovDate).format('DD MMM YYYY') #" },
                 { field: 'DocNo', title: 'No.Dokumen' },
                    { field: 'DocDate', title: 'Tgl.Dokumen', template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                    { field: 'DueDate', title: 'Jatuh Tempo', template: "#= (DueDate == undefined) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                    { field: 'CustomerName', title: 'Nama Pelanggan' },
                    { field: 'addrees', title: 'Alamat' },
                    { field: 'DPPAmt', title: 'DPP' },
                    { field: 'PPNAmt', title: 'PPN' },
                    { field: 'Total', title: 'Total' }
            ];

            if (me.data.optionSRGL == "2")
                query = "FakturNoGab";
            else query = "FakturNo";
        }
        console.log(query);
        console.log(me.optionstndr);
        console.log(me.data.optionSRGL);
        me.FakturNo(x, query, col);
    }

    me.initialize = function () {
        me.data = {};
        me.data.DateFrom = me.now();
        me.data.DateTo = me.now();
        //me.data.DateFrom = "2014/01/01";
        //me.data.DateTo = "2014/04/30";
        $('#isALL').prop('checked', false);
        me.data.isALL = false;
        $('#isC2').prop('checked', false);
        me.data.isC2 = false;
        $('#isC3').prop('checked', false);
        me.data.isC3 = false;
        me.data.optionPrintFormat = "0";
        me.data.optionSRGL = "0";
        me.data.optionPrePrint = "1";
        me.data.optionSRK = "0";
        me.data.optionPPFF = "0";
        me.data.optionFullHalf = "0";
        $http.post('om.api/FakturPajak/Default').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
                  me.data.ProductType = dl.ProductType;
                  me.data.ProfitCenter = dl.ProfitCenter;
                  if (dl.ProfitCenter == '000') {
                      $('#ProfitCenter').removeAttr('disabled');
                  }
                  if (me.data.ProfitCenter == '000' || me.data.ProfitCenter == '100') {
                      $('#isPenggantian').prop('checked', true);
                      me.data.isPenggantian = true;
                      $('#isDP').prop('checked', true);
                      me.data.isDP = true;
                      $('#isTermijn').prop('checked', true);
                      me.data.isTermijn = true;
                  }
                  me.loadsigname(dl.ProfitCenter);
              });

        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Faktur Pajak",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "ProfitCenter", model: "data.ProfitCenter", text: "Profit Center", cls: "span3", type: "select2", datasource: "UnitUsaha", disable: true },
                     {
                         type: "optionbuttons",
                         name: "optionsrvc",
                         model: "data.optionSRGL",
                         cls: "span4",
                         show: "data.ProfitCenter == '200'",
                         items: [
                             { name: "0", text: "Standar" },
                             { name: "1", text: "Rinci" },
                             { name: "2", text: "Gabungan" },
                             { name: "3", text: "Lampiran" },
                             { name: "lbl", text: "Body Reapair", type: "label", disable: true },
                             { name: "4", text: "Standar" },
                             { name: "5", text: "Rinci" },
                             { name: "6", text: "Khusus" },
                         ]
                     },
                    {
                        type: "optionbuttons",
                        name: "print",
                        model: "data.optionPrintFormat",
                        cls: "span3",
                        show: "data.ProfitCenter == '100' || data.ProfitCenter == '000'",
                        items: [
                            { name: "0", text: "Pre Printed" },
                            { name: "1", text: "Formating" },
                        ]
                    },
                    {
                        type: "optionbuttons",
                        name: "print",
                        model: "data.optionPrePrint",
                        cls: "span3",
                        text: "print",
                        show: "data.ProfitCenter == '200'",
                        items: [
                            { name: "0", text: "Pre Printed" },
                            { name: "1", text: "Printed" },
                        ]
                    },

                    //{
                    //    type: "optionbuttons",
                    //    name: "optionsrvc",
                    //    model: "data.optionSRK",
                    //    cls: "span5",
                    //    show: "data.ProfitCenter == '200'",
                    //    text: "Body Repair",
                    //    items: [
                    //        { name: "0", text: "Standar" },
                    //        { name: "1", text: "Rinci" },
                    //        { name: "2", text: "Khusus" },
                    //    ]
                    //},
                    {
                        type: "optionbuttons",
                        name: "options",
                        model: "data.optionPPFF",
                        //cls: "span3 full",
                        show: "data.ProfitCenter == '300'",
                        items: [
                            { name: "0", text: "Pre Printed (Terlampir)" },
                            { name: "1", text: "Pre Printed (Detail)" },
                            { name: "2", text: "Formating (Terlampir)" },
                            { name: "3", text: "Formating (Detail)" },
                        ]
                    },
                    {
                        text: "Tanggal",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DateFrom", text: "", cls: "span4", type: "ng-datepicker" },
                            { name: "DateTo", text: "", cls: "span4", type: "ng-datepicker" },
                        ]
                    },
                     {
                         type: "optionbuttons",
                         name: "FullHalf",
                         model: "data.optionFullHalf",
                         cls: "span4",
                         show: "data.ProfitCenter == '300'|| data.ProfitCenter == '200'",
                         items: [
                             { name: "0", text: "Full" },
                             { name: "1", text: "Half" },
                         ]
                     },
                     {
                         type: "controls",
                         cls: "span2",
                         show: "data.ProfitCenter == '100'",
                         items: [
                             { name: "isFooter", cls: "span1", type: "check" },
                             { type: "label", text: "Footer", cls: "span2 mylabel" },
                         ]
                     },
                    {
                        type: "controls",
                        cls: "span2",
                        show: "data.ProfitCenter == '100'",
                        items: [
                            { name: "isShowSparePart", cls: "span1", type: "check" },
                            { type: "label", text: "Tampilkan Spare Part", cls: "span3 mylabel" },
                        ]
                    },
                     {
                         text: "No.Faktur",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "isALL", cls: "span1", type: "check" },
                             { name: "FakturNoFrom", cls: "span3", text: "", type: "popup", disable: true, click: "field(1)" },
                             { name: "FakturNoTo", cls: "span3", text: "", type: "popup", disable: true, click: "field()" },
                         ]
                     },
                    {
                        text: "Penanda Tangan",
                        type: "controls",
                        cls: "span8",
                        required: true,
                        items: [
                           { name: "SignName", model: "data.SignName", text: "Unit Usaha", cls: "span3", type: "select2", datasource: "SignName" },
                           { name: "JobTitle", cls: "span3", text: "", type: "text", disable: true },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3",
                        show: "data.ProfitCenter == '000'|| data.ProfitCenter == '100' ",
                        items: [
                            { name: "isDP", cls: "span1", type: "check" },
                            { type: "label", text: "Uang Muka", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3",
                        show: "data.ProfitCenter == '000'|| data.ProfitCenter == '100' ",
                        items: [
                            { name: "isHargaJual", cls: "span1", type: "check" },
                            { type: "label", text: "Harga Jual", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3",
                        show: "data.ProfitCenter == '000'|| data.ProfitCenter == '100' ",
                        items: [
                            { name: "isPenggantian", cls: "span1", type: "check" },
                            { type: "label", text: "Penggantian", cls: "span7 mylabel" },
                        ]
                    },

                     {
                         type: "controls",
                         cls: "span3",
                         show: "data.ProfitCenter == '000'|| data.ProfitCenter == '100' ",
                         items: [
                             { name: "isTermijn", cls: "span1", type: "check" },
                             { type: "label", text: "Termijn", cls: "span7 mylabel" },
                         ]
                     },
                     {
                         type: "controls",
                         cls: "span3",
                         text: "Detail",
                         show: "data.ProfitCenter == '200' && optionstndr == '2'",
                         items: [
                             { name: "isShowJasaMaterial", cls: "span1", type: "check" },
                             { type: "label", text: "Tampilkan Jasa dan Material", cls: "span7 mylabel" },
                         ]
                     },
                      {
                          type: "controls",
                          cls: "span3 full",
                          show: "data.ProfitCenter == '200'",
                          items: [
                              { name: "isShowPotongan", cls: "span1", type: "check" },
                              { type: "label", text: "Tampilkan Potongan", cls: "span7 mylabel" },
                          ]
                      },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RptRegisterHarianPenerbitan");
    }



});