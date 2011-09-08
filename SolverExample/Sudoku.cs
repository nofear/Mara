//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/SolverExample/Sudoku.cs $
 * 
 * 7     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 6     8-08-07 21:56 Patrick
 * moved scope of PropagationLevel
 * 
 * 5     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 4     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 3     12-04-07 22:52 Patrick
 * subclassing from Problem
 * 
 * 2     12-04-07 1:04 Patrick
 * 
 * 1     12-04-07 1:01 Patrick
 * 
 * 18    22-03-07 23:43 Patrick
 * cleanup matrix code
 * 
 * 17    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 16    8-03-07 0:39 Patrick
 * enable highpropagation level
 * 
 * 15    7-03-07 18:24 Patrick
 * added constructors
 * 
 * 14    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 13    27-02-07 1:01 Patrick
 * use matrix constructor
 * 
 * 12    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 11    11-06-06 22:39 Patrick
 * added Set( string )
 * 
 * 10    11-06-06 21:32 Patrick
 * added Set(..)
 * 
 * 9     5-06-06 21:48 Patrick
 * limit domain to 1..9
 * 
 * 8     14-03-06 22:28 Patrick
 * 
 * 7     22-02-06 22:38 Patrick
 * renamed FSolver to Solver
 * 
 * 6     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 5     8-01-06 22:07 Patrick
 * 
 * 4     8-01-06 18:13 Patrick
 * refactored problems
 * 
 * 3     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 2     5-01-06 22:49 Patrick
 * refactored to Post() & reversible constraint lists
 * 
 * 1     20-10-05 21:27 Patrick
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace SolverExample
{
	public class Sudoku : Problem
	{
		public Sudoku( string initial ) :
			this()
		{
			Set( initial );
		}

		public Sudoku( int[] initial ) :
			this()
		{
			Set( initial );
		}

		public Sudoku() :
			base( 1, 9 )
		{
			int size	= 9;
			
			m_Matrix	= new IntVarMatrix( m_Solver, size, size, new IntInterval( 1, size ) );

			for( int idx = 0; idx < size; ++idx )
			{
				IntVarListAllDifferent constraint	= m_Matrix.Row( idx ).AllDifferent();
				constraint.Level	= PropagateLevel.High;

				m_Solver.Add( constraint );
			}
			
			for( int idx = 0; idx < size; ++idx )
			{
				IntVarListAllDifferent constraint	= m_Matrix.Col( idx ).AllDifferent();
				constraint.Level	= PropagateLevel.High;

				m_Solver.Add( constraint );
			}

			for( int subRow = 0; subRow < 3; ++subRow )
			{
				for( int subCol = 0; subCol < 3; ++subCol )
				{
					IntVarListAllDifferent constraint	= m_Matrix.Matrix( subRow * 3, subCol * 3, 3, 3 ).VarList.AllDifferent();
					constraint.Level	= PropagateLevel.High;

					m_Solver.Add( constraint );
				}
			}
		}
	
		public IntVarMatrix Matrix
		{
			get
			{
				return m_Matrix;
			}
		}

		public void Set( string initial )
		{
			for( int idx = 0; idx < 81; ++idx )
			{
				int value	= initial[ idx ] - '0';
				if( value > 0 )
				{
					m_Matrix.VarList[ idx ].Value = value;
				}
			}
		}

		public void Set( int[] initial )
		{
			for( int idx = 0; idx < 81; ++idx )
			{
				if( initial[ idx ] > 0 )
				{
					m_Matrix.VarList[ idx ].Value = initial[ idx ];
				}
			}
		}

		IntVarMatrix	m_Matrix;
	}
}

//--------------------------------------------------------------------------------
