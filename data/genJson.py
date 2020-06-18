# -*- coding: utf-8 -*-

import os, sys, xlrd, json, codecs

dataPath = sys.path[0]
clientPath = os.path.join(dataPath, '../client/Assets/Resources/Tables/')
serverPath = os.path.join(dataPath, '../client/Assets/Resources/Tables/')

def parseSheet(sheet):
    if sheet.nrows < 5:
        return
    name = sheet.name + '.json'
    names = sheet.row_values(1)
    cexports = sheet.row_values(2)
    sexports = sheet.row_values(3)
    types = sheet.row_values(4)
    
    isarray = types[0] == "[]"
    cdatas = [] if isarray else {}
    sdatas = [] if isarray else {}
    #print(cexports, sexports, types)
    for i in range(5, sheet.nrows, 1):
        row = sheet.row(i)
        cdata = {}
        sdata = {}
        for j in range(1, sheet.ncols, 1):
            value = sheet.cell_value(i, j)
            
            typ = types[j]
            if len(str(value)) == 0 or len(typ) == 0:
                continue
            if typ == 'str' or typ == 'string':
            	value = str(value)
            elif typ == 'float':
                value = float(value)
            elif typ == 'int':
                value = int(value)
            elif typ == 'bool':
                value = value == 1
            elif typ == 'json':
                value = json.loads(value)
            else:
                print('未知类型：', typ, name)
                continue
            if cexports[j] == 1:
                cdata[names[j]] = value
            if sexports[j] == 1:
                sdata[names[j]] = value 
        if len(cdata) != 0:
            if isarray:
                cdatas.append(cdata)
            else:
                uniqueId = cdata[names[1]]
                if cdatas.get(uniqueId) != None:
                    print('client 重复的id：', uniqueId, name)
                cdatas[uniqueId] = cdata
        if len(sdata) != 0:
            if isarray:
                sdatas.append(sdata)
            else:
                uniqueId = sdata[names[1]]
                if sdatas.get(uniqueId) != None:
                    print('sever 重复的id：', uniqueId, name)
                sdatas[uniqueId] = sdata
    writeJson(clientPath + name, cdatas)
    writeJson(serverPath + name, sdatas)


def writeJson(path, container):
    if len(container) == 0:
        return
    print('write json to', path)
    # print(json.dumps(container, ensure_ascii=False))
    with codecs.open(path, 'w', encoding='utf-8') as f:
        json.dump(container, f, ensure_ascii=False, indent=2)

def genJson(path):
    for root, _, files in os.walk(path):
        for name in files:
            if name.startswith('~$') or not name.endswith('.xlsx'):
                continue
            print('...' + name)
            filePath = os.path.join(root, name)
            with xlrd.open_workbook(filePath) as book:
                for sheet in book.sheets():
                    parseSheet(sheet)

def main():
    genJson(os.path.join(dataPath, 'data'))

if __name__ == '__main__':
    try:
        main()
        input('press anykey to quit\n')
    except:
        import traceback
        traceback.print_exc()
        input('runtime error')
