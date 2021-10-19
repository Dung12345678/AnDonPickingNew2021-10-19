
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class RunCasseFacade : BaseFacade
	{
		protected static RunCasseFacade instance = new RunCasseFacade(new RunCasseModel());
		protected RunCasseFacade(RunCasseModel model) : base(model)
		{
		}
		public static RunCasseFacade Instance
		{
			get { return instance; }
		}
		protected RunCasseFacade():base() 
		{ 
		} 
	
	}
}
	