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
using System.Windows.Controls;

namespace SolcaseUtility
{
    public partial class SolCaseServicesUDScreens : System.Web.UI.Page
    {
        public string SelectedUDS { get; private set; }
        public int SelectedUDSIndex { get; private set; }

        public static class UDSGlobals
        {
            public static DataSet solcaseScreens { get; set; }

            public static XmlDocument xmlDoc { get; set; }

            static UDSGlobals()
            {
                // initialize MyData property in the constructor with static methods
                solcaseScreens = new DataSet();
            }
        }

       
        protected void UDS_Page_Load(object sender, EventArgs e)
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

            SolcaseUtility.SolCaseService testWebService = new SolcaseUtility.SolCaseService();
            XmlElement xmlReturn = testWebService.getV3PRListUDScreens(txtBoxMatterId.Text);

            Globals.xmlDoc = new XmlDocument();
            Globals.xmlDoc = xmlReturn.OwnerDocument;
            XmlReader xmlReader = new XmlNodeReader(Globals.xmlDoc);

            UDSGlobals.solcaseScreens = new DataSet();
            UDSGlobals.solcaseScreens.ReadXml(xmlReader);

            // populate the client name
            //div_clientName.InnerText = Globals.solcaseScreens.Tables["Client"].Rows[0]["CL-NAME"].ToString();

            // update the xml dom for each node FIELD-DESC, change the ";" to a CR Or Environment.Newline

            xmlReader.Close();

            // populate the grid view
            //div_matterDesc2.InnerText = UDSGlobals.solcaseScreens.Tables["Matter"].Rows[0]["DESCRIPTION"].ToString();

            string[] selectedColumns = new[] { "UDS-TYPE", "GROUP-NO", "SCREEN-NAME", "FIELD-DESC", "UD-FIELD" };

            DataTable displayedColumns = new DataView(UDSGlobals.solcaseScreens.Tables["UDScreenDetails"]).ToTable(false, selectedColumns);

            // add line throws to grid
            foreach (DataRow drOutput in displayedColumns.Rows)
            {
                drOutput["FIELD-DESC"] = drOutput["FIELD-DESC"].ToString().Replace(";", @"<br>");
                drOutput["UD-FIELD"] = drOutput["UD-FIELD"].ToString().Replace(";", @"<br>");
            }

             //GridViewClientDocs.DataSource = Globals.solcaseDocs.Tables["SolDoc"];
            GridViewUDScreens.DataSource = displayedColumns;
            
            GridViewUDScreens.DataBind();

        }


        protected void GridViewUDScreens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //disable HTML encoding in all rows and columns

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell ObjTC in e.Row.Cells)
                {
                    string decodedText = HttpUtility.HtmlDecode(ObjTC.Text);
                    ObjTC.Text = decodedText;
                }
            }
        }

        protected void Button12_Click(object sender, EventArgs e)
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
            string fileName = txtBoxMatterId.Text + today.ToString("dd-MM-yyyy") +".xml";

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


        protected void GridViewUDScreens_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridViewUDScreens_OnSort(object sender, EventArgs e)
        {

        }

    }

    public static class UDSCopier
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