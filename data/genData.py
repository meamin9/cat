# -*- coding: utf-8 -*-

import os,sys
import xml.etree.ElementTree as ET

def genNotice():
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
public enum NoticeKey {
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

def genConstants():
    tree = ET.parse('constants.xml')
    root = tree.getroot()
    cf = open('../client/Assets/Scripts/data/Constants.cs', 'w')
    gf = open('../server/src/data/constants.go', 'w')
    cf.write(
'''// 此文件由脚本自动生成
namespace data {
    public static class Constants {
'''
    )
    gf.write(
'''// 此文件由脚本自动生成
package data

const (
'''
    )
    for e in root:
        t = e.attrib.get('type')
        desc = e.attrib.get('desc', '').encode('utf-8')
        key = e.tag
        if t == 'string':
            value = '"%s"' % e.text
        else:
            value = e.text
        s = '%spublic static %s %s = %s;  // %s\n' % (' '*8, t, key, value, desc)
        cf.write(s)
        s = '%sConst%s = %s  // %s\n' % (' '*4, key, value, desc)
        gf.write(s)
    cf.write('    }\n}\n')
    gf.write(')\n')
    cf.close()
    gf.close()

def main():
    genNotice()
    genConstants()

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        raw_input('runerror')
