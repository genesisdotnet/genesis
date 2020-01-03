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
            _pathFragment = pathFragment;
            _contents = contents;
            _objectName = objectName;
        }
        private readonly string _pathFragment;
        public string PathFragment => _pathFragment;

        private readonly string _contents;
        public string Contents => _contents;

        private readonly string _objectName;
        public string ObjectName => _objectName;
    }
}
