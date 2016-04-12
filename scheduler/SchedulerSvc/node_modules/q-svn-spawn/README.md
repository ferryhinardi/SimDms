[![Build Status](https://travis-ci.org/muchweb/q-svn-spawn.svg?branch=master)](https://travis-ci.org/muchweb/q-svn-spawn)
[![Dependency Status](https://gemnasium.com/muchweb/q-svn-spawn.svg)](https://gemnasium.com/muchweb/q-svn-spawn)
[![NPM version](https://badge.fury.io/js/q-svn-spawn.svg)](http://badge.fury.io/js/q-svn-spawn)

# q-svn-spawn

Wrapper for [ddliu](https://github.com/ddliu)'s [svn-spawn](https://github.com/ddliu/node-svn-spawn) that makes use of [Q promises](https://github.com/kriskowal/q); an easy way to access svn repository with node.js.

## Features

- Easy to use
- Fast way to add local changes
- Query SVN infomation as array or object
- Common SVN commands are all supported

## Usage

Create SVN client instance

```js
var QSVNSpawn = require('q-svn-spawn'),
  client = new QSVNSpawn({
    cwd: '/path to your SVN working directory',

    // Optional, authentication not required if already saved
    username: 'username',
    password: 'password',
  });
```

`svn update`

```js
client.update().done(function (data) {
  console.log('Updated');
});
```

`svn info`

```js
client.getInfo().done(function (data) {
  console.log('Repository url is %s', data.url);
});
```

Make some changes and commit all

```js
console.log('Adding local changes to commit');
client.addLocal()
  .then(function () {
    console.log('Committing to repository');
    return client.commit('commit message here');
  })
  .done(function () {
    console.log('Local changes has been committed!');
  });
```

Adding single file

```js

client.add('relative/path/to/file')
  .then(function () {
    return client.commit(['commit message here', 'relative/path/to/file']);
  })
  .done(function (data) {
    console.log('Committed one file!', data);
  });
```

Run any SVN command

```js
client.cmd(['subcommand', '--option1=xx', '--option2=xx', 'arg1', 'arg2']).done(function (data) {
  console.log('Subcommand done');
});
```

## Result Object

For return value documentation please refer to [svn-spawn documentation](https://github.com/ddliu/node-svn-spawn#result-object).

## Requirements

You need to have the `svn` command installed.

## Installation

```bash
npm install q-svn-spawn
```

## Testing

```bash
npm test
```