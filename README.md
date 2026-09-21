# COLLECTIVE

## Creators

- Jhon Chriztopher Nice (TUP-e)
- Josef Magloire Placer (Dxdiag77)

## Overview

We envision a programming language specifically designed around object-oriented programming for game development. Creating something in the language is centered around classes and instances/entities with inherent properties.Python is the benchmark for simplicity, while the language retains the four pillars of Java's object-oriented programming approach. This will mix easy and intuitive syntax withmodularity and safety which are essential in effective game development.

The inspiration and target users for this language are aspiring game developers who have little or no prior programming experience. The word "Colt" can mean an inexperienced individual, so the goal of our language is to make it easier for such aspiring game developers to create at least simple games without first having to learn a large amount of syntax. The scripting experience should be simple and lightweight, minimizing unnecessary syntax rules and avoiding small syntax mistakes such as forgetting a semicolon.

## Host language and build

- Host language: C# v.14
- Version metadata: `global.json` (pins .NET SDK 10.0.100)
- Build: `./build.sh`
- [Anything a fresh clone needs to know.]

## Running it


| Command | What it does |
|---|---|
| `./run <file>` | [Executes a program. Available from Lab 4.] |
| `./run --tokenize <file>` | [Prints the token stream.] |
| `./run --parse <file>` | [Prints the parsed tree.] |
| `./run --eval <file>` | [Evaluates each expression and prints its value.] |
| `./run` | [Starts the REPL.] |


Exit codes:

- `0` — successful execution.
- `65` — lexical or syntax error.
- `70` — runtime error.

## File extension

`[.colt]` [Must match the `ext` field in every tests/lab*/manifest.json.]

## Lexical structure

Collective uses a simple, dynamically typed lexical structure designed for beginner-friendly game development and object-oriented programming. Source code is divided into tokens by scanning the input from left to right. Whitespace and comments are ignored except when newlines are needed for line tracking. The scanner recognizes punctuation, operators, literals, identifiers, reserved keywords, and the end-of-file marker.

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
| `print`    | Prints a value to standard output.                          |
| `if`       | Starts a conditional statement.                             |
| `else`     | Provides an alternative branch for a conditional statement. |
| `while`    | Starts a while loop.                                        |
| `for`      | Starts a for loop.                                          |
| `return`   | Returns a value from a method or function.                  |
| `break`    | Immediately exits the nearest loop.                         |
| `continue` | Skips the remainder of the current loop iteration.          |
| `true`     | Boolean literal representing true.                          |
| `false`    | Boolean literal representing false.                         |
| `nil`      | Represents the absence of a value (commonly known as NULL).                          |

### Operators

| Operator | Category      | Operands | Associativity | Precedence    |
| -------- | ------------- | -------- | ------------- | ------------- |
| `=`      | assignment    | binary   | To Be Defined | To Be Defined |
| `==`     | comparison    | binary   | To Be Defined | To Be Defined |
| `!`      | logical       | unary    | To Be Defined | To Be Defined |
| `!=`     | comparison    | binary   | To Be Defined | To Be Defined |
| `<`      | comparison    | binary   | To Be Defined | To Be Defined |
| `<=`     | comparison    | binary   | To Be Defined | To Be Defined |
| `>`      | comparison    | binary   | To Be Defined | To Be Defined |
| `>=`     | comparison    | binary   | To Be Defined | To Be Defined |
| `+`      | arithmetic    | binary   | To Be Defined | To Be Defined |
| `-`      | arithmetic    | binary   | To Be Defined | To Be Defined |
| `*`      | arithmetic    | binary   | To Be Defined | To Be Defined |
| `/`      | arithmetic    | binary   | To Be Defined | To Be Defined |
| `%`      | arithmetic    | binary   | To Be Defined | To Be Defined |
| `.`      | member access | binary   | To Be Defined | To Be Defined |


` The scanner recognizes both single-character operators and their multi-character forms where applicable.
Operator precedence and associativity will be finalized in the grammar later on. `

### Literals

| Kind    | Syntax          | Produces                                                             |
| ------- | --------------- | -------------------------------------------------------------------- |
| number  | `42`, `3.14`    | A numeric runtime value.                                             |
| string  | `"hello"`       | A string runtime value containing the characters between the double quotes. |
| boolean | `true`, `false` | A boolean runtime value.                                             |
| nil     | `nil`           | The language's value representing the absence of a value.            |

` Numbers support integer and decimal forms. Strings use double quotes. The scanner preserves the source lexeme while storing the interpreted literal value separately in the token.`

Strings support the following escape sequences:

| Escape | Meaning |
|---|---|
| `\n` | newline |
| `\t` | tab |
| `\"` | double quote |
| `\\` | backslash |

Strings cannot span multiple lines.

### Identifiers

* **Start characters:** letters (`A-Z`, `a-z`) or `_`
* **Continue characters:** letters, digits (`0-9`), or `_`
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
* Block comments are not supported.
* Comments are discarded and do not produce tokens.
* Newlines encountered after comments are still counted for source-line tracking.

### Whitespace and termination

* Whitespace is **not significant** to the language.
* Spaces and tabs are discarded by the scanner.
* Newlines are discarded but increment the scanner's line counter.
* Statements are ** terminated by newlines **.
* Blocks are delimited by `{` and `}`.
* Grouped expressions, method calls, and parameter lists use `(` and `)`.
* Arguments and similar comma-separated elements use `,`.
* Member access uses `.`.


## Token output format

```
[one line of real --tokenize output]
```

[What each field means. Frozen as of Lab 1; changes are recorded in the
changelog.]

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

Message format:

```
[one real static error]
[one real runtime error]
```


| Failure | Exit code |
|---|---|
| [lexical error] | 65 |
| [syntax error] | 65 |
| [runtime error] | 70 |


## Testing conventions


| Folder | Activity | Mode | Flag |
|---|---|---|---|
| tests/lab1 | Scanner | sidecar | `--tokenize` |
| tests/lab2 | Parser | sidecar | `--parse` |
| tests/lab3 | Evaluator | inline | `--eval` |
| tests/lab4 | Context | inline | none |
| tests/lab5 | Functions | inline | none |


```
[specific tests]...
```

Run locally with:

```bash
curl -sSL https://raw.githubusercontent.com/WhiteLicorice/cmsc-124-harness/v1.1/run_tests.py -o run_tests.py
./build.sh
python3 run_tests.py tests/lab1
```

## Sample code

```
[a short program]
```

Output:

```
[its output]
```

## Design rationale

[Why the language is the way it is. Cover the choices that surprised you, the
features you cut, and the decisions you reversed. Specific reasons, not
approval of your own work.]

## Known limitations

- [What doesn't work, what is unimplemented, where behavior is worse than you
  would like.]

## Changelog


| Activity | What changed in the language |
|---|---|
| Lab 1 | [entry] |
