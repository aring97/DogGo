using System.Collections.Generic;
using System.Linq;
using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class OwnerRepository: IOwnerRepository
    {
        private readonly IConfiguration _config;
        public OwnerRepository(IConfiguration config)
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
        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT o.Id AS ownerId, o.Name AS ownerName, Email, Address, Phone, n.Name AS NeighborName, n.Id AS NeighborId FROM Owner o LEFT JOIN Neighborhood n ON o.NeighborhoodId=n.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("ownerName")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("NeighborId")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborName"))
                            },
                            Phone = reader.GetString(reader.GetOrdinal("Phone"))
                        };
                        owners.Add(owner);
                    }
                    reader.Close();
                    return owners;
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using(SqlConnection conn= Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id AS ownerId, o.Name AS ownerName, Email, Address, Phone, n.Name AS NeighborName, n.Id AS NeighborId FROM Owner o LEFT JOIN Neighborhood n ON o.NeighborhoodId=n.Id WHERE o.Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("ownerName")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("NeighborId")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborName"))
                            },
                            Phone = reader.GetString(reader.GetOrdinal("Phone"))
                        };
                        reader.Close();
                        return owner;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }
    }
}
