var sqljob = require('../../lib/sqljob');
var config = require('../../config');

exports.start = function () {
    sqljob.run({
        name: "sync_its",
        info: "sync its data",
        duration: { d: 0, h: 1, m: 0, s: 0 },
        method: "series",
        sqlcon: config.sql.dealer,
        tasks: [
			{ name: "task1", command: "exec uspfn_SyncPmHstIts", info: "sync data to PmHstIts" },
        ]
    });

    sqljob.run({
        name: "sync_cs",
        info: "sync cs data",
        duration: { d: 0, h: 1, m: 0, s: 0 },
        method: "series",
        sqlcon: config.sql.dealer,
        tasks: [
			{ name: "task1", command: "exec uspfn_SyncCsLkuTDayCallView", info: "sync data to CsLkuTDayCallView" },
			{ name: "task2", command: "exec uspfn_SyncCsLkuStnkExtView", info: "sync data to CsLkuStnkExtView" },
			{ name: "task3", command: "exec uspfn_SyncCsLkuBpkbView", info: "sync data to CsLkuBpkbView" },
			{ name: "task4", command: "exec uspfn_SyncCsLkuBirthdayView", info: "sync data to CsLkuBirthdayView" },
        ]
    });

    sqljob.run({
        name: "sp_job",
        info: "sparepart job",
        duration: { d: 0, h: 3, m: 0, s: 0 },
        method: "series",
        sqlcon: config.sql.dealer,
        tasks: [
			{ name: "task1", command: "exec uspfn_spAutomaticOrderSparepart '6006406','6006406'", info: "AOS ReOrder" },
        ]
    });
}
