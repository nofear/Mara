//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmpVal/FltVarCmpValEqual.cs $
 * 
 * 29    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 28    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 27    5-03-08 0:34 Patrick
 * use Epsilon
 * 
 * 26    29-02-08 22:21 Patrick
 * 
 * 25    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 24    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 23    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 22    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 21    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 20    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 19    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 18    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var == value.
	/// </summary>
	public class FltVarCmpValEqual : FltVarCmpVal
	{
		public FltVarCmpValEqual( FltVar var0, double val ) :
			base( var0, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "==", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !Epsilon.Equal( Var0.Value, Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( m_Value );
		}
	}
}
