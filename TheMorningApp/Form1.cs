using IpData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace TheMorningApp
{
    public partial class Form1 : Form
    {        
        public string CountryResponse { get; set; }
        public string ProvinceResponse { get; set; }
        public string CityResponse { get; set; }
        public string IPResponse { get; set; }
        public string Temperature { get; set; }
        public string ExchangeOfUSD { get; set; }

        RegistryKey reg =
                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public Form1()
        {
            reg.SetValue("Morning APP", Application.ExecutablePath.ToString());
            InitializeComponent();
            HttpClient LocationAPI = new HttpClient();
            HttpResponseMessage ApiResponse = LocationAPI.
                GetAsync("https://api.ipdata.co?api-key=015532bfef7cf1dc515c0984472da7f29aebfa2a7924559272d84438")
                .Result;
            if (ApiResponse.IsSuccessStatusCode)
            {
                var responseResult = ApiResponse.Content.ReadAsStringAsync().Result;
                JObject obj = JObject.Parse(responseResult);
                CountryResponse = (string)obj["country_name"];// .GetValue("country_name");
                ProvinceResponse = (string)obj.GetValue("region");
                CityResponse = (string)obj.GetValue("city");
                IPResponse = (string)obj.GetValue("ip");
            }
            else
            {
                Console.WriteLine("An Error has occured:" + ApiResponse.StatusCode + ApiResponse.ReasonPhrase);
            }

            HttpClient WeatherAPI = new HttpClient();
            HttpResponseMessage weatherResponse = WeatherAPI.
                GetAsync
                ($"https://api.openweathermap.org/data/2.5/weather?q={CityResponse}&appid=04901bda849c03c85e34cc2001a5c026")
                .Result;
            if (weatherResponse.IsSuccessStatusCode)
            {
                var weatherResult = weatherResponse.Content.ReadAsStringAsync().Result;
                JObject Wobj = JObject.Parse(weatherResult);
                Temperature = (string)Wobj["main"]["temp"];
            }

            HttpClient ExchangeAPI = new HttpClient();
            HttpResponseMessage ExResponse = ExchangeAPI.
                GetAsync
                ("https://api.exchangeratesapi.io/latest?base=CAD")
                .Result;
            if (ExResponse.IsSuccessStatusCode)
            {
                var ChangeResult = ExResponse.Content.ReadAsStringAsync().Result;
                JObject Cobj = JObject.Parse(ChangeResult);
                ExchangeOfUSD = (string)Cobj["rates"]["USD"];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CountryName.Text = CountryResponse;
            ProvinceName.Text = ProvinceResponse;
            CityName.Text = CityResponse;
            IPName.Text = IPResponse;
            Temp.Text = Temperature + " °F";
            Exchange.Text = "CAD : USD =" + ExchangeOfUSD + " : 1";
        }
    }
}
