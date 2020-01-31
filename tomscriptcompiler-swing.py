import os
import datetime
import re
import argparse

CONFIG_PATH = './tms.config'
INDENTATION = '    '

result = ''

sourceFile = ''
outputPyFile = ''
outputExeFile = ''

codeLanguage = 'standard'
identifiers = {}

tokens = []
variables = {}
sourceLines = []


def loadConfig():
    global identifiers

    for filename in os.listdir('./lang/'):
        with open('./lang/' + filename, 'r') as lang:
            lines = lang.readlines()
            values = {}
            for line in lines:
                values[line.split('=')[1].strip()] = line.split('=')[0]
            identifiers[os.path.splitext(os.path.basename(filename))[0]] = values


def loadSourceFile():
    with open(sourceFile, 'r') as source:
        source = source.read().replace('\n', '\n\n')
        return source.splitlines()


def featureDetection():
    global sourceLines, codeLanguage
    
    sourceLines = [line for line in sourceLines]

    for line in sourceLines:
        if line.startswith('language'):
            codeLanguage = line.split(' ')[1].strip()
            sourceLines.remove(line)


def intialTokenReplacement():
    global sourceLines

    keys = list(identifiers[codeLanguage].keys())

    for i in range(len(sourceLines)):
        for key in keys:
            if key.lower() in sourceLines[i].lower():
                sourceLines[i] = re.sub(key, identifiers[codeLanguage][key].strip(), sourceLines[i], flags=re.IGNORECASE)

    targetIndex = sourceLines.index('START_PROGRAM')
    sourceLines = sourceLines[targetIndex:]





def lexicalAnalysis():
    global sourceLines

    sourceLines = [item.strip() for item in sourceLines]  # Strip all of the elements of the list

    lexemes = []  # Creates a list of the lexemes in the TomScript code

    for line in sourceLines:  # Loop through each line,
        if (line == ''):  # checking if it is empty,
            lexemes.append('|')  # if it is add a pipe to the lexemes list
            continue  # and continue the next iteration

        lexeme = ''  # Create the current lexeme,
        previousTokenKnown = False
        # and loop through each character in the line
        for i, char in enumerate(line):
            if char == '\n':
                lexemes.append('|')
                lexeme = ''

            lexeme += char  # Add the current character to the current lexeme
            if (i+1 < len(line)):  # If the next character exists,
                if (line[i+1] == ' '):  # and it is whitespace,

                    if (tokenIsKnown(lexeme)):
                        #lexemes.append(identifiers[codeLanguage][lexeme.strip()])
                        lexemes.append(lexeme.strip())
                        previousTokenKnown = True
                    else:
                        if (previousTokenKnown):
                            lexemes.append(lexeme.strip())
                        else:
                            lexemes.append(lexeme)
                        previousTokenKnown = False

                    lexeme = ''

        if (tokenIsKnown(lexeme)):
            #lexemes.append(identifiers[codeLanguage][lexeme.strip()].strip())
            lexemes.append(lexeme.strip())
            previousTokenKnown = True
        else:
            if (previousTokenKnown):
                lexemes.append(lexeme.strip())
            else:
                lexemes.append(lexeme)
            previousTokenKnown = False

    print(lexemes)
    return lexemes


def tokenAnalysis(tokens):
    updatedTokens = []

    for token in tokens:
        if token.lower() in identifiers[codeLanguage]:
            updatedTokens.append(identifiers[codeLanguage].get(token.lower()))
        else:
            updatedTokens.append(token)

    return updatedTokens


def tokenIsKnown(token):
    return token.strip() in identifiers[codeLanguage].values()



def logError(code, message):
    print(f'Error ({str(code)}) : {message}')


def parseCode(prefix=''):
    global tokens, variables

    translatedCode = ''

    if (tokens[0] == '|'):
        tokens = tokens[1:]

    elif (tokens[0] == 'WRITE'):
        tokens = tokens[1:]
        translatedCode += 'print("'

        while (tokens[0] != '|'):
            if (tokens[0].strip() in variables):
                translatedCode += '" + ' + tokens[0].strip() + ' + "'
            else:
                translatedCode += tokens[0]
            tokens = tokens[1:]

        translatedCode += '")\n'
        tokens = tokens[1:]

    elif (tokens[0] == 'CREATE'):
        if (tokens[1] == 'VARIABLE'):
            tokens = tokens[2:]
            variableName = tokens[0]
            variables[variableName] = None

            translatedCode += variableName + ' = None\n'
            tokens = tokens[1:]

    elif (tokens[0] == 'SET'):
        variableName = tokens[1]
        tokens = tokens[3:]
        variableValue = ''

        if (tokens[0] == 'READ'):
            variableValue += parseCode()
            tokens = tokens[1:]
            variables[variableName] = 'READ'
            translatedCode += variableName + ' = input()\n'
        else:
            while (tokens[0] != '|'):
                variableValue += tokens[0]
                tokens = tokens[1:]
                variables[variableName] = variableValue
                translatedCode += variableName + ' = "' + ''.join(variableValue) + '"\n'
        tokens = tokens[1:]

    elif (tokens[0] == 'READ'):
        tokens = tokens[1:]
        translatedCode += 'input()'

    elif (tokens[0] == 'REPEAT'):
        if (tokens[1] == 'FOREVER'):
            tokens = tokens[2:]
            translatedCode += 'while True:\n'

            while (tokens[0] != 'REPEAT_END'):
                translatedCode += parseCode(prefix=INDENTATION)

            tokens = tokens[1:]
        elif (tokens[2] == 'TIMES'):
            times = tokens[1]
            tokens = tokens[3:]
            translatedCode += 'for i in range(' + times + '):\n'

            while (tokens[0] != 'REPEAT_END'):
                translatedCode += parseCode(prefix=INDENTATION)

            tokens = tokens[1:]

    if (translatedCode != ''):
        translatedCode = prefix + translatedCode

    return translatedCode


def generateCode():
    global tokens
    translatedCode = ''

    if not (tokens[0] == 'START_PROGRAM' and tokens[-1] == 'END_PROGRAM'):
        logError(1, 'Start and/or end token(s) missing from source file.')
        return ''

    tokens = tokens[1:-1]

    i = 0
    while len(tokens) > 0:
        translatedCode += parseCode()
        print('Compiling .' + ' .'*i, end='\r')
        i += 1

    return translatedCode


def main():
    global sourceFile, sourceLines, tokens, outputPyFile

    try:
        # Get the arguments from the command line
        parser = argparse.ArgumentParser(description='Compile tomscript code.')
        parser.add_argument('tomscriptfile', type=str, help='Provide the source file.')
        parser.add_argument('outputpyfile', type=str, help='Path of the outputted py file.')
        parser.add_argument('-o', '--output-exe-file', type=str, help='Path of the outputted exe file (optional).')
        parser.add_argument('-e', '--instant-exit', action='store_true', help='Disables the "Press enter to exit..." message at the end of the program (optional).')
        args = parser.parse_args()

        sourceFile = args.tomscriptfile
        outputPyFile = args.outputpyfile
        outputExeFile = args.output_exe_file
        instantExit = args.instant_exit == True
    except:
        # Ask the user for instructions in the terminal
        sourceFile = input('Source .tms file: ').strip()
        outputPyFile = input('Output .py file: ').strip()
        raw = input('Output .exe file (enter to skip): ').strip()
        outputExeFile = None if raw == '' else raw
        raw = input('Disable enter-quit message (y/n, default n): ').strip().lower()
        instantExit = False if raw == 'n' or raw == '' else True


    startTime = datetime.datetime.now() # Get the time of the start of operations

    loadConfig() # Read all the installed languages into memory
    sourceLines = loadSourceFile() # Read the TMS source file
    featureDetection() # Detect features of the script
    intialTokenReplacement() # Replace intial tokens according to the chosen language
    tokens = lexicalAnalysis() # Analyse and split the code into tokens
    code = generateCode() # Generate the code from the tokens


    if (code == ''):
        quit() # Quit if the code is blank


    if not instantExit:
        code += '\nprint("Press enter to exit...")\ninput()' # Add an input call if the user doesn't request instant exit


    with open(outputPyFile, 'w') as file:
        file.write(code) # Write the code to a python file

    if outputExeFile is not None: # If the user requests it,
        os.system(f'cd {os.getcwd()}') # Navigate to the current working directory
        os.system('pip install pyinstaller') # Install pyinstaller, if it isn't already

        exeDir = os.path.split(outputExeFile)[0]
        exeName = os.path.basename(outputExeFile)

        os.system(f'pyinstaller {outputPyFile} -F --distpath {exeDir} -n {exeName}') # Compile the python file into an exe file
        os.system(f'rmdir /Q /S build') # Remove temporary folder

    
    endTime = datetime.datetime.now() # Get the time of the end of operations
    elapsedTime = endTime - startTime # Calculate the time difference
    print('\nCompiled in ' + str(elapsedTime.total_seconds() * 1000) + 'ms.') # Output the time difference

    print('Thanks for using TomScript. Press enter to exit...')
    input()


if __name__ == "__main__":
    main()