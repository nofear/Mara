using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;

using NUnit.Framework;

namespace SolverTest.Interval.Int
{
	[TestFixture]
	public class IntIntervalTest
	{
		[Test]
		public void Equals()
		{
			IntInterval i	= new IntInterval( -100, 100 );
			IntInterval a	= new IntInterval( -100, 100 );
			IntInterval b	= new IntInterval( -99, 100 );
			IntInterval c	= new IntInterval( -100, 99 );
			
			Assert.IsTrue( i.Equals( a ) );
			Assert.IsFalse( i.Equals( b ) );
			Assert.IsFalse( i.Equals( c ) );
		}

		[Test]
		public void ContainsValue()
		{
			IntInterval i	= new IntInterval( -100, 100 );
			
			Assert.IsFalse( i.Contains( -101 ) );
			Assert.IsTrue( i.Contains( -100 ) );
			Assert.IsTrue( i.Contains( 0 ) );
			Assert.IsTrue( i.Contains( 100 ) );
			Assert.IsFalse( i.Contains( 101 ) );
		}

		[Test]
		public void ContainsInterval()
		{
			IntInterval a	= new IntInterval( -100, 100 );
			IntInterval b	= new IntInterval( -50, 50 );
		
			Assert.IsTrue( a.Contains( a ) );
			Assert.IsTrue( a.Contains( b ) );
			Assert.IsFalse( b.Contains( a ) );
		}

		[Test]
		public void Union()
		{
			IntInterval a	= new IntInterval( -100, 50 );
			IntInterval b	= new IntInterval( -50, 100 );
			Assert.AreEqual( new IntInterval( -100, 100 ), a.Union( b ) );
		}

		[Test]
		public void Difference25()
		{
			IntInterval a	= new IntInterval( -100, 100 );
			IntInterval b	= new IntInterval( -150, -50 );
			IntInterval c	= new IntInterval( 50, 150 );
			Assert.AreEqual( new IntInterval( -49, 100 ), a.Difference( b ) );
			Assert.AreEqual( new IntInterval( -100, 49 ), a.Difference( c ) );
		}

		[Test]
		public void Difference3()
		{
			IntInterval a	= new IntInterval( -100, 100 );
			IntInterval b	= new IntInterval( -150, 150 );
			Assert.AreEqual( IntInterval.Empty, a.Difference( b ) );
		}

		[Test]
		public void Difference4()
		{
			IntInterval a	= new IntInterval( -100, 100 );
			IntInterval b	= new IntInterval( -50, 50 );
			Assert.AreEqual( new IntInterval( -100, 100 ), a.Difference( b ) );
		}

		[Test]
		public void Intersect25()
		{
			IntInterval a	= new IntInterval( -100, 50 );
			IntInterval b	= new IntInterval( -50, 100 );
			Assert.AreEqual( new IntInterval( -50, 50 ), a.Intersect( b ) );
			Assert.AreEqual( new IntInterval( -50, 50 ), b.Intersect( a ) );
		}

		[Test]
		public void Intersect16()
		{
			IntInterval a	= new IntInterval( -150, -100 );
			IntInterval b	= new IntInterval( -50, 50 );
			IntInterval c	= new IntInterval( 100, 150 );
			Assert.AreEqual( IntInterval.Empty, b.Intersect( a ) );
			Assert.AreEqual( IntInterval.Empty, b.Intersect( c ) );
		}
	
		[Test]
		public void IntersectsWith()
		{
			IntInterval a0	= new IntInterval( -100, -51 );
			IntInterval a	= new IntInterval( -100, -50 );
			IntInterval b	= new IntInterval( -50, 50 );
			IntInterval c	= new IntInterval( 50, 100 );
			IntInterval c0	= new IntInterval( 51, 100 );
			
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
			IntInterval a	= new IntInterval( -10, -5 );
			IntInterval b	= new IntInterval( -10, 5 );
			IntInterval c	= new IntInterval( -5, 10 );
			IntInterval d	= new IntInterval( 5, 10 );

			Assert.AreEqual( new IntInterval( 25, 100 ), a * a );
			Assert.AreEqual( new IntInterval( -50, 100 ), a * b );
			Assert.AreEqual( new IntInterval( -100, 50 ), a * c );
			Assert.AreEqual( new IntInterval( -100, -25 ), a * d );

			Assert.AreEqual( new IntInterval( -50, 100 ), b * b );
			Assert.AreEqual( new IntInterval( -100, 50 ), b * c );
			Assert.AreEqual( new IntInterval( -100, 50 ), b * d );

			Assert.AreEqual( new IntInterval( -50, 100 ), c * c );
			Assert.AreEqual( new IntInterval( -50, 100 ), c * d );

			Assert.AreEqual( new IntInterval( 25, 100 ), d * d );
		}

		[Test]
		public void Addition()
		{
			IntInterval a	= new IntInterval( -10, 5 );
			IntInterval b	= new IntInterval( -5, 10 );

			Assert.AreEqual( new IntInterval( -15, 15 ), a + b );
		}

		[Test]
		public void Substraction()
		{
			IntInterval a	= new IntInterval( -10, 5 );
			IntInterval b	= new IntInterval( -5, 10 );

			Assert.AreEqual( new IntInterval( -20, 10 ), a - b );
		}

		[Test]
		public void Negate()
		{
			IntInterval a	= new IntInterval( -10, 5 );
			IntInterval b	= new IntInterval( -5, 10 );

			Assert.AreEqual( new IntInterval( -5, 10 ), -a );
			Assert.AreEqual( new IntInterval( -10, 5 ), -b );
		}
	}
}
