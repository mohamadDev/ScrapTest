using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace KlimaScrap
{
    class Ftp
    {
        private string _host = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        public Ftp(string host)
        {
            _host = host;
        }
        public void Download( string localFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(_host);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
LogSystem.Log(ex.ToString());
                }
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                LogSystem.Log(ex.ToString());
            }

        }

        public List<string> ListFiles()
        {
           
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_host);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(ftpStream);
                string names = reader.ReadToEnd();

                reader.Close();
                ftpResponse.Close();

                return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
              
            }
            catch (Exception e)
            {

                LogSystem.Log(e.ToString());
                throw;
            }
        }
    }
}
