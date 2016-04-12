using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    class OmRpSalRgs040BXcl
    {
        #region Properties
        private string seq, seq2, area, dealer, outlet, modelCategory, colorCategory, marketModel, year;
        private int rowIndex = 0;
        private int indentTitleDesc = 6;
        private int marketModelCount = 1;
        private int dealerWidth, outletWidth, modelWidth, colorWidth, totalDetailWidth;

        private decimal janType, febType, marType, aprType, mayType, junType, julType, augType, sepType, octType, novType, decType, totalType;
        private decimal janSubTtl, febSubTtl, marSubTtl, aprSubTtl, maySubTtl, junSubTtl, julSubTtl, augSubTtl, sepSubTtl, octSubTtl, novSubTtl, decSubTtl, totalSubTtl;
        private decimal janDealer, febDealer, marDealer, aprDealer, mayDealer, junDealer, julDealer, augDealer, sepDealer, octDealer, novDealer, decDealer, total_Dealer;
        private decimal janGT, febGT, marGT, aprGT, mayGT, junGT, julGT, augGT, sepGT, octGT, novGT, decGT, totalGT, avgGT;
        private decimal janArea, febArea, marArea, aprArea, mayArea, junArea, julArea, augArea, sepArea, octArea, novArea, decArea, total_Area, avgArea;        
        private decimal janJabodetabek, febJabodetabek, marJabodetabek, aprJabodetabek, mayJabodetabek, junJabodetabek, julJabodetabek, augJabodetabek, sepJabodetabek, octJabodetabek, novJabodetabek, decJabodetabek, totalJabodetabek;

        private decimal janAVG, febAVG, marAVG, aprAVG, mayAVG, junAVG, julAVG, augAVG, sepAVG, octAVG, novAVG, decAVG, totalAVG;

        private int totalRow = 0, totalRowDtl = 0, beforeRowType = 0;
        private int totalAVGRow = 0, TotalAVGOutletRow = 0, TotalAVGAreaRow = 0, TotalAVGDealerRow = 0, totalAVGGTRow = 0, TotalAVGAreaRowJBDTK = 0;
        private int totalDealer = 0, totalOutlet = 0, spaceTotal = 0, spaceTotalDealer = 0, totalColour = 0, totalDealerGT = 1;
        private bool dealerStat = false;

        private DataSet ds = new DataSet();
        Dictionary<int, string> parameter = new Dictionary<int, string>();
        private StringBuilder sbSaveCurrentState;
        string fileName;
        decimal totalMonth = 0;
        decimal averageMonth = 0;
        decimal averageDealer = 0;
        decimal averageArea = 0;
        decimal averageMonthGT = 0;
        int currentYear = 0;
        int iOutlet=0;
        int iDealer = 0;
        DataTable dt0 = new DataTable();
        DataTable dt1 = new DataTable();
        StringBuilder sb = new StringBuilder();
        ExcelFileWriter excelReport = new ExcelFileWriter(0);
        #endregion

        public OmRpSalRgs040BXcl(DataSet dataSet, Dictionary<int, string> param, string flName)
        {
            ds = dataSet;
            parameter = param;
            fileName = flName;
        }

        void totalperoutlet()
        {

            excelReport.SetCellValue("TOTAL Outlet", 8 + rowIndex, 0 + dealerWidth, 1, 1 + outletWidth, ExcelCellStyle.YellowTotal);
            iOutlet++;
            decimal average = 1;// Convert.ToDecimal(row["Total"].ToString()) / averageDealer;
            average = totalType / totalMonth;
            //decimal jan = Convert.ToDecimal(row["Jan"].ToString());
            //decimal feb = Convert.ToDecimal(row["Feb"].ToString());
            //decimal mar = Convert.ToDecimal(row["Mar"].ToString());
            //decimal apr = Convert.ToDecimal(row["Apr"].ToString());
            //decimal may = Convert.ToDecimal(row["May"].ToString());
            //decimal jun = Convert.ToDecimal(row["Jun"].ToString());
            //decimal jul = Convert.ToDecimal(row["Jul"].ToString());
            //decimal aug = Convert.ToDecimal(row["Aug"].ToString());
            //decimal sep = Convert.ToDecimal(row["Sep"].ToString());
            //decimal oct = Convert.ToDecimal(row["Oct"].ToString());
            //decimal nov = Convert.ToDecimal(row["Nov"].ToString());
            //decimal dec = Convert.ToDecimal(row["Dec"].ToString());
            
            //excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, (parameter[14] != "" ? totalRowDtl : totalOutlet + 1), dealerWidth, ExcelCellStyle.LeftBorderedTop);
            //excelReport.SetCellValue("Total Per Outlet", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue(janType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janType < average && janType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(febType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febType < average && febType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(marType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marType < average && marType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(aprType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprType < average && aprType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(mayType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayType < average && mayType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(junType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junType < average && junType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(julType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julType < average && julType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(augType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augType < average && augType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(sepType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepType < average && sepType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(octType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octType < average && octType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(novType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novType < average && novType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(decType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decType < average && decType != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(totalType.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

            //seq = row["seq"].ToString();
            //outlet = row["BranchName"].ToString();
            //totalOutlet = totalRowDtl = parameter[14].ToString() != "" ? 0 : 1;
            //averageDealer = 0;
            rowIndex++;
            janAVG = janType / totalOutlet;
            febAVG = febType / totalOutlet;
            marAVG = marType / totalOutlet;
            aprAVG = aprType / totalOutlet;
            mayAVG = mayType / totalOutlet;
            junAVG = junType / totalOutlet;
            julAVG = julType / totalOutlet;
            augAVG = augType / totalOutlet;
            sepAVG = sepType / totalOutlet;
            octAVG = octType / totalOutlet;
            novAVG = novType / totalOutlet;
            decAVG = decType / totalOutlet;
            
            totalAVG = janAVG +
                     febAVG +
                     marAVG +
                     aprAVG +
                     mayAVG +
                     junAVG +
                     julAVG +
                     augAVG +
                     sepAVG +
                     octAVG +
                     novAVG +
                     decAVG;
            average = totalAVG / totalMonth;


            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, 1 + outletWidth, ExcelCellStyle.PurpleTotal);
            excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < average && janAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < average && febAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < average && marAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < average && aprAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < average && mayAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < average && junAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < average && julAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < average && augAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < average && sepAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < average && octAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < average && novAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < average && decAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

            janType =
            febType =
            marType =
            aprType =
            mayType =
            junType =
            julType =
            augType =
            sepType =
            octType =
            novType =
            decType =
            totalType = 0;

        }

        void totaldealer()
        {
            excelReport.SetCellValue("TOTAL Dealer", 8 + rowIndex, 0, 1, 1 + dealerWidth + outletWidth, ExcelCellStyle.BrownTotal);
            decimal average = 1;
            iDealer++;
            // Convert.ToDecimal(row["Total"].ToString()) / averageDealer;
            //decimal jan = Convert.ToDecimal(row["Jan"].ToString());
            //decimal feb = Convert.ToDecimal(row["Feb"].ToString());
            //decimal mar = Convert.ToDecimal(row["Mar"].ToString());
            //decimal apr = Convert.ToDecimal(row["Apr"].ToString());
            //decimal may = Convert.ToDecimal(row["May"].ToString());
            //decimal jun = Convert.ToDecimal(row["Jun"].ToString());
            //decimal jul = Convert.ToDecimal(row["Jul"].ToString());
            //decimal aug = Convert.ToDecimal(row["Aug"].ToString());
            //decimal sep = Convert.ToDecimal(row["Sep"].ToString());
            //decimal oct = Convert.ToDecimal(row["Oct"].ToString());
            //decimal nov = Convert.ToDecimal(row["Nov"].ToString());
            //decimal dec = Convert.ToDecimal(row["Dec"].ToString());
            average = total_Dealer / totalMonth;
            excelReport.SetCellValue(Convert.ToInt32(janDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janDealer < Convert.ToInt32(average) && janDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(febDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febDealer < Convert.ToInt32(average) && febDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(marDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marDealer < Convert.ToInt32(average) && marDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(aprDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprDealer < Convert.ToInt32(average) && aprDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(mayDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayDealer < Convert.ToInt32(average) && mayDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(junDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junDealer < Convert.ToInt32(average) && junDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(julDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julDealer < Convert.ToInt32(average) && julDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(augDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augDealer < Convert.ToInt32(average) && augDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(sepDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepDealer < Convert.ToInt32(average) && sepDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(octDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octDealer < Convert.ToInt32(average) && octDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(novDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novDealer < Convert.ToInt32(average) && novDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(decDealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decDealer < Convert.ToInt32(average) && decDealer != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(total_Dealer).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);

            rowIndex++;

            iOutlet = iOutlet == 0 ? 1 : iOutlet;
            janAVG = janDealer / iOutlet;
            febAVG = febDealer / iOutlet;
            marAVG = marDealer / iOutlet;
            aprAVG = aprDealer / iOutlet;
            mayAVG = mayDealer / iOutlet;
            junAVG = junDealer / iOutlet;
            julAVG = julDealer / iOutlet;
            augAVG = augDealer / iOutlet;
            sepAVG = sepDealer / iOutlet;
            octAVG = octDealer / iOutlet;
            novAVG = novDealer / iOutlet;
            decAVG = decDealer / iOutlet;

            totalAVG = janAVG +
                     febAVG +
                     marAVG +
                     aprAVG +
                     mayAVG +
                     junAVG +
                     julAVG +
                     augAVG +
                     sepAVG +
                     octAVG +
                     novAVG +
                     decAVG;
            average = totalAVG / totalMonth;

            excelReport.SetCellValue("AVG", 8 + rowIndex, 0, 1, 1 + dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);
            excelReport.SetCellValue(Convert.ToInt32(janAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(average) && janAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(febAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(average) && febAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(marAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(average) && marAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(aprAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(average) && aprAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(mayAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(average) && mayAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(junAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(average) && junAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(julAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(average) && julAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(augAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(average) && augAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(sepAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(average) && sepAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(octAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(average) && octAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(novAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(average) && novAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(decAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(average) && decAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(totalAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

            janDealer =
            marDealer =
            aprDealer =
            mayDealer =
            junDealer =
            julDealer =
            augDealer =
            sepDealer =
            octDealer =
            novDealer =
            decDealer =
            total_Dealer = 0;
            iOutlet = 0; 

        }

        void totalArea()
        {
            excelReport.SetCellValue("TOTAL Area "+ area, 8 + rowIndex, 0, 1, 1 + dealerWidth + outletWidth, ExcelCellStyle.PinkTotal);
            decimal average = 1;// Convert.ToDecimal(row["Total"].ToString()) / averageDealer;
            average = total_Area / totalMonth;
            excelReport.SetCellValue(Convert.ToInt32(janArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janArea < Convert.ToInt32(average) && janArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(febArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febArea < Convert.ToInt32(average) && febArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(marArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marArea < Convert.ToInt32(average) && marArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(aprArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprArea < Convert.ToInt32(average) && aprArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(mayArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayArea < Convert.ToInt32(average) && mayArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(junArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junArea < Convert.ToInt32(average) && junArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(julArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julArea < Convert.ToInt32(average) && julArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(augArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augArea < Convert.ToInt32(average) && augArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(sepArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepArea < Convert.ToInt32(average) && sepArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(octArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octArea < Convert.ToInt32(average) && octArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(novArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novArea < Convert.ToInt32(average) && novArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(decArea).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decArea < Convert.ToInt32(average) && decArea != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(total_Area).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);

            janAVG = janArea / iDealer;
            febAVG = febArea / iDealer;
            marAVG = marArea / iDealer;
            aprAVG = aprArea / iDealer;
            mayAVG = mayArea / iDealer;
            junAVG = junArea / iDealer;
            julAVG = julArea / iDealer;
            augAVG = augArea / iDealer;
            sepAVG = sepArea / iDealer;
            octAVG = octArea / iDealer;
            novAVG = novArea / iDealer;
            decAVG = decArea / iDealer;

            totalAVG = janAVG +
                     febAVG +
                     marAVG +
                     aprAVG +
                     mayAVG +
                     junAVG +
                     julAVG +
                     augAVG +
                     sepAVG +
                     octAVG +
                     novAVG +
                     decAVG;
            average = totalAVG / totalMonth;



            rowIndex++;
            excelReport.SetCellValue("AVG "+area, 8 + rowIndex, 0, 1, 1 + dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);
            excelReport.SetCellValue(Convert.ToInt32(janAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(average) && janAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(febAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(average) && febAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(marAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(average) && marAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(aprAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(average) && aprAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(mayAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(average) && mayAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(junAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(average) && junAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(julAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(average) && julAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(augAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(average) && augAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(sepAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(average) && sepAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(octAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(average) && octAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(novAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(average) && novAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(decAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(average) && decAVG != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(totalAVG).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
            
            janArea =
            febArea =
            marArea =
            aprArea =
            mayArea =
            junArea =
            julArea =
            augArea =
            sepArea =
            octArea =
            novArea =
            decArea =
            total_Area = 0;
            iOutlet = 0;
            iDealer = 0;
        }

        public void CreateReport(string pserver)
        {
            dt0 = ds.Tables[0];
            dt1 = ds.Tables[1];

            seq = dt0.Rows[0]["Seq"].ToString();
            seq2 = dt0.Rows[0]["Seq2"].ToString();
            area = parameter[9].ToString() == "1" ? dt0.Rows[0]["Area"].ToString() : "";
            dealer = parameter[10].ToString() == "1" ? dt0.Rows[0]["CompanyName"].ToString() : "";
            outlet = parameter[11].ToString() == "1" ? dt0.Rows[0]["BranchName"].ToString() : "";
            modelCategory = dt0.Columns.Contains("ModelCatagory") == true ? dt0.Rows[0]["ModelCatagory"].ToString() : "";
            marketModel = parameter[12].ToString() == "1" ? dt0.Rows[0]["MarketModel"].ToString() : "";
            colorCategory = parameter[13].ToString() == "1" ? dt0.Rows[0]["ColourName"].ToString() : "";
            year = dt0.Rows[0]["Year"].ToString();
            currentYear = Convert.ToInt32(dt0.Rows[0]["Year"].ToString());

            sb = new StringBuilder();
            sb.Append(parameter[11].ToString() == "1" ? "Outlet" : "");
            sb.Append((parameter[12].ToString() == "1" && parameter[14].ToString() == "") ? ", Type" : "");
            sb.Append(parameter[13].ToString() == "1" ? " & Colour" : "");
            sb.Append(parameter[14].ToString() != "" ? " & Type : " + parameter[14].ToString() : "");
            if (sb.Length != 0)
                sb.Append(" - ");
            sb.Append(parameter[16].ToString() + " " + parameter[15].ToString());

            string title = sb.ToString().StartsWith(",") ? sb.ToString().Remove(0, 1) : sb.ToString();

            //Pemetaan ukuran column
            dealerWidth = parameter[10].ToString() == "1" ? 1 : 0;
            outletWidth = parameter[11].ToString() == "1" ? 1 : 0;
            modelWidth = (parameter[12].ToString() == "1" && parameter[14].ToString() == "") ? 1 : 0;
            colorWidth = parameter[13].ToString() == "1" ? 1 : 0;

            totalDetailWidth = dealerWidth + outletWidth + modelWidth + colorWidth;

            excelReport = new ExcelFileWriter(fileName, currentYear.ToString(), "OmRpSalRgs045", pserver);
            CreateHeader(excelReport, rowIndex, totalDetailWidth, title, currentYear.ToString());
            marketModelCount = 1;


            area = "";
            dealer = "";
            outlet = "";
            colorCategory = "";

            foreach (DataRow row in dt0.Rows)
            {

                if (row["CompanyName"].ToString() == "SSBT")
                    outlet = outlet;
                if (row["CompanyName"].ToString().Contains("zzzz") || (row["seq"].ToString() != "3" && parameter[11].ToString() == "1"))
                    continue;
                totalMonth = (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
                    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 :
                    (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) == currentYear
                    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) :
                    (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
                    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) == currentYear) ? Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) :
                    Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) + 1;

                if (parameter[11].ToString() != "1" && parameter[12].ToString() != "1" && row["Seq"].ToString() != "1")
                {
                    averageArea = (totalMonth > averageArea) ? totalMonth : averageArea;
                    averageMonthGT = (totalMonth > averageMonthGT) ? totalMonth : averageMonthGT;
                    averageMonth = (totalMonth > averageMonth) ? totalMonth : averageMonth;
                }

                if (area == "")
                {
                    #region first run
                    excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                    if (parameter[11].ToString() == "1")
                    {
                    
                        if (parameter[13].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.LeftBorderedStandard);
                        }
                        else
                        {
                            excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedStandard);
                        }
                        outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                    }
                    else
                    {
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.LeftBorderedStandard);
                    }

                    dealer = row["CompanyName"].ToString();
                    area = row["Area"].ToString();
                    totalOutlet = 1;
                    totalDealer = 1;
                    #endregion
                }
                else
                {
                    if (parameter[11].ToString() == "1")
                    {
                        #region Outlet

                        if (area == row["Area"].ToString() &&
                            dealer == row["CompanyName"].ToString() &&
                            outlet == row["BranchName"].ToString())
                        {
                            totalOutlet++;
                            totalDealer++;
                        }

                        else if (area == row["Area"].ToString() &&
                            dealer == row["CompanyName"].ToString() &&
                            outlet != row["BranchName"].ToString())
                        {

                            excelReport.ReplaceRestorePoint("RestoreOutlet", outlet, 1, totalOutlet, 1, ExcelCellStyle.LeftBorderedStandard);
                            totalperoutlet();
                            totalDealer += 3;
                            totalOutlet = 1;
                           

                            //if (dealer == "SST")
                            //{
                            //    excelReport.CloseExcelFileWriter();
                            //    return;
                            //}

                            rowIndex++;

                            excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");

                            dealer = row["CompanyName"].ToString();
                            outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                        }
                        else if (area == row["Area"].ToString() &&
                      dealer != row["CompanyName"].ToString() &&
                      outlet != row["BranchName"].ToString())
                        {

                            if (parameter[11].ToString() == "1")
                            {
                                excelReport.ReplaceRestorePoint("RestoreOutlet", outlet, dealerWidth, totalOutlet, 1, ExcelCellStyle.LeftBorderedStandard);                                
                                totalperoutlet();
                                totalOutlet = 1;
                                rowIndex++;
                            }
                            //excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer, 1, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer + 2, 1, ExcelCellStyle.LeftBorderedStandard);
                           
                            totaldealer();
                            totalDealer = 1;
                            totalOutlet = totalDealer = 1;

                            rowIndex++;
                            //excelReport.CloseExcelFileWriter();
                            //return;
                            excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                            excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");
                            dealer = row["CompanyName"].ToString();
                            outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                            //excelReport.CloseExcelFileWriter();
                            //return;
                        }
                        else
                        {


                            if (parameter[11].ToString() == "1")
                            {
                                excelReport.ReplaceRestorePoint("RestoreOutlet", outlet, dealerWidth, totalOutlet, 1, ExcelCellStyle.LeftBorderedStandard);

                                totalOutlet = 1;
                                totalperoutlet();
                                rowIndex++;
                            }
                            //excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer, 1, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer + 2, 1, ExcelCellStyle.LeftBorderedStandard);
                            totalDealer = 1;
                            totaldealer();                       
                            rowIndex++;
                            totalArea();
                            rowIndex++;
                            totalOutlet = totalDealer = 1;
                            //excelReport.CloseExcelFileWriter();
                            //return;

                            excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                            excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");

                            dealer = row["CompanyName"].ToString();
                            outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                            area = row["Area"].ToString();

                            //excelReport.CloseExcelFileWriter();
                            //return;
                        }
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.LeftBorderedStandard);




                        #endregion

                    }
                    else
                    {
                        #region not outlet


                        if (area == row["Area"].ToString() &&
                            dealer == row["CompanyName"].ToString())
                        {
                            totalOutlet++;
                            totalDealer++;
                        }

                        else if (area == row["Area"].ToString() &&
                               dealer != row["CompanyName"].ToString())
                        {

                       
                            //excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer, 1, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer , 1, ExcelCellStyle.LeftBorderedStandard);
                        
                            totaldealer();
                            totalDealer = 1;
                            totalOutlet = totalDealer = 1;

                            rowIndex++;
                            //excelReport.CloseExcelFileWriter();
                            //return;
                            excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                            //excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");
                            dealer = row["CompanyName"].ToString();
                            outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                            //excelReport.CloseExcelFileWriter();
                            //return;
                        }
                        else
                        {


                         
                            //excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer, 1, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer , 1, ExcelCellStyle.LeftBorderedStandard);                            
                            totaldealer();
                            totalDealer = 1;
                            rowIndex++;
                            totalArea();
                            rowIndex++;
                            totalOutlet= totalDealer = 1;
                            //excelReport.CloseE8xcelFileWriter();
                            //return;

                            excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                            //excelReport.RestorePoint(8 + rowIndex, "RestoreOutlet");

                            dealer = row["CompanyName"].ToString();
                            //outlet = parameter[11].ToString() == "1" ? row["BranchName"].ToString() : "";
                            area = row["Area"].ToString();

                            //excelReport.CloseExcelFileWriter();
                            //return;
                        }
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.LeftBorderedStandard);



                        #endregion
                    }


                }

                CreateDetailStandard(excelReport, row, rowIndex, totalDetailWidth);
                rowIndex++;
            }

            //excelReport.CloseExcelFileWriter();
            //return;
            if (parameter[11].ToString() == "1")
            {
                //Column,mergedown-1,mergeaccros-1,
                excelReport.ReplaceRestorePoint("RestoreOutlet", outlet, dealerWidth, totalOutlet, 1, ExcelCellStyle.LeftBorderedStandard);
                totalperoutlet();
                rowIndex++;
                excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer + 2, 1, ExcelCellStyle.LeftBorderedStandard);
            }
            else
            {
                excelReport.ReplaceRestorePoint("RestoreDealer", dealer, 0, totalDealer , 1, ExcelCellStyle.LeftBorderedStandard);
                //rowIndex++;
            }
            
            totaldealer();
            rowIndex++;
            totalArea();

            excelReport.CloseExcelFileWriter();
        }


        private void CreateDetailStandard(ExcelFileWriter excelReport, DataRow row, int rowIndex, int totalDetailWidth)
        {
            if (parameter[12].ToString() != "1")
            {
                totalMonth = (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
        && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 :
        (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) == currentYear
        && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) :
        (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
        && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) == currentYear) ? Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) :
        Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) + 1;
            }

            decimal jan = Convert.ToDecimal(row["Jan"].ToString());
            decimal feb = Convert.ToDecimal(row["Feb"].ToString());
            decimal mar = Convert.ToDecimal(row["Mar"].ToString());
            decimal apr = Convert.ToDecimal(row["Apr"].ToString());
            decimal may = Convert.ToDecimal(row["May"].ToString());
            decimal jun = Convert.ToDecimal(row["Jun"].ToString());
            decimal jul = Convert.ToDecimal(row["Jul"].ToString());
            decimal aug = Convert.ToDecimal(row["Aug"].ToString());
            decimal sep = Convert.ToDecimal(row["Sep"].ToString());
            decimal oct = Convert.ToDecimal(row["Oct"].ToString());
            decimal nov = Convert.ToDecimal(row["Nov"].ToString());
            decimal dec = Convert.ToDecimal(row["Dec"].ToString());
            decimal avg = Convert.ToDecimal(row["AVG"].ToString());

            //if (parameter[11].ToString() == "1" && parameter[12].ToString() == "1" && row["Seq"].ToString() == "2")
            //    avg = Convert.ToDecimal(row["Total"].ToString()) / averageMonth;
            //else if (parameter[12].ToString() == "1" && row["Seq"].ToString() == "1")
            //    avg = Convert.ToDecimal(row["Total"].ToString()) / averageArea;
            //else
            //    avg = Convert.ToDecimal(row["Total"].ToString()) / totalMonth;

            excelReport.SetCellValue(Convert.ToInt32(jan).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(avg) && jan != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(feb).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(avg) && feb != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(mar).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(avg) && mar != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(apr).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(avg) && apr != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(may).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(avg) && may != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(jun).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(avg) && jun != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(jul).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(avg) && jul != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(aug).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(avg) && aug != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(sep).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(avg) && sep != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(oct).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(avg) && oct != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(nov).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(avg) && nov != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(dec).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(avg) && dec != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(Convert.ToDecimal(row["Total"].ToString())).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(Convert.ToInt32(Convert.ToDecimal(row["AVG"].ToString())).ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);


            janType += Convert.ToDecimal(row["Jan"].ToString());
            febType += Convert.ToDecimal(row["Feb"].ToString());
            marType += Convert.ToDecimal(row["Mar"].ToString());
            aprType += Convert.ToDecimal(row["Apr"].ToString());
            mayType += Convert.ToDecimal(row["May"].ToString());
            junType += Convert.ToDecimal(row["Jun"].ToString());
            julType += Convert.ToDecimal(row["Jul"].ToString());
            augType += Convert.ToDecimal(row["Aug"].ToString());
            sepType += Convert.ToDecimal(row["Sep"].ToString());
            octType += Convert.ToDecimal(row["Oct"].ToString());
            novType += Convert.ToDecimal(row["Nov"].ToString());
            decType += Convert.ToDecimal(row["Dec"].ToString());
            totalType = janType + febType + marType + aprType + mayType + junType + julType + augType + sepType + octType + novType + decType;

            janDealer += Convert.ToDecimal(row["Jan"].ToString());
            febDealer += Convert.ToDecimal(row["Feb"].ToString());
            marDealer += Convert.ToDecimal(row["Mar"].ToString());
            aprDealer += Convert.ToDecimal(row["Apr"].ToString());
            mayDealer += Convert.ToDecimal(row["May"].ToString());
            junDealer += Convert.ToDecimal(row["Jun"].ToString());
            julDealer += Convert.ToDecimal(row["Jul"].ToString());
            augDealer += Convert.ToDecimal(row["Aug"].ToString());
            sepDealer += Convert.ToDecimal(row["Sep"].ToString());
            octDealer += Convert.ToDecimal(row["Oct"].ToString());
            novDealer += Convert.ToDecimal(row["Nov"].ToString());
            decDealer += Convert.ToDecimal(row["Dec"].ToString());
            total_Dealer += totalType;


            janArea += Convert.ToDecimal(row["Jan"].ToString());
            febArea += Convert.ToDecimal(row["Feb"].ToString());
            marArea += Convert.ToDecimal(row["Mar"].ToString());
            aprArea += Convert.ToDecimal(row["Apr"].ToString());
            mayArea += Convert.ToDecimal(row["May"].ToString());
            junArea += Convert.ToDecimal(row["Jun"].ToString());
            julArea += Convert.ToDecimal(row["Jul"].ToString());
            augArea += Convert.ToDecimal(row["Aug"].ToString());
            sepArea += Convert.ToDecimal(row["Sep"].ToString());
            octArea += Convert.ToDecimal(row["Oct"].ToString());
            novArea += Convert.ToDecimal(row["Nov"].ToString());
            decArea += Convert.ToDecimal(row["Dec"].ToString());
            total_Area += totalType;          

        }

        private void CreateHeader(ExcelFileWriter excelReport, int rowIndex, int totalDetailWidth, string title, string year)
        {
            if (parameter[11] == "1") excelReport.SettingColumnWidth(1); else excelReport.SettingColumnWidth(2); //dealer
            if (parameter[11] == "1" && parameter[12] == "1") excelReport.SettingColumnWidth(2); else if (parameter[11] == "1" && parameter[12] == "0") excelReport.SettingColumnWidth(3);
            if (parameter[12] == "1" && parameter[14] == "") excelReport.SettingColumnWidth(3);
            if (parameter[13] == "1") excelReport.SettingColumnWidth(3);

            //if (parameter[9] == "1") excelReport.SettingColumnWidth(3);// Outlet


            excelReport.SetCellValue("SALES TREND REPORT", 0, 0, 1, dealerWidth + outletWidth + modelWidth + indentTitleDesc + indentTitleDesc + 2, ExcelCellStyle.Header, false, "20");
            excelReport.SetCellValue(title, 1, 0, 1, dealerWidth + outletWidth + modelWidth + indentTitleDesc + indentTitleDesc + 2, ExcelCellStyle.Header2, false);

            excelReport.SetCellValue("Periode ", 2, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[1].ToString(), 2, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Branch Manager", 2, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[2].ToString(), 2, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[3].ToString(), 3, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Head", 3, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[4].ToString(), 3, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[5].ToString(), 4, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Coordinator", 4, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[6].ToString(), 4, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[7].ToString(), 5, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Wiraniaga", 5, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[8].ToString(), 5, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            if (parameter[10].ToString() == "1")
                excelReport.SetCellValue("Dealer", 6, 0, 2, dealerWidth, ExcelCellStyle.CenterBorderedBold);

            if (parameter[11].ToString() == "1")
                excelReport.SetCellValue("Outlet", 6, 0 + dealerWidth, 2, outletWidth, ExcelCellStyle.CenterBorderedBold);

            if (parameter[12].ToString() == "1" && parameter[14].ToString() == "")
                excelReport.SetCellValue("Type", 6, 0 + dealerWidth + outletWidth, 2, modelWidth, ExcelCellStyle.CenterBorderedBold);

            if (parameter[13].ToString() == "1")
                excelReport.SetCellValue("Color", 6, 0 + dealerWidth + outletWidth + modelWidth, 2, colorWidth, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Bulan", 6, 0 + totalDetailWidth, 1, 12, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 6, 12 + totalDetailWidth, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("AVG", 6, 13 + totalDetailWidth, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jan", 7, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Feb", 7, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Mar", 7, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Apr", 7, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("May", 7, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jun", 7, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jul", 7, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Aug", 7, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sep", 7, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Oct", 7, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Nov", 7, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Dec", 7, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            totalRowDtl = 0;
            rowIndex = 0;
            //if (parameter[14] == "")
            //    excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
            //else
            //    if (parameter[11] == "1")
            //        excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());


        }

    }
    public enum RestorePoint { CompanyCodeRestorePoint, OutletRestorePoint, SalesHeadRestorePoint, SalesCoordinatorrestorePoint, SalesmanRestorePoint, MarketModelRestorePoint, GradeRestorePoint, TotalRestorePoint, AverageRestorePoint, ColorRestorePoint, JanRestorePoint, FebRestorePoint, MarRestorePoint, AprRestorePoint, MayRestorePoint, JunRestorePoint, JulRestorePoint, AugRestorePoint, SepRestorePoint, OctRestorePoint, NovRestorePoint, DecRestorePoint, TotalTableRestorePoint };
}