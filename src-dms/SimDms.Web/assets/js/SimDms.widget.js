toastr.options.timeOut = 5000;
toastr.options.positionClass = 'toast-bottom-right';

SimDms.Success = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.success(msg);
    }
};

SimDms.Info = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.info(msg);
    }
};

SimDms.Warning = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-full-width';
        toastr.warning(msg);
    }
};

SimDms.Error = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-full-width';
        toastr.error(msg);
    }
};

SimDms.Widget = function (options) {
    "use strict";
    var IsFormLoad = false;
    var oTable = null;
    var _this = this;
    var _maps = {};
    var _validasi = null;
    var selector = SimDms.selector + " .main";
    var ajaxSettings = { ajaxLoader: $(".ajax-loader"), urlList: undefined, urlSave: undefined, urlDelete: undefined };
    var rowSettings = { evtchanged: [], evtselected: [], isSelected: false };
    var tabSettings = { tabchanged: [] };
    var tableSettings = { evtclick: [], rowclick: [] };
    var ajaxRequestCounter = 0;

    this.serviceName = function () {
        return svcName;
    };

    this.kgrids = [];

    this.ReformatNumber = function () {
        $("input[type=text].number").each(function (i, v) {
            var val0 = $(this).val();
            var valt = number_format(val0, 2);
            $(this).val(valt);
        });

        $("input[type=text].number-int").each(function (i, v) {
            var val0 = $(this).val();
            var valt = number_format(val0, 0);
            $(this).val(valt);
        });

    };

    this.render = function (callback) {
        var xtype = options.xtype || "form";
        var content = $(selector);
        var html = "";
        var _this = this;
        SimDms.activeObject = this;

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
                html = "<form>" + this.generatePanels(options.panels) + "<div id=\"ext-target\" class=\"panel\" ></div></form>";
                break;
            case "grid-form":
                html = this.generateGrid(options) + this.generateForm(options.items);
                break;
            case "panel-form":
                html = "<form  id=\"gl-panel-form\" class=\"gl-panel\">" + this.generatePanels(options.panels) + "</form>" + this.generateForm(options.items);
                break;
            case "grid-panels":
                html = this.generateGrid(options) + "<form class=\"gl-form\">" + this.generatePanels(options.panels) + "</form>";
                break;
            case "iframe":
                html = this.generateIframe(options);
                break;
            default:
                break;
        }

        var html_title = ((options.title === undefined) ? "" : "<h3>" + options.title + "</h3>") +
                         ((options.subtitle === undefined) ? "" : "<h5>" + options.subtitle + "</h5>");

        $(".page .title h3").html(options.title);
        $(".page .toolbar").html(this.generateButtons(options.toolbars));

        $(".body > .panel").fadeOut();

        var html_widget = "<div class=\"gl-widget\">" + html + "</div>";
        content.html(html_widget);

        // render generic behaviour
        if (!Modernizr.inputtypes.date) {
            $("input[type='date']").addClass("datepicker");
        }

        $(".spinner").spinner();
        $(".buttonset").buttonset();
        //$(".select2").select2();
        $(".datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
            dateFormat: "dd-M-yy",
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
        });

        // begin kendo controls
        $("[data-type=kdatepicker]").kendoDatePicker({ format: "dd-MMM-yyyy" });
        $("[data-type=kmonthpicker]").kendoDatePicker({ start: "year", depth: "year", format: "MMMM yyyy" });
        $("[data-type=ktimepicker]").kendoTimePicker({ format: "HH:mm" });
        $("[data-type=kdatetimepicker]").kendoDateTimePicker({ format: "dd-MMM-yyyy HH:mm" });
        $("[data-type=decimal]").kendoNumericTextBox({ format: "n2", step: 0.01 });
        $("[data-type=int]").kendoNumericTextBox(
            {
                format: "#,#",
                step: 1,
                decimals: 0
            }
        );
        $("[data-type=krichtext]").kendoEditor({ encoded: true });
        //$("[data-type=maskedtextbox]").kendoMaskedTextBox({ mask: "000-000-0000" });
        // end kendo controls

        //$(".timepicker").timepicker();
        $(".datetimepicker").datetimepicker();
        $(".switch").on("click", function () {
            var name = $(this).children()[0].name;
            var value = $("#" + name + "Y").is(':checked');
            $("input[name='" + name + "']").val(value);
            var item = _maps[name];
            if (item !== undefined && item.onChanged !== undefined) {
                item.onChanged(this, value);
            }
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
            $(".page .main form").trigger("TabChanged", [name]);
        });

        $(".panel > .subtitle > .icon.icon-minus").on("click", function () {
            if ($(this).parent().hasClass("collapse")) {
                $(this).parent().removeClass("collapse");
                $(this).parent().parent().children().slideDown();
                $(this).removeClass("icon-plus").addClass("icon-minus");
            }
            else {
                $(this).parent().addClass("collapse");
                $(this).parent().parent().children().not(".subtitle").slideUp();
                $(this).removeClass("icon-minus").addClass("icon-plus");
            }
        });

        $("input[type=text], textarea")
            .not("[data-type=date],[data-type=datepicker],[data-type=kdatepicker]," +
                 "[data-type=kdatetimepicker],[data-type=kmonthpicker]," +
                 "[readonly=readonly]")
            .on("blur", function () {
                var attr = $(this).attr("class");
                var ignoreCaseStatus = -1;

                if (_this.isNullOrEmpty(attr) === false) {
                    ignoreCaseStatus = attr.search("ignore-uppercase");
                }

                if (ignoreCaseStatus < 0) {
                    var val = $(this).val().toUpperCase();
                    $(this).val(val);
                }
            });

        $("input[type=text].number").on("blur", function () {
            var val0 = $(this).val();
            var valt = number_format(val0, 2);
            $(this).val(valt);
        });

        $("input[type=text].number").on("keydown", function (event) {
            return isNumber(event);
        });

        $("input[type=text].number").on("focus", function () {
            var val0 = $(this).val();
            var valt = val0.replace(/,/g, "");
            $(this).val(valt);
        });

        $("input[type=text].number-int").on("keydown", function (event) {
            return isNumber(event);
        });

        $("input[type=text].number-int").on("blur", function () {
            var val0 = $(this).val();
            var valt = number_format(val0, 0);
            $(this).val(valt);
        });

        $("input[type=text].number-int").on("focus", function () {
            var val0 = $(this).val();
            var valt = val0.replace(/,/g, "");
            $(this).val(valt);
        });

        $("input[type=text]").keydown(function (evt) {
            if (evt.keyCode == 27) {

            }
        });

        $("input[type=text].number-only").on("keydown", function (e) {
            var code = e.keyCode;
            var trueCode = "8|9|13|27|37|39|46|48|49|50|51|52|53|54|55|56|57|96|97|98|99|100|101|102|103|104|105|";
            if (trueCode.indexOf(code + "|") > -1) {
                return true;
            }
            else {
                return false;
            }
        });

        $("input[type=text].number-only").on("blur", function () {
            var val = $(this).val();
            if (val != "") {
                val = val * 1
                $(this).val(val);
            }
        });

        $("input[type=text].first-zero-number-only").on("keydown", function (e) {
            var code = e.keyCode;
            var trueCode = "8|9|13|27|37|39|46|48|49|50|51|52|53|54|55|56|57|96|97|98|99|100|101|102|103|104|105|";
            if (trueCode.indexOf(code + "|") > -1) {
                return true;
            }
            else {
                return false;
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

        window.onbeforeunload = function () { };

        // setting ajax url
        if (options.urlList !== undefined) {
            ajaxSettings.urlList = SimDms.baseUrl + options.urlList;
        }

        if (options.urlSave !== undefined) {
            ajaxSettings.urlSave = SimDms.baseUrl + options.urlSave;
        }

        if (options.urlDelete !== undefined) {
            ajaxSettings.urlDelete = SimDms.baseUrl + options.urlDelete;
        }

        this.showGrid(xtype, options);

        if (callback !== undefined) {
            callback("configured");
        }

        this.overrideElementSettings();
        this.initializeEvent();
        this.initializeUpload();
        this.initializeSelect();
        this.initializePopup();
        this.initializePopupContainer();
        this.initializeInnerGrid();
        this.initializeKGrid();
        this.initializeImagesInput();

        /* 
			realtime validation check by validetta.js - Validetta - Client-side form validation jQuery plugin 
			OK: 2014-03-12 15:27:22

        setTimeout (function()
        {
            _validasi = $(".page .main form").validetta({
                realTime : true,
                display : 'inline',
                errorClass    : 'validetta-inline',
                errorClose : false,
                onCompleteFunc : function(object,event){
                    event.preventDefault(); 
                    $(".page .main form").trigger("ValidationOK",  [$(".main .gl-widget").serializeObject()] );
                }
            });            

        },7777);
        // 2014-05-22 => replace with verify.js
        */
        var IsNeedExtForm = options.loadform || false;
        if (IsNeedExtForm) {
            this.loadForm();
        }

    };

    this.getAjaxRequestCounter = function () {
        return ajaxRequestCounter;
    }

    this.incrementAjaxRequestCounter = function () {
        ajaxRequestCounter++;
    }

    this.decrementAjaxRequestCounter = function () {
        ajaxRequestCounter--;
    }

    this.generateForm = function (items, prefix) {
        var html = "";
        var self = this;

        $.each(items || [], function (idx, item) {
            var cls = ((item.cls === undefined) ? "" : " class='" + item.cls + "'");
            html += "<div" + cls + ">" + self.generateLabel(item) + "<div>" + self.generateInput(item) + "</div></div>";
        });

        return "<form class=\"gl-form\" >" + (prefix || "") + html + "</form>";
    };

    this.generatePanel = function (options, prefix) {
        var items = options.items;
        var html = "";
        var self = this;

        $.each(items || [], function (idx, item) {
            var cls = ((item.cls === undefined) ? "" : " class='" + item.cls + "'");
            var wxCls = ((item.cls === undefined) ? "class='span8'" : " class='" + item.cls + "'");
            var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");
            var xHeight = ((item.height === undefined) ? "" : "height:" + item.height + "px;");
            switch (item.type || "") {
                case "divider":
                    html += self.generateInput(item);
                    break;
                case "freespace":
                    html += "<div id=\"" + item.name + "\" class=\"panel\" " + m5 + "></div>";
                    break;
                case "wxdiv":
                    html += "<div " + wxCls + m5 + " style=\"display:cell;margin:20px 0px 15px 0px;\"><div width=\"100%\" style=\"display:cell;padding-top:10px;padding-bottom:15px;padding-left:2px;\">";
                    html += "<div id=\"" + item.name + "\"  style=\"display:cell;padding-top:10px;padding-bottom:15px;padding-left:2px;padding-right:10px;" + xHeight + "  \"></div></div></div>";
                    break;
                case "wxtable":
                    html += self.generateWxTable(item, "");
                    break;
                case "tabs":
                    html += self.generateTabs(item, "");
                    break;
                case "label":
                    var style = item.style != undefined ? item.style : "";

                    html += "<div " + " style='line-height: 33px; " + style + "\' " + cls + " id='" + item.name + "' name='" + item.name + m5 + "' >" + item.text + "</div>";
                    break;
                default:
                    html += "<div " + cls + m5 + ">" + self.generateLabel(item) + "<div>" + self.generateInput(item) + "</div></div>";
                    break;
            }
        });

        var m5 = ((options.show === undefined) ? "" : "  ng-show=\"" + options.show + "\" ");
        var m8 = ((options.disable === undefined) ? "" : "  ng-disabled=\"" + options.disable + "\" ");
        var ngOption = m5 + m8;

        var name = ((options.name === undefined) ? "" : (" id=\"" + options.name + "\""));
        var cls = ((options.cls === undefined || options.cls === "") ? " class=\"panel\"" : (" class=\"panel " + options.cls + "\""));
        return "<div" + name + cls + ngOption + ">" + (prefix || "") + html + "</div>";
    };

    this.generateButtons = function (buttons) {
        var html = "";
        $.each((buttons || []), function (idx, val) {
            var ngClick = ((val.click === undefined) ? "" : " ng-click='" + val.click + "'");
            var ngShow = ((val.show === undefined) ? "" : " ng-show='" + val.show + "'");
            var ngDisable = ((val.disable === undefined) ? "" : " ng-disabled='" + val.disable + "'");
            var ng = ngClick + ngShow + ngDisable;
            var cls = "class='button small " + ((val.cls === undefined) ? "' " : val.cls + "' ");
            var style = (val.style != undefined) ? "style=\"" + val.style + "\"" : "";
            html += "<button type=\"button\" " + cls + " name='" + val.name + "' id='" + val.name + "' " + ng + style + "><i class='icon " + (val.icon || "") + "'></i>" + val.text + "</button>";
        });
        return html;
    }

    this.generateOptionButtons = function (item) {
        var html = "";
        var model = ((item.model === undefined) ? "optionButtons" : item.model);
        model = " ng-model=\"" + model + "\" ";
        $.each((item.items || []), function (idx, val) {
            var ngClick = ((val.click === undefined) ? "" : " ng-click='" + val.click + "'");
            var ngShow = ((val.show === undefined) ? "" : " ng-show='" + val.show + "'");
            var ngDisable = ((val.disable === undefined) ? "" : " ng-disabled='" + val.disable + "'");
            var ng = ngClick + ngShow + ngDisable;
            var cls = "class='btn " + (val.cls === undefined ? "btn-default' " : val.cls + "' ");
            var opt = " btn-radio=\"'" + val.name + "'\" ";
            html += "<label " + cls + model + ng + opt + ">" + val.text + "</label>";
        });
        return "<div class=\"btn-group\">" + html + "</div><div class=\"wxdiv\"></div>";
    }

    this.generateModalButtons = function (buttons) {
        var html = "";
        $.each((buttons || []), function (idx, val) {
            var ngClick = ((val.click === undefined) ? "" : ' ng-click="' + val.click + "\"");
            var ngShow = ((val.show === undefined) ? "" : ' ng-show="' + val.show + "\"");
            var ngDisable = ((val.disable === undefined) ? "" : ' ng-disabled="' + val.disable + "\"");
            var ng = ngClick + ngShow + ngDisable;
            var cls = 'class="button small' + ((val.cls === undefined) ? "\"" : ' ' + val.cls + "\"");
            var style = (val.style != undefined) ? 'style="' + val.style + "\"" : "";
            var button = '<button type="button" data-toggle="modal" id="' + val.name + '" name="' + val.name + '" data-target="#' + val.target + "\" " + cls + ng + style + '><i class="icon ' + (val.icon || "") + '"></i>' + val.text + '</button>';
            html += button +
            '<div class="modal fade" id="' + val.target + '" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                        '<div class="modal-header bootstrap-dialog-draggable btn-info">' +
                            '<div class="bootstrap-dialog-header">' +
                                '<div class="bootstrap-dialog-title"><b>' + val.modalTitle + '</b></div>' +
                                '<div class="bootstrap-dialog-close-button" style="display: none;">' +
                                    '<button class="close">�</button>' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                        '<div class="modal-body">'
                            + val.modalContent +
                        '</div>' +
                        '<div class="modal-footer">'
                                + val.modalFooter +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>';
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
        var html = "<div><div id=\"" + options.name + "\"></div></div>";
        var m5 = ((options.show === undefined) ? "" : "  ng-show=\"" + options.show + "\" ");
        return "<div class=\"panel chart\" " + m5 + ">" + (prefix || "") + html + "</div>";
    }

    this.generateIframe = function (options) {
        localStorage.setItem("LayoutUrl", options.url);
        var html = "<iframe  frameborder=\"0\" src=\""+ options.url +"\" style=\"overflow:hidden;height:100%;width:100%\" height=\"100%\" width=\"100%\"></iframe>";
        return html;
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
                html += "<button class='btn small " + (val.cls || "") + "' name='" + (val.name || "") + "' id='" + (val.name || "") + "'><i class='icon " + (val.icon || "") + "'></i>" + (val.text || "Add") + "</button>";
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

    this.generateWxTable = function (item, prefix) {

        var buttons = item.buttons || [];
        var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");

        return "<div id=\"" + item.name + "_parent\" class=\"panel dataTables_wrapper " + m5 +
            ((item.cls === undefined) ? "" : item.cls) + "\">" +
            "<div width=\"100%\" style='display:cell;padding-top:10px;padding-bottom:15px;padding-left:2px;'>" +
            "<div id=\"" + item.name + "\"  style='display:cell;padding-top:20px;padding-bottom:15px;padding-left:2px;'></div></div></div>";
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
        var _this = this;
        var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");
        if (prefix.length < 30) { prefix = "" };
        var model = ((item.model === undefined) ? "tabpage1" : item.model);
        model = " ng-model=\"" + model + "\" ";

        $.each(items, function (idx, val) {
            var cls = (val.cls || "") + ((val.active || false) ? (((val.cls == undefined) ? "" : " ") + "active") : "");
            var name = (val.name == undefined) ? "" : val.name;
            var ngClick = ((val.click === undefined) ? "" : " ng-click='" + val.click + "'");
            var ngShow = ((val.show === undefined) ? "" : " ng-show='" + val.show + "'");
            var ngDisable = ((val.disable === undefined) ? "" : " ng-disabled='" + val.disable + "'");
            var opt = " btn-radio=\"'" + val.name + "'\" ";
            var ng = model + ngClick + ngShow + ngDisable + opt;
            var attr = "class=\"" + cls + "\"  " + ng + " data-name=\"" + name + "\"";
            html += "<p " + attr + "><a href=\"javascript:void(0)\">" + val.text + "</a></p>";
        });
        tabs = "<div class=\"tabs\" " + m5 + ((item.name == undefined) ? "" : " data-id=\"" + item.name + "\"") + ">" + html + "</div>";

        if (_this.isNullOrEmpty(item.onChanged) == false) {
            tabSettings.tabchanged.push(item.onChanged);
        }
        return "<div class=\"panel tab_wrapper\">" + (prefix || "") + tabs + "</div>";
    }

    this.generatePanels = function (panels) {
        var html = "";
        var self = this;

        $.each(panels || [], function (idx, item) {
            var prefix = "";
            if (item.title !== undefined) {
                prefix += "<div class=\"subtitle\">" + item.title + "<i class='icon " + (item.icon || "icon-minus") + "'></i></div>"
            }
            else if (idx > 0) {
                if (item.showDivider == undefined || item.showDivider == true) {
                    prefix += "<div class=\"divider\"></div>";
                }
                else {
                    prefix += "<div class=\"spacer\"></div>";
                }
            }

            var xtype = item.xtype || "panel";

            switch (xtype) {
                case "table":
                    html += self.generateTable(item, prefix);
                    break;
                case "tableX1":
                    html += self.generateTableX1(item, prefix);
                    break;
                case "wxtable":
                    html += self.generateWxTable(item, prefix);
                    break;
                case "table-grid":
                    html += self.generateTableGrid(item, prefix);
                    break;
                case "tabs":
                    html += self.generateTabs(item, prefix);
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
                case "div":
                    html += "<div id=\"" + item.name + "\" class=\"wxdiv\"" + " style=\"" + item.style + "></div>";
                    break;
                default:
                    html += self.generatePanel(item, prefix);
                    break;
            }
        });

        return html;
    };

    this.generateLabel = function (item) {
        var required = item.required || false;
        if (item.validasi != undefined) {
            if (item.validasi.indexOf("required") > -1) {
                required = true;
            }
        }

        var name = ((item.name || "") === "") ? "" : " for=\"" + item.name + "\"";
        var labelWidth = ((item.labelWidth || "") === "") ? "" : " style=\"width:" + item.labelWidth + "px\"";
        var type = (((item.type || "") === "switch-full") || ((item.cls || "").indexOf("full-length") >= 0)) ? " class=\"full\"" : "";

        
        var html = "<label" + name + type + labelWidth + ">" + ((required) ? "<b><font color=\"red\">*</font></b> " : "") + (item.text || "&nbsp;") + "</label>";
        return html;
    };

    this.generateInput = function (item) {
        var name = ((item.name == undefined) ? "" : " name=\"" + item.name + "\"");
        var placeHolder = " placeHolder=\"" + ((item.placeHolder || item.text) || "") + "\"";
        var required = ((item.required || false) ? "  required=\"required\"" : "");
        var readonly = ((item.readonly || false) ? " readonly=\"readonly\"" : "");
        var validasi = ((item.validasi === undefined) ? "" : "  data-validate=\"" + item.validasi + "\" ");
        var uiEvent = ((item.event === undefined) ? "" : "  ui-event=\"" + item.event + "\" ");
        var swm = ((item.model === undefined) ? "data." + item.name : item.model);
        var m1 = ((item.model === undefined) ? "  ng-model=\"data." + item.name + "\" " : "  ng-model=\"" + item.model + "\" ");
        var m2 = ((item.keypress === undefined) ? "" : "  ui-keypress=\"" + item.keypress + "\" ");
        var m3 = ((item.keydown === undefined) ? "" : "  ui-keydown=\"" + item.keydown + "\" ");
        var m4 = ((item.keyup === undefined) ? "" : "  ui-keyup=\"" + item.keyup + "\" ");
        var m5 = ((item.show === undefined) ? "" : "  ng-show=\"" + item.show + "\" ");
        var m6 = ((item.options === undefined) ? "" : "  ng-options=\"" + item.options + "\" ");
        var m7 = ((item.value === undefined) ? "" : "  value=\"" + item.value + "\" ");
        var m8 = ((item.disable === undefined) ? "" : "  ng-disabled=\"" + item.disable + "\" ");
        var m9 = ((item.change === undefined) ? "" : "  ng-change=\"" + item.change + "\" ");
        var m10 = ((item.attr === undefined) ? "" : "  " + item.attr + " ");

        var m11 = ((item.click === undefined) ? "" : "  ng-click=\"" + item.click + "\" ");
        var m12 = ((item.disable === undefined) ? "" : "  disabled=\"" + item.disable + "\" ");

        var style = ((item.style == undefined) ? "" : "  style=\"" + item.style + "\"  ");
        var model = uiEvent + m1 + m2 + m3 + m4 + m5 + m6 + m7 + m8 + m9 + m10 + style;

        var attribut = name + placeHolder + required + readonly + validasi + style + model;
        var type = item.type || "text";
        var ngClick = ((item.click === undefined) ? "" : " ng-click='" + item.click + "'");
        var html = "";

        var minValue = ((item.min === undefined) ? "" : "  min=\"" + item.min + "\" ");
        var maxValue = ((item.max === undefined) ? "" : "  max=\"" + item.max + "\" ");
        var maxMsg = ((item.msg === undefined) ? "" : "  data-max-msg=\"" + item.msg + "\" ");
        var kendoAttr = minValue + maxValue + maxMsg;

        var wxCls = ((item.cls === undefined) ? "class='span8'" : " class='" + item.cls + "'");
        var xHeight = ((item.height === undefined) ? "" : "height:" + item.height + "px;");
        var action = item.action ? (' data-action="' + item.action + '" ') : '';
        var _any = item.any ? item.any : '';

        switch (type) {
            case "image":
                html = this.generateImage(item);
                break;
            case "grid":
                html = this.generateInnerGrid(item);
                break;
            case "text":
                var maxlength = "";
                if (item.maxlength) {
                    maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                }
                //html = "<input data-input-type='" + (item.dataInputType || "") + "' pattern='" + (item.pattern || "") + "' type='text' name='" + item.name + "' class='" + item.cls + "' id='" + item.name + "'" + placeHolder + required + readonly + maxlength + "/>";
                html = "<input data-input-type='" + (item.dataInputType || "") + "' type='text'  class='" + item.cls + "' id='" + item.name + "'" + attribut + maxlength + "/>";
                break;
            case "decimal":
                //var wrapper = " class=\"k-number-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                //html = "<div" + wrapper + "><input kendo-numeric-text-box  type=\"text\" data-type=\"decimal\" " + attribut + kendoAttr + "/></div>";
                html = "<div class='spinner-wrapper'><input type='text' class='spinner' name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + "/></div>";
                break;
            case "int":
                //var wrapper = " class=\"k-number-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                //html = "<div" + wrapper + "><input kendo-numeric-text-box type=\"text\" data-type=\"int\" " + attribut + kendoAttr + "/></div>";
                html = "<div class='spinner-wrapper'><input type='text' class='spinner' name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + "/></div>";
                break;
            case "date":
                html = "<input type='text' data-type='date' id='" + item.name + "'" + attribut + "/>";
                break;
            case "textarea":
                var maxlength = "";
                if (item.maxlength) {
                    maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                }
                html = "<textarea  id='" + item.name + "' " + attribut + maxlength + "></textarea>";
                break;
            case "krichtext":
                var wrapper = " class=\"k-richtext-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><textarea" + attribut + " data-type=\"krichtext\"></textarea></div>";
                break;
            case "datepicker":
                html = "<div class='datepicker-wrapper'><input type='text' data-type='datepicker' class='datepicker' placeholder='dd-MMM-yyyy' name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + "/></div>";
                break;
            case "kdatepicker":
                var wrapper = " class=\"k-datepicker-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input type=\"text\" data-type=\"kdatepicker\" placeholder=\"dd-MMM-yyyy\"" + name + required + readonly + validasi + style + model + "/></div>";
                break;
            case "datetimepicker":
                html = "<div class='datepicker-wrapper'><input type='text' data-type='datetimepicker' class='datetimepicker' placeholder='hh:mm' name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + "/></div>";
                break;
            case "ng-datepicker":
                var mkd = ((item.model === undefined) ? "  k-ng-model=\"data." + item.name + "\" " : "  k-ng-model=\"" + item.model + "\" ");
                var mkdc = ((item.format === undefined) ? " placeholder=\"dd-MMM-yyyy\" k-format=\"'dd-MMM-yyyy'\" " : "  placeholder=\"" + item.format + "\"    k-format=\"'" + item.format + "'\" ");
                var wrapper = " class=\"k-datepicker-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input kendo-date-picker id='" + item.name + "' " + name + required + readonly + validasi + style + model.replace(m1, mkd) + mkdc + "/></div>";
                break;
            case "ng-monthpicker":
                var mkd = ((item.model === undefined) ? "  k-ng-model=\"data." + item.name + "\" " : "  k-ng-model=\"" + item.model + "\" ");
                var mkdc = ((item.format === undefined) ? " placeholder=\"MMM-yyyy\" k-format=\"'MMM-yyyy'\" " : "  placeholder=\"" + item.format + "\"    k-format=\"'" + item.format + "'\" ");
                var wrapper = " class=\"k-datepicker-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input kendo-date-picker id='" + item.name + "' " + name + required + readonly + validasi + style + model.replace(m1, mkd) + mkdc + "/></div>";
                break;
            case "ng-datetimepicker":
                var mkdt = ((item.model === undefined) ? "  k-ng-model=\"data." + item.name + "\" " : "  k-ng-model=\"" + item.model + "\" ");
                var mkdtc = ((item.format === undefined) ? " placeholder=\"dd-MMM-yyyy HH:mm\" k-format=\"'dd-MMM-yyyy HH:mm'\" " : "  placeholder=\"" + item.format + "\"    k-format=\"'" + item.format + "'\" ");
                var wrapper = " class=\"k-datepicker-wrapper" + ((item.cls == undefined) ? "\"" : (" " + item.cls + "\""));
                html = "<div" + wrapper + "><input kendo-date-time-picker id='" + item.name + "' " + name + required + readonly + validasi + style + model.replace(m1, mkdt) + mkdtc + "/></div>";
                break;
            case "ng-maskedit":
                var mkdef = ((item.mask === undefined) ? "" : " mask=\"" + item.mask + "\" ");
                html = "<input class=\"maskedit-inputbox\" mask-edit id='" + item.name + "' " + name + required + readonly + validasi + style + model + mkdef + placeHolder + " />";
                break;
            case "timepicker":
                html = "<select  id='" + item.name + "' " + attribut + ">";
                var time = moment([1900, 0, 0]), stime = "";
                for (var i = 0; i < 48; i++) {
                    stime = time.format("HH:mm");
                    time = time.add("m", 30);
                    html += "<option value='" + stime + "'>" + stime + "</option>";
                }
                html += "</select>";
                break;
            case "spinner":
                html = "<div class='spinner-wrapper'><input type='text' class='spinner' name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + "/></div>";
                break;
            case "popup":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper' element-type='popup'><input type='text' id='" + item.name + "'" + attribut + maxlength + "/>" +
                       "<button type=\"button\"  " + m11 + " class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\"  " + model + "><i class=\"icon " + (item.icon || "icon-search") + "\"></i></button></div>";

                break;
            case "popup-lookup":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper' element-type='popup'><input type='text'  id='" + item.name + "'" + attribut + maxlength + "/>" +
                       "<button type=\"button\"  " + m11 + " class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\" " + model + "><i class=\"icon " + (item.icon || "icon-search") + "\"></i></button></div>";
                _this.setPopup(item);


                break;
            case "upload":
                var maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                html = "<div class='popup-wrapper'><input type='hidden' id='" + item.name + "' name='" + item.name + "" + "' ><input type='text' name='" + item.name + "Showed' id='" + item.name + "'Showed" + placeHolder + required + readonly + maxlength + style + "/>" +
                       "<button binding-type='button-upload'  " + m11 + m8 + " binding-on-progress='" + item.onProgress + "' binding-on-upload='" + item.onUpload + "' binding-callback='" + item.callback + "' binding-url='" + item.url + "' binding-data='" + item.name + "' type=\"button\" class=\"button\" id=\"btn" + (item.name || "") + "\" name=\"btn" + (item.name || "") + "\" ><i class=\"icon " + (item.icon || "icon-search") + "\"></i></button></div>";
                break;
            case "buttons":
                html += _this.generateButtons(item.items);
                break;
            case "optionbuttons":
                html += _this.generateOptionButtons(item);
                break;
            case "modalbuttons":
                html += _this.generateModalButtons(item.items);
                break;

            case "select":

                if (item.data === undefined) {
                    var attr = "";

                    var dataSource = item.dataSource;

                    if (dataSource !== undefined) {
                        attr += "dataSource='" + dataSource + "'";
                    }

                    attr += m8;

                    readonly = ((item.readonly || false) ? " disabled=\"disabled\"" : "");

                    html = "<select data-opttext='" + (item.opt_text || SimDms.selectOneText) + "'  id='" + item.name + "' " + attribut + " " + attr + ">" +
                           "<option value='" + (item.opt_val || "") + "'>" + (item.opt_text || SimDms.selectOneText) + "</option>" + this.generateOption(item) +
                           "</select>";
                }
                break;

            case "select2":

                var mOpt = ((item.select2 === undefined) ? " ui-select2 " : "  ui-select2=\"" + item.select2 + "\" ");
                var ngReq = ((item.required === undefined) ? "" : " ng-required=\"true\" ");

                html = "<select data-placeholder='" + (item.opt_text || SimDms.selectOneText) + "'  id='" + item.name + "' " + attribut + " " + mOpt + ngReq + ">";// +
                // "<option value='" + (item.opt_val || "") + "'>" + (item.opt_text || SimDms.selectOneText) + "</option>" + this.generateOption(item) +
                // "</select>";
                html += '<option value="">' + (item.opt_text || SimDms.selectOneText) + '</option>';
                html += '<option ng-repeat="x in ' + item.datasource + '"" value="{{x.value}}">{{x.text}}</option></select>';

                break;

            case "dropdown":
                html = "<select name='" + item.name + "' id='" + item.name + "' " + required + validasi + style + model + ">" +
                       "<option>" + (item.opt_text || SimDms.selectOneText) + "</option>" + this.generateOption(item) +
                       "</select>";
                break;
            case "label":
                html = "<label  style='line-height: 33px; " + item.style + "\'" + m5 + "'>" + item.text + "</label>";
            case "controls":
                var items = item.items || [];
                $.each(items, function (idx, val) {
                    var cls_attr = (val.cls == undefined) ? "" : " class=\"" + val.cls + "\"";
                    html += "<div" + cls_attr + ">" + _this.generateInput(val) + "</div>";
                });
                var id = (item.name == undefined) ? "" : " id=\"" + item.name + "\"";
                html = "<div" + id + " class=\"controls-wrapper\" " + style + ">" + html + "</div>";
                break;
            case "row":
                var items = item.items || [];
                $.each(items, function (idx, val) {
                    var cls_attr = (val.cls == undefined) ? "" : " class=\"" + val.cls + "\"";
                    if (val.showLabel !== undefined) {
                        html += "<div" + cls_attr + ">" + _this.generateLabel(val) + "<div>" + _this.generateInput(val) + "</div></div>";
                    }
                    else {
                        html += "<div" + cls_attr + ">" + _this.generateInput(val) + "</div>";
                    }
                });
                var id = (item.name == undefined) ? "" : " id=\"" + item.name + "\"";
                html = "<div" + id + " class=\"controls-wrapper\">" + html + "</div>";
                break;
            case "lookup":
                html = "<div class=\"lookup-wrapper\">" +
                       "<div class=\"span2\">" +
                       "<input type=\"text\" name=\"" + (item.name || "") + "\" readonly=\"readonly\" placeholder=\"" + (item.namePlaceholder || "") + "\" />" +
                       "</div>" +
                       "<div class=\"span6\">" +
                       "<input type=\"text\" name=\"" + (item.display || "") + "\" readonly=\"readonly\" placeholder=\"" + (item.displayPlaceholder || "") + "\" />" +
                       "<button type=\"button\" " + m11 + " class=\"btn\" id=\"" + (item.btnName || "") + "\"  " + model + "><i class=\"icon icon-search\"></i></button>" +
                       "</div>" +
                       "</div>";
                break;
            case "switch":
                var float = (item.float === undefined) ? "" : " " + item.float;
                html = "<div class=\"switch" + float + "\">" +
                       "<input id=\"" + item.name + "N\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\" value=\"false\" checked>" +
                       "<label for=\"" + item.name + "N\" onclick=\"\">Tidak</label>" +
                       "<input id=\"" + item.name + "Y\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\" value=\"false\" >" +
                       "<label for=\"" + item.name + "Y\" onclick=\"\">Ya</label>" +
                       "<span></span>" +
                       "</div>";
                _maps[item.name] = item;
                break;
            case "x-switch":
                html = "<toggle-switch model=\"" + swm + "\" " + model + " on-label=\"Ya\" off-label=\"Tidak\"></toggle-switch>";
                break;
            case "ng-switch":
                html = "<toggle-switch model=\"" + swm + "\" " + model + "></toggle-switch>";
                break;
            case "switch-full":
                var float = (item.float === undefined) ? "" : " " + item.float;
                html = "<div class=\"switch" + float + "\">" +
                       "<input id=\"" + item.name + "N\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\" value=\"false\" checked>" +
                       "<label for=\"" + item.name + "N\" onclick=\"\">Tidak</label>" +
                       "<input id=\"" + item.name + "Y\" name=\"" + item.name + "\" type=\"radio\" data-type=\"switch\" value=\"false\" >" +
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
            case "hidden":
                html = "<input type='hidden' name='" + item.name + "' id='" + item.name + model + "'/>";
                break;
            case "div":
                var styles = (item.style != undefined) ? item.style : "display:cell;padding-top:10px;padding-bottom:15px;padding-left:2px;";
                html = "<div id='" + item.name + "' width=\"100%\" style='" + styles + "' class='" + item.cls + "'></div>";
                break;
            case "hr":
                html = "<hr style='margin-bottom: 30px;'></hr>";
                break;
            case "check":
                html = "<input type='checkbox' name='" + item.name + "' id='" + item.name + "' class='' style='" + item.style + "' class='" + item.cls + "'/>";
                break;
            case "ng-check":
                html = "<input type='checkbox' name='" + item.name + "' id='" + item.name + "' class='' style='" + item.style + "' class='" + item.cls + "'  model=\"" + swm + "\" " + model + "/>";
                break;
            case "numeric":
                var maxlength = "";
                if (item.maxlength) {
                    maxlength = " maxlength=\"" + (item.maxlength || 50) + "\"";
                }
                //html = "<input data-input-type='" + (item.dataInputType || "") + "' pattern='" + (item.pattern || "") + "' type='text' name='" + item.name + "' class='" + item.cls + "' id='" + item.name + "'" + placeHolder + required + readonly + maxlength + "/>";
                html = "<input data-input-type='" + (item.dataInputType || "") + "' type='text'  class='number-only " + item.cls + "' id='" + item.name + "'" + attribut + maxlength + " />";
                break;
            case "wxdiv":
                html += "<div " + wxCls + m5 + " style=\"display:cell;margin:0px 0px 0px 0px;\"><div width=\"100%\" style=\"display:cell;padding-top:0px;padding-bottom:0px;padding-left:0px;\">";
                html += "<div id=\"" + item.name + "\"  style=\"display:cell;padding-top:00px;padding-bottom:0px;padding-left:2px;padding-right:0px;" + xHeight + "  \"></div></div></div>";
                break;
            case "wxtable":
                html += self.generateWxTable(item, "");
                break;
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
                    var item_autoid = nameRadio + (val.id || i);

                    html += "<label  for=\"" + item_autoid + "\" class=\"radio-inline\"><input type=\"radio\" data-value=\"" + item_value + "\" value=\"" + item_value +
                            "\" name=\"" + nameRadio + "\" id=\"" + item_autoid + "\" " + _any + item_checked + ">" + item_label + "</label>";
                });
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
                    aLengthMenu: [[5, 10, 15, 25, 50, 100, 1000], [5, 10, 15, 25, 50, 100, 1000]],
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
        });
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

    this.refreshGrid = function () {
        if (oTable !== undefined && oTable !== null) {
            oTable.fnClearTable();
        }
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
                           "<button class=\"btn small btn-info\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                           "</div>" +
                           "<table id=\"tblGrid\">" +
                           "<thead><tr>" + colhtml + "</tr></thead>" +
                           "</table>" +
                           "</div>" +
                           "<div class=\"buttons \">" +
                           "<button class=\"btn small btn-success\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                           "<button class=\"btn small btn-danger\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                           "</div>" +
                           "</div>";

            if (!_lookup.isConfugired) {
                $(".body > .panel.lookup").empty();
                $(".body > .panel.lookup").html(template);
                $(".body > .panel.lookup #btnClosePanel, .body > .panel #btnCancelPanel").on("click", function () {
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
                    iDisplayLength: 10,
                    //"sScrollX": "100%",
                    //"sScrollXInner": "110%",
                    //"bScrollCollapse": true
                }).fnSetFilteringDelay();
            }

            if (_lookup.isShown) {
                $(".body > .panel").fadeOut("slow", function () {
                    _lookup.isShown = false;
                });

                var e = "<div class=\"table-pannel-scroll\">";
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
                if (field.type === "dateTime") {
                    value = (value !== "&nbsp;") ? moment(value).format(SimDms.dateTimeFormat) : "&nbsp;";
                }
                if (field.type === "time") {
                    value = (value !== "&nbsp;") ? moment(value).format(SimDms.timeFormat) : "&nbsp;";
                }
                if (field.type === "price") {
                    value = (value !== "&nbsp;") ? number_format(value) : number_format(0);
                }
                if (field.type === "numeric") {
                    value = (value !== "&nbsp;") ? number_format(value, 2) : number_format(0, 2);
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
                    tableSettings.evtclick[i]($(this).context.className.split('-')[1].split(' ')[0], data, options);
                }
            }
        });

        $.each($(selector + " tbody tr"), function (key, val) {
            var rec = $(this);
            rec.on("click", function (evt) {
                if (tableSettings.rowclick[0] !=undefined)
                tableSettings.rowclick[0](key, data[key], selector);
            });
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

    this.onRowClick = function (callback) { tableSettings.rowclick.push(callback); }

    this.onGridClick = function (callback) { _this.gridSettings.evtclick.push(callback); }

    this.loadForm = function () {

        if (!IsFormLoad) {

            var _this = this;
            var _init = true;

            var template = "<div class=\"panel klookup\"  style=\"height:90%;width:100%\">" +
                           "<iframe  frameborder=\"0\" style=\"overflow:hidden;height:100%;width:100%\" height=\"100%\" width=\"100%\"></iframe>" +
                           "<div style=\"height:15px;\"></div><div class=\"buttons right\">" +
                           "<div class=\"button small btn btn-danger\" id=\"btnClosePanel\"> " +
                           "<i class=\"icon icon-hand-right\"></i>   Close</div>" +
                           "</div></div>";

            var $div = $(template);

            var HideForm = function () {
                $(".body > .panel").fadeOut();
            }

            $(".body > .panel.lookup").empty();
            $(".body > .panel.lookup").html($div);

            $("div#btnClosePanel").on("click", HideForm);

            //var popupLinker = options.compile($div)(options.scope); 
            // save Url to local database 
            localStorage.setItem("webFormUrl", "");

            $(".panel iframe").attr("src", "/Form");

            //return popupLinker;
            IsFormLoad = true;

        }

    }
};

SimDms.Widget.prototype.alert = function (mesage) {
    $(".message-alert > .message-text").text(mesage);
    $(".message-background,.message-alert").fadeIn(function () {
        $(".message-alert > .message-button > a").off();
        $(".message-alert > .message-button > a.btn-close").on("click", function () {
            $(".message-background,.message-alert").fadeOut();
        });
    });
}

SimDms.Widget.prototype.clear = function (panel) {
    var selector = ".main" + (panel == undefined ? "" : " #" + panel);
    $(selector + " select," + selector + " input," + selector + " textarea").val("");
    $(selector + " input[type='radio']").attr("value", "false");
    $(selector + " input[type='radio']").prop("checked", true).val(true);
    $(selector + " .panel.kgrid.k-panel > div > div.k-grid").empty().removeClass("k-widget");
    var editor = $(selector + " [data-type=krichtext]").data("kendoEditor");
    if (editor !== undefined) {
        editor.value("");
    }

    this.populate(this.default || {});
}

SimDms.Widget.prototype.confirm = function (mesage, callback) {
    $(".message-dialog > .message-text").html(mesage);
    $(".message-background,.message-dialog").fadeIn(function () {
        $(".message-dialog > .message-button > a").off();
        $(".message-dialog > .message-button > a.btn-yes").on("click", function () {
            $(".message-background,.message-dialog").fadeOut();
            if (callback !== undefined) callback("Yes");
        });
        $(".message-dialog > .message-button > a.btn-cancel").on("click", function () {
            $(".message-background,.message-dialog").fadeOut();
            if (callback !== undefined) callback("No");
        });
    });
}

SimDms.Widget.prototype.enable = function (options, callback) {
    $.each(options.items, function (idx, val) {
        var ctrl = $("[name=" + val + "]");
        var type = ctrl.data("type");
        if (ctrl.is("select")) {
            ctrl.attr("disabled", !options.value);
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
};

SimDms.Widget.prototype.endsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
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

        if ((options.type || "") == "kgridlnk") {
            var html = _this.toHtmlTable(data.data, items, undefined, options.border);
            _this.submitXls((options.fileName || options.name), html);
        }
        else {
            var html = _this.toHtmlTable(data, items, undefined, options.border);
            _this.submitXls((options.fileName || options.name), html);
        }
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
    var html = this.toHtmlTable(data, options.items, undefined, options.border);
    _this.submitXls((options.fileName || options.name), html);
}

SimDms.Widget.prototype.getnumber = function (number, decimals, dec_point, thousands_sep) {
    return number_format(number, decimals, dec_point, thousands_sep);
}

SimDms.Widget.prototype.guid = (function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
                   .toString(16)
                   .substring(1);
    }
    return function () {
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
               s4() + '-' + s4() + s4() + s4();
    };
})();

SimDms.Widget.prototype.hideAccordion = function () {
    $(".panel > .subtitle > .icon.icon-minus").hide();
}

SimDms.Widget.prototype.hideAjaxLoad = function () {
    $(".page .ajax-loader").fadeOut();
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

SimDms.Widget.prototype.hidePanel = function (options, callback) {
    var _this = this;
    var panel = null;

    if (_this.isNullOrEmpty(callback)) {
        callback = function () { };
    }

    if (typeof options == "string") {
        panel = $("#" + options);
        panel.slideUp(callback);
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                panel = $("#" + val);
            }
            else {
                if (val.type == "kgrid") {
                    panel = $("#" + val.name).parent().parent();
                }
                if (val.type == "k-grid") {
                    panel = $("#" + val.name).parent().parent().fadeOut();
                }
                if (_this.isNullOrEmpty(val.type)) {
                    panel = $("#" + val.name);
                }
            }

            if (options.unanimated) {
                panel.hide(callback);
                console.log("hide unanimated");
            }
            else {
                panel.fadeOut(callback);
                console.log("hide animated");
            }
        });
    }
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

SimDms.Widget.prototype.initializeKGrid = function () {
    var _this = this;
    var grids = _this.kgrids;
    $.each(grids, function () {
    });
}

SimDms.Widget.prototype.populate = function (data, arg1, arg2) {
    var iterator = 1;
    var _this = this;
    _this.iterator = 1;

    var selectorContainer = "";

    if (arguments.length == 2) {
        if (typeof arg1 !== "function") {
            if (_this.isNullOrEmpty(arg1) == false) {
                selectorContainer += arg1 + " ";
            }
        }
    }
    else if (arguments.length == 3) {
        if (_this.isNullOrEmpty(arg1) == false) {
            selectorContainer += arg1 + " ";
        }
    }

    $.each(data, function (key, val) {
        var ctrl = $(selectorContainer + " [name=" + key + "]");
        var type = ctrl.data("type");
        _this.populateValue(type, val, ctrl, selectorContainer, key)
        ctrl.removeClass("error");
    });

    setTimeout(function () {
        $(".page .main form").valid();
        if (typeof arg1 === "function") { arg1(data); }
        if (typeof arg2 === "function") { arg2(data); }
    }, 2000);

    // additional for select param
    $(".select-main").hide();
    $(".select-shadow").show();

    this.ReformatNumber();
}

SimDms.Widget.prototype.populateArea = function (container, data) {
    var _this = this;
    var selectorContainer = (container || "") + " ";

    $(".page .ajax-loader").fadeIn();
    $.each(data, function (key, value) {
        var ctrl = $(selectorContainer + " [name=" + key + "]");
        var type = ctrl.data("type");
        _this.populateValue(type, value, ctrl, selectorContainer)
        ctrl.removeClass("error");
    });
    $(".page .ajax-loader").fadeOut();
}

SimDms.Widget.prototype.populateValue = function (type, value, ctrl, selectorContainer, key) {
    var _this = this;

    if (ctrl.hasClass("select-param")) {
        var html = "<option value=\"" + value + "\">" + value + "</option>";
        ctrl.html(html);
    }

    switch (type) {
        case "date":
            value = (value) ? moment(value).format("DD-MMM-YYYY") : undefined;
            ctrl.val(value);
            break;
        case "datepicker":
            value = (value) ? moment(value).format("DD-MMM-YYYY") : undefined;
            ctrl.val(value);
            break;
        case "kdatepicker":
            value = (value) ? moment(value).format("DD-MMM-YYYY") : undefined;
            ctrl.val(value);
            break;
        case "kmonthpicker":
            value = (value) ? moment(value, "YYYYMM").format("MMMM YYYY") : undefined;
            ctrl.val(value);
            break;
        case "datetime":
            value = (value) ? moment(value).format("DD-MMM-YYYY HH:mm") : undefined;
            ctrl.val(value);
            break;
        case "datetimepicker":
            value = (value) ? moment(value).format("DD-MMM-YYYY HH:mm") : undefined;
            ctrl.val(value);
            break;
        case "kdatetimepicker":
            value = (value) ? moment(value).format("DD-MMM-YYYY HH:mm") : undefined;
            ctrl.val(value);
            break;
        case "decimal":
            ctrl.data("kendoNumericTextBox").value(_this.getnumber(value, 2));
            break;
        case "int":
            ctrl.data("kendoNumericTextBox").value(_this.getnumber(value, 0));
            break;
        case "krichtext":
            var y = document.createElement('textarea');
            y.innerHTML = value;
            ctrl.data("kendoEditor").value(y.value);
            break;
        case "switch":
            value = (value || false);
            $(selectorContainer + "#" + key + "Y").prop('checked', value).val(value);
            $(selectorContainer + "#" + key + "N").prop('checked', !value).val(value);
            break;
        default:
            ctrl.val(value);
            break;
    }
}

SimDms.Widget.prototype.post = function (url, arg1, arg2) {
    var _this = this;
    _this.incrementAjaxRequestCounter();

    if (_this.isNullOrEmpty(arg1.showAjax) == true || arg1.showAjax == false) {
        $(".page .ajax-loader").fadeIn();
    }

    if (arguments.length == 2) {
        $.post(SimDms.baseUrl + url, function (result) {
            arg1(result);
            _this.decrementAjaxRequestCounter();

            if (_this.getAjaxRequestCounter() - 1 <= 0) {
            }
            $(".page .ajax-loader").fadeOut();
        });
    }
    if (arguments.length == 3) {
        $.ajax({
            type: "POST",
            url: SimDms.baseUrl + url,
            data: arg1,
            traditional: true,
            success: function (result) {
                arg2(result);
                if (_this.isNullOrEmpty(arg1.showAjax) == true || arg1.showAjax == false) {
                    if (_this.getAjaxRequestCounter() - 1 <= 0) {
                    }
                }
                _this.decrementAjaxRequestCounter();
                $(".page .ajax-loader").fadeOut();

                if (result.success !== undefined) {
                    if (result.success) { }
                    else {
                        if (result.message !== undefined) {
                            //_this.Warning(result.message);
                        }
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                _this.Error(errorThrown);
                $(".page .ajax-loader").fadeOut();
            }
        });
    }
}

SimDms.Widget.prototype.select = function (options, callback) {
    var _this = this;
    var params = options.params || {};
    var selector = (options.selector || ("#" + options.name));

    if (options.data !== undefined) {
        $(selector).html("<option value='" + (options.optionValue || "") + "'>" + (options.optionText || $(selector).data("opttext") || SimDms.selectOneText) + "</option>");
        $.each(options.data, function (idx, val) {
            $(selector).append("<option value=\"" + val.value + "\">" + val.text + "</option>");
        });
        if (callback !== undefined && typeof callback === "function") callback("configured");
        return;
    }

    if (options.url == undefined) {
        if (callback !== undefined && typeof callback === "function") callback("url undefined");
        return;
    };

    $.each((options.additionalParams || []), function (idx, val) {
        params[val.name] = _this.getValue({ name: val.element, type: val.type }) || val.value;
    });

    _this.post(options.url, params, function (result) {
        $(selector).html("<option value='" + (options.optionValue || "") + "'>" + (options.optionText || $(selector).data("opttext") || SimDms.selectOneText) + "</option>");
        $.each(result, function (idx, val) {
            $(selector).append("<option value=\"" + val.value + "\">" + val.text + "</option>");
        });

        if (options.selected !== undefined) {
            $(selector).val(options.selected);
        }

        if (_this.isNullOrEmpty(options.callbackValue) == false) {
            $(selector).val(options.callbackValue);
        }

        if (callback !== undefined && typeof callback === "function") {
            callback(result);
        }
    });
}

SimDms.Widget.prototype.selectedRows = function (name, callback) {
    if (arguments.length == 2 && typeof name == "string" && typeof callback == "function") {
        var rows = $("#" + name + " tbody tr.k-state-selected");
        var grid = $("#" + name).data("kendoGrid");
        var data = [];
        $.each(rows, function (idx, val) {
            data.push(grid.dataItem(val));
        });
        return callback((data == null) ? undefined : data);
    }
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
            _this.select({ selector: selector, url: options.url, params: params }, function () {
                $(selector).change();
            });

            if (options.name !== undefined) {
                $("[name=" + options.name + "]").show();
                $("[name=" + options.name + "stext]").hide();
            }
        });
    }
}

SimDms.Widget.prototype.serializeObject = function (area) {
    var selector = ".page .main form" + ((area == undefined) ? "" : " #" + area);
    return $(selector).serializeObject();
}

SimDms.Widget.prototype.showAjaxLoad = function () {
    $(".page .ajax-loader").fadeIn();
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

SimDms.Widget.prototype.showNotification = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        $(".page > .notif-wrapper > .notification").text(msg);
        $(".page > .notif-wrapper").show();
        $(".page > .notif-wrapper").addClass("bounceInUp").removeClass("fadeOutRight");
        setTimeout(function () { $(".page > .notif-wrapper").addClass("fadeOutRight").removeClass("bounceInUp"); }, 3000);
    }
}

SimDms.Widget.prototype.Success = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.success(msg);
    }
}

SimDms.Widget.prototype.Info = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.info(msg);
    }
}

SimDms.Widget.prototype.Warning = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-full-width';
        toastr.warning(msg);
    }
}

SimDms.Widget.prototype.Error = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        toastr.options.positionClass = 'toast-bottom-full-width';
        toastr.error(msg);
    }
}


SimDms.Widget.prototype.showPanel = function (options, callback) {
    var _this = this;

    if (_this.isNullOrEmpty(callback)) {
        callback = function () { };
    }

    if (typeof options == "string") {
        $("#" + options).slideDown(callback);
    }
    else if (typeof options == "object") {
        $.each(options, function (idx, val) {
            if (typeof val == "string") {
                $("#" + val).fadeIn(callback);
            }
            else {
                if (val.type == "kgrid") {
                    $("#" + val.name).parent().parent().fadeIn(callback);
                }

                if (val.type == "k-grid") {
                    var panel = $("#" + val.name).parent().parent();
                    panel.fadeIn(callback);
                }

                if (_this.isNullOrEmpty(val.type)) {
                    var panel = $("#" + val.name);
                    panel.fadeIn(callback);

                    if (val.hideDivider) {
                        panel.children("div.divider").hide();
                    }
                }
            }
        });
    }
}

SimDms.Widget.prototype.showReport = function (options) {
    options.par = options.par || [];
    var url = "Reports/Viewer.aspx?rpt=" + options.id;
    var par = "";
    for (var i = 0; i < (options.par).length; i++) {
        par += ((i === 0) ? "&par=" : ";") + options.par[i];
    }
    console.log(par);
    console.log(SimDms.baseUrl + url + par + type + filename);
    var type = "&type=" + options.type;
    var filename = "&filename=" + options.filename;
    var rparam = "&rparam=" + options.rparam;
    if (options.type === "export") {
        //console.log("Window Load");
        window.location = SimDms.baseUrl + url + par + type + filename + rparam;
    }
    else if (options.panel === undefined) {
        console.log("undefined");
        $(".frame iframe").attr("src", SimDms.baseUrl + url + par + type + rparam);
    }
    else {
        $("#" + options.panel).load(SimDms.baseUrl + url + par + type + rparam);
        console.log("define");
    }
}


SimDms.Widget.prototype.showPdfReport = function (options, callback) {
    //console.log(options);
    //var _this = this;
    //var _init = true;

    //return showLookup();

    //function showLookup() {

    //    var template = "<div class=\"panel klookup\"  style=\"height:90%;width:100%\">" +
    //                   "<iframe  frameborder=\"0\" style=\"overflow:hidden;height:100%;width:100%\" height=\"100%\" width=\"100%\"></iframe>" +
    //                   "<div style=\"height:15px;\"></div><div class=\"buttons right\">" +
    //                   "<div class=\"button small btn btn-danger\" id=\"btnClosePanel\"> " +
    //                   "<i class=\"icon icon-hand-right\"></i>   Exit</div>" +
    //                   "</div></div>";

    //    var $div = $(template);

    //    var HideForm = function () {
    //        $(".body > .panel").fadeOut();
    //    }

    //    $(".body > .panel.lookup").empty();
    //    $(".body > .panel.lookup").html($div);
    //    $(".body > .panel").fadeIn("slow");

    //    $("div#btnClosePanel").on("click", HideForm);

    //    //var popupLinker = options.compile($div)(options.scope); 
    //    // save Url to local database 
    //    localStorage.setItem("webFormUrl", options.url);
    //    console.log(options.url);
    //    var urlRpt = window.reportUrl + "?id=" + options.id + "&pparam=" + options.pparam + "&rparam=" + options.rparam;

    //    options.par = options.par || [];
    //    var url = "Reports/Viewer.aspx?rpt=" + options.id;
    //    var par = "&par=" + options.pparam;
    //    var type = "&type=" + options.type;
    //    var rparam = "&rparam=" + options.rparam;
    //    //console.log(SimDms.baseUrl + url + par + type);
    //    //redirect to 
    //    $(".panel iframe").attr("src", SimDms.baseUrl + url + par + type);
}


SimDms.Widget.prototype.showPdfReportNewTab = function (options) {
    var urlRpt = window.reportUrl + "?id=" + options.id + "&pparam=" + options.pparam + "&rparam=" + options.rparam;

    options.par = options.par || [];
    var url = "Reports/Viewer.aspx?rpt=" + options.id;
    var par = "&par=" + options.pparam;
    var type = "&type=" + options.type;
    var rparam = "&rparam=" + options.rparam;
    //console.log(SimDms.baseUrl + url + par + type);
    //redirect to 
    //$(".panel iframe").attr("src", SimDms.baseUrl + url + par + type + rparam);
    var hst = window.location.origin;
    if (window.location.pathname.indexOf('/layout') > 0)
    {
        hst +="/" +window.location.pathname.split('/')[1];
    }

    console.log(hst + "/" + url + par + type + rparam);
    window.open(hst + "/" + url + par + type + rparam);
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
                       "<button class=\"btn small\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                       "</div>" +
                       "<div class='panel inside frame'>" +
                       "<iframe class=\"framereport\"" +
                       "</iframe>" +
                       "</div>" +
                       "</div>" +
                       "</div>" +
                       "</form>";

        if (!_lookup.isConfugired) {
            var heightScreen = $(window).height();
            console.log(heightScreen);
            $(".body > .panel").empty();
            $(".body > .panel").html(template);
            $('.panel').css({ width: '90%' });
            //$('.lookup > .panel').css({ 'height': heightScreen });
            $('.inside').css({ width: 1310, 'margin-top': 30, height: (heightScreen - 100), border: 'none' });
            $('.framereport').css({ width: '100%', height: '100%', 'margin-top': -20, 'margin-left': -20, bottom: 0 });
            $(".body > .panel #btnClosePanel, .body > .panel #btnCancelPanel").on("click", function () {
                _lookup.hide();
            });
        }

        _lookup.isConfugired = false;

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
                var par = "&par=" + options.par;
                var type = "&type=" + options.type;
                var rparam = "&rparam=" + options.rparam;
                console.log(SimDms.baseUrl + url + par + type);
                $(".frame iframe").attr("src", SimDms.baseUrl + url + par + type + rparam);
            });
        }
    },
    hide: function () {
        var _lookup = this;
        $(".body > .panel").fadeOut("slow", function () {
            _lookup.isShown = false;
        });
    }
}

SimDms.Widget.prototype.XlsxReport = function (o, callback) {
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
            else {
                $('.page > .ajax-loader').hide();
                MsgBox("Tidak Ada Data Untuk Ditampilkan", MSG_WARNING);
            }

            if (callback != undefined) {
                callback();
            }
        });
    }
}

SimDms.Widget.prototype.showToolbars = function (items) {
    $(".page .toolbar > div").hide();
    $.each(items, function (idv, val) {
        $(".page .toolbar > div." + val).show();
    });
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

SimDms.Widget.prototype.validate = function (s) {
    if (s !== undefined) {
        s = "form#" + s;
        return $(s).valid();
    }
    return $(".page .main form").valid();
}

SimDms.Widget.prototype.submit = function () {
    $(".page .main form").submit();
}

SimDms.Widget.prototype.reset = function () {
    $(".page .main form").trigger('reset');
    //$("span.validetta-inline").addClass("hide");
}

SimDms.Widget.prototype.OnValidation = function (e) {
    $(".page .main form").bind("ValidationOK", e);
}

SimDms.Widget.prototype.TabChanged = function (e) {
    $(".page .main form").bind("TabChanged", e);
}

SimDms.Widget.prototype.validateTime = function (time) {
    if (time === null || time.length !== 5) { time = "00:00" };
    return time;
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

SimDms.Widget.prototype.toHtmlTable = function (data, items, header, border) {
    if (data == undefined) return;

    var html = "";
    var items = items || [];
    var eborder = (border == undefined) ? "thin solid" : "";
    var sborder = (eborder.length > 0) ? " style=\"border:" + eborder + "\"" : "";

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
            var attr = sborder;
            if (eborder.length > 0) {
                if (val.width !== undefined) {
                    attr = " style=\"border:" + eborder + ";width:" + val.width + "px\"";
                }
            }
            else {
                if (val.width !== undefined) {
                    attr = " style=\"width:" + val.width + "px\"";
                }
            }
            var text = (val.text || val.title || val.name);
            html += "<th" + attr + ">" + text + "</th>";
        });
        html += "</tr>";
    }

    $.each(data, function (idx, val) {
        html += "<tr>";

        $.each(items, function () {
            var text = val[this.name || this.field];
            if (this.type == "date" && text != null && text.length > 0) {
                text = moment(text).format('MM/DD/YYYY');
            }
            if (this.type == "datetime" && text != null && text.length > 0) {
                text = moment(text).format('MM/DD/YYYY HH:mm:ss');
            }
            if (this.type == "text" && text != null && text.length > 0) {
                text = "=\"" + text + "\"";
            }
            html += "<td" + sborder + ">" + (text || "") + "</td>";
        });
        html += "</tr> \n";
        return;
    });

    html = "<table>\n" + html + "</table>";

    return html;
}

SimDms.Widget.prototype.numberFormat = function (number, decimals, dec_point, thousands_sep) {
    return number_format(number, decimals, dec_point, thousands_sep);
}


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

function number_format(number, decimals, dec_point, thousands_sep) {
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

$(".dropdown > li").hover(function () {

    var $container = $(this),
        $list = $container.find("ul"),
        $anchor = $container.find("a"),
        height = $list.height() * 1.1,       // make sure there is enough room at the bottom
        multiplier = height / maxHeight;     // needs to move faster if list is taller

    // need to save height here so it can revert on mouseout            
    $container.data("origHeight", $container.height());

    // so it can retain it's rollover color all the while the dropdown is open
    $anchor.addClass("hover");

    // make sure dropdown appears directly below parent list item    
    $list
        .show()
        .css({
            paddingTop: $container.data("origHeight")
        });

    // don't do any animation if list shorter than max
    if (multiplier > 1) {
        $container
            .css({
                height: maxHeight,
                overflow: "hidden"
            })
            .mousemove(function (e) {
                var offset = $container.offset();
                var relativeY = ((e.pageY - offset.top) * multiplier) - ($container.data("origHeight") * multiplier);
                if (relativeY > $container.data("origHeight")) {
                    $list.css("top", -relativeY + $container.data("origHeight"));
                };
            });
    }

}, function () {

    var $el = $(this);

    // put things back to normal
    $el
        .height($(this).data("origHeight"))
        .find("ul")
        .css({ top: 0 })
        .hide()
        .end()
        .find("a")
        .removeClass("hover");

});

function isNumber(event) {
    if (event) {
        var charCode = (event.which) ? event.which : event.keyCode;
        if (charCode != 190 && charCode > 31 && charCode != 173 && charCode != 109 &&
           (charCode < 48 || charCode > 57) &&
           (charCode < 96 || charCode > 105) &&
           (charCode < 37 || charCode > 40) &&
            charCode != 110 && charCode != 8 && charCode != 46)
            return false;
    }
    return true;
}


