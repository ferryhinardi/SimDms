var config = module.exports = {};

//sql database (all)
config.sql = {};
//config.sql.simdms = { user: 'sa', password: 'P4ssw0rd-01', server: 'tbsdmsdb01', database: 'SimDms' };
//config.sql.dealer = { user: 'sa', password: 'P4ssw0rd-01', server: 'localhost', database: 'SDMS' };

config.sql.simdms = { user: 'sa', password: '123', server: '127.0.0.1', database: 'SimDms' };


//dealer config (dlr)
config.dlr = {};
config.dlr.code = "";
config.dlr.simdata = 'http://dms.suzuki.co.id/simdata_dev/';
config.dlr.upload = "http://dms.suzuki.co.id/upload_dev";
config.dlr.uplcd = "UPLCD";

//backup config (dlr)
config.backup = {};
config.backup.options = {
    sqlcon: config.sql.dealer,
    method: "parallel",
    segment: 1000,
    zip: true,
};
