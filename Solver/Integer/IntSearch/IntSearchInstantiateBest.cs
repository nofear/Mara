//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntSearchInstantiateBest.cs $
 * 
 * 25    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 24    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 23    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 22    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 21    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 20    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 19    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 18    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 17    27-04-07 0:32 Patrick
 * added comment setup
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	public class IntSearchInstantiateBest : IntSearch
	{
		/// <summary>
		/// </summary>
		/// <param name="solver"></param>
		public IntSearchInstantiateBest() :
			this( IntVarValueSelector.Min )
		{
		}

		public IntSearchInstantiateBest( IntVarValueSelector.Select select ) :
			base()
		{
			m_SelectValue	= select;
		}

		public override IntSearchGoal Create( IntVar var )
		{
			return new IntSearchGoalInstantiateBest( var, m_SelectValue );
		}

		IntVarValueSelector.Select m_SelectValue;

		/// <summary>
		/// 
		/// </summary>
		private class IntSearchGoalInstantiateBest : IntSearchGoal
		{
			public IntSearchGoalInstantiateBest( IntVar var, IntVarValueSelector.Select select ) :
				base( var )
			{
				m_SelectValue		= select;
			}

			public override string ToString()
			{
				return "IntSearchInstantiateBest(" + m_IntVar.ToString() + ")";
			}

			public override Goal Create()
			{
				int val		= m_SelectValue( m_IntVar );

				return new GoalOr( m_IntVar == val, m_IntVar != val );
			}

			IntVarValueSelector.Select	m_SelectValue;
		}
	};
}

//--------------------------------------------------------------------------------
