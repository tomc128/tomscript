**PLEASE NOTE** There is a major compiler update coming to TomScript, and so much of this documentation may change!

# Welcome to TomScript
#### A completely useless yet undeniably awesome programming language.

This language was created as an extremely high-level programming language suitable for anyone to learn! Originally started as a joke, we have been able to create a fully-functioning coding language for all to enjoy!
TomScript is built atop a Python-based compiler, which results in an extremely light-weight and efficient language.


## Features

- Console printing
- Variable declaration
- Calculation
- Simple and human-readable syntax
- Effortless build configuration
- Exe and Python build support


## How to use TomScript

Using TomScript is simple! Simply create a `.tms` file containing your code, edit the config to your preference, and run the compiler!


## System Requirements

- Windows 7 or newer
- Python 3.7 or newer


## Basic syntax

Every TomScript program is structured in the following way:
    
    [Optional Comments]
    
    start

    // code
    
    end

As you can see, the "start" and "end" syntax signals to the compiler the start and end of the code.

### Printing to screen

Printing to the screen is called using the "write" syntax. Simply type "write" followed by the string, integer or variable you wish to print.

    write Hello, world!

### Reading user input

To read input from the user, simply use the "user input" syntax, as shown:

    set variablename to user input

### Creating a variable

Creating a variable is simple! Use the "create variable" syntax, as follows:

    create variable name


Want to set a default value? Just use:

    create variable name and set variable to Steve

Here, we use "and" to tell the compiler to combine these operations.

### Setting a variable

Want to change the value of a variable during your program? That's easy:

    set name to Jim

<<<<<<< Updated upstream
### Calculating values

Want to add some spice to your life using numbers? Just use the "calculate" statement:
=======
TomScript automatically detects the value of the variable from a set statement. This means you'll need to set a default value in order to use numbers in calculations or loops:

    create variable count and set count to 1
    set count to user input
    repeat count times

### Calculating values

Want to perform a calculation? That's easy with TomScript, thanks to how lazily I coded it! It's powered by Python's `eval()` function, so you can use almost any mathematical expression you'd like:

    set myVariable to the result of 2 * (2 + 2) / 4 + (14)**(0.5)

### Printing to screen

Printing to the screen is called using the "write" syntax. Simply type "write" followed by the string, integer or variable you wish to print.

    write Hello, world!

    write So, you are called name?
>>>>>>> Stashed changes

    calculate 6 + 7

To use the result of this calculation, TomScript handily creates a variable called "result" which works just like any other variable!

    write result
    create variable calculation and set calculation to result

### Comments

TomScript supports comments, meaning you can annotate your code without the compiler adding it to your program! Simply precede your comment with "//" like so:

    // this is a comment!


<<<<<<< Updated upstream
## Configuration

The `tms.config` file specifies the options that the compiler should use. It includes the following fields:
=======
What about a condition loop? We've got you covered:

    repeat while count is greater than or equal to 7

    // code

    stop

Remember, the variable used must be an integer, so it'll need a default type.

## Compiler UI

The new C# compiler comes with a slick new UI.
>>>>>>> Stashed changes

    source sourceCode.tms
    output sourceCode.tms.py

The `source` parameter specifies the input TomScript, and the `output` parameter specifies the Python output file. You should set these to your desired source file and output file.


### Supported build parameters

<<<<<<< Updated upstream
    option compile comments // when building to a Python file, this option converts your code's comments to Python comments
    
    option compile executable // this option builds to an exe file, as well as a Python file
    
    option compile debug // this option builds to an exe file, while retaining all temporary build files that are generated
    
    option commentLines 0 // allows you to comment before start in your tms file, edit to accommodate how many lines of comments you use, if any
=======
And the checkbox to specify whether you'd also like to compile the outputted Python to an executable file. You'll need access to pip - pyinstaller will be automagically installed.

Finally, the compile button lets you compile the checked scripts.
>>>>>>> Stashed changes


Any questions? Feel free to create an issue or fork the repo and fix it yourself!
