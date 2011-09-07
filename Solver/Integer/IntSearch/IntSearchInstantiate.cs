//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntSearchInstantiate.cs $
 * 
 * 27    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 26    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 25    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 24    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 23    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 22    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 21    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 20    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 19    27-04-07 0:32 Patrick
 * added comment setup
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	public class IntSearchInstantiate : IntSearch
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solver"></param>
		public IntSearchInstantiate() :
			this( IntVarValueSelector.Min )
		{
		}

		public IntSearchInstantiate( IntVarValueSelector.Select select ) :
			base()
		{
			m_SelectValue	= select;
		}

		public override IntSearchGoal Create( IntVar var )
		{
			return new IntSearchGoalInstantiate( var, m_SelectValue );
		}

		IntVarValueSelector.Select	m_SelectValue;


		/// <summary>
		/// 
		/// </summary>
		private class IntSearchGoalInstantiate : IntSearchGoal
		{
			public IntSearchGoalInstantiate( IntVar var, IntVarValueSelector.Select select ) :
				base( var )
			{
				m_SelectValue	= select;
			}

			public override string ToString()
			{
				return "IntSearchInstantiate(" + m_IntVar.ToString() + ")";
			}

			public override Goal Create()
			{
				int val		= m_SelectValue( m_IntVar );
				
				return new GoalOr( m_IntVar == val, new GoalAnd( m_IntVar != val, this ) );
			}

			IntVarValueSelector.Select	m_SelectValue;
		}
	};
}

//--------------------------------------------------------------------------------
