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
        public string ApiServiceNamespace { get; set; } = "Genesis.Services";
        public string ApiServiceSuffix { get; set; } = "Service";
        public bool InitNullable { get; set; } = true;
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
