
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class AndonPickingConfigFacade : BaseFacade
	{
		protected static AndonPickingConfigFacade instance = new AndonPickingConfigFacade(new AndonPickingConfigModel());
		protected AndonPickingConfigFacade(AndonPickingConfigModel model) : base(model)
		{
		}
		public static AndonPickingConfigFacade Instance
		{
			get { return instance; }
		}
		protected AndonPickingConfigFacade():base() 
		{ 
		} 
	
	}
}
	