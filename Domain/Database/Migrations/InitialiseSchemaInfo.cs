using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class InitialiseSchemaInfo
    {
        public static IEnumerable<string> Commands()
        {
            return new string[]
            {
                "CREATE TABLE SchemaVersions (Number INTEGER PRIMARY KEY NOT NULL, Timestamp DATETIME)"
            };
        }
    }
}
