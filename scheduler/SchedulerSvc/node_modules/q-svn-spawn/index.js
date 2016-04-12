/*global require: true */
/*global setImmediate: true */
/*global module: true */

(function () {
	'use strict';

	var Q = require('q'),
		SVNSpawn = require('svn-spawn'),
		QSVNSpawn = function (options) {
			this.swn_spawn = new SVNSpawn(options);
			return this;
		};

	/*
		Allowed functions
		 - cmd
		 - checkout
		 - update
		 - commit
		 - add
		 - del
		 - info
		 - getInfo
		 - status
		 - getStatus
		 - log
		 - getLog
		 - addLocal
		 - addLocalUnversioned
	*/
	
	QSVNSpawn.prototype.runCustom = function (func, params) {
		var deferred = Q.defer();
		setImmediate(function () {

			console.log(func, params);

			if (typeof params === 'undefined')
				this.swn_spawn[func](function (err, data) {
					if (err)
						return deferred.reject(err);
					
					deferred.resolve(data);
				});
			else
				this.swn_spawn[func](params, function (err, data) {
					if (err)
						return deferred.reject(err);
					
					deferred.resolve(data);
				});

		}.bind(this));
		return deferred.promise;
	};

	QSVNSpawn.prototype.cmd = function (params) {
	    return this.runCustom('cmd', params);
	};

	QSVNSpawn.prototype.checkout = function (params) {
	    return this.runCustom('checkout', params);
	};

	QSVNSpawn.prototype.update = function (params) {
	    return this.runCustom('update', params);
	};

	QSVNSpawn.prototype.commit = function (params) {
	    return this.runCustom('commit', params);
	};

	QSVNSpawn.prototype.add = function (params) {
	    return this.runCustom('add', params);
	};

	QSVNSpawn.prototype.del = function (params) {
	    return this.runCustom('del', params);
	};

	QSVNSpawn.prototype.info = function (params) {
	    return this.runCustom('info', params);
	};

	QSVNSpawn.prototype.getInfo = function (params) {
	    return this.runCustom('getInfo', params);
	};

	QSVNSpawn.prototype.status = function (params) {
	    return this.runCustom('status', params);
	};

	QSVNSpawn.prototype.getStatus = function (params) {
	    return this.runCustom('getStatus', params);
	};

	QSVNSpawn.prototype.log = function (params) {
	    return this.runCustom('log', params);
	};

	QSVNSpawn.prototype.getLog = function (params) {
	    return this.runCustom('getLog', params);
	};

	QSVNSpawn.prototype.addLocal = function (params) {
	    return this.runCustom('addLocal', params);
	};

	QSVNSpawn.prototype.addLocalUnversioned = function (params) {
	    return this.runCustom('addLocalUnversioned', params);
	};

	module.exports = QSVNSpawn;

}());

