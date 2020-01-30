using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Common
{
    public static class Download
    {
        public static string GetTextFromUrl(string Url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            String stringResponse = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                stringResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            return stringResponse;
        }

        public static string UnBase64(string stringResponse)
        { 
            string str = stringResponse.Substring(1, stringResponse.Length - 2).Replace(@"\", "");
            byte[] data;
            using (var ms = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var decompressed = new MemoryStream())
                    {
                        gzip.CopyTo(decompressed);
                        data = decompressed.ToArray();
                    }
                }
            }
            return Encoding.UTF8.GetString(data);
        }

        public static string GetTextFromUrlDummy(string Url)
        {

            return System.IO.File.ReadAllText(@"D:\_WORK\_МХП\MХП_ДОК\2.json");
            //return @"{""arr"": [{
            //                ""GoodsCategoryFullPath"": null,
            //                ""GoodsItemTypeDescription"": ""Товар франшизы"",
            //                ""_AnalyticCode"": ""157"",
            //                ""_AnalyticCode2"": null,
            //                ""_AnalyticCode3"": null,
            //                ""_Barcode"": null,
            //                ""_Comment"": """",
            //                ""_GoodsCategory"": {
            //                    ""_CanPayByBonus"": false,
            //                    ""_GoodsCategoryFullPath"": null,
            //                    ""_GoodsCategoryId"": 448,
            //                    ""_GoodsCategoryName"": ""м'ясо Курка охолоджена"",
            //                    ""_MacroGroupId"": null,
            //                    ""_PointsGeneratePercent"": 0,
            //                    ""_TopGoodsCategoryId"": 400
            //                },
            //                ""_GoodsItemId"": 718,
            //                ""_GoodsItemName"": ""Стегно куряче охол."",
            //                ""_GoodsItemType"": 1,
            //                ""_IsClosed"": false,
            //                ""_Price"": 0,
            //                ""_SertificateExpirationDate"": null,
            //                ""_SertificateNumber"": null,
            //                ""_VATGroup"": 0,
            //                ""__UOM"": {
            //                    ""_UOMId"": 2,
            //                    ""_UOMName"": ""кг""
            //                    }
            //                }]}";
        }
    }
}
