//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltSearch/FltGenerate.cs $
 * 
 * 12    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 11    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 10    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 9     28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 8     27-06-07 22:16 Patrick
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
 * 4     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 3     27-02-07 0:30 Patrick
 * use array instead of FltVarList
 * 
 * 2     8-02-07 0:18 Patrick
 * refactored a bit
 * 
 * 1     17-03-06 20:00 Patrick
 * added float version of Generate
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float.Search
{
	/// <summary>
	/// Summary description for FltGenerate.
	/// </summary>
	public class FltGenerate : Goal
	{
		public FltGenerate( Solver solver, FltVar[] list, FltVarSelector.Select select, FltSearch search ) :
			base( solver )
		{
			m_FltVarList		= list;
			m_SelectVar			= select;
			m_Search			= search;
		}
 
		public FltGenerate( Solver solver, FltVar[] list ) :
			base( solver )
		{
			m_FltVarList		= list;
			m_SelectVar			= FltVarSelector.CardinalityMin;
			m_Search			= new FltSearchDichotomize();
		}
 	
		public override string ToString()
		{
			return "FltGenerate()";
		}
 
		public override void Execute()
		{
			FltVar var	    = m_SelectVar( m_FltVarList );
			
			if( !ReferenceEquals( var, null ) )
			{
				Add( new GoalAnd( m_Search.Create( var ), this ) );			    
			}
		}
 
		FltVar[]					m_FltVarList;
		FltVarSelector.Select		m_SelectVar;
		FltSearch					m_Search;
	}
}

//--------------------------------------------------------------------------------
 
