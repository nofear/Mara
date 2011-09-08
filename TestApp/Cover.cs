using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;
using MaraInterval.Interval;
using MaraSolver.Reversible;

namespace TestApp
{
	public class Cover : SolverBase
	{
		public Cover( Solver solver, int columns, IntVarList list ) :
			base( solver )
		{
			m_Columns	= columns;

			m_List		= list;
			m_Avail		= new IntVar( solver, 0, list.Count - 1 );;

			m_Index		= new IntVar( solver, IntDomain.Empty, "" );
			m_Union		= new IntVar( solver, IntDomain.Empty, "" );
		}

		public IntVarList List
		{
			get
			{
				return m_List;
			}
		}

		public IntVar Avail
		{
			get
			{
				return m_Avail;
			}
		}

		public IntVar Index
		{
			get
			{
				return m_Index;
			}
		}

		public IntVar Domain
		{
			get
			{
				return m_Union;
			}
		}

		public bool IsBound()
		{
			if( !m_Union.Domain.IsSimple() )
				return false;
			
			return m_Union.Domain.Cardinality == m_Columns;
		}

		public void Add( int index )
		{
			IntVar var	= m_List[ index ];

			bool add	= !m_Union.Domain.IntersectsWith( var.Domain );
			if( add )
			{
				m_Index.Union( index );
				m_Union.Union( var.Domain );
			}
		}

		public int Select()
		{
			int chosenIdx		= m_List.Count;
			int chosenVarCard	= 0;

			for( int idx = m_Avail.Min; idx <= m_Avail.Max; ++idx )
			{
				IntVar var		= m_List[ idx ];

				if( m_Avail.Domain.Contains( idx )
						&& !m_Union.Domain.IntersectsWith( var.Domain ) )
				{
					int varCard		= var.Domain.Cardinality;

					if( chosenIdx == m_List.Count
							|| ( chosenVarCard < varCard ) )
					{
						chosenIdx		= idx;
						chosenVarCard	= varCard;
					}
				}
			}

			return chosenIdx;
		}


		int			m_Columns;
		IntVarList	m_List;
		IntVar		m_Avail;
		IntVar		m_Index;
		IntVar		m_Union;
	
		public class AddIndex : Goal
		{
			public AddIndex( Cover cover, int index ) :
				base( cover.m_Solver )
			{
				m_Cover		= cover;
				m_Index		= index;
			}

			public override void Execute()
			{
				m_Cover.Add( m_Index );
			}
			
			Cover	m_Cover;
			int		m_Index;
		}


		public class Search : Goal
		{
			public Search( Cover cover ) :
				base( cover.Solver )
			{
				m_Cover		= cover;
			}

			public override string ToString()
			{
				return "Generate()";
			}
	 
			public override void Execute()
			{
				if( m_Cover.IsBound() )
					return;
			
				int index    = m_Cover.Select();
				if( index > m_Cover.m_Avail.Max )
				{
					Fail();
					return;
				}
				
				//ExecuteDelegate add = delegate { m_Cover.Add( index ); };
				
				Goal choice1	= new GoalAnd( new AddIndex( m_Cover, index ), m_Cover.m_Avail != index );
				Goal choice2	= m_Cover.m_Avail != index;
				Goal search		= new GoalOr( choice1, choice2 );
				
				Add( new GoalAnd( search, this ) );
			}
	 
			Cover	m_Cover;
		}
	}
}
