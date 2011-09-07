//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltVarExprVal/FltVarExprValMulRev.cs $
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
 * 
 * 7     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 6     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 5     20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 4     20-02-07 20:59 Patrick
 * refactored var domain a bit
 * 
 * 3     5-02-07 0:17 Patrick
 * refactored fltdomain class
 * 
 * 2     6/02/06 9:14a Patrick
 * synchronized with integer version
 * 
 * 1     6/01/06 11:13a Patrick
 * converted from int classes
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarExprValMulRev.
	/// </summary>
	public class FltVarExprValMulRev : FltVarExprVal
	{	
		public FltVarExprValMulRev( double val, FltVar var1 ) :
			this( new FltVar( var1.Solver ), val, var1 )
		{
		}

		public FltVarExprValMulRev( FltVar var0, double val, FltVar var1 ) :
			base( var0, var1, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( "*", true, wd );
		}

		public override bool IsViolated()
		{
			return  ( IsBound()
						&& !( Var0.Value == Value * Var1.Value ) );
		}

		public override void Update()
		{
			Update1();
		}

		public void Update1()
		{
			FltDomain v0	= Var0.Domain;
			FltDomain v1	= Var1.Domain;

			FltDomain w0, w1;
			do
			{
				w0	= v0;
				w1	= v1;

				v0	= v0.Intersect( Domain * v1 );
				v1	= v1.Intersect( v0 / Domain );
			}
			while( !ReferenceEquals( w0, v0 ) || !ReferenceEquals( w1, v1 ) );

			Var0.Update( v0 );
			Var1.Update( v1 );
		}

		public void Update2()
		{
			FltDomain dom0	= Domain * Var1.Domain;
			FltDomain dom1	= dom0 / Domain;
		
			// v0	= val * v1
			// v1	= v0 / val
			Var0.Intersect( dom0 );
			Var1.Intersect( dom1 );
		}	
	}
}
