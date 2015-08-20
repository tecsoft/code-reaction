using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Tests.Services.Helpers
{
    public static class DbTestHelper
    {
        public static void ResetDatabase()
        {
            FileInfo dbFile = new FileInfo("../../../Domain/Database/CodeReview.sqlite");
            if (dbFile.Exists == false)
                throw new FileNotFoundException();

            dbFile.CopyTo("CodeReviewTest.sqlite", true);
        }

        public static void DebugValidationErrors(DbEntityValidationException ex)
        {
            foreach (var error in ex.EntityValidationErrors)
            {
                foreach (var message in error.ValidationErrors)
                {
                    Console.WriteLine(message.PropertyName + ": " + message.ErrorMessage);
                }
            }
        }
    }
}
