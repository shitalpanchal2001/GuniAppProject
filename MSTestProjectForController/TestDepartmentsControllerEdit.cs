using GuniApp.Web.Controllers;
using GuniApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace MSTestProjectForController
{
    [TestClass]
    public class TestDepartmentsControllerEdit
    {
        /// <summary>
        ///     Test the HTTP GET method for /Departments/Edit
        /// </summary>
        [TestMethod]
        public async Task CheckEditReturnsView()
        {
            // 1. Arrange
            // create an IDisposable dbContext object.
            using var dbContext 
                = DbContextMocker.GetApplicationDbContext(nameof(CheckEditReturnsView));
            var controller = new DepartmentsController(dbContext);
            int editDeptId = DbContextMocker.SeedData_Departments[0].DepartmentId;

            
            // 2. Act
            var actionResult = await controller.Edit(editDeptId);

            
            // 3. Assert 

            // (a) Check if the result of the action is a View!
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            System.Console.WriteLine("--- completed step (a)");


            var viewResult = actionResult as ViewResult;

            // (b) If defined, check if the returned view is named as Edit.
            if(!string.IsNullOrEmpty(viewResult.ViewName))
            {
                Assert.AreEqual("Edit", viewResult.ViewName);
            }
            System.Console.WriteLine("--- completed step (b)");

            // (c) Check if the model returned is an object of the correct type.
            Assert.IsInstanceOfType(viewResult.Model, typeof(Department));
            System.Console.WriteLine("--- completed step (c)");


            var editDept = viewResult.Model as Department;

            // (d) Check if the Model returned by the Action Method to the View, is the correct type
            Assert.IsNotNull(editDept);
            System.Console.WriteLine("--- completed step (d)");


            // (e) Check if the data returned  matches with the Seeded Data.
            var expectedDept = DbContextMocker.SeedData_Departments
                                .SingleOrDefault(e => e.DepartmentId == editDeptId);
            Assert.AreEqual<int>( 
                expectedDept.DepartmentId, editDept.DepartmentId
                , $"Department ID does not match with seeded data.");
            Assert.AreEqual<string>(
                expectedDept.DepartmentName, expectedDept.DepartmentName
                , $"Department Name does not match with seeded data.");
            System.Console.WriteLine("--- completed step (e)");

        }


        /// <summary>
        ///     Test the HTTP POST method for /Departments/Edit
        /// </summary>
        [TestMethod]
        public async Task CheckEditUpdatesOk()
        {
            // Arrange
            // create an IDisposable dbContext object.
            using var dbContext
                = DbContextMocker.GetApplicationDbContext(nameof(CheckEditUpdatesOk));
            var controller = new DepartmentsController(dbContext);
            int editDeptId = DbContextMocker.SeedData_Departments[0].DepartmentId;
            var editDeptName = DbContextMocker.SeedData_Departments[0].DepartmentName;
            var changedDeptName = editDeptName.ToUpper();

            // 2. Act

            // -- (a) Call HTTP GET for EDIT
            var actionResult = await controller.Edit(editDeptId);

            // -- (b) Extract the department that you want to Edit
            var editDept = (actionResult as ViewResult).Model as Department;

            // -- (c) Modify the data.
            editDept.DepartmentName = changedDeptName;

            // -- (d) Update the Data (Call HTTP POST for EDIT)
            actionResult = await controller.Edit(editDeptId, editDept);


            // 3. Assert

            // (a) Check if the Redirect to the Index Action was received.
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToActionResult));

            // (b) Check if the Redirect is to the "Index" Action method.
            Assert.AreEqual("Index", (actionResult as RedirectToActionResult).ActionName);
        }



        [TestMethod]
        public async Task CheckEditUpdatesWithInjectedModelStateError()
        {
            // 1. Arrange
            using var dbContext 
                = DbContextMocker.GetApplicationDbContext("CheckEditUpdatesWithInjectedModelStateError"); 
            var controller = new DepartmentsController(dbContext);
            int editDeptId = DbContextMocker.SeedData_Departments[0].DepartmentId;
            var editDeptName = DbContextMocker.SeedData_Departments[0].DepartmentName;
            var changedDeptName = editDeptName.ToUpper();

            // 2. Act
            
            // --- (a) Call HTTP GET for EDIT
            var actionResultEditGet = await controller.Edit(editDeptId);

            // --- (b) Extract the department that you want to Edit
            var editDept = (actionResultEditGet as ViewResult).Model as Department;

            // --- (c) Modify the data.
            editDept.DepartmentName = changedDeptName;

            // --- (d) Inject a ModelState Error
            //         To check if the Controller implements an IsValid block before update!
            controller.ModelState.AddModelError("TestError", "Error inserted during testing.");

            // --- (e) Update the Data (calls HTTP POST for EDIT)
            var actionResultEditPost = await controller.Edit(editDeptId, editDept);

            
            // 3. Assert.

            // (a) Since ModelState has an error,
            //     ensure that you are returning back to the same Edit operation.
            Assert.IsInstanceOfType(actionResultEditPost, typeof(ActionResult));

            // (b) Extract the ViewResult
            var actionResult = actionResultEditPost as ViewResult;

            // (c) If defined,  check if the Returned View is named as "Edit"
            if (!string.IsNullOrEmpty(actionResult.ViewName))
            {
                Assert.AreEqual("Edit", actionResult.ViewName);
            }

            // (d) Check if the model returned is an object of the correct type
            Assert.IsInstanceOfType(actionResult.Model, typeof(Department));

            // (e) Check if the ModelState for the action result contains any errors.
            var modelstate = actionResult.ViewData.ModelState;
            Assert.AreEqual<bool>(false, modelstate.IsValid,
                "ModelState does not report any errors!  Something else might be wrong!");

            // (f) Check if the ModelState is carrying the error injected earlier in ACT Step (e)
            Assert.IsTrue(modelstate.ContainsKey("TestError"));
            Assert.AreEqual<string>(
                "Error inserted during testing."
                , modelstate["TestError"].Errors[0].ErrorMessage
                , "ModelState Error for [TestError] is not the same as what was injected!");
        }

    }
}
