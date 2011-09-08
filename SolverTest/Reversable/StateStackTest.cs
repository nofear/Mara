using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using MaraSolver;
using MaraSolver.Integer;
using MaraInterval.Interval;

namespace SolverTest.Reversable
{
	[TestFixture]
	public class StateStackTest
	{
		[Test]
		public void PreviousState()
		{
			Solver p	= new Solver( -10000, 10000 );
			IntVar a	= new IntVar( p, 0, 100, "a" );

			p.StateStack.Begin();
			a.Max	= 60;

			p.StateStack.Begin();
			a.Max	= 30;

			p.StateStack.Begin();

			p.StateStack.Begin();
			a.Max	= 10;

			IntDomain d1	= a.DomainPrev;
			Assert.AreEqual( d1, new IntDomain( 0, 30 ) );

			int c1			= d1.Cardinality - a.Domain.Cardinality;
			Assert.AreEqual( c1, 20 );

			p.StateStack.Cancel();

			p.StateStack.Cancel();

			IntDomain d2	= a.DomainPrev;
			Assert.AreEqual( d2, new IntDomain( 0, 60 ) );

			int c2			= d2.Cardinality - a.Domain.Cardinality;
			Assert.AreEqual( c2, 30 );
			
			p.StateStack.Cancel();

			IntDomain d3	= a.DomainPrev;
			Assert.AreEqual( d3, new IntDomain( 0, 100 ) );

			int c3			= d3.Cardinality - a.Domain.Cardinality;
			Assert.AreEqual( c3, 40 );

			p.StateStack.Cancel();

			Assert.AreEqual( a.Domain, new IntDomain( 0, 100 ) );
		}
	}
}
