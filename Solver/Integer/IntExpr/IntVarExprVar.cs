//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntExpr/IntVarExprVar.cs $
 * 
 * 28    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 27    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 26    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 25    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 24    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 23    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 22    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 21    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 20    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 19    2/27/07 1:40a Patrick
 * added overrides to keep the compiler happy
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarExpr.
	/// </summary>
	public abstract class IntVarExprVar : IntVarExpr
	{
		protected IntVarExprVar( IntVar var0, IntVar var1, IntVar var2 ) :
			base( var0.Solver, new Variable[] { var0, var1, var2 } )
		{
			m_Var0	= var0;
			m_Var1	= var1;
			m_Var2	= var2;
		}

		protected string ToString( string expr, bool wd )
		{
			return Var0.ToString( wd ) + "=" + GetExprString( expr, wd );
		}
		
		private string GetExprString( string expr, bool wd )
		{
			return m_Var1.ToString( wd ) + expr + m_Var2.ToString( wd );
		}

		public override IntVar Var0
		{
			get
			{
				return m_Var0;
			}
		}

		public IntVar Var1
		{
			get
			{
				return m_Var1;
			}
		}

		public IntVar Var2
		{
			get
			{
				return m_Var2;
			}
		}

		IntVar		m_Var0;
		IntVar		m_Var1;
		IntVar		m_Var2;
	}
}
