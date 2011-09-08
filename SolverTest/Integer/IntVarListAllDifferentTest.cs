using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;
using MaraInterval.Interval;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarListAllDifferentTest
	{
		[Test]
		public void High()
		{
			Solver solver	= new Solver( -10000, 10000 );
			IntVar a	= new IntVar( solver, 1, 3, "a" );
			IntVar b	= new IntVar( solver, 1, 2, "b" );
			IntVar c	= new IntVar( solver, 1, 2, "c" );

			IntVarListAllDifferent cons	= new IntVarListAllDifferent(
				solver,
				new IntVar[] { a, b, c } );
			
			cons.Level	= PropagateLevel.High;
			
			solver.Add( cons );
			solver.Propagate();

			Assert.AreEqual( a.Domain, new IntDomain( 3, 3 ) );
			Assert.AreEqual( b.Domain, new IntDomain( 1, 2 ) );
			Assert.AreEqual( c.Domain, new IntDomain( 1, 2 ) );
		}
	}
}
