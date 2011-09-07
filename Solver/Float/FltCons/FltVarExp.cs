//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarExp.cs $
 * 
 * 16    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 15    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 14    14-01-08 20:07 Patrick
 * fixed name
 * 
 * 13    14-01-08 19:55 Patrick
 * fixed typo
 * 
 * 12    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 11    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 10    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 9     27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 8     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 7     11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 6     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 5     21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 4     10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 * 
 * 3     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 2     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 1     23-02-07 3:35 Patrick
 * added FltVarExp
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraSolver.Float;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	public class FltVarExp : ConstraintVar1<FltVar,FltVar>
	{
		public FltVarExp( FltVar var1 ) :
			base( new FltVar( var1.Solver ), var1 )
		{
			//QQQ: could make this more space efficient
			Var0.Name	= "Exp(" + var1.ToString(false) + ")";
		}

		public override string ToString()
		{
			return Var0.ToString();
		}

		public override bool IsViolated()
		{
			return false;
		}

		public override void Update()
		{
			Var0.Intersect( Var1.Domain.Interval.Exp() );
			Var1.Intersect( Var0.Domain.Interval.Log() );
		}
	}
}

//--------------------------------------------------------------------------------
