# -*- coding: utf-8 -*-

import os,sys
import xml.etree.ElementTree as ET

def genNotice():
    print "genNotice"
    tree = ET.parse('notice_zh_cn.xml')
    root = tree.getroot()
    gofmt = \
'''// 此文件是代码自动生成，不要手动修改
package keys

const (
%s
)
'''
    csfmt = \
'''// 此文件是代码自动生成，不要手动修改
public enum Notice {
%s
}
'''
    gostrs = []
    csstrs = []
    for i, c in enumerate(root):
        name = c.find('name').text.encode('utf-8')
        content = c.find('content').text.encode('utf-8')
        if i == 0 :
            gostrs.append('    %s int32 = iota  // %s' % (name, content))
        else:
            gostrs.append('    %s  // %s' % (name, content))
        csstrs.append('    %s,  // %s' % (name, content))
    with open('../server/src/keys/notice.go', 'w') as f:
        f.write(gofmt % '\n'.join(gostrs))
    with open('../client/Assets/Scripts/Game/Notice/Keys.cs', 'w') as f:
        f.write(csfmt % '\n'.join(csstrs))

def main():
    genNotice()

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        raw_input('runerror')
