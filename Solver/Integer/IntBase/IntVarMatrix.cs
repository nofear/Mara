//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntBase/IntVarMatrix.cs $
 * 
 * 29    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 28    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 27    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 26    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 25    31-07-07 22:27 Patrick
 * removed obsolete Copy(..)
 * 
 * 24    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 23    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 22    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 21    3/24/07 2:20a Patrick
 * more magic
 * 
 * 20    22-03-07 23:43 Patrick
 * add constraints to problem
 * 
 * 19    22-03-07 23:29 Patrick
 * cleanup
 * 
 * 18    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 17    8-03-07 0:36 Patrick
 * added Matrix(..)
 * 
 * 16    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 15    28-02-07 23:45 Patrick
 * display domain
 * 
 * 14    26-02-07 18:41 Patrick
 * added initial interval/domain constructor
 * 
 * 13    6/02/06 9:15a Patrick
 * updated GetString(..)
 * 
 * 12    14-03-06 22:07 Patrick
 * added integer & float namespace
 */
//--------------------------------------------------------------------------------
 
using System;
using System.Text;

using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarMatrix.
	/// </summary>
	public class IntVarMatrix : SolverBase
	{
		public IntVarMatrix( Solver solver, int rowCount, int colCount ) :
			this( solver, rowCount, colCount, solver.Horizon )
		{
		}

		public IntVarMatrix( Solver solver, int rowCount, int colCount, IntInterval interval ) :
			this( solver, rowCount, colCount, new IntDomain( interval ) )
		{
		}
	
		public IntVarMatrix( Solver solver, int rowCount, int colCount, IntDomain domain ) :
			base( solver )
		{
			m_VarList		= null;
			m_RowCount		= rowCount;
			m_ColCount		= colCount;
		
			InitMatrix( domain );
		}

		private IntVarMatrix( Solver solver, int rowCount, int colCount, IntVarList list ) :
			base( solver )
		{
			m_VarList		= list;
			m_RowCount		= rowCount;
			m_ColCount		= colCount;
		}

		public override string ToString()
		{
			StringBuilder str	= new StringBuilder();
			
			for( int row = 0; row < m_RowCount; ++row )
			{
				for( int col = 0; col < m_ColCount; ++col )
				{
					if( col > 0 )
					{
						str.Append( "\t" );
					}	

					str.Append( Cell( row, col ).Domain.ToString() );
				}

				str.Append( "\n" );
			}

			return str.ToString();
		}
		
		public int RowCount
		{
			get
			{
				return m_RowCount;
			}
		}

		public int ColCount
		{
			get
			{
				return m_ColCount;
			}
		}
		public IntVarList VarList
		{
			get
			{
				return m_VarList;
			}
		}

		public IntVar this[ int row, int col  ]
		{
			get
			{
				return Cell( row, col );
			}
		}

		public IntVar Cell( int row, int col )
		{
			return m_VarList[ row * m_ColCount + col ];
		}

		public IntVarMatrix Matrix( int rowOffset, int colOffset, int rowCount, int colCount )
		{
			IntVarList list	= new IntVarList( m_Solver );
						
			for( int row = 0; row < rowCount; ++row )
			{
				for( int col = 0; col < colCount; ++col )
				{
					list.Add( Cell( rowOffset + row, colOffset + col ) );
				}
			}
			
			return new IntVarMatrix( m_Solver, rowCount, colCount, list );
		}

		public IntVarList DiagLeftTopToBottomRight()
		{
			IntVarList list	= new IntVarList( m_Solver );

			if( m_RowCount == m_ColCount )
			{
				int size	= m_RowCount;

				for( int idx = 0; idx < size; ++idx )
				{
					list.Add( Cell( idx, idx ) );
				}
			}
			
			return list;
		}

		public IntVarList DiagRightTopToBottomLeft()
		{
			IntVarList list	= new IntVarList( m_Solver );

			if( m_RowCount == m_ColCount )
			{
				int size	= m_RowCount;

				for( int idx = 0; idx < size; ++idx )
				{
					list.Add( Cell( idx, ( size - 1 ) - idx ) );
				}
			}
			
			return list;
		}

		public IntVarList Row( int vrow )
		{
			return Row( vrow, 0, m_ColCount );
		}

		public IntVarList Col( int vcol )
		{
			return Col( vcol, 0, m_RowCount );
		}

		public IntVarList Row( int vrow, int colStart, int colStop )
		{
			IntVarList list	= new IntVarList( m_Solver );

			for( int col = colStart; col < colStop; ++col )
			{
				list.Add( Cell( vrow, col ) );
			}
			
			return list;
		}

		public IntVarList Col( int vcol, int rowStart, int rowStop )
		{
			IntVarList list	= new IntVarList( m_Solver );

			for( int row = rowStart; row < rowStop; ++row )
			{
				list.Add( Cell( row, vcol ) );
			}
			
			return list;
		}

		private void InitMatrix( IntDomain domain )
		{
			m_VarList	= new IntVarList( m_Solver, m_RowCount * m_ColCount );

			for( int row = 0; row < m_RowCount; ++row )
			{
				for( int col = 0; col < m_ColCount; ++col )
				{
					string name		= row.ToString() + "." + col.ToString();

					IntVar cell		= new IntVar( m_Solver, domain, name );
										
					m_VarList.Add( cell );
				}
			}
		}	

		static public IntVarMatrix operator+( IntVarMatrix lhs, IntVarMatrix rhs )
		{
			if( ( lhs.RowCount != rhs.RowCount )
					|| ( lhs.ColCount != rhs.ColCount ) )
				throw new Exception( "" );
			
			Solver solver	= lhs.Solver;
			int rowCount	= lhs.RowCount;
			int colCount	= lhs.ColCount;
			
			IntVarList list	= new IntVarList( solver );
						
			for( int row = 0; row < rowCount; ++row )
			{
				for( int col = 0; col < colCount; ++col )
				{
					IntVarExprVar expr	=  lhs[ row, col ] + rhs[ row, col ];
					solver.Add( expr );
				
					list.Add( expr.Var0 );
				}
			}
			
			return new IntVarMatrix( solver, rowCount, colCount, list );
		}

		static public IntVarMatrix operator*( IntVarMatrix lhs, int[] rhs )
		{
			if( lhs.ColCount != rhs.Length )
				throw new Exception( "" );

			Solver solver	= lhs.Solver;
			IntVarList list	= new IntVarList( solver );
						
			for( int row = 0; row < lhs.RowCount; ++row )
			{
				IntVar[] rowList	= new IntVar[ lhs.ColCount ];

				for( int col = 0; col < lhs.ColCount; ++col )
				{
					rowList[ col ]	= lhs.Cell( row, col );
				}

				IntVarListDotProduct dp		= new IntVarListDotProduct( solver, rowList, rhs );
				solver.Add( dp );

				list.Add( dp.Var0 );
			}
			
			return new IntVarMatrix( solver, lhs.ColCount, 1, list );
		}
		
		IntVarList		m_VarList;
		int				m_RowCount;
		int				m_ColCount;
	}
}

//--------------------------------------------------------------------------------
