require.config({
    baseUrl: '/content/scripts/sys'
});

require
(
    [
        'main'
    ],
    function (main) {
        angular.bootstrap(document, ['main']);
    }
);