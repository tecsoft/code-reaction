using CodeReaction.Domain.Commits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.HouseKeeping
{
    public class HouseKeepingService
    {
        public static void ImportLatestLogs()
        {
            try
            {

                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    var commitService = new CommitService(unitOfWork);

                    int? lastRevision = commitService.GetLastKnownRevision();

                    if (lastRevision == null)
                    {
                        // todo we need to set up the case where we haven't yet primed the last revision nb

                        lastRevision = 30000;
                    }

                    int count = commitService.PersistLatestCommitData(lastRevision.Value);

                    // debug count information

                    unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
