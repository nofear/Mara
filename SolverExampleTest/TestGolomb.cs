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
	public class TestGolomb : TestBase
	{
		[Test] public void Test03() { Test( 3 ); }
		[Test] public void Test04() { Test( 4 ); }
		[Test] public void Test05() { Test( 5 ); }
		[Test] public void Test06() { Test( 6 ); }
		[Test] public void Test07() { Test( 7 ); }
		[Test] public void Test08() { Test( 8 ); }
		[Test] public void Test09() { Test( 9 ); }
		[Test] public void Test10() { Test( 10 ); }

		public void Test( int n )
		{
			int[] list	= new int[] { 1, 3, 6, 11, 17, 25, 34, 44, 55, 72 };
		
			int optimum		= Golomb( n );
			
			Assert.AreEqual( optimum, list[ n - 2 ] );
		}

		public int Golomb( int n )
		{
			Golomb golomb		= new Golomb( n );
			Solver solver		= golomb.Solver;
			solver.IntObjective.Var		= golomb.MarkList[ golomb.MarkList.Count - 1 ];
			solver.IntObjective.Step	= 1;

			SolutionGoal solution	= new SolutionGoal( solver, solver.IntObjective.Var );

			solver.Solve(	new GoalAnd(
								new IntGenerate( solver,
									golomb.MarkList.ToArray(),
									IntVarSelector.FirstNotBound,
									new IntSearchInstantiateBest() ),
								solution ) );

			int count	= CountSolution( solver );

			return solution.Value;
		}

		public class SolutionGoal : Goal
		{
			public SolutionGoal( Solver solver, IntVar var ) :
				base( solver )
			{
				m_Variable	= var;
				m_Value		= m_Variable.Domain.Max;
			}

			public override void Execute()
			{
				m_Value		= m_Variable.Value;
			}

			public int Value
			{
				get
				{
					return m_Value;
				}
			}

			private IntVar	m_Variable;
			private int		m_Value;
		};
	}
}
