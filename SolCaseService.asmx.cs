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
        public XmlElement getSOSNewIndividualClientsToday()
        {
          
    string sql = @"Select DISTINCT
    'Birkett Long LLP' AS ""DIVISION"",
    'Individual' AS ""COMPANY-OR-INDIVIDUAL"",
    PUB.CLIDB.""A-FORENAME"" As ""FIRST-NAME"",
    PUB.CLIDB.""A-NAME"" As ""LAST-NAME"",
    PUB.CLIDB.""A-TITLE"" As ""SALUTATION"",
    PUB.CLIDB.""A-ADDRESS1"" As ""PHYSICAL-ADDR-1"",
    PUB.CLIDB.""A-ADDRESS2"" As ""PHYSICAL-ADDR-2"",
    PUB.CLIDB.""A-ADDRESS3"" As ""PHYSICAL-ADDR-CITY"",
    PUB.CLIDB.""A-ADDRESS4"" As ""PHYSICAL-ADDR-STATE"",
    PUB.CLIDB.""A-POSTCODE"" As ""PHYSICAL-ADDR-POSTCODE"",
    'UK' As ""PHYSICAL-ADDR-COUNTRY"",
    PUB.CLIDB.""A-ADDRESS1"" As ""POSTAL-ADDR-1"",
    PUB.CLIDB.""A-ADDRESS2"" As ""POSTAL-ADDR-2"",
    PUB.CLIDB.""A-ADDRESS3"" As ""POSTAL-ADDR-CITY"",
    PUB.CLIDB.""A-ADDRESS4"" As ""POSTAL-ADDR-STATE"",
    PUB.CLIDB.""A-POSTCODE"" As ""POSTAL-ADDR-POSTCODE"",    
    'UK' As ""POSTAL-ADDR-COUNTRY"",
    case 
      when locate('(',PUB.CLIDB.""TELEPHONE1"") > 0 then left(PUB.CLIDB.""TELEPHONE1"", locate('(', PUB.CLIDB.""TELEPHONE1"") - 1)
      when locate('/',PUB.CLIDB.""TELEPHONE1"") > 0 then left(PUB.CLIDB.""TELEPHONE1"", locate('/', PUB.CLIDB.""TELEPHONE1"") - 1)
      when locate('-',PUB.CLIDB.""TELEPHONE1"") > 0 then left(PUB.CLIDB.""TELEPHONE1"", locate('-', PUB.CLIDB.""TELEPHONE1"") - 1) 
      else PUB.CLIDB.""TELEPHONE1""
      end As ""PHONE-1-NUMBER"",
    case 
      when left(PUB.CLIDB.""TELEPHONE1"", 2) = '07' then 'Mobile'
      else 'Home'
      end As ""PHONE-1-DESCRIPTION"",    
    case 
      when locate('(',PUB.CLIDB.""TELEPHONE1"") > 0 then substr(PUB.CLIDB.""TELEPHONE1"", locate('(', PUB.CLIDB.""TELEPHONE1""))
      when locate('-',PUB.CLIDB.""TELEPHONE1"") > 0 then substr(PUB.CLIDB.""TELEPHONE1"", locate('-', PUB.CLIDB.""TELEPHONE1"") + 1) 
      else ''
      end As ""PHONE-1-NOTES"",
    case 
      when left(PUB.CLIDB.""TELEPHONE2"", 2) = '07' then 'Mobile'
      else 'Home'
      end As ""PHONE-2-DESCRIPTION"",    
    case 
      when locate('(',PUB.CLIDB.""TELEPHONE2"") > 0 then left(PUB.CLIDB.""TELEPHONE2"", locate('(', PUB.CLIDB.""TELEPHONE2"") - 1)
      when locate('/',PUB.CLIDB.""TELEPHONE1"") > 0 then substr(PUB.CLIDB.""TELEPHONE1"", locate('/', PUB.CLIDB.""TELEPHONE1"") + 1) 
      when locate('-',PUB.CLIDB.""TELEPHONE2"") > 0 then right(PUB.CLIDB.""TELEPHONE2"", locate('-', PUB.CLIDB.""TELEPHONE1""))   
      else PUB.CLIDB.""TELEPHONE2""
      end As ""PHONE-2-NUMBER"",
    case 
      when locate('(',PUB.CLIDB.""TELEPHONE2"") > 0 then substr(PUB.CLIDB.""TELEPHONE2"", locate('(', PUB.CLIDB.""TELEPHONE2"")) 
      when locate('-',PUB.CLIDB.""TELEPHONE2"") > 0 then substr(PUB.CLIDB.""TELEPHONE2"", locate('-', PUB.CLIDB.""TELEPHONE2"") + 1)
      else ''
      end As ""PHONE-2-NOTES"",
    to_char(PUB.CLIDB.""A-DOB"", 'YYYY-MM-DD') As ""DOB"",
    PUB.CLIDB.GENDER as ""GENDER"",
    PUB.CLIDB.""CL-CODE"" As ""IMPORT-REFERENCE"",
    'Client' as ""PARTICIPANT-TYPE"",
    PUB.MATDB.""MT-CODE"", 
    PUB.MATDB.""MT-TYPE"", 
    PUB.MATTYPE.DESCRIPTION as ""MT-TYPE-DESC"", 
    PUB.MATDB.""DATE-OPENED"" as ""MT-DATE-OPENED"", 
    PUB.MATDB.DESCRIPTION as ""MT-DESC"", 
    PUB.MATDB.""DATE-CLOSED"" as ""MT-DATE-CLOSE"", 
    PUB.CLIDB.""DATE-OPENED"" as ""CL-DATE-OPENED"" , 
    PUB.CLIDB.""MOD-DATE"" as ""CL-MOD-DATE"", 
    PUB.CLIDB.""MOD-TIME"" As ""CL-MOD-TIME"", 
    PUB.CLIDB.""RISK-FLAG""
From
    PUB.CLIDB, PUB.MATDB , PUB.MATTYPE
Where
    PUB.CLIDB.""CL-CODE"" = PUB.MATDB.""CL-CODE"" and
    PUB.MATDB.""MT-TYPE"" = PUB.MATTYPE.""MT-TYPE"" and
    --PUB.CLIDB.""DATE-OPENED"" = curdate() and
    PUB.CLIDB.""DATE-OPENED"" = TIMESTAMPADD(SQL_TSI_DAY,-2,curdate()) and
    PUB.CLIDB.""TYPE"" = 'P'
Order By
    PUB.CLIDB.""CL-CODE"" Desc
With(NOLOCK)"
;
            OdbcConnection conn = null;
            OdbcDataReader reader = null;

            try
            {
                // open connection
                conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["SOSLivex64"].ToString());
                conn.Open();
                // execute the SQL
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                //cmd.Parameters.Add("MatterIdentifier", OdbcType.VarChar).Value = inputMatterCode; // "example 081389-000002"
                reader = cmd.ExecuteReader();

                //Console.WriteLine("Database = {0} \nDriver = {1} \nQuery {2}\nConnection String = {3}\nServer Version = {4}\nDataSource = {5}",
                //conn.Database, conn.Driver, cmd.CommandText, conn.ConnectionString, conn.ServerVersion, conn.DataSource);

                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("SOSNewClients");
                XmlAttribute recCount = xmlDoc.CreateAttribute("HAS-RECORDS");
                recCount.Value = reader.HasRows.ToString();
                rootNode.Attributes.Append(recCount);
                xmlDoc.AppendChild(rootNode);

                while (reader.Read())
                {
                    XmlNode soldocNode = xmlDoc.CreateElement("IndividualClient");
                    XmlAttribute importReference = xmlDoc.CreateAttribute("IMPORT-REFERENCE");
                    XmlAttribute companyOrIndividual = xmlDoc.CreateAttribute("COMPANY-OR-INDIVIDUAL");
                    XmlAttribute firstName = xmlDoc.CreateAttribute("FIRST-NAME");
                    XmlAttribute lastName = xmlDoc.CreateAttribute("LAST-NAME");
                    XmlAttribute salutation = xmlDoc.CreateAttribute("SALUTATION");
                    XmlAttribute physicalAddr1 = xmlDoc.CreateAttribute("PHYSICAL-ADDR-1");
                    XmlAttribute physicalAddr2 = xmlDoc.CreateAttribute("PHYSICAL-ADDR-2");
                    XmlAttribute physicalAddrCity = xmlDoc.CreateAttribute("PHYSICAL-ADDR-CITY");
                    XmlAttribute physicalAddrState = xmlDoc.CreateAttribute("PHYSICAL-ADDR-STATE");
                    XmlAttribute physicalAddrPostCode = xmlDoc.CreateAttribute("PHYSICAL-ADDR-POSTCODE");
                    XmlAttribute physicalAddrCountry = xmlDoc.CreateAttribute("PHYSICAL-ADDR-COUNTRY");
                    XmlAttribute postalAddr1 = xmlDoc.CreateAttribute("POSTAL-ADDR-1");
                    XmlAttribute postalAddr2 = xmlDoc.CreateAttribute("POSTAL-ADDR-2");
                    XmlAttribute postalAddrCity = xmlDoc.CreateAttribute("POSTAL-ADDR-CITY");
                    XmlAttribute postalAddrState = xmlDoc.CreateAttribute("POSTAL-ADDR-STATE");
                    XmlAttribute postalAddrPostCode = xmlDoc.CreateAttribute("POSTAL-ADDR-POSTCODE");
                    XmlAttribute postalAddrCountry = xmlDoc.CreateAttribute("POSTAL-ADDR-COUNTRY");
                    XmlAttribute phone1Number = xmlDoc.CreateAttribute("PHONE-1-NUMBER");
                    XmlAttribute phone1Description = xmlDoc.CreateAttribute("PHONE-1-DESCRIPTION");
                    XmlAttribute phone1Notes = xmlDoc.CreateAttribute("PHONE-1-NOTES");
                    XmlAttribute phone2Number = xmlDoc.CreateAttribute("PHONE-2-NUMBER");
                    XmlAttribute phone2Description = xmlDoc.CreateAttribute("PHONE-2-DESCRIPTION");
                    XmlAttribute phone2Notes = xmlDoc.CreateAttribute("PHONE-2-NOTES");
                    XmlAttribute dob = xmlDoc.CreateAttribute("DOB");
                    
                    XmlAttribute gender = xmlDoc.CreateAttribute("GENDER");
                    XmlAttribute participantType = xmlDoc.CreateAttribute("PARTICIPANT-TYPE");
                    XmlAttribute matterCode = xmlDoc.CreateAttribute("MT-CODE");
                    XmlAttribute matterDesc = xmlDoc.CreateAttribute("MT-DESC");
                    XmlAttribute matterType = xmlDoc.CreateAttribute("MT-TYPE");
                    XmlAttribute matterTypeDescription = xmlDoc.CreateAttribute("MT-TYPE-DESC");
                    XmlAttribute matterOpenDate = xmlDoc.CreateAttribute("MT-DATE-OPENED");
                    XmlAttribute matterDateClosed = xmlDoc.CreateAttribute("MT-DATE-CLOSE");
                    XmlAttribute clientOpenDate = xmlDoc.CreateAttribute("CL-DATE-OPENED");
                    XmlAttribute clientModDate = xmlDoc.CreateAttribute("CL-MOD-DATE");
                    XmlAttribute clientModTime = xmlDoc.CreateAttribute("CL-MOD-TIME");
                    XmlAttribute riskFlag = xmlDoc.CreateAttribute("RISK-FLAG");

                    // assign values from result set reader
                    companyOrIndividual.Value = reader["COMPANY-OR-INDIVIDUAL"].ToString();
                    firstName.Value = reader["FIRST-NAME"].ToString();
                    lastName.Value = reader["LAST-NAME"].ToString();
                    salutation.Value = reader["SALUTATION"].ToString();
                    physicalAddr1.Value = reader["PHYSICAL-ADDR-1"].ToString();
                    physicalAddr2.Value = reader["PHYSICAL-ADDR-2"].ToString();
                    physicalAddrCity.Value = reader["PHYSICAL-ADDR-CITY"].ToString();
                    physicalAddrState.Value = reader["PHYSICAL-ADDR-STATE"].ToString();
                    physicalAddrPostCode.Value = reader["PHYSICAL-ADDR-POSTCODE"].ToString();
                    physicalAddrCountry.Value = reader["PHYSICAL-ADDR-COUNTRY"].ToString();
                    postalAddr1.Value = reader["POSTAL-ADDR-1"].ToString();
                    postalAddr2.Value = reader["POSTAL-ADDR-2"].ToString();
                    postalAddrCity.Value = reader["POSTAL-ADDR-CITY"].ToString();
                    postalAddrState.Value = reader["POSTAL-ADDR-STATE"].ToString();
                    postalAddrPostCode.Value = reader["POSTAL-ADDR-POSTCODE"].ToString();
                    postalAddrCountry.Value = reader["POSTAL-ADDR-COUNTRY"].ToString();
                    phone1Number.Value = reader["PHONE-1-NUMBER"].ToString();
                    phone1Description.Value = reader["PHONE-1-DESCRIPTION"].ToString();
                    phone1Notes.Value = reader["PHONE-1-NOTES"].ToString();
                    phone2Number.Value = reader["PHONE-2-NUMBER"].ToString();
                    phone2Description.Value = reader["PHONE-2-DESCRIPTION"].ToString();
                    phone2Notes.Value = reader["PHONE-2-NOTES"].ToString();
                    dob.Value = reader["DOB"].ToString();
                    
                    gender.Value = reader["GENDER"].ToString();
                    participantType.Value = reader["PARTICIPANT-TYPE"].ToString();
                    matterCode.Value = reader["MT-CODE"].ToString();
                    matterDesc.Value = reader["MT-DESC"].ToString();
                    matterType.Value = reader["MT-TYPE"].ToString();
                    matterTypeDescription.Value = reader["MT-TYPE-DESC"].ToString();
                    matterOpenDate.Value = reader["MT-DATE-OPENED"].ToString();
                    matterDateClosed.Value = reader["MT-DATE-CLOSE"].ToString();
                    clientOpenDate.Value = reader["CL-DATE-OPENED"].ToString();
                    clientModDate.Value = reader["CL-MOD-DATE"].ToString();
                    clientModTime.Value = reader["CL-MOD-TIME"].ToString();
                    riskFlag.Value = reader["RISK-FLAG"].ToString();

                    // assign attributes to soldocNode
                    soldocNode.Attributes.Append(companyOrIndividual);
                    soldocNode.Attributes.Append(firstName);
                    soldocNode.Attributes.Append(lastName);
                    soldocNode.Attributes.Append(salutation);
                    soldocNode.Attributes.Append(physicalAddr1);
                    soldocNode.Attributes.Append(physicalAddr2);
                    soldocNode.Attributes.Append(physicalAddrCity);
                    soldocNode.Attributes.Append(physicalAddrState);
                    soldocNode.Attributes.Append(physicalAddrPostCode);
                    soldocNode.Attributes.Append(physicalAddrCountry);
                    soldocNode.Attributes.Append(postalAddr1);
                    soldocNode.Attributes.Append(postalAddr2);
                    soldocNode.Attributes.Append(postalAddrCity);
                    soldocNode.Attributes.Append(postalAddrState);
                    soldocNode.Attributes.Append(postalAddrPostCode);
                    soldocNode.Attributes.Append(postalAddrCountry);
                    soldocNode.Attributes.Append(phone1Number);
                    soldocNode.Attributes.Append(phone1Description);
                    soldocNode.Attributes.Append(phone1Notes);
                    soldocNode.Attributes.Append(phone2Number);
                    soldocNode.Attributes.Append(phone2Description);
                    soldocNode.Attributes.Append(phone2Notes);
                    soldocNode.Attributes.Append(dob);
                    
                    soldocNode.Attributes.Append(gender);
                    soldocNode.Attributes.Append(participantType);
                    soldocNode.Attributes.Append(matterCode);
                    soldocNode.Attributes.Append(matterDesc);
                    soldocNode.Attributes.Append(matterType);
                    soldocNode.Attributes.Append(matterTypeDescription);
                    soldocNode.Attributes.Append(matterOpenDate);
                    soldocNode.Attributes.Append(matterDateClosed);
                    soldocNode.Attributes.Append(clientOpenDate);
                    soldocNode.Attributes.Append(clientModDate);
                    soldocNode.Attributes.Append(clientModTime);
                    soldocNode.Attributes.Append(riskFlag);

                    // append node to matter
                    rootNode.AppendChild(soldocNode);
                }

                reader.Close();
                conn.Close();

                return xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SOSNewClients");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = e.Message.ToString();
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }
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
                        matterNode = xmlDoc.CreateElement("Matter");
                        matterCode = xmlDoc.CreateAttribute("MT-CODE");
                        matterDesc = xmlDoc.CreateAttribute("MAT-DESCRIPTION");

                        // the matter code has changed, create a new Matter Node                       
                        matterCode.Value = reader["MT-CODE"].ToString();
                        matterNode.Attributes.Append(matterCode);

                        matterDesc.Value = reader["MAT-DESCRIPTION"].ToString();
                        matterNode.Attributes.Append(matterDesc);
                        clientNode.AppendChild(matterNode);

                        // reset the current matter to the latest, ready for next change, when that happens
                        currentMatterCode = reader["MT-CODE"].ToString();
                    }

                    XmlNode soldocNode = xmlDoc.CreateElement("SolDoc");
                    XmlAttribute mtCode = xmlDoc.CreateAttribute("MT-CODE");
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
                    mtCode.Value = reader["MT-CODE"].ToString();
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
                    soldocNode.Attributes.Append(mtCode);
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
                XmlNode rootNode = xmlDocError.CreateElement("SOSOpenMatters");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = e.Message.ToString();
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }
        }

        [WebMethod]
        public XmlElement nD_Import(string matterCodeInput)
        {
            String inputMatterCode = matterCodeInput;

            Regex rgxClient = new Regex(@"[0-9]{6}-[0-9]{6}");

            if (!rgxClient.IsMatch(inputMatterCode))
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseScreens");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = "Matter Code must be a 6 digit number, hyphen then 6 digit number";
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
    PUB.HISTORY.DESCRIPTION As ""HST-DESCRIPTION"",
    PUB.HISTORY.""LEVEL-FEE-EARNER"",
    PUB.HISTORY.""DOCUMENT-NAME"" As ""HST-DOCUMENT-NAME"",
    PUB.HISTORY.""DOCUMENT-TYPE"" As ""HST-DOCUMENT-TYPE"",
    PUB.HISTORY.""HISTORY-NO"",
    to_char(PUB.HISTORY.""DATE-INSERTED"",'DD-MM-YYYY') As ""HST-DATE-INSERTED"",
    PUB.""HISTORY-DOCS"".""DOCUMENT-NO"" As ""HD-DOCUMENT-NO"",
    PUB.""HISTORY-DOCS"".""ORIG-DOC-NAME"" As ""HD-ORIG-DOC-NAME"",
    PUB.""HISTORY-DOCS"".""DOCUMENT-NAME"" As ""HD-DOCUMENT-NAME"",
    PUB.""HISTORY-DOCS"".AUTHOR As ""HD-AUTHOR"",
    PUB.""HISTORY-DOCS"".""DOCUMENT-TYPE"" As ""HD-DOCUMENT-TYPE"",
    PUB.""HISTORY-DOCS"".""FINAL-VERSION"" As ""HD-FINAL-VERSION"",
    PUB.""HISTORY-DOCS"".""DATE-AMENDED"" As ""HD-DATE-AMENDED"",
    PUB.""HISTORY-DOCS"".""AMENDED-BY"" As ""HD-AMENDED-BY"",
    PUB.""HISTORY-DOCS"".""ACTION"" As ""HD-ACTION"",
    PUB.""HISTORY-DOCS"".""DATE-CREATED"" As ""HD-DATE-CREATED"",
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
    PUB.""HISTORY-DOCS"" On PUB.""HISTORY-DOCS"".""MT-CODE"" = PUB.HISTORY.""MT-CODE""
            And PUB.""HISTORY-DOCS"".""ORIG-DOC-NAME"" = PUB.HISTORY.""DOCUMENT-NAME"" Inner Join
    PUB.""DOC-CONTROL"" On PUB.""DOC-CONTROL"".""DOC-ID"" = PUB.HISTORY.""DOCUMENT-NAME"" Inner Join
    PUB.""FILE-LOCATION"" On PUB.""FILE-LOCATION"".""LOC-NAME"" = PUB.""DOC-CONTROL"".""ST-LOCATION""
Where
    PUB.MATDB.""MT-CODE"" = ?
order by 
    PUB.HISTORY.DESCRIPTION, PUB.""HISTORY-DOCS"".""DOCUMENT-NO"" asc
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
                XmlAttribute clientCode = xmlDoc.CreateAttribute("CL-CODE");;

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

                    XmlNode soldocNode = xmlDoc.CreateElement("ndImport");
                    XmlAttribute histDesc = xmlDoc.CreateAttribute("HST-DESCRIPTION");
                    XmlAttribute histNo = xmlDoc.CreateAttribute("HISTORY-NO");
                    XmlAttribute docName = xmlDoc.CreateAttribute("HST-DOCUMENT-NAME");
                    XmlAttribute solcaseDocType = xmlDoc.CreateAttribute("HST-DOCUMENT-TYPE");
                    XmlAttribute dateInserted = xmlDoc.CreateAttribute("HST-DATE-INSERTED");
                    XmlAttribute histFE = xmlDoc.CreateAttribute("LEVEL-FEE-EARNER");

                    XmlAttribute hdDocNo = xmlDoc.CreateAttribute("HD-DOCUMENT-NO");
                    XmlAttribute hdOrigDocName = xmlDoc.CreateAttribute("HD-ORIG-DOC-NAME");
                    XmlAttribute hdDocName = xmlDoc.CreateAttribute("HD-DOCUMENT-NAME");
                    XmlAttribute hdAuthor = xmlDoc.CreateAttribute("HD-AUTHOR");
                    XmlAttribute hdDocType = xmlDoc.CreateAttribute("HD-DOCUMENT-TYPE");
                    XmlAttribute hdFinalVersion = xmlDoc.CreateAttribute("HD-FINAL-VERSION");
                    XmlAttribute hdDateAmended = xmlDoc.CreateAttribute("HD-DATE-AMENDED");
                    XmlAttribute hdAmendedBy = xmlDoc.CreateAttribute("HD-AMENDED-BY");
                    XmlAttribute hdAction = xmlDoc.CreateAttribute("HD-ACTION");
                    XmlAttribute hdDateCreated = xmlDoc.CreateAttribute("HD-DATE-CREATED");

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
                    // watch out for commas
                    histDesc.Value = reader["HST-DESCRIPTION"].ToString().Replace(" & ", " &amp; ").Replace(","," ");
                    docName.Value = reader["HST-DOCUMENT-NAME"].ToString();
                    solcaseDocType.Value = reader["HST-DOCUMENT-TYPE"].ToString();
                    dateInserted.Value = reader["HST-DATE-INSERTED"].ToString();
                    histFE.Value = reader["LEVEL-FEE-EARNER"].ToString();

                    // you need to add 1 to the doc no as Net Docs import Version starts at 1
                    var ndVersion = 0;
                    ndVersion = Convert.ToInt32(reader["HD-DOCUMENT-NO"]);
                    ndVersion = ndVersion + 1;
                    hdDocNo.Value = ndVersion.ToString();
                    hdOrigDocName.Value = reader["HD-ORIG-DOC-NAME"].ToString();
                    hdDocName.Value = reader["HD-DOCUMENT-NAME"].ToString();
                    hdAuthor.Value = reader["HD-AUTHOR"].ToString();
                    hdDocType.Value = reader["HD-DOCUMENT-TYPE"].ToString();
                    hdFinalVersion.Value = reader["HD-FINAL-VERSION"].ToString();
                    hdDateAmended.Value = reader["HD-DATE-AMENDED"].ToString();
                    hdAmendedBy.Value = reader["HD-AMENDED-BY"].ToString();
                    hdAction.Value = reader["HD-ACTION"].ToString();
                    hdDateCreated.Value = reader["HD-DATE-CREATED"].ToString();

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

                    soldocNode.Attributes.Append(hdDocNo);
                    soldocNode.Attributes.Append(hdOrigDocName);
                    soldocNode.Attributes.Append(hdDocName);
                    soldocNode.Attributes.Append(hdAuthor);
                    soldocNode.Attributes.Append(hdDocType);
                    soldocNode.Attributes.Append(hdFinalVersion);
                    soldocNode.Attributes.Append(hdDateAmended);
                    soldocNode.Attributes.Append(hdAmendedBy);
                    soldocNode.Attributes.Append(hdDateCreated);

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
                XmlNode rootNode = xmlDocError.CreateElement("SOSOpenMatters");
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

        [WebMethod]
        public XmlElement getV3PRListUDScreens(string matterCodeInput)
        {
            String inputMatterCode = matterCodeInput;

            Regex rgxClient = new Regex(@"[A-Z,0-9]{6}-[A-Z,0-9]{6}");

            if (!rgxClient.IsMatch(inputMatterCode))
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseScreens");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = "Matter Code must be a 6 digit number, hyphen then 6 digit number";
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }

            string sql = @"Select
    X.""DATE-OPENED"",
    X.""LEVEL-FEE-EARNER"",
    X.""MT-CODE"",
    X.""CL-CODE"",
    X.DESCRIPTION,
    X.""MT-TYPE"",
    PUB.UDDETAIL.""UD-FIELD"" As ""UD-FIELD"",
    PUB.UDDETAIL.""GROUP-NO"" As ""GROUP-NO"",
    PUB.UDDETAIL.""UDS-TYPE"" As ""UDS-TYPE"",
    PUB.UDDETAIL.""MT-TYPE"" As ""UDS-MTTYPE"",
    PUB.UDSCREEN.DESCRIPTION As ""SCREEN-NAME"",
    PUB.UDSCREEN.""FIELD-DESC"" As ""FIELD-DESC"",
    PUB.UDSCREEN.""ENTRY-ORDER"" As ""FIELD-ORDER""
From
    PUB.MATDB X,
    PUB.UDDETAIL,
    PUB.UDSCREEN
Where
    PUB.UDDETAIL.""OWNER-CODE"" = X.""MT-CODE"" And
    PUB.UDSCREEN.""UDS-TYPE"" = PUB.UDDETAIL.""UDS-TYPE"" And
    PUB.UDSCREEN.""MT-TYPE"" = X.""MT-TYPE"" And
    X.""MT-CODE"" = ? And
    PUB.UDDETAIL.""UDS-TYPE"" in (Select PUB.UDSCREEN.""UDS-TYPE"" from PUB.UDSCREEN where PUB.UDSCREEN.""MT-TYPE"" = X.""MT-TYPE"")
Order By
  X.""MT-CODE"", PUB.UDDETAIL.""UDS-TYPE"", PUB.UDDETAIL.""GROUP-NO"" Asc
WITH(NOLOCK)";

            OdbcConnection conn = null;
            OdbcDataReader reader = null;

            try
            {
                // open connection
                conn = new OdbcConnection(ConfigurationManager.ConnectionStrings["SolCaseLivex64"].ToString());
                conn.Open();
                // execute the SQL
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.Parameters.Add("MatterCode", OdbcType.VarChar).Value = inputMatterCode; // "example V3PR"
                reader = cmd.ExecuteReader();

                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("SolcaseUDScreens");
                xmlDoc.AppendChild(rootNode);

                XmlNode matterNode = xmlDoc.CreateElement("Matter");
                XmlAttribute matterCode = xmlDoc.CreateAttribute("MT-CODE");
                XmlAttribute matterType = xmlDoc.CreateAttribute("MT-TYPE");
                XmlAttribute matterDesc = xmlDoc.CreateAttribute("DESCRIPTION");

                Boolean matterNodeNotCreated = true;

                while (reader.Read())
                {
                    if (matterNodeNotCreated)
                    {
                        // only once used, turn off once processed                                               
                        matterCode.Value = reader["MT-CODE"].ToString();
                        matterNode.Attributes.Append(matterCode);

                        matterType.Value = reader["MT-TYPE"].ToString();
                        matterNode.Attributes.Append(matterType);

                        matterDesc.Value = reader["DESCRIPTION"].ToString();
                        matterNode.Attributes.Append(matterDesc);

                        rootNode.AppendChild(matterNode);

                        matterNodeNotCreated = false;
                    }

                    XmlNode udScreensNode = xmlDoc.CreateElement("UDScreenDetails");
                    XmlAttribute UDSfield = xmlDoc.CreateAttribute("UD-FIELD");
                    XmlAttribute GroupNo = xmlDoc.CreateAttribute("GROUP-NO");
                    XmlAttribute UDSType = xmlDoc.CreateAttribute("UDS-TYPE");
                    XmlAttribute ScreenName = xmlDoc.CreateAttribute("SCREEN-NAME");
                    XmlAttribute fieldDesc = xmlDoc.CreateAttribute("FIELD-DESC");
                    XmlAttribute fieldOrder = xmlDoc.CreateAttribute("FIELD-ORDER");

                    // assign values from result set reader
                    UDSfield.Value = reader["UD-FIELD"].ToString();
                    GroupNo.Value = reader["GROUP-NO"].ToString();
                    UDSType.Value = reader["UDS-TYPE"].ToString();
                    ScreenName.Value = reader["SCREEN-NAME"].ToString();
                    fieldDesc.Value = reader["FIELD-DESC"].ToString(); 
                    fieldOrder.Value = reader["FIELD-ORDER"].ToString();

                    // assign attributes to soldocNode
                    udScreensNode.Attributes.Append(UDSType);
                    udScreensNode.Attributes.Append(GroupNo);
                    udScreensNode.Attributes.Append(ScreenName);
                    udScreensNode.Attributes.Append(UDSfield);
                    udScreensNode.Attributes.Append(fieldDesc);
                    udScreensNode.Attributes.Append(fieldOrder);

                    // append node to matter
                    matterNode.AppendChild(udScreensNode);
                }

                reader.Close();
                conn.Close();

                return xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                XmlDocument xmlDocError = new XmlDocument();
                XmlNode rootNode = xmlDocError.CreateElement("SolcaseUDScreens");
                xmlDocError.AppendChild(rootNode);
                XmlNode errorNode = xmlDocError.CreateElement("WebServiceError");
                errorNode.InnerText = e.Message.ToString();
                rootNode.AppendChild(errorNode);
                return xmlDocError.DocumentElement;
            }

        }

    }
}
