//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Int/IntDomain2.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 5     17-11-07 16:07 Patrick
 * renamed to IntDomain2 (think of better name later)
 * 
 * 4     14-11-07 22:39 Patrick
 * fixed GetHashCode()
 * 
 * 3     25-07-07 3:55 Patrick
 * added readonl
 * 
 * 2     27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 1     21-06-07 22:35 Patrick
 * added class
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
	public sealed class IntDomain2 : IIntInterval, IEnumerable<IntInterval>
	{
		public IntDomain2()
		{
			SetEmpty();
		}

		public IntDomain2( int val ) :
			this( val, val )
		{
		}

		public IntDomain2( int min, int max ) :
			this( new IntInterval( min, max ) )
		{
		}

		public IntDomain2( IntInterval interval )
		{
			m_Interval		= interval;
			m_IntervalList	= null;
		}

		public IntDomain2( IntDomain2 domain )
		{
			m_Interval		= domain.m_Interval;
			m_IntervalList	= domain.m_IntervalList;
		}

		public IntDomain2( int[] list )
		{
			if( list.Length % 2 == 0 )
			{
				m_Interval		= new IntInterval( list[ 0 ], list[ list.Length - 1 ] );
				m_IntervalList	= null;
				
				int count	= list.Length / 2;
				if( count > 1 )
				{
					m_IntervalList	= new List<IntInterval>( count );

					for( int idx = 0; idx < count; ++idx )
					{
						m_IntervalList.Add( new IntInterval( list[ idx * 2 ], list[ idx * 2 + 1 ] ) );
					}
				}
			}
		}

		public IntDomain2( IEnumerable<IntInterval> list )
		{
			IEnumerator<IntInterval> it		= list.GetEnumerator();
			if( it.MoveNext() )
			{
				int min	= it.Current.Min;
				int max	= it.Current.Max;
				int count	= 1;
				
				while( it.MoveNext() )
				{
					min		= Math.Min( min, it.Current.Min );
					max		= Math.Max( max, it.Current.Max );
					++count;
				}
				
				m_Interval		= new IntInterval( min, max );
				
				if( count > 1 )
				{
					m_IntervalList	=  new List<IntInterval>( list );
					m_IntervalList.Sort( new IntInterval.Comparer() );
				}
				else
				{
					m_IntervalList	= null;
				}
			}
			else
			{
				SetEmpty();
			}
		}
		
		private IntDomain2( IntInterval intv1, IntInterval intv2 )
		{
			m_Interval		= new IntInterval( intv1.Min, intv2.Max );
			m_IntervalList	= new List<IntInterval>( 2 );
			m_IntervalList.Add( intv1 );
			m_IntervalList.Add( intv2 );
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

			IntDomain2 domain	= obj as IntDomain2;
			if( !ReferenceEquals( domain, null ) )
				return Equals( domain );
		
			return false;
		}

		public bool Equals( IntDomain2 domain )
		{
			if( ReferenceEquals( domain, this ) )
				return true;

			if( IsSimple() && domain.IsSimple() )
				return m_Interval.Equals( domain.m_Interval );

			if( IsSimple() ^ domain.IsSimple() )
				return false;

			bool equal	= m_IntervalList.Count == domain.m_IntervalList.Count;
			if( equal )
			{
				for( int idx = 0; idx < m_IntervalList.Count && equal; ++idx )
				{
					equal	&= m_IntervalList[ idx ].Equals( domain.m_IntervalList[ idx ] );
				}
			}
			
			return equal;
		}

		public override int GetHashCode()
		{
			return m_Interval.Max << 16 + m_Interval.Min;
		}

		public static IntDomain2 Empty
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
				{
					return m_Interval.Cardinality;
				}
				else
				{
					int card	= 0;
					
					foreach( IntInterval interval in m_IntervalList )
					{
						card	+= interval.Cardinality;
					}
					
					return card;
				}
			}
		}

		public IntInterval[] ToArray()
		{
			if( IsSimple() )
				return new IntInterval[] { m_Interval };
			
			return m_IntervalList.ToArray();
		}

		public bool IsSimple()
		{
			return ReferenceEquals( m_IntervalList, null );
		}

		public bool IsEmpty()
		{
			return m_Interval.IsEmpty();
		}

		public bool Contains( int val )
		{
			if( IsSimple() )
				return m_Interval.Contains( val );

			bool contains	= false;

			IntInterval interval	= new IntInterval( val, val );

			int index		= m_IntervalList.BinarySearch( interval, new IntInterval.Comparer() );
			if( index < 0 )
			{
				index	= ~index;
				
				if( index > 0 )
				{
					contains	= m_IntervalList[ index - 1 ].Contains( val );
				}
			}
			else
			{
				contains	= m_IntervalList[ index ].Contains( val );
			}
			
			return contains;	
		}

		public bool Contains( IntInterval interval )
		{
			if( IsSimple() )
				return m_Interval.Contains( interval );

			bool contains	= false;

			int index		= m_IntervalList.BinarySearch( interval, new IntInterval.Comparer() );
			if( index < 0 )
			{
				index	= ~index;
				
				contains	= ( index > 0 )
							? m_IntervalList[ index - 1 ].Contains( interval )
							: m_IntervalList[ 0 ].Contains( interval );
			}
			else
			{
				contains	= true;
			}
			
			return contains;	
		}

		#region Union
			
		public IntDomain2 Union( int val )
		{
			return Union( val, val );
		}

		public IntDomain2 Union( int min, int max )
		{
			return Union( new IntInterval( min, max ) );
		}

		public IntDomain2 Union( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return new IntDomain2( interval );

			// ~4
			if( Contains( interval ) )
				return this;

			// 3
			if( interval.Contains( m_Interval ) )
				return new IntDomain2( interval );

			if( IsSimple() )
			{
				// 1
				if( interval.Max < m_Interval.Min -1 )
					return new IntDomain2( interval, m_Interval );

				// 6
				if( interval.Min > m_Interval.Max +1 )
					return new IntDomain2( m_Interval, interval );
							
				// 2, 5
				return new IntDomain2( m_Interval.Union( interval ) );
			}
			else
			{
				IntDomain2 result		= new IntDomain2();
				result.m_Interval		= m_Interval.Union( interval );
				result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count + 1 );

				// 1
				if( interval.Max < m_Interval.Min - 1 )
				{
					result.m_IntervalList.Add( interval );
					result.m_IntervalList.AddRange( m_IntervalList );
				}

				// 6
				else if( interval.Min > m_Interval.Max +1 )
				{
					result.m_IntervalList.AddRange( m_IntervalList );
					result.m_IntervalList.Add( interval );
				}
				else
				{
					// 6
					int idx	= 0;
					for( ;idx < m_IntervalList.Count && m_IntervalList[ idx ].Max < interval.Min - 1; ++idx )
					{
						result.m_IntervalList.Add( m_IntervalList[ idx ] );
					}
					
					if( idx < m_IntervalList.Count )
					{
						IntInterval intv	= interval;
						
						// adjacent | intersect
						if( idx < m_IntervalList.Count
										&& IntersectsWithOrIsAdjacent( m_IntervalList[ idx ], intv ) )
						{
							intv	= intv.Union( m_IntervalList[ idx ] );
							++idx;
						}
	
						// 3
						for( ;idx < m_IntervalList.Count && intv.Contains( m_IntervalList[ idx ] ); ++idx ) {}

						// adjacent | intersect
						if( idx < m_IntervalList.Count
										&& IntersectsWithOrIsAdjacent( m_IntervalList[ idx ], intv ) )
						{
							intv	= intv.Union( m_IntervalList[ idx ] );
							++idx;
						}
					
						result.m_IntervalList.Add( intv );

						// 1
						for( ;idx < m_IntervalList.Count; ++idx )
						{
							result.m_IntervalList.Add( m_IntervalList[ idx ] );
						}
					}
					else
					{
						// 6
						result.m_IntervalList.Add( interval );
					}
				}
				
				result.CheckSimple();
				
				return result;
			}
		}
		
		public IntDomain2 Union( IntDomain2 domain )
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
			else
			{
				IntDomain2 result		= new IntDomain2();
				result.m_Interval		= m_Interval.Union( domain.m_Interval );
				result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count + domain.m_IntervalList.Count );

				// 1 : completely before
				if( domain.m_Interval.Max < m_Interval.Min -1 )
				{
					result.m_IntervalList.AddRange( domain.m_IntervalList );
					result.m_IntervalList.AddRange( m_IntervalList );
				}

				// 6 : completely after
				else if( domain.m_Interval.Min > m_Interval.Max +1 )
				{
					result.m_IntervalList.AddRange( m_IntervalList );
					result.m_IntervalList.AddRange( domain.m_IntervalList );
				}
				else
				{
					int idx		= 0;
					int domIdx	= 0;
					bool equal	= true;

					while( idx < m_IntervalList.Count
								|| domIdx < domain.m_IntervalList.Count )
					{
						// 6 : 
						if( idx == m_IntervalList.Count )
						{
							equal	= false;

							for( ;domIdx < domain.m_IntervalList.Count; ++domIdx )
							{
								result.m_IntervalList.Add( domain.m_IntervalList[ domIdx ] );
							}
						}

						// 1 : remaining intervals after
						else if( domIdx == domain.m_IntervalList.Count )
						{
							for( ;idx < m_IntervalList.Count; ++idx )
							{
								result.m_IntervalList.Add( m_IntervalList[ idx ] );
							}
						}
						else
						{
							IntInterval intv		= m_IntervalList[ idx ];
							IntInterval domIntv		= domain.m_IntervalList[ domIdx ];

							// 1 : completely before
							if( domIntv.Max < intv.Min -1 )
							{
								equal	= false;

								result.m_IntervalList.Add( domIntv );
								++domIdx;
							}
							// 6 : completely after
							else if( domIntv.Min > intv.Max +1 )
							{
								result.m_IntervalList.Add( intv );
								++idx;
							}
							// 3
							else if( domIntv.Min < intv.Min && domIntv.Max > intv.Max )
							{
								equal	= false;

								++idx;
							}
							// 4
							else if( domIntv.Min >= intv.Min && domIntv.Max <= intv.Max )
							{
								++domIdx;
							}
							// 2, 5
							else
							{
								equal	= false;
								
								IntInterval intvTmp		= intv.Union( domIntv );
								
								++idx;

								// contains...
								for( ;idx < m_IntervalList.Count
										&& intvTmp.Contains( m_IntervalList[ idx ] );
										++idx ) {};

								// adjacent | intersect...
								while( idx < m_IntervalList.Count
												&& IntersectsWithOrIsAdjacent( m_IntervalList[ idx ], intvTmp ) )
								{
									intvTmp		= intvTmp.Union( m_IntervalList[ idx ] );
									++idx;
								}
								
								result.m_IntervalList.Add( intvTmp );

								++domIdx;
							}
						}				
					}

					if( equal )
						return this;
				
				
					result.CheckSimple();
				}
				
				return result;
			}
		}

		#endregion

		#region Difference

		public IntDomain2 Difference( int val )
		{
			return Difference( val, val );
		}

		public IntDomain2 Difference( int min, int max )
		{
			return Difference( new IntInterval( min, max ) );
		}
	
		public IntDomain2 Difference( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			// 1, 6
			if( !IntersectsWith( interval ) )
				return this;

			// 3 : completely remove interval => empty
			if( interval.Min <= m_Interval.Min && interval.Max >= m_Interval.Max )
				return m_Empty;

			// Result domain
			IntDomain2 result		= new IntDomain2();
			result.m_Interval		= m_Interval.Difference( interval );

			if( IsSimple() )
			{
				// 4 : divide into two intervals...
				if( interval.Min > m_Interval.Min && interval.Max < m_Interval.Max )
				{
					result.m_IntervalList	= new List<IntInterval>( 2 );
					result.m_IntervalList.Add( new IntInterval( m_Interval.Min, interval.Min -1 ) );
					result.m_IntervalList.Add( new IntInterval( interval.Max +1, m_Interval.Max ) );
				}
			}
			else
			{
				result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count + 1 );

				int idx	= 0;

				// 6
				for( ;idx < m_IntervalList.Count && m_IntervalList[ idx ].Max < interval.Min; ++idx )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ] );
				}
			
				// 2
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min < interval.Min
						&& m_IntervalList[ idx ].Max <= interval.Max )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Difference( interval ) );
					++idx;
				}

				// 4 : divide into two intervals...
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min < interval.Min 
						&& m_IntervalList[ idx ].Max > interval.Max )
				{
					result.m_IntervalList.Add( new IntInterval( m_Interval.Min, interval.Min -1 ) );
					result.m_IntervalList.Add( new IntInterval( interval.Max +1, m_Interval.Max ) );
				}
				// 3
				else
				{
					for( ;idx < m_IntervalList.Count && interval.Contains( m_IntervalList[ idx ] ); ++idx ) {}
				}				

				// 5
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min >= interval.Min
						&& m_IntervalList[ idx ].Max > interval.Max )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Difference( interval ) );
					++idx;
				}

				// 1
				for( ;idx < m_IntervalList.Count; ++idx ) 
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ] );
				}

				result.CheckSimple();
			}
			
			return result;
		}

		public IntDomain2 Difference( IntDomain2 domain )
		{
			if( ReferenceEquals( domain, this ) )
				return m_Empty;

			if( domain.IsEmpty() )
				return this;

			if( domain.IsSimple() )
				return Difference( domain.m_Interval );

			//1, 6
			if( !IntersectsWith( domain.m_Interval ) )
				return this;

			// Need to be a bit smart, put our interval in a list, in case it gets chopped up.
			List<IntInterval> intervalList;
			if( IsSimple() )
			{
				intervalList	= new List<IntInterval>( 1 );
				intervalList.Add( m_Interval );
			}
			else
			{
				intervalList	= m_IntervalList;
			}

			IntDomain2 result		= new IntDomain2();
			result.m_IntervalList	= new List<IntInterval>( intervalList.Count + domain.m_IntervalList.Count );

			int idx		= 0;
			int domIdx	= 0;

			while( idx < intervalList.Count
						|| domIdx < domain.m_IntervalList.Count )
			{
				// 6 
				if( idx == intervalList.Count )
				{
					domIdx	= domain.m_IntervalList.Count;
				}
				// 1
				else if( domIdx == domain.m_IntervalList.Count )
				{
					idx		= intervalList.Count;
				}
				else
				{
					IntInterval intv		= intervalList[ idx ];
					IntInterval domIntv		= domain.m_IntervalList[ domIdx ];

					// 1 : completely before
					if( domIntv.Max < intv.Min -1 )
					{
						++domIdx;
					}
					// 6 : completely after
					else if( domIntv.Min > intv.Max +1 )
					{
						result.m_IntervalList.Add( intv );
						++idx;
					}
					// 3
					else if( domIntv.Min < intv.Min && domIntv.Max > intv.Max )
					{
						++idx;
					}
					// 4 : divide into two intervals...
					else if( domIntv.Min > intv.Min && domIntv.Max < intv.Max )
					{
						while( domIdx < domain.m_IntervalList.Count
									&& domIntv.Min > intv.Min
									&& domIntv.Max < intv.Max )
						{
							result.m_IntervalList.Add( new IntInterval( intv.Min, domIntv.Min -1 ) );

							intv		= new IntInterval( domIntv.Max +1, intv.Max );

							++domIdx;

							if( domIdx < domain.m_IntervalList.Count )
							{
								domIntv		= domain.m_IntervalList[ domIdx ];
							}
						}

						result.m_IntervalList.Add( intv );
					}
					// 2
					else if( domIntv.Min <= intv.Min && domIntv.Max < intv.Max )
					{
						result.m_IntervalList.Add( intv.Difference( domIntv ) );

						++domIdx;
					}
					// 5
					else if( domIntv.Min > intv.Min && domIntv.Max >= intv.Max )
					{
						result.m_IntervalList.Add( intv.Difference( domIntv ) );

						++idx;
					}
				}
			}

			result.UpdateInterval();
			
			return result;		
		}

		#endregion

		#region Intersect

		public IntDomain2 Intersect( int val )
		{
			return Intersect( val, val );		
		}

		public IntDomain2 Intersect( int min, int max )
		{
			return Intersect( new IntInterval( min, max ) );
		}

		public IntDomain2 Intersect( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return m_Empty;

			// 1, 6
			if( !IntersectsWith( interval ) )
				return m_Empty;

			// 3
			if( m_Interval.Min >= interval.Min && m_Interval.Max <= interval.Max )
				return this;

			IntInterval intv	= m_Interval.Intersect( interval );

			IntDomain2 result	= new IntDomain2();
			result.m_Interval	= intv;

			if( !IsSimple() )
			{
				result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count );

				int idx	= 0;

				// 6 : completely before
				for( ;idx < m_IntervalList.Count && m_IntervalList[ idx ].Max < interval.Min; ++idx ) {};

				// 2
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min < interval.Min
						&& m_IntervalList[ idx ].Max <= interval.Max )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Intersect( interval ) );
					++idx;
				}

				// 4
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min < interval.Min 
						&& m_IntervalList[ idx ].Max > interval.Max )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Intersect( interval ) );
				}
				// 3
				else
				{
					for( ;idx < m_IntervalList.Count && interval.Contains( m_IntervalList[ idx ] ); ++idx )
					{
						result.m_IntervalList.Add( m_IntervalList[ idx ] );
					}
				}				

				// 5
				if( idx < m_IntervalList.Count
						&& m_IntervalList[ idx ].Min > interval.Min
						&& m_IntervalList[ idx ].Max >= interval.Max )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Intersect( interval ) );
					++idx;
				}

				// 1 : complete after can be skipped

				result.CheckSimple();
			}

			return result;
		}

		public IntDomain2 Intersect( IntDomain2 domain )
		{
			if( ReferenceEquals( domain, this ) )
				return this;

			if( domain.IsEmpty() )
				return m_Empty;

			if( domain.IsSimple() )
				return Intersect( domain.m_Interval );

			// 1, 6
			if( !IntersectsWith( domain ) )
				return m_Empty;

			// If the target is contained, then the result would be equal to the domain, hence we return the domain.
			if( IsSimple() )
			{
				if( domain.Contains( m_Interval ) )
					return this;

				if( m_Interval.Contains( domain.m_Interval ) )
					return domain;
			}
			// Need to be a bit smart, put our interval in a list, in case it gets chopped up.
			List<IntInterval> intervalList;
			if( IsSimple() )
			{
				intervalList	= new List<IntInterval>( 1 );
				intervalList.Add( m_Interval );
			}
			else
			{
				intervalList	= m_IntervalList;
			}

			IntDomain2 result		= new IntDomain2();
			result.m_IntervalList	= new List<IntInterval>( intervalList.Count + domain.m_IntervalList.Count );

			int idx		= 0;
			int domIdx	= 0;
			bool equal	= true;

			while( idx < intervalList.Count
						|| domIdx < domain.m_IntervalList.Count )
			{
				// 6 
				if( idx == intervalList.Count )
				{
					domIdx	= domain.m_IntervalList.Count;
				}
				// 1
				else if( domIdx == domain.m_IntervalList.Count )
				{
					idx		= intervalList.Count;
					equal	= false;
				}
				else
				{
					IntInterval intv		= intervalList[ idx ];
					IntInterval domIntv		= domain.m_IntervalList[ domIdx ];

					// 1 : completely before
					if( domIntv.Max < intv.Min -1 )
					{
						++domIdx;
					}
					// 6 : completely after
					else if( domIntv.Min > intv.Max +1 )
					{
						++idx;
						equal	= false;
					}
					// 3
					else if( domIntv.Min <= intv.Min && domIntv.Max >= intv.Max )
					{
						result.m_IntervalList.Add( intv );
						++idx;
					}
					// ~4 : divide into two intervals...
					else if( domIntv.Min > intv.Min && domIntv.Max < intv.Max )
					{
						result.m_IntervalList.Add( domIntv );
						++domIdx;
						equal	= false;
					}
					// 2
					else if( domIntv.Min <= intv.Min && domIntv.Max < intv.Max )
					{
						result.m_IntervalList.Add( intv.Intersect( domIntv ) );
						++domIdx;
						equal	= false;
					}
					// 5
					else if( domIntv.Min > intv.Min && domIntv.Max >= intv.Max )
					{
						result.m_IntervalList.Add( intv.Intersect( domIntv ) );
						++idx;
						equal	= false;
					}
				}
			}

			if( equal )
				return this;

			result.UpdateInterval();
			
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

		public bool IntersectsWith( IntDomain2 domain )
		{
			if( ReferenceEquals( domain, this ) )
				return true;
			
			if( domain.IsEmpty() )
				return false;

			// 1, 6
			if( !m_Interval.IntersectsWith( domain.m_Interval ) )
				return false;

			if( domain.IsSimple() )
				return IntersectsWith( domain.m_Interval );
			
			if( IsSimple() )
				return domain.IntersectsWith( m_Interval );
			
			int index0		= 0;
			int index1		= 0;
			bool intersect	= false;

			while( index0 < m_IntervalList.Count
					&& index1 < domain.m_IntervalList.Count
					&& !intersect )
			{
				IntInterval ival0	= m_IntervalList[ index0 ];
				IntInterval ival1	= domain.m_IntervalList[ index1 ];

				if( ival0.Max < ival1.Min )
				{
					++index0;
				}
				else if( ival1.Max < ival0.Min )
				{
					++index1;
				}
				else
				{
					intersect	= true;
				}
			}

			return intersect;
		}

		private bool IsAllZero( IntInterval interval )
		{
			bool zero	= true;

			int index		= m_IntervalList.BinarySearch( interval, new IntInterval.Comparer() );
			if( index < 0 )
			{
				index	= ~index;
				
				if( index < m_IntervalList.Count )
				{
					zero	&= !m_IntervalList[ index ].IntersectsWith( interval );
				}
				if( index > 0 )
				{
					zero	&= !m_IntervalList[ index - 1 ].IntersectsWith( interval );
				}
			}
			else
			{
				// interval found => not zero
				zero	= false;
			}
			
			return zero;
		}

		private void SetEmpty()
		{
			m_Interval	= IntInterval.Empty;
		}

		private void CheckSimple()
		{
			if( m_IntervalList.Count == 1 )
			{
				m_IntervalList	= null;
			}
		}

		private void UpdateInterval()
		{
			if( m_IntervalList.Count == 1 )
			{
				m_Interval		= m_IntervalList[ 0 ];
				m_IntervalList	= null;
			}
			else
			{
				m_Interval		= new IntInterval( m_IntervalList[ 0 ].Min, m_IntervalList[ m_IntervalList.Count - 1 ].Max );
			}
		}

		static private bool IntersectsWithOrIsAdjacent( IntInterval lhs, IntInterval rhs )
		{
			return ( lhs.Min + 1 <= rhs.Max )
						&& ( lhs.Max -1 >= rhs.Min );
		}

		#region Add/Subtract/Multiply/Divide Value

		public IntDomain2 Add( int value )
		{
			if( value == 0 )
				return this;
			
			return new IntDomain2( m_Interval + value );
		}

		public IntDomain2 Subtract( int value )
		{
			if( value == 0 )
				return this;
			
			return new IntDomain2( m_Interval - value );
		}

		public IntDomain2 Multiply( int value )
		{
			if( value == 1 )
				return this;

			return new IntDomain2( m_Interval * value );
		}

		public IntDomain2 Divide( int value )
		{
			if( value == 1 )
				return this;

			return new IntDomain2( m_Interval - value );
		}

		#endregion

		#region Domain (+,-,*,/) Value

		// v0 = v1 + value
		public static IntDomain2 operator +( IntDomain2 lhs, int rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - value
		public static IntDomain2 operator -( IntDomain2 lhs, int rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * value
		public static IntDomain2 operator *( IntDomain2 lhs, int rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / value
		public static IntDomain2 operator /( IntDomain2 lhs, int rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Domain

		public IntDomain2 Add( IntDomain2 domain )
		{
			IntInterval interval	= domain.Interval;

			if( interval.Min == 0 && interval.Max == 0 )
				return this;

			return new IntDomain2( m_Interval + interval );
		}

		public IntDomain2 Subtract( IntDomain2 domain )
		{
			IntInterval interval	= domain.Interval;

			if( interval.Min == 0 && interval.Max == 0 )
				return this;

			return new IntDomain2( m_Interval - interval );
		}

		public IntDomain2 Multiply( IntDomain2 domain )
		{
			IntInterval interval	= domain.Interval;

			if( interval.Min == 1 && interval.Max == 1 )
				return this;

			return new IntDomain2( m_Interval * interval );
		}

		public IntDomain2 Divide( IntDomain2 domain )
		{
			IntInterval interval	= domain.Interval;

			if( interval.Min == 1 && interval.Max == 1 )
				return this;

			bool div2	= false;

			IntInterval intv1	= IntInterval.Divide1( m_Interval, interval, ref div2 );
			if( div2 )
			{
				IntInterval intv2	= IntInterval.Divide2( m_Interval, interval, div2 );
			
				return new IntDomain2( new IntInterval[] { intv1, intv2 } );
			}			
			else
			{
				return new IntDomain2( intv1 );
			}
		}

		#endregion

		#region Domain (+,-,*,/) Domain

		// v0 = v1 + v2
		public static IntDomain2 operator +( IntDomain2 lhs, IntDomain2 rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - v2
		public static IntDomain2 operator -( IntDomain2 lhs, IntDomain2 rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * v2
		public static IntDomain2 operator *( IntDomain2 lhs, IntDomain2 rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / v2
		public static IntDomain2 operator /( IntDomain2 lhs, IntDomain2 rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		public IntDomain2 Sqrt()
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.Max < 0 )
				return m_Empty;

			if( ( m_Interval.IsZero() )
					|| ( m_Interval.Min == 1 && m_Interval.Max == 1 ) )
				return this;

			IntDomain2 result	= new IntDomain2();
			result.m_Interval		= m_Interval.Sqrt();

			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count );
			for( int idx = 0; idx < m_IntervalList.Count; ++idx )
			{
				IntInterval interval	= m_IntervalList[ idx ];
			
				if( interval.Max >= 0 )
				{
					result.m_IntervalList.Add( interval.Sqrt() );
				}
			}		

			return result;
		}

		public IntDomain2 Square()
		{
			if( IsEmpty() )
				return m_Empty;

			if( ( m_Interval.IsZero() )
					|| ( m_Interval.Min == 1 && m_Interval.Max == 1 ) )
				return this;

			IntDomain2 result	= new IntDomain2();
			result.m_Interval		= m_Interval.Square();

			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count );
			foreach( IntInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval.Square() );
			}		

			return result;
		}

		#region Negate

		// v0 = -v1
		public static IntDomain2 operator -( IntDomain2 domain )
		{
			return domain.Negate();
		}

		public IntDomain2 Negate()
		{
			if( m_Interval.IsBound() && m_Interval.Min == 0 )
				return this;
			
			IntDomain2 result		= new IntDomain2();
			result.m_Interval		= -m_Interval;
			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<IntInterval>( m_IntervalList.Count );
			for( int idx = m_IntervalList.Count; idx > 0; --idx )
			{
				result.m_IntervalList.Add( -m_IntervalList[ idx - 1 ] );
			}		
			
			return result;
		}

		#endregion

		#region IntDomainOneEnumerator

		// Inner class implements IEnumerator interface:
		private sealed class SimpleIntervalEnumerator : IEnumerator<IntInterval>
		{
			bool			m_MoveNext;
			IntDomain2		m_Domain;

			public SimpleIntervalEnumerator( IntDomain2 domain )
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
			else
				return m_IntervalList.GetEnumerator();
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

		IntInterval			m_Interval;
		List<IntInterval>	m_IntervalList;

		static readonly IntDomain2 m_Empty	= new IntDomain2();
	}
}

//--------------------------------------------------------------------------------
