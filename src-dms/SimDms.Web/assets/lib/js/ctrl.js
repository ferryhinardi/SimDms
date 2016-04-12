(function () {
    'use strict';

    angular.module('simdms', [])

    .directive('ngEnter', function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if(event.which === 13) {
                    scope.$apply(function(){
                        scope.$eval(attrs.ngEnter);
                    });                        
                    event.preventDefault();
                }
            });
        };
    })

	.controller('BarcodeCtrl', function TodoCtrl($scope, $location, $http) {
	    
	    $scope.test = "VIN";

	    $scope.allowSave = false;

	    $scope.VIN = "";
	    $scope.Driver = "";

	    $scope.data = {
	        Engine : '',
	        SalesModel : '',
	        Colour  : '',
	        ServiceBookNo  : '',
	        KeyNo  : '',
	        Dealer : '',
	        DONo  : '',
	        SJNo: ''
	    }

	    $scope.clear = function () {
	        $scope.data = {
	            Engine: '',
	            SalesModel: '',
	            Colour: '',
	            ServiceBookNo: '',
	            KeyNo: '',
	            Dealer: '',
	            DONo: '',
	            SJNo: ''
	        };
	        $scope.VIN = "";
	        $scope.Driver = "";
	        $("#txtVIN").focus();
	        $scope.allowSave = false;
	    }


	    $scope.loadByVIN = function (e) {

	        $scope.allowSave = false;

	        $http.post('http://localhost:13629/om.api/ReceivingUnit/GeData?Vehicle=' + $scope.VIN).
               success(function (data, status, headers, config) {

                   $scope.data = {
                       Engine : data.data.EngineCode + data.data.EngineNo,
                       SalesModel : data.data.SalesModelCode + data.data.SalesModelYear,
                       Colour : data.Colour + '(' + data.data.ColourCode + ')',
                       ServiceBookNo : data.data.ServiceBookNo,
                       KeyNo : data.data.KeyNo,
                       Dealer : data.DealerAbbreviation + '(' + data.data.CompanyCode + ')',
                       DONo : data.data.DONo,
                       SJNo : ""
                   }

                   $scope.allowSave = true;
                   
               }).
               error(function (data, status, headers, config) {
                   console.log(data);
                   alert(data);
                   $scope.allowSave = false;
               });



	    }

	});
})();
