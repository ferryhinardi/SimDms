appControllers
.service('AuthService', function ($q, $http, USER_ROLES, BASE_ADDRESS) {

    $http.defaults.useXDomain = true;
    var LOCAL_TOKEN_KEY = 'm-simdms-token';

    var username = '';
    var isAuthenticated = false;
    var role = '';
    var internalUser = false;
    var authToken;
    var userData = {};


    function loadUserCredentials() {
        var token = window.localStorage.getItem(LOCAL_TOKEN_KEY);
        if (token) {
            useCredentials(token);
        }
    }

    function storeUserCredentials(token) {
        window.localStorage.setItem(LOCAL_TOKEN_KEY, token);
        useCredentials(token);
    }

    function useCredentials(token) {
         
        isAuthenticated = true;
        authToken = token;

        if (username == 'admin') {
            role = USER_ROLES.admin
        }
        if (username == 'user') {
            role = USER_ROLES.public
        }

        // Set the token as header for your requests!
        //$http.defaults.headers.common['X-Auth-Token'] = token;        
        

        //setTimeout(function () {
        //    $http.get(BASE_ADDRESS + "suzuki/simdms/userinfo")
        //    .success(function (data) {
        //        console.log(data);
        //        if (data.success == true) {
        //            userData = data.data;
        //        }   
        //    })
        //},500)
        

    }

    function destroyUserCredentials() {
        authToken = undefined;
        username = '';
        isAuthenticated = false;
        $http.defaults.headers.common['X-Auth-Token'] = undefined;
        $http.defaults.headers.common['Authorization'] = undefined;
        window.localStorage.removeItem(LOCAL_TOKEN_KEY);
    }

    var login = function (name, pw) {
        return $q(function (resolve, reject) {
            var params = {
                UserName: name,
                Password: pw,
                client_id: 'SIMDMS',
                grant_type: 'password'
            }
            
            var url = BASE_ADDRESS + "auth/gettoken";
		
		    $.ajax({
		      type: "POST",
		      url: url,
		      data: params,
		      timeout: 33000,
		    }).done(function (data) {  
		        if (data.access_token !== null) {
		            $http.defaults.headers.common['Authorization'] = 'Bearer ' + data.access_token;
                    $http.defaults.withCredentials= true;
		            setTimeout(function () {
                        console.log($http.defaults.headers.common['Authorization'])
                        storeUserCredentials(data.access_token);
		            },1000)                    
                    resolve('Login success.');
                } else {
                    reject('Login Failed.');
                }
		      })
		      //.fail(function(error) {
			  //  alert( "Access Denied" );			 
		      //})
		      //.always(function() {
			  //   //$ionicLoading.hide();
		      //});

        });
    };

    var logout = function () {
        destroyUserCredentials();
    };

    var isAuthorized = function (authorizedRoles) {
        if (!angular.isArray(authorizedRoles)) {
            authorizedRoles = [authorizedRoles];
        }
        return (isAuthenticated && authorizedRoles.indexOf(role) !== -1);
    };

    loadUserCredentials();

    return {
        login: login,
        logout: logout,
        isAuthorized: isAuthorized,
        isAuthenticated: function () { return isAuthenticated; },
        username: function () { return username; },
        role: function () { return role; },
        internalOnly: function () { return internalUser; },
        userInfo: function () { return userData; }
    };
});


//appControllers
//.factory('AuthInterceptor', function ($rootScope, $q, AUTH_EVENTS) {
//  return {
//    responseError: function (response) {
//      $rootScope.$broadcast({
//        401: AUTH_EVENTS.notAuthenticated,
//        403: AUTH_EVENTS.notAuthorized
//      }[response.status], response);
//      return $q.reject(response);
//    }
//  };
//})
 
//.config(function ($httpProvider) {
//  $httpProvider.interceptors.push('AuthInterceptor');
//});