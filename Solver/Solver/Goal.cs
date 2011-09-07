//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/Goal.cs $
 * 
 * 26    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 25    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 24    25-07-07 3:37 Patrick
 * cleanup log
 * 
 * 23    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 22    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 21    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 20    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 19    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 18    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 17    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 16    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 15    7-03-07 18:26 Patrick
 * moved propagation queue from Solver to Problem
 * 
 * 14    23-02-07 2:03 Patrick
 * added And(..) / Or(..)
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Summary description for Goal.
	/// </summary>
	public abstract class Goal : SolverBase
	{
		protected Goal( Solver solver ) :
			base( solver )
		{
		}

		public abstract void Execute();

		public void Fail()
		{
			m_Solver.GoalStack.Fail();
		}

		public void Add( Goal goal )
		{
			m_Solver.GoalStack.Add( goal );
		}

		public Goal And( Goal g0 )
		{
			return new GoalAnd( this, g0 );		
		}

		public Goal Or( Goal g0 )
		{
			return new GoalOr( this, g0 );		
		}

	}
}

//--------------------------------------------------------------------------------
