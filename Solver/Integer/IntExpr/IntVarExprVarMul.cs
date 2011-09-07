//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntExpr/IntVarExprVarMul.cs $
 * 
 * 29    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 28    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 27    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 26    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 25    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 24    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 23    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 22    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 21    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 20    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarExprMul.
	/// </summary>
	public class IntVarExprVarMul : IntVarExprVar
	{
		public IntVarExprVarMul( IntVar var1, IntVar var2 ) :
			this( new IntVar( var1.Solver ), var1, var2 )
		{
		}

		public IntVarExprVarMul( IntVar var0, IntVar var1, IntVar var2 ) :
			base( var0, var1, var2 )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "*", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value == Var1.Value * Var2.Value ) );
		}

		// v0	= v1 * v2
		// v1	= v0 / v2
		// v2	= v0 / v1
		public override void Update()
		{
			IntDomain v0	= Var0.Domain;
			IntDomain v1	= Var1.Domain;
			IntDomain v2	= Var2.Domain;

			IntDomain w0, w1, w2;
			do
			{
				w0	= v0;
				w1	= v1;
				w2	= v2;

				v0	= v0.Intersect( v1 * v2 );
				v1	= v1.Intersect( v0 / v2 );
				v2	= v2.Intersect( v0 / v1 );
			}
			while( !ReferenceEquals( w0, v0 ) || !ReferenceEquals( w1, v1 ) || !ReferenceEquals( w2, v2 ) );

			Var0.Update( v0 );
			Var1.Update( v1 );
			Var2.Update( v2 );
		}	
	}
}
