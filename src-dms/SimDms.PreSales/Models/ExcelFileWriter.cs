using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

namespace SimDms.PreSales.Models
{
    /// <summary>
    /// Use this class to boost print excel report
    /// --------------------------------------------
    /// Created by : Seandy A.K
    /// Created Date : 21-5-2012
    /// </summary>
    public class ExcelFileWriter
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);

        public const int SW_RESTORE = 9;

        private int sheetCount = 1;
        private int row = 0;
              
        string saveFile;

        bool wordWrap = false;
        object missing = System.Reflection.Missing.Value;

        private StringBuilder sb;
        private StringBuilder sbFreeze;
        private bool ApplyFreezeEveryPage = false;

        /// <summary>
        /// Panduan penting!!!
        /// Dengan menggunakan class ini patut diperhatikan bahwa kita TIDAK DAPAT kembali ke cell atau row atau column SEBELUMNYA..
        /// Urutan tiap cell-nya harus berurutan dari A, B, C, ..., dst
        /// Urutan tiap row-nya selalu berurutan dari 1, 2, 3, ..., dst
        /// </summary>
        /// <param name="saveFileName">Nama yang ingin digunakan untus save report yang sudah selesai dibuat</param>
        /// <param name="sheetName">Nama sheet pertama</param>
        /// <param name="reportID">Tidak dipergunakan untuk sekarang</param>
        public ExcelFileWriter(string saveFileName, string sheetName, string reportID)
        {
            saveFile = saveFileName;
            sb = new StringBuilder();
            CreateHeader();
            AddCellStyle();
            CreateNewWorksheet(sheetName);
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

        public string CloseExcelFileWriter()
        {
            return CloseExcelFileWriter(saveFile, sb);
        }

        /// <summary>
        /// Untuk men-save hasil build Report secara manual
        /// </summary>
        /// <param name="sbSaveXml">String yang ingin di simpan kedalam file</param>
        /// <returns></returns>
        public string CloseExcelFileWriter(string saveFileName, StringBuilder sbSaveXml)
        {
            try
            {
                CreateEndWorkSheet();
                return sbSaveXml.ToString();
            }
            catch
            {
                return "";
            }
        }

        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, ExcelCellStyle.Default, false);
        }

        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols, ExcelCellStyle style)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, style, false);
        }

        public void SetCellValue(string value, int idxRow, int idxCol, int nRows, int nCols, ExcelCellStyle style, bool number)
        {
            SetCellValue(value, idxRow, idxCol, nRows, nCols, style, number, "14.25");
        }

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
        /// Setting method ini ketika kita ingin melanjutkan penulisan excel terlebih dahulu dan akan mengisi baris ini terakhir.
        /// Masukkan baris ke berapa yang ingin dimasukkan..
        /// INGAT!!tidak bisa memasukkan baris yang sudah tercetak!Jadi gunakan restore point ini pada baris yang sedang berjalan.
        /// Jangan sebelum atau sesudahnya..Karena baris yang sudah di lewati tidak akan bisa di rubah di kemudian... (^o^)V
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
            if (sbFreeze != null)
                sb.AppendLine(sbFreeze.ToString());

            sb.AppendLine("   <ProtectObjects>False</ProtectObjects>");
            sb.AppendLine("   <ProtectScenarios>False</ProtectScenarios>");
            sb.AppendLine("  </WorksheetOptions>");
            sb.AppendLine(" </Worksheet>");
            if (!ApplyFreezeEveryPage)
                sbFreeze = null;

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
            double columnWidth = nCol * 60;
            sb.AppendLine("     <Column ss:AutoFitWidth=\"0\" ss:Width=\"" + columnWidth.ToString() + "\"/>");
        }

        public void SettingColumnWidthMini(int nCol)
        {
            double columnWidth = nCol * 25;
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

        private void CreateEndWorkSheet()
        {
            sb.AppendLine("     </Row>");
            sb.AppendLine("  </Table>");
            sb.AppendLine("  <WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">");
            if (sbFreeze != null)
                sb.AppendLine(sbFreeze.ToString());

            sb.AppendLine("   <ProtectObjects>False</ProtectObjects>");
            sb.AppendLine("   <ProtectScenarios>False</ProtectScenarios>");
            sb.AppendLine("  </WorksheetOptions>");
            sb.AppendLine(" </Worksheet>");
            sb.AppendLine("</Workbook>");

            if (!ApplyFreezeEveryPage)
                sbFreeze = null;
        }

        public void FreezeCols(int row, int cols)
        {
            FreezeCols(row, cols, false);
        }

        public void FreezeCols(int row, int cols, bool freezeEveryPage)
        {
            sbFreeze = new StringBuilder();
            sbFreeze.AppendLine("<PageSetup>");
            sbFreeze.AppendLine("<Header x:Margin=\"0.3\"/>");
            sbFreeze.AppendLine("<Footer x:Margin=\"0.3\"/>");
            sbFreeze.AppendLine("<PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/>");
            sbFreeze.AppendLine("</PageSetup>");
            sbFreeze.AppendLine("<Selected/>");
            sbFreeze.AppendLine("<FreezePanes/>");
            sbFreeze.AppendLine("<FrozenNoSplit/>");
            if (row != 0)
            {
                sbFreeze.AppendLine("<SplitHorizontal>" + row + "</SplitHorizontal>");
                sbFreeze.AppendLine("<TopRowBottomPane>" + row + "</TopRowBottomPane>");
            }
            if (cols != 0)
            {
                sbFreeze.AppendLine("<SplitVertical>" + cols + "</SplitVertical>");
                sbFreeze.AppendLine("<LeftColumnRightPane>" + cols + "</LeftColumnRightPane>");
            }
            if (row != 0 && cols == 0)
            {
                sbFreeze.AppendLine("<ActivePane>2</ActivePane>");
            }
            else if (row != 0 && cols != 0)
            {
                sbFreeze.AppendLine("<ActivePane>0</ActivePane>");
            }
            else if (row == 0 && cols != 0)
            {
                sbFreeze.AppendLine("<ActivePane>1</ActivePane>");
            }
            sbFreeze.AppendLine("<Panes>");
            sbFreeze.AppendLine(" <Pane>");
            sbFreeze.AppendLine("  <Number>3</Number>");
            sbFreeze.AppendLine(" </Pane>");
            sbFreeze.AppendLine(" <Pane>");
            sbFreeze.AppendLine("  <Number>1</Number>");
            sbFreeze.AppendLine(" </Pane>");
            if (row != 0 || cols != 0)
            {
                sbFreeze.AppendLine(" <Pane>");
                sbFreeze.AppendLine("  <Number>2</Number>");
                sbFreeze.AppendLine(" </Pane>");
                sbFreeze.AppendLine(" <Pane>");
                sbFreeze.AppendLine("  <Number>0</Number>");
                sbFreeze.AppendLine(" </Pane>");
            }
            sbFreeze.AppendLine("</Panes>");
            ApplyFreezeEveryPage = freezeEveryPage;
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

#region CenterHeader2
            sb.AppendLine("  <Style ss:ID=\"CenterHeader2\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\" />");
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

#region Center Bordered Bold Wrap
            sb.AppendLine("  <Style ss:ID=\"CenterBorderedBoldWrap\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
#endregion

#region Center Border Standard
            sb.AppendLine("  <Style ss:ID=\"CenterBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
#endregion

#region Center Border Standard Wrap
            sb.AppendLine("  <Style ss:ID=\"CenterBorderedStandardWrap\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
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

#region Right Bottom Bordered Bold Number
            sb.AppendLine("  <Style ss:ID=\"RightBottomBorderedBoldNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Bottom\"/>");
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

#region Right Center Bordered Standard
            sb.AppendLine("  <Style ss:ID=\"RightCenterBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/>");
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

#region Right Bottom Bordered Standard Number
            sb.AppendLine("  <Style ss:ID=\"RightBottomBorderedStandardNumber\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Bottom\"/>");
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

#region Right Bordered Standard Percent
            sb.AppendLine("  <Style ss:ID=\"RightBorderedStandardPercent\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
            sb.AppendLine("  </Style>");
#endregion

#region Right Bordered Bold Percent
            sb.AppendLine("  <Style ss:ID=\"RightBorderedBoldPercent\"> ss:Parent=\"Percentage\"");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
            sb.AppendLine("  </Style>");
#endregion

#region Right Bottom Bordered Bold Percent
            sb.AppendLine("  <Style ss:ID=\"RightBottomBorderedBoldPercent\"> ss:Parent=\"Percentage\"");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Bottom\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

#region Left Bottom Border Standard
            sb.AppendLine("  <Style ss:ID=\"LeftBottomBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Bottom\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
#endregion

#region Left Bottom Border Bold
            sb.AppendLine("  <Style ss:ID=\"LeftBottomBorderedBold\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Bottom\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("  </Style>");
#endregion

#region Left Center Border Standard
            sb.AppendLine("  <Style ss:ID=\"LeftCenterBorderedStandard\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" />");
            sb.AppendLine("  </Style>");
#endregion

#region Left Bordered Bold Wrap
            sb.AppendLine("  <Style ss:ID=\"LeftBorderedBoldWrap\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\" ss:WrapText=\"1\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
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

#region Center Yellow
            sb.AppendLine("  <Style ss:ID=\"CenterYellow\">");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Top\"/>");
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

#region Yellow Total Percent
            sb.AppendLine("  <Style ss:ID=\"YellowTotalPercent\"> ss:Parent=\"Percentage\"");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#FFFF99\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

#region Purple Total Percent
            sb.AppendLine("  <Style ss:ID=\"PurpleTotalPercent\"> ss:Parent=\"Percentage\"");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#E6CDFF\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

#region Light Purple Total Number
    sb.AppendLine("  <Style ss:ID=\"LightPurpleTotalNumber\">");
    sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
    sb.AppendLine("   <Borders>");
    sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("   </Borders>");
    sb.AppendLine("   <Interior ss:Color=\"#CCC0DA\" ss:Pattern=\"Solid\"/>");
    sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
    sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
    sb.AppendLine("  </Style>");
#endregion

#region Light Purple Total Percent
    sb.AppendLine("  <Style ss:ID=\"LightPurpleTotalPercent\"> ss:Parent=\"Percentage\"");
    sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
    sb.AppendLine("   <Borders>");
    sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
    sb.AppendLine("   </Borders>");
    sb.AppendLine("   <Interior ss:Color=\"#CCC0DA\" ss:Pattern=\"Solid\"/>");
    sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
    sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

#region Light Orange Total 
sb.AppendLine("  <Style ss:ID=\"LightOrangeTotal\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#FCD5B4\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Orange Total Decimal
sb.AppendLine("  <Style ss:ID=\"LightOrangeTotalDecimal\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#FCD5B4\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Orange Total Number
sb.AppendLine("  <Style ss:ID=\"LightOrangeTotalNumber\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#FCD5B4\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Orange Total Percent
sb.AppendLine("  <Style ss:ID=\"LightOrangeTotalPercent\"> ss:Parent=\"Percentage\"");
sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#FCD5B4\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Blue Total 
sb.AppendLine("  <Style ss:ID=\"LightBlueTotal\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#B6DDE8\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Blue Total Decimal
sb.AppendLine("  <Style ss:ID=\"LightBlueTotalDecimal\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#B6DDE8\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0.0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Blue Total Number
sb.AppendLine("  <Style ss:ID=\"LightBlueTotalNumber\">");
sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#B6DDE8\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"_(* #,##0_);_(* \\(#,##0\\);_(* &quot;-&quot;_);_(@_)\"/>");
sb.AppendLine("  </Style>");
#endregion

#region Light Blue Total Percent
sb.AppendLine("  <Style ss:ID=\"LightBlueTotalPercent\"> ss:Parent=\"Percentage\"");
sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
sb.AppendLine("   <Borders>");
sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
sb.AppendLine("   </Borders>");
sb.AppendLine("   <Interior ss:Color=\"#B6DDE8\" ss:Pattern=\"Solid\"/>");
sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

#region Green Total Percent
            sb.AppendLine("  <Style ss:ID=\"GreenTotalPercent\"> ss:Parent=\"Percentage\"");
            sb.AppendLine("   <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>");
            sb.AppendLine("   <Borders>");
            sb.AppendLine("    <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("    <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>");
            sb.AppendLine("   </Borders>");
            sb.AppendLine("   <Interior ss:Color=\"#CCFFCC\" ss:Pattern=\"Solid\"/>");
            sb.AppendLine("   <Font ss:Size=\"9\" ss:Bold=\"1\"/>");
            sb.AppendLine("   <NumberFormat ss:Format=\"Percent\"/>");
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

    public enum ExcelCellStyle { Default, Header, Header2, CenterHeader2, Standard, CenterBorderedBold, CenterBorderedBoldWrap, CenterBorderedStandard, CenterBorderedStandardWrap, RightBorderedStandard, RightCenterBorderedStandard, RightBottomBorderedStandardNumber, RightBorderedStandardNumber, RightBorderedStandardPercent, RightBottomBorderedBoldPercent, RightBorderedBoldPercent, RightBorderedStandardDecimal, RightBorderedStandardRedNumber, RightBorderedBold, RightBorderedBoldRed, RightBottomBorderedBoldNumber, RightBorderedBoldNumber, RightBorderedBoldDecimal, RightBorderedBoldRedNumber, LeftBold, LeftBorderedBold, LeftBorderedTop, LeftBottomBorderedBold, LeftBottomBorderedStandard, LeftCenterBorderedStandard, LeftBorderedStandard, LeftBorderedStandardWrap, LeftBorderedBoldWrap, BorderedStandard, PinkTotal, PinkTotalNumber, PinkTotalRedNumber, PinkTotalDecimal, BrownTotal, BrownTotalNumber, BrownTotalDecimal, BrownTotalRedNumber, BlueTotal, BlueTotalNumber, BlueTotalDecimal, BlueTotalRedNumber, CenterYellow, YellowTotal, YellowTotalNumber, YellowTotalDecimal, YellowTotalPercent, YellowTotalRedNumber, PurpleTotal, PurpleTotalNumber, PurpleTotalDecimal, PurpleTotalPercent, PurpleTotalRedNumber, GreenTotal, GreenTotalNumber, GreenTotalDecimal, GreenTotalPercent, GreenTotalRedNumber, GrayTotal, GrayTotalNumber, GrayTotalDecimal, GrayTotalRedNumber, LightGrayTotal, LightGrayTotalNumber, LightGrayTotalDecimal, LightGrayTotalRedNumber, LightBrownTotal, LightOrangeTotalDecimal, LightOrangeTotalNumber, LightOrangeTotal, LightOrangeTotalPercent, LightBlueTotalDecimal, LightBlueTotalNumber, LightBlueTotal, LightBlueTotalPercent, LightPurpleTotalPercent, LightPurpleTotalNumber };
    public enum RestorePointBackgroundWorker { StartofLine, EndofLine }
}
