using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SolcaseUtility
{
    public partial class SolCaseServicesWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SolCaseService testWebService = new SolCaseService();
            XmlElement xmlReturn = testWebService.getClientDocs("081389");
            TextBox1.Text = xmlReturn.ToString();
        }
    }
}