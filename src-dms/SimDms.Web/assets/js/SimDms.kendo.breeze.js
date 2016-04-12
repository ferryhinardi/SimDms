SimDms.Widget.prototype.gridNumericFilter = function(element, options) {
  options = options || {format:'n0'}; // default 0 dec.
  element.kendoNumericTextBox(options);
};
	
SimDms.Widget.prototype.blookup = function (options, callback) {
    var _this = this;
    var _init = true;

    if ( options.multiSelect === undefined )
    {
        options.multiSelect = true;
    }

    if (typeof options.onSelected == "function") {
        var selector = (options.selector || ("#" + options.name));
        $(selector).on("click", showLookup);

        if (options.controller !== undefined) {
            var _url = options.controller.url;
            var _value = "#" + options.controller.value;
            var _text = "#" + options.controller.text;

            $(_value).on("change", function () {
                _this.post(_url, { id: $(this).val() }, function (result) {
                    $(_text).val(result.data);
                    if (!result.success) {
                        $(_value).val("");
                    }
                });
            });
        }
    }
    else {
        return showLookup();
    }
	
    function showLookup() {
        var kloolup = {
            kds: undefined,
            evtDblClick: [],
            evtOnCancel: [],
            evtOnExit:[],
            dblClick: function (callback) {
                this.evtDblClick.push(callback);
            },
            onCancel: function (callback) {
                this.evtOnCancel.push(callback);
            },
            onExit: function (callback) {
                this.evtOnExit.push(callback);
            }
        };
		
        var trandom = "";
        var possible = "abcdefghijklmnopqrstuvwxyz";
        for (var i = 0; i < 5; i++) trandom += possible.charAt(Math.floor(Math.random() * possible.length));

        var gridName = "bZgrid" + trandom;
        var params = {};
        if (options.params !== undefined) {
            if (options.params.name == "controls" && options.params.items !== undefined) {
                $.each(options.params.items, function (idx, val) {
                    params[val.param] = $("[name=" + val.name + "]").val();
                });
            }  else {
                for (var key in options.params) {
                    if (key.substr(0, 3) !== "flt") {
                        params[key] = options.params[key];
                    }
                }
            }
        }
        else {
            params = $("#" + gridName + ".kgrid").find(".k-filter").serializeObject();
        }

        if (_this.isNullOrEmpty(options.dynamicParams) == false) {
            $.each(options.dynamicParams || [], function (key, val) {
                if (_this.isNullOrEmpty(val.value)==false) {
                    params[(val.name || "")] = val.value;
                }
                else {
                    params[(val.name || "")] = $("[name='" + (val.element || "") + "']").val();
                }
            });
        }
        var template = "<div class=\"panel klookup\">" +
                       "<div class=\"header\">" +
                       "<div class=\"title\">" + options.title + "</div>" +
                       "<div class=\"buttons\">" +
                       "<button class=\"button small btn btn-info\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                       "</div>" +
                       "</div>" +
                       "<div class=\"content kgrid\">" +
                       "<div id=\"" + gridName + "\"></div>" +
                       "</div>" +
                       "<div class=\"footer\"><div class=\"buttons\">" +
                       "<button class=\"button small btn btn-success\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                       "<button class=\"button small btn btn-danger\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                       "</div></div>" +
                       "</div>";

        $(".body > .panel.lookup").empty();
        $(".body > .panel.lookup").html(template);
        $(".body > .panel").fadeIn("slow");

        function dataBound(e) {

            function hide() {
                $(".body > .panel").fadeOut();
                $(".body > .panel .kgrid").find(".k-filter input").val("");

            }

            function onCancel()
            {
                hide();
                if (kloolup.evtOnCancel.length) {
                    kloolup.evtOnCancel[0]("Close");
                }
            }
                  
            $(".body > .panel").find("#btnCancelPanel,#btnClosePanel").on("click", onCancel);
            
            var grid = $(".kgrid #" + gridName).data("kendoGrid");
            if (grid !== undefined) {
                var row = undefined;
                $(".k-grid-content tr").on("click", function () { row = $(this); });
                $(".k-grid-content tr").on("dblclick", selectData);
                $(".body > .panel").find("#btnSelectData").on("click", selectData);
            }

            function selectData() {
                if (row !== undefined) {
                    var data = grid.dataItem(row);
                    for (var i = 0; i < kloolup.evtDblClick.length; i++) {
                        kloolup.evtDblClick[i](data);
                        hide();
                    }

                    if (typeof options.onSelected == "function") {
                        options.onSelected(data);
                        hide();
                    }

                    if (options.onSelectedRows !== undefined) {
                        var rows = $(".kgrid #" + gridName + " tbody tr.k-state-selected");
                        var data = [];
                        $.each(rows, function (idx, val) {
                            data.push(grid.dataItem(val));
                        });
                        options.onSelectedRows(data);
                        hide();
                    }
                }
            }

            $(".kgrid #" + gridName).find(".k-filter input, .k-filter select").off();
           /* $(".kgrid #" + gridName).find(".k-filter input, .k-filter select").on("change", function () {
                $.extend(params, $(".kgrid #" + gridName).find(".k-filter").serializeObject());
                kloolup.kds.read();
            }); //*/
        }

        var schema = { model: { fields: {}}};

        $.each(options.columns, function (idx, val) {
            if (val.type !== undefined)
            {         
              schema.model.fields[val.field] = JSON.parse('{"type": "' + val.type + '"}'); 
            }
        });

        function errorRaise(e)
        {
            console.log(e);
        }

        kloolup.kds = new kendo.data.extensions.BreezeDataSource({
            entityManager: options.manager,
            endPoint: options.query,       
            schema: schema,          
            defaultSort: options.defaultSort,
            onFail: errorRaise
        });        

        $(".kgrid #" + gridName).empty();
        var oGrid = $(".kgrid #" + gridName).kendoGrid({
            dataSource: kloolup.kds,
            toolbar: generateFilters(options.filters),
            detailTemplate: options.detailTemplate,
            groupable: (options.groupable === undefined ? false : options.groupable),
            sortable: (options.sortable === undefined ? true : options.sortable),
            filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" } } } : options.filterable),
            pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 15, 25, 50, 100] , refresh: true} : options.pageable),
            dataBinding: (options.dataBinding || function () { }),
            selectable: ((options.multiSelect || false) ? "multiple" : false),
            columns: options.columns,
            detailInit: options.detailInit,
            resizable: (options.resizable === undefined ? false : options.resizable),
            dataBound: dataBound,
            navigatable: true,
            resizable: true
        });

        function generateFilters(filters) {
            var html = "";

            $.each(filters || [], function (idx, val) {
                var clss = (val.cls === undefined) ? "" : " class='" + val.cls + "'";
                var left = (val.left == undefined) ? "" : " style=\"padding-left:" + val.left + "px\"";
                switch (val.type) {
                    case "controls":
                        html += "<div" + clss + ">\n";
                        html += "<label>" + val.text + "</label>\n";
                        html += "<div" + left + "><div class=\"controls\">";
                        $.each(val.items || [], function () {
                            html += "<div class=\"" + (this.cls || "") + "\">\n";
                            html += generateItem(this);
                            html += "</div>\n";
                        });
                        html += "</div></div>\n";
                        html += "</div>\n";
                        break;
                    default:
                        html += "<div" + clss + ">\n";
                        html += "<label>" + val.text + "</label>\n";
                        html += "<div" + left + ">" + generateItem(val) + "</div>\n";
                        html += "</div>\n";
                        break;
                }
            });

            html = (html.length == 0) ? "" : "<div class=\"k-filter\">\n" + html + "</div>";
            return html;
        }

        function generateItem(item) {
            var html = "";
            var clss = (item.cls == undefined) ? "" : " class='" + item.cls + "'";
            var labl = (item.text == undefined) ? "" : "<label>" + item.text + "</label>\n";
            var plch = " placeHolder=\"" + (item.placeHolder || item.text || "") + "\"";
            var attr = " name=\"" + item.name + "\"" + plch;
            switch (item.type) {
                case "select":

                    var htmlOptions = "";
                    $.each(item.items || [], function (key, val) {
                        htmlOptions += "<option value='" + (val.value || "") + "'>" + (val.text || "") + "</option>";
                    });

                    html += "<select id='" + (item.name || "") + "' name='" + (item.name || "") + "'>" + htmlOptions + "</select>";

                    break;

                default:
                    html += "<input type=\"text\"" + attr + "></input>\n";
                    break;
            }
            return html;
        }

        return kloolup;
    }
}

SimDms.Widget.prototype.showForm = function (options, callback) {
    var _this = this;
    var _init = true;

    return showLookup();

    function showLookup() {

        var template = "<div class=\"panel klookup\"  style=\"height:90%;width:100%\">" +
                       "<iframe  frameborder=\"0\" style=\"overflow:hidden;height:100%;width:100%\" height=\"100%\" width=\"100%\"></iframe>" +
                       "<div style=\"height:15px;\"></div><div class=\"buttons\">" +
                       "<div class=\"button small btn btn-danger\" id=\"btnClosePanel\"> " +
                       "<i class=\"icon icon-hand-right\"></i>   Exit</div>" +
                       "</div></div>";

        var $div = $(template);

        var  HideForm = function()
          {
              $(".body > .panel").fadeOut();
          }

        $(".body > .panel.lookup").empty();
        $(".body > .panel.lookup").html($div);
        $(".body > .panel").fadeIn("slow");
        
        $("div#btnClosePanel").on("click", HideForm);

        //var popupLinker = options.compile($div)(options.scope); 
        // save Url to local database 
        localStorage.setItem("webFormUrl", options.url);
        localStorage.setItem("params", options.params || "");

        $(".panel iframe").attr("src", "Form");
        localStorage.setItem("CloseInterval", false);
        localStorage.setItem("RefreshGrid", false);

        var stop = setInterval(function () {
            var b = localStorage.getItem("CloseInterval");
            //console.log(b);
            if (b == "true") {
                $(".body > .panel").fadeOut();
                clearInterval(stop);
            }
        },0);
        //return popupLinker;
    }
}

SimDms.Widget.prototype.showFlatFile = function (options, callback) {
    var _this = this;
    var _init = true;

    return showLookup();

    function showLookup() {

        var template = "<div class=\"panel klookup\"  style=\"height:90%;width:100%\">" +
                       //"<div id=\"pnlViewData\" style=\"overflow:scroll;height:100%;width:100%;line-height: 17px; background-color:#DDDDDD\" height=\"100%\" width=\"100%\"></div>" +
                       "<textarea id=\"pnlViewData\" style=\"overflow:scroll;height:100%;width:100%;line-height: 17px; background-color:#DDDDDD\" height=\"100%\" width=\"100%\"></textarea>" +
                       "<div style=\"height:15px;\"></div><div class=\"buttons right\">" +
                        "<button class=\"button small btn btn-success\" id=\"btnSaveFile\" ng-click=\"PopupSaveFile()\" > " +
                        "<i class=\"icon icon-hand-right\"></i>   Save File</button>" +
                        "<button class=\"button small btn btn-info\" id=\"btnSendFile\" ng-click=\"PopupSendFile()\" > " +
                        "<i class=\"icon icon-hand-right\"></i>   Send File</button>" +
                        "<button class=\"button small btn btn-danger\" id=\"btnClosePanel\"> " +
                        "<i class=\"icon icon-hand-right\"></i>   Exit</button>" +
                       "</div></div>";

        var $div = $(template);

        var HideForm = function () {
            $(".body > .panel").fadeOut();
        }

        //var SavePopup = function () {
        //}

        //var SendPopup = function () {
        //}

        $(".body > .panel.lookup").empty();
        $(".body > .panel.lookup").html($div);
        $(".body > .panel").fadeIn("slow");
        $('div.buttons > button.button').css({ "margin-right": "8px" })
        $("button#btnClosePanel").on("click", HideForm);
        $("button#btnSaveFile").on("click", SavePopup);
        $("button#btnSendFile").on("click", SendPopup);

        var datas = options.data.split('\n');
        $.each(datas, function (key, value) {
            //console.log(key);
            //console.log(value);
            //$('div.panel > div#pnlViewData').append(value);
            $('div.panel > textarea#pnlViewData').append(value);
        });
        
        if(typeof callback=="function")
        {callback(); }
        //var popupLinker = options.compile($div)(options.scope); 

        // save Url to local database 
        //localStorage.setItem("webFormUrl", options.url);
        //console.log(options.url);
        //var urlRpt = window.reportUrl + "?id=" + options.id + "&pparam=" + options.pparam + "&rparam=" + options.rparam;

        //options.par = options.par || [];
        //var url = "Reports/Viewer.aspx?rpt=" + options.id;
        //var par = "&par=" + options.pparam;
        //var type = "&type=" + options.type;
        //var rparam = "&rparam=" + options.rparam;
        ////console.log(SimDms.baseUrl + url + par + type);
        ////redirect to 
        //$(".panel iframe").attr("src", SimDms.baseUrl + url + par + type);
    }
}

SimDms.Widget.prototype.showPdfReport = function (options, callback) {
        var _this = this;
        var _init = true;

        return showLookup();

       
        function showLookup() {
          
          

     
            var printtemplate = "<div class=\"button small btn btn-success\" id=\"btnDwnldRpt\"> " +
                                "<i class=\"icon icon-print\"></i> Print</div> " +
                                //"<div class=\"button small btn btn-success\" id=\"btnPrintRpt\"> " +
                                //"<i class=\"icon icon-print\"></i> Print</div> " +
                                //'<select placeholder="Tipe Service" style="width:150px;float:right" name="print"  id="printersrc"> ' +
                                //'<option value="\\172.16.101.122\EpsonLQ-2180">CutePDF</option> ' +
                                //'<option value="1">EPSONLQ-xxxx</option> ' +
                                //'<option value="2">CutePDF Writer</option> ' +
                                //'</select> ' +
                                '';

            var template = "<div class=\"panel klookup\"  style=\"height:90%;width:100%\">" +
                           "<iframe frameborder=\"0\" style=\"overflow:auto;height:100%;width:100%\" height=\"100%\" width=\"100%\"></iframe>" +
                           "<div style=\"height:15px;\"></div><div  class=\"buttons left\" style=\"width:440px;\">" +
                           "<div class=\"button small btn btn-danger\" id=\"btnClosePanel\"> " +
                           "<i class=\"icon icon-hand-right\"></i>   Exit</div> " +                           
                           (options.textprint != undefined ? (options.textprint == true ? printtemplate : "") : "") +
                             
                           "</div></div>";

            var $div = $(template);

            var HideForm = function () {
                $(".body > .panel").fadeOut();
            }

            var dlrpt = function downloadrpt() {
                console.log(window.location);
                var hst = window.location.origin;

                //hst += "/" + window.location.pathname.split('/')[1];  
                hst += "/" + window.location.pathname.replace('/Form', '').replace('/layout', '').replace('/', '');
                
                var url = hst + "/TextReport/DownloadReport?rpt=" + options.id + "&par=" + options.pparam + "&rparam=" + options.rparam;
                console.log(url);
                window.open(url);
                
            };

            var prntrpt = function printtxtreport() {                
                var hst = window.location.origin;
                //hst += "/" + window.location.pathname.split('/')[1];        
                hst += "/" + window.location.pathname.replace('/Form', '').replace('/', '');
                var url = hst + "/TextReport/PrintReport";
                //?rpt=" + options.id + "&par=" + options.pparam + "&rparam=" + options.rparam;
                
                
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { rpt: options.id, par: options.pparam, rparam: options.rparam, pnm: $("#printersrc").val() },
                    dataType: 'json',
                    success: function (rslt) {
                        if (!rslt.success) {
                            alert('Terjadi kesalahan \n' + rslt.msg);
                        }                        
                    }
                });

                console.log(url);
                //window.open(url);               
            }

            $(".body > .panel.lookup").empty();
            $(".body > .panel.lookup").html($div);
            $(".body > .panel").fadeIn("slow");

            $("div#btnClosePanel").on("click", HideForm);

            $("#btnDwnldRpt").on('click', dlrpt)
            $("#btnPrintRpt").on('click', prntrpt)

            //var popupLinker = options.compile($div)(options.scope); 
            // save Url to local database 
            localStorage.setItem("webFormUrl", options.url);
            console.log(options.url);
            var urlRpt = window.reportUrl + "?id=" + options.id + "&pparam=" + options.pparam + "&rparam=" + options.rparam;

            options.par = options.par || [];
            var url = "Reports/Viewer.aspx?rpt=" + options.id;
            var par = "&par=" + options.pparam;
            var type = "&type=" + options.type;
            var rparam = "&rparam=" + options.rparam;
            //console.log(SimDms.baseUrl + url + par + type);
            //redirect to 
            $(".panel iframe").attr("src", SimDms.baseUrl + url + par + type+rparam);
        }
    }
