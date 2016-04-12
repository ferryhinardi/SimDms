var Sdms = {
    //baseApi: 'http://dms.suzuki.co.id/simdms/MobileSpr/',
    baseApi: 'http://tbsdmsap01/simdms/MobileSpr/',
    //baseApi: 'http://localhost:7701/MobileSpr/',
};
Sdms.Mobile = function (options) {
    var self = this;
    this.options = options;
    this.cordova = undefined;
    $(document).ready(function () { self.render() });
}

Sdms.Mobile.prototype.render = function () {
    var self = this;
    var storage = self.storage;
    var options = self.options;
    var html = '';

    async.eachSeries((options.pages || []), renderPage, function (err) {
        if (err) throw err;

        html += '<div class="shortcut"></div>';       // render shortcut
        html += '<div class="popup"></div>';          // render popup
        html += '<div class="pageblock"></div>';      // render pageblock
        html += '<div class="ajaxloader"><i class="fa fa-cog fa-spin"/></i></div>';     // render ajaxloader

        $('body').html(html);
        setTimeout(function () {
            $('button').on('click', function (e) {
                e.preventDefault();
            });
            $('.page [data-action=back]').on('click', function (e) {
                e.preventDefault();
                var nav = $(this).data('back');
                self.navigate(nav, { back: true });
                self.clearfix();
            });
            $('.page [data-action=shortcut]').on('click', function (e) {
                self.shortcut();
            });
        }, 1000);

        if (window.cordova) {
            document.addEventListener('deviceready', function () {
                self.cordova = window.cordova;
                if (options.onRendered) options.onRendered();
            }, false);
        }
        else {
            if (options.onRendered) options.onRendered();
        }
    });

    function renderPage(page, callback) {
        if (page.type == 'login') renderLogin(page);
        if (page.type == 'form') renderForm(page);
        if (page.type == 'nav') renderNav(page);
        if (callback) callback();
    }

    function renderHeader(page) {
        html += ' <div class="header">';
        html += '  <div class="toolbar">';
        html += ((page.back) ? ('<i class="fa fa-chevron-left" data-action="back" data-back="' + page.back + '"></i>') : '');
        html += '  </div>';
        html += '  <div class="toolbar right">';
        html += '   <i class="fa fa-ellipsis-v" data-action="shortcut"></i>';
        html += '  </div>';
        html += '  <div class="title">';
        html += '   <div class="title-brand">' + (options.brand || page.brand) + '</div>';
        html += '   <div class="title-page">' + page.title + '</div>';
        html += '  </div>';
        html += ' </div>';
    }

    function renderFooter(page) {
        if (page.footer) {
            html += ' <div class="footer">';
            (page.footer.toolbars || []).forEach(function (toolbar) {
                html += '<div class="footer-bar" data-action="' + toolbar.action + '">';
                html += ' <i class="fa ' + toolbar.icon + '"></i>';
                html += ' <a href="#">' + toolbar.text + '</a>';
                html += '</div>';
            });
            html += ' </div>';
        }
    }

    function renderLogin(page) {
        html += '<div class="page login" data-name="' + page.name + '">';
        html += ' <div class="header">';
        html += '  <div class="toolbar"></div>';
        html += '  <div class="toolbar right"></div>';
        html += '  <div class="title">';
        html += '   <div class="title-brand">' + (options.brand || '') + '</div>';
        html += '   <div class="title-page">Sparepart Mobile</div>';
        html += '  </div>';
        html += ' </div>';
        html += ' <div class="logo">';
        html += '  <img src="img/logo.jpg" alt="" />';
        html += ' </div>';
        html += ' <div class="form-wrapper">';
        html += '  <form role="form">';
        html += '   <button class="btn btn-primary btn-block" data-action="register"><i class="fa fa-sign-in"></i>&nbsp;Register</button>';
        html += '   <button class="btn btn-primary btn-block" data-action="exit"><i class="fa fa-sign-out"></i>&nbsp;Exit</button>';
        html += '  </form>';
        html += ' </div>';
        html += '</div>\n';
    }

    function renderForm(page) {
        html += '<div class="page" data-name="' + page.name + '">';
        renderHeader(page);
        html += ' <div class="content nav animated ' + ((page.footer) ? 'hasfooter' : '') + '"><div class="title"></div><div class="form"></div></div>';
        renderFooter(page);
        html += '</div>\n';
    }

    function renderNav(page) {
        html += '<div class="page overthrow" data-name="' + page.name + '">';
        renderHeader(page);
        html += ' <div class="content nav animated"><div class="title"></div><div class="list"></div></div>';
        renderFooter(page);
        html += '</div>\n';
    }
}

Sdms.Mobile.prototype.navigate = function (name, options) {
    if ((name || '').length < 1) return;

    var self = this;
    var page1 = $('.page.active');
    var page2 = $('.page[data-name=' + name + ']');

    if (page1.data.name !== name) {
        page1.removeClass('active');
        page2.addClass('active');

        if (options && options.back) {
            if (self.options.onPageChanged) {
                self.options.onPageChanged(name, { back: true });
            }
        }
        else {
            if (self.options.onPageChanged) {
                self.options.onPageChanged(name);
            }
        }
    }
    this.clearfix();
}

Sdms.Mobile.prototype.clearfix = function () {
    var shortcut = $('.shortcut');

    if (shortcut.hasClass('active')) {
        shortcut.removeClass('active');
    }
}

Sdms.Mobile.prototype.shortcut = function () {
    var self = this;
    var shortcut = $('.shortcut');

    if (shortcut.hasClass('active')) {
        shortcut.removeClass('active');
    }
    else {
        shortcut.addClass('active');

        if (shortcut.children().length == 0) {
            var html = '';
            html += '<a href="#" data-action="clrdlr">Clear Dealer</a>';
            html += '<a href="#" data-action="chgbrc">Change Branch</a>';
            shortcut.append(html);

            // adding event on click, link to action
            $('.shortcut a').on('click', function (e) {
                e.preventDefault();
                var action = $(this).data('action');
                switch (action) {
                    case 'clrdlr':
                        self.storage.remove('sdms.user');
                        window.location.reload();
                        break;
                    case 'chgbrc':
                        var pageName = $('.page.active').data('name');
                        if (pageName == 'branch') {
                            self.clearfix();
                        }
                        else {
                            self.navigate('branch');
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }
}

Sdms.Mobile.prototype.signout = function () {
    window.location.reload();
}

// update dom
Sdms.Mobile.prototype.list = function (options) {
    var self = this;
    var html = '';

    switch (options.type || 'nav') {
        case 'list':
            html += '<div class="form-list">';
            (options.data || []).forEach(function (row) {
                html += '<div class="list-group">';
                html += ' <a href="#" class="list-group-item" data-action="' + (options.action || row.name) + '" data-field="' + row.name + '">';
                html += '  <h4 class="list-group-item-heading">' + row.text + '</h4>';
                if (row.subtext) { html += '  <p>' + row.subtext + '</p>'; }
                html += ' </a>';
                html += '</div>';
            });
            html += '</div>';
            break;
        default:
            (options.data || []).forEach(function (row) {
                var page = (row.page) ? ('data-page="' + row.page + '"') : '';
                html += '<div class="row" data-action="' + row.name + '" ' + page + '>';
                html += '  <div class="list-title">' + (row.text) + '</div>';
                html += '  <div class="list-subtitle">' + (row.subtext || '&nbsp;') + '</div>';
                if (row.info) {
                    for (var i = 0; i < (row.info || []).length  ; i++) {
                        html += '  <div class="list-info' + (i + 1).toString() + '">' + (row.info[i] || '') + '</div>';
                    }
                }
                html += '</div>';
            });
            break;
    }
    $(options.selector).html(html);
}

Sdms.Mobile.prototype.select = function (options) {
    var self = this;
    var html = '';
    (options.data || []).forEach(function (row) {
        html += '<option value="' + row.name + '">' + row.text + '<option>';
    });
    $(options.selector).html(html);
}

// event
Sdms.Mobile.prototype.onclick = function (page, action, script) {
    $('[data-name=' + page + '] [data-action=' + action + ']').off('click');
    $('[data-name=' + page + '] [data-action=' + action + ']').on('click', script);
}

// replace localStorage
Sdms.Mobile.prototype.storage = {
    set: function (key, value) {
        if (Object.prototype.toString.call(value) == '[object Array]') {
            var realValue = [];
            (value || []).forEach(function (val) {
                if (val) realValue.push(val);
            });
            localStorage.setItem(key, JSON.stringify(realValue));
            value = realValue;
        }
        else {
            localStorage.setItem(key, JSON.stringify(value));
        }

        return value;
    },
    get: function (key) {
        var value = JSON.parse(localStorage.getItem(key));
        if (Object.prototype.toString.call(value) == '[object Array]') {
            var realValue = [];
            (value || []).forEach(function (val) {
                if (val) realValue.push(val);
            });
            return realValue;
        }
        else {
            return value;
        }
    },
    remove: function (key) {
        localStorage.removeItem(key);
    },
    update: function (key, value) {
        var data = this.get(key);
        for (var name in value) {
            data[name] = value[name];
        }
        this.set(key, data);
    },
    clear: function () {
        localStorage.clear();
    }
}

Sdms.Mobile.prototype.post = function (options) {
    var self = this;
    $.ajax({
        type: 'POST',
        url: Sdms.baseApi + options.url,
        crossDomain: true,
        dataType: 'jsonp',
        success: function (data, status, xhr) {
            if (options.key) {
                self.storage.set(options.key, data);
            }
            if (options.success) options.success(data, status, xhr);
        },
        error: function (res, status, err) {
            if (options.error) options.error(res, status, err);

            console.log(JSON.stringify(res));
            alert(Sdms.baseApi + options.url);
            alert(JSON.stringify(res));
        }
    });

    if (this.cordova) {
        console.log(Sdms.baseApi + options.url)
    }
}

Sdms.Mobile.prototype.postData = function (options) {
    var self = this;
    var user = self.storage.get('sdms.user');
    var baseApi = user.ApiUrl;
    var url = baseApi + options.url;

    $.ajax({
        type: 'POST',
        url: url,
        crossDomain: true,
        data: options.data,
        dataType: 'jsonp',
        success: function (data, status, xhr) {
            if (options.key) {
                self.storage.set(options.key, data);
            }
            if (options.success) options.success(data, status, xhr);
        },
        error: function (res, status, err) {
            if (options.error) options.error(res, status, err);

            console.log(JSON.stringify(res));
            alert(Sdms.baseApi + options.url);
            alert(JSON.stringify(res));
        }
    });

    if (this.cordova) {
        console.log(Sdms.baseApi + options.url)
    }
}

Sdms.Mobile.prototype.alert = function (message) {
    alert(message);
}

Sdms.Mobile.prototype.confirm = function (options) {
    var r = confirm(options.message);
    if (r == true) {
        if (options.onOK) options.onOK();
        if (options.onOk) options.onOk();
    }
    else {
        if (options.onCancel) options.onCancel();
    }
}

Sdms.Mobile.prototype.focus = function (page, field) {
    $('[data-name=' + page + '] [data-field=' + field + ']').focus();
}

Sdms.Mobile.prototype.writeTitle = function () {
    var user = mobile.storage.get('sdms.user');
    if (!user) {
        mobile.navigate('register');
        return;
    }

    if (!user.Outlet) {
        mobile.navigate('branch');
        return;
    }

    $('.page.active').find('.content .title').text(user.Outlet.OutletName);
}

Sdms.Mobile.prototype.showProcess = function () {
    $('.pageblock,.ajaxloader').addClass('active');
}

Sdms.Mobile.prototype.hideProcess = function () {
    $('.pageblock,.ajaxloader').removeClass('active');
}