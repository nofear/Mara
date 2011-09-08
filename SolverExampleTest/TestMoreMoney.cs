using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer;
using MaraSolver.Integer.Search;

namespace SolverExampleTest
{
	[TestFixture]
	public class TestMoreMoney : TestBase
	{
		[Test]
		public void Test()
		{
			MoreMoney mm	= new MoreMoney();
			Solver solver	= mm.Solver;
			
			solver.Solve( new IntGenerate( solver,
									solver.IntVarList.ToArray(),
									IntVarSelector.CardinalityMin,
									new IntSearchInstantiateBest() ) );

			List<int> list	= new List<int>();
			foreach( IntVar var in solver.IntVarList )
			{
				list.Add( var.Value );
			}

			Assert.AreEqual( list.ToArray(), new int[] { 7,5,1,6,0,8,9,2,9567,1085,10652,10652 } );

			int count	= CountSolution( solver );

			Assert.AreEqual( count, 1 );							
		}
	}
}
