//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmpVal/FltVarCmpVal.cs $
 * 
 * 25    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 24    5-03-08 0:34 Patrick
 * use Epsilon
 * 
 * 23    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 22    31-07-07 19:29 Patrick
 * fixed Copy(..)
 * 
 * 21    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 20    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 19    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 18    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 17    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 16    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 15    1-02-07 22:54 Patrick
 * added change flag to variable
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Globalization;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Base class for the constraint that implements comparison between a
	/// variable and a value.
	/// </summary>
	public abstract class FltVarCmpVal : ConstraintVar0<FltVar>
	{
		protected FltVarCmpVal( FltVar var0, double val ) :
			base( var0 )
		{
			m_Value		= val;
		}

		protected string ToString( string compare, bool wd )
		{
			return Var0.ToString( wd ) + compare + m_Value.ToString( CultureInfo.CurrentCulture );
		}

		public double Value
		{
			get
			{
				return m_Value;
			}
		}

		protected double m_Value;
	}
}
