$(function () {

    //if ("onhashchange" in window) {

    //    window.lastfiledownload = "";

    //    function loadjscssfile(filename, filetype) {
    //        if (filetype == "js") { //if filename is a external JavaScript file
    //            var fileref = document.createElement('script')
    //            fileref.setAttribute("type", "text/javascript")
    //            fileref.setAttribute("src", filename)
    //        }
    //        else if (filetype == "css") { //if filename is an external CSS file
    //            var fileref = document.createElement("link")
    //            fileref.setAttribute("rel", "stylesheet")
    //            fileref.setAttribute("type", "text/css")
    //            fileref.setAttribute("href", filename)
    //        }
    //        if (typeof fileref != "undefined")
    //            document.getElementsByTagName("head")[0].appendChild(fileref)
    //    }

    //    function removejscssfile(filename, filetype) {
    //        var targetelement = (filetype == "js") ? "script" : (filetype == "css") ? "link" : "none" //determine element type to create nodelist from
    //        var targetattr = (filetype == "js") ? "src" : (filetype == "css") ? "href" : "none" //determine corresponding attribute to test for
    //        var allsuspects = document.getElementsByTagName(targetelement)
    //        for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
    //            if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) != null && allsuspects[i].getAttribute(targetattr).indexOf(filename) != -1)
    //                allsuspects[i].parentNode.removeChild(allsuspects[i]) //remove element by calling parentNode.removeChild()
    //        }
    //    }

    //    function locationHashChanged() {
    //        var id = location.hash.substring(1);
    //        var url = "api/files/templates/" + id;

    //        var myPageCtl = $('#content');

    //        myPageCtl.fadeOut(333, function () {
                                  
    //                myPageCtl.load(url + "/html", function () {

    //                    if (window.lastfiledownload != "") {
    //                        removejscssfile(window.lastfiledownload + "/js", "js");
    //                        removejscssfile(window.lastfiledownload + "/css", "css");
    //                    }

    //                    loadjscssfile(url + "/js", "js");
    //                    loadjscssfile(url + "/css", "css");

    //                    setup_widgets_desktop();
    //                    window.lastfiledownload = url;

    //                    myPageCtl.fadeIn(555);

    //                })
    //            })
            

    //        //$("#main").load(url + "&type=html", function () {
    //        //    if (window.lastfiledownload != "") {
    //        //        removejscssfile(window.lastfiledownload, "js");
    //        //        removejscssfile(window.lastfiledownload + "&type=css", "css");
    //        //    }
    //        //    loadjscssfile(url, "js");
    //        //    loadjscssfile(url + "&type=css", "css");
    //        //    window.lastfiledownload = url;

    //        //})
    //    }
    //    //window.onhashchange = locationHashChanged;
    //}

})