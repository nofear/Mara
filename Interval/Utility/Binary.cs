//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Utility/Binary.cs $
 * 
 * 2     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 5     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 4     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 3     22-02-07 0:17 Patrick
 * fixed minor thing
 * 
 * 2     21-02-07 20:22 Patrick
 * 
 * 1     21-02-07 20:04 Patrick
 * added class
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraInterval.Utility
{
	[CLSCompliant(false)]
	public class Binary
	{
		public static int BitCount( UInt32 n )                          
		{
			UInt32 m1	= 0x55555555;
			UInt32 m2	= 0x33333333;
			UInt32 m3	= 0x0F0F0F0F;

			n = (n & m1) + ((n>>1) & m1);
			n = (n & m2) + ((n>>2) & m2);
			n = (n & m3) + ((n>>4) & m3);

			return (int) ( n % 255 );
		}

		public static int BitCount2( UInt32 n )
		{
			n   = n - ((n >> 1) & 0xDB6DB6DB) - ((n >> 2) & 0x49249249);

			return (int)( ( n + ( n >> 3 ) ) & 0xC71C71C7 ) % 63;
		}

		// Return word where a all bits from  (including) the
		//   lowest set bit to bit 0 are set.
		// Return 0 if no bit is set.
		public static UInt32 LowestBit01Edge( UInt32 x )
		{
			if( 0 == x )
				return 0;

			return  x^(x-1);
		}

		// Return word where a all bits from (including) the
		//   lowest set bit to most significant bit are set.
		// Return 0 if no bit is set.
		public static UInt32 LowestBit10Edge( UInt32 x )
		{
			if( 0 == x )
				return 0;

			x ^= (x-1);
			// here  x == lowest_bit_01edge(x);
			return  ~(x>>1);
		}

		// Return index of lowest bit set.
		// Bit index ranges from zero to BITS_PER_LONG-1.
		// Examples:
		//    ***1 --> 0
		//    **10 --> 1
		//    *100 --> 2
		// Return 0 (also) if no bit is set.
		public static int LowestBitIdx( UInt32 x )
		{
			//    if ( 1>=x )  return  x-1; // 0 if 1, ~0 if 0
			//    if ( 0==x )  return  0;

			int r = 0;
			
			x &= (UInt32)(-x);  // isolate lowest bit
			
			if( ( x & 0xffff0000 ) != 0 )  r += 16;
			if( ( x & 0xff00ff00 ) != 0 )  r += 8;
			if( ( x & 0xf0f0f0f0 ) != 0 )  r += 4;
			if( ( x & 0xcccccccc ) != 0 )  r += 2;
			if( ( x & 0xaaaaaaaa ) != 0 )  r += 1;

			return r;
		}

		public static int LowestZeroIdx( UInt32 x )
		{
			return LowestBitIdx( ~x );
		}

		// Return word where only the lowest set bit in x is set.
		// Return 0 if no bit is set.
		public static UInt32 LowestBit( UInt32 x )
		{
			//    if ( 0==x )  return 0;
			//    return  ((x^(x-1)) >> 1) + 1;
			//    return  (x & (x-1)) ^ x;

			return  x & (UInt32)(-x);  // use: -x == ~x + 1
		}

		// Return word where only the lowest unset bit in x is set.
		// Return 0 if all bits are set.
		public static UInt32 LowestZero( UInt32 x )
		{
			//    return  (x ^ (x+1)) & ~x;
			//    return  ((x ^ (x+1)) >> 1 ) + 1;

			x = ~x;

			return  x & (UInt32)(-x);
		}

		// Isolate lowest block of ones.
		// e.g.:
		// x   = *****011100
		// l   = 00000000100
		// y   = *****100000
		// x^y = 00000111100
		// ret = 00000011100
		public static UInt32 LowestBlock( UInt32 x )
		{
			UInt32 l = x & (UInt32)(-x);  // lowest bit
			UInt32 y = x + l;
			y ^= x;

			return  y & (y>>1);
		}
		// -------------------------

		// Return word were the lowest bit set in x is cleared.
		// Return 0 for input == 0.
		public static UInt32 DeleteLowestBit( UInt32 x )
		{
			return  x & (x-1);
		}

		// Return word were the lowest unset bit in x is set.
		// Return ~0 for input == ~0.
		public static UInt32 SetLowestZero( UInt32 x )
		{
			return  x | (x+1);
		}

		// Return word where all the (low end) ones are set.
		// Example:  01011011 --> 00000011
		// Return 0 if lowest bit is zero:
		//       10110110 --> 0
		public static UInt32 LowBits( UInt32 x )
		{
			if( ~0U == x )
				return ~0U;

			return (((x+1)^x) >> 1);
		}

		// Return word where all the (low end) zeros are set.
		// Example:  01011000 --> 00000111
		// Return 0 if all bits are set.
		public static UInt32 LowZeros( UInt32 x )
		{
			if( 0 == x ) 
				return ~0U;

			return (((x-1)^x) >> 1);
		}

		// Return word where a all bits from (including) the
		//   highest set bit to bit 0 are set.
		// Return 0 if no bit is set.
		//
		// Feed the result into bit_count() to get
		//   the index of the highest bit set.
		public static UInt32 HighestBit01Edge( UInt32 x )
		{
			x |= x>>1;
			x |= x>>2;
			x |= x>>4;
			x |= x>>8;
			x |= x>>16;

			return  x;
		}

		// Return word where a all bits from  (including) the
		//   highest set bit to most significant bit are set.
		// Return 0 if no bit is set.
		public static UInt32 HighestBit10Edge( UInt32 x )
		{
			if( 0 == x )
				return 0;

			x = HighestBit01Edge(x);

			return  ~(x>>1);
		}

		// Return word where only the highest bit in x is set.
		// Return 0 if no bit is set.
		public static UInt32 HighestBit( UInt32 x )
		{
			x = HighestBit01Edge(x);

			return  x ^ (x>>1);
		}

		// Return word where only the highest unset bit in x is set.
		// Return 0 if all bits are set.
		public static UInt32 HighestZero( UInt32 x )
		{
			return HighestBit( ~x );
		}

		// Return word were the highest unset bit in x is set.
		// Return ~0 for input == ~0.
		public static UInt32 SetHighestZero( UInt32 x )
		{
		//    if ( ~0UL==x )  return  ~0UL;
			return  x | HighestBit( ~x );
		}

		// Return index of highest bit set.
		// Return 0 if no bit is set.
		public static int HighestBitIdx( UInt32 x )
		{
			if( 0 == x )
				return  0;

			//    // this version avoids all if() statements:
			//    x = ( highest_bit(x) << 1 ) - 1;
			//    return  bit_count_01(x) - 1;

			int r = 0;

			if( ( x & 0xffff0000 ) != 0 ) { x >>= 16;  r += 16; }
			if( ( x & 0x0000ff00 ) != 0 ) { x >>=  8;  r +=  8; }
			if( ( x & 0x000000f0 ) != 0 ) { x >>=  4;  r +=  4; }
			if( ( x & 0x0000000c ) != 0 ) { x >>=  2;  r +=  2; }
			if( ( x & 0x00000002 ) != 0 ) {            r +=  1; }

			return r;
		}

		public static int HigestZeroIdx( UInt32 x )
		{
			return HighestBitIdx( ~x );
		}

		// Return word where all the (high end) zeros are set.
		// e.g.:  00011001 --> 11100000
		// Return 0 if highest bit is set:
		//        11011001 --> 00000000
		public static UInt32 HighZero( UInt32 x )
		{
			x |= x>>1;
			x |= x>>2;
			x |= x>>4;
			x |= x>>8;
			x |= x>>16;

			return  ~x;
		}

		// Return word where all the (high end) ones are set.
		// e.g.  11001011 --> 11000000
		// Return 0 if highest bit is zero:
		//       01110110 --> 00000000
		public static UInt32 HighBits( UInt32 x )
		{
			return HighZero( ~x );
		}
   }
}
