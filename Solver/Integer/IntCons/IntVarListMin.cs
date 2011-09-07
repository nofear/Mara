//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntVarListMin.cs $
 * 
 * 40    2/22/09 2:42p Patrick
 * 
 * 39    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 38    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 37    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 36    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 35    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 34    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 33    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 32    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 31    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 30    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 29    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for FVarListMin.
	/// </summary>
	public class IntVarListMin : ConstraintVarList1<IntVar,IntVarList>
	{
		public IntVarListMin( IntVarList list ) :
			this( new IntVar( list.Solver ), list )
		{
		}

		public IntVarListMin( IntVar var0, IntVarList list ) :
			base( var0, list )
		{
		}

		public override string ToString( bool wd )
		{
			return Var0.ToString( wd ) + "=Min(" + VarList.ToString( wd ) + ")";
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

			Var0.Intersect( VarList.MinInterval );

			foreach( IntVar var in VarList )
			{
				var.Min	= Var0.Min;
			}
		}
	}
}
