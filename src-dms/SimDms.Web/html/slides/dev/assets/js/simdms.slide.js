SimDms.Slide = function (options) {
    "use strict";

    this.render = function (rootId) {
        var selector = "#" + (rootId || "impress");
        var contents = options.contents || [];
        var debug = options.debug || false;

        var html = "";

        var default_content = {
            scale: "1",
            cls: "step",
            x: 0, y: 0, z: 0,
            rx: 0, ry: 0, rz: 0,
            debug: false
        }

        $.each(contents, function (idx, data) {
            var val = $.extend({}, default_content, data);

            var h1 = ((val.h1 == undefined) ? "" : "<h1>" + val.h1 + "</h1>");
            var h2 = ((val.h2 == undefined) ? "" : "<h2>" + val.h2 + "</h2>");
            var h3 = ((val.h3 == undefined) ? "" : "<h3>" + val.h3 + "</h3>");
            var h4 = ((val.h4 == undefined) ? "" : "<h4>" + val.h4 + "</h4>");
            var h5 = ((val.h5 == undefined) ? "" : "<h5>" + val.h5 + "</h5>");

            var title = h1 + h2 + h3 + h4 + h5
            var subtitle = ((val.subtitle == undefined) ? "" : "<h3>" + val.subtitle + "</h3>");
            var img = ((data.img == undefined) ? "" : "<img src=\"assets/img/" + data.img + "\"></img>");

            var items = "";

            if ((data.items || []).length > 0) {
                $.each(data.items || [], function (i, val) {
                    items += "<li>" + val.text + "</li>"
                });
                items = "<ul>" + items + "</ul>";
            }

            var name = (val.name == undefined) ? "" : (" id=\"" + val.name + "\"");
            html += "<div" + name + " class=\"" + val.cls + "\" " +
                    "data-scale=\"" + val.scale + "\"" +
				    "data-x=\"" + val.x + "\" data-y=\"" + val.y + "\" data-z=\"" + val.z + "\" " +
					"data-rotate-x=\"" + val.rx + "\"" + "data-rotate-y=\"" + val.ry + "\"" + "data-rotate-z=\"" + val.rz + "\" >" +
					"<div>" + title + subtitle + items + img + "</div>" +
					"</div>";
        });

        $(selector).empty();
        $(selector).html(html);
        if (!debug) {
            impress().init(rootId);
        }
    }
}