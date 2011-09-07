//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntExprVal/IntVarExprVal.cs $
 * 
 * 29    9/04/08 8:29p Patrick
 * Moved interval classes into own project
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
 * 25    31-07-07 19:55 Patrick
 * fixed copy
 * 
 * 24    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 23    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 22    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 21    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 20    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 19    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 18    2-02-07 23:19 Patrick
 * moved code from IntVarDomain to IntDomain
 * 
 * 17    1-02-07 22:54 Patrick
 * added change flag to variable
 * 
 * 16    2-11-06 22:27 Patrick
 * renamed IntDomain => IntVarDomain
 * 
 * 15    6/02/06 9:15a Patrick
 * updated GetString(..)
 * 
 * 14    5/29/06 9:37p Patrick
 * added operators
 * 
 * 13    14-03-06 22:19 Patrick
 * added constraint namespace
 * 
 * 12    14-03-06 22:07 Patrick
 * added integer & float namespace
 * 
 * 11    14-03-06 21:51 Patrick
 * put classes into proper namespace
 * 
 * 10    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 9     24-01-06 23:25 Patrick
 * 
 * 8     1/13/06 11:46p Patrick
 * renamed IntVarExprVal -> IntVarExprValList
 * added base class IntVarExprVal
 * 
 * 7     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 6     7-01-06 21:35 Patrick
 * refactored ToString()
 * 
 * 5     1/04/06 2:55p Patrick
 * renamed Add( cons ) => WhenVar( cons )
 * 
 * 4     5-01-06 22:49 Patrick
 * refactored to Post() & reversible constraint lists
 * 
 * 3     10/19/05 10:10p Patrick
 * refactored Int/Flt Domain
 * 
 * 2     26-08-05 20:47 Patrick
 * refactored Enable()/Disabled() => property + OnEnable() + OnDisable()
 * removed OnEnable() in constructor
 * 
 * 1     7/12/05 9:38p Patrick
 * added first exprval classes
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Globalization;
using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarExpr.
	/// </summary>
	public abstract class IntVarExprVal : IntVarExpr
	{
		protected IntVarExprVal( IntVar var0, IntVar var1, int val ) :
			base( var0.Solver, new Variable[] { var0, var1 } )
		{
			m_Var0		= var0;
			m_Var1		= var1;
			m_Value		= val;
			m_Domain	= new IntDomain( val );
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

		public int Value
		{
			get
			{
				return m_Value;
			}
		}
	
		public IntDomain Domain
		{
			get
			{
				return m_Domain;
			}
		}

		IntVar		m_Var0;
		IntVar		m_Var1;
		int			m_Value;
		IntDomain	m_Domain;
	}
}
