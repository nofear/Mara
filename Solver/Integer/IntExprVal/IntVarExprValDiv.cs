//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntExprVal/IntVarExprValDiv.cs $
 * 
 * 24    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 23    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 22    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 21    9-08-07 19:09 Patrick
 * do complete propagation on domains
 * 
 * 20    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 19    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 18    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 17    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 16    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 15    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarExprValDiv.
	/// </summary>
	public class IntVarExprValDiv : IntVarExprVal
	{	
		public IntVarExprValDiv( IntVar var1, int val ) :
			this( new IntVar( var1.Solver ), var1, val )
		{
		}

		public IntVarExprValDiv( IntVar var0, IntVar var1, int val ) :
			base( var0, var1, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "/", false, wd );
		}

		public override bool IsViolated()
		{
			return  ( IsBound()
						&& !( Var0.Value == Var1.Value / Value ) );
		}

		public override void Update()
		{
			// v0	= v1 / val
			// v1	= v0 * val
			IntDomain v0	= Var0.Domain;
			IntDomain v1	= Var1.Domain;

			IntDomain w0, w1;
			do
			{
				w0	= v0;
				w1	= v1;

				v0	= v0.Intersect( v1 / Domain );
				v1	= v1.Intersect( v0 * Domain );
			}
			while( !ReferenceEquals( w0, v0 ) || !ReferenceEquals( w1, v1 ) );

			Var0.Update( v0 );
			Var1.Update( v1 );
		}	
	}
}
