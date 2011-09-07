//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/GoalOr.cs $
 * 
 * 32    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 31    31-07-07 22:02 Patrick
 * made members protected
 * 
 * 30    29-07-07 22:30 Patrick
 * changed protection level
 * 
 * 29    25-07-07 3:37 Patrick
 * cleanup log
 * 
 * 28    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 27    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 26    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 25    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 24    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 23    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 22    31-03-07 11:08 Patrick
 * added Index property
 * 
 * 21    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 20    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 19    2-03-07 20:30 Patrick
 * removed dependency from StateStack
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Search goal used to create a search tree.
	/// </summary>
	public class GoalOr : Goal
	{
		public GoalOr( Solver solver, Goal[] goalList ) :
			base( solver )
		{
			m_GoalList		= goalList;
			m_Index			= 0;
		}

		public GoalOr( Goal g0 ) :
			this( g0.Solver, new Goal[] { g0 } )
		{	
		}

		public GoalOr( Goal g0, Goal g1 ) :
			this( g0.Solver, new Goal[] { g0, g1 } )
		{
		}

		public GoalOr( Goal g0, Goal g1, Goal g2 ) :
			this( g0.Solver, new Goal[] { g0, g1, g2 } )
		{
		}

		public override string ToString()
		{
			StringBuilder str	= new StringBuilder();
			
			str.Append( "Or( #" );
			str.Append( m_Index.ToString( CultureInfo.CurrentCulture ) );
			str.Append( ", " );
			
			for( int idx = 0; idx < m_GoalList.Length; ++idx )
			{
				Goal goal	= m_GoalList[ idx ];
				if( idx > 0 )
				{
					str.Append( ", " );
				}
			
				str.Append( goal.ToString() );
			}
			
			str.Append( " )" );
			
			return str.ToString();
		}

		internal Goal[] GoalList
		{
			get
			{
				return m_GoalList;
			}
		}

		public int Index
		{
			get
			{
				return m_Index;
			}
		}

		public bool IsDone()
		{
			return m_Index == m_GoalList.Length;
		}

		public override void Execute()
		{
			Push();

			int index	= m_Index++;
			
			Add( m_GoalList[ index ] );
		}

		internal void Push()
		{
			m_Solver.GoalStack.Push();
			m_Solver.StateStack.Begin();
		}

		internal void Pop()
		{
			m_Solver.StateStack.Cancel();
			m_Solver.GoalStack.Pop();
		}

		protected Goal[]	m_GoalList;
		protected int		m_Index;
	}
}
