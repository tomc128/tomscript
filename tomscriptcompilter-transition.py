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
    'user input': 'USER_INPUT',
    'calculate': 'CALCULATE',
    'repeat': 'REPEAT',
    'forever': 'FOREVER'
}

def loadConfig():
    global sourceFile, outputPyFile, outputExeFile

    with open(CONFIG_PATH, 'r') as config:
        for line in config.readlines():
            line = line.strip()
            if line.startswith('source-file'):
                sourceFile = line[12:]
            elif line.startswith('output-py-file'):
                outputPyFile = line[16:]
            elif line.startswith('output-exe-file'):
                outputPyFile = line[17:]

def loadSourceFile():
    with open(sourceFile, 'r') as source:
        return source.readlines()

def lexicalAnalysis():
    sourceLines = loadSourceFile() # Contains each line of the TomScript file
    sourceLines = [item.strip() for item in sourceLines] # Strip all of the elements of the list

    lexemes = [] # Creates a list of the lexemes in the TomScript code

    for line in sourceLines: # Loop through each line,
        if (line == ''): # checking if it is empty,
            lexemes.append('|') # if it is add a pipe to the lexemes list
            continue # and continue the next iteration

        lexeme = '' # Create the current lexeme,
        previousTokenKnown = False
        for i, char in enumerate(line): # and loop through each character in the line
            lexeme += char # Add the current character to the current lexeme
            if (i+1 < len(line)): # If the next character exists,
                if (line[i+1] == ' '): # and it is whitespace,

                    if (tokenIsKnown(lexeme)):
                        lexemes.append(tokenIdentifiers[lexeme.strip().lower()])
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

def generateCode(tokens):
    print(tokens)

    if (tokens[0] != 'START_PROGRAM' or tokens[-1] != 'END_PROGRAM'):
        logError(1, 'Start and/or end token(s) missing from source file.')
        return

    translatedCode = ''

    for i in range(len(tokens)):
        if (tokens[i] == 'WRITE'):
            translatedCode += 'print("'
            i += 1
            while (tokens[i] != '|'):
                # Write value to the console
                translatedCode += tokens[i]
                i += 1
            translatedCode+= '")'

    return translatedCode

        


loadConfig()

tokens = lexicalAnalysis() # Analyse all keywords in the code

code = generateCode(tokens)

print(code)