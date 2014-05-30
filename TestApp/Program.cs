//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------

using System;
using System.Timers;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Linq.Expressions;
using MaraSolver;
using MaraSolver.Utility;
using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraSolver.Integer.Search;
using MaraSolver.Float;
using MaraSolver.Float.Search;
using MaraSolver.BaseConstraint;

using SolverExample;

//--------------------------------------------------------------------------------

namespace TestApp
{
	/// <summary>
	/// Holds application code
	/// </summary>
	class Program : App
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		
		static void Main(string[] args)
		{
			Thread.CurrentThread.Priority	= ThreadPriority.Highest;

			//Multiply();
			//Poly1();
			//Cover1();
			//Scheduler1();
			//Prop1();
			//Generate();
			//AppMoreMoney.MoreMoney();
			//Thread1();
			//Sudoku();
			AppMagicSquare.MagicSquare();
			//AppMagicSquare.MagicSquare();
			//AppMagicSquare.MagicSquare2();
			//AppMagicSquare.MagicSquare4();
			//AppMagicSquare.MagicSquare4b();
			//Sudoku();
			//AppGolomb.Golomb();
			//AppGolomb.Golomb( 10 );
			AppGolomb.Golomb( 11 );

			Console.In.ReadLine();
			
		}


		private static void Poly1()
		{
	
			Random rnd	= new Random(0);
			for( int i = 0; i < 10000000; ++i )
			{
				double a	= rnd.Next( -10, 10 );
				double b	= rnd.Next( -10, 10 ) ;
				double c	= rnd.Next( -10, 10 ) ;

				a=2;b=8;c=8;

				Solver s		= new Solver( -1000000, 1000000 );
				FltVar x		= new FltVar( s, "x" );
				FltVarExpr exp	= a*x*x+ b*x;
				s.Add( exp );
				s.Add( exp == -c );

				s.PrintConstraints();
				s.PrintVariables();

				Console.Out.WriteLine( "{0}.x^2 + {1}.x + {2} = 0", a, b, c );

				s.Solve( new FltGenerate( s, new FltVar[] { x } ) );
				Console.Out.WriteLine( x.ToString(true) );
				while( s.Next() )
				{
					Console.Out.WriteLine( x.ToString(true) );
				}
			}
		}

		private static void Cover1()
		{
			ExactCover ec = new ExactCover( 8 );
			ec.Add( new int[] { 1, 0, 0, 0, 0, 0, 0, 0 }, "0" );
			ec.Add( new int[] { 0, 1, 0, 0, 0, 0, 0, 0 }, "1" );
			ec.Add( new int[] { 0, 0, 1, 0, 0, 0, 0, 0 }, "2" );
			ec.Add( new int[] { 0, 0, 0, 1, 0, 0, 0, 0 }, "3" );
			ec.Add( new int[] { 0, 0, 0, 0, 1, 0, 0, 0 }, "4" );
			ec.Add( new int[] { 0, 0, 0, 0, 0, 1, 0, 0 }, "5" );
			ec.Add( new int[] { 0, 0, 0, 0, 0, 0, 1, 0 }, "6" );
			ec.Add( new int[] { 0, 0, 0, 0, 0, 0, 0, 1 }, "7" );

			ec.Add( new int[] { 1, 1, 0, 0, 0, 0, 0, 0 }, "8" );
			ec.Add( new int[] { 0, 1, 1, 0, 0, 0, 0, 0 }, "9" );
			ec.Add( new int[] { 0, 0, 1, 1, 0, 0, 0, 0 }, "10" );
			ec.Add( new int[] { 0, 0, 0, 1, 1, 0, 0, 0 }, "11" );
			ec.Add( new int[] { 0, 0, 0, 0, 1, 1, 0, 0 }, "12" );
			ec.Add( new int[] { 0, 0, 0, 0, 0, 1, 1, 0 }, "13" );
			ec.Add( new int[] { 0, 0, 0, 0, 0, 0, 1, 1 }, "14" );

			ec.Add( new int[] { 1, 1, 1, 1, 0, 0, 0, 0 }, "15" );
			ec.Add( new int[] { 0, 0, 0, 0, 1, 1, 1, 1 }, "16" );

			Cover cover = new Cover( ec.Solver, 8, ec.List );
			Cover.Search goal = new Cover.Search( cover );

			ec.Solver.Solve( goal );
			ec.Solver.Out.WriteLine( cover.Index.Domain.Cardinality.ToString() + "\t" + cover.Index.ToString() );

			while( ec.Solver.Next() )
			{
				ec.Solver.Out.WriteLine( cover.Index.Domain.Cardinality.ToString() + "\t" + cover.Index.ToString() );
			}

			ec.Solver.PrintInformation();
		}

		static void Prop1()
		{
			Solver solver	= new Solver( -10000, 10000 );
			IntVar a		= new IntVar( solver, 6, 10, "a" );
			IntVar b		= new IntVar( solver, 1, 2, "b" );
			IntVarExprVal cons	= new IntVarExprValMul( a, b, 5 );
			solver.Add( cons );
			solver.Propagate();

			Console.WriteLine();
		}

		static void Mul1()
		{
			Solver solver	= new Solver( 0, 10000 );
			IntVarMatrix m1	= new IntVarMatrix( solver, 2, 2, new IntInterval( 0, 100 ) );
			int[] v			= new int[] { 1, 2 };
			IntVarMatrix m2	= m1 * v;
			solver.Propagate();
			solver.PrintVariables( Console.Out );
		
			Solver s1	= new Solver( -1000000, 1000000 );
			IntVar a	= new IntVar( s1, IntDomain.Random( -100, 100, 1 ), "a" );
			IntVar b	= new IntVar( s1, IntDomain.Random( -100, 100, 1 ), "b" );
			IntVar c	= new IntVar( s1, IntDomain.Random( -100, 100, 1 ), "c" );
			IntVarList l	= new IntVarList( s1, new IntVar[] { a, b, c } );
			s1.Add( l.Mul() );
			s1.Propagate();
			s1.PrintVariables( Console.Out );
		}

		static void Mul2()
		{
			IntDomain v		= new IntDomain();
			for( int idx = 0; idx < 100000; ++idx )
			{
				Solver s	= new Solver( -1000, 1000 );
				IntVar a	= new IntVar( s, IntDomain.Random( -100, 100, 1 ), "a" );
				IntVar b	= new IntVar( s, IntDomain.Random( -100, 100, 1 ), "b" );
				IntVar c	= new IntVar( s, IntDomain.Random( -100, 100, 1 ), "c" );
				IntVar d	= new IntVar( s, IntDomain.Random( -100, 100, 1 ), "d" );
				IntVarList l	= new IntVarList( s, new IntVar[] { a, b, c, d } );
				//p.Add( a + b + c + d );
				s.Add( l.Sum() );

				s.Propagate();
				s.PrintVariables();
				s.PrintConstraints();
			}
		}

        static void Test1()
		{
			Solver solver		= new Solver( 0, 100 );
			IntVar a			= new IntVar( solver, 0, 10 );
			IntVar b			= new IntVar( solver, 0, 10 );
			IntVar c			= new IntVar( solver, 0, 10 );
			IntVarList l		= new IntVarList( a, b, c );
			IntVarListSum sum	= l.Sum();
			
			solver.Add( sum );
			solver.Propagate();
			
			sum.Var0.Value		= 6;
			
			a.Value		= 1;
			b.Value		= 2;
		}		

		static void Test2()
		{
			Solver solver		= new Solver( 0, 100 );
			IntVar a			= new IntVar( solver, 1, 3, "a" );
			IntVar b			= new IntVar( solver, 1, 3, "b" );
			IntVar c			= new IntVar( solver, 1, 3, "c" );
			IntVarList l		= new IntVarList( a, b, c );
			IntVarListAllDifferent diff	= l.AllDifferent();

			solver.Add( diff );
			solver.Propagate();

			a.Value		= 1;
			b.Value		= 2;
		}		

		static void Test3()
		{
			Solver solver		= new Solver( 0, 10000 );
			IntVar a			= new IntVar( solver, 1, 9, "a" );
			IntVar b			= new IntVar( solver, 1, 9, "b" );
			IntVarExpr expr		= a * a + b * b;

			solver.Add( expr );
			solver.Propagate();

			a.Value		= 4;
			b.Value		= 2;
		}



	}		
}
