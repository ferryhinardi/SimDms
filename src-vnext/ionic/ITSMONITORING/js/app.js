// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'starter.controllers' is found in controllers.js
angular.module('starter', ['ionic', 'starter.controllers', 'ionic-material', 'LocalStorageModule', 'ionMdInput', 'ngMaterial'])

//.constant('BASE_ADDRESS', 'https://dms.suzuki.co.id/idsrv')
.constant('BASE_ADDRESS', 'http://localhost:50573')

.constant('AUTH_EVENTS', {
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})

.constant('USER_ROLES', {
    admin: 'admin_role',
    public: 'public_role'
})
    
.run(function ($ionicPlatform) {
    $ionicPlatform.ready(function () {
        // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
        // for form inputs)
        if (window.cordova && window.cordova.plugins.Keyboard) {
            cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
            cordova.plugins.Keyboard.disableScroll(true);

        }
        if (window.StatusBar) {
            // org.apache.cordova.statusbar required
            StatusBar.styleDefault();
        }
    });
})

//.config(['$httpProvider', function ($httpProvider) {
//    $httpProvider.defaults.headers = {
//        'Access-Control-Allow-Origin': '*',
//        'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
//        'Access-Control-Allow-Headers': 'Content-Type, X-Requested-With'
//    };
//    delete $httpProvider.defaults.headers.common['X-Requested-With'];
//}])

.config(function (localStorageServiceProvider) {
    localStorageServiceProvider
      .setPrefix('SUZUKI-EXHIBITION');
})

    .factory('principal', ['$q', '$http', '$timeout',
  function ($q, $http, $timeout) {
      var _identity = undefined,
        _authenticated = false;

      return {
          isIdentityResolved: function () {
              return angular.isDefined(_identity);
          },
          isAuthenticated: function () {
              return _authenticated;
          },
          isInRole: function (role) {
              if (!_authenticated || !_identity.roles) return false;

              return _identity.roles.indexOf(role) != -1;
          },
          isInAnyRole: function (roles) {
              if (!_authenticated || !_identity.roles) return false;

              for (var i = 0; i < roles.length; i++) {
                  if (this.isInRole(roles[i])) return true;
              }

              return false;
          },
          authenticate: function (identity) {
              _identity = identity;
              _authenticated = identity != null;

              // for this demo, we'll store the identity in localStorage. For you, it could be a cookie, sessionStorage, whatever
              if (identity) localStorage.setItem("demo.identity", angular.toJson(identity));
              else localStorage.removeItem("demo.identity");
          },
          identity: function (force) {
              var deferred = $q.defer();

              if (force === true) _identity = undefined;

              // check and see if we have retrieved the identity data from the server. if we have, reuse it by immediately resolving
              if (angular.isDefined(_identity)) {
                  deferred.resolve(_identity);
                  return deferred.promise;
              }

              // otherwise, retrieve the identity data from the server, update the identity object, and then resolve.
              //                   $http.get('/svc/account/identity', { ignoreErrors: true })
              //                        .success(function(data) {
              //                            _identity = data;
              //                            _authenticated = true;
              //                            deferred.resolve(_identity);
              //                        })
              //                        .error(function () {
              //                            _identity = null;
              //                            _authenticated = false;
              //                            deferred.resolve(_identity);
              //                        });

              // for the sake of the demo, we'll attempt to read the identity from localStorage. the example above might be a way if you use cookies or need to retrieve the latest identity from an api
              // i put it in a timeout to illustrate deferred resolution
              var self = this;
              $timeout(function () {
                  _identity = angular.fromJson(localStorage.getItem("demo.identity"));
                  self.authenticate(_identity);
                  deferred.resolve(_identity);
              }, 1000);

              return deferred.promise;
          }
      };
  }
    ])
// authorization service's purpose is to wrap up authorize functionality
// it basically just checks to see if the principal is authenticated and checks the root state 
// to see if there is a state that needs to be authorized. if so, it does a role check.
// this is used by the state resolver to make sure when you refresh, hard navigate, or drop onto a
// route, the app resolves your identity before it does an authorize check. after that,
// authorize is called from $stateChangeStart to make sure the principal is allowed to change to
// the desired state
.factory('authorization', ['$rootScope', '$state', 'principal',
 function ($rootScope, $state, principal) {
     return {
         authorize: function () {
             return principal.identity()
               .then(function () {
                   var isAuthenticated = principal.isAuthenticated();

                   if ($rootScope.toState.data.roles && $rootScope.toState.data.roles.length > 0 && !principal.isInAnyRole($rootScope.toState.data.roles)) {
                       if (isAuthenticated) $state.go('accessdenied'); // user is signed in but not authorized for desired state
                       else {
                           // user is not authenticated. stow the state they wanted before you
                           // send them to the signin state, so you can return them when you're done
                           $rootScope.returnToState = $rootScope.toState;
                           $rootScope.returnToStateParams = $rootScope.toStateParams;

                           // now, send them to the signin state so they can log in
                           $state.go('signin');
                       }
                   }
               });
         }
     };
 }
])

.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider

    .state('app', {
        url: "/app",
        abstract: true,
        templateUrl: "templates/menu.html",
        controller: 'AppCtrl'
    })

    .state('app.search', {
        url: "/search",
        views: {
            'menuContent': {
                templateUrl: "templates/search.html"
            }
        }
    })

    .state('app.browse', {
        url: "/browse",
        views: {
            'menuContent': {
                templateUrl: "templates/browse.html"
            }
        }
    })

    .state('app.changePassword', {
        url: "/change-password",
        views: {
            'menuContent': {
                templateUrl: "templates/change-password.html",
                controller: 'ChangePasswordCtrl'
            }
        }
    })

      .state('app.playlists', {
          url: "/playlists",
          views: {
              'menuContent': {
                  templateUrl: "templates/playlists.html",
                  controller: 'PlaylistsCtrl'
              }
          }
      })

      .state('app.dovsfpvsits', {
          url: "/dovsfpvsits",
          views: {
              'menuContent': {
                  templateUrl: "templates/dovsfpvsits.html",
                  controller: 'dovsfpvsitsCtrl'
              }
          }
      })
      
            .state('app.dovsfpvsits2', {
          url: "/dovsfpvsits2",
          views: {
              'menuContent': {
                  templateUrl: "templates/dovsfpvsits2.html",
                  controller: 'dovsfpvsits2Ctrl'
              }
          }
      })

      .state('app.stcvsinvvsits', {
          url: "/stcvsinvvsits",
          views: {
              'menuContent': {
                  templateUrl: "templates/stcvsinvvsits.html",
                  controller: 'stcvsinvvsitsCtrl'
              }
          }
      })

            .state('app.welcome', {
                url: "/welcome",
                views: {
                    'menuContent': {
                        templateUrl: "templates/welcome.html",
                        controller: 'WelcomeCtrl'
                    }
                }
            })

      .state('app.detail', {
          url: "/detail/:Tipe",
          views: {
              'menuContent': {
                  templateUrl: "templates/detail.html",
                  controller: 'TipeCtrl'
              }
          }
      })

    .state('app.single', {
        url: "/playlists/:Id/:EventName",
        views: {
            'menuContent': {
                templateUrl: "templates/playlist.html",
                controller: 'PlaylistCtrl'
            }
        }
    });
    // if none of the above states are matched, use this as the fallback
    $urlRouterProvider.otherwise('/app/welcome');
    // $urlRouterProvider.otherwise('/app/stcvsinvvsits');
});

var myApp = angular.module('starter');

myApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    //$httpProvider.defaults.headers = {
    //    'Access-Control-Allow-Origin': '*',
    //    'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
    //    'Access-Control-Allow-Headers': 'Content-Type, X-Requested-With'
    //};
    delete $httpProvider.defaults.headers.common['X-Requested-With'];
}]);

function NumDigit(n) {
    return n.toFixed(0).replace(/./g, function (c, i, a) {
        return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
    });
}
