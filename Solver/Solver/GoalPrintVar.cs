using System;
using System.Collections.Generic;
using System.Text;

namespace MaraSolver
{
	public class GoalPrintVar : Goal
	{
		public GoalPrintVar( Solver solver, Variable var ) :
			base( solver )
		{
			m_Var	= var;
		}
		
		public override void Execute()
		{
			m_Solver.Out.WriteLine( m_Var.ToString() );
		}
		
		Variable m_Var;
	}
}
