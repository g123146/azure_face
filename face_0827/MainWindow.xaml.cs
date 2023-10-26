using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

//jolin person_ID"f1dbba8f-1282-4f16-a058-d13af59879e8",
//jay person_ID : 379ec61d-1a7d-4b23-b433-fd0ddc04f377
namespace face_0827
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //   CreatePerson("jolin", "a", "http://img.epochtimes.com/i6/1503260821492384.jpg");
            // CreatePerson("hugua", "b", "http://img.ltn.com.tw/Upload/ent/page/800/2016/02/02/phpGkHE6T.jpg", "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d1/%E7%93%9C%E5%93%A5.JPG/1200px-%E7%93%9C%E5%93%A5.JPG", "https://images.chinatimes.com/newsphoto/2018-09-29/900/BE0100_P_01_02.jpg");
            //CreatePerson("ashin", "http://image13.m1905.cn/uploadfile/2015/0417/20150417092249201699.jpeg");
            //CreatePerson("ashin", "https://i.imgur.com/jSD3yUn.jpeg");
            // CreatePerson("ashin", "http://img4.cache.netease.com/photo/0026/2013-11-26/9EK4N89O4CJ80026.jpg");
            //CreatePerson("hugua", "http://img.ltn.com.tw/Upload/ent/page/800/2016/02/02/phpGkHE6T.jpg");
            //CreatePerson("hugua", "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d1/%E7%93%9C%E5%93%A5.JPG/1200px-%E7%93%9C%E5%93%A5.JPG");
            // CreatePerson("hugua", "https://images.chinatimes.com/newsphoto/2018-09-29/900/BE0100_P_01_02.jpg");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        static async Task CreatePersonGroup()//create person group
        {
            string result;

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'name':'faces','userData':'example','recognitionModel' : 'recognition_03'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PutAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();

            }

            Console.WriteLine("PersonGroup Msg: " + result);

        }

        async Task MakeDetectRequest(string url)//faceDetect
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
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'"+url+"'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }

            await Identify(result);//傳faceID過去
            
            string gender, age;
            JArray jsonReceive = JArray.Parse(result);//轉成json格式
            JObject JObj = JObject.Parse(jsonReceive.Last.Last.First.ToString());
            gender = JObj["gender"].ToString();
            age = JObj["age"].ToString();
            text_gender.Text = gender;
            text_age.Text = age;

            //  Console.WriteLine("Detect: " + result);

        }



        async Task CreatePerson(string name, string userData)//create person
        {
            string personID;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'name': '"+name+"','userData': '"+userData+"'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                personID = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine("Create person: " + personID);//personID
            
        //    addFace(personID);
        }


        static async Task addFace(string personID, string url)
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
            queryString["userData"] = "qqq";
            //   queryString["targetFace"] = "{string}";
            queryString["detectionModel"] = "detection_03";
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons/" + personID + "/persistedFaces?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'"+url+"'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }

            //  Console.WriteLine("persistedFaceId: " + result);

            await Train(result);
           
        }

        static async Task Train(string faceID)//train
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
            await getTrainingStatus();
            //      Console.WriteLine("Train: " + result);
           
        }

        static async Task getTrainingStatus()
        {
            string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/training?" + queryString;

            var response = await client.GetAsync(uri);
            result = await response.Content.ReadAsStringAsync();
           // text.Text += ("Get training status: " + result +"\n");
        Console.WriteLine("Get training status: " + result);
        }

        async Task Identify(string receive)
        {
            string faceID;
            JArray jsonReceive = JArray.Parse(receive);//轉成json格式
            JObject JObj;
                                                       // JObject JObj = JObject.Parse(jsonReceive.First.First.ToString());
                                                       // faceID = JObj["faceId"].ToString(); 
            faceID = jsonReceive.First.First.First.ToString();
             string result;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/identify?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'PersonGroupId': 'face_20210831', 'faceIds': ['"+faceID+"'], 'maxNumOfCandidatesReturned': 1,'confidenceThreshold': 0.5}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine("Identify: " + result);
            string personID, confidence;
            jsonReceive = JArray.Parse(result);//轉成json格式
            JObj = JObject.Parse(jsonReceive.Last.Last.First.First.ToString());
            personID = JObj["personId"].ToString();
            string name = await GetPersonListAsync(personID);
            confidence = JObj["confidence"].ToString();
            text_name.Text = name;
            text_confidence.Text = confidence;
        }

        async Task<string> GetPersonListAsync(string personID)
        {
            string result;
            string name = "";
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "216cca43148a4c52912c0452c957c352");

            // Request parameters
            var uri = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/face_20210831/persons?" + queryString;

            var response = await client.GetAsync(uri);
            result = await response.Content.ReadAsStringAsync();

            JArray jsonReceive = JArray.Parse(result);//轉成json格式
            for(int i=0;i < jsonReceive.Count;i++)
            {
             //   JArray Jarray = JArray.Parse(jsonReceive[i].First.ToString());
                if(jsonReceive[i].First.First.ToString() == personID)
                {
                    name = jsonReceive[i].First.Next.Next.First.ToString();
                    break;
                }
            }
            
            return name;

        }

        private void btn_enter_Click(object sender, RoutedEventArgs e)//顯示圖片
        {
            Uri uri1 = new Uri(text_url.Text);
            BitmapImage bitmapImage1 = new BitmapImage(uri1);
            img.Source = bitmapImage1;
            
        }

        private async void btn_identify_Click(object sender, RoutedEventArgs e)
        {
            await MakeDetectRequest(text_url.Text);
        }

        private async void btn_train_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_showPics_Click(object sender, RoutedEventArgs e)
        {
            Window1 show_window = new Window1();
            show_window.ShowDialog();
        }
    }
}
