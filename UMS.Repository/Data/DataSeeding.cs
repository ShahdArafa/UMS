using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Repository.Data
{
    public class DataSeeding
    {
        public static void SeedData(StoreContext context)
        {
            //if (!context.Applications.Any())
            //{
            //    var application = new Application
            //    {
            //        FullName = "Shahd Ahmed",
            //        Email = "shahd@example.com",
            //        Phone = "01065148786",
            //        DesiredDepartment = "Computer Science",
            //        DocumentsPath = "C:\\Users\\Shahd\\Downloads",
            //        StudentId = 1//42021364
            //    };

            //    context.Applications.Add(application);
            //    context.SaveChanges();
            //}


           
        }
    }
}
