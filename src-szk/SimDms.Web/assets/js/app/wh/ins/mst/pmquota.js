var isEdit = false;

$(document).ready(function () {
    var options = {
        title: "Master Quota Indent",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-info", icon: "fa fa-file", click: "New()" },
            { name: "btnRefresh", text: "Search", cls: "btn btn-info", icon: "fa fa-refresh", click: "Refresh()" },
            { name: "btnSave", text: "Save", cls: "btn btn-info", icon: "fa fa-save", click: "Save()" },
            { name: "btnDownload", text: "Download", cls: "btn btn-info", icon: "fa fa-file-excel-o", click: "Download()" },
        ],
        panels: [
            {
                name: "pnlPeriod",
                cls: "full",
                items: [
                    { name: "CompanyCode", id: "CompanyCode", text: "Dealer", cls: "span4 full", type: "select", opt_text: "-- SELECT ALL --" },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Period",
                        items: [
                            { name: "PeriodYear", id: "PeriodYear", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            { name: "PeriodMonth", id: "PeriodMonth", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    { name: "QuotaBy", id: "QuotaBy", text: "Quota By", cls: "span3", type: "select" },
                ]
            },
            { name: 'wxgridcell', xtype: 'wxtable' },
        ]
    };

    var Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    var hotpmQuota = hotRenderer();
    var scoreresult = "";
    

    function init(s) {
        $('#btnDownload').attr('disabled', 'disabled');
        setTimeout(function () {
            $.ajax({
                async: true,
                type: "POST",
                url: "wh.api/pmQuota/default",
                success: function (response) {
                    if (response.success) {
                        //$('#PeriodYear').select2('val', response.PeriodYear);
                        //$('#QuotaBy').select2('val', response.QuotaBy);
                        //$('#PeriodYear').attr('disabled', 'disabled');
                        //$('#QuotaBy').attr('disabled', 'disabled');

                        Wx.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/Company", optionalText: "-- SELECT ALL --" });

                        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years", optionalText: "-- SELECT ALL --" }, function () {
                            $('#PeriodYear').select2('val', response.PeriodYear);
                        });
                        Wx.select({ selector: "[name=PeriodMonth]", url: "wh.api/combo/listofmonth", optionalText: "-- SELECT ALL --" });
                        Wx.select({ selector: "[name=QuotaBy]", url: "wh.api/pmQuota/QuotaBy" }, function () {
                            $('#QuotaBy').select2('val', response.QuotaBy);
                        });
                    } else {
                        Wx.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/Company", optionalText: "-- SELECT ALL --" });
                        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years", optionalText: "-- SELECT ALL --" }, function () {
                            $('#PeriodYear').select2('val', response.PeriodYear);
                        });
                        Wx.select({ selector: "[name=PeriodMonth]", url: "wh.api/combo/listofmonth", optionalText: "-- SELECT ALL --" });
                        Wx.select({ selector: "[name=QuotaBy]", url: "wh.api/pmQuota/QuotaBy"});
                        //$('#QuotaBy').removeAttr('disabled');
                    }
                }
            })
        }, 1000)
             
        params = $('#pnlPeriod').serializeObject();
    }

    $('#btnNew').click(function () {
        $('#btnSave').removeAttr('disabled');
        $('#btnDownload').attr('disabled', 'disabled');
        init();
        hotpmQuota = hotRenderer();
        //$('#wxgridcell').Handsontable('clear');
        
    });

    $('#btnRefresh').click(function () {
        params = $('#pnlPeriod').serializeObject();

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/pmQuota/ReloadQuota",
            success: function (response) {
                if (response.message == "Success") {
                    scoreresult = response.data;
                    ReloadCallback(response);
                    //$('#btnSave').attr('disabled', 'disabled');
                    $('#btnDownload').removeAttr('disabled');
                    $('#btnSave').removeAttr('disabled');
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });
    });

    $('#btnSave').click(function () {
        if ($('#QuotaBy').val() == '') { alert('Silikan pilih Quota By terlebih dahulu'); return; }

        params = $('#pnlPeriod').serializeObject();
        var data = hotpmQuota.getData().slice(0);
        data.splice(0, 1);

        listdata = [];

        for (var i = 0; i < data.length - 1; i++) {
            //console.log(data[i][8]);
            if (data[i][4] == null || data[i][4] == "") { data[i][4] = "-" } else { data[i][4] }
            if (data[i][5] == null || data[i][5] == "") { data[i][5] = "-" } else { data[i][5] }
            if (data[i][6] == null || data[i][6] == "") { data[i][6] = "-" } else { data[i][6] }
            if (data[i][8] == null || data[i][8] == "") { data[i][8] = '0' } else { data[i][8] }
            listdata.push(rowToJson(data[i]));
        }
        params.listScore = JSON.stringify(listdata);
        //alert(params.listScore);
        //alert(isEdit);
        //if (isEdit == false) {
            $.ajax({
                async: true,
                type: "POST",
                data: params,
                url: "wh.api/pmQuota/Save",
                success: function (data) {
                    if (data.success) {
                        scoreresult = data.value;
                        $('#btnSave').attr('disabled', 'disabled');
                        $('#btnDownload').removeAttr('disabled');
                        sdms.info("Success", "Success");
                        return;
                    }
                    if (!data.success) {
                        sdms.info(data.message, "Error");
                    }
                }
            });
        //} else {
        //    $.ajax({
        //        async: true,
        //        type: "POST",
        //        data: params,
        //        url: "wh.api/pmQuota/Edit",
        //        success: function (data) {
        //            if (data.success) {
        //                isEdit = false;
        //                scoreresult = data.value;
        //                $('#btnSave').attr('disabled', 'disabled');
        //                $('#btnDownload').removeAttr('disabled');
        //                sdms.info("Update Quota Success", "Success");
        //                return;
        //            }
        //            if (!data.success) {
        //                sdms.info(data.message, "Error");
        //            }
        //        }
        //    });
        //}
    });

    $('#btnDownload').click(function () {
        //params = $('#pnlPeriod').serializeObject();
        var data = hotpmQuota.getData().slice(0);
        data.splice(0, 1);

        listdata = [];

        for (var i = 0; i < data.length - 1; i++) {
            listdata.push(rowToJson(data[i]));
        }
        params = JSON.stringify(listdata);

        location.href = 'wh.api/pmQuota/DownloadExcelFile?listScore=' + params;

        scoreresult = "";
        $('#btnDownload').attr('disabled', 'disabled');
        $('#btnSave').removeAttr('disabled');
        //saveCallback();
    });
});

function saveCallback() {
    hotpmQuota.alter('remove_row', 1, window.hotpmQuota.countRows());
    hotpmQuota.setDataAtCell([], null, null, 'loadData');
}

function ReloadCallback(result) {

    if (result.data !== null) {
        var i, n = result.data.length;
        var datadump = [];

        for (i = 0; i < n; i++) {
            //datadump.push(me.jsonToRow(response.data[i]));
            var returnObj = result.data[i];
            datadump.push([1 + i, 0, returnObj.CompanyCode])
            datadump.push([1 + i, 1, returnObj.PeriodYear])
            datadump.push([1 + i, 2, returnObj.PeriodMonth])
            datadump.push([1 + i, 3, returnObj.TipeKendaraan])
            datadump.push([1 + i, 4, returnObj.Variant])
            datadump.push([1 + i, 5, returnObj.Transmisi])
            datadump.push([1 + i, 6, returnObj.ColourCode])
            datadump.push([1 + i, 7, returnObj.QuotaQty])
            datadump.push([1 + i, 8, returnObj.IndentQty])
        }

        hotpmQuota.alter('remove_row', 2, window.hotpmQuota.countRows());
        hotpmQuota.setDataAtCell(datadump, null, null, 'loadData');

    }
}

function onGenerate() {
    var a = 1;
}

function hotRenderer() {

    function firstRowRenderer (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.fontWeight = 'bold';
        td.style.backgroundColor = '#ECECEC';
    };

    function readOnlyRenderer(instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
        td.style.textAlign = 'right';
    };

    var clkNew = '';
    var clkOld = '';
    var data = [
        ["Dealer", "Period Year", "Period Month", "Tipe", "Variant", "TM", "Colour", "Quota", "Indent"]
    ],
      container = document.getElementById('wxgridcell'),
      settings1 = {
          data: data,
          colWidths: [100, 75, 75, 250, 150, 50, 100, 75, 75],
          columns: [
            {
                //data: "No",
                //type: "numeric"
            },
            {
                //data: "Period Year",
                type: "numeric"
            },
            {
                //data: "Period Month"
                type: "numeric"
                //type: 'dropdown',
                //source: ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"]
            },
            {
                //data: "Tipe Kendaraan",
                //type: 'dropdown',
                //source: ["MT", "AT"]
            },
            {
                //data: "Variant",
                //type: 'dropdown',
                //source: ["MT", "AT"]
            },
            {
                //data: "TM",
                //type: 'dropdown',
                //source: ["MT", "AT"]
            },
            {
                //data: "Warna",
                //type: 'dropdown',
                //source: ["D", "DL", "K", "W", "WT"]
            },
            {
                //data: "Jumlah Kuota",
                type: 'numeric',
                format: '0,0'
            },
            {
                //data: "Jumlah Indent",
                type: 'numeric',
                format: '0,0',
                readOnly: true
            },
          ],
          contextMenu: true,
          minSpareRows: 1,
          currentRowClassName: 'currentRow',
          currentColClassName: 'currentCol',
          autoWrapRow: true,
          manualColumnResize: true,
          persistentState: true,
          cell: [
          { row: 0, col: 0, className: "htCenter" },
          { row: 0, col: 1, className: "htCenter" },
          { row: 0, col: 2, className: "htCenter" },
          { row: 0, col: 3, className: "htCenter" },
          { row: 0, col: 4, className: "htCenter" },
          { row: 0, col: 5, className: "htCenter" },
          { row: 0, col: 6, className: "htCenter" },
          { row: 0, col: 7, className: "htCenter" },
          { row: 0, col: 8, className: "htCenter" },
          ],
          cells: function (row, col, prop) {
              var cellProperties = {};

              if (row === 0) { cellProperties.readOnly = true; }

              if (row === 0) { cellProperties.renderer = firstRowRenderer; }

              if (row > 0 && (col === 4 || col === 5)) {
                  if ($('#QuotaBy').val() == 'TYP') { cellProperties.readOnly = true; }
                  else { cellProperties.readOnly = false; }
              }

              if (row > 0 && (col === 6)) {
                  if ($('#QuotaBy').val() == 'TYP' || $('#QuotaBy').val() == 'VAR') { cellProperties.readOnly = true; }
                  else { cellProperties.readOnly = false; }
              }

              return cellProperties;

          },
          afterOnCellMouseDown: function (sender, e, val) {
              if (e.row === -1) {
                  this.getInstance().deselectCell();
              }
              clkNew = e.row + '' + e.col
              if (clkNew === clkOld) {
                  if (e.col == '0') {
                      Dealer(sender, e, val);
                  }
                  if (e.col == '3') {
                      //alert($('#QuotaBy').val());
                      if ($('#QuotaBy').val() == 'TYP') {
                          hotpmQuota.setDataAtCell(e.row, e.col + 1, '-');
                          hotpmQuota.setDataAtCell(e.row, e.col + 2, '-');
                          hotpmQuota.setDataAtCell(e.row, e.col + 3, '-');
                      }
                      TipeKendaraan(sender, e, val);
                  }
                  if (e.col == '4') {
                      if ($('#QuotaBy').val() != 'TYP') {
                          if ($('#QuotaBy').val() == 'VAR') {
                              hotpmQuota.setDataAtCell(e.row, e.col + 2, '-');
                          }
                          Variant(sender, e, val);
                      }
                  }
                  if (e.col == '6') {
                      if ($('#QuotaBy').val() != 'TYP' && $('#QuotaBy').val() != 'VAR') {
                          Colour(sender, e, val);
                      }
                  }
                  clkOld = '';
              } else {
                  clkOld = clkNew;
              }
          },
          //afterChange: function (changes, source) {

          //    if (source !== 'edit') return;

          //    if (changes !== null) {
          //        var me = this;
          //        var cRow = changes[0][0];
          //        switch (changes[0][1]) {
          //            case 1: {
          //                me.setDataAtCell(cRow, 0, cRow);
          //            } break;
          //            case 7: {
          //                $('#btnSave').removeAttr('disabled');
          //                isEdit = true;
          //                console.log(isEdit);
          //            } break;
          //            default:
          //                {
          //                    $('#InjectData').val('0');
          //                    $('#InjectData').trigger('change')
          //                }
          //        }

          //    }

          //}
      };

    hotpmQuota = new Handsontable(container, settings1);
    hotpmQuota.render();

    //$('.ht_master').css('position', 'absolute');
    $('.wtHolder').css('margin-top', '0');
    return hotpmQuota;
}

function Dealer(sender, e, val) {
    sdms.lookup({
        title: 'Tipe Kendaraan',
        url: 'wh.api/pmquota/Dealer',
        sort: [{ field: 'DealerCode', dir: 'asc' }],
        editable: true,
        fields: [
            { name: "DealerCode", text: "Dealer Code" },
            { name: "DealerAbbreviation", text: "Dealer Abbr" },
            { name: "DealerName", text: "Dealer Name" },
        ],
        dblclick: function (result) {
            hotpmQuota.setDataAtCell(e.row, e.col, result.DealerCode);
        }
    });
}

function TipeKendaraan(sender, e, val) {
    sdms.lookup({
        title: 'Tipe Kendaraan',
        url: 'wh.api/pmquota/Tipe',
        sort: [{ field: 'GroupCode', dir: 'asc' }],
        editable: true,
        fields: [
            { name: "GroupCode", text: "Tipe Kendarann" },
        ],
        dblclick: function (result) {
            hotpmQuota.setDataAtCell(e.row, e.col, result.GroupCode);
            hotpmQuota.setDataAtCell(e.row, e.col + 5, 0);
            //hotpmQuota.setDataAtCell(e.row, e.col -3, e.row);
        }
    });
}

function Variant(sender, e, val) {
    sdms.lookup({
        title: 'Tipe Kendaraan',
        url: 'wh.api/pmquota/Varian?Tipe=' + hotpmQuota.getDataAtCell(e.row, e.col - 1),
        sort: [{ field: 'GroupCode', dir: 'asc' }],
        fields: [
            { name: "GroupCode", text: "Tipe Kendarann" },
            { name: "TypeCode", text: "Variant" },
            { name: "TransmissionType", text: "Transmission" },
        ],
        dblclick: function (result) {
            hotpmQuota.setDataAtCell(e.row, e.col, result.TypeCode);
            hotpmQuota.setDataAtCell(e.row, e.col + 1, result.TransmissionType);
        }
    });
}

function Colour(sender, e, val) {
    sdms.lookup({
        title: 'Tipe Kendaraan',
        url: 'wh.api/pmquota/ColorCode?Tipe=' + hotpmQuota.getDataAtCell(e.row, e.col - 3) + ' &Varian=' + hotpmQuota.getDataAtCell(e.row, e.col - 2) + ' &Tranmisi=' + hotpmQuota.getDataAtCell(e.row, e.col - 1),
        sort: [{ field: 'GroupCode', dir: 'asc' }],
        fields: [
            { name: "ColourCode", text: "Colour Code" },
            { name: "ColourName", text: "Colour Name" },
        ],
        dblclick: function (result) {
            hotpmQuota.setDataAtCell(e.row, e.col, result.ColourCode);
        }
    });
}

function rowToJson(data) {
    var returnObj = new Object();
    returnObj.CompanyCode = data[0];
    returnObj.PeriodYear = data[1];
    returnObj.PeriodMonth = data[2];
    returnObj.TipeKendaraan = data[3];
    returnObj.Variant = data[4];
    returnObj.Transmisi = data[5];
    returnObj.ColourCode = data[6];
    returnObj.QuotaQty = data[7];
    returnObj.IndentQty = data[8];
    returnObj.QuotaBy = data[9];
    return returnObj;
}
