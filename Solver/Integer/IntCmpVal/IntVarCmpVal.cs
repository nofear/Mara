//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmpVal/IntVarCmpVal.cs $
 * 
 * 26    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 25    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 24    31-07-07 19:29 Patrick
 * fixed Copy(..)
 * 
 * 23    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 22    27-06-07 22:17 Patrick
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
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Base class for the constraint that implements comparison between a
	/// variable and a value.
	/// </summary>
	public abstract class IntVarCmpVal : ConstraintVar0<IntVar>
	{
		protected IntVarCmpVal( IntVar var0, int val ) :
			base( var0 )
		{
			m_Value		= val;
		}

		protected string ToString( string compare, bool wd )
		{
			return Var0.ToString( wd ) + compare + m_Value.ToString( CultureInfo.CurrentCulture );
		}

		public int Value
		{
			get
			{
				return m_Value;
			}

			set
			{
				m_Value		= value;
			}
		}

		int		m_Value;
	}
}
