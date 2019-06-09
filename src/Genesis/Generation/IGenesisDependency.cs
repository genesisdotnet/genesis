using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Generation
{
    public interface IGenesisDependency
    {
        string PathFragment { get; }
        string Contents { get; }
    }
}
