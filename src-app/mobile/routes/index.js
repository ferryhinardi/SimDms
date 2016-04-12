var fs = require('fs');
var path = require('path');

module.exports = {
    start: function (app, http) {
        fs.readdir(path.join(__dirname, "./api"), function (err, files) {
            for (var i in files) {
                var controller = files[i].substr(0, files[i].length - 3);
                for (var action in require(path.join(__dirname, "./api", controller))) {
                    var link = "/api/" + controller + "/" + action + "/:id?";
                    app.post(link, require(path.join(__dirname, "./api", controller))[action]);
                }
            }

            http.createServer(app).listen(app.get('port'), function () {
                console.log('Express server listening on port ' + app.get('port'));
            });
        });
    },
    map: function (map) {
        return function (req, res) {
            fs.readFile(path.join(__dirname, '../client', map + '.html'), 'utf8', function (err, data) {
                if (err) { throw err };

                res.send(data);
            })
        }
    }
}
