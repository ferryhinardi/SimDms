$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        })
    })
}

var VIN = {
    Engine: "",
    SalesModel: "",
    Colour: "",
    ServiceBookNo: "",
    KeyNo: "",
    Dealer: "",
    DONo: "",
    SJNo: ""
}

//function MyViewModel() {
//    var self = this;
//    self.data = ko.observable(VIN);     
//};

//ko.applyBindings(new MyViewModel());


//var ViewModel = function (data) {
//    this.Engine = data.data.EngineCode + data.data.EngineNo;
//    this.SalesModel = data.data.SalesModelCode + data.data.SalesModelYear;
//    this.Colour = data.Colour + '(' + data.data.ColourCode + ')';
//    this.ServiceBookNo = data.data.ServiceBookNo;
//    this.KeyNo = data.data.KeyNo;
//    this.Dealer = data.DealerAbbreviation + '(' + data.data.CompanyCode + ')';
//    this.DONo = data.data.DONo;
//    this.SJNo = "";
//};

var ClearDoc = function ()
{
    VIN = {
        Engine: "",
        SalesModel: "",
        Colour: "",
        ServiceBookNo: "",
        KeyNo: "",
        Dealer: "",
        DONo: "",
        SJNo: ""
    };
    $.each(VIN, function (key, val) {
        document.getElementById("id" + key).value = val;
    });
    document.getElementById("txtVIN").value = "";
    document.getElementById("txtDriver").value = "";

    $("#txtVIN").focus();
};


var LoadDoc = function () {

    var a = document.getElementById("txtVIN").value;
    var url = "om.api/ReceivingUnit/GeData?Vehicle=" + a;

    $.ajax({
        method: "POST",
        url: url
    })
      .done(function (data) {
          if (data.data != undefined) {
              VIN = {
                  Engine: data.data.EngineCode + data.data.EngineNo,
                  SalesModel: data.data.SalesModelCode + data.data.SalesModelYear,
                  Colour: data.Colour + '(' + data.data.ColourCode + ')',
                  ServiceBookNo: data.data.ServiceBookNo,
                  KeyNo: data.data.KeyNo,
                  Dealer: data.DealerAbbreviation + '(' + data.data.CompanyCode + ')',
                  DONo: data.data.DONo,
                  SJNo: ""
              }

              $.each(VIN, function (key, val) {
                  document.getElementById("id" + key).value = val;
              });

              console.log(VIN)
          }
      });

}

$("#txtVIN").enterKey(function () {
    var a = $("#txtVIN").val();
    var url = "om.api/ReceivingUnit/GeData?Vehicle=" + a;

    $.ajax({
        method: "POST",
        url: url
    })
      .done(function (data) {
          if (data.data != undefined) {
              VIN = {
                  Engine: data.data.EngineCode + data.data.EngineNo,
                  SalesModel: data.data.SalesModelCode + data.data.SalesModelYear,
                  Colour: data.Colour + '(' + data.data.ColourCode + ')',
                  ServiceBookNo: data.data.ServiceBookNo,
                  KeyNo: data.data.KeyNo,
                  Dealer: data.DealerAbbreviation + '(' + data.data.CompanyCode + ')',
                  DONo: data.data.DONo,
                  SJNo: ""
              }

              $.each(VIN, function (key, val) {
                  document.getElementById("id" + key).value = val;
              });

              console.log(VIN)
          }
      });

})



