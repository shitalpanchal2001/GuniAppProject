using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuniApp.Web.Models;
using GuniApp.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Added the Nuget Package:
//      Microsoft.EntityFrameworkCore               5.x
//      Microsoft.EntityFrameworkCore.InMemory      5.x

namespace MSTestProjectForController
{
    public static class DbContextMocker
    {

        public static ApplicationDbContext GetApplicationDbContext(string databaseName)
        {
            // Create a fresh service provider, therefore a fresh InMemory database instance
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // create a new options instance telling the context to use InMemory datbase
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Create the instance of the DbContext (an InMemory Database over here)
            var dbContext = new ApplicationDbContext(options);

           

            return dbContext;
        }

            };


        /// <summary>
        ///     An extension Method to seed the data into the InMemory Database
        /// </summary>
        /// <param name="context"></param>

    }
