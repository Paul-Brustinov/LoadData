using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadData.Documents
{
    public class Entity
    {
        public Guid Guid;
        String Art;
        String Name;

        public Entity(String guid, String art, String name)
        {
            Guid = Guid.ParseExact(guid, "D");
            Art = art;
            Name = name;
        }

        public void Merge(SqlConnection conn)
        {
            using (var command = new SqlCommand("[dbo].[_MhpDocs_EntityMerge]", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = Guid;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                command.Parameters.Add("@Art", SqlDbType.NVarChar).Value = Art;
                command.ExecuteNonQuery();
            }
        }
    }
}
