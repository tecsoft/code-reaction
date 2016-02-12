using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Repositories
{
    public class DbCodeReview : DbContext
    {
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Commit> Commits { get; set; }

        public DbCodeReview() : base("CodeReaction")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        } 
    }
}
