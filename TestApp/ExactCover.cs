using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;
using MaraInterval.Interval;

namespace TestApp
{
	public class ExactCover : Problem
	{
		public ExactCover( int columns ) :
			base( 0, 1024 )
		{
			m_List	= new IntVarList( m_Solver );
		}

		public IntVarList List
		{
			get
			{
				return m_List;
			}
		}

		public void Add( int[] row, string name )
		{
			bool[] rowbool	= new bool[ row.Length ];
			
			for( int idx = 0; idx < row.Length; ++idx )
			{
				rowbool[ idx ]	= row[ idx ] != 0;
			}
			
			Add( rowbool, name );
		}

		public void Add( bool[] row, string name )
		{
			IntDomain domain	= new IntDomain( row );
			IntVar var			= new IntVar( m_Solver, domain, name );
		
			m_List.Add( var );
		}




		private IntVarList m_List;
	}
}
