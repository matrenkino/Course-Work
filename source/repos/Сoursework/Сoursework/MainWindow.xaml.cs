using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Coursework
{
    public partial class MainWindow : Window
    {
        private GMapControl gmapControl = new GMapControl();
        private string apiKey = "cZg85nuAEwbnFJeJriYcm1vGOKAtBFEJ";

        public MainWindow()
        {
            InitializeComponent();

            gmapControl.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            BingMapProvider.Instance.ClientKey = "cgBFQt2f65lM8qeRwJVY~4a1vKEQ-B8i3zJPVIM5FMw~AuQzDbTFWVayUcyN7jGdxs9MrlYkzVmjzmCzi7AZrcU1Trdxvs4wLygeUnnVcS9D";

            gmapControl.Position = new PointLatLng(40.712776, -74.005974);
            gmapControl.MinZoom = 2;
            gmapControl.MaxZoom = 17;
            gmapControl.Zoom = 15;

            grid.Children.Add(gmapControl);
        }

        private async void search_concerts_Click(object sender, RoutedEventArgs e)
        {
            string cityName = city.Text;

            DateTime selectedStartDate = StartDate.SelectedDate ?? DateTime.MinValue;
            DateTime selectedEndDate = EndDate.SelectedDate ?? DateTime.MaxValue;

            if (string.IsNullOrEmpty(cityName))
            {
                await LoadEvents(apiKey, null, selectedStartDate, selectedEndDate);
            }
            else
            {
                await LoadEvents(apiKey, cityName, selectedStartDate, selectedEndDate);
            }
        }

        private async Task LoadEvents(string apiKey, string city, DateTime startDate, DateTime endDate)
        {
            var client = new RestClient("https://app.ticketmaster.com");
            var request = new RestRequest("discovery/v2/events.json", RestSharp.Method.GET);
            request.AddParameter("apikey", apiKey);
            if (!string.IsNullOrEmpty(city))
            {
                request.AddParameter("city", city);
            }

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show("Ошибка RestSharp: " + response.ErrorMessage);
                MessageBox.Show("Исключение: " + response.ErrorException?.Message);
                MessageBox.Show("Тело ответа: " + response.Content);
                return;
            }

            string json = response.Content;
            JObject eventData = JObject.Parse(json);

            if (eventData.ContainsKey("_embedded") && eventData["_embedded"]["events"] is JArray events)
            {
                Console.WriteLine("Ключ 'events' присутствует в ответе от API Ticketmaster.");

                string debugPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string markerImagePath = System.IO.Path.Combine(debugPath, "location.png");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    gmapControl.Markers.Clear();

                    foreach (var ev in events)
                    {
                        double eventLatitude = (double)ev["_embedded"]["venues"][0]["location"]["latitude"];
                        double eventLongitude = (double)ev["_embedded"]["venues"][0]["location"]["longitude"];
                        string eventName = (string)ev["name"];
                        string eventType = (string)ev["type"];
                        string eventDate = (string)ev["dates"]["start"]["localDate"];
                        string eventLocation = (string)ev["_embedded"]["venues"][0]["name"];
                        string eventUrl = (string)ev["url"];

                        DateTime eventDateTime = DateTime.Parse(eventDate);

                        if (eventDateTime >= startDate && eventDateTime <= endDate)
                        {
                            GMapMarker marker = new GMapMarker(new PointLatLng(eventLatitude, eventLongitude));
                            marker.Shape = new System.Windows.Controls.Image
                            {
                                Source = new BitmapImage(new Uri(markerImagePath)),
                                Width = 20,
                                Height = 20
                            };

                            ToolTipService.SetToolTip(marker.Shape, eventName);

                            marker.Shape.MouseLeftButtonDown += (sender, e) =>
                            {
                                ShowEventDetails(eventName, eventType, eventDate, eventLocation, eventUrl);
                            };

                            gmapControl.Markers.Add(marker);
                        }
                    }
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine("Ключ 'events' отсутствует в ответе от API Ticketmaster.");
                });
            }
        }

        private void ShowEventDetails(string name, string type, string date, string location, string url)
        {
            EventDetailsWindow detailsWindow = new EventDetailsWindow(name, type, date, location, url);
            detailsWindow.Owner = this;
            detailsWindow.ShowDialog();
        }
    }
}
//https://www.songkick.com/api_key_requests/new
//https://artists.bandsintown.com/support/api-installation
//https://developer.ticketmaster.com/products-and-docs/apis/discovery-api/v2/#search-events-v2
//"C:\\Users\\yamat\\source\\repos\\Сoursework\\loction.png"
//cgBFQt2f65lM8qeRwJVY~4a1vKEQ-B8i3zJPVIM5FMw~AuQzDbTFWVayUcyN7jGdxs9MrlYkzVmjzmCzi7AZrcU1Trdxvs4wLygeUnnVcS9D
//a7405164-d914-4542-8212-7591fd706732
//cZg85nuAEwbnFJeJriYcm1vGOKAtBFEJ