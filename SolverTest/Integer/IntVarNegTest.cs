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
	public class IntVarNegTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -100, 100 );
			IntVar a		= new IntVar( solver, 5, 10, "a" );
			IntVarNeg neg	= -a;
			IntVar b		= neg.Var0;

			solver.Add( neg );
			solver.Propagate();
			
			Assert.AreEqual( b.Domain, new IntDomain( -10, -5 ) );
		}
	}
}
