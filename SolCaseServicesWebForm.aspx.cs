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
    public partial class SolCaseServicesUDScreens : System.Web.UI.Page
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

        protected void Button1_Click(object sender, EventArgs e)
        {

            Response.Clear();

            Regex rgxClient = new Regex(@"[0-9]{6}");

            if (!rgxClient.IsMatch(txtBoxClientId.Text))
            {

                div_xml.InnerText = "Please enter a Code Code in format 6 numbers with a leading '0' if needed.";
                return;
            }

            SolCaseService testWebService = new SolCaseService();
            XmlElement xmlReturn = testWebService.getClientDocs(txtBoxClientId.Text);

            Globals.xmlDoc = new XmlDocument();
            Globals.xmlDoc = xmlReturn.OwnerDocument;
            XmlReader xmlReader = new XmlNodeReader(Globals.xmlDoc);

            Globals.solcaseDocs = new DataSet();
            Globals.solcaseDocs.ReadXml(xmlReader);

            // populate the client name
            try
            {
                div_clientName.InnerText = Globals.solcaseDocs.Tables["Client"].Rows[0]["CL-NAME"].ToString();
            } catch
            {
                div_clientName.InnerText = "";
            }

            // create an additional column for the dataset
            // create a new dataset table "SolDoc" column to generate the proposed file name if not exists
            if (!Globals.solcaseDocs.Tables["SolDoc"].Columns.Contains("PROPOSED-FILE-NAME"))
            {
                Globals.solcaseDocs.Tables["SolDoc"].Columns.Add("PROPOSED-FILE-NAME", typeof(String));
            }
            // Now populate the new column
            foreach (DataRow row in Globals.solcaseDocs.Tables["SolDoc"].Rows)
            {
                row["PROPOSED-FILE-NAME"] = FileNameCorrector.ToValidFileName(row["HST-DESCRIPTION"].ToString() + "." + row["EXTENSION"].ToString());
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


        protected void Button2_Click(object sender, EventArgs e)
        {
            // Download the whole page as an xml file to the browser
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

            Globals.xmlDoc.WriteTo(writer);
            writer.Flush();

            // create the filename and write to the browser
            DateTime today = DateTime.Today;
            string fileName = txtBoxClientId.Text + today.ToString("dd-MM-yyyy") +".xml";

            Response.Clear();
            byte[] byteArray = stream.ToArray();
            Response.AddHeader("Content-Disposition", "attachment;filename="+fileName);
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.CacheControl = "No-cache";
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/xml";
            Response.BinaryWrite(byteArray);
            //Response.WriteFile(fileName);
            Response.Flush();
            Response.SuppressContent = true;
            Response.End();

            writer.Close();
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

            div_clientName.InnerText = string.Format("[MT-CODE] = '{0}'", SelectedMatter);

            string[] selectedColumns = new[] {"MT-CODE", "HISTORY-NO", "HST-DESCRIPTION", "PROPOSED-FILE-NAME" };

            DataTable displayedColumns = new DataView(Globals.solcaseDocs.Tables["SolDoc"]).ToTable(false, selectedColumns);
            DataView dataView = displayedColumns.DefaultView;
            dataView.RowFilter = string.Format("[MT-CODE] = '{0}'", SelectedMatter);
       

            //GridViewClientDocs.DataSource = Globals.solcaseDocs.Tables["SolDoc"];
            GridViewClientDocs.DataSource = dataView;
            GridViewClientDocs.DataBind();
        }

    }

    public static class Copier
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