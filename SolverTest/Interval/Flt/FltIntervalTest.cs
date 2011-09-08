using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;
using MaraInterval.Utility;

using NUnit.Framework;

namespace SolverTest.Interval.Flt
{
	[TestFixture]
	public class FltIntervalTest
	{
		[Test]
		public void Equals()
		{
			FltInterval i	= new FltInterval( -100, 100 );
			FltInterval a	= new FltInterval( -100, 100 );
			FltInterval b	= new FltInterval( -99, 100 );
			FltInterval c	= new FltInterval( -100, 99 );
			
			Assert.IsTrue( i.Equals( a ) );
			Assert.IsFalse( i.Equals( b ) );
			Assert.IsFalse( i.Equals( c ) );
		}

		[Test]
		public void ContainsValue()
		{
			FltInterval i	= new FltInterval( -100, 100 );
			
			Assert.IsFalse( i.Contains( -101 ) );
			Assert.IsTrue( i.Contains( -100 ) );
			Assert.IsTrue( i.Contains( 0 ) );
			Assert.IsTrue( i.Contains( 100 ) );
			Assert.IsFalse( i.Contains( 101 ) );
		}

		[Test]
		public void ContainsInterval()
		{
			FltInterval a	= new FltInterval( -100, 100 );
			FltInterval b	= new FltInterval( -50, 50 );
		
			Assert.IsTrue( a.Contains( a ) );
			Assert.IsTrue( a.Contains( b ) );
			Assert.IsFalse( b.Contains( a ) );
		}

		[Test]
		public void Union()
		{
			FltInterval a	= new FltInterval( -100, 50 );
			FltInterval b	= new FltInterval( -50, 100 );
			
			FltInterval c	= a.Union( b );
			
			Assert.AreEqual( new FltInterval( -100, 100 ), c );
		}
		
		[Test]
		public void IntersectsWith()
		{
			FltInterval a0	= new FltInterval( -100, -51 );
			FltInterval a	= new FltInterval( -100, -50 );
			FltInterval b	= new FltInterval( -50, 50 );
			FltInterval c	= new FltInterval( 50, 100 );
			FltInterval c0	= new FltInterval( 51, 100 );
			
			Assert.IsFalse( a0.IntersectsWith( b ) );
			Assert.IsFalse( b.IntersectsWith( a0 ) );
			Assert.IsFalse( b.IntersectsWith( c0 ) );
			Assert.IsFalse( c0.IntersectsWith( b ) );

			Assert.IsTrue( a.IntersectsWith( b ) );
			Assert.IsTrue( b.IntersectsWith( a ) );
			Assert.IsTrue( b.IntersectsWith( c ) );
			Assert.IsTrue( c.IntersectsWith( b ) );
		}

		[Test]
		public void Multiply()
		{
			FltInterval a	= new FltInterval( -10, -5 );
			FltInterval b	= new FltInterval( -10, 5 );
			FltInterval c	= new FltInterval( -5, 10 );
			FltInterval d	= new FltInterval( 5, 10 );

			Assert.AreEqual( new FltInterval( 25, 100 ), a * a );
			Assert.AreEqual( new FltInterval( -50, 100 ), a * b );
			Assert.AreEqual( new FltInterval( -100, 50 ), a * c );
			Assert.AreEqual( new FltInterval( -100, -25 ), a * d );

			Assert.AreEqual( new FltInterval( -50, 100 ), b * b );
			Assert.AreEqual( new FltInterval( -100, 50 ), b * c );
			Assert.AreEqual( new FltInterval( -100, 50 ), b * d );

			Assert.AreEqual( new FltInterval( -50, 100 ), c * c );
			Assert.AreEqual( new FltInterval( -50, 100 ), c * d );

			Assert.AreEqual( new FltInterval( 25, 100 ), d * d );
		}

		[Test]
		public void Addition()
		{
			FltInterval a	= new FltInterval( -10, 5 );
			FltInterval b	= new FltInterval( -5, 10 );

			Assert.AreEqual( new FltInterval( -15, 15 ), a + b );
		}

		[Test]
		public void Substraction()
		{
			FltInterval a	= new FltInterval( -10, 5 );
			FltInterval b	= new FltInterval( -5, 10 );

			Assert.AreEqual( new FltInterval( -20, 10 ), a - b );
		}

		[Test]
		public void Negate()
		{
			FltInterval a	= new FltInterval( -10, 5 );
			FltInterval b	= new FltInterval( -5, 10 );

			Assert.AreEqual( new FltInterval( -5, 10 ), -a );
			Assert.AreEqual( new FltInterval( -10, 5 ), -b );
		}

		[Test]
		public void LogToExp()
		{
			for( int idx = 0; idx < 1000; ++idx )
			{
				FltInterval x	= FltInterval.Random( 1E6 );
				FltInterval x1	= x.Log();
				FltInterval x2	= x1.Exp();
				
				Assert.IsTrue( x2.Contains( x ) );
			}
		}

		[Test]
		public void ExpToLog()
		{
			for( int idx = 0; idx < 1000; ++idx )
			{
				FltInterval x	= FltInterval.Random( 1E6 );
				FltInterval x1	= x.Exp();
				FltInterval x2	= x1.Log();
				
				Assert.IsTrue( x2.Contains( x ) );
			}
		}

		[Test]
		public void DivToMul()
		{
			for( int idx = 0; idx < 1000; ++idx )
			{
				FltInterval x	= FltInterval.Random( 1E6 );
				FltInterval y	= FltInterval.Random( 1E6 );
				FltInterval c1	= x / y;
				FltInterval c2	= c1 * y;
				
				Assert.IsTrue( c2.Contains( x ) );
			}
		}

		[Test]
		public void MulToDiv()
		{
			for( int idx = 0; idx < 1000; ++idx )
			{
				FltInterval x	= FltInterval.Random( 1E6 );
				FltInterval y	= FltInterval.Random( 1E6 );
				FltInterval c1	= x * y;
				FltInterval c2	= c1 / y;
				
				Assert.IsTrue( c2.Contains( x ) );
			}
		}
		
	}
}
