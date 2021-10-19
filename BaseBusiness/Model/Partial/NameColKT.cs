using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMS.Model
{
	public partial class NameColKN : BaseModel
	{
			private string columnF;
			private string order;
			public string ColumnF
			{
				get { return columnF; }
				set { columnF = value; }
			}

			public string Order
			{
				get { return order; }
				set { order = value; }
			}
	}
}
