using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Documents
{
    public class Agent
    {
        public Guid Guid;
        String Name;
        String Code;
        String VatNo;

        public Agent(String guid, String name, String code, String vatNo)
        {
            Guid = Guid.ParseExact(guid, "D");
            Name = name;
            Code = code;
            VatNo = vatNo;
        }

        public void Merge(SqlConnection conn)
        {
            using (var command = new SqlCommand("[dbo].[_MhpDocs_AgentMerge]", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = Guid;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = Code;
                command.Parameters.Add("@VatNo", SqlDbType.NVarChar).Value = VatNo;

                command.ExecuteNonQuery();
            }
        }
        
    }
}
