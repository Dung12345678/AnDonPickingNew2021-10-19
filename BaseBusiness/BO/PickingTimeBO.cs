
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class PickingTimeBO : BaseBO
	{
		private PickingTimeFacade facade = PickingTimeFacade.Instance;
		protected static PickingTimeBO instance = new PickingTimeBO();

		protected PickingTimeBO()
		{
			this.baseFacade = facade;
		}

		public static PickingTimeBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	