using GuniApp.Web.Controllers;
using GuniApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MSTestProjectForController
{
    [TestClass]
    public class TestDepartmentsController
    {

        [TestMethod]
        public async Task CheckIndexReturnsView()
        {
            // 1. Arrange
            // create an IDisposable dbContext object.
            using var dbContext = DbContextMocker.GetApplicationDbContext("TestMethod1");
            var controller = new DepartmentsController(dbContext);

            // 2. Act
            var actionResult = await controller.Index();

            // 3. Assert 
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            System.Console.WriteLine("Finished the test!");
        }


        [TestMethod]
        public async Task CheckIndexReturnsData()
        {
            // 1. Arrange
            // create an IDisposable dbContext object.
            using var dbContext = DbContextMocker.GetApplicationDbContext("TestMethod2");
            var controller = new DepartmentsController(dbContext);

            // 2. Act
            var actionResult = await controller.Index();

            // 3. Assert
            
            // (a) if the result of the action is a View
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            var result = actionResult as ViewResult;

            // (b) if the name of the View is "Index"
            // -- check if the name of the view has been explicitly defined.
            if (!string.IsNullOrEmpty(result.ViewName))
            {
                // -- if defined explicitly, check if the name is "Index"
                Assert.AreEqual("Index", result.ViewName);
            }

            // (c) if the model returned is an object of the correct type
            Assert.IsInstanceOfType(result.Model, typeof(List<Department>));

            var departments = result.Model as List<Department>;

            // (d) Check if the data returned matches with the Seeded Data.
            Assert.AreEqual<int>(
                DbContextMocker.SeedData_Departments.Length
                , departments.Count
                , "Number of rows returned does not match!");

            int ndx = 0;
            foreach(Department dept in departments)
            {
                var expectedDept = DbContextMocker.SeedData_Departments[ndx];
                Assert.AreEqual<int>(
                    expectedDept.DepartmentId
                    , dept.DepartmentId
                    , $"Department ID does not match with seeded data for Row # {ndx}");
                Assert.AreEqual<string>(
                    expectedDept.DepartmentName
                    , dept.DepartmentName
                    , $"Department Name does not match with seeded data for Row # {ndx}");
                ndx++;
            }

        }

    }
}
