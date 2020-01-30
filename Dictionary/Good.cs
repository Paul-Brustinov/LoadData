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
    public class Good
    {
        public int GoodsItemId;
        public string GoodsItemName;
        public int GoodsItemType;
        public int IsClosed;
        public int UOMId;
        public int TopGoodsCategoryId;
        public int GoodsCategoryId;
        public int AnalyticCode;




        public Good()
        {

        }
        public Good(string goodsItemId, string _goodsItemName, string _goodsItemType,
                    string _isClosed, string _uOMId, string _topGoodsCategoryId,
                    string _goodsCategoryId, string _analyticCode)
        {
            Int32.TryParse(goodsItemId, out GoodsItemId);
            GoodsItemName = _goodsItemName;
            Int32.TryParse(_goodsItemType, out GoodsItemType);

            Int32.TryParse(_isClosed, out IsClosed);
            Int32.TryParse(_uOMId, out UOMId);
            Int32.TryParse(_topGoodsCategoryId, out TopGoodsCategoryId);

            Int32.TryParse(_goodsCategoryId, out GoodsCategoryId);
            Int32.TryParse(_analyticCode, out AnalyticCode);
        }

        public static void ImportGoods(JArray GoodsObject, SqlConnection conn)
        {
            foreach (JObject item in GoodsObject)
            {
                //int i = (int)item.Property("_GoodsItemId").Value;
                var good = new Good(
                    (string)item["_GoodsItemId"],
                    (string)item["_GoodsItemName"],
                    (string)item["_GoodsItemType"],
                    (string)item["_IsClosed"],
                    (string)item["__UOM"]["_UOMId"],
                    (string)item["_GoodsCategory"]["_TopGoodsCategoryId"],
                    (string)item["_GoodsCategory"]["_GoodsCategoryId"],
                    (string)item["_AnalyticCode"]
                    );
                using (var command = new SqlCommand("_MhpImport_Goods_do", conn)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    command.Parameters.Add("@_GoodsItemId", SqlDbType.Int).Value = good.GoodsItemId;
                    command.Parameters.Add("@_GoodsItemName", SqlDbType.NVarChar).Value = good.GoodsItemName;
                    command.Parameters.Add("@_GoodsItemType", SqlDbType.Int).Value = good.GoodsItemType;
                    command.Parameters.Add("@_IsClosed", SqlDbType.Bit).Value = good.IsClosed;
                    command.Parameters.Add("@_UOMId", SqlDbType.Int).Value = good.UOMId;
                    command.Parameters.Add("@_TopGoodsCategoryId", SqlDbType.Int).Value = good.TopGoodsCategoryId;
                    command.Parameters.Add("@_GoodsCategoryId", SqlDbType.Int).Value = good.GoodsCategoryId;
                    command.Parameters.Add("@_AnalyticCode", SqlDbType.Int).Value = good.AnalyticCode;

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
