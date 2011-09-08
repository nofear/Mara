using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer.Search;
using MaraSolver.Integer;

namespace TestApp
{
	public class AppMagicSquare : App
	{
		static public void MagicSquare()
		{
			Thread.CurrentThread.Priority	= ThreadPriority.Highest;
		
			MagicSquare ms	= new MagicSquare( 4 );
			Solver solver	= ms.Solver;

			solver.Solve( new IntGenerate( solver,
									solver.IntVarList.ToArray(),
									IntVarSelector.CardinalityMin,
									new IntSearchDichotomize() ) );

			//solver.Out.Write( ms.Matrix.ToString() );
			//solver.PrintInformation();
			//solver.PrintConstraints();
			//solver.PrintVariables();
			solver.Out.WriteLine();

			SearchAll( solver );
							
			solver.PrintConstraints();
			solver.PrintInformation();
		}

		static public void MagicSquare1()
		{
			DateTime start	= DateTime.Now;
		
			MagicSquare ms	= new MagicSquare( 4 );
			Solver solver	= ms.Solver;

			ThreadStart starter1	= delegate { Execute ( solver, "1" ); };
			Thread t1				= new Thread( starter1 );
			t1.Priority	= ThreadPriority.AboveNormal;
			t1.Start();
			t1.Join();
			
			TimeSpan span	= DateTime.Now - start;
			
			Console.Out.WriteLine( span.ToString() );
			Console.Out.WriteLine();
		}
	}
}
