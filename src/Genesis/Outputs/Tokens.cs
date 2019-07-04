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
        /// Seperator characters for multiple output files in a template. This is kinda ghetto and lazy but for now, whatevs
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
    }
}
