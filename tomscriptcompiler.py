import os

sourceFile = ''
outputFile = ''
optionCompileComments = False
optionCompileExecutable = False
optionCompileDebug = False
startOffset = 0

def loadSettings():
    global sourceFile, outputFile, optionCompileComments, optionCompileExecutable, optionCompileDebug, commentCount, startOffset
    with open('tms.config', 'r') as file:
        for line in file.readlines():
            if line.startswith('source'):
                sourceFile = line[6:].strip()
            elif line.startswith('output'):
                outputFile = line[6:].strip()
            elif line.strip() == 'option compile comments':
                optionCompileComments = True
            elif line.strip() == 'option compile executable':
                optionCompileExecutable = True
            elif line.strip() == 'option compile debug':
                optionCompileDebug = True
            elif line.startswith('option commentLines'):
                commentCount = int(line[19:].strip())
                startOffset = commentCount - 1
                


def readFile(inputFile):
    with open(sourceFile, 'r') as file:
        lines = file.readlines()

    return lines


def isNumber(value):
    try:
        value = int(value)
        return True
    except:
        return False


def parseScript():
    lines = readFile(sourceFile)
    variables = {'result': None}

    with open(outputFile, 'w') as file:
        if lines[startOffset].strip() != 'start' or lines[-1].strip() != 'end':
            file.write("Error during compilation")
            return

        file.write("# Compiled with TSC")
            
        for line in lines:
            if line.startswith('create variable'):
                if 'and set variable to' in line:
                    # Create variable with value
                    variableName = line[16:]
                    variableName = variableName[:variableName.index(
                        ' ')].strip()
                    variableValue = line[37 + len(variableName):].strip()
                    variables[variableName] = variableValue
                else:
                    # Create variable without value
                    variableName = line[16:].strip()
                    variables[variableName] = None

            elif line.startswith('write'):
                # Write to console
                data = line[6:]
                words = data.split()
                i = 0
                for word in words:
                    if word in variables:
                        # This word is a variable, replace it with the value
                        if variables[word] == None:
                            words[i] = '[variable unset]'
                        else:
                            words[i] = f'{variables[word]}'
                    i += 1

                data = ' '.join(words)
                file.write(f'print(f"{data}")\n')

            elif line.startswith('set'):
                # Set variable
                data = line[4:]
                variableName = data[:data.index(' ')].strip()
                variableValue = data[data.index('to') + 2:].strip()

                if variableValue == 'user input':
                    variableValue = 'input()'
                    variables[variableName] = '{' + variableName + '}'
                elif variableValue == 'result':
                    variableValue = variables['result']
                    variables[variableName] = variableValue
                elif not isNumber(variableValue):
                    variableValue = f'"{variableValue}"'

                file.write(f'{variableName} = {variableValue}\n')

            elif line.startswith('calculate'):
                # Eval
                data = line[10:]
                variables['result'] = eval(data)
                
            elif line.startswith('//'):
                if optionCompileComments:
                    file.write(line.replace('//', '#'))
                else:
                    pass

        file.write('input()')




loadSettings()
parseScript()

if optionCompileExecutable:
    os.system(f'cd {os.getcwd()}')
    os.system('pip insall pyinstaller')
    os.system(f'pyinstaller -F {outputFile}')
    if not optionCompileDebug:
        os.system(f'move /Y dist\\{outputFile[:-3]}.exe {outputFile[:-3]}.exe')
        os.system(f'rmdir /Q /S dist && rmdir /Q /S build')
