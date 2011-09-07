//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Utility/QueueEx.cs $
 * 
 * 3     20-10-07 1:00 Patrick
 * 
 * 2     18-10-07 23:20 Patrick
 * added constraint threaded
 * 
 * 1     16-10-07 0:43 Patrick
 * added threaded propagation queue
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

//--------------------------------------------------------------------------------
namespace MaraSolver.Utility
{
	public class QueueEx<T>
	{
		public QueueEx()
		{
			m_Count		= 0;
			m_Queue		= new Queue<T>();
			m_Idle		= new ManualResetEvent( true );
			m_Busy		= new ManualResetEvent( false );
		}

		public int Count
		{
			get
			{
				lock( this )
				{
					return m_Count;
				}
			}
		}

		public ManualResetEvent Idle
		{
			get
			{
				return m_Idle;
			}
		}

		public ManualResetEvent Busy
		{
			get
			{
				return m_Busy;
			}
		}

		public void PushUnsafe( T item )
		{
			m_Queue.Enqueue( item );

			if( m_Queue.Count == 1 )
			{
				m_Busy.Set();
			}

			if( m_Count == 0 )
			{
				m_Idle.Reset();
			}

			++m_Count;
		}

		public void Push( T item )
		{
			lock( this )
			{
				PushUnsafe( item );
			}
		}

		public T Obtain()
		{
			T item	= default(T);

			lock( this )
			{
				if( m_Queue.Count > 0 )
				{
					item	= m_Queue.Dequeue();

					if( m_Queue.Count == 0 )
					{
						m_Busy.Reset();
					}
				}
			}
			
			return item;
		}

		public void Release()
		{
			lock( this )
			{
				--m_Count;
				
				if( m_Count == 0 )
				{
					m_Idle.Set();
				}
			}
		}		

		public void Clear()
		{
			lock( this )
			{
				if( m_Count > 0 )
				{
					m_Queue.Clear();

					m_Count		= 0;

					m_Idle.Set();
					m_Busy.Reset();
				}
			}		
		}

		int					m_Count;
		public Queue<T>		m_Queue;
		ManualResetEvent	m_Idle;
		ManualResetEvent	m_Busy;
	}

}

//--------------------------------------------------------------------------------
