namespace KlimaScrap
{
    public class StationInfo
    {
        public int Stations_id { get; set; }
        public int von_datum { get; set; }
        public int bis_datum { get; set; }
        public double geoBreite { get; set; }
        public double geoLaenge { get; set; }
        public double Stationshoehe { get; set; }
        public string Stationsname { get; set; }
        public string Bundesland { get; set; }
    }
}