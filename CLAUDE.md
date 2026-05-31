# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Mara is a constraint programming (CP) / finite-domain solver library written in C# targeting .NET Framework 4.5. The solution was created in Visual Studio 2013 (`Mara.sln`). There is no Makefile, no `dotnet` SDK-style project file, and no README — this is a classic full-framework MSBuild solution.

## Build and test

The expected toolchain is MSBuild + NuGet (for NUnit 3.12), not `dotnet`. Restore NuGet packages before the first build (`packages/` is expected at the solution root).

- Build whole solution: `msbuild Mara.sln /p:Configuration=Debug` (or `Release`).
- NUnit tests in `SolverTest` and `SolverExampleTest` are run via the NUnit console runner against the built test assemblies (`bin/Debug/SolverTest.dll`, `bin/Debug/SolverExampleTest.dll`). Filter to a single test with `--test=<FullyQualifiedName>` (NUnit 3 console syntax).
- `SolverTest/Program.cs` has an empty `Main` — the test project's `<OutputType>Exe</OutputType>` is legacy; tests are discovered through the NUnit attributes, not the entry point.
- `TestApp` is a console executable used as a scratch driver for examples — `TestApp/Program.cs:Main` selects which demo to run by uncommenting one of the calls (currently `AppMondriaan.Mondriaan()`).

## Architecture

There are six projects with a strict dependency direction: `Interval` → `Solver` → `SolverExample` → (`TestApp`, `SolverExampleTest`); `SolverTest` references `Interval` + `Solver`.

### Interval (assembly `Interval`, namespace `MaraInterval.*`)
Pure value-type domain primitives: `IntInterval`, `IntDomain`, `IntDomainList`, `FltInterval`, `FltDomain`, plus bit/epsilon utilities. Has no dependency on the solver and is intended to be reusable on its own.

### Solver (assembly `Solver`, namespace `MaraSolver.*`)
The core CP engine. The big picture is a five-layer stack — when adding constraints/variables/search, fit your code into the appropriate layer rather than creating new top-level concepts:

1. **Reversible state** (`Reversable/`, namespace `MaraSolver.Reversible`): `IStateStack` / `StateStack` is the trail. `RevBase` / `RevValue<T>` / `RevList<T>` / `RevObject` are values that automatically push their previous state onto the trail when mutated. The solver restores trailed state on backtrack.
2. **Variables** (`Variable/`, `Integer/IntBase/`, `Float/FltBase/`): `Variable` is the abstract base; `IntVar` and `FltVar` are domain-backed reversible variables. `IntVarList` / `IntVarMatrix` (and Flt equivalents) are convenience containers with operator overloads that build constraints/expressions.
3. **Constraints** (`Constraint/`, `Integer/IntCmp*`, `Integer/IntCons/`, `Integer/IntExpr*`, and the matching `Float/` subtrees): `Constraint` (a subclass of `Goal`) is the abstract base. The subtype hierarchy `ConstraintVar0/1/2/List0/List1/List2` indicates arity over `Variable`s; `ConstraintMeta` and `ConstraintVarExpr` are higher-level. Each concrete constraint lives in a folder named after its category — `IntCmp` for var-var comparisons, `IntCmpVal` for var-const comparisons, `IntCons` for n-ary list constraints (`AllDifferent`, `Sum`, `DotProduct`, `Index`, `Min`, `Max`, …), `IntExpr` / `IntExprVal` for arithmetic expression constraints, `IntSearch` for search heuristics. Float side mirrors this with extra `FltTrig` and `FltCons/FltVar{Log,Pow,Exp,Sin,Neg}` for transcendentals. **When adding a new constraint, the folder + class-name prefix decides where it belongs.**
4. **Propagation + search** (`Solver/`): `Solver` owns the variable/constraint lists, the propagation queue (`IPropagationQueue` / `PropagationQueueVar` / `PropagationQueueCons`), the state stack, the goal stack, and an `IntObjective` for optimization. `Goal` (abstract) is the unit of search work; `GoalAnd` / `GoalOr` / `GoalStack` implement the choice-point machinery; `Solver.Solve(goal)` finds the first solution and `Solver.Next()` enumerates further solutions on backtrack. `IntGenerate` / `FltGenerate` and the various `*Selector` / `*Instantiate*` classes are the labeling strategies.
5. **Problem facade** (`Solver/Problem.cs`): thin wrapper holding a `Solver` — `SolverExample` problems (`Sudoku`, `Golomb`, `MagicSquare`, `MoreMoney`) subclass `Problem` and expose a domain-specific API on top.

Key invariants worth knowing before touching the solver:
- `Constraint.Post()` hooks the constraint onto its variables; `Constraint.Add()` registers composite/child constraints; `Constraint.Update()` runs the propagator. `Solver.Add(constraint)` performs all three and assigns `constraint.Index`.
- Constraint propagation strength is set via `constraint.Level = PropagateLevel.{Low,Normal,High}` — see `Sudoku.cs` for typical use.
- Anything that mutates variable state during propagation **must** go through reversible types so it can be undone on backtrack. New constraints commonly hold their working state in `RevValue<T>` or `RevList<T>` fields.
- `Solver` is a `partial` class; the bulk of its API lives in `Solver/Solver/Solver.cs`. `RootNamespace` is `MaraSolver` (despite `AssemblyName` being `Solver`).
- `unsafe` code is enabled for `Solver` and `Interval` (`AllowUnsafeBlocks`); `Solver` Debug defines include a `rop` symbol — leave it alone unless you understand the propagation-queue variant it gates.

### SolverExample, SolverExampleTest, SolverTest, TestApp
- `SolverExample` contains canonical CP textbook problems modelled on top of `Problem`.
- `SolverExampleTest` is the NUnit suite that exercises those examples end-to-end (e.g. `TestGolomb` asserts optimum values for N=3..10).
- `SolverTest` is the lower-level NUnit suite covering individual constraints, interval/domain math, and reversible state, mirroring the `Solver`/`Interval` folder structure.
- `TestApp` is a Windows console app used for ad-hoc experiments and to drive the `Mondriaan` solver (which lives only inside `TestApp/Mondriaan/`, not in `SolverExample`).

## File-history banners

Most `.cs` files start with a `$Log: ...$` block from an old SourceSafe-style VCS — these are historical and should not be updated. Don't extend or reformat them when editing.
