//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/RevList.cs $
 * 
 * 13    8-11-07 2:03 Patrick
 * fixed indent
 * 
 * 12    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 11    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
 
//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
  	[DebuggerDisplay("{m_List}")]
    public class RevList<T> : RevBase, IList<T>, ICollection<T>, IEnumerable<T>
      {
            public RevList( IStateStack stateStack ) : 
				base( stateStack )
            {
                  m_List      = new List<T>();
            }
 
            public override object State
            {
				get
				{
					List<T> list	= m_List;
					
					m_List	= new List<T>( m_List );

					return list;
				}
				set
				{
					m_List      = value as List<T>;
				}
            }
            
            public int Capacity
			{
				get
				{
					return m_List.Capacity;
				}

				set
				{
					m_List.Capacity		= value;;
				}
			}
 
            #region ICollection<T> Members
 
            public int Count
            {
                  get
                  {
                        return m_List.Count;
                  }
            }
 
            public T this[ int index ]
            {
                  get
                  {
                        return m_List[ index ];
                  }
                 
                  set
                  {
                        Store();
 
                        m_List[ index ]         = value;
                  }
            }
 
            public void Clear()
            {
                  Store();
 
                  m_List.Clear();
            }
 
            public bool Contains( T item )
            {
                  return m_List.Contains( item );
            }
 
            public int IndexOf( T item )
            {
                  return m_List.IndexOf( item );
            }
 
			public int FindLastIndex( Predicate<T> match )
			{
				return m_List.FindLastIndex( match );
			}
			
			public int FindLastIndex( int startIndex, Predicate<T> match )
			{
				return m_List.FindLastIndex( startIndex, match );
			}
			
			public int FindLastIndex( int startIndex, int count, Predicate<T> match )
			{
				return m_List.FindLastIndex( startIndex, count, match );
			}

            public void Add( T item )
            {
                  Store();
 
                  m_List.Add( item );
            }
 
            public void Insert( int index, T item )
            {
                  Store();
 
                  m_List.Insert( index, item );
            }
 
            public bool Remove( T item )
            {
                  Store();
 
                  return m_List.Remove( item );
            }
 
            public void RemoveAt( int index )
            {
                  Store();
 
                  m_List.RemoveAt( index );
            }
 
            public void CopyTo( T[] array, int arrayIndex )
            {
                  m_List.CopyTo( array, arrayIndex );
            }
 
            public bool IsReadOnly
            {
                  get
                  {
                        return false;
                  }
            }
 
            #endregion
 
            #region IEnumerable<T> Members
 
            public IEnumerator<T> GetEnumerator()
            {
                  return m_List.GetEnumerator();
            }
 
            #endregion
 
            #region IEnumerable Members
 
            IEnumerator IEnumerable.GetEnumerator()
            {
                  return m_List.GetEnumerator();
            }
 
            #endregion
 
            List<T> m_List;
      }
}

//--------------------------------------------------------------------------------
