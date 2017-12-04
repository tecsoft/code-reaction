﻿using CodeReaction.Domain.Database.Migrations;
using CodeReaction.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database
{
    public static class SchemaMigration
    {
        public static void Execute()
        {
            using (var database = new DbCodeReview())
            {
                int currentVersion = 0;

                try
                {
                    currentVersion = database.SchemaVersion.Max(v => v.Number);
                }
                catch
                {
                    // doesn't exist start from 0
                }

                int newVersion = Index.Migrations.Keys.Max();

                while (currentVersion < newVersion)
                {
                    ExecuteForVersion(database, ++currentVersion);

                    database.SchemaVersion.Add(
                        new SchemaVersion { Number = currentVersion, Timestamp = DateTime.UtcNow }
                    );

                    database.SaveChanges();
                }
            } 
        }

        private static void ExecuteForVersion(DbContext dbContext, int version)
        {
            string className = Index.Migrations[version];

            if (!string.IsNullOrEmpty(className))
            {
                Type type = Type.GetType("CodeReaction.Domain.Database.Migrations." + className);

                var commands = type?.GetMethod("Commands").Invoke(null, null) as IEnumerable<string>;

                if (commands == null)
                {
                    string msg = $"Migration {version} [{className}] incorrectly configured";
                    System.Diagnostics.Debug.WriteLine(msg);
                    throw new Exception(msg);
                }
                else
                {
                    foreach (var cmd in commands)
                    {
                        try
                        {
                            dbContext.Database.ExecuteSqlCommand(cmd);
                        }
                        catch( Exception ex )
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
#if (DEBUG == false )                      
                            throw;
#endif
                        }
                    }
                }
            }
        }
    }
}
