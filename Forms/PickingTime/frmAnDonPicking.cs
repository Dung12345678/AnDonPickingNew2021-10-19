using BMS.Business;
using BMS.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using InControls.Common;
using InControls.PLC.FX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMS
{
	public delegate void FontSize(decimal fontSize1, decimal fontSize2, decimal fontSize3, decimal fontSize4, decimal fontSize5, decimal fontSize6, decimal fontSize7);
	public delegate void SendData(string value, string CD, string XK);
	public partial class frmAnDonPicking : Form
	{
		Thread _threadResetTakt;
		Thread _threadLoadAndon;
		Thread _threadUpdateCurrent;
		Thread _threadLocation;
		Thread _threadLoadAllK;
		Thread _threadLoadDelay;
		Thread _threadShowColor;
		Thread _threadKSDAndSD;
		public ManualResetEvent _EventUpdateAndon;
		public AndonModel _OAndonModel;
		public AndonPickingConfigModel _andonConfig;
		private FxSerialDeamon _FxSerial;
		string _cmd;
		FxCommandResponse _res;
		//đổi màu kt2 và kn2
		List<CCheckOrderKT> lstColorColKT1 = new List<CCheckOrderKT>();
		List<CCheckOrderKT> lstColorColKT2 = new List<CCheckOrderKT>();
		List<CCheckOrderKN> lstColorColKN2 = new List<CCheckOrderKN>();
		List<CCheckOrderKN> lstColorColKN1 = new List<CCheckOrderKN>();
		List<string> lstNameColCasseKSD = new List<string>();
		List<string> lstNameColCasseSD = new List<string>();
		List<string> lstNameColMotorKSD = new List<string>();
		List<string> lstNameColMotorSD = new List<string>();
		List<NameColKN> lstNameColKN = new List<NameColKN>();
		List<CCheckKCasse> lstCheckCasseOK = new List<CCheckKCasse>();
		List<CCheckKMotor> lstCheckMotorOK = new List<CCheckKMotor>();
		List<CCheckKCasse> lstCheckOrderCasseAwait = new List<CCheckKCasse>();
		List<CCheckKMotor> lstCheckOrderMotorAwait = new List<CCheckKMotor>();

		int oldHeightGrid = 0;
		bool _isBreakTime = false;

		int _StartKT1 = 0;
		int _StartKT2 = 0;
		int _StartKN1 = 0;
		int _StartKN2 = 0;
		int _StartKMotor = 0;
		int _StartKCasse = 0;

		int _KT1 = 0;
		int _KT2 = 0;
		int _KN1 = 0;
		int _KN2 = 0;
		int _KCasse = 0;
		int _KMotor = 0;

		int _countTakt = 0;
		int _countTakt1 = 0;
		int _countTakt2 = 0;
		int _countTakt3 = 0;
		int _countTakt4 = 0;
		int _countTakt5 = 0;
		int _Stock = 0;

		int _countTaktIn1 = 0;
		int _countTaktIn2 = 0;
		int _countTaktOut1 = 0;
		int _countTaktOut2 = 0;
		int _countTaktMotor = 0;
		int _countTaktCasse = 0;

		int columnGrd = 0;

		public frmAnDonPicking()
		{
			InitializeComponent();
		}
		private void frmAnDonPicking_Load(object sender, EventArgs e)
		{
			frmServer frm = new frmServer();
			frm.ShowInTaskbar = false;
			frm._SendData = new SendData(sendData);
			frm.Show();

			load();

			_OAndonModel = new AndonModel();

			// Chạy thread load Andon theo thời gian hiện tại
			_threadLoadAndon = new Thread(new ThreadStart(LoadAndon));
			_threadLoadAndon.IsBackground = true;
			_threadLoadAndon.Start();

			// Thread update lại plan current
			_threadUpdateCurrent = new Thread(new ThreadStart(threadUpdatePlanCurrent));
			_threadUpdateCurrent.IsBackground = true;
			_threadUpdateCurrent.Start();

			// Load font size trong bảng FontConfig
			ArrayList arrAndonConfig = AndonPickingConfigBO.Instance.FindAll();
			if (arrAndonConfig.Count > 0)
			{
				_andonConfig = (AndonPickingConfigModel)arrAndonConfig[0];
				fontSizefn(_andonConfig.FontSize1, _andonConfig.FontSize2, _andonConfig.FontSize3,
					_andonConfig.FontSize4, _andonConfig.FontSize5, _andonConfig.FontSize6, _andonConfig.FontSize7);
				OpenPort(TextUtils.ToInt(_andonConfig.ComPLC));
			}
			//loadTime();
			loadReMainTime();
			threadAllK();

			// Thread reset Takt time khi các CD hoàn thành.
			_threadResetTakt = new Thread(new ThreadStart(threadResetTaktTime));
			_threadResetTakt.IsBackground = true;
			_threadResetTakt.Start();
			// Thread reset Takt time khi các CD hoàn thành.
			_threadKSDAndSD = new Thread(new ThreadStart(SDAndKSD));
			_threadKSDAndSD.IsBackground = true;
			_threadKSDAndSD.Start();
			////Thread Load Time
			//_threadLoadTime = new Thread(new ThreadStart(loadTime));
			//_threadLoadTime.IsBackground = true;
			//_threadLoadTime.Start();

			////Đẩy giá trị từ F12 đến F1
			//_threadLocation = new Thread(new ThreadStart(threadLocation));
			//_threadLocation.IsBackground = true;
			//_threadLocation.Start();

			//////Load All Kho 
			//_threadLoadAllK = new Thread(new ThreadStart(threadAllK));
			//_threadLoadAllK.IsBackground = true;
			//_threadLoadAllK.Start();

			//Load delay
			_threadLoadDelay = new Thread(new ThreadStart(LoadDelay));
			_threadLoadDelay.IsBackground = true;
			_threadLoadDelay.Start();

			//Load màu khi kho bắn Order 

			oldHeightGrid = grdData.Height;
			WidthColumn();
			loadFit();

			LoadprogressBar();
			WightRemainTime();
		}
		//hiển thị chiều dài rộng của 3 cột remaintime, delay, sự cố 
		void WightRemainTime()
		{
			tableLayoutPanel1.ColumnCount = colCD1.Width + colCD2.Width + colCD3.Width;
			TableLayoutColumnStyleCollection styles = this.tableLayoutPanel1.ColumnStyles;
			styles[0].Width = 232;
			styles[2].Width = colCD1.Width + colCD2.Width + colCD3.Width;
			styles[3].Width = colCD4.Width + colCD5.Width + colCD6.Width;
			styles[4].Width = colCD7.Width + colCD8.Width + 3;
			styles[1].Width = tableLayoutPanel1.Width - (styles[0].Width + styles[2].Width + styles[3].Width + styles[4].Width);
		}
		void WidthColumn()
		{
			int WidthGrd = grdData.Size.Width;
			columnGrd = (WidthGrd - 230) / (grvData.Columns.Count - 1);
			for (int i = 1; i < grvData.Columns.Count; i++)
			{
				grvData.VisibleColumns[i].Width = columnGrd;
			}

		}
		/// <summary>
		/// Load Số lần Delay và Time Delay
		/// </summary>
		void LoadDelay()
		{
			while (true)
			{
				Thread.Sleep(1000);
				try
				{
					DataSet dts = TextUtils.GetListDataFromSP("spGetAndonDetailsByXK", "AnDonDetails"
						   , new string[] { }
						   , new object[] { });
					DataTable dataKT1 = dts.Tables[0];
					DataTable dataKT2 = dts.Tables[1];
					DataTable dataKN1 = dts.Tables[2];
					DataTable dataKN2 = dts.Tables[3];
					DataTable dataKCasse = dts.Tables[4];
					DataTable dataKMotor = dts.Tables[5];

					this.Invoke((MethodInvoker)delegate
					{
						btnKindIn1.Text = TextUtils.ToString(dataKT1.Rows[0]["TotalDelayNum"]);
						btnTimingIn1.Text = TextUtils.ToString(dataKT1.Rows[0]["TotalDelayTime"]);
						btnErrorIn1.Text = TextUtils.ToString(dataKT1.Rows[0]["TotalRiskNum"]);

						btnKindIn2.Text = TextUtils.ToString(dataKT2.Rows[0]["TotalDelayNum"]);
						btnTimingIn2.Text = TextUtils.ToString(dataKT2.Rows[0]["TotalDelayTime"]);
						btnErrorIn2.Text = TextUtils.ToString(dataKT2.Rows[0]["TotalRiskNum"]);

						btnKindOut1.Text = TextUtils.ToString(dataKN1.Rows[0]["TotalDelayNum"]);
						btnTimingOut1.Text = TextUtils.ToString(dataKN1.Rows[0]["TotalDelayTime"]);
						btnErrorOut1.Text = TextUtils.ToString(dataKN1.Rows[0]["TotalRiskNum"]);

						btnKindOut2.Text = TextUtils.ToString(dataKN2.Rows[0]["TotalDelayNum"]);
						btnTimingOut2.Text = TextUtils.ToString(dataKN2.Rows[0]["TotalDelayTime"]);
						btnErrorOut2.Text = TextUtils.ToString(dataKN2.Rows[0]["TotalRiskNum"]);

						btnKindMotor.Text = TextUtils.ToString(dataKMotor.Rows[0]["TotalDelayNum"]);
						btnTimingMotor.Text = TextUtils.ToString(dataKMotor.Rows[0]["TotalDelayTime"]);
						btnErrorMotor.Text = TextUtils.ToString(dataKMotor.Rows[0]["TotalRiskNum"]);

						btnKindCasse.Text = TextUtils.ToString(dataKCasse.Rows[0]["TotalDelayNum"]);
						btnTimingCasse.Text = TextUtils.ToString(dataKCasse.Rows[0]["TotalDelayTime"]);
						btnErrorCasse.Text = TextUtils.ToString(dataKCasse.Rows[0]["TotalRiskNum"]);
					});
				}
				catch (Exception ex)
				{

				}
			}
		}
		void threadAllK()
		{
			try
			{
				//Hiển thị các cột tương ứng
				DataTable dt = TextUtils.LoadDataFromSP("spGetAddAutoXKNew", "a", new string[] { }, new object[] { });
				//Hiển thị kho trong 
				//Xóa các giá trị trong bảng KT
				for (int i = 1; i <= 41; i++)
				{
					grvData.SetRowCellValue(0, "F" + i, "");
					grvData.SetRowCellValue(1, "F" + i, "");
					grvData.SetRowCellValue(2, "F" + i, "");
					grvData.SetRowCellValue(3, "F" + i, "");
				}
				lstColorColKT1.Clear();
				lstColorColKT2.Clear();
				lstColorColKN1.Clear();
				lstColorColKN2.Clear();
				lstCheckOrderCasseAwait.Clear();
				lstCheckOrderMotorAwait.Clear();
				lstNameColMotorKSD.Clear();
				lstNameColCasseKSD.Clear();
				lstNameColCasseSD.Clear();
				lstNameColMotorSD.Clear();
				lstCheckCasseOK.Clear();
				lstCheckMotorOK.Clear();
				for (int i = 1; i <= dt.Rows.Count; i++)
				{
					for (int j = 0; j < 6; j++)
					{
						int check = TextUtils.ToInt(dt.Rows[i - 1][dt.Columns[3 + j].ColumnName]);
						int Row = 0;
						int CheckKho2 = 0;
						string ColumnName = TextUtils.ToString(dt.Columns[3 + j].ColumnName);
						switch (ColumnName)
						{
							case "KT1":
								Row = 0;
								break;
							case "KT2":
								Row = 0;
								CheckKho2 = 1;
								break;
							case "KN1":
								Row = 1;
								break;
							case "KN2":
								CheckKho2 = 1;
								Row = 1;
								break;
							case "KCasse":
								Row = 3;
								break;
							case "KMotor":
								Row = 2;
								break;
							default:
								break;
						}
						//Check 1 trạng thái chờ , 2 Đã xong , 3 không sử dụng
						switch (check)
						{
							case 1:
								grvData.SetRowCellValue(Row, "F" + i, TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]));
								//Add hiển thị bảng tạm KT1
								if (CheckKho2 == 0 && Row == 0)
								{
									//Hiển thị màu KT2
									CCheckOrderKT cCheckOrderKT = new CCheckOrderKT();
									cCheckOrderKT.ColumnF = "F" + i;
									cCheckOrderKT.ValueKT = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstColorColKT1.Add(cCheckOrderKT);
								}
								//Add hiển thị bảng tạm KT2
								else if (CheckKho2 == 1 && Row == 0)
								{
									//Hiển thị màu KT2
									CCheckOrderKT cCheckOrderKT = new CCheckOrderKT();
									cCheckOrderKT.ColumnF = "F" + i;
									cCheckOrderKT.ValueKT = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstColorColKT2.Add(cCheckOrderKT);
								}

								//Add hiển thị bảng tạm KN1
								if (CheckKho2 == 0 && Row == 1)
								{
									CCheckOrderKN cCheckOrderKN1 = new CCheckOrderKN();
									cCheckOrderKN1.ColumnF = "F" + i;
									cCheckOrderKN1.ValueKN = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstColorColKN1.Add(cCheckOrderKN1);
								}
								//Add hiển thị bảng tạm KN2
								else if (CheckKho2 == 1 && Row == 1)
								{
									//Hiển thị màu KT2
									CCheckOrderKN cCheckOrderKN2 = new CCheckOrderKN();
									cCheckOrderKN2.ColumnF = "F" + i;
									cCheckOrderKN2.ValueKN = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstColorColKN2.Add(cCheckOrderKN2);
								}

								if (Row == 2)
								{
									CCheckKMotor cCheckKMotor = new CCheckKMotor();
									cCheckKMotor.Order = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									cCheckKMotor.ColumnF = "F" + i;
									lstCheckOrderMotorAwait.Add(cCheckKMotor);
								}
								else if (Row == 3)
								{
									CCheckKCasse checkKCasseOK = new CCheckKCasse();
									checkKCasseOK.ColumnF = "F" + i;
									checkKCasseOK.Order = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstCheckOrderCasseAwait.Add(checkKCasseOK);
								}
								break;

							case 2:
								grvData.SetRowCellValue(Row, "F" + i, TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]));
								////Hiển thị trạng thái đã xong KT1
								//if (Row == 0)
								//{

								//}
								//else if (Row == 1)//Hiển thị trạng thái đã xong KN2
								//{

								//}
								if (Row == 2)
								{
									CCheckKMotor cCheckKMotor = new CCheckKMotor();
									cCheckKMotor.Order = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									cCheckKMotor.ColumnF = "F" + i;
									lstCheckMotorOK.Add(cCheckKMotor);
								}
								else if (Row == 3)
								{
									CCheckKCasse checkKCasseOK = new CCheckKCasse();
									checkKCasseOK.ColumnF = "F" + i;
									checkKCasseOK.Order = TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]);
									lstCheckCasseOK.Add(checkKCasseOK);
								}
								break;
							case 3:
								grvData.SetRowCellValue(Row, "F" + i, "");
								if (Row == 2)
								{
									lstNameColMotorKSD.Add("F" + i);
								}
								else if (Row == 3)
								{
									lstNameColCasseKSD.Add("F" + i);
								}
								break;
							case 0:
								if (Row == 2)
								{
									grvData.SetRowCellValue(Row, "F" + i, TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]));
									lstNameColMotorSD.Add("F" + i);
								}
								else if (Row == 3)
								{
									grvData.SetRowCellValue(Row, "F" + i, TextUtils.ToString(dt.Rows[i - 1]["OrderCode"]) + TextUtils.ToString(dt.Rows[i - 1]["Cnt"]));
									lstNameColCasseSD.Add("F" + i);
								}
								break;
							default:
								break;
						}
					}
				}
			}
			catch
			{

			}
		}
		/// <summary>
		/// Load các row trong grid đều nhau
		/// </summary>
		void loadFit()
		{
			if (grvData.RowCount > 0)
			{
				grvData.RowHeight = -1;
				int totalHeightRow = this.getSumHeightRows();
				if ((oldHeightGrid - grvData.ColumnPanelRowHeight - 30) > totalHeightRow)
				{
					grvData.RowHeight = (oldHeightGrid - grvData.ColumnPanelRowHeight - 15) / grvData.RowCount;
					lbChi.Location = new System.Drawing.Point((columnGrd * 11) + 230, 40 + grdData.Location.Y);
					lbChi.Size = new System.Drawing.Size(4, grvData.RowHeight + 5);
					lbChiNgang.Location = new System.Drawing.Point((columnGrd * 11) + 230, 40 + grvData.RowHeight + grdData.Location.Y);
					lbChiNgang.Size = new System.Drawing.Size((columnGrd * 9), 4);
					lbChi1.Location = new System.Drawing.Point((columnGrd * 20) + 230, 40 + grvData.RowHeight + grdData.Location.Y);
					lbChi1.Size = new System.Drawing.Size(4, grvData.RowHeight * 3 + 5);
				}
			}
		}
		int getSumHeightRows()
		{
			int total = 0;
			GridViewInfo vi = grvData.GetViewInfo() as GridViewInfo;
			for (int i = 0; i < grvData.RowCount; i++)
			{
				GridRowInfo ri = vi.RowsInfo.FindRow(i);
				if (ri != null)
					total += ri.Bounds.Height;
			}

			return total;
		}
		int ColumnF = 30;
		/// <summary>
		/// Nhận giá trị tính toán để hiển thị lên gridview
		/// </summary>
		/// <param name="value">gửi Mã Order và PID</param>
		/// <param name="CD"> Mã công đoạn hoặc mã kho</param>
		/// <param name="XK"> Nếu XK ='' là Line gửi lên còn != '' là XK gửi lên hoặc XK gửi số Order để hiển thị</param>
		void sendData(string value, string CD, string XK)
		{
			this.Invoke((MethodInvoker)delegate
			{
				try
				{
					// khi gặp sự cố báo gì 
					value = value.ToUpper().Trim();
					if (value == "Error")
					{

					}
					if (value == "OK")
					{

					}
					//Line gửi TCP
					if (XK.Trim() == "")
					{
						string Order = "";
						string[] arr = value.Split(' ');
						if (arr.Length > 1)
						{
							//Order = arr[1].Substring(0, arr[1].Length - 1);
							Order = arr[1];
						}
						value = value.Substring(0, value.Length - 1);
						DataSet dt = TextUtils.GetListDataFromSP("spLoadAnDonPickingCD", "LoadAricleID", new string[] { "@OrderCode" }, new object[] { Order });
						DataTable dtCasse = dt.Tables[0];
						DataTable dtMotor = dt.Tables[1];
						grvData.SetRowCellValue(0, CD, arr[0]);
						grvData.SetRowCellValue(1, CD, arr[0]);

						if (dtCasse.Rows.Count > 0)
						{
							string Casse = TextUtils.ToString(dtCasse.Rows[0]["ArticleID"]);
							grvData.SetRowCellValue(3, CD, Casse);
						}
						else
						{
							grvData.SetRowCellValue(3, CD, "");
						}
						if (dtMotor.Rows.Count > 0)
						{
							string Motor = TextUtils.ToString(dtMotor.Rows[0]["ArticleID"]);
							grvData.SetRowCellValue(2, CD, Motor);
						}
						else
						{
							grvData.SetRowCellValue(2, CD, "");
						}
					}
					else
					{
						threadAllK();
					}
				}
				catch
				{

				}
			});

		}
		void SDAndKSD()
		{
			while (true)
			{
				try
				{
					Thread.Sleep(500);
					DataTable dt = TextUtils.Select("SELECT TOP 1 * FROM [ShiStock].[dbo].[StatusColorStock]");
					if (dt == null || dt.Rows.Count == 0) continue;
					for (int i = 1; i < dt.Columns.Count; i++)
					{
						string Value = TextUtils.ToString(dt.Rows[0][i]);
						string[] ValueSplit = Value.Split(';');
						ProgressBarControl control = (ProgressBarControl)tableLayoutPanel1.Controls.Find("progressBar" + (dt.Columns[i].ColumnName), false)[0];
						control.Invoke((MethodInvoker)delegate
						{
							//Hiển thị thời gian bắt đầu
							loadTime(TextUtils.ToInt(ValueSplit[1]), dt.Columns[i].ColumnName);
							// sử dụng
							if (TextUtils.ToInt(ValueSplit[0]) == 0)
							{
								//_KT1 = 0;
								//control.BackColor = Color.Cyan;
								//Load lại thời gian và màu của progress
								if (dt.Columns[i].ColumnName == "KT2" || dt.Columns[i].ColumnName == "KN2")
								{
									control.Properties.EndColor = Color.DarkTurquoise;
									control.Properties.StartColor = Color.DarkTurquoise;
								}
								else
								{
									control.Properties.EndColor = Color.Cyan;
									control.Properties.StartColor = Color.Cyan;
								}
							}
							else //không Sử dụng
							{
								//_KT1 = 1;
								if (dt.Columns[i].ColumnName == "KT1")
									_StartKT1 = 0;
								if (dt.Columns[i].ColumnName == "KT2")
									_StartKT2 = 0;
								if (dt.Columns[i].ColumnName == "KN1")
									_StartKN1 = 0;
								if (dt.Columns[i].ColumnName == "KN2")
									_StartKN2 = 0;
								if (dt.Columns[i].ColumnName == "KCasse")
									_StartKCasse = 0;
								if (dt.Columns[i].ColumnName == "KMotor")
									_StartKMotor = 0;
								control.Position = TextUtils.ToInt(ValueSplit[1]);
								control.Properties.EndColor = Color.FromArgb(255, 192, 128);
								control.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
						});
					}
				}
				catch (Exception)
				{

				}
			}
		}
		/// <summary>
		/// Load khi nhận được tín hiệu k sử dụng và sử dụng
		/// </summary>
		/// <param name="value"></param>
		/// <param name="CD"></param>

		void LoadKSD(string value, string CD)
		{
			try
			{
				this.Invoke((MethodInvoker)delegate
					{
						if (value == "KSD")
						{
							if (CD == "KT1")
							{
								_KT1 = 1;
								progressBarKT1.Position = _countTakt;
								progressBarKT1.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKT1.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
							if (CD == "KT2")
							{
								_KT2 = 1;
								progressBarKT2.Position = _countTakt1;
								progressBarKT2.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKT2.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
							if (CD == "KN1")
							{
								_KN1 = 1;
								progressBarKN1.Position = _countTakt2;
								progressBarKN1.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKN1.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
							if (CD == "KN2")
							{
								progressBarKN2.Position = _countTakt3;
								_KN2 = 1;
								progressBarKN2.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKN2.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
							if (CD == "KCasse")
							{
								_KCasse = 1;
								progressBarKCasse.Position = _countTakt4;
								progressBarKCasse.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKCasse.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
							if (CD == "KMotor")
							{
								_KMotor = 1;
								progressBarKMotor.Position = _countTakt5;
								progressBarKMotor.Properties.EndColor = Color.FromArgb(255, 192, 128);
								progressBarKMotor.Properties.StartColor = Color.FromArgb(255, 192, 128);
							}
						}
						if (value == "SD")
						{
							if (CD == "KT1")
							{
								_KT1 = 0;
								progressBarKT1.Properties.EndColor = Color.Cyan;
								progressBarKT1.Properties.StartColor = Color.Cyan;
							}
							if (CD == "KT2")
							{
								_KT2 = 0;
								progressBarKT2.Properties.EndColor = Color.DarkTurquoise;
								progressBarKT2.Properties.StartColor = Color.DarkTurquoise;
							}
							if (CD == "KN1")
							{
								_KN1 = 0;
								progressBarKN1.Properties.EndColor = Color.Cyan;
								progressBarKN1.Properties.StartColor = Color.Cyan;
							}
							if (CD == "KN2")
							{
								_KN2 = 0;
								progressBarKN2.Properties.EndColor = Color.DarkTurquoise;
								progressBarKN2.Properties.StartColor = Color.DarkTurquoise;
							}
							if (CD == "KCasse")
							{
								_KCasse = 0;
								progressBarKCasse.Properties.EndColor = Color.Cyan;
								progressBarKCasse.Properties.StartColor = Color.Cyan;
							}
							if (CD == "KMotor")
							{
								_KMotor = 0;
								progressBarKMotor.Properties.EndColor = Color.Cyan;
								progressBarKMotor.Properties.StartColor = Color.Cyan;
							}
						}
					});
			}
			catch
			{

			}
		}
		/// <summary>
		/// Sét font chữ
		/// </summary>
		/// <param name="fSize1"></param>
		/// <param name="fSize2"></param>
		/// <param name="fSize3"></param>
		/// <param name="fSize4"></param>
		/// <param name="fSize5"></param>
		/// <param name="fSize6"></param>
		/// <param name="fSize7"></param>
		private void fontSizefn(decimal fSize1, decimal fSize2, decimal fSize3, decimal fSize4, decimal fSize5, decimal fSize6, decimal fSize7)
		{
			try
			{
				txtLocation.Font = new Font(txtLocation.Font.FontFamily, (float)fSize1, txtLocation.Font.Style);
				txtPickingTime.Font = new Font(txtPickingTime.Font.FontFamily, (float)fSize1, txtPickingTime.Font.Style);
				txtRemainTime.Font = new Font(txtRemainTime.Font.FontFamily, (float)fSize1, txtRemainTime.Font.Style);
				txtTimeLine.Font = new Font(txtTimeLine.Font.FontFamily, (float)fSize1, txtTimeLine.Font.Style);
				txtDelay.Font = new Font(txtDelay.Font.FontFamily, (float)fSize1, txtDelay.Font.Style);
				btnSuCo.Font = new Font(btnSuCo.Font.FontFamily, (float)fSize1, btnSuCo.Font.Style);

				btnLocationStockIn1.Font = new Font(btnLocationStockIn1.Font.FontFamily, (float)fSize2, btnLocationStockIn1.Font.Style);
				btnLocationStockOut1.Font = new Font(btnLocationStockOut1.Font.FontFamily, (float)fSize2, btnLocationStockOut1.Font.Style);
				btnLocationStockMotor.Font = new Font(btnLocationStockMotor.Font.FontFamily, (float)fSize2, btnLocationStockMotor.Font.Style);
				btnLocationStockCasse.Font = new Font(btnLocationStockCasse.Font.FontFamily, (float)fSize2, btnLocationStockCasse.Font.Style);


				btnStockIn1.Font = new Font(btnStockIn1.Font.FontFamily, (float)fSize3, btnStockIn1.Font.Style);
				btnStockIn2.Font = new Font(btnStockIn2.Font.FontFamily, (float)fSize3, btnStockIn2.Font.Style);
				btnStockOut1.Font = new Font(btnStockOut1.Font.FontFamily, (float)fSize3, btnStockOut1.Font.Style);
				btnStockOut2.Font = new Font(btnStockOut2.Font.FontFamily, (float)fSize3, btnStockOut2.Font.Style);
				btnStockMotor.Font = new Font(btnStockMotor.Font.FontFamily, (float)fSize3, btnStockMotor.Font.Style);
				btnStockCasse.Font = new Font(btnStockCasse.Font.FontFamily, (float)fSize3, btnStockCasse.Font.Style);

				btnRemainTimeStockIn1.Font = new Font(btnRemainTimeStockIn1.Font.FontFamily, (float)fSize3, btnRemainTimeStockIn1.Font.Style);
				btnRemainTimeStockIn2.Font = new Font(btnRemainTimeStockIn2.Font.FontFamily, (float)fSize3, btnRemainTimeStockIn2.Font.Style);
				btnRemainTimeStockOut1.Font = new Font(btnRemainTimeStockOut1.Font.FontFamily, (float)fSize3, btnRemainTimeStockOut1.Font.Style);
				btnRemainTimeStockOut2.Font = new Font(btnRemainTimeStockOut2.Font.FontFamily, (float)fSize3, btnRemainTimeStockOut2.Font.Style);
				btnRemainTimeStockMotor.Font = new Font(btnRemainTimeStockMotor.Font.FontFamily, (float)fSize3, btnRemainTimeStockMotor.Font.Style);
				btnRemainTimeStockCasse.Font = new Font(btnRemainTimeStockCasse.Font.FontFamily, (float)fSize3, btnRemainTimeStockCasse.Font.Style);

				btnKindIn1.Font = new Font(btnKindIn1.Font.FontFamily, (float)fSize3, btnKindIn1.Font.Style);
				btnKindIn2.Font = new Font(btnKindIn2.Font.FontFamily, (float)fSize3, btnKindIn2.Font.Style);
				btnKindOut1.Font = new Font(btnKindOut1.Font.FontFamily, (float)fSize3, btnKindOut1.Font.Style);
				btnKindOut2.Font = new Font(btnKindOut2.Font.FontFamily, (float)fSize3, btnKindOut2.Font.Style);
				btnKindMotor.Font = new Font(btnKindMotor.Font.FontFamily, (float)fSize3, btnKindMotor.Font.Style);
				btnKindCasse.Font = new Font(btnKindCasse.Font.FontFamily, (float)fSize3, btnKindCasse.Font.Style);

				btnTimingIn1.Font = new Font(btnTimingIn1.Font.FontFamily, (float)fSize3, btnTimingIn1.Font.Style);
				btnTimingIn2.Font = new Font(btnTimingIn2.Font.FontFamily, (float)fSize3, btnTimingIn2.Font.Style);
				btnTimingOut1.Font = new Font(btnTimingOut1.Font.FontFamily, (float)fSize3, btnTimingOut1.Font.Style);
				btnTimingOut2.Font = new Font(btnTimingOut2.Font.FontFamily, (float)fSize3, btnTimingOut2.Font.Style);
				btnTimingMotor.Font = new Font(btnTimingMotor.Font.FontFamily, (float)fSize3, btnTimingMotor.Font.Style);
				btnTimingCasse.Font = new Font(btnTimingCasse.Font.FontFamily, (float)fSize3, btnTimingCasse.Font.Style);

				btnErrorIn1.Font = new Font(btnErrorIn1.Font.FontFamily, (float)fSize3, btnErrorIn1.Font.Style);
				btnErrorIn2.Font = new Font(btnErrorIn2.Font.FontFamily, (float)fSize3, btnErrorIn2.Font.Style);
				btnErrorOut1.Font = new Font(btnErrorOut1.Font.FontFamily, (float)fSize3, btnErrorOut1.Font.Style);
				btnErrorOut2.Font = new Font(btnErrorOut2.Font.FontFamily, (float)fSize3, btnErrorOut2.Font.Style);
				btnErrorMotor.Font = new Font(btnErrorMotor.Font.FontFamily, (float)fSize3, btnErrorMotor.Font.Style);
				btnErrorCasse.Font = new Font(btnErrorCasse.Font.FontFamily, (float)fSize3, btnErrorCasse.Font.Style);

				this.colLocation.AppearanceCell.Font = new Font(this.colLocation.AppearanceCell.Font.FontFamily, (float)fSize4, this.colLocation.AppearanceCell.Font.Style);
				this.colLocation.AppearanceHeader.Font = new Font(this.colLocation.AppearanceHeader.Font.FontFamily, (float)fSize4, this.colLocation.AppearanceHeader.Font.Style);
			}
			catch
			{

			}
		}
		void loadReMainTime()
		{
			try
			{
				btnRemainTimeStockIn1.Text = btnStockIn1.Text.Trim();
				btnRemainTimeStockIn2.Text = btnStockIn2.Text.Trim();
				btnRemainTimeStockOut1.Text = btnStockOut1.Text.Trim();
				btnRemainTimeStockOut2.Text = btnStockOut2.Text.Trim();
				btnRemainTimeStockMotor.Text = btnStockMotor.Text.Trim();
				btnRemainTimeStockCasse.Text = btnStockCasse.Text.Trim();
			}
			catch
			{

			}
		}
		///
		/// Load time cuar ProgressBar khi khởi tạo
		/// </summary>
		void LoadprogressBar()
		{
			_countTakt = TextUtils.ToInt(btnStockIn1.Text);
			progressBarKT1.Properties.Minimum = 0;
			progressBarKT1.Properties.Maximum = _countTakt;
			progressBarKT1.Position = _countTakt;
			progressBarKT1.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKT1.Properties.StartColor = Color.FromArgb(255, 192, 128);

			_countTakt1 = TextUtils.ToInt(btnStockIn2.Text);
			progressBarKT2.Properties.Minimum = 0;
			progressBarKT2.Properties.Maximum = _countTakt1;
			progressBarKT2.Position = _countTakt1;
			progressBarKT2.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKT2.Properties.StartColor = Color.FromArgb(255, 192, 128);

			_countTakt2 = TextUtils.ToInt(btnStockOut1.Text);
			progressBarKN1.Properties.Minimum = 0;
			progressBarKN1.Properties.Maximum = _countTakt2;
			progressBarKN1.Position = _countTakt2;
			progressBarKN1.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKN1.Properties.StartColor = Color.FromArgb(255, 192, 128);

			_countTakt3 = TextUtils.ToInt(btnStockOut1.Text);
			progressBarKN2.Properties.Minimum = 0;
			progressBarKN2.Properties.Maximum = _countTakt3;
			progressBarKN2.Position = _countTakt3;
			progressBarKN2.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKN2.Properties.StartColor = Color.FromArgb(255, 192, 128);

			_countTakt4 = TextUtils.ToInt(btnStockMotor.Text);
			progressBarKMotor.Properties.Minimum = 0;
			progressBarKMotor.Properties.Maximum = _countTakt4;
			progressBarKMotor.Position = _countTakt4;
			progressBarKMotor.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKMotor.Properties.StartColor = Color.FromArgb(255, 192, 128);

			_countTakt5 = TextUtils.ToInt(btnStockCasse.Text);
			progressBarKCasse.Properties.Minimum = 0;
			progressBarKCasse.Properties.Maximum = _countTakt5;
			progressBarKCasse.Position = _countTakt5;
			progressBarKCasse.Properties.EndColor = Color.FromArgb(255, 192, 128);
			progressBarKCasse.Properties.StartColor = Color.FromArgb(255, 192, 128);
		}
		/// <summary>
		/// Load Time cho các Kho
		/// </summary>
		private void loadTime(int Takt, string StockName)
		{
			try
			{
				// chuyen giay ve h phut giay
				//TimeSpan t = TimeSpan.FromMinutes(TextUtils.ToInt(TextUtils.ToDate(TextUtils.ToString(dt.Rows[1]["TimePicking"])).ToString("mm")));
				//string time = t.ToString(@"hh\:mm\:ss");

				TimeSpan t = TimeSpan.FromSeconds(Takt);
				//Kho trong 1
				if (StockName == "KT1" && _StartKT1 == 0)
				{
					_StartKT1 = 1;
					btnLocationStockIn1.Text = "KHO TRONG";
					btnRemainTimeStockIn1.Text = btnStockIn1.Text = t.ToString(@"mm\:ss");
					_countTaktIn1 = _countTakt = Takt;
					progressBarKT1.Properties.Minimum = 0;
					progressBarKT1.Properties.Maximum = _countTakt;
					progressBarKT1.Position = _countTakt;
				}
				// Kho trong 2
				if (StockName == "KT2" && _StartKT2 == 0)
				{
					_StartKT2 = 1;
					btnRemainTimeStockIn2.Text = btnStockIn2.Text = t.ToString(@"mm\:ss");
					_countTaktIn2 = _countTakt1 = Takt;
					progressBarKT2.Properties.Minimum = 0;
					progressBarKT2.Properties.Maximum = _countTakt1;
					progressBarKT2.Position = _countTakt1;
				}
				// Kho ngoài 1 
				if (StockName == "KN1" && _StartKN1 == 0)
				{
					_StartKN1 = 1;
					btnLocationStockOut1.Text = "KHO NGOÀI";
					btnRemainTimeStockOut1.Text = btnStockOut1.Text = t.ToString(@"mm\:ss");
					_countTaktOut1 = _countTakt2 = Takt;
					progressBarKN1.Properties.Minimum = 0;
					progressBarKN1.Properties.Maximum = _countTakt2;
					progressBarKN1.Position = _countTakt2;
				}
				//Kho ngoài 2
				if (StockName == "KN2" && _StartKN2 == 0)
				{
					_StartKN2 = 1;
					btnRemainTimeStockOut2.Text = btnStockOut2.Text = t.ToString(@"mm\:ss");
					_countTaktOut2 = _countTakt3 = Takt;
					progressBarKN2.Properties.Minimum = 0;
					progressBarKN2.Properties.Maximum = _countTakt3;
					progressBarKN2.Position = _countTakt3;
				}
				//Kho motor
				if (StockName == "KMotor" && _StartKMotor == 0)
				{
					_StartKMotor = 1;
					btnLocationStockMotor.Text = "KHO MOTOR";
					btnRemainTimeStockMotor.Text = btnStockMotor.Text = t.ToString(@"mm\:ss");
					_countTaktMotor = _countTakt4 = Takt;
					progressBarKMotor.Properties.Minimum = 0;
					progressBarKMotor.Properties.Maximum = _countTakt4;
					progressBarKMotor.Position = _countTakt4;
				}
				//Kho casse
				if (StockName == "KCasse" && _StartKCasse == 0)
				{
					_StartKCasse = 1;
					btnLocationStockCasse.Text = "KHO CASSE";
					btnRemainTimeStockCasse.Text = btnStockCasse.Text = t.ToString(@"mm\:ss");
					_countTaktCasse = _countTakt5 = Takt;
					progressBarKCasse.Properties.Minimum = 0;
					progressBarKCasse.Properties.Maximum = _countTakt5;
					progressBarKCasse.Position = _countTakt5;
				}
			}
			catch
			{

			}

		}
		/// <summary>
		/// Lấy dữ liệu AnDon khi được add thời gian vào thì chạy tất cả 
		/// </summary>
		private void LoadAndon()
		{
			while (true)
			{
				Thread.Sleep(500);
				try
				{
					// Store load Andon theo ngày giờ hiện tại
					ArrayList arr = AndonBO.Instance.GetListObject("spGetAndonByDateTimeNow", new string[] { }, new object[] { });
					if (arr.Count > 0)
					{
						_OAndonModel = (AndonModel)arr[0];
					}
				}
				catch (Exception ex)
				{
					File.AppendAllText(Application.StartupPath + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
						DateTime.Now.ToString("HH:mm:ss") + ":LoadAndon(): " + ex.ToString() + Environment.NewLine);
				}
			}
		}

		/// <summary>
		/// Thời gian nghỉ giải lao thì dừng
		/// </summary>
		private void threadUpdatePlanCurrent()
		{
			while (true)
			{
				Thread.Sleep(200);
				try
				{
					if (_OAndonModel.ID == 0) continue;
					if (DateTime.Now >= _OAndonModel.ShiftStartTime && DateTime.Now <= _OAndonModel.StartTimeBreak1)
					{

					}
					else if (DateTime.Now >= _OAndonModel.EndTimeBreak1 && DateTime.Now <= _OAndonModel.StartTimeBreak2)
					{

					}
					else if (DateTime.Now >= _OAndonModel.EndTimeBreak2 && DateTime.Now <= _OAndonModel.StartTimeBreak3)
					{

					}
					else if (DateTime.Now >= _OAndonModel.EndTimeBreak3 && DateTime.Now <= _OAndonModel.StartTimeBreak4)
					{

					}
					else if (DateTime.Now >= _OAndonModel.EndTimeBreak4 && DateTime.Now <= _OAndonModel.ShiftEndTime)
					{

					}
					else
					{

						_isBreakTime = true;
						continue;
					}
					_isBreakTime = false;
					//_EventUpdateAndon.WaitOne();
				}
				catch (Exception ex)
				{
					File.AppendAllText(Application.StartupPath + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
						DateTime.Now.ToString("HH:mm:ss") + ":threadUpdatePlanCurrent(): " + ex.ToString() + Environment.NewLine);
				}
			}
		}

		/// <summary>
		/// Làm mới lại thời gian
		/// </summary>
		/// <param name="StockName">Tên kho cần làm mới </param>
		/// <param name="reset">true </param>
		void ResetTime(string StockName, bool reset)
		{
			try
			{
				if ((reset || StockName == "KT1") && _KT1 == 0)
				{
					progressBarKT1.Position = _countTakt;
					_countTaktIn1 = _countTakt;
				}
				if (reset || StockName == "KT2" && _KT2 == 0)
				{
					progressBarKT2.Position = _countTakt1;
					_countTaktIn2 = _countTakt1;
				}
				if (reset || StockName == "KN1" && _KN1 == 0)
				{
					progressBarKN1.Position = _countTakt2;
					_countTaktOut1 = _countTakt2;
				}
				if (reset || StockName == "KN2" && _KN2 == 0)
				{
					progressBarKN2.Position = _countTakt3;
					_countTaktOut2 = _countTakt3;
				}
				if (reset || StockName == "KMotor" && _KMotor == 0)
				{
					progressBarKMotor.Position = _countTakt4;
					_countTaktMotor = _countTakt4;
				}
				if (reset || StockName == "KCasse" && _KCasse == 0)
				{
					progressBarKCasse.Position = _countTakt5;
					_countTaktCasse = _countTakt5;
				}
			}
			catch
			{

			}

		}

		/// <summary>
		/// Load thời gian giảm dần mỗi giây
		/// </summary>
		private void threadResetTaktTime()
		{
			while (true)
			{
				Thread.Sleep(1000);
				if (_OAndonModel.ID == 0) continue;
				if (_isBreakTime) continue;

				if (_OAndonModel.ShiftEndTime < DateTime.Now)
				{
					ResetTime("", true);
					continue;
				}
				this.Invoke((MethodInvoker)delegate
				{
					try
					{
						// chuyen giay ve h phut giay
						//1:kho trong 1 ; 2:kho trong 2; 3:kho ngoài 1; 4:kho ngoài 2 ; 5:kho motor ; 6:kho casse

						//_KT1=0 Sử dụng, 1 không sử dụng và _StartKT1=1 truyền giá trị takt time. 0 mặc định chưa truyền
						//Kho trong1
						if (_StartKT1 == 1)
						{
							progressBarKT1.Position--;
							_countTaktIn1--;
							TimeSpan t = TimeSpan.FromSeconds(_countTaktIn1);
							if (_countTaktIn1 < 0)
								btnRemainTimeStockIn1.Text = "-" + t.ToString(@"mm\:ss");
							else
								btnRemainTimeStockIn1.Text = t.ToString(@"mm\:ss");
						}
						//kho trong 2
						if (_StartKT2 == 1)
						{
							progressBarKT2.Position--;
							_countTaktIn2--;
							TimeSpan t1 = TimeSpan.FromSeconds(_countTaktIn2);
							if (_countTaktIn2 < 0)
								btnRemainTimeStockIn2.Text = "-" + t1.ToString(@"mm\:ss");
							else
								btnRemainTimeStockIn2.Text = t1.ToString(@"mm\:ss");
						}
						if (_StartKN1 == 1)
						{
							// kho ngoài 1 
							progressBarKN1.Position--;
							_countTaktOut1--;
							TimeSpan t2 = TimeSpan.FromSeconds(_countTaktOut1);
							if (_countTaktOut1 < 0)
								btnRemainTimeStockOut1.Text = "-" + t2.ToString(@"mm\:ss");
							else
								btnRemainTimeStockOut1.Text = t2.ToString(@"mm\:ss");
						}
						if (_StartKN2 == 1)
						{
							//Kho ngoài 2
							progressBarKN2.Position--;
							_countTaktOut2--;
							TimeSpan t3 = TimeSpan.FromSeconds(_countTaktOut2);
							if (_countTaktOut2 < 0)
								btnRemainTimeStockOut2.Text = "-" + t3.ToString(@"mm\:ss");
							else
								btnRemainTimeStockOut2.Text = t3.ToString(@"mm\:ss");
						}
						//Kho motor
						if (_StartKMotor == 1)
						{

							progressBarKMotor.Position--;
							_countTaktMotor--;
							TimeSpan t4 = TimeSpan.FromSeconds(_countTaktMotor);
							if (_countTaktMotor < 0)
								btnRemainTimeStockMotor.Text = "-" + t4.ToString(@"mm\:ss");
							else
								btnRemainTimeStockMotor.Text = t4.ToString(@"mm\:ss");
						}
						//kho casse
						if (_StartKCasse == 1)
						{
							progressBarKCasse.Position--;
							_countTaktCasse--;
							TimeSpan t5 = TimeSpan.FromSeconds(_countTaktCasse);
							if (_countTaktCasse < 0)
								btnRemainTimeStockCasse.Text = "-" + t5.ToString(@"mm\:ss");
							else
								btnRemainTimeStockCasse.Text = t5.ToString(@"mm\:ss");
						}

					}
					catch
					{

					}
				});
			}
		}

		/// <summary>
		/// Khởi tạo dữ liệu grid view lúc mới load form
		/// </summary>
		void load()
		{
			try
			{
				DataTable _dtTotal = new DataTable();

				DataColumn dataColumn1 = new DataColumn("Loaction", typeof(string));
				DataColumn dataColumn2 = new DataColumn("CD1", typeof(string));
				DataColumn dataColumn3 = new DataColumn("CD2", typeof(string));
				DataColumn dataColumn4 = new DataColumn("CD3", typeof(string));
				DataColumn dataColumn5 = new DataColumn("CD4", typeof(string));
				DataColumn dataColumn6 = new DataColumn("CD5", typeof(string));
				DataColumn dataColumn7 = new DataColumn("CD6", typeof(string));
				DataColumn dataColumn8 = new DataColumn("CD7", typeof(string));
				DataColumn dataColumn9 = new DataColumn("CD8", typeof(string));

				DataColumn dataColumnF1 = new DataColumn("F1", typeof(string));
				DataColumn dataColumnF2 = new DataColumn("F2", typeof(string));
				DataColumn dataColumnF3 = new DataColumn("F3", typeof(string));
				DataColumn dataColumnF4 = new DataColumn("F4", typeof(string));
				DataColumn dataColumnF5 = new DataColumn("F5", typeof(string));
				DataColumn dataColumnF6 = new DataColumn("F6", typeof(string));
				DataColumn dataColumnF7 = new DataColumn("F7", typeof(string));
				DataColumn dataColumnF8 = new DataColumn("F8", typeof(string));
				DataColumn dataColumnF9 = new DataColumn("F9", typeof(string));
				DataColumn dataColumnF10 = new DataColumn("F10", typeof(string));
				DataColumn dataColumnF11 = new DataColumn("F11", typeof(string));
				DataColumn dataColumnF12 = new DataColumn("F12", typeof(string));
				DataColumn dataColumnF13 = new DataColumn("F13", typeof(string));
				DataColumn dataColumnF14 = new DataColumn("F14", typeof(string));
				DataColumn dataColumnF15 = new DataColumn("F15", typeof(string));
				DataColumn dataColumnF16 = new DataColumn("F16", typeof(string));
				DataColumn dataColumnF17 = new DataColumn("F17", typeof(string));
				DataColumn dataColumnF18 = new DataColumn("F18", typeof(string));
				DataColumn dataColumnF19 = new DataColumn("F19", typeof(string));
				DataColumn dataColumnF20 = new DataColumn("F20", typeof(string));
				DataColumn dataColumnF21 = new DataColumn("F21", typeof(string));
				DataColumn dataColumnF22 = new DataColumn("F22", typeof(string));
				DataColumn dataColumnF23 = new DataColumn("F23", typeof(string));
				DataColumn dataColumnF24 = new DataColumn("F24", typeof(string));
				DataColumn dataColumnF25 = new DataColumn("F25", typeof(string));
				DataColumn dataColumnF26 = new DataColumn("F26", typeof(string));
				DataColumn dataColumnF27 = new DataColumn("F27", typeof(string));
				DataColumn dataColumnF28 = new DataColumn("F28", typeof(string));
				DataColumn dataColumnF29 = new DataColumn("F29", typeof(string));
				DataColumn dataColumnF30 = new DataColumn("F30", typeof(string));
				DataColumn dataColumnF31 = new DataColumn("F31", typeof(string));
				DataColumn dataColumnF32 = new DataColumn("F32", typeof(string));
				DataColumn dataColumnF33 = new DataColumn("F33", typeof(string));
				DataColumn dataColumnF34 = new DataColumn("F34", typeof(string));
				DataColumn dataColumnF35 = new DataColumn("F35", typeof(string));
				DataColumn dataColumnF36 = new DataColumn("F36", typeof(string));
				DataColumn dataColumnF37 = new DataColumn("F37", typeof(string));
				DataColumn dataColumnF38 = new DataColumn("F38", typeof(string));
				DataColumn dataColumnF39 = new DataColumn("F39", typeof(string));
				DataColumn dataColumnF40 = new DataColumn("F40", typeof(string));

				_dtTotal.Columns.Add(dataColumn1);
				_dtTotal.Columns.Add(dataColumn2);
				_dtTotal.Columns.Add(dataColumn3);
				_dtTotal.Columns.Add(dataColumn4);
				_dtTotal.Columns.Add(dataColumn5);
				_dtTotal.Columns.Add(dataColumn6);
				_dtTotal.Columns.Add(dataColumn7);
				_dtTotal.Columns.Add(dataColumn8);
				_dtTotal.Columns.Add(dataColumn9);

				_dtTotal.Columns.Add(dataColumnF1);
				_dtTotal.Columns.Add(dataColumnF2);
				_dtTotal.Columns.Add(dataColumnF3);
				_dtTotal.Columns.Add(dataColumnF4);
				_dtTotal.Columns.Add(dataColumnF5);
				_dtTotal.Columns.Add(dataColumnF6);
				_dtTotal.Columns.Add(dataColumnF7);
				_dtTotal.Columns.Add(dataColumnF8);
				_dtTotal.Columns.Add(dataColumnF9);
				_dtTotal.Columns.Add(dataColumnF10);
				_dtTotal.Columns.Add(dataColumnF11);
				_dtTotal.Columns.Add(dataColumnF12);
				_dtTotal.Columns.Add(dataColumnF13);
				_dtTotal.Columns.Add(dataColumnF14);
				_dtTotal.Columns.Add(dataColumnF15);
				_dtTotal.Columns.Add(dataColumnF16);
				_dtTotal.Columns.Add(dataColumnF17);
				_dtTotal.Columns.Add(dataColumnF18);
				_dtTotal.Columns.Add(dataColumnF19);
				_dtTotal.Columns.Add(dataColumnF20);
				_dtTotal.Columns.Add(dataColumnF21);
				_dtTotal.Columns.Add(dataColumnF22);
				_dtTotal.Columns.Add(dataColumnF23);
				_dtTotal.Columns.Add(dataColumnF24);
				_dtTotal.Columns.Add(dataColumnF25);
				_dtTotal.Columns.Add(dataColumnF26);
				_dtTotal.Columns.Add(dataColumnF27);
				_dtTotal.Columns.Add(dataColumnF28);
				_dtTotal.Columns.Add(dataColumnF29);
				_dtTotal.Columns.Add(dataColumnF30);
				_dtTotal.Columns.Add(dataColumnF31);
				_dtTotal.Columns.Add(dataColumnF32);
				_dtTotal.Columns.Add(dataColumnF33);
				_dtTotal.Columns.Add(dataColumnF34);
				_dtTotal.Columns.Add(dataColumnF35);
				_dtTotal.Columns.Add(dataColumnF36);
				_dtTotal.Columns.Add(dataColumnF37);
				_dtTotal.Columns.Add(dataColumnF38);
				_dtTotal.Columns.Add(dataColumnF39);
				_dtTotal.Columns.Add(dataColumnF40);

				DataRow row = _dtTotal.NewRow();
				row["Loaction"] = "KHO TRONG";
				_dtTotal.Rows.Add(row);

				DataRow row1 = _dtTotal.NewRow();
				row1["Loaction"] = "KHO NGOÀI";
				_dtTotal.Rows.Add(row1);

				DataRow row2 = _dtTotal.NewRow();
				row2["Loaction"] = "KHO MOTOR";
				_dtTotal.Rows.Add(row2);

				DataRow row3 = _dtTotal.NewRow();
				row3["Loaction"] = "KHO CASE";
				_dtTotal.Rows.Add(row3);

				grdData.DataSource = _dtTotal;
			}
			catch
			{

			}
		}

		/// <summary>
		/// Đổi màu cho từng cột 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grvData_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			try
			{
				if (e.Column == colLocation)
				{
					e.Appearance.BackColor = Color.Blue;
					e.Appearance.ForeColor = Color.White;
				}
				else
				{
					// nếu có chữ hiển thị màu xanh  không có màu tối
					if (TextUtils.ToString(grvData.GetRowCellValue(e.RowHandle, e.Column)) != "")
					{
						e.Appearance.BackColor = Color.Lime;
					}
					else
						e.Appearance.BackColor = Color.Silver;
				}
				if ((e.Column.Caption.ToUpper().Contains("CD") && e.RowHandle == 2 && TextUtils.ToString(grvData.GetRowCellValue(e.RowHandle, e.Column)) == "")
					|| (e.Column.Caption.ToUpper().Contains("CD") && e.RowHandle == 3 && TextUtils.ToString(grvData.GetRowCellValue(e.RowHandle, e.Column)) == ""))
				{
					e.Appearance.BackColor = Color.FromArgb(255, 192, 128);
				}
				for (int i = 1; i <= 41; i++)
				{
					string col = "F" + i;
					GridView View = sender as GridView;

					if (lstNameColCasseKSD != null && lstNameColCasseKSD.Count > 0)
					{
						if (lstNameColCasseKSD.Contains(e.Column.FieldName) && e.RowHandle == 3)
						{
							string category = View.GetRowCellDisplayText(3, View.Columns[e.Column.FieldName]);
							if (e.RowHandle == 3)
								if (category == "") e.Appearance.BackColor = Color.FromArgb(255, 192, 128);
						}
					}
					if (lstNameColMotorKSD != null && lstNameColMotorKSD.Count > 0)
					{
						if (lstNameColMotorKSD.Contains(e.Column.FieldName))
						{
							string category = View.GetRowCellDisplayText(e.RowHandle, View.Columns[e.Column.FieldName]);
							if (e.RowHandle == 2)
								if (category == "") e.Appearance.BackColor = Color.FromArgb(255, 192, 128);
						}
					}

					if (lstColorColKT2.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 0)
					{
						e.Appearance.BackColor = Color.DarkTurquoise;
					}
					if (lstColorColKT1.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 0)
						e.Appearance.BackColor = Color.Cyan;

					if (lstColorColKN2.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 1)
					{
						e.Appearance.BackColor = Color.DarkTurquoise;
					}
					if (lstColorColKN1.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 1)
						e.Appearance.BackColor = Color.Cyan;

					if (lstCheckOrderCasseAwait != null && lstCheckOrderCasseAwait.Count > 0)
					{
						if (lstCheckOrderCasseAwait.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 3)
						{
							string Check = TextUtils.ToString(grvData.GetRowCellValue(3, e.Column));
							if (Check == "")
							{
								var ketqua = lstCheckOrderCasseAwait.Where(x => x.ColumnF == e.Column.FieldName).Select(s => s.Order).ToList();
								//		ketqua[0];	
								grvData.SetRowCellValue(3, e.Column, ketqua[0]);
							}
							e.Appearance.BackColor = Color.Cyan;
						}
					}
					if (lstCheckOrderMotorAwait != null && lstCheckOrderMotorAwait.Count > 0)
					{
						if (lstCheckOrderMotorAwait.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0 && e.RowHandle == 2)
						{
							string Check = TextUtils.ToString(grvData.GetRowCellValue(2, e.Column));
							if (Check == "")
							{
								var ketqua = lstCheckOrderMotorAwait.Where(x => x.ColumnF == e.Column.FieldName).Select(s => s.Order).ToList();
								grvData.SetRowCellValue(2, e.Column, ketqua[0]);
							}
							e.Appearance.BackColor = Color.Cyan;
						}
					}

					if (e.Column.FieldName == col && e.RowHandle == 3)
					{
						if (lstCheckCasseOK.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0)
						{
							e.Appearance.BackColor = Color.Lime;
						}
						else
							e.Appearance.BackColor = Color.Silver;
					}
					if (e.Column.FieldName == col && e.RowHandle == 2)
					{
						if (lstCheckMotorOK.Where(x => x.ColumnF == e.Column.FieldName).Count() > 0)
						{
							e.Appearance.BackColor = Color.Lime;
						}
						else
							e.Appearance.BackColor = Color.Silver;
					}
				}
			}
			catch
			{

			}
		}

		/// <summary>
		/// Cài time
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void configShiftToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmSettingPickingTime frm = new frmSettingPickingTime();
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//loadTime();
			}
		}


		private void configShiftTool_Click(object sender, EventArgs e)
		{
			frmShifts frm = new frmShifts();

			if (frm.ShowDialog() == DialogResult.OK)
			{

			}

		}
		private void configToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmAndonConfigVer4 frmAndonConfig = new frmAndonConfigVer4();

			if (frmAndonConfig.ShowDialog() == DialogResult.OK)
			{

			}
			//frmAndonConfig._UpdateAndon = new UpdateAndon(editAndon);
		}

		/// <summary>
		/// set font chữ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void anDonConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmConfig frm = new frmConfig();
			frm._FontSize = new FontSize(fontSizefn);
			if (frm.ShowDialog() == DialogResult.OK)
			{
				threadAllK();
			}
		}
		// PLC
		public void OpenPort(int port)
		{
			if (_FxSerial == null)
			{
				_FxSerial = new FxSerialDeamon();
				_FxSerial.Start(port);
			}
		}
		public void ClosePort()
		{
			if (_FxSerial != null)
			{
				_FxSerial.Dispose();
			}
			_FxSerial = null;
		}
		private void writePLC()
		{
			while (true)
			{
				Thread.Sleep(1000);
				try
				{
					if (_isBreakTime || DateTime.Now < _OAndonModel.ShiftStartTime || DateTime.Now > _OAndonModel.ShiftEndTime)
					{
						_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
										new FxAddress(_andonConfig.AreaDelayPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
						_res = _FxSerial.Send(0, _cmd);


						_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
										new FxAddress(_andonConfig.AreaRiskPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
						_res = _FxSerial.Send(0, _cmd);


						if (_res.ResultCode == ResultCodeConst.rcFailt || _res.ResultCode == ResultCodeConst.rcNotSettting ||
							_res.ResultCode == ResultCodeConst.rcTimeout)
						{
							ClosePort();

							OpenPort(TextUtils.ToInt(_andonConfig.ComPLC));
						}
						continue;
					}

					// M1 là delay
					// bật đèn delay
					if ((_countTakt <= 0 || _countTakt1 <= 0 || _countTakt2 <= 0 || _countTakt3 <= 0 || _countTakt4 <= 0 || _countTakt5 <= 0) && _OAndonModel.IsStart == true)
					{
						_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOn,
										new FxAddress(_andonConfig.AreaDelayPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
						_res = _FxSerial.Send(0, _cmd);
					}
					////  bật đèn sự cố
					//if ((_OStatusColorCDModel.CD1 == 3 || _OStatusColorCDModel.CD2 == 3 || _OStatusColorCDModel.CD3 == 3
					//	|| _OStatusColorCDModel.CD4 == 3 || _OStatusColorCDModel.CD5 == 3 || _OStatusColorCDModel.CD6 == 3 || _OStatusColorCDModel.CD7 == 3
					//	|| _OStatusColorCDModel.CD8 == 3 || _OStatusColorCDModel.CD9 == 3 || _OStatusColorCDModel.CD10 == 3 || _OStatusColorCDModel.CD11 == 3 || _OStatusColorCDModel.CD12 == 3
					//	|| _OStatusColorCDModel.CD13 == 3 || _OStatusColorCDModel.CD14 == 3) && _OAndonModel.IsStart == true)
					//{
					//	_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOn,
					//					new FxAddress(_andonConfig.AreaRiskPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
					//	_res = _FxSerial.Send(0, _cmd);
					//}
					//tắt đèn delay
					if (_countTakt > 0 || _countTakt1 > 0 || _countTakt2 > 0 || _countTakt3 > 0 || _countTakt4 > 0 || _countTakt5 > 0)
					{
						_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
										new FxAddress(_andonConfig.AreaDelayPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
						_res = _FxSerial.Send(0, _cmd);
					}
					// Tắt đèn sự cố
					//if (_OStatusColorCDModel.CD1 != 3 && _OStatusColorCDModel.CD2 != 3 && _OStatusColorCDModel.CD3 != 3
					//	&& _OStatusColorCDModel.CD4 != 3 && _OStatusColorCDModel.CD5 != 3 && _OStatusColorCDModel.CD6 != 3 && _OStatusColorCDModel.CD7 != 3
					//	&& _OStatusColorCDModel.CD8 != 3 && _OStatusColorCDModel.CD9 != 3 && _OStatusColorCDModel.CD10 != 3 && _OStatusColorCDModel.CD11 != 3 && _OStatusColorCDModel.CD12 != 3
					//	&& _OStatusColorCDModel.CD13 != 3 && _OStatusColorCDModel.CD14 != 3)
					//{
					//	_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
					//					new FxAddress(_andonConfig.AreaRiskPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
					//	_res = _FxSerial.Send(0, _cmd);
					//}

					if (_res.ResultCode == ResultCodeConst.rcFailt || _res.ResultCode == ResultCodeConst.rcNotSettting ||
							_res.ResultCode == ResultCodeConst.rcTimeout)
					{
						ClosePort();

						OpenPort(TextUtils.ToInt(_andonConfig.ComPLC));
					}
				}
				catch (Exception ex)
				{
					File.AppendAllText(Application.StartupPath + "/Error_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt",
						DateTime.Now.ToString("HH:mm:ss") + ":writePLC(): " + ex.ToString() + Environment.NewLine);
				}
			}
		}
		private void frmAnDonPicking_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				if (_threadResetTakt != null) _threadResetTakt.Abort();
				if (_threadLoadAndon != null) _threadLoadAndon.Abort();
				if (_threadUpdateCurrent != null) _threadUpdateCurrent.Abort();
				if (_threadLoadAllK != null) _threadLoadAllK.Abort();

				OpenPort(TextUtils.ToInt(_andonConfig.ComPLC));
				// Tắt đèn delay
				_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
								new FxAddress(_andonConfig.AreaDelayPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
				_res = _FxSerial.Send(0, _cmd);
				// tắt đèn sự cố
				_cmd = FxCommandHelper.Make(FxCommandConst.FxCmdForceOff,
								new FxAddress(_andonConfig.AreaRiskPLC, FxAddressLayoutType.AddressLayoutByte));// tắt
				_res = _FxSerial.Send(0, _cmd);

				if (_res.ResultCode == ResultCodeConst.rcFailt || _res.ResultCode == ResultCodeConst.rcNotSettting ||
						_res.ResultCode == ResultCodeConst.rcTimeout)
				{
					ClosePort();

					OpenPort(TextUtils.ToInt(_andonConfig.ComPLC));
				}
			}
			catch
			{

			}
		}
		private Rectangle Transform(Graphics g, int degree, Rectangle r)
		{
			try
			{
				g.TranslateTransform(r.Width, r.Height);

				g.RotateTransform(degree);
				float cos = (float)Math.Round(Math.Cos(degree * (Math.PI / 180)), 2);
				float sin = (float)Math.Round(Math.Sin(degree * (Math.PI / 180)), 2);
				Rectangle r1 = r;
				r1.X = (int)(r.X * cos + r.Y * sin);
				r1.Y = (int)(r.X * (-sin) + r.Y * cos);
				return r1;
			}
			catch
			{
				Rectangle r1 = r;
				return r1;
			}

		}
		private void grvData_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			try
			{
				if (e.Column == colLocation)
				{
					return;
				}

				System.Drawing.Drawing2D.GraphicsState state = e.Graphics.Save();
				Rectangle rect = e.Bounds;
				StringFormat format = e.Appearance.GetTextOptions().GetStringFormat();
				format.FormatFlags |= StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
				rect = Transform(e.Graphics, 180, rect);
				//e.Appearance.FillRectangle(e.Cache, rect);
				e.Appearance.DrawBackground(e.Cache, rect);
				//e.Appearance.DrawVString(e.Cache, e.CellValue.ToString(), e.Appearance.GetFont(), e.Appearance.GetForeBrush(e.Cache), rect, new StringFormat(), 90);

				e.Graphics.DrawString(e.DisplayText, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), rect, format);
				e.Handled = true;
				e.Graphics.Restore(state);
			}
			catch
			{

			}
		}

		private void btnRemainTimeStockMotor_Click(object sender, EventArgs e)
		{

		}
	}
}
