//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Flt/FltDomain.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 70    7-03-08 21:54 Patrick
 * do exact compare
 * 
 * 69    5-03-08 0:37 Patrick
 * updated interval methods
 * 
 * 68    29-02-08 22:15 Patrick
 * use epsilon comparison
 * 
 * 67    29-02-08 21:54 Patrick
 * use epsilon comparison
 * 
 * 66    15-11-07 2:16 Patrick
 * fixed issue with cardinality of empty interval
 * 
 * 65    14-11-07 22:39 Patrick
 * fixed GetHashCode()
 * 
 * 64    10-11-07 1:26 Patrick
 * added ICloneable
 * 
 * 63    12-10-07 22:31 Patrick
 * fixed comment
 * 
 * 62    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 61    20-06-07 22:46 Patrick
 * renamed namespace
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
	public sealed class FltDomain : IFltInterval, IEnumerable<FltInterval>, ICloneable
	{
		public FltDomain()
		{
			SetEmpty();
		}

		public FltDomain( double val ) :
			this( val, val )
		{
		}

		public FltDomain( double min, double max ) :
			this( new FltInterval( min, max ) )
		{
		}

		public FltDomain( FltInterval interval )
		{
			m_Interval		= interval;
			m_IntervalList	= null;
		}

		public FltDomain( FltDomain domain )
		{
			m_Interval		= domain.m_Interval;
			m_IntervalList	= domain.m_IntervalList;
		}

		public FltDomain( double[] list )
		{
			if( list.Length % 2 == 0 )
			{
				m_Interval		= new FltInterval( list[ 0 ], list[ list.Length - 1 ] );
				m_IntervalList	= null;
				
				int count	= list.Length / 2;
				if( count > 1 )
				{
					m_IntervalList	= new List<FltInterval>( count );

					for( int idx = 0; idx < count; ++idx )
					{
						m_IntervalList.Add( new FltInterval( list[ idx * 2 ], list[ idx * 2 + 1 ] ) );
					}
				}
			}
		}

		public FltDomain( IEnumerable<FltInterval> list )
		{
			IEnumerator<FltInterval> it		= list.GetEnumerator();
			if( it.MoveNext() )
			{
				double min	= it.Current.Min;
				double max	= it.Current.Max;
				int count	= 1;
				
				while( it.MoveNext() )
				{
					min		= Math.Min( min, it.Current.Min );
					max		= Math.Max( max, it.Current.Max );
					++count;
				}
				
				m_Interval		= new FltInterval( min, max );
				
				if( count > 1 )
				{
					m_IntervalList	=  new List<FltInterval>( list );
					m_IntervalList.Sort( new FltInterval.Comparer() );
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
		
		private FltDomain( FltInterval intv1, FltInterval intv2 )
		{
			m_Interval		= new FltInterval( intv1.Min, intv2.Max );
			m_IntervalList	= new List<FltInterval>( 2 );
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
				foreach( FltInterval interval in this )
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

			FltDomain domain	= obj as FltDomain;
			if( !ReferenceEquals( domain, null ) )
				return Equals( domain );
		
			return false;
		}

		public bool Equals( FltDomain domain )
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
			return (int) m_Interval.Max << 16 + (int) m_Interval.Min;
		}

		public static FltDomain Empty
		{
			get
			{
				return m_Empty;
			}
		}

		public double Min
		{
			get
			{
				return m_Interval.Min;
			}
		}

		public double Max
		{
			get
			{
				return m_Interval.Max;
			}
		}

		public FltInterval Interval
		{
			get
			{
				return m_Interval;
			}
		}

		public double Cardinality
		{
			get
			{
				if( IsSimple() )
					return m_Interval.Cardinality;

				double card	= 0;
				
				foreach( FltInterval interval in m_IntervalList )
				{
					card	+= interval.Cardinality;
				}
				
				return card;
			}
		}

		public FltInterval[] ToArray()
		{
			if( IsSimple() )
				return new FltInterval[] { m_Interval };
			
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

		public bool Contains( double val )
		{
			if( IsSimple() )
				return m_Interval.Contains( val );

			bool contains	= false;

			FltInterval interval	= new FltInterval( val, val );

			int index		= m_IntervalList.BinarySearch( interval, new FltInterval.Comparer() );
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

		public bool Contains( FltInterval interval )
		{
			if( IsSimple() )
				return m_Interval.Contains( interval );

			bool contains	= false;

			int index		= m_IntervalList.BinarySearch( interval, new FltInterval.Comparer() );
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
			
		public FltDomain Union( double val )
		{
			return Union( val, val );
		}

		public FltDomain Union( double min, double max )
		{
			return Union( new FltInterval( min, max ) );
		}

		public FltDomain Union( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return new FltDomain( interval );

			// ~4
			if( Contains( interval ) )
				return this;

			// 3
			if( interval.Contains( m_Interval ) )
				return new FltDomain( interval );

			if( IsSimple() )
			{
				// 1
				if( interval.Max < m_Interval.Min )
					return new FltDomain( interval, m_Interval );

				// 6
				if( interval.Min > m_Interval.Max )
					return new FltDomain( m_Interval, interval );
							
				// 2, 5
				return new FltDomain( m_Interval.Union( interval ) );
			}
			else
			{
				FltDomain result		= new FltDomain();
				result.m_Interval		= m_Interval.Union( interval );
				result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count + 1 );

				// 1
				if( interval.Max < m_Interval.Min )
				{
					result.m_IntervalList.Add( interval );
					result.m_IntervalList.AddRange( m_IntervalList );
				}

				// 6
				else if( interval.Min > m_Interval.Max )
				{
					result.m_IntervalList.AddRange( m_IntervalList );
					result.m_IntervalList.Add( interval );
				}
				else
				{
					// 6
					int idx	= 0;
					for( ;idx < m_IntervalList.Count && ( m_IntervalList[ idx ].Max < interval.Min ); ++idx )
					{
						result.m_IntervalList.Add( m_IntervalList[ idx ] );
					}
					
					if( idx < m_IntervalList.Count )
					{
						FltInterval intv	= interval;
						
						// intersect
						if( idx < m_IntervalList.Count
								&& m_IntervalList[ idx ].IntersectsWith( intv ) )
						{
							intv	= intv.Union( m_IntervalList[ idx ] );
							++idx;
						}
	
						// 3
						for( ;idx < m_IntervalList.Count && intv.Contains( m_IntervalList[ idx ] ); ++idx ) {}

						// intersect
						if( idx < m_IntervalList.Count
								&& m_IntervalList[ idx ].IntersectsWith( intv ) )
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
		
		public FltDomain Union( FltDomain domain )
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
				FltDomain result		= new FltDomain();
				result.m_Interval		= m_Interval.Union( domain.m_Interval );
				result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count + domain.m_IntervalList.Count );

				// 1 : completely before
				if( domain.m_Interval.Max < m_Interval.Min )
				{
					result.m_IntervalList.AddRange( domain.m_IntervalList );
					result.m_IntervalList.AddRange( m_IntervalList );
				}

				// 6 : completely after
				else if( domain.m_Interval.Min > m_Interval.Max )
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
							FltInterval intv		= m_IntervalList[ idx ];
							FltInterval domIntv		= domain.m_IntervalList[ domIdx ];

							// 1 : completely before
							if( domIntv.Max < intv.Min )
							{
								equal	= false;

								result.m_IntervalList.Add( domIntv );
								++domIdx;
							}
							// 6 : completely after
							else if( domIntv.Min > intv.Max )
							{
								result.m_IntervalList.Add( intv );
								++idx;
							}
							// 3
							else if( domIntv.Contains( intv ) )
							{
								equal	= false;

								++idx;
							}
							// 3
							else if( intv.Contains( domIntv ) )
							{
								++domIdx;
							}
							// 2, 5
							else
							{
								equal	= false;
								
								FltInterval intvTmp		= intv.Union( domIntv );
								
								++idx;

								// contains...
								for( ;idx < m_IntervalList.Count
										&& intvTmp.Contains( m_IntervalList[ idx ] );
										++idx ) {};

								// intersect...
								while( idx < m_IntervalList.Count
												&& m_IntervalList[ idx ].IntersectsWith( intvTmp ) )
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

		public FltDomain Difference( double val )
		{
			return Difference( val, val );
		}

		public FltDomain Difference( double min, double max )
		{
			return Difference( new FltInterval( min, max ) );
		}
	
		public FltDomain Difference( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			// 1, 6
			if( !IntersectsWith( interval ) )
				return this;

			// 3 : completely remove interval => empty
			if( interval.Contains( m_Interval ) )
				return m_Empty;

			// Result domain
			FltDomain result		= new FltDomain();
			result.m_Interval		= m_Interval.Difference( interval );

			if( IsSimple() )
			{
				// 4 : divide into two intervals...
				if( I4_Divide( interval, m_Interval ) )
				{
					result.m_IntervalList	= new List<FltInterval>( 2 );
					result.m_IntervalList.Add( new FltInterval( m_Interval.Min, Epsilon.Prev( interval.Min ) ) );
					result.m_IntervalList.Add( new FltInterval( Epsilon.Next( interval.Max ), m_Interval.Max ) );
				}
			}
			else
			{
				result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count + 1 );

				int idx	= 0;

				// 6
				for( ;idx < m_IntervalList.Count && m_IntervalList[ idx ].Max < interval.Min; ++idx )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ] );
				}
			
				// 2
				if( idx < m_IntervalList.Count
						&& I2_IntersectMin(interval , m_IntervalList[ idx ]) )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Difference( interval ) );
					++idx;
				}

				// 4 : divide into two intervals...
				if( idx < m_IntervalList.Count
						&& I4_Divide( interval, m_IntervalList[ idx ] ) )
				{
					result.m_IntervalList.Add( new FltInterval( m_Interval.Min, Epsilon.Prev( interval.Min ) ) );
					result.m_IntervalList.Add( new FltInterval( Epsilon.Next( interval.Max ), m_Interval.Max ) );
				}
				// 3
				else
				{
					for( ;idx < m_IntervalList.Count && interval.Contains( m_IntervalList[ idx ] ); ++idx ) {}
				}				

				// 5
				if( idx < m_IntervalList.Count
						&& I5_IntersectMax(interval , m_IntervalList[ idx ]) )
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

		public FltDomain Difference( FltDomain domain )
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
			List<FltInterval> intervalList;
			if( IsSimple() )
			{
				intervalList	= new List<FltInterval>( 1 );
				intervalList.Add( m_Interval );
			}
			else
			{
				intervalList	= m_IntervalList;
			}

			FltDomain result		= new FltDomain();
			result.m_IntervalList	= new List<FltInterval>( intervalList.Count + domain.m_IntervalList.Count );

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
					FltInterval intv		= intervalList[ idx ];
					FltInterval domIntv		= domain.m_IntervalList[ domIdx ];

					// 1 : completely before
					if( domIntv.Max < intv.Min )
					{
						++domIdx;
					}
					// 6 : completely after
					else if( domIntv.Min > intv.Max )
					{
						result.m_IntervalList.Add( intv );
						++idx;
					}
					// 3
					else if( domIntv.Contains( intv ) )
					{
						++idx;
					}
					// 4 : divide into two intervals...
					else if( I4_Divide( intv, domIntv ) )
					{
						while( domIdx < domain.m_IntervalList.Count
									&& I4_Divide( intv, domIntv ) )
						{
							result.m_IntervalList.Add( new FltInterval( intv.Min, Epsilon.Prev( domIntv.Min ) ) );

							intv		= new FltInterval( Epsilon.Next( domIntv.Max ), intv.Max );

							++domIdx;

							if( domIdx < domain.m_IntervalList.Count )
							{
								domIntv		= domain.m_IntervalList[ domIdx ];
							}
						}

						result.m_IntervalList.Add( intv );
					}
					// 2
					else if( I2_IntersectMin( intv , domIntv ) )
					{
						result.m_IntervalList.Add( intv.Difference( domIntv ) );

						++domIdx;
					}
					// 5
					else if( I5_IntersectMax( intv , domIntv ) )
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

		public FltDomain Intersect( double val )
		{
			return Intersect( val, val );		
		}

		public FltDomain Intersect( double min, double max )
		{
			return Intersect( new FltInterval( min, max ) );
		}

		public FltDomain Intersect( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return m_Empty;

			// 1, 6
			if( !IntersectsWith( interval ) )
				return m_Empty;

			// 3
			if( interval.Min <= m_Interval.Min && interval.Max >= m_Interval.Max )
			//if( interval.Contains( m_Interval ) )
				return this;

			FltInterval intv	= m_Interval.Intersect( interval );

			FltDomain result	= new FltDomain();
			result.m_Interval	= intv;

			if( !IsSimple() )
			{
				result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );

				int idx	= 0;

				// 6 : completely before
				for( ;idx < m_IntervalList.Count && ( m_IntervalList[ idx ].Max < interval.Min ); ++idx ) {};

				// 2
				if( idx < m_IntervalList.Count
						&& I2_IntersectMin( m_IntervalList[ idx ] , interval ) )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Intersect( interval ) );
					++idx;
				}

				// 4
				if( idx < m_IntervalList.Count
						&& I4_Divide( m_IntervalList[ idx ], interval ) )
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
						&& I5_IntersectMax( m_IntervalList[ idx ] , interval ) )
				{
					result.m_IntervalList.Add( m_IntervalList[ idx ].Intersect( interval ) );
					++idx;
				}

				// 1 : complete after can be skipped

				result.UpdateInterval();
			}

			return result;
		}

		public FltDomain Intersect( FltDomain domain )
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
			List<FltInterval> intervalList;
			if( IsSimple() )
			{
				intervalList	= new List<FltInterval>( 1 );
				intervalList.Add( m_Interval );
			}
			else
			{
				intervalList	= m_IntervalList;
			}

			FltDomain result		= new FltDomain();
			result.m_IntervalList	= new List<FltInterval>( intervalList.Count + domain.m_IntervalList.Count );

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
					FltInterval intv		= intervalList[ idx ];
					FltInterval domIntv		= domain.m_IntervalList[ domIdx ];

					// 1 : completely before
					if( domIntv.Max < intv.Min )
					{
						++domIdx;
					}
					// 6 : completely after
					else if( domIntv.Min > intv.Max )
					{
						++idx;
						equal	= false;
					}
					// 3
					else if( domIntv.Contains( intv ) )
					{
						result.m_IntervalList.Add( intv );
						++idx;
					}
					// ~4 : divide into two intervals...
					else if( I4_Divide( intv, domIntv ) )
					{
						result.m_IntervalList.Add( domIntv );
						++domIdx;
						equal	= false;
					}
					// 2
					else if( I2_IntersectMin( intv, domIntv ) )
					{
						result.m_IntervalList.Add( intv.Intersect( domIntv ) );
						++domIdx;
						equal	= false;
					}
					// 5
					else if( I5_IntersectMax( intv, domIntv ) )
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

		public bool IntersectsWith( FltInterval interval )
		{
			if( ReferenceEquals( interval, this ) )
				return true;
			
			if( IsEmpty() || interval.IsEmpty() )
				return false;
		
			// 4 : divide into two intervals...
			if( I4_Divide( interval, m_Interval )
					&& !IsSimple() )
			{
				if( IsAllZero( interval ) )
					return false;
			}

			return m_Interval.IntersectsWith( interval );
		}

		public bool IntersectsWith( FltDomain domain )
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
				FltInterval ival0	= m_IntervalList[ index0 ];
				FltInterval ival1	= domain.m_IntervalList[ index1 ];

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

		private bool IsAllZero( FltInterval interval )
		{
			bool zero	= true;

			int index		= m_IntervalList.BinarySearch( interval, new FltInterval.Comparer() );
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
			m_Interval	= FltInterval.Empty;
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
				m_Interval		= new FltInterval( m_IntervalList[ 0 ].Min, m_IntervalList[ m_IntervalList.Count - 1 ].Max );
			}
		}

		// returns true if rhs divides lhs in two.
		static private bool I4_Divide( FltInterval lhs, FltInterval rhs )
		{
			return ( lhs.Min > rhs.Min )
						&& ( lhs.Max < rhs.Max );
		}

		static private bool I2_IntersectMin( FltInterval lhs , FltInterval rhs )
		{
			return lhs.IntersectsWith( rhs )
						&& ( lhs.Max > rhs.Max );
		}

		static private bool I5_IntersectMax( FltInterval lhs , FltInterval rhs )
		{
			return lhs.IntersectsWith( rhs )
						&& ( lhs.Min < rhs.Min );
		}

		#region Add/Subtract/Multiply/Divide Value

		public FltDomain Add( double value )
		{
			if( value == 0 )
				return this;
			
			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval + value;

			if( IsSimple() )
				return result;
			
			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval + value );
			}		

			return result;
		}

		public FltDomain Subtract( double value )
		{
			if( value == 0 )
				return this;
			
			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval - value;

			if( IsSimple() )
				return result;
			
			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval - value );
			}		

			return result;
		}

		public FltDomain Multiply( double value )
		{
			if( value == 1 )
				return this;

			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval * value;

			if( IsSimple() )
				return result;
			
			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval * value );
			}		

			return result;
		}

		public FltDomain Divide( double value )
		{
			if( value == 1 )
				return this;

			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval / value;

			if( IsSimple() )
				return result;
			
			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval / value );
			}		

			return result;
		}

		#endregion

		#region Domain (+,-,*,/) Value

		// v0 = v1 + value
		public static FltDomain operator +( FltDomain lhs, double rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - value
		public static FltDomain operator -( FltDomain lhs, double rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * value
		public static FltDomain operator *( FltDomain lhs, double rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / value
		public static FltDomain operator /( FltDomain lhs, double rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Domain

		public FltDomain Add( FltDomain domain )
		{
			FltInterval interval	= domain.Interval;

			if( interval.IsZero() )
				return this;

			return new FltDomain( ( m_Interval + interval ).Inflate() );
		}

		public FltDomain Subtract( FltDomain domain )
		{
			FltInterval interval	= domain.Interval;

			if( interval.IsZero() )
				return this;

			return new FltDomain( ( m_Interval - interval ).Inflate() );
		}

		public FltDomain Multiply( FltDomain domain )
		{
			FltInterval interval	= domain.Interval;

			if( interval.IsOne() )
				return this;

			FltInterval result	= m_Interval * interval;
			if( result.IsEmpty() )
				return m_Empty;

			return new FltDomain( result.Inflate() );
		}

		public FltDomain Divide( FltDomain domain )
		{
			FltInterval interval	= domain.Interval;

			if( interval.IsOne() )
				return this;

			bool div2	= false;

			FltInterval intv1	= FltInterval.Divide1( m_Interval, interval, ref div2 );
			if( div2 )
			{
				FltInterval intv2	= FltInterval.Divide2( m_Interval, interval, div2 );
			
				return new FltDomain( new FltInterval[] { intv1.Inflate(), intv2.Inflate() } );
			}			
			else
			{
				return new FltDomain( intv1.Inflate() );
			}
		}

		#endregion

		#region Domain (+,-,*,/) Domain

		// v0 = v1 + v2
		public static FltDomain operator +( FltDomain lhs, FltDomain rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - v2
		public static FltDomain operator -( FltDomain lhs, FltDomain rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * v2
		public static FltDomain operator *( FltDomain lhs, FltDomain rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / v2
		public static FltDomain operator /( FltDomain lhs, FltDomain rhs )
		{
			return lhs.Divide( rhs );
		}

		#endregion

		#region Sqrt(), Square(), Pow(), Abs()

		public FltDomain Sqrt()
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.Max < 0 )
				return m_Empty;

			if( m_Interval.IsZero() || m_Interval.IsOne() )
				return this;

			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval.Sqrt();

			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				if( interval.Max >= 0 )
				{
					result.m_IntervalList.Add( interval.Sqrt() );
				}
			}		

			result.CheckSimple();

			return result;
		}

		public FltDomain Square()
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.IsZero() || m_Interval.IsOne() )
				return this;

			if( IsSimple() )
				return new FltDomain( m_Interval.Square() );

			FltDomain result	= Abs();
			result.m_Interval	= result.m_Interval.Square();

			if( result.IsSimple() )
				return result;

			for( int idx = 0; idx < result.m_IntervalList.Count; ++idx )
			{
				result.m_IntervalList[ idx ]	= result.m_IntervalList[ idx ].Square();
			}

			return result;
		}

		public FltDomain Pow( int power )
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.IsZero() || m_Interval.IsOne() )
				return this;

			FltDomain result	= new FltDomain();
			result.m_Interval	= m_Interval.Pow( power );

			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval.Pow( power ) );
			}
			
			if( power < 0 )
			{
				result.m_IntervalList.Reverse();
			}	

			return result;
		}

		public FltDomain Abs()
		{
			if( IsEmpty() )
				return m_Empty;

			if( m_Interval.Min >= 0 )
				return this;

			if( m_Interval.Max <= 0 )
				return Negate();
			
			FltDomain lower		= -Intersect( double.MinValue, 0 );
			FltDomain upper		= Intersect( 0, double.MaxValue );

			return lower.Union( upper );
		}

		#endregion

		#region Negate

		// v0 = -v1
		public static FltDomain operator -( FltDomain domain )
		{
			return domain.Negate();
		}

		public FltDomain Negate()
		{
			if( m_Interval.IsBound() && m_Interval.Min == 0 )
				return this;
			
			FltDomain result		= new FltDomain();
			result.m_Interval		= -m_Interval;
			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( -interval );
			}		
			
			return result;
		}

		#endregion

		public FltDomain Inflate()
		{
			if( IsEmpty() )
				return m_Empty;
			
			FltDomain result		= new FltDomain();
			result.m_Interval		= m_Interval.Inflate();
			if( IsSimple() )
				return result;

			result.m_IntervalList	= new List<FltInterval>( m_IntervalList.Count );
			foreach( FltInterval interval in m_IntervalList )
			{
				result.m_IntervalList.Add( interval.Inflate() );
			}		

			return result;	
		}

		#region SimpleIntervalEnumerator

		// Inner class implements IEnumerator interface:
		private sealed class SimpleIntervalEnumerator : IEnumerator<FltInterval>
		{
			bool			m_MoveNext;
			FltDomain		m_Domain;

			public SimpleIntervalEnumerator( FltDomain domain )
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

			#region IEnumerator<FltInterval> Members

			public FltInterval Current
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

		#region IEnumerable<FltInterval> Members

		public IEnumerator<FltInterval> GetEnumerator()
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

		#region IEnumerable<FltInterval> Members

		IEnumerator<FltInterval> IEnumerable<FltInterval>.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new FltDomain( this );
		}

		#endregion

		FltInterval			m_Interval;
		List<FltInterval>	m_IntervalList;

		static FltDomain m_Empty	= new FltDomain();
	}
}

//--------------------------------------------------------------------------------
