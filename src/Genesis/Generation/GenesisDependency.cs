using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Generation
{
    public class GenesisDependency : IGenesisDependency
    {
        public GenesisDependency(string pathFragment, string contents)
        {
            _pathFragment = pathFragment;
            _contents = contents;
        }
        private readonly string _pathFragment;
        public string PathFragment => _pathFragment;

        private readonly string _contents;
        public string Contents => _contents;
    }
}
