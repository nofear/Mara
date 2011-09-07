//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/IStateStack.cs $
 * 
 * 7     25-07-07 23:01 Patrick
 * added Clone() to IStateStack
 * 
 * 6     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 5     31-03-07 12:55 Patrick
 * implemented support to retrieve previous domain state
 * 
 * 4     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 3     7-03-07 22:46 Patrick
 * added Begin(),End(),Cancel() to interface
 * 
 * 2     19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 1     6/14/06 10:28p Patrick
 * added interface IStateStack
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	public interface IStateStack : ICloneable
	{
		void Store( IState obj );
		bool IsStored( IState obj );
		
		object GetState( IState obj );
		
		void Begin();
		void End();
		void Cancel();
	}
}

//--------------------------------------------------------------------------------
