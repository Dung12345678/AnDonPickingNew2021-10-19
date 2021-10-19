namespace BMS
{
	partial class frmSettingPickingTime
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettingPickingTime));
			this.grdData = new DevExpress.XtraGrid.GridControl();
			this.grvData = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colLocation = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colTimePicking = new DevExpress.XtraGrid.Columns.GridColumn();
			this.mnuMenu = new System.Windows.Forms.ToolStrip();
			this.btnNew = new System.Windows.Forms.ToolStripButton();
			this.btnEdit = new System.Windows.Forms.ToolStripButton();
			this.btnDelete = new System.Windows.Forms.ToolStripButton();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.btnCancel = new System.Windows.Forms.ToolStripButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtLocation = new System.Windows.Forms.TextBox();
			this.pickerTime = new System.Windows.Forms.DateTimePicker();
			((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grvData)).BeginInit();
			this.mnuMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// grdData
			// 
			this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdData.Location = new System.Drawing.Point(2, 170);
			this.grdData.MainView = this.grvData;
			this.grdData.Name = "grdData";
			this.grdData.Size = new System.Drawing.Size(704, 281);
			this.grdData.TabIndex = 21;
			this.grdData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvData});
			// 
			// grvData
			// 
			this.grvData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold);
			this.grvData.Appearance.HeaderPanel.Options.UseFont = true;
			this.grvData.Appearance.Row.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold);
			this.grvData.Appearance.Row.Options.UseFont = true;
			this.grvData.ColumnPanelRowHeight = 50;
			this.grvData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colLocation,
            this.colTimePicking});
			this.grvData.GridControl = this.grdData;
			this.grvData.Name = "grvData";
			this.grvData.OptionsBehavior.Editable = false;
			this.grvData.RowHeight = 50;
			// 
			// colID
			// 
			this.colID.Caption = "ID";
			this.colID.FieldName = "ID";
			this.colID.Name = "colID";
			// 
			// colLocation
			// 
			this.colLocation.Caption = "Location";
			this.colLocation.FieldName = "Location";
			this.colLocation.Name = "colLocation";
			this.colLocation.Visible = true;
			this.colLocation.VisibleIndex = 0;
			this.colLocation.Width = 481;
			// 
			// colTimePicking
			// 
			this.colTimePicking.Caption = "Time Picking";
			this.colTimePicking.DisplayFormat.FormatString = "mm:ss";
			this.colTimePicking.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			this.colTimePicking.FieldName = "TimePicking";
			this.colTimePicking.Name = "colTimePicking";
			this.colTimePicking.Visible = true;
			this.colTimePicking.VisibleIndex = 1;
			this.colTimePicking.Width = 748;
			// 
			// mnuMenu
			// 
			this.mnuMenu.AutoSize = false;
			this.mnuMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
			this.mnuMenu.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mnuMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.mnuMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.mnuMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnEdit,
            this.btnDelete,
            this.btnSave,
            this.btnCancel});
			this.mnuMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.mnuMenu.Location = new System.Drawing.Point(0, 0);
			this.mnuMenu.Name = "mnuMenu";
			this.mnuMenu.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.mnuMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.mnuMenu.Size = new System.Drawing.Size(708, 42);
			this.mnuMenu.TabIndex = 22;
			this.mnuMenu.Text = "toolStrip2";
			// 
			// btnNew
			// 
			this.btnNew.AutoSize = false;
			this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
			this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(55, 40);
			this.btnNew.Tag = "Form_Department_Add";
			this.btnNew.Text = "&Add";
			this.btnNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.AutoSize = false;
			this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
			this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(55, 40);
			this.btnEdit.Tag = "Form_Department_Edit";
			this.btnEdit.Text = "Edit";
			this.btnEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.AutoSize = false;
			this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
			this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(55, 40);
			this.btnDelete.Tag = "Form_Department_Del";
			this.btnDelete.Text = "Delete";
			this.btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnSave
			// 
			this.btnSave.AutoSize = false;
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(55, 40);
			this.btnSave.Text = "Save";
			this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnSave.Visible = false;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnCancel.AutoSize = false;
			this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
			this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(55, 40);
			this.btnCancel.Text = "Cancel";
			this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnCancel.Visible = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Times New Roman", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(14, 121);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(249, 43);
			this.label2.TabIndex = 47;
			this.label2.Text = "Takt time (s): ";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Times New Roman", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(14, 58);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 43);
			this.label1.TabIndex = 46;
			this.label1.Text = "Location:";
			// 
			// txtLocation
			// 
			this.txtLocation.Font = new System.Drawing.Font("Times New Roman", 28F, System.Drawing.FontStyle.Bold);
			this.txtLocation.Location = new System.Drawing.Point(266, 58);
			this.txtLocation.Name = "txtLocation";
			this.txtLocation.Size = new System.Drawing.Size(430, 50);
			this.txtLocation.TabIndex = 49;
			// 
			// pickerTime
			// 
			this.pickerTime.CustomFormat = "mm:ss";
			this.pickerTime.Font = new System.Drawing.Font("Times New Roman", 28F, System.Drawing.FontStyle.Bold);
			this.pickerTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.pickerTime.Location = new System.Drawing.Point(266, 114);
			this.pickerTime.Name = "pickerTime";
			this.pickerTime.ShowUpDown = true;
			this.pickerTime.Size = new System.Drawing.Size(131, 50);
			this.pickerTime.TabIndex = 50;
			// 
			// frmSettingPickingTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 453);
			this.Controls.Add(this.pickerTime);
			this.Controls.Add(this.txtLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.mnuMenu);
			this.Controls.Add(this.grdData);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSettingPickingTime";
			this.Text = "Setting Picking Time";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettingPickingTime_FormClosing);
			this.Load += new System.EventHandler(this.frmSettingPickingTime_Load);
			((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grvData)).EndInit();
			this.mnuMenu.ResumeLayout(false);
			this.mnuMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DevExpress.XtraGrid.GridControl grdData;
		private DevExpress.XtraGrid.Views.Grid.GridView grvData;
		private DevExpress.XtraGrid.Columns.GridColumn colID;
		private DevExpress.XtraGrid.Columns.GridColumn colLocation;
		private DevExpress.XtraGrid.Columns.GridColumn colTimePicking;
		private System.Windows.Forms.ToolStrip mnuMenu;
		private System.Windows.Forms.ToolStripButton btnNew;
		private System.Windows.Forms.ToolStripButton btnEdit;
		private System.Windows.Forms.ToolStripButton btnDelete;
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.ToolStripButton btnCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtLocation;
		private System.Windows.Forms.DateTimePicker pickerTime;
	}
}