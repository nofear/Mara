//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltExprVal/FltVarExprVal.cs $
 * 
 * 13    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 12    10-01-08 23:41 Patrick
 * added additional operators
 * 
 * 11    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 10    31-07-07 19:29 Patrick
 * fixed Copy(..)
 * 
 * 9     5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 8     27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 7     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 6     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 5     21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 4     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 3     1-02-07 22:54 Patrick
 * added change flag to variable
 * 
 * 2     6/02/06 9:14a Patrick
 * synchronized with integer version
 * 
 * 1     6/01/06 11:13a Patrick
 * converted from int classes
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Globalization;
using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarExpr.
	/// </summary>
	public abstract class FltVarExprVal : FltVarExpr
	{
		protected FltVarExprVal( FltVar var0, FltVar var1, double val ) :
			base( var0.Solver, new Variable[] { var0, var1 } )
		{
			m_Var0		= var0;
			m_Var1		= var1;
			m_Value		= val;
			m_Domain	= new FltDomain( val );
		}

		protected string ToString( string expr, bool rev, bool wd )
		{
			return Var0.ToString( wd ) + "=" + GetExprString( expr, rev, wd );
		}

		private string GetExprString( string expr, bool rev, bool wd )
		{
			string s1	= Var1.ToString( wd );
			string s2	= Value.ToString( CultureInfo.CurrentCulture );

			return	  ( !rev )
					? s1 + expr + s2
					: s2 + expr + s1;
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

		public double Value
		{
			get
			{
				return m_Value;
			}
		}
	
		public FltDomain Domain
		{
			get
			{
				return m_Domain;
			}
		}

		FltVar		m_Var0;
		FltVar		m_Var1;
		double		m_Value;
		FltDomain	m_Domain;
	}
}
