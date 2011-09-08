using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Float;
using MaraSolver.Integer;

using NUnit.Framework;

namespace SolverTest.Float
{
	[TestFixture]
	public class FltVarListIndexTest
	{
		[Test]
		public void Test()
		{
			Solver solver	= new Solver( -1000, 1000 );
			FltVar a	= new FltVar( solver, -10, -5, "a" );
			FltVar b	= new FltVar( solver, -1, 1, "b" );
			FltVar c	= new FltVar( solver, 5, 10, "c" );
			FltVarList list	= new FltVarList( solver, new FltVar[] { a, b, c } );

			IntVar index			= new IntVar( solver );
			FltVarListIndex cons	= list.At( index );
			FltVar result			= cons.Var0;

			solver.Add( cons );
			solver.Propagate();

			result.Intersect( -8, 8 );
			result.Difference( -2, 6 );
			cons.Index.Difference( 1 );
		}
	}
}
