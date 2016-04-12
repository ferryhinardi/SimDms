var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemModificationController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=INCD').
    success(function (data, status, headers, config) {
        me.comboInterChangeCode = data;
    });

    me.browse = function () {

        var lookup = Wx.blookup({
            name: "itemModifBrowse",
            title: "Lookup Item Modification",
            manager: spManager,
            query: "ItemPartModifBrowse",
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "Part No" },
            { field: "NewPartNo", title: "New PartNo" },
            { field: "InterchangeCode", title: "Interchange Code" },
            { field: "ProductType", title: "Product Type" },
            { field: "PartCategory", title: "Part Category" } 

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
              
                me.lookupAfterSelect(data);
                me.data.InterChangeCode =  data.InterchangeCode;
                me.ItemModifSelect(data.NewPartNo, "new")
                me.ItemModifSelect(data.PartNo, "old")

                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.ItemModifSelect = function (s,n) {
        var url = "sp.api/ItemModification/GetObject?SQL=sp_SpMstItemModifSelect '" + me.data.CompanyCode + "','" + s + "'"
     
        $http.post(url).
        success(function (data, status, headers, config) {
            if (!data[0]) {
                if (n == "new") {
                    me.data.NewPartNo = '';
                    me.data.PartNameBaru = '';
                    me.data.ProductTypeBaru = '';
                    me.data.ProductTypeNameBaru = '';
                    me.data.PartCategoryBaru = '';
                    me.data.CategoryNameBaru = '';
                    me.newPartNo();
                } else {
                    me.data.PartNo = '';
                    me.data.PartNameLama = '';
                    me.data.ProductTypeLama = '';
                    me.data.ProductTypeNameLama = '';
                    me.data.PartCategoryLama = '';
                    me.data.CategoryNameLama = '';
                    me.oldPartNo();
                }
            } else {
                if (n == "new") {

                    me.data.NewPartNo = data[0].PartNo;
                    me.data.PartNameBaru = data[0].PartName;
                    me.data.ProductTypeBaru = data[0].ProductType;
                    me.data.ProductTypeNameBaru = data[0].ProductTypeName;
                    me.data.PartCategoryBaru = data[0].PartCategory;
                    me.data.CategoryNameBaru = data[0].CategoryName;
                } else {
                    me.data.PartNo = data[0].PartNo;
                    me.data.PartNameLama = data[0].PartName;
                    me.data.ProductTypeLama = data[0].ProductType;
                    me.data.ProductTypeNameLama = data[0].ProductTypeName;
                    me.data.PartCategoryLama = data[0].PartCategory;
                    me.data.CategoryNameLama = data[0].CategoryName;
                }

            }
        });
    }

    me.newPartNo = function () {
        var lookup = Wx.klookup({
            name: "PartNoLookup",
            title: "Lookup Part No",
            serverBinding: true,
            url: "sp.api/Grid/ItemPartLookupGrid",
            pageSize: 10,
            columns: [
            { field: "PartNo", title: "Part No" },
            { field: "PartName", title: "Part Name" },
            { field: "SupplierCode", title: "Supplier Code" },
            { field: "IsGenuinePart", title: "IsGenuinePart" },
            { field: "ProductType", title: "ProductType" },
            { field: "PartCategory", title: "Part Category" },
            { field: "CategoryName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NewPartNo = data.PartNo;
                me.data.PartNameBaru = data.PartName;
                me.data.ProductTypeBaru = data.ProductType;
                me.data.ProductTypeNameBaru = data.ProductTypeName;
                me.data.PartCategoryBaru = data.PartCategory;
                me.data.CategoryNameBaru = data.CategoryName;
     
 
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.oldPartNo = function () {
        var lookup = Wx.klookup({
            name: "PartNo2Lookup",
            title: "Lookup PartNo",
            serverBinding: true,
            url: "sp.api/Grid/ItemPartLookupGrid",
            pageSize: 10,
            columns: [
            { field: "PartNo", title: "Part No" },
            { field: "PartName", title: "Part Name" },
            { field: "SupplierCode", title: "Supplier Code" },
            { field: "IsGenuinePart", title: "IsGenuinePart" },
            { field: "ProductType", title: "ProductType" },
            { field: "PartCategory", title: "Part Category" },
            { field: "CategoryName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.data.PartNameLama = data.PartName;
                me.data.ProductTypeLama = data.ProductType;
                me.data.ProductTypeNameLama = data.ProductTypeName;
                me.data.PartCategoryLama = data.PartCategory;
                me.data.CategoryNameLama = data.CategoryName;
 
                me.isSave = false;
                me.Apply();
            }
        });
    }
 
    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            me.ItemModifSelect($('#PartNo').val(), "old");
            //$http.post('gn.api/masteritem/CheckItem?PartNo=' + $('#PartNo').val()).
            //success(function (v, status, headers, config) {
            //    if (v.masterinfo) {
            //        me.data.PartNameLama = v.masterinfo.PartName;
            //        me.data.ProductTypeLama = v.masterinfo.ProductType;
            //        me.data.ProductTypeNameLama = v.masterinfo.ProductType;
            //        me.data.PartCategoryLama = v.masterinfo.PartCategory
            //        me.data.CategoryNameLama = v.masterinfo.
            //    } else {
            //        $('#PartNo').val('');
            //        $('#PartNameLama').val('');
            //        me.newPartNo();
            //    }
            //});
        } else {
            me.data.PartNo = '';
            me.data.PartNameLama = '';
            me.data.ProductTypeLama = '';
            me.data.ProductTypeNameLama = '';
            me.data.PartCategoryLama = '';
            me.data.CategoryNameLama = '';
            me.oldPartNo();
        }
    });

    $("[name = 'NewPartNo']").on('blur', function () {
        if ($('#NewPartNo').val() || $('#NewPartNo').val() != '') {
            me.ItemModifSelect($('#NewPartNo').val(), "new");
            
        } else {
            me.data.NewPartNo = '';
            me.data.PartNameBaru = '';
            me.data.ProductTypeBaru = '';
            me.data.ProductTypeNameBaru = '';
            me.data.PartCategoryBaru = '';
            me.data.CategoryNameBaru = '';
            me.newPartNo();
        }
    });

    me.saveData = function (e, param) {
        console.log(param);
        if (param.InterChangeCode == '') {
            MsgBox('InterChange Code Harus diisi', MSG_ERROR);
            return;
        }

        $http.post('sp.api/ItemModification/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e); 
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/itemmodification/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");

                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e); 
                });
            }
        });
    }

    me.initialize = function () {
        $http.get('breeze/sparepart/CurrentUserInfo').
        success(function (dl, status, headers, config) {
            me.data.CompanyCode = dl.CompanyCode;
            me.data.EndMark = 0;
            me.data.UnitConversion = 1;
      });
    }

 me.start();
}

$(document).ready(function () {
    var options = {
        title: "Item Modification",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [
            {
                name: "pnlInfoPartLama",
                title: "Part Old Information",
				items: [
				
                        {
                            text: "Part No Old",
                            type: "controls",
							required: true,
                            items: [
                                { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", readonly: false,  validasi:"required" ,click:"oldPartNo()"},
                                { name: "PartNameLama", cls: "span6", placeHolder: "Part Name Old", readonly: true , required: true}
                            ]
                        },			
                        {
                            text: "Product Type",
                            type: "controls",
                            cls : "span3",
                            items: [
                                { name: "ProductTypeLama", cls: "span2", placeHolder: " ", readonly: true, type : "text" },
                                { name: "ProductTypeNameLama", cls: "span", placeHolder: " ", readonly: true, type: "text" },
								]
                        },
                        {
                            text: "Category Part",
                            type: "controls",
                            cls: "span3",
                            items: [
                                
								{ name: "PartCategoryLama", cls: "span2", placeHolder: " ", readonly: true, text: "Kategori Part", type: "text" },
                                { name: "CategoryNameLama", cls: "span", placeHolder: " ", readonly: true, type: "text" }
                            ]
                        },

				   
                ]
            },
            {
                name: "pnlInfoPartBaru",
                title: "Part New Information",
                items: [
                        {
                            text: "Part No New",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "NewPartNo", cls: "span2", placeHolder: "Part No New", type: "popup", btnName: "btnNewPartNo", readonly: false,  validasi:"required", click: "newPartNo()" },
                                { name: "PartNameBaru", cls: "span6", placeHolder: "Part Name New", readonly: true, required: true }
                            ]
                        },
                        {
                            text: "Product Type",
                            type: "controls",
                            cls: "span3",
                            items: [
                                { name: "ProductTypeBaru", cls: "span2", placeHolder: " ", readonly: true },
                                { name: "ProductTypeNameBaru", cls: "span", placeHolder: " ", readonly: true },
								
                            ]
                        },
                         {
                             text: "Category Part",
                             type: "controls",
                             cls: "span3",
                             items: [

                                 { name: "PartCategoryBaru", cls: "span2", placeHolder: " ", readonly: true, text: "Kategori Part" },
                                 { name: "CategoryNameBaru", cls: "span", placeHolder: " ", readonly: true }
                             ]
                         },
				        { name: "InterChangeCode", text: "InterChange Code", type: "select2", cls: "span3", datasource: "comboInterChangeCode", required: true, validasi: "required"},
                        { name: "EndMark", text: "End Mark", cls: "span2 ", required: true, validasi: "required"},
                        { name: "UnitConversion", text: "Unit Conversion", cls: "span3  ", required: true,  readonly: true }
                ]
            },

 
        ]
    };

 



/*
            case "ItemBrowse":
                widget.populate($.extend({}, widget.default, data));
                widget.lookup.hide();

               
          
                var url = "sp.api/ItemModification/GetObject"

                $.ajax({
                    async: false,
                    type: "POST",
                    url: url,
                    data: { sql: "sp_SpMstItemModifSelect", companycode: data.CompanyCode, partno: data.PartNo }
                }
                ).done(function (x) {
                    $("#PartNameLama").val(x.result[0].PartName);
                    $("#ProductTypeLama").val(x.result[0].ProductType);
                    $("#ProductTypeNameLama").val(x.result[0].ProductTypeName);
                    $("#PartCategoryLama").val(x.result[0].PartCategory);
                    $("#CategoryNameLama").val(x.result[0].CategoryName);
                });

   
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { sql: "sp_SpMstItemModifSelect", companycode: data.CompanyCode, partno: data.NewPartNo }
                }
                ).done(function (x) {
 
                    $("#PartNameBaru").val(x.result[0].PartName);
                    $("#ProductTypeBaru").val(x.result[0].ProductType);
                    $("#ProductTypeNameBaru").val(x.result[0].ProductTypeName);
                    $("#PartCategoryBaru").val(x.result[0].PartCategory);
                    $("#CategoryNameBaru").val(x.result[0].CategoryName);
                });

 */

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemModificationController");
    }






});