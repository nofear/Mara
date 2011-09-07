//--------------------------------------------------------------------------------
// Copyright � 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmpEqual.cs $
 * 
 * 25    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 24    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 23    4-03-08 23:39 Patrick
 * use Epsilon compare
 * 
 * 22    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 21    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 20    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 19    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 18    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 17    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 16    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 15    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var0 == var1, where both variables are float.
	/// </summary>
	public class FltVarCmpEqual : FltVarCmp
	{
		public FltVarCmpEqual( FltVar var0, FltVar var1 ) :
			base( var0, var1 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "==", wd );
		}

		public override void Unbound()
		{
			base.Unbound();
			
			// If we call unbound on a leading variable, we need to unbound the
			// related variable too.
			if( Var0.IsLead && Var1.IsLead )
			{
				Var0.UnboundDirect();
				Var1.UnboundDirect();
			}
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !Epsilon.Equal( Var0.Value, Var1.Value ) );
		}

		public override void Update()
		{
			FltDomain tmp	= Var0.Domain.Intersect( Var1.Domain );
		
			Var0.Update( tmp );
			Var1.Update( tmp );
		}
	}
}
