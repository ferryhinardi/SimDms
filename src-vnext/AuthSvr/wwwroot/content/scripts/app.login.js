runAllForms();

window.identityServer = (function () {
    "use strict";

    var identityServer = {
        getModel: function () {
            var modelJson = document.getElementById("modelJson");
            var encodedJson = '';
            if (typeof (modelJson.textContent) !== undefined) {
                encodedJson = modelJson.textContent;
            } else {
                encodedJson = modelJson.innerHTML;
            }
            var json = Encoder.htmlDecode(encodedJson);
            var model = JSON.parse(json);
            return model;
        }
    };

    return identityServer;
})();

(function () {
    "use strict";

    (function () {
        var app = angular.module("app", []);

        app.controller("LoginCtrl", function ($scope, Model) {
            $scope.model = Model;

            $scope.mySplit = function (input, splitChar, splitIndex) {
                $scope.array = input.split(splitChar);
                return $scope.result = $scope.array[splitIndex];
            }
        });

        app.directive("antiForgeryToken", function () {
            return {
                restrict: 'E',
                replace: true,
                scope: {
                    token: "="
                },
                template: "<input type='hidden' name='{{token.name}}' value='{{token.value}}'>"
            };
        });

        app.directive("focusIf", function ($timeout) {
            return {
                restrict: 'A',
                scope: {
                    focusIf: '='
                },
                link: function (scope, elem, attrs) {
                    if (scope.focusIf) {
                        $timeout(function () {
                            elem.focus();
                        }, 100);
                    }
                }
            };
        });

        app.filter("split", function () {
            return function (input, splitChar, splitIndex) {
                // do some bounds checking here to ensure it has that index
                return input.split(splitChar)[splitIndex];
            }
        });
    })();

    (function () {
        var model = identityServer.getModel();
        angular.module("app").constant("Model", model);
        if (model.autoRedirect && model.redirectUrl) {
            if (model.autoRedirectDelay < 0) {
                model.autoRedirectDelay = 0;
            }
            window.setTimeout(function () {
                window.location = model.redirectUrl;
            }, model.autoRedirectDelay * 1000);
        }
    })();

    // Validation
    $("#login-form").validate({
        // Rules for form validation
        rules: {
            username: {
                required: true
            },
            password: {
                required: true
            }
        },

        // Messages for form validation
        messages: {
            username: {
                required: 'Please enter your username'
            },
            password: {
                required: 'Please enter your password'
            }
        },

        errorClass: 'invalid',

        errorElement: 'em',

        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        },

        highlight: function (element, errorClass, validClass) {
            $(element).parent().removeClass('state-success');
            $(element).parent().addClass('state-error');
        },

        unhighlight: function (element, errorClass, validClass) {
            $(element).parent().removeClass('state-error');
            $(element).parent().addClass('state-success');
        }
    });
})();
