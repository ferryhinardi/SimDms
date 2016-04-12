var sqlutil = require('../sqlutil');

var config = {
    user: 'sa',
    password: '123',
    server: 'localhost',
    database: 'bit_sby'
}


sqlutil.backup(config, {
    table: 'SpMstItemInfo',
    keys: ['CompanyCode', 'PartNo'],
    zip: true
}, function (err, result) {
    console.log(result);
});
