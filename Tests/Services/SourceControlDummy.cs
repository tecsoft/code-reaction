using System;
using System.Collections.Generic;
using CodeReaction.Domain.Commits;
using System.Linq;

namespace CodeReaction.Tests.Services
{
    internal class SourceControlDummy : ISourceControl
    {
        private List<Commit> list;

        public SourceControlDummy(List<Commit> list)
        {
            this.list = list;
        }

        public List<Commit> Commits {  get { return list; } }

        public CommitDiff GetRevision(long revision)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Commit> GetSince(long revision, int maxNumber)
        {
            return list.Where(c => c.Revision > revision).OrderBy( c => c.Revision ).Take(maxNumber).ToList();
        }
    }
}