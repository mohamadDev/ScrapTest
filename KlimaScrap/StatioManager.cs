using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace KlimaScrap
{
    class StatioManager
    {
        public const string pathtoStations =
            @"ftp://ftp-cdc.dwd.de/climate_environment/CDC/observations_germany/climate/daily/kl/recent/KL_Tageswerte_Beschreibung_Stationen.txt";

        private  Ftp ftp;
        public StatioManager()
        {
            ftp = new Ftp(pathtoStations);
        }

        public void IngestStations()
        {
            var local = $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}.csv";
            ftp.Download(local);
            var ls= new CsvParser();
            var stations=ls.ParsStationCsv(local);
            InsertAllRows(stations);
        }

        private void InsertAllRows(List<StationInfo> info)
        {
            try
            {
                string ConnectionString = "Server = 195.201.118.249; Port = 3306; Database = KlimaDb; Uid = root; Pwd = !@#Cels123; ";
                StringBuilder sCommand = new StringBuilder("INSERT INTO `KlimaDb`.`station`(`stationId`,`StationName`,`LastUpdate`,`lat`,`lon`,`alt`,`from`,`to`,`Bundesland`) VALUES ");
                using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                {
                    var dt = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                    List<string> Rows = new List<string>();
                    for (int i = 0; i < info.Count; i++)
                    {
                        Rows.Add($"('{info[i].Stations_id}','{info[i].Stationsname}','{dt}','{info[i].geoBreite}','{info[i].geoLaenge}','{info[i].Stationshoehe}','{info[i].von_datum}','{info[i].bis_datum}','{info[i].Bundesland}')");
                    }
                    sCommand.Append(string.Join(",", Rows));
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
