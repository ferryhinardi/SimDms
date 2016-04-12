var Sdms = {
    baseUrl: '/',
    baseApi: 'http://tbsdmsap01/SimDms/',
    settings: {}
}

Sdms.Layout = function (options) {
    var self = this;

    function initialize() {
        if (options && options.onInitialized) {
            options.onInitialized();
        }

        $(window).on('hashchange', function () {
            if (options && options.onStateChanged) {
                options.onStateChanged(self, (window.location.hash || '#login').substr(1));
            }
        });

        options.onStateChanged(self, (window.location.hash || '#login').substr(1));
    }

    this.state = {};

    setTimeout(function () {
        initialize();
    }, 400);
}

Sdms.Mobile = function (options) {
    var self = this;

    function initialize() {
        var html = '';

        (options.pages || []).forEach(function (page) {
            switch (page.type) {
                case 'login':
                    html += '<div id="' + page.name + '" class="page hide">';
                    html += ' <div class="header">';
                    html += '  <div class="toolbar"></div>';
                    html += '  <div class="toolbar right"></div>';
                    html += '  <div class="title">';
                    html += '   <div class="title-brand">Sdms Mobile</div>';
                    html += '   <div class="title-page">Login</div>';
                    html += '  </div>';
                    html += ' </div>';
                    html += ' <div class="logo">';
                    html += '  <img src="assets/img/logo.jpg" alt="" />';
                    html += ' </div>';
                    html += ' <div class="form-wrapper">';
                    html += '  <form role="form">';
                    html += '   <div class="form-group">';
                    html += '    <input type="email" class="form-control" name="UserName" placeholder="Enter User Name">';
                    html += '    <input type="password" class="form-control" name="Password" placeholder="Password">';
                    html += '   </div>';
                    html += '   <button type="submit" class="btn btn-primary btn-block" id="btnLogin">Login</button>';
                    html += '  </form>';
                    html += ' </div>';
                    html += '</div>';

                    break;
                case 'page':
                    html += '<div id="' + page.name + '" class="page hide">';
                    html += ' <div class="header">';
                    if (page.toolbars) {
                        (page.toolbars).forEach(function (toolbar) {
                            switch (toolbar) {
                                case 'back':
                                    html += '  <div class="toolbar">';
                                    html += '   <i class="icon fa fa-angle-left left back"></i>';
                                    html += '  </div>';
                                    break;
                            }
                        });
                    }
                    else {
                        html += '  <div class="toolbar blank"></div>';
                    }
                    html += '  <div class="toolbar right">';
                    html += '   <i class="icon fa fa-ellipsis-v shortcut"></i>';
                    html += '  </div>';
                    html += '  <div class="title">';
                    html += '   <div class="title-brand">Sdms Mobile</div>';
                    html += '   <div class="title-page">' + page.title + '</div>';
                    html += '  </div>';
                    html += ' </div>';
                    html += ' <div class="content">';

                    if (page.listTitle !== undefined) {
                        html += '  <div class="title"><i class="' + resolveIconTag(page.icon) + '"></i>' + page.listTitle + '</div>';
                    }

                    html += '  <div class="list"></div>';
                    html += ' </div>';
                    html += '</div>';
                    break;
            }
        });

        $('#main').html(html);

        if (options.onInitialized) options.onInitialized(self);
    }

    initialize();

    function resolveIconTag(icon) {
        var icon = (icon || 'fa-tag');
        var icon_tag = ((icon.substr(0, 5) == 'glyph') ? 'glyphicon ' : 'fa ') + icon;
        return icon_tag;
    }
}

Sdms.Mobile.prototype.initBehaviour = function () {
    $('#shortcut a').on('click', function (e) {
        e.preventDefault();

        var hash = $(this).attr('href').substr(1);
        if (hash == 'logout') {
            logOut();
        }
    });
    $('#main .toolbar > .shortcut').on('click', function (e) {
        e.preventDefault();
        if ($('#shortcut').hasClass('shown')) {
            $('#shortcut').slideUp('fast');
            $('#shortcut').removeClass('shown');
        }
        else {
            $('#shortcut').slideDown('fast');
            $('#shortcut').addClass('shown');
        }
    });
}
