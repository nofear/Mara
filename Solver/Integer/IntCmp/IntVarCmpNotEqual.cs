//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCmp/IntVarCmpNotEqual.cs $
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
 * 21    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 20    27-06-07 22:17 Patrick
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
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarCmpNotEqual.
	/// </summary>
	public class IntVarCmpNotEqual : IntVarCmp
	{
		public IntVarCmpNotEqual( IntVar var0, IntVar var1 ) :
			base( var0, var1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "!=", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value != Var1.Value ) );
		}

		public override void Post()
		{
			Var0.OnVarIsBoundList.Add( this );
			Var1.OnVarIsBoundList.Add( this );
		}

		public override void Update()
		{
			if( Var1.IsBound() )
			{
				Var0.Difference( Var1.Value );
			}

			if( Var0.IsBound() )
			{
				Var1.Difference( Var0.Value );
			}
		}
	}
}
