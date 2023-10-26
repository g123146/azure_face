using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Binding = System.Windows.Data.Binding;
using Newtonsoft.Json;

namespace face_0827
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        List<Person> personList = new List<Person>();
        string personName = "";
        string person_userData = "";
        public Window1()
        {
            InitializeComponent();
            openWindowAsync();
            
        }

        public async Task openWindowAsync()
        {

            List<string> personNameList = new List<string>();
            personList = await GetPersonListAsync();
            for (int i = 0; i < personList.Count; i++)
            {
                personNameList.Add(personList[i].name);
            }
            name_list.ItemsSource = personNameList;//載入人物清單

        }
         async void name_list_SelectionChanged(object sender, SelectionChangedEventArgs args)//listview內東西被選擇時
        {
            List<Image> pics = new List<Image>();
            personName = name_list.SelectedItem.ToString();
            for (int i = 0; i < personList.Count; i++)
            {
                if(personName == personList[i].name)
                {
                    foreach(string faceId in personList[i].persistedFaceIds)
                    {
                        string url_string = await GetFace(personList[i].personId, faceId);

                        try
                        {
                            Image img = new Image();
                            Uri uri1 = new Uri(url_string);
                            BitmapImage bitmapImage1 = new BitmapImage(uri1);
                            img.Source = bitmapImage1;
                            pics.Add(img);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    break;
                }
                
              
            }

            pic_list.ItemsSource = pics;

        }

        private void btn_create_Click(object sender, RoutedEventArgs e)//新增person
        {
            Window2 show_window = new Window2();
            show_window.ShowDialog();
        }

        private async void btn_addPic_Click(object sender, RoutedEventArgs e)//新增圖片
        {
            for (int i = 0; i < personList.Count; i++)
            {

                Person temp = new Person();
                temp = (Person)personList[i];
                if (personName == temp.name)
                {
                    person_userData = temp.userData;
                    await MakeDetectRequest(pic_url.Text, temp.personId);
                    break;
                }
            }
        }

        async Task<List<Person>> GetPersonListAsync()
        {
            ArrayList personArrayList = new ArrayList();
          
            string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            // Request parameters
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons?" + queryString;

            var response = await client.GetAsync(uri);
            result = await response.Content.ReadAsStringAsync();

            personList = JsonConvert.DeserializeObject<List<Person>>(result);

            return personList;

        }
        async Task MakeDetectRequest(string url, string personID)//faceDetect
        {
            string result;

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            // Request parameters
            queryString["returnFaceId"] = "true";
            queryString["returnFaceLandmarks"] = "false";
            queryString["returnFaceAttributes"] = "gender,age,emotion";
            queryString["recognitionModel"] = "recognition_03";
            queryString["returnRecognitionModel"] = "false";
            queryString["detectionModel"] = "detection_01";
            queryString["faceIdTimeToLive"] = "86400";
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'" + url + "'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }

            await addFace(personID,url);
            Console.WriteLine("Detect: " + result);

        }


        async Task addFace(string personID, string url)
        {
            /*
            string personID;
            var jsonReceive = JObject.Parse(receive);//轉成json格式
            personID = jsonReceive["personId"].ToString();*/

            string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            // Request parameters
            queryString["userData"] = url;
            //   queryString["targetFace"] = "{string}";
            queryString["detectionModel"] = "detection_03";
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons/" + personID + "/persistedFaces?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'" + url + "'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }

            //  Console.WriteLine("persistedFaceId: " + result);

            await Train(personID, url);
           
        }

        async Task Train(string personID, string url)//train
        {
            string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/train?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }

        

        }

        async Task<string> GetFace(string personId, string persistedFaceId)
        {
            string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons/"+personId + "/persistedFaces/"+persistedFaceId+"?" + queryString;

            var response = await client.GetAsync(uri);
            result = await response.Content.ReadAsStringAsync();
            getFace face = JsonConvert.DeserializeObject<getFace>(result);
            return face.userData;
        }

    }

    class Person
    {
        public string personId { get; set; }
        public List<string> persistedFaceIds { get; set; }
        public string name { get; set; }
        public string userData { get; set; }
      
    }

    class getFace
    {
        public string persistedFaceId { get; set; }
        public string userData { get; set; }
    }
}
