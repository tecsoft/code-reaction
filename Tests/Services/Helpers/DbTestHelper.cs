using System;
using System.Data.Entity.Validation;
using System.IO;

namespace CodeReaction.Tests.Services.Helpers
{
    public static class DbTestHelper
    {
        public static void ResetDatabase()
        {
            FileInfo dbFile = new FileInfo("../../../Domain/Database/CodeReaction.sqlite");
            if (dbFile.Exists == false)
                throw new FileNotFoundException();

            dbFile.CopyTo("CodeReaction.sqlite", true);
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

        public static void Try( Action dbAction )
        {
            try
            {
                dbAction();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                DebugValidationErrors(ex);
                throw;
            }
        }
    }
}
