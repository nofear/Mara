//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltExprVal/FltVarExprValSubRev.cs $
 * 
 * 17    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 16    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 15    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 14    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 13    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 12    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 11    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 10    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 9     21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 8     10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarExprValSubRev.
	/// </summary>
	public class FltVarExprValSubRev : FltVarExprVal
	{	
		public FltVarExprValSubRev( double val, FltVar var1 ) :
			this( new FltVar( var1.Solver ), val, var1 )
		{
		}

		public FltVarExprValSubRev( FltVar var0, double val, FltVar var1 ) :
			base( var0, var1, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "-", true, wd );
		}

		public override bool IsViolated()
		{
			return  ( IsBound()
						&& !( Var0.Value == Value - Var1.Value ) );
		}

		public override void Update()
		{
			// v0	= val - v1
			// v1	= val - v0
			Var0.Intersect( Domain - Var1.Domain );
			Var1.Intersect( Domain - Var0.Domain );
		}	
	}
}
