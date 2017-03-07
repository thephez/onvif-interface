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
            this.lblCapabilities = new System.Windows.Forms.Label();
            this.lbxCapabilities = new System.Windows.Forms.ListBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.gbxPtzControl = new System.Windows.Forms.GroupBox();
            this.numPtzCmdSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblCmdSpeed = new System.Windows.Forms.Label();
            this.lblPtzLocationZoom = new System.Windows.Forms.Label();
            this.lblPtzLocationY = new System.Windows.Forms.Label();
            this.lblPtzLocationX = new System.Windows.Forms.Label();
            this.lblPtzLocation = new System.Windows.Forms.Label();
            this.lbxPtzInfo = new System.Windows.Forms.ListBox();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.lblCmdDuration = new System.Windows.Forms.Label();
            this.numCmdDuration = new System.Windows.Forms.NumericUpDown();
            this.btnPanRight = new System.Windows.Forms.Button();
            this.btnPanLeft = new System.Windows.Forms.Button();
            this.btnTiltDown = new System.Windows.Forms.Button();
            this.btnTiltUp = new System.Windows.Forms.Button();
            this.btnSetConnectInfo = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.gbxDeviceInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.gbxPtzControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPtzCmdSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCmdDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetOnvifInfo
            // 
            this.btnGetOnvifInfo.Location = new System.Drawing.Point(77, 448);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 534);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(866, 22);
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
            this.gbxDeviceInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxDeviceInfo.Controls.Add(this.lblCapabilities);
            this.gbxDeviceInfo.Controls.Add(this.lbxCapabilities);
            this.gbxDeviceInfo.Controls.Add(this.lblModel);
            this.gbxDeviceInfo.Controls.Add(this.lblHwID);
            this.gbxDeviceInfo.Controls.Add(this.lblSerial);
            this.gbxDeviceInfo.Controls.Add(this.lblFirmware);
            this.gbxDeviceInfo.Controls.Add(this.btnGetOnvifInfo);
            this.gbxDeviceInfo.Controls.Add(this.lblDeviceTime);
            this.gbxDeviceInfo.Location = new System.Drawing.Point(236, 21);
            this.gbxDeviceInfo.Name = "gbxDeviceInfo";
            this.gbxDeviceInfo.Size = new System.Drawing.Size(260, 484);
            this.gbxDeviceInfo.TabIndex = 8;
            this.gbxDeviceInfo.TabStop = false;
            this.gbxDeviceInfo.Text = "Device Info";
            // 
            // lblCapabilities
            // 
            this.lblCapabilities.AutoSize = true;
            this.lblCapabilities.Location = new System.Drawing.Point(9, 111);
            this.lblCapabilities.Name = "lblCapabilities";
            this.lblCapabilities.Size = new System.Drawing.Size(112, 13);
            this.lblCapabilities.TabIndex = 10;
            this.lblCapabilities.Text = "Supported Capabilities";
            // 
            // lbxCapabilities
            // 
            this.lbxCapabilities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxCapabilities.FormattingEnabled = true;
            this.lbxCapabilities.Location = new System.Drawing.Point(9, 127);
            this.lbxCapabilities.Name = "lbxCapabilities";
            this.lbxCapabilities.Size = new System.Drawing.Size(245, 316);
            this.lbxCapabilities.TabIndex = 9;
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
            // gbxPtzControl
            // 
            this.gbxPtzControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxPtzControl.Controls.Add(this.numPtzCmdSpeed);
            this.gbxPtzControl.Controls.Add(this.lblCmdSpeed);
            this.gbxPtzControl.Controls.Add(this.lblPtzLocationZoom);
            this.gbxPtzControl.Controls.Add(this.lblPtzLocationY);
            this.gbxPtzControl.Controls.Add(this.lblPtzLocationX);
            this.gbxPtzControl.Controls.Add(this.lblPtzLocation);
            this.gbxPtzControl.Controls.Add(this.lbxPtzInfo);
            this.gbxPtzControl.Controls.Add(this.btnZoomOut);
            this.gbxPtzControl.Controls.Add(this.btnZoomIn);
            this.gbxPtzControl.Controls.Add(this.lblCmdDuration);
            this.gbxPtzControl.Controls.Add(this.numCmdDuration);
            this.gbxPtzControl.Controls.Add(this.btnPanRight);
            this.gbxPtzControl.Controls.Add(this.btnPanLeft);
            this.gbxPtzControl.Controls.Add(this.btnTiltDown);
            this.gbxPtzControl.Controls.Add(this.btnTiltUp);
            this.gbxPtzControl.Location = new System.Drawing.Point(519, 21);
            this.gbxPtzControl.Name = "gbxPtzControl";
            this.gbxPtzControl.Size = new System.Drawing.Size(286, 484);
            this.gbxPtzControl.TabIndex = 9;
            this.gbxPtzControl.TabStop = false;
            this.gbxPtzControl.Text = "PTZ Control";
            // 
            // numPtzCmdSpeed
            // 
            this.numPtzCmdSpeed.Location = new System.Drawing.Point(115, 109);
            this.numPtzCmdSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPtzCmdSpeed.Name = "numPtzCmdSpeed";
            this.numPtzCmdSpeed.Size = new System.Drawing.Size(46, 20);
            this.numPtzCmdSpeed.TabIndex = 14;
            this.numPtzCmdSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPtzCmdSpeed.ThousandsSeparator = true;
            this.numPtzCmdSpeed.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblCmdSpeed
            // 
            this.lblCmdSpeed.AutoSize = true;
            this.lblCmdSpeed.Location = new System.Drawing.Point(6, 111);
            this.lblCmdSpeed.Name = "lblCmdSpeed";
            this.lblCmdSpeed.Size = new System.Drawing.Size(105, 13);
            this.lblCmdSpeed.TabIndex = 13;
            this.lblCmdSpeed.Text = "Command Speed (%)";
            // 
            // lblPtzLocationZoom
            // 
            this.lblPtzLocationZoom.AutoSize = true;
            this.lblPtzLocationZoom.Location = new System.Drawing.Point(180, 165);
            this.lblPtzLocationZoom.Name = "lblPtzLocationZoom";
            this.lblPtzLocationZoom.Size = new System.Drawing.Size(85, 13);
            this.lblPtzLocationZoom.TabIndex = 12;
            this.lblPtzLocationZoom.Text = "Location (zoom):";
            // 
            // lblPtzLocationY
            // 
            this.lblPtzLocationY.AutoSize = true;
            this.lblPtzLocationY.Location = new System.Drawing.Point(180, 147);
            this.lblPtzLocationY.Name = "lblPtzLocationY";
            this.lblPtzLocationY.Size = new System.Drawing.Size(65, 13);
            this.lblPtzLocationY.TabIndex = 11;
            this.lblPtzLocationY.Text = "Location (y):";
            // 
            // lblPtzLocationX
            // 
            this.lblPtzLocationX.AutoSize = true;
            this.lblPtzLocationX.Location = new System.Drawing.Point(180, 130);
            this.lblPtzLocationX.Name = "lblPtzLocationX";
            this.lblPtzLocationX.Size = new System.Drawing.Size(65, 13);
            this.lblPtzLocationX.TabIndex = 10;
            this.lblPtzLocationX.Text = "Location (x):";
            // 
            // lblPtzLocation
            // 
            this.lblPtzLocation.AutoSize = true;
            this.lblPtzLocation.Location = new System.Drawing.Point(173, 111);
            this.lblPtzLocation.Name = "lblPtzLocation";
            this.lblPtzLocation.Size = new System.Drawing.Size(72, 13);
            this.lblPtzLocation.TabIndex = 9;
            this.lblPtzLocation.Text = "PTZ Location";
            // 
            // lbxPtzInfo
            // 
            this.lbxPtzInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxPtzInfo.FormattingEnabled = true;
            this.lbxPtzInfo.Location = new System.Drawing.Point(9, 187);
            this.lbxPtzInfo.Name = "lbxPtzInfo";
            this.lbxPtzInfo.Size = new System.Drawing.Size(266, 290);
            this.lbxPtzInfo.TabIndex = 8;
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(195, 65);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(50, 25);
            this.btnZoomOut.TabIndex = 7;
            this.btnZoomOut.Text = "Out";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(195, 34);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(50, 25);
            this.btnZoomIn.TabIndex = 6;
            this.btnZoomIn.Text = "In";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            // 
            // lblCmdDuration
            // 
            this.lblCmdDuration.AutoSize = true;
            this.lblCmdDuration.Location = new System.Drawing.Point(6, 137);
            this.lblCmdDuration.Name = "lblCmdDuration";
            this.lblCmdDuration.Size = new System.Drawing.Size(97, 13);
            this.lblCmdDuration.TabIndex = 5;
            this.lblCmdDuration.Text = "Command Duration";
            this.lblCmdDuration.Visible = false;
            // 
            // numCmdDuration
            // 
            this.numCmdDuration.Location = new System.Drawing.Point(105, 135);
            this.numCmdDuration.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCmdDuration.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCmdDuration.Name = "numCmdDuration";
            this.numCmdDuration.Size = new System.Drawing.Size(56, 20);
            this.numCmdDuration.TabIndex = 4;
            this.numCmdDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCmdDuration.ThousandsSeparator = true;
            this.numCmdDuration.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numCmdDuration.Visible = false;
            // 
            // btnPanRight
            // 
            this.btnPanRight.Location = new System.Drawing.Point(113, 45);
            this.btnPanRight.Name = "btnPanRight";
            this.btnPanRight.Size = new System.Drawing.Size(50, 25);
            this.btnPanRight.TabIndex = 3;
            this.btnPanRight.Text = "Right";
            this.btnPanRight.UseVisualStyleBackColor = true;
            // 
            // btnPanLeft
            // 
            this.btnPanLeft.Location = new System.Drawing.Point(13, 45);
            this.btnPanLeft.Name = "btnPanLeft";
            this.btnPanLeft.Size = new System.Drawing.Size(50, 25);
            this.btnPanLeft.TabIndex = 2;
            this.btnPanLeft.Text = "Left";
            this.btnPanLeft.UseVisualStyleBackColor = true;
            // 
            // btnTiltDown
            // 
            this.btnTiltDown.Location = new System.Drawing.Point(63, 68);
            this.btnTiltDown.Name = "btnTiltDown";
            this.btnTiltDown.Size = new System.Drawing.Size(50, 25);
            this.btnTiltDown.TabIndex = 1;
            this.btnTiltDown.Text = "Down";
            this.btnTiltDown.UseVisualStyleBackColor = true;
            // 
            // btnTiltUp
            // 
            this.btnTiltUp.Location = new System.Drawing.Point(63, 22);
            this.btnTiltUp.Name = "btnTiltUp";
            this.btnTiltUp.Size = new System.Drawing.Size(50, 25);
            this.btnTiltUp.TabIndex = 0;
            this.btnTiltUp.Text = "Up";
            this.btnTiltUp.UseVisualStyleBackColor = true;
            // 
            // btnSetConnectInfo
            // 
            this.btnSetConnectInfo.Location = new System.Drawing.Point(37, 47);
            this.btnSetConnectInfo.Name = "btnSetConnectInfo";
            this.btnSetConnectInfo.Size = new System.Drawing.Size(109, 30);
            this.btnSetConnectInfo.TabIndex = 10;
            this.btnSetConnectInfo.Text = "Set Connection Info";
            this.btnSetConnectInfo.UseVisualStyleBackColor = true;
            this.btnSetConnectInfo.Click += new System.EventHandler(this.btnSetConnectInfo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 556);
            this.Controls.Add(this.btnSetConnectInfo);
            this.Controls.Add(this.gbxPtzControl);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.gbxDeviceInfo);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtIP);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbxDeviceInfo.ResumeLayout(false);
            this.gbxDeviceInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.gbxPtzControl.ResumeLayout(false);
            this.gbxPtzControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPtzCmdSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCmdDuration)).EndInit();
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
        private System.Windows.Forms.GroupBox gbxPtzControl;
        private System.Windows.Forms.Button btnPanRight;
        private System.Windows.Forms.Button btnPanLeft;
        private System.Windows.Forms.Button btnTiltDown;
        private System.Windows.Forms.Button btnTiltUp;
        private System.Windows.Forms.NumericUpDown numCmdDuration;
        private System.Windows.Forms.Label lblCmdDuration;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.ListBox lbxPtzInfo;
        private System.Windows.Forms.Label lblPtzLocationX;
        private System.Windows.Forms.Label lblPtzLocation;
        private System.Windows.Forms.Label lblPtzLocationY;
        private System.Windows.Forms.Label lblPtzLocationZoom;
        private System.Windows.Forms.Button btnSetConnectInfo;
        private System.Windows.Forms.NumericUpDown numPtzCmdSpeed;
        private System.Windows.Forms.Label lblCmdSpeed;
    }
}

