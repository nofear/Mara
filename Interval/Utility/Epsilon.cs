//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Utility/Epsilon.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 7     4-03-08 22:41 Patrick
 * prevent overflow of negative numbers
 * 
 * 6     2-03-08 21:00 Patrick
 * added Pow(..)
 * 
 * 5     29-02-08 21:54 Patrick
 * added Epsilon compare
 * 
 * 4     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 3     2-03-07 20:29 Patrick
 * 
 * 2     2-03-07 20:29 Patrick
 * added comment
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraInterval.Utility
{
	public class Epsilon
	{
		public static double NextOne( double val )
		{
			return Diff( val, 1 );
		}

		public static double Next( double val )
		{
			return Diff( val, m_Value + 1 );
		}

		public static double PrevOne( double val )
		{
			return Diff( val, -1 );
		}

		public static double Prev( double val )
		{
			return Diff( val, -( m_Value + 1 ) );
		}

		private static double Diff( double val, Int64 diff )
		{
			double r	= val;

			unsafe
			{
				Int64 x	= * (Int64*) &r;
				if( x < 0 )
					x	= ~( (Int64) 1 << 63 ) - x;

				x	+= diff;
				
				if( x < 0 )
					x	= ~( (Int64) 1 << 63 ) - x;

				r      = * (double*) &x;
			}

			return r;
		}

		public static bool Equal( double x, double y )
		{
			if( x == y )
				return true;
		
			unsafe
			{
				Int64 xInt	= * (Int64*) &x;
				if( xInt < 0 )
					xInt = ~( (Int64) 1 << 63 ) - xInt;

				Int64 yInt	= * (Int64*) &y;
				if( yInt < 0 )
					yInt = ~( (Int64) 1 << 63 ) - yInt;

				Int64 diff	= Math.Abs( xInt - yInt );
				
				return diff <= m_Value;
			}
		}

		public static bool NotEqual( double x, double y )
		{
			return !Equal( x, y );
		}

		public static bool Less( double x, double y )
		{
			return ( x < y ) && !Equal( x, y );
		}

		public static bool LessEqual( double x, double y )
		{
			return ( x < y ) || Equal( x, y );
		}

		public static bool Greater( double x, double y )
		{
			return ( x > y ) && !Equal( x, y );
		}

		public static bool GreaterEqual( double x, double y )
		{
			return ( x > y ) || Equal( x, y );
		}
		
		public static Int64 Value
		{
			get
			{
				return m_Value;
			}

			set
			{
				m_Value		= value;
			}
		}

		private static Int64 m_Value	= 1<<22;
	}
}

//--------------------------------------------------------------------------------
