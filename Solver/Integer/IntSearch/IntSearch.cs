//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntSearch.cs $
 * 
 * 15    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 14    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 13    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 12    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 11    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 10    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 9     27-04-07 0:32 Patrick
 * added comment setup
 * 
 * 8     27-04-07 0:26 Patrick
 * renamed classes
 * 
 * 7     27-04-07 0:24 Patrick
 * refactored IntSearch
 * 
 * 6     9-03-07 23:06 Patrick
 * updated copyright notice
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	public abstract class IntSearch
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="solver"></param>
		protected IntSearch() :
			base()
		{
		}

		public abstract IntSearchGoal Create( IntVar var );

		/// <summary>
		/// 
		/// </summary>
		public abstract class IntSearchGoal : Goal
		{
			protected IntSearchGoal( IntVar var ) :
				base( var.Solver )
			{
				m_IntVar	= var;
			}

			public abstract Goal Create();

			public override void Execute()
			{
				if( !m_IntVar.IsBound() )
				{
					Add( Create() );
				}
			}
			
			protected IntVar m_IntVar;
		};
	};
}

//--------------------------------------------------------------------------------
