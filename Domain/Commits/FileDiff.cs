using CodeReaction.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public class FileDiff
    {
        public string Name { get; set; }

        public string Previous { get; set; }
        public string Current { get; set; }
        public IList<LineDiff> Lines { get; set; }

        public FileState FileState { get; set; }
        public FileType FileType { get; set; }
        public string OriginalName { get; set; }

        public FileDiff(FileState fileState, FileType fileType, string fileName, string originalName)
        {
            FileState = fileState;
            FileType = fileType;
            Name = fileName;
            OriginalName = originalName;
            Lines = new List<LineDiff>();  
        }

        public LineDiff AddLine(ChangeState state, string text, int removedLineNumber, int addedLineNumber)
        {
            LineDiff line = new LineDiff(state, text, removedLineNumber, addedLineNumber);
                 
            Lines.Add(line);
            return line;
        }
    }
}
