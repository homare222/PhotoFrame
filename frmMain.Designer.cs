namespace PhotoFrame
{
    partial class frmMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pctBox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItemChangeFormSize = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTopMost = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pctBox)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pctBox
            // 
            this.pctBox.BackColor = System.Drawing.Color.White;
            this.pctBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pctBox.Location = new System.Drawing.Point(0, 0);
            this.pctBox.Margin = new System.Windows.Forms.Padding(0);
            this.pctBox.Name = "pctBox";
            this.pctBox.Size = new System.Drawing.Size(250, 250);
            this.pctBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctBox.TabIndex = 0;
            this.pctBox.TabStop = false;
            this.pctBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pctBox_MouseDown);
            this.pctBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pctBox_MouseMove);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemInfo,
            this.toolStripMenuItem,
            this.ToolStripMenuItemChangeFormSize,
            this.ToolStripMenuItemTopMost,
            this.ToolStripMenuItemSettings,
            this.ToolStripMenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(161, 120);
            // 
            // ToolStripMenuItemInfo
            // 
            this.ToolStripMenuItemInfo.Name = "ToolStripMenuItemInfo";
            this.ToolStripMenuItemInfo.Size = new System.Drawing.Size(160, 22);
            this.ToolStripMenuItemInfo.Text = "情報";
            // 
            // toolStripMenuItem
            // 
            this.toolStripMenuItem.Name = "toolStripMenuItem";
            this.toolStripMenuItem.Size = new System.Drawing.Size(157, 6);
            // 
            // ToolStripMenuItemChangeFormSize
            // 
            this.ToolStripMenuItemChangeFormSize.Name = "ToolStripMenuItemChangeFormSize";
            this.ToolStripMenuItemChangeFormSize.Size = new System.Drawing.Size(160, 22);
            this.ToolStripMenuItemChangeFormSize.Text = "最大化";
            this.ToolStripMenuItemChangeFormSize.Click += new System.EventHandler(this.ToolStripMenuItemChangeFormSize_Click);
            // 
            // ToolStripMenuItemTopMost
            // 
            this.ToolStripMenuItemTopMost.CheckOnClick = true;
            this.ToolStripMenuItemTopMost.Name = "ToolStripMenuItemTopMost";
            this.ToolStripMenuItemTopMost.Size = new System.Drawing.Size(160, 22);
            this.ToolStripMenuItemTopMost.Text = "常に前面に表示";
            this.ToolStripMenuItemTopMost.CheckedChanged += new System.EventHandler(this.ToolStripMenuItemTopMost_CheckedChanged);
            // 
            // ToolStripMenuItemSettings
            // 
            this.ToolStripMenuItemSettings.Name = "ToolStripMenuItemSettings";
            this.ToolStripMenuItemSettings.Size = new System.Drawing.Size(160, 22);
            this.ToolStripMenuItemSettings.Text = "設定";
            this.ToolStripMenuItemSettings.Click += new System.EventHandler(this.ToolStripMenuItemSettings_Click);
            // 
            // ToolStripMenuItemExit
            // 
            this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
            this.ToolStripMenuItemExit.Size = new System.Drawing.Size(160, 22);
            this.ToolStripMenuItemExit.Text = "終了";
            this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notify";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 250);
            this.Controls.Add(this.pctBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmMain_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pctBox)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pctBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemInfo;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemChangeFormSize;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTopMost;
    }
}

