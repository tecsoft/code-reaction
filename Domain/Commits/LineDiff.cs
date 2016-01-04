using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public class LineDiff
    {
        public ChangeState Changed { get; set; }
        public string Text { get; set; }

        public int RemovedLineNumber { get; set; }
        public int AddedLineNumber { get; set; }

        public string Id { get; private set; }

        public LineDiff(ChangeState state, string text, int removedLineNumber, int addedLineNumber )
        {
            Changed = state; 
            Text = text;
            RemovedLineNumber = removedLineNumber;
            AddedLineNumber = addedLineNumber;

            Id = removedLineNumber + "_" + addedLineNumber;
        }
    }
}
