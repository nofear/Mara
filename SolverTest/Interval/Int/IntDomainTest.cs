using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;

using NUnit.Framework;

namespace SolverTest.Interval.Int
{
	[TestFixture]
	public class IntDomainTest
	{
		private void Validate( IntDomain domain, int min, int max )
		{
			Assert.AreEqual( domain.Interval, new IntInterval( min, max ) );

			for( int val = min; val <= max; ++val )
			{
				Assert.IsTrue( domain.Contains( val ) );
			}
		}
		
		[Test]
		public void Union1()
		{
			IntDomain a	= new IntDomain( -20, -1 );
			IntDomain b	= new IntDomain( 0, 19 );
			IntDomain c	= a.Union( b );
			
			Validate( c, -20, 19 );
		}

		[Test]
		public void Union6()
		{
			IntDomain a	= new IntDomain( -20, -1 );
			IntDomain b	= new IntDomain( 0, 19 );
			IntDomain c	= b.Union( a );
			
			Validate( c, -20, 19 );
		}
		
		[Test]
		public void Difference1()
		{
			IntDomain c	= new IntDomain( 16, 47 );
			IntDomain a	= new IntDomain( c );
			IntDomain r	= a.Difference( new IntDomain( 0, 15 ) );
			Assert.AreEqual( r, c ); 
		}

		[Test]
		public void Difference6()
		{
			IntDomain c	= new IntDomain( 16, 47 );
			IntDomain a	= new IntDomain( c );
			IntDomain r	= a.Difference( new IntDomain( 48, 63 ) );
			Assert.AreEqual( r, c ); 
		}

		public void Intersect2()
		{
			IntDomain a	= new IntDomain( 8, 55 );
			IntDomain b	= new IntDomain( 40, 63 );
			IntDomain c	= b.Intersect( a );
			Validate( c, 40, 55 );
		}

		[Test]
		public void Intersect5()
		{
			IntDomain a	= new IntDomain( 8, 55 );
			IntDomain b	= new IntDomain( 40, 63 );
			IntDomain c	= a.Intersect( b );
			Validate( c, 40, 55 );
		}

		[Test]
		public void IntersectsWith4()
		{
			IntDomain a	= new IntDomain( new int[] { 0, 15, 48, 63 } );
			
			Assert.IsFalse( a.IntersectsWith( new IntInterval( -10, -1 ) ) );
			Assert.IsFalse( a.IntersectsWith( new IntInterval( 16, 47 ) ) );
			Assert.IsFalse( a.IntersectsWith( new IntInterval( 64, 80 ) ) );

			Assert.IsTrue( a.IntersectsWith( new IntInterval( 15, 47 ) ) );
			Assert.IsTrue( a.IntersectsWith( new IntInterval( 16, 48 ) ) );
		}

		[Test]
		public void EmptyUnion()
		{
			IntDomain a	= new IntDomain( 8, 55 );
			
			Assert.AreEqual( a.Union( a ), a );
			Assert.AreEqual( a.Union( new IntDomain() ), a );
		}

		[Test]
		public void EmptyDifference()
		{
			IntDomain a	= new IntDomain( 8, 55 );
			
			Assert.IsTrue( a.Difference( a ).IsEmpty() );
			Assert.AreEqual( a.Difference( new IntDomain() ), a );
		}

		[Test]
		public void EmptyIntersect()
		{
			IntDomain a	= new IntDomain( 8, 55 );
			
			Assert.AreEqual( a.Intersect( a ), a );
			Assert.IsTrue( a.Intersect( new IntDomain() ).IsEmpty() );
		}

		[Test]
		public void Enumerator()
		{
			List<IntInterval> l1	= new List<IntInterval>( 3 );
			l1.Add( new IntInterval( 8, 23 ) );
			l1.Add( new IntInterval( 40, 55 ) );
			l1.Add( new IntInterval( 72, 87 ) );
		
			IntDomain a	= new IntDomain( l1 );
			
			List<IntInterval> list	= new List<IntInterval>( a );
		}
	}
}
