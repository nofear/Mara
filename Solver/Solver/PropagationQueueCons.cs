//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/PropagationQueueCons.cs $
 * 
 * 20    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 19    26-12-07 15:17 Patrick
 * 
 * 18    14-11-07 23:58 Patrick
 * 
 * 17    25-10-07 21:55 Patrick
 * added Close()
 * 
 * 16    18-10-07 23:19 Patrick
 * check if queue is empty
 * 
 * 15    4-09-07 21:29 Patrick
 * only propagate when not violated
 * 
 * 14    8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 13    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 12    8-08-07 21:54 Patrick
 * changed IUpdate interface
 * 
 * 11    25-07-07 22:59 Patrick
 * added ICloneable
 * 
 * 10    25-07-07 3:59 Patrick
 * 
 * 9     12-07-07 21:35 Patrick
 * use IUpdate
 * 
 * 8     27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 7     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 6     20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 5     10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 * 
 * 4     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 3     8-03-07 21:56 Patrick
 * renamed class
 * 
 * 2     8-03-07 21:37 Patrick
 * added interface
 * 
 * 1     8-03-07 1:27 Patrick
 * added alternative PropagationQueue
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using MaraSolver.BaseConstraint;
using MaraSolver.Integer;
using MaraSolver.Float;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Class that's responsible for propagation invalidated constraints & variables.
	/// </summary>
	public sealed class PropagationQueueCons : IPropagationQueue
	{
		public PropagationQueueCons()
		{
			m_IsPropagating				= false;
			m_IsViolated				= false;
			m_Queue						= new Queue<IDemon>();
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

			if( !m_IsViolated )
			{
				Enqueue( var );

				Propagate();
			}
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
					IDemon item	= m_Queue.Dequeue();

					item.InQueue	= false;

					item.Update();

					++item.CountUpdate;

					if( m_IsViolated )
					{
						++item.CountFail;
					}
				}

				if( m_Queue.Count > 0 )
				{
					foreach( IDemon item in m_Queue )
					{
						item.InQueue	= false;
					}

					m_Queue.Clear();
				}
				
				m_IsPropagating		= false;
			}
		}

		public void Enqueue( Variable var )
		{
			if( var.IsBound() )
			{
				Enqueue( var.OnVarIsBoundList );
			}
			
			if( var.IsIntervalChanged() )
			{
				Enqueue( var.OnVarIntervalList );
			}

			Enqueue( var.OnVarDomainList );
		}

		public void Enqueue( IList<IDemon> itemList )
		{
			for( int idx = 0; idx < itemList.Count; ++idx )
			{
				IDemon item		= itemList[ idx ];

				if( !item.InQueue )
				{
					item.InQueue	= true;

					m_Queue.Enqueue( item );
				}
			}
		}

		bool				m_IsPropagating;
		bool				m_IsViolated;
		Queue<IDemon>		m_Queue;

	}
}

//--------------------------------------------------------------------------------
