
var dataServicegnMaster = new breeze.DataService({
    serviceName:  "breeze/gnMaster",
    hasServerMetadata: false
});

var gnManager = new breeze.EntityManager({
    dataService: dataServicegnMaster
});


var dataServiceSparePart = new breeze.DataService({
    serviceName:  "breeze/sparepart",
    hasServerMetadata: false
});

var spManager = new breeze.EntityManager({
    dataService: dataServiceSparePart
});

var dataSales = new breeze.DataService({
    serviceName: "breeze/sales",
    hasServerMetadata: false
});

var spSalesManager = new breeze.EntityManager({
    dataService: dataSales
});

var dataPajak = new breeze.DataService({
    serviceName: "breeze/tax",
    hasServerMetadata: false
});

var TaxManager = new breeze.EntityManager({
    dataService: dataPajak
});

var pengeluaranManager = new breeze.EntityManager({
    dataService: new breeze.DataService({ serviceName:  "breeze/pengeluaran", hasServerMetadata: false })
});

var dataServicePersediaan = new breeze.DataService({
    serviceName:   "breeze/sppersediaan",
    hasServerMetadata: false
});

var spPersediaanManager = new breeze.EntityManager({
    dataService: dataServicePersediaan
});

var dataServicePembelian = new breeze.DataService({
    serviceName:  "breeze/sppembelian",
    hasServerMetadata: false
});

var spPembelianManager = new breeze.EntityManager({
    dataService: dataServicePembelian
});

var dataServicePenerimaan = new breeze.DataService({
    serviceName:  "breeze/sppenerimaan",
    hasServerMetadata: false
});

var spPenerimaanManager = new breeze.EntityManager({
    dataService: dataServicePenerimaan
});


var dataServiceStockOpname= new breeze.DataService({
    serviceName:  "breeze/spstockopname",
    hasServerMetadata: false
});

var spStockOpnameManager = new breeze.EntityManager({
    dataService: dataServiceStockOpname
});

var dataSparepartUtility = new breeze.DataService({
    serviceName:  "breeze/sputility",
    hasServerMetadata: false
});

var SpUtilityManager = new breeze.EntityManager({
    dataService: dataSparepartUtility
});

var svService = new breeze.DataService({
    serviceName: "breeze/svtransaksi",
    hasServerMetadata: false    
});

var svServiceManager = new breeze.EntityManager({
    dataService: svService
});

/** REPORT **/
var spRptPenerimaan = new breeze.DataService({
    serviceName: "breeze/sprptpenerimaan",
    hasServerMetadata: false
});

var spReportPenerimaanManager = new breeze.EntityManager({
    dataService: spRptPenerimaan
});

var spRptInventory = new breeze.DataService({
    serviceName: "breeze/sprptinventory",
    hasServerMetadata: false
});

var spReportInventoryManager = new breeze.EntityManager({
    dataService: spRptInventory
});

var spRptRegisterManager = new breeze.EntityManager({
    dataService: new breeze.DataService({ serviceName: "breeze/sprptregister", hasServerMetadata: false })
});

var spRptPenjualan = new breeze.DataService({
    serviceName: "breeze/sprptpenjualan",
    hasServerMetadata: false
});

var spReportPenjualanManager = new breeze.EntityManager({
    dataService: spRptPenjualan
});

var MasterService = new breeze.EntityManager({
    dataService: new breeze.DataService({serviceName: "breeze/svmaster", hasServerMetadata: false})
});

var ReportService = new breeze.EntityManager({
    dataService: new breeze.DataService({ serviceName: "breeze/svreport", hasServerMetadata: false })
});

var dataServiceUtility = new breeze.DataService({
    serviceName: "breeze/svutility",
    hasServerMetadata: false
});

var SvUtilityManager = new breeze.EntityManager({
    dataService: dataServiceUtility
});

var MasterITS = new breeze.EntityManager({
    dataService: new breeze.DataService({ serviceName: "breeze/itsmaster", hasServerMetadata: false })
});

var UtilityITS = new breeze.EntityManager({
    dataService: new breeze.DataService({ serviceName: "breeze/itsutility", hasServerMetadata: false })
});