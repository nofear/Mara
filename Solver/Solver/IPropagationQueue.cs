//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/IPropagationQueue.cs $
 * 
 * 7     2/10/09 10:00p Patrick
 * simplified code
 * 
 * 6     25-10-07 21:55 Patrick
 * added Close()
 * 
 * 5     16-10-07 0:43 Patrick
 * added threaded propagation queue
 * 
 * 4     25-07-07 22:59 Patrick
 * added ICloneable
 * 
 * 3     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 2     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 1     8-03-07 21:37 Patrick
 * added interface
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public interface IPropagationQueue
	{
		bool IsViolated
		{
			get;
			set;
		}
		
		void OnChangeVariable( Variable var );

		void Propagate();
	}
}

//--------------------------------------------------------------------------------
