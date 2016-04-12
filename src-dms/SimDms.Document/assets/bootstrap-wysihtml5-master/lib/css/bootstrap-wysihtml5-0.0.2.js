!function (a, b) {
    "use strict";
    var c = function (a, b) {
        var c = {
            "font-styles": "<li class='dropdown'><a class='btn dropdown-toggle' data-toggle='dropdown' href='#'><i class='icon-font'></i>&nbsp;<span class='current-font'>" + b.font_styles.normal + "</span>&nbsp;<b class='caret'></b>" + "</a>" + "<ul class='dropdown-menu'>" + "<li><a data-wysihtml5-command='formatBlock' data-wysihtml5-command-value='div'>" + b.font_styles.normal + "</a></li>" + "<li><a data-wysihtml5-command='formatBlock' data-wysihtml5-command-value='h1'>" + b.font_styles.h1 + "</a></li>" + "<li><a data-wysihtml5-command='formatBlock' data-wysihtml5-command-value='h2'>" + b.font_styles.h2 + "</a></li>" + "<li><a data-wysihtml5-command='formatBlock' data-wysihtml5-command-value='h3'>" + b.font_styles.h3 + "</a></li>" + "</ul>" + "</li>",
            emphasis: "<li><div class='btn-group'><a class='btn' data-wysihtml5-command='bold' title='CTRL+B'>" + b.emphasis.bold + "</a>" + "<a class='btn' data-wysihtml5-command='italic' title='CTRL+I'>" + b.emphasis.italic + "</a>" + "<a class='btn' data-wysihtml5-command='underline' title='CTRL+U'>" + b.emphasis.underline + "</a>" + "</div>" + "</li>",
            lists: "<li><div class='btn-group'><a class='btn' data-wysihtml5-command='insertUnorderedList' title='" + b.lists.unordered + "'><i class='icon-list'></i></a>" + "<a class='btn' data-wysihtml5-command='insertOrderedList' title='" + b.lists.ordered + "'><i class='icon-th-list'></i></a>" + "<a class='btn' data-wysihtml5-command='Outdent' title='" + b.lists.outdent + "'><i class='icon-indent-right'></i></a>" + "<a class='btn' data-wysihtml5-command='Indent' title='" + b.lists.indent + "'><i class='icon-indent-left'></i></a>" + "</div>" + "</li>",
            link: "<li><div class='bootstrap-wysihtml5-insert-link-modal modal hide fade'><div class='modal-header'><a class='close' data-dismiss='modal'>&times;</a><h3>" + b.link.insert + "</h3>" + "</div>" + "<div class='modal-body'>" + "<input value='http://' class='bootstrap-wysihtml5-insert-link-url input-xlarge'>" + "</div>" + "<div class='modal-footer'>" + "<a href='#' class='btn' data-dismiss='modal'>" + b.link.cancel + "</a>" + "<a href='#' class='btn btn-primary' data-dismiss='modal'>" + b.link.insert + "</a>" + "</div>" + "</div>" + "<a class='btn' data-wysihtml5-command='createLink' title='" + b.link.insert + "'><i class='icon-share'></i></a>" + "</li>",
            image: "<li>" + "<a class='btn add-image' title='" + b.image.insert + "'><i class='icon-picture'></i></a>" 
//                    + "<div class='bootstrap-wysihtml5-insert-image-modal modal hide fade'>"
//                    + "<div class='modal-header'><a class='close' data-dismiss='modal'>&times;</a>"
//                    + "<h3>" + b.image.insert + "</h3>" 
//                    + "</div>" + "<div class='modal-body'>"
//                    + "<input value='' type='text' class='bootstrap-wysihtml5-insert-image-url input-xlarge' placeholder='file name, max-width : 880px, png file'>  OR  <button class='btn upld' onclick='myfunctiontrig()'><i class='icon-cloud'></i> Upload from your local</button> <input type='file' style='display:none' class='uploaders'/>"
//                    + "</div>" + "<div class='modal-footer'>"
//                    + "<a href='#' class='btn' data-dismiss='modal'>" + b.image.cancel + "</a>"
//                    + "<a href='#' class='btn btn-primary' data-dismiss='modal'>" + b.image.insert
//                    + "</a>" + "</div>" + "</div>" + "<a class='btn' data-wysihtml5-command='insertImage' title='"
//                    + b.image.insert + "'><i class='icon-picture'></i></a>" 
                    + "</li>",
            html: "<li><div class='btn-group'><a class='btn' data-wysihtml5-action='change_view' title='" + b.html.edit + "'><i class='icon-pencil'></i></a>" + "</div>" + "</li>",
            color: "<li class='dropdown'><a class='btn dropdown-toggle' data-toggle='dropdown' href='#'><span class='current-color'>" + b.colours.black + "</span>&nbsp;<b class='caret'></b>" + "</a>" + "<ul class='dropdown-menu'>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='black'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='black'>" + b.colours.black + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='silver'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='silver'>" + b.colours.silver + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='gray'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='gray'>" + b.colours.gray + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='maroon'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='maroon'>" + b.colours.maroon + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='red'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='red'>" + b.colours.red + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='purple'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='purple'>" + b.colours.purple + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='green'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='green'>" + b.colours.green + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='olive'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='olive'>" + b.colours.olive + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='navy'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='navy'>" + b.colours.navy + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='blue'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='blue'>" + b.colours.blue + "</a></li>" + "<li><div class='wysihtml5-colors' data-wysihtml5-command-value='orange'></div><a class='wysihtml5-colors-title' data-wysihtml5-command='foreColor' data-wysihtml5-command-value='orange'>" + b.colours.orange + "</a></li>" + "</ul>" + "</li>"
        };
        return c[a]
    }, d = function (b, c) {
        this.el = b, this.toolbar = this.createToolbar(b, c || f), this.editor = this.createEditor(c), window.editor = this.editor, a("iframe.wysihtml5-sandbox").each(function (b, c) {
            a(c.contentWindow).off("focus.wysihtml5").on({
                "focus.wysihtml5": function () {
                    a("li.dropdown").removeClass("open")
                }
            })
        })
    };
    d.prototype = {
        constructor: d,
        createEditor: function (a) {
            a = a || {}, a.toolbar = this.toolbar[0];
            var c = new b.Editor(this.el[0], a);
            if (a && a.events) for (var d in a.events) c.on(d, a.events[d]);
            return c
        },
        createToolbar: function (b, d) {
            var e = this,
                h = a("<ul/>", {
                    "class": "wysihtml5-toolbar",
                    style: "display:none"
                }),
                i = d.locale || f.locale || "en";
            for (var j in f) {
                var k = !1;
                d[j] !== undefined ? d[j] === !0 && (k = !0) : k = f[j], k === !0 && (h.append(c(j, g[i])), j === "html" && this.initHtml(h), j === "link" && this.initInsertLink(h), j === "image" && this.initInsertImage(h))
            }
            if (d.toolbar) for (j in d.toolbar) h.append(d.toolbar[j]);
            return h.find("a[data-wysihtml5-command='formatBlock']").click(function (b) {
                var c = b.target || b.srcElement,
                    d = a(c);
                e.toolbar.find(".current-font").text(d.html())
            }), h.find("a[data-wysihtml5-command='foreColor']").click(function (b) {
                var c = b.target || b.srcElement,
                    d = a(c);
                e.toolbar.find(".current-color").text(d.html())
            }), this.el.before(h), h
        },
        initHtml: function (a) {
            var b = "a[data-wysihtml5-action='change_view']";
            a.find(b).click(function (c) {
                a.find("a.btn").not(b).toggleClass("disabled")
            })
        },
        initInsertImage: function (b) {
            var c = this,
                d = b.find(".bootstrap-wysihtml5-insert-image-modal"),
                e = d.find(".bootstrap-wysihtml5-insert-image-url"),
                f = d.find("a.btn-primary"),
                g = e.val(),
                h = function () {
                    var a = e.val();
                    e.val(g), c.editor.composer.commands.exec("insertImage", a)
                };
            e.keypress(function (a) {
                a.which == 13 && (h(), d.modal("hide"))
            }), f.click(h), d.on("shown", function () {
                e.focus()
            }), d.on("hide", function () {
                c.editor.currentView.element.focus()
            }), b.find("a[data-wysihtml5-command=insertImage]").click(function () {
                var b = a(this).hasClass("wysihtml5-command-active");
                return b ? !0 : (d.modal("show"), d.on("click.dismiss.modal", '[data-dismiss="modal"]', function (a) {
                    a.stopPropagation()
                }), !1)
            })
        },
        initInsertLink: function (b) {
            var c = this,
                d = b.find(".bootstrap-wysihtml5-insert-link-modal"),
                e = d.find(".bootstrap-wysihtml5-insert-link-url"),
                f = d.find("a.btn-primary"),
                g = e.val(),
                h = function () {
                    var a = e.val();
                    e.val(g), c.editor.composer.commands.exec("createLink", {
                        href: a,
                        target: "_blank",
                        rel: "nofollow"
                    })
                }, i = !1;
            e.keypress(function (a) {
                a.which == 13 && (h(), d.modal("hide"))
            }), f.click(h), d.on("shown", function () {
                e.focus()
            }), d.on("hide", function () {
                c.editor.currentView.element.focus()
            }), b.find("a[data-wysihtml5-command=createLink]").click(function () {
                var b = a(this).hasClass("wysihtml5-command-active");
                return b ? !0 : (d.appendTo("body").modal("show"), d.on("click.dismiss.modal", '[data-dismiss="modal"]', function (a) {
                    a.stopPropagation()
                }), !1)
            })
        }
    };
    var e = {
        resetDefaults: function () {
            a.fn.wysihtml5.defaultOptions = a.extend(!0, {}, a.fn.wysihtml5.defaultOptionsCache)
        },
        bypassDefaults: function (b) {
            return this.each(function () {
                var c = a(this);
                c.data("wysihtml5", new d(c, b))
            })
        },
        shallowExtend: function (b) {
            var c = a.extend({}, a.fn.wysihtml5.defaultOptions, b || {}),
                d = this;
            return e.bypassDefaults.apply(d, [c])
        },
        deepExtend: function (b) {
            var c = a.extend(!0, {}, a.fn.wysihtml5.defaultOptions, b || {}),
                d = this;
            return e.bypassDefaults.apply(d, [c])
        },
        init: function (a) {
            var b = this;
            return e.shallowExtend.apply(b, [a])
        }
    };
    a.fn.wysihtml5 = function (b) {
        if (e[b]) return e[b].apply(this, Array.prototype.slice.call(arguments, 1));
        if (typeof b == "object" || !b) return e.init.apply(this, arguments);
        a.error("Method " + b + " does not exist on jQuery.wysihtml5")
    }, a.fn.wysihtml5.Constructor = d;
    var f = a.fn.wysihtml5.defaultOptions = {
        "font-styles": !0,
        color: !1,
        emphasis: !0,
        lists: !0,
        html: !0,
        link: !1,
        image: !0,
        events: {},
        parserRules: {
            classes: {
                "wysiwyg-color-silver": 1,
                "wysiwyg-color-gray": 1,
                "wysiwyg-color-white": 1,
                "wysiwyg-color-maroon": 1,
                "wysiwyg-color-red": 1,
                "wysiwyg-color-purple": 1,
                "wysiwyg-color-fuchsia": 1,
                "wysiwyg-color-green": 1,
                "wysiwyg-color-lime": 1,
                "wysiwyg-color-olive": 1,
                "wysiwyg-color-yellow": 1,
                "wysiwyg-color-navy": 1,
                "wysiwyg-color-blue": 1,
                "wysiwyg-color-teal": 1,
                "wysiwyg-color-aqua": 1,
                "wysiwyg-color-orange": 1
            },
            tags: {
                b: {},
                i: {},
                br: {},
                ol: {},
                ul: {},
                li: {},
                h1: {},
                h2: {},
                h3: {},
                blockquote: {},
                u: 1,
                img: {
                    check_attributes: {
                        width: "numbers",
                        alt: "alt",
                        src: "url",
                        height: "numbers"
                    }
                },
                a: {
                    set_attributes: {
                        target: "_blank",
                        rel: "nofollow"
                    },
                    check_attributes: {
                        href: "url"
                    }
                },
                span: 1,
                div: 1
            }
        },
        stylesheets: ["./lib/css/wysiwyg-color.css"],
        locale: "en"
    };
    typeof a.fn.wysihtml5.defaultOptionsCache == "undefined" && (a.fn.wysihtml5.defaultOptionsCache = a.extend(!0, {}, a.fn.wysihtml5.defaultOptions));
    var g = a.fn.wysihtml5.locale = {
        en: {
            font_styles: {
                normal: "Normal text",
                h1: "Heading 1",
                h2: "Heading 2",
                h3: "Heading 3"
            },
            emphasis: {
                bold: "Bold",
                italic: "Italic",
                underline: "Underline"
            },
            lists: {
                unordered: "Unordered list",
                ordered: "Ordered list",
                outdent: "Outdent",
                indent: "Indent"
            },
            link: {
                insert: "Insert link",
                cancel: "Cancel"
            },
            image: {
                insert: "Insert image",
                cancel: "Cancel"
            },
            html: {
                edit: "Edit HTML"
            },
            colours: {
                black: "Black",
                silver: "Silver",
                gray: "Grey",
                maroon: "Maroon",
                red: "Red",
                purple: "Purple",
                green: "Green",
                olive: "Olive",
                navy: "Navy",
                blue: "Blue",
                orange: "Orange"
            }
        }
    }
} (window.jQuery, window.wysihtml5);


function myfunctiontrig() {
    //alert('Max image width 880px')
    //$('.uploaders').trigger('click');
    $('.close').click();
}