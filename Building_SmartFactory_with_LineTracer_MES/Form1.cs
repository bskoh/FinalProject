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

using MySql.Data.MySqlClient;
using System.Data;

using System.ComponentModel;
using System.Runtime.InteropServices;

using WinFormAnimation;

namespace FinalProejct_MES
{
    public partial class Form1 : Form
    {
        //CAM 관련
        CvCapture capture_webcam;
        IplImage scr_webcam;
        int OCR_reading_cnt = 0;
        int OCR_reading_cnt_retry = 5;

        IplImage rotate;

        public byte[,,] inImage, outImage;

        int i = 0, j = 0;
        //int pos_x, pos_y;

        //포지션 x,y각각 배열로 선언하기
        // unknown, pick, drop, pick stay, drop stay
        int[] pos_x = { 0, 482, 21, 181, 362, 0 };
        int[] pos_y = { 0, 291, 291, 483, 74, 0 };

        private readonly Animator2D pick_to_predrop = new Animator2D();
        private readonly Animator2D predrop_to_dropstay = new Animator2D();
        private readonly Animator2D dropstay_to_drop = new Animator2D();
        private readonly Animator2D drop_to_prepick = new Animator2D();
        private readonly Animator2D prepick_to_pickstay = new Animator2D();
        private readonly Animator2D pickstay_to_pick = new Animator2D();

        string dataIn_stage = "", temp_stage = "";
        string dataIn_agv = "", temp_agv = "";
        char sperator = ',';
        string[] law_stage;
        string[] law_agv;
        bool DataReceiveAble = false;

        int cnt_snd_stage = 0;
        int cnt_snd_agv = 0;
        int cnt_snd_stage_pre = 0;
        int cnt_snd_agv_pre = 0;

        public string[] face_info = new string[30];
        public string[] OCR = new string[6] { "language", "textAngle", "orientation", "boundingBox", "text", "text" };

        //STAGE RCV value
        string  S_Cnt_MES_Stage             = "";
        int  S_Num_SEQ_stage             = 0;
        bool S_Exist_Drop_stage          = Const.EMPTY;
        bool S_Exist_Pick_stage          = Const.EMPTY;
        bool S_Exist_Buffer_stage        = Const.EMPTY;
        bool S_Exist_Drop_AGV            = Const.EMPTY;
        bool S_Exist_Pick_AGV            = Const.EMPTY;
        bool S_Motor_Conv1               = Const.STOP_STAGE;
        bool S_Motor_Conv2               = Const.STOP_STAGE;
        bool S_Drop_ready                = Const.NOTREADY;
        bool S_Pick_ready                = Const.NOTREADY;
        bool S_Stopper_open              = Const.CLOSE;

        //AGV RCV value
        
        string      A_Cnt_MES_AGV          = "";
        int      A_Num_SEQ_AGV          = 0;
        int      A_Stat_Move            = Const.STOP_AGV;
        int      A_Stat_ing             = Const.STOP_AGV;
        bool     A_Exist_AGV            = Const.EMPTY;
        int      A_Stat_arriv           = Const.PICK;
        int      A_Stat_compt           = Const.FAIL;
        string   A_RFID                 = "";

        string[] RFID_Loc = new string[]
        {
            "11111111",
            "2099e1267e",            //Pick
            "a2e3053074",            //Drop
            "45a5e52b2e",            //Pick stay
            "b6c008255b",            //Drop stay
            "a78d3a6373",            //Pre Pick stay
            "87e06e636a",            //Pre Drop stay
            "8"
        };

        string[] Password_en = new string[] { "TRUE", "PASS", "YES", "SSED", "SSVD","true","pass","yes","ssed","ssvd"};
        string[] Password_ko = new string[] { "합", "양품"};
        string[] Failword_en = new string[] { "FALSE", "FAIL", "NO", "X", "FAI","false","fail","no","x","fai"};
        string[] Failword_ko = new string[] { "불", "불량품"};

        int cnt = 3;

        int Seq_pick = 0;
        int Seq_drop = 0;
        int Seq_stage = 0;

        int Seq_pick_delay = 0;
        int Seq_drop_delay = 0;
        int Seq_stage_delay = 0;

        string Rfid_temp = "";          // UI Refresh 위한 RFID 임시 저장값

        //DBDBDBDBDBDBDB
        
        public MemoryStream ms = new MemoryStream();
        public byte[] Blob_image;

        string[] DB_data_AGV = new string[] { "1989-10-02 20:20:04", "0", "Unkn", "0", "UnknProdct" };
        string[] DB_data_Stage = new string[] { "1989-10-02 20:20:04", "0", "0", "UnknStauts", "UnknStauts" };
        string[] DB_data_Inspection = new string[] { "1989-10-02 20:20:04", "UnknProdct", "data", "Unkn" };

        Form2 frm2 = new Form2();
        Form3 frm3 = new Form3();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 애니메이션 (MAP, AGV)
            pb_AGV.Parent = pb_map;
            pb_AGV.BackColor = Color.Transparent;
            pb_map.Location = new Point(20, 20);
            pb_map.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_AGV.Location = new Point(130, -10);
            pb_AGV.SizeMode = PictureBoxSizeMode.StretchImage;

            pick_to_predrop.Paths = new Path2D(new WinFormAnimation.Path(-10, -10, 2000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(300, 390, 2000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(-10, 90, 2000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(390, 390, 2000, AnimationFunctions.Liner)));

            predrop_to_dropstay.Paths = new Path2D(new WinFormAnimation.Path(90, 220, 1000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(390, 390, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(220, 480, 4000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(390, 640, 4000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(480, 580, 1000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(640, 640, 1000, AnimationFunctions.Liner))));

            dropstay_to_drop.Paths = new Path2D(new WinFormAnimation.Path(580, 700, 2000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(640, 640, 2000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(700, 700, 3000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(640, 300, 3000, AnimationFunctions.Liner)));

            drop_to_prepick.Paths = new Path2D(new WinFormAnimation.Path(700, 700, 1000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(300, 230, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(700, 600, 1000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(230, 230, 1000, AnimationFunctions.Liner)));

            prepick_to_pickstay.Paths = new Path2D(new WinFormAnimation.Path(600, 480, 2000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(230, 230, 2000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(480, 230, 4000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(230, -10, 4000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(230, 130, 2000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(-10, -10, 2000, AnimationFunctions.Liner))));

            pickstay_to_pick.Paths = new Path2D(new WinFormAnimation.Path(130, -10, 4000, AnimationFunctions.Liner),
                new WinFormAnimation.Path(-10, -10, 4000, AnimationFunctions.CubicEaseOut))
                .ContinueTo(new Path2D(
                    new WinFormAnimation.Path(-10, -10, 4000, AnimationFunctions.Liner),
                    new WinFormAnimation.Path(-10, 300, 4000, AnimationFunctions.Liner)));

            // 픽쳐박스 설정
            pb_Conv_Pick.Parent = pb_map;
            pb_Conv_Pick.BackColor = Color.Transparent;
            pb_Conv_Pick.Location = new Point(50, 320);
            pb_Conv_Pick.SizeMode = PictureBoxSizeMode.StretchImage;

            pb_Conv_Drop.Parent = pb_map;
            pb_Conv_Drop.BackColor = Color.Transparent;
            pb_Conv_Drop.Location = new Point(400, 320);
            pb_Conv_Drop.SizeMode = PictureBoxSizeMode.StretchImage;

            pb_Stopper.Parent = pb_map;
            pb_Stopper.BackColor = Color.Transparent;
            pb_Stopper.Location = new Point(330, 260);
            pb_Stopper.SizeMode = PictureBoxSizeMode.StretchImage;

            pb_Cam.Parent = pb_map;
            pb_Cam.BackColor = Color.Transparent;
            pb_Cam.Location = new Point(400, 240);
            pb_Cam.SizeMode = PictureBoxSizeMode.StretchImage;

            pb_BoxDrop.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_BoxPick.SizeMode = PictureBoxSizeMode.StretchImage;

            tmr_UI_Refresh.Enabled = true;

            
            frm3.Show();
            //Camera 연결
            try
            {
                capture_webcam = CvCapture.FromCamera(CaptureDevice.DShow, 1); // 노트북일경우 0은 내장카메라 , 1은 외장카메라 // 데스크탑일 경우 처음 연결한게 0
                capture_webcam.SetCaptureProperty(CaptureProperty.FrameWidth, 640);    // 화면의 크기(pictureBoxIpl의 width보다 같거나 작으면 됨
                capture_webcam.SetCaptureProperty(CaptureProperty.FrameHeight, 480);   // 화면의 크기(pictureBoxIpl의 height보다 같거나 작으면 됨
                tmr_CAM.Enabled = true;
            }
            catch
            {
                tmr_CAM.Enabled = false;
            }
            

            //serial port
            string[] comlist = System.IO.Ports.SerialPort.GetPortNames();
            

            if (comlist.Length > 0)
            {
                cbbox_portlist_AGV.Items.AddRange(comlist);
                cbbox_portlist_stage.Items.AddRange(comlist);
                //제일 처음에 위치한 녀석을 선택
                cbbox_portlist_AGV.SelectedIndex = 0;
                cbbox_portlist_stage.SelectedIndex = 0;
            }
            
            

        }


        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIn_stage = Serial_stage.ReadExisting();
            this.Invoke(new EventHandler(showdata_stage));
        }


        private void Serial_agv_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIn_agv = Serial_agv.ReadExisting();
            this.Invoke(new EventHandler(showdata_AGV));
        }

        private void GetRawData_stage(string[] str)
        {
            try
            {
                S_Cnt_MES_Stage = str[0];
                S_Num_SEQ_stage = int.Parse(str[1]);
                if (str[2] == "1") { S_Exist_Drop_AGV = true; } else { S_Exist_Drop_AGV = false; };
                if (str[3] == "1") { S_Exist_Drop_stage = true; } else { S_Exist_Drop_stage = false; };
                if (str[4] == "1") { S_Exist_Buffer_stage = true; } else { S_Exist_Buffer_stage = false; };
                if (str[5] == "1") { S_Exist_Pick_stage = true; } else { S_Exist_Pick_stage = false; };
                if (str[6] == "1") { S_Exist_Pick_AGV = true; } else { S_Exist_Pick_AGV = false; };
                if (str[7] == "1") { S_Motor_Conv1 = true; } else { S_Motor_Conv1 = false; };
                if (str[8] == "1") { S_Motor_Conv2 = true; } else { S_Motor_Conv2 = false; };
                if (str[9] == "1") { S_Drop_ready = true; } else { S_Drop_ready = false; };
                if (str[10] == "1") { S_Pick_ready = true; } else { S_Pick_ready = false; };

                str[11] = str[11].Substring(0, str[11].Length - 1);

                if (str[11] == "1") { S_Stopper_open = true; } else { S_Stopper_open = false; };

                DB_data_Stage = new string[frm3.Col_Stage.Length];

                DB_data_Stage[0] = DateTime.Now.ToString("HH:mm:ss.fff");

                if (S_Exist_Pick_stage) DB_data_Stage[1] = "1"; else DB_data_Stage[1] = "0";
                if (S_Exist_Drop_stage) DB_data_Stage[2] = "1"; else DB_data_Stage[2] = "0";
                if (S_Motor_Conv1) DB_data_Stage[3] = "1"; else DB_data_Stage[3] = "0";
                if (S_Motor_Conv2) DB_data_Stage[4] = "1"; else DB_data_Stage[4] = "0";
            }
            catch
            {

            }
        }

        private void GetRawData_AGV(string[] str)
        {
            
            A_Cnt_MES_AGV     = str[0];
            A_Num_SEQ_AGV     = int.Parse(str[1]);
            A_Stat_Move       = int.Parse(str[2]);
            A_Stat_ing        = int.Parse(str[3]);
            if (str[4] == "1") { A_Exist_AGV = true; } else { A_Exist_AGV = false; };
            A_Stat_arriv      = int.Parse(str[5]);
            A_Stat_compt      = int.Parse(str[6]);
            A_RFID = str[7] + str[8] + str[9] + str[10] + str[11];

            A_RFID = A_RFID.Substring(0, A_RFID.Length - 1);


            DB_data_AGV = new string[frm3.Col_Stage.Length];

            DB_data_AGV[0] = DateTime.Now.ToString("HH:mm:ss.fff");
            DB_data_AGV[1] = "1";

            for (int i = 0; i < RFID_Loc.Length; i++)
            {
                if (A_RFID == RFID_Loc[i])
                {
                    DB_data_AGV[2] = i.ToString();
                    break;
                }
            }
            if (A_Exist_AGV) DB_data_AGV[3] = "1"; else DB_data_AGV[3] = "0";

            DB_data_AGV[4] = lbl_OCR.Text;

        }

        private void showdata_stage(object sender, EventArgs e)
        {
            temp_stage += dataIn_stage;

            if (dataIn_stage.Contains("\r"))
            {
                //if (temp_stage.Contains(cnt_snd_stage_pre.ToString("D3")) || temp_stage.Contains("e"))
                //{
                if (temp_stage.Length == 28)
                {
                    frm3.ADD_StageComm("[RCV] : " + temp_stage);
                    law_stage = temp_stage.Split(sperator);
                    GetRawData_stage(law_stage);
                    temp_stage = "";
                    cnt_snd_stage_pre = 0;
                }
                temp_stage = "";
                //}
            }
        }
        private void showdata_AGV(object sender, EventArgs e)
        {
            temp_agv += dataIn_agv;

            if (dataIn_agv.Contains("\r"))
            {
                //if (temp_agv.Contains(cnt_snd_agv_pre.ToString("D3")) || temp_agv.Contains("e"))
                //{
                if (temp_agv.Length == 33)
                {
                    frm3.ADD_AGVComm("[RCV] : " + temp_agv);
                    law_agv = temp_agv.Split(sperator);
                    GetRawData_AGV(law_agv);
                    cnt_snd_agv_pre = 0;
                }

                temp_agv = "";
                //}
            }
        }


        public bool SendDataToStage(string cmd = "stat", int loc = 0)
        {
            string str = "";

            cnt_snd_stage++;
            if (cnt_snd_stage > 999)
                cnt_snd_stage = 1;
            //
            //if (cnt_snd_stage_pre == 0)
            //{
                cnt_snd_stage_pre = cnt_snd_stage;
                str = cnt_snd_stage.ToString("D3") + "," + cmd + "," + loc.ToString();
                Serial_stage.WriteLine(str);
                frm3.ADD_StageComm("[SND] : " + str);
                return true;
            //}
            //return false;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cv.ReleaseImage(scr_webcam);
            if (scr_webcam != null) scr_webcam.Dispose();

            if (Serial_agv.IsOpen)
            {
                Serial_agv.Close();
            }
            if(Serial_stage.IsOpen)
            {
                Serial_stage.Close();
            }

            if (tmr_stage.Enabled) tmr_stage.Enabled = false;
            if (tmr_PICK.Enabled) tmr_PICK.Enabled = false;
            if (tmr_DROP.Enabled) tmr_DROP.Enabled = false;
            if (tmr_DB.Enabled) tmr_DB.Enabled = false;

        }

        public void capture_OCR()
        {
            try
            {

                string save_name = "OCR" + DateTime.Now.ToString("yyyy-MM-dd-hh시mm분ss초");
                Cv.SaveImage("../../Image_OCR/" + save_name + ".jpg", scr_webcam);
                using (IplImage ipl = new IplImage("../../Image_OCR/" + save_name + ".jpg", LoadMode.AnyColor))
                {
                    picbox_OCR_result.ImageIpl = ipl;
                    picbox_OCR_result.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                Blob_image = ms.ToArray();
            
                // Execute the REST API call.
                string language;
                if (rb_eng.Checked == true)
                    language = "en";
                else if (rb_kor.Checked == true)
                    language = "ko";
                else
                    language = "unk";

                DB_data_Inspection[3] = "Unkn";
                MakeAnalysisRequest("../../Image_OCR/" + save_name + ".jpg", language);

                // listBox1.Items.Add("캡쳐");    
                // 캡쳐가 되었다는 것을 listBox1에 출력한다.
            }
            catch
            {

            }
        }
        private void btn_capture_Click(object sender, EventArgs e)
        {
            capture_OCR();
        }


        private bool SendDataToAGV(string cmd = "stat")
        {
            string str = "";

            //str = cnt_snd_agv.ToString("D3") + "," + cmd;
            //frm3.ADD_AGVComm("[SND] : " + str);
            //return true;



            cnt_snd_agv++;
            if (cnt_snd_agv > 999)
                cnt_snd_agv = 1;
            /*
            if (cnt_snd_agv_pre == 0)
            {
                cnt_snd_agv_pre = cnt_snd_agv;
                */
            str = cnt_snd_agv.ToString("D3") + "," + cmd;
            Serial_agv.WriteLine(str);
            frm3.ADD_AGVComm("[SND] : " + str);
            return true;
            // }

            //return false;                      
        }



        /* 유사도 검출 함수
         */
         /*
        IplImage match;
        public IplImage Templit(IplImage src, IplImage temp)
        {
            match = src;
            IplImage templit = temp;
            IplImage tm = new IplImage(new CvSize(match.Size.Width - templit.Size.Width + 1, match.Size.Height - templit.Size.Height + 1), BitDepth.F32, 1);

            CvPoint minloc, maxloc;
            Double minval, maxval;

            Cv.MatchTemplate(match, templit, tm, MatchTemplateMethod.SqDiffNormed);

            Cv.MinMaxLoc(tm, out minval, out maxval, out minloc, out maxloc);

            Cv.DrawRect(match, new CvRect(minloc.X, minloc.Y, templit.Width, templit.Height), CvColor.Red, 3);

            return match;
        }
        
        public void Dispose()
        {
            if (match != null) Cv.ReleaseImage(match);
        }
        */
        const string subscriptionKey_anal = "06b3728ec92f41cebb35ac0855007a33";
        const string subscriptionKey_face = "d306729ef0aa482c821c78244ec3cb03";
        const string uriBase = "https://koreacentral.api.cognitive.microsoft.com/vision/v1.0/analyze";
        const string uriBase_OCR = "https://koreacentral.api.cognitive.microsoft.com/vision/v1.0/ocr";
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
                    face_info[0]    = json[0]["faceId"].ToString();
                    face_info[1]    = json[0]["faceAttributes"]["gender"].ToString();
                    face_info[2]    = json[0]["faceAttributes"]["age"].ToString();
                    face_info[3]    = json[0]["faceAttributes"]["glasses"].ToString();
                    face_info[4]    = json[0]["faceAttributes"]["hair"]["hairColor"][0]["color"].ToString();
                    face_info[5]    = (double.Parse(json[0]["faceAttributes"]["emotion"]["anger"].ToString()) * 100).ToString();
                    face_info[6]    = (double.Parse(json[0]["faceAttributes"]["emotion"]["disgust"].ToString()) * 100).ToString();
                    face_info[7]    = (double.Parse(json[0]["faceAttributes"]["emotion"]["fear"].ToString()) * 100).ToString();
                    face_info[8]    = (double.Parse(json[0]["faceAttributes"]["emotion"]["happiness"].ToString()) * 100).ToString();
                    face_info[9]    = (double.Parse(json[0]["faceAttributes"]["emotion"]["neutral"].ToString()) * 100).ToString();
                    face_info[10]   = (double.Parse(json[0]["faceAttributes"]["emotion"]["sadness"].ToString()) * 100).ToString();
                    face_info[11]   = (double.Parse(json[0]["faceAttributes"]["emotion"]["surprise"].ToString()) * 100).ToString();

                }
                else
                {
                    face_info[0] =  ""; 
                    face_info[1] =  "";
                    face_info[2] =  "";
                    face_info[3] =  "";
                    face_info[4] =  "";
                    face_info[5] =  "";
                    face_info[6] =  "";
                    face_info[7] =  "";
                    face_info[8] =  "";
                    face_info[9] =  "";
                    face_info[10] = "";
                    face_info[11] = "";
                    
                }
            }
            
        }
        /// <summary>
        /// Gets the analysis of the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        async void MakeAnalysisRequest(string imageFilePath, string language = "unk")
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey_anal);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "visualFeatures=Categories,Description,Adult,Faces,Tags&language=en";
            string requestParameters_OCR = "language=" + language;

            // Assemble the URI for the REST API Call.
            //string uri = uriBase + "?" + requestParameters;
            string uri = uriBase_OCR + "?" + requestParameters_OCR;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();





                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                Console.WriteLine(JsonPrettyPrint(contentString));

                //analyze_image A_image = JsonConvert.DeserializeObject<analyze_image>(contentString);
                JObject Jsonobject = JObject.Parse(contentString);

                //string temp = Jsonobject["categories"][0]["name"].ToString();
                try
                {
                    OCR[0] = Jsonobject["language"].ToString();
                    OCR[1] = Jsonobject["textAngle"].ToString();
                    OCR[2] = Jsonobject["orientation"].ToString();
                    if (OCR[2] != "NotDetected")
                    {
                        OCR[3] = Jsonobject["regions"][0]["lines"][0]["words"][0]["boundingBox"].ToString();
                        OCR[4] = Jsonobject["regions"][0]["lines"][0]["words"][0]["text"].ToString();
                        OCR[4] = OCR[4].ToUpper();

                        string[] OCR_pos_str = OCR[3].Split(',');
                        int[] OCR_pos = new int[4];

                        for (int i = 0; i < OCR_pos.Length; i++)
                        {
                            OCR_pos[i] = int.Parse(OCR_pos_str[i]);
                        }

                    ///////////////////////////////////

                    //Bitmap bit = new Bitmap(picbox_OCR_result.Image);




                    //Bitmap bit = new Bitmap(Rotate(picbox_OCR_orgin.ImageIpl, int.Parse(OCR[1])).ToBitmap());
                    /*
                        int inW = bit.Width;
                        int inH = bit.Height;

                        byte[,,] inImage = new byte[3, inW, inH];

                    for (int i = 0; i < inW; i++)
                        {
                            for (int j = 0; j < inH; j++)
                            {
                                Color c = bit.GetPixel(i, j);
                                inImage[0, i, j] = c.R;
                                inImage[1, i, j] = c.G;
                                inImage[2, i, j] = c.B;
                            }
                        }

                        outImage = new byte[3, inImage.GetLength(1), inImage.GetLength(0)];

                        int angle = int.Parse(OCR[1]);

                        double radian = angle * Math.PI / 180.0;

                        int center_x = inW / 2;
                        int center_y = inH / 2;

                        for (int i = 0; i < outImage.GetLength(0); i++)
                        {
                            for (int j = 0; j < outImage.GetLength(1); j++)
                            {
                                int oldI = (int)(Math.Cos(radian) * (i - center_x)
                                            + Math.Sin(radian) * (j - center_y)) + center_x;
                                int oldJ = (int)(-Math.Sin(radian) * (i - center_x)
                                            + Math.Cos(radian) * (j - center_y)) + center_y;
                                if ((0 <= oldI && oldI < outImage.GetLength(0)) && (0 <= oldJ && oldJ < outImage.GetLength(1)))
                                    outImage[i, j] = inImage[oldI, oldJ];
                            }
                        }

                        bit = outImage;

                    */
                    /////////////////////////

                    lbl_OCR_angle.Text = OCR[2] + "  " + OCR[1] + "˚ ";

                        DB_data_Inspection[0] = DateTime.Now.ToString("HH:mm:ss.fff");
                        DB_data_Inspection[1] = OCR[4];

                        //image 넣어줘야함
                        //DB_data_Inspection[2] = "data";


                        //DB에 image 넣을때 사용
                        //picbox_OCR_result.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //Blob_image = ms.ToArray();

                        lbl_OCR.Text = OCR[4];

                    if (OCR[0] == "en")
                    {
                        for (int i = 0; i < Password_en.Length; i++)
                        {
                            if (Password_en.Contains(OCR[4])) 
                            {
                                DB_data_Inspection[3] = "PASS";
                            }
                            else
                            {
                                for (int j = 0; j < Failword_en.Length; j++)
                                {
                                    if (Failword_en.Contains(OCR[4]))
                                    {
                                        DB_data_Inspection[3] = "FAIL";
                                    }
                                    else
                                    {
                                        DB_data_Inspection[3] = "UNKN";
                                    }
                                }
                            }
                        }
                    }
                    else if (OCR[0] == "ko")
                    {
                        for (int i = 0; i < Password_ko.Length; i++)
                        {
                            if (Password_ko.Contains(OCR[4]))
                            {
                                DB_data_Inspection[3] = "PASS";
                            }
                            else
                            {
                                for (int j = 0; j < Failword_ko.Length; j++)
                                {
                                    if (Failword_ko.Contains(OCR[4]))
                                    {
                                        DB_data_Inspection[3] = "FAIL";
                                    }
                                    else
                                    {
                                        DB_data_Inspection[3] = "UNKN";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        DB_data_Inspection[3] = "UNKN";
                    }
                        /*
                        if (OCR[4] == "PASS" || OCR[4] == "SSVD")
                        {
                            DB_data_Inspection[3] = "PASS";
                        }
                        else
                        {
                            DB_data_Inspection[3] = "FAIL";
                        }
                        */

                        OCR[5] = DateTime.Now.ToString("HHmmss") + OCR[4];


                        DB_data_Inspection[1] = OCR[5];
                        DB_data_Inspection[2] = "data";//Blob_image.ToString();
                                                       //DB_data_Inspection[3] = OCR[4];

                    //frm3.DB_insert("Inspection", frm3.Col_Inspection, DB_data_Inspection);

                    Bitmap bit;

                        if (OCR[2] == "Up")
                    {
                        bit = new Bitmap(Rotate(picbox_OCR_orgin.ImageIpl, 360).ToBitmap());
                        picbox_OCR_result.Image = bit.Clone(new Rectangle(OCR_pos[0], OCR_pos[1], OCR_pos[2], OCR_pos[3])
                                                                  , System.Drawing.Imaging.PixelFormat.DontCare);
                    }
                        else if (OCR[2] == "Down")
                        {
                            bit = new Bitmap(Rotate(picbox_OCR_orgin.ImageIpl, 180).ToBitmap());
                        picbox_OCR_result.Image = bit.Clone(new Rectangle(OCR_pos[0], OCR_pos[1], OCR_pos[2], OCR_pos[3])
                                                                  , System.Drawing.Imaging.PixelFormat.DontCare);


                    }
                        else if (OCR[2] == "Left")
                    {
                        bit = new Bitmap(Rotate(picbox_OCR_orgin.ImageIpl, 270).ToBitmap());
                        picbox_OCR_result.Image = bit.Clone(new Rectangle(OCR_pos[0]+80, OCR_pos[1]-80, OCR_pos[2], OCR_pos[3])
                                                                  , System.Drawing.Imaging.PixelFormat.DontCare);

                    }
                        else if (OCR[2] == "Right")
                    {
                        bit = new Bitmap(Rotate(picbox_OCR_orgin.ImageIpl, 90).ToBitmap());
                        picbox_OCR_result.Image = bit.Clone(new Rectangle(OCR_pos[0]+80, OCR_pos[1]-80, OCR_pos[2], OCR_pos[3])
                                                                  , System.Drawing.Imaging.PixelFormat.DontCare);

                    }
                    else
                        {

                        }

                }
                    else
                    {
                        using (IplImage ipl = new IplImage("C:/Users/kccistc/Desktop/OneDrive/report/FinalProject/image/kakao_qeu_white.png", LoadMode.AnyColor))
                        {
                            picbox_OCR_result.ImageIpl = ipl;
                        }
                    }

                }
                    catch
                {

                }


        }
        }
        public void Parse_json(string str = "")
        {
            JObject Jsonobject = JObject.Parse(str);

            string temp = Jsonobject["categories"][0]["name"].ToString();
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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tmr_count_Tick(object sender, EventArgs e)
        {
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


        private void tmr_CAM_Tick(object sender, EventArgs e)
        {
            scr_webcam = capture_webcam.QueryFrame();
            picbox_OCR_orgin.ImageIpl = scr_webcam;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm2.ShowDialog();
        }

        private void tmr_PICK_Tick(object sender, EventArgs e)
        {

            switch (Seq_pick)
            {
                case 0: //현재 상태 확인용
                    if (S_Pick_ready && (A_Exist_AGV == Const.EMPTY))
                    {
                        Seq_pick++;
                    }
                    else
                    {

                    }
                    break;

                case 1: // Stage가 ready 상태임을 확인
                    if (S_Pick_ready == Const.READY
                     && A_Exist_AGV == Const.EMPTY)
                    {
                        Seq_pick++;
                    }
                    else
                    {
                        Seq_pick--;
                    }
                    break;

                case 2: //stage가 ready 상태이므로 AGV에 move 명령 보냄
                    if ((A_RFID == RFID_Loc[Const.PICK_STAY]
                        || A_RFID == RFID_Loc[Const.PICK])
                        && S_Exist_Pick_stage == Const.EXIST)
                    {
                        Seq_pick++;
                    }
                    break;

                case 3:  
                    if (SendDataToAGV("pick"))
                    {
                        Seq_pick++;
                    }
                    break;

                case 4: //AGV pick stage 도착 대기
                    //if (SendDataToAGV() && SendDataToStage())
                    //{
                        if ((A_Stat_Move == Const.STOP_AGV
                            && A_RFID == RFID_Loc[Const.PICK])
                            || S_Exist_Pick_AGV == Const.EXIST)
                        {
                            Seq_pick++;
                        }
                    //}
                    break;

                case 5: //AGV pick stage 도착완료, pick 명령 보냄
                    //if (SendDataToAGV("pick"))
                    Seq_pick++;
                    break;

                case 6: //pick 완료 확인
                    //if (SendDataToAGV())
                    //{
                        if (A_Stat_compt == Const.DFLT)
                        {
                            //pick 명령 완료 대기
                        }
                        else
                        {
                            if (A_Stat_compt == Const.SUCC)
                            {
                                Seq_pick++;
                            }
                            else if (A_Stat_compt == Const.FAIL)
                            {
                                if (Seq_pick_delay > 3)
                                {
                                    Seq_pick_delay = 0;
                                    Seq_pick = 2;
                                }
                                Seq_pick_delay++;
                            }
                        }
                    //}
                    break;

                case 7:
                    Seq_pick = 0;
                    Seq_drop = 1;
                    break;
                default:
                    break;
            }
        }
        private void tmr_DROP_Tick(object sender, EventArgs e)
        {

            switch (Seq_drop)
            {
                case 0: //현재 상태 확인용
                    if (S_Drop_ready && (A_Exist_AGV == Const.EXIST))
                    {
                        Seq_drop++;
                    }
                    else
                    {

                    }
                    break;

                case 1: // Stage가 ready 상태임을 확인
                    if (S_Drop_ready == Const.READY
                     && A_Exist_AGV == Const.EXIST)
                    {
                        Seq_drop++;
                    }
                    else
                    {
                        Seq_drop--;
                    }
                    break;

                case 2: //stage가 ready 상태이므로 AGV에 move 명령 보냄
                    if ((A_RFID == RFID_Loc[Const.DROP_STAY]
                        || A_RFID == RFID_Loc[Const.DROP])
                        && S_Exist_Drop_stage == Const.EMPTY)
                    {
                        Seq_drop++;

                        Seq_drop_delay = 0;
                    }
                    break;

                case 3:
                    if (SendDataToAGV("drop"))
                    {
                        Seq_drop++;
                    }
                    break;

                case 4: //AGV drop stage 도착 대기
                    //if (SendDataToAGV() && SendDataToStage())
                    //{
                    if ((A_Stat_Move == Const.STOP_AGV
                        && A_RFID == RFID_Loc[Const.DROP])
                        || S_Exist_Drop_AGV == Const.EXIST)
                    {
                        Seq_drop++;
                    }
                    else
                    {
                        if (Seq_drop_delay > 3)
                        {
                            Seq_drop_delay = 0;
                            Seq_drop--;
                        }
                        Seq_drop_delay++;
                    }
                    //}
                    break;

                case 5: //AGV drop stage 도착완료, Drop 명령 보냄
                    //if (SendDataToAGV("drop"))
                    Seq_drop++;
                    break;

                case 6: //Drop 완료 확인
                    //if (SendDataToAGV())
                    //{
                        if (A_Stat_compt == Const.DFLT)
                        {
                            //drop 명령 완료 대기
                        }
                        else
                        {
                            if (A_Stat_compt == Const.SUCC)
                            {
                                Seq_drop++;
                            }
                            else if (A_Stat_compt == Const.FAIL)
                            {
                                Seq_drop = 2;
                            }
                        }
                    //}
                    break;

                case 7:
                    Seq_drop = 0;
                    Seq_pick = 1;
                    break;
                default:
                    break;
            }
        }

        private void tmr_DB_Tick(object sender, EventArgs e)
        {
            //현재 변수를 배열에 최신화해서 넣어준다.
            //두번째 매개변수는 DB의 Field 값이라서 안넣어줘도 됨(바꾸고싶은것만 넣어줘도 된다)
            string dt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            DB_data_AGV[0] = dt;
            DB_data_AGV[1] = "1";

            for (int i = 0; i < RFID_Loc.Length; i++)
            {
                if (A_RFID == RFID_Loc[i])
                {
                    DB_data_AGV[2] = i.ToString();
                }
            }
            if (A_Exist_AGV == Const.EXIST) { DB_data_AGV[3] = "1"; } else { DB_data_AGV[3] = "0"; }
            DB_data_AGV[4] = OCR[5];
            
            frm3.DB_insert("AGV", frm3.Col_AGV, DB_data_AGV);

            DB_data_Stage[0] = dt;
            if (S_Exist_Pick_stage == Const.EXIST) { DB_data_Stage[1] = "1"; } else { DB_data_Stage[1] = "0"; }
            if (S_Exist_Drop_stage == Const.EXIST) { DB_data_Stage[2] = "1"; } else { DB_data_Stage[2] = "0"; }
            if (S_Motor_Conv1 == Const.EXIST) { DB_data_Stage[3] = "1"; } else { DB_data_Stage[3] = "0"; }
            if (S_Motor_Conv2 == Const.EXIST) { DB_data_Stage[4] = "1"; } else { DB_data_Stage[4] = "0"; }

            frm3.DB_insert("Stage", frm3.Col_Stage, DB_data_Stage);


        }

        private void btn_connect_AGV_Click(object sender, EventArgs e)
        {
            if (Serial_agv.IsOpen)
            {
                MessageBox.Show("{0} is already opened", Serial_agv.PortName.ToString());
            }
            else
            {
                try
                {
                    Serial_agv.PortName = cbbox_portlist_AGV.Text;
                    Serial_agv.BaudRate = Convert.ToInt32(cbbox_baudrate_AGV.Text);
                    Serial_agv.Open();

                    if (Serial_agv.IsOpen)
                    {
                        btn_connect_AGV.Enabled = false;
                        cbbox_portlist_AGV.Enabled = false;
                        btn_disconnect_AGV.Enabled = true;
                        cbbox_baudrate_AGV.Enabled = false;
                        
                        string[] comlist = System.IO.Ports.SerialPort.GetPortNames();

                        if (comlist.Length > 0)
                        {
                            cbbox_portlist_stage.Items.Clear();
                            cbbox_portlist_stage.Items.AddRange(comlist);
                            //제일 처음에 위치한 녀석을 선택
                            cbbox_portlist_stage.SelectedIndex = 0;
                        }

                        if (Serial_stage.IsOpen)
                        {
                            btn_Pick_SEQ_Start.Enabled = true;
                            btn_Drop_SEQ_Start.Enabled = true;
                            btn_Stage_SEQ_Start.Enabled = true;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("AGV Comm. Fail!!");
                }
            }
        }

        private void btn_disconnect_AGV_Click(object sender, EventArgs e)
        {
            if (Serial_agv.IsOpen == true)
            {
                Serial_agv.Close();

                btn_connect_AGV.Enabled = true;
                cbbox_portlist_AGV.Enabled = true;
                btn_disconnect_AGV.Enabled = false;
                cbbox_baudrate_AGV.Enabled = true;
            }
            if (tmr_stage.Enabled) tmr_stage.Enabled = false;
            if (tmr_PICK.Enabled) tmr_PICK.Enabled = false;
            if (tmr_DROP.Enabled) tmr_DROP.Enabled = false;
        }

        private void btn_connect_stage_Click(object sender, EventArgs e)
        {
            if (Serial_stage.IsOpen)
            {
                MessageBox.Show("{0} is already opened", Serial_stage.PortName.ToString());
            }
            else
            {
                try
                {
                    Serial_stage.PortName = cbbox_portlist_stage.Text;
                    Serial_stage.BaudRate = Convert.ToInt32(cbbox_baudrate_stage.Text);
                    Serial_stage.Open();

                    if (Serial_stage.IsOpen)
                    {
                        btn_connect_stage.Enabled = false;
                        cbbox_portlist_stage.Enabled = false;
                        btn_disconnect_stage.Enabled = true;
                        cbbox_baudrate_stage.Enabled = false;

                        string[] comlist = System.IO.Ports.SerialPort.GetPortNames();

                        if (comlist.Length > 0)
                        {
                            cbbox_portlist_stage.Items.Clear();
                            cbbox_portlist_stage.Items.AddRange(comlist);
                            //제일 처음에 위치한 녀석을 선택
                            cbbox_portlist_stage.SelectedIndex = 0;
                        }

                        if(Serial_agv.IsOpen)
                        {
                            btn_Pick_SEQ_Start.Enabled = true;
                            btn_Drop_SEQ_Start.Enabled = true;
                            btn_Stage_SEQ_Start.Enabled = true;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Stage Comm. Fail!!");
                }
            }
        }

        private void btn_disconnect_stage_Click(object sender, EventArgs e)
        {
            if (Serial_stage.IsOpen == true)
            {
                Serial_stage.Close();

                btn_connect_stage.Enabled = true;
                cbbox_portlist_stage.Enabled = true;
                btn_disconnect_stage.Enabled = false;
                cbbox_baudrate_stage.Enabled = true;
            }
            if (tmr_stage.Enabled) tmr_stage.Enabled = false;
            if (tmr_PICK.Enabled) tmr_PICK.Enabled = false;
            if (tmr_DROP.Enabled) tmr_DROP.Enabled = false;
        }

        private void btn_Pick_SEQ_Start_Click(object sender, EventArgs e)
        {
            //Serial_agv.WriteLine("001,stat");
            //SendDataToAGV();
            
            
            if (tmr_PICK.Enabled)
            {
                tmr_PICK.Enabled = false;
                btn_Pick_SEQ_Start.Text = "Pick Sequence Start";
            }
            else
            {
                tmr_PICK.Enabled = true;
                btn_Pick_SEQ_Start.Text = "Pick Sequence Stop";
            }
            
            
        }

        private void btn_Drop_SEQ_Start_Click(object sender, EventArgs e)
        {
            //SendDataToStage();

            //serial_stage.WriteLine("001,stat");

            
            if (tmr_DROP.Enabled)
            {
                tmr_DROP.Enabled = false;
                btn_Drop_SEQ_Start.Text = "Drop Sequence Start";
            }
            else
            {
                tmr_DROP.Enabled = true;
                btn_Drop_SEQ_Start.Text = "Drop Sequence Stop";
            }
            
            
        }
        public void Send_TestData(string str)
        {
            str = "999," + str;
            Serial_stage.WriteLine(str);
            frm3.ADD_StageComm(str);
        }
        private void btn_SendAuto_Click(object sender, EventArgs e)
        {
            Send_TestData("auto");
        }

        private void btn_SendManu_Click(object sender, EventArgs e)
        {
            Send_TestData("manu");
        }

        private void btn_PDok_Click(object sender, EventArgs e)
        {
            Send_TestData("pdok");
        }

        private void btn_PDNo_Click(object sender, EventArgs e)
        {
            Send_TestData("pdno");
        }

        private void tmr_DataCheck_Tick(object sender, EventArgs e)
        {
            SendDataToAGV();
            SendDataToStage();
        }

        private void btn_DBtest_Click(object sender, EventArgs e)
        {
            if (tmr_DB.Enabled) { tmr_DB.Enabled = false; } else { tmr_DB.Enabled = true; }
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Text.Contains("AGV"))
            {
                SendDataToAGV("rset");
                Seq_pick = 0;
                Seq_drop = 0;
            }
            else if(btn.Text.Contains("Stage"))
            {
                SendDataToStage("rset");
                Seq_stage = 0;
            }

        }

        private void btn_Stage_SEQ_Start_Click(object sender, EventArgs e)
        {
            if (tmr_stage.Enabled)
            {
                tmr_stage.Enabled = false;
                btn_Stage_SEQ_Start.Text = "Stage Sequence Start";
            }
            else
            {
                tmr_stage.Enabled = true;
                btn_Stage_SEQ_Start.Text = "Stage Sequence Stop";
            }
        }

        private void tmr_UI_Refresh_Tick(object sender, EventArgs e)
        {
            if (Serial_agv.IsOpen && Serial_stage.IsOpen)
                tmr_DataCheck.Enabled = true;
            else
                tmr_DataCheck.Enabled = false;

            // AGV Animation
            if(Rfid_temp != A_RFID)
            {
                // Pick stage -> Pre Drop stay
                if (A_RFID == RFID_Loc[1] && A_Exist_AGV == Const.EXIST        // AGV의 위치가 pickstage이고,
                    && A_Stat_Move == Const.FORWARD_AGV)                       // 물건이 있으며, 전진상태이면
                {
                    pb_AGV.Image = Properties.Resources.AGV_with_product_tr;
                    pick_to_predrop.Play(pb_AGV, Animator2D.KnownProperties.Location);
                    
                    Rfid_temp = A_RFID;
                }
                // Pre Drop stay -> Drop stay
                else if(A_RFID == RFID_Loc[6])
                {
                    pb_AGV.Image = Properties.Resources.AGV_with_product_tr;
                    predrop_to_dropstay.Play(pb_AGV, Animator2D.KnownProperties.Location);

                    Rfid_temp = A_RFID;
                }
                // Drop stay -> Drop stage
                else if(A_RFID == RFID_Loc[4] && A_Exist_AGV == Const.EXIST
                    && A_Stat_Move == Const.FORWARD_AGV)
                {
                    pb_AGV.Image = Properties.Resources.AGV_with_product_m_tr;
                    dropstay_to_drop.Play(pb_AGV, Animator2D.KnownProperties.Location);

                    Rfid_temp = A_RFID;
                }
                // Drop stage -> Pre Pick stay
                else if(A_RFID == RFID_Loc[2] && A_Exist_AGV == Const.EMPTY
                    && A_Stat_Move == Const.FORWARD_AGV)
                {
                    pb_AGV.Image = Properties.Resources.agv_empty_m;
                    drop_to_prepick.Play(pb_AGV, Animator2D.KnownProperties.Location);

                    Rfid_temp = A_RFID;
                }
                // Pre Pick stay -> Pick stay
                else if(A_RFID == RFID_Loc[5])
                {
                    pb_AGV.Image = Properties.Resources.agv_empty_m;
                    prepick_to_pickstay.Play(pb_AGV, Animator2D.KnownProperties.Location);

                    Rfid_temp = A_RFID;
                }
                // Pick stay -> Pick stage
                else if(A_RFID == RFID_Loc[3] && A_Exist_AGV == Const.EMPTY
                    && A_Stat_Move == Const.FORWARD_AGV)
                {
                    pb_AGV.Image = Properties.Resources.agv_empty;
                    pickstay_to_pick.Play(pb_AGV, Animator2D.KnownProperties.Location);

                    Rfid_temp = A_RFID;
                }
            }

            // Conv Product 위치
            // Drop stage
            if(S_Exist_Drop_stage == Const.EXIST)
            {
                pb_BoxDrop.Visible = true;
                pb_BoxDrop.Location = new Point(600, 300);

            }
            else if (S_Exist_Buffer_stage == Const.EXIST)
            {
                pb_BoxDrop.Visible = true;
                pb_BoxDrop.Location = new Point(500, 300);
            }
            else
            {
                pb_BoxDrop.Visible = false;
            }
            // Pick stage
            if(S_Exist_Pick_stage == Const.EXIST)
            {
                pb_BoxPick.Visible = true;
                pb_BoxPick.Location = new Point(90, 300);
            }
            else
            {
                pb_BoxPick.Visible = false;
            }

            // Stopper Open, Close, Throw
            if(S_Stopper_open == Const.OPEN)
            {
                pb_Stopper.Image = Properties.Resources.Stopper_90degree_tr;
                pb_Stopper.Location = new Point(215, 260);
                pb_Stopper.Width = 170;
                pb_Stopper.Height = 55;
            }
            else if(S_Stopper_open == Const.CLOSE)
            {
                pb_Stopper.Image = Properties.Resources.Stopper_0degree_tr;
                pb_Stopper.Location = new Point(330, 260);
                pb_Stopper.Width = 55;
                pb_Stopper.Height = 170;
            }
            else if(DB_data_Inspection[3] == "FAIL")
            {
                pb_Stopper.Image = Properties.Resources.Stopper_45degree_tr;
                pb_Stopper.Location = new Point(275, 260);
                pb_Stopper.Width = 110;
                pb_Stopper.Height = 340;
            }
        }

        private void tmr_stage_Tick(object sender, EventArgs e)
        {
            switch (Seq_stage)
            {
                case 0:
                    //if (SendDataToStage())
                    //{

                    SendDataToStage("auto");
                    Seq_stage = 2 ;
                    //}
                    break;

                case 1: //Drop stage에 제품 있거나 AGV가 없으면 Auto 시작한다.
                    if (S_Exist_Drop_stage == Const.EXIST
                        && S_Exist_Drop_AGV == Const.EMPTY)
                    {
                        SendDataToStage("auto");
                        Seq_stage++;
                        Seq_stage_delay = 0;
                    }
                    else
                    {
                        Seq_stage--;
                    }
                    break;

                case 2:
                    if (S_Exist_Drop_stage == Const.EMPTY 
                        && S_Exist_Buffer_stage == Const.EXIST)
                    {
                        Seq_stage_delay++;
                        if (Seq_stage_delay > 3)
                        {
                            Seq_stage_delay = 0;
                            if (DB_data_Inspection[3] != "Unkn")
                            {
                                Seq_stage++;
                            }
                        }
                        if (Seq_stage_delay == 1)
                        {
                            capture_OCR();
                        }
                    }
                    break;

                case 3:
                    if (DB_data_Inspection[3] == "PASS")
                    {
                        SendDataToStage("pdok");
                        Seq_stage++;
                        OCR_reading_cnt = 0;
                    }
                    else if (DB_data_Inspection[3] == "FAIL")
                    {
                        SendDataToStage("pdno");
                        Seq_stage++;
                        OCR_reading_cnt = 0;
                    }
                    else
                    {
                        OCR_reading_cnt++;
                        Seq_stage--;
                        if(OCR_reading_cnt > OCR_reading_cnt_retry)
                        {
                            SendDataToStage("pdno");
                            Seq_stage++;
                            OCR_reading_cnt = 0;
                        }
                    }
                    break;

                case 4:
                    if (S_Exist_Buffer_stage == Const.EXIST)
                    {
                        Seq_stage++;
                    }
                    else
                    {
                        Seq_stage--;
                    }
                    break;

                case 5:
                    Seq_stage = 0;
                    lbl_OCR.Text = OCR[4] = "";
                    break;
                  
                default:
                    break;
            }
        }

        public IplImage Rotate(IplImage src, int angle)
        {
            rotate = new IplImage(src.Size, BitDepth.U8, 3);
            CvMat matrix = Cv.GetRotationMatrix2D(Cv.Point2D32f(src.Width / 2, src.Height / 2), angle, 1);
            Cv.WarpAffine(src, rotate, matrix, Interpolation.Linear, CvScalar.ScalarAll(0));
            return rotate;
        }

        /*
        private void DisplayImage(byte[,] arr)
        {
            //그림을 표현하기 위한 bitmap 사이즈 조절
            Paper = new Bitmap(arr.GetLength(0), arr.GetLength(1));

            Color c;
            int r, g, b;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    //image data를 RGB에 각각넣어줌
                    r = g = b = arr[i, j];
                    c = Color.FromArgb(r, g, b);
                    //90도로 돌아가므로 j,i순서로 입력
                    Paper.SetPixel(j, i, c);
                }
            }
            //image 출력
            picbx_revision.Image = Paper;
        }
        */
        protected override void OnPaintBackground(PaintEventArgs e)
        // Paint background with underlying graphics from other controls
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;

            if (Parent != null)
            {
                // Take each control in turn
                int index = Parent.Controls.GetChildIndex(this);
                for (int i = Parent.Controls.Count - 1; i > index; i--)
                {
                    Control c = Parent.Controls[i];

                    // Check it's visible and overlaps this control
                    if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
                    {
                        // Load appearance of underlying control and redraw it on this background
                        Bitmap bmp = new Bitmap(c.Width, c.Height, g);
                        c.DrawToBitmap(bmp, c.ClientRectangle);
                        g.TranslateTransform(c.Left - Left, c.Top - Top);
                        g.DrawImageUnscaled(bmp, Point.Empty);
                        g.TranslateTransform(Left - c.Left, Top - c.Top);
                        bmp.Dispose();
                    }
                }
            }
        }



    }

    public static class Util
    {
        public enum Effect { Roll, Slide, Center, Blend }

        public static void Animate(Control ctl, Effect effect, int msec, int angle)
        {
            int flags = effmap[(int)effect];
            if (ctl.Visible) { flags |= 0x10000; angle += 180; }
            else
            {
                if (ctl.TopLevelControl == ctl) flags |= 0x20000;
                else if (effect == Effect.Blend) throw new ArgumentException();
            }
            flags |= dirmap[(angle % 360) / 45];
            bool ok = AnimateWindow(ctl.Handle, msec, flags);
            if (!ok) throw new Exception("Animation failed");
            ctl.Visible = !ctl.Visible;
        }

        private static int[] dirmap = { 1, 5, 4, 6, 2, 10, 8, 9 };
        private static int[] effmap = { 0, 0x40000, 0x10, 0x80000 };

        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr handle, int msec, int flags);
    }
    static class Const
    {
        //STAGE
        public const bool EMPTY = false;
        public const bool EXIST = true;

        public const bool STOP_STAGE = false;
        public const bool RUNNING = true;

        public const bool CLOSE = false;
        public const bool OPEN = true;

        public const bool NOTREADY = false;
        public const bool READY = true;

        //AGV
        public const int STOP_AGV = 0;
        public const int FORWARD_AGV = 1;
        public const int BACKWARD_AGV = 2;

        public const int INITIAL = 0;
        public const int PICKING = 1;
        public const int DROPPING = 2;

        public const int ETC = 0;
        public const int PICK = 1;
        public const int DROP = 2;
        public const int PICK_STAY = 3;
        public const int DROP_STAY = 4;

        public const int DFLT = 0;
        public const int SUCC = 1;
        public const int FAIL = 2;

    }
}