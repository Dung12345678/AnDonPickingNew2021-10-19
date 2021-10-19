
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class AndonPickingConfigBO : BaseBO
	{
		private AndonPickingConfigFacade facade = AndonPickingConfigFacade.Instance;
		protected static AndonPickingConfigBO instance = new AndonPickingConfigBO();

		protected AndonPickingConfigBO()
		{
			this.baseFacade = facade;
		}

		public static AndonPickingConfigBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	