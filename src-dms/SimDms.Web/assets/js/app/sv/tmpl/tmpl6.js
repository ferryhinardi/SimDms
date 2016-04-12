$(document).ready(function () {
    var options = {
        title: "Popup",
        xtype: "report",
        toolbars: [{ name: "btnProcess", text: "Process", icon: "icon-bolt" }],
        items: [
            { name: "field01", text: "Field 01", cls: "span4" },
            { name: "field03", text: "Field 03", cls: "span4" },
            { name: "field04", text: "Field 07", type: "datepicker", cls: "span4" },
            { name: "field05", text: "Field 08", type: "datepicker", cls: "span4" },
            { name: "field06", text: "Field 09", type: "popup", cls: "span4", btnName: "btnLookup1" },
            { name: "field02", text: "Field 02", type: "popup", cls: "span4", btnName: "btnLookup2" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        $(".frame").css({ top: 185 });
        $(".frame iframe").attr('src', SimDms.baseUrl + 'assets/js/app/sv/tmpl/tmpl6.js');
    })
});