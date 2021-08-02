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
                // for Doc No > 0 you casnnot truct the Sub Path as that is from the original Doc! You can to make it up
                if (row["HD-DOCUMENT-NO"].ToString() == "0")
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

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("FILE-NAME"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("FILE-NAME", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["FILE-NAME"] = FileNameCorrector.ToValidFileName(row["HST-DESCRIPTION"].ToString() + "." + row["EXTENSION"].ToString());
            }

            if (!Globals.solcaseDocs.Tables["ndImport"].Columns.Contains("ACCESS"))
            {
                Globals.solcaseDocs.Tables["ndImport"].Columns.Add("ACCESS", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["ndImport"].Rows)
            {
                row["ACCESS"] = "VES";
            }

            xmlReader.Close();

            // populate the tree view
            TreeViewMatterList.Nodes.Clear();

            foreach (DataRow row in Globals.solcaseDocs.Tables["Matter"].Rows)
            {
                TreeNode matterNode = new TreeNode(row["MT-CODE"].ToString());

                TreeViewMatterList.Nodes.Add(matterNode);

            }

            // clear the grid view
            GridViewClientDocs.DataSource = null;
           GridViewClientDocs.DataBind();

        }

        protected void ExportGridToCSV()
        {
            // lets get a datatable again
            string[] selectedColumns = new[] { "filePath", "HISTORY-NO", "FILE-NAME", "EXTENSION", "ST-LOCATION", "ACCESS", "HD-DOCUMENT-NO", "HD-AUTHOR", "HD-DATE-CREATED", "HD-AMENDED-BY", "HD-DATE-AMENDED" };

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
            string[] selectedColumns = new[] {"filePath", "HISTORY-NO", "FILE-NAME", "EXTENSION", "ST-LOCATION", "ACCESS", "HD-DOCUMENT-NO", "HD-AUTHOR", "HD-DATE-CREATED", "HD-AMENDED-BY", "HD-DATE-AMENDED" };

            DataTable displayedColumns = new DataView(Globals.solcaseDocs.Tables["ndImport"]).ToTable(false, selectedColumns);

            //GridViewClientDocs.DataSource = Globals.solcaseDocs.Tables["SolDoc"];
            GridViewClientDocs.DataSource = displayedColumns;
            GridViewClientDocs.DataBind();

        }

    }

    public static class Extensions
    {
        public static string ToCSV(this DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

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
}