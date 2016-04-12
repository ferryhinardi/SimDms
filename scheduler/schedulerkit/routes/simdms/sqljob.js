var sqljob = require('../../lib/sqljob');
var config = require('../../config');

exports.start = function () {
    sqljob.run({
        name: "sync_data_insert",
        info: "sync data to suzukir4",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '06:00:00', finish: '09:00:00' }, { start: '10:00:00', finish: '18:00:00' }],
        method: "series",
        sqlcon: config.sql.simdms,
        tasks: [
            { name: "task1", command: "exec uspfn_SyncDataInsToSuzukiR4", info: "Sync data insert SDMS to database suzukir4" },
        ]
    });

    sqljob.run({
        name: "sync_data_update",
        info: "sync data to suzukir4",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '06:00:00', finish: '07:30:00' }, { start: '20:00:00', finish: '21:30:00' }],
        method: "series",
        sqlcon: config.sql.simdms,
        tasks: [
            { name: "task1", command: "exec uspfn_SyncDataUpdToSuzukiR4", info: "Sync data update SDMS to database suzukir4" },
        ]
    });

    sqljob.run({
        name: "sync_dashboard",
        info: "calculate data PmExecutive Summary",
        duration: { d: 0, h: 0, m: 10, s: 0 },
        runningtimes: [{ start: '07:00:00', finish: '20:00:00' }],
        method: "parallel",
        sqlcon: config.sql.simdms,
        tasks: [
            { name: "task1", command: "exec uspfn_SyncPmExecSummary", info: "extract data to table PmExecSummaryView" },
            { name: "task2", command: "exec uspfn_SyncPmExecSummary2", info: "extract data to table PmExecSummaryView2" },
            { name: "task3", command: "exec uspfn_SyncPmExecSummary3", info: "Eksekutif Summary Report by Month" },
            { name: "task4", command: "exec uspfn_SyncPmFakturPolisi", info: "sync data faktur polisi" },
        ]
    });

    sqljob.run({
        name: "sync_customer",
        info: "synchroize data customer",
        duration: { d: 0, h: 0, m: 10, s: 0 },
        runningtimes: [{ start: '07:00:00', finish: '20:00:00' }],
        sqlcon: config.sql.simdms,
        tasks: [
            { name: "task1", command: "exec uspfn_SyncCsCustomerView", info: "extract data to table CsCustomerView each year" },
            { name: "task2", command: "exec uspfn_SyncCsCustomerVehicleView", info: "extract data to table uspfn_SyncCsCustomerVehicleView each year" },
        ]
    });

    sqljob.run({
        name: "backup_db",
        info: "backup database for tbsdmsdb01",
        duration: { d: 0, h: 3, m: 0, s: 0 },
        runningtimes: [{ start: '17:00:00', finish: '21:20:00' }],
        method: "series",
        sqlcon: config.sql.simdms,
        tasks: [
            { name: "task1", command: "exec uspfn_spbackupdatabasesimdms", info: "backup database simdms" },
            { name: "task2", command: "exec uspfn_spbackupdatabasesimdmsdata", info: "backup database simdmsdata" },
            { name: "task3", command: "exec uspfn_spbackupdatabasesdmslink", info: "backup database sdmslink" },
            { name: "task4", command: "exec uspfn_spbackupdatabasesuzukir2", info: "backup database suzuki r2" },
            { name: "task5", command: "exec uspfn_spbackupdatabasesuzukir4", info: "backup database suzuki r4" },
            { name: "task6", command: "exec uspfn_spbackupdatabasesmarketshare", info: "backup database smarketshare" }
        ]
    });
}
