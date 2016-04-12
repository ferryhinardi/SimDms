appControllers.controller('ITSDealerCtrl', function ($scope,$state,$stateParams,$filter, $http, BASE_ADDRESS, BASE_COLOR, $timeout, ionicMaterialMotion, ionicMaterialInk) {

    $scope.isAnimated = $stateParams.isAnimated;

    $scope.isNasional = false;
    $scope.HeaderTitle = "ITS Monitoring - Dealer";
    $scope.Title = "Previous vs Current </br>Stock, Invoice, INQ, SPK";
        
    $scope.data = [];
    $scope.filterData = [];
    $scope.listComparation = [];
    $scope.ListBranchs = [];
    $scope.ListFilterBranchs = [];
    $scope.lastUpdateTime = "";
    $scope.currentCompany = "";
    $scope.flag = 0;
    

    $scope.options = {
        data: [],
        selectedOption: [],
        selectedOption2: [],
        DisplayType: "T"
    };
	
	$scope.loadDealer = function(cb) {	

		$http.post(BASE_ADDRESS + "ListOutletDealer")
        .success(function (ret) {
            $scope.options.data = ret.data.Table;
            $scope.ListBranchs = ret.data.Table1;
        })
        .error(function (a, b, c, d) {
            console.log(a, b, c, d)
        })

		$http.post(BASE_ADDRESS + "CurrentCompany")
        .success(function (ret) {
            var temp;
            $scope.currentCompany = ret;
			console.log($scope.currentCompany)
            if ($scope.currentCompany == "")
                $scope.disabledCompany = false;
            else {
                $scope.disabledCompany = true;
                temp = $filter('filter')($scope.options.data, { DealerCode: ret });
            }
        })
        .error(function (a, b, c, d) {
            console.log(a, b, c, d)
        })
		
		if (cb) cb();
	}
    
	$scope.loadDealer();

    $scope.
        $watch('options.selectedOption', function(company) {
            var CompanyCode = "ALL";
            if (company.length > 0)
                CompanyCode = company;
            $scope.selectedCompany = CompanyCode;
            $scope.ListFilterBranchs = $filter('filter')($scope.ListBranchs, { DealerCode: CompanyCode });
            $scope.selectedOutlet = "ALL";
            GetDataExecutiveSummary(null);
        });
    
    $scope.
        $watchGroup(['options.selectedOption2', 'options.DisplayType'], 
        function(newValues, oldValues, scope) {
            var OutletCode = (newValues[0] == null || newValues[0] == undefined || typeof newValues[0] != "string") ? "ALL" : newValues[0]; // OutletCode
            var DisplayType = (newValues[1] == null || newValues[1] == undefined) ? $scope.options.DisplayType : newValues[1];
            console.log({ BranchCode: OutletCode, DisplayType: DisplayType })

            $scope.selectedOutlet = OutletCode;
            $scope.filterData = $filter('filter')($scope.data, { BranchCode: OutletCode, DisplayType: DisplayType });
            if ($scope.filterData.length == 0) 
                $scope.filterData = reset();
            else 
                $scope.filterData = $scope.filterData[0];

            GetExecutiveSummary();
        });

    $scope.doRefresh = function() {
		$scope.loadDealer(function(){
		
			if ($scope.currentCompany == "")
                $scope.disabledCompany = false;
            else {
                $scope.disabledCompany = true;
                temp = $filter('filter')($scope.options.data, { DealerCode: $scope.currentCompany });
            }
			
			GetDataExecutiveSummary(function() {
				$scope.$broadcast('scroll.refreshComplete');
			})			
		});

    };

    function GetDataExecutiveSummary(Callback) {
        if ($scope.selectedCompany == "ALL") $scope.filterData = reset();
        var BranchCode = (typeof $scope.selectedOutlet != "string") ? "ALL" : $scope.selectedOutlet;
        console.log($scope.selectedCompany, BranchCode, $scope.options.DisplayType);

        //$scope.data = dataDemo.data.Table;
        //$scope.filterData = $filter('filter')($scope.data, { BranchCode: BranchCode, DisplayType: $scope.options.DisplayType });
        //if ($scope.filterData.length == 0) 
        //    $scope.filterData = reset();
        //else 
        //    $scope.filterData = $scope.filterData[0];
        //GetExecutiveSummary();

        // if (typeof Callback === "function") {
        //    Callback();
        // }

        var data = {
            id: $scope.selectedCompany
        };

        $http.get(BASE_ADDRESS + "DealerExecutiveSummary/" + $scope.selectedCompany )
            .success(function (ret) {
				console.log(ret)
                $scope.data = ret.data.Table;
                $scope.filterData = $filter('filter')($scope.data, { BranchCode: BranchCode, DisplayType: $scope.options.DisplayType });
                if ($scope.filterData.length == 0) 
                    $scope.filterData = reset();
                else 
                    $scope.filterData = $scope.filterData[0];
                GetExecutiveSummary();
            })
            .error(function (a, b, c, d) {
                console.log(a, b, c, d)
            })
            .then(function() {
                GetExecutiveSummary();
                if (typeof Callback === "function") {
                    Callback();
                }
            })
    }

    function reset() {
        var data = {
            CurrentDate: new Date(),
            PrevMonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-1, new Date().getDate()),
            Prev2MonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-2, new Date().getDate()),
            PeriodOfCurrMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate(),
            PeriodOfPrevMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth(), 0).getDate(),
            CurrentStock: 0,
            LastDateStock: 0,
            PreviousStock: 0,
            Previous2Stock: 0,
            CurrentInv: 0,
            LastDateInv: 0,
            PreviousInv: 0,
            Previous2Inv: 0,
            CurrentInq: 0,
            LastDateInq: 0,
            PreviousInq: 0,
            Previous2Inq: 0,
            CurrentSpk: 0,
            LastDateSpk: 0,
            PreviousSpk: 0,
            Previous2Spk: 0,
            LastUpdateDate: ""
        };
        return data;
    }

    function GetExecutiveSummary() {
        $scope.listComparation = [];
        var data = $scope.filterData;
        // console.log(data);
        $scope.lastUpdateTime = (data.LastUpdateDate == "") ? "No Connection" : moment(data.LastUpdateDate, "YYYY-MM-DD hh:mm:ss").fromNow();
        var dataStock = {
            name: 'STOCK',
            image: 'do',
            cls: 'font-stock-white',
            pcls: 'color-white',
            color: BASE_COLOR.STOCK,
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentStock),
            prevToDate: NumDigit(data.LastDateStock),
            prevTotal: NumDigit(data.PreviousStock),
            prev2Total: NumDigit(data.Previous2Stock),
            CmpToDate: (data.CurrentStock == 0 || data.LastDateStock == 0) ? "-" : NumDigit((data.CurrentStock / data.LastDateStock) * 100) + '%',
            CmpToTotal: (data.CurrentStock == 0 || data.PreviousStock == 0) ? "-" : NumDigit((data.CurrentStock / data.PreviousStock) * 100) + '%',
            CmpTo2Total: (data.CurrentStock == 0 || data.Previous2Stock == 0) ? "-" : NumDigit((data.CurrentStock / data.Previous2Stock) * 100) + '%'

        }
        $scope.listComparation.push(dataStock);
        var dataINV = {
            name: 'INVOICE',
            image: 'invoice',
            cls: 'font-stock-white',
            pcls: 'color-white',
            color: BASE_COLOR.INVOICE,
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInv),
            prevToDate: NumDigit(data.LastDateInv),
            prevTotal: NumDigit(data.PreviousInv),
            prev2Total: NumDigit(data.Previous2Inv),
            CmpToDate: (data.CurrentInv == 0 || data.LastDateInv == 0) ? "-" : NumDigit((data.CurrentInv / data.LastDateInv) * 100) + '%',
            CmpToTotal: (data.CurrentInv == 0 || data.PreviousInv == 0) ? "-" : NumDigit((data.CurrentInv / data.PreviousInv) * 100) + '%',
            CmpTo2Total: (data.CurrentInv == 0 || data.Previous2Inv == 0) ? "-" : NumDigit((data.CurrentInv / data.Previous2Inv) * 100) + '%'
        }
        $scope.listComparation.push(dataINV);

        var beforeDate = moment(data.CurrentDate).add(-1, 'days');

        var dataInq = {
            name: 'INQUIRY',
            image: 'inquiry',
            cls: 'font-stock-black',
            pcls: '',
            color: BASE_COLOR.INQUIRY,
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInq),
            prevToDate: NumDigit(data.LastDateInq),
            prevTotal: NumDigit(data.PreviousInq),
            prev2Total: NumDigit(data.Previous2Inq),

            isITS: true,
            out1name: 'OUTS. ' + moment(beforeDate).format('D MMM').toUpperCase(),
            out1: NumDigit(data.OUT1INQ),
            funame: 'FU    ' + moment(beforeDate).format('D MMM').toUpperCase(),
            fu: NumDigit(data.FUINQ),
            out2name: 'OUTS. ' + moment(data.CurrentDate).format('D MMM').toUpperCase(),
            out2: NumDigit(data.OUT2INQ),

            CmpToDate: (data.CurrentInq == 0 || data.LastDateInq == 0) ? "-" : NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%',
            CmpToTotal: (data.CurrentInq == 0 || data.PreviousInq == 0) ? "-" : NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%',
            CmpTo2Total: (data.CurrentInq == 0 || data.Previous2Inq == 0) ? "-" : NumDigit((data.CurrentInq / data.Previous2Inq) * 100) + '%'
        }
        $scope.listComparation.push(dataInq);
        var dataSpk = {
            name: 'SPK',
            image: 'spk',
            cls: 'font-stock-black',
            pcls: '',
            color: BASE_COLOR.SPK,
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentSpk),
            prevToDate: NumDigit(data.LastDateSpk),
            prevTotal: NumDigit(data.PreviousSpk),
            prev2Total: NumDigit(data.Previous2Spk),


            isITS: true,
            out1name: 'OUTS. ' + moment(beforeDate).format('D MMM').toUpperCase(),
            out1: NumDigit(data.OUT1SPK),
            funame: 'FU    ' + moment(beforeDate).format('D MMM').toUpperCase(),
            fu: NumDigit(data.FUSPK),
            out2name: 'OUTS. ' + moment(data.CurrentDate).format('D MMM').toUpperCase(),
            out2: NumDigit(data.OUT2SPK),

            CmpToDate: (data.CurrentSpk == 0 || data.LastDateSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%',
            CmpToTotal: (data.CurrentSpk == 0 || data.PreviousSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%',
            CmpTo2Total: (data.CurrentSpk == 0 || data.Previous2Spk == 0) ? "-" : NumDigit((data.CurrentSpk / data.Previous2Spk) * 100) + '%'
        }
        $scope.listComparation.push(dataSpk);
    
        $timeout(function () {
            ionicMaterialMotion.fadeSlideIn({
                selector: '.animate-fade-slide-in .item'
            });
        }, 200);

        // Activate ink for controller
        ionicMaterialInk.displayEffect();
    }
 

});


appControllers.controller('ITSNasionalCtrl', function ($scope,$state,$stateParams,$filter, $http, BASE_ADDRESS, BASE_COLOR, $timeout, ionicMaterialMotion, ionicMaterialInk) {

    $scope.isAnimated = $stateParams.isAnimated;

    $scope.isNasional = true;
    $scope.HeaderTitle = "ITS Monitoring - National";
    $scope.Title = "Previous vs Current </BR>DO, Faktur Polisi, INQ, SPK";
        
    $scope.data = [];
    $scope.filterData = [];
    $scope.listComparation = [];
    $scope.ListBranchs = [];
    $scope.ListFilterBranchs = [];
    $scope.lastUpdateTime = "";
    $scope.currentCompany = "";
    $scope.flag = 0;
    
    $scope.options = {
        data: [],
        selectedOption: [],
        selectedOption2: [],
        DisplayType: "T"
    };
	
	$scope.dataOption = true;

    $scope.
        $watch('options.DisplayType', 
        function(newValues, oldValues) {
            GetDataExecutiveSummary();
        });

    $scope.doRefresh = function() {
        GetDataExecutiveSummary(function () {
            $scope.$broadcast('scroll.refreshComplete');
        })         
    };

    function GetDataExecutiveSummary(Callback) {

        var url = BASE_ADDRESS;

        if ($scope.options.DisplayType == 'T') {
            url += "DovsFPvsITS";
        } else {
            url += "DovsFPvsITS2";
        }
       
        $http.get(url)
            .success(function (ret) {
				console.log(ret)
				$scope.data = ret.data.Table[0];
                console.log($scope.data)
            })
            .error(function (a, b, c, d) {
                console.log(a, b, c, d)
            })
            .then(function () {
                console.log('then');
                GetExecutiveSummary();
                if (typeof Callback === "function") {
                    Callback();
                }
            })
    }

    function reset() {
        var data = {
            CurrentDate: new Date(),
            PrevMonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-1, new Date().getDate()),
            Prev2MonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-2, new Date().getDate()),
            PeriodOfCurrMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate(),
            PeriodOfPrevMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth(), 0).getDate(),
            CurrentStock: 0,
            LastDateStock: 0,
            PreviousStock: 0,
            Previous2Stock: 0,
            CurrentInv: 0,
            LastDateInv: 0,
            PreviousInv: 0,
            Previous2Inv: 0,
            CurrentInq: 0,
            LastDateInq: 0,
            PreviousInq: 0,
            Previous2Inq: 0,
            CurrentSpk: 0,
            LastDateSpk: 0,
            PreviousSpk: 0,
            Previous2Spk: 0,
            LastUpdateDate: ""
        };
        return data;
    }

    function GetExecutiveSummary() {
        $scope.listComparation = [];
        var data = $scope.data;
        // console.log(data);
        //$scope.lastUpdateTime = (data.LastUpdateDate == "") ? "No Connection" : moment(data.LastUpdateDate, "YYYY-MM-DD hh:mm:ss").fromNow();

        var dataStock = {
            name: 'DO',
            image: 'do',
            cls: 'font-stock-white',
            pcls: 'color-white',
            isITS: false,
            color: BASE_COLOR.STOCK,
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',

            currToDate: NumDigit(data.CurrentDO),
            prevToDate: NumDigit(data.LastDateDO),
            prevTotal: NumDigit(data.PreviousDO),

            CmpToDate: (data.CurrentDO == 0 || data.LastDateDO == 0) ? "-" : NumDigit((data.CurrentDO / data.LastDateDO) * 100) + '%',
            CmpToTotal: (data.CurrentDO == 0 || data.PreviousDO == 0) ? "-" : NumDigit((data.CurrentDO / data.PreviousDO) * 100) + '%',
        }
        $scope.listComparation.push(dataStock);
        var dataINV = {
            name: 'F. POL',
            image: 'faktur',
            isITS: false,
            cls: 'font-stock-white',
            pcls: 'color-white',
            color: BASE_COLOR.INVOICE,
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            
            currToDate: NumDigit(data.CurrentFP),
            prevToDate: NumDigit(data.LastDateFP),
            prevTotal: NumDigit(data.PreviousFP),
             
            CmpToDate: (data.CurrentFP == 0 || data.LastDateFP == 0) ? "-" : NumDigit((data.CurrentFP / data.LastDateFP) * 100) + '%',
            CmpToTotal: (data.CurrentFP == 0 || data.PreviousFP == 0) ? "-" : NumDigit((data.CurrentFP / data.PreviousFP) * 100) + '%',
            
        }
        $scope.listComparation.push(dataINV);

        var beforeDate = moment(data.CurrentDate).add(-1, 'days');
        
        var dataInq = {
            name: 'INQUIRY',
            image: 'inquiry',
            cls: 'font-stock-black',
            pcls: '',
            isITS: true,
            color: BASE_COLOR.INQUIRY,
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
             
            currToDate: NumDigit(data.CurrentInq),
            prevToDate: NumDigit(data.LastDateInq),
            prevTotal: NumDigit(data.PreviousInq),

            out1name: 'OUTS. ' + moment(beforeDate).format('D MMM').toUpperCase(),
            out1: NumDigit(data.OUT1INQ),
            funame: 'FU    ' + moment(beforeDate).format('D MMM').toUpperCase(),
            fu: NumDigit(data.FUINQ),
            out2name: 'OUTS. ' + moment(data.CurrentDate).format('D MMM').toUpperCase(),
            out2: NumDigit(data.OUT2INQ),
            
            CmpToDate: (data.CurrentInq == 0 || data.LastDateInq == 0) ? "-" : NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%',
            CmpToTotal: (data.CurrentInq == 0 || data.PreviousInq == 0) ? "-" : NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%',
            
        }
        $scope.listComparation.push(dataInq);


        var dataSpk = {
            name: 'SPK',
            image: 'spk',
            cls: 'font-stock-black',
            pcls: '',
            color: BASE_COLOR.SPK,
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',

            currToDate: NumDigit(data.CurrentSpk),
            prevToDate: NumDigit(data.LastDateSpk),
            prevTotal: NumDigit(data.PreviousSpk),

            isITS: true,
            out1name: 'OUTS. ' + moment(beforeDate).format('D MMM').toUpperCase(),
            out1: NumDigit(data.OUT1SPK),
            funame: 'FU    ' + moment(beforeDate).format('D MMM').toUpperCase(),
            fu: NumDigit(data.FUSPK),
            out2name: 'OUTS. ' + moment(data.CurrentDate).format('D MMM').toUpperCase(),
            out2: NumDigit(data.OUT2SPK),

            CmpToDate: (data.CurrentSpk == 0 || data.LastDateSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%',
            CmpToTotal: (data.CurrentSpk == 0 || data.PreviousSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%',

        }
        $scope.listComparation.push(dataSpk);

    
        $timeout(function () {
            ionicMaterialMotion.fadeSlideIn({
                selector: '.animate-fade-slide-in .item'
            });
        }, 200);

        // Activate ink for controller
        ionicMaterialInk.displayEffect();
    }
 

});

