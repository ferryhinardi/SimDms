"use strict";
 
window.onerror = function (errorMsg, url, lineNumber, column, errorObj) {
    // Send object with all data to server side log, using severity fatal, 
    // from logger "onerrorLogger"
    JL("onerrorLogger").fatalException({
        "msg": "Exception!",
        "errorMsg": errorMsg, "url": url,
        "line number": lineNumber, "column": column
    }, errorObj);

    // Tell browser to run its own error handler as well   
    return false;
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}


define(['angular', 'angularAMD', 'api/config/routes/1/', 'angular-sanitize', 'angular-animate', 
     'angular-route', 'ui-codemirror',  'xtform', 'ng-moment','ui-bootstrap','angular-resource', 
    //'handsontable', 'ngMap',  'angular-markdown-text', 'scrollbars', 'sortable', 'angular-form-gen','autofocus',  'prettify','amplify'
    //'dt-plugins','ng-datatable','ngDialog','ngNotify', 'autofields', 'autofields-bootstrap','colorpicker','ngRule','ng.nv.d3',
    'angular-block-ui', 'ui-utils',  'ng-calendar',   'pickDay',   'kendo.ext' , 'ngHandsontable', 
    'door3css','adapt-strap','adapt-strap.tpl','ng-idle', 'angularModalService' ],


    function (ng, angularAMD, config) { 

        var app = angular.module("app", ['ng', 'ngRoute', 'ngSanitize', 'ngAnimate', 'angularMoment',
            'kendo.directives', 'ui.utils', 'ui.bootstrap', 'xtForm', 'ngIdle', 'ngResource',  'ui.calendar', 'ngHandsontable',           
            //'datatables.bootstrap', 'datatables.colreorder', 'datatables.columnfilter', 'datatables.colvis','datatables', 'autofields',
            //'datatables.fixedcolumns', 'datatables.fixedheader', 'datatables.scroller', 'datatables.tabletools', 'ngDialog', 'ngNotify',
            //'autofocus','ui.sortable', 'ngMap','fg', 'markdown','ngScrollbars', 'nvd3',
            'ui.codemirror',  'blockUI', 'door3.css','adaptv.adaptStrap','angularModalService']);

         app.config(function ($provide, $httpProvider) {  

            $provide.factory('MyHttpInterceptor', function ($q) {
                return {
                    request: function (config) {
                        return config || $q.when(config);
                    }, 
                    requestError: function (rejection) {
                        var msg = rejection.data || "Could not connect to the server !!!";
                        bootbox.alert(msg);
                        return $q.reject(rejection);
                    },
                    response: function (response) {
                        if (response.data.success !== undefined) {
                            if (!response.data.success) {
                                var msg = response.data.message || "Could not connect to the server !!!";
                                bootbox.dialog({
                                    message: msg,
                                    title: ' <font color="red"><i class="fa fa-exclamation-triangle"></i>  Error !!!</font>',
                                    onEscape: function () {

                                    },
                                    show: true,
                                    backdrop: true,
                                    closeButton: true,
                                    animate: true,
                                    buttons: {
                                        '<i class="fa fa-times"></i> Close': {
                                            className: "btn-default",
                                            callback: function () { }
                                        },
                                    }
                                });
                            } else {
                                if (response.data.message !== undefined) {
                                    toastr["success"](response.data.message, "success")
                                }
                            }
                        }
                        return response || $q.when(response);
                    },
                    responseError: function (rejection) {
                        var msg = rejection.data || "Could not connect to the server !!!";
                        bootbox.alert(msg);
                        return $q.reject(rejection);
                    }
                };
            });
 
            // Add the interceptor to the $httpProvider.
            $httpProvider.interceptors.push('MyHttpInterceptor');
 
        });

         app.config(['$httpProvider', function ($httpProvider) {
             $httpProvider.defaults.headers.common = { 'X-Requested-With': 'XMLHttpRequest' };
         }]);

        app.directive('confirmPassword', [function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attrs, ngModel) {
                    var validate = function (viewValue) {
                        var password = scope.$eval(attrs.confirmPassword);
                        ngModel.$setValidity('match', ngModel.$isEmpty(viewValue) || viewValue == password);
                        return viewValue;
                    }
                    ngModel.$parsers.push(validate);
                    scope.$watch(attrs.confirmPassword, function (value) {
                        validate(ngModel.$viewValue);
                    })
                }
            }
        }])
        .directive('formField', function ($compile, $parse) {
            return {
                restrict: 'E',
                compile: function (element, attrs) {
                    var fieldGetter = $parse(attrs.field);

                    return function (scope, element, attrs) {
                        var template, field, id;
                        field = fieldGetter(scope);
                        id = 'form-field-' + field.name; // make a unique string id to match label/input

                        template = [
                            '<div class="form-group">',
                            '  <label class="control-label" for="' + id + '">' + field.label + '</label>',
                            '  <input class="form-control" ng-model="' + attrs.ngModel + '.' + field.name + '" type="' + (field.type || 'text') + '"',
                            (field.required ? ' required ' : ''),
                            (field.minLength ? ' ng-minlength="' + field.minLength + '"' : ''),
                            (field.maxLength ? ' ng-maxlength="' + field.maxLength + '"' : ''),
                            (field.pattern ? ' ng-pattern="' + field.pattern + '"' : ''),
                            ' id="' + id + '">',
                            '</div>'
                        ].join('');

                        element.replaceWith($compile(template)(scope));
                    }
                }
            }
        })
        .directive('collection', function () {
            return {
                restrict: "E",
                replace: true,
                scope: {
                    collection: '='
                },
                template: "<ul><member ng-repeat='member in collection' member='member'></member></ul>"
            }
        })
        .directive('member', function ($compile) {
            return {
                restrict: "E",
                replace: true,
                scope: {
                    member: '='
                },
                template: '<li class><a href="{{member.Url}}"><i class="{{member.Icon}}"></i> <span class="menu-item-parent">{{member.MenuCaption}}</span></a></li>',
                link: function (scope, element, attrs) {
                    if (angular.isArray(scope.member.children)) {
                        element.append("<collection collection='member.Detail'></collection>");
                        $compile(element.contents())(scope)
                    }
                }
            }
        })
        .filter('propsFilter', function () {
            return function (items, props) {
                var out = [];

                if (angular.isArray(items)) {
                    items.forEach(function (item) {
                        var itemMatches = false;

                        var keys = Object.keys(props);
                        for (var i = 0; i < keys.length; i++) {
                            var prop = keys[i];
                            var text = props[prop].toLowerCase();
                            if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                                itemMatches = true;
                                break;
                            }
                        }

                        if (itemMatches) {
                            out.push(item);
                        }
                    });
                } else {
                    // Let the output be the input untouched
                    out = items;
                }

                return out;
            }
        }).controller('NavigationCtrl', function ($scope, $http, $compile) {

            $scope.navItems = [{ "MenuId": "M1", "MenuCaption": "Time Management", "Url": "#", "Icon": null, "ModuleName": null, "ParentName": null, "Detail": [{ "MenuId": "M11", "MenuCaption": "test1", "Url": "#", "Icon": null, "ModuleName": "Time Management", "ParentName": "Time Management", "Detail": [{ "MenuId": "M111", "MenuCaption": "test-1.1", "Url": "#", "Icon": null, "ModuleName": "Time Management", "ParentName": "Time Management / test1", "Detail": [] }] }] }, { "MenuId": "M2", "MenuCaption": "Payroll Administration", "Url": "#", "Icon": null, "ModuleName": null, "ParentName": null, "Detail": [{ "MenuId": "M21", "MenuCaption": "test2", "Url": "#", "Icon": null, "ModuleName": "Payroll Administration", "ParentName": "Payroll Administration", "Detail": [] }] }, { "MenuId": "M3", "MenuCaption": "Personal Administration", "Url": "#", "Icon": null, "ModuleName": null, "ParentName": null, "Detail": [{ "MenuId": "M31", "MenuCaption": "test", "Url": "#", "Icon": null, "ModuleName": "Personal Administration", "ParentName": "Personal Administration", "Detail": [] }, { "MenuId": "M32", "MenuCaption": "sdsd", "Url": "#", "Icon": "", "ModuleName": "Personal Administration", "ParentName": "Personal Administration", "Detail": [] }] }];

            //$http.get('trio/sm/setup/menu/GenerateNavigationMenu')
            //.success(function (a, b, c, d) {
            //    $scope.navItems = a;
            //    var myEl = angular.element(document.querySelector('#myNavigation'));
            //    myEl.append("<collection collection='navItems'></collection>");
            //    $compile(myEl.contents())($scope)
            //})


        }).directive('showData', function ($compile) {
            return {
                scope: true,
                link: function (scope, element, attrs) {
                    var el;
                    attrs.$observe('template', function (tpl) {
                        if (angular.isDefined(tpl)) {
                            el = $compile(tpl)(scope);
                            element.html("");
                            element.append(el);
                        }
                    });
                }
            };
        });

        app.factory('queueBlockUI', function (blockUI, $timeout, $q) {
            return function (messages) {

                var x = 0, defer = $q.defer();

                function next() {

                    var msg = messages[x];

                    if (!x) {
                        blockUI.start(msg);
                    } else if (x < messages.length) {
                        blockUI.message(msg);
                    } else {
                        blockUI.stop();
                        defer.resolve();
                    }

                    if (x++ < messages.length) {
                        $timeout(next, Math.floor(Math.random() * 500 + 1000));
                    }
                }

                next();

                return defer.promise;
            }
        });
        //app.config(function (xtFormConfigProvider) {
        //    // Add custom validation strategy
        //    xtFormConfigProvider.addValidationStrategy('customStrategy', function (form, ngModel) {
        //        return ngModel.$invalid && (form.$submitted || ngModel.$dirty);
        //    });
        //});

        //// Set as default for all forms
        //app.config(function (xtFormConfigProvider) {
        //    // Add custom validation strategy
        //    xtFormConfigProvider.setDefaultValidationStrategy('customStrategy');
        //});
        

        app.provider('toggleSwitchConfig', function () {
            this.onLabel = 'On';
            this.offLabel = 'Off';
            this.knobLabel = '\u00a0';

            var self = this;
            this.$get = function () {
                return {
                    onLabel: self.onLabel,
                    offLabel: self.offLabel,
                    knobLabel: self.knobLabel
                };
            };
        });

        app.directive('toggleSwitch', function (toggleSwitchConfig) {
            return {
                restrict: 'EA',
                replace: true,
                require: 'ngModel',
                scope: {
                    disabled: '@',
                    onLabel: '@',
                    offLabel: '@',
                    knobLabel: '@'
                },
                template: '<div role="radio" class="ngswitch" ng-class="{ \'disabled\': disabled }">' +
                    '<div class="switch-animate" ng-class="{\'switch-off\': !model, \'switch-on\': model}">' +
                    '<span class="switch-left" ng-bind="onLabel"></span>' +
                    '<span class="knob" ng-bind="knobLabel"></span>' +
                    '<span class="switch-right" ng-bind="offLabel"></span>' +
                    '</div>' +
                    '</div>',
                compile: function (element, attrs) {
                    if (!attrs.onLabel) { attrs.onLabel = toggleSwitchConfig.onLabel; }
                    if (!attrs.offLabel) { attrs.offLabel = toggleSwitchConfig.offLabel; }
                    if (!attrs.knobLabel) { attrs.knobLabel = toggleSwitchConfig.knobLabel; }

                    return this.link;
                },
                link: function (scope, element, attrs, ngModelCtrl) {
                    var KEY_SPACE = 32;

                    element.on('click', function () {
                        scope.$apply(scope.toggle);
                    });

                    element.on('keydown', function (e) {
                        var key = e.which ? e.which : e.keyCode;
                        if (key === KEY_SPACE) {
                            scope.$apply(scope.toggle);
                        }
                    });

                    ngModelCtrl.$formatters.push(function (modelValue) {
                        return modelValue;
                    });

                    ngModelCtrl.$parsers.push(function (viewValue) {
                        return viewValue;
                    });

                    ngModelCtrl.$render = function () {
                        scope.model = ngModelCtrl.$viewValue;
                    };

                    scope.toggle = function toggle() {
                        if (!scope.disabled) {
                            scope.model = !scope.model;
                            ngModelCtrl.$setViewValue(scope.model);
                        }
                    };
                }
            };
        });

        //app.value('DEBUG', true);

    app.config(
    [
        '$routeProvider',
        '$locationProvider',
        '$controllerProvider',
        '$compileProvider',
        '$filterProvider',
        '$provide',

        function ($routeProvider, $locationProvider, $controllerProvider, $compileProvider, $filterProvider, $provide) {
            app.controller = $controllerProvider.register;
            app.directive = $compileProvider.directive;
            app.filter = $filterProvider.register;
            app.factory = $provide.factory;
            app.service = $provide.service;

            if (config.routes !== undefined) {
                angular.forEach(config.routes, function (route, path) {
                    $routeProvider.when(path, angularAMD.route({ templateUrl: route.templateUrl, controllerUrl: route.controllerUrl, css: route.css || undefined }));
                });
            }

            $routeProvider
                .when("/page/:id", angularAMD.route({
                    templateUrl: function (rp) { return 'api/files/templates/' + rp.id + '/html'; },
                    resolve: {
                        load: ['$q', '$rootScope', '$location', function ($q, $rootScope, $location) {
                            var path = $location.path();
                            var parsePath = path.split("/");
                            var parentPath = parsePath[1];
                            var controllerName = parsePath[2];
                            var loadController = "api/files/templates/" + controllerName + "/js";
                            var deferred = $q.defer();
                            require([loadController], function () {
                                $rootScope.$apply(function () {
                                    deferred.resolve();
                                });
                            });
                            return deferred.promise;
                        }]
                    }
                }))
                .otherwise({ redirectTo: '/' })

        }
    ]);
        
    app.controller('bodyController', function ($scope, $http, $compile, $route, $modal, ModalService) {
        $scope.$on(
            "$routeChangeSuccess",
            function handleRouteChangeEvent(event) {
                //console.log("Route Change:", $route.current);
            }
        );

        $scope.changePassword = function () {
            ModalService.showModal({
                templateUrl: "change-password.html",
                controller: ModalCtrl,
                backdrop: 'static'
            }).then(function (modal) {
                modal.element.modal();
                modal.close.then(function (result) {
                    close(null, 500);
                });
            });
        }



        var ModalCtrl = function ($scope, $element, $http, close) {

            $scope.changePwd = function () {
                console.log($scope.data)
                var link = "sm/setup/user/changepassword";

                bootbox.confirm("Apakah anda yakin ? Pastikan anda sudah mengingat password baru yang anda masukkan !!!", function (result) {
                    if (result) {
                        $http.post(link, $scope.data).then(function (response) {
                            if (response.data !== null) {
                                if (response.data.success === true) {
                                    $scope.display = false;
                                    $element.modal('hide');
                                    close({
                                        data: response.data.data
                                    }, 500);
                                }
                            }
                        })
                    }
                })

            };

            $scope.cancel = function () {
                $element.modal('hide');
                $scope.display = false;
                close(null, 500);
            };

        };

    });

    app.directive("passwordVerify", function () {
        return {
            require: "ngModel",
            scope: {
                passwordVerify: '='
            },
            link: function (scope, element, attrs, ctrl) {
                scope.$watch(function () {
                    var combined;

                    if (scope.passwordVerify || ctrl.$viewValue) {
                        combined = scope.passwordVerify + '_' + ctrl.$viewValue;
                    }
                    return combined;
                }, function (value) {
                    if (value) {
                        ctrl.$parsers.unshift(function (viewValue) {
                            var origin = scope.passwordVerify;
                            if (origin !== viewValue) {
                                ctrl.$setValidity("passwordVerify", false);
                                return undefined;
                            } else {
                                ctrl.$setValidity("passwordVerify", true);
                                return viewValue;
                            }
                        });
                    }
                });
            }
        };
    });
        

    //app.factory('$exceptionHandler', function () {
    //    return function (exception, cause) {
    //        JL('Angular').fatalException(cause, exception);
    //        //throw exception;
    //    };
    //});

    app.run(function ($rootScope, $templateCache, $timeout) {
        $rootScope.$on('$viewContentLoaded', function () {
            $(document).ready(function () {
                if (window.$ !== undefined)
                {
                    $timeout(function () {
                        pageSetUp();                         
                    },1000)                    
                }
            });
        });
    });

    return angularAMD.bootstrap(app);

});
