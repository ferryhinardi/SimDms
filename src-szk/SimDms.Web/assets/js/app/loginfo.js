$(document).ready(function () {
    $.post(SimDms.baseUrl + "layout/listmodules", function (result) {
        var html = "";
        $.each((result.data || []), function (idx, row) {
            var animate = ""; // "animated fadeInRight animate " + idx;
            if (row.IsPublish) {
                if (row.InternalLink) {
                    html += "<li class=\"" + animate + "\"><a href=\"" + SimDms.baseUrl + "layout#lnk/" + row.ModuleId + "\" >" + row.ModuleCaption + "</a></li>";
                }
                else {
                    html += "<li class=\"" + animate + "\"><a href=\"" + row.ModuleUrl + "?id=" + result.id + "\" >" + row.ModuleCaption + "</a></li>";
                }
            }
            else {
                html += "<li class=\"" + animate + "\"><label>" + row.ModuleCaption + "</label></li>";
            }
        });
        var modules = $("div.modules");
        modules.empty();
        modules.html("<ul>Modules" + html + "</ul>");
    });
});

Array.prototype.sortBy = function () {
    function _sortByAttr(attr) {
        var sortOrder = 1;
        if (attr[0] == "-") {
            sortOrder = -1;
            attr = attr.substr(1);
        }
        return function (a, b) {
            var result = (a[attr] < b[attr]) ? -1 : (a[attr] > b[attr]) ? 1 : 0;
            return result * sortOrder;
        }
    }
    function _getSortFunc() {
        if (arguments.length == 0) {
            throw "Zero length arguments not allowed for Array.sortBy()";
        }
        var args = arguments;
        return function (a, b) {
            for (var result = 0, i = 0; result == 0 && i < args.length; i++) {
                result = _sortByAttr(args[i])(a, b);
            }
            return result;
        }
    }
    return this.sort(_getSortFunc.apply(null, arguments));
}