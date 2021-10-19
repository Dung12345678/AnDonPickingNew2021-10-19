using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMS.Model
{
	public partial class CCheckOrderKN : BaseModel
	{

		private string columnF;
		private string valueKN;
		public string ColumnF
		{
			get { return columnF; }
			set { columnF = value; }
		}

		public string ValueKN
		{
			get { return valueKN; }
			set { valueKN = value; }
		}
	}
}
