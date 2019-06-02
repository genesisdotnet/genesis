using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Genesis
{
    public static class Dbug
    {
        /// <summary>
        /// Writes a line to Debug.WriteLine
        /// </summary>
        /// <param name="sender">'this'</param>
        /// <param name="message">the message you want written</param>
        public static void Dump(object sender, string message = "")
        {
            Debug.WriteLine($@"{sender.GetType().Name}.{new StackTrace().GetFrame(1).GetMethod().Name}|{message}");
        }
    }
}
