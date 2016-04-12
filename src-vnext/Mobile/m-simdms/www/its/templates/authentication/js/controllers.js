appControllers.controller('myLoginCtrl', function ($scope, $state,$ionicHistory,$ionicViewSwitcher,$timeout, $ionicPlatform, $mdDialog, AuthService, $http, BASE_ADDRESS,$cookies ) {

    $http.defaults.useXDomain = true;

    $scope.data = {
        username: '',
        password: ''
    }

    $scope.navigateTo = function (stateName,objectData) {
	
		if (stateName=="exit"){
			
		} else
        if ($ionicHistory.currentStateName() != stateName) {
            $ionicHistory.nextViewOptions({
                disableAnimate: false,
                disableBack: true
            });

            //Next view animate will display in back direction
            $ionicViewSwitcher.nextDirection('back');

            $state.go(stateName, {
                isAnimated: objectData,
            }, {reload: true});
        }
    }; // End of navigateTo.
	
	
    $scope.login = function ($event) {


	    console.log($scope.data)
	    if ($scope.data.username != '' && $scope.data.password != '') {
	        //AuthService.login($scope.data.username,$scope.data.password).then(function (data) {
            //    console.log(data)
	        //})

             var params = {
                UserName: $scope.data.username,
                Password: $scope.data.password,
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


		            $http.post(BASE_ADDRESS + "suzuki/simdms/userinfo", {
		                headers: {'Authorization': 'Bearer ' + data.access_token}
		            })            .success(function (data) {
		                console.log(data);
		                if (data.success == true) {
		                    userData = data.data;
		                }   
		            });


		    //        $http.defaults.headers.common['Authorization'] = 'Bearer ' + data.access_token;
		    //        $http.defaults.headers.get['Authorization'] = 'Bearer ' + data.access_token;
            //        window.globalVariable.Token = 'Bearer ' + data.access_token;
            //        $http.defaults.withCredentials = false;

		    //        setTimeout(function () {
		    //            console.log($http.defaults.headers.common['Authorization'])

            //             $http.get(BASE_ADDRESS + "suzuki/simdms/userinfo")
            //.success(function (data) {
            //    console.log(data);
            //    if (data.success == true) {
            //        userData = data.data;
            //    }   
            //})

                        //$.ajax({

                        //      // The 'type' property sets the HTTP method.
                        //      // A value of 'PUT' or 'DELETE' will trigger a preflight request.
                        //      type: 'GET',

                        //      // The URL to make the request to.
                        //      url: BASE_ADDRESS + "suzuki/simdms/userinfo",

                        //      // The 'contentType' property sets the 'Content-Type' header.
                        //      // The JQuery default for this property is
                        //      // 'application/x-www-form-urlencoded; charset=UTF-8', which does not trigger
                        //      // a preflight. If you set this value to anything other than
                        //      // application/x-www-form-urlencoded, multipart/form-data, or text/plain,
                        //      // you will trigger a preflight request.
                        //      contentType: 'application/x-www-form-urlencoded; charset=UTF-8',

                        //      xhrFields: {
                        //        // The 'xhrFields' property sets additional fields on the XMLHttpRequest.
                        //        // This can be used to set the 'withCredentials' property.
                        //        // Set the value to 'true' if you'd like to pass cookies to the server.
                        //        // If this is enabled, your server must respond with the header
                        //        // 'Access-Control-Allow-Credentials: true'.
                        //        withCredentials: true
                        //      },

                        //      headers: {

                        //          'Authorization' : $http.defaults.headers.common['Authorization']
                        //        // Set any custom headers here.
                        //        // If you set any non-simple headers, your server must include these
                        //        // headers in the 'Access-Control-Allow-Headers' response header.
                        //      },

                        //      success: function (dataX) {
                        //          console.log(dataX)
                        //        // Here's where you handle a successful response.
                        //      }

 
                        //    });     

                         
		                                
                    
                } else {
                    
                }
		      })
	    }
	}
	
	$scope.test = function ($event) {

	    //$.ajax({
	    //    type: "GET",
	    //    url: BASE_ADDRESS + "suzuki/simdms/userinfo",
	        
	    //    contentType: "application/json; charset=utf-8",
	    //    dataType: "json",
	    //    success: function (data) {
        //        console.log(data)
	    //    }

	    //})
                   $http.get(BASE_ADDRESS + "suzuki/simdms/userinfo")
            .success(function (data) {
                console.log(data);
                if (data.success == true) {
                    userData = data.data;
                }   
            })
	}
	
});  

appControllers.controller('myChangePasswordCtrl', function ($scope, $state,$ionicHistory,$ionicViewSwitcher,$timeout, $mdUtil, $mdSidenav, $log, $ionicPlatform, $mdDialog, $mdBottomSheet, $mdMenu, $mdSelect) {

    $scope.navigateTo = function (stateName,objectData) {	
        if ($ionicHistory.currentStateName() != stateName) {
            $ionicHistory.nextViewOptions({
                disableAnimate: false,
                disableBack: true
            });

            //Next view animate will display in back direction
            $ionicViewSwitcher.nextDirection('back');

            $state.go(stateName, {
                isAnimated: objectData,
            }, {reload: true});
        }
    }; // End of navigateTo.
	
	
	$scope.logout = function($event){
		$mdDialog.show({
            controller: 'DialogController',
            templateUrl: 'confirm-dialog.html',
            targetEvent: $event,
            locals: {
                displayOption: {
                    title: "Logout ?",
                    content: "Do you really want to logout ?",
                    ok: "Yes",
                    cancel: "Cancel"
                }
            }
        }).then(function () {
            $state.go('app.login', {
                isAnimated: true,
            });
        }, function () {
            $state.go('app.dashboard', {
                isAnimated: true,
            });
        });		
	}
	
	
});  
