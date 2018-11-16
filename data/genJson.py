# -*- coding: utf-8 -*-

import os, sys, xlrd, json, codecs

dataPath = sys.path[0]
clientPath = os.path.join(dataPath, '../client/Assets/Resources/Tables/')
serverPath = os.path.join(dataPath, '../client/Assets/Resources/Tables/')

def parseSheet(sheet):
    if sheet.nrows < 5:
        return
    name = sheet.name + '.json'
    cdatas = []
    cexports = sheet.row_values(1)
    sdatas = []
    sexports = sheet.row_values(2)
    types = sheet.row_values(3)
    uniqueId = 'id' in types
    if uniqueId:
        cdatas = {}
        sdatas = {}
    # print(cexports, sexports, types)
    for i in range(4, sheet.nrows, 1):
        row = sheet.row(i)
        cdata = {}
        sdata = {}
        uniqueId = None
        for j in range(sheet.ncols):
            value = str(sheet.cell_value(i, j))
            typ = types[j]
            if typ == 'id':
                value = int(float(value))
                uniqueId = value
            elif typ == 'int':
                value = int(float(value))
            elif typ == 'bool':
                value = value == '1' or value.upper() == 'TRUE'
            elif typ == 'json':
                value = json.load(value)
            if cexports[j] != '':
                cdata[cexports[j]] = value
            if sexports[j] != '':
                sdata[sexports[j]] = value 
        if len(cdata) != 0:
            if uniqueId is None:
                cdatas.append(cdata)
            else:
                if cdatas.get(uniqueId) != None:
                    print('client 重复的id：', uniqueId, name)
                cdatas[uniqueId] = cdata
        if len(sdata) != 0:
            if uniqueId is None:
                sdatas.append(sdata)
            else:
                if sdatas.get(uniqueId) != None:
                    print('sever 重复的id：', uniqueId, name)
                sdatas[uniqueId] = sdata
    writeJson(clientPath + name, cdatas)
    writeJson(serverPath + name, sdatas)

def writeJson(path, container):
    if len(container) == 0:
        return
    # print(json.dumps(container, ensure_ascii=False))
    with codecs.open(path, 'w', encoding='utf-8') as f:
        json.dump(container, f, ensure_ascii=False)

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
