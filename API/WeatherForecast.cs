namespace API
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        //Nullale property has been disabled in API.csproj file, no need to specify '?' or set null reference
       // public string? Summary { get; set; }
        public string Summary { get; set; }

    }
}