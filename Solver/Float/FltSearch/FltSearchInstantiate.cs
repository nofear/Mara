//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltSearch/FltSearchInstantiate.cs $
 * 
 * 13    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 12    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 11    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 10    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 9     28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 8     27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 7     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 6     11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 5     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 4     7-03-07 18:52 Patrick
 * removed problem -> Solver reference
 * 
 * 3     7-03-07 18:26 Patrick
 * moved propagation queue from Solver to Problem
 * 
 * 2     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 1     17-03-06 20:00 Patrick
 * added float version of Generate
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float.Search
{
	public class FltSearchInstantiate : FltSearch
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solver"></param>
		public FltSearchInstantiate() :
			this( FltVarValueSelector.Min )
		{
		}

		public FltSearchInstantiate( FltVarValueSelector.Select select ) :
			base()
		{
			m_SelectValue	= select;
		}

		public override FltSearchGoal Create( FltVar var )
		{
			return new FltSearchGoalInstantiate( var.Solver, var, m_SelectValue );
		}

		FltVarValueSelector.Select	m_SelectValue;


		/// <summary>
		/// 
		/// </summary>
		private class FltSearchGoalInstantiate : FltSearchGoal
		{
			public FltSearchGoalInstantiate( Solver solver, FltVar var, FltVarValueSelector.Select select ) :
				base( solver, var )
			{
				m_SelectValue	= select;
			}

			public override string ToString()
			{
				return "FltSearchInstantiate(" + m_FltVar.ToString() + ")";
			}

			public override Goal Create()
			{
				double val		= m_SelectValue( m_FltVar );
				
				return new GoalOr( m_FltVar == val, new GoalAnd( m_FltVar != val, this ) );							
			}

			FltVarValueSelector.Select	m_SelectValue;
		}
	};
}
//--------------------------------------------------------------------------------
