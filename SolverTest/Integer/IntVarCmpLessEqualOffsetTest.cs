using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraInterval.Interval;
using MaraSolver.Integer;

using NUnit.Framework;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarCmpLessEqualOffsetTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -10000, 10000 );
			IntVar a		= new IntVar( solver, 0, 20, "a" );
			IntVar b		= new IntVar( solver, 10, 10, "b" );
			IntVarCmpLessEqualOffset cons	= new IntVarCmpLessEqualOffset( a, b, 5 );		// a + 5 <= b
			
			solver.Add( cons );
			solver.Propagate();
			
			Assert.AreEqual( a.Domain, new IntDomain( 0, 5 ) );
		}
	}
}
