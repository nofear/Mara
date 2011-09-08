using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;
using MaraInterval.Interval;

using NUnit.Framework;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarListDotProductTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -10000, 10000 );
			IntVar a	= new IntVar( solver, 1, 2, "a" );
			IntVar b	= new IntVar( solver, 2, 3, "b" );
			IntVar c	= new IntVar( solver, 3, 4, "c" );
			IntVar d	= new IntVar( solver, 4, 5, "d" );

			IntVarListDotProduct cons	= new IntVarListDotProduct(
				solver,
				new IntVar[] { a, b, c, d },
				new int[] { 1000, 100, 10, 1 } );
			IntVar result	= cons.Var0;
			
			solver.Add( cons );
			solver.Propagate();
			
			Assert.AreEqual( result.Domain, new IntDomain( 1234, 2345 ) );
		}
	}
}
