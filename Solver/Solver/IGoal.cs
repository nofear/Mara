//--------------------------------------------------------------------------------
// Copyright � 2004-2006 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Solver/IGoal.cs $
 * 
 * 1     17-01-06 22:33 Patrick
 * removed interface
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace Solver
{
	public interface IGoal
	{
		string ToString();

		void Execute();
	}
}

//--------------------------------------------------------------------------------
