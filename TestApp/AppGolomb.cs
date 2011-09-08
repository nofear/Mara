using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SolverExample;

using MaraSolver;
using MaraSolver.Integer.Search;

namespace TestApp
{
	public class AppGolomb : App
	{
		static public void Golomb()
		{
			for( int n = 2; n <= 11; ++n )
			{
				Console.WriteLine( "Golomb #" + n.ToString() );

				Golomb( n );
			}
		}		
		
		static public void Golomb( int n )
		{
			Golomb golomb		= new Golomb( n );
			Solver solver		= golomb.Solver;
			solver.IntObjective.Var		= golomb.MarkList[ golomb.MarkList.Count - 1 ];
			solver.IntObjective.Step	= 1;

			Objective objective		= new Objective();
			objective.Value			= int.MaxValue;
			objective.Register( solver.IntObjective );

			solver.Solve( new GoalAnd( new IntGenerate( solver,
												golomb.MarkList.ToArray(),
												IntVarSelector.FirstNotBound,
												new IntSearchInstantiateBest() ),
										new SolutionGoal( solver, solver.IntObjective, objective, "0" ) ) );

			SearchAll( solver );
			
			Console.Out.WriteLine();
			Console.Out.WriteLine( solver.Time.ToString() );
		}

		static void GolombExecute( Solver solver, Objective objective, IntSearch search, string instance )
		{
			DateTime start	= DateTime.Now;
			
			solver.Solve( new GoalAnd( new IntGenerate( solver,
												solver.IntVarList.ToArray(),
												IntVarSelector.FirstNotBound,
												search ),
										new SolutionGoal( solver, solver.IntObjective, objective, instance ) ) );

			SearchAll( solver, instance );
			
			TimeSpan span	= DateTime.Now - start;
			
			Console.Out.WriteLine();
			Console.Out.WriteLine( instance + "\t" + span.ToString() );
		}

		class Objective
		{
			public Objective()
			{
				m_Value		= int.MaxValue;
				m_List		= new List<IntObjective>();
			}
			
			public void Register( IntObjective obj )
			{
				lock( this )
				{
					m_List.Add( obj );	
				}
			}
		
			public int Value
			{
				set
				{
					lock( this )
					{
						if( m_Value > value )
						{
							m_Value		= value;
						
							foreach( IntObjective obj in m_List )
							{
								obj.Value	= m_Value;
							}
						
						}
					
					}
				}
			}
		
			int m_Value;
			List<IntObjective> m_List;
		};

		class SolutionGoal : Goal
		{
			public SolutionGoal( Solver solver, IntObjective objective, Objective value, string instance ) :
				base( solver )
			{
				m_Objective			= objective;
				m_Value				= value;
				m_Instance			= instance;
			}

			public override void Execute()
			{
				Console.WriteLine( m_Instance.ToString()+ "# found solution: " + m_Objective.Var.ToString( true ) + ", " + Solver.Time.ToString() );
			
				m_Value.Value		= m_Objective.Value;
			}

			Objective		m_Value;
			IntObjective	m_Objective;
			string			m_Instance;
		};
	}
}
