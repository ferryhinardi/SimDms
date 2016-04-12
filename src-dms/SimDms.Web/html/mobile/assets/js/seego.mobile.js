SeeGo.Mobile = function (options) {
    var _this = this;

    this.render = function () {
        if (options.debug === undefined || options.debug === false) {
            window.location.href = "#";
        }

        var selector = options.selector || "body";
        var html = "";
        var views = options.views || [];
        $.each(views, function (index, view) {
            html += "<div data-role=\"view\" data-title=\"" + view.title + "\" id=\"" + view.name + "\" data-transition=\"" + (view.transition || "slide:left") + "\">";
            html += generateHeader(view.header);
            html += generateContent(view.content);
            html += generateFooter(view.footer);
            html += "</div>";
        });

        $(selector).html(html);
        var app = new kendo.mobile.Application(document.body);
        if (kendo.ui.Slider) {
            $("[type=range]").kendoSlider({ tooltip: { enabled: false } });
        }
    }

    function generateHeader(header) {
        var html = "";
        html += "<header data-role=\"header\">";
        html += "<div data-role=\"navbar\">";
        html += "<span data-role=\"view-title\"></span>";
        if (header !== undefined) {
            $.each(header || [], function (idx, val) {
                var text = val.text || val.name;
                var name = (val.name === undefined) ? "" : " name=\"" + val.name + "\"";
                var icon = (val.icon === undefined) ? "" : " data-icon=\"" + val.icon + "\"";
                var href = (val.href === undefined) ? "" : " href=\"" + val.href + "\"";
                var role = " data-role=\"" + (val.role || "button") + "\"";
                var alig = " data-align=\"" + (val.align || "left") + "\"";
                var attr = name + icon + href + role + alig;
                html += "<a" + attr + ">" + text + "</a>";
            });
        }
        html += "</div>";
        html += "</header>";
        return html;
    }

    function generateFooter(footer) {
        html = "";
        if (footer !== undefined) {
            html += "<footer data-role=\"footer\">";
            html += "<div data-role=\"tabstrip\">";
            $.each(footer || [], function (idx, val) {
                var text = val.text || val.name;
                var name = (val.name === undefined) ? "" : " name=\"" + val.name + "\"";
                var icon = (val.icon === undefined) ? "" : " data-icon=\"" + val.icon + "\"";
                var href = (val.href === undefined) ? "" : " href=\"" + val.href + "\"";
                var attr = name + icon + href;
                html += "<a" + attr + ">" + text + "</a>";
            });
            html += "</div>";
            html += "</footer>";
        }
        return html;
    }

    function generateContent(content) {
        html = "";
        if (content !== undefined) {
            $.each(content || [], function (idx, panel) {
                switch (panel.type) {
                    case "link":
                        html += generateLink(panel);
                        break;
                    case "buttons":
                        html += generateButtons(panel);
                        break;
                    default:
                        html += generatePanel(panel);
                        break;
                }
            });
        }
        return html;
    }

    function generatePanel(panel) {
        var html = "";
        html += "<ul data-role=\"listview\" data-style=\"inset\" data-type=\"group\">";
        html += "<li>" + (panel.title || "");
        html += "<ul>";
        $.each(panel.items, function (idx, val) {
            html += "<li><label>";
            html += "<div>" + (val.text || "") + "</div>";

            var name = (val.name === undefined) ? "" : " name=\"" + val.name + "\"";
            var attr = name;

            var item_type = val.type || "text";
            switch (item_type) {
                case "textarea":
                    var style = " style=\"" + (val.style || "resize: none") + "\"";
                    html += "<textarea " + attr + style + "></textarea>";
                    break;
                case "btn-group":
                    html += "<ul data-role=\"buttongroup\" data-index=\"0\" data-select=\"regroup\">";
                    $.each(val.buttons || [], function (i, button) {
                        html += "<li style=\"font-size: 0.8em;\">" + button + "</li>";
                    });
                    html += "</ul>";
                    break;
                case "switch":
                    var offLabel = " data-off-label=\"" + (val.offLabel || "No") + "\"";
                    var onLabel = " data-on-label=\"" + (val.onLabel || "Yes") + "\"";
                    var attr = offLabel + onLabel;
                    html += "<div>";
                    html += "<input type=\"checkbox\" data-role=\"switch\" checked=\"checked\"" + attr + "/>";
                    html += "</div>";
                    break;
                default:
                    var type = " type=\"" + item_type + "\"";
                    html += "<input " + attr + type + " />";
                    break;
            }
            html += "</label></li>";
        });
        html += "</ul>";
        html += "</li>";
        html += "</ul>";
        return html;
    }

    function generateLink(panel) {
        var html = "";
        html += "<ul data-role=\"listview\" data-style=\"inset\" data-type=\"group\">";
        html += "<li>" + (panel.title || "");
        html += "<ul>";
        $.each(panel.items, function (idx, val) {
            if (val.text !== undefined) {
                var link = (val.href === undefined) ? val.text : "<a href=\"" + val.href + "\">" + (val.text || "") + "</a>";
                html += "<li>" + link + "</li>";
            }
        });
        html += "</ul>";
        html += "</li>";
        html += "</ul>";
        return html;
    }

    function generateButtons(panel) {
        var html = "";
        html += "<ul data-role=\"listview\" data-style=\"inset\" >";
        $.each(panel.items, function (idx, val) {
            if (val.text !== undefined) {
                var href = " href=\"" + val.href + "\"";
                var clas = " class=\"" + (val.cls || "") + " " + (val.align || "") + "\"";
                var attr = href + clas;
                html += "<a data-role=\"button\"" + attr + ">" + (val.text || "") + "</a>";
            }
        });
        html += "</ul>";
        return html;
    }
};