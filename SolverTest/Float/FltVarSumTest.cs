using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Float;

using NUnit.Framework;

namespace SolverTest.Float
{
	[TestFixture]
	public class FltVarSumTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -1000, 1000 );
			FltVar i0	= new FltVar( solver, 0, 10 );
			FltVar i1	= new FltVar( solver, 10, 20 );
			FltVar i2	= new FltVar( solver, 20, 30 );
			FltVar s	= new FltVar( solver, 30, 60 );

			FltVarList list		= new FltVarList( solver, new FltVar[] { i0, i1, i2 } );
			FltVarListSum sum	= list.Sum();

			solver.Add( sum );
			solver.Propagate();

			Assert.AreEqual( s.Domain, sum.Var0.Domain );
		}
	}
}
