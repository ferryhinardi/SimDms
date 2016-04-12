var sqljob = require('../../lib/sqljob');
var config = require('../../config');

exports.start = function () {
    sqljob.run({
        name: "sync_data_insert",
        info: "sync data to suzukir4",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '06:00:00', finish: '09:00:00' }, { start: '10:00:00', finish: '18:00:00' }],
        method: "series",
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task1", command: "exec uspfn_SyncDataInsToSuzukiR4", info: "Sync data insert SDMS to database suzukir4" },
        ]
    });

    sqljob.run({
        name: "sync_data_update",
        info: "sync data to suzukir4",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '09:00:00', finish: '10:00:00' }, { start: '06:00:00', finish: '07:30:00' }],
        method: "series",
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task1", command: "exec uspfn_SyncDataUpdToSuzukiR4", info: "Sync data update SDMS to database suzukir4" },
        ]
    });

    sqljob.run({
        name: "sync_dashboard",
        info: "calculate data PmExecutive Summary",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '07:00:00', finish: '20:00:00' }],
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task1", command: "exec uspfn_SyncDashboard", info: "Sync data for dashboard" },
        ]
    });

    sqljob.run({
        name: "sync_customer",
        info: "synchroize data customer",
        duration: { d: 0, h: 0, m: 30, s: 0 },
        runningtimes: [{ start: '07:00:00', finish: '20:00:00' }],
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task1", command: "exec uspfn_SyncCsCustomerView", info: "extract data to table CsCustomerView each year" },
            { name: "task2", command: "exec uspfn_SyncCsCustomerVehicleView", info: "extract data to table uspfn_SyncCsCustomerVehicleView each year" },
            { name: "task3", command: "exec uspfn_remove_duplicate", info: "remove duplicate data" },
        ]
    });

    sqljob.run({
        name: "backup_db",
        info: "backup database for tbsdmsdb01",
        duration: { d: 0, h: 3, m: 0, s: 0 },
        runningtimes: [{ start: '01:00:00', finish: '06:00:00' }],
        method: "series",
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task0", command: "exec master..sp_clearLogFile", info: "cleanup log file" },
            { name: "task1", command: "exec uspfn_spbackupdatabasesimdms", info: "backup database simdms" },
            { name: "task2", command: "exec uspfn_spbackupdatabasesimdmsdata", info: "backup database simdmsdata" },
            { name: "task3", command: "exec uspfn_SPBackupDatabaseSimDmsDocument", info: "backup database SimDmsDocument" },
            { name: "task4", command: "exec uspfn_SPBackupDatabaseSimDmsR2", info: "backup database simdmsr2" },
            { name: "task5", command: "exec uspfn_spbackupdatabasesdmslink", info: "backup database sdmslink" },
            { name: "task6", command: "exec uspfn_spbackupdatabasesmarketshare", info: "backup database smarketshare" },
            { name: "task7", command: "exec uspfn_SPBackupDatabaseSdmsCisDoc", info: "backup database CIS Doc" },
            { name: "task8", command: "exec uspfn_SPBackupDatabaseSdmsCis", info: "backup database CIS" },
            { name: "task9", command: "exec uspfn_SPBackupDatabaseSdms_Documentation", info: "backup database Sdms_Documentation" },
            { name: "task10", command: "exec uspfn_SPBackupDatabaseSRP", info: "backup database SRP" },
            { name: "task11", command: "exec uspfn_spbackupdatabasesuzukir2", info: "backup database suzuki r2" },
            { name: "task12", command: "exec uspfn_spbackupdatabasesuzukir4", info: "backup database suzuki r4" },
            { name: "task13", command: "exec uspfn_SPBackupDatabaseBTNet", info: "backup database BTNet" }

        ]
    });
	
	sqljob.run({
        name: "sync_unit_intake",
        info: "synchroize data unit intake",
        duration: { d: 0, h: 4, m: 0, s: 0 },
        runningtimes: [{ start: '06:00:00', finish: '23:59:00' }],
        sqlcon: config.serverdb(),
        tasks: [
            { name: "task1", command: "exec uspfn_SyncSvUnitIntake 1", info: "uspfn_SyncGenerateUnitIntake" },
        ]
    });
}


