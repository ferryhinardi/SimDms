$(document).ready(function () {

    window.UserInfo = {};

    $.post(SimDms.baseUrl + "layout/listmodules",  function (result) {
        var html = "";
        $.each((result.data.sortBy("ModuleIndex") || []), function (idx, row) {
            var animate = ""; // "animated fadeInRight animate" + idx;
            if (row.IsPublish) {
                if (row.InternalLink) {
                    html += "<li class=\"" + animate + "\"><a href=\"" + SimDms.baseUrl + "layout#lnk/" + row.ModuleId + "\" >" + row.ModuleCaption + "</a></li>";
                }
                else {
                    html += "<li class=\"" + animate + "\"><a href=\"" + row.ModuleUrl + "?id=" + result.id + "\" >" + row.ModuleCaption + "</a></li>";
                }
            }
            else {
                html += "<li class=\"" + animate + "\"><label>" + row.ModuleCaption + "</label></li>";
            }
        });

        window.UserInfo = result.userdata;
        CheckPasswordExpired(html);

    });

    function showModules(html)
    {
        var modules = $("div.modules");
        modules.empty();
        modules.html("<ul>Modules" + html + "</ul>");        
    }

    function ChangePassword(html, dialogRef)
    {

        var ChangeModel = { 
            Username: window.UserInfo.UserId,
            OldPassword: $("#oldpassword").val(),
            NewPassword: $("#newpassword").val(),
            ConfirmPassword: $("#confirmpassword").val()
        };

        $.ajax({
                type: "POST",
                url: SimDms.baseUrl + "Account/ChangePassword",
                data: ChangeModel,
                traditional: true,
                success: function (result) {
                    if (result.success)
                    {
                       showModules(html);   
                       dialogRef.setClosable(true);
                       dialogRef.close();  
                       MsgBox(result.message, MSG_SUCCESS);
                    } 
                    else 
                    {
                        MsgBox(result.message, MSG_ERROR);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    MsgBox(errorThrown,MSG_ERROR);
                }
            });
    }

    function CheckPasswordExpired(html)
    {

        if (window.UserInfo.RequiredChange)
        {
            BootstrapDialog.show({
                        message:   $('<div class="row"><input id="oldpassword" class="form-control col-x-12" placeholder="Old Password" type="password"></div>' +
                                    '<div class="row"><input id="newpassword"  class="form-control col-x-12" placeholder="New Password" type="password"></div>' + 
                                    '<div class="row"><input id="confirmpassword"  class="form-control col-x-12" placeholder="Confirm Password" type="password"></div>'),
                        closable: false,
                        draggable: true,
                        type: BootstrapDialog.TYPE_DANGER,
                        title: 'Your password is expired.... Please change your password!',
                        buttons: [{
                            label: 'Change Password',
                            cssClass: 'btn-success',
                            action: function(dialogRef){
                                ChangePassword(html, dialogRef);
                            }
                        }, {
                            label: 'Cancel',
                            action: function(dialogRef){
                                dialogRef.close();
                                window.location.href = "LogOut";
                            }
                        }]
                    });
        } else {
            showModules(html);
            //window.location = "/layout";
        }

     }


});


Array.prototype.sortBy = function () {
    function _sortByAttr(attr) {
        var sortOrder = 1;
        if (attr[0] == "-") {
            sortOrder = -1;
            attr = attr.substr(1);
        }
        return function (a, b) {
            var result = (a[attr] < b[attr]) ? -1 : (a[attr] > b[attr]) ? 1 : 0;
            return result * sortOrder;
        }
    }
    function _getSortFunc() {
        if (arguments.length == 0) {
            throw "Zero length arguments not allowed for Array.sortBy()";
        }
        var args = arguments;
        return function (a, b) {
            for (var result = 0, i = 0; result == 0 && i < args.length; i++) {
                result = _sortByAttr(args[i])(a, b);
            }
            return result;
        }
    }
    return this.sort(_getSortFunc.apply(null, arguments));

}

