# Welcome to TomScript
#### A completely useless yet undeniably awesome programming language.

This language was created as an extremely high-level programming language suitable for anyone to learn! Originally started as a joke, we have been able to create a fully-functioning coding language for all to enjoy! TomScript code is compiled using a custom C# compiler into Python.


## Features

- Console printing
- Variable declaration
- Simple and human-readable syntax
- Effortless build configuration
- Python and exe build support


## How to use TomScript

Using TomScript is simple! Simply create a `.tms` file containing your code, and run the compiler!


## System Requirements

- Windows 7 or newer
- .NET Framework 4.8 for compilation of scripts
- Python 3.x for execution of scripts
- (optional) Access to `pip` for automagic install of `pyinstaller` for exe compilation


## Basic syntax

Every TomScript program is structured in the following way:
    
    code properties
    
    start

    // code
    
    end

As you can see, the "start" and "end" syntax signals to the compiler the start and end of the code.

## Code Properties

TomScript currently has one property that can be changed on a script-by-script basis.

    language {language}

This changes the language subset used in the code. Each language subset contains different words for each command. Currently, these languages exist:

- standard
- friendly
- fancy

The default language is `standard`, which will be used in the documentation here on out.

### Creating a variable

Creating a variable is simple! Use the "create variable" syntax, as follows:

    create variable name


Want to set a default value? Just use:

    create variable name and set name to Steve

### Combining commands

As you can see from the example above, commands can be combined with the command `and`, which may vary across language subsets.

    write Welcome to TomScript! and set input to read

### Setting a variable

Want to change the value of a variable during your program? That's easy:

    set name to Jim

TomScript automatically detects the value of the variable from a set statement. This means you'll need to set a default value in order to use numbers in calculations or loops.

### Printing to screen

Printing to the screen is called using the "write" syntax. Simply type "write" followed by the string, integer or variable you wish to print.

    write Hello, world!
    write So, you are called name?

### Reading user input

To read input from the user, simply use the "user input" syntax, as shown:

    set variablename to user input

### Loops

A forever loop will execute the code inside of it continually:

    repeat forever

    // code

    stop

Don't want an endless loop? TomScript has you covered with another loop:

    repeat 10 times

    // code

    stop

Remember, you can replace `10` with any integer variable you like.

Still not right? Use a conditional loop:

    repeat while count is less than or equal to maxIterations

    // code

    stop

Remember, the variable needs to be an integer for this to work.

### Calculating values

Want to perform a calculation? That's easy with TomScript, thanks to how lazily I coded it! It's powered by Python's `eval()` function, so you can use almost any mathematical expression you'd like:

    set myVariable to the result of 2 * (2 + 2) / 4 + (14)**(0.5)

### Comments

TomScript supports comments, meaning you can annotate your code without the compiler adding it to your program! Simply precede your comment with "//" like so:

    // This is a comment!

They work both after lines...

    create variable name // This will hold, believe it or not, the user's name

... or on their own

    // TODO: actual code
    write Something should happen here, but it doesn't.

## Compiler UI

The new C# compiler comes with a slick new UI.

![Compiler UI](img/Compiler.png)

TomScript files can be added by pasting the path of one at the top of the program, or by using the browse button. You'll need to click `+ Add Source File` for the file to be added to the queue. Alternatively, select multiple files with the browse button and they will be automatically added.

The centre of the program shows the queue of files to process, with checkboxes in case you'd like to only compile certain files that you've added.

Beneath this is the output directory text box, complete with browse button.

There's a checkbox which enables you to specify if you'd like to compile the py file to an exe file. This requires `pyinstaller`, which we'll automagically install using `pip`.

Finally, the compile button lets you compile the checked scripts.


Any questions? Feel free to create an issue or fork the repo and fix it yourself!
