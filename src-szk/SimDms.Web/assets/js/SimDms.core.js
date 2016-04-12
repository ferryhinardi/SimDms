!function () {
    var SimDms = {
        version: "1.0.0",
        baseUrl: "/",
        selector: "body",
        timeFormat: "hh:mm:ss",
        dateFormat: "DD-MMM-YYYY",
        dateTimeFormat: "DD-MMM-YYYY hh:mm:ss",
        template: '<div class="page">' +
                  ' <div class="header"><div class="app-title"></div><div class="app-subtitle"></div><div class="user-info"></div></div>' +
                  ' <div class="navmenu"></div>' +
                  ' <div class="title"><h3 class="animated">&nbsp;</h3></div>' +
                  ' <div class="toolbar"></div>' +
                  ' <div class="body"><div class="main animated"></div><div class="panel"></div><div id="myresultSQL"></div></div>' +
                  ' <div class="footer"></div>' +
                  ' <div class="ajax-loader"></div>' +
                  ' <div class="overlay"></div>' +
                  ' <div class="popup-frame outer">' +
                  '  <div class="popup-frame inner">' +
                  '   <div class="popup-header">' +
                  '    <div class="title"></div>' +
                  '    <div class="corner-bar close"><i class="icon icon-remove"></i></div>' +
                  '    <div class="spacer-both"></div>' +
                  '   </div>' +
                  '   <div class="popup-body"></div>' +
                  '   <div class="popup-footer"></div>' +
                  '  </div>' +
                  ' </div>' +
                  ' <div class="notif-wrapper"><div class="notification"></div></div>' +
                  '</div>' +
                  '<div id="print" class="wg-print"></div>' +
                  '<div id="modal" class="wg-modal"><div class="modal fade"></div></div>' +
                  '<div id="loading" class="wg-loading"><i class="fa fa-circle-o-notch fa-spin"></i></div>',

        template2: '<div class="page">' +
          //' <div class="header"><div class="app-title"></div><div class="app-subtitle"></div><div class="user-info"></div></div>' +
          //' <div class="navmenu"></div>' +
          //' <div class="title"><h3 class="animated">&nbsp;</h3></div>' +
          //' <div class="toolbar"></div>' +
          //' <div class="body"><div class="main animated"></div><div class="panel"></div></div>' +
          ' <div id="myContent"></div>' +
          ' <div class="footer"></div>' +
          ' <div class="ajax-loader"></div>' +
          ' <div class="overlay"></div>' +
          ' <div class="popup-frame outer">' +
          '  <div class="popup-frame inner">' +
          '   <div class="popup-header">' +
          '    <div class="title"></div>' +
          '    <div class="corner-bar close"><i class="icon icon-remove"></i></div>' +
          '    <div class="spacer-both"></div>' +
          '   </div>' +
          '   <div class="popup-body"></div>' +
          '   <div class="popup-footer"></div>' +
          '  </div>' +
          ' </div>' +
          ' <div class="notif-wrapper"><div class="notification"></div></div>' +
          '</div>' +
          '<div id="print" class="wg-print"></div>' +
          '<div id="modal" class="wg-modal"><div class="modal fade"></div></div>' +
          '<div id="loading" class="wg-loading"><i class="fa fa-circle-o-notch fa-spin"></i></div>',

        selectOneText: "-- SELECT ONE --",
        selectAllText: "-- SELECT ALL --",
        defaultEmployeePhoto: "/assets/img/employee/person.png",
        defaultIdentityCardPhoto: "/assets/img/employee/ktp.png",
        themes: "",
        switchChangeDelay: 500,
        defaultTimeout: 500,
        popupHeight: 350,
        popupWidth: 600,
        defaultTitle: { popup: "Suzuki Data Warehouse" },
        defaultInformationMessage: "Your data has been saved.",
        defaultErrorMessage: "Sorry, we cannot processing your request.\nPlease, try again later!",
        exportXlsUrl: "excel/export",
    }

    window.SimDms = SimDms;
    window.sdms = SimDms;
}();

SimDms.Layout = function () {
    "use strict";

    var _this = this;
    var isRendered = false;
    var configured = false;
    var module = "";
    var group = "";
    var menu = "";
    var tick01 = 0;
    var tick10 = 0;

    this.render = function (options) {
        if (isRendered) {
            return;
        }

        var selector = SimDms.selector;
        this.configure(selector, options);

        var moduleChanged = [];
        var groupChanged = [];
        var menuChanged = [];
        var rendered = [];

        this.onModuleChanged = function (callback) { moduleChanged.push(callback); };
        this.onGroupChanged = function (callback) { groupChanged.push(callback); };
        this.onMenuChanged = function (callback) { menuChanged.push(callback); };
        this.onRendered = function (callback) { menuChanged.push(callback); };
        this.onHashChanged(function (e, data) {
            var hashs = data.split("/");
            if (hashs.length > 0) {
                if (hashs[0] === "lnk") {
                    if (hashs.length > 1) {
                        if (module != (hashs[1] || "")) {
                            for (var i = 0; i < moduleChanged.length; i++) {
                                moduleChanged[i](e, (hashs[1] || ""));
                            }
                            module = hashs[1];
                        }

                        if (group !== (hashs[2] || "")) {
                            for (var i = 0; i < groupChanged.length; i++) {
                                groupChanged[i](e, (hashs[1] || ""), (hashs[2] || ""));
                            }
                            group = hashs[2];
                        }

                        if (menu !== (hashs[3] || "")) {
                            for (var i = 0; i < menuChanged.length; i++) {
                                menuChanged[i](e, (hashs[1] || ""), (hashs[2] || ""), (hashs[3] || ""));
                            }
                            menu = hashs[3];
                        }
                    }
                }
                else {
                    if (hashs.length <= 1) {
                        if (module.length > 0) {
                            for (var i = 0; i < moduleChanged.length; i++) {
                                moduleChanged[i](e, "");
                            }
                            for (var i = 0; i < groupChanged.length; i++) {
                                groupChanged[i](e, "", "");
                            }
                            for (var i = 0; i < menuChanged.length; i++) {
                                menuChanged[i](e, "", "");
                            }
                            module = "";
                            group = "";
                            menu = "";
                        }
                    }
                }
            }
        });

        this.initializePopup();

        // update status rendered
        isRendered = true;
        for (var i = 0; i < rendered.length; i++) {
            rendered[i]();
        }
    }

    this.configure = function (selector, options) {
        if (configured) {
            //console.log("already configured...!");
            return;
        }

        $(selector).empty();

        if (options.useExt !== undefined)
            $(selector).html(SimDms.template2);
        else
            $(selector).html(SimDms.template);

        if (options != undefined) {
            $(".page .header .app-title").text(options.title || "");
            $(".page .header .app-subtitle").text(options.subtitle || "");
        }


        // tracking on changing hash
        var hashChanged = [];
        var hash = "";
        var parse = function (val) {
            if (!val) {
                val = "";
            } else {
                if (val.substring(0, 1) == "#") {
                    val = val.substring(1);
                }
            }
            return val;
        };

        setInterval(function () {
            var newHash = parse(window.location.hash);
            if (hash != newHash) {
                for (var i = 0; i < hashChanged.length; i++) {
                    hashChanged[i](this, newHash);
                }
                hash = newHash;
            }

            if (SimDms.onSecondChanged !== undefined) {
                tick01++;
                if (tick01 % 10 == 0) {
                    SimDms.onSecondChanged();
                    tick01 = 0;
                }
            }

            if (SimDms.onTenSecondChanged !== undefined) {
                tick10++;
                if (tick10 % 100 == 0) {
                    SimDms.onTenSecondChanged();
                    tick10 = 0;
                }
            }

        }, 100);

        this.onHashChanged = function (callback) { hashChanged.push(callback); };

        // update status configured
        configured = true;
    }
}

SimDms.Layout.prototype.loadNavMenu = function (id) {
    "use strict";

    var url = SimDms.baseUrl + "layout/listmenu/" + id;
    var nav = $(SimDms.selector + " .navmenu");
    var _this = this;
    $.post(url, function (result) {
        if (result.success) {
            var html = "";
            $.each(result.data, function (idx, val) {
                if (val.Detail !== null && val.Detail.length > 0) {
                    html += "<li class=\"has-dropdown\"><a>" + val.MenuCaption +
                            "</a>" + _this.loadNavMenuChild(val.Detail, id) + "</li>";
                }
                else {
                    if ((val.MenuUrl || "").length > 0) {
                        html += "<li><a href=\"#lnk/" + id + "/" + val.MenuUrl + "\">" + val.MenuCaption + "</a></li>";
                    }
                    else {
                        html += "<li><a href=\"#lnk/" + id + "\">" + val.MenuCaption + "</a></li>";
                    }
                }
            });

            html = "<div class=\"top-bar\"><div class=\"top-bar-section\"><ul>" + html + "</ul></div></div>";
            nav.html(html);
            nav.foundation('topbar');
            $('.has-dropdown').off();
            $('.has-dropdown').on('click', function () {
                $(this).find('.dropdown').hide();
            });
            $('.has-dropdown').on('mousemove', function () {
                $(this).find('.dropdown').fadeIn()
            });
        }
    });
}

SimDms.Layout.prototype.loadNavMenuChild = function (data, id) {
    var html = "";
    var _this = this;
    $.each(data, function (idx, val) {
        if (val.Detail !== null && val.Detail.length > 0) {
            html += "<li class=\"has-dropdown\"><a>" + val.MenuCaption +
                    "</a>" + _this.loadNavMenuChild(val.Detail, id) + "</li>";
        }
        else {
            html += "<li><a href=\"#lnk/" + id + "/" + val.MenuUrl + "\">" + val.MenuCaption + "</a></li>";
        }
    });
    return "<ul class=\"dropdown\">" + html + "</ul>";
}

SimDms.Layout.prototype.loadAjaxLoader = function () {
    var html = "";
    for (var i = 1; i <= 5; i++) {
        html += "<div class=\"wBall\" id=\"wBall_" + i + "\">" +
                "<div class=\"wInnerBall\">" +
                "</div>" +
                "</div>"
    }
    html = "<div class=\"loading-image\"></div><div class=\"windows8\">" + html + "</div>";
    $(".page .ajax-loader").empty().html(html);
}

SimDms.Layout.prototype.loadUserInfo = function (user) {
    var html = "login as : " + user + "&nbsp;|&nbsp;" +
               "<a href=\"" + SimDms.baseUrl + "account/logout" + "\">logout</a>&nbsp;|&nbsp;" +
               "<a data-id='change-password'>change password</a>&nbsp;|&nbsp;" +
               "<a href=\"" + SimDms.baseUrl + "account/loginfo" + "\">menu</a>&nbsp;|&nbsp;" +
               "<a href=\"" + SimDms.baseUrl + "account/RedirectToDoc" + "\">documentation</a>";

    $(".page .header .user-info").html(html);
    this.initializeCornerBarMenu();
}

SimDms.Layout.prototype.endsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

SimDms.Layout.prototype.autoMovePopup = function () {
    $(window).resize(function () {
        var popupContainer = $(".popup-frame.outer");
        var screenHeight = $(window).height();
        var screenWidth = $(window).width();
        var popupHeight = SimDms.popupHeight;
        var popupWidth = SimDms.popupWidth;
        var left = (screenWidth - popupWidth) / 2;
        var top = (screenHeight - popupHeight) / 2;

        popupContainer.css({
            "position": "absolute",
            "left": left + "px",
            "top": top + "px",
            "height": popupHeight + "px",
            "width": popupWidth + "px"
        });
    });
}

SimDms.Layout.prototype.showNotification = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        $(".page > .notif-wrapper > .notification").text(msg);
        $(".page > .notif-wrapper").fadeIn();
        setTimeout(function () { $(".page > .notif-wrapper").fadeOut(); }, 3000);
    }
}

function getRadioValue(name) {
    var rbs = document.getElementsByName(name);
    var ret = null;
    for (var i = 0; i < rbs.length; i++) {
        if (rbs[i].checked) {
            ret = rbs[i].value;
            break;
        }
    }
    return ret;
}



function setRadioValue(name,value) {
    var rbs = document.getElementsByName(name);
    for (var i = 0; i < rbs.length; i++) {
        if (rbs[i].value == value.toString()) {
            document.getElementById(rbs[i].id).checked = true;
            break;
        }
    }
}

$.fn.resetState = function () {
    $(this).each(function () {
        var rbs = document.getElementsByName(this.name);
        for (var i = 0; i < rbs.length; i++) {
            document.getElementById(rbs[i].id).checked = rbs[i].defaultChecked;
            //document.getElementById(rbs[i].id).value = rbs[i].getAttribute("data-value");
        }
    });
}


$.fn.serializeObject = function () {
    var row = {};
    var oList = $(this).find('input[type="hidden"],input[type="text"],input[type="password"],input[type="checkbox"]:checked,input[type="radio"]:checked,select,textarea');

    oList.each(function () {
        if (this.name == null || this.name == undefined || this.name == '') return;
        var _val = null

        if ($(this).is('select')) {
            _val = $(this).find('option:selected').val();
        }
        else if ($(this).hasClass("number")) {
            _val = this.value.replace(",", "");
        }
        else {
            var _type = $(this).data("type") || "";
            switch (_type) {
                case "int":
                    _val = $(this).data("kendoNumericTextBox").value();
                    break;
                case "switch":
                    //_val = eval(this.value);
                    //break;
                case "radio":
                    _val = getRadioValue(this.name);
                    console.log("radio: ",this.name, _val);
                    break;
                case "decimal":
                    _val = $(this).data("kendoNumericTextBox").value();
                    break;
                case "datepicker":
                    if (this.value.length > 0) {
                        _val = moment(this.value, SimDms.dateFormat).format("YYYY-MM-DD");
                    }
                    else {
                        _val = undefined;
                    }
                    break;
                case "datetimepicker":
                    if (this.value.length > 0) {
                        _val = moment(this.value, SimDms.dateFormat).format("YYYY-MM-DD hh:mmA");
                    }
                    else {
                        _val = undefined;
                    }
                    break;
                default:
                    _val = this.value;
                    break;
            }
        }

        row[this.name] = _val;

    });
    return row;
}





// CUSTOM ELEMENT AREA 

SimDms.Layout.prototype.loadChangePasswordForm = function () {
    //var html = "";
    //html += "<div class='overlay'></div>";
    //html += "<form id='formChangePassword' name='formChangePassword'>" +
    //            "<div class='popup-frame outer change-password'>" +
    //                "<div class='popup-frame inner'>" +
    //                    "<div class='header'>" +
    //                        "<div class='title'>Change Password</div>" +
    //                    "</div>" +
    //                    "<div class='body'>" +
    //                        "<div class='input-wrapper'><input required='true' type='password' name='OldPassword' id='OldPassword' class='' placeholder='Old Password' /></div>" +
    //                        "<div class='input-wrapper'><input required='true' type='password' name='NewPassword' id='NewPassword' class='' placeholder='New Password' /></div>" +
    //                        "<div class='input-wrapper'><input required='true' type='password' name='ConfirmNewPassword' id='ConfirmNewPassword' class='' placeholder='Confirm Password' /></div>" +
    //                    "</div>" +
    //                    "<div class='popup-footer'>" +
    //                        "<div class='panel-buttons'>" +
    //                            "<button class='small' id='btnCancelChangePassword' name='btnCancelChangePassword'>Cancel</button>" +
    //                            "<button class='small' id='btnConfirmChangePassword' name='btnConfirmChangePassword'>Confirm</button>" +
    //                        "</div>" +
    //                        "<div class='spacer'></div>" +
    //                    "</div>" +
    //                "</div>" +
    //            "</div>" +
    //        "</form>";

    //$("body").append(html);
}

SimDms.Layout.prototype.initializePopup = function () {
    var overlay = $(".overlay");
    var popup = $(".popup-frame.outer");

    overlay.on("click", function (evt) {
        overlay.fadeOut();
        popup.fadeOut();
    });

    $(".popup-frame.outer > .popup-frame.inner > .popup-header > .corner-bar.close").on("click", function (evt) {
        overlay.fadeOut();
        popup.fadeOut();
    });
}

SimDms.Layout.prototype.showCustomPopup = function (options) {
    /*  OPTIONS DETAILS

        options = {
            isCustomSize: true or false,
            customeSize: [width, heigth],
            callback: function() {},
            title: "title string",
            buttons: [
                { name: "", cls: "", text: "", icon: "" },
                { name: "", cls: "", text: "", icon: "" },
                { name: "", cls: "", text: "", icon: "" }
            ],
            htmlContent: "string of html content"
        }
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        var overlay = $(".overlay");
        var popupFrameOuter = $(".popup-frame.outer");
        var popupFrameInner = popupFrameOuter.children(".popup-frame.inner");
        var windowHeight = $(window).height();
        var windowWidth = $(window).width();
        var popupWidth = popupFrameOuter.width();
        var popupHeight = popupFrameOuter.height();
        var marginTop = 0;
        var marginLeft = 0;

        if (options.customSize) {
            popupWidth = options.customSize[0];
            popupHeight = options.customSize[1];
        }

        marginLeft = (windowWidth - popupWidth) / 2;
        marginTop = 100;

        popupFrameOuter.css({
            "width": popupWidth + "px",
            "position": "absolute",
            "left": marginLeft + "px",
            "top": marginTop + "px"
        });

        if (_this.isNullOrEmpty(popupHeight) == false && options.isCustomSize == true) {
            popupFrameOuter.css({
                "height": popupHeight + "px"
            });
        }

        var header = popupFrameInner.children(".popup-header");
        var body = popupFrameInner.children(".popup-body");
        var footer = popupFrameInner.children(".popup-footer");
        var htmlButtons = "";

        $.each(options.buttons || [], function (key, val) {
            htmlButtons += "<button id='" + (val.name || "") + "' class='btn " + (val.cls) + "'><i class='icon " + (val.icon || "") + "'></i> " + (val.text || "") + "</button>";
        });

        if (_this.isNullOrEmpty(htmlButtons) == false) {
            htmlButtons = "<div class='buttons'>" + htmlButtons + "</div>";
        }

        header.children(".title").text(options.title || SimDms.defaultTitle.popup);
        body.html(options.htmlContent);
        footer.html(htmlButtons);

        overlay.fadeIn();
        popupFrameOuter.fadeIn();
        popupFrameInner.fadeIn();

        if (_this.isNullOrEmpty(options.callback) == false && typeof options.callback == "function") {
            options.callback();
        }
    }
}

SimDms.Layout.prototype.hideCustomPopup = function () {
    $(".popup-frame.outer, .overlay").fadeOut();
}




// INITIALIZE CUSTOM ELEMENT EVENT

SimDms.Layout.prototype.initializeCornerBarMenu = function () {
    var _this = this;
    var changePasswordCornerMenu = $("[data-id='change-password']");
    changePasswordCornerMenu.on("click", function (evt) {
        _this.evt_ChangePassword(evt);
    });
}

SimDms.Layout.prototype.evt_ChangePassword = function (evt) {
    var _this = this;
    var popupContent = "<form id='formChangePassword' name='formChangePassword'>" +
                       "<div class='input-wrapper'><input required='true' type='password' name='OldPassword' id='OldPassword' class='' placeholder='Old Password' /></div>" +
                       "<div class='input-wrapper'><input required='true' type='password' name='NewPassword' id='NewPassword' class='' placeholder='New Password' /></div>" +
                       "<div class='input-wrapper'><input required='true' type='password' name='ConfirmPassword' id='ConfirmNewPassword' class='' placeholder='Confirm Password' /></div>"
    "</form>";


    _this.showCustomPopup({
        isCustomSize: true,
        customSize: [500, null],
        title: "Change Password",
        htmlContent: popupContent,
        buttons: [
            { name: "btnSaveChangePassword", text: "Save Changes", icon: "icon-save", cls: "small" },
            { name: "btnCancelChangePassword", text: "Cancel", icon: "icon-refresh", cls: "small" },
        ],
        callback: function () {
            $("#btnSaveChangePassword").on("click", function (evt) {
                var widget = new SimDms.Widget({ autorender: true });

                var url = "Account/ChangePassword";
                var data = widget.getForms("formChangePassword");
                var validation = widget.validate("formChangePassword");

                if (validation) {
                    widget.post(url, data, function (result) {
                        if (result.Status) {
                            _this.hideCustomPopup();
                        }
                        else {
                            alert(result.Message);
                        }
                    });
                }
            });

            $("#btnCancelChangePassword").on("click", function (evt) {
                _this.hideCustomPopup();
            });
        }
    });
}

SimDms.Layout.prototype.evt_WindowResize = function (evt) {
    // RESIZING AND MOVING POPUP FRAME
    var overlay = $(".overlay");
    var popup = $(".popup-frame.outer");
    var windowHeight = $(window).height();
    var windowWidth = $(window).width();
    var popupWidth = popup.width();
    var popupHeight = popup.height();
    var marginLeft = (windowWidth - popupWidth) / 2;
    var marginTop = 100;

    popup.css({
        "position": "absolute",
        "left": marginLeft + "px",
        "top": marginTop + "px"
    });
}



// UTILITIES AREA

SimDms.Layout.prototype.isNullOrEmpty = function (params) {
    var status = false;
    var _this = this;

    if (_this.isArray(params) == true) {
        $.each(params, function (key, val) {
            if (_this.isNullOrEmpty(val) == true) {
                status = true;
            }
        });
    }
    else {
        if (params != null && params != undefined && params !== null && params !== undefined && $.trim(params) != "" && $.trim(params) !== "") {
            status = false;
        }
        else {
            status = true;
        }
    }

    return status;
}

SimDms.Layout.prototype.isArray = function (params) {
    try {
        if (params.constructor === Array) {
            return true;
        }
        else {
            return false;
        }
    }
    catch (ex) {
        return false;
    }
}
