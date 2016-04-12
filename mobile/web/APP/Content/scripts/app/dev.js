$(document).ready(function () {

    var wi = $('#ribbon').width() + 22;
    $('#vertical').css("width", wi);
    var he = $(".page-footer").offset().top - 88;
    $('#vertical').css("height", he);

    //$('#vertical').css("width", wi);

    $('#ribbon').resize(function () {
        $('#vertical').css("width", $('#ribbon').width() + 22);
        //$('#vertical').css("width", $('#ribbon').width());
        
        setTimeout(function () {
            var height = $(".page-footer").offset().top - 88;
            var width = $('#ribbon').width() + 22;
            $('#vertical').css("width", width);
            //$('#vertical').css("width", wi);
            $('#vertical').css("height", height - ($('#ribbon').offset().left < 200 ? 4 : 0));
            //$('#vertical').css("height", wi);

        }, 50)
    });

    function onResize(e) {
        console.log("Resized :: Splitter <b>#" + this.element[0].id + "</b>");
        console.log(e);
        if (!(e.height==0 || e.width ==0))
        {
            if (this.element[0].id == 'horizontal')
            {
                var wi = $('#left-pane').width(), he = $('#left-pane').height();
               
                $('#wgleft.jarviswidget>div').css('width', wi - 2);
                $('#wgleft.jarviswidget>div').css('height', e.height - 42);
                $('#wg-left-body').css('width', wi - 2);
                $('#wg-left-body').css('height', e.height - 42);
                $('#left-pane').css('height', e.height - 6);
                $('#txtJS').css('width', wi - 40);
                $('#txtJS').css('height', e.height - 64);
                console.log(e.height, he, $('#wg-left-body').height())
            }
        }
    }
    
    //function onExpand(e) {
    //    console.log("Expanded :: Pane <b>#" + e.pane.id + "</b> from splitter <b>#" + this.element[0].id + "</b> expanded");
    //    console.log(e);
    //}

    //function onCollapse(e) {
    //    console.log("Collapsed :: Pane <b>#" + e.pane.id + "</b> from splitter <b>#" + this.element[0].id + "</b> collapsed");
    //    console.log(e);
    //}

    $("#vertical").kendoSplitter({
        orientation: "vertical",
        panes: [
            { collapsible: true, size: "75%" },
            { collapsible: true, min: "200px", max: "300px", size: "25%" }
        ],
        //expand: onExpand,
        //collapse: onCollapse,
        //resize: onResize
    });

    $("#horizontal").kendoSplitter({
        panes: [
            { collapsible: true },
            { collapsible: true },
            { collapsible: true }
        ],
        //expand: onExpand,
        //collapse: onCollapse,
        //resize: onResize
    });


    $('#left-pane').on('resize', function () {
        setTimeout(function () {
            var wi = $('#left-pane').width(), he = $('#left-pane').height();
            if (!(wi==0 || he==0))
            {
                editor.setSize(wi - 6,he -6)
            }
        }, 100);
    });
    $('#center-pane').on('resize', function () {
        setTimeout(function () {
            var wi = $('#center-pane').width(), he = $('#center-pane').height();
            if (!(wi == 0 || he == 0)) {
                editor2.setSize(wi - 6, he - 6)
            }
        }, 100);
    });

    $('#right-pane').on('resize', function () {
        setTimeout(function () {
            var wi = $('#right-pane').width(), he = $('#right-pane').height();
            if (!(wi == 0 || he == 0)) {
                editor3.setSize(wi - 6, he - 6)
            }
        }, 100);
    });    $('#left-pane').trigger('resize');    $('#center-pane').trigger('resize');    $('#right-pane').trigger('resize');


    function JsDevCommit(id)
    {
        alert('Commit: ' + id)
    }

    
    function JsDevRollback(id)
    {
        alert('Rollback: ' + id)
    }

    
    function JsDevDiff(id)
    {
        alert('Diff: ' + id)
    }

    
    function JsDevLog(id)
    {
        alert('Log: ' + id)
    }




});