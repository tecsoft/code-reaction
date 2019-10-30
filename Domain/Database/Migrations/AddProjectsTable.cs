using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class AddProjectsTable
    {
        public static IEnumerable<string> Commands()
        {
            return new string[]
            {
@"CREATE TABLE [Projects] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Name] text NOT NULL
, [Path] text NOT NULL
, [IsDefault] BOOL NOT NULL DEFAULT 0
);"
            };
        }
    }
}
