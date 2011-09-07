//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmp/IntVarCmp.cs $
 * 
 * 25    2/22/09 2:42p Patrick
 * 
 * 24    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 23    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 22    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 21    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 20    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 19    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 18    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 17    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 16    1-02-07 22:54 Patrick
 * added change flag to variable
 * 
 * 15    6/02/06 9:15a Patrick
 * updated GetString(..)
 * 
 * 14    6/01/06 1:09p Patrick
 * added 'withDomain' parameter to ToString()
 * 
 * 13    14-03-06 22:19 Patrick
 * added constraint namespace
 * 
 * 12    14-03-06 22:07 Patrick
 * added integer & float namespace
 * 
 * 11    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 10    8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 9     7-01-06 21:34 Patrick
 * refactored ToString()
 * 
 * 8     5-01-06 22:49 Patrick
 * refactored to Post() & reversible constraint lists
 * 
 * 7     26-08-05 20:47 Patrick
 * refactored Enable()/Disabled() => property + OnEnable() + OnDisable()
 * removed OnEnable() in constructor
 * 
 * 6     6/28/05 10:11p Patrick
 * using generics, makes int/flt const classes obsolete
 * 
 * 5     3-06-05 19:53 Patrick
 * 
 * 4     26-05-05 23:02 Patrick
 * renamed classes
 * 
 * 3     26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 * 
 * 2     18-05-05 22:02 Patrick
 * renamed Register()/Unregister() => Enable()/Disable()
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmp.
	/// </summary>
	public abstract class IntVarCmp: ConstraintVar1<IntVar,IntVar>
	{
		protected IntVarCmp( IntVar var0, IntVar var1 ) :
			base( var0, var1 )
		{
		}

		protected string ToString( string compare, bool wd )
		{
			return Var0.ToString( wd ) + compare + Var1.ToString( wd );
		}

		public override void Post()
		{
			Post( Variable.How.OnInterval );
		}
	}
}
