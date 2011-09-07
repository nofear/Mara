//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListMax.cs $
 * 
 * 40    2/22/09 2:42p Patrick
 * 
 * 39    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 38    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 37    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 36    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 35    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 34    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 33    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 32    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 31    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 30    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 29    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 28    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVarListMax.
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVarListMax : ConstraintVarList1<IntVar,IntVarList>
	{
		public IntVarListMax( IntVarList list ) :
			this( new IntVar( list.Solver ), list )
		{
		}

		public IntVarListMax( IntVar var0, IntVarList list ) :
			base( var0, list )
		{
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=Max(" + VarList.ToString( wd ) + ")";
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

			Var0.Intersect( VarList.MaxInterval );

			foreach( IntVar var in VarList )
			{
				var.Max	= Var0.Max;
			}
		}
	}
}
