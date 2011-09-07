//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVarList1.cs $
 * 
 * 42    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 41    29-02-08 19:40 Patrick
 * fixed Post()/Post(..)
 * 
 * 40    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 39    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 38    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 37    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 36    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 35    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 34    27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 33    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 32    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 31    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 30    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for IntVarConsVec.
	/// </summary>
	[DebuggerDisplay("{Var0},{VarList}")]
	public abstract class ConstraintVarList1<TVar0,TVarList> : ConstraintVar
		where TVar0:Variable
		where TVarList:VariableList
	{
		protected ConstraintVarList1( TVar0 var0, TVarList list ) :
			base( list.Solver, new Variable[] { var0 }, new VariableList[] { list } )
		{
		}

		internal override void OnSet()
		{
			m_TypeVar0		= (TVar0) VariableList[ 0 ];
			m_TypeVarList	= (TVarList) VariableListList[ 0 ];
		}

		public override string ToString( bool wd )
		{
			return "(" + Var0.ToString( wd ) + "," + VarList.ToString( wd ) + ")";
		}

		public TVar0 Var0
		{
			get
			{
				return m_TypeVar0;
			}
		}		

		public TVarList VarList
		{
			get
			{
				return m_TypeVarList;
			}
		}
	
		TVar0		m_TypeVar0;
		TVarList	m_TypeVarList;
	}
}
