//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Int/IIntInterval.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 5     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 4     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 3     21-02-07 20:07 Patrick
 * update
 * 
 * 2     10/17/06 10:53p Patrick
 * 
 * 1     6/28/06 7:56p Patrick
 * added IIntInterval
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraInterval.Interval
{
	public interface IIntInterval
	{
		int Min
		{
			get;
		}

		int Max
		{
			get;
		}
		
		int Cardinality
		{
			get;
		}
	}
}

//--------------------------------------------------------------------------------
