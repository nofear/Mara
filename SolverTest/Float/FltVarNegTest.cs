using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraInterval.Interval;
using MaraSolver.Float;

using NUnit.Framework;

namespace SolverTest.Float
{
	[TestFixture]
	public class FltVarNegTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -100, 100 );
			FltVar a		= new FltVar( solver, 5, 10, "a" );
			FltVarNeg neg	= -a;
			FltVar b		= neg.Var0;

			solver.Add( neg );
			solver.Propagate();
			
			Assert.AreEqual( b.Domain, new FltDomain( -10, -5 ) );
		}
	}
}
