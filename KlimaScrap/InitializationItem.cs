using System.Collections.Generic;
using System.IO;

namespace KlimaScrap
{   // a class that can be used for initializing purposes 
    class InitializationItem
    {
        public bool Resume { get; set; }
        public List<string> FilesIngested { get; set; }

        internal static void AddNewFiletoIngestedList()
        {

        }
    }
}