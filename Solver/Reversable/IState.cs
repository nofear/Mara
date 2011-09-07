//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/IState.cs $
 * 
 * 11    14-11-07 22:44 Patrick
 * removed StateId construct
 * 
 * 10    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 9     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 8     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 7     19-02-07 22:30 Patrick
 * using StateId
 * 
 * 6     19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 5     14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 4     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 3     25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 2     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 1     12/14/05 10:06p Patrick
 * refactored using interface and delegates
 * 
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	public interface IState
	{
		object State
		{
			get;
			set;
		}
	}
}

//--------------------------------------------------------------------------------
