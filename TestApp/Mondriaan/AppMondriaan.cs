using System.Threading;

using MaraSolver;
using MaraSolver.Integer.Search;

namespace TestApp {
	public class AppMondriaan: App {
		static public void Mondriaan() {
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			Mondriaan m = new Mondriaan(4);
			Solver solver = m.Solver;

			solver.Solve(new IntGenerate(solver,
									m.Matrix.VarList.ToArray(),
									IntVarSelector.FirstNotBound,
									new IntSearchInstantiate(IntVarValueSelector.Max)));






			//solver.Out.Write( ms.Matrix.ToString() );
			//solver.PrintInformation();
			//solver.PrintConstraints();
			//solver.PrintVariables();
			solver.Out.WriteLine();

			int count = 1;

			solver.Out.WriteLine("Solution #" + count);
			solver.Out.WriteLine("Area: " + m.Area);
			solver.Out.WriteLine("Objective: " + solver.IntObjective.Value);
			solver.Out.WriteLine(m.Matrix);


			while(solver.Next()) {
				++count;

				solver.Out.WriteLine("Solution #" + count);
				solver.Out.WriteLine("Area: " + m.Area);
				solver.Out.WriteLine("Objective: " + solver.IntObjective.Value);
				solver.Out.WriteLine(m.Matrix);
			}

			solver.PrintConstraints();
			solver.PrintInformation();
		}

	}

}
