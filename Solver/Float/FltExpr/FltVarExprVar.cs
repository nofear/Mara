//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltExpr/FltVarExprVar.cs $
 * 
 * 27    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 26    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 25    10-01-08 23:41 Patrick
 * added additional operators
 * 
 * 24    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 23    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 22    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 21    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 20    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 19    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 18    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 17    2/27/07 1:39a Patrick
 * added override's to keep the compiler happy
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarExpr.
	/// </summary>
	public abstract class FltVarExprVar : FltVarExpr
	{
		protected FltVarExprVar( FltVar var0, FltVar var1, FltVar var2 ) :
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

		public override FltVar Var0
		{
			get
			{
				return m_Var0;
			}
		}

		public FltVar Var1
		{
			get
			{
				return m_Var1;
			}
		}

		public FltVar Var2
		{
			get
			{
				return m_Var2;
			}
		}

		FltVar		m_Var0;
		FltVar		m_Var1;
		FltVar		m_Var2;
	}
}
