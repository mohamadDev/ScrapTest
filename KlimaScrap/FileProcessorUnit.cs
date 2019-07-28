using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;


namespace KlimaScrap
{
    //Ideally this class has to be implemented with akka or service bus or any cloud related technology to 
    //skip to use that inorder to save cost and time on test 
    class FileProcessorUnit
    {
        private string _path;

        //queue and message on Azure or any other similar technology are a good choice for this unit 
        private Queue<FilePathItem> _files = new Queue<FilePathItem>();


        public FileProcessorUnit(string path, List<string> fileList)
        {
            LogSystem.Log($"init file proc unit {DateTime.Now}");
            _path = path;
            foreach (var itm in fileList)
            {
                _files.Enqueue(new FilePathItem()
                {
                    Remote = new StringBuilder().Append(_path).Append(itm).ToString(),
                    Local = itm
                });
            }
        }

        public void Process()
        {
            LogSystem.Log($"proc started {DateTime.Now}");
            while (_files.Count > 0)
            {
                var itm = _files.Dequeue();
                try
                {
                    var ftp = new Ftp(itm.Remote);
                    //the only reason that i download zip files on the machine is to create a simple resume & disaster recovery system and show that i am thinking of those 
                    // in my codes i will explain in the other part (Part 2) of my task what is the better solution 
                    ftp.Download(itm.Local);
                    var tmpdir = Unzip(itm.Local);
                    Ingest(tmpdir);
                }
                catch (Exception e)
                {
                    LogSystem.Log($"proc file {itm.Local} failed at {DateTime.Now} due to {e.Message}");

                }

            }
        }

        private Guid Unzip(string pth)
        {
            LogSystem.Log($"unziping {pth}");
            var ret = Guid.NewGuid();
            Directory.CreateDirectory(ret.ToString());
            using (ZipArchive archive = ZipFile.OpenRead(pth))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(ret.ToString(), entry.FullName));
                }
            }

            return ret;
        }

        private void Ingest(Guid extrated)
        {
            LogSystem.Log($"ingesting {extrated} {DateTime.Now}");
            try
            {
                var drInfo = new DirectoryInfo(extrated.ToString());
                var taskFiles = drInfo.GetFiles("produkt_klima_tag_*.txt").FirstOrDefault();
                if (taskFiles == null)
                {
                    LogSystem.Log($"no data found");
                    return;
                }

                var data = new CsvParser().ParsClimateCsv(taskFiles.FullName);
                InsertAllRows(data);
                //tobe added some mechanism for DR and file managment 

            }
            catch (Exception e)
            {
                LogSystem.Log($"ingestion failed {extrated} due to {e.Message}");

            }


        }

        private void InsertAllRows(List<ClimateInfo> data)
        {
            try
            {
                string ConnectionString =
                    "Server = 195.201.118.249; Port = 3306; Database = KlimaDb; Uid = root; Pwd = !@#Cels123; ";
                StringBuilder sCommand =
                    new StringBuilder(
                        "INSERT INTO `KlimaDb`.`ClimateData`(`Id`,`StationId`,`MessDatum`,`QN3`,`FX`,`FM`,`Qn4`,`RSK`,`RSKF`,`SDK`,`SHKTAG`,`NM`,`VPM`,`PM`,`TMK`,`UPM`,`TXK`,`TNK`,`TGK`,`EOR`)  VALUES ");
                using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                {
                    
                    List<string> rows = new List<string>();
                    foreach (var t in data)
                    {
                        rows.Add(
                            $"('{Guid.NewGuid()}','{t.STATIONS_ID}','{t.MESS_DATUM}','{t.QN_3}','{t.FX}','{t.FM}'," +
                            $"'{t.QN_4}','{t.RSK}','{t.RSKF}','{t.SDK}','{t.SHK_TAG}','{t.NM}','{t.VPM}'," +
                            $"'{t.PM}','{t.TMK}','{t.UPM}','{t.TXK}','{t.TNK}','{t.TGK}','{t.eor}')");
                    }

                    sCommand.Append(string.Join(",", rows));
                    sCommand.Append(";");
                    mConnection.Open();
                    using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

     
    }
}
