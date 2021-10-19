
using System.Collections;
using BMS.Model;
namespace BMS.Facade
{
	
	public class RunMotorFacade : BaseFacade
	{
		protected static RunMotorFacade instance = new RunMotorFacade(new RunMotorModel());
		protected RunMotorFacade(RunMotorModel model) : base(model)
		{
		}
		public static RunMotorFacade Instance
		{
			get { return instance; }
		}
		protected RunMotorFacade():base() 
		{ 
		} 
	
	}
}
	