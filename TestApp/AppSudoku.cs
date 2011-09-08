using System;
using System.Collections.Generic;
using System.Text;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer.Search;
using System.IO;

namespace TestApp
{
	class AppSudoku : App
	{
		static public void Sudoku1()
		{
			int[] s2 = { 0, 6, 0, 1, 0, 4, 0, 5, 0,
							0, 0, 8, 3, 0, 5, 6, 0, 0,
							2, 0, 0, 0, 0, 0, 0, 0, 1,
							8, 0, 0, 4, 0, 7, 0, 0, 6,
							0, 0, 6, 0, 0, 0, 3, 0, 0,
							7, 0, 0, 9, 0, 1, 0, 0, 4,
							5, 0, 0, 0, 0, 0, 0, 0, 2,
							0, 0, 7, 2, 0, 6, 9, 0, 0,
							0, 4, 0, 5, 0, 8, 0, 7, 0 };

			int[] evil = {	4, 2, 0,   0, 0, 0,   0, 1, 0,
							0, 0, 0,   5, 4, 0,   0, 3, 0,
							0, 0, 6,   0, 0, 7,   0, 0, 0,

							0, 0, 0,   0, 0, 0,   2, 7, 9,
							0, 1, 0,   0, 0, 0,   0, 6, 0,
							3, 4, 2,   0, 0, 0,   0, 0, 0,

							0, 0, 0,   9, 0, 0,   3, 0, 0,
							0, 6, 0,   0, 3, 8,   0, 0, 0,
							0, 8, 0,   0, 0, 0,   0, 5, 7 };

				Sudoku soduku	= new Sudoku( s2 );
				Solver solver	= soduku.Solver;

				solver.Solve( new IntGenerate( solver,
										soduku.Matrix.VarList.ToArray(),
										IntVarSelector.CardinalityMin,
										new IntSearchInstantiateBest() ) );

				solver.GoalStack.PrintOrStack( solver.Out );

				solver.PrintInformation();

				solver.Out.WriteLine();
				solver.Out.WriteLine( soduku.Matrix.ToString() );

				SearchAll( solver );

				solver.Out.Write( "\t" + solver.Time.ToString());
				solver.Out.WriteLine();
		}		

		static public void Sudoku()
		{
			DateTime time	= DateTime.Now;

			int max		= 0;

			StreamReader file	= new StreamReader( "X:\\Dev\\sudoku.txt" );
			int index = 0;
			while( !file.EndOfStream
						&& index < 250 )
			{
				string str	= file.ReadLine();
		
				Sudoku sodoku	= new Sudoku( str );
				Solver solver	= sodoku.Solver;
				solver.Solve( new IntGenerate( solver,
										solver.IntVarList.ToArray(),
										IntVarSelector.CardinalityMin,
										new IntSearchInstantiateBest() ) );

				SearchAll( solver );

				//solver.Out.WriteLine( solver.Time.ToString() );

				int maxTmp	= solver.GoalStack.StackOrCount;
				if( max <= maxTmp )
				{
					max		= maxTmp;
				}

				++index;
			}

			Console.Out.WriteLine();
			Console.Out.WriteLine( max.ToString() );
			Console.Out.WriteLine( ( DateTime.Now - time ).ToString() + " ms" );
		}

	}
}
