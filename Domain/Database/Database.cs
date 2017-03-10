using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using System.Data.Entity;

namespace CodeReaction.Domain.Repositories
{
    public class DbCodeReview : DbContext
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Commit> Commits { get; set; }

        public DbCodeReview() : base("CodeReaction")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        } 
    }
}
