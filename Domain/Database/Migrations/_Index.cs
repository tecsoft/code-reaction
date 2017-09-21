using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class Index
    {
        public static readonly IDictionary<int, string> Migrations = new Dictionary<int, string>()
        {
            {1, "InitialiseSchemaInfo" },
        };
        public static int Max()
        {
            return Migrations.Keys.Max();
        }
    }
}
