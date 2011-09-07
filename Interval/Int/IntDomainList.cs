//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Mara.root/Mara/Interval/Int/IntDomainList.cs $
 * 
 * 2     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 1     9/04/08 8:09p Patrick
 * 
 * 2     17-11-07 18:00 Patrick
 * added IntDomainList
 * 
 * 1     17-11-07 16:07 Patrick
 * added class
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraInterval.Interval
{
	public class IntDomainList : List<IntDomain>
	{
		public IntDomainList() :
			base()
		{
		}

		public IntDomainList( int capacity ) :
			base( capacity )
		{
		}

		public IntDomainList( IEnumerable<IntDomain> collection ) :
			base( collection )
		{
		}

		// Returns sum cardinality.
		public int SumCardinality
		{
			get
			{
				int cardinality	= 0;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					cardinality		+= this[ idx ].Cardinality;
				}

				return cardinality;			
			}
		}

		// Returns domain of cardinality values.
		public IntDomain Cardinality
		{
			get
			{
				IntDomain card	= IntDomain.Empty;
			
				for( int idx = 0; idx < Count; ++idx )
				{
					card	= card.Union( this[ idx ].Cardinality );
				}

				return card;			
			}
		}

	}
}

//--------------------------------------------------------------------------------
