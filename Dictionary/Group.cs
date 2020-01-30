using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Dictionary
{
    public class Group
    {
        public int GoodsCategoryId;
        public int TopGoodsCategoryId;
        public string GoodsCategoryName;
        public bool CanPayByBonus;
        public string GoodsCategoryFullPath;
        public int MacroGroupId;
        public decimal PointsGeneratePercent;


        public Group(string goodsCategoryId, string topGoodsCategoryId, string goodsCategoryName, bool canPayByBonus, string goodsCategoryFullPath, string macroGroupId, string pointsGeneratePercent)
        {
            Int32.TryParse(goodsCategoryId, out GoodsCategoryId);
            GoodsCategoryName = goodsCategoryName==null?"": goodsCategoryName;
            Int32.TryParse(topGoodsCategoryId, out TopGoodsCategoryId);
            CanPayByBonus = canPayByBonus;
            GoodsCategoryFullPath = goodsCategoryFullPath;
            Int32.TryParse(macroGroupId, out MacroGroupId);
            Decimal.TryParse(pointsGeneratePercent, out PointsGeneratePercent);
        }

        public Group(){}

        public static void ImportGroups(JArray GroupsObject, SqlConnection conn)
        {
            foreach (JObject item in GroupsObject)
            {
                var group = new Group(
                    (string)item["_GoodsCategoryId"],
                    (string)item["_TopGoodsCategoryId"],
                    (string)item["_GoodsCategoryName"],
                    (bool)item["_CanPayByBonus"],
                    (string)item["_GoodsCategoryFullPath"],
                    (string)item["_MacroGroupId"],
                    (string)item["_PointsGeneratePercent"]
                    );
                using (var command = new SqlCommand("_MhpImport_Groups_do", conn)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    command.Parameters.Add("@_GoodsCategoryId", SqlDbType.Int).Value = group.GoodsCategoryId;
                    command.Parameters.Add("@_TopGoodsCategoryId", SqlDbType.Int).Value = group.TopGoodsCategoryId;
                    command.Parameters.Add("@_GoodsCategoryName", SqlDbType.NVarChar).Value = group.GoodsCategoryName;
                    command.Parameters.Add("@_CanPayByBonus", SqlDbType.Bit).Value = group.CanPayByBonus;
                    command.Parameters.Add("@_GoodsCategoryFullPath", SqlDbType.NVarChar).Value = group.GoodsCategoryFullPath??"";
                    command.Parameters.Add("@_MacroGroupId", SqlDbType.Int).Value = group.MacroGroupId;
                    command.Parameters.Add("@_PointsGeneratePercent", SqlDbType.Int).Value = group.PointsGeneratePercent;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
