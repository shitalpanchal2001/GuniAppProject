using System;
using Xunit;
using Xunit.Abstractions;
using GuniApp.Web.Controllers;
using GuniApp.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace xUnitTestProjectForApiController
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void TestGetAllDepartments()
        {
            // Arrange
            using var dbContext = DbContextMocker.GetApplicationDbContext("DbXTest1");
            var controller = new DepartmentsApiController(dbContext);
            
            // Act  ( HTTP GET: api/DepartmentsApi )
            var actionResult = await controller.GetDepartments();   // HTTP GET: api/DepartmentsApi
            var departments = actionResult.Value;

            // Assert
            
            // (a) Check if the API action method returns any data.
            Assert.NotNull(departments);
            _testOutputHelper.WriteLine("--- (a) Completed");

            // (b) Check if the number of departments is as per the seeded data.
            int numberOfDepts = (departments as IList<Department>).Count;
            Assert.Equal(DbContextMocker.SeedData_Departments.Length, numberOfDepts);
            _testOutputHelper.WriteLine("--- (b) Completed");
        }


        [Fact]
        public async Task TestGetSecondDepartment()
        {
            // Arrange
            using var dbContext = DbContextMocker.GetApplicationDbContext("DbXTest2");
            var controller = new DepartmentsApiController(dbContext);
            var deptId = DbContextMocker.SeedData_Departments[1].DepartmentId;

            // Act  ( HTTP GET: api/DepartmentsApi/2 )
            var actionResult = await controller.GetDepartment(deptId);
            var department = actionResult.Value;

            // Assert

            // (a) Check if the API action method returned any data
            Assert.NotNull(department);

            // (b) Check if the data is the same as the seeded data in the inmemory database
            Assert.Equal(DbContextMocker.SeedData_Departments[1].DepartmentId, department.DepartmentId);
            Assert.Equal(DbContextMocker.SeedData_Departments[1].DepartmentName, department.DepartmentName);
        }

        [Fact]
        public async Task TestAddNewDepartment()
        {
            // Arrange
            using var dbContext = DbContextMocker.GetApplicationDbContext("DbXTest3");
            var controller = new DepartmentsApiController(dbContext);
            var newDept = new Department
            {
                DepartmentId = 9,
                DepartmentName = "Arts Department"
            };

            // Act  ( HTTP POST ) 
            var actionResult = await controller.PostDepartment(newDept);

            // Assert

            // (a) Check the Action Result Type
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            // (b) Check the result.
            var result = actionResult.Result as CreatedAtActionResult;
            Assert.NotNull(result);

            // (c) Check if the department added is not null.
            var deptAdded = result.Value as Department;
            Assert.NotNull(deptAdded);

            // (d) Check if the department added has the correct data.
            Assert.Equal(deptAdded.DepartmentId, newDept.DepartmentId);
            Assert.Equal(deptAdded.DepartmentName, newDept.DepartmentName);

            // HTTP GET
            var actionResult2 = await controller.GetDepartments();
            var departments = actionResult2.Value;
            _testOutputHelper.WriteLine("--- Departments in the InMemory Database");
            foreach(Department dept in departments) 
            {
                _testOutputHelper.WriteLine($"{dept.DepartmentName} [ ID: {dept.DepartmentId} ]");
            }
            var noOfDepts = (departments as IList<Department>).Count;
            _testOutputHelper.WriteLine($"Total Number of Departments: {noOfDepts}");

            // (e) Check if the number of departments in the InMemory Database is correct.
            Assert.Equal(DbContextMocker.SeedData_Departments.Length + 1, noOfDepts);
        }
    }
}
