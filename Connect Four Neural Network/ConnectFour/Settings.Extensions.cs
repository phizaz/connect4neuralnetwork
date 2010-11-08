using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour.Properties
{
    internal sealed partial class Settings
    {
        // Your code goes here, safe from being overwritten by the IDE
        public System.Collections.Specialized.StringCollection NetworkPaths
        {
            get
            {
                if (NetworkPathsRaw == null)
                {
                    NetworkPathsRaw = new System.Collections.Specialized.StringCollection();
                    for (int i = 0; i <= 10; ++i)
                        NetworkPathsRaw.Add(null);
                }
                return NetworkPathsRaw;
            }

            set
            {
                NetworkPathsRaw = value;
            }
        }

        public string CurrentNetworkPath { get { return NetworkPaths[Difficulty]; } }

    }
}
