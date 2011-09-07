//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarListMax.cs $
 * 
 * 37    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 36    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 35    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 34    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 33    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 32    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 31    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 30    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 29    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 28    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarListMax.
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class FltVarListMax : ConstraintVarList1<FltVar,FltVarList>
	{
		public FltVarListMax( FltVarList list ) :
			this( new FltVar( list.Solver ), list )
		{
		}

		public FltVarListMax( FltVar var0, FltVarList list ) :
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
			if( VarList.Count > 0 )
			{
				Var0.Intersect( VarList.MaxInterval );

				foreach( FltVar var in VarList )
				{
					var.Max	= Var0.Max;
				}
			}
		}
	}
}
