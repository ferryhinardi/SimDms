$(document).ready(function () {
    var options = {
        title: "JS Test",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "FileName", text: "File Name", cls: "span6" },
                    { name: "FileContent", text: "Content", type: "textarea", cls: "span6" },
                ],
            },
        ],
        toolbars: [
            { name: "btnXlsList1", text: "Xls List1", icon: "fa fa-refresh" },
            { name: "btnXlsList2", text: "Xls List2", icon: "fa fa-refresh" },
            { name: "btnXlsList3", text: "Xls List3", icon: "fa fa-refresh" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        $("#btnXlsList1").on("click", function () {
            postData();
        });

        $("#btnXlsList2").on("click", function () {
            widget.post("cs.api/excel/list", { name: "test" }, function (result) {
                console.log(result);
            });
        });

        $("#btnXlsList3").on("click", function () {
            widget.exportXls({
                name: "Customer",
                source: "cs.api/excel/customers",
                items: [
                    { name: "CustomerCode", text: "Cust. Code" },
                    { name: "CustomerName", text: "Cust. Name", width: 1400 }
                ]
            })
        });
    });

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }

    function postData() {
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", SimDms.baseUrl + SimDms.exportXlsUrl);

        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", "name");
        hiddenField.setAttribute("value", $("#FileName").val());

        form.appendChild(hiddenField);

        document.body.appendChild(form);
        form.submit();
    }

    //function exportXls(options) {
    //    var html = "";
    //    $.each(options.headers, function () {
    //        html += "<th>" + this.name + "</th>";
    //    });
    //    html = "<tr>" + html + "</tr>";
    //    $.each(options.data, function (idx, val) {
    //        html += "<tr>";
    //        $.each(options.headers, function () {
    //            html += "<td>" + val[this.name] + "</td>";
    //        });
    //        html += "</tr>";
    //    });
    //    //console.log(html);
    //    html = "<table>" + html + "</table>";

    //    var form = document.createElement("form");
    //    form.setAttribute("method", "post");
    //    form.setAttribute("action", SimDms.baseUrl + "cs.api/excel/export");

    //    var field1 = document.createElement("input");
    //    field1.setAttribute("type", "hidden");
    //    field1.setAttribute("name", "name");
    //    field1.setAttribute("value", "demox");

    //    var field2 = document.createElement("input");
    //    field2.setAttribute("type", "hidden");
    //    field2.setAttribute("name", "html");
    //    field2.setAttribute("value", htmlEscape(html));

    //    form.appendChild(field1);
    //    form.appendChild(field2);
    //    document.body.appendChild(form);
    //    form.submit();
    //}

    //function htmlEscape(str) {
    //    return String(str)
    //            .replace(/&/g, '&amp;')
    //            .replace(/"/g, '&quot;')
    //            .replace(/'/g, '&#39;')
    //            .replace(/</g, '&lt;')
    //            .replace(/>/g, '&gt;');
    //}
});
