using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace COMP2001_API.Models
{
    public class DataAccess
    {
        private readonly string _connection;

        public DataAccess(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("COMP2001_DB");
        }

        public bool Validate(User usr)
        {

            using (SqlConnection sql = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("ValidateUser", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Email", usr.Email));
                    cmd.Parameters.Add(new SqlParameter("@Password", usr.Password));

                    SqlParameter output = new SqlParameter("@Validation", SqlDbType.Int);
                    output.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(output);

                    sql.Open();


                    cmd.ExecuteNonQuery();
                    
                    bool response = (int)output.Value != 0;


                    return response;
                }

            }
        }

        public void Register(User usr, out string responseMessage)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("Register", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Email", usr.Email));
                    cmd.Parameters.Add(new SqlParameter("@Password", usr.Password));
                    cmd.Parameters.Add(new SqlParameter("@FirstName", usr.First_Name));
                    cmd.Parameters.Add(new SqlParameter("@LastName", usr.Last_Name));

                    SqlParameter output = new SqlParameter("@ResponseMessage", SqlDbType.NVarChar, 250);
                    output.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(output);

                    sql.Open();


                    cmd.ExecuteNonQuery();
                    responseMessage = output.Value.ToString();
                }
            }
        }

        public void Update(User usr, int id)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateUser", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        //Send a dbnull if the string is empty
                        cmd.Parameters.Add(new SqlParameter("@Email", string.IsNullOrEmpty(usr.Email) ? (object)DBNull.Value : usr.Email));

                        cmd.Parameters.Add(new SqlParameter("@FirstName", string.IsNullOrEmpty(usr.First_Name) ? (object)DBNull.Value : usr.First_Name));

                        cmd.Parameters.Add(new SqlParameter("@Password", string.IsNullOrEmpty(usr.Password) ? (object)DBNull.Value : usr.Password));

                        cmd.Parameters.Add(new SqlParameter("@LastName", string.IsNullOrEmpty(usr.Last_Name) ? (object)DBNull.Value : usr.Last_Name));

                        cmd.Parameters.Add(new SqlParameter("@id", id));


                        sql.Open();


                        cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteUser", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    sql.Open();

                    cmd.ExecuteNonQuery();

                }
            }
        }

       
    }
}
