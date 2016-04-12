"use strict"

function omUtilUploadPolda($scope, $http, $injector) {
    var me = $scope;
    var routeURL = "om.api/UploadFakturPolda/";

    $injector.invoke(BaseController, this, { $scope: me });

    me.tableDefault = [
      ["No. Faktur", "Tgl. Faktur", "Merk", "Tipe", "Thn Pembuatan", "Thn Perakitan", "Silinder", "Warna", "No. Rangka", "No. Mesin", "Bahan Bakar", "Pemilik", "Pemilik2", "Alamat", "Alamat2", "Alamat3", "Dealer Code", "Dealer Name", "Jenis Kendaraan", "No Form A", "Tgl Form A", "No KTP", "No Telp", "No HP", "No PIB", "No SUT", "No TPT", "No SRUT"]
    ]


    me.rowToJson = function (data) {
        var returnObj = new Object();
        returnObj.NoFaktur	    = data[0];
        returnObj.TglFaktur     = moment(data[1], 'DD-MM-YYYY').format('YYYY-MM-DD');
        returnObj.Merk	        = data[2];       
        returnObj.Tipe	        = data[3];   
        returnObj.ThnPembuatan  = data[4];
        returnObj.ThnPerakitan  = data[5];
        returnObj.Silinder	    = data[6];    
        returnObj.Warna	        = data[7];    
        returnObj.NoRangka	    = data[8];
        returnObj.NoMesin	    = data[9];    
        returnObj.BahanBakar	= data[10];   
        returnObj.Pemilik	    = data[11];    
        returnObj.Pemilik2	    = data[12];    
        returnObj.Alamat	    = data[13];    
        returnObj.Alamat2	    = data[14];    
        returnObj.Alamat3	    = data[15];    
        returnObj.DealerCode	= data[16];   
        returnObj.DealerName	= data[17];
        returnObj.JenisKendaraan= data[18];
        returnObj.NoFormA	    = data[19];
        returnObj.TglFormA	    = data[20];
        returnObj.NoKTP	        = data[21];
        returnObj.NoTelp	    = data[22];
        returnObj.NoHP	        = data[23];
        returnObj.NoPIB	        = data[24];
        returnObj.NoSUT	        = data[25];
        returnObj.NoTPT	        = data[26];
        returnObj.NoSRUT        = data[27];

        return returnObj;
    }

    me.jsonToRow = function (returnObj) {
        var data = [
        returnObj.NoFaktur	    = data[0],
        returnObj.TglFaktur     = data[1],
        returnObj.Merk	        = data[2],    
        returnObj.Tipe	        = data[3],
        returnObj.ThnPembuatan  = data[4],
        returnObj.ThnPerakitan  = data[5],
        returnObj.Silinder	    = data[6], 
        returnObj.Warna	        = data[7], 
        returnObj.NoRangka	    = data[8],
        returnObj.NoMesin	    = data[9], 
        returnObj.BahanBakar	= data[10],   
        returnObj.Pemilik	    = data[11],    
        returnObj.Pemilik2	    = data[12],    
        returnObj.Alamat	    = data[13],    
        returnObj.Alamat2	    = data[14],    
        returnObj.Alamat3	    = data[15],    
        returnObj.DealerCode	= data[16],   
        returnObj.DealerName	= data[17],
        returnObj.JenisKendaraan= data[18],
        returnObj.NoFormA	    = data[19],
        returnObj.TglFormA	    = data[20],
        returnObj.NoKTP	        = data[21],
        returnObj.NoTelp	    = data[22],
        returnObj.NoHP	        = data[23],
        returnObj.NoPIB	        = data[24],
        returnObj.NoSUT	        = data[25],
        returnObj.NoTPT	        = data[26],
        returnObj.NoSRUT        = data[27]
        ]
        return data;
    }
    
    me.save = function () {

        var data = window.hotFakturPolda.getData().slice(0);
        data.splice(0, 1);

        if (data.length === 1) return;

        if (me.rowToJson(data[0]).TglFaktur != moment(me.data.FakturDate).format("YYYY-MM-DD") || me.rowToJson(data[0]).DealerCode != me.data.CompanyCode) {
            Wx.Error("Wrong Dealer or Faktur Date"); 
            return;
        }

        var listNoFaktur = [];
        for (var i = 0; i < data.length - 1; i++) {
            listNoFaktur.push(me.rowToJson(data[i]).NoFaktur);
        }

        var postData = {
            Data: JSON.stringify(listNoFaktur)
        }
        $.ajax({
            type: "POST",
            url: 'om.api/UploadPolda/CheckDuplicateData',
            datatype: 'json',
            data: postData,
            success: function (response) {
                if (response) {
                    MsgConfirm("Duplicate data detected. Do you want to replace it with new data?", function (result) {
                        if (result) {
                            var listData = [];
                            for (var i = 0; i < data.length - 1; i++) {
                                listData.push(me.rowToJson(data[i]));
                            }

                            postData = {
                                Data: JSON.stringify(listData)
                            }

                            $.ajax({
                                type: "POST",
                                url: 'om.api/UploadPolda/Save',
                                dataType: 'json',
                                data: postData
                                , success: function (response) {
                                    if (response.success) {
                                        Wx.Success("Data saved...");
                                        saveCallback.call();
                                    }
                                    else {
                                        //Wx.Error("Duplication...");
                                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                    }
                                }
                                , error: function (response) {
                                }
                            });
                        }//end if (result)
                    });
                }
            },
            error: function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            }
        });
    }

    me.$watch('isActive', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.initialize = function () {
        $http.post("om.api/UploadPolda/Default").
        success(function (result) {
            me.data.CompanyCode = result.CompanyCode;
            me.data.CompanyName = result.CompanyName;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });

        me.hasChanged = false;
        me.isActive = "1";
        me.data.AllStatus = false;

    };

    me.Upload = function () {
        alert(me.data);
        //$http.post("om.api/UploadPolda/Upload", me.data).
        //success(function (result) {
        //    me.data.CompanyName = result.CompanyName;
        //}).
        //error(function (data, status, headers, config) {
        //    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        //});

    };

    me.Process = function () {
        alert(me.data.FakturDate);
    }

    webix.event(window, "resize", function () {
        me.gridDCSData.adjust();
    });


    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Upload Faktur Polisi POLDA",
        xtype: "panels",
        panels: [
            {
                name: "pnlInquiry",
                cls: "full",
                items: [

                    { type: "label", text: "Inquiri Sumber Data", cls: "span6", style: "font-size: 14px; color: blue; margin-top: 20px;" },
                    { type: "div", cls: "divider" },
                    {
                        type: "controls",
                        cls: "span8",
                        text: "Dealer",
                        items: [
                            { name: "CompanyName", id: "CompanyName", cls: "span4", type: "text", readonly: true },
                        ]
                    },
                    {
                        type: "optionbuttons",
                        cls: "span8",
                        text: "Sumber Data",
                        name: "isActive",
                        model: "isActive",
                        items: [
                            { name: "0", text: "DCS" },
                            { name: "1", text: "Excel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8",
                        text: "Tanggal Faktur",
                        items: [
                            { name: "FakturDate", id: "FakturDate", cls: "span2", type: 'ng-datepicker', disable: "isActive == 0" }
                        ]
                    },
                    { name: "txtFile", text: "Direktori File ", cls: "span5", readonly: true, type: "upload", icon: "icon-upload", url: "om.api/uploadpolda/upload", callback: "uploadCallback", onUpload: "onUpload", disable: "isActive == 0" },
                    {
                        type: "buttons", cls: "span5", items: [
                            { name: "btnSave", text: "Upload", cls: "btn btn-success span2", icon: "icon icon-upload", click: "save()", disable: "isActive == 0" }
                        ]
                    }
                ]
            },
            { name: 'wxgridcell', xtype: 'wxtable' },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omUtilUploadPolda");
    }

    var rowToJson = function (data) {
        var returnObj = new Object();
        returnObj.NoFaktur = data[0];
        returnObj.TglFaktur = data[1];
        returnObj.Merk = data[2];
        returnObj.Tipe = data[3];
        returnObj.ThnPembuatan = data[4];
        returnObj.ThnPerakitan = data[5];
        returnObj.Silinder = data[6];
        returnObj.Warna = data[7];
        returnObj.NoRangka = data[8];
        returnObj.NoMesin = data[9];
        returnObj.BahanBakar = data[10];
        returnObj.Pemilik = data[11];
        returnObj.Pemilik2 = data[12];
        returnObj.Alamat = data[13];
        returnObj.Alamat2 = data[14];
        returnObj.Alamat3 = data[15];
        returnObj.DealerCode = data[16];
        returnObj.DealerName = data[17];
        returnObj.JenisKendaraan = data[18];
        returnObj.NoFormA = data[19];
        returnObj.TglFormA = data[20];
        returnObj.NoKTP = data[21];
        returnObj.NoTelp = data[22];
        returnObj.NoHP = data[23];
        returnObj.NoPIB = data[24];
        returnObj.NoSUT = data[25];
        returnObj.NoTPT = data[26];
        returnObj.NoSRUT = data[27];
        return returnObj;
    }


    setTimeout(function () {

        var grayRenderer = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#ECECEC';
        };



        var data = [
            ["No. Faktur", "Tgl. Faktur", "Merk", "Tipe", "Thn Pembuatan", "Thn Perakitan", "Silinder", "Warna", "No. Rangka", "No. Mesin", "Bahan Bakar", "Pemilik", "Pemilik2", "Alamat", "Alamat2", "Alamat3", "Dealer Code", "Dealer Name", "Jenis Kendaraan", "No Form A", "Tgl Form A", "No KTP", "No Telp", "No HP", "No PIB", "No SUT", "No TPT", "No SRUT"]
        ],
          container = document.getElementById('wxgridcell'),
          settings1 = {
              data: data,
              colWidths: [120, 110, 60, 90, 65, 65, 55, 90, 100, 100, 60, 90, 70, 100, 100, 100, 70, 100, 70, 50, 50, 110, 70, 70, 70, 70],
              columns: [
                  {
                      //data: "NoFaktur"
                  },
                  {
                      //data: "TglFaktur",
                      //type: 'date',
                      //dateFormat: 'DD-MM-YYYY',
                      //correctFormat: true
                  },
                  {
                      //data: "Merk"
                  },
                  {
                      //data: "Tipe"
                  },
                  {
                      //data: "ThnPembuatan"
                      type: "numeric",
                  },
                  {
                      //data: "ThnPerakitan"
                      type: "numeric",
                  },
                  {
                      //data: "Silinder"
                  },
                  {
                      //data: "Warna"
                  },
                  {
                      //data: "NoRangka"
                  },
                  {
                      //data: "NoMesin"
                  },
                  {
                      //data: "BahanBakar"
                  },
                  {
                      //data: "Pemilik"
                  },
                  {
                      //data: "Pemilik2"
                  },
                  {
                      //data: "Alamat"
                  },
                  {
                      //data: "Alamat2"
                  },
                  {
                      //data: "Alamat3"
                  },
                  {
                      //data: "DealerCode"
                  },
                  {
                      //data: "DealerName"
                  },
                  {
                      //data: "JenisKendaraan"
                  },
                  {
                      //data: "NoFormA"
                  },
                  {
                      //data: "TglFormA"
                  },
                  {
                      //data: "NoKTP"
                  },
                  {
                      //data: "NoTelp"
                  },
                  {
                      //data: "NoHP"
                  },
                  {
                      //data: "NoPIB"
                  },
                  {
                      //data: "NoSUT"
                  },
                  {
                      //data: "NoTPT"
                  },
                  {
                      //data: "NoSRUT"
                  }
              ],
              contextMenu: true,
              minSpareRows: 1,
              currentRowClassName: 'currentRow',
              currentColClassName: 'currentCol',
              autoWrapRow: true,
              manualColumnResize: true,
              persistentState: true,
              rowHeights: [24, 24, 24],
              //width: 800,
              //height: 600,
              //fixedRowsTop: 2,
              //fixedColumnsLeft: 3,
              //className: "htCenter",
              //cell: [ ],
              cells: function (row, col, prop) {
                  var cellProperties = {};

                  if (row < 1) {
                      this.renderer = grayRenderer;
                      cellProperties.readOnly = true;
                  }

                  return cellProperties;

              },
              afterChange: function (changes, source) {

                  if (source !== 'edit') return;

                  if (changes !== null) {
                      var me = this;
                      var newvalue = changes[0][3];
                      var cRow = changes[0][0];
                      console.log(changes[0][1])

                  }

              }
          };

        window.hotFakturPolda = new Handsontable(container, settings1);
        //data[0][1] = "Ford"; //change "Kia" to "Ford" programatically
        hotFakturPolda.render();

        $('.htContainer').css('position', 'absolute');
        $('.htContainer').css('height', '500px');
        $('htContainer').css('width', '100%');

    }, 1000);

});

function saveCallback() {
    $("[name=txtFileShowed]").val('');
    window.hotFakturPolda.alter('remove_row', 1, window.hotFakturPolda.countRows());
    window.hotFakturPolda.setDataAtCell([], null, null, 'loadData');
}

function uploadCallback(result) {
    $("[name=txtFileShowed]").val(result.message);

    if (result.data !== null) {
        var i, n = result.data.length;
        var datadump = [];

        for (i = 0; i < n; i++) {
            //datadump.push(me.jsonToRow(response.data[i]));
            var returnObj = result.data[i];
            datadump.push([1 + i, 0, returnObj.NoFaktur])
            datadump.push([1 + i, 1, returnObj.TglFaktur])
            datadump.push([1 + i, 2, returnObj.Merk])
            datadump.push([1 + i, 3, returnObj.Tipe])
            datadump.push([1 + i, 4, returnObj.ThnPembuatan])
            datadump.push([1 + i, 5, returnObj.ThnPerakitan])
            datadump.push([1 + i, 6, returnObj.Silinder])
            datadump.push([1 + i, 7, returnObj.Warna])
            datadump.push([1 + i, 8, returnObj.NoRangka])
            datadump.push([1 + i, 9, returnObj.NoMesin])
            datadump.push([1 + i, 10, returnObj.BahanBakar])
            datadump.push([1 + i, 11, returnObj.Pemilik])
            datadump.push([1 + i, 12, returnObj.Pemilik2])
            datadump.push([1 + i, 13, returnObj.Alamat])
            datadump.push([1 + i, 14, returnObj.Alamat2])
            datadump.push([1 + i, 15, returnObj.Alamat3])
            datadump.push([1 + i, 16, returnObj.DealerCode])
            datadump.push([1 + i, 17, returnObj.DealerName])
            datadump.push([1 + i, 18, returnObj.JenisKendaraan])
            datadump.push([1 + i, 19, returnObj.NoFormA])
            datadump.push([1 + i, 20, returnObj.TglFormA])
            datadump.push([1 + i, 21, returnObj.NoKTP])
            datadump.push([1 + i, 22, returnObj.NoTelp])
            datadump.push([1 + i, 23, returnObj.NoHP])
            datadump.push([1 + i, 24, returnObj.NoPIB])
            datadump.push([1 + i, 25, returnObj.NoSUT])
            datadump.push([1 + i, 26, returnObj.NoTPT])
            datadump.push([1 + i, 27, returnObj.NoSRUT])
        }

        window.hotFakturPolda.alter('remove_row', 1, window.hotFakturPolda.countRows());
        window.hotFakturPolda.setDataAtCell(datadump, null, null, 'loadData');

    }
}

function onUpload() {
    var a = 1;
}