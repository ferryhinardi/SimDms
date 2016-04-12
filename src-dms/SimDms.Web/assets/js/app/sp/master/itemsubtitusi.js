var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

$(document).ready(function () {
    var options = {
        title: "Item Modification",
        xtype: "panels",
        toolbars: [
         
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnProcess", text: "Create Item", icon: "icon-bolt", cls: "hide" },
            { name: "btnCancel", text: "Cancel Item", icon: "icon-remove", cls: "hide" },
            { name: "btnClose", text: "Close Item", icon: "icon-signout", cls: "hide" },
            { name: "btnOpen", text: "Open Item", icon: "icon-signin", cls: "hide" },
        ],
        panels: [
            {
                name: "pnlInfoPartLama",
                title: "Informasi Part Lama",
				items: [
				
                        {
                            text: "No. Part Lama",
                            type: "controls",
							required: true,
                            items: [
                                { name: "PartNoLama", cls: "span2", placeHolder: "No. Part", type: "popup", btnName: "btnPartNo", readonly: false, required: true },
                                { name: "PartNameLama", cls: "span6", placeHolder: "Nama Part", readonly: true , required: true}
                            ]
                        },			
						
                        {
                            text: "Tipe Produk",
                            type: "controls",
                            items: [
                                { name: "TipeProdukNoLama", cls: "span1", placeHolder: " ", readonly: false },
                                { name: "TipeProdukNameLama", cls: "span2", placeHolder: " ", readonly: true},
								{ name: "KtgProdukNoLama2", type: "label", text:"test", width:"20%" },
                                { name: "KtgProdukNoLama", cls: "span1", placeHolder: " ", readonly: false },
                                { name: "KtgProdukNameLama",  cls: "span1", placeHolder: " ", readonly: true},								
                            ]
                        }	
						
                ]
            },
            {
                name: "pnlInfoPartBaru",
                title: "Informasi Part Baru",
                items: [

				
                ]
            },

 
        ]
    };
	
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(init);

    function init() {
        showPanel("pnlInfoPartLama");
        showPanel("pnlInfoPartBaru");
    }


    function refresh() {
        widget.post("cs.api/summary/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                var record = {};
                $.each(result.list, function (idx, val) {
                    record[val.ControlLink] = val.RemValue;
                })
                widget.populate(record);
            }
        });
    }
    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            $http.post('gn.api/masteritem/CheckItem?PartNo=' + $('#PartNo').val()).
            success(function (v, status, headers, config) {
                if (v.masterinfo) {
                    me.data.PartName = v.masterinfo.PartName;
                } else {
                    $('#PartNo').val('');
                    $('#PartName').val('');
                    me.PartNo();
                }
            });
        } else {
            me.data.PartNo = '';
            me.data.PartName = '';
            me.PartNo();
        }
    });
    $("#btnRefresh").on("click", refresh);
    $("#btnExportXls").on("click", exportXls);
    $("#btnBack").on("click", back);

    $("#btn3DaysCall").on("click", load3DaysCall);
    $("#btnBDayCall").on("click", loadBirthDayCall);
    $("#btnStnkExt").on("click", loadStnkExt);
    $("#btnBpkbRemind").on("click", loadBpkb);
    //$("#btnHoliays").on("click", loadHoliday);
});