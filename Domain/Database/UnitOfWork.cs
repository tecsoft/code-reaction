using CodeReaction.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain
{
    public class UnitOfWork : IDisposable
    {
        public DbCodeReview Context { get; protected set; }

        public UnitOfWork()
        {
            Context = new DbCodeReview();
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
