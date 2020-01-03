using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output
{
    public class DependencyEventArgs : EventArgs
    {
        private GenesisDependency _dependency;

        public DependencyEventArgs(GenesisDependency dependency)
        {
            _dependency = dependency;
        }

        public GenesisDependency Dependency {get=>_dependency;set=> _dependency = value;}
    }

    public class GenesisDependency : IOutputDependency
    {
        public GenesisDependency(string pathFragment, string objectName, string contents)
        {
            PathFragment = pathFragment;
            Contents = contents;
            ObjectName = objectName;
        }

        public string PathFragment { get; set; }
        public string Contents { get; set; }
        public string ObjectName { get; set; }
    }
}
