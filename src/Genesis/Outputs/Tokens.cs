using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output
{
    public static class Tokens
    {
        /// <summary>
        /// Replaced with the namespace of a given piece of code like classes, structs and enums
        /// </summary>
        public static readonly string Namespace = "~NAMESPACE~";
        /// <summary>
        /// The name of the object - usually a singular version of a database table's name
        /// </summary>
        public static readonly string ObjectName = "~OBJECT_NAME~";
        /// <summary>
        /// Properties for the generated object. This is applied once, conaining all of the object's properties
        /// </summary>
        public static readonly string PropertiesStub = "~PROPERTIES~";
        /// <summary>
        /// Code block for validation
        /// </summary>
        public static readonly string PropertiesValidationStub = "~PROPERTIES_VALIDATION~";
        /// <summary>
        /// The ObjectName as objectName
        /// </summary>
        public static readonly string ObjectNameAsArgument = "~OBJECT_NAME_ARGUMENT~";
        /// <summary>
        /// The ObjectName as Object Name
        /// </summary>
        public static readonly string ObjectNameSpaced = "~OBJECT_NAME_SPACED~";
        /// <summary>
        /// Constructor(s) for the generated class
        /// </summary>
        public static readonly string ConstructionStub = "~CONSTRUCTION~";
        /// <summary>
        /// Private member backing field for a property. Each property will have it's associated column's information. 
        /// </summary>
        public static readonly string PropertyMemberName = "~PROPERTY_MEMBER_NAME~";
        /// <summary>
        /// Outside name for a property, currently with get and set. Each property will have it's associated column's information. 
        /// </summary>
        public static readonly string PropertyName = "~PROPERTY_NAME~";
        /// <summary>
        /// The datatype for a property and backing field according to its property. Each property will have it's associated column's information. 
        /// </summary>
        public static readonly string PropertyDataType = "~PROPERTY_DATATYPE~";
        /// <summary>
        /// Hmm...
        /// </summary>
        public static readonly string EditorRowsStub = "~EDITOR_ROWS~";
        /// <summary>
        /// Separator characters for multiple output files in a template. This is kinda ghetto and lazy but for now, whatevs
        /// </summary>
        public static readonly string TemplateSeperator = "<!----->";
        /// <summary>
        /// Arbitrary version identifier
        /// </summary>
        public static readonly string Version = "~VERSION~";
        /// <summary>
        /// Each property will increment this
        /// </summary>
        public static readonly string PropertyCounter = "~COUNTER~";
        /// <summary>
        /// Name of the base type for an object
        /// </summary>
        public static readonly string BaseTypeName = "~BASE_CLASS~";
        /// <summary>
        /// Methods block placeholder (if supported by the Generator)
        /// </summary>
        public static readonly string MethodsStub = "~METHODS~";
        /// <summary>
        /// Interface list for constructor injections.
        /// </summary>
        public static readonly string Injections = "~INJECTIONS~";
        /// <summary>
        /// Class members for injected objects.
        /// </summary>
        public static readonly string InjectionMembers = "~INJECTION_MEMBERS~";
        /// <summary>
        /// Constructor parameters to members assignments.
        /// </summary>
        public static readonly string InjectionAssignment = "~INJECTION_ASSIGNMENT~";
        /// <summary>
        /// Object key's data type
        /// </summary>
        public static readonly string KeyDataType = "~KEY_DATATYPE~";
        /// <summary>
        /// Namespace for Grpc related output
        /// </summary>
        public static readonly string GrpcNamespace = "~GRPC_NAMESPACE~";
        /// <summary>
        /// Injection class members declarations
        /// </summary>
        public static readonly string RepositoryMembers = "~REPO_MEMBERS~";
        /// <summary>
        /// Injection member assignments, usually for a constructor
        /// </summary>
        public static readonly string RepositoryAssignments = "~REPO_ASSIGNMENTS~";
        /// <summary>
        /// Injection parameters, usually for a constructor
        /// </summary>
        public static readonly string RepositoryInjections = "~REPO_INJECTIONS~";
        /// <summary>
        /// The namespace for a using that contains ApiServices
        /// </summary>
        public static readonly string ApiServiceNamespace = "~API_SERVICE_NAMESPACE~";
        /// <summary>
        /// Api service class name suffix
        /// </summary>
        public static readonly string ApiServiceSuffix = "~API_SERVICE_SUFFIX~";
        /// <summary>
        /// Base class for whatever
        /// </summary>
        public static readonly string ObjectBaseClass = "~OBJECT_BASECLASS~";
        /// <summary>
        /// Suffix for filename / classname etc
        /// </summary>
        public static readonly string OutputSuffix = "~OUTPUT_SUFFIX~";
        /// <summary>
        /// The current context's base class that Models subclass
        /// </summary>
        public static readonly string ModelBaseClass = "~MODEL_BASE_CLASS~";
        /// <summary>
        /// Dependencies namespace
        /// </summary>
        public static readonly string DepsNamespace = "~DEPS_NAMESPACE~";
        /// <summary>
        /// Absolute path for where to place dependencies
        /// </summary>
        public static readonly string DepsPath = "~DEPS_PATH~";
        /// <summary>
        /// Namespace that the models reside
        /// </summary>
        public static readonly string DepsModelNamespace = "~DEPS_MODEL_NAMESPACE~";
    }
}
