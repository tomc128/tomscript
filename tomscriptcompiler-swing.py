import os
import datetime

CONFIG_PATH = './tms.config'
INDENTATION = '    '

result = ''

sourceFile = ''
outputPyFile = ''
outputExeFile = ''

tokenIdentifiers = {
    'start': 'START_PROGRAM',
    'end': 'END_PROGRAM',
    'create': 'CREATE',
    'variable': 'VARIABLE',
    'write': 'WRITE',
    'read': 'READ',
    'set': 'SET',
    'to': 'TO',
    'calculate': 'CALCULATE',
    'repeat': 'REPEAT',
    'forever': 'FOREVER',
    'and': '|',
    'result': 'RESULT',
    'if': 'IF',
    'stop': 'STOP',
    'times': 'TIMES',
    'done': 'DONE'
}

tokens = []
variables = {}
sourceLines = []


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


def intialTokenReplacement():
    global sourceLines

    sourceLines = [line.lower() for line in sourceLines]

    for i in range(len(sourceLines)):
        if 'is greater than' in sourceLines[i]:
            sourceLines[i] = sourceLines[i].replace('is greater than', 'IS_GREATER_THAN')
        if 'is less than' in sourceLines[i]:
            sourceLines[i] = sourceLines[i].replace('is less than', 'IS_LESS_THAN')
        if 'is equal to' in sourceLines[i]:
            sourceLines[i] = sourceLines[i].replace('is equal to', 'IS_EQUAL_TO')


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
                translatedCode += ' " + ' + tokens[0].strip() + ' + "'
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

            while (tokens[0] != 'STOP'):
                translatedCode += parseCode(prefix=INDENTATION)

            tokens = tokens[1:]
        elif (tokens[2] == 'TIMES'):
            times = tokens[1]
            tokens = tokens[3:]
            translatedCode += 'for i in range(' + times + '):\n'

            while (tokens[0] != 'STOP'):
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

    startTime = datetime.datetime.now()
    i = 0
    while len(tokens) > 0:
        translatedCode += parseCode()
        print('Compiling .' + ' .'*i, end='\r')
        i += 1
    print('')
    endTime = datetime.datetime.now()
    elapsedTime = endTime - startTime

    print('\nCompiled in ' + str(elapsedTime.total_seconds() * 1000) + 'ms.')

    return translatedCode


loadConfig()
sourceLines = loadSourceFile()
intialTokenReplacement()
tokens = lexicalAnalysis()


code = generateCode()


# if (code == ''):
#     quit()

# with open('output.py', 'w') as file:
#     file.write(code)
