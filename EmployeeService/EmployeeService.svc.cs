using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        }
        public Employee GetEmployeeById(int id)
        {
            string query = "SELECT * FROM Employee WHERE ID = @Id";
            Employee employee = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employee = new Employee()
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Name = reader["Name"].ToString(),
                                    ManagerID = reader["ManagerID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ManagerID"]),
                                    Enable = Convert.ToBoolean(reader["Enable"])
                                };

                            }
                        }
                    }
                    if (employee == null)
                    {
                        throw new KeyNotFoundException(id.ToString());
                    }
                    employee.Employees = GetSubordinates(id);
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch
            {
                throw;
            }

            return employee;
        }

        private List<Employee> GetSubordinates(int managerId)
        {
            List<Employee> subordinates = new List<Employee>();
            string query = "SELECT * FROM Employee WHERE ManagerID = @ManagerId AND ID != @ManagerId";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ManagerId", managerId);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Employee subordinate = new Employee
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Name = reader["Name"].ToString(),
                                    ManagerID = Convert.ToInt32(reader["ManagerID"]),
                                    Enable = Convert.ToBoolean(reader["Enable"])
                                };

                                subordinate.Employees = GetSubordinates(subordinate.ID);

                                subordinates.Add(subordinate);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return subordinates;
        }

        public void EnableEmployee(int id, bool enable)
        {
            string query = "UPDATE Employee SET Enable = @Enable WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Enable", enable);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }


}