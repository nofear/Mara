//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmpVal/IntVarCmpValGreater.cs $
 * 
 * 25    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 24    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 23    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 22    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 21    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 20    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 19    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 18    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmpValGreater.
	/// </summary>
	public class IntVarCmpValGreater : IntVarCmpVal
	{
		public IntVarCmpValGreater( IntVar var0, int val1 ) :
			base( var0, val1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( ">", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value > Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Value + 1, int.MaxValue );
		}
	}
}
