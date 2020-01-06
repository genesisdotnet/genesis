using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    /// <summary>
    /// Simple sloppy string helper for dependency injection services.
    /// </summary>
    public sealed class GrpcServiceInjector // copy paste isn't extensible
    {
        public static string GetDeclarationForServiceClass(ObjectGraph og, string suffix = "GrpcService", bool initNullable = true)
            => $"\t\t\tprivate readonly {GetServiceInterfaceName(og, suffix)} {GetServiceMemberName(og, suffix)}{((initNullable) ? " = null!" : string.Empty)};";
        public static string GetParameterForServiceClass(ObjectGraph og, string suffix = "GrpcService")
            => $"{GetServiceInterfaceName(og, suffix)} {GetServiceParamName(og, suffix)}";

        public static string GetAssignmentForServiceClass(ObjectGraph og, string suffix = "GrpcService")
            => $"\t\t\t{GetServiceMemberName(og, suffix)} = {GetServiceParamName(og, suffix)};";

        private static object GetServiceMemberName(ObjectGraph og, string suffix)
            => $"_{og.Name.ToSingular().ToCamelCase()}{suffix}";

        private static object GetServiceParamName(ObjectGraph og, string suffix)
            => $"{og.Name.ToSingular().ToCamelCase()}{suffix}";

        private static object GetServiceInterfaceName(ObjectGraph og, string suffix)
            => $"I{og.Name.ToSingular()}{suffix}";
    }

    /// <summary>
    /// Simple sloppy string helper for dependency injection strings.
    /// </summary>
    public sealed class RepoInjector
    {
        public static string GetDeclarationForObjectRepo(ObjectGraph og, string repoSuffix = "Repository", bool initNullable = true)
            => $"\t\t\tprivate readonly {GetRepoInterfaceName(og, repoSuffix)} {GetRepoMemberName(og, repoSuffix)}{((initNullable)? " = null!":string.Empty)};";
        public static string GetParameterForObjectRepo(ObjectGraph og, string repoSuffix = "Repository", bool initNullable = true)
            => $"{GetRepoInterfaceName(og, repoSuffix)} {GetRepoParamName(og, repoSuffix)}";

        public static string GetAssignmentForObjectRepo(ObjectGraph og, string repoSuffix = "Repository")
            => $"\t\t\t{GetRepoMemberName(og, repoSuffix)} = {GetRepoParamName(og, repoSuffix)};";
        
        private static object GetRepoMemberName(ObjectGraph og, string repoSuffix) 
            => $"_{og.Name.ToSingular().ToCamelCase()}{repoSuffix}";
        
        private static object GetRepoParamName(ObjectGraph og, string repoSuffix)
            => $"{og.Name.ToSingular().ToCamelCase()}{repoSuffix}";

        private static object GetRepoInterfaceName(ObjectGraph og, string repoSuffix)
            => $"I{og.Name.ToSingular()}{repoSuffix}";
    }
}
