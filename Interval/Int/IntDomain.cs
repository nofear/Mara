//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Int/IntDomain.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 45    5-03-08 0:37 Patrick
 * updated interval methods
 * 
 * 44    20-12-07 21:58 Patrick
 * minor speed improvement
 * 
 * 43    21-11-07 1:52 Patrick
 * fixed issue with finding edge 0/1
 * 
 * 42    15-11-07 2:01 Patrick
 * fixed bug in IntersectsWith(..), intersection was not deteted correctly
 * 
 * 41    14-11-07 22:39 Patrick
 * fixed GetHashCode()
 * 
 * 40    10-11-07 1:26 Patrick
 * added ICloneable
 * 
 * 39    8-11-07 2:03 Patrick
 * added bool[] constructor
 * 
 * 38    25-07-07 3:55 Patrick
 * added readonl
 * 
 * 37    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 36    14-03-07 0:38 Patrick
 * keep random simple for now
 * 
 * 35    13-03-07 21:48 Patrick
 * added ToArray(), Random()
 * 
 * 34    13-03-07 21:20 Patrick
 * added Random()
 * fixed FindPrevZero()
 * 
 * 33    12-03-07 16:10 Patrick
 * fixed minor bug in Contains( val )
 * 
 * 32    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 31    8-03-07 1:26 Patrick
 * fixed issue in FindPrevZero(..)
 * 
 * 30    28-02-07 23:39 Patrick
 * fixed/renamed find next/prev zero/one
 * 
 * 29    27-02-07 22:57 Patrick
 * added Square()/Sqrt()
 * 
 * 28    21-02-07 20:07 Patrick
 * update
 * 
 * 27    21-02-07 20:04 Patrick
 * added Binary class
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
//1     |---|
// |---|
//
//2     |---|
//    |---|
//
//3     |---|
//    |-------|
//
//4   |-------|
//      |---|
//
//5     |---|
//        |---|
//
//6     |---|
//           |---|
//--------------------------------------------------------------------------------

namespace MaraInterval.Interval
{
	public sealed class IntDomain : IIntInterval, IEnumerable<IntInterval>, ICloneable
	{
		public IntDomain()
		{
			SetEmpty();
		}

		public IntDomain( int val ) :
			this( val, val )
		{
		}

		public IntDomain( int min, int max ) :
			this( new IntInterval( min, max ) )
		{
		}

		public IntDomain( IntInterval interval )
		{
			m_Interval		= interval;
			m_BitArray		= null;
			m_Offset		= 0;
		}

		public IntDomain( IntDomain domain )
		{
			m_Interval		= domain.m_Interval;
			m_BitArray		= domain.m_BitArray;
			m_Offset		= domain.m_Offset;
		}

		public IntDomain( int[] list )
		{
			if( list.Length % 2 == 0 )
			{
				int count	= list.Length / 2;
			
				m_Interval		= new IntInterval( list[ 0 ], list[ list.Length - 1 ] );

				if( count == 1 )
				{
					m_BitArray		= null;
					m_Offset		= 0;
				}
				else
				{
					m_BitArray		= new UInt32[ ToLength( m_Interval ) ];
					m_Offset		= ToOffset( m_Interval.Min );

					for( int idx = 0; idx < count; ++idx )
					{
						SetOne( list[ idx * 2 ], list[ idx * 2 + 1 ] );
					}
				}
			}
		}

		public IntDomain( bool[] list )
		{
			m_Interval		= new IntInterval( 0, list.Length - 1 );
			m_BitArray		= new UInt32[ ToLength( m_Interval ) ];
			m_Offset		= 0;
		
			for( int idx = 0; idx < list.Length; ++idx )
			{
				if( list[ idx ] )
				{
					int offset	= ToOffset( idx );
					int mod		= ToMod( idx );
					
					UInt32 mask	= (UInt32) 1 << mod;

					m_BitArray[ offset ]	|= mask;
				}
			}
			
			UpdateInterval( true, true );
		}

		public IntDomain( IEnumerable<IntInterval> list )
		{
			IEnumerator<IntInterval> it		= list.GetEnumerator();
			if( it.MoveNext() )
			{
				int min		= it.Current.Min;
				int max		= it.Current.Max;
				int count	= 1;
				
				while( it.MoveNext() )
				{
					min		= Math.Min( min, it.Current.Min );
					max		= Math.Max( max, it.Current.Max );
					++count;
				}
				
				m_Interval		= new IntInterval( min, max );
				if( count == 1 )
				{
					m_BitArray		= null;
					m_Offset		= 0;
				}
				else
				{
					m_BitArray		= new UInt32[ ToLength( m_Interval ) ];
					m_Offset		= ToOffset( min );

					it.Reset();
					while( it.MoveNext() )
					{
						SetOne( it.Current );
					}			
				}
			}
			else
			{
				SetEmpty();
			}
		}

		public override string ToString()
		{
			StringBuilder str	= new StringBuilder();
			
			if( m_Interval.IsEmpty() )
			{
				str.Append( "[]" );
			}
			else
			{
				foreach( IntInterval interval in this )
				{
					str.Append( interval.ToString() );
				}
			}
						
			return str.ToString();
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( this, obj ) )
				return true;

			IntDomain domain	= obj as IntDomain;
			if( !ReferenceEquals( domain, null ) )
				return Equals( domain );
			
			return false;
		}

		public bool Equals( IntDomain domain )
		{
			if( ReferenceEquals( domain, this ) )
				return true;

			if( m_Interval != domain.m_Interval )
				return false;

			if( IsSimple() && domain.IsSimple() )
				return true;

			if( IsSimple() ^ domain.IsSimple() )
				return false;

			int offset			= ToOffset( m_Interval.Min );
			int len				= ToLength( m_Interval );
			int srcOffset1		= offset - m_Offset;
			int srcOffset2		= offset - domain.m_Offset;

			return CompareRange( m_BitArray, srcOffset1, domain.m_BitArray, srcOffset2, len );
		}

		public override int GetHashCode()
		{
			return m_Interval.Max << 16 + m_Interval.Min;
		}

		public static IntDomain Empty
		{
			get
			{
				return m_Empty;
			}
		}

		public int Min
		{
			get
			{
				return m_Interval.Min;
			}
		}

		public int Max
		{
			get
			{
				return m_Interval.Max;
			}
		}

		public IntInterval Interval
		{
			get
			{
				return m_Interval;
			}
		}

		public int Cardinality
		{
			get
			{
				if( IsSimple() )
					return m_Interval.Cardinality;
			
				int card	= 0;

				unsafe
				{
					fixed( UInt32* pSrc = m_BitArray )
					{
						UInt32* src = pSrc;
						int length	= m_BitArray.Length;

						for( int idx = 0; idx < length; ++idx )
						{
							card	+= Binary.BitCount( *src++ );
						}
					}
				}			

				return card;
			}
		}

		public IntInterval[] ToArray()
		{
			if( IsSimple() )
				return new IntInterval[] { m_Interval };
		
			return new List<IntInterval>( this ).ToArray();
		}

		public bool IsSimple()
		{
			return ReferenceEquals( m_BitArray, null );
		}

		public bool IsEmpty()
		{
			return m_Interval.IsEmpty();
		}

		public bool Contains( int val )
		{
			if( IsSimple() )
				return m_Interval.Contains( val );

			if( !m_Interval.Contains( val ) )
				return false;

			return Get( val );	
		}

		public bool Contains( IntInterval interval )
		{
			if( IsSimple() )
				return m_Interval.Contains( interval );

			if( !m_Interval.Contains( interval ) )
				return false;

			return IsAllOne( interval );
		}

		#region Union
			
		public IntDomain Union( int val )
		{
			// QQQ: could refactor into a special case
			return Union( val, val );
		}

		public IntDomain Union( int min, int max )
		{
			return Union( new IntInterval( min, max ) );
		}

		public IntDomain Union( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return new IntDomain( interval );

			if( Contains( interval ) )
				return this;

			IntInterval intv	= m_Interval.Union( interval );

			IntDomain result	= new IntDomain();
			result.m_Interval	= intv;

			if( IsSimple() )
			{
				// 1, 6
				if( m_Interval.Max < interval.Min - 1
						|| m_Interval.Min > interval.Max + 1 )
				{
					if( intv.Cardinality <= m_MaxCardinality )
					{
						result.m_BitArray	= new UInt32[ ToLength( intv ) ];
						result.m_Offset		= ToOffset( intv.Min );
						result.SetOne( m_Interval );
						result.SetOne( interval );
					}
				}
			}
			else
			{
				int offset		= ToOffset( intv.Min );
			
				UInt32[] bitArray		= new UInt32[ ToLength( intv ) ];

				// 1, 2, 5, 6
				if( m_Interval.Min <= interval.Min || m_Interval.Max >= interval.Max )
				{
					// only need to copy difference
					IntInterval diff	= m_Interval.Difference( interval );
				
					int len1	= ToLength( diff );
					int offset1	= ToOffset( diff.Min );
				
					CopyRange( m_BitArray, offset1 - m_Offset, bitArray, offset1 - offset, len1 );
				}
				
				result.m_BitArray	= bitArray;
				result.m_Offset		= offset;
				result.SetOne( interval );
				result.CheckSimple();
			}
			
			return result;
		}

		public IntDomain Union( IntDomain domain )
		{
			if( ReferenceEquals( domain, this ) )
				return this;
			
			if( domain.IsEmpty() )
				return this;

			if( IsEmpty() )
				return domain;

			if( domain.IsSimple() )
				return Union( domain.m_Interval );

			if( IsSimple() )
			{
				if( m_Interval.Contains( domain.m_Interval ) )
					return this;
			
				return domain.Union( m_Interval );
			} 

			IntInterval intv			= m_Interval.Union( domain.m_Interval );

			IntInterval offset			= ToOffset( m_Interval );
			IntInterval domainOffset	= ToOffset( domain.m_Interval );

			UInt32[] bitArray		= new UInt32[ ToLength( intv ) ];

			bool bitArrayChanged	= false;

			// 2, 3, 4, 5 : intersects
			if( offset.IntersectsWith( domainOffset ) )
			{
				int len;
				int dstOffset		= 0;
				int srcOffset1		= offset.Min - m_Offset;
				int srcOffset2		= domainOffset.Min -  domain.m_Offset;

				if( offset.Min != domainOffset.Min )
				{
					// 2, 3 : copy part of begin domain
					if( offset.Min > domainOffset.Min )
					{
						len		= offset.Min - domainOffset.Min;

						CopyRange( domain.m_BitArray, srcOffset2, bitArray, dstOffset, len );

						dstOffset		+= len;
						srcOffset2		+= len;
					}
					// 4, 5 : copy part of begin this
					else //if( offset.Min < domainOffset.Min )
					{
						len		= domainOffset.Min - offset.Min;

						CopyRange( m_BitArray, srcOffset1, bitArray, dstOffset, len );

						dstOffset		+= len;
						srcOffset1		+= len;
					}
					
					bitArrayChanged		= true;
				}
				
				// union intersecting part.
				int offsetMin	= Math.Max( offset.Min, domainOffset.Min );
				int offsetMax	= Math.Min( offset.Max, domainOffset.Max );

				len		= offsetMax - offsetMin + 1;
				
				bitArrayChanged		|= UnionRange( m_BitArray, srcOffset1, domain.m_BitArray, srcOffset2, bitArray, dstOffset, len );

				dstOffset		+= len;
				srcOffset1		+= len;
				srcOffset2		+= len;

				if( offset.Max != domainOffset.Max )
				{
					// 3, 5 : copy part of end domain
					if( offset.Max < domainOffset.Max )
					{
						len		= domainOffset.Max - offset.Max;

						CopyRange( domain.m_BitArray, srcOffset2, bitArray, dstOffset, len );

						dstOffset		+= len;
						srcOffset2		+= len;
					}
					// 2, 4 : copy part of end this
					else //if( offset.Max > domainOffset.Max )
					{
						len		= offset.Max - domainOffset.Max;

						CopyRange( m_BitArray, srcOffset1, bitArray, dstOffset, len );

						dstOffset		+= len;
						srcOffset1		+= len;
					}
					
					bitArrayChanged		= true;
				}
			}
			else
			{
				// 1
				if( domainOffset.Max < offset.Min )
				{
					int idx		=  offset.Min - domainOffset.Min;
					
					CopyRange( m_BitArray, 0, bitArray, idx, m_BitArray.Length );	
					CopyRange( domain.m_BitArray, 0, bitArray, 0, domain.m_BitArray.Length );	
				}
				// 6
				else if( domainOffset.Min > offset.Max )
				{
					int idx		= domainOffset.Min - offset.Min;
					
					CopyRange( m_BitArray, 0, bitArray, 0, m_BitArray.Length );	
					CopyRange( domain.m_BitArray, 0, bitArray, idx, domain.m_BitArray.Length );	
				}
				
				bitArrayChanged		= true;
			}
				
			bool changed	= ( m_Interval != intv ) || bitArrayChanged;
			if( !changed )
				return this;

			IntDomain result	= new IntDomain( this );
			result.m_Interval	= intv;
			result.m_BitArray	= bitArray;
			result.m_Offset		= ToOffset( intv.Min );
			result.CheckSimple();

			return result;
		}

		#endregion

		#region Difference

		public IntDomain Difference( int val )
		{
			if( !m_Interval.Contains( val ) )
				return this;
			
			if( m_Interval.IsBound() && m_Interval.Min == val )
				return m_Empty;

			IntInterval intv	= ( m_Interval.Min == val ) ? new IntInterval( val + 1, m_Interval.Max )
								: ( m_Interval.Max == val ) ? new IntInterval( m_Interval.Min, val - 1 )
								: m_Interval;

			IntDomain result	= new IntDomain();
			result.m_Interval	= intv;

			if( IsSimple() )
			{
				// 4 : divide into two intervals...
				if( val > m_Interval.Min && val < m_Interval.Max )
				{
					result.m_BitArray	= new UInt32[ ToLength( intv ) ];
					result.m_Offset		= ToOffset( intv.Min );
					result.SetOne( m_Interval );
					result.Clear( val );
				}
			}
			else
			{
				if( !Get( val ) )
					return this;

				UInt32[] bitArray	= new UInt32[ m_BitArray.Length ];

				CopyRange( m_BitArray, 0, bitArray, 0, m_BitArray.Length );
			
				result.m_BitArray	= bitArray;
				result.m_Offset		= m_Offset;
				result.Clear( val );

				result.UpdateInterval( m_Interval.Min == val, m_Interval.Max == val );
			}			

			return result;		
		}

		public IntDomain Difference( int min, int max )
		{
			return Difference( new IntInterval( min, max ) );
		}
	
		public IntDomain Difference( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			// 1, 6
			if( !m_Interval.IntersectsWith( interval ) )
				return this;

			// 3 : completely remove interval => empty
			if( interval.Min <= m_Interval.Min && interval.Max >= m_Interval.Max )
				return m_Empty;

			// 4 : divide into two intervals...
			if( interval.Min > m_Interval.Min && interval.Max < m_Interval.Max )
			{
				if( !IsSimple() && IsAllZero( interval ) )
					return this;
			
				// check if we can split
				if( interval.Cardinality >= m_MaxCardinality )
					return this;
			}

			IntInterval intv	= m_Interval.Difference( interval );

			IntDomain result	= new IntDomain();
			result.m_Interval	= intv;

			if( IsSimple() )
			{
				// 4 : divide into two intervals...
				if( interval.Min > m_Interval.Min && interval.Max < m_Interval.Max )
				{
					result.m_BitArray	= new UInt32[ ToLength( intv ) ];
					result.m_Offset		= ToOffset( intv.Min );
					result.SetOne( m_Interval.Min, interval.Min - 1 );
					result.SetOne( interval.Max + 1, m_Interval.Max );
				}
			}
			else
			{
				UInt32[] bitArray	= new UInt32[ ToLength( intv ) ];

				// 4 : divide into two intervals...
				if( interval.Min > m_Interval.Min && interval.Max < m_Interval.Max )
				{
					UInt32 mask1	= ToMaskEnd( interval.Min - 1 );
					UInt32 mask2	= ToMaskBegin( interval.Max + 1 );
				
					int offsetMin1	= ToOffset( m_Interval.Min );
					int offsetMax1	= ToOffset( interval.Min - 1 );
					int offsetMin2	= ToOffset( interval.Max + 1 );
					int offsetMax2	= ToOffset( m_Interval.Max );

					if( offsetMax1 == offsetMin2 )
					{
						int len		= offsetMax2 - offsetMin1 + 1;
					
						CopyRange( m_BitArray, offsetMin1 - m_Offset, bitArray, 0, len );

						bitArray[ offsetMax1 - offsetMin1 ]		&= ~( ~mask1 & ~mask2 );
					}
					else
					{
						int len1		= offsetMax1 - offsetMin1 + 1;
						int len2		= offsetMax2 - offsetMin2 + 1;

						CopyRange( m_BitArray, offsetMin1 - m_Offset, bitArray, 0, len1 );
						CopyRange( m_BitArray, offsetMin2 - m_Offset, bitArray, offsetMin2 - offsetMin1, len2 );
						
						bitArray[ offsetMax1 - offsetMin1 ]		&= mask1;
						bitArray[ offsetMin2 - offsetMin1 ]		&= mask2;
					}
				}
				else
				{
					int len		= bitArray.Length;
					int offset	= ToOffset( intv.Min );
				
					CopyRange( m_BitArray, offset - m_Offset, bitArray, 0, len );

					if( interval.Min <= m_Interval.Min
								&& interval.Max < m_Interval.Max )
					{
						// 2 : left overlap
						bitArray[ 0 ]		&= ToMaskBegin( intv.Min );
					}

					else if( interval.Min > m_Interval.Min
								&& interval.Max >= m_Interval.Max )
					{
						// 5 : right overlap
						bitArray[ len - 1 ]	&= ToMaskEnd( intv.Max );
					}
				}

				result.m_BitArray	= bitArray;
				result.m_Offset		= ToOffset( intv.Min );
				result.UpdateInterval( interval.Min <= m_Interval.Min, interval.Max >= m_Interval.Max );
			}

			return result;
		}

		public IntDomain Difference( IntDomain domain )
		{
			if( ReferenceEquals( domain, this ) )
				return m_Empty;
			
			if( domain.IsEmpty() )
				return this;

			if( domain.IsSimple() )
				return Difference( domain.m_Interval );

			//1, 6
			if( !m_Interval.IntersectsWith( domain.m_Interval ) )
				return this;

			// 
			int length	= ToLength( m_Interval );

			if( IsSimple() )
			{
				if( domain.IsAllZero( m_Interval ) )
					return this;
			
				m_BitArray	= new UInt32[ length ];
				m_Offset	= ToOffset( m_Interval.Min );
				SetOne( m_Interval );
			}

			IntInterval offset			= ToOffset( m_Interval );
			IntInterval domainOffset	= ToOffset( domain.m_Interval );

			// 2, 3, 4, 5
			bool bitArrayChanged	= false;

			UInt32[] bitArray		= new UInt32[ length ];

			int len;
			int dstOffset		= 0;
			int srcOffset1		= offset.Min - m_Offset;
			int srcOffset2		= domainOffset.Min -  domain.m_Offset;

			if( offset.Min != domainOffset.Min )
			{
				// 2, 3 : copy part of begin domain
				if( offset.Min > domainOffset.Min )
				{
					srcOffset2		+= offset.Min - domainOffset.Min;
				}
				// 4, 5 : copy part of begin this
				else //if( offset.Min < domainOffset.Min )
				{
					len		= domainOffset.Min - offset.Min;

					CopyRange( m_BitArray, srcOffset1, bitArray, dstOffset, len );

					dstOffset		+= len;
					srcOffset1		+= len;
				}				
			}
			
			// difference intersecting part.
			int offsetMin	= Math.Max( offset.Min, domainOffset.Min );
			int offsetMax	= Math.Min( offset.Max, domainOffset.Max );

			len		= offsetMax - offsetMin + 1;
			
			bitArrayChanged		|= DifferenceRange( m_BitArray, srcOffset1, domain.m_BitArray, srcOffset2, bitArray, dstOffset, len );

			dstOffset		+= len;
			srcOffset1		+= len;
			srcOffset2		+= len;

			if( offset.Max != domainOffset.Max )
			{
				// 3, 5 : copy part of end domain
				if( offset.Max < domainOffset.Max )
				{
					srcOffset2		+= domainOffset.Max - offset.Max;
				}
				// 2, 4 : copy part of end this
				else //if( offset.Max > domainOffset.Max )
				{
					len		= offset.Max - domainOffset.Max;

					CopyRange( m_BitArray, srcOffset1, bitArray, dstOffset, len );

					dstOffset		+= len;
					srcOffset1		+= len;
				}
			}
			
			if( !bitArrayChanged )
				return this;

			IntDomain result	= new IntDomain();
			result.m_Interval	= m_Interval;
			result.m_BitArray	= bitArray;
			result.m_Offset		= offset.Min;
			result.UpdateInterval( domain.m_Interval.Min <= m_Interval.Min, domain.m_Interval.Max >= m_Interval.Max );
			
			return result;
		}

		#endregion

		#region Intersect

		public IntDomain Intersect( int val )
		{
			if( !m_Interval.IntersectsWith( val ) )
				return m_Empty;

			if( IsSimple() && m_Interval.IsBound() && m_Interval.Min == val )
				return this;

			return new IntDomain( val );
		}

		public IntDomain Intersect( int min, int max )
		{
			return Intersect( new IntInterval( min, max ) );
		}

		public IntDomain Intersect( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return m_Empty;

			// 1, 6
			if( !m_Interval.IntersectsWith( interval ) )
				return m_Empty;

			// 3
			if( m_Interval.Min >= interval.Min && m_Interval.Max <= interval.Max )
				return this;

			IntInterval intv	= m_Interval.Intersect( interval );

			IntDomain result = new IntDomain();
			result.m_Interval	= intv;

			if( !IsSimple() )
			{
				int len				= ToLength( intv );

				UInt32[] bitArray	= new UInt32[ len ];

				int offset		= ToOffset( intv.Min );
				int srcOffset	= offset - m_Offset;
				
				CopyRange( m_BitArray, srcOffset, bitArray, 0, len );

				bitArray[ 0 ]		&= ToMaskBegin( intv.Min );
				bitArray[ len - 1 ]	&= ToMaskEnd( intv.Max );

				result.m_BitArray	= bitArray;
				result.m_Offset		= offset;
				result.UpdateInterval( true, true );
			}

			return result;
		}

		public IntDomain Intersect( IntDomain domain )
		{
			if( ReferenceEquals( domain, this ) )
				return this;
			
			if( domain.IsEmpty() )
				return m_Empty;
		
			// 1, 6
			if( !m_Interval.IntersectsWith( domain.m_Interval) )
				return m_Empty;

			if( domain.IsSimple() )
				return Intersect( domain.m_Interval );

			if( IsSimple() )
			{
				// 3
				if( m_Interval.Min >= domain.m_Interval.Min && m_Interval.Max <= domain.m_Interval.Max )
				{
					if( domain.IsAllOne( m_Interval ) )
						return this;
				}
			
				return domain.Intersect( m_Interval );
			}

			IntInterval offset			= ToOffset( m_Interval );
			IntInterval domainOffset	= ToOffset( domain.m_Interval );

			// 2, 3, 4, 5
			bool bitArrayChanged	= false;

			IntInterval intv		= m_Interval.Intersect( domain.m_Interval );
		
			UInt32[] bitArray		= new UInt32[ ToLength( intv ) ];

			int dstOffset		= 0;
			int srcOffset1		= offset.Min - m_Offset;
			int srcOffset2		= domainOffset.Min -  domain.m_Offset;

			if( offset.Min != domainOffset.Min )
			{
				// 2, 3 : discard part of begin domain
				if( offset.Min > domainOffset.Min )
				{
					srcOffset2		+= offset.Min - domainOffset.Min;
				}
				// 4, 5 : discard part of begin this
				else //if( offset.Min < domainOffset.Min )
				{
					srcOffset1		+= domainOffset.Min - offset.Min;
					
					bitArrayChanged	= true;
				}				
			}

			// intersect intersecting part.
			int offsetMin	= Math.Max( offset.Min, domainOffset.Min );
			int offsetMax	= Math.Min( offset.Max, domainOffset.Max );

			int len		= offsetMax - offsetMin + 1;
			
			bitArrayChanged		|= IntersectRange( m_BitArray, srcOffset1, domain.m_BitArray, srcOffset2, bitArray, dstOffset, len );

			dstOffset		+= len;
			srcOffset1		+= len;
			srcOffset2		+= len;

			if( offset.Max != domainOffset.Max )
			{
				// 3, 5 : discard part of end domain
				if( offset.Max < domainOffset.Max )
				{
					srcOffset2		+= domainOffset.Max - offset.Max;
				}
				// 2, 4 : discard part of end this
				else //if( offset.Max > domainOffset.Max )
				{
					srcOffset1		+= offset.Max - domainOffset.Max;

					bitArrayChanged	= true;
				}
			}
			
			if( !bitArrayChanged )
				return this;
				
			IntDomain result	= new IntDomain();
			result.m_Interval	= intv;
			result.m_BitArray	= bitArray;
			result.m_Offset		= ToOffset( intv.Min );
			result.UpdateInterval( true, true );
								
			return result;
		}

		#endregion

		public bool IntersectsWith( IntInterval interval )
		{
			if( ReferenceEquals( interval, this ) )
				return true;
			
			if( IsEmpty() || interval.IsEmpty() )
				return false;

			// ~4 : divide into two intervals...
			if( interval.Min > m_Interval.Min
					&& interval.Max < m_Interval.Max
					&& !IsSimple() )
			{
				if( IsAllZero( interval ) )
					return false;
			}

			return m_Interval.IntersectsWith( interval );
		}

		public bool IntersectsWith( IntDomain domain )
		{
			if( ReferenceEquals( domain, this ) )
				return true;
			
			if( IsEmpty() || domain.IsEmpty() )
				return false;

			// 1, 6
			if( !m_Interval.IntersectsWith( domain.m_Interval ) )
				return false;

			if( domain.IsSimple() )
				return IntersectsWith( domain.m_Interval );
			
			if( IsSimple() )
				return domain.IntersectsWith( m_Interval );

			IntInterval offset			= ToOffset( m_Interval );
			IntInterval domainOffset	= ToOffset( domain.m_Interval );

				// 2, 3, 4, 5
			bool intersects		= false;

			IntInterval interval	= m_Interval.Intersect( domain.m_Interval );
		
			int srcOffset1		= offset.Min - m_Offset;
			int srcOffset2		= domainOffset.Min -  domain.m_Offset;

			if( offset.Min != domainOffset.Min )
			{
				// 2, 3 : discard part of begin domain
				if( offset.Min > domainOffset.Min )
				{
					srcOffset2		+= offset.Min - domainOffset.Min;
				}
				// 4, 5 : discard part of begin this
				else //if( offset.Min < domainOffset.Min )
				{
					srcOffset1		+= domainOffset.Min - offset.Min;
					
					intersects	= true;
				}				
			}

			if( intersects )
				return true;
	
			// intersect intersecting part.

			int offsetMin	= Math.Max( offset.Min, domainOffset.Min );
			int offsetMax	= Math.Min( offset.Max, domainOffset.Max );

			int len		= offsetMax - offsetMin + 1;
			
			intersects		|= IntersectsWithRange( m_BitArray, srcOffset1, domain.m_BitArray, srcOffset2, len );

			if( offset.Max != domainOffset.Max )
			{
				// 3, 5 : discard part of end domain
				if( offset.Max < domainOffset.Max )
				{
				}
				// 2, 4 : discard part of end this
				else //if( offset.Max > domainOffset.Max )
				{
					intersects	= true;
				}
			}
		
			return intersects;
		}
		
		/// <summary>
		/// When doing a difference or an intersection with a non continues
		/// domain, the min/max bound must be check for any uncovered 'holes'.
		/// 
		/// </summary>
		/// <param name="updateMin"> update min bound</param>
		/// <param name="updateMax"> update max bound</param>
		private void UpdateInterval( bool updateMin, bool updateMax )
		{
			bool empty	= false;

			int min		= m_Interval.Min;
			int max		= m_Interval.Max;

			// check if min bound changes (left overlap)
			if( updateMin )
			{
				min		= FindNextOne( min );
				empty	= min > max;
			}
			
			// check if max bound changes (right overlap)
			if( updateMax && !empty )
			{
				max		= FindPrevOne( max );
				empty	= min > max;
			}
			
			if( empty )
			{
				SetEmpty();
			}
			else
			{
				m_Interval	= new IntInterval( min, max );

				CheckSimple();
			}		
		}

		private void CheckSimple()
		{
			if( IsAllOne( m_Interval ) )
			{
				m_BitArray	= null;
				m_Offset	= 0;
			}
		}

		private void SetEmpty()
		{
			m_Interval	= IntInterval.Empty;
			m_BitArray	= null;
			m_Offset	= 0;			
		}

		private bool Get( int index )
		{
			if( !m_Interval.Contains( index ) )
				throw new IndexOutOfRangeException();

			int offset	= ToOffset( index ) - m_Offset;
			int mod		= ToMod( index );

			UInt32 mask	= (UInt32) 1 << mod;

			return ( m_BitArray[ offset ] & mask ) != 0;
		}

		private void Set( int index, bool value )
		{
			if( !m_Interval.Contains( index ) )
				throw new IndexOutOfRangeException();

			int offset	= ToOffset( index ) - m_Offset;
			int mod		= ToMod( index );
			
			UInt32 mask	= (UInt32) 1 << mod;

			m_BitArray[ offset ]	&= ~mask;
			
			if( value )
			{
				m_BitArray[ offset ]	|= mask;
			}
		}

		private void Clear( int index )
		{
			int offset	= ToOffset( index ) - m_Offset;
			int mod		= ToMod( index );
			
			UInt32 mask	= (UInt32) 1 << mod;

			m_BitArray[ offset ]	&= ~mask;
		}

		private void Set( int index )
		{
			int offset	= ToOffset( index ) - m_Offset;
			int mod		= ToMod( index );
			
			UInt32 mask	= (UInt32) 1 << mod;

			m_BitArray[ offset ]	|= mask;
		}

		static bool CompareRange( UInt32[] srcBitArray1, int srcOffset1,
									 UInt32[] srcBitArray2, int srcOffset2,
									 int length )
		{
			bool equal	= true;

			unsafe
			{
				fixed( UInt32* pSrc1 = srcBitArray1, pSrc2 = srcBitArray2 )
				{
					UInt32* src1 = pSrc1 + srcOffset1;
					UInt32* src2 = pSrc2 + srcOffset2;

					int idx	= 0;
					while( idx < length && ( *src1++ == *src2++ ) )
					{
						++idx;
					}
					
					equal	= ( idx == length );
				}
			}
			
			return equal;
		}

		static void CopyRange( UInt32[] srcBitArray, int srcOffset,
								 UInt32[] dstBitArray, int dstOffset,
								 int length )
		{
			if( srcOffset + length > srcBitArray.Length
					|| dstOffset + length > dstBitArray.Length )
				throw new ArgumentOutOfRangeException();
		
			unsafe
			{
				fixed( UInt32* pSrc = srcBitArray, pDst = dstBitArray )
				{
					UInt32* src		= pSrc + srcOffset;
					UInt32* dst		= pDst + dstOffset;

					for( int idx = 0; idx < length; ++idx )
					{
						*dst++	= *src++;
					}
				}
			}
		}
		
		static bool UnionRange( UInt32[] srcBitArray1, int srcOffset1,
								UInt32[] srcBitArray2, int srcOffset2,
								UInt32[] dstBitArray, int dstOffset,
								int length )
		{
			bool changed	= false;

			unsafe
			{
				fixed( UInt32* pSrc1 = srcBitArray1, pSrc2 = srcBitArray2, pDst = dstBitArray )
				{
					UInt32* src1	= pSrc1 + srcOffset1;
					UInt32* src2	= pSrc2 + srcOffset2;
					UInt32* dst		= pDst + dstOffset;

					for( int idx = 0; idx < length; ++idx )
					{
						*dst		= *src1 | *src2;
						
						changed		|= *src1 != *dst;
						
						++src1;
						++src2;
						++dst;
					}
				}
			}

			return changed;
		}

		static bool DifferenceRange( UInt32[] srcBitArray1, int srcOffset1,
										UInt32[] srcBitArray2, int srcOffset2,
										UInt32[] dstBitArray, int dstOffset,
										int length )
		{
			bool changed	= false;

			unsafe
			{
				fixed( UInt32* pSrc1 = srcBitArray1, pSrc2 = srcBitArray2, pDst = dstBitArray )
				{
					UInt32* src1	= pSrc1 + srcOffset1;
					UInt32* src2	= pSrc2 + srcOffset2;
					UInt32* dst		= pDst + dstOffset;

					for( int idx = 0; idx < length; ++idx )
					{
						*dst		= *src1 & ~*src2;
						
						changed		|= *src1 != *dst;
						
						++src1;
						++src2;
						++dst;
					}
				}
			}
			
			return changed;
		}

		static bool IntersectRange( UInt32[] srcBitArray1, int srcOffset1,
										UInt32[] srcBitArray2, int srcOffset2,
										UInt32[] dstBitArray, int dstOffset,
										int length )
		{
			bool changed	= false;

			unsafe
			{
				fixed( UInt32* pSrc1 = srcBitArray1, pSrc2 = srcBitArray2, pDst = dstBitArray )
				{
					UInt32* src1	= pSrc1 + srcOffset1;
					UInt32* src2	= pSrc2 + srcOffset2;
					UInt32* dst		= pDst + dstOffset;

					for( int idx = 0; idx < length; ++idx )
					{
						*dst		= *src1 & *src2;
						
						changed		|= *src1 != *dst;
						
						++src1;
						++src2;
						++dst;
					}
				}
			}
			
			return changed;
		}

		static bool IntersectsWithRange( UInt32[] srcBitArray1, int srcOffset1,
										UInt32[] srcBitArray2, int srcOffset2,
										int length )
		{
			bool intersects	= false;

			unsafe
			{
				fixed( UInt32* pSrc1 = srcBitArray1, pSrc2 = srcBitArray2 )
				{
					UInt32* src1	= pSrc1 + srcOffset1;
					UInt32* src2	= pSrc2 + srcOffset2;

					for( int idx = 0; idx < length && !intersects; ++idx )
					{
						UInt32 s1	= *src1++;
						UInt32 s2	= *src2++;

						intersects	|= ( s1 & s2 ) != 0;
					}
				}
			}
			
			return intersects;
		}

		// 11111111.11110000
		//               <=x
		private int FindNextOne( int idx )
		{
			int min		= idx;
			int max		= m_Interval.Max;
		
			if( min <= max )
			{
				int offset	= ToOffset( min ) - m_Offset;

				UInt32 maskBegin	= ToMaskBegin( min );
				if( ( m_BitArray[ offset ] & maskBegin ) == 0x0 )
				{
					++offset;
					
					while( offset < m_BitArray.Length
							&& m_BitArray[ offset ] == 0x0 )
					{
						++offset;
					}
					
					min		= ( offset + m_Offset ) << 5;

					if( offset < m_BitArray.Length )
					{
						min		+= Binary.LowestBitIdx( m_BitArray[ offset ] );
					}
				}
				else
				{
					int mod			= ToMod( min );

					UInt32 val		= m_BitArray[ offset ];
					UInt32 mask		= (UInt32) 1 << mod;

					while( ( val & mask ) == 0 )
					{
						mask	<<= 1;
						++min;
					}
				}
			}
			
			return min;
		}

		// 00001111.11110000
		// x =>
		private int FindPrevOne( int idx )
		{
			int min		= m_Interval.Min;
			int max		= idx;
		
			if( min <= max )
			{
				int offset	= ToOffset( max ) - m_Offset;

				UInt32 maskEnd	= ToMaskEnd( max );
				if( ( m_BitArray[ offset ] & maskEnd ) == 0x0 )
				{
					--offset;
					
					while( offset >= 0
							&& m_BitArray[ offset ] == 0x0 )
					{
						--offset;
					}
					
					max		= ( offset + m_Offset ) << 5;

					if( offset >= 0 )
					{
						max		+= Binary.HighestBitIdx( m_BitArray[ offset ] );
					}
				}
				else
				{
					int mod			= ToMod( max );

					UInt32 val		= m_BitArray[ offset ];
					UInt32 mask		= (UInt32) 1 << mod;

					while( ( val & mask ) == 0 )
					{
						mask	>>= 1;
						--max;
					}
				}
			}
			
			return max;
		}

		// 00111111.111100
		//   x =>
		private int FindPrevZero( int idx )
		{
			int min		= idx;
			int max		= m_Interval.Max;

			if( min <= max )
			{
				int offset	= ToOffset( min ) - m_Offset;
				
				UInt32 maskBegin	= ToMaskBegin( min );
				if( ( m_BitArray[ offset ] & maskBegin ) == maskBegin )
				{
					++offset;
					
					while( offset < m_BitArray.Length
							&& m_BitArray[ offset ] == 0xFFFFFFFF )
					{
						++offset;
					}

					min		= ( offset + m_Offset ) << 5;
					if( min <= max )
					{
						min		+= Binary.LowestZeroIdx( m_BitArray[ offset ] );
					}
				}
				else
				{
					int mod			= ToMod( min );

					UInt32 val		= m_BitArray[ offset ];
					UInt32 mask		= (UInt32) 1 << mod;

					while( ( val & mask ) != 0 )
					{
						mask	<<= 1;
						++min;
					}
				}
			}
			
			return min;
		}

		private void SetOne( IntInterval interval )
		{
			SetOne( interval.Min, interval.Max );
		}
		
		private void SetOne( int min, int max )
		{
			int offsetMin	= ToOffset( min ) - m_Offset;
			int offsetMax	= ToOffset( max ) - m_Offset;
			
			if( offsetMin == offsetMax )
			{
				m_BitArray[ offsetMin ]		|= ToMask( min, max );
			}
			else
			{
				m_BitArray[ offsetMin ]		|= ToMaskBegin( min );
				m_BitArray[ offsetMax ]		|= ToMaskEnd( max );

				for( int idx = offsetMin + 1; idx <= offsetMax - 1; ++idx )
				{
					m_BitArray[ idx ]		= 0xFFFFFFFF;
				}
			}
		}
		
		private bool IsAllOne( IntInterval interval )
		{
			int offsetMin	= ToOffset( interval.Min ) - m_Offset;
			int offsetMax	= ToOffset( interval.Max ) - m_Offset;
			
			if( offsetMin == offsetMax )
			{
				UInt32 mask			= ToMask( interval.Min, interval.Max );
			
				return ( m_BitArray[ offsetMin ] & mask ) == mask;
			}
			else
			{
				const UInt32 allOne	= 0xFFFFFFFF;
			
				for( int idx = offsetMin + 1; idx <= offsetMax - 1; ++idx )
				{
					if( m_BitArray[ idx ] != allOne )
						return false;
				}

				UInt32 maskBegin	= ToMaskBegin( interval.Min );
				
				if( ( m_BitArray[ offsetMin ] & maskBegin ) != maskBegin )
					return false;

				UInt32 maskEnd		= ToMaskEnd( interval.Max );

				if( ( m_BitArray[ offsetMax ] & maskEnd ) != maskEnd )
					return false;

			}

			return true;
		}

		private bool IsAllZero( IntInterval interval )
		{
			int offsetMin	= ToOffset( interval.Min ) - m_Offset;
			int offsetMax	= ToOffset( interval.Max ) - m_Offset;
			
			if( offsetMin == offsetMax )
			{
				UInt32 mask			= ToMask( interval.Min, interval.Max );
			
				return ( m_BitArray[ offsetMin ] & mask ) == 0x0;
			}
			else
			{
				for( int idx = offsetMin + 1; idx <= offsetMax - 1; ++idx )
				{
					if( m_BitArray[ idx ] != 0x0 )
						return false;
				}

				UInt32 maskBegin	= ToMaskBegin( interval.Min );

				if( ( m_BitArray[ offsetMin ] & maskBegin ) != 0x0 )
					return false;

				UInt32 maskEnd		= ToMaskEnd( interval.Max );

				if( ( m_BitArray[ offsetMax ] & maskEnd ) != 0x0 )
					return false;

			}

			return true;
		}

		private static int ToLength( IntInterval interval )
		{
			int len		= ToOffset( interval.Max ) - ToOffset( interval.Min ) + 1;
			if( len > m_MaxCardinality )
				throw new ArgumentException();

			return len;
		}

		private static IntInterval ToOffset( IntInterval interval )
		{
			return new IntInterval( ToOffset( interval.Min ), ToOffset( interval.Max ) );
		}
		
		private static int ToOffset( int offset )
		{
			return offset >> 5;
		}

		private static int ToMod( int offset )
		{
			return offset & 0x1F;
		}

		private static UInt32 ToMaskBegin( int min )
		{
			return (~0U) << ( min & 0x1F );
		}

		private static UInt32 ToMaskEnd( int max )
		{
			return (~0U) >> ( 31 - ( max & 0x1F ) );
		}

		// return ToMaskBegin( min ) & ToMaskEnd( max );
		private static UInt32 ToMask( int min, int max )
		{
			return ( (~0U) << ( min & 0x1F ) ) & ( (~0U) >> ( 31 - ( max & 0x1F ) ) );
		}


		#region Add/Subtract/Multiply/Divide Value

		public IntDomain Add( int value )
		{
			if( value == 0 )
				return this;
			
			return new IntDomain( m_Interval + value );
		}

		public IntDomain Subtract( int value )
		{
			if( value == 0 )
				return this;
			
			return new IntDomain( m_Interval - value );
		}

		public IntDomain Multiply( int value )
		{
			if( value == 1 )
				return this;

			return new IntDomain( m_Interval * value );
		}

		public IntDomain Divide( int value )
		{
			if( value == 1 )
				return this;

			return new IntDomain( m_Interval - value );
		}

		#endregion

		#region Domain (+,-,*,/) Value

		// v0 = v1 + value
		public static IntDomain operator +( IntDomain lhs, int rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - value
		public static IntDomain operator -( IntDomain lhs, int rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * value
		public static IntDomain operator *( IntDomain lhs, int rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / value
		public static IntDomain operator /( IntDomain lhs, int rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Domain

		public IntDomain Add( IntDomain domain )
		{
			IntInterval interval = domain.Interval;

			if( interval.Min == 0 && interval.Max == 0 )
				return this;

			return new IntDomain( m_Interval + interval );
		}

		public IntDomain Subtract( IntDomain domain )
		{
			IntInterval interval = domain.Interval;

			if( interval.Min == 0 && interval.Max == 0 )
				return this;
				
			return new IntDomain( m_Interval - interval );
		}

		public IntDomain Multiply( IntDomain domain )
		{
			IntInterval interval = domain.Interval;

			if( interval.Min == 1 && interval.Max == 1 )
				return this;

			return new IntDomain( m_Interval * interval );
		}

		public IntDomain Divide( IntDomain domain )
		{
			IntInterval interval = domain.Interval;

			if( interval.Min == 1 && interval.Max == 1 )
				return this;

			return new IntDomain( m_Interval / interval );
		}

		#endregion

		#region Domain (+,-,*,/) Domain

		// v0 = v1 + v2
		public static IntDomain operator +( IntDomain lhs, IntDomain rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - v2
		public static IntDomain operator -( IntDomain lhs, IntDomain rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * v2
		public static IntDomain operator *( IntDomain lhs, IntDomain rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / v2
		public static IntDomain operator /( IntDomain lhs, IntDomain rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		#region Sqrt(), Square(), Pow(), Abs()

		public IntDomain Sqrt()
		{
			if( IsEmpty() || m_Interval.Max < 0 )
				return m_Empty;

			if( ( m_Interval.Min == 0 && m_Interval.Max == 0 ) || ( m_Interval.Min == 1 && m_Interval.Max == 1 ) )
				return this;

			IntDomain result	= new IntDomain();
			result.m_Interval	= m_Interval.Sqrt();
			
			if( IsSimple() )
				return result;
			
			result.m_BitArray	= new UInt32[ ToLength( result.m_Interval ) ];
			result.m_Offset		= ToOffset( result.m_Interval.Min );
			
			foreach( IntInterval interval in this )
			{
				int min		= ( interval.Min > 0 )
							? (int) Math.Floor( Math.Sqrt( interval.Min ) )
							: 0;
				int max		= (int) Math.Ceiling( Math.Sqrt( interval.Max ) );
				
				result.SetOne( min, max );
			}

			return result;
		}

		public IntDomain Square()
		{
			if( IsEmpty() )
				return m_Empty;

			if( ( m_Interval.Min == 0 && m_Interval.Max == 0 ) || ( m_Interval.Min == 1 && m_Interval.Max == 1 ) )
				return this;

			IntDomain result	= new IntDomain();
			result.m_Interval	= m_Interval.Square();

			if( IsSimple() || result.m_Interval.Cardinality > m_MaxCardinality )
				return result;

			result.m_BitArray	= new UInt32[ ToLength( result.m_Interval ) ];
			result.m_Offset		= ToOffset( result.m_Interval.Min );
			
			foreach( IntInterval interval in this )
			{
				result.SetOne( interval.Square() );
			}
			
			return result;
		}

		public IntDomain Pow( int power )
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.IsZero() || m_Interval.IsOne() )
				return this;

			IntDomain result	= new IntDomain();
			result.m_Interval	= m_Interval.Pow( power );

			if( IsSimple() )
				return result;

			if( IsSimple() || result.m_Interval.Cardinality > m_MaxCardinality )
				return result;

			result.m_BitArray	= new UInt32[ ToLength( result.m_Interval ) ];
			result.m_Offset		= ToOffset( result.m_Interval.Min );
			
			foreach( IntInterval interval in this )
			{
				result.SetOne( interval.Pow( power ) );
			}
			
			return result;
		}

		public IntDomain Abs()
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.Max >= 0 )
				return this;

			if( m_Interval.Max <= 0 )
				return Negate();
			
			IntDomain lower		= -Intersect( int.MinValue, 0 );
			IntDomain upper		= Intersect( 0, int.MaxValue );

			return lower.Union( upper );
		}

		#endregion

		#region Negate

		// v0 = -v1
		public static IntDomain operator -( IntDomain domain )
		{
			return domain.Negate();
		}

		public IntDomain Negate()
		{
			if( m_Interval.Min == 0 && m_Interval.Max == 0 )
				return this;
			
			IntDomain result	= new IntDomain();
			result.m_Interval	= -m_Interval;

			if( IsSimple() )
				return result;

			result.m_BitArray	= new UInt32[ ToLength( result.m_Interval ) ];
			result.m_Offset		= ToOffset( result.m_Interval.Min );
			
			foreach( IntInterval interval in this )
			{
				result.SetOne( -interval );
			}
			
			return result;			
		}

		#endregion

		#region Random

		public static IntDomain Random( int min, int max )
		{
			return new IntDomain( IntInterval.Random( min, max ) );		
		}

		public static IntDomain Random( int min, int max, int count )
		{
			return new IntDomain( IntInterval.Random( min, max ) );		
		}

		#endregion

		#region IntervalEnumerator
		
		// Inner class implements IEnumerator interface:
		private sealed class BitArrayIntervalEnumerator : IEnumerator<IntInterval>
		{
			IntInterval		m_Interval;		// current interval

			IntDomain		m_Domain;
			int				m_Offset;		// current min bound

			public BitArrayIntervalEnumerator( IntDomain domain )
			{
				m_Domain	= domain;

				Reset();
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				bool moveNext	= !m_Domain.m_Interval.IsEmpty()
										&& m_Offset <= m_Domain.m_Interval.Max;
				if( moveNext )
				{
					int min		= m_Offset;
					int max		= m_Domain.FindPrevZero( m_Offset );
					
					m_Offset	= m_Domain.FindNextOne( max );
					
					m_Interval	= new IntInterval( min, max - 1);
				}
				
				return moveNext;
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				m_Offset	= m_Domain.m_Interval.Min;
				m_Interval	= null;
			}

			#region IEnumerator<IntInterval> Members

			public IntInterval Current
			{
				get
				{
					return m_Interval;
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
					return m_Interval;
				}
			}

			#endregion
		}

		#endregion

		#region IntDomainOneEnumerator

		// Inner class implements IEnumerator interface:
		private sealed class SimpleIntervalEnumerator : IEnumerator<IntInterval>
		{
			bool			m_MoveNext;
			IntDomain		m_Domain;

			public SimpleIntervalEnumerator( IntDomain domain )
			{
				m_MoveNext	= true;
				m_Domain	= domain;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				bool moveNext	= m_MoveNext;
				
				m_MoveNext	= false;
				
				return moveNext;
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				m_MoveNext	= true;
			}

			#region IEnumerator<IntInterval> Members

			public IntInterval Current
			{
				get
				{
					return m_Domain.m_Interval;
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
					return m_Domain.m_Interval;
				}
			}

			#endregion
		}

		#endregion

		#region IEnumerable<IntInterval> Members

		public IEnumerator<IntInterval> GetEnumerator()
		{
			if( IsSimple() )
				return new SimpleIntervalEnumerator( this );
			
			return new BitArrayIntervalEnumerator( this );
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<IntInterval> Members

		IEnumerator<IntInterval> IEnumerable<IntInterval>.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new IntDomain( this );
		}

		#endregion

		static int m_MaxCardinality	= 1024;

		public static int MaxCardinality
		{
			get
			{
				return m_MaxCardinality;
			}
			set
			{
				m_MaxCardinality	= value;
			}
		}

		IntInterval		m_Interval;
		UInt32[]		m_BitArray;
		int				m_Offset;

		static readonly IntDomain m_Empty	= new IntDomain();
		static readonly Random m_Random		= new Random( 0 );
	}
}

//--------------------------------------------------------------------------------
