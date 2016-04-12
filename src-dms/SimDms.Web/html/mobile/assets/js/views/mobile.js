$(document).ready(function () {
    var mobile = new SimDms.Mobile({
        //debug: true,
        views: [
            {
                title: "Sdms Mobile",
                name: "module",
                content: [
                    {
                        title: "Module List",
                        type: "link",
                        items: [
                            { text: "Service", href: "service" },
                            { text: "Sparepart" },
                            { text: "Sales" },
                        ]
                    },
                    {
                        type: "link",
                        items: [{ text: "Logout", href: "#" }]
                    },
                ]
            },
            {
                title: "Service",
                name: "service",
                header: [
                    { text: "Module", href: "module" },
                ],
                content: [
                    {
                        title: "Service Module",
                        type: "link",
                        items: [
                            { text: "Form Penerimaan", href: "spk1" },
                            { text: "List Form Penerimaan" },
                            { text: "Dashboard" },
                        ]
                    },
                    {
                        type: "link",
                        items: [{ text: "Back to module", href: "module" }]
                    },
                ]
            },
            {
                title: "Penerimaan (1)",
                name: "spk1",
                header: [
                    { text: "Service", href: "service" },
                    { text: "Next", href: "spk2", align: "right" },
                ],
                content: [
                    {
                        title: "Penerimaan",
                        items: [
                            { name: "PoliceRegNo", text: "No Polisi" },
                            { text: "Tanggal", type: "date" },
                            { text: "Waktu Datang", type: "time" },
                            { text: "Waktu Janji", type: "time" },
                            { text: "Waktu Selesai", type: "time" },
                        ]
                    },
                    {
                        title: "Data Pelanggan",
                        items: [
                            { text: "Nama Pelangan" },
                            { text: "Alamat Rumah", type: "textarea" },
                            { text: "Telepon Rumah" },
                            { text: "Telepon Kantor" },
                            { text: "Alamat Email" },
                        ]
                    },
                ]
            },
            {
                title: "Penerimaan (2)",
                name: "spk2",
                header: [
                    { text: "Prev", href: "spk1" },
                    { text: "Next", href: "spk3", align: "right" },
                ],
                content: [
                    {
                        title: "Informasi",
                        items: [
                            { text: "No Polisi" },
                            { text: "No Rangka" },
                            { text: "No Mesin" },
                            { text: "Tahun Pembuatan", type: "number" },
                            { text: "Jarak Tempuh", type: "number" },
                            { text: "Fuel (%)", type: "number" },
                        ]
                    },
                ],
            },
            {
                title: "Penerimaan (3)",
                name: "spk3",
                header: [
                    { text: "Prev", href: "spk2" },
                    { text: "Next", href: "spk4", align: "right" },
                ],
                content: [
                    {
                        title: "Informasi",
                        items: [
                            { text: "No Polisi" },
                            { text: "Bemper Depan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Kap Mesin", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Pintu Depan Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Fender Depan Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Velg Depan Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Pintu Belakang Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Fender Belakang Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Velg Belakang Kanan", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Bemper Belakang", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Pintu Bagasi", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Fender Belakang Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Velg Belakang Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Pintu Belakang Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Pintu Depan Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Fender Depan Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Velg Depan Kiri", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Roof", type: "btn-group", buttons: ["Mulus", "Cacat", "Gores", "Penyok"] },
                            { text: "Catatan", type: "textarea" },
                        ]
                    },
                    {
                        title: "Informasi",
                        items: [
                            { text: "1. Kondisi Wiper", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "2. Kondisi Aki", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                        ]
                    }
                ]
            },
            {
                title: "Penerimaan (4)",
                name: "spk4",
                header: [
                    { text: "Prev", href: "spk3" },
                    { text: "Next", href: "spk5", align: "right" },
                ],
                content: [
                    {
                        title: "Pemeriksaan Fungsi",
                        items: [
                            { text: "1. Klakson, semua lampu (depan, belakang, rem, signal)", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "2. Kinerja & kualitas wiper", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "3. A/C dan Pemanas", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "4. Power window & spion", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "5. Rem tangan", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "6. Pedal Rem / Pedal Kopling", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "7. Saluran dan Cairan Power Steering", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "8. Transmisi dan Kondisi mesin (kebocoran)", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "9. Pemeriksaan dengan menggunakan Suzuki Diagnostic Tools (SDT)", type: "switch", offLabel: "Tidak", onLabel: "Baik" },
                            { text: "Catatan", type: "textarea" },
                        ]
                    },
                ]
            },
            {
                title: "Penerimaan (5)",
                name: "spk5",
                header: [
                    { text: "Prev", href: "spk4" },
                    { text: "Save", href: "spksave", align: "right" },
                ],
                content: [
                    {
                        title: "Perbaikan",
                        items: [
                            { text: "Pekerjaan Paket" },
                            { text: "Permintaan Pekerjaan" },
                            { text: "Hasil Perbaikan" },
                            { text: "Oli Mesin" },
                            { text: "Estimasi Biaya Perawatan" },
                            { text: "Nama SA" },
                            { text: "Phone SA" },
                            { text: "Nama Foreman" },
                        ]
                    },
                ]
            },
            {
                title: "Spk Information",
                name: "spksave",
                header: [
                    { text: "Service", href: "service" },
                ],
            }
        ]
    });
    mobile.render();
});
