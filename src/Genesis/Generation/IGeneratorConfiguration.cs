using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Generation
{
    public interface IGeneratorConfiguration
    {
        string Namespace { get; set; }
        string OutputPath { get; set; }
    }
}
