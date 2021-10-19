
using System;
namespace BMS.Model
{
	public partial class RunMotorModel : BaseModel
	{
		public int ID {get; set;}
		
		public string OrderCode {get; set;}
		
		public string PID {get; set;}
		
		public DateTime? CreatDate {get; set;}
		
	}
}
	