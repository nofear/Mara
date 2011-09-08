using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer;

using NUnit.Framework;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarListIndexTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -1000, 1000 );
			IntVar a	= new IntVar( solver, -10, -5, "a" );
			IntVar b	= new IntVar( solver, -1, 1, "b" );
			IntVar c	= new IntVar( solver, 5, 10, "c" );
			IntVarList list	= new IntVarList( solver, new IntVar[] { a, b, c } );

			IntVar index			= new IntVar( solver );
			IntVarListIndex cons	= list.At( index );
			IntVar result			= cons.Var0;

			solver.Add( cons );
			solver.Propagate();

			result.Intersect( -8, 8 );
			result.Difference( -2, 6 );
			cons.Index.Difference( 1 );
		}
	}
}
