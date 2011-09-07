//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/Problem.cs $
 * 
 * 18    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 17    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 16    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 15    12-04-07 23:07 Patrick
 * removed Propagate()
 * 
 * 14    31-03-07 12:55 Patrick
 * implemented support to retrieve previous domain state
 * 
 * 13    26-03-07 14:31 Patrick
 * no need to manually call Problem.Post()
 * 
 * 12    22-03-07 21:09 Patrick
 * refactored Clone()
 * 
 * 11    21-03-07 23:41 Patrick
 * moved init to Clone(..)
 * 
 * 10    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 9     20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 8     15-03-07 1:20 Patrick
 * use constraint propagationqueue
 * 
 * 7     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 6     8-03-07 21:56 Patrick
 * renamed class
 * 
 * 5     8-03-07 21:38 Patrick
 * using IPropagationQueue interface
 * 
 * 4     7-03-07 22:47 Patrick
 * use IStateStack interface
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MaraSolver.Reversible;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;
using MaraSolver.Float;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public class Problem
	{
		public Problem() :
			this( IntInterval.Whole )
		{
		}
	
		public Problem( int min, int max ) :
			this( new IntInterval( min, max ) )
		{
		}

		public Problem( IntInterval horizon )
		{
			m_Solver	= new Solver( horizon );
		}

		public Solver Solver
		{
			get
			{
				return m_Solver;
			}
		}

		protected Solver m_Solver;

	}
}

//--------------------------------------------------------------------------------
