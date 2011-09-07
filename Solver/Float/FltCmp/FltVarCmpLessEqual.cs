//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmpLessEqual.cs $
 * 
 * 26    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 25    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 24    4-03-08 23:39 Patrick
 * use Epsilon compare
 * 
 * 23    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 22    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 21    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 20    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 19    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 18    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 17    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 16    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 15    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 * 
 * 14    9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 13    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 12    6/02/06 9:14a Patrick
 * synchronized with integer version
 * 
 * 11    14-03-06 22:07 Patrick
 * added integer & float namespace
 * 
 * 10    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 9     16-02-06 22:32 Patrick
 * updated class description
 * 
 * 8     19-01-06 21:40 Patrick
 * using Var.Domain instead of proxy code
 * 
 * 7     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 6     6/28/05 10:11p Patrick
 * using generics, makes int/flt const classes obsolete
 * 
 * 5     6/05/05 10:59p Patrick
 * splitted IsViolated() functionality
 * added IsVarViolated()
 * 
 * 4     26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 * 
 * 3     23-05-05 22:04 Patrick
 * renamed MinBound/MaxBound => Min/Max
 * 
 * 2     13-05-05 23:16 Patrick
 * added ToString()
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var0 <= var1, where both variables are float.
	/// </summary>
	public class FltVarCmpLessEqual : FltVarCmp
	{
		public FltVarCmpLessEqual( FltVar var0, FltVar var1 ) :
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
						&& !Epsilon.LessEqual( Var0.Value, Var1.Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Math.Min( Var0.Min, Var1.Min ), Math.Min( Var0.Max, Var1.Max ) );
			Var1.Intersect( Math.Max( Var0.Min, Var1.Min ), Math.Max( Var0.Max, Var1.Max ) );
		}
	}
}
