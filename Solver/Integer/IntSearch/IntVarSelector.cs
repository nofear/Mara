//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntSearch/IntVarSelector.cs $
 * 
 * 10    4-03-08 23:24 Patrick
 * filter out empty variables
 * 
 * 9     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 8     3/24/07 2:20a Patrick
 * more magic
 * 
 * 7     20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 6     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 5     24-02-07 0:26 Patrick
 * slightly faster code
 * changed to IntVar[]
 * 
 * 4     20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 3     19-02-07 22:43 Patrick
 * cleanup vardomain api
 * 
 * 2     17-03-06 20:00 Patrick
 * added into Search namespace
 * 
 * 1     17-03-06 19:49 Patrick
 * refactored to generic IntGenerate
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer.Search
{
	/// <summary>
	/// 
	/// </summary>
	public class IntVarSelector
	{
		public delegate IntVar Select( IntVar[] list );

		static public IntVar FirstNotBound( IntVar[] list )
		{
			int idx;
			for( idx = 0;
					idx < list.Length
						&& ( list[ idx ].IsEmpty()
								|| list[ idx ].IsBound() );
					++idx ) {};

			return   ( idx < list.Length )
					? list[ idx ]
					: null;
		}

		static public IntVar CardinalityMin( IntVar[] list )
		{
			IntVar chosenVar	= null;
			int chosenVarCard	= 0;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				IntVar var		= list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty() )
				{
					int varCard		= var.Domain.Cardinality;

					if( ReferenceEquals( chosenVar, null )
								|| ( chosenVarCard > varCard ) )
					{
						chosenVar		= var;
						chosenVarCard	= varCard;
					}
				}
			}

			return chosenVar;
		}

		static public IntVar CardinalityMax( IntVar[] list )
		{
			IntVar chosenVar	= null;
			int chosenVarCard	= 0;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				IntVar var		= list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty() )
				{
					int varCard		= var.Domain.Cardinality;

					if( ReferenceEquals( chosenVar, null )
								|| ( chosenVarCard < varCard ) )
					{
						chosenVar		= var;
						chosenVarCard	= varCard;
					}
				}
			}

			return chosenVar;
		}

		static public IntVar MostConstrained( IntVar[] list )
		{
			IntVar chosenVar     = null;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				IntVar var    = list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty()
						&& ( ReferenceEquals( chosenVar, null )
								|| chosenVar.ConstraintList.Count < var.ConstraintList.Count ) )
				{
					chosenVar     = var;
				}
			}

			return chosenVar;
		}
		
		static public IntVar Random( IntVar[] list )
		{
			List<IntVar> boundList	= new List<IntVar>();

			for( int idx = 0; idx < list.Length; ++idx )
			{
				IntVar var    = list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty() )
				{
					boundList.Add( var );
				}
			}		
			
			if( boundList.Count == 0 )
				return null;
			
			int index	= m_Random.Next( 0, boundList.Count );
		
			return boundList[ index ];
		}
		
		static Random m_Random	= new Random();
	}
}

//--------------------------------------------------------------------------------
