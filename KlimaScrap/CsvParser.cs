using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace KlimaScrap
{
    class CsvParser
    {
        public List<StationInfo> ParsStationCsv(string path)
        {
            var raw=File.ReadAllLines(path);
            var ret= new List<StationInfo>();
            for (int  i= 2; i < raw.Length; i++)
            {
                var colmn=  System.Text.RegularExpressions.Regex.Split(raw[i], @"\s{1,}");

                try
                {
                    ret.Add(new StationInfo()
                    {
                        Stations_id = int.Parse(colmn[0]),
                        von_datum = int.Parse(colmn[1]),
                        bis_datum = int.Parse(colmn[2]),
                        Stationshoehe = int.Parse(colmn[3]),
                        geoBreite = double.Parse(colmn[4]),
                        geoLaenge = double.Parse(colmn[5]),
                        Stationsname = colmn[6],
                        Bundesland = colmn[7]

                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
               
            }

            return ret;
          

        }

        public List<ClimateInfo> ParsClimateCsv(string path)
        {
            var raw = File.ReadAllLines(path);
            var ret = new List<ClimateInfo>();
            for (int i = 1; i < raw.Length; i++)
            {
                var colmn = raw[i].Split(";");
               
                try
                {
                    ret.Add(new ClimateInfo()
                    {
                        STATIONS_ID = int.Parse(colmn[0]),
                        MESS_DATUM= int.Parse(colmn[1]),
                        QN_3= double.Parse(colmn[2]),
                        FX = double.Parse(colmn[3]),
                        FM = double.Parse(colmn[4]),
                        QN_4 = double.Parse(colmn[5]),
                        RSK=double.Parse(colmn[6]),
                        RSKF = double.Parse(colmn[7]),
                        SDK= double.Parse(colmn[8]),
                        SHK_TAG = double.Parse(colmn[9]),
                        NM= double.Parse(colmn[10]),
                        VPM = double.Parse(colmn[11]),
                        PM = double.Parse(colmn[12]),
                        TMK = double.Parse(colmn[13]),
                        UPM= double.Parse(colmn[14]),
                        TXK= double.Parse(colmn[15]),
                        TNK= double.Parse(colmn[16]),
                        TGK= double.Parse(colmn[17]),
                        eor = colmn[18]

                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }

            }

            return ret;
        }
    }
    //in my assumption just for simplicity station Ids are Unique
}
