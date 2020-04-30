namespace Test
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pb_Line = new System.Windows.Forms.PictureBox();
            this.pb_AGV = new System.Windows.Forms.PictureBox();
            this.btn_rfid = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Line)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AGV)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_Line
            // 
            this.pb_Line.Image = ((System.Drawing.Image)(resources.GetObject("pb_Line.Image")));
            this.pb_Line.Location = new System.Drawing.Point(90, 75);
            this.pb_Line.Name = "pb_Line";
            this.pb_Line.Size = new System.Drawing.Size(800, 800);
            this.pb_Line.TabIndex = 1;
            this.pb_Line.TabStop = false;
            // 
            // pb_AGV
            // 
            this.pb_AGV.Image = global::Test.Properties.Resources.agv;
            this.pb_AGV.Location = new System.Drawing.Point(90, 75);
            this.pb_AGV.Name = "pb_AGV";
            this.pb_AGV.Size = new System.Drawing.Size(50, 50);
            this.pb_AGV.TabIndex = 2;
            this.pb_AGV.TabStop = false;
            // 
            // btn_rfid
            // 
            this.btn_rfid.Location = new System.Drawing.Point(847, 13);
            this.btn_rfid.Name = "btn_rfid";
            this.btn_rfid.Size = new System.Drawing.Size(119, 35);
            this.btn_rfid.TabIndex = 3;
            this.btn_rfid.Text = "button1";
            this.btn_rfid.UseVisualStyleBackColor = true;
            this.btn_rfid.Click += new System.EventHandler(this.btn_rfid_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(1000, 128);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(125, 22);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(1000, 156);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(125, 22);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(1000, 184);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(125, 22);
            this.checkBox3.TabIndex = 13;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(1000, 212);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(125, 22);
            this.checkBox4.TabIndex = 14;
            this.checkBox4.Text = "checkBox4";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(1000, 240);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(125, 22);
            this.checkBox5.TabIndex = 15;
            this.checkBox5.Text = "checkBox5";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(1000, 268);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(125, 22);
            this.checkBox6.TabIndex = 16;
            this.checkBox6.Text = "checkBox6";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1478, 1050);
            this.Controls.Add(this.checkBox6);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btn_rfid);
            this.Controls.Add(this.pb_AGV);
            this.Controls.Add(this.pb_Line);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pb_Line)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_Line;
        private System.Windows.Forms.PictureBox pb_AGV;
        private System.Windows.Forms.Button btn_rfid;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
    }
}

