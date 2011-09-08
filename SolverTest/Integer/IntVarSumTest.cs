using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;

using NUnit.Framework;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarSumTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -1000, 1000 );
			IntVar i0	= new IntVar( solver, 0, 10 );
			IntVar i1	= new IntVar( solver, 10, 20 );
			IntVar i2	= new IntVar( solver, 20, 30 );
			IntVar s	= new IntVar( solver, 30, 60 );

			IntVarList list		= new IntVarList( solver, new IntVar[] { i0, i1, i2 } );
			IntVarListSum sum	= list.Sum();

			solver.Add( sum );
			solver.Propagate();
			
			Assert.AreEqual( s.Domain, sum.Var0.Domain );
		}
	}
}
