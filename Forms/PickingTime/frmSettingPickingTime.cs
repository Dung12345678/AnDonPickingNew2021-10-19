using BMS.Business;
using BMS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmSettingPickingTime : Form
	{
		public PickingTimeModel _Picking;
		private bool _isAdd;
		public frmSettingPickingTime()
		{
			InitializeComponent();
		}
		private void frmSettingPickingTime_Load(object sender, EventArgs e)
		{
			LoadCboLoaction();
			ClearInterface();
			LoadData();
		}
		private void LoadData()
		{
			// Load PickingTime lên gridview
			DataTable data = TextUtils.LoadDataFromSP("spGetPickingTime", "PickingTime", new string[] { }, new object[] { });
			grdData.DataSource = data;
		}
		private void LoadCboLoaction()
		{

		}
		private void SetInterface(bool isEdit)
		{
			grdData.Enabled = !isEdit;

			btnSave.Visible = isEdit;
			btnCancel.Visible = isEdit;

			btnNew.Visible = !isEdit;
			btnEdit.Visible = !isEdit;
			btnDelete.Visible = !isEdit;
		}
		private void btnNew_Click(object sender, EventArgs e)
		{
			SetInterface(true);
			_isAdd = true;
			ClearInterface();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (_isAdd)
			{
				_Picking = new PickingTimeModel();
			}
			else
			{
				int ID = Convert.ToInt32(grvData.GetRowCellValue(grvData.FocusedRowHandle, colID).ToString());
				_Picking = PickingTimeBO.Instance.FindByPK(ID) as PickingTimeModel;
			}
			_Picking.Location = txtLocation.Text.Trim();
			_Picking.TimePicking = pickerTime.Value;
			string t = (pickerTime.Value.ToString("mm:ss"));
			if (_isAdd)
			{
				PickingTimeBO.Instance.Insert(_Picking);
			}
			else
			{
				PickingTimeBO.Instance.Update(_Picking);
			}
			SetInterface(false);
			ClearInterface();

			LoadData();
		}
		void ClearInterface()
		{
			txtLocation.Text = "";
			DateTime date = DateTime.Now.Date;
			pickerTime.Value = date.AddMinutes(0);
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (!grvData.IsDataRow(grvData.FocusedRowHandle))
				return;
			SetInterface(true);
			_isAdd = false;
			pickerTime.Value = TextUtils.ToDate3(grvData.GetFocusedRowCellValue(colTimePicking));
			txtLocation.Text = TextUtils.ToString(grvData.GetFocusedRowCellValue(colLocation));
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			SetInterface(false);
			ClearInterface();
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (!grvData.IsDataRow(grvData.FocusedRowHandle))
				return;
			int ID = TextUtils.ToInt(grvData.GetRowCellValue(grvData.FocusedRowHandle, colID).ToString());
			string strName = grvData.GetRowCellValue(grvData.FocusedRowHandle, colLocation).ToString();

			DialogResult result = MessageBox.Show(String.Format("Are you want to delete [{0}] ?", strName), TextUtils.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.No) return;
			try
			{
				PickingTimeBO.Instance.Delete(ID);
				LoadData();
			}
			catch (Exception)
			{
				MessageBox.Show("An error occurred during processing, please try again later!");
			}
		}

		private void frmSettingPickingTime_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
	}
}
