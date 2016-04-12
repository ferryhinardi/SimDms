var require = {
    waitSeconds: 0,
    paths: {
        'jquery': 'libs/jquery/jquery-2.1.3.min',
        'bootstrap': 'libs/bootstrap/bootstrap.min',
        'domReady': 'libs/domReady',
        'extjs': 'libs/ext-all'
},
    shim: {
        'extjs': { 'exports': 'Ext', deps: ['jquery','domReady'] }
    },
    priority: [
        'jquery',
        'bootstrap',
        'extjs'
    ]
};