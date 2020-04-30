namespace FinalProejct_MES
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tmr_UI_Refresh = new System.Windows.Forms.Timer(this.components);
            this.Serial_stage = new System.IO.Ports.SerialPort(this.components);
            this.Serial_agv = new System.IO.Ports.SerialPort(this.components);
            this.tmr_CAM = new System.Windows.Forms.Timer(this.components);
            this.btn_OCR = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbl_OCR_angle = new System.Windows.Forms.Label();
            this.lbl_OCR = new System.Windows.Forms.Label();
            this.rb_kor = new System.Windows.Forms.RadioButton();
            this.rb_eng = new System.Windows.Forms.RadioButton();
            this.rb_auto = new System.Windows.Forms.RadioButton();
            this.picbox_OCR_result = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.picbox_OCR_orgin = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.button1 = new System.Windows.Forms.Button();
            this.tmr_DROP = new System.Windows.Forms.Timer(this.components);
            this.tmr_PICK = new System.Windows.Forms.Timer(this.components);
            this.tmr_stage = new System.Windows.Forms.Timer(this.components);
            this.tmr_DB = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.pb_BoxPick = new System.Windows.Forms.PictureBox();
            this.pb_BoxDrop = new System.Windows.Forms.PictureBox();
            this.pb_Cam = new System.Windows.Forms.PictureBox();
            this.pb_Stopper = new System.Windows.Forms.PictureBox();
            this.pb_Conv_Drop = new System.Windows.Forms.PictureBox();
            this.pb_Conv_Pick = new System.Windows.Forms.PictureBox();
            this.pb_AGV = new System.Windows.Forms.PictureBox();
            this.pb_map = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_AGV = new System.Windows.Forms.Label();
            this.btn_disconnect_AGV = new System.Windows.Forms.Button();
            this.btn_connect_AGV = new System.Windows.Forms.Button();
            this.cbbox_baudrate_AGV = new System.Windows.Forms.ComboBox();
            this.cbbox_portlist_AGV = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_Stage = new System.Windows.Forms.Label();
            this.btn_disconnect_stage = new System.Windows.Forms.Button();
            this.btn_connect_stage = new System.Windows.Forms.Button();
            this.cbbox_baudrate_stage = new System.Windows.Forms.ComboBox();
            this.cbbox_portlist_stage = new System.Windows.Forms.ComboBox();
            this.btn_Pick_SEQ_Start = new System.Windows.Forms.Button();
            this.btn_Drop_SEQ_Start = new System.Windows.Forms.Button();
            this.btn_Stage_SEQ_Start = new System.Windows.Forms.Button();
            this.btn_SendAuto = new System.Windows.Forms.Button();
            this.btn_SendManu = new System.Windows.Forms.Button();
            this.btn_PDok = new System.Windows.Forms.Button();
            this.btn_PDNo = new System.Windows.Forms.Button();
            this.tmr_DataCheck = new System.Windows.Forms.Timer(this.components);
            this.btn_DBtest = new System.Windows.Forms.Button();
            this.btn_agv_reset = new System.Windows.Forms.Button();
            this.btn_Stage_Reset = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_OCR_result)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_OCR_orgin)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_BoxPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_BoxDrop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Cam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Stopper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Conv_Drop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Conv_Pick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_map)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmr_UI_Refresh
            // 
            this.tmr_UI_Refresh.Interval = 1000;
            this.tmr_UI_Refresh.Tick += new System.EventHandler(this.tmr_UI_Refresh_Tick);
            // 
            // Serial_stage
            // 
            this.Serial_stage.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // Serial_agv
            // 
            this.Serial_agv.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Serial_agv_DataReceived);
            // 
            // tmr_CAM
            // 
            this.tmr_CAM.Interval = 33;
            this.tmr_CAM.Tick += new System.EventHandler(this.tmr_CAM_Tick);
            // 
            // btn_OCR
            // 
            this.btn_OCR.Location = new System.Drawing.Point(91, 602);
            this.btn_OCR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_OCR.Name = "btn_OCR";
            this.btn_OCR.Size = new System.Drawing.Size(165, 62);
            this.btn_OCR.TabIndex = 7;
            this.btn_OCR.Text = "OCR";
            this.btn_OCR.UseVisualStyleBackColor = true;
            this.btn_OCR.Click += new System.EventHandler(this.btn_capture_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.lbl_OCR_angle);
            this.groupBox1.Controls.Add(this.lbl_OCR);
            this.groupBox1.Controls.Add(this.rb_kor);
            this.groupBox1.Controls.Add(this.rb_eng);
            this.groupBox1.Controls.Add(this.rb_auto);
            this.groupBox1.Controls.Add(this.btn_OCR);
            this.groupBox1.Controls.Add(this.picbox_OCR_result);
            this.groupBox1.Controls.Add(this.picbox_OCR_orgin);
            this.groupBox1.Location = new System.Drawing.Point(942, 132);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(358, 682);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 549);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(142, 15);
            this.label14.TabIndex = 12;
            this.label14.Text = "문자 방향 및 각도 : ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 520);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "검출된 문자 : ";
            // 
            // lbl_OCR_angle
            // 
            this.lbl_OCR_angle.AutoSize = true;
            this.lbl_OCR_angle.Location = new System.Drawing.Point(162, 549);
            this.lbl_OCR_angle.Name = "lbl_OCR_angle";
            this.lbl_OCR_angle.Size = new System.Drawing.Size(15, 15);
            this.lbl_OCR_angle.TabIndex = 12;
            this.lbl_OCR_angle.Text = "-";
            // 
            // lbl_OCR
            // 
            this.lbl_OCR.AutoSize = true;
            this.lbl_OCR.Location = new System.Drawing.Point(119, 520);
            this.lbl_OCR.Name = "lbl_OCR";
            this.lbl_OCR.Size = new System.Drawing.Size(15, 15);
            this.lbl_OCR.TabIndex = 12;
            this.lbl_OCR.Text = "-";
            // 
            // rb_kor
            // 
            this.rb_kor.AutoSize = true;
            this.rb_kor.Location = new System.Drawing.Point(239, 578);
            this.rb_kor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rb_kor.Name = "rb_kor";
            this.rb_kor.Size = new System.Drawing.Size(85, 19);
            this.rb_kor.TabIndex = 11;
            this.rb_kor.Text = "KOREAN";
            this.rb_kor.UseVisualStyleBackColor = true;
            // 
            // rb_eng
            // 
            this.rb_eng.AutoSize = true;
            this.rb_eng.Checked = true;
            this.rb_eng.Location = new System.Drawing.Point(122, 578);
            this.rb_eng.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rb_eng.Name = "rb_eng";
            this.rb_eng.Size = new System.Drawing.Size(88, 19);
            this.rb_eng.TabIndex = 11;
            this.rb_eng.TabStop = true;
            this.rb_eng.Text = "ENGLISH";
            this.rb_eng.UseVisualStyleBackColor = true;
            // 
            // rb_auto
            // 
            this.rb_auto.AutoSize = true;
            this.rb_auto.Location = new System.Drawing.Point(7, 578);
            this.rb_auto.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rb_auto.Name = "rb_auto";
            this.rb_auto.Size = new System.Drawing.Size(58, 19);
            this.rb_auto.TabIndex = 11;
            this.rb_auto.Text = "Auto";
            this.rb_auto.UseVisualStyleBackColor = true;
            // 
            // picbox_OCR_result
            // 
            this.picbox_OCR_result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picbox_OCR_result.Location = new System.Drawing.Point(18, 270);
            this.picbox_OCR_result.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picbox_OCR_result.Name = "picbox_OCR_result";
            this.picbox_OCR_result.Size = new System.Drawing.Size(321, 234);
            this.picbox_OCR_result.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_OCR_result.TabIndex = 10;
            this.picbox_OCR_result.TabStop = false;
            // 
            // picbox_OCR_orgin
            // 
            this.picbox_OCR_orgin.Location = new System.Drawing.Point(18, 24);
            this.picbox_OCR_orgin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picbox_OCR_orgin.Name = "picbox_OCR_orgin";
            this.picbox_OCR_orgin.Size = new System.Drawing.Size(320, 240);
            this.picbox_OCR_orgin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_OCR_orgin.TabIndex = 6;
            this.picbox_OCR_orgin.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 325);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 112);
            this.button1.TabIndex = 12;
            this.button1.Text = "Face";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tmr_DROP
            // 
            this.tmr_DROP.Interval = 500;
            this.tmr_DROP.Tick += new System.EventHandler(this.tmr_DROP_Tick);
            // 
            // tmr_PICK
            // 
            this.tmr_PICK.Interval = 500;
            this.tmr_PICK.Tick += new System.EventHandler(this.tmr_PICK_Tick);
            // 
            // tmr_stage
            // 
            this.tmr_stage.Interval = 500;
            this.tmr_stage.Tick += new System.EventHandler(this.tmr_stage_Tick);
            // 
            // tmr_DB
            // 
            this.tmr_DB.Interval = 1000;
            this.tmr_DB.Tick += new System.EventHandler(this.tmr_DB_Tick);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pb_BoxPick);
            this.panel1.Controls.Add(this.pb_BoxDrop);
            this.panel1.Controls.Add(this.pb_Cam);
            this.panel1.Controls.Add(this.pb_Stopper);
            this.panel1.Controls.Add(this.pb_Conv_Drop);
            this.panel1.Controls.Add(this.pb_Conv_Pick);
            this.panel1.Controls.Add(this.pb_AGV);
            this.panel1.Controls.Add(this.pb_map);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 900);
            this.panel1.TabIndex = 13;
            // 
            // pb_BoxPick
            // 
            this.pb_BoxPick.Image = global::FinalProejct_MES.Properties.Resources.Product;
            this.pb_BoxPick.Location = new System.Drawing.Point(131, 358);
            this.pb_BoxPick.Name = "pb_BoxPick";
            this.pb_BoxPick.Size = new System.Drawing.Size(60, 60);
            this.pb_BoxPick.TabIndex = 8;
            this.pb_BoxPick.TabStop = false;
            this.pb_BoxPick.Visible = false;
            // 
            // pb_BoxDrop
            // 
            this.pb_BoxDrop.Image = global::FinalProejct_MES.Properties.Resources.Product;
            this.pb_BoxDrop.Location = new System.Drawing.Point(796, 358);
            this.pb_BoxDrop.Name = "pb_BoxDrop";
            this.pb_BoxDrop.Size = new System.Drawing.Size(60, 60);
            this.pb_BoxDrop.TabIndex = 7;
            this.pb_BoxDrop.TabStop = false;
            this.pb_BoxDrop.Visible = false;
            // 
            // pb_Cam
            // 
            this.pb_Cam.Image = global::FinalProejct_MES.Properties.Resources.cctv;
            this.pb_Cam.Location = new System.Drawing.Point(566, 335);
            this.pb_Cam.Name = "pb_Cam";
            this.pb_Cam.Size = new System.Drawing.Size(80, 80);
            this.pb_Cam.TabIndex = 6;
            this.pb_Cam.TabStop = false;
            // 
            // pb_Stopper
            // 
            this.pb_Stopper.Image = ((System.Drawing.Image)(resources.GetObject("pb_Stopper.Image")));
            this.pb_Stopper.Location = new System.Drawing.Point(469, 361);
            this.pb_Stopper.Name = "pb_Stopper";
            this.pb_Stopper.Size = new System.Drawing.Size(71, 288);
            this.pb_Stopper.TabIndex = 5;
            this.pb_Stopper.TabStop = false;
            // 
            // pb_Conv_Drop
            // 
            this.pb_Conv_Drop.Image = ((System.Drawing.Image)(resources.GetObject("pb_Conv_Drop.Image")));
            this.pb_Conv_Drop.Location = new System.Drawing.Point(566, 434);
            this.pb_Conv_Drop.Name = "pb_Conv_Drop";
            this.pb_Conv_Drop.Size = new System.Drawing.Size(300, 60);
            this.pb_Conv_Drop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Conv_Drop.TabIndex = 3;
            this.pb_Conv_Drop.TabStop = false;
            // 
            // pb_Conv_Pick
            // 
            this.pb_Conv_Pick.Image = ((System.Drawing.Image)(resources.GetObject("pb_Conv_Pick.Image")));
            this.pb_Conv_Pick.Location = new System.Drawing.Point(111, 434);
            this.pb_Conv_Pick.Name = "pb_Conv_Pick";
            this.pb_Conv_Pick.Size = new System.Drawing.Size(300, 60);
            this.pb_Conv_Pick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_Conv_Pick.TabIndex = 3;
            this.pb_Conv_Pick.TabStop = false;
            // 
            // pb_AGV
            // 
            this.pb_AGV.Image = ((System.Drawing.Image)(resources.GetObject("pb_AGV.Image")));
            this.pb_AGV.Location = new System.Drawing.Point(20, 20);
            this.pb_AGV.Name = "pb_AGV";
            this.pb_AGV.Size = new System.Drawing.Size(55, 55);
            this.pb_AGV.TabIndex = 1;
            this.pb_AGV.TabStop = false;
            // 
            // pb_map
            // 
            this.pb_map.Image = global::FinalProejct_MES.Properties.Resources.Line;
            this.pb_map.ImageLocation = "";
            this.pb_map.Location = new System.Drawing.Point(20, 20);
            this.pb_map.Name = "pb_map";
            this.pb_map.Size = new System.Drawing.Size(860, 860);
            this.pb_map.TabIndex = 4;
            this.pb_map.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbl_AGV);
            this.panel2.Controls.Add(this.btn_disconnect_AGV);
            this.panel2.Controls.Add(this.btn_connect_AGV);
            this.panel2.Controls.Add(this.cbbox_baudrate_AGV);
            this.panel2.Controls.Add(this.cbbox_portlist_AGV);
            this.panel2.Location = new System.Drawing.Point(942, 9);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(626, 50);
            this.panel2.TabIndex = 14;
            // 
            // lbl_AGV
            // 
            this.lbl_AGV.AutoSize = true;
            this.lbl_AGV.Location = new System.Drawing.Point(30, 17);
            this.lbl_AGV.Name = "lbl_AGV";
            this.lbl_AGV.Size = new System.Drawing.Size(35, 15);
            this.lbl_AGV.TabIndex = 7;
            this.lbl_AGV.Text = "AGV";
            // 
            // btn_disconnect_AGV
            // 
            this.btn_disconnect_AGV.Enabled = false;
            this.btn_disconnect_AGV.Location = new System.Drawing.Point(485, 14);
            this.btn_disconnect_AGV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_disconnect_AGV.Name = "btn_disconnect_AGV";
            this.btn_disconnect_AGV.Size = new System.Drawing.Size(113, 22);
            this.btn_disconnect_AGV.TabIndex = 5;
            this.btn_disconnect_AGV.Text = "disconnect";
            this.btn_disconnect_AGV.UseVisualStyleBackColor = true;
            this.btn_disconnect_AGV.Click += new System.EventHandler(this.btn_disconnect_AGV_Click);
            // 
            // btn_connect_AGV
            // 
            this.btn_connect_AGV.Location = new System.Drawing.Point(403, 14);
            this.btn_connect_AGV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_connect_AGV.Name = "btn_connect_AGV";
            this.btn_connect_AGV.Size = new System.Drawing.Size(75, 22);
            this.btn_connect_AGV.TabIndex = 6;
            this.btn_connect_AGV.Text = "connect";
            this.btn_connect_AGV.UseVisualStyleBackColor = true;
            this.btn_connect_AGV.Click += new System.EventHandler(this.btn_connect_AGV_Click);
            // 
            // cbbox_baudrate_AGV
            // 
            this.cbbox_baudrate_AGV.FormattingEnabled = true;
            this.cbbox_baudrate_AGV.Location = new System.Drawing.Point(276, 14);
            this.cbbox_baudrate_AGV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbbox_baudrate_AGV.Name = "cbbox_baudrate_AGV";
            this.cbbox_baudrate_AGV.Size = new System.Drawing.Size(121, 23);
            this.cbbox_baudrate_AGV.TabIndex = 4;
            this.cbbox_baudrate_AGV.Text = "9600";
            // 
            // cbbox_portlist_AGV
            // 
            this.cbbox_portlist_AGV.FormattingEnabled = true;
            this.cbbox_portlist_AGV.Location = new System.Drawing.Point(149, 14);
            this.cbbox_portlist_AGV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbbox_portlist_AGV.Name = "cbbox_portlist_AGV";
            this.cbbox_portlist_AGV.Size = new System.Drawing.Size(121, 23);
            this.cbbox_portlist_AGV.TabIndex = 3;
            this.cbbox_portlist_AGV.Text = "COM3";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbl_Stage);
            this.panel3.Controls.Add(this.btn_disconnect_stage);
            this.panel3.Controls.Add(this.btn_connect_stage);
            this.panel3.Controls.Add(this.cbbox_baudrate_stage);
            this.panel3.Controls.Add(this.cbbox_portlist_stage);
            this.panel3.Location = new System.Drawing.Point(942, 65);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(626, 50);
            this.panel3.TabIndex = 16;
            // 
            // lbl_Stage
            // 
            this.lbl_Stage.AutoSize = true;
            this.lbl_Stage.Location = new System.Drawing.Point(27, 18);
            this.lbl_Stage.Name = "lbl_Stage";
            this.lbl_Stage.Size = new System.Drawing.Size(45, 15);
            this.lbl_Stage.TabIndex = 7;
            this.lbl_Stage.Text = "Stage";
            // 
            // btn_disconnect_stage
            // 
            this.btn_disconnect_stage.Enabled = false;
            this.btn_disconnect_stage.Location = new System.Drawing.Point(485, 15);
            this.btn_disconnect_stage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_disconnect_stage.Name = "btn_disconnect_stage";
            this.btn_disconnect_stage.Size = new System.Drawing.Size(113, 22);
            this.btn_disconnect_stage.TabIndex = 5;
            this.btn_disconnect_stage.Text = "disconnect";
            this.btn_disconnect_stage.UseVisualStyleBackColor = true;
            this.btn_disconnect_stage.Click += new System.EventHandler(this.btn_disconnect_stage_Click);
            // 
            // btn_connect_stage
            // 
            this.btn_connect_stage.Location = new System.Drawing.Point(403, 15);
            this.btn_connect_stage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_connect_stage.Name = "btn_connect_stage";
            this.btn_connect_stage.Size = new System.Drawing.Size(75, 22);
            this.btn_connect_stage.TabIndex = 6;
            this.btn_connect_stage.Text = "connect";
            this.btn_connect_stage.UseVisualStyleBackColor = true;
            this.btn_connect_stage.Click += new System.EventHandler(this.btn_connect_stage_Click);
            // 
            // cbbox_baudrate_stage
            // 
            this.cbbox_baudrate_stage.FormattingEnabled = true;
            this.cbbox_baudrate_stage.Location = new System.Drawing.Point(276, 16);
            this.cbbox_baudrate_stage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbbox_baudrate_stage.Name = "cbbox_baudrate_stage";
            this.cbbox_baudrate_stage.Size = new System.Drawing.Size(121, 23);
            this.cbbox_baudrate_stage.TabIndex = 4;
            this.cbbox_baudrate_stage.Text = "9600";
            // 
            // cbbox_portlist_stage
            // 
            this.cbbox_portlist_stage.FormattingEnabled = true;
            this.cbbox_portlist_stage.Location = new System.Drawing.Point(149, 16);
            this.cbbox_portlist_stage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbbox_portlist_stage.Name = "cbbox_portlist_stage";
            this.cbbox_portlist_stage.Size = new System.Drawing.Size(121, 23);
            this.cbbox_portlist_stage.TabIndex = 3;
            this.cbbox_portlist_stage.Text = "COM3";
            // 
            // btn_Pick_SEQ_Start
            // 
            this.btn_Pick_SEQ_Start.Enabled = false;
            this.btn_Pick_SEQ_Start.Location = new System.Drawing.Point(24, 28);
            this.btn_Pick_SEQ_Start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Pick_SEQ_Start.Name = "btn_Pick_SEQ_Start";
            this.btn_Pick_SEQ_Start.Size = new System.Drawing.Size(206, 38);
            this.btn_Pick_SEQ_Start.TabIndex = 17;
            this.btn_Pick_SEQ_Start.Text = "Pick Sequence Start";
            this.btn_Pick_SEQ_Start.UseVisualStyleBackColor = true;
            this.btn_Pick_SEQ_Start.Click += new System.EventHandler(this.btn_Pick_SEQ_Start_Click);
            // 
            // btn_Drop_SEQ_Start
            // 
            this.btn_Drop_SEQ_Start.Enabled = false;
            this.btn_Drop_SEQ_Start.Location = new System.Drawing.Point(24, 70);
            this.btn_Drop_SEQ_Start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Drop_SEQ_Start.Name = "btn_Drop_SEQ_Start";
            this.btn_Drop_SEQ_Start.Size = new System.Drawing.Size(206, 38);
            this.btn_Drop_SEQ_Start.TabIndex = 17;
            this.btn_Drop_SEQ_Start.Text = "Drop Sequence Start";
            this.btn_Drop_SEQ_Start.UseVisualStyleBackColor = true;
            this.btn_Drop_SEQ_Start.Click += new System.EventHandler(this.btn_Drop_SEQ_Start_Click);
            // 
            // btn_Stage_SEQ_Start
            // 
            this.btn_Stage_SEQ_Start.Enabled = false;
            this.btn_Stage_SEQ_Start.Location = new System.Drawing.Point(24, 112);
            this.btn_Stage_SEQ_Start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Stage_SEQ_Start.Name = "btn_Stage_SEQ_Start";
            this.btn_Stage_SEQ_Start.Size = new System.Drawing.Size(206, 38);
            this.btn_Stage_SEQ_Start.TabIndex = 17;
            this.btn_Stage_SEQ_Start.Text = "Stage Sequence Start";
            this.btn_Stage_SEQ_Start.UseVisualStyleBackColor = true;
            this.btn_Stage_SEQ_Start.Click += new System.EventHandler(this.btn_Stage_SEQ_Start_Click);
            // 
            // btn_SendAuto
            // 
            this.btn_SendAuto.Location = new System.Drawing.Point(24, 155);
            this.btn_SendAuto.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_SendAuto.Name = "btn_SendAuto";
            this.btn_SendAuto.Size = new System.Drawing.Size(103, 38);
            this.btn_SendAuto.TabIndex = 17;
            this.btn_SendAuto.Text = "Send Auto";
            this.btn_SendAuto.UseVisualStyleBackColor = true;
            this.btn_SendAuto.Click += new System.EventHandler(this.btn_SendAuto_Click);
            // 
            // btn_SendManu
            // 
            this.btn_SendManu.Location = new System.Drawing.Point(127, 155);
            this.btn_SendManu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_SendManu.Name = "btn_SendManu";
            this.btn_SendManu.Size = new System.Drawing.Size(103, 38);
            this.btn_SendManu.TabIndex = 17;
            this.btn_SendManu.Text = "Send Manu";
            this.btn_SendManu.UseVisualStyleBackColor = true;
            this.btn_SendManu.Click += new System.EventHandler(this.btn_SendManu_Click);
            // 
            // btn_PDok
            // 
            this.btn_PDok.Location = new System.Drawing.Point(24, 198);
            this.btn_PDok.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_PDok.Name = "btn_PDok";
            this.btn_PDok.Size = new System.Drawing.Size(103, 38);
            this.btn_PDok.TabIndex = 17;
            this.btn_PDok.Text = "INSP Succ";
            this.btn_PDok.UseVisualStyleBackColor = true;
            this.btn_PDok.Click += new System.EventHandler(this.btn_PDok_Click);
            // 
            // btn_PDNo
            // 
            this.btn_PDNo.Location = new System.Drawing.Point(127, 198);
            this.btn_PDNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_PDNo.Name = "btn_PDNo";
            this.btn_PDNo.Size = new System.Drawing.Size(103, 38);
            this.btn_PDNo.TabIndex = 17;
            this.btn_PDNo.Text = "INSP Fail";
            this.btn_PDNo.UseVisualStyleBackColor = true;
            this.btn_PDNo.Click += new System.EventHandler(this.btn_PDNo_Click);
            // 
            // tmr_DataCheck
            // 
            this.tmr_DataCheck.Interval = 300;
            this.tmr_DataCheck.Tick += new System.EventHandler(this.tmr_DataCheck_Tick);
            // 
            // btn_DBtest
            // 
            this.btn_DBtest.Location = new System.Drawing.Point(24, 282);
            this.btn_DBtest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_DBtest.Name = "btn_DBtest";
            this.btn_DBtest.Size = new System.Drawing.Size(206, 38);
            this.btn_DBtest.TabIndex = 18;
            this.btn_DBtest.Text = "DB Test";
            this.btn_DBtest.UseVisualStyleBackColor = true;
            this.btn_DBtest.Click += new System.EventHandler(this.btn_DBtest_Click);
            // 
            // btn_agv_reset
            // 
            this.btn_agv_reset.Location = new System.Drawing.Point(24, 240);
            this.btn_agv_reset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_agv_reset.Name = "btn_agv_reset";
            this.btn_agv_reset.Size = new System.Drawing.Size(103, 38);
            this.btn_agv_reset.TabIndex = 19;
            this.btn_agv_reset.Text = "AGV Reset";
            this.btn_agv_reset.UseVisualStyleBackColor = true;
            this.btn_agv_reset.Click += new System.EventHandler(this.btn_reset_Click);
            // 
            // btn_Stage_Reset
            // 
            this.btn_Stage_Reset.Location = new System.Drawing.Point(127, 240);
            this.btn_Stage_Reset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Stage_Reset.Name = "btn_Stage_Reset";
            this.btn_Stage_Reset.Size = new System.Drawing.Size(103, 38);
            this.btn_Stage_Reset.TabIndex = 19;
            this.btn_Stage_Reset.Text = "Stage Reset";
            this.btn_Stage_Reset.UseVisualStyleBackColor = true;
            this.btn_Stage_Reset.Click += new System.EventHandler(this.btn_reset_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Pick_SEQ_Start);
            this.groupBox2.Controls.Add(this.btn_Stage_Reset);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.btn_agv_reset);
            this.groupBox2.Controls.Add(this.btn_Drop_SEQ_Start);
            this.groupBox2.Controls.Add(this.btn_DBtest);
            this.groupBox2.Controls.Add(this.btn_SendAuto);
            this.groupBox2.Controls.Add(this.btn_Stage_SEQ_Start);
            this.groupBox2.Controls.Add(this.btn_PDok);
            this.groupBox2.Controls.Add(this.btn_SendManu);
            this.groupBox2.Controls.Add(this.btn_PDNo);
            this.groupBox2.Location = new System.Drawing.Point(1310, 132);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(259, 511);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Manual Command";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1539, 879);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "MES(Manufacturing Execution System)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_OCR_result)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_OCR_orgin)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_BoxPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_BoxDrop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Cam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Stopper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Conv_Drop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Conv_Pick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_map)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmr_UI_Refresh;
        private System.IO.Ports.SerialPort Serial_stage;
        private System.IO.Ports.SerialPort Serial_agv;
        private OpenCvSharp.UserInterface.PictureBoxIpl picbox_OCR_orgin;
        private System.Windows.Forms.Timer tmr_CAM;
        private System.Windows.Forms.Button btn_OCR;
        private OpenCvSharp.UserInterface.PictureBoxIpl picbox_OCR_result;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb_kor;
        private System.Windows.Forms.RadioButton rb_eng;
        private System.Windows.Forms.RadioButton rb_auto;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lbl_OCR;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbl_OCR_angle;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer tmr_DROP;
        private System.Windows.Forms.Timer tmr_PICK;
        private System.Windows.Forms.Timer tmr_stage;
        private System.Windows.Forms.Timer tmr_DB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_disconnect_AGV;
        private System.Windows.Forms.Button btn_connect_AGV;
        private System.Windows.Forms.ComboBox cbbox_baudrate_AGV;
        private System.Windows.Forms.ComboBox cbbox_portlist_AGV;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_disconnect_stage;
        private System.Windows.Forms.Button btn_connect_stage;
        private System.Windows.Forms.ComboBox cbbox_baudrate_stage;
        private System.Windows.Forms.ComboBox cbbox_portlist_stage;
        private System.Windows.Forms.Button btn_Pick_SEQ_Start;
        private System.Windows.Forms.Button btn_Drop_SEQ_Start;
        private System.Windows.Forms.Button btn_Stage_SEQ_Start;
        private System.Windows.Forms.Label lbl_AGV;
        private System.Windows.Forms.Label lbl_Stage;
        private System.Windows.Forms.Button btn_SendAuto;
        private System.Windows.Forms.Button btn_SendManu;
        private System.Windows.Forms.Button btn_PDok;
        private System.Windows.Forms.Button btn_PDNo;
        private System.Windows.Forms.Timer tmr_DataCheck;
        private System.Windows.Forms.Button btn_DBtest;
        private System.Windows.Forms.Button btn_agv_reset;
        private System.Windows.Forms.Button btn_Stage_Reset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pb_AGV;
        private System.Windows.Forms.PictureBox pb_Conv_Drop;
        private System.Windows.Forms.PictureBox pb_Conv_Pick;
        private System.Windows.Forms.PictureBox pb_map;
        private System.Windows.Forms.PictureBox pb_Stopper;
        private System.Windows.Forms.PictureBox pb_Cam;
        private System.Windows.Forms.PictureBox pb_BoxDrop;
        private System.Windows.Forms.PictureBox pb_BoxPick;
    }
}

