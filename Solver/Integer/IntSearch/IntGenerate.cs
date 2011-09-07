//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntGenerate.cs $
 * 
 * 37    1/11/09 8:07p Patrick
 * 
 * 36    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 35    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 34    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 33    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 32    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 31    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 30    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 29    27-04-07 0:28 Patrick
 * renamed classes
 * 
 * 28    27-04-07 0:26 Patrick
 * renamed classes
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	/// <summary>
	/// Summary description for IntGenerate.
	/// </summary>
	public class IntGenerate : Goal
	{
		public IntGenerate( Solver solver, IntVar[] list, IntVarSelector.Select select, IntSearch search ) :
			base( solver )
		{
			m_IntVarList		= list;
			m_SelectVar			= select;
			m_Search			= search;
			m_Depth				= new RevValue<double>( solver.StateStack, 1 );
		}
 
		public IntGenerate( Solver solver, IntVar[] list ) :
			base( solver )
		{
			m_IntVarList		= list;
			m_SelectVar			= IntVarSelector.CardinalityMin;
			m_Search			= new IntSearchDichotomize();
			m_Depth				= new RevValue<double>( solver.StateStack, 1 );
		}
 
		public override string ToString()
		{
			return "IntGenerate()";
		}
 
		public override void Execute()
		{
			IntVar var	    = m_SelectVar( m_IntVarList );
			
			if( !ReferenceEquals( var, null ) )
			{
				Add( new GoalAnd( m_Search.Create( var ), this ) );			    
			
				m_Depth.Value	*= var.Domain.Cardinality;
			}
		}
 
		IntVar[]				m_IntVarList;
		IntVarSelector.Select	m_SelectVar;
		IntSearch				m_Search;
		RevValue<double>		m_Depth;
	}
  }

//--------------------------------------------------------------------------------
 
