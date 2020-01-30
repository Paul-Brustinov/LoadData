using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Documents
{
    public class DocRow
    {
        public Guid DocGuid;
        public int RowNo;
        public Entity Entity;
        public Decimal Qty;
        public Decimal Sum;
        public Decimal SumWithoutNDS;

        public DocRow(Guid docGuid, int rowNo, Entity entity, String qty, String sum, String sumWithNDS)
        {
            DocGuid = docGuid;
            RowNo = rowNo;
            Entity = entity;
            Qty = Decimal.Parse(qty);
            Sum = Decimal.Parse(sum);
            SumWithoutNDS = Decimal.Parse(sumWithNDS);
        }

        public void Merge(SqlConnection conn)
        {
            using (var command = new SqlCommand("[dbo].[_MhpDocs_DocRowMerge]", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add("@DocGuid", SqlDbType.UniqueIdentifier).Value = DocGuid;
                command.Parameters.Add("@RowNo", SqlDbType.Int).Value = RowNo;
                command.Parameters.Add("@EntGuid", SqlDbType.UniqueIdentifier).Value = Entity.Guid;
                command.Parameters.Add("@Qty", SqlDbType.Money).Value = Qty;
                command.Parameters.Add("@Sum", SqlDbType.Money).Value = Sum;
                command.Parameters.Add("@SumWithoutNDS", SqlDbType.Money).Value = SumWithoutNDS;
                command.ExecuteNonQuery();
            }
        }

        public static void ImportDocRows(Guid docGuiid, JArray docRowsObject, SqlConnection conn)
        {
            int i = 1;
            foreach (JObject item in docRowsObject)
            {
                var entity = new Entity(
                    item["Ид"].ToString(),
                    item["Артикул"].ToString(),
                    item["Наименование"].ToString()
                );
                entity.Merge(conn);

                DocRow docRow = new DocRow(
                    docGuiid
                    , i++
                    , entity
                    , item["Количество"].ToString()
                    , item["СуммаСНДС"].ToString()
                    , item["Сумма"].ToString()
                );

                docRow.Merge(conn);
            }
        }
    }
}
