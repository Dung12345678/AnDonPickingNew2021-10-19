using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMS
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//DataSet dts = TextUtils.GetListDataFromSP("spGetAndonDetails", "AnDonDetails"
			//		, new string[1] { "@AndonID" }
			//		, new object[1] { 281 });
			//DataTable dataTableCD1 = dts.Tables[0];
			//DataTable dataTableCD2 = dts.Tables[1];
			//DataTable dataTableCD3 = dts.Tables[2];
			WidthColumn();
		}
	
		void WidthColumn()
		{
			int WidthGrd = grdData.Size.Width;
			int columnGrd = (WidthGrd - 230) / (grvData.Columns.Count - 1);
			for (int i = 1; i < grvData.Columns.Count; i++)
			{
				grvData.VisibleColumns[i].Width = columnGrd;
			}
		}
		private void grdData_Click(object sender, EventArgs e)
		{

		}
	}
}
