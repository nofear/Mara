using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;
using MaraSolver.Integer.Search;

namespace TestApp
{
	public class App
	{
		static public void Execute( Solver solver, string sep )
		{
			DateTime start	= DateTime.Now;

			solver.Solve( new IntGenerate( solver,
									solver.IntVarList.ToArray(),
									IntVarSelector.FirstNotBound,
									new IntSearchDichotomize() ) );
			SearchAll( solver, sep );
			
			TimeSpan span	= DateTime.Now - start;
			
			Console.Out.WriteLine();
			Console.Out.WriteLine( sep + "\t" + span.ToString() );
		}

		static public void SearchAll( Solver solver )
		{
			SearchAll( solver, "." );
		}

		static public void SearchAll( Solver solver, string sep )
		{
			int count	= 1;

			while( solver.Next() )
			{
				if( count % 100 == 0 )
				{
					solver.Out.Write( sep );
				}

				++count;
			}

			solver.Out.WriteLine( ": #" + count.ToString() );
		}
	}
}
