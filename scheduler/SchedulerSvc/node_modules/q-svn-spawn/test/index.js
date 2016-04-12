var Client = require('../'),
	fs = require('fs'),
	workingPath = __dirname + '/tmp/copy',
	repo = 'file://' + __dirname + '/tmp/repo',
	client = new Client({
		cwd: workingPath,
	});

module.exports = {
	'test checkout': function (test) {
		client.checkout(repo).done(function (data) {
			test.done();
		});
	},

	'test info': function (test) {
		client.getInfo().done(function (data) {
			test.ok('url' in data);
			test.done();
		});
	},

	'test update': function (test) {
		client.update().done(function (data) {
			test.ok(data.indexOf('At revision') !== -1);
			test.done();
		});
	},

	'test status': function (test) {
		client.getStatus().done(function (data) {
			test.done();
		});
	},

	'test log': function (test) {
		client.getLog().done(function (data) {
			test.ok('author' in data[0]);
			test.done();
		});
	},

	'test add': function (test) {
		fs.writeFileSync(workingPath + '/a.txt', new Date().toString());

		client.addLocal().done(function (data) {
			test.done();
		});
	},

	'test delete': function (test) {
		client.del('b.txt').done(function (data) {
			test.done();
		});
	},

	'test commit': function (test) {
		fs.writeFileSync(workingPath + '/a.txt', new Date().toString());

		client.addLocal().then(function () {
			return client.commit('test commit');
		}).then(function () {
			test.done();
		});
	},

	'test update': function (test) {
		client.update().done(function (data) {
			test.done();
		});
	},
};