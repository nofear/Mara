//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVarList0.cs $
 * 
 * 46    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 45    29-02-08 19:40 Patrick
 * fixed Post()/Post(..)
 * 
 * 44    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 43    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 42    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 41    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 40    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 39    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 38    27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 37    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 36    20-06-07 22:46 Patrick
 * renamed namespace
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
using System.Collections;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for ConstraintVarList.
	/// </summary>
	[DebuggerDisplay("{VarList}")]
	public abstract class ConstraintVarList0<TVarList> : ConstraintVar
		where TVarList:VariableList
	{
		protected ConstraintVarList0( TVarList list ) :
			base( list.Solver, new Variable[ 0 ], new VariableList[] { list } )
		{
		}

		internal override void OnSet()
		{
			m_TypeVarList	= (TVarList) VariableListList[ 0 ];
		}

		public override string ToString( bool wd )
		{
			return "(" + VarList.ToString( wd ) + ")";
		}

		public TVarList VarList
		{
			get
			{
				return m_TypeVarList;
			}
		}
	
		TVarList	m_TypeVarList;
	}
}
