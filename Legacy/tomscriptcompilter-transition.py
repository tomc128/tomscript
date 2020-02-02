import os

CONFIG_PATH = './tms.config'

sourceFile = ''
outputPyFile = ''
outputExeFile = ''

tokenIdentifiers = {
    'start': 'START_PROGRAM',
    'end': 'END_PROGRAM',
    'create': 'CREATE',
    'variable': 'VARIABLE',
    'write': 'WRITE',
    'set': 'SET',
    'to': 'TO',
    'input': 'INPUT',
    'calculate': 'CALCULATE',
    'repeat': 'REPEAT',
    'forever': 'FOREVER',
    'and': '|',
    'result': 'RESULT',
    'if': 'IF',
    'stop': 'STOP'
}


def loadConfig():
    global sourceFile, outputPyFile, outputExeFile

    with open(CONFIG_PATH, 'r') as config:
        for line in config.readlines():
            line = line.strip()
            if line.startswith('source-file'):
                sourceFile = line[12:]
            elif line.startswith('output-py-file'):
                outputPyFile = line[15:]
            elif line.startswith('output-exe-file'):
                outputExeFile = line[16:]


def loadSourceFile():
    with open(sourceFile, 'r') as source:
        source = source.read().replace('\n', '\n\n')
        return source.splitlines()


def lexicalAnalysis():
    sourceLines = loadSourceFile()  # Contains each line of the TomScript file
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
                print('EOL')
                lexemes.append('|')
                lexeme = ''

            lexeme += char  # Add the current character to the current lexeme
            if (i+1 < len(line)):  # If the next character exists,
                if (line[i+1] == ' '):  # and it is whitespace,

                    if (tokenIsKnown(lexeme)):
                        lexemes.append(
                            tokenIdentifiers[lexeme.strip().lower()])
                        previousTokenKnown = True
                    else:
                        if (previousTokenKnown):
                            lexemes.append(lexeme.strip())
                        else:
                            lexemes.append(lexeme)
                        previousTokenKnown = False

                    lexeme = ''

        if (tokenIsKnown(lexeme)):
            lexemes.append(tokenIdentifiers[lexeme.strip().lower()])
            previousTokenKnown = True
        else:
            if (previousTokenKnown):
                lexemes.append(lexeme.strip())
            else:
                lexemes.append(lexeme)
            previousTokenKnown = False

    return lexemes


def tokenAnalysis(tokens):
    updatedTokens = []

    for token in tokens:
        if token.lower() in tokenIdentifiers:
            updatedTokens.append(tokenIdentifiers.get(token.lower()))
        else:
            updatedTokens.append(token)

    return updatedTokens


def tokenIsKnown(token):
    return token.strip().lower() in tokenIdentifiers


def logError(code, message):
    print(f'Error ({str(code)}) : {message}')


def replaceTokensWithCode(tokens, variables, startIndex, translatedCode, result, isLoop=False):
    i = startIndex
    if (tokens[i] in tokenIdentifiers.values()):
        if (isLoop):
            translatedCode += '\t'

    if (tokens[i] == 'START_PROGRAM'):
        translatedCode += '# Start'
        translatedCode += '\n'
    elif (tokens[i] == 'END_PROGRAM'):
        translatedCode += '# End'
        translatedCode += '\n'
    elif (tokens[i] == 'WRITE'):
        translatedCode += 'print("'
        i += 1
        while (tokens[i] != '|'):
            # Write value to the console
            if (tokens[i] == 'RESULT'):
                translatedCode += str(result)
            else:
                translatedCode += tokens[i]
            i += 1
        translatedCode += '")'
        translatedCode += '\n'
    elif (tokens[i] == 'CREATE' and tokens[i + 1] == 'VARIABLE'):
        i += 2
        variableName = tokens[i]
        variables[variableName] = None
        translatedCode += variableName + ' = None'
        translatedCode += '\n'
    elif (tokens[i] == 'SET'):
        i += 1
        variableName = tokens[i]
        i += 2
        if (tokens[i] == 'INPUT'):
            variableValue = 'input()'
            variables[variableName] = variableValue
            translatedCode += variableName + ' = ' + variableValue
        else:
            variableValue = tokens[i]
            variables[variableName] = variableValue
            translatedCode += variableName + ' = "' + variableValue + '"'
        translatedCode += '\n'
    elif (tokens[i] == 'CALCULATE'):
        i += 1
        calculation = ''
        while (tokens[i] != '|'):
            calculation += tokens[i]
            i += 1
        result = eval(calculation)
    elif (tokens[i] == 'REPEAT' and tokens[i + 1] == 'FOREVER'):
        i += 2
        stopIndex = tokens.index('STOP')
        loopTokens = tokens[i: stopIndex]
        translatedCode += 'while True:\n'
        for j in range(len(loopTokens)):
            result = replaceTokensWithCode(loopTokens, variables, j, translatedCode, result, isLoop=False)
            translatedCode = result[0]
            i = result[1]

        i = stopIndex

    return (translatedCode, i, result)


def generateCode(tokens, variables):
    result = ''

    print(tokens)

    if (tokens[0] != 'START_PROGRAM' or tokens[-1] != 'END_PROGRAM'):
        logError(1, 'Start and/or end token(s) missing from source file.')
        return

    translatedCode = ''

    for i in range(len(tokens)):
        print(i)
        res = replaceTokensWithCode(tokens, variables, i, translatedCode, result)
        translatedCode = res[0]
        i = res[1]
        result = res[2]

    # translatedCode += '\n\nCompiled with love by TomScript compiler.'
    return translatedCode


loadConfig()

tokens = lexicalAnalysis()  # Analyse all keywords in the code

variables = {}

code = generateCode(tokens, variables)

with open(outputPyFile, 'w') as file:
    file.write(code)
