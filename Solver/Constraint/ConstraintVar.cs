//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVar.cs $
 * 
 * 73    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 72    29-02-08 19:40 Patrick
 * fixed Post()/Post(..)
 * 
 * 71    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 70    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 69    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 68    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 67    8-08-07 22:30 Patrick
 * 
 * 66    8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 65    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 64    8-08-07 21:53 Patrick
 * change IUpdate interface
 * added Index property
 * 
 * 63    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 62    31-07-07 21:43 Patrick
 * removed m_Index
 * 
 * 61    25-07-07 3:59 Patrick
 * renamed Fail() -> Violate()
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

using MaraSolver;
using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for Constraint.
	/// </summary>
	public abstract class ConstraintVar : Constraint
	{
		protected ConstraintVar( Solver solver, Variable[] varList, VariableList[] varListList ) :
			base( solver )
		{
			m_VariableList		= varList;
			m_VariableListList	= varListList;

			OnSet();
		}

		/// <summary>
		/// Callback used to initialize cached references.
		/// (generic casts were to slow)
		/// </summary>
		internal virtual void OnSet()
		{
		}

		public IList<Variable> VariableList
		{
			get
			{
				return m_VariableList;
			}
		}

		public IList<VariableList> VariableListList
		{
			get
			{
				return m_VariableListList;
			}
		}

		public override void Add()
		{
			foreach( Variable var in m_VariableList )
			{
				var.ConstraintList.Add( this );
			}

			foreach( VariableList varList in m_VariableListList )
			{
				foreach( Variable var in varList )
				{
					var.ConstraintList.Add( this );
				}
			}
		}

		public override void Post()
		{
			Post( Variable.How.OnDomain );
		}

		protected virtual void Post( Variable.How how )
		{
			foreach( Variable var in m_VariableList )
			{
				var[ how ].Add( this );
			}

			foreach( VariableList varList in m_VariableListList )
			{
				foreach( Variable var in varList )
				{
					var[ how ].Add( this );
				}
			}
		}

		public override void Unbound()
		{
			foreach( Variable var in m_VariableList )
			{
				var.Unbound();
			}

			foreach( VariableList varList in m_VariableListList )
			{
				varList.Unbound();
			}
		}

		public override bool IsBound()
		{
			if( Array.FindLastIndex<Variable>( m_VariableList, MaraSolver.Variable.IsBound ) == m_VariableList.Length )
				return false;

			if( Array.FindLastIndex<VariableList>( m_VariableListList, MaraSolver.VariableList.IsBound ) == m_VariableListList.Length )
				return false;

			return true;
		}
		
		Variable[]		m_VariableList;
		VariableList[]	m_VariableListList;
	}
}
