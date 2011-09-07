//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Utility/BitArray.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 15    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 14    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 13    6/17/06 1:56p Patrick
 * fixed Get(..)/Set(..)
 * 
 * 12    5-06-06 21:48 Patrick
 * using unsafe{..} to speed up bit operations
 * 
 * 11    16-03-06 22:33 Patrick
 * added generic bool enumerable
 * 
 * 10    14-03-06 21:51 Patrick
 * put classes into proper namespace
 * 
 * 9     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 8     8-01-06 15:38 Patrick
 * implemented ICollection interface
 * 
 * 7     8-01-06 15:16 Patrick
 * adjusted naming
 * 
 * 6     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 5     6/17/05 2:19p Patrick
 * refactored bit array
 * 
 * 4     16-06-05 21:17 Patrick
 * added Offset(..)
 * 
 * 3     6/14/05 10:33p Patrick
 * extended implemenation
 * 
 * 2     2-06-05 21:31 Patrick
 * added IntersectsWith
 * 
 * 1     1-06-05 23:55 Patrick
 * initial version
 * added FBase
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraInterval.Utility
{
	public class BitArray : ICollection, IEnumerable<bool>, IEnumerable, ICloneable
	{
		public BitArray() :
			this( 0, false )
		{
		}

		public BitArray( int size ) :
			this( size, false )
		{
		}

		public BitArray( int size, bool value )
		{
			m_Count		= size;
			m_Reserve	= ReserveSize( m_Count );
			m_BitArray	= new UInt32[ m_Reserve ];
			
			for( int idx = 0; idx < m_BitArray.Length; ++idx )
			{
				m_BitArray[ idx ]	= value
									? 0xFFFFFFFF
									: 0x00000000;
			}
		}

		public object Clone()
		{
			BitArray ba	= new BitArray();
			
			ba.m_Count		= m_Count;
			ba.m_Reserve	= m_Reserve;
			ba.m_BitArray	= m_BitArray.Clone() as UInt32[]; 
			
			return ba;
		}
		
		public bool this[ int index ]
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

		public bool Get( int index )
		{
			if( index < 0 || index > m_Count - 1 )
				throw new IndexOutOfRangeException();

			int offset	= index >> 5;
			int mod		= index & 0x1f;

			return ( m_BitArray[ offset ] & ( 1 << mod ) ) != 0;
		}

		public void Set( int index, bool value )
		{
			if( index < 0 || index > m_Count - 1 )
				throw new IndexOutOfRangeException();

			int offset	= index >> 5;
			int mod		= index & 0x1f;
			
			UInt32 mask	= (UInt32) 1 << mod;

			m_BitArray[ offset ]	&= ~mask;
			
			if( value )
			{
				m_BitArray[ offset ]	|= mask;
			}
		}

		public void Set( int idx0, int idx1 )
		{
			for( int idx = idx0; idx < idx1; ++idx )
			{
				Set( idx, true );
			}
		}

		public void Clear( int idx0, int idx1 )
		{
			for( int idx = idx0; idx < idx1; ++idx )
			{
				Set( idx, false );
			}
		}

		public void Or( BitArray ba )
		{
			int length	= Math.Min( m_BitArray.Length, ba.m_BitArray.Length );

			unsafe
			{
				fixed( UInt32* pDst = m_BitArray, pSrc = ba.m_BitArray )
				{
					UInt32* ps	= pSrc;
					UInt32* pd	= pDst;
				
					int count	= length / 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	|= *ps++;
						*pd++	|= *ps++;
						*pd++	|= *ps++;
						*pd++	|= *ps++;
					}

					count	= length % 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	|= *ps++;
					}
				}
			}
		}

		public void And( BitArray ba )
		{
			int length	= Math.Min( m_BitArray.Length, ba.m_BitArray.Length );

			unsafe
			{
				fixed( UInt32* pDst = m_BitArray, pSrc = ba.m_BitArray )
				{
					UInt32* ps	= pSrc;
					UInt32* pd	= pDst;
				
					int count	= length / 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	&= *ps++;
						*pd++	&= *ps++;
						*pd++	&= *ps++;
						*pd++	&= *ps++;
					}

					count	= length % 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	&= *ps++;
					}
				}
			}
		}

		public void Xor( BitArray ba )
		{
			int length	= Math.Min( m_BitArray.Length, ba.m_BitArray.Length );
		
			unsafe
			{
				fixed( UInt32* pDst = m_BitArray, pSrc = ba.m_BitArray )
				{
					UInt32* ps	= pSrc;
					UInt32* pd	= pDst;
				
					int count	= length / 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	^= *ps++;
						*pd++	^= *ps++;
						*pd++	^= *ps++;
						*pd++	^= *ps++;
					}

					count	= length % 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	^= *ps++;
					}
				}
			}
		}

		public void NotAnd( BitArray ba )
		{
			int length	= Math.Min( m_BitArray.Length, ba.m_BitArray.Length );

			unsafe
			{
				fixed( UInt32* pDst = m_BitArray, pSrc = ba.m_BitArray )
				{
					UInt32* ps	= pSrc;
					UInt32* pd	= pDst;
				
					int count	= length / 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	&= ~*ps++;
						*pd++	&= ~*ps++;
						*pd++	&= ~*ps++;
						*pd++	&= ~*ps++;
					}

					count	= length % 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pd++	&= ~*ps++;
					}
				}
			}
		}

		public void Not()
		{
			unsafe
			{
				fixed( UInt32* pArr = m_BitArray )
				{
					UInt32* pa	= pArr;
				
					int count	= m_BitArray.Length / 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pa	= ~*pa; pa++;
						*pa	= ~*pa; pa++;
						*pa	= ~*pa; pa++;
						*pa	= ~*pa; pa++;
					}

					count	= m_BitArray.Length % 4;
					for( int idx = 0; idx < count; ++idx )
					{
						*pa	= ~*pa; pa++;
					}
				}
			}
		}

		public bool IntersectsWith( BitArray ba )
		{
			bool intersect	= false;

			int lenght	= Math.Min( m_BitArray.Length, ba.m_BitArray.Length );

			unsafe
			{
				fixed( UInt32* pDst = m_BitArray, pSrc = ba.m_BitArray )
				{
					UInt32* ps	= pSrc;
					UInt32* pd	= pDst;

					for( int idx = 0; idx < lenght && !intersect; ++idx )
					{
						intersect	= ( *pSrc & *pDst ) != 0;
					}
				}
			}
		
			return intersect;
		}

		private void Reserve( int reserve )
		{
			UInt32[] bitArray = new UInt32[ reserve ];

			if( m_Reserve < reserve )
			{
				m_BitArray.CopyTo( bitArray, 0 );

			}
			else
			{
				for( int idx = 0; idx < reserve; ++idx )
				{
					bitArray[ idx ] = m_BitArray[ idx ];
				}
			}

			m_Reserve	= reserve;
			m_BitArray	= bitArray;
		}

		static private int ReserveSize( int count )
		{
			return (int) Math.Ceiling( (double) count / 32 );
		}

		#region ICollection Members

		public int Count
		{
			get
			{
				return m_Count;
			}
			
			set
			{
				int reserve		= ReserveSize( value );
				if( m_Reserve != reserve )
				{
					Reserve( reserve );
				}
			}
		}

		public void CopyTo( Array array, int index )
		{
			for( int idx = index; idx < m_Count; ++idx )
			{
				array.SetValue( Get( idx ), idx - index );
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		#endregion

		int			m_Count;
		int			m_Reserve;
		UInt32[]	m_BitArray;

		#region IEnumerable<bool> Members

		public IEnumerator<bool> GetEnumerator()
		{
			return new BitArrayEnumerator( this );
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new BitArrayEnumerator( this );
		}

		#endregion

		#region BitArrayEnumerator

		// Inner class implements IEnumerator interface:
		private class BitArrayEnumerator : IEnumerator<bool>
		{
			BitArray	m_Parent;
			int			m_Index;

			public BitArrayEnumerator( BitArray parent )
			{
				m_Parent	= parent;

				Reset();
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				bool valid	= m_Index < m_Parent.Count - 1;
				if( valid )
				{
					++m_Index;
				}

				return valid;
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				m_Index		= -1;
			}

			// Declare the Current property required by IEnumerator:
			public bool Current
			{
				get
				{
					return m_Parent[ m_Index ];
				}
			}

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
					return m_Parent[ m_Index ];
				}
			}

			#endregion
		}

		#endregion
	}
}

//--------------------------------------------------------------------------------
