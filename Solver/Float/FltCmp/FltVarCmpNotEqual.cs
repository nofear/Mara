//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmpNotEqual.cs $
 * 
 * 25    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 24    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 23    4-03-08 23:39 Patrick
 * use Epsilon compare
 * 
 * 22    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 21    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 20    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 19    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 18    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 17    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 16    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 15    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var0 != var1, where both variables are float.
	/// </summary>
	public class FltVarCmpNotEqual : FltVarCmp
	{
		public FltVarCmpNotEqual( FltVar var0, FltVar var1 ) :
			base( var0, var1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "!=", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& Epsilon.Equal( Var0.Value, Var1.Value ) );
		}

		public override void Update()
		{
			if( Var1.IsBound() )
			{
				Var0.Difference( Var1.Value );
			}

			if( Var0.IsBound() )
			{
				Var1.Difference( Var0.Value );
			}
		}
	}
}
