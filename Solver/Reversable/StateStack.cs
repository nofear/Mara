//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/StateStack.cs $
 * 
 * 28    2/22/09 2:42p Patrick
 * 
 * 27    14-11-07 22:44 Patrick
 * removed StateId construct
 * 
 * 26    25-07-07 23:01 Patrick
 * added Clone() to IStateStack
 * 
 * 25    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 24    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 23    31-03-07 12:55 Patrick
 * implemented support to retrieve previous domain state
 * 
 * 22    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 21    7-03-07 22:46 Patrick
 * clear map
 * 
 * 20    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 19    19-02-07 22:30 Patrick
 * using StateId
 * 
 * 18    19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 17    11/01/06 7:47p Patrick
 * make sure key comparison is done on reference
 * 
 * 16    6/14/06 10:21p Patrick
 * added IStateStack interface
 * 
 * 15    14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 14    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 13    21-02-06 22:57 Patrick
 * release map
 * 
 * 12    16-02-06 23:03 Patrick
 * added sealed
 * 
 * 11    19-01-06 21:09 Patrick
 * minor refactoring
 * 
 * 10    16-01-06 21:27 Patrick
 * use KeyValuePair<> as iterator
 * 
 * 9     25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 8     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 7     12/14/05 10:06p Patrick
 * refactored using interface and delegates
 * 
 * 6     10/19/05 10:16p Patrick
 * made class internal
 * 
 * 5     28-05-05 19:49 Patrick
 * upgrade to visual studio 2005
 * added generics where available
 * 
 * 4     26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	/// <summary>
	/// This class im
	/// </summary>
	public sealed class StateStack : IStateStack, ICloneable
	{
		sealed class StateMap :
			Dictionary<IState,object>
		{
			public object Get( IState obj )
			{
				object state;
				if( TryGetValue( obj, out state ) )
					return state;
			
				return null;
			}
		}
	
		public StateStack()
		{
			m_Stack		= new Stack<StateMap>();
			m_Map		= null;
		}

		#region ICloneable Members

		public object Clone()
		{
			return new StateStack();
		}

		#endregion

		public int Count
		{
			get
			{
				return m_Stack.Count;
			}
		}

		public void Begin()
		{
			Push();

			m_Map		= new StateMap();
		}

		public void End()
		{
			Pop();
		}

		public void Cancel()
		{
			Restore();
			Pop();
		}

		private void Push()
		{
			m_Stack.Push( m_Map );
		}

		private void Pop()
		{
			if( m_Stack.Count == 0 )
				throw new Exception( "empty stack" );
			
			m_Map.Clear();
			
			m_Map	= m_Stack.Pop();
		}		

		public object GetState( IState obj )
		{
			if( ReferenceEquals( m_Map, null ) )
				return null;

			return m_Map.Get( obj );
		}

		public bool IsStored( IState obj )
		{
			return m_Map.ContainsKey( obj );
		}

		public void Store( IState obj )
		{
			if( ReferenceEquals( m_Map, null ) )
				return;
		
			if( m_Map.ContainsKey( obj ) )
				return;

			m_Map[ obj ]	= obj.State;
		}

		private void Restore()
		{
			foreach( KeyValuePair<IState,object> keyValue in m_Map )
			{
				keyValue.Key.State	= keyValue.Value;
			}
		}

		Stack<StateMap>		m_Stack;
		StateMap			m_Map;
	}
}

//--------------------------------------------------------------------------------
