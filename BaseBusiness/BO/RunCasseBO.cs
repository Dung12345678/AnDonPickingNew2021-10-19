
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class RunCasseBO : BaseBO
	{
		private RunCasseFacade facade = RunCasseFacade.Instance;
		protected static RunCasseBO instance = new RunCasseBO();

		protected RunCasseBO()
		{
			this.baseFacade = facade;
		}

		public static RunCasseBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	