/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.core
{
    public class ActiveEventArgs : EventArgs
    {
        internal ActiveEventArgs (Node args)
        {
            Args = args;
        }

        public Node Args {
            get;
            private set;
        }
    }
}

