#! python
# -*- coding: utf-8 -*-

import os
import codecs, csv
# 提取csv中的Key生成常量到go代码中

Tem_Go = '''// Generated by cat/data/gen_notice.py
// DO NOT EDIT!
// source file: notice.csv

package notice

const (
    CNoticeHeader int = iota
    {notice}
)

'''
def format_go(records):
    strs = ['C{} // {}'.format(r[0], r[1]) for i, r in enumerate(records) if i >= 2]
    s = '\n    '.join(strs)
    gofile = '../server/src/app/notice/not_id.go'
    with codecs.open(gofile, 'w') as f:
        f.write(Tem_Go.format(**{'notice': s}))
        print('gen ' + gofile)

def main():
    source = 'notice.csv'
    with codecs.open(source, 'r') as f:
        records = csv.reader(f)
        format_go(records)

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        input('runtime error')
        
