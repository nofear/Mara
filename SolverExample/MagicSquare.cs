//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/SolverExample/MagicSquare.cs $
 * 
 * 5     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 4     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 3     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 2     12-04-07 22:52 Patrick
 * subclassing from Problem
 * 
 * 1     12-04-07 1:01 Patrick
 * 
 * 26    3/24/07 2:20a Patrick
 * more magic
 * 
 * 25    22-03-07 23:43 Patrick
 * cleanup matrix code
 * 
 * 24    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 23    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 22    26-02-07 18:48 Patrick
 * using new IntVarMatrix constructor
 * 
 * 21    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 20    6/02/06 9:18a Patrick
 * cleanup a bit
 * 
 * 19    14-03-06 22:28 Patrick
 * 
 * 18    22-02-06 22:38 Patrick
 * renamed FSolver to Solver
 * 
 * 17    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 16    1/13/06 11:50p Patrick
 * using []
 * 
 * 15    19-01-06 22:48 Patrick
 * 
 * 14    19-01-06 21:40 Patrick
 * using Var.Domain instead of proxy code
 * 
 * 13    8-01-06 22:05 Patrick
 * 
 * 12    8-01-06 18:13 Patrick
 * refactored problems
 * 
 * 11    8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 10    5-01-06 22:49 Patrick
 * refactored to Post() & reversible constraint lists
 * 
 * 9     20-10-05 21:27 Patrick
 * 
 * 8     6/28/05 11:15p Patrick
 * use sum with parameter => remove duplicate variables & constraints
 * 
 * 7     6/07/05 10:31p Patrick
 * renamed namespace
 * 
 * 6     1-06-05 23:56 Patrick
 * remove symmetry
 * 
 * 5     26-05-05 20:00 Patrick
 * 
 * 4     23-05-05 22:49 Patrick
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver;
using MaraInterval.Interval;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace SolverExample
{
	/// <summary>
	/// Summary description for FMagicSquare.
	/// </summary>
	public class MagicSquare : Problem
	{
		public MagicSquare( int count ) :
			base( 0, 10000 )
		{
			m_Matrix			= new IntVarMatrix( m_Solver, count, count, new IntInterval( 1, count * count ) );
			m_MagicConstant		= ( count * ( count * count + 1 ) ) / 2;

			for( int idx = 0; idx < count; ++idx )
			{
				m_Solver.Add( m_Matrix.Row( idx ).Sum( m_MagicConstant ) );
				m_Solver.Add( m_Matrix.Col( idx ).Sum( m_MagicConstant ) );
			}

			bool mm		= false;
			if( mm )
			{
				if( count % 2 == 0 )
				{
					int magicConstant2	= m_MagicConstant / 2;

					for( int idx = 0; idx < count; ++idx )
					{
						m_Solver.Add( m_Matrix.Row( idx, 0, count / 2 ).Sum( magicConstant2 ) );
						m_Solver.Add( m_Matrix.Row( idx, count / 2, count ).Sum( magicConstant2 ) );
						m_Solver.Add( m_Matrix.Col( idx, 0, count / 2 ).Sum( magicConstant2 ) );
						m_Solver.Add( m_Matrix.Col( idx, count / 2, count ).Sum( magicConstant2 ) );
					}
				}
			}
		
			m_Solver.Add( m_Matrix.DiagLeftTopToBottomRight().Sum( m_MagicConstant ) );
			m_Solver.Add( m_Matrix.DiagRightTopToBottomLeft().Sum( m_MagicConstant ) );

			IntVarListAllDifferent ad	= m_Matrix.VarList.AllDifferent();
			m_Solver.Add( ad );

			// remove symmetry
			m_Solver.Add( m_Matrix[ 0, 0 ] < m_Matrix[ 0, count - 1 ] );
			m_Solver.Add( m_Matrix[ 0, 0 ] < m_Matrix[ count - 1, count - 1 ] );
			m_Solver.Add( m_Matrix[ 0, 0 ] < m_Matrix[ count - 1, 0 ] );
			m_Solver.Add( m_Matrix[ 0, count - 1 ] < m_Matrix[ count - 1, 0 ] );
		}

		public IntVarMatrix Matrix
		{
			get
			{
				return m_Matrix;
			}
		}

		public int MagicConstant
		{
			get
			{
				return m_MagicConstant;
			}
		}
		
		IntVarMatrix	m_Matrix;
		int				m_MagicConstant;
	}
}

//--------------------------------------------------------------------------------
