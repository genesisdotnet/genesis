using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Protos
{
    public class ProtoServiceConfig : GeneratorConfiguration
    {
        public int Version { get; set; } = 3;
        public string GrpcNamespace { get; set; } = "Genesis.Grpc.Services";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
