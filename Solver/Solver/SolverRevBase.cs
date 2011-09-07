using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver.Reversible;

namespace MaraSolver
{
	public abstract class SolverRevBase : SolverBase, IState
	{
		protected SolverRevBase( Solver solver ) :
			base( solver )
		{
		}

		protected SolverRevBase()
		{
		}



	}
}
