//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntGenerateImpact.cs $
 * 
 * 22    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 21    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 20    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 19    7-08-07 15:31 Patrick
 * 
 * 18    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 17    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 16    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 15    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 14    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 13    20-06-07 22:46 Patrick
 * renamed namespace
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

using MaraSolver;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	public class IntGenerateImpact : Goal
	{
		public IntGenerateImpact( Solver solver, IntVar[] list, IntSearch search, IntVarSelector.Select select ) :
			base( solver )
		{
			m_IntVarList		= list;
			m_Search			= search;
			m_SelectVar			= select;
		}

		public IntGenerateImpact( Solver solver, IntVar[] list ) :
			base( solver )
		{
			m_IntVarList		= list;
			m_Search			= new IntSearchDichotomize();
			m_SelectVar			= IntVarSelector.CardinalityMin;
		}
 
		public override string ToString()
		{
			return "IntGenerateImpact()";
		}
 
		public static int SumCardinality( IntVar[] list )
		{
			int cardinality		= 0;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				cardinality		+= list[ idx ].Domain.Cardinality;
			}
			
			return cardinality;
		}
  
		public override void Execute()
		{
			List<Impact> impactList	= new List<Impact>();

			foreach( IntVar var in m_IntVarList )
			{
				if( !var.IsBound() )
				{
					IntSearch.IntSearchGoal search	= m_Search.Create( var );
					GoalOr goalOr					= search.Create() as GoalOr;
					Goal[] goalList					= goalOr.GoalList;

					int start		= SumCardinality( m_IntVarList );

					int impact		= 0;
					foreach( ConstraintVar constraint in goalList )
					{
						var.Solver.StateStack.Begin();

						Solver.PropagationQueue.IsViolated	= false;

						constraint.Update();

						impact	+= start - SumCardinality( m_IntVarList );
						
						var.Solver.StateStack.Cancel();
					}
										
					impactList.Add( new Impact( var, impact ) );
				}

				Solver.PropagationQueue.IsViolated	= false;
			}

			if( impactList.Count > 0 )
			{
				impactList.Sort( new Impact.Comparer() );

				IntVar[] intVarList	= new IntVar[ impactList.Count ];
				for( int idx = 0; idx < impactList.Count; ++idx )
				{
					intVarList[ idx ]	= impactList[ idx ].m_Var ;
				}

				Add( new IntGenerate( Solver, intVarList, m_SelectVar, m_Search ) );			    
//				Add( new GoalAnd( m_SearchNew( intVarList[ 0 ], m_SelectVarValue ), this ) );		    
			}			
		}
 
		[DebuggerDisplay("{m_Var} {m_Impact} {m_Cardinality}")]
		sealed class Impact
		{
			public Impact( IntVar var, int impact )
			{
				m_Var			= var;
				m_Cardinality	= var.Domain.Cardinality;
				m_Impact		= impact;
			}
			
			public sealed class Comparer : IComparer<Impact>
			{
				public int Compare( Impact x, Impact y ) 
				{
					int r	= ( x.m_Impact < y.m_Impact ) ? 1
							: ( x.m_Impact > y.m_Impact ) ? -1
							: ( x.m_Cardinality < y.m_Cardinality ) ? -1
							: ( x.m_Cardinality > y.m_Cardinality ) ? 1
							: 0;
							
					return r;
				}
			}
			
			public IntVar	m_Var;
			public int		m_Cardinality;
			public int		m_Impact;
		}

		IntVar[]					m_IntVarList;
		IntSearch			m_Search;
		IntVarSelector.Select		m_SelectVar;
	}
}
