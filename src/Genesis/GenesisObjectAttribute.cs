using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class GenesisObjectAttribute : Attribute
    {
        public GenesisObjectAttribute()
        {
            
        }
    }
}