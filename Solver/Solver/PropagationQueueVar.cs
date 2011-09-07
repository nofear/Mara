//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/PropagationQueueVar.cs $
 * 
 * 54    2/22/09 2:42p Patrick
 * 
 * 53    2/10/09 10:00p Patrick
 * simplified code
 * 
 * 52    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 51    26-12-07 15:17 Patrick
 * 
 * 50    25-10-07 21:55 Patrick
 * added Close()
 * 
 * 49    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 48    8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 47    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 46    8-08-07 21:54 Patrick
 * changed IUpdate interface
 * 
 * 45    25-07-07 22:59 Patrick
 * added ICloneable
 * 
 * 44    25-07-07 3:59 Patrick
 * 
 * 43    12-07-07 21:35 Patrick
 * use IUpdate
 * 
 * 42    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 41    20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 40    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 39    8-03-07 21:56 Patrick
 * renamed class
 * 
 * 38    8-03-07 21:37 Patrick
 * added interface
 * 
 * 37    21-02-07 0:39 Patrick
 * renamed IsViolated -> IsEmpty
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using MaraSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Class that's responsible for propagation invalidated constraints & variables.
	/// </summary>
	public sealed class PropagationQueueVar : IPropagationQueue
	{
		public PropagationQueueVar()
		{
			m_IsPropagating		= false;
			m_IsViolated		= false;
			m_Queue				= new Queue<Variable>();
		}
		
		public bool IsViolated
		{
			get
			{
				return m_IsViolated;
			}

			set
			{
				m_IsViolated	= value;
			}
		}

		/// <summary>
		/// Enqueue the given variable.
		/// </summary>
		public void OnChangeVariable( Variable var )
		{
			m_IsViolated	|= var.IsEmpty();

			if( !m_IsViolated
					&& !var.InQueue )
			{
				m_Queue.Enqueue( var );

				var.InQueue	= true;
			}
		
			Propagate();
		}

		/// <summary>
		/// Propagate all the variables in the queue.
		/// If FailOnFirstViolation == true then the method will return on the first
		/// violation encountered, otherwise it'll keep propagating until the queue
		/// is empty.
		/// </summary>
		public void Propagate()
		{
			if( !m_IsPropagating )
			{
				m_IsPropagating		= true;

				while( !m_IsViolated
							&& m_Queue.Count > 0 )
				{
					Variable var	= m_Queue.Dequeue();

					var.InQueue	= false;

					Propagate( var );
				}

				foreach( Variable var in m_Queue )
				{
					var.InQueue	= false;
				}

				m_Queue.Clear();
				
				m_IsPropagating		= false;
			}
		}

		public void Propagate( Variable var )
		{
			if( var.IsBound() )
			{
				Propagate( var, var.OnVarIsBoundList );
			}
			
			if( var.IsIntervalChanged() )
			{
				Propagate( var, var.OnVarIntervalList );
			}
			
			Propagate( var, var.OnVarDomainList );
		}

		private void Propagate( Variable var, IList<IDemon> itemList )
		{
			if( m_IsViolated )
				return;
			
			if( itemList.Count == 0 )
				return;

			for( int idx = 0; idx < itemList.Count; ++idx )
			{
				IDemon item	= itemList[ idx ];

				item.Update( var );
				
				++item.CountUpdate;

				if( m_IsViolated )
				{
					++item.CountFail;
					break;
				}
			}
		}

		bool				m_IsPropagating;
		bool				m_IsViolated;
		Queue<Variable>		m_Queue;
	}
}

//--------------------------------------------------------------------------------
