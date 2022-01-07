using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebApplication1.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        Employee emp;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        //convert(varchar(10),DateOfJoining,120) AS 
        public string GetQuery()
        {
            string query = @"SELECT EmployeeId, EmployeeName, Department,
                            CONVERT(varchar(10), DateOfJoining, 120) AS DateOfJoining
                            ,PhotoFileName
                            FROM dbo.Employee
                            ";
            return query;
        }

        public string GetEmployeeNameQuery()
        {
            string query = @"SELECT DepartmentName FROM Department
                            ";
            return query;
        }

        public string PostQuery(Employee emp)
        {
            /*
            string query = @"INSERT INTO dbo.Employee 
                            (EmployeeName,Department,DateOfJoining,PhotoFileName)
                            VALUES
                            ('" + emp.Name + @"',
                            '" + emp.Department + @"',
                            '" + emp.DateOfJoining.ToString("dd-mm-yyyy") + @"',
                            '" + emp.PhotoFileName + @"'
                            )
                            ";
            */
            string query = @"INSERT INTO dbo.Employee 
                            (EmployeeName, Department, DateOfJoining, PhotoFileName)
                            VALUES
                            (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)
                            ";
            return query;
        }
        public string UpdateQuery(Employee emp)
        {
            
            string query = @"UPDATE dbo.Employee SET 
                            EmployeeName = '" + emp.EmployeeName + @"',
                            Department = '" + emp.Department + @"',
                            DateOfJoining = '" + emp.DateOfJoining.ToString("yyyy-MM-dd") + @"'
                            WHERE EmployeeId = '" + emp.EmployeeId + @"'
                            ";
            
            /*
            string query = @"UPDATE INTO dbo.Employee SET
                            (EmployeeName, Department, DateOfJoining, PhotoFileName)
                            VALUES
                            (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)
                            ";
            */
            return query;
        }
        public string DeleteQuery(int id)
        {
            string query = @"DELETE FROM dbo.Employee
                             WHERE EmployeeId = '" + id + @"'
                            ";
            return query;
        }

        [HttpGet]
        public IActionResult Get()     
        {
            try
            {
                string query = this.GetQuery();
                DataTable table = new DataTable();

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }
                return new OkObjectResult(table);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        [HttpPost]
        public IActionResult Post(Employee emp)
        {
            try
            {
                string query = PostQuery(emp);

                DataTable table = new DataTable();

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName ?? (object)DBNull.Value);
                        myCommand.Parameters.AddWithValue("@Department", emp.Department ?? (object)DBNull.Value);
                        myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                        myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName ?? (object)DBNull.Value);

                        myCommand.ExecuteNonQuery();

                        myCon.Close();
                    }
                }

                return new OkObjectResult("Added Successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        [HttpPut]
        public IActionResult Put(Employee emp)
        {
            try
            {
                string query = UpdateQuery(emp);

                DataTable table = new DataTable();

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new OkObjectResult("Updated Successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                string query = DeleteQuery(id);

                DataTable table = new DataTable();

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new OkObjectResult("Deleted Successfully.");
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch(Exception ex)
            {
                return new JsonResult("anonymous.png");
            }
        }

        [Route("GetAllDepartmentNames")]
        public IActionResult GetAllDepartmentNames()
        {
            try
            {
                string query = this.GetEmployeeNameQuery();
                DataTable table = new DataTable();

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new OkObjectResult(table);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }
    }
}
