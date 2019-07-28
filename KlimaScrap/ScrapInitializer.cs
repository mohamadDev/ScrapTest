using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace KlimaScrap
{
    class ScrapInitializer
    {
        private  string _path;
        public ScrapInitializer(string pth)
        {
            _path = pth;
        }

        public List<string> Init()
        {

            try
            {
                LogSystem.Log($"Exec {DateTime.Now}");
                var inititem= CheckResume();
                return  ListFiles(inititem);

            }
            catch (Exception e)
            {
                LogSystem.Log($"An fatal Error happend {DateTime.Now}");
                LogSystem.Log(e.Message);
                throw;
            }
           
        }
        private List<string> ListFiles(InitializationItem inititem)
        {
            LogSystem.Log($"LoadFiles {DateTime.Now}");
            try
            {
                var ftp =new Ftp(_path);
                var ftpres=ftp.ListFiles();
                var myRegex = new Regex(@"^.*\.(zip)$", RegexOptions.IgnoreCase);
                var ret = ftpres.Where(x => myRegex.IsMatch(x)).ToList();
                return inititem.Resume ? ret.Except(inititem.FilesIngested).ToList() : ret;
            }
            catch (Exception e)
            {
                LogSystem.Log($"An fatal Error happend {DateTime.Now}");
                LogSystem.Log(e.Message);
            }
            LogSystem.Log("Empty Folder");
            return new List<string>();
        }





        //this is not implemented here this function checks in case scraper is intrupted finds the last row checks with file and resumes the process 
        // in order to save time I skip this atm and in the test it always pretends that there have not been any interuption 
        private InitializationItem CheckResume()
        {
            LogSystem.Log($"init {DateTime.Now}");
            return  new InitializationItem()
            {
                Resume = false
            };

        }

   
    }

 
}
