# AGENTS.md

Guidance for AI coding agents working in this repository.

## Overview

Mara is a constraint programming (CP) / finite-domain solver library in C#, originally written 2004-2011 for Visual Studio 2013 / .NET Framework 4.5 and ported to **SDK-style projects targeting `net10.0`**. The `Mara.sln` file is retained — `dotnet` builds it as-is. Licensed Apache 2.0; copyright Patrick de Boer.

## Build and test

Toolchain is the .NET 10 SDK. On Apple Silicon via Homebrew, the SDK lives at `/opt/homebrew/opt/dotnet/libexec`; if `dotnet` isn't on PATH, export `DOTNET_ROOT=/opt/homebrew/opt/dotnet/libexec` and prepend it to PATH.

- Build whole solution: `dotnet build Mara.sln` (Debug by default; add `-c Release` for release).
- Run tests: `dotnet test SolverTest/SolverTest.csproj` and `dotnet test SolverExampleTest/SolverExampleTest.csproj`. Filter to a single test with `--filter "FullyQualifiedName~TestName"`.
- `SolverTest/Program.cs` and `SolverExampleTest/Program.cs` are legacy empty-`Main` files left over from VS2013-era `OutputType=Exe` projects. Both csproj files exclude them via `<Compile Remove="Program.cs" />`. Tests are discovered through NUnit attributes by the test SDK (NUnit 3.14 + NUnit3TestAdapter + Microsoft.NET.Test.Sdk).
- `TestApp` is a console executable used as a scratch driver for examples — `TestApp/Program.cs:Main` selects which demo to run by uncommenting one of the calls (currently `AppMondriaan.Mondriaan()`).
- One known pre-existing test failure: `SolverTest.Interval.Flt.FltIntervalTest.ExpToLog`. The test uses `System.Random` with a fixed seed; .NET 6+ changed the PRNG algorithm, so the sequence differs from what the 2008-era test expected. Not a build regression.

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
- `Solver` Debug defines include a `rop` symbol — leave it alone unless you understand the propagation-queue variant it gates.
- No `unsafe`/`fixed` code anywhere. `Interval/Int/IntDomain.cs` uses `Span<uint>` / `ReadOnlySpan<uint>` for the bit-array hot paths; `Epsilon.cs` and `FltInterval.cs` use `BitConverter.DoubleToInt64Bits` / `Int64BitsToDouble` for double↔Int64 reinterpretation. Don't reintroduce `AllowUnsafeBlocks`.

### SolverExample, SolverExampleTest, SolverTest, TestApp
- `SolverExample` contains canonical CP textbook problems modelled on top of `Problem`.
- `SolverExampleTest` is the NUnit suite that exercises those examples end-to-end (e.g. `TestGolomb` asserts optimum values for N=3..10).
- `SolverTest` is the lower-level NUnit suite covering individual constraints, interval/domain math, and reversible state, mirroring the `Solver`/`Interval` folder structure.
- `TestApp` is a cross-platform console app used for ad-hoc experiments and to drive the `Mondriaan` solver (which lives only inside `TestApp/Mondriaan/`, not in `SolverExample`).

## File-history banners

Most `.cs` files start with a `$Log: ...$` block from an old SourceSafe-style VCS — these are historical and should not be updated. Don't extend or reformat them when editing.
