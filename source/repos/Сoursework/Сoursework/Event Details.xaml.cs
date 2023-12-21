using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Coursework
{
    public partial class EventDetailsWindow : Window
    {
        public EventDetailsWindow(string name, string type, string date, string location, string url)
        {
            InitializeComponent();
            SetContent(name, type, date, location, url);
        }

        private void SetContent(string name, string type, string date, string location, string url)
        {
            l1.Content = $"Название: {name}";
            l2.Content = $"Тип: {type}";
            l3.Content = $"Дата: {date}";
            l4.Content = $"Место: {location}";
            l5.Text = $"Ссылка: {url}";
        }
    }
}
