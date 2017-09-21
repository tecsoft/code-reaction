using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class AddTimestampOnComments
    {
        public static IEnumerable<string> Commands()
        {
            return new string[]
            {
                "ALTER TABLE Comments ADD Timestamp DATETIME"
            };
        }
    }
}
