using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using LoadData.Common;

namespace LoadData.Dictionary
{
    class LoadDictionary
    {
        static void Main(string[] args)
        {


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ImpCnn"].ConnectionString;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();

                string GoodsUrl = System.Configuration.ConfigurationManager.AppSettings["GoodsUrl"];
                string GoodsJson = Download.GetTextFromUrl(GoodsUrl);
                GoodsJson = Download.UnBase64(GoodsJson);
                var GoodsObject = JsonConvert.DeserializeObject<JArray>(GoodsJson);

                Good.ImportGoods(GoodsObject, conn);

                string GroupsUrl = System.Configuration.ConfigurationManager.AppSettings["GroupsUrl"];
                string GroupsJson = Download.GetTextFromUrl(GroupsUrl);
                GroupsJson = Download.UnBase64(GroupsJson);

                var GroupsObject = JsonConvert.DeserializeObject<JArray>(GroupsJson);
                Group.ImportGroups(GroupsObject, conn);
            }

            //Console.WriteLine(GroupsJson);

            //conn.Close();
            Console.WriteLine("Импорт успешно закончен");
            //Console.ReadLine();
        }
    }
}

