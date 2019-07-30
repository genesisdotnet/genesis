using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Genesis
{
    public class GenesisAssembly
    {
        #region Notes on Inlining
        // It is important to mark this method as NoInlining, otherwise the JIT could decide
        // to inline it into the Main method. That could then prevent successful unloading
        // of the plugin because some of the MethodInfo / Type / Plugin.Interface / HostAssemblyLoadContext
        // instances may get lifetime extended beyond the point when the plugin is expected to be
        // unloaded.
        #endregion

        /// <summary>
        /// Loads an <see cref="Assembly"/> from a <see cref="Stream"/> that is able to be unloaded.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> with the contents of an <see cref="Assembly"/>.</param>
        /// <param name="loadContextWeakReference">The <see cref="WeakReference"/> object holding a reference to the <see cref="GenesisAssemblyLoadContext"/>.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly FromStream(Stream stream, out WeakReference loadContextWeakReference)
        {
            var alc = new GenesisAssemblyLoadContext();
            loadContextWeakReference = new WeakReference(alc); // Create a weak reference to the AssemblyLoadContext that will allow us to detect when the unload completes.

            return alc.LoadFromStream(stream);
        }
    }
}
