﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeReaction.Domain.Commits
{
    public class CommitQuery
    {
        public string Keyword { get; set; }
        public int? Max { get; set; }
        public string ExcludeAuthor { get; set; }
        public string IncludeAuthor { get; set; }
        public bool ExcludeApproved { get; set; }
        public int? Project { get; set; }

        private IQueryable<Commit> query;

        public CommitQuery( IQueryable<Commit> commits )
        {
            query = commits;
        }

        public IEnumerable<Commit> Execute()
        {
            if (string.IsNullOrEmpty(ExcludeAuthor) == false)
            {
                query = query.Where(c => c.Author != ExcludeAuthor);
            }

            if (string.IsNullOrEmpty(IncludeAuthor) == false)
            {
                query = query.Where(c => c.Author == IncludeAuthor);
            }

            if ( string.IsNullOrEmpty(Keyword) == false )
            {
                char[] space = new char[] { ' ' };

                foreach( var word in Keyword.Split(space, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Where(c => c.Message.Contains(word) || c.Author.Contains(word));
                }
            }

            if ( ExcludeApproved )
            {
                query = query.Where( c => c.ApprovedBy == null );
            }

            query = query.OrderBy(c => c.Timestamp);

            if (Project.HasValue)
            {
                query = query.Where(c => c.Project == Project);
            }

            if ( Max.HasValue )
            {
                query = query.Take(Max.Value);
            }

            

            return query.ToList();
        }
    }
}
