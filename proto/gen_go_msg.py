#! python3
# -*- coding: utf-8 -*-
import os, sys, re, codecs

Tem = u'''// Generated by cat/proto/gen_go_msg.py
// DO NOT EDIT!
// Source: {name}

package proto

import (
    "cellnet"
    "reflect"
    _ "cellnet/codec/gogopb"
    "cellnet/codec"
)

const (
{const}
)

func init() {{
{reg}
}}
'''
Tem_const = u'''    Code{name} int = {msgid}'''
Tem_reg = u'''
    {com}cellnet.RegisterMessageMeta(&cellnet.MessageMeta{{
        Codec: codec.MustGetCodec("gogopb"),
        Type:  reflect.TypeOf((*{name})(nil)).Elem(),
        ID: {msgid},
    }})'''

remsg = re.compile(r"((?P<com>(//.*\s)+)|.*\s)message\s+(?P<msg>\w+)\s*\{")

def id(name):
    n = 0
    for c in 'proto.' + name:
        i = ord(c)
        n += (n << 5) + (i << 7) + i
    n &= 0xFFFF
    return n

def gen_msg_file(name, outdir):
    msgs = []
    coms = {}
    print('deal ' + name)
    with codecs.open(name, 'r', 'utf-8') as f:
        c = f.read()
        m = remsg.search(c)
        n = len(c)
        while m != None:
            i = m.end(0)
            b = 1
            while i < n:
                if c[i] == '{':
                    b += 1
                elif c[i] == '}':
                    b -= 1
                i += 1
                if b == 0:
                    break
            if b != 0:
                print('syntax error')
                break
            msg = m.group('msg')
            com = m.group('com')
            msgs.append(msg)
            if com :
                coms[msg] = '\n    '.join(com.split('\n'))
            else:
                coms[msg] = ''
            m = remsg.search(c, i)
    base, _ = os.path.splitext(name)
    w = '{}/{}.msg.go'.format(outdir, base)
    print('gen ' + w)
    with codecs.open(w, 'w', 'utf-8') as f:
        reg = []
        const = []
        for m in msgs:
            d = {'name':m, 'msgid':id(m), 'com':coms[m]}
            reg.append(Tem_reg.format(**d))
            const.append(Tem_const.format(**d))
        c = Tem.format(**{'name':name, 'reg':'\n'.join(reg), 'const':'\n'.join(const)})
        f.write(c)

def gen_msg():
    outdir = '../server/src/proto'
    if not os.path.exists(outdir):
        os.makedirs(outdir)
    for root, _, files in os.walk('./'):
        for f in files:
            if not f.endswith('.proto'):
                continue
            if f in ('common.proto',):
                continue
            gen_msg_file(f, outdir)

def main():
    os.chdir(sys.path[0])
    gen_msg()
    #gen_msg_file('account.proto', '.')

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        input('runtime error')