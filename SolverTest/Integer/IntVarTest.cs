using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MaraSolver;
using MaraSolver.Integer;

namespace SolverTest.Integer
{
	[TestFixture]
	public class IntVarTest
	{
		[Test]
		public void Enumerator()
		{
			Solver solver	= new Solver( -1000, 1000 );
			IntVar var		= new IntVar( solver, 0 );

			var.Difference( 0 );
			
			foreach( int x in var )
			{
				Assert.Fail();
			}
		}
	}
}
