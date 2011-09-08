using System;
using MaraSolver;
using MaraSolver.Integer;

namespace SolverExample
{
	public class MoreMoney : Problem
	{
		public MoreMoney() :
			base( 0, 1000000 )
		{
			IntVar d	= new IntVar( m_Solver, 0, 9, "d");
			IntVar e	= new IntVar( m_Solver, 0, 9, "e");
			IntVar m	= new IntVar( m_Solver, 1, 9, "m");
			IntVar n	= new IntVar( m_Solver, 0, 9, "n");
			IntVar o	= new IntVar( m_Solver, 0, 9, "o");
			IntVar r	= new IntVar( m_Solver, 0, 9, "r");
			IntVar s	= new IntVar( m_Solver, 1, 9, "s");
			IntVar y	= new IntVar( m_Solver, 0, 9, "y");

			IntVarList list	= new IntVarList( m_Solver, new IntVar[] { d, e, m, n, o, r, s, y } );
			m_Solver.Add( list.AllDifferent() );
				
			IntVarListDotProduct send		= new IntVarListDotProduct( m_Solver,
															new IntVar[] { s, e, n, d },
															new int[] { 1000, 100, 10, 1 } );
			IntVarListDotProduct more		= new IntVarListDotProduct( m_Solver,
															new IntVar[] { m, o, r, e },
															new int[] { 1000, 100, 10, 1 } );
			IntVarListDotProduct money		= new IntVarListDotProduct( m_Solver,
															new IntVar[] { m, o, n, e, y },
															new int[] { 10000, 1000, 100, 10, 1 } );
			m_Solver.Add( send );
			m_Solver.Add( more );
			m_Solver.Add( money );

			IntVarExpr sendMore = send.Var0 + more.Var0;
			m_Solver.Add( sendMore );

			IntVarCmp cmp = sendMore.Var0 == money.Var0;
			m_Solver.Add( cmp );
		}
	}
}
