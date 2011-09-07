//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltSearch/FltSearch.cs $
 * 
 * 10    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 9     11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 8     5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 7     27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 6     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 5     11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 4     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 3     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 2     8-02-07 0:18 Patrick
 * refactored a bit
 * 
 * 1     17-03-06 20:00 Patrick
 * added float version of Generate
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float.Search
{
	public abstract class FltSearch
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solver"></param>
		protected FltSearch()
		{
		}

		public abstract FltSearchGoal Create( FltVar var );

		/// <summary>
		/// 
		/// </summary>
		public abstract class FltSearchGoal : Goal
		{
			protected FltSearchGoal( Solver solver, FltVar var ) :
				base( solver )
			{
				m_FltVar	= var;
			}

			public abstract Goal Create();

			public override void Execute()
			{
				if( !m_FltVar.IsBound() )
				{
					Add( Create() );
				}
			}
			
			protected FltVar m_FltVar;
		};
	};
}

//--------------------------------------------------------------------------------
