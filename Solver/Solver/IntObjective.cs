//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/IntObjective.cs $
 * 
 * 21    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 20    25-10-07 21:55 Patrick
 * added Value property
 * 
 * 19    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 18    29-07-07 22:12 Patrick
 * removed obsolete Copy()
 * 
 * 17    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 16    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 15    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 14    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 13    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 12    27-04-07 0:24 Patrick
 * refactored IntSearch
 * 
 * 11    26-03-07 15:09 Patrick
 * cleanup IntObjective()
 * 
 * 10    13-03-07 1:25 Patrick
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.Integer;
using MaraSolver.Integer.Search;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public sealed class IntObjective : SolverBase
	{
		public IntObjective( Solver solver ) :
			base( solver )
		{
			m_Variable	= null;
			m_Value		= int.MaxValue;
			m_Step		= 1;
		}

		public override string ToString()
		{
			if( ReferenceEquals( m_Variable, null ) )
				return "";

			return m_Variable.ToString( true ) + "<=" + m_Value.ToString( CultureInfo.CurrentCulture );
		}

		public int Value
		{
			get
			{
				return m_Value;
			}
			
			set
			{
				m_Value		= Math.Min( m_Value, value );
			}
		}

		
		public int Step
		{
			get
			{
				return m_Step;
			}
			
			set
			{
				m_Step		= value;
			}
		}

		public IntVar Var
		{
			get
			{
				return m_Variable;
			}
			
			set
			{
				m_Variable		= value;
			}
		}
		
		/// <summary>
		/// Called in GoalStack, after the first Solve().
		/// </summary>
		public void Init()
		{
			if( !ReferenceEquals( m_Variable, null ) )
			{
				m_Value		= m_Variable.Max;
			}
		}
		
		/// <summary>
		/// Called in GoalStack, before the next solution search is started.
		/// </summary>
		public void Next()
		{
			if( !ReferenceEquals( m_Variable, null ) )
			{
				m_Value		= m_Variable.Max - m_Step;
			}
		}

		public Goal Create()
		{
			return m_Variable <= m_Value;
		}
		
		IntVar			m_Variable;
		volatile int	m_Value;
		int				m_Step;
	}
}

//--------------------------------------------------------------------------------
