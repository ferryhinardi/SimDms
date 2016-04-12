(function () {
    
var oldHtml = "";
var oldCSS = "";
var oldJS = "";
var init = true;

fullscreenHTMLMode = false;
fullscreenCSSMode = false;
fullscreenJSMode = false;
fullscreenWGTMode = false;


var mixedMode = {
    name: "htmlmixed",
    scriptTypes: [{
        matches: /\/x-handlebars-template|\/x-mustache/i,
        mode: null
    },
                  {
                      matches: /(text|application)\/(x-)?vb(a|script)/i,
                      mode: "vbscript"
                  }]
};
    
var height = $(".page-footer").offset().top - 110;
$('#codeeditor').css('height', height);
$('#codeeditor').removeClass('layout-hide');

$('#ribbon').resize(function () {
    setTimeout(function () {
        var height = $(".page-footer").offset().top - 110;
        //$('#codeeditor').fadeOut();
        $('#codeeditor').css("height", height - ($('#ribbon').offset().left < 200 ? 4 : 0));
        //$('#codeeditor').fadeIn();
    }, 50)
});



window.html_editor = CodeMirror.fromTextArea($("#code-html textarea")[0], {
    lineNumbers: true,
    tabMode: "indent",
    mode: mixedMode,
    styleActiveLine: true,
    dragDrop: true,
    //lineWrapping: true,
    foldGutter: true,
    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter", "CodeMirror-lint-markers"],
    keyMap: "sublime",
    autoCloseBrackets: true,
    matchBrackets: true,
    showCursorWhenSelecting: true,
    theme: 'monokai',
    //lint: true
});



var htmlCompletion = new CodeCompletion(html_editor, new HtmlCompletion());
html_editor.setOption("onKeyEvent", function (cm, e) {
    console.log(e)
    return htmlCompletion.handleKeyEvent(cm, e);
});


$('[name=btntabs]').on('change', function (e) {
    var id = '#content-' + (e.currentTarget.id);
    $('.widget-group').hide();
    $(id).show();
})

window.css_editor = CodeMirror.fromTextArea($("#code-css textarea")[0], {
    lineNumbers: true,
    tabMode: "indent",
    styleActiveLine: true,
    keyMap: "sublime",
    autoCloseBrackets: true,
    matchBrackets: true,
    showCursorWhenSelecting: true,
    theme: 'monokai',
    dragDrop: true,
    //lineWrapping: true,
    mode: "css",
    foldGutter: true,
     
    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter", "CodeMirror-lint-markers"],
    //lint: true,
});

var cssCompletion = new CodeCompletion(css_editor, new CssCompletion());
css_editor.setOption("onKeyEvent", function (cm, e) {
    return cssCompletion.handleKeyEvent(cm, e);
});


window.cfg_editor = CodeMirror.fromTextArea($("#content-optData textarea")[0], {
    lineNumbers: true,
    tabMode: "indent",
    styleActiveLine: true,
    keyMap: "sublime",
    autoCloseBrackets: true,
    matchBrackets: true,
    showCursorWhenSelecting: true,
    dragDrop: true,
    //lineWrapping: true,
    extraKeys: {"Ctrl-Space": "autocomplete"},
    mode: {name: "javascript", globalVars: true},
    foldGutter: true,    
    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter", "CodeMirror-lint-markers"],
    //lint: true,
});


function passAndHint(cm) {
    setTimeout(function () { cm.execCommand("autocomplete"); }, 100);
    return CodeMirror.Pass;
}

function myHint(cm) {
    return CodeMirror.showHint(cm, CodeMirror.ternHint, { async: true });
}

CodeMirror.commands.autocomplete = function (cm) {
    CodeMirror.showHint(cm, myHint);
}
    
var server = new CodeMirror.TernServer();
    
window.js_editor = CodeMirror.fromTextArea($("#code-js textarea")[0], {
    lineNumbers: true,
    tabMode: "indent",
    styleActiveLine: true,
    keyMap: "sublime",
    autoCloseBrackets: true,
    matchBrackets: true,
    showCursorWhenSelecting: true,
    theme: 'monokai',
    dragDrop: true,
    //lineWrapping: true,
    extraKeys: {
        "'.'": passAndHint,
        "Ctrl-Space": function (cm) { server.complete(cm); },
        "Ctrl-I": function (cm) { server.showType(cm); },
        "Ctrl-O": function (cm) { server.showDocs(cm); },
        "Alt-.": function (cm) { server.jumpToDef(cm); },
        "Alt-,": function (cm) { server.jumpBack(cm); },
        "Ctrl-Q": function (cm) { server.rename(cm); },
        "Ctrl-.": function (cm) { server.selectName(cm); }
    },
    mode: {name: "javascript", globalVars: true},
    foldGutter: true,
     
    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter","CodeMirror-lint-markers"],
    lintWith: CodeMirror.javascriptValidator,
    textHover: { delay: 300 },
    onKeyEvent: function (e, s) {
        if (s.type == "keyup" && ((s.keyCode >= 65 && s.keyCode <= 90) || s.keyCode == 190))
        { CodeMirror.showHint(e, myHint); }
    },
    //lint: true,
});

window.js_editor.on("cursorActivity", function (cm) { server.updateArgHints(cm); });

html_editor.on("focus", function () {
    $(".html-logo").fadeOut("slow");
});
html_editor.on("blur", function () {
    $(".html-logo").fadeIn("slow");
});

html_editor.on("change", function (instance, change) {
    update();
});

css_editor.on("focus", function () {
    $(".css-logo").fadeOut("slow");
});
css_editor.on("blur", function () {
    $(".css-logo").fadeIn("slow");
});
css_editor.on("change", function (instance, change) {
    update();
});

js_editor.on("focus", function () {
    $(".js-logo").fadeOut("slow");
});
js_editor.on("blur", function () {
    $(".js-logo").fadeIn("slow");
});
js_editor.on("change", function (instance, change) {
    update();
});

    
// Full screen HTML
function fullscreenHTML() {
    toggleMaximize("west");
}

// Full screen CSS
function fullscreenCSS() {
    toggleMaximize("east");
}

// Full screen Widget
function fullscreenWGT() {
    toggleMaximize("south");
}

// Full screen JS
function fullscreenJS() {
    if (fullscreenJSMode)
    {
        window.theLayout.close("south", true, true);
        window.theLayout.close("east", true, true);
        window.theLayout.close("west", true, true);
    } else {
        window.theLayout.open("south", false, true);
        window.theLayout.open("east", false, true);
        window.theLayout.open("west", false, true);
    }
}

// If fullscreen HTML mode
$('#fullscreen-html-toggle').on('click', function () {
    if (fullscreenHTMLMode) {
        fullscreenHTMLMode = false;
        $('#fullscreen-html').removeClass('fullscreen-small-exit');
        $('#fullscreen-html').addClass('fullscreen-small');
        fullscreenHTML();
        $('.layout').fadeIn();
    }
    else {
        fullscreenHTML();
        fullscreenHTMLMode = true;
        $('#fullscreen-html').removeClass('fullscreen-small');
        $('#fullscreen-html').addClass('fullscreen-small-exit');
        $('.layout').fadeOut();
    }
});

// If fullscreen CSS mode
$('#fullscreen-css-toggle').on('click', function () {
    if (fullscreenCSSMode) {
        fullscreenCSSMode = false;
        $('#fullscreen-css').removeClass('fullscreen-small-exit');
        $('#fullscreen-css').addClass('fullscreen-small');
        fullscreenCSS();
        $('.layout').fadeIn();
    }
    else {
        fullscreenCSS();
        fullscreenCSSMode = true;
        $('#fullscreen-css').removeClass('fullscreen-small');
        $('#fullscreen-css').addClass('fullscreen-small-exit');
        $('.layout').fadeOut();
    }
});

// If fullscreen JS mode
$('#fullscreen-js-toggle').on('click', function () {
    if (fullscreenJSMode) {
        fullscreenJSMode = false;
        $('#fullscreen-js').removeClass('fullscreen-small-exit');
        $('#fullscreen-js').addClass('fullscreen-small');
        fullscreenJS();
        $('.layout').fadeIn();
    }
    else {
        fullscreenJSMode = true;
        fullscreenJS();
        $('#fullscreen-js').removeClass('fullscreen-small');
        $('#fullscreen-js').addClass('fullscreen-small-exit');
        $('.layout').fadeOut();
    }
});


// If fullscreen Widget mode
$('#fullscreen-wgt-toggle').on('click', function () {
    if (fullscreenWGTMode) {
        fullscreenWGTMode = false;
        $('#fullscreen-css').removeClass('fullscreen-small-exit');
        $('#fullscreen-css').addClass('fullscreen-small');
        fullscreenWGT();
        $('.layout').fadeIn();
    }
    else {
        fullscreenWGT();
        fullscreenWGTMode = true;
        $('#fullscreen-css').removeClass('fullscreen-small');
        $('#fullscreen-css').addClass('fullscreen-small-exit');
        $('.layout').fadeOut();
    }
});

function resetLayout() {

}

function update() {

}

function redrawEditors() {
    html_editor.refresh();
    css_editor.refresh();
    js_editor.refresh();
}

function toggleMaximize(paneName, cbPane) {
    var pane = cbPane || paneName, myLayout = window.theLayout,
    $Pane = myLayout.panes[pane]
, state = myLayout.state
, s = state[pane]
, container = state.container
, isMaximized = null
, panePaddingAndBorderHeight = s.outerHeight - s.css.height
, panePaddingAndBorderWidth = s.outerWidth - s.css.width
    ;
    if (pane === "north" || pane === "south") {
        if ($Pane.height() == s.css.height) {
            $Pane.css({
                height: container.innerHeight - panePaddingAndBorderHeight
            , zIndex: 3
            });
            isMaximized = true;
        }
        else { // RESET pane to what state says it *should be*
            $Pane.css({
                height: s.css.height
            , zIndex: 1
            });
            isMaximized = false;
        }
    }
    else if (pane === "east" || pane === "west") {
        if ($Pane.width() == s.css.width) {
            s.top = $Pane.css("top"); // save value | TODO: add top/bottom/left/right to state.pane.css data
            $Pane.css({
                //	need to also set top & height if want to cover north/south panes
                //	if only want to cover west-center-east panes, then DO NOT set top or height!
                top: container.insetTop
            , height: container.innerHeight - panePaddingAndBorderHeight
            , width: container.innerWidth - panePaddingAndBorderWidth
            , zIndex: 3
            });
            isMaximized = true;
        }
        else { // RESET pane to what state says it *should be*
            $Pane.css({
                top: s.top
            , height: s.css.height
            , width: s.css.width
            , zIndex: 1
            });
            isMaximized = false;
        }
    }

    // if no valid pane was passed, then exit now
    if (isMaximized === null) return;

    // set flags so can check a pane's state to see if it is 'maximized'
    s.maximized = isMaximized;
    // set var for use by onresizeall callback to re-maximize pane after window.resize
    container.maximizedPane = isMaximized ? pane : '';

    // OPTIONALLY show/hide all other panes in Layout
    for (var i = 0; i < 5; i++) {
        var name = $.layout.config.allPanes[i]
        , $P = myLayout.panes[name];
        if (!$P || name == pane) continue; // SKIP un/maximized pane
        if (isMaximized && $P.is(":visible")) {
            state[name].hiddenByMaximize = true; // set a state-flag
            $P.css("visibility", "hidden"); // make pane invisible
            if (name !== "center")
                myLayout.resizers[name].hide(); // ditto for its resizer-bar
        }
        else if (!isMaximized && state[name].hiddenByMaximize) {
            state[name].hiddenByMaximize = false; // clear flag
            $P.css("visibility", "visible"); // reset visibility
            if (name !== "center")
                myLayout.resizers[name].show(); // ditto for its resizer-bar
        }
    }

    // if maximized, add events to catch pane.close or resizeAll, which UN-maximize the pane
    if (isMaximized) {
        $Pane.bind("layoutpaneonclose_start.toggleMaximize", toggleMaximize)
            .bind("layoutpaneonresize_start.toggleMaximize", toggleMaximize);
        // TODO: pane.onresize is not reliably firing when layout resized
        //		try adding a callback to layoutonresize_start as well, pane = state.container.maximizedPane
    }
    else {
        // remove events (above) added when pane was maximized
        $Pane.unbind(".toggleMaximize");
        if (!cbPane) // skip if this is being called by runCallback() to avoid disrupting sequence
            myLayout.resizeAll();
    }
};

 
    
    window.theLayout = $('#codeeditor').layout();

    $.each($.layout.config.borderPanes, function (i, pane) {
        var o = theLayout.options[pane];
        o.livePaneResizing = !o.livePaneResizing;
    });

    $(document).on("mousedown", ".ui-layout-resizer", function () {
        $("<div class='victim_overlay'></div>").appendTo("#result");
    });

    $(document).on("mouseup", ".ui-layout-resizer", function () {
        $(".victim_overlay").remove();
        redrawEditors();
    });

    $(document).on("mouseup", ".ui-draggable-dragging", function () {
        $(".victim_overlay").remove();
        redrawEditors();
    });

    init = !init;

    $("#btn-load-schema").on("click", function (e) {

        var _filename = $('#search-fld').val();

        $.ajax({
            url: "appconfig/system/infodetail?id=" + _filename,
        })
          .done(function (data) {
              html_editor.setValue(data.form);              
              js_editor.setValue(data.column);
              cfg_editor.setValue(data.field);
              toastr["success"]("Load schema : " + _filename + " done !!!", "Success")
          });

    });

    $('#btn-save-editor').on('click', function (e) {

        var _html = html_editor.getValue();
        var _css = css_editor.getValue();
        var _js = js_editor.getValue();
        var _cfg = cfg_editor.getValue();

        var _filename = $('#search-fld').val();

        e.preventDefault();

        bootbox.confirm("Are you sure?", function (confirmed) {
            if (confirmed == true) {
                //bootbox.prompt("Enter your commit description or comment?", function (result) {
                //    if (result === null) {
                //        console.log("Prompt dismissed");
                //    } else {
                        var data = {
                            html: _html,
                            css: _css,
                            js: _js,
                            cfg: _cfg,
                            filename: _filename
                        }

                        $.ajax({
                            type: "POST",
                            url: "api/config/savecontent",
                            data: data
                        })
                          .done(function (msg) {
                              toastr["success"]( "Data has been saved with Id: " + msg,"Success")
                          });
                //    }
                //});
            }
        });

    });

    if ("onhashchange" in window) {        
        function locationHashChanged() {
            var id = location.hash.substring(1);
            $('#search-fld').val(id);
            loadCode();
            //var data = {
            //    filename: id
            //}
            //$.ajax({
            //    type: "GET",
            //    url: "appengine/loadcontent",
            //    data: data
            //})
            //.done(function (data) {
            //    js_editor.setValue("");
            //    css_editor.setValue("");
            //    cfg_editor.setValue("");
            //    html_editor.setValue("");
            //    if (data.success == true) {
            //        if (data.data !== null) {
            //            js_editor.setValue(data.data.js || "");
            //            css_editor.setValue(data.data.css || "");
            //            cfg_editor.setValue(data.data.cfg || "");
            //            html_editor.setValue(data.data.html || "");
            //        }
            //    }
            //});
        }
        window.onhashchange = locationHashChanged;
        window.theLayout.close("east", true, true);
        window.theLayout.close("south", true, true);
    }


    $('#search-fld').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            loadCode();
        }
    });


    var loadCode = function () {

        var _filename = $('#search-fld').val();

        var data = {
            filename: _filename
        }
        $.ajax({
            type: "GET",
            url: "appengine/loadcontent",
            data: data
        })
          .done(function (data) {
              js_editor.setValue("");
              css_editor.setValue("");
              cfg_editor.setValue("");
              html_editor.setValue("");
              if (data.success==true)
              {
                  if(data.data !== null)
                  {
                      js_editor.setValue(data.data.js || "");
                      css_editor.setValue(data.data.css || "");
                      cfg_editor.setValue(data.data.cfg || "");
                      html_editor.setValue(data.data.html || "");
                  }
              }
          });
    }


})()

