namespace FinalProejct_MES
{
    partial class Form3
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.lst_AGV = new System.Windows.Forms.ListView();
            this.lst_stage = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lst_inspection = new System.Windows.Forms.ListView();
            this.tmr_DBSelect = new System.Windows.Forms.Timer(this.components);
            this.lst_AGVComm = new System.Windows.Forms.ListBox();
            this.lst_StageComm = new System.Windows.Forms.ListBox();
            this.btn_Read_log = new System.Windows.Forms.Button();
            this.chkbox_Log = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lst_AGV
            // 
            this.lst_AGV.Dock = System.Windows.Forms.DockStyle.Top;
            this.lst_AGV.FullRowSelect = true;
            this.lst_AGV.HideSelection = false;
            this.lst_AGV.Location = new System.Drawing.Point(0, 0);
            this.lst_AGV.Name = "lst_AGV";
            this.lst_AGV.Size = new System.Drawing.Size(1902, 176);
            this.lst_AGV.TabIndex = 0;
            this.lst_AGV.UseCompatibleStateImageBehavior = false;
            this.lst_AGV.SelectedIndexChanged += new System.EventHandler(this.lst_AGV_SelectedIndexChanged);
            // 
            // lst_stage
            // 
            this.lst_stage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lst_stage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lst_stage.FullRowSelect = true;
            this.lst_stage.HideSelection = false;
            this.lst_stage.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lst_stage.Location = new System.Drawing.Point(0, 176);
            this.lst_stage.Name = "lst_stage";
            this.lst_stage.Size = new System.Drawing.Size(1902, 140);
            this.lst_stage.TabIndex = 0;
            this.lst_stage.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeader2
            // 
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lst_inspection
            // 
            this.lst_inspection.Dock = System.Windows.Forms.DockStyle.Top;
            this.lst_inspection.FullRowSelect = true;
            this.lst_inspection.HideSelection = false;
            this.lst_inspection.Location = new System.Drawing.Point(0, 316);
            this.lst_inspection.Name = "lst_inspection";
            this.lst_inspection.Size = new System.Drawing.Size(1902, 130);
            this.lst_inspection.TabIndex = 1;
            this.lst_inspection.UseCompatibleStateImageBehavior = false;
            // 
            // tmr_DBSelect
            // 
            this.tmr_DBSelect.Interval = 1000;
            this.tmr_DBSelect.Tick += new System.EventHandler(this.tmr_DBSelect_Tick);
            // 
            // lst_AGVComm
            // 
            this.lst_AGVComm.Dock = System.Windows.Forms.DockStyle.Left;
            this.lst_AGVComm.FormattingEnabled = true;
            this.lst_AGVComm.ItemHeight = 15;
            this.lst_AGVComm.Location = new System.Drawing.Point(0, 446);
            this.lst_AGVComm.Name = "lst_AGVComm";
            this.lst_AGVComm.Size = new System.Drawing.Size(888, 587);
            this.lst_AGVComm.TabIndex = 4;
            // 
            // lst_StageComm
            // 
            this.lst_StageComm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lst_StageComm.FormattingEnabled = true;
            this.lst_StageComm.ItemHeight = 15;
            this.lst_StageComm.Location = new System.Drawing.Point(888, 446);
            this.lst_StageComm.Name = "lst_StageComm";
            this.lst_StageComm.Size = new System.Drawing.Size(1014, 587);
            this.lst_StageComm.TabIndex = 4;
            // 
            // btn_Read_log
            // 
            this.btn_Read_log.Location = new System.Drawing.Point(404, 67);
            this.btn_Read_log.Name = "btn_Read_log";
            this.btn_Read_log.Size = new System.Drawing.Size(198, 114);
            this.btn_Read_log.TabIndex = 5;
            this.btn_Read_log.Text = "Read Log";
            this.btn_Read_log.UseVisualStyleBackColor = true;
            this.btn_Read_log.Visible = false;
            this.btn_Read_log.Click += new System.EventHandler(this.btn_Read_log_Click);
            // 
            // chkbox_Log
            // 
            this.chkbox_Log.AutoSize = true;
            this.chkbox_Log.Location = new System.Drawing.Point(12, 12);
            this.chkbox_Log.Name = "chkbox_Log";
            this.chkbox_Log.Size = new System.Drawing.Size(128, 19);
            this.chkbox_Log.TabIndex = 6;
            this.chkbox_Log.Text = "Auto Read Log";
            this.chkbox_Log.UseVisualStyleBackColor = true;
            this.chkbox_Log.CheckedChanged += new System.EventHandler(this.chkbox_Log_CheckedChanged);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.chkbox_Log);
            this.Controls.Add(this.btn_Read_log);
            this.Controls.Add(this.lst_StageComm);
            this.Controls.Add(this.lst_AGVComm);
            this.Controls.Add(this.lst_inspection);
            this.Controls.Add(this.lst_stage);
            this.Controls.Add(this.lst_AGV);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form3";
            this.Text = "Comm. log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lst_AGV;
        private System.Windows.Forms.ListView lst_stage;
        private System.Windows.Forms.ListView lst_inspection;
        private System.Windows.Forms.Timer tmr_DBSelect;
        private System.Windows.Forms.ListBox lst_AGVComm;
        private System.Windows.Forms.ListBox lst_StageComm;
        private System.Windows.Forms.Button btn_Read_log;
        private System.Windows.Forms.CheckBox chkbox_Log;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}