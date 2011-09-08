using System;
using System.Collections.Generic;
using System.Text;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer;
using MaraSolver.Integer.Search;

namespace TestApp
{
	public class AppMoreMoney  : App
	{
		static public void MoreMoney()
		{		
			MoreMoney mm	= new MoreMoney();
			Solver solver	= mm.Solver;

			solver.PropagationQueue.Propagate();

			solver.PrintVariables();
			
			solver.Solve( new IntGenerate( solver,
									solver.IntVarList.ToArray(),
									IntVarSelector.CardinalityMin,
									new IntSearchInstantiateBest() ) );

			solver.PrintInformation();
			solver.PrintVariables();
			solver.PrintConstraints();

			SearchAll( solver );

			solver.PrintConstraints();
			solver.PrintInformation();
		}
	}
}
