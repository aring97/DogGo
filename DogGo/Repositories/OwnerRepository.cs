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
                    cmd.CommandText = @"SELECT o.Id AS ownerId, Email, o.Name AS ownerName, 
                                        Address, NeighborhoodId, Phone, 
                                        n.Name AS neighborhoodName FROM Owner o 
                                        LEFT JOIN Neighborhood n 
                                        ON o.neighborhoodId=n.Id";
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
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Neighborhood=new Neighborhood
                            {
                                Id=reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Name=reader.GetString(reader.GetOrdinal("neighborhoodName"))
                            }
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
                    cmd.CommandText = @"SELECT o.Id AS ownerId, Email, o.Name AS ownerName, Address, NeighborhoodId,
                                        Phone, n.Name AS neighborhoodName
                                        FROM Owner o LEFT JOIN Neighborhood n ON 
                                        o.NeighborhoodId= n.Id WHERE o.Id=@id";
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
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId=reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood=new Neighborhood
                            {
                                Id=reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Name=reader.GetString(reader.GetOrdinal("neighborhoodName"))
                            }
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

        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner owner = new Owner()
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        reader.Close();
                        return owner;
                    }

                    reader.Close();
                    return null;
                }
            }
        }

        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.id = id;
                }
            }
        }

        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Owner> GetOwnersSortedByName()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id AS ownerId, Email, o.Name AS ownerName, 
                                        Address, NeighborhoodId, Phone, 
                                        n.Name AS neighborhoodName FROM Owner o 
                                        LEFT JOIN Neighborhood n 
                                        ON o.neighborhoodId=n.Id
                                        ORDER BY ownerName";
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
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Name = reader.GetString(reader.GetOrdinal("neighborhoodName"))
                            }
                        };
                        owners.Add(owner);
                    }
                    reader.Close();
                    return owners;
                }
            }
        }
    }
}
