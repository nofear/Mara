using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer.Search;

namespace SolverExampleTest
{
	[TestFixture]
	public class TestMagicSquare : TestBase
	{
		[Test]
		public void IntSearchDichotomize()
		{
			Test( new IntSearchDichotomize() );
		}

		[Test]
		public void IntSearchInstantiate()
		{
			Test( new IntSearchInstantiate() );
		}

		[Test]
		public void IntSearchInstantiateBest()
		{
			Test( new IntSearchInstantiateBest() );
		}

		private void Test( IntSearch search )
		{
			MagicSquare ms	= new MagicSquare( 4 );
			Solver solver	= ms.Solver;

			solver.Solve( new IntGenerate( solver,
									solver.IntVarList.ToArray(),
									IntVarSelector.CardinalityMin,
									search ) );
									
			int count	= CountSolution( solver );

			Assert.AreEqual( count, 880 );							
		}
	}
}
