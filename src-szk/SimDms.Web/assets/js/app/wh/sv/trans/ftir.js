var widget = new SimDms.Widget({
    title: 'Field Technical Information Report',
    xtype: 'panels',
    //autorender: true,
    toolbars: [
        //{ action: 'demo', text: 'Demo', icon: 'fa fa-file', name: "demo" },
        { action: 'add', text: 'New', icon: 'fa fa-file', name: "add" },
        { action: 'save', text: 'Save', icon: 'fa fa-save', name: "save" },
        { action: 'browse', text: 'Browse', icon: 'fa fa-search' },
        { action: 'print', text: 'Print Claim Tag Warranty', icon: 'fa fa-print', name: 'printtag', disabled: true },
        { action: 'printForm', text: 'Print Form', icon: 'fa fa-print', name: 'printform', disabled: true },
    ],
    panels: [
        {
            name: 'pnlFtir1',
            type: 'text-left',
            items: [
                { name: 'FtirNo', text: 'No FTIR', cls: 'span3', readonly: true },
                { name: 'ScanNo', text: 'No. FTIR on SCAN', cls: 'span3', placeholder: 'No. FTIR on SCAN' },
                { name: 'FtirDate', text: 'Tanggal Dibuat', cls: 'span3', type: 'datepicker', disabled: true, required: true },
                { name: 'FtirEventDate', text: 'Tanggal Perbaikan', cls: 'span3', type: 'datepicker', required: true },
                { name: 'FtirMaker', text: 'Nama Pembuat', cls: 'span8', required: true },                
                {
                    text: 'Dealer', type: 'controls',
                    items: [
                        { name: 'DealerCode', text: 'Company Code', cls: 'span2', readonly: true },
                        { name: 'OutletCode', text: 'Branch Code', cls: 'span2', readonly: true },
                        { name: 'DealerName', text: 'Nama Dealer', cls: 'span4', readonly: true },
                    ]
                },
                {
                    text: 'VIN / Model', type: 'controls', cls: 'span8',
                    items: [
                        { name: 'VinNo', text: 'VIN', cls: 'span3', required: true },
                        { name: 'Model', text: 'Model', cls: 'span5', required: true, readonly: true },
                    ]
                },
                {
                    text: 'No Mesin / Transmisi', type: 'controls', cls: 'span8',
                    items: [
                        { name: 'Machine', text: 'No Mesin', cls: 'span3', required: true, readonly: true },
                        { name: 'TransmNo', text: 'No Transmisi', cls: 'span5', required: true, readonly: true }
                    ]
                },
                { name: 'FtirRegDate', text: 'Tanggal Registrasi', cls: 'span3 full', required: true, type: 'datepicker' },
                {
                    name: 'FtirCategory', text: 'Kategori FTIR', type: 'select', required: true, cls: 'span5 full',
                    items: [
                        { value: 'I', text:  'I.  Kasus kecelakaan, kebakaran dan sejenisnya' },
                        { value: 'II', text: 'II. General atau Umum' },
                        //{ value: 'III', text: 'III. Konsultasi' },
                        //{ value: 'IV', text: 'IV.  Reguler FTIR (dokumen pelengkap klaim warranty)' },
                       // { value: 'V', text: 'Z. Lain-lain' },
                    ]
                },
                {
                    text: 'Bagian/Judul', type: 'controls', cls: 'span8',
                    items: [
                        {
                            name: 'TitleCategory', type: 'select', cls: 'span3', required: true,
                            items: [
                                { value: 'ENGINE', text: 'ENGINE' },
                                { value: 'TRANSMISSION', text: 'TRANSMISSION' },
                                { value: 'ELECTRICAL', text: 'ELECTRICAL' },
                                { value: 'SUSPENSION', text: 'SUSPENSION' },
                                { value: 'BRAKE', text: 'BRAKE' },
                                { value: 'BODY', text: 'BODY' },
                            ]
                        },
                        { name: 'TitleName', text: 'Judul', cls: 'span5', required: true },
                    ]
                },
                {
                    text: 'Jarak Tempuh', type: 'controls', cls: 'span8', required: true,
                    items: [
                        { name: 'Odometer', type: 'int', cls: 'span2' },
                        { type: 'label', text: 'km &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Waktu Pemakaian', cls: 'span1' },
                        { name: 'UsageTime', cls: 'span2', readonly: true },
                        { type: 'label', text: 'hari', cls: 'span1' }
                    ]
                },
            ]
        },
        
        {
            type: 'text-left',
            title: 'Uraian Kejadian',
            items: [
                { name: 'EvtInfoA01', text: 'Keluhan Pelanggan', type: 'textarea', required: true },
                {
                    name: 'EvtInfoA02', text: 'Masalah dapat timbul kembali', type: 'radio-switch', cls: "full-length"
                },
            ]
        },
        {
            type: 'text-left',
            items: [
                { name: 'EvtInfoA03', text: 'Apa', type: 'textarea', placeholder: 'Apa masalah yang terjadi', required: true },
            ]
        },
        {
            type: 'text-left',
            items: [
                { name: 'EvtInfoA06', text: 'Bagaimana', type: 'textarea', placeholder: 'Bagaimana terjadinya masalah', required: true },
                {
                    text: 'Frekuensi', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoA07', cls: 'span2 text-right', type: 'int' },
                        { type: 'label', cls: 'span1', text: 'beberapa kali', disabled: true },
                        {
                            name: 'EvtInfoA08', cls: 'span2', type: 'select',
                            items: [
                                { value: 'day', text: 'Hari' },
                                { value: 'week', text: 'Minggu' },
                                { value: 'month', text: 'Bulan' },
                                { value: 'every', text: 'Setiap Saat' },
                            ]
                        },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Uraian Kejadian',
            items: [
                { name: 'EvtInfoB01', text: 'Kapan', type: 'textarea', placeholder: 'Kapan masalah tersebut terjadi', required: true },
                { name: 'EvtInfoB01Chk01', text: 'Saat mesin distarter', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk02', text: 'Sesudah mesin distarter', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk03', text: 'Saat mesin dimatikan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk04', text: 'Percepatan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk05', text: 'Melaju', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk06', text: 'Perlambatan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk07', text: 'Pengereman', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk08', text: 'Sebelum Stop', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk09', text: 'Stationer (idle)', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk10', text: 'Belok Kanan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk11', text: 'Belok Kiri', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk12', text: 'Jalan Menanjak', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk13', text: 'Jalan Menurun', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk14', text: 'Mundur', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk15', text: 'Jalan Macet', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk16', text: 'Perpindahan Gigi', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoB01Chk17', text: 'Jalan terjal / kasar', type: 'radio-switch', cls: 'span4 full', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    name: 'EvtInfoB02', text: 'Pemanasan Mesin', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Tidak Berhubungan' },
                        { value: '2', text: '2. Sebelum Pemanasan' },
                        { value: '3', text: '3. Sesudah Pemanasan' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    name: 'EvtInfoB0201', text: 'Kecepatan', type: 'select', cls: 'span5', required: true,
                    items: [
                        { value: 'Y', text: 'Berhubungan' },
                        { value: 'N', text: 'Tidak Berhubungan' },
                    ]
                },
            ]
        },
        {
            //title: 'Pada kasus yang berhubungan dengan kecepatan',
            type: 'text-left',
            items: [
                {
                    text: 'Kecepatan kendaraan', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoB0202A', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: 's.d', disabled: true },
                        { name: 'EvtInfoB0202B', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: 'kph', disabled: true },
                    ]
                },
                { name: 'EvtInfoB0203', text: 'Komentar Tambahan', type: 'textarea' },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    name: 'EvtInfoB0301', text: 'R P M', type: 'select', cls: 'span5',
                    items: [
                        { value: 'Y', text: 'Berhubungan' },
                        { value: 'N', text: 'Tidak Berhubungan' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    text: 'RPM Mesin', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoB0302A', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: 's.d', disabled: true },
                        { name: 'EvtInfoB0302B', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: 'rpm', disabled: true },
                    ]
                },
                { name: 'EvtInfoB0303', text: 'Komentar Tambahan', type: 'textarea' },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    text: 'Posisi Gas / Gigi', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoB0401', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: '%', disabled: true },
                        { name: 'EvtInfoB0402', type: 'int', cls: 'span1' },
                        { type: 'label', cls: 'span1', text: 'Gigi', disabled: true },
                    ]
                },
                {
                    name: 'EvtInfoB0403', text: 'Air Conditioner (AC)', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Tidak Berkaitan' },
                        { value: '2', text: '2. Menggunakan AC' },
                        { value: '3', text: '3. Tidak Menggunakan AC' },
                    ]
                },
                {
                    name: 'EvtInfoB0404', text: 'Suhu Udara', type: 'select', cls: 'span5',
                    items: [
                        { value: 'Y', text: 'Berkaitan' },
                        { value: 'N', text: 'Tidak Berkaitan' },
                    ]
                },
            ]
        },
        {
            name: 'pnlSuhu',
            type: 'text-left',
            items: [
                {
                    text: 'Terjadi Pada Suhu', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoB0501', type: 'int', cls: 'span1' },
                        { type: 'label', text: 's.d', cls: 'span1' },
                        { name: 'EvtInfoB0502', type: 'int', cls: 'span1' },
                        {
                            name: 'EvtInfoB0503', type: 'select', cls: 'span1', opt_text: '--',
                            items: [
                                { value: 'C', text: 'Celcius' },
                                { value: 'F', text: 'Farenheit' },
                            ]
                        },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Uraian Kejadian (Kondisi)',
            items: [
                { name: 'EvtInfoC01', text: 'Kondisi', placeholder: 'Saat terjadinya masalah', type: 'textarea' },
            ]
        },
        {
            type: 'text-left',
            title: 'Kondisi Jalan',
            items: [
                { name: 'EvtInfoC01Chk01', text: 'Kota', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk02', text: 'Pinggiran Kota', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk03', text: 'Tol', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk04', text: 'Pegunungan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk05', text: 'Lereng', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk06', text: 'Pasir', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk07', text: 'Dataran Tinggi', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk08', text: 'Trek / Balap', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk09', text: 'Terjal', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk10', text: 'Jalan Berlubang', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk11', text: 'Bukit', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC01Chk12', text: 'Jalan Bergelombang', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Jalan Beraspal Rata',
            items: [
                { name: 'EvtInfoC02Chk01', text: 'Aspal', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC02Chk02', text: 'Beton', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC02Chk03', text: 'Tambalan', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Jalan Tidak Rata',
            items: [
                { name: 'EvtInfoC03Chk01', text: 'Tanah', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC03Chk02', text: 'Bergelombang', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC03Chk03', text: 'Pasir', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC03Chk04', text: 'Lumpur', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Jalan Bersalju',
            items: [
                { name: 'EvtInfoC04Chk01', text: 'Hujan Salju', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC04Chk02', text: 'Salju Padat', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC04Chk03', text: 'Lumpur', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC04Chk04', text: 'Es', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Jalan Banjir',
            items: [
                {
                    text: 'Kedalaman Air', type: 'controls', cls: 'span8', items: [
                        { name: 'EvtInfoC05', type: 'int', cls: 'span2' },
                        { type: 'label', text: '(misal: 20cm, 0.5kaki dll)' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Kondisi Basah',
            items: [
                { name: 'EvtInfoC06Chk01', text: 'Hujan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC06Chk02', text: 'Hujan Lebat', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC06Chk03', text: 'Sesudah Hujan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC06Chk04', text: 'Setelah Dicuci', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            items: [
                {
                    text: 'Jumlah Penumpang', type: 'controls', cls: 'span4', items: [
                        { name: 'EvtInfoC0701', cls: 'span4', type: 'int', required: true },
                        { type: 'label', text: 'orang' },
                    ]
                },
                {
                    text: 'Jumlah Beban', type: 'controls', cls: 'span4', items: [
                        { name: 'EvtInfoC0702', cls: 'span4', type: 'decimal' },
                        { type: 'label', text: 'kg' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Tindakan',
            items: [
                { name: 'EvtInfoC0801', text: 'Yang diperiksa', type: 'textarea', required: true },
                { name: 'EvtInfoC0802', text: 'Hasil Pemeriksaan', type: 'textarea', required: true },
                { name: 'EvtInfoC0803', text: 'Kode DTC', placeholder: 'Hasil pemeriksaan dengan SDT', type: 'textarea', required: true },
                { name: 'EvtInfoC0804', text: 'Prediksi', placeholder: 'Kemungkinan penyebab terjadinya masalah', type: 'textarea', required: true },
                { name: 'EvtInfoC0805', text: 'Alasan', placeholder: 'Alasan dari Penilaian', type: 'textarea', required: true },
                {
                    name: 'EvtInfoC0806', text: 'Status Perbaikan', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Diganti dengan part yang baru' },
                        { value: '2', text: '2. Repair/Diperbaiki' },
                        //{ value: '3', text: '3. Diperbaiki Sementara' },
                    ]
                },
                { name: 'EvtInfoC0807', text: 'Tindakan', placeholder: 'Tindakan yang dilakukan', type: 'textarea', required: true },
                { name: 'IsAvailPartDmg', any:' onclick="checkAvailablePartDamage(this)" ', text: 'Apakah tersedia part yang rusak / claim', type: 'radio-switch', required: true, cls: 'span4 full-length', action: 'checkPartAvailbility' },
                { name: 'IsReportToSis', any: ' onclick="checkAvailablePartDamage(this)" ', text: 'Jika tersedia, part sudah dikirim ke PT. SIS', type: 'radio-switch', required: true, cls: 'span4 full-length' },

            ]
        },

        {
            type: 'text-left',
            items: [
                { name: 'EstimatedDelivery', text: 'Tanggal Pengiriman', type: 'datepicker', cls: 'span3', disabled: true },
                {
                    name: 'NotSendingCategory', text: 'Alasan tidak bisa kirim', type: 'select', cls: 'span5', required: true,
                    items: [
                        { value: '1', text: '1. Part sudah rusak / hancur' },
                        { value: '2', text: '2. Dilakukan perbaikan menjadi OK' },
                        //{ value: '3', text: '3. Ongkos Pembayaran / Reparasi' },
                        //{ value: '4', text: '4. Menunggu Part Pengganti' },
                        //{ value: '5', text: '5. Tunggu Konsumen' },
                        { value: '3', text: '3. Tidak ada part yang diganti' },
                    ]
                },

                {
                    name: 'DikirimVia', text: 'Dikirim Via', type: 'select', cls: 'span3 full',
                    items: [
                        { value: '1', text: '1. External Dealer/Direct' },
                        { value: '2', text: '2. Expedisi Pandu Siwi' },
                        { value: '3', text: '3. Expedisi Atlas' },
                    ]
                },

                { name: 'AirWayBillNo', text: 'No. Air Waybill', type: 'text', cls: 'span3', placeholder: 'No. Air Waybill' },
                { name: 'PartReceivedDate', text: 'Tgl. Penerimaan Barang', type: 'datepicker', cls: 'span3', disabled: true },

                { name: 'EvtInfoA04', text: 'No Part', type: 'text', placeholder: 'Yang perlu dianalisa sebagai penyebab masalah' },
                { name: 'EvtInfoA05', text: 'Nama Part', type: 'text', placeholder: 'Nama Part yang bermasalah' },
                {
                    name: 'EvtInfoC0808', text: 'Hasil Perbaikan', type: 'select', cls: 'span5', required: true,
                	items: [
                        { value: '1', text: '1. Terselesaikan' },
                        { value: '2', text: '2. Tidak Terselesaikan' },
                        { value: '3', text: '3. Menjadi lebih baik' },
                	]
                },
                {
                    name: 'EvtInfoC0809', text: 'Kepuasan Pelanggan', type: 'select', cls: 'span5', required: true,
                    items: [
                        { value: '1', text: '1. Puas' },
                        { value: '2', text: '2. Tidak Puas' },
                    ]
                },
            ]
        },

        {
            type: 'text-left',
            title: 'Penggunaan Kendaraan',
            items: [
                {
                    name: 'EvtInfoC0901', text: 'Frekuensi Mengemudi', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Setiap hari' },
                        { value: '2', text: '2. Beberapa kali/minggu' },
                        { value: '3', text: '3. Sekali/minggu' },
                        { value: '4', text: '4. Sebulan sekali' },
                        { value: '5', text: '5. Lama tidak dipakai' },
                    ]
                },
                {
                    text: 'Jarak tempuh per Hari', type: 'controls', cls: 'span4', items: [
                        { name: 'EvtInfoC0902', cls: 'span4', type: 'int' },
                        { type: 'label', text: 'Km' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Pemakaian Rutin Kendaraan',
            items: [
                { name: 'EvtInfoC0902Chk01', text: 'Pulang pergi', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk02', text: 'Belanja / pickup', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk03', text: 'Transportasi', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk04', text: 'Waktu luang / liburan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk05', text: 'Bisnis Pengiriman', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk06', text: 'Balapan / Off road', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0902Chk07', text: 'Penarik kendaraan', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Cara / kebiasaan pemakaian kendaraan',
            items: [
                { name: 'EvtInfoC0903Chk01', text: 'Langsung dijalankan, tidak dipanaskan', type: 'radio-switch', cls: 'span4 full-length' },
                { name: 'EvtInfoC0903Chk02', text: 'Pengulangan mengemudi dalam waktu singkat (< 10 m)', type: 'radio-switch', cls: 'span4 full-length' },
                { name: 'EvtInfoC0903Chk03', text: 'Selalu mengemudi dengan kecepatan tinggi', type: 'radio-switch', cls: 'span4 full-length' },
                { name: 'EvtInfoC0903Chk04', text: 'Hanya mengemudi malam hari', type: 'radio-switch', cls: 'span4 full-length' },
                { name: 'EvtInfoC0903Chk05', text: 'Sering berhenti dan mematikan mesin', type: 'radio-switch', cls: 'span4 full-length' },
            ]
        },
        {
            type: 'text-left',
            title: 'Area Pemakaian Kendaraan',
            items: [
                { name: 'EvtInfoC0904Chk01', text: 'Perkotaan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk02', text: 'Lereng / bukit', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk03', text: 'Pinggiran kota', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk04', text: 'Tol', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk05', text: 'Pegunungan', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk06', text: 'Wilayah Salju', type: 'radio-switch', cls: 'span4', float: 'left' },
                { name: 'EvtInfoC0904Chk07', text: 'Pantai', type: 'radio-switch', cls: 'span4', float: 'left' },
            ]
        },
        {
            type: 'text-left',
            title: 'Situasi Parkir',
            items: [
                {
                    name: 'EvtInfoC090501', text: 'Lokasi Parkir', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Di dalam' },
                        { value: '2', text: '2. Di luar' },
                    ]
                },
                {
                    name: 'EvtInfoC090502', text: 'Kondisi Lantai', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Rata' },
                        { value: '2', text: '2. Tidak Rata' },
                    ]
                },
                {
                    name: 'EvtInfoC090503', text: 'Sarung Mobil', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '1. Tertutup' },
                        { value: '2', text: '2. Tidak Tertutup' },
                    ]
                },
                { name: 'EvtInfoC090504', text: 'Komentar Tambahan', type: 'textarea' }
            ]
        },
        {
            type: 'text-left',
            title: 'Informasi Tambahan',
            items: [
                {
                    name: 'EvtInfoC090601', text: 'Usia Konsuman', type: 'select', cls: 'span5',
                    items: [
                        { value: '1', text: '< 10 th' },
                        { value: '2', text: '10 s.d 20 th' },
                        { value: '3', text: '20 s.d 30 th' },
                        { value: '4', text: '30 s.d 40 th' },
                        { value: '5', text: '40 s.d 50 th' },
                        { value: '6', text: '50 s.d 60 th' },
                        { value: '7', text: '> 60 th' },
                    ]
                },
                {
                    text: 'Pengalaman berkendara', type: 'controls', cls: 'span4', items: [
                        { name: 'EvtInfoC090602', cls: 'span4', type: 'int' },
                        { type: 'label', text: 'tahun' },
                    ]
                },
            ]
        },
        {
            type: 'text-left',
            title: 'Attachment',
            items: [

                    { name: "FileID1", type: "hidden" },
                    {
                        name: "FileName1", text: "Attachment 1", readonly: true, type: "upload",
                        url: "wh.api/svtrans/UploadFile", icon: "fa fa-file-archive-o", callback: "uploadCallback1", cls: 'span5'
                    },
                    {
                        type: 'buttons',
                        cls: 'span2',
                        items:[
                           { name: 'btnView1', text: 'View', action: 'viewImage(1)', btntype: ' type="button" ' },
                        ]
                    },
                    { name: "FileID2", type: "hidden" },
                    { name: "FileName2", text: "Attachment 2", readonly: true, type: "upload", url: "wh.api/svtrans/UploadFile", icon: "fa fa-file-archive-o", callback: "uploadCallback2", cls: 'span5' },
                    {
                        type: 'buttons',
                        cls: 'span2',
                        items: [
                            { name: 'btnView2', text: 'View', action: 'viewImage(2)', btntype: ' type="button" ' },
                        ]
                    },
                    { name: "FileID3", type: "hidden" },
                    { name: "FileName3", text: "Attachment 3", readonly: true, type: "upload", url: "wh.api/svtrans/UploadFile", icon: "fa fa-file-archive-o", callback: "uploadCallback3", cls: 'span5' },
                    {
                        type: 'buttons',
                        cls: 'span2',
                        items: [
                            { name: 'btnView3', text: 'View', action: 'viewImage(3)', btntype: ' type="button" ' },
                        ]
                    },
                    { name: "FileID4", type: "hidden" },
                    { name: "FileName4", text: "Attachment 4", readonly: true, type: "upload", url: "wh.api/svtrans/UploadFile", icon: "fa fa-file-archive-o", callback: "uploadCallback4", cls: 'span5' },
                    {
                        type: 'buttons',
                        cls: 'span2',
                        items: [
                            { name: 'btnView4', text: 'View', action: 'viewImage(4)', btntype: ' type="button" ' },
                        ]
                    },
                    { name: "FileID5", type: "hidden" },
                    { name: "FileName5", text: "Attachment 5", readonly: true, type: "upload", url: "wh.api/svtrans/UploadFile", icon: "fa fa-file-archive-o", callback: "uploadCallback5", cls: 'span5' },
                    {
                        type: 'buttons',
                        cls: 'span2',
                        items: [
                            { name: 'btnView5', text: 'View', action: 'viewImage(5)', btntype: ' type="button" ' },
                        ]
                    },
            ]
        },
        {},
        {},
    ],
    onInit: function (wgt) {
        var init = {
            FtirDate: new Date(),
            FtirEventDate: new Date(),
            FtirRegDate: new Date(),
            //EstimatedDelivery: new Date(),
            Odometer: 0,
            UsageTime: 0,
            EvtInfoA07: 0,
            EvtInfoB0202A: 0,
            EvtInfoB0202B: 0,
            EvtInfoB0302A: 0,
            EvtInfoB0302B: 0,
            EvtInfoB0401: 0,
            EvtInfoB0402: 0,
            EvtInfoB0501: 0,
            EvtInfoB0502: 0,
            EvtInfoB0503: 'C',
            EvtInfoC05: 0,
            EvtInfoC0701: 0,
            EvtInfoC0702: 0,
            EvtInfoC0902: 0,
            EvtInfoC090602: 0,
            FileID1: '',
            FileID2: '',
            FileID3: '',
            FileID4: '',
            FileID5: '',
            FileName5: '',
            FileName4: '',
            FileName3: '',
            FileName2: '',
            FileName1: '',
            FileName5Showed: '',
            FileName4Showed: '',
            FileName3Showed: '',
            FileName2Showed: '',
            FileName1Showed: ''
        };

        $("[name=printtag], [name=printform]").attr("disabled", "disabled");

        if ($("[name=FtirDate]").val() == "") {
            $("[name=PartReceivedDate]").val(moment(new Date).format("DD MMM YYYY"));;
        } else {
            $("[name=PartReceivedDate]").val($("[name=FtirDate]").val())
        }

        ///*Option Sparepart */
        //$("[for=IsReportToSis]").hide();
        //$("[for=IsReportToSisY]").hide();
        //$("[for=IsReportToSisN]").hide();
        //$("[for=EstimatedDelivery]").hide();
        //$("[name=EstimatedDelivery]").hide();

        //$("[name=NotSendingCategory]").removeAttr("required");
        //$("#s2id_NotSendingCategory").hide();
        //$("[for=NotSendingCategory]").hide();
        //$("[name=EstimatedDelivery]").attr("disabled", "disabled");
        //$("[name=EstimatedDelivery]").removeAttr("required");
        //$("#s2id_DikirimVia").hide();
        //$("[for=DikirimVia]").hide();
        //$("[name=AirWayBillNo]").hide();
        //$("[for=AirWayBillNo]").hide();
        ///*****/

        wgt.post('wh.api/svtrans/GetCurrentDealer', function (r) {
            if (r.DealerCode && r.OutletCode) {
                init.DealerCode = r.DealerCode;
                init.OutletCode = r.OutletCode;
                init.DealerName = r.DealerName;
                wgt.populate(init);
                wgt.init = init;
                $("[name=PartReceivedDate]").hide();
                $("[name=ScanNo]").hide();
                $("[for=PartReceivedDate]").hide();
                $("[for=ScanNo]").hide();
            }
            else {
                $("[name=add]").hide();
                //$("[name=PartReceivedDate]").removeAttr('disabled');

                //sdms.info({ type: 'error', text: 'Anda tidak memiliki otorisasi untuk menggunakan menu ini' });
                //wgt.showToolbars([]);
                //$('.body > .main > .gl-widget').html('');
            }
        });
    },
    add: function () {
        widget.clear();
        widget.populate(widget.init);
        $("[name=printtag], [name=printform]").attr("disabled", "disabled");
    },
    demo: function() {
        alert("ok");
        var params = {
            ReportId: "dealerlist.trdx",
            Parameters : "{name:''}"
        }
        SimDms.openReport(params, "Demo")
    },
    save: function () {
        var record = widget.serializeObject();
        console.log(record);

        var IsValid = $(".page .main form").valid();

        //console.log(record);
        
        if (!record.DealerCode) { sdms.info({ type: 'error', text: 'Kode Dealer tidak boleh kosong' }); return; }
        if (!record.OutletCode) { sdms.info({ type: 'error', text: 'Kode Outlet tidak boleh kosong' }); return; }
        if (!record.FileID1) { sdms.info({ type: 'error', text: 'Harap sertakan lampiran dokumen/attachment, minimal 1 file!!!' }); return; }

        if (IsValid) {
            $("[name='save']").attr('disabled', 'disabled');

            sdms.showAjaxLoad();
            sdms.save({
                url: 'wh.api/svtrans/ftirsave',
                params: record,
                finish: function (r) {
                    if (r.success && r.data) {

                        console.log("data from database");
                        console.log(r.data);

                        widget.populate(r.data);
                        sdms.notify({ type: 'success', text: 'Data Saved' });
                        $("[name=printtag], [name=printform]").removeAttr("disabled");
                        $("[name='save']").removeAttr('disabled');
                    }
                    else {
                        sdms.notify({ type: 'error', text: r.message });
                        $("[name='save']").removeAttr('disabled');
                    }
                    sdms.hideAjaxLoad();
                }
            });
            
        } else {
            sdms.info({ type: 'error', text: 'Harap isi semua mandatori field!!!' });
        }
    },
    browse: function () {
        sdms.lookup({
            title: 'Lookup Data',
            url: 'wh.api/svtrans/ftirlist',
            sort: [{ field: 'FtirNo', dir: 'desc' }, { field: 'DealerCode', dir: 'asc' }, { field: 'OutletCode', dir: 'asc' }],
            fields: [
                { name: 'FtirNo', text: 'Ftir No', width: 150 },
                { name: 'VinNo', text: 'VIN', width: 220 },
                { name: 'DealerCode', text: 'Dealer', width: 120 },
                { name: 'DealerName', text: 'Nama Dealer', width: 360 },
                { name: 'OutletCode', text: 'Outlet', width: 120 },
                { name: 'FtirMaker', text: 'Nama Pembuat', width: 300 },
                { name: 'FtirEventDate', text: 'Tanggal Kejadian', width: 140, type: 'date' },
                { name: 'FtirDate', text: 'Tanggal Dibuat', width: 140, type: 'date' },
                { name: 'FtirRegDate', text: 'Tanggal Registrasi', width: 140, type: 'date' },
                { name: 'Model', text: 'Model', width: 100 },
                { name: 'Machine', text: 'No Mesin', width: 120 },
                { name: 'TransmNo', text: 'No Transmisi', width: 120 },
                { name: 'Odometer', type: 'int', width: 120 },
                { name: 'UsageTime', type: 'int', width: 120 },
            ],
            dblclick: 'loadRecord',
            onclick: 'loadRecord'
        });
    },
    loadRecord: function (row) {
        widget.clear();
        row.FileName1Showed = row.FileName1;
        row.FileName2Showed = row.FileName2;
        row.FileName3Showed = row.FileName3;
        row.FileName4Showed = row.FileName4;
        row.FileName5Showed = row.FileName5;
        console.log(row);
        widget.populate(row);
        checkPartAvailbilityDmg(row);

        $("[name=printtag], [name=printform]").removeAttr("disabled");
    },
    print: function()
    {
        var FtirNo = $("[name=FtirNo]").val();
        if (FtirNo !== undefined)
        {
            console.log("No FTIR: " + FtirNo);
            var url = "wh.api/svtrans/ClaimTagWarranty?FTIRNO=" + FtirNo;
            window.location = url;
        }
    },
    printForm: function () {
        var FtirNo = $("[name=FtirNo]").val();
        if (FtirNo !== undefined) {
            var url = "wh.api/svtrans/FormFTIR?FTIRNO=" + FtirNo;
            window.location = url;
        }
    }
});


function checkPartAvailbilityDmg(rec)
{
    var b = rec.IsAvailPartDmg;
    var c = rec.IsReportToSis;

    //if (b) {
    //    $("[for=IsReportToSis]").show();
    //    $("[for=IsReportToSisY]").show();
    //    $("[for=IsReportToSisN]").show();
    //    $("[for=EstimatedDelivery]").show();
    //    $("[name=EstimatedDelivery]").show();
    //    if (c) {
    //        $("[name=NotSendingCategory]").removeAttr("required");
    //        $("#s2id_NotSendingCategory").hide();
    //        $("[for=NotSendingCategory]").hide();
    //        $("[name=EstimatedDelivery]").removeAttr("disabled");
    //        $("[name=EstimatedDelivery]").attr("required", "required");
    //        $("#s2id_DikirimVia").show()
    //        $("[for=DikirimVia]").show();
    //        $("[name=AirWayBillNo]").show();
    //        $("[for=AirWayBillNo]").show();
    //    } else {
    //        $("[name=NotSendingCategory]").attr("required", "required");
    //        $("#s2id_NotSendingCategory").show();
    //        $("[for=NotSendingCategory]").show();
    //        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
    //        $("[name=EstimatedDelivery]").removeAttr("required");
    //        $("#s2id_DikirimVia").hide();
    //        $("[for=DikirimVia]").hide();
    //        $("[name=AirWayBillNo]").hide();
    //        $("[for=AirWayBillNo]").hide();
    //    }
    //} else {
    //    $("[for=IsReportToSis]").hide();
    //    $("[for=IsReportToSisY]").hide();
    //    $("[for=IsReportToSisN]").hide();
    //    $("[for=EstimatedDelivery]").hide();
    //    $("[name=EstimatedDelivery]").hide();

    //    $("[name=NotSendingCategory]").removeAttr("required");
    //    $("#s2id_NotSendingCategory").hide();
    //    $("[for=NotSendingCategory]").hide();
    //    $("[name=EstimatedDelivery]").attr("disabled", "disabled");
    //    $("[name=EstimatedDelivery]").removeAttr("required");
    //    $("#s2id_DikirimVia").hide();
    //    $("[for=DikirimVia]").hide();
    //    $("[name=AirWayBillNo]").hide();
    //    $("[for=AirWayBillNo]").hide();
    //}


    //if (b && c) {
    //    $("[name=NotSendingCategory]").attr("required", false);
    //    $("[name=EstimatedDelivery]").removeAttr("disabled");
    //    $("[name=EstimatedDelivery]").attr("required", true);
    //} else {
    //    $("[name=NotSendingCategory]").attr("required", true);
    //    $("[name=EstimatedDelivery]").attr("disabled", "disabled");
    //    $("[name=EstimatedDelivery]").val("");
    //    $("[name=EstimatedDelivery]").removeAttr("required");
    //}

    if (b.toString() === "true" && c.toString() === "true") {
        $("[name=NotSendingCategory]").removeAttr("required");
        $("[name=NotSendingCategory]").hide();
        $("#s2id_NotSendingCategory").hide()
        $("[for=NotSendingCategory]").hide();
        $("[name=EstimatedDelivery]").removeAttr("disabled");
        $("[name=EstimatedDelivery]").attr("required", "required");
        $("[name=EstimatedDelivery]").val(moment(new Date).format("DD-MMM-YYYY"));
        $("[name=PartReceivedDate]").removeAttr("disabled");
    } else if (b.toString() === "true" && c.toString() === "false") {
        $("[name=NotSendingCategory]").attr("required", "required");
        $("[name=NotSendingCategory]").show();
        $("#s2id_NotSendingCategory").show()
        $("[for=NotSendingCategory]").show();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val("");
        $("[name=PartReceivedDate]").removeAttr("disabled");
    } else if (b.toString() === "false" && c.toString() === "true") {
        $("[name=NotSendingCategory]").removeAttr("disabled");
        $("[name=NotSendingCategory]").hide();
        $("#s2id_NotSendingCategory").hide()
        $("[for=NotSendingCategory]").hide();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val(moment(new Date).format("DD-MMM-YYYY"));
        if ($("[name=FtirDate]").val() == "") {
            $("[name=PartReceivedDate]").val(moment(new Date).format("DD-MMM-YYYY"));;
        } else {
            $("[name=PartReceivedDate]").val($("[name=FtirDate]").val())
        }
        $("[name=PartReceivedDate]").attr("disabled", "disabled");
    } else {
        $("[name=NotSendingCategory]").attr("required", "required");
        $("[name=NotSendingCategory]").show();
        $("#s2id_NotSendingCategory").show()
        $("[for=NotSendingCategory]").show();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val("");
        if ($("[name=FtirDate]").val() == "") {
            $("[name=PartReceivedDate]").val(moment(new Date).format("DD-MMM-YYYY"));;
        } else {
            $("[name=PartReceivedDate]").val($("[name=FtirDate]").val())
        }
        $("[name=PartReceivedDate]").attr("disabled", "disabled");
    }


    $(".page .main form").valid();
}

var checkAvailablePartDamage = function (obj) {
    
    var b = $("[name=IsAvailPartDmg]:checked").val();
    var c = $("[name=IsReportToSis]:checked").val();

    //if (b.toString() === "true") {
    //    $("[for=IsReportToSis]").show();
    //    $("[for=IsReportToSisY]").show();
    //    $("[for=IsReportToSisN]").show();
    //    $("[for=EstimatedDelivery]").show();
    //    $("[name=EstimatedDelivery]").show();
    //    if (c.toString() === "true") {
    //        $("[name=NotSendingCategory]").removeAttr("required");
    //        $("#s2id_NotSendingCategory").hide();
    //        $("[for=NotSendingCategory]").hide();
    //        $("[name=EstimatedDelivery]").removeAttr("disabled");
    //        $("[name=EstimatedDelivery]").attr("required", "required");
    //        $("#s2id_DikirimVia").show()
    //        $("[for=DikirimVia]").show();
    //        $("[name=AirWayBillNo]").show();
    //        $("[for=AirWayBillNo]").show();
    //    } else {
    //        $("[name=NotSendingCategory]").attr("required", "required");
    //        $("#s2id_NotSendingCategory").show();
    //        $("[for=NotSendingCategory]").show();
    //        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
    //        $("[name=EstimatedDelivery]").removeAttr("required");
    //        $("#s2id_DikirimVia").hide();
    //        $("[for=DikirimVia]").hide();
    //        $("[name=AirWayBillNo]").hide();
    //        $("[for=AirWayBillNo]").hide();
    //    }
    //} else {
    //    $("[for=IsReportToSis]").hide();
    //    $("[for=IsReportToSisY]").hide();
    //    $("[for=IsReportToSisN]").hide();
    //    $("[for=EstimatedDelivery]").hide();
    //    $("[name=EstimatedDelivery]").hide();

    //    $("[name=NotSendingCategory]").removeAttr("required");
    //    $("#s2id_NotSendingCategory").hide();
    //    $("[for=NotSendingCategory]").hide();
    //    $("[name=EstimatedDelivery]").attr("disabled", "disabled");
    //    $("[name=EstimatedDelivery]").removeAttr("required");
    //    $("#s2id_DikirimVia").hide();
    //    $("[for=DikirimVia]").hide();
    //    $("[name=AirWayBillNo]").hide();
    //    $("[for=AirWayBillNo]").hide();
    //}

    console.log(b.toString() + "," + c.toString())

    if (b.toString() === "true" && c.toString() === "true") {
        $("[name=NotSendingCategory]").removeAttr("required");
        $("[name=NotSendingCategory]").hide();
        $("#s2id_NotSendingCategory").hide()
        $("[for=NotSendingCategory]").hide();
        $("[name=EstimatedDelivery]").removeAttr("disabled");
        $("[name=EstimatedDelivery]").attr("required", "required");
        $("[name=EstimatedDelivery]").val(moment(new Date).format("DD-MMM-YYYY"));
        $("[name=PartReceivedDate]").removeAttr("disabled");
    } else if (b.toString() === "true" && c.toString() === "false") {
        $("[name=NotSendingCategory]").attr("required", "required");
        $("[name=NotSendingCategory]").show();
        $("#s2id_NotSendingCategory").show()
        $("[for=NotSendingCategory]").show();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val("");
        $("[name=PartReceivedDate]").removeAttr("disabled");
    } else if (b.toString() === "false" && c.toString() === "true") {
        $("[name=NotSendingCategory]").removeAttr("disabled");
        $("[name=NotSendingCategory]").hide();
        $("#s2id_NotSendingCategory").hide()
        $("[for=NotSendingCategory]").hide();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val(moment(new Date).format("DD-MMM-YYYY"));
        if ($("[name=FtirDate]").val() == "") {
            $("[name=PartReceivedDate]").val(moment(new Date).format("DD-MMM-YYYY"));;
        } else {
            $("[name=PartReceivedDate]").val($("[name=FtirDate]").val())
        }
        $("[name=PartReceivedDate]").attr("disabled", "disabled");
    }else{
        $("[name=NotSendingCategory]").attr("required", "required");
        $("[name=NotSendingCategory]").show();
        $("#s2id_NotSendingCategory").show()
        $("[for=NotSendingCategory]").show();
        $("[name=EstimatedDelivery]").attr("disabled", "disabled");
        $("[name=EstimatedDelivery]").removeAttr("required");
        $("[name=EstimatedDelivery]").val("");
        if ($("[name=FtirDate]").val() == "") {
            $("[name=PartReceivedDate]").val(moment(new Date).format("DD-MMM-YYYY"));;
        } else {
            $("[name=PartReceivedDate]").val($("[name=FtirDate]").val())
        }
        $("[name=PartReceivedDate]").attr("disabled", "disabled");
    }

    $(".page .main form").valid();
    
    //var record = widget.serializeObject();
    //checkPartAvailbilityDmg(record);

};

$('[name=VinNo]').on('blur', function (e) {
    var vinno = ($('[name=VinNo]').val());
    $.ajax({
        async: false,
        type: "POST",
        url: "wh.api/svtrans/GetModelInfo",
        data: "VIN=" + vinno,
    }).done(function (data) {
        $('[name=Model]').val('');
        $('[name=Machine]').val('');
        $('[name=TransmNo]').val('');
        if (data !== undefined && data !== null)
        {
            if (data.result == 1)
            {
                $('[name=Model]').val(data.data[0].Model);
                $('[name=Machine]').val(data.data[0].Engine);
                $('[name=TransmNo]').val(data.data[0].Transmision);
                $(".page .main form").valid();
            }
        }
    });

});

function uploadCallback1(result, obj) {
    if (result.status) {
        console.log(result);
        $('[name=FileName1').val(result.data.FileName);
        $('[name=FileName1Showed').val(result.data.FileName);
        $('[name=FileID1').val(result.data.FileID);
    }
}

function uploadCallback2(result, obj) {
    if (result.status) {
        console.log(result);
        $('[name=FileName2').val(result.data.FileName);
        $('[name=FileName2Showed').val(result.data.FileName);
        $('[name=FileID2').val(result.data.FileID);
    }
}

function uploadCallback3(result, obj) {
    if (result.status) {
        console.log(result);
        $('[name=FileName3').val(result.data.FileName);
        $('[name=FileName3Showed').val(result.data.FileName);
        $('[name=FileID3').val(result.data.FileID);
    }
}

function uploadCallback4(result, obj) {
    if (result.status) {
        console.log(result);
        $('[name=FileName4').val(result.data.FileName);
        $('[name=FileName4Showed').val(result.data.FileName);
        $('[name=FileID4').val(result.data.FileID);
    }
}

function uploadCallback5(result, obj) {
    if (result.status) {
        console.log(result);
        $('[name=FileName5').val(result.data.FileName);
        $('[name=FileName5Showed').val(result.data.FileName);
        $('[name=FileID5').val(result.data.FileID);
    }
}


$('label').click(function () { 
    var labelID = $(this).attr('for');
    if (labelID !==undefined) console.log(labelID);
});

$('#btnView1,#btnView2,#btnView3,#btnView4,#btnView5').click(function () {
    var id = this.id.substring(7);
    viewImage(id);
});

$('input[name=FtirEventDate], input[name=FtirRegDate]').on('change', function () {
    var pnlFtir1 = widget.serializeObject('pnlFtir1');
    var wktpemakaian = (pnlFtir1.FtirRegDate && pnlFtir1.FtirEventDate)
        ? moment(pnlFtir1.FtirEventDate).diff(moment(pnlFtir1.FtirRegDate), 'days')
        : 0;
    $('#UsageTime').val(wktpemakaian > 0 ? wktpemakaian : 0);
});

function viewImage (id)
{
    var ctr = "#FileID" + id;
    var value = $(ctr).val();
    console.log(value);

    if (value !=='' && value !== undefined)
    {

        var link = "main/downloadfile?filename=" + value;
        console.log(link);

        function OpenInNewTab(url) {
            var win = window.open(SimDms.baseUrl + url, '_blank');
            win.focus();
        }

        OpenInNewTab(link);

    }        
}
