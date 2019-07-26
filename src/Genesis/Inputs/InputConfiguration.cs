using System.Composition;

namespace Genesis.Input
{
    /// <summary>
    /// Primary configuration class for individual Current
    /// </summary>
    [Export(nameof(IInputConfiguration), typeof(IInputConfiguration))]
    public class InputConfiguration : IInputConfiguration
    {
        public InputConfiguration()
        {
            //inherit this and declare a public property called .Config; on your custom Executor. 
            //Genesis will activate an instance for you during discovery and assign it to your executor if the stars align. (if it exists, and is parsable)
        }
    }
}