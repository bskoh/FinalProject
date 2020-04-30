namespace FinalProejct_MES
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_blink = new System.Windows.Forms.Panel();
            this.lbl_cnt = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbl_anger = new System.Windows.Forms.Label();
            this.lbl_disgust = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbl_fear = new System.Windows.Forms.Label();
            this.lbl_surprise = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_happiness = new System.Windows.Forms.Label();
            this.lbl_sadness = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_neutral = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_face = new System.Windows.Forms.Button();
            this.lbl_hair = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_glasses = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_age = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_gender = new System.Windows.Forms.Label();
            this.lbl_FaceID = new System.Windows.Forms.Label();
            this.tmr_CAM = new System.Windows.Forms.Timer(this.components);
            this.tmr_count = new System.Windows.Forms.Timer(this.components);
            this.picbox_face_origin = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnl_blink.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_face_origin)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnl_blink);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.btn_face);
            this.panel1.Controls.Add(this.lbl_hair);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lbl_glasses);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lbl_age);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbl_gender);
            this.panel1.Controls.Add(this.lbl_FaceID);
            this.panel1.Controls.Add(this.picbox_face_origin);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 491);
            this.panel1.TabIndex = 10;
            // 
            // pnl_blink
            // 
            this.pnl_blink.BackColor = System.Drawing.Color.White;
            this.pnl_blink.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pnl_blink.Controls.Add(this.lbl_cnt);
            this.pnl_blink.Location = new System.Drawing.Point(26, 17);
            this.pnl_blink.Name = "pnl_blink";
            this.pnl_blink.Size = new System.Drawing.Size(320, 304);
            this.pnl_blink.TabIndex = 11;
            this.pnl_blink.Visible = false;
            // 
            // lbl_cnt
            // 
            this.lbl_cnt.BackColor = System.Drawing.Color.Transparent;
            this.lbl_cnt.Font = new System.Drawing.Font("굴림", 150F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbl_cnt.ForeColor = System.Drawing.Color.White;
            this.lbl_cnt.Location = new System.Drawing.Point(0, 0);
            this.lbl_cnt.Name = "lbl_cnt";
            this.lbl_cnt.Size = new System.Drawing.Size(320, 304);
            this.lbl_cnt.TabIndex = 10;
            this.lbl_cnt.Text = "0";
            this.lbl_cnt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_cnt.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lbl_anger);
            this.groupBox2.Controls.Add(this.lbl_disgust);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lbl_fear);
            this.groupBox2.Controls.Add(this.lbl_surprise);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lbl_happiness);
            this.groupBox2.Controls.Add(this.lbl_sadness);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.lbl_neutral);
            this.groupBox2.Location = new System.Drawing.Point(375, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 304);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "감정상태(%)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "화남 : ";
            // 
            // lbl_anger
            // 
            this.lbl_anger.AutoSize = true;
            this.lbl_anger.Location = new System.Drawing.Point(123, 33);
            this.lbl_anger.Name = "lbl_anger";
            this.lbl_anger.Size = new System.Drawing.Size(15, 15);
            this.lbl_anger.TabIndex = 0;
            this.lbl_anger.Text = "-";
            // 
            // lbl_disgust
            // 
            this.lbl_disgust.AutoSize = true;
            this.lbl_disgust.Location = new System.Drawing.Point(123, 59);
            this.lbl_disgust.Name = "lbl_disgust";
            this.lbl_disgust.Size = new System.Drawing.Size(15, 15);
            this.lbl_disgust.TabIndex = 0;
            this.lbl_disgust.Text = "-";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(28, 217);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "놀람 : ";
            // 
            // lbl_fear
            // 
            this.lbl_fear.AutoSize = true;
            this.lbl_fear.Location = new System.Drawing.Point(123, 89);
            this.lbl_fear.Name = "lbl_fear";
            this.lbl_fear.Size = new System.Drawing.Size(15, 15);
            this.lbl_fear.TabIndex = 0;
            this.lbl_fear.Text = "-";
            // 
            // lbl_surprise
            // 
            this.lbl_surprise.AutoSize = true;
            this.lbl_surprise.Location = new System.Drawing.Point(123, 217);
            this.lbl_surprise.Name = "lbl_surprise";
            this.lbl_surprise.Size = new System.Drawing.Size(15, 15);
            this.lbl_surprise.TabIndex = 0;
            this.lbl_surprise.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "역겨움 : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 0;
            this.label10.Text = "슬픔 : ";
            // 
            // lbl_happiness
            // 
            this.lbl_happiness.AutoSize = true;
            this.lbl_happiness.Location = new System.Drawing.Point(123, 132);
            this.lbl_happiness.Name = "lbl_happiness";
            this.lbl_happiness.Size = new System.Drawing.Size(15, 15);
            this.lbl_happiness.TabIndex = 0;
            this.lbl_happiness.Text = "-";
            // 
            // lbl_sadness
            // 
            this.lbl_sadness.AutoSize = true;
            this.lbl_sadness.Location = new System.Drawing.Point(123, 193);
            this.lbl_sadness.Name = "lbl_sadness";
            this.lbl_sadness.Size = new System.Drawing.Size(15, 15);
            this.lbl_sadness.TabIndex = 0;
            this.lbl_sadness.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "공포 : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 162);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "중립 : ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "행복 : ";
            // 
            // lbl_neutral
            // 
            this.lbl_neutral.AutoSize = true;
            this.lbl_neutral.Location = new System.Drawing.Point(123, 162);
            this.lbl_neutral.Name = "lbl_neutral";
            this.lbl_neutral.Size = new System.Drawing.Size(15, 15);
            this.lbl_neutral.TabIndex = 0;
            this.lbl_neutral.Text = "-";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(63, 459);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(102, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "머리카락 색 : ";
            // 
            // btn_face
            // 
            this.btn_face.Location = new System.Drawing.Point(387, 386);
            this.btn_face.Name = "btn_face";
            this.btn_face.Size = new System.Drawing.Size(164, 62);
            this.btn_face.TabIndex = 7;
            this.btn_face.Text = "FACE!";
            this.btn_face.UseVisualStyleBackColor = true;
            this.btn_face.Click += new System.EventHandler(this.btn_face_Click);
            // 
            // lbl_hair
            // 
            this.lbl_hair.AutoSize = true;
            this.lbl_hair.Location = new System.Drawing.Point(171, 459);
            this.lbl_hair.Name = "lbl_hair";
            this.lbl_hair.Size = new System.Drawing.Size(15, 15);
            this.lbl_hair.TabIndex = 0;
            this.lbl_hair.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(76, 438);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "안경 착용 : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 410);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "나이 : ";
            // 
            // lbl_glasses
            // 
            this.lbl_glasses.AutoSize = true;
            this.lbl_glasses.Location = new System.Drawing.Point(171, 438);
            this.lbl_glasses.Name = "lbl_glasses";
            this.lbl_glasses.Size = new System.Drawing.Size(15, 15);
            this.lbl_glasses.TabIndex = 0;
            this.lbl_glasses.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 372);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "성별 : ";
            // 
            // lbl_age
            // 
            this.lbl_age.AutoSize = true;
            this.lbl_age.Location = new System.Drawing.Point(171, 410);
            this.lbl_age.Name = "lbl_age";
            this.lbl_age.Size = new System.Drawing.Size(15, 15);
            this.lbl_age.TabIndex = 0;
            this.lbl_age.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 342);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "FaceID : ";
            // 
            // lbl_gender
            // 
            this.lbl_gender.AutoSize = true;
            this.lbl_gender.Location = new System.Drawing.Point(171, 372);
            this.lbl_gender.Name = "lbl_gender";
            this.lbl_gender.Size = new System.Drawing.Size(15, 15);
            this.lbl_gender.TabIndex = 0;
            this.lbl_gender.Text = "-";
            // 
            // lbl_FaceID
            // 
            this.lbl_FaceID.AutoSize = true;
            this.lbl_FaceID.Location = new System.Drawing.Point(171, 342);
            this.lbl_FaceID.Name = "lbl_FaceID";
            this.lbl_FaceID.Size = new System.Drawing.Size(15, 15);
            this.lbl_FaceID.TabIndex = 0;
            this.lbl_FaceID.Text = "-";
            // 
            // tmr_CAM
            // 
            this.tmr_CAM.Interval = 33;
            this.tmr_CAM.Tick += new System.EventHandler(this.tmr_CAM_Tick);
            // 
            // tmr_count
            // 
            this.tmr_count.Interval = 1000;
            this.tmr_count.Tick += new System.EventHandler(this.tmr_count_Tick);
            // 
            // picbox_face_origin
            // 
            this.picbox_face_origin.Location = new System.Drawing.Point(26, 25);
            this.picbox_face_origin.Name = "picbox_face_origin";
            this.picbox_face_origin.Size = new System.Drawing.Size(320, 296);
            this.picbox_face_origin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_face_origin.TabIndex = 8;
            this.picbox_face_origin.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(443, 520);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 83);
            this.button1.TabIndex = 11;
            this.button1.Text = "CLOSE";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 628);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnl_blink.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_face_origin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnl_blink;
        private System.Windows.Forms.Label lbl_cnt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_anger;
        private System.Windows.Forms.Label lbl_disgust;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbl_fear;
        private System.Windows.Forms.Label lbl_surprise;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_happiness;
        private System.Windows.Forms.Label lbl_sadness;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbl_neutral;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_face;
        private System.Windows.Forms.Label lbl_hair;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_glasses;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_age;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_gender;
        private System.Windows.Forms.Label lbl_FaceID;
        private OpenCvSharp.UserInterface.PictureBoxIpl picbox_face_origin;
        private System.Windows.Forms.Timer tmr_CAM;
        private System.Windows.Forms.Timer tmr_count;
        private System.Windows.Forms.Button button1;
    }
}