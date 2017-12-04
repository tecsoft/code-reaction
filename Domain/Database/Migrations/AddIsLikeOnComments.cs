using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class AddIsLikeOnComments
    {
        public static IEnumerable<string> Commands()
        {
            return new string[]
            {
                "ALTER TABLE Comments ADD IsLike BOOL DEFAULT 0",
                "UPDATE Comments SET isLIke = 0 WHERE IsLike IS NULL"
            };
        }
    }
}
