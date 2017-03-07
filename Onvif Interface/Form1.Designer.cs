namespace Onvif_Interface
{
    partial class Form1
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
            this.btnGetOnvifInfo = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblDeviceTime = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFirmware = new System.Windows.Forms.Label();
            this.lblSerial = new System.Windows.Forms.Label();
            this.lblHwID = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.gbxDeviceInfo = new System.Windows.Forms.GroupBox();
            this.lbxCapabilities = new System.Windows.Forms.ListBox();
            this.lblCapabilities = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1.SuspendLayout();
            this.gbxDeviceInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetOnvifInfo
            // 
            this.btnGetOnvifInfo.Location = new System.Drawing.Point(37, 47);
            this.btnGetOnvifInfo.Name = "btnGetOnvifInfo";
            this.btnGetOnvifInfo.Size = new System.Drawing.Size(109, 30);
            this.btnGetOnvifInfo.TabIndex = 2;
            this.btnGetOnvifInfo.Text = "Get Device Info";
            this.btnGetOnvifInfo.UseVisualStyleBackColor = true;
            this.btnGetOnvifInfo.Click += new System.EventHandler(this.btnGetOnvifInfo_Click);
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(37, 21);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(110, 20);
            this.txtIP.TabIndex = 0;
            this.txtIP.Text = "127.0.0.1";
            // 
            // lblDeviceTime
            // 
            this.lblDeviceTime.AutoSize = true;
            this.lblDeviceTime.Location = new System.Drawing.Point(6, 88);
            this.lblDeviceTime.Name = "lblDeviceTime";
            this.lblDeviceTime.Size = new System.Drawing.Size(36, 13);
            this.lblDeviceTime.TabIndex = 2;
            this.lblDeviceTime.Text = "Time: ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 420);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssLbl
            // 
            this.tssLbl.Name = "tssLbl";
            this.tssLbl.Size = new System.Drawing.Size(39, 17);
            this.tssLbl.Text = "Status";
            // 
            // lblFirmware
            // 
            this.lblFirmware.AutoSize = true;
            this.lblFirmware.Location = new System.Drawing.Point(6, 34);
            this.lblFirmware.Name = "lblFirmware";
            this.lblFirmware.Size = new System.Drawing.Size(52, 13);
            this.lblFirmware.TabIndex = 4;
            this.lblFirmware.Text = "Firmware:";
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.Location = new System.Drawing.Point(6, 52);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(46, 13);
            this.lblSerial.TabIndex = 5;
            this.lblSerial.Text = "Serial #:";
            // 
            // lblHwID
            // 
            this.lblHwID.AutoSize = true;
            this.lblHwID.Location = new System.Drawing.Point(6, 70);
            this.lblHwID.Name = "lblHwID";
            this.lblHwID.Size = new System.Drawing.Size(70, 13);
            this.lblHwID.TabIndex = 6;
            this.lblHwID.Text = "Hardware ID:";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(6, 16);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(39, 13);
            this.lblModel.TabIndex = 7;
            this.lblModel.Text = "Model:";
            // 
            // gbxDeviceInfo
            // 
            this.gbxDeviceInfo.Controls.Add(this.lblCapabilities);
            this.gbxDeviceInfo.Controls.Add(this.lbxCapabilities);
            this.gbxDeviceInfo.Controls.Add(this.lblModel);
            this.gbxDeviceInfo.Controls.Add(this.lblHwID);
            this.gbxDeviceInfo.Controls.Add(this.lblSerial);
            this.gbxDeviceInfo.Controls.Add(this.lblFirmware);
            this.gbxDeviceInfo.Controls.Add(this.lblDeviceTime);
            this.gbxDeviceInfo.Location = new System.Drawing.Point(294, 21);
            this.gbxDeviceInfo.Name = "gbxDeviceInfo";
            this.gbxDeviceInfo.Size = new System.Drawing.Size(260, 281);
            this.gbxDeviceInfo.TabIndex = 8;
            this.gbxDeviceInfo.TabStop = false;
            this.gbxDeviceInfo.Text = "Device Info";
            // 
            // lbxCapabilities
            // 
            this.lbxCapabilities.FormattingEnabled = true;
            this.lbxCapabilities.Location = new System.Drawing.Point(9, 139);
            this.lbxCapabilities.Name = "lbxCapabilities";
            this.lbxCapabilities.Size = new System.Drawing.Size(245, 134);
            this.lbxCapabilities.TabIndex = 9;
            // 
            // lblCapabilities
            // 
            this.lblCapabilities.AutoSize = true;
            this.lblCapabilities.Location = new System.Drawing.Point(9, 123);
            this.lblCapabilities.Name = "lblCapabilities";
            this.lblCapabilities.Size = new System.Drawing.Size(112, 13);
            this.lblCapabilities.TabIndex = 10;
            this.lblCapabilities.Text = "Supported Capabilities";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(154, 21);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(66, 20);
            this.numPort.TabIndex = 1;
            this.numPort.Value = new decimal(new int[] {
            8251,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.gbxDeviceInfo);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.btnGetOnvifInfo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbxDeviceInfo.ResumeLayout(false);
            this.gbxDeviceInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetOnvifInfo;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblDeviceTime;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssLbl;
        private System.Windows.Forms.Label lblFirmware;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.Label lblHwID;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.GroupBox gbxDeviceInfo;
        private System.Windows.Forms.ListBox lbxCapabilities;
        private System.Windows.Forms.Label lblCapabilities;
        private System.Windows.Forms.NumericUpDown numPort;
    }
}

