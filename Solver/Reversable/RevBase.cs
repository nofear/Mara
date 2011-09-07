//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/RevBase.cs $
 * 
 * 27    14-11-07 22:44 Patrick
 * removed StateId construct
 * 
 * 26    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 25    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 24    4-07-07 20:11 Patrick
 * added Init(..)
 * 
 * 23    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 22    31-03-07 12:55 Patrick
 * implemented support to retrieve previous domain state
 * 
 * 21    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 20    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 19    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 18    19-02-07 22:30 Patrick
 * using StateId
 * 
 * 17    19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 16    10/12/06 9:50p Patrick
 * bit smarter code
 * 
 * 15    6/14/06 10:21p Patrick
 * added IStateStack interface
 * 
 * 14    17-03-06 20:35 Patrick
 * added class diagrams
 * 
 * 13    14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 12    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 11    25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 10    8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 9     5-01-06 21:57 Patrick
 * changed access mode of data member
 * 
 * 8     12/14/05 10:06p Patrick
 * refactored using interface and delegates
 * 
 * 7     6-12-05 21:23 Patrick
 * refactored FRev -> RevValue
 * 
 * 6     10/19/05 9:17p Patrick
 * added comments
 * 
 * 5     3-06-05 19:55 Patrick
 * protected constructor
 * 
 * 4     26-05-05 20:27 Patrick
 * added FBase
 * 
 * 3     26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	/// <summary>
	/// Base class for a reversible object.
	/// </summary>
	public abstract class RevBase : IState
	{
		protected RevBase( IStateStack stateStack )
		{
			m_StateStack	= stateStack;
		}

		/// <summary>
		/// Property to store and restore the state of the object
		/// </summary>
		public abstract object State
		{
			get;
			set;
		}

		/// <summary>
		/// Property to get the previous state of the object
		/// </summary>
		public object StatePrev
		{
			get
			{
				return m_StateStack.GetState( this );
			}
		}

		/// <summary>
		/// Method to call when the object changes, this results in the
		/// state being stored.
		/// </summary>
		public void Store()
		{
			m_StateStack.Store( this );
		}
		
		public bool IsStored
		{
			get
			{
				return m_StateStack.IsStored( this );
			}
		}
		
		IStateStack		m_StateStack;
	}
}
