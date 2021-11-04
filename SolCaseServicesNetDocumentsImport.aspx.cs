using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace SolcaseUtility
{
    public partial class SolCaseServicesNetDocumentsImport : System.Web.UI.Page
    {
        public string SelectedMatter { get; private set; }
        public int SelectedMatterIndex { get; private set; }

        public static class Globals
        {
            public static DataSet solcaseDocs { get; set; }

            public static XmlDocument xmlDoc { get; set; }

            static Globals()
            {
                // initialize MyData property in the constructor with static methods
                solcaseDocs = new DataSet();
            }
        }

        public static class FileNameCorrector
        {
            private static HashSet<char> invalid = new HashSet<char>(Path.GetInvalidFileNameChars());

            public static string ToValidFileName(string name, char replacement = '\0')
            {
                var builder = new StringBuilder();
                foreach (var cur in name)
                {
                    if (cur > 31 && cur < 128 && !invalid.Contains(cur))
                    {
                        builder.Append(cur);
                    }
                    else if (replacement != '\0')
                    {
                        builder.Append(replacement);
                    }
                }

                // replace ".pdf.pdf" and set ampersand to "and"
                builder.Replace(".pdf.pdf", ".pdf");
                builder.Replace("&amp;", "and");

                return builder.ToString();
            }
        }




        protected void Page_Load(object sender, EventArgs e)
        {

            // create data grid headers, the data grid is 1000 width

        }

        protected void Button11_Click(object sender, EventArgs e)
        {

            Response.Clear();

            Regex rgxClient = new Regex(@"[0-9]{6}-[0-9]{6}");

            if (!rgxClient.IsMatch(txtBoxMatterId.Text))
            {

                div_xml.InnerText = "Please enter a Matter Id in format 6 x 6 numbers numbers with a leading '0' if needed.";
                return;
            }

            Regex rgxClientnD = new Regex(@"^[0-9]{1,10}$");

            if (!rgxClientnD.IsMatch(txtBoxNetDocsClient.Text))
            {

                div_xml.InnerText = "Please enter a numeric Net Docs Client Id.";
                return;
            }

            if (!rgxClientnD.IsMatch(TxtBoxnetDocsMatter.Text))
            {

                div_xml.InnerText = "Please enter a numeric Net Docs Matter Id.";
                return;
            }

            SolCaseService testWebService = new SolCaseService();
            XmlElement xmlReturn = testWebService.nD_Import(txtBoxMatterId.Text);

            Globals.xmlDoc = new XmlDocument();
            Globals.xmlDoc = xmlReturn.OwnerDocument;
            XmlReader xmlReader = new XmlNodeReader(Globals.xmlDoc);

            Globals.solcaseDocs = new DataSet();
            Globals.solcaseDocs.ReadXml(xmlReader);

            // populate the client name removed from NetDocs Importer
            //div_clientName.InnerText = Globals.solcaseDocs.Tables["Client"].Rows[0]["CL-NAME"].ToString();

            // create an additional column for the dataset
            // create a new dataset table "SolDoc" column to generate the proposed file name if not exists
            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("filePath"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("filePath", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                // depending on version (doc no) switch the file name
                // for Doc No > 1 (0 in Solcase, 1 converted for nD) you cannot truct the Sub Path as that is from the original Doc! You can to make it up
                if (row["HD-DOCUMENT-NO"].ToString() == "1")
                {          
                    row["filePath"] = row["DOS-PATH"].ToString() + row["SUB-PATH"].ToString() + row["HD-ORIG-DOC-NAME"].ToString();
                } else
                {
                    // create the subpath
                    string subPath = "";
                    subPath = "HST" + row["HD-DOCUMENT-NAME"].ToString().Substring(0, 3) + "\\HST" + row["HD-DOCUMENT-NAME"].ToString().Substring(3, 3) + "\\";
                    row["filePath"] = row["DOS-PATH"].ToString() + subPath + row["HD-DOCUMENT-NAME"].ToString();
                }           
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("Client"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("Client", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["Client"] = txtBoxNetDocsClient.Text;
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("Matter"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("Matter", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["Matter"] = TxtBoxnetDocsMatter.Text;
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("FILE-NAME"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("FILE-NAME", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["FILE-NAME"] = FileNameCorrector.ToValidFileName(row["HST-DESCRIPTION"].ToString() + "." + row["EXTENSION"].ToString());
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("PROFILE-TYPE"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("PROFILE-TYPE", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                if (row["HST-DOCUMENT-TYPE"].ToString() == "E")
                {
                    row["PROFILE-TYPE"] = "EM";
                } else
                {
                    row["PROFILE-TYPE"] = "GEN";
                }

            }

           

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("ACCESS"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("ACCESS", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["ACCESS"] = "";
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("VERSION-KEY"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("VERSION-KEY", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                 row["VERSION-KEY"] = row["HD-ORIG-DOC-NAME"].ToString().Substring(0,8);
            }

            xmlReader.Close();

            // populate the tree view
            TreeViewMatterList.Nodes.Clear();

            foreach (DataRow row in Globals.solcaseDocs.Tables["Matter"].Rows)
            {
                System.Web.UI.WebControls.TreeNode matterNode = new System.Web.UI.WebControls.TreeNode(row["MT-CODE"].ToString());

                TreeViewMatterList.Nodes.Add(matterNode);

            }

            // clear the grid view
            GridViewClientDocs.DataSource = null;
           GridViewClientDocs.DataBind();

            DateTime today = DateTime.Today;
            string fileName = txtBoxMatterId.Text + today.ToString("dd-MM-yyyy") + ".csv";

            // update the command text box
            // build command line from app setrtings
            string host = ConfigurationManager.AppSettings.Get("host");
            string user = ConfigurationManager.AppSettings.Get("user");
            string pass = ConfigurationManager.AppSettings.Get("pass");
            string cabinet = ConfigurationManager.AppSettings.Get("CurrentCabinet");

            txtBoxCommand.Text = "Start C:\\nDImport\\ndimport /user=" + user + " /pass=" + pass + " /host=" + host + " /cab=\""+ cabinet + "\" /list=\"C:\\Users\\kieran$caulfield\\Downloads\\" + fileName + "\" /maxerr=500 /utf8=N /dateformat=D /utc=N";

        }

        protected void ExportGridToCSV()
        {
            // lets get a datatable again
            string[] selectedColumns = new[] { "filePath", "Client", "Matter", "PROFILE-TYPE", "FILE-NAME", "EXTENSION", "ST-LOCATION", "ACCESS", "VERSION-KEY", "HD-DOCUMENT-NO", "HD-AUTHOR-NAME", "HD-DATE-CREATED", "HD-AMENDED-BY", "HD-DATE-AMENDED" };

            DataTable displayedColumns = new DataView(Globals.solcaseDocs.Tables["ndImport"]).ToTable(false, selectedColumns);

            var report = displayedColumns.ToCSV();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(report);

            // create the filename and write to the browser
            DateTime today = DateTime.Today;
            string fileName = txtBoxMatterId.Text + today.ToString("dd-MM-yyyy") + ".csv";

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "text/csv";
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
                        
        }



        protected void Button12_Click(object sender, EventArgs e)
        {
            ExportGridToCSV();    
        }
        protected void GridViewClientDocs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridViewClientDocs_OnSort(object sender, EventArgs e)
        {

        }

        protected void TreeViewMatterList_SelectedNodeChanged(object sender, EventArgs e)
        {
            SelectedMatter = TreeViewMatterList.SelectedNode.Text;
            SelectedMatterIndex = TreeViewMatterList.Nodes.IndexOf(TreeViewMatterList.SelectedNode);

            div_matterDesc.InnerText = Globals.solcaseDocs.Tables["Matter"].Rows[SelectedMatterIndex]["MAT-DESCRIPTION"].ToString();

            //' Note KC you cannot show columns from Client or Matter table in the Grid
            string[] selectedColumns = new[] { "filePath", "Client", "Matter", "PROFILE-TYPE", "FILE-NAME", "EXTENSION", "ST-LOCATION", "ACCESS", "HD-DOCUMENT-NO", "HD-AUTHOR", "HD-DATE-CREATED", "HD-AMENDED-BY", "HD-DATE-AMENDED", "HISTORY-NO" };

            DataTable displayedColumns = new DataView(Globals.solcaseDocs.Tables["ndImport"]).ToTable(false, selectedColumns);

            //GridViewClientDocs.DataSource = Globals.solcaseDocs.Tables["SolDoc"];
            GridViewClientDocs.DataSource = displayedColumns;
            GridViewClientDocs.DataBind();

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            //https://docs.microsoft.com/en-us/troubleshoot/aspnet/upload-file-to-web-site

            string strFileName;
            string strFilePath;
            string strFolder;
            strFolder = Server.MapPath("./upload/");
            // Retrieve the name of the file that is posted.
            strFileName = oFile.PostedFile.FileName;
            strFileName = Path.GetFileName(strFileName);
            if (oFile.Value != "")
            {
                // Create the folder if it does not exist.
                if (!Directory.Exists(strFolder))
                {
                    Directory.CreateDirectory(strFolder);
                }
                // Save the uploaded file to the server.
                strFilePath = strFolder + strFileName;
                if (File.Exists(strFilePath))
                {
                    lblUploadResult.Text = strFileName + " already exists on the server!";
                }
                else
                {
                    oFile.PostedFile.SaveAs(strFilePath);
                    lblUploadResult.Text = strFileName + " has been successfully uploaded.";

                    string csvPath = strFolder + strFileName;

                    try
                    {
                        ReadCSV csv = new ReadCSV(csvPath);

                        try
                        {
                            //UploadGrid.DataBind();
                            UploadGrid.DataSource = csv.readCSV;
                            UploadGrid.DataBind();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
            }
            else
            {
                lblUploadResult.Text = "Click 'Browse' to select the file to upload.";
            }
            // Display the result of the upload.
            frmConfirmation.Visible = true;
        }

        protected void UploadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GridViewRow row = UploadGrid.SelectedRow;

            // now populate the fields on the screen
            //txtBoxMatterId.Text = row.Cells[1].Text;
            txtBoxMatterId.Text = UploadGrid.SelectedRow.Cells[2].Text;
            txtBoxNetDocsClient.Text = UploadGrid.SelectedRow.Cells[3].Text;
            TxtBoxnetDocsMatter.Text = UploadGrid.SelectedRow.Cells[4].Text;

        }

        protected void UploadGrid_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void UploadGrid_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            // now populate the fields on the screen
            txtBoxMatterId.Text = UploadGrid.SelectedRow.Cells[2].Text;
            txtBoxNetDocsClient.Text = UploadGrid.SelectedRow.Cells[3].Text;
            TxtBoxnetDocsMatter.Text = UploadGrid.SelectedRow.Cells[4].Text;
        }
    }

    public static class Extensions
    {
        public static string ToCSV(this DataTable table)
        {
            var result = new StringBuilder();
            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //   result.Append(table.Columns[i].ColumnName);
            //    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            //}

            // need to ignore the natural table column header and put in the titles required by nD Import
            // "filePath", "Client", "Matter", "PROFILE-TYPE", "FILE-NAME", "EXTENSION", "ST-LOCATION", "ACCESS", "HD-DOCUMENT-NO", "HD-AUTHOR-NAME", "HD-DATE-CREATED", "HD-AMENDED-BY", "HD-DATE-AMENDED""
            result.Append("filepath,Client,Matter,Document Type,DOCUMENT NAME,DOCUMENT EXTENSION,FOLDER,ACCESS,VERSION KEY,VERSION NUMBER,CREATED BY,CREATED DATE,LAST MODIFIED BY,LAST MODIFIED DATE" + "\n");
            
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }

            return result.ToString();


        }
    }


    public static class nDCopier
    {
        public static async Task<string> CopyFiles(Dictionary<string, string> files, Action<double> progressCallback)
        {
            long total_size = files.Keys.Select(x => new FileInfo(x).Length).Sum();

            long total_read = 0;

            double progress_size = 1000.0;

            string progressText = "";

            foreach (var item in files)
            {
                long total_read_for_file = 0;

                var from = item.Key;
                var to = item.Value;

                progressText = progressText + "Copy " + from.ToString() + " to " + to.ToString() + Environment.NewLine;

                using (var outStream = new FileStream(to, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (var inStream = new FileStream(from, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        try
                        {
                            await CopyStream(inStream, outStream, x =>
                            {
                                total_read_for_file = x;
                                progressCallback(((total_read + total_read_for_file) / (double)total_size) * progress_size);
                            });
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }

                total_read += total_read_for_file;

            }
            return progressText;
        }

        public static async Task CopyStream(Stream from, Stream to, Action<long> progress)
        {
            int buffer_size = 10240;

            byte[] buffer = new byte[buffer_size];

            long total_read = 0;

            while (total_read < from.Length)
            {
                int read = await from.ReadAsync(buffer, 0, buffer_size);

                await to.WriteAsync(buffer, 0, read);

                total_read += read;

                progress(total_read);
            }
        }

    }

    public class ReadCSV
    {
        public DataTable readCSV;

        public ReadCSV(string fileName, bool firstRowContainsFieldNames = true)
        {
            readCSV = GenerateDataTable(fileName, firstRowContainsFieldNames);
        }

        private static DataTable GenerateDataTable(string fileName, bool firstRowContainsFieldNames = true)
        {
            DataTable result = new DataTable();

            if (fileName == "")
            {
                return result;
            }

            string delimiters = ",";
            string extension = Path.GetExtension(fileName);

            if (extension.ToLower() == "txt")
                delimiters = "\t";
            else if (extension.ToLower() == "csv")
                delimiters = ",";

            var rowCount = 0;

            using (TextFieldParser tfp = new TextFieldParser(fileName))
            {
                tfp.SetDelimiters(delimiters);

                // Get The Column Names
                if (!tfp.EndOfData)
                {
                    rowCount++;
                    string[] fields = tfp.ReadFields();

                    for (int i = 0; i < fields.Count(); i++)
                    {
                        if (firstRowContainsFieldNames) 
                            result.Columns.Add(fields[i]);                 
                         else
                           result.Columns.Add("Col" + i); 
                    }

                    // If first line is data then add it
                    if (!firstRowContainsFieldNames)
                        result.Rows.Add(fields);
                }

                // Get Remaining Rows from the CSV
                while (!tfp.EndOfData)
                    result.Rows.Add(tfp.ReadFields());
                

            }

            // filter out rows where teh Client Id or File Reference is not set propertly or at all

            return result;
        }
    }
}