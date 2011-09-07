//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmp/IntVarCmpLessEqual.cs $
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
 * 
 * 17    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 16    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 * 
 * 15    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 14    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 13    6/02/06 9:15a Patrick
 * updated GetString(..)
 * 
 * 12    14-03-06 22:07 Patrick
 * added integer & float namespace
 * 
 * 11    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 10    19-01-06 21:40 Patrick
 * using Var.Domain instead of proxy code
 * 
 * 9     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 8     7-01-06 21:34 Patrick
 * refactored ToString()
 * 
 * 7     6/28/05 10:11p Patrick
 * using generics, makes int/flt const classes obsolete
 * 
 * 6     6/05/05 11:00p Patrick
 * splitted IsViolated() functionality
 * added IsVarViolated()
 * 
 * 5     26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 * 
 * 4     23-05-05 22:04 Patrick
 * renamed MinBound/MaxBound => Min/Max
 * 
 * 3     13-05-05 23:18 Patrick
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmpLessEqual.
	/// </summary>
	public class IntVarCmpLessEqual : IntVarCmp
	{
		public IntVarCmpLessEqual( IntVar var0, IntVar var1 ) :
			base( var0, var1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "<=", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value <= Var1.Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Math.Min( Var0.Min, Var1.Min ), Math.Min( Var0.Max, Var1.Max ) );
			Var1.Intersect( Math.Max( Var0.Min, Var1.Min ), Math.Max( Var0.Max, Var1.Max ) );
		}
	}
}
