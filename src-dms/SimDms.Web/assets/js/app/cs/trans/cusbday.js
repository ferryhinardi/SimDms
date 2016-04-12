
//VARIABLES
var widget;
var variables = {};
var flag = 0;



//RENDERING VIEW

$(document).ready(function () {
    var options = {
        title: "Customer Birthday",
        xtype: "panels",
        panels: [
            {
                title: "Customer Details",
                items: [
                    {
                        text: "Dealer",
                        type: "controls", items: [
                            { name: "CompanyCode", type: "text", cls: "span2", placeHolder: "Dealer Code", readonly: true },
                            { name: "CompanyName", type: "text", cls: "span6", placeHolder: "Dealer Name", readonly: true }
                        ]
                    },
                    {
                        text: "Branch",
                        type: "controls", items: [
                            { name: "BranchCode", type: "text", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", type: "text", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Branch Code", readonly: true, type: "popup" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "CustomerAddress", type: "textarea", text: "Address", required: false, readonly: true },
                    { name: "CustomerTelephone", text: "Telephone", cls: "span5", required: false, readonly: true },
                    {
                        type: "controls",
                        text: "Birthdate",
                        items: [
                            { type: "text", name: "CustomerBirthDate", type: "date", cls: "span3", readonly: true },
                            { type: "text", name: "CustomerAge", cls: "span1", readonly: true },
                        ]
                    }
                ]
            },
            {
                title: "Marital Status",
                items: [
                    { name: "CustomerIsMarried", text: "Is Married", type: "switch", cls: "span4", float: "left" },
                ]
            },
            {
                title: "Husband / Wife Birthday",
                name: "PanelSpouse",
                items: [
                    { name: "SpouseName", text: "Husband / Wife Name" },
                    { name: "SpouseTelephone", text: "Telephone", cls: "span5" },
                    { name: "SpouseRelation", text: "Husband / Wife Relation", cls: "span5" },
                    { name: "SpouseBirthDate", type: "datepicker", text: "Husband / Wife Birth Date", cls: "span4" }
                ]
            },
            {
                title: "Children",
                name: "ChildrenOption",
                items: [
                    {
                        name: "NumberOfChildren", text: "Number of children", type: "select", cls: "span4",
                        items: [
                            { text: "0", value: "0" },
                            { text: "1", value: "1" },
                            { text: "2", value: "2" },
                            { text: "3", value: "3" }
                        ]
                    }
                ]
            },
            {
                title: "First Child Birthday",
                name: "PanelChild1",
                items: [
                    { name: "ChildName1", type: "text", text: "Children Name" },
                    { name: "ChildTelephone1", type: "text", text: "Telephone", cls: "span5" },
                    { name: "ChildBirthDate1", type: "datepicker", text: "Children birth date", cls: "span4" }
                ]
            },
            {
                title: "Second Child Birthday",
                name: "PanelChild2",
                items: [
                    { name: "ChildName2", type: "text", text: "Children Name" },
                    { name: "ChildTelephone2", type: "text", text: "Telephone", cls: "span5" },
                    { name: "ChildBirthDate2", type: "datepicker", text: "Children birth date", cls: "span4" }
                ]
            },
            {
                title: "Third Child Birthday",
                name: "PanelChild3",
                items: [
                    { name: "ChildName3", type: "text", text: "Children Name" },
                    { name: "ChildTelephone3", type: "text", text: "Telephone", cls: "span5" },
                    { name: "ChildBirthDate3", type: "datepicker", text: "Children birth date", cls: "span4" }
                ]
            },
            {
                title: "Customer Birthday Greeting by",
                name: "PanelSouvenirSentDateCustomer",
                items: [
                    { name: "CustomerGiftSentDate", text: "Sent Date", type: "datepicker", cls: "span4", float: "left" },
                    { name: "IsGiftTelephoneCustomer", text: "Telephone", type: "switch", cls: "span4 souvenir-switch", float: "left" },
                    { name: "IsGiftCardCustomer", text: "Gift Card", type: "switch", cls: "span4 souvenir-switch", float: "left" },
                    { name: "IsGiftLetterCustomer", text: "Letter", type: "switch", cls: "span4 souvenir-switch", float: "left" },
                    { name: "IsGiftSmsCustomer", text: "SMS", type: "switch", cls: "span4 souvenir-switch", float: "left" },
                    { name: "IsGiftSouvenirCustomer", text: "Souvenir", type: "switch", cls: "span4 souvenir-switch", float: "left" }
                ]
            },
            {
                title: "Souvenir Spouse",
                name: "PanelSouvenirSentDateSpouse",
                items: [
                    { name: "SpouseGiftSentDate", text: "Sent Date", type: "datepicker", cls: "span4 full", float: "left" },
                    { name: "IsGiftCardSpouse", text: "Gift Card", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftLetterSpouse", text: "Letter", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSmsSpouse", text: "SMS", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSouvenirSpouse", text: "Souvenir", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                title: "Souvenir Child 1",
                name: "PanelSouvenirSentDateChild1",
                items: [
                    { name: "ChildGiftSentDate1", text: "Sent Date", type: "datepicker", cls: "span4 full", float: "left" },
                    { name: "IsGiftCardChild1", text: "Gift Card", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftLetterChild1", text: "Letter", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSmsChild1", text: "SMS", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSouvenirChild1", text: "Souvenir", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                title: "Souvenir Child 2",
                name: "PanelSouvenirSentDateChild2",
                items: [
                    { name: "ChildGiftSentDate2", text: "Sent Date", type: "datepicker", cls: "span4 full", float: "left" },
                    { name: "IsGiftCardChild2", text: "Gift Card", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftLetterChild2", text: "Letter", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSmsChild2", text: "SMS", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSouvenirChild2", text: "Souvenir", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                title: "Souvenir Child 3",
                name: "PanelSouvenirSentDateChild3",
                items: [
                    { name: "ChildGiftSentDate3", text: "Sent Date", type: "datepicker", cls: "span4 full", float: "left" },
                    { name: "IsGiftCardChild3", text: "Gift Card", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftLetterChild3", text: "Letter", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSmsChild3", text: "SMS", type: "switch", cls: "span4", float: "left" },
                    { name: "IsGiftSouvenirChild3", text: "Souvenir", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                title: "Souvenir Received",
                name: "panelSouvenirReceived",
                items: [
                    { name: "CustomerGiftReceivedDate", text: "Customer", type: "datepicker", cls: "span4", float: "left" },
                    { name: "SpouseGiftReceivedDate", text: "Spouse", type: "datepicker", cls: "span4", float: "left" },
                    { name: "ChildGiftReceivedDate1", text: "Child 1", type: "datepicker", cls: "span4", float: "left" },
                    { name: "ChildGiftReceivedDate2", text: "Child 2", type: "datepicker", cls: "span4", float: "left" },
                    { name: "ChildGiftReceivedDate3", text: "Child 3", type: "datepicker", cls: "span4", float: "left" },
                ]
            },
            {
                title: "Customer Voice",
                items: [
                    { name: "CustomerComment", text: "Comment", type: "textarea", cls: "span8" },
                    { name: "SpouseComment", text: "Spouse", type: "textarea", cls: "span8" },
                    { name: "ChildComment1", text: "Child 1", type: "textarea", cls: "span8" },
                    { name: "ChildComment2", text: "Child 2", type: "textarea", cls: "span8" },
                    { name: "ChildComment3", text: "Child 3", type: "textarea", cls: "span8" },
                ]
            },
            {
                items: [
                    { name: "AdditionalInquiries", type: "textarea", text: "Additional inquiries", cls: "hide" },
                    //{ name: "Status", text: "Status", cls: "hide", readonly: true },
                    //{ name: "StatusInfo", text: "Status", cls: "span3", readonly: true },
                    //{ name: "StatusInfo", text: "Status", cls: "span2", type: "switch", readonly: true },
                    { name: "Finish", text: "Finish", cls: "span2", type: "switch", float: "left" },
                    { name: "Reason", text: "Alasan", cls: "span6", type: "select" },
                    { name: "field_kosong", text: "  ", cls: "span6", type: "divider" }
                ]
            },
        ],
        toolbars: [
            { name: "btnClear", text: "New", icon: "icon-file", cls: "hide" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
        ],
    }

    widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "Reason", url: "cs.api/Combo/Reasons/" },
    ]);
    widget.render(onAfterRender());
    widget.lookup.onDblClick(function (e, data, name) {
        evt_lookupOnDblClick(e, data, name);
    });
});





//ELEMENT EVENTS   

function evt_lookupOnDblClick(e, data, name) {
    if (widget.isNullOrEmpty(data) == false) {
        flag = 1;
        var isBirthdayEmpty = true;

        if (name == "Customer") {
            var params = {
                CustomerCode: data.CustomerCode
            };
            widget.showToolbars(["btnBrowse", "btnClear"]);
            widget.post("cs.api/CustBirthday/get", params, function (result) {
                if (result.status) {
                    if (widget.isNullOrEmpty(result.data) == false) {
                        widget.populate(result.data, populateData(result.data), syncCompanyInformation);
                        isBirthdayEmpty = false;
                    }
                }
                else {
                    isBirthdayEmpty = true;
                }
            });
        }
        else {
            widget.showToolbars(["btnBrowse", "btnClear", "btnDelete"]);
            isBirthdayEmpty = true;
        }

        if (isBirthdayEmpty) {
            var extendedData = $.extend({
                CustomerBirthDate: data.BirthDate,
                CustomerAddress: data.Address,
                CustomerTelephone: data.PhoneNo
            }, data);

            widget.populate(extendedData);
            populateData(extendedData);
        }
        widget.lookup.hide();
    }
}





//AREA PROCESS

function saveCustomerBirthday() {
    widget.showToolbars(["btnBrowse", "btnEdit", "btnClear"]);
    var isValid = $(".main form").valid();

    if (isValid == true) {
        onBeforeSaving();
        var params = widget.getForms();

        params["Status"] = widget.getValue({ name: "Finish", type: "switch" });
        if (params["Status"] == "false" && params["Reason"] == "") {
            $('#Reason').focus();
            widget.Error("Silahkan input field Alasan");

            var puterror = setInterval(function () {
                $('#Reason').addClass('error');
                setTimeout(function () { if ($('#Reason').hasClass("error")) { $('#Reason').removeClass('error'); } }, 250);
            }, 300);
            setTimeout(function () { clearInterval(puterror); }, 2400);

            return;
        }

        params["CustomerTypeOfGift"] = widget.getValue({ name: "IsGiftCardCustomer", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftLetterCustomer", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSmsCustomer", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSouvenirCustomer", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftTelephoneCustomer", type: "switch" });
        params["SpouseTypeOfGift"] = widget.getValue({ name: "IsGiftCardSpouse", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftLetterSpouse", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSmsSpouse", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSouvenirSpouse", type: "switch" });
        params["ChildTypeOfGift1"] = widget.getValue({ name: "IsGiftCardChild1", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftLetterChild1", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSmsChild1", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSouvenirChild1", type: "switch" });
        params["ChildTypeOfGift2"] = widget.getValue({ name: "IsGiftCardChild2", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftLetterChild2", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSmsChild2", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSouvenirChild2", type: "switch" });
        params["ChildTypeOfGift3"] = widget.getValue({ name: "IsGiftCardChild3", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftLetterChild3", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSmsChild3", type: "switch" }) + "|" + widget.getValue({ name: "IsGiftSouvenirChild3", type: "switch" });

        widget.post("cs.api/CustBirthday/Save", params, function (result) {
            if (result.success == true) {
                widget.showToolbars(["btnBrowse", "btnSave", "btnClear", "btnDelete"]);
                if (widget.isNullOrEmpty(result.data.Reason) == false) {
                    //result.data.Status = ((result.data.Finish == "true") ? 2 : 0);
                    $("#Reason").val(result.data.Reason.toUpperCase() || "");

                    //if (result.data.hasRelation == true) {
                    //    $("#StatusInfo").val("Done");
                    //}
                    //else {
                    //    $("#StatusInfo").val("Not Done");
                    //}
                }
            }
            widget.showNotification(result.message);

        });
    }
}

function deleteCustomerBirthday() {
    widget.confirm("Anda yakin akan menghapus data ini?", function (result) {
        if (result == "Yes") {
            var url = "cs.api/CustBirthday/Delete";
            var params = {
                "CustomerCode": $("[name='CustomerCode']").val(),
                "CompanyCode": $("[name='CompanyCode']").val()
            };

            widget.post(url, params, function (result) {
                if (result.success) {
                    clearFormInput();
                }

                widget.showNotification(result.message);
            });
        }
    });
}

function loadDefaultCompanyIdentity() {
    var url = "cs.api/CustBirthday/Default";
    widget.post(url, function (result) {
        if (widget.isNullOrEmpty(result) == false) {
            variables.CompanyCode = result.CompanyCode;
            variables.CompanyName = result.CompanyName;
            variables.BranchCode = result.BranchCode;
            variables.BranchName = result.BranchName;

            widget.populate(result);
        }
    });
}

function populateData(data) {
    syncCompanyInformation();

    var numOfChild = data.NumberOfChildren || 0;

    if (numOfChild > 0) {
        $("#NumberOfChildren").val(parseInt(numOfChild));
        showChildren(numOfChild);
    }
    else {
        $("NumberOfChildren").val("0");
    }

    if (widget.isNullOrEmpty(data.CustomerTypeOfGift) == false) {
        var customerTypeOfGift = data.CustomerTypeOfGift.split("|");
        if (customerTypeOfGift[0] == "true") {
            widget.changeSwitchValue({
                name: "IsGiftCardCustomer",
                value: "true"
            });
        }
        if (customerTypeOfGift[1] == "true") {
            widget.changeSwitchValue({
                name: "IsGiftLetterCustomer",
                value: "true"
            });
        }
        if (customerTypeOfGift[2] == "true") {
            widget.changeSwitchValue({
                name: "IsGiftSmsCustomer",
                value: true
            });
        }
        if (customerTypeOfGift[3] == "true") {
            widget.changeSwitchValue({
                name: "IsGiftSouvenirCustomer",
                value: true
            });
        }
        if (customerTypeOfGift[4] == "true") {
            widget.changeSwitchValue({
                name: "IsGiftTelephoneCustomer",
                value: true
            });
        }
    }

    if (data.NumberOfSpouse > 0) {
        if (widget.isNullOrEmpty(data.SpouseTypeOfGift) == false) {
            var spouseTypeOfGift = data.SpouseTypeOfGift.split("|");
            if (spouseTypeOfGift[0] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftCardSpouse",
                    value: "true"
                });
            }
            if (spouseTypeOfGift[1] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftLetterSpouse",
                    value: "true"
                });
            }
            if (spouseTypeOfGift[2] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSmsSpouse",
                    value: true
                });
            }
            if (spouseTypeOfGift[3] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSouvenirSpouse",
                    value: true
                });
            }
        }

        if (widget.isNullOrEmpty(data.ChildTypeOfGift1) == false) {
            var childTypeOfGift1 = data.ChildTypeOfGift1.split("|");
            if (childTypeOfGift1[0] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftCardChild1",
                    value: "true"
                });
            }
            if (childTypeOfGift1[1] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftLetterChild1",
                    value: "true"
                });
            }
            if (childTypeOfGift1[2] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSmsChild1",
                    value: true
                });
            }
            if (childTypeOfGift1[3] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSouvenirChild1",
                    value: true
                });
            }
        }

        if (widget.isNullOrEmpty(data.ChildTypeOfGift2) == false) {
            var childTypeOfGift2 = data.ChildTypeOfGift2.split("|");
            if (childTypeOfGift2[0] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftCardChild2",
                    value: "true"
                });
            }
            if (childTypeOfGift2[1] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftLetterChild2",
                    value: "true"
                });
            }
            if (childTypeOfGift2[2] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSmsChild2",
                    value: true
                });
            }
            if (childTypeOfGift2[3] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSouvenirChild2",
                    value: true
                });
            }
        }

        if (widget.isNullOrEmpty(data.ChildTypeOfGift3) == false) {
            var childTypeOfGift3 = data.ChildTypeOfGift3.split("|");
            if (childTypeOfGift3[0] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftCardChild3",
                    value: "true"
                });
            }
            if (childTypeOfGift3[1] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftLetterChild3",
                    value: "true"
                });
            }
            if (childTypeOfGift3[2] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSmsChild3",
                    value: true
                });
            }
            if (childTypeOfGift3[3] == "true") {
                widget.changeSwitchValue({
                    name: "IsGiftSouvenirChild3",
                    value: true
                });
            }
        }
    }

    if (data.NumberOfSpouse > 0) {
        showSpouse(true);
        widget.changeSwitchValue({
            name: "CustomerIsMarried",
            value: true
        });
    }
    else {
        showSpouse(false);
        widget.changeSwitchValue({
            name: "CustomerIsMarried",
            value: false
        });
    }

    if (data.Status == 1) {
        data.Finish = true;
        $("[name=Reason]").parent().parent().slideUp();
    }
    else {
        data.Finish = false;
        $("[name=Reason]").parent().parent().slideDown();
    }
    //if (data.Status == 1) {
    //    $("#StatusInfo").val("Done");
    //}
    //else {
    //    $("#StatusInfo").val("Not Done");
    //}

    CalculateAge(data);
}

function clearFormInput() {
    widget.clearForm();
    showChildren(0);
    showSpouse(false);
    flag = 0;
    widget.showToolbars(["btnBrowse"]);

    if (widget.isNullOrEmpty(variables["CompanyCode"])) {
        loadDefaultCompanyIdentity();
    }
    else {
        widget.populate(variables);
    }
}

function showSpouse(isVisible) {
    if (isVisible == true || isVisible == "true") {
        $("#PanelSpouse").fadeIn();
        $("#panelSouvenirReceived").fadeIn();
        widget.showPanels([
            {
                name: "PanelSouvenirSentDateSpouse",
                isVisible: true
            }
        ]);
        widget.showItem([
            {
                name: "SpouseComment",
                isVisible: true
            },
            {
                name: "SouvenirReceivedDateSpouse",
                isVisible: true
            }
        ]);
        showChildOption(true);
    }
    
    if (isVisible == false || isVisible == "false") {
        $("#PanelSpouse").fadeOut();
        $("#panelSouvenirReceived").fadeOut();

        setTimeout(function () {
            $("#panelSouvenirReceived .span4").show();
        }, (SimDms.switchChangeDelay || 500));

        widget.clearPanelInputs([
            "PanelSpouse",
            "PanelSouvenirSentDateSpouse"
        ]);
        widget.showItem([
            {
                name: "SpouseComment",
                isVisible: false
            },
            {
                name: "SouvenirReceivedDateSpouse",
                isVisible: false
            }
        ]);
        widget.showPanels([
            {
                name: "PanelSouvenirSentDateSpouse",
                isVisible: false
            }
        ]);

        showChildOption(false);
    }
}

function showChildOption(isVisible) {
    if (isVisible == true) {
        $("#ChildrenOption").fadeIn();
    }
    else {
        $("#ChildrenOption").fadeOut();
        $("#NumberOfChildren").val("0");
        showChildren(0);
    }
}

function showChildren(numOfChild) {
    if (numOfChild > 0) {
        for (var i = 1; i <= 3; i++) {
            if (i > numOfChild) {
                $("#PanelChild" + i).fadeOut();
                widget.clearPanelInputs([
                    "PanelChild" + i,
                    "PanelSouvenirSentDateChild" + i
                ]);
                widget.showItem([
                    {
                        name: "SouvenirReceivedDateChild" + i,
                        isVisible: false
                    },
                    {
                        name: "ChildComment" + i,
                        isVisible: false
                    },
                ]);
                widget.showPanels([
                    {
                        name: "PanelSouvenirSentDateChild" + i,
                        isVisible: false
                    }
                ]);
            }
            else {
                $("#PanelChild" + i).fadeIn();
                widget.showItem([
                    {
                        name: "SouvenirReceivedDateChild" + i,
                        isVisible: true
                    },
                    {
                        name: "ChildComment" + i,
                        isVisible: true
                    },
                ]);
                widget.showPanels([
                    {
                        name: "PanelSouvenirSentDateChild" + i,
                        isVisible: true
                    }
                ]);
            }
        }
    }
    else {
        for (var i = 1; i <= 3; i++) {
            $("#PanelChild" + i).fadeOut();
            widget.clearPanelInputs([
                "PanelChild" + i,
                "PanelSouvenirSentDateChild" + i
            ]);
            widget.showItem([
                {
                    name: "SouvenirReceivedDateChild" + i,
                    isVisible: false
                },
                {
                    name: "ChildComment" + i,
                    isVisible: false
                },
            ]);
            widget.showPanels([
                {
                    name: "PanelSouvenirSentDateChild" + i,
                    isVisible: false
                }
            ]);
        }
    }
}

function validateBtnSave() {
    var totalSwitch = $(".souvenir-switch input:checked").length;
    var totalY = 0;
    $.each($(".souvenir-switch input:checked"), function (index, item) {
        var id = $(item).attr("id");
        var status = id.substring(id.length - 1, id.length);
        if (status == "Y") totalY += 1
    });
    if (totalY > 0)
        return true;
    return false;
}

function initializeComponentEvent() {
    $(".souvenir-switch input, input[name=CustomerGiftSentDate]").change(function (e) {
        if (validateBtnSave() && $("input[name=CustomerGiftSentDate]").val() != "" && flag == 1)
            $("#btnSave").show();
        else
            $("#btnSave").hide();
    })
    $("input[name='CustomerIsMarried']").change(function (event) {
        var _customerIsMarried = $(this);

        setTimeout(function () {
            var maritalStatus = _customerIsMarried.val();
            if (maritalStatus == true || maritalStatus == "true") {
                maritalStatus = true;
            }
            else if (maritalStatus == false || maritalStatus == "false") {
                maritalStatus = false;
            }
            showSpouse(maritalStatus);
        }, (SimDms.switchChangeDelay || 500));
    });

    $("#NumberOfChildren").on("change", function (event) {
        showChildren($(this).val());
    });

    $("#btnClear").on("click", function () {
        clearFormInput();
    });

    $("#btnCustomerCode").on("click", function () {
        var lookup = widget.klookup({
            name: "CustomerBirthday",
            title: "Customer Birthday List",
            url: "cs.api/Lookup/CsCustBirthdays",
            //sort: ({ field: "CustomerBirthDay", dir: "asc" }),
            serverBinding: true,
            params: { Outstanding: "Y" },
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "CustomerCode", text: "ID Cust.", cls: "span2" },
                        { name: "CustomerName", text: "Customer Name", cls: "span6" },
                    ]
                }
            ],
            columns: [
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "CustomerTelephone", title: "PhoneNo", width: 250 },
                { field: "CustomerBirthDay", title: "Birth Day", width: 70 },
                { field: "CustomerBirthDate", title: "Birth Date", width: 80, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('MMM YYYY') #", filterable: false/*{ extra: true }*/ },
            ],
        });

        lookup.dblClick(function (data) {
            flag = 1;
            widget.showToolbars(["btnBrowse", "btnClear"]);
            widget.populate(data);
            var params = {
                CustomerCode: data.CustomerCode
            };
            widget.post("cs.api/CustBirthday/get", params, function (result) {
                if (result.success) {
                    if (widget.isNullOrEmpty(result.data) == false) {
                        widget.populate(result.data, populateData(result.data));
                        isBirthdayEmpty = false;
                    }
                }
                else {
                    isBirthdayEmpty = true;
                }
            });
        });
    });

    $("#btnBrowse").on("click", function () {
        var lookup = widget.klookup({
            name: "CustomerBirthday",
            title: "Customer Birthday List",
            url: "cs.api/Lookup/CsCustBirthdays",
            //sort: ({ field: "CustomerBirthDay", dir: "asc" }),
            //sort: ({ field: "CustomerCode", dir: "asc" }),
            serverBinding: true,
            params: { Outstanding: "N" },
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "CustomerCode", text: "ID Cust.", cls: "span2" },
                        { name: "CustomerName", text: "Customer Name", cls: "span6" },
                    ]
                }
            ],
            columns: [
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "PhoneNo", title: "Telephone", width: 120 },
                { field: "CustomerBirthDate", title: "Birth Date", width: 120, template: "#= (CustomerBirthDate == undefined) ? '' : moment(CustomerBirthDate).format('DD MMM YYYY') #" },
                { field: "Status", title: "Status", width: 100, template: "#= (Status == 1) ? 'FINISH' : '<span class=font-red>NOT FINISH</span>' #" },
            ],
        });

        lookup.dblClick(function (data) {
            widget.showToolbars(["btnBrowse", "btnClear", "btnDelete"]);

            if (widget.isNullOrEmpty(data) == false) {
                if (widget.isNullOrEmpty(data.SpouseName) == false) {
                    data["NumberOfSpouse"] = 1;
                    data["IsMarried"] = true;
                }

                var numberOfChildren = 0;
                for (var i = 0; i < 3; i++) {
                    var tempVal = data["ChildName" + (i+1)];

                    if (widget.isNullOrEmpty(tempVal) == false) {
                        numberOfChildren += 1;
                    }
                }

                data["NumberOfChildren"] = numberOfChildren;
            }
            widget.populate(data, populateData(data));
        });
    });

    $("#btnEdit").on("click", function () {
        widget.showToolbars(["btnSave", "btnCancel"]);
    });

    $("#btnSave").on("click", function () {
        saveCustomerBirthday();
    });

    $("#btnCancel").on("click", function () {
        widget.showToolbars(["btnBrowse", "btnEdit", "btnClear"]);
    });

    $("#btnDelete").on("click", function () {
        deleteCustomerBirthday();
    });

    $("#IsGiftSouvenirCustomerN, #IsGiftSouvenirCustomerY").on("change", function (evt) {
        var _this = $(this);

        setTimeout(function () {
            widget.showItem([
                {
                    name: "CustomerGiftReceivedDate",
                    isVisible: _this.val()
                }
            ]);

            countingSouvenirReceivedDateInput(_this.val());
        }, SimDms.defaultTimeout);
    });

    $("#IsGiftSouvenirSpouseY, #IsGiftSouvenirSpouseN").on("change", function (evt) {
        var _this = $(this);
        setTimeout(function () {
            var swithValue = _this.val();

            widget.showItem([
                {
                    name: "SpouseGiftReceivedDate",
                    isVisible: swithValue
                }
            ]);
          
            countingSouvenirReceivedDateInput(_this.val());
        }, SimDms.defaultTimeout);
    });

    for (var i = 1; i <= 3; i++) {
        $("#IsGiftSouvenirChild" + i + "Y, #IsGiftSouvenirChild" + i + "N").on("change", function (evt) {
            var _this = $(this);
            var currenElementName = _this.attr("name");

            setTimeout(function () {
                widget.showItem([
                   {
                       name: "ChildGiftReceivedDate" + currenElementName.slice(-1),
                       isVisible: _this.val()
                   }
                ]);
                countingSouvenirReceivedDateInput(_this.val());
            }, SimDms.defaultTimeout);
        });
    }


    $("#FinishN").on("click", function () {
        $("[name=Reason]").parent().parent().slideDown();
    });

    $("#FinishY").on("click", function () {
        $("[name=Reason]").parent().parent().slideUp();
    });

}





//AREA CALLBACK FUNCTION

function onAfterRender() {
    variables["souvenirReceivedDateCounter"] = 0;
    setTimeout(function () {
        loadDefaultCompanyIdentity();
        showSpouse(false);
        initializeComponentEvent();
        panelSouvenirReceivedListener(true);

        widget.showItem([
            {
                name: "SpouseGiftReceivedDate",
                isVisible: false
            },
            {
                name: "CustomerGiftReceivedDate",
                isVisible: false
            }
        ]);

        for (var i = 1; i <= 3; i++) {
            widget.showItem([
                {
                    name: "ChildGiftReceivedDate" + i,
                    isVisible: false
                }
            ]);
        }

    }, 500);
}

function onBeforeSaving() {

    if ($("input[name='CustomerIsMarried']").attr("value") == false) {
        widget.changeSwitchValue({ name: "CustomerIsMarried", value: false });

        if (widget.isNullOrEmpty($("#SpouseName").val()) == true) {
            showSpouse(false);
        }
    }
}

function syncCompanyInformation() {
    variables.CompanyCode = $("[name='CompanyCode']").val();
    variables.CompanyName = $("[name='CompanyName']").val();
    variables.BranchCode = $("[name='BranchCode']").val();
    variables.BranchName = $("[name='BranchName']").val();
}

function panelSouvenirReceivedListener(loopTwice) {

}

function countingSouvenirReceivedDateInput(value) {
    var panelVisibility = false;

    if (value == true || value == "true") {
        variables["souvenirReceivedDateCounter"] = variables["souvenirReceivedDateCounter"] + 1;
    }
    else if (value == false || value == "false") {
        variables["souvenirReceivedDateCounter"] = variables["souvenirReceivedDateCounter"] - 1;
    }

    if (variables["souvenirReceivedDateCounter"] > 0) {
        panelVisibility = true;
    }
    else {
        panelVisibility = false;
    }

    widget.showPanels([
        {
            name: "panelSouvenirReceived",
            isVisible: panelVisibility
        }
    ]);
}




//AREA UTILITAS
function CalculateAge(data) {
    var customerBirthDate = widget.cleanJsonDate(data.CustomerBirthDate);
    customerBirthDate = new Date(customerBirthDate);
    var age = parseInt(Date.now() - customerBirthDate);


    age = getAge(customerBirthDate);
    $("#CustomerAge").val(Math.round(age) + " tahun");
}

function getAge(dateString) {
    var today = new Date();
    var birthDate = new Date(dateString);
    var age = today.getFullYear() - birthDate.getFullYear();

    var m = today.getMonth() - birthDate.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }

    return age;
}