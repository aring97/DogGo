using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DogGo.Models;

namespace DogGo.Repositories
{
    public class WalksRepository: IWalksRepository 
    {
        private readonly IConfiguration _config;
        public WalksRepository(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public List<Walks> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn= Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM Walks WHERE WalkerId=@id";
                    cmd.Parameters.AddWithValue("@id", walkerId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Walks> walks= new List<Walks>();
                    while (reader.Read())
                    {
                        Walks walk = new Walks()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId"))
                        };
                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }
    }
}
