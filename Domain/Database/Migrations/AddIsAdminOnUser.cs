using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class AddIsAdminOnUser
    {
        public static IEnumerable<string> Commands()
        {
            return new string[]
            {
                "ALTER TABLE Users ADD IsAdmin BOOL DEFAULT 0",
                "UPDATE Users SET IsAdmin = 0 WHERE IsAdmin IS NULL",
            };
        }
    }
}
