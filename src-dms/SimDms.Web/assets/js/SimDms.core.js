!function () {
var SimDms = {
    version: "1.0.0",
    baseUrl: "/",
    selector: "#content",
    timeFormat: "HH:mm:ss",
    dateFormat: "DD-MMM-YYYY",
    dateTimeFormat: "DD-MMM-YYYY HH:mm",
    template: "<div class=\"page\" id=\"currentpage\">" +
              "<div class=\"header\"><div class=\"app-title\"></div><div class=\"app-subtitle\"></div><div class=\"user-info\">" +

                //"<div class=\"pull-right\"> " +
                //"<div id=\"hide-menu\" class=\"btn-header pull-right\"><span> <a href=\"javascript:void(0);\" data-action=\"toggleMenu\" title=\"Collapse Menu\"><i class=\"fa fa-reorder\"></i></a> </span></div> " +
                //"<div id=\"logout\" class=\"btn-header transparent pull-right\"><span> <a href=\"account/logout\" title=\"Sign Out\" data-action=\"userLogout\" ><i class=\"fa fa-sign-out\"></i></a> </span></div> " +
                //"<div id=\"fullscreen\" class=\"btn-header transparent pull-right\"><span> <a href=\"javascript:void(0);\"  " +
                //"data-action=\"launchFullscreen\" title=\"Full Screen\"><i class=\"fa fa-arrows-alt\"></i></a></span></div></div> " +

              "</div><div class='branch-info'></div><div class='notification-corner-wrapper'><div class='notification-corner'></div></div>     </div>" +
              "<div id=\"ribbon\"><span class=\"ribbon-button-alignment\"><button id=\"btnRefresh\" type=\"button\" class=\"btn btn-ribbon\" onclick=\"RefreshEvent(this)\" >" + 
              "<i class=\"fa fa-refresh\"></i></button></span><span class=\"breadcrumb\">Dashboard</span>" +

              "</div><div class=\"title\"><h3 class=\"animated\">&nbsp;</h3></div>" +
              "<div class=\"toolbar\"></div>" +
              "<div class=\"body\"><div class=\"main animated\"></div><div class=\"panel shadow\"></div><div class=\"panel lookup\"></div></div>" +
              "<div class=\"footer\"></div>" +
              "<div class=\"ajax-loader\"></div>" +
              "<div class=\"overlay\"></div>" +
              "<div class=\"popup-frame outer\">" +
              "<div class=\"popup-frame inner\">" +
              "<div class='popup-header'></div>" +
              "<div class='popup-body'></div>" +
              "<div class='popup-footer'></div>" +
              "</div>" +
              "</div>" +
              "<div class=\"message-background\"></div>" +
              "<div class=\"message-dialog\">" +
                  "<div class=\"message-text\"></div>" +
                  "<div class=\"message-button\"><a class=\"btn-yes\">Ya</a><a class=\"btn-cancel\">Tidak</a></div>" +
              "</div>" +
              "<div class=\"message-alert\">" +
                  "<div class=\"message-text animated\"></div>" +
                  "<div class=\"message-button\"><a class=\"btn-close\">Close</a></div>" +
              "</div>" +
              "<div class=\"notif-wrapper animated fadeOutRight\"><div class=\"notification\"></div></div>" +
              "</div>",
    template2: "<div class=\"page\" id=\"currentpage\">" +

              "<div class=\"title\"><h3 class=\"animated\">&nbsp;</h3></div>" +
              "<div class=\"toolbar\"></div>" +
              "<div class=\"body\"><div class=\"main animated\"></div>" +
              "<div class=\"panel shadow\">" +
              "</div><div class=\"panel lookup\"></div></div>" +
              "<div class=\"footer\"></div>" +
              "<div class=\"ajax-loader\"></div>" +
              "<div class=\"overlay\"></div>" +
              "<div class=\"popup-frame outer\">" +
              "<div class=\"popup-frame inner\">" +
              "<div class='popup-header'></div>" +
              "<div class='popup-body'></div>" +
              "<div class='popup-footer'></div>" +
              "</div>" +
              "</div>" +
              "</div>",             
    selectOneText: "-- SELECT ONE --",
    selectAllText: "-- SELECT ALL --",
    defaultEmployeePhoto: "assets/img/employee/person.png",
    defaultIdentityCardPhoto: "assets/img/employee/ktp.png",
    defaultFamilyCardPhoto: "assets/img/employee/kk.png",
    themes: "",
    switchChangeDelay: 500,
    defaultTimeout: 500,
    popupHeight: 350,
    popupWidth: 600,
    defaultErrorMessage: "Sorry, we cannot process your request.\nPlease, try again later!",
    defaultInformationMessage: "Your data has been saved.",
    exportXlsUrl: "excel/export"
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

        if (options != undefined) {
            $(selector).html(SimDms.template);
            $(".page .header .app-title").text(options.title || "");
            $(".page .header .app-subtitle").text(options.subtitle || "");
        } else {
            $(selector).html(SimDms.template2);                       
            $(".page .title").css("top","8px");
            $(".page .toolbar").css("margin-top","35px");
        }

        // start demo

        $('#main')
            .append('<div class="demo"><span id="demo-setting"><i class="fa fa-cog txt-color-blueDark"></i></span><form><section>' +
            '<a data-id="change-password" style="font-size:12px;"><li>Change Password</li></a>' +
            '<a data-id="change-profit" style="font-size:12px;"><li>Change Profit Center</li></a>' +
            '<a data-id="change-branch"  style="font-size:12px;"><li>Change Branch</li></a>' +
            '<a data-id="change-tpgo"  style="font-size:12px;"><li>Change Type Of Goods</li></a>' +
            '<div class="pull-right" style="margin-top:22px"> ' +
            '<div id="hide-menu" class="btn-header pull-right"><span> <a href="javascript:void(0);" data-action="toggleMenu" title="Collapse Menu"><i class="fa fa-reorder"></i></a> </span></div> ' +
            '<div id="logout" class="btn-header transparent pull-right"><span> <a href="account/logout" title="Sign Out" data-action="userLogout" ><i class="fa fa-sign-out"></i></a> </span></div> ' +
            '<div id="fullscreen" class="btn-header transparent pull-right"><span> <a href="javascript:void(0);"  ' +
            'data-action="launchFullscreen" title="Full Screen"><i class="fa fa-arrows-alt"></i></a></span></div></div> ' +
            '</section></form></div>'
            );

        // hide bg options
        var smartbgimage = "<h6 class='margin-top-10 semi-bold'>Background</h6><img src='img/pattern/graphy-xs.png' data-htmlbg-url='img/pattern/graphy.png' width='22' height='22' class='margin-right-5 bordered cursor-pointer'><img src='img/pattern/tileable_wood_texture-xs.png' width='22' height='22' data-htmlbg-url='img/pattern/tileable_wood_texture.png' class='margin-right-5 bordered cursor-pointer'><img src='img/pattern/sneaker_mesh_fabric-xs.png' width='22' height='22' data-htmlbg-url='img/pattern/sneaker_mesh_fabric.png' class='margin-right-5 bordered cursor-pointer'><img src='img/pattern/nistri-xs.png' data-htmlbg-url='img/pattern/nistri.png' width='22' height='22' class='margin-right-5 bordered cursor-pointer'><img src='img/pattern/paper-xs.png' data-htmlbg-url='img/pattern/paper.png' width='22' height='22' class='bordered cursor-pointer'>";
        $("#smart-bgimages")
            .fadeOut();

        $('#demo-setting')
            .click(function () {
                //console.log('setting');
                $('.demo')
                    .toggleClass('activate');
            })
        
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
            //$.each(result.data, function (idx, val) {
            //    if (val.Detail !== null && val.Detail.length > 0) {
            //        html += "<li class=\"has-dropdown\"><a>" + val.MenuCaption +
            //                "</a>" + _this.loadNavMenuChild(val.Detail, id) + "</li>";
            //    }
            //    else {
            //        if ((val.MenuUrl || "").length > 0) {
            //            html += "<li><a href=\"#lnk/" + id + "/" + val.MenuUrl + "\">" + val.MenuCaption + "</a></li>";
            //        }
            //        else {
            //            html += "<li><a href=\"#lnk/" + id + "\">" + val.MenuCaption + "</a></li>";
            //        }
            //    }
            //});

            var accountBar = "<ul class='right'>";
            accountBar += "<li class='has-dropdown'>";
            accountBar += "<a>Account</a>";
            accountBar += "<ul class='dropdown'>";
            accountBar += "<li><a data-id='change-branch'>Change Branch</a></li>";
            accountBar += "<li><a data-id='change-password'>Change Password</a></li>";
            accountBar += "<li><a data-id='change-profit'>Change Profit Center</a></li>";
            accountBar += "</ul>";
            accountBar += "</li>";
            accountBar += "</ul>";

            html = "<div class=\"top-bar\"><div class=\"top-bar-section\"><ul>" + html + "</ul>" + accountBar + "</div></div>";
            nav.html(html);
            nav.foundation('topbar');
            _this.initializeCornerBarMenu();
            _this.initializeBranchCodeEvent();
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

function clickSideMenu() {
	if (!$.root_.hasClass("menu-on-top")) {
		$('html').toggleClass("hidden-menu-mobile-lock");
		$.root_.toggleClass("hidden-menu");
		$.root_.removeClass("minified");
		} else if ($.root_.hasClass("menu-on-top") && $(window).width() < 979) {
            $('html').toggleClass("hidden-menu-mobile-lock");
            $.root_.toggleClass("hidden-menu");
            $.root_.removeClass("minified");
		}
	$("#currentpage").trigger('layout');
    return false;
}

SimDms.Layout.prototype.loadUserInfo = function (user, branchCode, branchName, typepart, version) {
    var typepart = (typepart != undefined) ? typepart : "";
    typepart = (typepart != "") ? "&nbsp;&nbsp;|&nbsp; <b class=\"type-part\">" + typepart + "</b>" : typepart;
    var html = "login as : " + user + "&nbsp;|&nbsp;" +
               "<a href=\"" + SimDms.baseUrl + "account/logout" + "\">logout</a>&nbsp;|&nbsp;" +
               //"<a data-id='change-password'>change password</a>&nbsp;|&nbsp;" +
               //"<a data-id='change-branch'>change branch code</a>&nbsp;|&nbsp;" +
               "<a href='#' onclick='return clickSideMenu()'>menu</a>&nbsp;|&nbsp;" +
               "<a href=\"" + SimDms.baseUrl + "account/redirecttodoc" + "\" target='_blank'>documentation</a>" +
               "" + typepart+
                "&nbsp;&nbsp;|&nbsp; Version : " + version + " &nbsp; &nbsp;";

    //var myShortcutBtn = '<li>' +
	//					'<a href-void class="padding-10 padding-top-0 padding-bottom-0"' +
	//					'data-action="toggleShortcut"><i class="fa fa-arrow-down"></i> <u>S</u>hortcut</a>' +
	//				'</li>' +
	//				'<li class="divider"></li>' +
	//				'<li>' +
	//					'<a href-void class="padding-10 padding-top-0 padding-bottom-0"' +
	//					'data-action="launchFullscreen"><i class="fa fa-arrows-alt"></i> Full <u>S</u>creen</a>' +
	//				'</li>' +
	//				'<li class="divider"></li>' +
	//				'<li>' +
	//					'<a href="#/login" class="padding-10 padding-top-5 padding-bottom-5" data-action="userLogout">' +
    //                    '<i class="fa fa-sign-out fa-lg"></i> <strong><u>L</u>ogout</strong></a>' +
	//				'</li>';

    //console.log(myShortcutBtn);

    $(".page .header .user-info").html(html);
    $(".page .header .branch-info").html(branchCode + " - " + branchName);
    
    this.initializeCornerBarMenu();
}

SimDms.Layout.prototype.endsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

SimDms.Layout.prototype.autoMovePopup = function () {
    var _this = this;
    $(window).resize(function () {
        var popupContainer = $(".popup-frame.outer");
        var screenHeight = $(window).height();
        var screenWidth = $(window).width();
        //var popupHeight = SimDms.popupHeight;
        //var popupWidth = SimDms.popupWidth;   
        var popupWidth = _this.popupWidth;
        var popupHeight = _this.popupHeight;
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

$.fn.serializeObject = function () {
    var o = {};
    $(this).find('input[type="hidden"], input[type="text"], input[type="password"], input[type="checkbox"]:checked, input[type="radio"]:checked, select, textarea').each(function () {
        if ($(this).attr('type') == 'hidden') { //if checkbox is checked do not take the hidden field
            var $parent = $(this).parent();
            var $chb = $parent.find('input[type="checkbox"][name="' + this.name.replace(/\[/g, '\[').replace(/\]/g, '\]') + '"]');
            if ($chb != null) {
                if ($chb.prop('checked')) return;
            }
        }
        if (this.name === null || this.name === undefined || this.name === '') return;
        var elemValue = null;
        if ($(this).is('select')) {
            elemValue = $(this).find('option:selected').val();
        }
        else if ($(this).hasClass("number")) {
            elemValue = this.value.replace(",", "");
        }
        else {
            var type = $(this).data("type") || "";
            switch (type) {
                case "int":
                    elemValue = $(this).data("kendoNumericTextBox").value();
                    break;
                case "decimal":
                    elemValue = $(this).data("kendoNumericTextBox").value();
                    break;
                case "datepicker":
                    if (this.value.length > 0) {
                        elemValue = moment(this.value, SimDms.dateFormat).format("YYYY-MM-DD");
                    }
                    else {
                        elemValue = undefined;
                    }
                    break;
                case "kdatepicker":
                    if (this.value.length > 0) {
                        elemValue = moment(this.value, SimDms.dateFormat).format("YYYY-MM-DD");
                    }
                    else {
                        elemValue = undefined;
                    }
                    break;
                case "datetimepicker":
                    if (this.value.length > 0) {
                        elemValue = moment(this.value, SimDms.dateTimeFormat).format("YYYY-MM-DD HH:mm");
                    }
                    else {
                        elemValue = undefined;
                    }
                    break;
                case "kdatetimepicker":
                    if (this.value.length > 0) {
                        elemValue = moment(this.value, SimDms.dateTimeFormat).format("YYYY-MM-DD HH:mm");
                    }
                    else {
                        elemValue = undefined;
                    }
                    break;
                case "kmonthpicker":
                    if (this.value.length > 0) {
                        elemValue = moment(this.value, "MMMM YYYY").format("YYYYMM");
                    }
                    else {
                        elemValue = undefined;
                    }
                    break;
                default:
                    elemValue = this.value;
                    break;
            }
        }

        if (o[this.name] !== undefined && o[this.name] !== null) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(elemValue || '');
        } else {
            o[this.name] = elemValue;
        }
    });
    return o;
}



// INITIALIZE CUSTOM ELEMENT EVENT
SimDms.Layout.prototype.initializeCornerBarMenu = function () {
    var _this = this;
    var changePasswordCornerMenu = $("[data-id='change-password']");
    changePasswordCornerMenu.on("click", function (evt) {
        _this.evt_ChangePassword(evt);
    });
    _this.initializeBranchCodeEvent();
    _this.initializeProfitCenterEvent();
    _this.initializeTPGOEvent();
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
                var b =  $("form#formChangePassword").valid();

                //var validation = widget.validate("formChangePassword");


                if (b) {
                    widget.post(url, data, function (result) {
                        if (result.success) {
                            _this.hideCustomPopup();
                        }

                        widget.showNotification(result.message);
                    });
                } else {
                    console.log("validation failed");
                }
            });

            $("#btnCancelChangePassword").on("click", function (evt) {
                _this.hideCustomPopup();
            });
        }
    });
}

SimDms.Layout.prototype.evt_WindowResize = function (evt) {
    var _this = this;

    // RESIZING AND MOVING POPUP FRAME
    var overlay = $(".overlay");
    var popup = $(".popup-frame.outer");
    var windowHeight = $(window).height();
    var windowWidth = $(window).width();
    //var popupWidth = popup.width();
    //var popupHeight = popup.height();
    var popupWidth = _this.popupWidth;
    var popupHeight = _this.popupHeight;
    var marginLeft = (windowWidth - popupWidth) / 2;
    var marginTop = 100;

    popup.css({
        "position": "absolute",
        "left": marginLeft + "px",
        "top": marginTop + "px"
    });
}



// CUSTOM ELEMENT AREA 

SimDms.Layout.prototype.popupHeight = 0;
SimDms.Layout.prototype.popupWidth = 0;

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
        _this.popupHeight = popupHeight;
        _this.popupWidth = popupWidth;
        var marginTop = 0;
        var marginLeft = 0;

        if (options.customSize) {
            popupWidth = options.customSize[0];
            popupHeight = options.customSize[1];
            _this.popupHeight = popupHeight;
            _this.popupWidth = popupWidth;
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

        //console.log(body);
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
        if (params === undefined) return false;
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

SimDms.Layout.prototype.ajaxRequestCounter = 0;

SimDms.Layout.prototype.getAjaxRequestCounter = function () {
    return this.ajaxRequestCounter;
}

SimDms.Layout.prototype.incrementAjaxRequestCounter = function () {
    this.ajaxRequestCounter++;
}

SimDms.Layout.prototype.decrementAjaxRequestCounter = function () {
    this.ajaxRequestCounter--;
}

SimDms.Layout.prototype.post = function (url, arg1, arg2) {
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
                $(".page .ajax-loader").fadeOut();
            }
        });
    }
    if (arguments.length == 3) {
        $.post(SimDms.baseUrl + url, arg1, function (result) {
            arg2(result);
            if (_this.isNullOrEmpty(arg1.showAjax) == true || arg1.showAjax == false) {
                if (_this.getAjaxRequestCounter() - 1 <= 0) {
                    $(".page .ajax-loader").fadeOut();
                }
            }
            _this.decrementAjaxRequestCounter();
        });
    }
}


// CHANGE BRANCH CODE
SimDms.Layout.prototype.initializeBranchCodeEvent = function (params) {
    var _this = this;

    //Check if authenticated for changing branch code
    _this.post("gn.api/Role/IsChangeBranch", function (result) {
        if (!result.IsChangeBranchCode) {
            var userInfo = $(".user-info");
            var elem = $("[data-id='change-branch']");
            elem.remove();

            //var menus = userInfo.html();
            //menus = menus.replace("<a data-id=\"change-branch\">change branch code</a>&nbsp;|&nbsp;", "");              
            //userInfo.html(menus);
        }
        else {
            var menu = $("a[data-id='change-branch']");
            menu.on("click", function (evt) {
                var branches = "";
                var currentBranch = "";

                $.post("gn.api/User/CurrentBranch", function (result) {
                    currentBranch = result;

                    $.post("gn.api/Combo/Branches", function (result) {
                        $.each(result || [], function (key, val) {
                            if (val.value != currentBranch.CurrrentBranchCode) {
                                branches += "<option value='" + (val.value) + "'>" + (val.text || "") + "</option>";
                            }
                        });

                        var popupContent = "<form id='formChangeBranchCode' name='formChangeBranchCode'>" +
                                            "<div class='input-wrapper'><input type='text' readonly='true' required='false' type='password' name='CurrentBranch' id='CurrentBranch' class='' placeholder='CurrentBranch' value='" + ((currentBranch.CurrentBranchCode + " - " + currentBranch.CurrentBranchName) || "") + "' /></div>" +
                                            "<div class='input-wrapper'><select id='NextBranch' name='NextBranch' >" + branches + "</select></div>"
                        "</form>";

                        _this.showCustomPopup({
                            isCustomSize: true,
                            customSize: [500, null],
                            title: "Change Branch Code",
                            htmlContent: popupContent,
                            buttons: [
                                { name: "btnChangeBranchCode", text: "Change Branch", icon: "icon-save", cls: "small" },
                                { name: "btnCancelBranchCode", text: "Cancel", icon: "icon-refresh", cls: "small" },
                            ],
                            callback: function () {
                                $("#btnChangeBranchCode").on("click", function (evt) {
                                    var url = "gn.api/User/ChangeBranch";
                                    var params = $("[name=formChangeBranchCode]").serializeObject();

                                    $.post(url, params, function (result) {
                                        _this.showNotification(result.message || "");

                                        setTimeout(function () {
                                            if (result.status) {
                                                _this.hideCustomPopup();
                                                window.location.reload();
                                            }
                                        }, 1000);
                                    });
                                });

                                $("#btnCancelBranchCode").on("click", function (evt) {
                                    _this.hideCustomPopup();
                                });
                            }
                        });
                    });
                });
            });
        }
    });
}

// Change Profit Center
SimDms.Layout.prototype.initializeProfitCenterEvent = function (params) {
    var _this = this;
            var menu = $("a[data-id='change-profit']");
            menu.on("click", function (evt) {
                var prftcntr = "";
               
                $.get("breeze/sparepart/CurrentUserInfo", function (result) {
                    var currentPC = result.ProfitCenter;
                    var pcdesc = result.ProfitCenterName;
                    
                        $.post("gn.api/Combo/profitcenters", function (result) {
                        $.each(result || [], function (key, val) {                            
                                prftcntr += "<option value='" + (val.value) + "'>" + (val.text || "") + "</option>";                            
                        });

                        var popupContent = "<form id='formChangeProfitCenter' name='formChangeProfitCenter'>" +
                                            "<div class='input-wrapper'><input type='text' readonly='true' required='false' type='password' name='CurrentPC' id='CurrentBranch' class='' placeholder='Current Profit Center' value='" + (pcdesc || "") + "' /></div>" +
                                            "<div class='input-wrapper'><select id='NextBranch' name='NextProfitCenter' >" + prftcntr + "</select></div>"
                                            
                        "</form>";

                        _this.showCustomPopup({
                            isCustomSize: true,
                            customSize: [500, null],
                            title: "Change Profit Center",
                            htmlContent: popupContent,
                            buttons: [
                                { name: "btnChangeProfitCenter", text: "Change Profit Center", icon: "icon-save", cls: "small" },
                                { name: "btnCancelProfitCenter", text: "Cancel", icon: "icon-refresh", cls: "small" },
                            ],
                            callback: function () {
                                $("#btnChangeProfitCenter").on("click", function (evt) {
                                    var url = "gn.api/User/ChangeProfitCenter";
                                    var params = $("[name=formChangeProfitCenter]").serializeObject();

                                    $.post(url, params, function (result) {
                                        _this.showNotification(result.message || "");
                                        setTimeout(function () {
                                            if (result.success) {
                                                _this.hideCustomPopup();
                                                window.location.reload();
                                            }
                                        }, 1000);
                                    });
                                });

                                $("#btnCancelProfitCenter").on("click", function (evt) {
                                    _this.hideCustomPopup();
                                });
                            }
                        });
                    });
                });
            });
}

// Change Type of Part
SimDms.Layout.prototype.initializeTPGOEvent = function (params) {
    var _this = this;
    var menu = $("a[data-id='change-tpgo']");
    menu.on("click", function (evt) {
        var prftcntr = "";

        $.get("breeze/sparepart/CurrentUserInfo", function (result) {
            var currentTPGO = result.TypeOfGoods;
            var TPGOdesc = result.TypeOfGoodsName;

            $.post("gn.api/Combo/TypeOfGoods", function (result) {

                console.log(result);

                $.each(result || [], function (key, val) {
                    prftcntr += "<option value='" + (val.value) + "'>" + (val.text || "") + "</option>";
                });

                var popupContent = "<form id='formTPGOCenter' name='formTPGOCenter'>" +
                                    "<div class='input-wrapper'><input type='text' readonly='true' required='false' type='password' name='CurrentTPGO' id='CurrentTPGO' class='' placeholder='Current TPGO' value='" + (TPGOdesc || "") + "' /></div>" +
                                    "<div class='input-wrapper'><select id='NextTPGO' name='NextTPGO' >" + prftcntr + "</select></div>"

                "</form>";

                _this.showCustomPopup({
                    isCustomSize: true,
                    customSize: [500, null],
                    title: "Change Type Of Goods",
                    htmlContent: popupContent,
                    buttons: [
                        { name: "btnChangeTPGO", text: "Change Type Of Goods", icon: "icon-save", cls: "small" },
                        { name: "btnCancelTPGO", text: "Cancel", icon: "icon-refresh", cls: "small" },
                    ],
                    callback: function () {
                        $("#btnChangeTPGO").on("click", function (evt) {
                            var url = "gn.api/User/ChangeTPGO";
                            var params = $("[name=formTPGOCenter]").serializeObject();

                            $.post(url, params, function (result) {
                                _this.showNotification(result.message || "");
                                setTimeout(function () {
                                    if (result.success) {
                                        _this.hideCustomPopup();
                                        window.location.reload();
                                    }
                                }, 1000);
                            });
                        });

                        $("#btnCancelTPGO").on("click", function (evt) {
                            _this.hideCustomPopup();
                        });
                    }
                });
            });
        });
    });
}


SimDms.Layout.prototype.showNotification = function (msg) {
    if (msg !== undefined && msg.length > 0) {
        $(".page > .notif-wrapper > .notification").text(msg);
        $(".page > .notif-wrapper").fadeIn();
        setTimeout(function () { $(".page > .notif-wrapper").fadeOut(); }, 64000);
    }
}


var defaultNavigation = function()
{
    var html = "";
    if ( window.NodeMenusStore === undefined ) return html;

    $.each(window.NodeMenusStore, function (idx, val) {
        if ((val.MenuUrl || "").length > 0) {
            html += "<li><a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\" href=\"#lnk/" + window.moduleId + "/" + val.MenuUrl + "\">" + val.MenuCaption + "</a></li>";
        }
        else {
            html += "<li><a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\"  href=\"#lnk/" + window.moduleId + "/" + val.MenuId + "\">" + val.MenuCaption + "</a></li>";
        }   
    });

    var accountBar = "<ul class='right'>";
    accountBar += "<li class='has-dropdown'>";
    accountBar += "<a>Account</a>";
    accountBar += "<ul class='dropdown'>";
    accountBar += "<li><a data-id='change-branch'>Change Branch</a></li>";
    accountBar += "<li><a data-id='change-password'>Change Password</a></li>";    
    accountBar += "</ul>";
    accountBar += "</li>";
    accountBar += "</ul>";

    var ModuleMenu = "<ul>";
    ModuleMenu += "<li class='has-dropdown'>";
    ModuleMenu += "<a>" + window.moduleName + "</a>";
    ModuleMenu += "<ul class='dropdown'>";
    ModuleMenu += html;
    ModuleMenu += "</ul>";
    ModuleMenu += "</li>";
    ModuleMenu += "</ul>";

   return accountBar + ModuleMenu;
}


SimDms.Layout.prototype.loadHeaderNavMenuCtl = function () {
    "use strict";

    var url = SimDms.baseUrl + "layout/listmenu/" + window.moduleId;
    var nav = $(SimDms.selector + " .navmenu");
    var _this = this;

    var html = "";
    var thumb = "";

    var thumb_template = '<div class="thumbnail-item"><div class="thumbnail-glyph-wrap glyph-border-circle"><a href=\"{2}\"><div class="thumbnail-glyph {0}"></div></a></div><div class="thumbnail-text">{1}</div></div>';

    $.each(window.NodeMenusStore, function (idx, val) {
        var xicon = "";
        var slink = "";
        var Url = "";

        if (val.MenuIcon != null)
        {
            xicon = val.MenuIcon;
        }
        Url = "#lnk\/"+ window.moduleId  + "\/" ;
        if ((val.MenuUrl || "").length > 0) {
            Url +=  val.MenuUrl ;
            slink = "<a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\" href=\"#lnk\/" + window.moduleId + "\/" + val.MenuUrl + "\">" + val.MenuCaption + "</a>";
        }
        else {
            Url +=  val.MenuId ;
            slink = "<a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\"  href=\"#lnk\/" + window.moduleId + "\/" + val.MenuId + "\">" + val.MenuCaption + "</a>";
        }   

        html += "<li>" + slink + "</li>";

        thumb += thumb_template.replace("{0}",xicon).replace("{1}", slink).replace("{2}",Url);

    });

    var accountBar = "<ul class='right'>";
    accountBar += "<li class='has-dropdown'>";
    accountBar += "<a>Account</a>";
    accountBar += "<ul class='dropdown'>";
    accountBar += "<li><a data-id='change-branch'>Change Branch</a></li>";
    accountBar += "<li><a data-id='change-password'>Change Password</a></li>";    
    accountBar += "</ul>";
    accountBar += "</li>";
    accountBar += "</ul>";

    var ModuleMenu = "<ul>";
    ModuleMenu += "<li class='has-dropdown'>";
    ModuleMenu += "<a href=\"#lnk/" + window.moduleId + "\">" + window.moduleName + "</a>";
    ModuleMenu += "<ul class='dropdown'>";
    ModuleMenu += html;
    ModuleMenu += "</ul>";
    ModuleMenu += "</li>";
    ModuleMenu += "</ul>";

    html = "<div class=\"top-bar\"><div class=\"top-bar-section\">" +  accountBar + ModuleMenu +"</div></div>";

    nav.html(html);
    nav.foundation('topbar');
    _this.initializeCornerBarMenu();
    _this.initializeBranchCodeEvent();

    $('.has-dropdown').off();
    $('.has-dropdown').on('click', function () {
        $(this).find('.dropdown').hide();
    });
    $('.has-dropdown').on('mousemove', function () {
        $(this).find('.dropdown').fadeIn()
    });

    var thumb_body ='<div id="content-panel-body" class="x-panel-body x-panel-body-default x-panel-body-default x-noborder-trbl" style="overflow: auto; width: 100%; left: 0px; height: 100%; top: 46px;">';
        thumb_body += '<div id="content-panel-outerCt"  class="x-autocontainer-outerCt" style="width: 90%; height: 90%; margin:0 auto;"> ';
        thumb_body += '<div id="content-panel-innerCt" class="x-autocontainer-innerCt"style="width: 100%; height: 100%; margin:0 auto;">';
        thumb_body += '<div class="x-component  thumbnails" id="thumbnails-1028" tabindex="-1">';
        thumb_body += thumb + '</div></div></div></div>';

    //$(".page .body .main").slideUp();
    $(".page .title h3").html("&nbsp;").removeClass("fadeInLeft");
    $(".page .body .main").html(thumb_body);
    //$(".page .body .main").slideDown();

}


SimDms.Layout.prototype.loadHeaderNavMenu = function (id) {
    "use strict";    

    if (window.moduleId == id)
    {
        this.loadHeaderNavMenuCtl();
    }
    else
    {
        var _this = this;
        var url = SimDms.baseUrl + "layout/listmenu/" + id;
        $.post(url, function (result) {
            if (result.success) {

                window.NodeMenusStore = result.data;
                window.moduleName = result.moduleName;
                window.moduleId = id;

                _this.loadHeaderNavMenuCtl();
            }
        });
    }
}


SimDms.Layout.prototype.loadDynamicChildNavMenu = function (id) {
    "use strict";

    if (window.NodeMenusStore === undefined) return;

    var nav = $(SimDms.selector + " .navmenu");
    var _this = this;
    var html = "";
    var MainMenu = {};

    var n = window.NodeMenusStore.length;
    var thumb = "";
    var thumb_template = '<div class="thumbnail-item"><div class="thumbnail-glyph-wrap glyph-border-circle"><a href=\"{2}\"><div class="thumbnail-glyph {0}"></div></a></div><div class="thumbnail-text">{1}</div></div>';

    $.each(window.NodeMenusStore, function (idx, val) {

        if ( val.MenuId == id )
        {

            $.each(window.NodeMenusStore, function (idx, val) {
                if ((val.MenuUrl || "").length > 0) {
                    html += "<li><a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\" href=\"#lnk\/" + window.moduleId + "\/" + val.MenuUrl + "\">" + val.MenuCaption + "</a></li>";
                }
                else {
                    html += "<li><a data-id=\"" + val.MenuId + "\" route=\"" + idx + "\"  href=\"#lnk\/" + window.moduleId  + "\/" + val.MenuId + "\">" + val.MenuCaption + "</a></li>";
                }   
            });

            var accountBar = "<ul class='right'>";
            accountBar += "<li class='has-dropdown'>";
            accountBar += "<a>Account</a>";
            accountBar += "<ul class='dropdown'>";
            accountBar += "<li><a data-id='change-branch'>Change Branch</a></li>";
            accountBar += "<li><a data-id='change-password'>Change Password</a></li>";
            
            accountBar += "</ul>";
            accountBar += "</li>";
            accountBar += "</ul>";

            var ModuleMenu = "<ul>";
            ModuleMenu += "<li class='has-dropdown'>";
            ModuleMenu += "<a href=\"#lnk/" + window.moduleId + "\">" + window.moduleName + "</a>";
            ModuleMenu += "<ul class='dropdown'>";
            ModuleMenu += html;
            ModuleMenu += "</ul>";
            ModuleMenu += "</li>";
            ModuleMenu += "</ul>";

            MainMenu = val;
            html = "";

            $.each(MainMenu.Detail, function (idx, val) {
                var xicon = "";
                var slink = "";
                var Url = "";

                if (val.MenuIcon != null)
                {
                    xicon = val.MenuIcon;
                }
                Url = "#lnk\/" + window.moduleId + "\/" ;
                if ((val.MenuUrl || "").length > 0) {
                    Url += val.MenuUrl;
                    slink += "<a data-id=\"" + val.MenuId + "\"  href=\"#lnk\/" + window.moduleId + "\/" + val.MenuUrl + "\">" + val.MenuCaption + "</a>";
                }
                else {
                    Url +=  id + "\/" + val.MenuId;
                    slink += "<a data-id=\"" + val.MenuId + "\" href=\"#lnk\/" + window.moduleId + "\/" + id + "\/" + val.MenuId + "\">" + val.MenuCaption + "</a>";
                }      

                html += "<li>" + slink + "</li>";

                thumb += thumb_template.replace("{0}",xicon).replace("{1}", slink).replace("{2}",Url);       
            
            });

            ModuleMenu += "<ul>";
            ModuleMenu += "<li class='has-dropdown'>";
            ModuleMenu += "<a href=\"#lnk/" + window.moduleId + "/" + id + "\">" + MainMenu.MenuCaption + "</a>";
            ModuleMenu += "<ul class='dropdown'>";
            ModuleMenu += html;
            ModuleMenu += "</ul>";
            ModuleMenu += "</li>";
            ModuleMenu += "</ul>";

            html = "<div class=\"top-bar\"><div class=\"top-bar-section\">" +  accountBar  + ModuleMenu +"</div></div>";
            
            nav.html(html);
            nav.foundation('topbar');
            _this.initializeCornerBarMenu();
            _this.initializeBranchCodeEvent();

            $('.has-dropdown').off();
            $('.has-dropdown').on('click', function () {
                $(this).find('.dropdown').hide();
            });
            $('.has-dropdown').on('mousemove', function () {
                $(this).find('.dropdown').fadeIn()
            });           

            var thumb_body ='<div id="content-panel-body" class="x-panel-body x-panel-body-default x-panel-body-default x-noborder-trbl" style="overflow: auto; width: 100%; left: 0px; height: 100%; top: 46px;">';
               thumb_body += '<div id="content-panel-outerCt"  class="x-autocontainer-outerCt" style="width: 90%; height: 90%; margin:0 auto;"> ';
               thumb_body += '<div id="content-panel-innerCt" class="x-autocontainer-innerCt"style="width: 100%; height: 100%;">';
               thumb_body += '<div class="x-component  thumbnails" id="thumbnails-1028" tabindex="-1">';
            thumb_body += thumb + '</div></div></div></div>';

           // $(".page .body .main").slideUp();
            $(".page .title h3").html("&nbsp;").removeClass("fadeInLeft");
            $(".page .body .main").html(thumb_body);
            //$(".page .body .main").slideDown();

            return;
        }
    });




}
