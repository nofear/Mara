//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntSearchDichotomize.cs $
 * 
 * 34    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 33    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 32    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 31    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 30    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 29    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 28    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 27    20-06-07 22:46 Patrick
 * renamed namespace
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	public class IntSearchDichotomize : IntSearch
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solver"></param>
		public IntSearchDichotomize():
			base()
		{
		}

		public override IntSearchGoal Create( IntVar var )
		{
			return new IntSearchGoalDichotomize( var );
		}

		/// <summary>
		/// 
		/// </summary>
		private class IntSearchGoalDichotomize : IntSearchGoal
		{
			public IntSearchGoalDichotomize( IntVar var ) :
				base( var )
			{
			}

			public override string ToString()
			{
				return "IntSearchDichotomize(" + m_IntVar.ToString() + ")";
			}

			public override Goal Create()
			{
				int val		= IntVarValueSelector.Mid( m_IntVar );

				return new GoalOr( m_IntVar <= val, m_IntVar > val );
			}
		}
	};
}

//--------------------------------------------------------------------------------
