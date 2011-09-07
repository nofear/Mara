using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;
using MaraSolver.Float;

namespace MaraSolver
{
	public class SolverCopy
	{
		public SolverCopy()
		{
			m_GoalMap			= new Dictionary<Goal,Goal>();
		

		}


		public Variable[] Copy( Variable[] other )
		{
			Variable[] copy	= new Variable[ other.Length ];
			for( int idx = 0; idx < other.Length; ++idx )
			{
				copy[ idx ]		= m_VarList[ other[ idx ].Index ];
			}

			return copy;
		}

		public VariableList[] Copy( VariableList[] other )
		{
			VariableList[] copy	= new VariableList[ other.Length ];
			for( int idx = 0; idx < other.Length; ++idx )
			{
				copy[ idx ]		= m_VarListList[ other[ idx ].Index ];
			}

			return copy;
		}

		public IntVar[] Copy( IntVar[] other )
		{
			IntVar[] copy	= new IntVar[ other.Length ];
			for( int idx = 0; idx < other.Length; ++idx )
			{
				copy[ idx ]		= m_IntVarList[ other[ idx ].Index ];
			}

			return copy;
		}

		public FltVar[] Copy( FltVar[] other )
		{
			FltVar[] copy	= new FltVar[ other.Length ];
			for( int idx = 0; idx < other.Length; ++idx )
			{
				copy[ idx ]		= m_FltVarList[ other[ idx ].Index ];
			}

			return copy;
		}

		public Goal Copy( Goal other )
		{
			Goal copy;
			if( !m_GoalMap.TryGetValue( other, out copy ) )
			{
				copy	= other.Copy( this );

				m_GoalMap[ other ]	= copy;
			} 

			return copy;
		}

		public Goal[] Copy( Goal[] other )
		{
			Goal[] copy	= new Goal[ other.Length ];
			for( int idx = 0; idx < other.Length; ++idx )
			{
				copy[ idx ]		= Copy( other[ idx ] );
			}

			return copy;
		}

		private Dictionary<object,object>		m_Map;

	}
}
