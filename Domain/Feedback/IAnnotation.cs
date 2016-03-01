using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeReaction.Domain.Feedback
{
    public interface IAnnotation
    {
        string User { get; }
        long Revision { get;}
        string File { get; }
        string LineId { get; }
    }
}
