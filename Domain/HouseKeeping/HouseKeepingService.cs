using CodeReaction.Domain.Commits;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.HouseKeeping
{
    public class HouseKeepingService
    {
        UnitOfWork _uow;
        ISourceControl _sourceControl;

        public HouseKeepingService(UnitOfWork uow, ISourceControl sourceControl)
        {
            _uow = uow;
            _sourceControl = sourceControl;
        }

        public int ImportLatestLogs()
        {
            int? lastRevision = GetLastKnownRevision();

            if (lastRevision == null)
            {
                // todo we need to set up the case where we haven't yet primed the last revision nb
                var config = ConfigurationManager.GetSection("codeReaction") as CodeReactionConfigurationSection;

                lastRevision = config.Svn.StartRevision;
            }

            return PersistLatestCommitData(lastRevision.Value);
        }

        private int? GetLastKnownRevision()
        {
            return _uow.Context.Commits.Max(c => (int?)c.Revision);
        }

        private int PersistLatestCommitData(int lastRevision)
        {
            var list = _sourceControl.GetSince(lastRevision, 20);

            int result = list.Count();

            if (result > 0)
            {
                _uow.Context.Commits.AddRange(list);
            }

            return result;
        }
    }
}
