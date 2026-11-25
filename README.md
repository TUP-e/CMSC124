# COLLECTIVE

## Creators

- Jhon Chriztopher Nice (TUP-e)
- Josef Magloire Placer (Dxdiag77)

## Overview

We envision a programming language specifically designed around object-oriented programming for game development. Creating something in the language is centered around classes and instances/entities with inherent properties. Python is the benchmark for simplicity, while the language retains the four pillars of Java's object-oriented programming approach. This will mix easy and intuitive syntax with modularity and safety which are essential in effective game development.

The inspiration and target users for this language are aspiring game developers who have little or no prior programming experience. The word "Colt" can mean an inexperienced individual, so the goal of our language is to make it easier for such aspiring game developers to create at least simple games without first having to learn a large amount of syntax. The scripting experience should be simple and lightweight, minimizing unnecessary syntax rules and avoiding small syntax mistakes such as forgetting a semicolon.

## Host language and build

- Host language: C# v.14
- Version metadata: `global.json` (pins .NET SDK 10.0.100)
- Build: `./build.sh`
- A fresh clone needs the .NET SDK selected by `global.json`; `./build.sh` restores and publishes the interpreter to `build/`.

## Running it


| Command | What it does |
|---|---|
| `./run <file>` | Executes a program. Program execution arrives in Lab 4; until then this prints the Lab 0 line. |
| `./run --tokenize <file>` | Prints the token stream, one `Token(...)` line per token. |
| `./run --parse <file>` | Prints the parsed expression tree. Added in Lab 2. |
| `./run --eval <file>` | Evaluates each expression and prints its value. Added in Lab 3. |
| `./run` | Starts the REPL. At present it prints the tokens of each line. |


Exit codes:

- `0` — successful execution.
- `65` — lexical or syntax error.
- `70` — runtime error.

## File extension

`.colt`

Every `tests/lab*/manifest.json` uses `"ext": ".colt"`.

## Lexical structure

Collective uses a simple, dynamically typed lexical structure designed for beginner-friendly game development and object-oriented programming. Source code is divided into tokens by scanning the input from left to right. Whitespace and comments are ignored, except that newlines are counted for line tracking and mark where statements end. Newlines are not emitted as tokens. The scanner recognizes punctuation, operators, literals, identifiers, reserved keywords, and the end-of-file marker.

### Keywords

| Keyword    | Purpose                                                     |
| ---------- | ----------------------------------------------------------- |
| `class`    | Declares a class.                                           |
| `extends`  | Declares that a class inherits from another class.          |
| `new`      | Creates a new instance of a class.                          |
| `this`     | Refers to the current object instance.                      |
| `public`   | Declares a member as publicly accessible.                   |
| `private`  | Declares a member as accessible only within its class.      |
| `abstract` | Declares an abstract class or member.                       |
| `var`      | Declares a variable.                                        |
| `def`      | Declares a function or method.                              |
| `print`    | Prints a value to standard output.                          |
| `if`       | Starts a conditional statement.                             |
| `else`     | Provides an alternative branch for a conditional statement. |
| `while`    | Starts a while loop.                                        |
| `for`      | Reserved keyword retained for now; its syntax is not implemented yet. |
| `in`       | Reserved for possible `for`-`in` syntax.                    |
| `return`   | Returns a value from a method or function.                  |
| `break`    | Immediately exits the nearest loop.                         |
| `continue` | Skips the remainder of the current loop iteration.          |
| `and`      | Short-circuiting logical AND.                               |
| `or`       | Short-circuiting logical OR.                                |
| `true`     | Boolean literal representing true.                          |
| `false`    | Boolean literal representing false.                         |
| `nil`      | Represents the absence of a value (commonly known as NULL). |

### Operators

| Operator | Category      | Operands | Associativity | Precedence    |
| -------- | ------------- | -------- | ------------- | ------------- |
| `=`      | assignment    | binary   | right         | 1 (lowest)    |
| `or`     | logical (keyword) | binary | left        | 2             |
| `and`    | logical (keyword) | binary | left        | 3             |
| `==` `!=` | equality     | binary   | left          | 4             |
| `<` `<=` `>` `>=` | comparison | binary | left     | 5             |
| `+` `-`  | additive      | binary   | left          | 6             |
| `*` `/` `%` | multiplicative | binary | left        | 7             |
| `!` `-`  | unary         | unary    | right         | 8             |
| `.` `()` | member access / call | postfix | left     | 9 (highest)   |


The scanner recognizes both single-character operators and their multi-character forms. All binary operators are left-associative except assignment, which is right-associative. Precedence is encoded in the grammar added in Lab 2.

### Token inventory

The scanner emits 46 token types, including `EOF`, which is emitted once at the end of every file. `FOR` remains reserved by the group's current decision; its loop syntax is not part of the scanner's responsibilities. This updates the draft lab plan's recommendation to drop `for`.

| Token type | Example lexeme | Purpose |
|---|---|---|
| `LEFT_PAREN` | `(` | Opens a grouped expression or argument/parameter list. |
| `RIGHT_PAREN` | `)` | Closes a group or argument/parameter list. |
| `LEFT_BRACE` | `{` | Opens a block or class body. |
| `RIGHT_BRACE` | `}` | Closes a block or class body. |
| `DOT` | `.` | Member-access punctuation. |
| `COMMA` | `,` | Separates arguments and parameters. |
| `PLUS` | `+` | Addition or unary plus syntax. |
| `MINUS` | `-` | Subtraction or unary negation. |
| `STAR` | `*` | Multiplication. |
| `SLASH` | `/` | Division; `//` begins a line comment. |
| `PERCENT` | `%` | Remainder. |
| `EQUAL` | `=` | Assignment. |
| `EQUAL_EQUAL` | `==` | Equality comparison. |
| `NOT` | `!` | Logical negation. |
| `NOT_EQUAL` | `!=` | Inequality comparison. |
| `LESS` | `<` | Less-than comparison. |
| `LESS_EQUAL` | `<=` | Less-than-or-equal comparison. |
| `GREATER` | `>` | Greater-than comparison. |
| `GREATER_EQUAL` | `>=` | Greater-than-or-equal comparison. |
| `IDENTIFIER` | `player` | User-defined name that is not a reserved word. |
| `STRING` | `"hello"` | Double-quoted string literal. |
| `NUMBER` | `42`, `.5` | Numeric literal. |
| `CLASS` | `class` | Class declaration. |
| `EXTENDS` | `extends` | Class inheritance clause. |
| `NEW` | `new` | Instance-creation expression. |
| `THIS` | `this` | Current-instance reference. |
| `PUBLIC` | `public` | Public member visibility. |
| `PRIVATE` | `private` | Private member visibility. |
| `ABSTRACT` | `abstract` | Abstract class or method declaration. |
| `VAR` | `var` | Variable declaration. |
| `DEF` | `def` | Function or method declaration. |
| `PRINT` | `print` | Value-output statement. |
| `IF` | `if` | Conditional statement. |
| `ELSE` | `else` | Alternate conditional branch. |
| `WHILE` | `while` | While-loop statement. |
| `FOR` | `for` | Reserved keyword retained for now; syntax is not implemented yet. |
| `IN` | `in` | Reserved for possible `for`-`in` syntax. |
| `RETURN` | `return` | Function or method return statement. |
| `BREAK` | `break` | Exits the nearest loop. |
| `CONTINUE` | `continue` | Skips to the next loop iteration. |
| `AND` | `and` | Short-circuiting logical conjunction. |
| `OR` | `or` | Short-circuiting logical disjunction. |
| `TRUE` | `true` | Boolean true literal. |
| `FALSE` | `false` | Boolean false literal. |
| `NIL` | `nil` | Absence-of-value literal. |
| `EOF` | *(empty)* | End-of-file marker used by the parser. |

### Literals

| Kind    | Syntax          | Produces                                                             |
| ------- | --------------- | -------------------------------------------------------------------- |
| number  | `42`, `3.14`, `.5`, `1_000` | A numeric runtime value.                                |
| string  | `"hello"`       | A string runtime value containing the characters between the double quotes. |
| boolean | `true`, `false` | A boolean runtime value.                                             |
| nil     | `nil`           | The language's value representing the absence of a value.            |

Number rules:

- A number is an optional integer part followed by an optional fractional part, with at least one digit in total.
- A leading dot is allowed: `.5` is the number `0.5`.
- A dot is part of the number only when a digit follows it. `5.` scans as `NUMBER(5)` followed by `DOT`, and so does `5.toString`.
- Underscores may separate digits: `1_000` and `1_000.25`. They must sit between digits, so `1_`, `1__0`, and `1_.0` are rejected as invalid numbers, while `_1` is an identifier.
- Commas do not group digits: `1,000` scans as `NUMBER(1)`, `COMMA`, `NUMBER(000)`.
- There is no exponent notation: `1e5` is rejected as an invalid number or identifier.
- A leading `-` is the unary minus operator, not part of the literal.

The scanner keeps the source lexeme (`1_000`) and stores the parsed value (`1000`) separately in the token. Strings use double quotes only; a single quote is an unrecognized character.

Strings support the following escape sequences:

| Escape | Meaning |
|---|---|
| `\n` | newline |
| `\t` | tab |
| `\"` | double quote |
| `\\` | backslash |

Strings cannot span multiple lines. A newline before the closing quote is an unterminated-string error.

Any other escape sequence, such as `\q`, is an error. The scanner reports the invalid escape, drops both the backslash and the escaped character, keeps scanning the rest of the string, and exits 65 after printing the tokens it managed to scan. Adjacent string literals are separate tokens; there is no implicit concatenation.

### Identifiers

* **Start characters:** letters (`A-Z`, `a-z`) or `_`
* **Continue characters:** letters, digits (`0-9`), or `_`
* **Alphabet:** ASCII letters only; non-ASCII letters are not identifiers.
* **Case-sensitive:** Yes
* Identifiers may not begin with a digit.
* Reserved keywords are recognized as keyword tokens rather than identifiers.

Examples of valid identifiers:

```text
player
Player
player1
_player
enemy_health
```

Examples of invalid identifiers:

```text
1player
```

### Comments

* **Line comments:** `//`
* Everything from `//` to the end of the current line is ignored by the scanner.
* Line comments may appear after other source code on the same line.
* Block comments are not supported. `/*` is not a comment; it scans as `/` followed by `*`.
* Comments are discarded and do not produce tokens.
* Newlines encountered after comments are still counted for source-line tracking.

### Whitespace and termination

* Spaces, tabs, and carriage returns are discarded by the scanner.
* Newlines are not emitted as tokens, but they increment the line counter and mark the end of a statement.
* Blank lines and leading or trailing whitespace are ignored.
* Statements are line-based. A statement ends at a line break; an expression continues onto the next line only when the previous line ends with an operator, `(`, `,`, `=`, or `.`.
* There is no semicolon terminator. `;` is an unrecognized character.
* Blocks are delimited by `{` and `}`.
* Grouped expressions, method calls, and parameter lists use `(` and `)`.
* Arguments and similar comma-separated elements use `,`.
* Member access uses `.`.


## Token output format

`--tokenize` prints one line per token, in source order, followed by the EOF token:

```text
Token(type=EQUAL_EQUAL, lexeme="==", literal=null, line=2)
Token(type=NUMBER, lexeme="1_000", literal=1000, line=2)
Token(type=STRING, lexeme=""a\nb"", literal=a\nb, line=3)
Token(type=EOF, lexeme="", literal=null, line=4)
```

- `type` — the `TokenType` name.
- `lexeme` — the exact source text of the token, printed verbatim between quotes. A string token therefore shows its own quotes doubled.
- `literal` — the parsed value. Numbers print in host double form (`1`, `0.5`); strings re-escape `\\`, `\n`, `\t`, and `\"`; booleans print `true` or `false`; every other token, including `nil`, prints `null`.
- `line` — the 1-based line on which the token starts. EOF takes the line reached after the final newline.

The format is frozen as of Lab 1. Any change must be recorded in the changelog.

## Grammar

```
[Your complete context-free grammar, current as of the latest activity.
Unambiguous, with precedence and associativity encoded in rule structure.]
```

## Parse output format

```
[one line of real --parse output, e.g. (+ 1.0 (* 2.0 3.0))]
```

- Groupings print as: [form]
- Numbers print as: [form]

## Semantics

### Values and types

[What runtime values exist, and how they are represented in the host
language.]

### Value printing

- Numbers: [e.g. 5 rather than 5.0]
- Nil: [spelling]
- Strings: [with or without quotes]

### Truthiness

[The complete rule. Which values are false in a condition; everything else is
true.]

### Operator semantics

- Arithmetic: [accepted operand types]
- `+` on strings: [concatenation, error, or coercion]
- Mixed types: [what happens]
- Comparison: [accepted operand types]
- Equality across types: [false, or an error]
- Division by zero: [value produced, or runtime error]

### Scope and bindings

- Redeclaration in the same scope: [allowed or an error]
- Uninitialized variable holds: [value]
- Shadowing: [behavior]
- Undefined name: [static error with exit 65, or runtime error with exit 70]

### Control flow and functions

- Logical operators return: [booleans, or the operand]
- Dangling else binds to: [which if]
- Closure capture of a loop variable: [per iteration, or shared]
- Function with no return statement produces: [value]
- Arity mismatch: [message and exit code]

## Native functions


| Name | Arguments | Returns | Notes |
|---|---|---|---|
| [name] | [count and types] | [type] | [caveats] |


## Errors and diagnostics

Message format (real output for `a;b`):

```text
[line 1, col 2] Error: Unexpected character ';'.
  1 | a;b
    |  ^
  Trace: 0 earlier line(s) read, 1 token(s) scanned before this error:
    line 1: IDENTIFIER(a) <-- error here
Token(type=IDENTIFIER, lexeme="a", literal=null, line=1)
Token(type=IDENTIFIER, lexeme="b", literal=null, line=1)
Token(type=EOF, lexeme="", literal=null, line=1)
Scanning reached the end of the file (line 1): 2 valid token(s), 1 error(s).
```

The `[line N, col C]` diagnostic, the source line, the caret, and the trace go to stderr. The token dump and the final summary go to stdout. A file is rejected only after the whole input has been scanned, so every valid token still appears.

Runtime errors keep the same `[line N, col C]` message prefix and exit 70; the evaluator arrives in Lab 3.


| Failure | Exit code |
|---|---|
| Lexical error: unexpected character, invalid number, invalid escape, unterminated string | 65 |
| Syntax error: parser could not build a tree | 65 |
| Runtime error: bad operand type, division by zero, undefined name, arity mismatch | 70 |
| Usage error: unknown flag or missing file argument | 65 |
| Accepted: clean scan or successful execution | 0 |


## Testing conventions


| Folder | Activity | Mode | Flag |
|---|---|---|---|
| tests/lab1 | Scanner | sidecar | `--tokenize` |
| tests/lab2 | Parser | sidecar | `--parse` |
| tests/lab3 | Evaluator | inline | `--eval` |
| tests/lab4 | Context | inline | none |
| tests/lab5 | Functions | inline | none |


Each source file under `tests/lab1` is paired with a `.expected` file holding the exact stdout of `./run --tokenize`. A rejected file also has a `.exit` file containing `65`. Expectations are read and signed off, never regenerated blindly. The folder covers keywords, identifiers, numbers (leading dot, trailing dot, underscores, commas), strings and escapes, comments, whitespace and line tracking, multi-character operators with their single-character neighbors, and error recovery.

On Windows, run these commands from Git Bash; on Linux/macOS, run them from Bash:

```bash
curl -sSL https://raw.githubusercontent.com/WhiteLicorice/cmsc-124-harness/v1.1/run_tests.py -o run_tests.py
./build.sh
python3 run_tests.py tests/lab0
python3 run_tests.py tests/lab1
```

## Sample code

```text
var player = new Player("josef")
player.hp = 100
```

Output of `./run --tokenize`:

```text
Token(type=VAR, lexeme="var", literal=null, line=1)
Token(type=IDENTIFIER, lexeme="player", literal=null, line=1)
Token(type=EQUAL, lexeme="=", literal=null, line=1)
Token(type=NEW, lexeme="new", literal=null, line=1)
Token(type=IDENTIFIER, lexeme="Player", literal=null, line=1)
Token(type=LEFT_PAREN, lexeme="(", literal=null, line=1)
Token(type=STRING, lexeme=""josef"", literal=josef, line=1)
Token(type=RIGHT_PAREN, lexeme=")", literal=null, line=1)
Token(type=IDENTIFIER, lexeme="player", literal=null, line=2)
Token(type=DOT, lexeme=".", literal=null, line=2)
Token(type=IDENTIFIER, lexeme="hp", literal=null, line=2)
Token(type=EQUAL, lexeme="=", literal=null, line=2)
Token(type=NUMBER, lexeme="100", literal=100, line=2)
Token(type=EOF, lexeme="", literal=null, line=3)
```

## Design rationale

- **No semicolons.** Newlines already exist in every source file, the scanner already tracks them, and a beginner is more likely to forget a terminator than to miscount a line. The parser uses token line numbers instead of a `NEWLINE` token, which keeps the scanner and the token output simple. Expressions continue onto the next line only when the previous line ends with an operator, `(`, `,`, `=`, or `.`; that rule makes continuation visible at the end of the line instead of hiding it at the start of the next one.
- **Word logical operators.** `and` and `or` read like English and match the target audience. `!` and `!=` stay because they are already familiar and removing them would make unary negation and inequality inconsistent.
- **OOP keywords reserved early.** Classes, inheritance, and access control are the language's premise, so the scanner reserves all seven words from the beginning. Reserving them now means one stable token vocabulary across the labs and no later renaming of user identifiers. `var` and `def` join them so bindings and functions are always explicit: assignment updates an existing name, and a typo is a runtime error instead of a silent new binding.
- **Keep `for` reserved for now.** The draft lab plan recommended dropping it, but the current group decision is to retain the keyword while its eventual loop syntax remains undecided. This keeps the scanner vocabulary stable until that syntax is settled.
- **Numeric ergonomics.** `.5` is accepted because it is readable; `5.` is split into a number and a dot so member access such as `5.toString` stays unambiguous. Underscores group digits the way Python does, while commas are rejected as separators because they already separate arguments.
- **Small string escape set.** `\n`, `\t`, `\"`, and `\\` cover the common cases. An invalid escape is reported rather than silently dropped, because a string that quietly loses a character is harder to debug than a rejected file.
- **Stable token dump.** A fixed, grep-able `Token(...)` format lets every lab compare exact output and makes scanner regressions visible in CI.
- **Diagnostics with context.** Every lexical error prints the location, the source line, a caret, and the tokens scanned so far. That trace is why the scanner prints its partial output before exiting 65.

## Known limitations

- The parser, evaluator, statements, functions, and classes are scheduled for Labs 2–5. Only the scanner is implemented as of Lab 1.
- `--parse`, `--eval`, and plain program execution are documented but not available until Labs 2–4.
- `for` and `in` are reserved, but their loop syntax is not implemented in Lab 1.
- No block comments, no multiline strings, no exponent notation, and no comma digit separators.
- Identifiers are ASCII-only; Unicode identifiers are not supported.
- A rejected file prints its partial token dump to stdout before exiting 65. This is deliberate and was confirmed with the instructor; the Lab 1 manual's default says nothing rejected belongs on stdout.
- There is no type annotation syntax; `var` is always dynamically typed.

## Changelog


| Activity | What changed in the language |
|---|---|
| Lab 1 | Added `var`, `def`, `and`, `or`, and `in` to the reserved words; retained `for` as a keyword by the group's current decision. Froze the token output format and the lexical rules for numbers, strings, comments, and line-based termination. Documented the partial token dump on lexical errors. |
