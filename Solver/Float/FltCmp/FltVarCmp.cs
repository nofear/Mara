//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmp.cs $
 * 
 * 21    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 20    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 19    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 18    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 17    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 16    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 15    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 14    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 13    1-02-07 22:54 Patrick
 * added change flag to variable
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarCmpEqual.
	/// </summary>
	public abstract class FltVarCmp : ConstraintVar1<FltVar,FltVar>
	{
		protected FltVarCmp( FltVar var0, FltVar var1 ) :
			base( var0, var1 )
		{
		}

		protected string ToString( string compare, bool wd )
		{
			return Var0.ToString( wd ) + compare + Var1.ToString( wd );
		}
	}
}
