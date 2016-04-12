webix.protoUI({
  name:"wxtable", 
  $init:function(config){
      config.minHeight = config.minHeight === undefined ? 40 : config.minHeight;
      config.select = "row";
      config.scrollX = config.scrollX === undefined ? false : config.scrollX;
      config.scrollY = config.scrollY === undefined ? false : config.scrollY;
      config.autoheight = config.autoHeight === undefined ? true : config.autoHeight;
      config.resizeColumn= true; 
      config.navigation = true;
      config.height = config.height === undefined ? 0 : config.height;
  }
}, webix.ui.datatable);

function id_helper($element){
  //we need uniq id as reference
  var id = $element.attr("id");
  if (!id){
    id = webix.uid();
    $element.attr("id", id);
  }
  return id;
}

angular.module("webix", [])
  .directive('webixResize', [ "$parse", function($parse) {
    return {
      restrict: 'A',
      scope: false,
      link:function ($scope, $element, $attrs, $controller){
        var attr = $attrs["webixResize"];
        var id = id_helper($element);
        var view = webix.$$(id);
        if (view){
          view.define("resizeColumn", attr);
          view.refresh();
        }
      }
    };
}]);


// Module for broadcasting the following window events
// Written by Shaun Grady. Licensed under The MIT License (MIT).
// https://github.com/shaungrady/angular-window-events
// Version 0.1
angular.module('windowEventBroadcasts', []).
run(['$rootScope', '$window', '$document', function($rootScope, $window, $document) {
  var window = angular.element($window),
      vendorPrefix, prevEvent;

  // We'll let jQuery/jqLite handle cross-browser compatibility with window blur/focus
  // Blur events can be double-fired, so we'll filter those out with prevEvent tracking
  window.on('blur', function(event) {
    if (prevEvent !== 'blur')
        $rootScope.$broadcast('$windowBlur', event);
    prevEvent = 'blur';
  });

  window.on('focus', function(event) {
    if (prevEvent !== 'focus')
      $rootScope.$broadcast('$windowFocus', event);
    prevEvent = 'focus';
  });

  // With document visibility, we'll have to handle cross-browser compatibility ourselves
  // Compatibility: IE10+, FF10+, Chrome 14+, Safari 6.1+, Opera 12.1+, iOS Safari 7+
  // For more detailed compatibility statistics: http://caniuse.com/#feat=pagevisibility
  // Inspired by http://stackoverflow.com/q/1060008
  var visibilityChangeHandler = function visibilityChangeHandler(event) {
    if (this[vendorPrefix ? vendorPrefix + 'Hidden' : 'hidden'])
      $rootScope.$broadcast('$windowHide', event);
    else
      $rootScope.$broadcast('$windowShow', event);
  };

  // Determine if a vendor prefix is required to utilize the Page Visibility API
  if ('hidden' in $document)
    vendorPrefix = '';
  else
    angular.forEach(['moz', 'webkit', 'ms'], function(prefix) {
      if ((prefix + 'Hidden') in $document[0]) vendorPrefix = prefix;
    });

  if (angular.isDefined(vendorPrefix))
    $document[0].addEventListener(vendorPrefix + 'visibilitychange', visibilityChangeHandler);

}]);

angular.module('myswitch', ['ng']).directive('toggleSwitch', function () {
  return {
    restrict: 'EA',
    replace: true,
    scope: {
      model: '=',
      disabled: '@',
      onLabel: '@',
      offLabel: '@',
      caption: '@'
    },
    template: '<div class="ngswitch" ng-click="toggle()" ng-class="{ \'disabled\': disabled }">' +
              '<div class="switch-animate" ng-class="{\'switch-off\': !model, \'switch-on\': model}">' + 
              '<span class="switch-left" ng-bind="onLabel"></span><span class="knob" ng-bind="caption">' +
              '</span><span class="switch-right" ng-bind="offLabel"></span></div></div>',
    controller: function($scope) {
      $scope.toggle = function toggle() {
        if(!$scope.disabled) {
          $scope.model = !$scope.model;
        }
      };
    },
    compile: function(element, attrs) {
      if (!attrs.onLabel) { attrs.onLabel = 'On'; }
      if (!attrs.offLabel) { attrs.offLabel = 'Off'; }
      if (!attrs.caption) { attrs.caption = '\u00a0'; }
      if (!attrs.disabled) { attrs.disabled = false; }
    },
  };
});

angular.module('MaskedTextBox', ['ng']).directive('maskEdit', function () {
  return {
    require: '?ngModel',
    link: function(scope, element, attrs, controller) {
        element.mask(attrs.mask,{completed: function() {
            controller.$setViewValue(this.val());
            scope.$apply();
        }});
    }
  };
}).directive('validNumber', function() {
  return {
    require: '?ngModel',
    link: function(scope, element, attrs, ngModelCtrl) {
      if(!ngModelCtrl) {
        return; 
      }
      ngModelCtrl.$parsers.push(function(val) {
        var clean = val.replace( /[^0-9]+/g, '');
        if (val !== clean) {
          ngModelCtrl.$setViewValue(clean);
          ngModelCtrl.$render();
        }
        return clean;
      });

      element.bind('keypress', function(event) {
        if(event.keyCode === 32) {
          event.preventDefault();
        }
      });
    }
  };
});

var ISDEBUG = true;
var Wx = undefined;
var layout = undefined;

function ShowErrSystem(X) {
    var src = "sp.api/combo/SysMessages?ErrNo=" + X;
    $.ajax({
        async: false,
        type: "POST",
        url: src
    }).done(function (data) {
        MsgBox(data.ErrDesc, MSG_ERROR);
    });
}

function logIt(s)
{
   if (ISDEBUG)
   {
      console.log(s);
   }
}

function GetNumber(m)
{
    if (m !== undefined)
    {
        return m;
    } else {
        return 0;
    }
}

function GetValue(m)
{
    if (m !== undefined)
    {
        return m;
    } else {
        return "";
    }
}

var WxButtons = [ 
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    //{ name: "btnEdit",   text: "Edit",   cls:"btn btn-primary",    icon: "icon-edit",   show: "isLoadData && !isSave", click: "allowEdit()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
                    { name: "btnExecQuery", text: "Query", cls: "btn btn-warning", icon: "icon-search", show: "isEQAvailable", click: "execQuery()" },
                    { name: "btnExportXLS", text: "Export", cls: "btn btn-success", icon: "icon-print", show: "isEXLSAvailable", click: "exportToXLS()" },
];

SimDms.Angular = function (ctrl) {
  
    $(".page").attr("ng-app","myApp");
    $(".page").attr("ng-controller", ctrl);

    var $injector = angular.injector(["ng", "webix", "MaskedTextBox", 
      "ui.select2", "ui.bootstrap", "myswitch", "kendo.directives", 
      "ui.utils", "windowEventBroadcasts","ngResource","blockUI"]);   

    $injector.invoke(function ($rootScope, $compile, $document) {
        $compile($document)($rootScope);
        $rootScope.$digest();
    });

}                

function BaseController($scope, $http, $compile, $timeout, blockUI) {

    // Untuk mempermudah penulisan kode program, define variable me sebagai $scope (Alias)
    var me = $scope;

    // Define object untuk menyimpan/menampung data (baik dari interaksi dgn user (editing) mapupun load dari database)
    me.data = {};  

    // define local variable hasChange, fungsinya sebagai flag indikator perubahan data
    me.hasChanged = false;

    // define local variable isLoadData, fungsinya sebagai flag indikator load data dari browse function
    me.isLoadData = false;

    // define local variable isEditable, fungsinya sebagai flag indikator untuk memulai proses edit data (disabled primary/composite key)
    me.isEditable = false;

    // define local variable isSave, fungsinya sebagai flag indikator untuk menunjukan adanya perubahan data yg harus disimpan
    me.isSave = false;

    // check initialize status
    me.isInitialize = true;

    // current process stage
    me.isInProcess  = false; 

    // show/hide & enable/disable print button
    me.isPrintAvailable = false;
    me.isEQAvailable = false;
    me.isEXLSAvailable = false;

    me.isPrintEnable = true;

    me.grid = {};

    // Aktifkan fungsi $watch untuk memonitor perubahan data yang terjadi pada object 'data'
    me.$watch('data', function(newValue, oldValue) {
        me.stdChangedMonitoring(newValue, oldValue);
    }, true);

    me.startMonitoring = function(model)
    {
        me.$watch(model, function(newValue, oldValue) {
            me.stdChangedMonitoring(newValue, oldValue);
        }, true);        
    }

    me.execQuery = function()
    {
        alert('exec query')
    }

    me.exportToXLS = function()
    {
        alert('export to xls')
    }

    me.stdChangedMonitoring = function(n,o)
    {
       if (!me.isInProcess) {

         var eq = (n == o);
         
          // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
          if (!(_.isEmpty(n)) && !eq)
          {
              if(!me.hasChanged && !me.isLoadData)
              {
                  me.hasChanged = true;
                  me.isLoadData = false;                
              }  
              if (!me.isSave)
              {
                  me.isSave = true;
                  me.hasChanged = true;
                  me.isLoadData = false;  
              }
          } else {
              me.hasChanged = false;
              me.isSave = false;
          }        
        }      
    }




    me.delayEditing = function()
    {
        setTimeout(function()
        {
          me.isInProcess = false;
          me.ReformatNumber();
        }, 1234);
    }

    me.changedData = function(IsChanged)
    {
        if (IsChanged)
        {
            if(!me.hasChanged && !me.isLoadData)
            {
                me.hasChanged = true;
                me.isLoadData = false;                
            }  
            if (!me.isSave)
            {
                me.isSave = true;
            }
        }       
    }

    me.cancelOrClose = function()
    {        
        me.init();
    } 

    me.initialize = function()
    {
       
    }

    me.now = function(s)
    {
        if ( s !== undefined)
        {
            return moment().format(s);
        } 
        else 
        {
            return moment().format();
        }
    }

    me.loadTableData = function(o,d)
    {
        if (o){
            o.clearSelection();
            me.clearTable(o);
            o.define("data", d);
            o.render();
        }      
    }

    me.clearTable = function(o)
    {
      if (o)
        if (o.clearAll) {
            o.clearAll(); 
            o.adjust();          
        }
                
    }

    me.init = function()
    {

        me.hasChanged = me.isLoadData = me.isSave = me.isEditable = me.isInProcess = false;
        me.isInitialize = true;

        me.data = {};  
        me.grid = {};


        if (Wx !== undefined) {            
            Wx.reset();            
        }

        $("#btnCancel").html("<i class='icon icon-undo'></i>Cancel");

        setTimeout(function() {           
          me.initialize();
          $scope.$apply();
            setTimeout(function() {                 
                me.hasChanged = false;
                me.isLoadData = false;
                me.isEditable = false;
                me.isSave     = false;
                me.isInitialize = false;
                $scope.$apply();
             }, 3000);
        }, 50);
    }   

    // fungsi ini digunakan untuk menset flag indicator, 
    // agar state dari input/button berubah (show/hide/enable/disable)
    me.allowEdit = function()
    {
        me.isLoadData = false;
        me.isEditable = true;
        me.hasChanged = true;
        $("#btnCancel").html("<i class='icon icon-undo'></i>Cancel"); 
    }

    // fungsi bernilai TRUE, jika data dalam proses editing
    me.IsEditing = function()
    {
        var b = me.isEditable || me.isLoadData;
        if (b)
        {
           me.$broadcast("IsEditingEvent");
        }
        return b;
    }

    // berfungsi untuk mengirimkan signal yg akan diterima oleh $on(fName)
    me.trigger = function(fName)
    {
        me.$broadcast(fName);
    }
                              
    // fungsi ini digunakan untuk mengembalikan nilai object dari index element pada input selection
    me.GetObjectById = function(o,s)
    {

      if (s === undefined || s == null ) return null;

      var pos = $.map(o, function(obj, index) {
        if(obj.value == $.trim(s.toString())) {
          return obj;
        }
      });   

      if (pos != null )
        return pos[0];
      else
        return null;     
    }   
 
  // change button & control state by flag indicator
  me.startEditing = function()
  {
      me.isEditable = true;
      me.isSave = false;
      me.hasChanged = false;
      me.isLoadData = true;
      // change cancel button caption to "Close"
      $("#btnCancel").html("<i class='icon icon-hand-right'></i>Close");
  }

  
  // save data, posting data from user cache to the server
  me.save = function()
  { 
      if (Wx !== undefined)
      {
          Wx.submit();
      }
  }

  me.delete = function()
  {
      logIt("action save call on base controller");
  }

  // fungsi ini digunakan untuk inisialisasi data model dan persiapan editing data
  me.lookupAfterSelect = function(value)
  {
      me.isLoadData = true;
      me.data = value; 
      setTimeout(function()
      {       
          me.hasChanged = false;    
          me.startEditing();   
          me.isSave = false;  
          $scope.$apply();

          me.ReformatNumber();
          var selectorContainer = "";
          $.each(value, function (key, val) {
              var ctrl = $(selectorContainer + " [name=" + key + "]");
              var type = ctrl.data("type");
              //_this.populateValue(type, val, ctrl, selectorContainer, key)
              ctrl.removeClass("error");
          });

      }, 200);
  }

  // fungsi ini digunakan untuk populate data ke me.data
  me.populateData = function (value) {
      me.isLoadData = true;
      setTimeout(function () {
          me.hasChanged = false;
          me.startEditing();
          me.isSave = false;

          me.ReformatNumber();
          var selectorContainer = "";
          $.each(value, function (key, val) {
              var ctrl = $(selectorContainer + " [name=" + key + "]");
              me.data[key] = val;
              ctrl.removeClass("error");
          });

          $scope.$apply();
      }, 200);
  }


  me.ReformatNumber = function()
  {
      $("input[type=text].number").each(function (i,v) {
          var val0 = $(this).val();
          var valt = number_format(val0, 2);
          $(this).val(valt);
      });

      $("input[type=text].number-int").each(function (i,v) {
          var val0 = $(this).val();
          var valt = number_format(val0, 0);
          $(this).val(valt);
      });

      $("[data-type=int]").kendoNumericTextBox(
          { 
              format: "#,#", 
              step:1 ,
              decimals: 0
          }
      );    
  }

  // panggil fungsi ini setelah proses multiple loaddata selesai
  me.readyToModified = function()
  {
      me.isLoadData = true;     
      me.hasChanged = false;    
      me.startEditing();   
      me.isSave = false;  
    //  $scope.$apply();   
      setTimeout(function()
      {       
         me.isInProcess = false;
      }, 2000); 
  }

  me.readyToSave = function()
  {
      setTimeout(function()
      {       
          me.isLoadData = false; 
          me.hasChanged = true;    
          me.isSave = true;  
          $scope.$apply();
      }, 300);
  }

  me.Apply = function()
  {
      $scope.$apply();
  }

  me.start = function()
  {      
      setTimeout(function() { me.init(); }, 500);   
         
      IsFirstLoading = true;
      // start up point
      if (Wx !== undefined)
      {
          Wx.OnValidation(me.saveData);
          Wx.TabChanged(me.OnTabChange); 
      }
    
      setTimeout(function() { 
        IsFirstLoading = false; 
        $scope.$apply(); 

        $("[data-type=int]").kendoNumericTextBox(
            { 
                format: "#,#", 
                step:1 ,
                decimals: 0
            }
        );

            $(".page .main form").verify({
                beforeSubmit: function(sender,result)
                {
                    if(result)
                    {
                        $(".page .main form").trigger("ValidationOK",  [$(".main .gl-widget").serializeObject()] );
                    } else {
                       // Wx.Error("Validation failed !!!");
                    }
                },
                // prompt: function(element, text) {
                //   me.testError = "";
                //   if (text !==null) {
                //     me.testError += JSON.stringify(element) + " >> " + text + "\n";
                //     console.log(JSON.stringify(element) + " >> " + text);
                //   }
                      
                // }
            });

            // force validation form to show up mandatory fields
            $(".page .main form").submit();

          }, 1982);
  }



  me.GetLookupValue = function(sGroup,sCode, model)
  {
     $http.post('sp.api/MasterItem/GetLookupValueName?VarGroup=' + sGroup + "&varCode=" + sCode)
        .success(function(v, status, headers, config){
           me.data[model] = v;
        }).error(function(e, status, headers, config) {
           // MsgBox(e, MSG_ERROR);
           console.log(e); 
        });     
  }

  me.execSQL = function(sql, model)
  {
      var query = new breeze.EntityQuery()
                    .from("execSQL")
                    .withParameters({SQL: sql});

      spManager.executeQuery(query).then(function(data){
      }).fail(function(e) {
          //MsgBox(e, MSG_ERROR);  
          console.log(e); 
      });  
  }

 
  me.where = breeze.Predicate;

  var query = new breeze.EntityQuery().from("CurrentUserInfo");
    
  spManager.executeQuery(query).then(function(data){
      me.UserInfo = data.results[0];
      layout.loadUserInfo(me.UserInfo.UserId, me.UserInfo.BranchCode, me.UserInfo.CompanyName, me.UserInfo.ShowHideTypePart, me.UserInfo.SimDmsVersion);
  }).fail(function(e) {
      // MsgBox(e, MSG_ERROR); 
      console.log(e); 
  });

  me.dateFormat = function (val) {
      value = (val != undefined) ? moment(val).format('DD MMM YYYY') : '-';
      return value;
  };

  me.dateTimeFormat = function (val) {
      value = (val != undefined) ? moment(val).format('DD MMM YYYY HH:mm') : '-';
      return value;
  };

  me.timeFormat = function (val) {
      value = (val != undefined) ? moment(val).format('HH:mm') : ':';
      return value;
  };

  me.intFormat = function (val) {
      if (val == null || val == undefined || val == "") val = 0;

      value = number_format(val, 0);;
      return value;
  }

  me.decimalFormat = function (val) {
      if (val == null || val == undefined || val == "") val = 0;

      value = number_format(val, 2);;
      return value;
  }

  me.replaceNull = function (val) {
      if (val == null || val == undefined) val = "";

      return val;
  }

  me.CheckMandatory = function (Field, Names) {
      var Name = "";
      var q = Field.split(',', 75);
      var m = Names.split(',', 75);
      var r = q.length; // r panjang field
      var n = m.length; // n panjang nama
      var s = r; // untuk ambil nama

      for (i = 0; i < r; i++) {
          var x = $('#' + q[i]).val();
          if (x) {
              s = s - 1;
              console.log(x);
          } else {
              if (Name == "") {
                  Name = m[i];
              } else {
                  Name += ', ' + m[i];
              }
          }
      }

      return Name;
  }


}

// end of base controller


function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}