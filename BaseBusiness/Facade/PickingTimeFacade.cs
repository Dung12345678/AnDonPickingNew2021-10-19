
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class PickingTimeFacade : BaseFacade
	{
		protected static PickingTimeFacade instance = new PickingTimeFacade(new PickingTimeModel());
		protected PickingTimeFacade(PickingTimeModel model) : base(model)
		{
		}
		public static PickingTimeFacade Instance
		{
			get { return instance; }
		}
		protected PickingTimeFacade():base() 
		{ 
		} 
	
	}
}
	