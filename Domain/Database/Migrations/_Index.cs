using System.Collections.Generic;

namespace CodeReaction.Domain.Database.Migrations
{
    public static class Index
    {
        public static readonly IDictionary<int, string> Migrations = new Dictionary<int, string>()
        {
            {1, "InitialiseSchemaInfo" },
            {2, "AddTimestampOnComments" },
            {3, "AddIsLikeOnComments" },
            {4, "AddIsAdminOnUser" },
            {5, "AddProjectsTable" },
            {6, "AddProjectKeyOnCommitTable" },
            // id 7 retired{7, "" }
        };
    }
}
