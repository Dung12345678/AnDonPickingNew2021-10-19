
using System;
using System.Collections;
using BMS.Facade;
using BMS.Model;
namespace BMS.Business
{

	
	public class RunMotorBO : BaseBO
	{
		private RunMotorFacade facade = RunMotorFacade.Instance;
		protected static RunMotorBO instance = new RunMotorBO();

		protected RunMotorBO()
		{
			this.baseFacade = facade;
		}

		public static RunMotorBO Instance
		{
			get { return instance; }
		}
		
	
	}
}
	