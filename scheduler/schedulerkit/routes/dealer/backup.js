var sqlbak = require('../../lib/sqlbak');

var csTables = [
	{ name: "CsBpkbRetrievalInformation", filter: { field: "UpdatedDate" } },
	{ name: "CsCustBirthDay", filter: { field: "UpdatedDate" } },
	{ name: "CsCustBpkb", filter: { field: "UpdatedDate" } },
	{ name: "CsCustData", filter: { field: "UpdatedDate" } },
	{ name: "CsCustomerVehicle", filter: { field: "UpdatedDate" } },
	{ name: "CsStnkExt", filter: { field: "LastUpdatedDate" } },
	{ name: "CsTDayCall", filter: { field: "UpdatedDate" } }
]

var mpTables = [
	{ name: "HrEmployee", filter: { field: "UpdatedDate" } },
	{ name: "HrEmployeeAchievement", filter: { field: "UpdatedDate" } },
	{ name: "HrEmployeeMutation", filter: { field: "UpdatedDate" } },
	{ name: "HrEmployeeSales", filter: { field: "CreatedDate" } },
	{ name: "HrEmployeeTraining", filter: { field: "UpdatedDate" } }
];

var itsTables = [
	{
		name: 'OmHstInquirySales',
		filter: { field: "LastUpdateDate" },
		keys: ['CompanyCode', 'BranchCode', 'Year', 'Month', 'SONo', 'InvoiceNo', 'ChassisCode', 'ChassisNo']
	},
	{ name: "PmActivities", filter: { field: "LastUpdateDate" } },
	{ name: "PmBranchOutlets", filter: { field: "LastUpdateDate" } },
	{ name: "PmKdpAdditional", filter: { field: "LastUpdateDate" } },
	{ name: "PmHstITS", filter: { field: "LastUpdateDate" } }
]

var sdmsTables = [
	{ name: "GnMstCustomer", filter: { field: "LastUpdateDate" } },
	{ name: "svMstCustomerVehicle", filter: { field: "LastupdateDate" } },
	{ name: "omTrSalesBPK", filter: { field: "LastUpdateDate" } },
	{ name: "omTrSalesDO", filter: { field: "LastUpdateDate" } },
	{ name: "omTrSalesDODetail", filter: { field: "LastUpdateDate" } },
	{ name: "omTrSalesInvoice", filter: { field: "LastUpdateDate" } },
	{ name: "omTrSalesInvoiceVin", filter: { field: "LastUpdateDate" } },
	{ name: "omTrSalesSO", filter: { field: "LastUpdateDate" } },
	{ name: "OmTrSalesSOVin", filter: { field: "LastUpdateDate" } },
	{ name: "svMstCustomerVehicle", filter: { field: "LastupdateDate" } }
];


module.exports = {
    start: function () {		
		//CS
		setInterval(function () {
            sqlbak.start(csTables);
        }, 1000 * 60 * 60 * 1);
		
		//MP
		setInterval(function () {
            sqlbak.start(mpTables);
        }, 1000 * 60 * 60 * 1);
		
		//ITS
		setInterval(function () {
            sqlbak.start(itsTables);
        }, 1000 * 60 * 60 * 8);
		
		//SDMS
		setInterval(function () {
            sqlbak.start(sdmsTables);
        }, 1000 * 60 * 60 * 8);	
		
        sqlbak.start(csTables);
		sqlbak.start(mpTables);
		sqlbak.start(sdmsTables);
    },
    upload: function () {
        sqlbak.upload();
    }
}
