//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Flt/IFltInterval.cs $
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 4     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 3     9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 2     21-02-07 20:07 Patrick
 * update
 * 
 * 1     4-02-07 23:55 Patrick
 * added interface
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraInterval.Interval
{
	public interface IFltInterval
	{
		double Min
		{
			get;
		}

		double Max
		{
			get;
		}
		
		double Cardinality
		{
			get;
		}
	}
}

//--------------------------------------------------------------------------------
