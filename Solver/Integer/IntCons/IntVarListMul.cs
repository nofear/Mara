//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListMul.cs $
 * 
 * 45    2/22/09 2:42p Patrick
 * 
 * 44    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 43    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 42    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 41    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 40    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 39    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 38    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 37    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 36    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 35    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 34    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint: R = V1 * V2 * .. * Vn
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVarListMul : ConstraintVarList1<IntVar,IntVarList>
	{
		public IntVarListMul( IntVarList list ) :
			this( new IntVar( list.Solver ), list )
		{
		}
			
		public IntVarListMul( IntVar var0, IntVarList list ) :
			base( var0, list )
		{
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=Mul(" + VarList.ToString( wd ) + ")";
		}

		public override void Post()
		{
			Post( Variable.How.OnInterval );
		}

		public override bool IsViolated()
		{
			return false;
		}

		public override void Update()
		{
			if( VarList.Count == 0 )
				return;

			IntDomain domain	= VarList.Front().Domain;
			
			for( int idx = 1; idx < VarList.Count; ++idx )
			{
				domain	*= VarList[ idx ].Domain;
			}
			
			Var0.Intersect( domain );
		}
	}
}