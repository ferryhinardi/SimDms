var request = require('request');
var moment = require('moment');
var config = require('../../config');

var params = {
    uri: config.req.simdata,
    method: "POST",
    form: {
        DealerCode: "6006408",
        TableName: "GnMstEmployee"
    }
};

request(params, function (e, r, body) {
    console.log(body, moment(body).format("YYYYMMDD"));
});
