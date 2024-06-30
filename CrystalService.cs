
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Oracle.DataAccess.Client;
using word = Microsoft.Office.Interop.Word;


namespace Patholab_Common
{
    public class CrystalReport
    {
        OracleCommand cmd;
        ReportDocument CR;
        string workStationName;
        string reportPath;
        string server;
        string user;
        string pass;
        

        
        public CrystalReport(string Server, string User, string Pass, string ReportPath)
        {
            
            reportPath = ReportPath;
            server = Server;
            user = User;
            pass = Pass;
            var splited = reportPath.Split('\\');
        }

        public CrystalReport(string Server, string User, string Pass, string ReportPath, OracleCommand cmd)
        {
            reportPath = ReportPath;
            server = Server;
            user = User;
            pass = Pass;
            var splited = reportPath.Split('\\');
            this.cmd = cmd;
            this.workStationName = workStationName;
        }

        public void Load()
        {
            if (File.Exists(reportPath))
            {
                //load
                CR = new ReportDocument();
                CR.Load(reportPath);
            }
        }

        public void SetReportParameterValue(string name, object value)
        {
            //set report parameter value
            CR.SetParameterValue(name, value);
        }

        public void Login()
        {
            Tables crTables;
            var crTableLoginInfo = new TableLogOnInfo();
            var crConnectionInfo = new ConnectionInfo();
            crConnectionInfo.ServerName = server;
            crConnectionInfo.UserID = user;
            crConnectionInfo.Password = pass;
            crTables = CR.Database.Tables;
            foreach (Table crTable in crTables)
            {
                crTableLoginInfo = crTable.LogOnInfo;
                crTableLoginInfo.ConnectionInfo = crConnectionInfo;
                crTable.ApplyLogOnInfo(crTableLoginInfo);
            }
        }

        public void exportCrystalToWordRTFAndSave(string wordRTFPath)
        {
            //export
            ExportOptions crExportOptions;
            var crDiskFileDestinationOption = new DiskFileDestinationOptions();
            var crFormattypeOptions = new PdfRtfWordFormatOptions();
            crDiskFileDestinationOption.DiskFileName = wordRTFPath;
            crExportOptions = CR.ExportOptions;
            {
                crExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                crExportOptions.ExportFormatType = ExportFormatType.WordForWindows;
                crExportOptions.DestinationOptions = crDiskFileDestinationOption;
                crExportOptions.FormatOptions = crFormattypeOptions;
            }
            CR.Export();
        }

        public void PrintWithSettingsFromIni(string workStationId)
        {

            //הפונקציה לא בשימוש
            try
            {
                //string iniPath = @"C:\Temp\PrinterSettings.ini"; //For test
                string iniPath = @"\\lims-srv\limsPrograms\General\PrinterSettings.ini";
                var ini = new IniFile(iniPath);
                string sectionName = "workstation" + workStationId;
                var printerName = ini.GetString(sectionName, "PrinterName", "");
                var startPageN = int.Parse(ini.GetString(sectionName, "StartPageN", ""));
                var EndPageN = int.Parse(ini.GetString(sectionName, "EndPageN", ""));
                var mT = int.Parse(ini.GetString(sectionName, "PageMarginT", ""));
                var mR = int.Parse(ini.GetString(sectionName, "PageMarginR", ""));
                var mB = int.Parse(ini.GetString(sectionName, "PageMarginB", ""));
                var mL = int.Parse(ini.GetString(sectionName, "PageMarginL", ""));

                var pm = CR.PrintOptions.PageMargins;
                pm.bottomMargin = mB;
                pm.leftMargin = mL;
                pm.rightMargin = mR;
                pm.topMargin = mT;
                CR.PrintOptions.ApplyPageMargins(pm);
                CR.PrintOptions.PrinterName = printerName;

                CR.PrintToPrinter(1, true, startPageN, EndPageN);
            }
            catch (Exception ex)
            {

                MessageBox.Show("נכשלה הדפסת מדבקה." + "\n" + ex.Message, "Nautlus", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
        }

        public void PrintWithSettingsFromIniNew(string workStationId)
        {
            try
            {
                //string iniPath = @"C:\Temp\PrinterSettings.ini"; //For test
                // string iniPath = @"\\Lims-srv\LimsPrograms\General\PrinterSettings.ini";
                string iniPath = GetPathFromPhrase("Lims System Paths", "General");
                iniPath += "PrinterSettings.ini";
                var ini = new IniFile(iniPath);
                string sectionName = "workstation" + workStationId;
                var printerName = ini.GetString(sectionName, "PrinterName", "");
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show(".INI נכשלה הדפסת מדבקה בדוק הגדרות קובץ  ");
                    return;
                }
                var startPageN = int.Parse(ini.GetString(sectionName, "StartPageN", ""));
                var EndPageN = int.Parse(ini.GetString(sectionName, "EndPageN", ""));
                var mT = int.Parse(ini.GetString(sectionName, "PageMarginT", ""));
                var mR = int.Parse(ini.GetString(sectionName, "PageMarginR", ""));
                var mB = int.Parse(ini.GetString(sectionName, "PageMarginB", ""));
                var mL = int.Parse(ini.GetString(sectionName, "PageMarginL", ""));
                var copies = short.Parse(ini.GetString(sectionName, "Copies", ""));


                PrinterSettings ps = new PrinterSettings();
                ps.Copies = copies;
                ps.PrinterName = printerName;


                PageSettings pst = new PageSettings();
                pst.PrinterSettings.FromPage = startPageN;
                pst.PrinterSettings.ToPage = EndPageN;
                pst.Margins.Bottom = mB;
                pst.Margins.Top = mT;
                pst.Margins.Right = mR;
                pst.Margins.Left = mL;

                CR.PrintToPrinter(ps, pst, true);
            }
            catch (Exception ex)
            {
                Logger.WriteLogFile(ex);
                MessageBox.Show("נכשלה הדפסת מדבקה." + "\n" + ex.Message, "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
        }

        public void PrintDefault()
        {

            CR.PrintToPrinter(1, true, 0, 0);

        }

        public void close()
        {

            CR.Close();
            CR.Dispose();
        }

        public void exportWordRtfToPdf(string wordRTFPath, string pdfPath)
        {

            if (File.Exists(wordRTFPath))
            {
                //export from word to pdf
                var wa = new word.Application();
                var wd = wa.Documents.Open(wordRTFPath);

                try
                {

                    wd.ExportAsFixedFormat(pdfPath, word.WdExportFormat.wdExportFormatPDF);



                }
                catch (Exception e)
                {
                    MessageBox.Show("Err on exportWordRtfToPdf" + e.Message);

                }
                finally
                {
                    wd.Close();
                    wa.Quit();
                    wa = null;
                }
            }
        }

        public void showFile(string path)
        {
            if (File.Exists(path))
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(path);
                p.Start();
                //p.WaitForExit(); -since we dont delete the files from nautilus no need to waitForExit
            }
        }

        public void deleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetPathFromPhrase(string phraseHeaderName, string phraseEntryName)
        {
            try
            {


                string sql = "select phrase_description from lims_sys.phrase_entry where phrase_id in" +
                            "(select phrase_id from lims_sys.phrase_header  where name ='" + phraseHeaderName + "') and phrase_name='" + phraseEntryName + "'";
                cmd.CommandText = sql;
                var path = cmd.ExecuteScalar();
                cmd.Dispose();
                if (path != null)
                {
                    return path.ToString();
                }
                return null;
            }
            catch (Exception e)
            {

                MessageBox.Show("Invalid Path");
                return null;

            }
        }

    }
}