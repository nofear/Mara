//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarListMul.cs $
 * 
 * 38    9/04/08 8:29p Patrick
 * Moved interval classes into own project
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
using MaraInterval.Interval;
using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVarListMul.
	/// </summary>
	public class FltVarListMul : ConstraintVarList1<FltVar,FltVarList>
	{
		public FltVarListMul( FltVarList list ) :
			this( new FltVar( list.Solver ), list )
		{
		}
			
		public FltVarListMul( FltVar var0, FltVarList list ) :
			base( var0, list )
		{
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
				FltDomain domain	= VarList[ 0 ].Domain;
				
				for( int idx = 1; idx < VarList.Count; ++idx )
				{
					domain	*= VarList[ idx ].Domain;
				}
				
				Var0.Intersect( domain );
			}
		}
	}
}
