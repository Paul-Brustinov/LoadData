using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using LoadData.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace LoadData.Documents
{
    class LoadDocuments : System.Configuration.IConfigurationSectionHandler
    {
        static void Main(string[] args) 
        {

            List<string> tokens = System.Configuration.ConfigurationManager.GetSection("tokens") as List<string>;

            String strEndDate = DateTime.Now.ToString("yyyyMMdd");
            String strStartDate = DateTime.Now.AddDays(-4).ToString("yyyyMMdd"); //strEndDate; //

            if (args.Length > 0) strStartDate = args[0];
            if (args.Length > 1) strEndDate = args[1];

            string DocsUrl = System.Configuration.ConfigurationManager.AppSettings["DocsUrl"];
            //string DocsToken = System.Configuration.ConfigurationManager.AppSettings["DocsToken"];
            string Request = DocsUrl + "dateFrom=" + strStartDate + "&dateTo=" + strEndDate; //+ "&" + DocsToken;

            ////Console.WriteLine(Request);


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ImpCnn"].ConnectionString;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();

                string DocsJson = "";

                foreach (var t in tokens)
                {
                    try
                    {
                        DocsJson = Download.GetTextFromUrl(Request + "&token=" + t); //GetTextFromUrl(Request);
                        var DocsObject = JsonConvert.DeserializeObject<JArray>(DocsJson);
                        Document.ImportDocuments(DocsObject, conn);
                        Console.WriteLine($"Загрузка завершена! Загружено {DocsObject.Count().ToString()} документов");
                    }
                    catch(Exception e)
                    {
                        Log.WriteError(new LoadException() { e = e, Extrainfo = string.Format("Token: {0}; Response: {1}", t, DocsJson) });
                        throw (e);

                    }
                }
            }

            ////Agent agent = new Agent("9ceef7b3-4e00-11db-b854-0030482f22a6", "Миронівський хлібопродукт ПрАТ", "25412361", "254123610155");

            ////Console.ReadLine();

        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            List<string> myConfigObject = new List<string>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                foreach (XmlAttribute attrib in childNode.Attributes)
                {
                    myConfigObject.Add(attrib.Value);
                }
            }
            return myConfigObject;
        }
    }
}
