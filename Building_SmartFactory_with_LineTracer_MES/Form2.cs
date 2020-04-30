using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Drawing;
using System.Windows.Forms;

using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Text;

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using System.Web;

using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace FinalProejct_MES
{
    public partial class Form2 : Form
    {        
        public Form2()
        {
            InitializeComponent();
        }

        public string[] face_info = new string[30];
        CvCapture capture_laptopcam;
        IplImage scr_laptopcam;

        int cnt = 3;

        private void tmr_CAM_Tick(object sender, EventArgs e)
        {
            try
            {
                scr_laptopcam = capture_laptopcam.QueryFrame();
                picbox_face_origin.ImageIpl = scr_laptopcam;

                lbl_FaceID.Text = face_info[0];
                lbl_gender.Text = face_info[1];
                lbl_age.Text = face_info[2];
                lbl_glasses.Text = face_info[3];
                lbl_hair.Text = face_info[4];
                lbl_anger.Text = face_info[5];
                lbl_disgust.Text = face_info[6];
                lbl_fear.Text = face_info[7];
                lbl_happiness.Text = face_info[8];
                lbl_neutral.Text = face_info[9];
                lbl_sadness.Text = face_info[10];
                lbl_surprise.Text = face_info[11];
            }
            catch
            {

            }
        }

        private void tmr_count_Tick(object sender, EventArgs e)
        {
            if (cnt < 1)
            {
                lbl_cnt.Visible = false;
                tmr_count.Enabled = false;
                pnl_blink.Visible = true;
                pnl_blink.Visible = false;
                cnt = 3;
                Capture_face();
            }
            else
            {
                lbl_cnt.Visible = true;
                lbl_cnt.Text = cnt.ToString();
                cnt--;
            }
        }

        private void btn_face_Click(object sender, EventArgs e)
        {
            tmr_count.Enabled = true;
        }
        public void Capture_face()
        {
            try
            {
                string save_name = DateTime.Now.ToString("yyyy-MM-dd-hh시mm분ss초");

                Cv.SaveImage("../../Image_face/" + save_name + ".jpg", scr_laptopcam);
                using (IplImage ipl = new IplImage("../../Image_face/" + save_name + ".jpg", LoadMode.AnyColor))
                {
                    picbox_face_origin.ImageIpl = ipl;
                }

                // Execute the REST API call.
                MakeAnalysisRequest_face("../../Image_face/" + save_name + ".jpg");
            }
            catch
            {

            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lbl_cnt.Parent = picbox_face_origin;

            try
            {
                capture_laptopcam = CvCapture.FromCamera(CaptureDevice.DShow, 0); // 노트북일경우 0은 내장카메라 , 1은 외장카메라 // 데스크탑일 경우 처음 연결한게 0
                capture_laptopcam.SetCaptureProperty(CaptureProperty.FrameWidth, 640);    // 화면의 크기(pictureBoxIpl의 width보다 같거나 작으면 됨
                capture_laptopcam.SetCaptureProperty(CaptureProperty.FrameHeight, 480);   // 화면의 크기(pictureBoxIpl의 height보다 같거나 작으면 됨
                tmr_CAM.Enabled = true;
            }
            catch
            {
                tmr_CAM.Enabled = false;
            }
        }
        const string subscriptionKey_face = "d306729ef0aa482c821c78244ec3cb03";
        const string uriBase_FACE = "https://kohbyeongsoo-face.cognitiveservices.azure.com/face/v1.0/detect";

        async void MakeAnalysisRequest_face(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey_face);

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request parameters
            queryString["returnFaceId"] = "true";
            //queryString["returnFaceLandmarks"] = "true";
            queryString["returnFaceAttributes"] = "age,gender,smile,facialHair,glasses,headPose,emotion,hair";
            queryString["recognitionModel"] = "recognition_02";
            queryString["returnRecognitionModel"] = "true";
            queryString["detectionModel"] = "detection_01";

            // Assemble the URI for the REST API Call.
            //string uri = uriBase + "?" + requestParameters;
            string uri = uriBase_FACE + "?" + queryString;

            HttpResponseMessage response;
            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes(imageFilePath);

            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response = await client.PostAsync(uri, content);
                try
                {
                    response = await client.PostAsync(uri, content);
                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    // Display the JSON response.
                    Console.WriteLine("\nResponse:\n");
                    Console.WriteLine(JsonPrettyPrint(contentString));

                    //analyze_image A_image = JsonConvert.DeserializeObject<analyze_image>(contentString);
                    //var @object = JArray.Parse(contentString);

                    //string temp = @object[0].First.ToString();

                    if (contentString != "[]")
                    {
                        JArray json = JArray.Parse(contentString);
                        face_info[0] = json[0]["faceId"].ToString();
                        face_info[1] = json[0]["faceAttributes"]["gender"].ToString();
                        face_info[2] = json[0]["faceAttributes"]["age"].ToString();
                        face_info[3] = json[0]["faceAttributes"]["glasses"].ToString();
                        face_info[4] = json[0]["faceAttributes"]["hair"]["hairColor"][0]["color"].ToString();
                        face_info[5] = (double.Parse(json[0]["faceAttributes"]["emotion"]["anger"].ToString()) * 100).ToString();
                        face_info[6] = (double.Parse(json[0]["faceAttributes"]["emotion"]["disgust"].ToString()) * 100).ToString();
                        face_info[7] = (double.Parse(json[0]["faceAttributes"]["emotion"]["fear"].ToString()) * 100).ToString();
                        face_info[8] = (double.Parse(json[0]["faceAttributes"]["emotion"]["happiness"].ToString()) * 100).ToString();
                        face_info[9] = (double.Parse(json[0]["faceAttributes"]["emotion"]["neutral"].ToString()) * 100).ToString();
                        face_info[10] = (double.Parse(json[0]["faceAttributes"]["emotion"]["sadness"].ToString()) * 100).ToString();
                        face_info[11] = (double.Parse(json[0]["faceAttributes"]["emotion"]["surprise"].ToString()) * 100).ToString();

                    }
                    else
                    {
                        face_info[0] = "";
                        face_info[1] = "";
                        face_info[2] = "";
                        face_info[3] = "";
                        face_info[4] = "";
                        face_info[5] = "";
                        face_info[6] = "";
                        face_info[7] = "";
                        face_info[8] = "";
                        face_info[9] = "";
                        face_info[10] = "";
                        face_info[11] = "";

                        pnl_blink.BackColor = Color.Black;
                        pnl_blink.Visible = true;
                        pnl_blink.Visible = false;
                        pnl_blink.BackColor = Color.White;

                    }
                }
                catch
                {

                }
            }

        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cv.ReleaseImage(scr_laptopcam);
            if (scr_laptopcam != null) scr_laptopcam.Dispose();
        }
    }
}
