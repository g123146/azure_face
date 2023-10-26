using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace face_0827
{
    /// <summary>
    /// Window2.xaml 的互動邏輯
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        async Task CreatePerson(string name)
        {
            string personID, receive;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'name': '" + name + "','userData': ','}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                receive = await response.Content.ReadAsStringAsync();
            }
            var jsonReceive = JObject.Parse(receive);//轉成json格式
            personID = jsonReceive["personId"].ToString();
            create_text.Content = name + " success create !";

        }

        private async void btn_create_Click(object sender, RoutedEventArgs e)
        {
            await CreatePerson(inputName_textbox.Text);
        }
    }
}
