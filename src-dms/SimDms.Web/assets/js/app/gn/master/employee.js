var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnEmployeeController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });

    //$http.post('gn.api/combo/LoadLookup?CodeId=GNDR').
    //success(function (data, status, headers, config) {
    //    me.comboGenders = data;
    //});

    me.comboGenders = [{ "value": "L", "text": "Male" }, { "value": "P", "text": "Female" }];

    $http.post('gn.api/combo/LoadLookup?CodeId=SIZE').
    success(function (data, status, headers, config) {
        me.comboUniforms = data;
    });

    //$http.post('gn.api/combo/LoadLookup?CodeId=BLOD').
    //success(function (data, status, headers, config) {
    //    me.comboBloods = data;
    //});

    me.comboBloods = [{ "value": "A", "text": "A" }, { "value": "AB", "text": "AB" }, { "value": "B", "text": "B" }, { "value": "O", "text": "O" }];
    //$http.post('gn.api/combo/LoadLookup?CodeId=MRTL').
    //success(function (data, status, headers, config) {
    //    me.comboMarital = data;
    //});
    me.comboMarital = [{ "value": "D", "text": "Duda" }, { "value": "J", "text": "Janda" }, { "value": "K", "text": "Kawin" }, { "value": "T", "text": "Tidak Kawin" }];

    $http.post('gn.api/combo/LoadLookup?CodeId=PERS').
    success(function (data, status, headers, config) {
        me.comboPersonnels = data;
    });

    me.comboPersonnels = [{ "value": "1", "text": "AKTIF" }, { "value": "2", "text": "NON AKTIF" }, { "value": "3", "text": "KELUAR" }, { "value": "4", "text": "PENSIUN" }, { "value": "5", "text": "INVALID" }]

    //$http.post('gn.api/combo/LoadLookup?CodeId=RLGN').
    //success(function (data, status, headers, config) {
    //    me.comboReligions = data;
    //});

    me.comboReligions = [{ "value": "B", "text": "Budha" }, { "value": "H", "text": "Hindu" }, { "value": "I", "text": "Islam" }, { "value": "K", "text": "Kristen" }, { "value": "T", "text": "Katholik" }];

    //$http.post('gn.api/combo/LoadLookup?CodeId=SHOE').
    //success(function (data, status, headers, config) {
    //    me.comboShoeses = data;
    //});

    me.comboShoeses = [{ "value": "38", "text": "38" }, { "value": "39", "text": "39" }, { "value": "40", "text": "40" }, { "value": "41", "text": "41" }, { "value": "42", "text": "42" }, { "value": "43", "text": "43" }, { "value": "44", "text": "44" }];
    //$http.post('gn.api/combo/LoadLookup?CodeId=FEDU').
    //success(function (data, status, headers, config) {
    //    me.comboEducations = data;
    //});
    me.comboEducations = [{ "value": "1", "text": "STM" }, { "value": "10", "text": "S3" }, { "value": "2", "text": "SD" }, { "value": "3", "text": "SMP" }, { "value": "4", "text": "SMU / SMK" }, { "value": "5", "text": "D1" }, { "value": "6", "text": "D2" }, { "value": "7", "text": "D3" }, { "value": "8", "text": "S1" }, { "value": "9", "text": "S2" }];
    function lookupName(codeid, value, name) {
        $http.post('gn.api/Lookup/getLookupName?CodeId=' + codeid + '&LookupValue=' + value).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                switch (codeid) {
                    case 'CITY':
                        me.data.CityName = v.TitleName;
                        break;
                    case 'AREA':
                        me.data.AreaName = v.TitleName;
                        break;
                    case 'PROV':
                        me.data.ProvinceName = v.TitleName;
                        break;
                    case 'TITL':
                        me.data.TitleName = v.TitleName;
                        break;
                }
            } else {
                switch (codeid) {
                    case 'CITY':
                        $('#CityCode').val('');
                        $('#CityName').val('');
                        me.City();
                        break;
                    case 'AREA':
                        $('#AreaCode').val('');
                        $('#AreaName').val('');
                        me.Area();
                        break;
                    case 'PROV':
                        $('#ProvinceCode').val('');
                        $('#ProvinceName').val('');
                        me.Province();
                        break;
                    case 'TITL':
                        $('#TitleCode').val('');
                        $('#TitleName').val('');
                        me.Title();
                        break;
                }
            }
        });
    }

    me.Move = function () {
        localStorage.setItem('EmployeeID', me.data.EmployeeID);
        localStorage.setItem('EmployeeName', me.data.EmployeeName);
        localStorage.setItem('BranchCode', me.data.BranchCode);
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/employeemutation" });

    }

    me.LinkDetail = function () {
        me.detail.EmployeeID = me.data.EmployeeID;
        me.detail.BranchCode = me.data.BranchCode;
    }

    me.loadDetail = function (data) {
        $http.post('gn.api/Employee/EmployeeTrainingLoad?EmployeeID=' + data.EmployeeID + '&branchCode=' + data.BranchCode).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.grid1, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e);
           });
    }

    me.GetLookUpDtlInfo = function (EmployeeID, BranchCode, CodeID, LookUpValue) {
        var src = "gn.api/Employee/CheckLookUpDtl?EmployeeID=" + EmployeeID + "&BranchCode=" + BranchCode + "&CodeID=" + CodeID + "&LookUpValue=" + LookUpValue;
        $http.post(src)
            .success(function (v, status, headers, config) {
                if (v.success) {
                    switch (CodeID) {
                        case "CITY":
                            var param = { CityName: v.TitleName };
                            break;
                        case "AREA":
                            var param = { AreaName: v.TitleName };
                            break;
                        case "PROV":
                            var param = { ProvinceName: v.TitleName };
                            break;
                        default:
                            var param = { TitleName: v.TitleName };
                    }

                    angular.extend(me.data, v.data, param);
                }

            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });
    }

    me.browse = function () {
            var lookup = Wx.blookup({
                name: "MasterEmployeesBrowse",
                title: "Employees Browse",
                manager: gnManager,
                query: "Employees",
                defaultSort: "EmployeeName asc",
                columns: [
                { field: "EmployeeID", title: "Employee ID" },
                { field: "EmployeeName", title: "Employee Name" },
                { field: "Address1", title: "Address" },
                { field: "TitleCode", title: "Position" },
                { field: "JoinDate", title: "Join Date" },
                { field: "PersonnelStatus", title: "Personnel Status" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.lookupAfterSelect(data);
                    //me.GetLookUpDtlInfo(data.EmployeeID, data.BranchCode, "CITY", data.CityCode);
                    //me.GetLookUpDtlInfo(data.EmployeeID, data.BranchCode, "AREA", data.AreaCode);
                    //me.GetLookUpDtlInfo(data.EmployeeID, data.BranchCode, "PROV", data.ProvinceCode);
                    //me.GetLookUpDtlInfo(data.EmployeeID, data.BranchCode, "TITL", data.TitleCode);
                    $('#btnAddTraining').prop('disabled', false);
                    $('#btnMove').prop('disabled', false);
                    $('#BeginTrainingDate').prop('disabled', false);
                    $('#EndTrainingDate').prop('disabled', false);
                   
                    me.isSave = false;
                    me.Apply();
                    me.loadDetail(data);
                }
            });
        }

    me.City = function () {
                    var lookup = Wx.blookup({
                        name: "CityLookup",
                        title: "Lookup City",
                        manager: gnManager,
                        query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "CITY" }),
                        defaultSort: "LookUpValue asc",
                        columns: [
                            { field: "LookUpValue", title: "City Code" },
                            { field: "LookUpValueName", title: "City Name" }
                        ]
                    });
                    lookup.dblClick(function (data) {
                        if (data != null) {
                            me.isSave = false;
                            me.data.CityCode = data.LookUpValue;
                            me.data.CityName = data.LookUpValueName;
                            me.Apply();
                        }
                    });
        
            };

    me.Code= function () {
                if ($('#isSuzukiTraining').prop('checked') == true) {
                    var Codeid = 'SZKT';
                } else { var Codeid ='IEDU';}
                var lookup = Wx.blookup({
                    name: "CityLookup",
                    title: "Lookup City",
                    manager: gnManager,
                    query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: Codeid }),
                    defaultSort: "LookUpValue asc",
                    columns: [
                        { field: "LookUpValue", title: "Training Code" },
                        { field: "LookUpValueName", title: "Training Name" }
                    ]
                });
                lookup.dblClick(function (data) {
                    if (data != null) {
                        me.isSave = false;
                        me.detail.TrainingCode = data.LookUpValue;
                        me.Apply();
                    }
                });

            };

    me.Area = function () {
                var lookup = Wx.blookup({
                    name: "CityLookup",
                    title: "Lookup City",
                    manager: gnManager,
                    query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "AREA" }),
                    defaultSort: "LookUpValue asc",
                    columns: [
                        { field: "LookUpValue", title: "Area Code" },
                        { field: "LookUpValueName", title: "Area Name" }
                    ]
                });
                lookup.dblClick(function (data) {
                    if (data != null) {
                        me.isSave = false;
                        me.data.AreaCode = data.LookUpValue;
                        me.data.AreaName = data.LookUpValueName;
                        me.Apply();
                    }
                });

            };

    me.Province = function () {
                var lookup = Wx.blookup({
                    name: "CityLookup",
                    title: "Lookup City",
                    manager: gnManager,
                    query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "Prov" }),
                    defaultSort: "LookUpValue asc",
                    columns: [
                        { field: "LookUpValue", title: "Province Code" },
                        { field: "LookUpValueName", title: "Province Name" }
                    ]
                });
                lookup.dblClick(function (data) {
                    if (data != null) {
                        me.isSave = false;
                        me.data.ProvinceCode = data.LookUpValue;
                        me.data.ProvinceName = data.LookUpValueName;
                        me.Apply();
                    }
                });

            };

    me.Title = function () {
                var lookup = Wx.blookup({
                    name: "CityLookup",
                    title: "Lookup City",
                    manager: gnManager,
                    query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "TITL" }),
                    defaultSort: "LookUpValue asc",
                    columns: [
                        { field: "LookUpValue", title: "Position Code" },
                        { field: "LookUpValueName", title: "Position Name" }
                    ]
                });
                lookup.dblClick(function (data) {
                    if (data != null) {
                        me.isSave = false;
                        me.data.TitleCode = data.LookUpValue;
                        me.data.TitleName = data.LookUpValueName;
                        me.Apply();
                    }
                });
            };

    me.cancelOrClose = function () {
                $('#btnAddTraining').prop('disabled', true);
                $('#btnMove').prop('disabled', true);
                        me.init();
            }

    me.saveData = function (e, param) {

                        $http.post('gn.api/Employee/Save', me.data).
                            success(function (data, status, headers, config) {
                                if (data.status) {
                                    Wx.Success("Data saved...");
                                    me.startEditing();
                                    $('#btnAddTraining').prop('disabled', false);
                                    $('#btnMove').prop('disabled', false);
                                    $('#TrainingCode').prop('disabled', false);
                                    $('#BeginTrainingDate').prop('disabled', false);
                                    $('#EndTrainingDate').prop('disabled', false);
                                } else {
                                    MsgBox(data.message, MSG_ERROR);
                                }
                            }).
                            error(function (data, status, headers, config) {
                                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                            });
                    };

    me.AddEditTraining = function () {
        //alert(me.detail.TrainingCode);
        me.detail.EmployeeID = me.data.EmployeeID;
                if (me.detail.TrainingCode == '' || !me.detail.TrainingCode) {
                    SimDms.Warning("Please fill Training Code!");
                } else {
                    me.LinkDetail();
                    if ($('#isActive').prop('checked') == true) {
                        me.detail.isActive = true;
                    } else {
                        me.detail.isActive = false;
                    }
                    if ($('#isSuzukiTraining').prop('checked') == true) {
                        me.detail.isSuzukiTraining = true;
                    } else { me.detail.isSuzukiTraining = false; }
                    //$http.post('gn.api/employee/SaveTraining', $.extend({ EmployeeID: me.data.EmployeeID }, me.detail)).
                         $http.post('gn.api/employee/SaveTraining',me.detail).
                        success(function (data, status, headers, config) {
                            if (data.status) {
                                me.detail = {};
                                Wx.Success(data.message);
                                me.startEditing();
                                me.clearTable(me.grid1);
                                me.grid.model = data.data;
                                me.loadTableData(me.grid1, me.grid.model);
                                me.detail.BeginTrainingDate = me.now();
                                me.detail.EndTrainingDate = me.now();
                            } else {
                                MsgBox(data.message, MSG_ERROR);
                            }
                        }).
                        error(function (data, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        });
                }
                
    };

    me.delete = function () {
                MsgConfirm("Are you sure to delete current record?", function (result) {
                    $http.post('gn.api/Employee/Delete', me.data).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            me.init();
                            Wx.Success("Data deleted...");
                        } else {
                            MsgBox(data.message, MSG_ERROR);
                        }
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
                });
            }

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.LinkDetail();
                $http.post('gn.api/Employee/delete2', me.detail).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.detail = {};
                        me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = dl.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.detail.BeginTrainingDate = me.now();
                        me.detail.EndTrainingDate = me.now();
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e);
                });
            }
        });
    }

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.data.BirthDate = me.now();
        me.data.JoinDate = me.now();
        me.data.ResignDate = me.now();
        $('#isActive').prop('checked', true);
        me.detail.isActive = true;
        me.detail.BeginTrainingDate = me.now();
        me.detail.EndTrainingDate = me.now();
        $('#btnTrainingCode').removeAttr("style");
    }

    $("[name = 'CityCode']").on('blur', function () {
        if ($('#CityCode').val() || $('#CityCode').val() != '') {
            lookupName('CITY', $('#CityCode').val());
        } else {
            me.data.CityName = '';
            me.City();
        }
    });

    $("[name = 'AreaCode']").on('blur', function () {
        if ($('#AreaCode').val() || $('#AreaCode').val() != '') {
            lookupName('AREA', $('#AreaCode').val());
        } else {
            me.data.AreaName = '';
            me.Area();
        }
    });

    $("[name = 'ProvinceCode']").on('blur', function () {
        if ($('#ProvinceCode').val() || $('#ProvinceCode').val() != '') {
            lookupName('PROV', $('#ProvinceCode').val());
        } else {
            me.data.ProvinceName = '';
            me.Province();
        }
    });

    $("[name = 'TitleCode']").on('blur', function () {
        if ($('#TitleCode').val() || $('#TitleCode').val() != '') {
            lookupName('TITL', $('#TitleCode').val());
        } else {
            me.data.TitleName = '';
            me.Title();
        }
    });

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "IsSuzukiTraining", header: "Suzuki Training", width: 100 },
                { id: "TrainingCode", header: "Training Code", width: 200 },
                { id: "BeginTrainingDate", header: "Begin Date", width: 200 },
                { id: "EndTrainingDate", header: "End Date", width: 200},
                { id: "IsActive", header: "Status", width: 200 },
                { id: "Notes", header: "Notes", width: 200 },
            ],

            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Master Employee Account",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataEmployeeClass",
                                title: "Master Employee",
                                items: [
                                    { name: "BranchCode", type: "hidden", text: "", cls: "span8"},
                                    { name: "EmployeeID", type: "text", text: "Employee ID", cls: "span5", disable: "IsEditing() || testDisabled", validasi: "required" },
                                     {
                                         type: "buttons",
                                         cls : "span3",
                                         items: [
                                             { name: "btnMove", text: "Pindah Cabang", cls: "button small", icon: "icon-search", click: "Move()", disable: true }
                                         ]
                                     },
                                    { name: "EmployeeName", type: "text", text: "Employee Name", cls: "span8", validasi: "required" },
                                    { name: "Address1", type: "text", text: "Address", cls: "span8" },
                                    { name: "Address2", type: "text", text: "", cls: "span8" },
                                    { name: "Address3", type: "text", text: "", cls: "span8" },
                                    { name: "Address4", type: "text", text: "", cls: "span8" },
                                    {
                                        type: "controls", text: "City", required: true, items: [
                                            { name: "CityCode", model: "data.CityCode", type: "popup", text: "City Code", cls: "span3", click: "City()", validasi: "required" },
                                            { name: "CityName", model: "data.CityName", type: "", text: "City Name", cls: "span5", readonly: true },
                                        ],
                                    },
                                     {
                                         type: "controls", text: "Area", required: true, items: [
                                             { name: "AreaCode", model: "data.AreaCode", type: "popup", text: "Area Code", cls: "span3", click: "Area()", validasi: "required" },
                                             { name: "AreaName", model: "data.AreaName", type: "", text: "Area Name", cls: "span5", readonly: true },
                                         ],
                                     },
                                      {
                                          type: "controls", text: "Province", required: true, items: [
                                              { name: "ProvinceCode", model: "data.ProvinceCode", type: "popup", text: "Province Code", cls: "span3", click: "Province()", validasi: "required" },
                                              { name: "ProvinceName", model: "data.ProvinceName", type: "", text: "Province Name", cls: "span5", readonly: true },
                                          ],
                                      },
                      
                                      { name: "ZipNo", type: "text", text: "Pos Code", cls: "span4" },
                                      { name: "PhoneNo", type: "text", text: "Phone No", cls: "span4" },
                                      { name: "FaxNo", type: "text", text: "Fax No", cls: "span4" },
                                      { name: "HpNo", type: "text", text: "Hp No", cls: "span4" },
                    
                                       {
                                           type: "controls", text: "Title", required: true, items: [
                                               { name: "TitleCode", model: "data.TitleCode", type: "popup", text: "Title Code", cls: "span3",  click: "Title()", validasi: "required"},
                                               { name: "TitleName", model: "data.TitleName", type: "", text: "Title Name", cls: "span5", readonly: true },
                                           ],
                                       },
                                    { name: "IdentityNo", type: "text", text: "Identity No/ SIM", cls: "span4 full" },
                                    { name: "GenderCode", type: "select2", text: "Gender", cls: "span4", datasource: "comboGenders", required: true, validasi: "required" },
                                    { name: "MaritalStatusCode", type: "select2", text: "Marital Status", cls: "span4", datasource: "comboMarital", required: true, validasi: "required" },
                                    { name: "BirthPlace", type: "text", text: "Birth Place", cls: "span4" },
                                    { name: "BirthDate", type: "ng-datepicker", text: "Birth Date", cls: "span4" },
                                    { name: "ReligionCode", type: "select2", text: "Religion", cls: "span4", datasource: "comboReligions", required: true, validasi: "required" },
                                    { name: "BloodCode", type: "select2", text: "Blood Group", cls: "span4", datasource: "comboBloods" },
                                    { name: "UniformSize", type: "select2", text: "Uniform Size", cls: "span4", datasource: "comboUniforms" },
                                    { name: "ShoesSize", type: "select2", text: "Shoes Size", cls: "span4", datasource: "comboShoeses" },
                                    { name: "Height", type: "text", text: "Height (cm)", cls: "span4" },
                                    { name: "Weight", type: "text", text: "Weight (Kg)", cls: "span4" },
                                    { name: "PersonnelStatus", type: "select2", text: "Personnel Status", cls: "span4", datasource: "comboPersonnels", required: true, validasi: "required" },
                                    { name: "FormalEducation", type: "select2", text: "Formal Education", cls: "span4", datasource: "comboEducations" },
                                    { name: "JoinDate", type: "ng-datepicker", text: "Join Date", cls: "span4" },
                                    { name: "ResignDate", type: "ng-datepicker", text: "Resign Date", cls: "span4" }
                ]
            },
            {
                name: "Training",
                                title: "Training/ Education",
                                items: [
                                        { name: 'isSuzukiTraining', model: "detail.isSuzukiTraining", type: 'check', text: 'Suzuki', cls: 'span2 full', float: 'left' },
                                        //{
                                            //type: "controls", text: "Training Code", required: true, items: [
                                                { name: "TrainingCode", model: "detail.TrainingCode", type: "popup", text: "Training Code", cls: "span2", readonly: true, click: "Code()", style: "background-color: rgb(255, 218, 204)" },
                                            //],
                                        //},
                                        { name: "BeginTrainingDate", model: "detail.BeginTrainingDate", type: "ng-datepicker", text: "Begin Date", cls: "span3", disable: true },
                                        { name: "EndTrainingDate", model: "detail.EndTrainingDate", type: "ng-datepicker", text: "End Date", cls: "span3", disable: true },
                                        { name: 'isActive', model: "detail.isActive", type: 'check', text: 'Active', cls: 'span2 full', float: 'left' },

                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddTraining", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditTraining()", show: "detail.oid === undefined", disable: true },
                                { name: "btnUpdateTraining", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditTraining()", show: "detail.oid !== undefined" },
                                { name: "btnDeleteTraining", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                { name: "btnCancelTraining", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                        ]
                    },
                ]
            },
            {
                name: "wxsalestarget",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("gnEmployeeController");
    }

});
