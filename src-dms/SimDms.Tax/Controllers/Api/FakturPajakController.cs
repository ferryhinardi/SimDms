using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers.Api 
{
    public class FakturPajakController : BaseController 
    {
        public string sender;
        private static string
            STANDARD = "A",
            RINCI = "B",
            GABUNGAN = "C",
            LAMPIRAN = "D",
            STANDARTBASICMODEL = "E",
            RINCIBASICMODEL = "F",
            KHUSUS = "G";

        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProductType = ProductType,
                ProfitCenter = ProfitCenter 
            });
        }

        public JsonResult ValidatePrint(FakturPajak model)
        {
            bool statusPrint = true;
            var msg = "";
            DateTime dtpStart = model.DateFrom;
            DateTime dtpEnd = model.DateTo; ;
            string txtNoFrom = model.FakturNoFrom;
            string txtNoTo = model.FakturNoTo;
            object data = new object();
            
            if (dtpStart > dtpEnd)
            {
                msg = "Tanggal Mulai harus lebih kecil atau sama dengan tanggal akhir !";
                return Json(new { success = false, message = msg });
            }
            int comparison = string.Compare(txtNoFrom, txtNoTo, false);

            if (model.isALL == false)
            {
                if (comparison > 0)
                {
                    msg = string.Format(ctx.SysMsgs.Find("6001").MessageCaption,  "No.Faktur Pajak Awal", "No.Faktur Pajak Akhir");
                    return Json(new { success = false, message = msg });
                }
            }
            else { txtNoFrom = txtNoTo = ""; }

            var dateForm = dtpStart.Date.ToString("yyyyMMdd");
            var dateTo = dtpEnd.Date.ToString("yyyyMMdd");

            if (model.ProfitCenter == "100")
            {
                bool hidePart = true;
                if (model.isShowSparePart == true)
                    hidePart = false;

                if (model.optionPrintFormat == "0")
                {
                    // get FPJ Sales from table Tax Fpj Config
                    TxFpjConfig dt = ctx.TxFpjConfigs.Find("FPJSales");
                    if (dt != null)
                    {
                        switch (dt.FpjValue.ToString())
                        {
                            case "Form1":
                               data = new
                                {
                                    ReportId = "OmFakturPajak",
                                    DateForm = dateForm,
                                    DateTo = dateTo,
                                    FakturNoFrom = txtNoFrom,
                                    FakturNoTo = txtNoTo,
                                    SignName = model.SignName,
                                    JobTitle = model.JobTitle, 
                                    HidePart = hidePart
                                };
                                break;
                            case "Form2":
                                data = new
                                {
                                    ReportId = "OmFakturPajak",
                                    DateForm = dateForm,
                                    DateTo = dateTo,
                                    FakturNoFrom = txtNoFrom,
                                    FakturNoTo = txtNoTo,
                                    SignName = model.SignName,
                                    JobTitle = model.JobTitle,
                                    HidePart = hidePart
                                };
                                break;
                            default:
                               msg = "Value Tidak Sesuai Format !";
                               break;
                        }
                    }
                    else
                    {
                        msg = "Setting Dahulu Form FPJ Untuk Pre-Printed Di Menu Utility !";
                        Json(new { success = false, message = msg });
                    }
                }
                else
                {
                    if (ProductType == "2W")
                    {
                        data = new
                        {
                            ReportId = "OmFakturPajakC2",
                            DateForm = dateForm,
                            DateTo = dateTo,
                            FakturNoFrom = txtNoFrom,
                            FakturNoTo = txtNoTo,
                            SignName = model.SignName,
                            JobTitle = model.JobTitle,
                            HidePart = hidePart
                        };
                    }
                    else
                    {
                        data = new
                        {
                            ReportId = "OmFakturPajakC",
                            DateForm = dateForm,
                            DateTo = dateTo,
                            FakturNoFrom = txtNoFrom,
                            FakturNoTo = txtNoTo,
                            SignName = model.SignName,
                            JobTitle = model.JobTitle,
                            HidePart = hidePart
                        };
                    }
                }
                //reportViewer.SetReportParameter("", (chkFooter.Checked == true) ? true : false, user.CoProfile.ProductType, cbHargaJual.Checked, cbPenggantian.Checked, cbUangMuka.Checked, cbTermijn.Checked);
            }

            else if (model.ProfitCenter == "200")
            {
                string docStart = txtNoFrom.Length == 19 ? txtNoFrom.Remove(0, 8) : txtNoFrom;
                string docEnd = txtNoTo.Length == 19 ? txtNoTo.Remove(0, 8) : txtNoTo;

                if (model.isALL == true)
                {
                    // Jika Printed
                    if (model.optionPrePrint == "1")//(rbPrinted.Checked)
                    {
                        // JIKA BERDASARKAN FPJ NO
                        if (model.optionSRGL == "0")//(rbOption1.Checked) // STANDARD
                        {
                            if (model.optionFullHalf == "0")// (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 1,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 1);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Standart = STANDARD,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),STANDARD);
                            }
                        }
                        else if (model.optionSRGL == "1") // RINCI
                        {
                            if (model.optionFullHalf == "0")  //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 2,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                               //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 2);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Rinci = RINCI, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),RINCI);
                            }
                        }
                        else if (model.optionSRGL == "2") //if (rbOption3.Checked) // GABUNGAN
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                if (model.isShowJasaMaterial== false) //if (cbJasaMaterial.Checked == false)
                                {
                                    data = new
                                    {
                                        ReportId = "SvRpTrn008",
                                        DocStart = docStart,
                                        DocEnd = docEnd,
                                        FPJType = 3,
                                        Potongan = 0,
                                        DateForm = dateForm,
                                        DateTo = dateTo
                                    };
                                    //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 3);
                                }
                                else
                                {
                                    data = new
                                    {
                                        ReportId = "SvRpTrn008D",
                                        DocStart = docStart,
                                        DocEnd = docEnd,
                                        FPJType = 8,
                                        Potongan = 0,
                                        DateForm = dateForm,
                                        DateTo = dateTo
                                    };
                                    //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 8);
                                }

                            }
                            else
                            {
                                msg = "Report untuk setengah form belum tersedia !";
                                return Json(new { success = false, message = msg });
                            }
                        }
                        else if (model.optionSRGL == "3") //if (rbOption4.Checked) // LAMPIRAN
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 4,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 4);
                            }
                            else
                            {
                                msg = "Report untuk setengah form belum tersedia !";
                                return Json(new { success = false, message = msg });
                            }
                        }
                        else if (model.optionSRGL == "4") //if (rbOption5.Checked) // STANDARD BODY REPAIR
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 5,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 5);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    StandartBasicModel = STANDARTBASICMODEL,
                                    DateForm = dateForm, 
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),STANDARD);
                            }
                        }
                        else if (model.optionSRGL == "5")//if (rbOption6.Checked) // RINCI BODY REPAIR
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 6,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 6);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    RincianBasicModel = RINCIBASICMODEL, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                               // reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), RINCI);
                            }
                        }
                        else if (model.optionSRGL == "6") //if (rbKhusus.Checked) // Khusus Body Repai
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 7,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 7);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Khusus = KHUSUS, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), RINCI);
                            }
                        }
                    }
                    else if (model.optionPrePrint == "0") //if (rbPrePrinted.Checked)
                    {
                        // JIKA BERDASARKAN FPJ NO
                        if (model.optionSRGL == "0")//if (rbOption1.Checked) // STANDARD
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 1,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 1);
                        }
                        else if (model.optionSRGL == "1") //if (rbOption2.Checked) // RINCI
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 2,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 2);
                        }
                        else if (model.optionSRGL == "2") //if (rbOption3.Checked) // GABUNGAN
                        {
                            if (model.isShowJasaMaterial == false) //if (cbJasaMaterial.Checked == false)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Pre",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 3,
                                    Potongan = model.isShowPotongan,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Pre",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 8,
                                    Potongan = model.isShowPotongan,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                            }
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 3);
                        }
                        else if (model.optionSRGL == "3") //if (rbOption4.Checked) // LAMPIRAN
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 4,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 4);
                        }
                        else if (model.optionSRGL == "4")//if (rbOption5.Checked) // STANDARD BODY REPAIR
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 5,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 5);
                        }
                        else if (model.optionSRGL == "5") //if (rbOption6.Checked) // RINCI BODY REPAIR
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 6,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 6);
                        }
                        else if (model.optionSRGL == "6") //if (rbKhusus.Checked) // Khusus Body Repair
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 7,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 7);
                        }
                    }
                }
                else // BERDASARKAN TANGGAL
                {
                    if (model.optionPrePrint == "1") //if (rbPrinted.Checked)
                    {
                        if (model.optionSRGL == "0") //if (rbOption1.Checked) // STANDARD
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 1,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 1);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Standart = STANDARD,  
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),STANDARD);
                            }
                        }
                        else if (model.optionSRGL == "1") //if (rbOption2.Checked) // RINCI
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 2,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 2);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Rinci = RINCI, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), RINCI);
                            }
                        }
                        else if (model.optionSRGL == "2") //if (rbOption3.Checked) // GABUNGAN
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                if (model.isShowJasaMaterial == false)//if (cbJasaMaterial.Checked == false)
                                {
                                    data = new
                                    {
                                        ReportId = "SvRpTrn008",
                                        DocStart = docStart,
                                        DocEnd = docEnd,
                                        FPJType = 3,
                                        Potongan = 0,
                                        DateForm = dateForm,
                                        DateTo = dateTo
                                    };
                                    //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 3);
                                }
                                else
                                {
                                    data = new
                                    {
                                        ReportId = "SvRpTrn008D",
                                        DocStart = docStart,
                                        DocEnd = docEnd,
                                        FPJType = 8,
                                        Potongan = 0,
                                        DateForm = dateForm,
                                        DateTo = dateTo
                                    };
                                    //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 8);
                                }
                            }
                            else
                            {
                                msg = "Report untuk setengah form belum tersedia !";
                                return Json(new { success = false, message = msg });
                            }
                        }
                        else if (model.optionSRGL == "3") //if (rbOption4.Checked) // LAMPIRAN
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 4,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 4);
                            }
                            else
                            {
                                msg = "Report untuk setengah form belum tersedia !";
                                return Json(new { success = false, message = msg });
                            }
                        }
                        if (model.optionSRGL == "4")//if (rbOption5.Checked) // STANDARD
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 5,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 5);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Standart = STANDARD, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), STANDARD);
                            }
                        }
                        else if (model.optionSRGL == "5") //if (rbOption6.Checked) // RINCI
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 6,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 6);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Rinci = RINCI, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),RINCI);
                            }
                        }
                        else if (model.optionSRGL == "6") //if (rbKhusus.Checked) // Khusus Body Repair
                        {
                            if (model.optionFullHalf == "0") //if (rbFull.Checked)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 7,
                                    Potongan = 0,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 7);
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Short",
                                    ProductType = ProductType,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    Khusus = KHUSUS, 
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                                //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(),KHUSUS);
                            }
                        }
                    }
                    else if (model.optionPrePrint =="0") // if (rbPrePrinted.Checked)
                    {
                        if (model.optionSRGL == "0")//if (rbOption1.Checked) // STANDARD
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 1,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 1);
                        }
                        else if (model.optionSRGL == "1") //if (rbOption2.Checked) // RINCI
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 2,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 2);
                        }
                        else if (model.optionSRGL == "2") //if (rbOption3.Checked) // GABUNGAN
                        {
                            if (model.isShowJasaMaterial == false) //if (cbJasaMaterial.Checked == false)
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Pre",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 3,
                                    Potongan = model.isShowPotongan,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                            }
                            else
                            {
                                data = new
                                {
                                    ReportId = "SvRpTrn008Pre",
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    FPJType = 8,
                                    Potongan = model.isShowPotongan,
                                    DateForm = dateForm,
                                    DateTo = dateTo
                                };
                            }
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 3);
                        }
                        else if (model.optionSRGL == "3") //if (rbOption4.Checked) // LAMPIRAN
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 4,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 4);
                        }

                        if (model.optionSRGL == "4")//if (rbOption5.Checked) // STANDARD BODY REPAIR
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 5,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 5);
                        }
                        else if (model.optionSRGL == "5") //if (rbOption6.Checked) // RINCI BODY REPAIR
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 6,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 6);
                        }
                        else if (model.optionSRGL == "6") //if (rbKhusus.Checked) // Khusus Body Repai
                        {
                            data = new
                            {
                                ReportId = "SvRpTrn008Pre",
                                DocStart = docStart,
                                DocEnd = docEnd,
                                FPJType = 7,
                                Potongan = model.isShowPotongan,
                                DateForm = dateForm,
                                DateTo = dateTo
                            };
                            //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim(), 7);
                        }
                    }
                }
            }

            else if (model.ProfitCenter == "300")
            {
                //string docStart = txtNoFrom.Length == 19 ? txtNoFrom.Remove(0, 8) : txtNoFrom;
                //string docEnd = txtNoTo.Length == 19 ? txtNoTo.Remove(0, 8) : txtNoTo;

                string docStart = txtNoFrom;
                string docEnd = txtNoTo;

                var ReportID ="";
                if (model.optionPPFF == "0")
                {
                        if (model.rbPrePrinted == "True")
                        {
                            /* CHECK REPORT KIND (FULL OR HALF) */
                            ReportID = "SpRpTrn010PrePrinted";
                            if (sender == null)
                                ReportID = (model.optionFullHalf == "0") ? "SpRpTrn010PrePrinted" : "SpRpTrn010Short";
                            else if (sender == "SBAM")
                                ReportID = (model.optionFullHalf == "0") ? "SpRpTrn010SBAMPrePrinted" : "SpRpTrn010Short";

                             data = new
                            {
                                ReportId = ReportID,
                                periodBegin = dateForm,
                                periodEnd = dateTo,
                                DocStart = docStart,
                                DocEnd = docEnd,
                                profitCenter = model.ProfitCenter, 
                                SeqNo = 1
                            };
                            //viewer.SetReportParameter(signName, titleSign);
                            //viewer.ShowReport();
                        }
                        else
                        {
                            if (model.optionFullHalf == "1")
                            {
                                  msg = "Report untuk setengah form belum tersedia !";
                                  return Json(new { success = false, message = msg });
                            }
                            else
                            {
                                 ReportID = "SpRpTrn011A";
//                                 var query = string.Format(@"
//                                                        SELECT * FROM SysReport
//                                                            WHERE  ReportId = '{0}'
//                                                            ", ReportID);
//                                var dt = ctx.Database.SqlQuery<string>(query).First();
//                                string reportName = dt != null ? dt : string.Empty;
                                 data = new
                                {
                                    ReportId = ReportID,
                                    periodBegin = dateForm,
                                    periodEnd = dateTo,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    profitCenter = model.ProfitCenter
                                };
                                //viewer.SetReportParameter(reportId, reportName, signName, titleSign);
                                //viewer.ShowReport();
                            }
                        }
                }
                else if (model.optionPPFF == "1")
                {
                    if (model.optionFullHalf == "0")//if (rbFull.Checked)
                    {
                         TxFpjConfig dt = ctx.TxFpjConfigs.Find("FPJSales");
                        if (dt != null)
                        {
                            switch (dt.FpjValue.ToString())
                            {
                                case "Form1":
                                    data = new
                                            {
                                                ReportId = "SpRpTrn038",
                                                DateForm = dateForm,
                                                DateTo = dateTo,
                                                FakturNoFrom = txtNoFrom,
                                                FakturNoTo = txtNoTo,
                                                ProfitCenter = model.ProfitCenter,
                                                SignName = model.SignName
                                            };
                                    break;
                                case "Form2":
                                       data = new
                                            {
                                                ReportId = "SpRpTrn038A",
                                                DateForm = dateForm,
                                                DateTo = dateTo,
                                                FakturNoFrom = txtNoFrom,
                                                FakturNoTo = txtNoTo,
                                                ProfitCenter = model.ProfitCenter,
                                                SignName = model.SignName
                                            };
                                    break;
                                default:
                                    msg = "Value Tidak Sesuai Format !";
                                    break;
                            }
                        }
                        else
                        {
                            msg = "Setting Dahulu Form FPJ Untuk Pre-Printed Di Menu Utility !";
                                return Json(new { success = false, message = msg });
                        }
                    }
                    else
                    {
                        data = new
                                            {
                                                ReportId = "SpRpTrn038Short",
                                                DateForm = dateForm,
                                                DateTo = dateTo,
                                                FakturNoFrom = txtNoFrom,
                                                FakturNoTo = txtNoTo,
                                                ProfitCenter = model.ProfitCenter,
                                                SeqNo = 1
                                            };
                    }
                    //reportViewer.SetReportParameter(cbSignName.Text.Trim(), txtTitleSign.Text.Trim());
                }
                else if (model.optionPPFF == "2")//if (rbOption3.Checked)
                {
                    if (model.optionFullHalf == "0")//if (rbFull.Checked)
                    {
                        {
                            if (model.rbPrePrinted == "True")//if (rbPrePrinted.Checked)
                            {
                                data = new
                                            {
                                                ReportId = "SpRpTrn038V2Pre",
                                                DateForm = dateForm,
                                                DateTo = dateTo,
                                                FakturNoFrom = txtNoFrom,
                                                FakturNoTo = txtNoTo,
                                                ProfitCenter = model.ProfitCenter,
                                                SeqNo = 1 
                                            };
                                //XReportViewer viewer = new XReportViewer("SpRpTrn038V2Pre", companyCode, branchCode, periodBegin, periodEnd, firstFPJNo, lastFPJNo, profitCenterCode, seqNo);
                                //viewer.SetReportParameter(signName, titleSign);
                                //viewer.ShowReport();
                            }
                            else
                            {
                                 
                                ReportID = "SpRpTrn011A";

                                var query = string.Format(@"
                                                        SELECT * FROM SysReport
                                                            WHERE  ReportId = '{0}'
                                                            ", ReportID);
                                //var dt = ctx.Database.SqlQuery<string>(query).First();
                                data = new
                                {
                                    ReportId = ReportID,
                                    periodBegin = dateForm,
                                    periodEnd = dateTo,
                                    DocStart = docStart,
                                    DocEnd = docEnd,
                                    profitCenter = model.ProfitCenter
                                };
                                //XReportViewer viewer = new XReportViewer(reportId, companyCode, branchCode, periodBegin, periodEnd, firstFPJNo, lastFPJNo, profitCenterCode);
                                //viewer.SetReportParameter(reportId, reportName, signName, titleSign);
                                //viewer.ShowReport();
                            }
                        }
                    }
                    else
                    {
                          msg = "Report untuk setengah form belum tersedia !";
                          return Json(new { success = false, message = msg });
                    }
                }
                else
                {
                    if (model.optionFullHalf == "0")
                    {
                         data = new
                                            {
                                                ReportId = "SpRpTrn038V2",
                                                DateForm = dateForm,
                                                DateTo = dateTo,
                                                FakturNoFrom = txtNoFrom,
                                                FakturNoTo = txtNoTo,
                                                ProfitCenter = model.ProfitCenter,
                                                SignName = model.SignName
                                            };
                    }
                    else
                    {
                        msg = "Report untuk setengah form belum tersedia !";
                        return Json(new { success = false, message = msg });
                    }
                }
            }
            else
            {
                int statusCoret = 0;
                if (model.isHargaJual == true)
                    statusCoret++;
                if (model.isPenggantian == true)
                    statusCoret++;
                if (model.isTermijn == true)
                    statusCoret++;
                if (model.isDP== true)
                    statusCoret++;

                if (model.StatusCoret == 4)
                {
                        if (model.optionPrePrint== "0")//if (rbOption1.Checked)
                        {
                            data = new
                            {
                                ReportId = "ArRpTrn017B",
                                DateForm = dateForm,
                                DateTo = dateTo,
                                FakturNoFrom = txtNoFrom,
                                FakturNoTo = txtNoTo,
                                SignName = model.SignName,
                                JobTitle = model.JobTitle
                            };
                            //reportViewer.SetReportParameter("", "", "", cbHargaJual.Checked, cbPenggantian.Checked, cbUangMuka.Checked, cbTermijn.Checked);
                        }
                        else
                        {
                            data = new
                            {
                                ReportId = "ArRpTrn017",
                                DateForm = dateForm,
                                DateTo = dateTo,
                                FakturNoFrom = txtNoFrom,
                                FakturNoTo = txtNoTo,
                                SignName = model.SignName,
                                JobTitle = model.JobTitle
                            };
                            //reportViewer.SetReportParameter("", cbHargaJual.Checked, cbPenggantian.Checked, cbUangMuka.Checked, cbTermijn.Checked);
                        }
                    }
                else
                {
                     if (model.optionPrePrint== "0")//if (rbOption1.Checked)
                    {
                        data = new
                        {
                            ReportId = "ArRpTrn017B",
                            DateForm = dateForm,
                            DateTo = dateTo,
                            FakturNoFrom = txtNoFrom,
                            FakturNoTo = txtNoTo,
                            SignName = model.SignName,
                            JobTitle = model.JobTitle
                        };
                        //reportViewer.SetReportParameter("", "", "", cbHargaJual.Checked, cbPenggantian.Checked, cbUangMuka.Checked, cbTermijn.Checked);
                    }
                    else
                    {
                        data = new
                        {
                            ReportId = "ArRpTrn017",
                            DateForm = dateForm,
                            DateTo = dateTo,
                            FakturNoFrom = txtNoFrom,
                            FakturNoTo = txtNoTo,
                            SignName = model.SignName,
                            JobTitle = model.JobTitle
                        };
                        //reportViewer.SetReportParameter("", cbHargaJual.Checked, cbPenggantian.Checked, cbUangMuka.Checked, cbTermijn.Checked);
                    }
                }
            }

            if (statusPrint)
                return Json(new { success = true, message = "", data = data });
            else 
                return Json(new { success = false, message = msg});
        }

        //public GetPeriod GetPeriod()
        //{
        //    var data = ctx.CoProfileFinances.Select(a => new GetPeriod
        //    {
        //        Code = "AP",
        //        CompanyCode = a.CompanyCode,
        //        BranchCode = a.BranchCode,
        //        PeriodBeg = a.PeriodBeg.Value,
        //        PeriodEnd = a.PeriodEnd.Value
        //    }).Union(ctx.CoProfileFinances.Select(a => new GetPeriod
        //    {
        //        Code = "AR",
        //        CompanyCode = a.CompanyCode,
        //        BranchCode = a.BranchCode,
        //        PeriodBeg = a.PeriodBegAR.Value,
        //        PeriodEnd = a.PeriodEndAR.Value
        //    })).Union(ctx.CoProfileFinances.Select(a => new GetPeriod
        //    {
        //        Code = "GL",
        //        CompanyCode = a.CompanyCode,
        //        BranchCode = a.BranchCode,
        //        PeriodBeg = a.PeriodBegGL.Value,
        //        PeriodEnd = a.PeriodEndGL.Value
        //    })).Union(ctx.CoProfileSaleses.Select(a => new GetPeriod
        //    {
        //        Code = "SALES",
        //        CompanyCode = a.CompanyCode,
        //        BranchCode = a.BranchCode,
        //        PeriodBeg = a.PeriodBeg.Value,
        //        PeriodEnd = a.PeriodEnd.Value
        //    })).FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.Code.Equals("SALES"));

        //    return data;
        //}
    }
}
    
