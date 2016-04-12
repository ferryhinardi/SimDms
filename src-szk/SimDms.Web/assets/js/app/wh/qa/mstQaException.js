$(document).ready(function () {
    var options = {
        title: "Input Master Exception",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", cls: "btn btn-info", icon: "fa fa-refresh", click: "Refresh()" },
            { name: "btnSave", text: "Save", cls: "btn btn-info", icon: "fa fa-save", click: "Save()" },
            { name: "btnDownload", text: "Generate Excel", cls: "btn btn-info", icon: "fa fa-download", additional: "disabled=disabled", click: "Upload()" }
        ],
        panels: [
            {
                name: "pnlPeriod",
                items: [
                    { name: "Event", id: "Event", cls: "span3", text: "Event", type: "select" },
                    {
                        type: "controls",
                        cls: "span7",
                        text: "Period",
                        items: [
                            { name: "PeriodYear", id: "PeriodYear", cls: "span2", type: "select" },
                            { name: "PeriodMonth", id: "PeriodMonth", cls: "span3", type: "select" },
                        ]
                    }
                ]
            },
            { type: "div", cls: "divider" },
            { name: 'wxgridcell', xtype: 'wxtable' },
          
        ]
    };

    var Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        Wx.select({ selector: "[name=Event]", url: "wh.api/combo/QaMstException" });
        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/ListYears" });
        Wx.select({ selector: "[name=PeriodMonth]", url: "wh.api/combo/listofmonth" });
    }

    var mstQaException = exceptionRenderer();

    $('#btnRefresh').click(function () {
        params = $('#pnlPeriod').serializeObject();

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/Questionnaire/ReloadMstException",
            success: function (response) {
                if (response.message == "Success") {
                    ReloadCallback(response);
                    $('#btnDownload').attr('disabled', 'disabled');
                    $('#btnSave').removeAttr('disabled');
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });
    });
});

function exceptionRenderer() {

    var grayRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
    };

    var codeRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.color = '';
        td.style.background = '';
    };

    var data = [
        ["Event", "Server Year", "Server Month", "Max Prev Month Ex"]
    ],
      container = document.getElementById('wxgridcell'),
      settings1 = {
          data: data,
          colWidths: [80, 100, 100, 130],
          columns: [
              {
                  //data: "Event"
              },
              {
                  //data: "Server Year"
                  type: "numeric",
                  format: "0",
                  className: "htCenter"
              },
              {
                  //data: "Server Month"
                  type: "numeric",
                  format: "0",
                  className: "htCenter"
              },
              {
                  //data: "Max Prev Month Ex"
                  type: "numeric",
                  format: "0",
                  className: "htCenter"
              }
          ],
          contextMenu: true,
          minSpareRows: 1,
          currentRowClassName: 'currentRow',
          currentColClassName: 'currentCol',
          autoWrapRow: true,
          manualColumnResize: true,
          persistentState: true,
          cells: function (row, col, prop) {
              var cellProperties = {};

              if (row < 1) {
                  this.renderer = grayRenderer;
                  cellProperties.readOnly = true;
              }

              return cellProperties;
          },
          afterChange: function (changes, source) {
              if (source !== 'edit' && source !== 'paste' && source !== 'autofill') return;

              if (changes !== null) {
                  var me = this;
                  var newvalue = changes[0][3];
                  var cRow = changes[0][0];
                  var cCol = changes[0][1];
              }

              if (source === 'paste' || source === 'autofill') {
                  var row = 1;
                  while (mstQaException.getDataAtCell(row, 3) !== null || mstQaException.getDataAtCell(row, 4) !== null || mstQaException.getDataAtCell(row, 5) !== null || mstQaException.getDataAtCell(row, 6) !== null || mstQaException.getDataAtCell(row, 7) !== null) {
                      var total = 0;
                      for (var i = 3; i < 8; i++) {
                          if (mstQaException.getDataAtCell(row, i) !== "") {
                              total += mstQaException.getDataAtCell(row, i);
                          }
                      }
                      total = total / 5;
                      mstQaException.setDataAtCell(row, 8, total);
                      row++;
                  }
                  return;
              }

              if (cCol > 0 && cCol < 8) {
                  var total = 0;
                  for (var i = 3; i < 8; i++) {
                      if (mstQaException.getDataAtCell(cRow, i) !== "") {
                          total += mstQaException.getDataAtCell(cRow, i);
                      }
                  }
                  total = total / 5;
                  mstQaException.setDataAtCell(cRow, 8, total);
              }
              this.renderer = codeRenderer;
          }
      };

    mstQaException = new Handsontable(container, settings1);
    mstQaException.render();

    //$('.ht_master').css('position', 'absolute');
    $('.wtHolder').css('margin-top', '0');
    return mstQaException;
}

function ReloadCallback(result) {

    if (result.data !== null) {
        var i, n = result.data.length;
        var datadump = [];

        for (i = 0; i < n; i++) {
            var returnObj = result.data[i];
            datadump.push([1 + i, 0, returnObj.Event])
            datadump.push([1 + i, 1, returnObj.ServerYear])
            datadump.push([1 + i, 2, returnObj.ServerMonth])
            datadump.push([1 + i, 3, returnObj.MaxPrevMonthEx])
        }

        mstQaException.alter('remove_row', 1, window.mstQaException.countRows());
        mstQaException.setDataAtCell(datadump, null, null, 'loadData');

    }
}