sdms.print = function (o) {
    if (o.rows) {
        var html = '';
        (o.rows || []).forEach(function (row) {
            html += renderRow(row);
        });

        $('#print').html(html);
    }

    function renderRow(row) {
        var space = (row.space ? 'padding-top:' + row.space + 'px;' : '');
        var html = '<div class="row" style="' + space + '">';

        switch (row.type) {
            case 'pagebreak':
                return '<footer></footer>';
                break;
            case 'title':
                html += '<h1 class="title">' + row.text + '</h1>';
                break;
            case 'table':
                html += renderTable(row);
                break;
            case 'controls':
                html += renderControl(row);
            default:
        }

        html += '</div>';
        return html;
    }

    function renderTable(row) {
        var html = '<div class="col-12">';
        html += '<table class="row">';


        if (row.fields) {
            html += '<thead>';
            html += '<tr>';
            (row.fields || []).forEach(function (field) {
                var width = field.width ? ('width:' + field.width + 'px;') : '';
                var style = (field.style || '') + width;
                switch (field.type) {
                    case 'int':
                        html += '<th style="text-align:right;' + style + '">' + field.text + '</th>';
                        break;
                    case 'decimal':
                        html += '<th style="text-align:right;' + style + '">' + field.text + '</th>';
                        break;
                    default:
                        html += '<th style="' + style + '">' + field.text + '</th>';
                        break;
                }
            });
            html += '</tr>';
            html += '</thead>';

            if (row.data) {
                html += '<tbody>';
                (row.data || []).forEach(function (dtrow) {
                    html += '<tr>';
                    (row.fields || []).forEach(function (field) {
                        switch (field.type) {
                            case 'int':
                                html += '<td style="text-align:right">' + sdms.numberFormat(dtrow[field.name], 0) + '</td>';
                                break;
                            case 'decimal':
                                html += '<td style="text-align:right">' + sdms.numberFormat(dtrow[field.name], 2) + '</td>';
                                break;
                            default:
                                html += '<td>' + dtrow[field.name] + '</td>';
                                break;
                        }
                    });
                    html += '</tr>';
                })
                html += '</tbody>';
            }
        }
        else {
            if (row.data && row.data.length > 0) {
                html += '<thead>';
                html += '<tr>';
                for (var field in row.data[0]) {
                    html += '<th>' + field + '</th>';
                }
                html += '</tr>';
                html += '</thead>';

                html += '<tbody>';
                for (var i = 0; i < row.data.length; i++) {
                    var dtrow = row.data[i];
                    html += '<tr>';
                    for (var field in dtrow) {
                        html += '<td>' + dtrow[field] + '</td>';
                    }
                    html += '</tr>';
                }
                html += '</tbody>';
            }
        }
        html += '</table>';
        html += '</div>';

        return html;
    }

    function renderControl(row) {
        var html = '';
        if (row.controls) {
            (row.controls || []).forEach(function (control) {
                html += '<div class="col-' + (control.cols || '12') + '">';
                (control.data || []).forEach(function (field) {
                    html += '<div class="row-field row">';
                    (field || []).forEach(function (value, idx) {
                        var style = (control.fields && control.fields[idx] && control.fields[idx].style) ? control.fields[idx].style : '';
                        style += (control.fields && control.fields[idx] && control.fields[idx].width) ? ('width:' + control.fields[idx].width + 'px;') : '';

                        html += '<div style="' + style + '">' + value + '</div>';
                    });
                    html += '</div>';
                });
                html += '</div>';
            });
        }

        if (row.data && row.fields) {
            var control = row;
            html += '<div class="col-' + (control.cols || '12') + '">';
            (control.data || []).forEach(function (field) {
                html += '<div class="row-field row">';
                (field || []).forEach(function (value, idx) {
                    var style = (control.fields && control.fields[idx] && control.fields[idx].style) ? control.fields[idx].style : '';
                    style += (control.fields && control.fields[idx] && control.fields[idx].width) ? ('width:' + control.fields[idx].width + 'px;') : '';
                    html += '<div style="' + style + '">' + value + '</div>';
                });
                html += '</div>';
            });
            html += '</div>';
        }

        return html;
    }

    !function print() {
        if (!o.preventDefault) window.print();
    }();
}

var socketClient = io('http://tbsdmsap01:9091');
        
socketClient.on('connect', function(){	
	
    var urlSessionId = "http://tbsdmsap01:9091/sessionid";

    $.ajax({
        async: false,
        type: "GET",
        url: urlSessionId,
    }).done(function (data) {
        console.log(data)
        socketClient.emit('add user', data, data, data, true);
    });
		
    socketClient.on('login', function(info){
        console.log('login', info);
    });		
		
    socketClient.on('user joined', function(info){
        console.log('user joined', info);
    });	
		
    socketClient.on('user left', function(info){
        console.log('user left', info);
    });	
		
    //socketClient.emit('pm', param);

    socketClient.on('pm', function(from, command){
        console.log('pm: ', from,' > ', command);			
        socketClient.emit('reply',from,command + ' result');
    });


    socketClient.on('ping', function(info){
        console.log('ping: ', info);
    });	

    socketClient.on('log', function(info){
        console.log('log: ', info);
    });
		
    socketClient.on('command', function(from, command){

    });	
		
});