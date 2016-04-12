$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Daily Posting",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Posting Date",
                        type: "controls",
                        items: [
                            { name: "PostingDate", cls: "span2", readonly: true },
                        ]
                    },
                ],
            },
            { name: "pnlResult", xtype: "k-grid" }
        ],
        toolbars: [
            { name: "btnPost", text: "Posting", icon: "icon-bolt" },
        ],
    });

    widget.render(function () {
        widget.post('gn.api/posting/postingdate', function (result) {
            if (result && result.PostingDate) {
                widget.populate({ PostingDate: moment(result.PostingDate).format("DD MMM YYYY").toUpperCase() });
                $("#btnPost").on("click", posting);
            }
            else {
                widget.populate({ PostingDate: '-' });
                var html = '<br/><div style="color:blue"><i>Tidak ditemukan data yang perlu diposting...';
                html += '</i></div>';
                $('#pnlResult').html(html);
                $('#btnPost').attr('disabled', 'disabled');
            }
        });
    });

    function posting() {
        var filter = widget.serializeObject('pnlFilter');
        var message1 = 'Proses Posting<br/><small>Proses Posting akan dilakukan<br/>Pastikan semua transaksi sudah diselesaikan<br/>Data yang sudah diproses tidak bisa dimodifikasi</small>';
        var message2 = 'Konfirmasi Proses Posting<br/><small>Apakah anda yakin untuk posting tgl <strong>' + filter.PostingDate + '</strong><br/>Data yang sudah diproses tidak bisa dimodifikasi lagi<br/>Proses dilanjutkan?<small>';
        widget.confirm(message1, function (action) {
            if (action == "Yes") {
                widget.confirm(message2, function (action) {
                    if (action == "Yes") {
                        widget.post('gn.api/posting/dailyposting', function (result) {
                            if (result.ErrorNumber && result.ErrorMessage) {
                                var html = '<br/><div style="color:red"><i>' + result.ErrorMessage + '</i></div>';
                                $('#pnlResult').html(html);
                                console.log(result);
                            }
                            else {
                                var html = '<br/><div style="color:blue"><i>Posting berhasil dilakukan';
                                html += ', ' + result.PostingLog + ' pada ' + moment(result.CreatedDate).format("DD-MMM-YYYY HH:mm:ss");
                                html += '</i></div>';
                                $('#pnlResult').html(html);
                                $('#btnPost').attr('disabled', 'disabled');
                            }
                        });
                    }
                });
            }
            else {
                $('#pnlResult').html('');
            }
        })
    }
});