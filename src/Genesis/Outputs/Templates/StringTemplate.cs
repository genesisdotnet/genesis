using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Templates
{
    public class StringTemplate : IGeneratorTemplate
    {
        private readonly string content = string.Empty;
        public string Raw => content;

        public StringTemplate(string contents)
        {
            content = contents;
        }
    }
}
