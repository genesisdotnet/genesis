using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Genesis
{
    public static class GenesisGlobals
    {
        public static readonly string[] TrustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
    }
}
