using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Drawing;
using System.IO;
using SimDms.DataWarehouse.Models;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    /// <summary>
    /// Use this class to boost print excel report
    /// --------------------------------------------
    /// Created by : Seandy A.K
    /// Created Date : 21-5-2012
    /// </summary>
    public class ExcelFileWriter
    {
        //[DllImport("user32.dll")]
        //public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        //[DllImport("user32.dll")]
        //public static extern bool SetForegroundWindow(IntPtr WindowHandle);

        public const int SW_RESTORE = 9;

        private int sheetCount = 1;
        private int row = 0;

        SysUserView user;
        string saveFile;

        bool wordWrap = false;
        object missing = System.Reflection.Missing.Value;

        private StringBuilder sb;
        private string pServer = "";

        /// <summary>
        /// Panduan penting!!!
        /// Dengan menggunakan class ini patut diperhatikan bahwa kita TIDAK DAPAT kembali ke cell atau row atau column SEBELUMNYA..
        /// Urutan tiap cell-nya harus berurutan dari A, B, C, ..., dst
        /// Urutan tiap row-nya selalu berurutan dari 1, 2, 3, ..., dst
        /// </summary>
        /// <param name="saveFileName">Nama yang ingin digunakan untus save report yang sudah selesai dibuat</param>
        /// <param name="sheetName">Nama sheet pertama</param>
        /// <param name="reportID">Tidak dipergunakan untuk sekarang</param>
        public ExcelFileWriter(string saveFileName, string sheetName, string reportID,string pserver)
        {
            saveFile = saveFileName;
            sb = new StringBuilder();
            CreateHeader();
            AddCellStyle();
            CreateNewWorksheet(sheetName);
            pServer = pserver;
        }

        /// <summary>
        /// Method ini digunakan Apabila header dan body berbeda prosesnya...
        /// Disiapkan pertama kali untuk Background Worker dimana Header dibuat terlebih dahulu dengan Method ExcelFileWriter(string saveFileName, string sheetName, string reportID)
        /// yang kemudian untuk body digunakan method ini
        /// </summary>
        /// <param name="startRowIndex">Row pertama yang ingin dimasukkan</param>
        public ExcelFileWriter(int startRowIndex)
        {
            sb = new StringBuilder();
            sb.AppendLine("<Row>");

            row = startRowIndex;
        }

        /// <summary>
        /// Digunakan untuk memasukkan 1 line baris coding pada line yang sedang berjalan
        /// </summary>
        /// <param name="sbInsert">StringBuilder baru yang ingin dimasukkan ke dalam string awal</param>
        public void InsertString(StringBuilder sbInsert)
        {
            InsertString(sbInsert.ToString());
        }

        /// <summary>
        /// Digunakan untuk memasukkan 1 line baris coding pada line yang sedang berjalan
        /// </summary>
        /// <param name="sbInsert">String baru yang ingin dimasukkan secara langsung pada string awal yang sedang berjalan</param>
        public void InsertString(string sbInsert)
        {
            sb.AppendLine(sbInsert.ToString());
        }

        public bool CloseExcelFileWriter()
        {
            return CloseExcelFileWriter(saveFile, sb);
        }

        /// <summary>
        /// Untuk men-save hasil build Report secara manual
        /// </summary>
        /// <param name="sbSaveXml">String yang ingin di simpan kedalam file</param>
        /// <returns></returns>
        public bool CloseExcelFileWriter(string saveFileName, StringBuilder sbSaveXml)
        {
            try
            {                
                CreateEndWorkSheet();
                File.WriteAllText( pServer +saveFileName,
                    sbSaveXml.ToString());
                //ProcessStartInfo psi = new ProcessStartInfo(saveFile.ToString());
                //Process[] objProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                //if (objProcess.Length != 0)
                //{
                //    IntPtr hWnd = IntPtr.Zero;
                //    hWnd = objProcess[0].MainWindowHandle;
                //    //ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                //    //SetForegroundWindow(objProcess[0].MainWindowHandle);
                //}
                //Process.Start(psi);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Use For Draw Excel Value for each columns
        /// </summary>
        /// <param name="value">The value of each columns that want to be insert</param>
        /// <param name="idxRow">Start Row</param>
        /// <param name="idxCol">Start Col</param>
        /// <param name="nRows">How much row will it take</param>
        /// <param name="nCols">How much column will it take</param>
        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, ExcelCellStyle.Default, false);
        }

        /// <summary>
        /// Use For Draw Excel Value for each columns
        /// </summary>
        /// <param name="value">The value of each columns that want to be insert</param>
        /// <param name="idxRow">Start Row</param>
        /// <param name="idxCol">Start Col</param>
        /// <param name="nRows">How much row will it take</param>
        /// <param name="nCols">How much column will it take</param>
        /// <param name="style">Style that you want to include from ExcelCellStyle list</param>
        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols, ExcelCellStyle style)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, style, false);
        }

        /// <summary>
        /// Use For Draw Excel Value for each columns
        /// </summary>
        /// <param name="value">The value of each columns that want to be insert</param>
        /// <param name="idxRow">Start Row</param>
        /// <param name="idxCol">Start Col</param>
        /// <param name="nRows">How much row will it take</param>
        /// <param name="nCols">How much column will it take</param>
        /// <param name="style">Style that you want to include from ExcelCellStyle list</param>
        /// <param name="number">Set Columns as Numeric(default 0 value as - character)</param>
        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols, ExcelCellStyle style, bool number)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, style, number, "14.25");
        }

        /// <summary>
        /// Use For Draw Excel Value for each columns
        /// </summary>
        /// <param name="value">The value of each columns that want to be insert</param>
        /// <param name="idxRow">Start Row</param>
        /// <param name="idxCol">Start Col</param>
        /// <param name="nRows">How much row will it take</param>
        /// <param name="nCols">How much column will it take</param>
        /// <param name="style">Style that you want to include from ExcelCellStyle list</param>
        /// <param name="number">Set Columns as Numeric(default 0 value as - character)</param>
        /// <param name="heightRow">Set row height</param>
        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols, ExcelCellStyle style, bool number, string heightRow)
        {
            int currentCol = idxCol + 1;
            if (idxRow == 0)
            {
                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"" + heightRow + "\">");
                sb.Append("         <Cell ");
                sb.Append("ss:Index=\"" + currentCol.ToString() + "\" ");
                sb.Append(nCols == 1 ? "" : "ss:MergeAcross=\"" + Convert.ToString((nCols - 1)) + "\" ");
                sb.Append(nRows == 1 ? "" : "ss:MergeDown=\"" + Convert.ToString((nRows - 1)) + "\" ");
                sb.Append("ss:StyleID=\"" + style.ToString() + "\">");
                sb.Append("<Data ss:Type=\"");
                sb.Append(number ? "Number" : "String");
                sb.Append("\">");
                sb.Append(value);
                sb.AppendLine("</Data></Cell>");

                row = idxRow;
            }
            else if (row == idxRow)
            {
                //Untuk meng-handle data dengan baris yang sama
                sb.Append("         <Cell ");
                sb.Append("ss:Index=\"" + currentCol.ToString() + "\" ");
                sb.Append(nCols == 1 ? "" : "ss:MergeAcross=\"" + Convert.ToString((nCols - 1)) + "\" ");
                sb.Append(nRows == 1 ? "" : "ss:MergeDown=\"" + Convert.ToString((nRows - 1)) + "\" ");
                sb.Append("ss:StyleID=\"" + style.ToString() + "\">");
                sb.Append("<Data ss:Type=\"");
                sb.Append(number ? "Number" : "String");
                sb.Append("\">");
                sb.Append(value);
                sb.AppendLine("</Data></Cell>");
                row = idxRow;
            }
            else if (row == (idxRow - 1))
            {
                //Untuk meng-handle data dengan baris selanjutnya
                sb.AppendLine("     </Row>");

                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");
                sb.Append("         <Cell ");
                sb.Append("ss:Index=\"" + currentCol.ToString() + "\" ");
                sb.Append(nCols == 1 ? "" : "ss:MergeAcross=\"" + Convert.ToString((nCols - 1)) + "\" ");
                sb.Append(nRows == 1 ? "" : "ss:MergeDown=\"" + Convert.ToString((nRows - 1)) + "\" ");
                sb.Append("ss:StyleID=\"" + style.ToString() + "\">");
                sb.Append("<Data ss:Type=\"");
                sb.Append(number ? "Number" : "String");
                sb.Append("\">");
                sb.Append(value);
                sb.AppendLine("</Data></Cell>");
                row = idxRow;
            }
            else if (row < (idxRow - 1))
            {
                sb.AppendLine("     </Row>");

                //Untuk mengisi row kosong sebelum bertemu dengan data baris data yang baru
                for (int i = 0; i < idxRow - (row + 1); i++)
                {
                    sb.AppendLine("   <Row></Row>");
                }

                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");

                sb.Append("         <Cell ");
                sb.Append("ss:Index=\"" + currentCol.ToString() + "\" ");
                sb.Append(nCols == 1 ? "" : "ss:MergeAcross=\"" + Convert.ToString((nCols - 1)) + "\" ");
                sb.Append(nRows == 1 ? "" : "ss:MergeDown=\"" + Convert.ToString((nRows - 1)) + "\" ");
                sb.Append("ss:StyleID=\"" + style.ToString() + "\">");
                sb.Append("<Data ss:Type=\"");
                sb.Append(number ? "Number" : "String");
                sb.Append("\">");
                sb.Append(value);
                sb.AppendLine("</Data></Cell>");
                row = idxRow;
            }
            else
            {
                sb.AppendLine("   </Row>");

                //Untuk mengisi row kosong sebelum bertemu dengan data baris data yang baru
                for (int i = 0; i < idxRow - (row + 1); i++)
                {
                    sb.AppendLine("   <Row></Row>");
                }
                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");

                row = idxRow + 1;
            }
        }

        /// <summary>
        /// Set this method first if you want to continue with next row. You can insert many row into this restore point with ReplaceRestorePoint Method
        /// Should remember!! If you have continued with next row, without insert this method before, you CANNOT back to this line again at the future... (^o^)V
        /// </summary>
        /// <param name="idxRow"></param>
        /// <param name="keyID"></param>
        public void RestorePoint(int idxRow, string keyID)
        {
            if (row == (idxRow - 1))
            {
                //Untuk meng-handle data dengan baris selanjutnya
                sb.AppendLine("     </Row>");
                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");
                sb.AppendLine(keyID);
                row = idxRow;
            }
            else if (row < (idxRow - 1))
            {
                sb.AppendLine("     </Row>");
                for (int i = 0; i < idxRow - (row + 1); i++)
                {
                    sb.AppendLine("   <Row></Row>");
                }
                sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");
                sb.AppendLine(keyID);
                row = idxRow;
            }
            else if (row == idxRow)
            {
                sb.AppendLine(keyID);
            }
        }

        /// <summary>
        /// Untuk menghapus restore point yang sudah dibuat pertama kali dengan method RestorePoint
        /// </summary>
        /// <param name="keyID">Nama Restore Point yang ingin di hapus</param>
        public void RemoveRestorePoint(string keyID)
        {
            sb.Replace(keyID, "");
        }

        public void CreateBlankRow(int idxRow)
        {
            sb.AppendLine("     </Row>");
            sb.AppendLine("     <Row></Row>");
            sb.AppendLine("     <Row ss:AutoFitHeight=\"0\" ss:Height=\"14.25\">");
            row = idxRow;
        }

        public void ChangeSheet(string sheetName)
        {
            sb.AppendLine("     </Row>");
            sb.AppendLine("  </Table>");
            sb.AppendLine("  <WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">");
            sb.AppendLine("   <ProtectObjects>False</ProtectObjects>");
            sb.AppendLine("   <ProtectScenarios>False</ProtectScenarios>");
            sb.AppendLine("  </WorksheetOptions>");
            sb.AppendLine(" </Worksheet>");
            CreateNewWorksheet(sheetName);
        }

        /// <summary>
        /// Method ini digunakan untuk mengganti restore point yang telah kita setting terlebih dahulu, dengan settingan cell yang ingin kita masukkan
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="value"></param>
        /// <param name="idxCol"></param>
        /// <param name="nRows"></param>
        /// <param name="nCols"></param>
        /// <param name="style"></param>
        public void ReplaceRestorePoint(string keyId, string value, int idxCol, int nRows, int nCols, ExcelCellStyle style)
        {
            ReplaceRestorePoint(keyId, value, idxCol, nRows, nCols, style, false);
        }

        public void ReplaceRestorePoint(string keyId, string value, int idxCol, int nRows, int nCols, ExcelCellStyle style, bool number)
        {
            int currentCol = idxCol + 1;
            StringBuilder sbReplace = new StringBuilder();
            //Untuk meng-handle data dengan baris yang sama
            sbReplace.Append("         <Cell ");
            sbReplace.Append("ss:Index=\"" + currentCol.ToString() + "\" ");
            sbReplace.Append(nCols == 1 ? "" : "ss:MergeAcross=\"" + Convert.ToString((nCols - 1)) + "\" ");
            sbReplace.Append(nRows == 1 ? "" : "ss:MergeDown=\"" + Convert.ToString((nRows - 1)) + "\" ");
            sbReplace.Append("ss:StyleID=\"" + style.ToString() + "\">");
            sbReplace.Append("<Data ss:Type=\"");
            sbReplace.Append(number ? "Number" : "String");
            sbReplace.Append("\">");
            sbReplace.Append(value);
            sbReplace.Append("</Data></Cell>");
            sb.Replace(keyId, sbReplace.ToString());
        }

        public void ReplaceRestorePoint(string keyId, StringBuilder sbData)
        {
            sb.Replace(keyId, sbData.ToString());
        }

        /// <summary>
        /// Fungsi method ini untuk mengambil data yang sudah dibuat untuk disimpan terlebih dahulu.
        /// Mengapa disimpan? karena kita ingin mengisi row yang sudah terlewati sebelumnya dengan beberapa baris baru
        /// (dengan catatan : Sudah tersimpan RestorePoint sebelumnya pada baris yang ingin di ganti)
        /// Data ini akan digunakan untuk method LoadAndReplaceCurrentState...
        /// PENTING!!!: Jangan lupa setelah melakukan Save Current State kita memanggil kembali dengan menggunakan Load And Replace Current State
        /// </summary>
        /// <returns></returns>
        public StringBuilder SaveCurrentState()
        {
            StringBuilder sbReturn = sb;
            sb = new StringBuilder();

            return sbReturn;
        }

        /// <summary>
        /// Alur method ini :
        /// 1.User memassukkan Restore point mana yang ingin dirubah
        /// 2.User memasukkan Save point yang sudah tersimpan terlebih dahulu
        /// 3.Method kemudian akan me-Replace restore point pada save point yang sudah dimasukkan dengan data yang baru.
        /// (nb: patut diketahui, data yang baru yang dimaksud adalah data yang dimasukkan setelah proses Save State!
        ///  Sehingga lakukan Save State pada saat ingin mengisi RestorePoint tersebut.)
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="sbSaveState"></param>
        public void LoadAndReplaceCurrentState(string keyId, StringBuilder sbSaveState)
        {
            sbSaveState.Replace(keyId, sb.ToString());
            sb = sbSaveState;
        }

        /// <summary>
        /// Setelah inisialisasi class,HARUS setting method ini terlebih dahulu untuk menentukan ukuran kolom.
        /// Untuk ukuran kolom yang tidak perlu di tentukan,tidak perlu di isi.
        /// </summary>
        /// <param name="nCol"></param>
        public void SettingColumnWidth(int nCol)
        {
            double columnWidth = nCol * 50;
            sb.AppendLine("     <Column ss:AutoFitWidth=\"0\" ss:Width=\"" + columnWidth.ToString() + "\"/>");
        }

        #region Excel Properties
        /// <summary>
        /// Be carefull!!!
        /// After you change the sheet, you CAN NOT back to previous Sheet.
        /// </summary>
        /// <param name="SheetName">Your Sheet Name</param>
        private void CreateNewWorksheet(string sheetName)
        {
            sb.AppendLine("");
            sb.AppendLine(" <Worksheet ss:Name=\"" + sheetName + "\">");
            sb.AppendLine("  <Table>");
        }

        /// <summary>
        /// To Finalize Excel report
        /// Automatic Open Excel Application
        /// </summary>
        private void CreateEndWorkSheet()
        {
            sb.AppendLine("     </Row>");
            sb.AppendLine("  </Table>");
            sb.AppendLine("  <WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">");
            sb.AppendLine("   <ProtectObjects>False</ProtectObjects>");
            sb.AppendLine("   <ProtectScenarios>False</ProtectScenarios>");
            sb.AppendLine("  </WorksheetOptions>");
            sb.AppendLine(" </Worksheet>");
            sb.AppendLine("</Workbook>");
        }

        private void CreateHeader()
        {
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine("     xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            sb.AppendLine("     xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            sb.AppendLine("     xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            sb.AppendLine("     xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            sb.AppendLine("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
            sb.AppendLine("     <Author>SIM</Author>");
            sb.AppendLine("     <LastAuthor>SIM</LastAuthor>");
            sb.AppendLine("     <Created>" + DateTime.Now + "</Created>");
            sb.AppendLine("     <Company>PT. SIM</Company>");
            sb.AppendLine("     <Version>10.6626</Version>");
            sb.AppendLine("</DocumentProperties>");
            sb.AppendLine("<OfficeDocumentSettings xmlns='urn:schemas-microsoft-com:office:office'>");
            sb.AppendLine("     <DownloadComponents/>");
            sb.AppendLine("     <LocationOfComponents HRef='file:///\\'/>");
            sb.AppendLine("</OfficeDocumentSettings>");
            sb.AppendLine("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
            sb.AppendLine("     <WindowHeight>12525</WindowHeight>");
            sb.AppendLine("     <WindowWidth>19035</WindowWidth>");
            sb.AppendLine("     <WindowTopX>0</WindowTopX>");
            sb.AppendLine("     <WindowTopY>120</WindowTopY>");
            sb.AppendLine("     <ProtectStructure>False</ProtectStructure>");
            sb.AppendLine("     <ProtectWindows>False</ProtectWindows>");
            sb.AppendLine("</ExcelWorkbook>");
        }

        private void AddCellStyle()
        {
            sb.AppendLine("<Styles>");
            sb.AppendLine("  <Style ss:ID=\"Default\" ss:Name=\"Normal\">");
            sb.AppendLine("   <Alignment ss:Vertical=\"Bottom\"/>");
            sb.AppendLine("   <Borders/>");
            sb.AppendLine("   <Font/>");
            sb.AppendLine("   <Interior/>");
            sb.AppendLine("   <NumberFormat/>");
            sb.AppendLine("   <Protection/>");
            sb.AppendLine("  </Style>");
            sb.AppendLine("  <Style ss:ID=\"Header\">");
            sb.AppendLine("   <Font ss:Size=\"14\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            sb.AppendLine("  <Style ss:ID=\"Standard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\" />");
            sb.AppendLine("   <Font ss:Size=\"9\"/>");
            sb.AppendLine("  </Style>");

            #region Header2
            sb.AppendLine("  <Style ss:ID=\"Header2\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\" />");
            sb.AppendLine("   <Font ss:Size=\"12\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Bold
            sb.AppendLine("  <Style ss:ID=\"LeftBold\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\" />");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Bordered Bold
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedBold\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\" />");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Bordered Top
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedTop\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
            #endregion

            #region Center Bordered Bold
            sb.AppendLine("  <Style ss:ID=\"CenterBorderedBold\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Bold
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBold\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Bold Red
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBoldRed\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Bold Number
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBoldNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Bold Decimal
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBoldDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Bold Red Number
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBoldRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Standard
            sb.AppendLine("  <Style ss:ID=\"RightBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Standard Number
            sb.AppendLine("  <Style ss:ID=\"RightBorderedStandardNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Standard Decimal
            sb.AppendLine("  <Style ss:ID=\"RightBorderedStandardDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Right Bordered Standard Red Number
            sb.AppendLine("  <Style ss:ID=\"RightBorderedStandardRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Border Standard
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Border Standard Wrap
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedStandardWrap\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
            #endregion

            #region Left Border Standard Date
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedStandardDate\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("   <NumberFormat ss:Format=\"[ENG][$-409]d\\-mmm\\-yyyy;@\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Pink Total
            sb.AppendLine("  <Style ss:ID=\"PinkTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FF99CC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Pink Total Number
            sb.AppendLine("  <Style ss:ID=\"PinkTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FF99CC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Pink Total Decimal
            sb.AppendLine("  <Style ss:ID=\"PinkTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FF99CC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Pink Total Red Number
            sb.AppendLine("  <Style ss:ID=\"PinkTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FF99CC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Brown Total
            sb.AppendLine("  <Style ss:ID=\"BrownTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E1B89F\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Brown Total Number
            sb.AppendLine("  <Style ss:ID=\"BrownTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E1B89F\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Brown Total Decimal
            sb.AppendLine("  <Style ss:ID=\"BrownTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E1B89F\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Brown Total Red Number
            sb.AppendLine("  <Style ss:ID=\"BrownTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E1B89F\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Brown Total
            sb.AppendLine("  <Style ss:ID=\"LightBrownTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#DDD9C3\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Brown Total Number
            sb.AppendLine("  <Style ss:ID=\"LightBrownTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#DDD9C3\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Brown Total Decimal
            sb.AppendLine("  <Style ss:ID=\"LightBrownTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#DDD9C3\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Brown Total Red Number
            sb.AppendLine("  <Style ss:ID=\"LightBrownTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#DDD9C3\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Blue Total
            sb.AppendLine("  <Style ss:ID=\"BlueTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Blue Total Number
            sb.AppendLine("  <Style ss:ID=\"BlueTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Blue Total Decimal
            sb.AppendLine("  <Style ss:ID=\"BlueTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Blue Total Red Number
            sb.AppendLine("  <Style ss:ID=\"BlueTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Yellow Total
            sb.AppendLine("  <Style ss:ID=\"YellowTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FFFF99\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Yellow Total Number
            sb.AppendLine("  <Style ss:ID=\"YellowTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FFFF99\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Yellow Total Decimal
            sb.AppendLine("  <Style ss:ID=\"YellowTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FFFF99\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Yellow Total Red Number
            sb.AppendLine("  <Style ss:ID=\"YellowTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FFFF99\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Purple Total
            sb.AppendLine("  <Style ss:ID=\"PurpleTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E6CDFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Purple Total Number
            sb.AppendLine("  <Style ss:ID=\"PurpleTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E6CDFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Purple Total Decimal
            sb.AppendLine("  <Style ss:ID=\"PurpleTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E6CDFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Purple Total Red Number
            sb.AppendLine("  <Style ss:ID=\"PurpleTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E6CDFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Gray Total
            sb.AppendLine("  <Style ss:ID=\"GrayTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#BFBFBF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Gray Total Number
            sb.AppendLine("  <Style ss:ID=\"GrayTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#BFBFBF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Gray Total Decimal
            sb.AppendLine("  <Style ss:ID=\"GrayTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#BFBFBF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Gray Total Red Number
            sb.AppendLine("  <Style ss:ID=\"GrayTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#BFBFBF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Gray Total
            sb.AppendLine("  <Style ss:ID=\"LightGrayTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#C5BE97\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Gray Total Number
            sb.AppendLine("  <Style ss:ID=\"LightGrayTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#C5BE97\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Gray Total Decimal
            sb.AppendLine("  <Style ss:ID=\"LightGrayTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#C5BE97\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Light Gray Total Red Number
            sb.AppendLine("  <Style ss:ID=\"LightGrayTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#C5BE97\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Green Total
            sb.AppendLine("  <Style ss:ID=\"GreenTotal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFCC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Green Total Number
            sb.AppendLine("  <Style ss:ID=\"GreenTotalNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFCC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Green Total Decimal
            sb.AppendLine("  <Style ss:ID=\"GreenTotalDecimal\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFCC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            #endregion

            #region Green Total Red Number
            sb.AppendLine("  <Style ss:ID=\"GreenTotalRedNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFCC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\" ss:Color=\"#FF0000\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
            sb.AppendLine("  </Style>");
            sb.AppendLine("</Styles>");
            #endregion

        }
        #endregion
    }

    public enum ExcelCellStyle { Default, Header, Header2, Standard, CenterBorderedBold, RightBorderedStandard, RightBorderedStandardNumber, RightBorderedStandardDecimal, RightBorderedStandardRedNumber, RightBorderedBold, RightBorderedBoldRed, RightBorderedBoldNumber, RightBorderedBoldDecimal, RightBorderedBoldRedNumber, LeftBold, LeftBorderedBold, LeftBorderedTop, LeftBorderedStandardWrap, LeftBorderedStandardDate, LeftBorderedStandard, BorderedStandard, PinkTotal, PinkTotalNumber, PinkTotalRedNumber, PinkTotalDecimal, BrownTotal, BrownTotalNumber, BrownTotalDecimal, BrownTotalRedNumber, LightBrownTotal, LightBrownTotalNumber, LightBrownTotalDecimal, LightBrownTotalRedNumber, BlueTotal, BlueTotalNumber, BlueTotalDecimal, BlueTotalRedNumber, YellowTotal, YellowTotalNumber, YellowTotalDecimal, YellowTotalRedNumber, PurpleTotal, PurpleTotalNumber, PurpleTotalDecimal, PurpleTotalRedNumber, GreenTotal, GreenTotalNumber, GreenTotalDecimal, GreenTotalRedNumber, GrayTotal, GrayTotalNumber, GrayTotalDecimal, GrayTotalRedNumber, LightGrayTotal, LightGrayTotalNumber, LightGrayTotalDecimal, LightGrayTotalRedNumber };
    public enum RestorePointBackgroundWorker { StartofLine, EndofLine }

}