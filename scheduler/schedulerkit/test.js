var sqlutil = require("./lib/sqlutil");
var config = require("./config");

sqlutil.query({
    sqlcon: config.sql.dealer,
    query: "select * from pmBranchOutlets",
}, function (err, data) {
    for (var i = 0; i < 10; i++) {
        console.log(JSON.stringify(data[i]));
    }
});