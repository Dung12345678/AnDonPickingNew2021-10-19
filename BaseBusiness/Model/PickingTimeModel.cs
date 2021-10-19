
using System;
namespace BMS.Model
{
	public partial class PickingTimeModel : BaseModel
	{
		public int ID {get; set;}
		
		public string Location {get; set;}
		
		public DateTime? TimePicking {get; set;}
		
	}
}
	