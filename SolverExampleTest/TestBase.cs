using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver;

namespace SolverExampleTest
{
	public class TestBase
	{
		static public int CountSolution( Solver solver )
		{
			int count	= 1;
			while( solver.Next() )
			{
				if( count % 100 == 0 )
				{
					Console.Out.Write( "." );
				}
			
				++count;
			}

			Console.Out.WriteLine( " #" + count.ToString() );
		
			return count;
		}
	}
}
