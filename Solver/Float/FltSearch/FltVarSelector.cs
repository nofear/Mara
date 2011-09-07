//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltSearch/FltVarSelector.cs $
 * 
 * 9     4-03-08 23:24 Patrick
 * filter out empty variables
 * 
 * 8     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 7     20-03-07 23:51 Patrick
 * refactored all constraints on variable
 * 
 * 6     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 5     27-02-07 0:30 Patrick
 * use array instead of FltVarList
 * 
 * 4     20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 3     19-02-07 22:43 Patrick
 * cleanup vardomain api
 * 
 * 2     8-02-07 0:18 Patrick
 * refactored a bit
 * 
 * 1     17-03-06 20:00 Patrick
 * added float version of Generate
 */
//--------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
namespace MaraSolver.Float.Search
{
	/// <summary>
	/// 
	/// </summary>
	public class FltVarSelector
	{
		public delegate FltVar Select( FltVar[] list );

		static public FltVar FirstNotBound( FltVar[] list )
		{
			int idx;
			for( idx = 0;
					idx < list.Length
						&& ( list[ idx ].IsBound()
								|| list[ idx ].IsEmpty() );
					++idx ) {};

			return   ( idx < list.Length )
					? list[ idx ]
					: null;
		}

		static public FltVar CardinalityMin( FltVar[] list )
		{
			FltVar chosenVar     = null;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				FltVar var    = list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty()
						&& ( ReferenceEquals( chosenVar, null )
								|| ( chosenVar.Domain.Cardinality > var.Domain.Cardinality ) ) )
				{
					chosenVar     = var;
				}
			}

			return chosenVar;
		}

		static public FltVar CardinalityMax( FltVar[] list )
		{
			FltVar chosenVar     = null;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				FltVar var    = list[ idx ];
				if( !var.IsBound()
						&& !var.IsEmpty()
						&& ( ReferenceEquals( chosenVar, null )
								|| ( chosenVar.Domain.Cardinality < var.Domain.Cardinality ) ) )
				{
					chosenVar     = var;
				}
			}

			return chosenVar;
		}

		static public FltVar MostConstrained( FltVar[] list )
		{
			FltVar chosenVar     = null;

			for( int idx = 0; idx < list.Length; ++idx )
			{
				FltVar var    = list[ idx ];
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
	}
}

//--------------------------------------------------------------------------------
