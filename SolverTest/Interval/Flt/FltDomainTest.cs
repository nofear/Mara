using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;
using MaraInterval.Utility;

using NUnit.Framework;

namespace SolverTest.Interval.Flt
{
	[TestFixture]
	public class FltDomainTest
	{
		private void AssertEqual( FltDomain lhs, FltInterval[] rhs )
		{
			int index	= 0;
			foreach( FltInterval interval in lhs )
			{
				Assert.AreEqual( interval, rhs[ index++ ] );
			}
		}

		private void AssertEqual( FltDomain lhs, double[] rhs )
		{
			int idx	= 0;
			foreach( FltInterval interval in lhs )
			{
				Assert.AreEqual( interval, new FltInterval( rhs[ idx * 2 ], rhs[ idx * 2 + 1 ] ) );
				++idx;
			}
		}

		private FltDomain Setup()
		{
			return new FltDomain( new double[] { -20, -10, -5, 5, 10, 20 } );
		}

		[Test]
		public void Contains()
		{
			Assert.IsTrue( Setup().Contains( 0 ) );
			Assert.IsTrue( Setup().Contains( new FltInterval( -4, 4 ) ) );
		}

		[Test]
		public void UnionInterval()
		{
			FltDomain a		= Setup();
			FltDomain a1	= a.Union( -30, -25 );	AssertEqual( a1, new FltInterval[] { new FltInterval( -30, -25 ), new FltInterval( -20, -10 ), new FltInterval( -5, 5 ), new FltInterval( 10, 20 ) } );
			FltDomain a2	= a.Union( -30, -15 );	AssertEqual( a2, new FltInterval[] { new FltInterval( -30, -10 ), new FltInterval( -5, 5 ), new FltInterval( 10, 20 ) } );
			FltDomain a3	= a.Union( -30, -2 );	AssertEqual( a3, new FltInterval[] { new FltInterval( -30, 5 ), new FltInterval( 10, 20 ) } );
			FltDomain a4	= a.Union( -8, -7 );	AssertEqual( a4, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -8, -7 ), new FltInterval( -5, 5 ), new FltInterval( 10, 20 ) } );
			FltDomain a5	= a.Union( -12, -2 );	AssertEqual( a5, new FltInterval[] { new FltInterval( -20, 5 ), new FltInterval( 10, 20 ) } );
			FltDomain a6	= a.Union( -6, 6 );		AssertEqual( a6, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -6, 6 ), new FltInterval( 10, 20 ) } );
			FltDomain a7	= a.Union( -12, 12 );	AssertEqual( a7, new FltInterval[] { new FltInterval( -20, 20 ) } );
			FltDomain a8	= a.Union( 2, 12 );		AssertEqual( a8, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -5, 20 ) } );
			FltDomain a9	= a.Union( 7, 8 );		AssertEqual( a9, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -5, 5 ), new FltInterval( 7, 8 ), new FltInterval( 10, 20 ) } );
			FltDomain a10	= a.Union( 2, 30 );		AssertEqual( a10, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -5, 30 ) } );
			FltDomain a11	= a.Union( 15, 30 );	AssertEqual( a11, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -5, 5 ), new FltInterval( 10, 30 ) } );
			FltDomain a12	= a.Union( 25, 35 );	AssertEqual( a12, new FltInterval[] { new FltInterval( -20, -10 ), new FltInterval( -5, 5 ), new FltInterval( 10, 20 ), new FltInterval( 25, 35 ) } );
		}

		[Test]
		public void UnionDomain()
		{
			FltDomain a		= Setup();
			FltDomain a1	= a.Union( new FltDomain( new double[] { -15, 15 } ) );	Assert.IsTrue( a1.IsSimple() ); AssertEqual( a1, new FltInterval[] { new FltInterval( -20, 20 ) } );
			FltDomain a2	= a.Union( new FltDomain( new double[] { -15, 20 } ) );	Assert.IsTrue( a2.IsSimple() ); AssertEqual( a2, new FltInterval[] { new FltInterval( -20, 20 ) } );
		}
		
		[Test]
		public void DifferenceInterval()
		{
			FltDomain a		= new FltDomain( new double[] { 0, 10, 20, 30 } );
			FltDomain a1	= a.Difference( a.Interval );													Assert.AreEqual( FltDomain.Empty, a1 ); 
			FltDomain a2	= a.Difference( new FltInterval( -10, Epsilon.Prev( 0 ) ) );					Assert.AreEqual( a, a2 );		// 6
			FltDomain a3	= a.Difference( new FltInterval( Epsilon.Next( 10 ), Epsilon.Prev( 20 ) ) );	Assert.AreEqual( a, a3 );		// 4
			FltDomain a4	= a.Difference( new FltInterval( Epsilon.Next( 30 ), 40 ) );					Assert.AreEqual( a, a4 );		// 1
			FltDomain a5	= a.Difference( new FltInterval( -10, 0 ) );									AssertEqual( a5, new double[] { Epsilon.Next( 0 ), 10, 20, 30 } );

			FltDomain b		= new FltDomain( new double[] { 0, 30 } );
			FltDomain b1	= b.Difference( new FltInterval( 10, 20 ) );		AssertEqual( b1, new FltInterval[] { new FltInterval( 0, Epsilon.Prev( 10 ) ), new FltInterval( Epsilon.Next( 20 ), 30 ) } );
			FltDomain b2	= b.Difference( new FltInterval( -10, 10 ) );		AssertEqual( b2, new FltInterval[] { new FltInterval( Epsilon.Next( 10 ), 30 ) } );
			FltDomain b3	= b.Difference( new FltInterval( 20, 40 ) );		AssertEqual( b3, new FltInterval[] { new FltInterval( 0, Epsilon.Prev( 20 ) ) } );
		}

		[Test]
		public void DifferenceDomain()
		{
			FltDomain a		= new FltDomain( new double[] { 0, 10, 20, 30, 40, 50, 60, 70 } );
			FltDomain a1	= a.Difference( new FltDomain( new double[] { -10, -5, 15, 25, 45, 55, 59, 71 } ) );

			FltDomain b		= new FltDomain( new double[] { 0, 100 } );
			FltDomain b1	= b.Difference( new FltDomain( new double[] { 10, 20, 30, 40, 50, 60, 70, 80 } ) );
		}

		[Test]
		public void IntersectInterval()
		{
			FltDomain a		= new FltDomain( new double[] { 0, 10, 20, 30 } );
			FltDomain a1	= a.Intersect( a.Interval );							Assert.AreEqual( a, a1 );
			FltDomain a2	= a.Intersect( new FltInterval( -10, 5 ) );				AssertEqual( a2, new double[] { 0, 5 } );
			FltDomain a3	= a.Intersect( new FltInterval( 25, 40 ) );				AssertEqual( a3, new double[] { 25, 30 } );
			FltDomain a4	= a.Intersect( new FltInterval( 5, 25 ) );				AssertEqual( a4, new double[] { 5, 10, 20, 25 } );
			FltDomain a5	= a.Intersect( new FltInterval( -10, 25 ) );			AssertEqual( a5, new double[] { 0, 10, 20, 25 } );
			FltDomain a6	= a.Intersect( new FltInterval( 5, 40 ) );				AssertEqual( a6, new double[] { 5, 10, 20, 30 } );
		}

		[Test]
		public void IntersectDomain()
		{
			FltDomain a		= new FltDomain( new double[] { 0, 100 } );
			FltDomain b		= new FltDomain( new double[] { 10, 20, 30, 40, 50, 60 } );
			FltDomain a1	= a.Intersect( new FltDomain( b ) );					Assert.AreEqual( a1, b );

			FltDomain a2	= a.Intersect( new FltDomain( new double[] { -15, -10, -5, 5, 45, 55, 95, 105, 110, 120 } ) );		AssertEqual( a2, new double[] { 0, 5, 45, 55, 95, 100 } );
			FltDomain a3	= a.Intersect( new FltDomain( new double[] { -10, 110, 120, 130  } ) );								AssertEqual( a3, new double[] { 0, 100 } );
		}

		[Test]
		public void IntersectsWith4()
		{
			FltDomain a	= new FltDomain( new double[] { 0, 15, 48, 63 } );
			
			Assert.IsFalse( a.IntersectsWith( new FltInterval( -10, Epsilon.Prev( 0 ) ) ) );
			Assert.IsFalse( a.IntersectsWith( new FltInterval( Epsilon.Next( 15 ), Epsilon.Prev( 48 ) ) ) );
			Assert.IsFalse( a.IntersectsWith( new FltInterval( Epsilon.Next( 63 ), 80 ) ) );

			Assert.IsTrue( a.IntersectsWith( new FltInterval( 15, Epsilon.Prev( 48 ) ) ) );
			Assert.IsTrue( a.IntersectsWith( new FltInterval( Epsilon.Next( 15 ), 48 ) ) );
		}

		[Test]
		public void IntersectsWithDomain()
		{
			FltDomain a		= new FltDomain( new double[] { 0, 10, 20, 30 } );
			FltDomain a1	= new FltDomain( new double[] { -10, Epsilon.Prev( 0 ), Epsilon.Next( 10 ), Epsilon.Prev( 20 ), Epsilon.Next( 30 ), 40 } );

			Assert.IsFalse( a.IntersectsWith( a1 ) );
			Assert.IsFalse( a1.IntersectsWith( a ) );
		}

		[Test]
		public void EmptyUnion()
		{
			FltDomain a	= new FltDomain( 8, 55 );

			Assert.AreEqual( a.Union( a ), a );
			Assert.AreEqual( a.Union( new FltDomain() ), a );
		}

		[Test]
		public void EmptyDifference()
		{
			FltDomain a	= new FltDomain( 8, 55 );
			
			Assert.IsTrue( a.Difference( a ).IsEmpty() );
			Assert.AreEqual( a.Difference( new FltDomain() ), a );
		}

		[Test]
		public void EmptyIntersect()
		{
			FltDomain a	= new FltDomain( 8, 55 );
			
			Assert.AreEqual( a.Intersect( a ), a );
			Assert.IsTrue( a.Intersect( new FltDomain() ).IsEmpty() );
		}

		[Test]
		public void Enumerator()
		{
			List<FltInterval> l1	= new List<FltInterval>( 3 );
			l1.Add( new FltInterval( 8, 23 ) );
			l1.Add( new FltInterval( 40, 55 ) );
			l1.Add( new FltInterval( 72, 87 ) );
		
			FltDomain a	= new FltDomain( l1 );
			
			List<FltInterval> list	= new List<FltInterval>( a );
		}
	}
}
