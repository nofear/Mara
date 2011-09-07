//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarNeg.cs $
 * 
 * 18    9/04/08 8:29p Patrick
 * Moved interval classes into own project
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
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	public class IntVarNeg : ConstraintVar1<IntVar,IntVar>
	{
		public IntVarNeg( IntVar var1 ) :
			base( new IntVar( var1.Solver ), var1 )
		{
			//QQQ: could make this more space efficient
			Var0.Name	= "-" + var1.ToString();
		}

		public override string ToString()
		{
			return Var0.ToString();
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value == -Var1.Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( -Var1.Domain );
			Var1.Intersect( -Var0.Domain );
		}
	}
}

//--------------------------------------------------------------------------------
