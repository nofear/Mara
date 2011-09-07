//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmpVal/FltVarCmpValGreater.cs $
 * 
 * 31    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 30    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 29    5-03-08 0:34 Patrick
 * use Epsilon
 * 
 * 28    29-02-08 22:21 Patrick
 * 
 * 27    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 26    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 25    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 24    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 23    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 22    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 21    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 20    22-03-07 23:29 Patrick
 * cleanup code
 * 
 * 19    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
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
	/// Implements the constraint var > value.
	/// </summary>
	public class FltVarCmpValGreater : FltVarCmpVal
	{
		public FltVarCmpValGreater( FltVar var0, double val ) :
			base( var0, val )
		{
		}

		public override string ToString( bool wd )
		{
			return ToString( ">", wd );
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !Epsilon.Greater( Var0.Value, m_Value ) );
		}

		public override void Update()
		{
			Var0.Intersect( Epsilon.Next( m_Value ), double.MaxValue );
		}
	}
}
