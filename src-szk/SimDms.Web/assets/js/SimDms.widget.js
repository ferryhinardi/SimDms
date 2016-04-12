function openWindow(url, title, height, width, additionalOptions) {
    var container,
        content,
        options,
        wnd,
        name;

    // Replace non-alphanumeric characters
    //name = title.replace(/[^a-zA-Z0-9_]/, '');
    name = kendo.guid();

    // Default options
    options = {
        title: title,
        name: name,
        height: '70%',
        width: '80%',
        actions: ["Minimize", "Maximize", "Close"],
        iframe: true,
        content: url,
        animation: false,
        position: {
            left: '10%',
            top: '10%'
        },
        close: function (e) {
            this.destroy();
        }
    }

    try {
        options = mergeObjects(options, additionalOptions);
    } catch (e) {
        console.log('Failed while merging options:');
        console.log(e);
    }

    try {
        container = $('<div />').attr("id", name).appendTo(document.body);
    } catch (e) {
        console.log('Failed while creating/appending container to document body:');
        console.log(e);
    }

    try {
        wnd = container.kendoWindow(options).data('kendoWindow');
    } catch (e) {
        console.log('Failed while creating Kendo window:\n');
        console.log(e);
    }
}

function mergeObjects(obj1, obj2) {
    var obj3 = {};
    for (var attrname in obj1) { obj3[attrname] = obj1[attrname]; }
    for (var attrname in obj2) { obj3[attrname] = obj2[attrname]; }
    return obj3;
}

SimDms.Widget = function (options) {
    "use strict";

    var oTable = null;
    var _this = this;

    var selector = SimDms.selector + " .main";
    var ajaxSettings = { ajaxLoader: $(".ajax-loader"), urlList: undefined, urlSave: undefined, urlDelete: undefined };
    var rowSettings = { evtchanged: [], evtselected: [], isSelected: false };
    var tabSettings = { tabchanged: [] };
    var tableSettings = { evtclick: [] };
    //    var gridSettings = { evtclick: [] };

    this.render = function (callback) {
        var xtype = options.xtype || "form";
        var content = $(selector);
        var html = "";
        var _this = this;
        SimDms.activeObject = this;
        SimDms.onSecondChanged = undefined;
        SimDms.onTenSecondChanged = undefined;

        switch (xtype) {
            case "form":
                html = this.generateForm(options.items);
                break;
            case "panel":
                html = this.generatePanel(options);
                break;
            case "grid":
                html = this.generateGrid(options);
                break;
            case "report":
                html = "<form class=\"report\">" + this.generatePanel(options) + "<div class=\"panel frame\"><iframe></iframe></div></form>";
                break;
            case "panels":
                html = "<form>" + this.generatePanels(options.panels) + "</form>";
                break;
            case "grid-form":
                html = this.generateGrid(options) + this.generateForm(options.items);
                break;
            case "panel-form":
                html = "<form class=\"gl-panel\">" + this.generatePanels(options.panels) + "</form>" + this.generateForm(options.items);
                break;
            case "grid-panels":
                html = this.generateGrid(options) + "<form class=\"gl-form\">" + this.generatePanels(options.panels) + "</form>";
                break;
            default:
                break;
        }

        var html_title = ((options.title === undefined) ? "" : "<h3>" + options.title + "</h3>") +
                         ((options.subtitle === undefined) ? "" : "<h5>" + options.subtitle + "</h5>");

        $(".page .title h3").html(options.title);
        $(".page .toolbar").html(this.generateButtons(options.toolbars));
        $(".page .toolbar button").off();
        $(".page .toolbar button").on('click', function () {
            var __this = $(this);
            var action = __this.data('action');
            if (options.onToolbarClick && action) {
                options.onToolbarClick(action);
            }
        });

        var html_widget = "<div class=\"gl-widget\">" + html + "</div>";
        content.html(html_widget);

        // render generic behaviour
        var controls = [];
        $(".spinner").spinner();
        $(".buttonset").buttonset();
        $(".datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
            dateFormat: "dd-M-yy",
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
        });

        // begin kendo controls
        //$('#from-input').multiDatesPicker();
        //$('#from-input').removeClass("hasDatepicker");
        //$('#from-input').removeAttr("class");
        $("select").select2({ width: "100%" });

        $("[data-type=kdatepicker]").kendoDatePicker({ format: "dd-MMM-yyyy" });
        $("[data-type=kmonthpicker]").kendoDatePicker({ start: "year", depth: "year", format: "MMMM yyyy" });
        $("[data-type=ktimepicker]").kendoTimePicker({ format: "HH:mm" });
        $("[data-type=kdatetimepicker]").kendoDateTimePicker({ format: "dd-MMM-yyyy HH:mm" });
        $("[data-type=decimal]").kendoNumericTextBox({ format: "n2", step: 0.01 });
        controls = $("[data-type=int]") || [];
        for (var i = 0; i < controls.length; i++) {
            var control = $(controls[i]);
            control.kendoNumericTextBox({ format: "n0", step: 1, min: control.data('min'), max: control.data('max') });
        }

        $("[data-type=intt]").kendoNumericTextBox({ format: "n0", step: 1 });
        $("[data-type=krichtext]").kendoEditor({ encoded: true });
        $(".switch").on("click", function () {
            var name = $(this).children()[0].name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
        });
        $("form").on("submit", function (e) {
            e.preventDefault();
        });
        $(".tabs > p").on("click", function () {
            var $this = $(this);
            $this.parent().children().removeClass("active");
            $this.addClass("active");
            var id = $this.parent().data("id");
            var name = $this.data("name");
            for (var i = 0; i < tabSettings.tabchanged.length; i++) {
                tabSettings.tabchanged[i](this, id, name);
            }
            $(".panel." + id).hide();
            $(".panel." + id + "." + name).show();
        });
        $(".panel > .subtitle > .icon").on("click", function () {
            if ($(this).parent().hasClass("collapse")) {
                $(this).parent().removeClass("collapse");
                $(this).parent().parent().children().slideDown();
                $(this).removeClass("fa-plus").addClass("fa-minus");
            }
            else {
                $(this).parent().addClass("collapse");
                $(this).parent().parent().children().not(".subtitle").slideUp();
                $(this).removeClass("fa-minus").addClass("fa-plus");
            }
        });
        $("input[type=text], textarea").on("blur", function () {
            var attr = $(this).attr("class");
            var ignoreCaseStatus = -1;

            if (_this.isNullOrEmpty(attr) == false) {
                ignoreCaseStatus = attr.search("ignore-uppercase");
            }

            var dateTypeInputStatus = $(this).attr("data-type");
            if (dateTypeInputStatus == "datepicker") {
                return;
            }

            if (ignoreCaseStatus < 0) {
                var val = $(this).val().toUpperCase();
                $(this).val(val);
            }
        });
        $("input[type=text].number").on("blur", function () {
            var val0 = $(this).val();
            var valt = number_format(val0, 2);
            $(this).val(valt)
        });
        $("input[type=text].number-int").on("blur", function () {
            var val0 = $(this).val();
            var valt = number_format(val0, 0);
            $(this).val(valt)
        });
        $("input[type=text]").keydown(function (evt) {
            if (evt.keyCode == 27) {

            }
        });

        var tab_active = $(".tabs > p.active");
        if (tab_active.length > 0) {
            tab_active[0].click();
        }
        else {
            var tabs = $(".tabs > p");
            if (tabs.length > 0) {
                tabs[0].click();
            }
        }

        // setting ajax url
        if (options.urlList != undefined) {
            ajaxSettings.urlList = SimDms.baseUrl + options.urlList;
        }

        if (options.urlSave != undefined) {
            ajaxSettings.urlSave = SimDms.baseUrl + options.urlSave;
        }

        if (options.urlDelete != undefined) {
            ajaxSettings.urlDelete = SimDms.baseUrl + options.urlDelete;
        }

        if (options.onSwitchChange) {
            $('.switch [data-type=switch]').on('change', function (e) {
                var _this = this;
                setTimeout(function () {
                    var name = $(_this).attr('name');
                    var value = eval($(_this).val());
                    options.onSwitchChange(name, value);
                }, 400);
            });
        }

        this.showGrid(xtype, options);

        if (callback !== undefined) {
            callback("configured");
        }

        this.initializeEvent();
        this.initializeUpload();
        this.initializeSelect();
        this.initializePopup();
        this.initializeInnerGrid();
        this.initializeImagesInput();
    };

    this.generateForm = function (items, prefix) {
        var html = "";
        var self = this;

        $.each(items || [], function (idx, item) {
            var cls = ((item.cls === undefined) ? "" : " class='" + item.cls + "'");
            html += "<div" + cls + ">" + self.generateLabel(item) + "<div>" + self.generateInput(item) + "</div></div>";
        });

        return "<form class=\"gl-form\">" + (prefix || "") + html + "</form>";
    };

    this.generatePanel = function (options, prefix) {
        var items = options.items;
        var html = "";
        var self = this;

        if (options.type == "slide") {
            (options.items || []).forEach(function (item) {
                if (item.divs) {
                    html += '<div id="' + item.name + '">';
                    (item.divs || []).forEach(function (div) {
                        html += '<div class=' + div + '></div>';
                    });
                    html += '</div>';
                }
                else {
                    html += '<div id="' + item.name + '"></div>';
                }
            });
            return '<div class="slide ' + (options.cls || '') + '">' + html + '</div>';
        }
        /* Add By Ferry */
        else if (options.type == "dashboard") {
            html += "<div class=\"divider\"></div>";
            (options.items || []).forEach(function (item) {
                html += '<div id="' + item.name + '" class=\"' + (item.cls || "") + '\"></div>';
            });
            return '<div class="' + (options.cls || "") + '">' + html + '</div>';
        }
        /******************/
        else {
            $.each(items || [], function (idx, item) {
                var cls = ' class="' + (item.cls ? item.cls : '') + (options.type == 'text-left' ? ' left' : '') + '"';
                var style = typeof item.ctrlstyle != 'undefined' ? item.ctrlstyle : '';
                var styleSpan = typeof item.widthspanstyle != 'undefined' ? item.widthspanstyle : '';
                switch (item.type || "") {
                    case "divider":
                        html += self.generateInput(item);
                        break;
                    case "controls":
                        html += "<div" + cls + styleSpan + " data-panel='" + (item.panel || '') + "'>" + self.generateLabel(item) + "<div " + style + ">" + self.generateInput(item) + "</div></div>"; break;
                    default:
                        html += "<div" + cls + styleSpan + ">" + self.generateLabel(item) + "<div " + style + ">" + self.generateInput(item) + "</div></div>";
                        break;
                }
            });

            var name = ((options.name === undefined) ? "" : (" id=\"" + options.name + "\""));
            var cls = ((options.cls === undefined || options.cls === "") ? " class=\"panel\"" : (" class=\"panel " + options.cls + "\""));
            return "<div" + name + cls + ">" + (prefix || "") + html + "</div>";
        }
    };

    this.generateButtons = function (buttons) {
        var html = "";
        (buttons || []).forEach(function (val) {
            var action = (val.action == undefined) ? '' : ' data-action="' + val.action + '"';
            var addi = (typeof val.additional != 'undefined') ? val.additional : ' ';
            var btype = (typeof val.btntype != 'undefined') ? val.btntype : ' ';
            html += "<button " + action + " class='button small " + (val.cls || "") + "' name='" + val.name + "' id='" + val.name + "'" + btype + addi + "><i class='icon " + (val.icon || "") + "'></i>" + val.text + "</button>";
        });
        return html;
    }

    this.generateGrid = function (params) {
        var columns = options.columns || [];
        var filterHtml = "";
        var html = "";
        var self = this;

        if (options.filters !== undefined) {
            $.each(options.filters, function (idx, item) {
                filterHtml += "<div>" + self.generateLabel(item) + "<div>" + self.generateInput(item) + "</div></div>";
            });
            filterHtml = "<form class='filter'>" + filterHtml + "</form>";
        }

        if (options.urlList === undefined) {
            $.each(columns, function (idx, val) {
                if (val.bVisible === undefined || val.bVisible) {
                    sWidth = (val.sWidth == undefined) ? "" : " style=\"width:" + val.sWidth + "\"";
                    html += "<th" + sWidth + ">" + val.sTitle || "" + "</th>"
                }
            });

            return "<div class='gl-grid'>" + filterHtml + "<div class='dataTables_wrapper'><table class='dataTable'><thead><tr>" + html + "</tr></thead><tbody></tbody></table></div></div>";
        }

        return "<div class='gl-grid'>" + filterHtml + "<table><thead><tr>" + html + "</tr></thead><tbody></tbody></table></div>";
    }

    this.generateKGrid = function (options, prefix, kgridext) {
        if (options.columns !== undefined) {
            this.kgrids.push(options);
        }

        var html = "<div><div id=\"" + options.name + "\"></div></div>";
        var kGrid = "";
        if (options.tabName != undefined) {
            kGrid = "<div class=\"panel kgrid innergrid_wrapper " + (options.tabName || "") + " " + (options.cls || "") + "\">" + (prefix || "") + html + "</div>";
        }
        else {
            kGrid = "<div class=\"panel kgrid " + (kgridext || "") + "\">" + (prefix || "") + html + "</div>";
        }
        return kGrid;
    }

    this.generateChart = function (options, prefix) {
        return '<div class=\'panel kchart\'>' + (prefix || '') + '<div><div id=\'' + options.name + '\'></div></div>' + (options.footer || '') + '</div>';
    }

    this.generateImage = function (options) {
        var html = "";

        var size = "width: " + options.size[0] + "px;";
        size += "height: " + options.size[1] + "px;";

        var margins = "";
        if (this.isNullOrEmpty(options.margins) == false) {
            margins += "margin-left: " + (options.margins.left || 0) + "px";
        }

        var color = "background-color: black; color: white;";

        var style = size + margins;

        var imgStyle = "margin-bottom: 12px;";
        imgStyle += "cursor: pointer;";

        html += "<div class='' style='" + style + "' class='" + (options.cls || "") + "'>";
        html += "<img name='" + options.name + "' id='" + options.name + "' src='" + options.src + "' style='" + size + imgStyle + "' />";
        html += "</div>";

        this.setImageSettings(options);
        return html;
    }

    this.generateInnerGrid = function (options, suffix) {
        var columns = options.columns || [];
        var html = "";
        var self = this;
        html += "<div class='panel innergrid_wrapper " + (options.cls || "") + "'>";
        html += "<div class='subtitle'>" + options.title + "</div>";
        html += "<form id='" + (options.formName || "") + "' name='" + (options.formName || "") + "'>";
        html += this.generatePanel($.extend({}, options, { name: options.pnlname, cls: "hide" }));
        html += "</form>";
        if (self.isNullOrEmpty(options.buttons) == false && options.buttons.length > 0) {
            html += "<div class='toolbars'>";

            $.each(options.buttons, function (key, val) {
                html += "<button class='button small " + (val.cls || "") + "' name='" + (val.name || "") + "' id='" + (val.name || "") + "'><i class='icon " + (val.icon || "") + "'></i>" + (val.text || "Add") + "</button>";
            })

            html += "</div>";
            html += "<div class='divider'></div>";
        }
        html += "<table class='dataTable' name='" + (options.tblname || options.name) + "' id='" + (options.tblname || options.name) + "'>";
        html += "<thead>";
        $.each(columns, function (key, val) {
            var attr = "";
            if (_this.isNullOrEmpty(val.sWidth) == false) {
                attr += " width='" + val.sWidth.replace("px", "") + "px'";
            }

            html += "<th " + attr + ">" + val.sTitle + "</th>";
        });

        html += "</thead>";
        html += "</tbody>";
        html += "</tbody>";
        html += "</table>";

        html += "</div>";

        self.setInnerGrid(options);
        return html;
    }

    this.generateTable = function (item, prefix) {
        var columns = item.columns || [];
        var buttons = item.buttons || [];
        var html = "", table = "";

        if (item.showcheckbox) {
            html += "<th style=\"width: 40px\"><i class='icon icon-check-empty link'></i></th>";
        }
        $.each(columns, function (idx, val) {
            var style = (val.width === undefined) ? "" : " style=\"width: " + val.width + "px\"";
            var field = (val.name === undefined) ? "" : " data-field=\"" + val.name + "\"";
            var cls = (val.cls === undefined) ? "" : " data-class=\"" + val.cls + "\" class=\"" + val.cls + "\"";
            var type = (val.type === undefined) ? "" : " data-type=\"" + val.type + "\"";
            html += "<th" + style + field + cls + type + ">" + val.text + "</th>";
        });

        if (html.length > 0) {
            table = "<table class=\"dataTable table\" id=\"" + item.tblname + "\"><thead><tr>" + html + "</tr></thead><tbody></tbody></table>";
        }
        return "<div id=\"" + item.name + "\" class=\"panel dataTables_wrapper " +
            ((item.cls === undefined) ? "" : item.cls) + "\">" +
            (prefix || "") + this.generatePanel($.extend({}, item, { name: item.pnlname, cls: "hide" })) +
            "<div class=\"toolbars\">" + this.generateButtons(buttons) + "</div>" + (table || "") +
            "</div>";
    }

    this.generateTableGrid = function (item, prefix) {
        var columns = item.columns || [];
        var buttons = item.buttons || [];
        var html = "", table = "";

        $.each(columns, function (idx, val) {
            var style = (val.width === undefined) ? "" : " style=\"width: " + val.width + "px\"";
            var field = (val.name === undefined) ? "" : " data-field=\"" + val.name + "\"";
            var cls = (val.cls === undefined) ? "" : " data-class=\"" + val.cls + "\" class=\"" + val.cls + "\"";
            var type = (val.type === undefined) ? "" : " data-type=\"" + val.type + "\"";
            html += "<th" + style + field + cls + type + ">" + val.text + "</th>";
        });

        if (html.length > 0) {
            table = "<table class=\"dataTable table\" id=\"" + item.tblname + "\"><thead><tr>" + html + "</tr></thead><tbody></tbody></table>";
        }
        return "<div id=\"" + item.name + "\" class=\"panel dataTables_wrapper " +
            ((item.cls === undefined) ? "" : item.cls) + "\">" +
            (prefix || "") + this.generatePanel($.extend({}, item, { name: item.pnlname, cls: "hide" })) +
            "<div class=\"toolbars\">" + this.generateButtons(buttons) + "</div>" + (table || "") +
            "</div>";
    }

    this.generateTabs = function (item, prefix) {
        var items = item.items || [];
        var html = "", tabs = "";

        if (prefix.length < 30) { prefix = "" };

        $.each(items, function (idx, val) {
            var cls = (val.cls || "") + ((val.active || false) ? (((val.cls == undefined) ? "" : " ") + "active") : "");
            var name = (val.name == undefined) ? "" : val.name;
            var attr = "class=\"" + cls + "\" data-name=\"" + name + "\"";
            html += "<p " + attr + "><a href=\"javascript:void(0)\">" + val.text + "</a></p>";
        });
        tabs = "<div class=\"tabs\" " + ((item.name == undefined) ? "" : "data-id=\"" + item.name + "\"") + ">" + html + "</div>";
        return "<div class=\"panel tab_wrapper\">" + (prefix || "") + tabs + "</div>";
    }

    this.generateWxTable = function (item, prefix) {

        var buttons = item.buttons || [];
        var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");

        return "<div id=\"" + item.name + "_parent\" class=\"panel dataTables_wrapper " + m5 +
            ((item.cls === undefined) ? "" : item.cls) + "\">" +
            "<div width=\"100%\" style='display:cell;padding-top:10px;padding-bottom:15px;padding-left:2px;'>" +
            "<div id=\"" + item.name + "\"  style='display:cell;padding-top:20px;padding-bottom:15px;padding-left:2px;'></div></div></div>";
    }

    this.generatePanels = function (panels) {
        var html = "";
        var self = this;


        $.each(panels || [], function (idx, item) {
            var prefix = "";
            if (item.title !== undefined) {
                prefix += "<div class=\"subtitle\">" + item.title + "<i class='icon fa fa-minus'></i></div>"
            }
            else if (idx > 0) {
                prefix += "<div class=\"divider\"></div>"
            }

            var cls = ((item.cls === undefined) ? "" : " class='" + item.cls + "'");
            var wxCls = ((item.cls === undefined) ? "class='span8'" : " class='" + item.cls + "'");
            var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");
            var xHeight = ((item.height === undefined) ? "" : "height:" + item.height + "px;");

            var xtype = item.xtype || "panel";
            switch (xtype) {
                case "table":
                    html += self.generateTable(item, prefix);
                    break;
                case "table-grid":
                    html += self.generateTableGrid(item, prefix);
                    break;
                case "tabs":
                    html += self.generateTabs(item, prefix);
                    break;
                case "wxtable":
                    html += self.generateWxTable(item, "");
                    break;
                case "grid":
                    html += self.generateInnerGrid(item, prefix);
                    break;
                case "k-grid":
                    html += self.generateKGrid(item, prefix);
                    break;
                case "kgrid":
                    html += self.generateKGrid(item, prefix, "k-panel");
                    break;
                case "chart":
                    html += self.generateChart(item, prefix);
                    break;
                case "k-chart":
                    html += self.generateChart(item, prefix);
                    break;
                    //added by Benedict
                    //for Master Model Mapping
                case "panelStart":
                    html += "<div class=\"panel\">"
                    break;
                case "panelEnd":
                    html += "</div>";
                    break;
                case "wxTbl": 
                    html += "<div " + wxCls + m5 + " style=\"display:cell;margin:0px 0px 0px 0px;width:" + (parseInt(item.width) + 10) +"px \"><div width=\"100%\" style=\"display:cell;padding-top:10px;padding-bottom:0px;padding-left:0px;\">";
                    html += "<div style=\"text-align:center;border-style:solid;border-bottom:none;border-color:#a4bed4;border-width:1px;margin:0px -2px 0px 2px;font-family:Segoe UI;height:30px;line-height:1.6;background:linear-gradient(to bottom,white,#f2f2f2)\">" + item.title + "</div>";
                    html += "<div id=\"" + item.name + "\"  style=\"display:cell;padding-top:00px;padding-bottom:0px;padding-left:2px;padding-right:0px;" + xHeight + "  \"></div></div></div>";
                    break;
                case "wxdiv":
                    html += "<div " + wxCls + m5 + " style=\"display:cell;margin:0px 0px 0px 0px;width:" + item.width + "px \"><div width=\"100%\" style=\"display:cell;padding-top:10px;padding-bottom:0px;padding-left:0px;\">";
                    html += "<div id=\"" + item.name + "\"  style=\"display:cell;padding-top:00px;padding-bottom:0px;padding-left:2px;padding-right:0px;" + xHeight + "  \"></div></div></div>";
                    break;
                    //end added by Benedict
                default:
                    html += self.generatePanel(item, prefix);
                    break;
            }
        });

        return html;
    };

    this.generateLabel = function (item) {
        var required = item.required || false;
        var name = ((item.name || "") === "") ? "" : " for=\"" + item.name + "\"";
        var type = (((item.type || "") === "switch-full") || ((item.cls || "").indexOf("full-length") >= 0)) ? " class=\"full\"" : "";
        var style = typeof item.lblstyle != 'undefined' ? item.lblstyle : '';
        var html = "<label" + name + type + style + ">" + ((required) ? "* " : "") + (item.text || "&nbsp;") + "</label>";
        return html;
    };

    this.generateInput = function (item) {
        var placeHolder = " placeHolder=\"" + ((item.placeHolder || item.placeholder || item.text) || "") + "\"";
        var required = ((item.required || false) ? " required=\"required\"" : "");
        var readonly = ((item.readonly || false) ? " readonly=\"readonly\"" : "");
        var disabled = ((item.disabled || false) ? " disabled=\"disabled\"" : "");
        var action = item.action ? (' data-action="' + item.action + '" ') : '';
        var _any = item.any ? item.any : '';
        var attribute
            = ((typeof item.min == 'number') ? (' data-min="' + item.min + '"') : '')
            + ((typeof item.max == 'number') ? (' data-max="' + item.max + '"') : '');

        var type = item.type || "text";
        var html = "";

        switch (type) {
            case "image":
                html = this.generateImage(item);
                break;
            case 'label':
                html = '<label style="padding:10px 0 0">' + item.text + '</label>';
                break;
            case "grid":
                html = this.generateInnerGrid(item);
                break;
            case "text":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<input type='text' name='" + item.name + "' class='" + item.cls + "' id='" + item.name + "'" + placeHolder + required + readonly + maxlength + disabled + "/>";
                break;
            case "decimal":
                var wrapper = " class=\"k-number-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input name='" + item.name + "' type=\"text\" data-type=\"decimal\" " + placeHolder + required + readonly + "/></div>";
                break;
            case "int":
                var wrapper = " class=\"k-number-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = '<div' + wrapper + '><input name="' + item.name + '" type="text" data-type="int" ' + attribute + placeHolder + required + readonly + '/></div>';
                break;
            case "intt":
                var wrapper = " class=\"k-number-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input name='" + item.name + "' type=\"text\" data-type=\"intt\" " + placeHolder + required + readonly + "/></div>";
                break;
            case "date":
                html = "<input type='text' data-type='date' name='" + item.name + "' id='" + item.name + "'" + placeHolder + required + readonly + "/>";
                break;
            case "textarea":
                html = "<textarea name='" + item.name + "' id='" + item.name + "' " + placeHolder + required + readonly + "></textarea>";
                break;
            case "datepicker":
                html = "<div class='datepicker-wrapper'><input type='text' data-type='datepicker' class='datepicker' placeholder='dd-MMM-yyyy' name='" + item.name + "' id='" + item.name + "' " + required + disabled + "/></div>";
                break;
            case "multidates":
                html = "<div id='" + item.name + "'></div>";
                break;
            case "timepicker":
                html = "<select name='" + item.name + "' id='" + item.name + "' " + placeHolder + required + disabled+ ">";
                var time = moment([1900, 0, 0]), stime = "";
                for (var i = 0; i < 48; i++) {
                    stime = time.format("HH:mm");
                    time = time.add("m", 30);
                    html += "<option value='" + stime + "'>" + stime + "</option>";
                }
                html += "</select>";
                break;
            case "kdatepicker":
                var wrapper = " class=\"k-datepicker-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input type=\"text\" data-type=\"kdatepicker\" placeholder=\"dd-MMM-yyyy\"" + name + required + readonly + "/></div>";
                break;
            case "spinner":
                html = "<div class='spinner-wrapper'><input type='text' class='spinner' name='" + item.name + "' id='" + item.name + "' " + required + "/></div>";
                break;
            case "popup":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper' element-type='popup'><input type='text' name='" + item.name + "' id='" + item.name + "'" + placeHolder + required + readonly + maxlength + "/>" +
                       "<button type=\"button\" " + action + " class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\"><i class=\"icon " + (item.icon || "fa fa-search") + "\"></i></button></div>";

                break;
            case "popup-lookup":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper' element-type='popup'><input type='text' name='" + item.name + "' id='" + item.name + "'" + placeHolder + required + readonly + maxlength + "/>" +
                       "<button type=\"button\" " + action + " class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\" ><i class=\"icon " + (item.icon || "icon-search") + "\"></i></button></div>";

                _this.setPopup(item);

                break;
            case "upload":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper'><input type='hidden' id='" + item.name + "' name='" + item.name + "" + "' ><input type='text' name='" + item.name + "Showed' id='" + item.name + "'Showed" + placeHolder + required + readonly + maxlength + "/>" +
                       "<button binding-type='button-upload' binding-callback='" + item.callback + "' binding-url='" + item.url + "' binding-data='" + item.name + "' type=\"button\" class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\" ><i class=\"icon " + (item.icon || "icon-search") + "\"></i></button>";
                html +="</div>";
                break;
            case "buttons":
                html += _this.generateButtons(item.items);
                break;
            case "select":
                if (item.data === undefined) {
                    var attr = "";

                    var dataSource = item.dataSource;

                    if (dataSource !== undefined) {
                        attr += "dataSource='" + dataSource + "'";
                    }

                    if (item.disabled) attr += ' disabled="disabled"';

                    html = "<select data-type='select' name='" + item.name + "' id='" + item.name + "' " + placeHolder + required + readonly + " " + attr + ">"
                         + ((item.fullItem) ? "" : ("<option value=''>" + (item.opt_text || SimDms.selectOneText) + "</option>"))
                         + (this.generateOption(item))
                         + "</select>";
                }
                break;
            case "dropdown":
                html = "<select name='" + item.name + "' id='" + item.name + "' " + required + ">" +
                       "<option>" + (item.opt_text || SimDms.selectOneText) + "</option>" + this.generateOption(item) +
                       "</select>";
                break;
            case "controls":
                var items = item.items || [];
                $.each(items, function (idx, val) {
                    var item_readonly = ((val.readonly || false) ? " readonly='readonly'" : "");
                    var item_required = ((val.required || false) ? " required='required'" : "");
                    var place_holder = ((val.placeHolder == undefined) ? "" : " placeHolder='" + val.placeHolder + "'");
                    html += "<div class=\"" + (val.cls || "") + "\">" +
                            //"<input type=\"text\" name=\"" + (val.name || "") + "\" id=\"" + (val.name || "") + "\"" + place_holder + item_readonly + item_required + "/>" +
                            _this.generateInput(val) +
                            "</div>";
                });
                html = "<div class=\"controls-wrapper\">" + html + "</div>";
                break;
            case "lookup":
                html = "<div class=\"lookup-wrapper\">" +
                       "<div class=\"span2\">" +
                       "<input type=\"text\" name=\"" + (item.name || "") + "\" readonly=\"readonly\" placeholder=\"" + (item.namePlaceholder || "") + "\" />" +
                       "</div>" +
                       "<div class=\"span6\">" +
                       "<input type=\"text\" name=\"" + (item.display || "") + "\" readonly=\"readonly\" placeholder=\"" + (item.displayPlaceholder || "") + "\" />" +
                       "<button type=\"button\" class=\"button\" id=\"" + (item.btnName || "") + "\" ><i class=\"icon icon-search\"></i></button>" +
                       "</div>" +
                       "</div>";
                break;
            case "switch":
                var float = (item.float === undefined) ? "" : " " + item.float;
                html = "<div class=\"switch" + float + "\">" +
                       "<input id=\"" + item.name + "N\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\"  data-value=\"false\" value=\"false\" checked>" +
                       "<label for=\"" + item.name + "N\" onclick=\"\">Tidak</label>" +
                       "<input id=\"" + item.name + "Y\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\"  data-value=\"true\" value=\"true\" >" +
                       "<label for=\"" + item.name + "Y\" onclick=\"\">Ya</label>" +
                       "<span></span>" +
                       "</div>";
                break;
            case "switch-full":
                var float = (item.float === undefined) ? "" : " " + item.float;
                html = "<div class=\"switch" + float + "\">" +
                       "<input id=\"" + item.name + "N\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\"  data-value=\"false\" value=\"false\" checked>" +
                       "<label for=\"" + item.name + "N\" onclick=\"\">Tidak</label>" +
                       "<input id=\"" + item.name + "Y\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\"  data-value=\"true\" value=\"true\" >" +
                       "<label for=\"" + item.name + "Y\" onclick=\"\">Ya</label>" +
                       "<span></span>" +
                       "</div>";
                break;
            case "separator":
                html = "<div class=\"separator\"></div>";
                break;
            case "divider":
                html = "<div class=\"divider\"></div>";
                break;
            case "panel":
                html = "<div class=\"panel\"></div>";
                break;
            case "div":
                html = "<div id='" + item.name + "' style='margin-top:20px;width:100%'></div>";
                break;
            case "hidden":
                html = "<input type='hidden' name='" + item.name + "' id='" + item.name + "'/>";
                break;
            case "check":
                var spanstyle = typeof item.spanstyle != 'undefined' ? item.spanstyle : '';
                var style = typeof item.style != 'undefined' ? item.style : '';
                html = "<input type='checkbox' name='" + item.name + "' id='" + item.name + "' class='icon icon-check link' style='" + style + "' class='" + item.cls + "'/><span for=\"" + item.name + "\" style='" + spanstyle + " position:absolute; margin-top:10px; padding-left:5px;font-size: .875em; width: 160px; font-family: 'segoeui';'>" + item.text + "</span>";
                break;
            case "table":
                html = "<div id='" + item.name + "' style='width:" + item.width + ";'><input type='hidden' name='" + item.hdnname + "' id='" + item.hdnname + "'/><table>" +
                    "<thead><tr>";

                $.each(item.columns, function (idx, val) {
                    html += "<th style='width:" + val.width + "'>" + val.title + "</th>";
                });

                html += "</tr></thead><tbody id='" + item.dataId + "'></tbody></table></div>";
                break;
                //<div class="radio radio-success radio-inline">
                //        <input id="inlineRadio1" value="option1" name="radioInline" checked="" type="radio">
                //        <label for="inlineRadio1"> Inline One </label>
                //    </div> radio-" + item_color + "
            case "radiobutton":
                var nameRadio = item.name;
                var items = item.items || [];
                var i = 0;
                html += "<div class=\"form-horizontal\">";
                $.each(items, function (idx, val) {
                    i++;
                    var item_checked = (val.checked ? "  checked=\"checked\"" : "");
                    var item_color = (val.color || "default");
                    var item_value = (val.value || "");
                    var item_label = (val.label || "");
                    var item_autoid =  nameRadio + (val.id || i);

                    html += "<label  for=\"" + item_autoid + "\" class=\"radio-inline\"><input type=\"radio\" data-value=\"" + item_value + "\" value=\"" + item_value +
                            "\" name=\"" + nameRadio + "\" id=\"" + item_autoid + "\" " + _any + item_checked + ">" + item_label + "</label>";
                });
                html += "</div>";
                break;
            case "radio-switch":
                var nameRadio = item.name;
                var YA = "Ya", TIDAK = "Tidak";
                var option = item.option || undefined;

                if (option !== undefined)
                {
                    YA = option.Y;
                    TIDAK = option.N;
                }                

                html += "<div class=\"form-horizontal\">";

                html += "<label for=\"" + nameRadio + "Y\" class=\"radio-inline\"><input type=\"radio\" data-value=\"true\"  value=\"true\" class=\"radio radio-primary\"   data-type=\"radio\" id=\"" + nameRadio + "Y\" name=\"" + nameRadio + "\" " + _any + ">" + YA + "</label>";

                html += "<label for=\"" + nameRadio + "N\"   class=\"radio-inline\"><input type=\"radio\" data-value=\"false\"  value=\"false\"  class=\"radio radio-danger\"  data-type=\"radio\" id=\"" + nameRadio + "N\" name=\"" + nameRadio + "\" checked=\"checked\" " + _any + " >" + TIDAK + "</label>";

                html += "</div>";
                break;
            default:
                break;
        }

        return html;
    };

    this.generateOption = function (item) {
        var items = item.items || [];
        var html = "";

        $.each(items, function (idx, val) {
            html += "<option value='" + (val.value || "") + "'>" + (val.text || "") + "</option>"
        });

        return html;
    }

    this.showGrid = function (xtype, options) {
        if (xtype === "grid" || xtype === "grid-form" || xtype === "grid-panels") {
            var columns = options.columns || [],
                sortings = options.sortings || [],
                selector = ".page .main"

            if (ajaxSettings.urlList != undefined) {
                oTable = $(selector + " .gl-grid table").first().dataTable({
                    bProcessing: true,
                    bServerSide: true,
                    sServerMethod: "POST",
                    sPaginationType: "full_numbers",
                    sAjaxSource: ajaxSettings.urlList,
                    aaSorting: sortings,
                    aoColumns: columns,
                    fnPreDrawCallback: function () {
                        //$(".page .ajax-loader").fadeIn();
                    },
                    fnDrawCallback: function (e) {
                        $(selector + " table tbody tr").click(function (e) {
                            var self = $(this);
                            if (!self.hasClass("row_selected")) {
                                self.parent().children().removeClass("row_selected");
                                self.addClass("row_selected");
                                if (!rowSettings.isSelected) {
                                    for (var i = 0; i < rowSettings.evtselected.length; i++) {
                                        rowSettings.evtselected[i]({ selected: true });
                                    }
                                    rowSettings.isSelected = true;
                                }
                            }
                        });

                        $(selector + " table tbody tr").dblclick(function (e) {
                            $(".page .toolbar > #btnEdit").click();
                        });

                        if (e._iRecordsTotal <= e._iDisplayLength) {
                            $(".dataTables_paginate").hide();
                        }
                        else {
                            $(".dataTables_paginate").show();
                        }
                    },
                    aLengthMenu: [[5, 10, 15, 25, 50, 100], [5, 10, 15, 25, 50, 100]],
                    iDisplayLength: 10,
                }).fnSetFilteringDelay();
            }

            if (xtype === "grid-form" || xtype === "grid-panels") {
                $(selector + " .gl-form").hide();
            }
        }
    }

    this.showToolbars = function (items) {
        $(".page .toolbar > button").hide();
        $.each(items, function (idv, val) {
            $(".page .toolbar > button#" + val).show();
            $(".page .toolbar > [data-action=" + val + "]").show();
        });
    }

    this.selectedRow = function () {
        var selectedRow = $('tr.row_selected')[0];
        if (selectedRow === undefined || oTable === undefined || oTable == null) {
            return undefined;
        }
        else {
            var oRow = oTable.$('tr.row_selected')[0];
            var aData = oTable.fnGetData(oRow);
            return aData;
        }
    }

    this.refreshGrid = function () {
        if (oTable !== undefined && oTable !== null) {
            oTable.fnClearTable();
        }
    }

    this.selectedRow = function (name, callback) {
        if (arguments.length == 0) {
            // for support dataTable
            var selectedRow = $('tr.row_selected')[0];
            if (selectedRow === undefined || oTable === undefined || oTable == null) {
                return undefined;
            }
            else {
                var oRow = oTable.$('tr.row_selected')[0];
                var aData = oTable.fnGetData(oRow);
                return aData;
            }
        }
        else {
            // next will use this
            var row = $("#" + name + " tbody tr.k-state-selected")[0];
            var grid = $("#" + name).data("kendoGrid");
            var data = grid.dataItem(row);

            return callback((data == null) ? undefined : data);
        }

    }

    this.call = function (action, params) {
        if (options[action]) options[action](params);
    }

    this.lookup = {
        isConfugired: false,
        isShown: false,
        title: "",
        oTable: undefined,
        name: "lookup",
        settings: { source: "", columns: [] },
        events: {
            dblclick: []
        },
        onDblClick: function (callback) {
            this.events.dblclick.push(callback);
        },
        init: function (options) {
            this.settings.source = (options.source || "");
            this.settings.columns = (options.columns || []);
            this.settings.sortings = (options.sortings || []);
            this.title = options.title || "Search List";
            this.isConfugired = false;
            this.settings.additionalParams = options.additionalParams || [];
            if (options.name !== undefined) {
                this.name = options.name;
            }
        },
        show: function () {
            var _lookup = this;
            var colhtml = "";
            $.each(this.settings.columns, function (idx, val) {
                colhtml += "<td></td>";
            });
            var template = "<div class=\"lookup-panel\"><div class=\"lookup\">" +
                           "<div class=\"title\">" + _lookup.title + "</div>" +
                           "<div class=\"buttons\">" +
                           "<button class=\"button small\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                           "</div>" +
                           "<table id=\"tblGrid\">" +
                           "<thead><tr>" + colhtml + "</tr></thead>" +
                           "</table>" +
                           "</div>" +
                           "<div class=\"buttons\">" +
                           "<button class=\"button small\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                           "<button class=\"button small\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                           "</div>" +
                           "</div>";

            if (!_lookup.isConfugired) {

                $(".body > .panel").empty();
                $(".body > .panel").html(template);
                $(".body > .panel #btnClosePanel, .body > .panel #btnCancelPanel").on("click", function () {
                    _lookup.hide();
                });

                _lookup.oTable = $("#tblGrid").dataTable({
                    bProcessing: true,
                    bServerSide: true,
                    sServerMethod: "POST",
                    sAjaxSource: SimDms.baseUrl + _lookup.settings.source,
                    sPaginationType: "full_numbers",
                    bAutoWidth: false,
                    fnServerData: function (sSource, aoData, fnCallback, oSettings) {
                        var params = _lookup.settings.additionalParams;

                        if (_this.isNullOrEmpty(params) == false) {
                            $.each(params, function (key, val) {
                                if (_this.isNullOrEmpty(val.value) == false) {
                                    aoData.push({
                                        name: val.name,
                                        value: val.value
                                    });
                                }
                                else {
                                    aoData.push({
                                        name: val.name,
                                        value: _this.getValue({ name: val.element, type: val.type || "input" })
                                    });
                                }
                            });
                        }

                        oSettings.jqXHR = $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": fnCallback
                        });
                    },
                    aaSorting: _lookup.settings.sortings,
                    aoColumns: _lookup.settings.columns,
                    fnPreDrawCallback: function () {
                        _lookup.showAjaxLoad();
                    },
                    fnDrawCallback: function () {
                        var oTable = _lookup.oTable;
                        var events = _lookup.events;
                        var aData;

                        $("#tblGrid tbody tr").click(function (e) {
                            var self = $(this);
                            if (!self.hasClass("row_selected")) {
                                self.parent().children().removeClass("row_selected");
                                self.addClass("row_selected");
                            }

                            if (oTable !== undefined && oTable !== null) {
                                aData = oTable.fnGetData(this);
                            }
                        });
                        $("#tblGrid tbody tr").dblclick(function (e) {
                            if (oTable !== undefined && oTable !== null) {
                                aData = oTable.fnGetData(this);
                                for (var i = 0; i < _lookup.events.dblclick.length; i++) {
                                    _lookup.events.dblclick[i](this, aData, _lookup.name);
                                }
                            }
                        });
                        $(".body > .panel #btnSelectData").on("click", function (event) {
                            if (oTable !== undefined && oTable !== null && aData !== undefined) {
                                for (var i = 0; i < _lookup.events.dblclick.length; i++) {
                                    _lookup.events.dblclick[i](this, aData, _lookup.name);
                                }
                            }
                        });
                        _lookup.hideAjaxLoad();
                    },
                    aLengthMenu: [[10, 15, 25, 50, 100], [10, 15, 25, 50, 100]],
                    iDisplayLength: 15,
                    //"sScrollX": "100%",
                    //"sScrollXInner": "110%",
                    //"bScrollCollapse": true
                }).fnSetFilteringDelay();
            }

            if (_lookup.isShown) {
                $(".body > .panel").fadeOut("slow", function () {
                    _lookup.isShown = false;
                });
            }
            else {
                $(".body > .panel").fadeIn("slow", function () {
                    _lookup.isShown = true;
                });
            }

            _lookup.isConfugired = true;
        },
        hide: function () {
            var _lookup = this;
            $(".body > .panel").fadeOut("slow", function () {
                _lookup.isShown = false;
            });
        },
        showAjaxLoad: function () {
            $(".page .ajax-loader").fadeIn();
        },
        hideAjaxLoad: function () {
            $(".page .ajax-loader").fadeOut();
        }
    }

    this.populateTable = function (options, callback) {
        var selector = options.selector;
        if (options.data === undefined) {
            this.post(options.url, function (data) {
                _this.populateTableData(selector, data, options);
            });
        }
        else {
            var data = options.data;
            //_this.populateTableData(selector, options.data, options.selectable, options.multiselect);
            _this.populateTableData(selector, data, options);
        }
    }

    //this.populateTableData = function (selector, data, selectable, multiselect) {
    this.populateTableData = function (selector, data, options) {
        var html = "";
        var theads = $(selector + " thead th");
        var fields = [];
        $.each(theads, function (idx, val) {
            fields.push({ name: $(val).data("field"), cls: $(val).data("class"), type: $(val).data("type") })
        });

        $(selector + " tbody").empty();
        $.each(data, function (idx, val) {
            html = "<tr>";

            $.each(fields, function (i, field) {
                var cls = (field.cls === undefined) ? "" : " class=\"" + field.cls + "\"";
                var value = (val[field.name] || "&nbsp;");
                if (field.type === "checkbox") {
                    value = "<i class='icon icon-check link'></i>";
                }
                if (field.type === "date") {
                    value = (value !== "&nbsp;") ? moment(value).format(SimDms.dateFormat) : "&nbsp;";
                }
                if (field.type === "action") {
                    value = "<i class=\"icon icon-edit link\"></i><i class=\"icon icon-trash link\"></i>";
                }
                if (field.type === "edit") {
                    value = "<i class=\"icon icon-edit link\"></i>";
                }
                if ((field.name === "" || field.name === undefined) && field.type !== "action" && field.type !== "edit") {
                    value = "<i class='icon icon-check-empty link'></i>";
                }
                html += "<td" + cls + ">" + value + "</td>";
            });
            html += "</tr>";
            $(selector + " tbody").append(html);
        });
        $(selector + " tbody .icon").on("click", function () {
            for (var i = 0; i < tableSettings.evtclick.length; i++) {
                var columns = $(this).parent().parent().children();
                var data = [];

                if ($(this).hasClass("link")) {
                    $.each(columns, function (idx, val) { data.push(val.textContent) });
                    tableSettings.evtclick[i]($(this).context.className.split('-')[1].split(' ')[0], data);
                }
            }
        });

        $(selector + " thead tr th:nth-child(1) i").on("click", function (evt) {
            var self = $(this);
            var recordSelector = selector + " tbody tr";

            if (self.hasClass("icon-check")) {
                self.removeClass("icon-check");
                self.addClass("icon-check-empty");

                $.each($(recordSelector), function (key, val) {
                    $(this).removeClass("row_selected");
                    var iconHeader = $($(this).children()[0]).children("i");

                    iconHeader.removeClass("icon-check");
                    iconHeader.addClass("icon-check-empty");
                });
            }
            else {
                self.addClass("icon-check");
                self.removeClass("icon-check-empty");

                $.each($(recordSelector), function (key, val) {
                    $(this).addClass("row_selected");
                    var iconHeader = $($(this).children()[0]).children("i");

                    iconHeader.addClass("icon-check");
                    iconHeader.removeClass("icon-check-empty");
                });
            }
        });

        if (options.selectable) {
            if (options.multiselect) {
                $(selector + " tbody tr").click(function (e) {
                    var self = $(this);
                    var icon = $(this).children()[0];
                    icon = $(icon).children();

                    if (!self.hasClass("row_selected")) {
                        self.addClass("row_selected");
                        icon.addClass("icon-check");
                        icon.removeClass("icon-check-empty");
                    }
                    else {
                        self.removeClass("row_selected");
                        icon.removeClass("icon-check");
                        icon.addClass("icon-check-empty");
                    }
                });
            }
            else {
                $(selector + " tbody tr").click(function (e) {
                    var self = $(this);
                    var icon = $(this).children()[0];
                    icon = $(icon).children();

                    var recordSelector = selector + " tbody tr";
                    $.each($(recordSelector), function (key, val) {
                        var iconHeader = $($(this).children()[0]).children("i");

                        iconHeader.removeClass("icon-check");
                        iconHeader.addClass("icon-check-empty");
                        $(this).removeClass("row_selected");
                    });

                    if (!self.hasClass("row_selected")) {
                        self.parent().children().removeClass("row_selected");
                        self.addClass("row_selected");

                        icon.addClass("icon-check");
                        icon.removeClass("icon-check-empty");
                    }
                    else {
                        self.removeClass("row_selected");

                        icon.removeClass("icon-check");
                        icon.addClass("icon-check-empty");
                    }
                });
            }
        }
    }

    this.onTabsChanged = function (callback) { tabSettings.tabchanged.push(callback); }

    this.onTableClick = function (callback) { tableSettings.evtclick.push(callback); }

    this.onGridClick = function (callback) { _this.gridSettings.evtclick.push(callback); }

    if (options.autorender || options.onInit) {
        this.render(function () {
            if (options.onInit) {
                _this.options = options;
                options.onInit(_this);
            }
            if (options.onTbrClick) {
                $('.toolbar [data-action]').on('click', function () {
                    options.onTbrClick(_this, $(this).data('action'));
                });
            }

            $('[data-action]').on('click', function () {
                var name = $(this).data('action');
                if (options[name]) options[name]();
            })

            console.log('finish render');
        });
    }
};

jQuery.fn.dataTableExt.oApi.fnSetFilteringDelay = function (oSettings, iDelay) {
    var _that = this;

    if (iDelay === undefined) {
        iDelay = 500;
    }

    this.each(function (i) {
        $.fn.dataTableExt.iApiIndex = i;
        var
			$this = this,
			oTimerId = null,
			sPreviousSearch = null,
			anControl = $('input', _that.fnSettings().aanFeatures.f);

        anControl.unbind('keyup').bind('keyup', function () {
            var $$this = $this;

            if (sPreviousSearch === null || sPreviousSearch != anControl.val()) {
                window.clearTimeout(oTimerId);
                sPreviousSearch = anControl.val();
                oTimerId = window.setTimeout(function () {
                    $.fn.dataTableExt.iApiIndex = i;
                    _that.fnFilter(anControl.val());
                }, iDelay);
            }
        });

        return this;
    });
    return this;
};

SimDms.Widget.prototype.populateArea = function (container, data) {
    var _this = this;
    var selectorContainer = (container || "") + " ";

    $(".page .ajax-loader").fadeIn();
    $.each(data, function (key, value) {
        var $ctrl = $("[name=" + key + "]");
        var type = $ctrl.data("type");
        $ctrl.removeClass("error");

        if (type === undefined) {
            $(selectorContainer + "[name=\"" + key + "\"]").val(value);
        }
        else {
            if (type === "switch") {
                value = (value || false);
                $(selectorContainer + "#" + key + "Y").prop('checked', value).val(value);
                $(selectorContainer + "#" + key + "N").prop('checked', !value).val(value);
            }
            if (type === "datepicker" || type === "date") {
                value = (value) ? moment(value).format(SimDms.dateFormat) : undefined;
                $(selectorContainer + "[name=\"" + key + "\"]").val(value);
            }
        }
    });
    $(".page .ajax-loader").fadeOut();
}

SimDms.Widget.prototype.selectparam = function (options) {
    var _this = this;
    var selector = (options.selector || ("[name=" + options.name + "]"));

    $(selector).on("change", function () {
        var text = $(this).find("option:selected").text();
        $("[name=" + options.name + "stext]").val(text);
    });

    if (!$(selector).hasClass("select-param")) {
        $(selector).addClass("select-param");
    }

    if (options.name !== undefined) {
        $(selector).parent().append("<input type=\"text\" class=\"select-shadow\" style=\"display:none\" readonly=\"readonly\" name=\"" + options.name + "stext\"></text>");
        $(selector).addClass("select-main");
    }

    if (options.param !== undefined) {
        var selectorparam = ("[name=" + options.param + "]");
        $(selectorparam).on("blur", function () {
            if (options.name !== undefined) {
                if (!$("[name=" + options.name + "]").is(":visible")) {
                    var val = $("[name=" + options.name + "]").val();
                    _this.select({ selector: selector, url: options.url + "?id=" + $(selectorparam).val(), optionText: options.optionText }, function () {
                        $("[name=" + options.name + "]").val(val);
                        $(selector).change();
                    });
                }
                $("[name=" + options.name + "]").show();
                $("[name=" + options.name + "stext]").hide();
            }
        });
        $(selectorparam).on("change", function () {
            _this.select({ selector: selector, url: options.url + "?id=" + $(selectorparam).val(), optionText: options.optionText }, function () {
                $(selector).change();
            });
            if (options.name !== undefined) {
                $("[name=" + options.name + "]").show();
                $("[name=" + options.name + "stext]").hide();
            }
        });
    }

    if (options.params !== undefined) {
        var selectorparams = "";
        $.each(options.params, function () {
            selectorparams = (this.selector || ("#" + this.name));
        })

        $(selectorparams).on("blur", function () {
            if (options.name !== undefined) {
                $("[name=" + options.name + "]").show();
                $("[name=" + options.name + "stext]").hide();
            }
        });
        $(selectorparams).on("change", function () {
            var params = {};
            $.each(options.params, function () {
                params[this.param] = (this.value || $(this.selector || ("#" + this.name)).val());
            })
            _this.select({ selector: selector, url: options.url, params: params, optionalText: options.optionalText }, function () {
                $(selector).change();
            });

            if (options.name !== undefined) {
                $("[name=" + options.name + "]").show();
                $("[name=" + options.name + "stext]").hide();
            }
        });
    }
}

SimDms.Widget.prototype.populate = function (data, arg1, arg2)
{
    //$(".page .main label.error").hide();
    var iterator = 1;
    var _this = this;
    _this.iterator = 1;
    //var selectorContainer = ".main form ";

    var selectorContainer = "";

    if (arguments.length == 2) {
        //console.log("argument = 2");
        if (typeof arg1 !== "function") {
            if (_this.isNullOrEmpty(arg1) == false) {
                selectorContainer += arg1 + " ";
            }
        }
        else {
            arg1(data);
        }
    }
    else if (arguments.length == 3) {
        //console.log("argument = 3");
        if (_this.isNullOrEmpty(arg1) == false) {
            selectorContainer += ".main form " + arg1 + " ";
        }
        arg2(data);
    }

    //console.log(selectorContainer);
    //console.log(data);

    $(".page .ajax-loader").fadeIn();

    $.each(data, function (key, value) {
        setTimeout(function () {
            var $ctrl = $("[name=" + key + "]");
            var type = $ctrl.data("type");
            $ctrl.removeClass("error");

            //console.log(type,key,value);

            if (type === undefined) {
                $(selectorContainer + "[name=\"" + key + "\"]").val(value);
                //console.log(key,value)
            }
            else {
                if (type === "radio") {
                    value = (value || false);
                    setRadioValue(key, value);
                } else
                if (type === "switch") {
                    value = (value || false);
                    $(selectorContainer + "#" + key + "Y").prop('checked', value).val(value);
                    $(selectorContainer + "#" + key + "N").prop('checked', !value).val(value);
                } else
                if (type === "datepicker" || type === "date") {
                    value = (value) ? moment(value).format(SimDms.dateFormat) : undefined;
                    $(selectorContainer + "[name=\"" + key + "\"]").val(value);
                } else
                if (type === "decimal") {
                    value = (value) ? _this.numberFormat(value, 2) : 0.00;
                    $(selectorContainer + "[name=\"" + key + "\"]").data("kendoNumericTextBox").value(value);
                } else
                if (type === "int") {
                    value = (value) ? _this.numberFormat(value, 0) : 0;
                    $(selectorContainer + "[name=\"" + key + "\"]").data("kendoNumericTextBox").value(value);
                } else
                if (type === "intt") {
                    value = (value) ? _this.numberFormat(value, 0, ',', '') : 0;
                    //$(selectorContainer + "[name=\"" + key + "\"]").data("kendoNumericTextBox").value(value);
                    $("[name=\"" + key + "\"]").val(value);
                } else
                if (type === "select") {
                    $(selectorContainer + "[name=\"" + key + "\"]").select2('val', value);
                }

            }
        }, 50);
        //$(".page .main form").valid();
    });

    setTimeout(function () {
        $(".page .main form").valid();
    }, 2500);

    $(".page .ajax-loader").fadeOut();

    if (arg1 !== undefined && typeof arg1 === "function") {
        arg1(data);
    }
}

SimDms.Widget.prototype.post = function (url, arg1, arg2) {
    var _this = this;

    if (!(arg1 && arg1.showAjax == false)) {
        $(".page .ajax-loader").fadeIn();
    }

    if (arguments.length == 2) {
        $.post(SimDms.baseUrl + url, function (result) {
            arg1(result);
            $(".page .ajax-loader").fadeOut();
        });
    }
    if (arguments.length == 3) {
        $.post(SimDms.baseUrl + url, arg1, function (result) {
            arg2(result);
            if (_this.isNullOrEmpty(arg1.showAjax) == true || arg1.showAjax == false) {
                $(".page .ajax-loader").fadeOut();
            }
        });
    }
}

SimDms.Widget.prototype.showAjaxLoad = function () {
    $(".page .ajax-loader").fadeIn();
}

SimDms.Widget.prototype.hideAjaxLoad = function () {
    $(".page .ajax-loader").fadeOut();
}

SimDms.Widget.prototype.select = function (options, callback) {
    var _this = this;
    var params = options.params || {};

    if (options.data !== undefined) {
        $(options.selector).html("<option value='" + (options.optionValue || "") + "'>" + (options.optionText || $(options.selector).data("opttext") || SimDms.selectOneText) + "</option>");
        $.each(options.data, function (idx, val) {
            $(options.selector).append("<option value=\"" + val.value + "\">" + val.text + "</option>");
        });
        if (callback !== undefined && typeof callback === "function") callback("configured");
        return;
    }

    $.each((options.additionalParams || []), function (idx, val) {
        params[val.name] = _this.getValue({ name: val.element, type: val.type }) || val.value;
    });
    _this.post(options.url, params, function (result) {
        $(options.selector).html("<option value='" + (options.optionalValue || options.optionalValue || '') + "'>" + (options.optionalText || SimDms.selectOneText) + "</option>");
        $.each(result, function (idx, val) {
            $(options.selector).append("<option value=\"" + val.value + "\">" + val.text + "</option>");
        });

        if (options.selected !== undefined) {
            $(options.selector).val(options.selected);
        }

        if (callback !== undefined && typeof callback === "function") {
            callback(result);
        }
    });
}

SimDms.Widget.prototype.endsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

SimDms.Widget.prototype.timeDiff = function (time1, time2, midnight) {
    if (midnight) {
        if (time1 === null || time1.length !== 5) { time1 = "00:00" };
        if (time2 === null || time2.length !== 5) { time2 = "00:00" };
        var val = moment(time1, "HH:mm").diff(moment(time2, "HH:mm"), "minutes");
        return (val < 0) ? (val + 60 * 24) : val;
    }
    else {
        if (time1 === null || time1.length !== 5) { time1 = "00:00" };
        if (time2 === null || time2.length !== 5) { time2 = "00:00" };
        var val = moment(time1, "HH:mm").diff(moment(time2, "HH:mm"), "minutes");
        return (val < 0) ? 0 : val;
    }
}

SimDms.Widget.prototype.validateTime = function (time) {
    if (time === null || time.length !== 5) { time = "00:00" };
    return time;
}

SimDms.Widget.prototype.showReport = function (options) {
    options.par = options.par || [];
    var url = "Reports/Viewer.aspx?rpt=" + options.id;
    var par = "";
    for (var i = 0; i < (options.par).length; i++) {
        par += ((i === 0) ? "&par=" : ";") + options.par[i];
    }
    var type = "&type=" + options.type;

    if (options.filename === undefined) {
        options.filename = options.id;
    }
    var filename = "&filename=" + options.filename;
    console.log(options.panel);
    console.log(options.filename);
    if (options.type === "export" || options.type === "pdf") {
        console.log("Window Load");
        window.location = SimDms.baseUrl + url + par + type + filename;
    }
    else if (options.panel === undefined) {
        console.log("undefined");
        $(".frame iframe").attr("src", SimDms.baseUrl + url + par + type);
        console.log(SimDms.baseUrl + url + par + type);
    }
    else {
        $("#" + options.panel).load(SimDms.baseUrl + url + par + type);
        console.log("define");
    }
}

SimDms.Widget.prototype.ShowReportPopup = {
    isConfugired: false,
    isShown: false,
    name: "lookup",
    show: function (options) {
        var _lookup = this;

        var template = "<form class=\"report\">" +
                       "<div class=\"lookup-panel\"><div class=\"lookup\">" +
                       "<div class=\"title\">Report " + options.id + "</div>" +
                       "<div class=\"buttons\">" +
                       "<button class=\"button small\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                       "</div>" +
                       "<div class='panel inside frame'>" +
                       "<iframe class=\"framereport\"" +
                       "</iframe>" +
                       "</div>" +
                       "</div>" +
                       "</div>" +
                       "</form>";

        if (!_lookup.isConfugired) {

            $(".body > .panel").empty();
            $(".body > .panel").html(template);
            $('.panel').css({ width: 1350, 'max-width': 1400 });
            $('.lookup').css({ 'max-height': 1000, height: 650 });
            $('.inside').css({ width: 1310, 'margin-top': 30, height: '100%', border: 'none' });
            $('.framereport').css({ width: '100%', height: '100%', 'margin-top': -20, 'margin-left': -20, bottom: 0 });
            $(".body > .panel #btnClosePanel, .body > .panel #btnCancelPanel").on("click", function () {
                _lookup.hide();
            });
        }

        if (_lookup.isShown) {
            $(".body > .panel").fadeOut("slow", function () {
                _lookup.isShown = false;
            });
        }
        else {
            $(".body > .panel").fadeIn("slow", function () {
                _lookup.isShown = true;
                options.par = options.par || [];
                var url = "Reports/Viewer.aspx?rpt=" + options.id;
                var par = "";
                for (var i = 0; i < (options.par).length; i++) {
                    par += ((i === 0) ? "&par=" : ";") + options.par[i];
                }
                var type = "&type=" + options.type;
                $(".frame iframe").attr("src", SimDms.baseUrl + url + par + type);
            });
        }

        _lookup.isConfugired = true;
    },
    hide: function () {
        var _lookup = this;
        $(".body > .panel").fadeOut("slow", function () {
            _lookup.isShown = false;
        });
    }
}

SimDms.Info = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.info(msg);
    }
};

SimDms.Widget.prototype.XlsxReport = function (o) {
    switch (o.type) {
        case 'xlsx':
            reportXls(o);
            break;
        case 'preview':
            reportXls(o);
            break;
        default:
            break;
    }

    function reportXls(o) {
        $('.page > .ajax-loader').show();
        $.post(o.url, o.params, function (r) {

            if (r.rows) {
                if (r.rows < 60000 && r.fileUrl) {
                    $('.page > .ajax-loader').hide();
                    window.location = r.fileUrl;

                    SimDms.Info('data sebanyak ' + r.rows + ' record sedang digenerate');
                }
                else {
                    $('.page > .ajax-loader').hide();

                    SimDms.Info({ type: 'error', text: 'tidak bisa mengenerate data sebanyak ' + r.rows + ' record' });
                }
            }
        });
    }
}

SimDms.Widget.prototype.showNotification = function (msg, response) {
    if (msg) {
        if (sdms.info) {
            sdms.info(msg);
        }
        else {
            $(".page > .notif-wrapper > .notification").text(msg);
            $(".page > .notif-wrapper").fadeIn();
            setTimeout(function () { $(".page > .notif-wrapper").fadeOut(); }, 3000);
        }

        if (typeof response != 'undefined') {
            setTimeout(function () { response(); }, 1000);
        }
    }
}

SimDms.Widget.prototype.exportXls = function (options) {
    if ((options.type || "") == "kgrid") {
        this.exportXlsKgrid(options);
        return;
    }

    var _this = this;
    var items = options.items || [];

    _this.post(options.source, options.params || {}, function (data) {
        if (data == undefined) return;

        if ((typeof data) == "string") {
            data = $.parseJSON(data);
        }

        //console.log(data.length);

        //$.each(data || [], function (key, val) {
        //    console.log(val);
        //});

        var html = _this.toHtmlTable(data, items);
        var form = document.createElement("form");
        var generatedDate = new Date();
        //generatedDate = moment(generatedDate.toString(), "hh-mm-ss");
        _this.submitXls((options.fileName || options.name) + "_", html);
    });
}

SimDms.Widget.prototype.exportXlsKgrid = function (options) {
    var _this = this;

    var ds = $("#" + options.name).data("kendoGrid").dataSource;
    var fltDs = new kendo.data.DataSource({
        data: ds.data(),
        filter: ds.filter()
    });

    var data = ds.data();
    var html = this.toHtmlTable(data, options.items);
    _this.submitXls((options.fileName || options.name) + "_", html);
}

SimDms.Widget.prototype.toHtmlTable = function (data, items, header) {
    if (data == undefined) return;

    var html = "";
    var items = items || [];

    if (items.length == 0 && data.length > 0) {
        $.each(data[0], function (key, val) {
            items.push({ name: key });
        });
    }

    if (header !== undefined) {
        html += "<tr>" + header + "</tr>";
    }

    if (items.length > 0) {
        html += "<tr>";
        $.each(items, function (idx, val) {
            var attr = (val.width == undefined) ? "" : " style='width:" + val.width + "px'";
            var text = (val.text || val.title || val.name);
            html += "<th" + attr + ">" + text + "</th>";
        });
        html += "</tr>";
    }

    $.each(data, function (idx, val) {
        html += "<tr>";

        $.each(items, function () {
            var text = (val[this.name || this.field] || "");
            if (this.type == "date" && text != null && text.length > 0) {
                text = moment(text).format('MM/DD/YYYY');
            }
            if (this.type == "datetime" && text != null && text.length > 0) {
                text = moment(text).format('MM/DD/YYYY HH:mm:ss');
            }
            if (this.type == "text" && text != null && text.length > 0) {
                text = "=\"" + text + "\"";
            }

            html += "<td>" + text + "</td>";
        });
        html += "</tr> \n";
        return;
    });

    html = "<table>\n" + html + "</table>";

    return html;
}

SimDms.Widget.prototype.submitXls = function (name, html) {
    var form = document.createElement("form");
    form.setAttribute("method", "post");
    form.setAttribute("action", SimDms.baseUrl + SimDms.exportXlsUrl);

    var field1 = document.createElement("input");
    field1.setAttribute("type", "hidden");
    field1.setAttribute("name", "name");
    field1.setAttribute("value", name);

    var field2 = document.createElement("input");
    field2.setAttribute("type", "hidden");
    field2.setAttribute("name", "html");
    field2.setAttribute("value", this.htmlEscape(html));

    form.appendChild(field1);
    form.appendChild(field2);
    document.body.appendChild(form);
    form.submit();
}

SimDms.Widget.prototype.htmlEscape = function (str) {
    var result = String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        //.replace(/'/g, '&#39;');
        .replace(/'/g, ' ');
    //console.log(str, result);

    return result;
};

SimDms.Widget.prototype.showPanel = function (options) {
    if (typeof options == "string") {
        $("#" + options).slideDown();
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                //$("#" + val).fadeIn();
                var obj = $("#" + val);

                var isKGrid = obj.parent().parent().hasClass("kgrid");
                var isK_Grid = obj.parent().parent().hasClass("k-grid");

                if (isK_Grid || isKGrid) {
                    obj.parent().parent().fadeIn();
                }
                else {
                    $("#" + val).fadeIn();
                }
            }
            else {
                var obj = $("#" + val.name);

                if (val.type == "kgrid") {
                    //$("#" + val.name).parent().parent().fadeIn();
                    $("#" + val.name).fadeIn();
                }
            }
        });
    }
}

SimDms.Widget.prototype.hidePanel = function (options) {
    if (typeof options == "string") {
        $("#" + options).slideUp();
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                //$("#" + val).fadeOut();
                var obj = $("#" + val);
                var isKGrid = obj.parent().parent().hasClass("kgrid");
                var isK_Grid = obj.parent().parent().hasClass("k-grid");

                if (isK_Grid || isKGrid) {
                    obj.parent().parent().fadeOut();
                }
                else {
                    $("#" + val).fadeOut();
                }
            }
            else {
                if (val.type == "kgrid" || val.type == "k-grid") {
                    $("#" + val.name).parent().parent().fadeOut();
                    //$("#" + val.name).fadeOut();
                }
            }
        });
    }
}

SimDms.Widget.prototype.hideAccordion = function () {
    $(".panel > .subtitle > .fa.fa-minus").hide();
}

SimDms.Widget.prototype.numberFormat = function (number, decimals, dec_point, thousands_sep, type) {
    return number_format(number, decimals, dec_point, thousands_sep, type);
}

SimDms.Widget.prototype.xpost = function (url, arg1, arg2) {
    if (arguments.length == 2) { $.post(SimDms.baseUrl + url, function (result) { arg1(result) }) }
    if (arguments.length == 3) { $.post(SimDms.baseUrl + url, arg1, function (result) { arg2(result) }) }
}

SimDms.Widget.prototype.serializeObject = function (name) {
    if (name) {
        return $('#' + name).serializeObject();
    }
    else {
        return $('.body .gl-widget').serializeObject();
    }
}

SimDms.Widget.prototype.switchSlide = function (options) {
    if (options && options.slide1) {
        if (options.slide2 && options.slide1 !== options.slide2) {
            var slide1 = $(options.slide1);
            var slide2 = $(options.slide2);

            slide1.addClass('inactive');
            slide2.addClass('active');
            setTimeout(function () { slide1.removeClass('active').removeClass('inactive') }, (options.timeout || 2000));
        }
        else {
            $(options.slide1).addClass('active');
        }
    }
}

SimDms.Widget.prototype.onTbrClick = function (name, callback) {
    $('.toolbar #' + name).off('click');
    $('.toolbar #' + name).on('click', callback);
}

SimDms.Widget.prototype.setInterval = function (interval, callback) {
    if (SimDms.activeTimer) clearInterval(SimDms.activeTimer);
    SimDms.activeTimer = setInterval(callback, interval);
}

SimDms.Widget.prototype.requestFullscreen = function () {
    var element = document.documentElement;
    if (element.requestFullscreen) {
        element.requestFullscreen();
    }
    else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    }
    else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen();
    }
    else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
    $('body').addClass('full-window');
}

SimDms.Widget.prototype.exitFullscreen = function () {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    }
    else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
    }
    else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
    }
    else if (document.msExitFullscreen) {
        element.msExitFullscreen();
    }
    $('body').removeClass('full-window');
}

SimDms.Widget.prototype.requestFullWindow = function () {
    $('body').addClass('full-window');
}

SimDms.Widget.prototype.exitFullWindow = function () {
    $('body').removeClass('full-window');
}

SimDms.Widget.prototype.bind = function (options) {
    var self = this;
    var selector = (options.selector || ('[name=' + options.name + ']'));
    var type = $(selector).data('type');

    if (type == 'select') {
        self.bindSelect(options);
    }
}

SimDms.Widget.prototype.bindSelect = function (options) {
    if (options.parent) {
        $('[name=' + options.parent + ']').on('change', function () {
            var list = [];
            var group = $(this).val();
            if (options.defaultAll) {
                if (group) {
                    list = Enumerable.From(options.data).Where(function (x) { return x.group == group }).ToArray();
                }
                else {
                    list = Enumerable.From(options.data).ToArray();
                }
            }
            else {
                list = Enumerable.From(options.data).Where(function (x) { return x.group == group }).ToArray();
            }

            populate(list);
        });

        if (options.defaultAll) {
            populate(Enumerable.From(options.data).ToArray());
        }
    }
    else if (options.parents && options.parents.length > 1) {
        var parents = options.parents;

        for (var i = 0; i < parents.length; i++) {
            var list = [];
            var parent = parents[i];
            if (i < parents.length - 1) {
                $('[name=' + parent + ']').on('change', function () {
                    setTimeout(function () {
                        var group = $('[name=' + parents[parents.length - 1] + ']').val();
                        list = Enumerable.From(options.data).Where(function (x) { return x.group == group }).ToArray();
                        populate(list);
                    }, 200);
                });
            }
            else {
                $('[name=' + parent + ']').on('change', function () {
                    var group = $(this).val();
                    list = Enumerable.From(options.data).Where(function (x) { return x.group == group }).ToArray();
                    populate(list);
                });
            }
        }
    }
    else {
        populate(options.data);
    }

    function populate(rows) {
        if (rows) {
            var data = rows;
            var selector = (options.selector || ('[name=' + options.name + ']'));
            var combobox = d3.select(selector).html('');

            var mapkey = {};
            if (options.name == 'Dealer') {
                data = [];
                (rows || []).forEach(function (row) {
                    if (!mapkey[row.value]) {
                        mapkey[row.value] = row.text;
                        data.push({ value: row.value, text: row.text });
                    }
                });
            }

            if (data.length == 0) {
                data.unshift({ value: '', text: options.text || '--' });
            }
            else if (data.length > 1) {
                data.unshift({ value: '', text: options.text || '-- SELECT ONE --' });
            }
            combobox.selectAll('option').data(data).enter().append('option')
                .attr({ value: function (row) { return row.value }, rel: function (row) { return (typeof row.rel != 'undefined') ? row.rel : '' } })
                .text(function (row) { return row.text });

            if (options.onChange) {
                $(selector).off('change');
                $(selector).on('change', options.onChange);
            }

            if (typeof options.onTriggerAfter != 'undefined') {
                options.onTriggerAfter();
            }

            if (typeof options.initialValue != 'undefined' && options.initialValue != null)
                $('[name=' + options.name + ']').val(options.initialValue).change();
        }
    }
}

function number_format(number, decimals, dec_point, thousands_sep, type) {
    // http://kevin.vanzonneveld.net
    // +   original by: Jonas Raoni Soares Silva (http://www.jsfromhell.com)
    // +   improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // +     bugfix by: Michael White (http://getsprink.com)
    // +     bugfix by: Benjamin Lupton
    // +     bugfix by: Allan Jensen (http://www.winternet.no)
    // +    revised by: Jonas Raoni Soares Silva (http://www.jsfromhell.com)
    // +     bugfix by: Howard Yeend
    // +    revised by: Luke Smith (http://lucassmith.name)
    // +     bugfix by: Diogo Resende
    // +     bugfix by: Rival
    // +      input by: Kheang Hok Chin (http://www.distantia.ca/)
    // +   improved by: davook
    // +   improved by: Brett Zamir (http://brett-zamir.me)
    // +      input by: Jay Klehr
    // +   improved by: Brett Zamir (http://brett-zamir.me)
    // +      input by: Amir Habibi (http://www.residence-mixte.com/)
    // +     bugfix by: Brett Zamir (http://brett-zamir.me)
    // +   improved by: Theriault
    // +      input by: Amirouche
    // +   improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // *     example 1: number_format(1234.56);
    // *     returns 1: '1,235'
    // *     example 2: number_format(1234.56, 2, ',', ' ');
    // *     returns 2: '1 234,56'
    // *     example 3: number_format(1234.5678, 2, '.', '');
    // *     returns 3: '1234.57'
    // *     example 4: number_format(67, 2, ',', '.');
    // *     returns 4: '67,00'
    // *     example 5: number_format(1000);
    // *     returns 5: '1,000'
    // *     example 6: number_format(67.311, 2);
    // *     returns 6: '67.31'
    // *     example 7: number_format(1000.55, 1);
    // *     returns 7: '1,000.6'
    // *     example 8: number_format(67000, 5, ',', '.');
    // *     returns 8: '67.000,00000'
    // *     example 9: number_format(0.9, 0);
    // *     returns 9: '1'
    // *    example 10: number_format('1.20', 2);
    // *    returns 10: '1.20'
    // *    example 11: number_format('1.20', 4);
    // *    returns 11: '1.2000'
    // *    example 12: number_format('1.2000', 3);
    // *    returns 12: '1.200'
    // *    example 13: number_format('1 000,50', 2, '.', ' ');
    // *    returns 13: '100 050.00'
    // Strip all characters but numerical ones.
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
      prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
      sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
      dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
      s = '',
      toFixedFix = function (n, prec) {
          var k = Math.pow(10, prec);
          return '' + Math.round(n * k) / k;
      };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}


// Copy from SIMDMS Dealer

SimDms.Widget.prototype.clear = function (panel) {
    var selector = ".main" + (panel == undefined ? "" : " #" + panel);
    $(selector + " select," + selector + " input:not([type=radio])," + selector + " textarea").val("");
    $(selector + " input[data-type='switch']").attr("value", "false");
    $(selector + " input[data-type='switch']").prop("checked", true).val(true);
    $(selector + " input[data-type='radio']").resetState();
    $(selector + " .panel.kgrid.k-panel > div > div.k-grid").empty().removeClass("k-widget");
    var editor = $(selector + " [data-type=krichtext]").data("kendoEditor");
    if (editor !== undefined) {
        editor.value("");
    }
    var me = this;
    setTimeout(function () {
        me.populate(this.default || {});
    },2000)
}

SimDms.Widget.prototype.enable = function (options, callback) {
    $.each(options.items, function (idx, val) {
        var ctrl = $("[name=" + val + "]");
        var type = ctrl.data("type");
        if (ctrl.is("select")) {
            ctrl.attr("disabled", !options.value);
        }
        else if (ctrl.is("input[type='text']")) {
            ctrl.prop("disabled", !options.value);
            //ctrl.prop("readonly", !options.value);
        }
        else if (ctrl.is("button")) {
            ctrl.prop("disabled", !options.value);
        }
        else {
            switch (type) {
                case "kdatepicker":
                    var kdtp = ctrl.data("kendoDatePicker");
                    kdtp.enable(options.value);
                    break;
                default:
                    break;
            }
        }
    });
}

SimDms.Widget.prototype.hideControl = function (options) {
    if (typeof options == "string") {
        $("#" + options).parent().parent().parent().parent().slideUp();
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                $("#" + val).parent().parent().parent().parent().slideUp();
            }
        });
    }
}

SimDms.Widget.prototype.showControl = function (options) {
    if (typeof options == "string") {
        $("#" + options).parent().parent().parent().parent().slideDown();
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                $("#" + val).parent().parent().parent().parent().slideDown();
            }
        });
    }
}


// END OF Copy from SIMDMS Dealer


//Created by Mulia
SimDms.Widget.prototype.checked = function (options, callback) { //trigger when control checked
    var ctrl = $("input[name=" + options + "]");
    var type = ctrl.data("type");

    if (ctrl.is("input[type='checkbox']")) {
        $('input[name=' + options + ']').on('change', function () {
            callback(ctrl.is(":checked"));
        });
    }
}

SimDms.Widget.prototype.setValue = function (options, callback) { //set value of control
    var ctrl = $("[name=" + options.name + "]");
    var type = ctrl.data("type");
    var realVal = options.value;

    if (ctrl.is("select")) {
        ctrl.prop("selectedIndex", options.value);
    }
    else if (ctrl.is("input[type='text']")) {
        if (options.isControl) {
            realVal = $("select[name=" + options.controlName + "] option:selected").attr('rel');
        }
        ctrl.val(realVal);
    }
    else {
        switch (type) {
            default:
                break;
        }
    }

    if (typeof callback != 'undefined')
        callback();
}

//End of created by Mulia