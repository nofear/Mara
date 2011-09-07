//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Flt/FltInterval.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 49    7-03-08 21:54 Patrick
 * Do not use epsilon in compare
 * Added Pi & E
 * Added Random
 * 
 * 48    5-03-08 0:37 Patrick
 * updated interval methods
 * 
 * 47    2-03-08 20:59 Patrick
 * check for null reference in Equals(..)
 * 
 * 46    29-02-08 22:15 Patrick
 * use epsilon comparison
 * 
 * 45    14-01-08 20:30 Patrick
 * better bounds for Log/Exp
 * 
 * 44    15-11-07 2:16 Patrick
 * fixed issue with cardinality of empty interval
 * 
 * 43    14-11-07 22:39 Patrick
 * fixed GetHashCode()
 * 
 * 42    31-07-07 20:03 Patrick
 * added IsOne()
 * 
 * 41    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 40    14-03-07 0:38 Patrick
 * 
 * 39    13-03-07 22:26 Patrick
 * refactored Random()
 * 
 * 38    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 37    28-02-07 21:51 Patrick
 * use IsZero()
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
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
	[DebuggerDisplay("[{Min},{Max}]")]
	public sealed class FltInterval : IFltInterval
	{
		public FltInterval() :
			this( m_Empty.Min, m_Empty.Max )
		{
		}

		public FltInterval( FltInterval interval ) :
			this( interval.Min, interval.Max )
		{
		}

		public FltInterval( double val ) :
			this( val, val )
		{
		}

		public FltInterval( double min, double max )
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
			return Equals( obj as FltInterval );
		}

		public bool Equals( FltInterval interval )
		{
			if( ReferenceEquals( interval, null ) )
				return false;

			if( ReferenceEquals( interval, this ) )
				return true;

			return ( m_Min == interval.m_Min )
					&& ( m_Max == interval.m_Max );
		}

		public override int GetHashCode()
		{
			return (int) m_Max << 16 + (int) m_Min;
		}

		public double Min
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

		public double Max
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

		public double Cardinality
		{
			get
			{
				if( IsEmpty() )
					return 0.0;

				return ( m_Max - m_Min );
			}
		}

		public static FltInterval Empty
		{
			get
			{
				return m_Empty;
			}
		}

		public static FltInterval Whole
		{
			get
			{
				return m_Whole;
			}
		}

		public static FltInterval Pi
		{
			get
			{
				return m_ValuePi;
			}
		}

		public static FltInterval E
		{
			get
			{
				return m_ValueE;
			}
		}

		public bool IsBound()
		{
			return Epsilon.Equal( m_Min, m_Max );
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
			return m_Min == 1.0
					&& m_Max == 1.0;
		}

		public bool Contains( double val )
		{
			return ( val >= m_Min )
					&& ( val <= m_Max );
		}

		public bool Contains( FltInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return false;

			return ( interval.m_Min >= m_Min )
					&& ( interval.m_Max <= m_Max );
		}

		#region Union
		
		public FltInterval Union( double val )
		{
			return Union( new FltInterval( val, val ) );
		}

		public FltInterval Union( double min, double max )
		{
			return Union( new FltInterval( min, max ) );
		}
	
		public FltInterval Union( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return interval ;
			
			if( interval.Contains( this ) )
				return interval;

			if( Contains( interval ) )
				return this;

			double min	= Math.Min( m_Min, interval.m_Min );
			double max	= Math.Max( m_Max, interval.m_Max );
			
			return new FltInterval( min, max );
		}

		#endregion

		#region Difference
	
		public FltInterval Difference( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return this;

			if( IsEmpty() )
				return this ;

			// 4 : cannot divide into two intervals...
			if( ( interval.m_Min > m_Min ) && ( interval.m_Max < m_Max ) )
				return this;
			
			// 1, 6 : completely before or after
			if( ( interval.m_Max < m_Min ) || ( interval.m_Min > m_Max ) )
				return this;

			// 3 : completely remove interval => empty
			if( interval.Contains( this ) )
				return m_Empty;

			// 2 : left overlap
			if( IntersectsWith( interval )
					&& ( interval.m_Max < m_Max ) )
			{
				double min		= Epsilon.Next( interval.m_Max );
				double max		= m_Max;

				return new FltInterval( min, max );
			}

			// 5 : right overlap
			if( IntersectsWith( interval )
					&& ( interval.m_Min > m_Min ) )
			{
				double min		= m_Min;
				double max		= Epsilon.Prev( interval.m_Min );

				return new FltInterval( min, max );
			}
			
			return m_Empty;
		}

		#endregion

		#region Intersect

		public FltInterval Intersect( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return m_Empty;

			if( IsEmpty() )
				return this;

			// 3
			if( interval.m_Min <= m_Min && interval.m_Max >= m_Max )
				return this;

			// 1, 6 : completely before or after
			if( ( interval.m_Max < m_Min ) || ( interval.m_Min > m_Max ) )
				return m_Empty;

			// 2, 4, 5
			double min		= Math.Max( m_Min, interval.m_Min );
			double max		= Math.Min( m_Max, interval.m_Max );
			
			return new FltInterval( min, max );
		}

		#endregion

		#region IntersectsWith
		
		public bool IntersectsWith( double val )
		{
			return ( val <= m_Max )
						&& ( val >= m_Min );
		}

		public bool IntersectsWith( double min, double max )
		{
			return IntersectsWith( new FltInterval( min, max ) );
		}

		public bool IntersectsWith( FltInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return false;

			return ( interval.m_Min <= m_Max )
					&& ( interval.m_Max >= m_Min );
		}

		#endregion

		#region Sqrt(), Square(), Pow(), Abs(), Negate(), Exp(), Log()

		public FltInterval Sqrt()
		{
			if( IsEmpty() || m_Max < 0 )
				return m_Empty;

			if( IsZero() || IsOne() )
				return this;
			
			double min	= ( m_Min > 0 )
						? Math.Sqrt( m_Min )
						: 0;
			double max	= Math.Sqrt( m_Max );
			
			return new FltInterval( min, max );
		}

		public FltInterval Square()
		{
			if( IsEmpty() )
				return m_Empty;

			if( IsZero() || IsOne() )
				return this;

			double min, max;

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

			return new FltInterval( min, max );
		}

		public FltInterval Pow( int power )
		{
			if( IsEmpty() )
				return m_Empty;

			if( power == 0 )
			{
				if( IsZero() )
					return m_Empty;

				return new FltInterval( 1, 1 );
			}
			
			double min	= Math.Pow( m_Min, power );
			double max	= Math.Pow( m_Max, power );
			
			if( power < 0 )
				return new FltInterval( max, min );
			
			return new FltInterval( min, max );
		}
	
		public FltInterval Abs()
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( m_Min >= 0 )
				return this;
			
			if( m_Max <= 0 )
				return Negate();
			
			return new FltInterval( 0, Math.Max( -m_Min, m_Max ) );
		}

		public FltInterval Negate()
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( IsZero() )
				return this;

			return new FltInterval( -m_Max, -m_Min );
		}

		public FltInterval Exp()
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			// slightly enlarge interval
			double min	= Epsilon.PrevOne( Math.Exp( m_Min ) );
			double max	= Epsilon.NextOne( Math.Exp( m_Max ) );

			return new FltInterval( min, max );
		}

		public FltInterval Log()
		{
			if( IsEmpty() )
				return FltInterval.Empty;
			
			if( m_Max < 0 )
				return FltInterval.Empty;

			double min	= m_Min < 0 ? double.MinValue
						: m_Min == 0 ? 0
						: Epsilon.PrevOne( Math.Log( m_Min ) );
			double max	= Epsilon.NextOne( Math.Log( m_Max ) );

			return new FltInterval( min, max );
		}

		#endregion

		public FltInterval Inflate()
		{
			if( IsEmpty() )
				return m_Empty;
			
			double min	= m_Min;
			double max	= m_Max;
			
			if( !double.IsNegativeInfinity(min) )
				min		= Epsilon.Prev( min );
						
			if( !double.IsPositiveInfinity(max) )
				max		= Epsilon.Next( max );
		
			return new FltInterval( min, max );
		}

		#region Random

		public static FltInterval Random( double max )
		{
			double x	= RandomVal( max );
			double y	= RandomVal( max );

			return new FltInterval( Math.Min(x,y), Math.Max(x,y) );		
		}

		private static double RandomVal( double max )
		{
			double x;
			unsafe
			{
				int mexp	= (int) ( Math.Log(max)/Math.Log(2)/2 );
			
				Int64 exp	= (1<<10) + m_Random.Next( -mexp, mexp );
				Int64 mant	= Math.BigMul( m_Random.Next(), m_Random.Next() ) % ~(1L<<52);
				Int64 num	= (exp<<52|mant);
				
				x	= * (double*) &num;
			}

			return x;
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Interval

		public FltInterval Add( FltInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;
				
			return new FltInterval( m_Min + interval.Min, m_Max + interval.Max );
		}

		public FltInterval Subtract( FltInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			return new FltInterval( m_Min - interval.Max, m_Max - interval.Min );
		}

		public FltInterval Multiply( FltInterval interval )
		{
			if( IsEmpty() || interval.IsEmpty() )
				return m_Empty;

			double intvMin = interval.Min;
			double intvMax = interval.Max;

			if( m_Min < 0 )
			{
				if( m_Max > 0 )
				{
					if( intvMin < 0 )
					{
						if( intvMax > 0 )		// M * M
						{
							return new FltInterval( Math.Min( m_Min * intvMax, m_Max * intvMin ),
														Math.Max( m_Min * intvMin, m_Max * intvMax ) );
						}
						else                    // M * N
						{
							return new FltInterval( m_Max * intvMin, m_Min * intvMin );
						}
					}
					else
					{
						if( intvMax>0 )			// M * P
						{
							return new FltInterval( m_Min * intvMax, m_Max * intvMax );
						}
						else                    // M * Z
						{
							return m_Zero;
						}
					}
				}
				else
				{
					if( intvMin < 0 )
					{
						if( intvMax > 0 )		// N * M
						{
							return new FltInterval( m_Min * intvMax, m_Min * intvMin );
						}
						else                    // N * N
						{
							return new FltInterval( m_Max * intvMax, m_Min * intvMin );
						}
					}
					else
					{
						if( intvMax>0 )			// N * P
						{
							return new FltInterval( m_Min * intvMax, m_Max * intvMin );
						}
						else                    // N * Z
						{
							return m_Zero;
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
						if( intvMax > 0 )		// P * M
						{
							return new FltInterval( m_Max * intvMin, m_Max * intvMax );
						}
						else                    // P * N
						{
							return new FltInterval( m_Max * intvMin, m_Min * intvMax );
						}
					}
					else
					{
						if( intvMax > 0 )		// P * P
						{
							return new FltInterval( m_Min * intvMin, m_Max * intvMax );
						}
						else                    // P * Z
						{
							return m_Zero;
						}
					}
				}
				else							// Z * ?
				{
					return m_Zero;
				}
			}
		}

		public FltInterval Divide( FltInterval interval )
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
		public static FltInterval operator +( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - v2
		public static FltInterval operator -( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Subtract( rhs );
		}

		// v0 = v1 * v2
		public static FltInterval operator *( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Multiply( rhs );
		}

		// v0 = v1 / v2
		public static FltInterval operator /( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Divide( rhs );
		}

		static bool InZero( FltInterval interval )
		{
			if( interval.IsEmpty() )
				return false;

			return ( !( interval.Min > 0 ) )
							&& ( !( interval.Max < 0 ) );
		}

		static FltInterval DivideZero( FltInterval lhs )
		{
			if( lhs.IsZero() )
				return lhs;
			else
				return FltInterval.Whole;
		}

		static FltInterval DivideNonZero( FltInterval lhs, FltInterval rhs )
		{
			double lhsMin = lhs.Min;
			double lhsMax = lhs.Max;
			double rhsMin = rhs.Min;
			double rhsMax = rhs.Max;

			if( lhsMax < 0 )
			{
				if( rhsMax < 0 )
				{
					return new FltInterval( lhsMax / rhsMin, lhsMin / rhsMax );
				}
				else
				{
					return new FltInterval( lhsMin / rhsMin, lhsMax / rhsMax );
				}
			}
			else if( lhsMin < 0 )
			{
				if( rhsMax < 0 )
				{
					return new FltInterval( lhsMax / rhsMax, lhsMin / rhsMax );
				}
				else
				{
					return new FltInterval( lhsMin / rhsMin, lhsMax / rhsMin );
				}
			}
			else
			{
				if( rhsMax < 0 )
				{
					return new FltInterval( lhsMax / rhsMax, lhsMin / rhsMin );
				}
				else
				{
					return new FltInterval( lhsMin / rhsMax, lhsMax / rhsMin );
				}
			}
		}

		static FltInterval DividePositive( FltInterval lhs, double rhsMax )
		{
			if( lhs.IsZero() )
				return lhs;

			double lhsMin = lhs.Min;
			double lhsMax = lhs.Max;

			if( lhsMax < 0 )
			{
				return new FltInterval( double.NegativeInfinity, lhsMax / rhsMax );
			}
			else if( lhsMin < 0 )
			{
				return new FltInterval( double.NegativeInfinity, double.PositiveInfinity );
			}
			else
			{
				return new FltInterval( lhsMin / rhsMax, double.PositiveInfinity );
			}
		}

		static FltInterval DivideNegative( FltInterval lhs, double rhsMin )
		{
			if( lhs.IsZero() )
				return lhs;

			double lhsMin = lhs.Min;
			double lhsMax = lhs.Max;

			if( lhsMax < 0 )
			{
				return new FltInterval( lhsMax / rhsMin, double.PositiveInfinity );
			}
			else if( lhsMin < 0 )
			{
				return new FltInterval( double.NegativeInfinity, double.PositiveInfinity );
			}
			else
			{
				return new FltInterval( double.NegativeInfinity, lhsMin / rhsMin );
			}
		}

		public static FltInterval Divide1( FltInterval lhs, FltInterval rhs, ref bool b )
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

		public static FltInterval Divide2( FltInterval lhs, FltInterval rhs, bool b )
		{
			if( !b )
				return m_Empty;

			return DivideZeroPart2( lhs, rhs );
		}

		static FltInterval DivideZeroPart1( FltInterval lhs, FltInterval rhs, ref bool b )
		{
			if( lhs.IsZero() )
			{
				b = false;
				return lhs;
			}

			double lhsMin = lhs.Min;
			double lhsMax = lhs.Max;
			double rhsMin = rhs.Min;
			double rhsMax = rhs.Max;

			if( lhsMax < 0 )
			{
				b = true;
				return new FltInterval( double.NegativeInfinity, lhsMax / rhsMax );
			}
			else if ( lhsMin < 0 )
			{
				b = false;
				return Whole;
			}
			else
			{
				b = true;
				return new FltInterval( double.NegativeInfinity, lhsMin / rhsMin );
			}
		}

		static FltInterval DivideZeroPart2( FltInterval lhs, FltInterval rhs )
		{
			if( lhs.Max < 0 )
			{
				return new FltInterval( lhs.Max / rhs.Min, double.PositiveInfinity );
			}
			else
			{
				return new FltInterval( lhs.Min / rhs.Max, double.PositiveInfinity );
			}
		}

		public static FltInterval operator|( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Union( rhs );
		}

		public static FltInterval operator&( FltInterval lhs, FltInterval rhs )
		{
			return lhs.Intersect( rhs );
		}

		#endregion

		#region Add/Subtract/Multiply/Divide Value

		public FltInterval Add( double value )
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( value == 0.0 )
				return this;
			
			return new FltInterval( m_Min + value, m_Max + value );
		}

		public FltInterval Subtract( double value )
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( value == 0.0 )
				return this;
			
			return new FltInterval( m_Min - value, m_Max - value );
		}

		public FltInterval Multiply( double value )
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( value == 1.0 )
				return this;

			return new FltInterval( m_Min * value, m_Max * value );
		}

		public FltInterval Divide( double value )
		{
			if( IsEmpty() )
				return FltInterval.Empty;

			if( value == 1.0 )
				return this;

			return new FltInterval( m_Min / value, m_Max / value );
		}

		#endregion

		#region Interval (+,-,*,/) Value

		// v0 = v1 + value
		public static FltInterval operator +( FltInterval lhs, double rhs )
		{
			return lhs.Add( rhs );
		}

		// v0 = v1 - value
		public static FltInterval operator -( FltInterval lhs, double rhs )
		{
			return lhs.Subtract( rhs );	
		}

		// v0 = v1 * value
		public static FltInterval operator *( FltInterval lhs, double rhs )
		{
			return lhs.Multiply( rhs );	
		}

		// v0 = v1 / value
		public static FltInterval operator /( FltInterval lhs, double rhs )
		{
			return lhs.Divide( rhs );		
		}

		#endregion

		#region operator-

		public static FltInterval operator-( FltInterval interval )
		{
			return interval.Negate();
		}

		#endregion

		#region operator ==,!=
		
		// operator ==
		public static bool operator ==( FltInterval lhs, FltInterval rhs )
		{			
			return lhs.Equals( rhs );
		}

		// operator !=
		public static bool operator !=( FltInterval lhs, FltInterval rhs )
		{			
			return !lhs.Equals( rhs );
		}

		#endregion

		public sealed class Comparer : IComparer<FltInterval>
		{
			public int Compare( FltInterval x, FltInterval y ) 
			{
				int r	= ( x.Min < y.Min ) ? -1
						: ( x.Min > y.Min ) ? 1
						: ( x.Max < y.Max ) ? -1
						: ( x.Max > y.Max ) ? 1
						: 0;
						
				return r;
			}
		}

		static FltInterval m_Empty		= new FltInterval( double.MaxValue, double.MinValue );
		static FltInterval m_Whole		= new FltInterval( double.NegativeInfinity, double.PositiveInfinity );
		static FltInterval m_Zero		= new FltInterval( 0, 0 );
		static FltInterval m_ValuePi	= new FltInterval( Epsilon.PrevOne( Math.PI ), Epsilon.NextOne( Math.PI ) );
		static FltInterval m_ValueE		= new FltInterval( Epsilon.PrevOne( Math.E ), Epsilon.NextOne( Math.E ) );

		static Random m_Random			= new Random( 0 );

		double	m_Min;
		double	m_Max;
	};
}
