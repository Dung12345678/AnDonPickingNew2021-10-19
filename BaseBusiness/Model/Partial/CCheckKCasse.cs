using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMS.Model
{
	public partial class CCheckKCasse : BaseModel
	{
		private string columnF;
		private string valueCasse;
		private string order;
		public string ColumnF
		{
			get { return columnF; }
			set { columnF = value; }
		}

		public string ValueCasse
		{
			get { return valueCasse; }
			set { valueCasse = value; }
		}
		public string Order
		{
			get { return order; }
			set { order = value; }
		}
	}
}
