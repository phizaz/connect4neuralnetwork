using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public string CurrentNetworkPath 
        { 
            get { return NetworkPaths[Difficulty]; }
            set { NetworkPaths[Difficulty] = value; }
        }

        public string NetworkDirectory
        {
            get 
            {  
                string d1, d2;
                return CurrentNetworkPath == null || (d1 = Path.GetDirectoryName(CurrentNetworkPath)) == null || (d2 = Path.GetDirectoryName(d1)) == null ? System.Reflection.Assembly.GetExecutingAssembly().Location : d2;
            }
        }
    }
}
