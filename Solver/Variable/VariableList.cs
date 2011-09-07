//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Variable/VariableList.cs $
 * 
 * 76    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 75    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 74    10-11-07 1:44 Patrick
 * rename BaseAt => Get(..), added Set(..)
 * 
 * 73    12-10-07 22:38 Patrick
 * using smarter indexing
 * 
 * 72    12-10-07 0:28 Patrick
 * moved compare class
 * 
 * 71    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 70    12-07-07 21:32 Patrick
 * removed registration
 * 
 * 69    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 68    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 67    4-07-07 21:08 Patrick
 * added Init(..)
 * 
 * 66    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 65    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 64    26-03-07 16:12 Patrick
 * added Name property
 * 
 * 63    22-03-07 21:09 Patrick
 * refactored Clone()
 * 
 * 62    20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 61    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 60    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 59    24-02-07 0:46 Patrick
 * cleanup code
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using MaraSolver.BaseConstraint;
using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Summary description for VariableList.
	/// </summary>
	public abstract class VariableList : RevBase, IList<Variable>
	{
		protected VariableList( Solver solver ) :
			base( solver.StateStack )
		{
			m_Solver		= solver;
			m_Name			= string.Empty;
			m_IndexList		= new List<int>();
		}

		#region RevBase members

		public override object State
		{
			get
			{
				List<int> list	= m_IndexList;

				m_IndexList	= new List<int>( m_IndexList );

				return list;
			}

			set
			{
				m_IndexList      = value as List<int>;
			}
		}

		#endregion

		public Solver Solver
		{
			get
			{
				return m_Solver;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}

			set
			{
				m_Name	= value;
			}
		}

		public int Count
		{
			get
			{
				return m_IndexList.Count;
			}
		}

		public int Capacity
		{
			get
			{
				return m_IndexList.Capacity;
			}

			set
			{
				m_IndexList.Capacity	= value;
			}
		}

		public virtual string ToString( bool wd )
		{
			StringBuilder str	= new StringBuilder();
	
			for( int idx = 0; idx < m_IndexList.Count; ++idx )
			{
				if( idx > 0 )
				{
					str.Append( "," );
				}

				str.Append( this[ idx ].ToString( wd ) );
			}			

			return str.ToString();
		}

		public override string ToString()
		{
			return ToString( true );
		}

		public bool IsBound()
		{
			int idx;
			for( idx = 0;
					idx < m_IndexList.Count
						&& this[ idx ].IsBound();
					++idx ) {};
	
			return idx == m_IndexList.Count;
		}

		public virtual void Unbound()
		{
			for( int idx = 0; idx < m_IndexList.Count; ++idx )
			{
				this[ idx ].Unbound();
			}			
		}
		
		protected abstract Variable Get( int index );

		protected void Set( int index, Variable var )
		{
			int valueIndex	= var.Index;
			if( m_IndexList[ index ] != valueIndex )
			{
				Store();
		
				m_IndexList[ index ]	= valueIndex;
			}
		}
	
		#region IList<Variable> Members

		public Variable this[ int index ]
		{
			get
			{
				return Get( index );
			}

			set
			{
				Set( index, value );
			}
		}

		public void Insert( int index, Variable var )
		{
			Store();

			m_IndexList.Insert( index, var.Index );
		}

		public void RemoveAt( int index )
		{
			Store();

			m_IndexList.RemoveAt( index );
		}

		public int IndexOf( Variable var )
		{
			return m_IndexList.IndexOf( var.Index );
		}

		#endregion

		#region ICollection<Variable> Members

		public void Add( Variable var )
		{
			Store();

			m_IndexList.Add( var.Index );
		}

		public bool Remove( Variable var )
		{
			Store();

			return m_IndexList.Remove( var.Index );
		}

		public void Clear()
		{
			Store();

			m_IndexList.Clear();
		}

		public bool Contains( Variable var )
		{
			return m_IndexList.Contains( var.Index );
		}

		public void CopyTo( Variable[] array, int arrayIndex )
		{
			for( int idx = 0; idx < m_IndexList.Count; ++idx )
			{
				array[ arrayIndex++ ]	= this[ idx ];
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region VariableEnumerator

		// Inner class implements IEnumerator interface:
		private sealed class VariableEnumerator : IEnumerator<Variable>
		{
			VariableList	m_VarList;
			int				m_Index;

			public VariableEnumerator( VariableList varList )
			{
				m_VarList	= varList;
				m_Index		= -1;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				int index	= m_Index + 1;
				if( index < 0
						|| index >= m_VarList.Count )
					return false;
				
				m_Index		= index;
				
				return true;
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				m_Index		= -1;
			}

			#region IEnumerator<Variable> Members

			public Variable Current
			{
				get
				{
					return m_VarList[ m_Index ];
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			#endregion
		}

		#endregion
	
		#region IEnumerable<Variable> Members

		public IEnumerator<Variable> GetEnumerator()
		{
			return new VariableEnumerator( this );
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Predicates
		
		public static bool IsBound( VariableList varList )
		{
			return varList.IsBound();
		}

		#endregion

		protected Solver	m_Solver;
		protected List<int>	m_IndexList;
		private string		m_Name;
	}
}
