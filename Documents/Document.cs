using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Documents
{
    public class Document
    {
        public Guid Guid;
        public String DocNo;
        public DateTime DocDate;
        public String Name;
        public Decimal DocSum;
        public Agent AgFrom;
        public Agent AgTo;
        public IList<DocRow> DocRows;

        public Document(String guid, String docNo, String docDate, String docName, String docSum, Agent agFrom, Agent agTo)
        {
            Guid = Guid.ParseExact(guid, "D");
            DocNo = docNo;
            DocDate = DateTime.ParseExact(docDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Name = docName;
            DocSum = Decimal.Parse(docSum);
            AgFrom = agFrom;
            AgTo = agTo;
            DocRows = new List<DocRow>();
        }

        public void Merge(SqlConnection conn)
        {
            using (var command = new SqlCommand("[dbo].[_MhpDocs_DocumentMerge]", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add("@Guid"      , SqlDbType.UniqueIdentifier).Value = Guid;
                command.Parameters.Add("@DocNo"     , SqlDbType.NVarChar).Value = DocNo;
                command.Parameters.Add("@DocDate"   , SqlDbType.DateTime).Value = DocDate;
                command.Parameters.Add("@Name"      , SqlDbType.NVarChar).Value = Name;
                command.Parameters.Add("@DocSum"    , SqlDbType.Money).Value = DocSum;
                command.Parameters.Add("@AgFromGuid", SqlDbType.UniqueIdentifier).Value = AgFrom.Guid;
                command.Parameters.Add("@AgToGuid", SqlDbType.UniqueIdentifier).Value = AgTo.Guid;
                
                command.ExecuteNonQuery();
            }
        }

        public static void ImportDocuments(JArray documentsObject, SqlConnection conn)
        {
            foreach (JObject item in documentsObject)
            {
                var jAgFrom = item["Контрагенты"].First(e => e["Роль"].ToString() == "Продавец");
                Agent AgFrom = new Agent(
                    jAgFrom["Ид"].ToString()
                    , jAgFrom["Наименование"].ToString()
                    , jAgFrom["Код"].ToString()
                    , jAgFrom["ИНН"].ToString()
                    );
                AgFrom.Merge(conn);

                var jAgTo = item["Контрагенты"].First(e => e["Роль"].ToString() == "Покупатель");
                Agent AgTo = new Agent(
                    jAgTo["Ид"].ToString()
                    , jAgTo["Наименование"].ToString()
                    , jAgTo["Код"].ToString()
                    , jAgTo["ИНН"].ToString()
                    );
                AgTo.Merge(conn);

                Document document = new Document(
                    item["Ид"].ToString()
                    , item["Номер"].ToString()
                    , item["Дата"].ToString()
                    , item["ХозОперация"].ToString()
                    , item["Сумма"].ToString()
                    , AgFrom
                    , AgTo
                    );

                document.Merge(conn);

                var jDocRows = (JArray)item["Товары"];

                DocRow.ImportDocRows(document.Guid, jDocRows, conn);
                //Console.WriteLine("-----");
            }
        }


    }
}
