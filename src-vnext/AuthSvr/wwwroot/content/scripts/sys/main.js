define(['dependencyResolverFor'], function (dependencyResolverFor) {
    var app = angular.module('main', ['ng','ngRoute']);

    app.config(
    [
        '$routeProvider',
        '$locationProvider',
        '$controllerProvider',
        '$compileProvider',
        '$filterProvider',
        '$provide',
        '$routeParams',

        function ($routeProvider, $locationProvider, $controllerProvider,
                    $compileProvider, $filterProvider, $provide, $routeParams)
        {
            app.controller = $controllerProvider.register;
            app.directive = $compileProvider.directive;
            app.filter = $filterProvider.register;
            app.factory = $provide.factory;
            app.service = $provide.service;

            $locationProvider.html5Mode(true);

            $routeProvider.
                when('/', {
                    templateUrl: 'api/files/templates/welcome/html',
                    resolve: dependencyResolverFor(['api/files/templates/welcome/js'])
                }).
                when('/page/:id',
                        {
                            templateUrl: function (params) {
                                return 'api/files/templates/' + params.id + '/html';
                            },
                            resolve: function (params) {
                                return dependencyResolverFor('api/files/templates/' + params.id + '/js')
                            }
                        }
                    ).
                otherwise({ redirectTo: '/' });

            //$routeProvider.when(path,
            //{
            //    templateUrl: route.templateUrl,
            //    resolve: dependencyResolverFor(route.dependencies)
            //});
            
            //$routeProvider.otherwise({ redirectTo: config.defaultRoutePaths });
        }
    ]);

    return app;
});