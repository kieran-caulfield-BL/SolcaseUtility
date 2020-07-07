using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SolcaseUtility
{
    /// <summary>
    /// Webservices to call Solcase and SOS databases
    /// </summary>
    [WebService(Namespace = "http://birkettlong.co.uk/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]

    public class SolCaseService : System.Web.Services.WebService
    {
        public SolCaseService()
        {
            //InitializeComponent();
        }

        [WebMethod]
        public XmlElement getClientDocs(string clientId)
        {
            //String inputMatterCode = "081389";

            String inputMatterCode = clientId;

            Regex rgxClient = new Regex(@"[0-9]{6}");

            if (!rgxClient.IsMatch(inputMatterCode))
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseDocs");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = "Client Code must be a 6 digit number";
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }

            string sql = @"Select
    PUB.MATDB.""MT-CODE"",
    PUB.MATDB.""CL-CODE"",
    PUB.MATDB.DESCRIPTION As ""MAT-DESCRIPTION"",
    PUB.MATDB.IMPLEMENTATION,
    PUB.MATDB.""DATE-OPENED"",
    PUB.MATDB.""MT-TYPE"",
    PUB.CLIDB.CONTACT1,
    PUB.HISTORY.DESCRIPTION As ""HST-DESCRIPTION"",
    PUB.HISTORY.""LEVEL-FEE-EARNER"",
    PUB.HISTORY.""DOCUMENT-NAME"",
    PUB.HISTORY.""DOCUMENT-TYPE"",
    PUB.HISTORY.""HISTORY-NO"",
    to_char(PUB.HISTORY.""DATE-INSERTED"",'DD-MM-YYYY') As ""DATE-INSERTED"",
    PUB.""DOC-CONTROL"".""DOC-GROUP"",
    PUB.""DOC-CONTROL"".""ST-LOCATION"",
    PUB.""DOC-CONTROL"".""SUB-PATH"",
    PUB.""DOC-CONTROL"".""TYPE"" As ""DOC-TYPE"",
    PUB.""DOC-CONTROL"".EXTENSION,
    PUB.""DOC-CONTROL"".VERSION,
    PUB.""FILE-LOCATION"".DESCRIPTION As ""LOC-DESCRIPTION"",
    PUB.""FILE-LOCATION"".""LOC-OPTIONS"",
    PUB.""FILE-LOCATION"".""TYPE"" As ""LOC-TYPE""
From
    PUB.MATDB Inner Join
    PUB.HISTORY On PUB.HISTORY.""MT-CODE"" = PUB.MATDB.""MT-CODE""
            And PUB.HISTORY.IMPLEMENTATION = PUB.MATDB.IMPLEMENTATION
            And PUB.HISTORY.""DATE-INSERTED"" >= PUB.MATDB.""DATE-OPENED"" Inner Join
    PUB.""DOC-CONTROL"" On PUB.""DOC-CONTROL"".""DOC-ID"" = PUB.HISTORY.""DOCUMENT-NAME"" Inner Join
    PUB.""FILE-LOCATION"" On PUB.""FILE-LOCATION"".""LOC-NAME"" = PUB.""DOC-CONTROL"".""ST-LOCATION"" Inner Join
    PUB.CLIDB On PUB.CLIDB.""CL-CODE"" = PUB.MATDB.""CL-CODE""
Where
    PUB.MATDB.""CL-CODE"" = ? And
    PUB.HISTORY.""DOCUMENT-TYPE"" Is Not Null And
    PUB.HISTORY.""DOCUMENT-TYPE"" <> ''
order by 
    PUB.MATDB.""CL-CODE"",
    PUB.MATDB.""MT-CODE"",
    PUB.HISTORY.""DATE-INSERTED"" ASC
";
            OdbcConnection conn = null;
            OdbcDataReader reader = null;

            try
            {
                // open connection
                conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["SolCaseLivex64"].ToString());
                conn.Open();
                // execute the SQL
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.Parameters.Add("MatterIdentifier", OdbcType.VarChar).Value = inputMatterCode; // "example 081389-000002"
                reader = cmd.ExecuteReader();

                //Console.WriteLine("Database = {0} \nDriver = {1} \nQuery {2}\nConnection String = {3}\nServer Version = {4}\nDataSource = {5}",
                //conn.Database, conn.Driver, cmd.CommandText, conn.ConnectionString, conn.ServerVersion, conn.DataSource);

                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("SolcaseDocs");
                xmlDoc.AppendChild(rootNode);

                XmlNode clientNode = xmlDoc.CreateElement("Client");
                XmlAttribute clientCode = xmlDoc.CreateAttribute("CL-CODE");
                XmlAttribute clientName = xmlDoc.CreateAttribute("CL-NAME");

                XmlNode matterNode = xmlDoc.CreateElement("Matter");
                XmlAttribute matterCode = xmlDoc.CreateAttribute("MT-CODE");
                XmlAttribute matterDesc = xmlDoc.CreateAttribute("MAT-DESCRIPTION");


                Boolean clientNodeNotCreated = true;
                String currentMatterCode = ""; // on change of matter we need to create another matter node

                while (reader.Read())
                {
                    if (clientNodeNotCreated)
                    {
                        // only once used, turn off once processed                        
                        clientCode.Value = reader["CL-CODE"].ToString();
                        clientNode.Attributes.Append(clientCode);

                        clientName.Value = reader["CONTACT1"].ToString();
                        clientNode.Attributes.Append(clientName);
                        rootNode.AppendChild(clientNode);
                        clientNodeNotCreated = false;
                    }


                    if (currentMatterCode != reader["MT-CODE"].ToString())
                    {
                        // the matter code has changed, create a new Matter Node                       
                        matterCode.Value = reader["MT-CODE"].ToString();
                        matterNode.Attributes.Append(matterCode);

                        matterDesc.Value = reader["MAT-DESCRIPTION"].ToString();
                        matterNode.Attributes.Append(matterDesc);
                        clientNode.AppendChild(matterNode);
                    }

                    XmlNode soldocNode = xmlDoc.CreateElement("SolDoc");
                    XmlAttribute histDesc = xmlDoc.CreateAttribute("HST-DESCRIPTION");
                    XmlAttribute histNo = xmlDoc.CreateAttribute("HISTORY-NO");
                    XmlAttribute docName = xmlDoc.CreateAttribute("DOCUMENT-NAME");
                    XmlAttribute solcaseDocType = xmlDoc.CreateAttribute("DOCUMENT-TYPE");
                    XmlAttribute dateInserted = xmlDoc.CreateAttribute("DATE-INSERTED");
                    XmlAttribute histFE = xmlDoc.CreateAttribute("LEVEL-FEE-EARNER");
                    XmlAttribute docGroup = xmlDoc.CreateAttribute("DOC-GROUP");
                    XmlAttribute stLocation = xmlDoc.CreateAttribute("ST-LOCATION");
                    XmlAttribute dosPath = xmlDoc.CreateAttribute("DOS-PATH");
                    XmlAttribute subPath = xmlDoc.CreateAttribute("SUB-PATH");
                    XmlAttribute actualDocType = xmlDoc.CreateAttribute("DOC-TYPE");
                    XmlAttribute docExt = xmlDoc.CreateAttribute("EXTENSION");
                    XmlAttribute docVersion = xmlDoc.CreateAttribute("VERSION");
                    XmlAttribute fileLoc = xmlDoc.CreateAttribute("LOC-DESCRIPTION");
                    XmlAttribute fileLocType = xmlDoc.CreateAttribute("LOC-TYPE");

                    // assign values from result set reader
                    histNo.Value = reader["HISTORY-NO"].ToString();
                    histDesc.Value = reader["HST-DESCRIPTION"].ToString().Replace("&", "&amp;");
                    docName.Value = reader["DOCUMENT-NAME"].ToString();
                    solcaseDocType.Value = reader["DOCUMENT-TYPE"].ToString();
                    dateInserted.Value = reader["DATE-INSERTED"].ToString();
                    histFE.Value = reader["LEVEL-FEE-EARNER"].ToString();
                    docGroup.Value = reader["DOC-GROUP"].ToString();
                    stLocation.Value = reader["ST-LOCATION"].ToString();
                    // split LOC-OPTIONS to get the DOS PATH
                    string[] locOptions = reader["LOC-OPTIONS"].ToString().Split(',');
                    dosPath.Value = locOptions[2].ToString();
                    subPath.Value = reader["SUB-PATH"].ToString();
                    actualDocType.Value = reader["DOC-TYPE"].ToString();
                    docExt.Value = reader["EXTENSION"].ToString();
                    docVersion.Value = reader["VERSION"].ToString();
                    fileLoc.Value = reader["LOC-DESCRIPTION"].ToString();
                    fileLocType.Value = reader["LOC-TYPE"].ToString();

                    // assign attributes to soldocNode
                    soldocNode.Attributes.Append(histNo);
                    soldocNode.Attributes.Append(histDesc);
                    soldocNode.Attributes.Append(docName);
                    soldocNode.Attributes.Append(solcaseDocType);
                    soldocNode.Attributes.Append(dateInserted);
                    soldocNode.Attributes.Append(histFE);
                    soldocNode.Attributes.Append(docGroup);
                    soldocNode.Attributes.Append(stLocation);
                    soldocNode.Attributes.Append(dosPath);
                    soldocNode.Attributes.Append(subPath);
                    soldocNode.Attributes.Append(actualDocType);
                    soldocNode.Attributes.Append(docExt);
                    soldocNode.Attributes.Append(docVersion);
                    soldocNode.Attributes.Append(fileLoc);
                    soldocNode.Attributes.Append(fileLocType);

                    // append node to matter
                    matterNode.AppendChild(soldocNode);
                }

                reader.Close();
                conn.Close();

                return xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseDocs");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = e.Message.ToString();
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }
        }
    
        [WebMethod]
        public XmlElement getOpenMatterList(string matterTypeInput)
        {
            String inputMatterCode = matterTypeInput;

            Regex rgxClient = new Regex(@"[A-Z,0-9]{4}");

            if (!rgxClient.IsMatch(inputMatterCode))
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseDocs");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = "Mattyer Type must be a 4 digit number";
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }

            string sql = @"Select
    A.MATDB.""MT-TYPE"" As ""MT-TYPE"",
    A.MATDB.""DATE-CLOSED"" As ""DATE-CLOSED"",
    A.MATDB.ENTITYTYPE As ENTITYTYPE,
    A.MATDB.BRANCH As BRANCH,
    A.MATDB.""MT-CODE"" As ""MT-CODE"",
    A.MATDB.""CL-CODE"" As ""CL-CODE"",
    A.MATDB.DESCRIPTION As DESCRIPTION,
    A.MATDB.""FEE-EARNER"" As ""FEE-EARNER"",
    A.MATDB.""DATE-OPENED"" As ""DATE-OPENED"",
    A.MATDB.""ST-CODE"" As ""ST-CODE"",
    PUB.FEETR.""NAME"",
    PUB.FEETR.DEPARTMENT
From
    A.MATDB Inner Join
    PUB.FEETR On PUB.FEETR.""FEE-EARNER"" = A.MATDB.""FEE-EARNER""
Where
    A.MATDB.""MT-TYPE"" = ? And
    A.MATDB.""DATE-CLOSED"" Is Null
With(NOLOCK)";

            OdbcConnection conn = null;
            OdbcDataReader reader = null;

            try
            {
                // open connection
                conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["SOSLivex64"].ToString());
                conn.Open();
                // execute the SQL
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.Parameters.Add("MatterType", OdbcType.VarChar).Value = inputMatterCode; // "example V3PR"
                reader = cmd.ExecuteReader();

                //Console.WriteLine("Database = {0} \nDriver = {1} \nQuery {2}\nConnection String = {3}\nServer Version = {4}\nDataSource = {5}",
                //conn.Database, conn.Driver, cmd.CommandText, conn.ConnectionString, conn.ServerVersion, conn.DataSource);

                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("SOSOpenMatters");
                xmlDoc.AppendChild(rootNode);

                while (reader.Read())
                {

                    XmlNode matterNode = xmlDoc.CreateElement("Matter");
                    XmlAttribute matterCode = xmlDoc.CreateAttribute("MT-CODE");
                    XmlAttribute matterType = xmlDoc.CreateAttribute("MT-TYPE");
                    XmlAttribute matterDesc = xmlDoc.CreateAttribute("DESCRIPTION");

                    // the matter code has changed on each new record so create a new Matter Node                       
                    matterCode.Value = reader["MT-CODE"].ToString();
                    matterNode.Attributes.Append(matterCode);

                    matterType.Value = reader["MT-TYPE"].ToString();
                    matterNode.Attributes.Append(matterType);

                    matterDesc.Value = reader["DESCRIPTION"].ToString();
                    matterNode.Attributes.Append(matterDesc);

                    rootNode.AppendChild(matterNode);

                    XmlNode sosMatterNode = xmlDoc.CreateElement("SOSMatterDetails");
                    XmlAttribute branch = xmlDoc.CreateAttribute("BRANCH");
                    XmlAttribute clCode = xmlDoc.CreateAttribute("CL-CODE");
                    XmlAttribute feeEarner = xmlDoc.CreateAttribute("FEE-EARNER");
                    XmlAttribute dateOpened = xmlDoc.CreateAttribute("DATE-OPENED");
                    XmlAttribute stCode = xmlDoc.CreateAttribute("ST-CODE");
                    XmlAttribute feName = xmlDoc.CreateAttribute("NAME");
                    XmlAttribute feDept = xmlDoc.CreateAttribute("DEPARTMENT");

                    // assign values from result set reader
                    branch.Value = reader["BRANCH"].ToString();
                    clCode.Value = reader["CL-CODE"].ToString();
                    feeEarner.Value = reader["FEE-EARNER"].ToString();
                    dateOpened.Value = reader["DATE-OPENED"].ToString();
                    stCode.Value = reader["ST-CODE"].ToString();
                    feName.Value = reader["NAME"].ToString();
                    feDept.Value = reader["DEPARTMENT"].ToString();

                    // assign attributes to soldocNode
                    sosMatterNode.Attributes.Append(branch);
                    sosMatterNode.Attributes.Append(clCode);
                    sosMatterNode.Attributes.Append(feeEarner);
                    sosMatterNode.Attributes.Append(dateOpened);
                    sosMatterNode.Attributes.Append(stCode);
                    sosMatterNode.Attributes.Append(feName);
                    sosMatterNode.Attributes.Append(feDept);

                    // append node to matter
                    matterNode.AppendChild(sosMatterNode);
                }

                reader.Close();
                conn.Close();

                return xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SOSOpenMatters");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = e.Message.ToString();
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }

        }
    
    }
}
