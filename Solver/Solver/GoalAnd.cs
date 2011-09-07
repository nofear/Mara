//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/GoalAnd.cs $
 * 
 * 32    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 31    25-07-07 3:37 Patrick
 * cleanup log
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
 * 
 * 26    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 25    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 24    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 23    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 22    21-02-07 0:38 Patrick
 * fixed ToString()
 * 
 * 21    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Search goal used to combine a number of other goals.
	/// This goal succeeds if all sub goals have succeeded.
	/// </summary>
	public sealed class GoalAnd : Goal
	{
		public GoalAnd( Solver solver, Goal[] goalList ) :
			base( solver )
		{
			m_GoalList		= goalList;
			m_Index			= new RevValue<int>( solver.StateStack, 0 );
		}

		public GoalAnd( Goal g0 ) :
			this( g0.Solver, new Goal[] { g0 } )
		{	
		}

		public GoalAnd( Goal g0, Goal g1 ) :
			this( g0.Solver, new Goal[] { g0, g1 } )
		{
		}

		public GoalAnd( Goal g0, Goal g1, Goal g2 ) :
			this( g0.Solver, new Goal[] { g0, g1, g2 } )
		{
		}

		public override string ToString()
		{
			StringBuilder str	= new StringBuilder();
			
			str.Append( "And( #" );
			str.Append( m_Index.ToString() );
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

		public override void Execute()
		{
			if( m_Index.Value >= m_GoalList.Length )
				throw new IndexOutOfRangeException();

			int index	= m_Index.Value++;

			if( m_Index.Value < m_GoalList.Length )
			{
				Add( this );
			}
			
			Add( m_GoalList[ index ] );
		}

		Goal[]			m_GoalList;
		RevValue<int>	m_Index;
	}
}
