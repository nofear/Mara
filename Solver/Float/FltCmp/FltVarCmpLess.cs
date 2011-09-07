//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmpLess.cs $
 * 
 * 28    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 27    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 26    4-03-08 23:39 Patrick
 * use Epsilon compare
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
 * 
 * 17    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;

using MaraInterval.Utility;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var0 < var1, where both variables are float.
	/// </summary>
	public class FltVarCmpLess : FltVarCmp
	{
		public FltVarCmpLess( FltVar var0, FltVar var1 ) :
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
						&& !Epsilon.Less( Var0.Value, Var1.Value ) );
		}

		public override void Update()
		{
			double min0		= Math.Min( Var0.Min, Epsilon.Prev( Var1.Min ) );
			double max0		= Math.Min( Var0.Max, Epsilon.Prev( Var1.Max ) );

			double min1		= Math.Max( Epsilon.Next( Var0.Min ), Var1.Min );
			double max1		= Math.Max( Epsilon.Next( Var0.Max ), Var1.Max );
		
			Var0.Intersect( min0, max0 );
			Var1.Intersect( min1, max1 );
		}
	}
}
