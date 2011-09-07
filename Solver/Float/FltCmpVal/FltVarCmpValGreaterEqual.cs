//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmpVal/FltVarCmpValGreaterEqual.cs $
 * 
 * 32    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 31    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 30    5-03-08 0:34 Patrick
 * use Epsilon
 * 
 * 29    29-02-08 22:21 Patrick
 * 
 * 28    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 27    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 26    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 25    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 24    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 23    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 22    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 21    22-03-07 23:29 Patrick
 * cleanup code
 */
//--------------------------------------------------------------------------------

using System;
using System.Globalization;

using MaraInterval.Utility;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var >= value.
	/// </summary>
	public class FltVarCmpValGreaterEqual : FltVarCmpVal
	{
		public FltVarCmpValGreaterEqual( FltVar var0, double val ) :
			base( var0, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( ">=", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !Epsilon.GreaterEqual( Var0.Value, m_Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( m_Value, double.MaxValue );
		}
	}
}
