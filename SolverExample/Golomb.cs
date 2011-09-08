using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;

namespace SolverExample
{
/*
	2          0.05s
	3          0.00s
	4          0.00s    
	5          0.00s    
	6          0.00s    
	7          0.02s    
	8          0.03s    
	9          0.48s    
	10         1.67s    
	11         30.28s
	12         225.22s
	13         5882.19s
*/

	public class Golomb : Problem
	{
		public Golomb( int n ) :
			base()
		{
			int maxLength	= (int) Math.Pow( 2, (n-1) ) - 1;

			m_Solver.Horizon	= new IntInterval( 0, maxLength );
  
			m_MarkList		= new IntVarList( m_Solver );
			for( int idx = 0; idx < n; ++idx )
			{
				m_MarkList.Add( new IntVar( m_Solver, idx, maxLength, "m" + ( idx + 1 ).ToString() ) );
			}
			
			int mark	= 0;
			int pos		= 0;

			m_DiffList	= new IntVarList( m_Solver, n * ( n -1 ) );
			for( int i = 0; i < n - 1; ++i )
			{
				for( int j = i + 1; j < n; ++j )
				{
					if( i == n/2 && j == n-1 )
						mark = pos;

					IntVarExpr exp	= m_MarkList[ j ] - m_MarkList[ i ];
					exp.Var0.Min	= 1;
					exp.Var0.Name	= "#" + m_MarkList[ j ].Name + "-" + m_MarkList[ i ].Name;
					m_Solver.Add( exp );

					m_DiffList.Add( exp.Var0 );
					
					++pos;
				}
			}

			IntVarListAllDifferent ad	= m_DiffList.AllDifferent();
			//ad.Level	= Constraint.PropagateLevel.High;
			m_Solver.Add( ad );

			// start mark at 0
			m_MarkList[ 0 ].Value	= 0;

			// lower half should be less than the half difference
			IntVarCmp cmp		=  m_MarkList[ (n-1)/2 ] < m_DiffList[ mark ];
			m_Solver.Add( cmp );

			Console.WriteLine( cmp.ToString() +", " + m_DiffList[ mark ].ToString() );
		}

		public IntVarList MarkList
		{
			get
			{
				return m_MarkList;
			}
		}

		public IntVarList DiffList
		{
			get
			{
				return m_DiffList;
			}
		}

		IntVarList	m_MarkList;
		IntVarList	m_DiffList;
	}
}
