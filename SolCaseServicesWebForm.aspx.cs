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
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SolcaseUtility
{
    public partial class SolCaseServicesWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // create data grid headers, the data grid is 1000 width

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

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

            // export the xml doc

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

            xmlDoc.WriteTo(writer);
            writer.Flush();
            Response.Clear();
            byte[] byteArray = stream.ToArray();
            Response.AddHeader("Content-Disposition", "attachment;filename=ClientDocs.xml");
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/xml";
            Response.BinaryWrite(byteArray);
            //Response.WriteFile("ClientDocs.xml");
            //Response.End();
            writer.Close();

            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);

            GridViewClientDocs.DataSource = ds;
            GridViewClientDocs.DataBind();

            xmlReader.Close();

            div_xml.InnerHtml = "<Table>";

            XmlReader xmlReader2 = new XmlNodeReader(xmlDoc);

            while (xmlReader2.Read())
            {
                xmlReader2.MoveToElement();

                if (xmlReader2.Name == "SolDoc")
                {

                    div_xml.InnerHtml = div_xml.InnerHtml + "<tr><td>" + xmlReader2.Name + "</td>";

                    while (xmlReader2.MoveToNextAttribute())
                    {
                        div_xml.InnerHtml = div_xml.InnerHtml + "<td> " + xmlReader2.Name + " " + xmlReader2.Value + "</td>";
                    }

                    div_xml.InnerHtml = div_xml.InnerHtml + "</tr>";
                }
            }

            div_xml.InnerHtml = div_xml.InnerHtml + "</Table>";

            xmlReader2.Close();
        }

    }
}