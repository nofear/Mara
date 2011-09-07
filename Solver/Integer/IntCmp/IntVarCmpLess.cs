//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmp/IntVarCmpLess.cs $
 * 
 * 26    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
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
 * 22    27-06-07 22:17 Patrick
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
 * 
 * 17    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmpLess.
	/// </summary>
	public class IntVarCmpLess : IntVarCmp
	{
		public IntVarCmpLess( IntVar var0, IntVar var1 ) :
			base( var0, var1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "<", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value < Var1.Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Math.Min( Var0.Min, Var1.Min - 1 ), Math.Min( Var0.Max, Var1.Max - 1 ) );
			Var1.Intersect( Math.Max( Var0.Min + 1, Var1.Min ), Math.Max( Var0.Max + 1, Var1.Max ) );
		}
	}
}
