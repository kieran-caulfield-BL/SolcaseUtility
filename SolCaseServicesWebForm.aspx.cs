using System;
using System.IO;
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
    public partial class SolCaseServicesWebForm : System.Web.UI.Page
    {
        public string SelectedMatter { get; private set; }
        public int SelectedMatterIndex { get; private set; }

        public static class Globals
        {
            public static DataSet solcaseDocs { get; set; }

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

            XmlDocument xmlDoc = xmlReturn.OwnerDocument;
            XmlReader xmlReader = new XmlNodeReader(xmlDoc);

            Globals.solcaseDocs = new DataSet();
            Globals.solcaseDocs.ReadXml(xmlReader);

            // populate the client name
            div_clientName.InnerText = Globals.solcaseDocs.Tables["Client"].Rows[0]["CL-NAME"].ToString();

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

            string[] selectedColumns = new[] { "HISTORY-NO", "HST-DESCRIPTION", "PROPOSED-FILE-NAME" };

            DataTable displayedColumns = new DataView(Globals.solcaseDocs.Tables["SolDoc"]).ToTable(false, selectedColumns);

            //GridViewClientDocs.DataSource = Globals.solcaseDocs.Tables["SolDoc"];
            GridViewClientDocs.DataSource = displayedColumns;
            GridViewClientDocs.DataBind();
        }

    }
}