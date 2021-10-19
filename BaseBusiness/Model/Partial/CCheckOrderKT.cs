using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BMS.BO;

namespace BMS.Model
{
	public partial class CCheckOrderKT : BaseModel
	{
		private string columnF;
		private string valueKT;
		public string ColumnF
		{
			get { return columnF; }
			set { columnF = value; }
		}

		public string ValueKT
		{
			get { return valueKT; }
			set { valueKT = value; }
		}
	}

}

