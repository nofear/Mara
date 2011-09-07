//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Int/IntInterval.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 59    5-03-08 0:37 Patrick
 * updated interval methods
 * 
 * 58    2-03-08 20:59 Patrick
 * check for null reference in Equals(..)
 * 
 * 57    15-11-07 2:16 Patrick
 * fixed issue with cardinality of empty interval
 * 
 * 56    14-11-07 22:39 Patrick
 * fixed GetHashCode()
 * 
 * 55    15-09-07 0:27 Patrick
 * removed sealed
 * 
 * 54    25-07-07 3:55 Patrick
 * added readonl
 * 
 * 53    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 52    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 51    14-03-07 0:38 Patrick
 * 
 * 50    13-03-07 22:26 Patrick
 * refactored Random()
 * 
 * 49    13-03-07 21:48 Patrick
 * added Random()
 * 
 * 48    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 47    28-02-07 21:51 Patrick
 * use IsZero()
 * 
 * 46    27-02-07 22:57 Patrick
 * check that interval does not change for Square()/Sqrt()
 * 
 * 45    27-02-07 21:35 Patrick
 * return this when Negate( 0,0 )
 * 
 * 44    27-02-07 21:18 Patrick
 * added Abs(), Sqrt(), Negate()
 * 
 * 43    21-02-07 20:07 Patrick
 * update
 * 
 * 42    13-02-07 21:55 Patrick
 * sealed
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

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
	[DebuggerDisplay("[{Min},{Max}]")]
	public class IntInterval : IIntInterval
	{
		public IntInterval() :
			this( m_Empty.Min, m_Empty.Max )
		{
		}

		public IntInterval( IntInterval interval ) :
			this( interval.Min, interval.Max )
		{
		}

		public IntInterval( int val ) :
			this( val, val )
		{
		}

		public IntInterval( int min, int max )
		{
			m_Min	= min;
			m_Max	= max;
		}

		public override string ToString()
		{
			StringBuilder str	= new StringBuilder();
			
			if( IsEmpty() )
			{
				str.Append( "[]" );
			}
			else
			{
				str.Append( "[" );
				str.Append( m_Min.ToString( CultureInfo.CurrentCulture ) );
				
				if( m_Min != m_Max )
				{
					str.Append( "," );
					str.Append( m_Max.ToString( CultureInfo.CurrentCulture ) );
				}
				
				str.Append( "]" );
			}
						
			return str.ToString();
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as IntInterval );
		}

		public bool Equals( IntInterval interval )
		{
			if( ReferenceEquals( interval, null ) )
				return false;

			if( ReferenceEquals( interval, this ) )
				return true;

			return m_Min == interval.m_Min
						&& m_Max == interval.m_Max;
		}

		public override int GetHashCode()
		{
			return m_Max << 16 + m_Min;
		}

		public int Min
		{
			get
			{
				return m_Min;
			}

			set
			{
				m_Min	= value;
			}
		}

		public int Max
		{
			get
			{
				return m_Max;
			}

			set
			{
				m_Max	= value;
			}
		}

		public int Cardinality
		{
			get
			{
				if( IsEmpty() )
					return 0;

				return ( m_Max - m_Min ) + 1;
			}
		}

		public static IntInterval Empty
		{
			get
			{
				return m_Empty;
			}
		}

		public static IntInterval Whole
		{
			get
			{
				return m_Whole;
			}
		}

		public bool IsBound()
		{
			return m_Min == m_Max;
		}

		public bool IsValid()
		{
			return m_Min <= m_Max;
		}

		public bool IsEmpty()
		{
			return m_Min > m_Max;
		}

		public bool IsZero()
		{
			return m_Min == 0
						&& m_Max == 0;
		}

		public bool IsOne()
		{
			return m_Min == 1
						&& m_Max == 1;
		}

		public bool Contains( int val )
		{
			return ( m_Min <= val )
						&& ( val <= m_Max );
		}

		public bool Contains( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return false;

			return ( interval.m_Min >= m_Min )
						&& ( interval.m_Max <= m_Max );
		}

		#region Union
		
		public IntInterval Union( int val )
		{
			return Union( val, val );
		}

		public IntInterval Union( int min, int max )
		{
			return Union( new IntInterval( min, max ) );
		}
	
		public IntInterval Union( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return interval ;
					
			if( interval.m_Min >= m_Min && interval.m_Max <= m_Max )
				return this;

			int min		= Math.Min( m_Min, interval.m_Min );
			int max		= Math.Max( m_Max, interval.m_Max );
			
			return new IntInterval( min, max );
		}

		#endregion

		#region Difference
	
		public IntInterval Difference( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return this ;

			// 4 : cannot divide into two intervals...
			if( interval.m_Min > m_Min && interval.m_Max < m_Max )
				return this;
			
			// 1, 6 : completely before or after
			if( interval.m_Max < m_Min || interval.m_Min > m_Max )
				return this;

			// 3 : completely remove interval => empty
			if( interval.m_Min <= m_Min && interval.m_Max >= m_Max )
				return m_Empty;

			// 2 : left overlap
			if( interval.m_Min <= m_Min
						&& interval.m_Max < m_Max )
			{
				int min		= interval.m_Max + 1;
				int max		= m_Max;

				return new IntInterval( min, max );
			}

			// 5 : right overlap
			if( interval.m_Min > m_Min
						&& interval.m_Max >= m_Max )
			{
				int min		= m_Min;
				int max		= interval.m_Min - 1;

				return new IntInterval( min, max );
			}
			
			return m_Empty;
		}

		#endregion

		#region Intersect

		public IntInterval Intersect( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return m_Empty;

			if( IsEmpty() )
				return this;

			// 3
			if( interval.Min <= m_Min && interval.Max >= m_Max )
				return this;

			// 1, 6 : completely before or after
			if( interval.m_Max < m_Min || interval.m_Min > m_Max )
				return m_Empty;

			// 2, 4, 5
			int min		= Math.Max( m_Min, interval.m_Min );
			int max		= Math.Min( m_Max, interval.m_Max );
			
			return new IntInterval( min, max );
		}

		#endregion

		#region IntersectsWith
		
		public bool IntersectsWith( int val )
		{
			return ( val <= m_Max )
						&& ( val >= m_Min );
		}

		public bool IntersectsWith( int min, int max )
		{
			return IntersectsWith( new IntInterval( min, max ) );
		}

		public bool IntersectsWith( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return false;

			return ( interval.m_Min <= m_Max )
						&& ( interval.m_Max >= m_Min );
		}

		#endregion

		#region Sqrt(), Square(), Pow(), Abs(), Negate(), Exp(), Log()

		public IntInterval Sqrt()
		{
			if( IsEmpty() || m_Max < 0 )
				return IntInterval.Empty;

			if( IsZero() || ( m_Min == 1 && m_Max == 1 ) )
				return this;
			
			int min		= ( m_Min > 0 )
						? (int) Math.Floor( Math.Sqrt( m_Min ) )
						: 0;
			int max		= (int) Math.Ceiling( Math.Sqrt( m_Max ) );
			
			return new IntInterval( min, max );
		}

		public IntInterval Square()
		{
			if( IsEmpty() )
				return m_Empty;

			if( IsZero() || ( m_Min == 1 && m_Max == 1 ) )
				return this;

			int min, max;

			if( m_Max < 0 )
			{
				min		= ( m_Max * m_Max );
				max		= ( m_Min * m_Min );
			}
			else if( m_Min > 0 )
			{
				min		= ( m_Min * m_Min );
				max		= ( m_Max * m_Max );
			}
			else
			{
				min		= 0;
				max		= ( -m_Min > m_Max )
						? ( m_Min * m_Min )
						: ( m_Max * m_Max );
			}

			return new IntInterval( min, max );
		}

		public IntInterval Pow( int power )
		{
			if( IsEmpty() )
				return m_Empty;

			if( power == 0 )
			{
				if( IsZero() )
					return m_Empty;

				return new IntInterval( 1, 1 );
			}
			
			int min		= (int) Math.Pow( m_Min, power );
			int max		= (int) Math.Pow( m_Max, power );
			
			return new IntInterval( min, max );
		}

		public IntInterval Abs()
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( m_Min >= 0 )
				return this;
			
			if( m_Max <= 0 )
				return Negate();
			
			return new IntInterval( 0, Math.Max( -m_Min, m_Max ) );
		}

		public IntInterval Negate()
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( IsZero() )
				return this;

			return new IntInterval( -m_Max, -m_Min );
		}

		public IntInterval Exp()
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			int min		= (int) Math.Floor( Math.Exp( m_Min ) );
			int max		= (int) Math.Floor( Math.Exp( m_Max ) );

			return new IntInterval( min, max );
		}

		public IntInterval Log()
		{
			if( IsEmpty() )
				return IntInterval.Empty;
			
			if( m_Max < 0 )
				return IntInterval.Empty;

			int min		= m_Min < 0 ? int.MinValue
						: m_Min == 0 ? 0
						: (int) Math.Floor( Math.Log( m_Min ) );
			int max		= (int) Math.Ceiling( Math.Log( m_Max ) );

			return new IntInterval( min, max );
		}

		#endregion

		#region Random

		public static IntInterval Random( int min, int max )
		{
			int card_max	= max - min;
			int card	= (int) Math.Ceiling( m_Random.NextDouble() * card_max );
			int offset	= (int) Math.Round( m_Random.NextDouble() * ( card_max - card ) );
		
			int intv_min	= min + offset;
			int intv_max	= min + offset + card;

			return new IntInterval( intv_min, intv_max );		
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Interval

		public IntInterval Add( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;
				
			return new IntInterval( m_Min + interval.Min, m_Max + interval.Max );
		}

		public IntInterval Subtract( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			return new IntInterval( m_Min - interval.Max, m_Max - interval.Min );
		}

		public IntInterval Multiply( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			int min0	= interval.Min;
			int max0	= interval.Max;

			int min0min	= min0 * m_Min;
			int min0max	= min0 * m_Max;
			int max0min	= max0 * m_Min;
			int max0max	= max0 * m_Max;

			int min		= Math.Min( Math.Min( min0min, min0max ), Math.Min( max0min, max0max ) );
			int max		= Math.Max( Math.Max( min0min, min0max ), Math.Max( max0min, max0max ) );
			
			return new IntInterval( min, max );			
		}

		public IntInterval Multiply2( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			int intvMin		= interval.Min;
			int intvMax		= interval.Max;

			if( m_Min < 0 )
			{
				if( m_Max > 0 )
				{
					if( intvMin < 0 )
					{
						if( intvMax > 0 )		// M * M
						{
							return new IntInterval( Math.Min( m_Min * intvMax, m_Max * intvMin ),
														Math.Max( m_Min * intvMin, m_Max * intvMax ) );
						}
						else                    // M * N
						{
							return new IntInterval( m_Max * intvMin, m_Min * intvMin );
						}
					}
					else
					{
						if( intvMax>0 )			// M * P
						{
							return new IntInterval( m_Min * intvMax, m_Max * intvMax );
						}
						else                    // M * Z
						{
							return new IntInterval( 0, 0 );
						}
					}
				}
				else
				{
					if( intvMin < 0 )
					{
						if( intvMax > 0 )		// N * M
						{
							return new IntInterval( m_Min * intvMax, m_Min * intvMin );
						}
						else                    // N * N
						{
							return new IntInterval( m_Max * intvMax, m_Min * intvMin );
						}
					}
					else
					{
						if( intvMax>0 )			// N * P
						{
							return new IntInterval( m_Min * intvMax, m_Max * intvMin );
						}
						else                    // N * Z
						{
							return new IntInterval( 0, 0 );
						}
					}
				}
			}
			else
			{
				if( m_Max > 0 )
				{
					if( intvMin < 0 )
					{
						if( intvMax > 0 )			// P * M
						{
							return new IntInterval( m_Max * intvMin, m_Max * intvMax );
						}
						else                    // P * N
						{
							return new IntInterval( m_Max * intvMin, m_Min * intvMax );
						}
					}
					else
					{
						if( intvMax > 0 )		// P * P
						{
							return new IntInterval( m_Min * intvMin, m_Max * intvMax );
						}
						else                    // P * Z
						{
							return new IntInterval( 0, 0 );
						}
					}
				}
				else							// Z * ?
				{
					return new IntInterval( 0, 0 );
				}
			}
		}

		public IntInterval Divide( IntInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			if( InZero( interval ) )
			{
				if( interval.Min != 0 )
				{
					if( interval.Max != 0 )
					{
						return DivideZero( this );
					}
					else
					{
						return DivideNegative( this, interval.Min );
					}
				}
				else
				{
					if( interval.Max !=0 )
					{
						return DividePositive( this, interval.Max );
					}
					else
					{
						return m_Empty;
					}
				}
			}
			else
			{
				return DivideNonZero( this, interval );
			}
		}

		#endregion
		
		#region Interval (+,-,*,/,|,&) Interval

		// v0 = v1 + v2
		public static IntInterval operator +( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - v2
		public static IntInterval operator -( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * v2
		public static IntInterval operator *( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / v2
		public static IntInterval operator /( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Divide( rhs );
		}

		static bool InZero( IntInterval interval )
		{
			if( interval.IsEmpty() )
				return false;

			return ( !( interval.Min > 0 ) )
							&& ( !( interval.Max < 0 ) );
		}

		static IntInterval DivideZero( IntInterval lhs )
		{
			if( lhs.IsZero() )
				return lhs;
			else
				return IntInterval.Whole;
		}

		static IntInterval DivideNonZero( IntInterval lhs, IntInterval rhs )
		{
			int lhsMin = lhs.Min;
			int lhsMax = lhs.Max;
			int rhsMin = rhs.Min;
			int rhsMax = rhs.Max;

			if( lhsMax < 0 )
			{
				if( rhsMax < 0 )
				{
					return new IntInterval( lhsMax / rhsMin, lhsMin / rhsMax );
				}
				else
				{
					return new IntInterval( lhsMin / rhsMin, lhsMax / rhsMax );
				}
			}
			else if( lhsMin < 0 )
			{
				if( rhsMax < 0 )
				{
					return new IntInterval( lhsMax / rhsMax, lhsMin / rhsMax );
				}
				else
				{
					return new IntInterval( lhsMin / rhsMin, lhsMax / rhsMin );
				}
			}
			else
			{
				if( rhsMax < 0 )
				{
					return new IntInterval( lhsMax / rhsMax, lhsMin / rhsMin );
				}
				else
				{
					return new IntInterval( lhsMin / rhsMax, lhsMax / rhsMin );
				}
			}
		}

		static IntInterval DividePositive( IntInterval lhs, int rhsMax )
		{
			if( lhs.IsZero() )
				return lhs;

			int lhsMin = lhs.Min;
			int lhsMax = lhs.Max;

			if( lhsMax < 0 )
			{
				return new IntInterval( Integer.neg_inf, lhsMax / rhsMax );
			}
			else if( lhsMin < 0 )
			{
				return new IntInterval( Integer.neg_inf, Integer.pos_inf );
			}
			else
			{
				return new IntInterval( lhsMin / rhsMax, Integer.pos_inf );
			}
		}

		static IntInterval DivideNegative( IntInterval lhs, int rhsMin )
		{
			if( lhs.IsZero() )
				return lhs;

			int lhsMin = lhs.Min;
			int lhsMax = lhs.Max;

			if( lhsMax < 0 )
			{
				return new IntInterval( lhsMax / rhsMin, Integer.pos_inf );
			}
			else if( lhsMin < 0 )
			{
				return new IntInterval( Integer.neg_inf, Integer.pos_inf );
			}
			else
			{
				return new IntInterval( Integer.neg_inf, lhsMin / rhsMin );
			}
		}

		public static IntInterval Divide1( IntInterval lhs, IntInterval rhs, ref bool b )
		{
			b = false;

			if( lhs.IsEmpty() || rhs.IsEmpty() )
				return m_Empty;

			if( InZero( rhs ) )
			{
				if( rhs.Min != 0 )
				{
					if( rhs.Max != 0 )
					{
						return DivideZeroPart1( lhs, rhs, ref b );
					}
					else
					{
						return DivideNegative( lhs, rhs.Min );
					}
				}
				else
				{
					if( rhs.Max != 0 )
					{
						return DividePositive( lhs, rhs.Max );
					}
					else
					{
						return m_Empty;
					}
				}
			}
			else
			{
				return DivideNonZero(lhs, rhs);
			}
		}

		public static IntInterval Divide2( IntInterval lhs, IntInterval rhs, bool b )
		{
			if( !b )
				return m_Empty;

			return DivideZeroPart2( lhs, rhs );
		}

		static IntInterval DivideZeroPart1( IntInterval lhs, IntInterval rhs, ref bool b )
		{
			if( lhs.IsZero() )
			{
				b = false;
				return lhs;
			}

			int lhsMin = lhs.Min;
			int lhsMax = lhs.Max;
			int rhsMin = rhs.Min;
			int rhsMax = rhs.Max;

			if( lhsMax < 0 )
			{
				b = true;
				return new IntInterval( Integer.neg_inf, lhsMax / rhsMax );
			}
			else if ( lhsMin < 0 )
			{
				b = false;
				return Whole;
			}
			else
			{
				b = true;
				return new IntInterval( Integer.neg_inf, lhsMin / rhsMin );
			}
		}

		static IntInterval DivideZeroPart2( IntInterval lhs, IntInterval rhs )
		{
			if( lhs.Max < 0 )
			{
				return new IntInterval( lhs.Max / rhs.Min, Integer.pos_inf );
			}
			else
			{
				return new IntInterval( lhs.Min / rhs.Max, Integer.pos_inf );
			}
		}

		public static IntInterval operator|( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Union( rhs );
		}

		public static IntInterval operator&( IntInterval lhs, IntInterval rhs )
		{
			return lhs.Intersect( rhs );
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Value

		public IntInterval Add( int value )
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( value == 0 )
				return this;
			
			return new IntInterval( m_Min + value, m_Max + value );
		}

		public IntInterval Subtract( int value )
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( value == 0 )
				return this;
			
			return new IntInterval( m_Min - value, m_Max - value );
		}

		public IntInterval Multiply( int value )
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( value == 1 )
				return this;

			return new IntInterval( m_Min * value, m_Max * value );
		}

		public IntInterval Divide( int value )
		{
			if( IsEmpty() )
				return IntInterval.Empty;

			if( value == 1 )
				return this;

			return new IntInterval( m_Min / value, m_Max / value );
		}

		#endregion

		#region Interval (+,-,*,/) Value

		// v0 = v1 + value
		public static IntInterval operator +( IntInterval lhs, int rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - value
		public static IntInterval operator -( IntInterval lhs, int rhs )
		{
			return lhs.Subtract( rhs );	
		}

		// v0 = v1 * value
		public static IntInterval operator *( IntInterval lhs, int rhs )
		{
			return lhs.Multiply( rhs );	
		}

		// v0 = v1 / value
		public static IntInterval operator /( IntInterval lhs, int rhs )
		{
			return lhs.Divide( rhs );		
		}

		#endregion
	
		#region operator-

		public static IntInterval operator-( IntInterval interval )
		{
			return interval.Negate();
		}

		#endregion

		#region operator ==,!=
		
		// operator ==
		public static bool operator ==( IntInterval lhs, IntInterval rhs )
		{			
			return lhs.Equals( rhs );
		}

		// operator !=
		public static bool operator !=( IntInterval lhs, IntInterval rhs )
		{			
			return !lhs.Equals( rhs );
		}

		#endregion

		sealed class Integer
		{
			static public int neg_inf	= int.MinValue;
			static public int pos_inf	= int.MaxValue;
		}

		public sealed class Comparer : IComparer<IntInterval>
		{
			public int Compare( IntInterval x, IntInterval y ) 
			{
				int r	= ( x.Min < y.Min ) ? -1
						: ( x.Min > y.Min ) ? 1
						: ( x.Max < y.Max ) ? -1
						: ( x.Max > y.Max ) ? 1
						: 0;
						
				return r;
			}
		}

		static readonly IntInterval m_Empty	= new IntInterval( int.MaxValue, int.MinValue );
		static readonly IntInterval m_Whole	= new IntInterval( Integer.neg_inf, Integer.pos_inf );
		static readonly Random m_Random		= new Random( 0 );

		int	m_Min;
		int m_Max;
	};
}
